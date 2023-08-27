/////////////////////////////////////////////////////////////////////////////////
// CodeLab for Paint.NET
// Copyright ©2006 Rick Brewster, Tom Jackson. All Rights Reserved.
// Portions Copyright ©2007-2017 BoltBait. All Rights Reserved.
// Portions Copyright ©2017-2018 Jason Wendt. All Rights Reserved.
// Portions Copyright ©2016 Jacob Slusser. All Rights Reserved.
// Portions Copyright ©2004 yetanotherchris.  All Rights Reserved.
// Portions Copyright ©Microsoft Corporation. All Rights Reserved.
//
// THE CODELAB DEVELOPERS MAKE NO WARRANTY OF ANY KIND REGARDING THE CODE. THEY
// SPECIFICALLY DISCLAIM ANY WARRANTY OF FITNESS FOR ANY PARTICULAR PURPOSE OR
// ANY OTHER WARRANTY.  THE CODELAB DEVELOPERS DISCLAIM ALL LIABILITY RELATING
// TO THE USE OF THIS CODE.  NO LICENSE, EXPRESS OR IMPLIED, BY ESTOPPEL OR
// OTHERWISE, TO ANY INTELLECTUAL PROPERTY RIGHTS IS GRANTED HEREIN.
//
// Latest distribution: https://www.BoltBait.com/pdn/codelab
/////////////////////////////////////////////////////////////////////////////////
// Intellisense code from http://www.codeproject.com/KB/cs/diy-intellisense.aspx
// which required tons of fixes in order to get it to work properly.
/////////////////////////////////////////////////////////////////////////////////
// Scintilla editor from https://github.com/jacobslusser/ScintillaNET
// Implemented in CodeLab and customized by Jason Wendt.
/////////////////////////////////////////////////////////////////////////////////

using PaintDotNet;
using PlatformSpellCheck;
using ScintillaNET;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace PdnCodeLab
{
    public sealed class CodeTextBox : Scintilla
    {
        private readonly Timer timer = new Timer();
        private readonly IntelliBox iBox = new IntelliBox();
        private readonly FindAndReplace findPanel = new FindAndReplace();
        private readonly IntelliTip intelliTip = new IntelliTip();
        private readonly IndicatorBar indicatorBar = new IndicatorBar();
        private readonly List<int> errorLines = new List<int>();
        private readonly List<int> warningLines = new List<int>();
        private readonly List<int> matchLines = new List<int>();
        private readonly ToolStrip lightBulbMenu = new ToolStrip();
        private readonly ScaledToolStripDropDownButton bulbIcon = new ScaledToolStripDropDownButton();
        private readonly Dictionary<Guid, ScintillaNET.Document> docCollection = new Dictionary<Guid, ScintillaNET.Document>();
        private readonly Dictionary<Guid, DocMeta> docMetaCollection = new Dictionary<Guid, DocMeta>();
        private const int Preprocessor = 64;
        private const BindingFlags userScriptBindingFlags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;
        private SpellChecker spellChecker;

        [Flags]
        private enum DelayedOperation
        {
            None = 0,
            UpdateIndicatorBar = 1,
            ScrollCaret = 2,
            Spellcheck = 4
        }

        #region Variables for different states
        private Theme theme;
        private Guid docGuid = Guid.Empty;
        private int posAtIBox = InvalidPosition;
        private int previousLine = 0;
        private int dwellWordPos = InvalidPosition;
        private int lastCaretPos = 0;
        private bool mapScroll = false;
        private bool Replacing = false;
        private int maxLineNumberCharLength = 0;
        private int disableIntelliTipPos = InvalidPosition;
        private DelayedOperation delayedOperation = DelayedOperation.None;
        private bool useExtendedColors = false;
        private bool spellCheckEnabled = false;
        private bool updatingStyles = false;
        private int autoBraceOpenPos = InvalidPosition;
        private bool suppressContextMenu = true;
        #endregion

        #region Properties
        private DocMeta DocumentMeta
        {
            get
            {
                List<int> foldedLines = new List<int>();
                for (int lineIndex = 0; lineIndex < this.Lines.Count; lineIndex++)
                {
                    lineIndex = this.Lines[lineIndex].ContractedFoldNext;
                    if (lineIndex == InvalidPosition)
                    {
                        break;
                    }

                    foldedLines.Add(lineIndex);
                }

                return new DocMeta(this.DocLineFromVisible(this.FirstVisibleLine), this.AnchorPosition, this.CurrentPosition, foldedLines);
            }
            set
            {
                this.AnchorPosition = value.AnchorPos;
                this.CurrentPosition = value.CaretPos;
                this.FirstVisibleLine = value.ScrollPos;
                foreach (int line in value.FoldedLines)
                {
                    this.Lines[line].FoldLine(FoldAction.Contract);
                }
            }
        }

        internal IReadOnlyCollection<int> Bookmarks
        {
            get
            {
                List<int> bkmkList = new List<int>();
                for (int i = 0; i < this.Lines.Count; i++)
                {
                    if ((this.Lines[i].MarkerGet() & BookmarkMargin.Mask) > 0)
                    {
                        bkmkList.Add(i);
                    }
                }

                return bkmkList.ToArray();
            }
            set
            {
                foreach (int line in value)
                {
                    this.Lines[line].MarkerAdd(BookmarkMargin.Marker);
                }
            }
        }

        internal bool LineNumbersEnabled
        {
            get
            {
                return (this.Margins[LeftMargin.LineNumbers].Width > 0);
            }
            set
            {
                if (value)
                {
                    this.Margins[LeftMargin.LineNumbers].Width = this.TextWidth(Style.LineNumber, new string('9', this.Lines.Count.ToString().Length + 1)) + 2;
                    this.Margins[LeftMargin.Padding].Width = (this.CodeFoldingEnabled) ? 0 : 6;
                }
                else
                {
                    this.Margins[LeftMargin.LineNumbers].Width = 0;
                    this.Margins[LeftMargin.Padding].Width = (this.CodeFoldingEnabled) ? 0 : 2;
                }
            }
        }

        internal bool BookmarksEnabled
        {
            get
            {
                return (this.Margins[LeftMargin.Bookmarks].Width > 0);
            }
            set
            {
                if (value)
                {
                    this.Margins[LeftMargin.Bookmarks].Width = this.Lines[0].Height;
                    this.Markers[BookmarkMargin.Marker].Symbol = MarkerSymbol.Bookmark;
                }
                else
                {
                    this.Markers[BookmarkMargin.Marker].Symbol = MarkerSymbol.Empty;
                    this.Margins[LeftMargin.Bookmarks].Width = 0;
                }
            }
        }

        internal bool CodeFoldingEnabled
        {
            get
            {
                return (this.Margins[LeftMargin.CodeFolding].Width > 0);
            }
            set
            {
                if (value)
                {
                    this.Margins[LeftMargin.CodeFolding].Width = this.Lines[0].Height;
                    this.Margins[LeftMargin.Padding].Width = 0;
                }
                else
                {
                    this.Margins[LeftMargin.CodeFolding].Width = 0;
                    this.FoldAll(FoldAction.Expand);
                    this.Margins[LeftMargin.Padding].Width = (this.LineNumbersEnabled) ? 6 : 2;
                }
            }
        }

        internal bool MapEnabled
        {
            get
            {
                return (this.Margins.Right > 1);
            }
            set
            {
                this.VScrollBar = !value;
                indicatorBar.Visible = value;
                this.Margins.Right = (value) ? indicatorBar.Width + 1 : 1;

                UpdateIndicatorBar();

                if (findPanel.Visible)
                {
                    findPanel.Location = new Point(this.ClientRectangle.Right - this.Margins.Right - findPanel.Width, 0);
                }
            }
        }

        internal bool IsVirgin
        {
            get => !(this.CanUndo || this.CanRedo) && this.Text.Equals(DefaultCode.Default);
        }

        internal bool CaretLineFrameEnabled
        {
            get
            {
                return this.CaretLineFrame > 0;
            }
            set
            {
                this.CaretLineFrame = value ? UIUtil.Scale(2) : 0;
                switch (this.theme)
                {
                    case Theme.Dark:
                        SetCaretLineDarkColor();
                        break;
                    case Theme.Light:
                    default:
                        SetCaretLineLightColor();
                        break;
                }
            }
        }

        internal bool DisableAutoComplete
        {
            get;
            set;
        }

        internal bool UseExtendedColors
        {
            get
            {
                return this.useExtendedColors;
            }
            set
            {
                this.useExtendedColors = value;

                if (this.Lexer == Lexer.Cpp)
                {
                    UpdateSubstyleAllocations();
                    UpdateSyntaxHighlighting();
                }
            }
        }

        internal bool SpellcheckEnabled
        {
            get
            {
                return this.spellCheckEnabled;
            }
            set
            {
                bool enable = value && SpellChecker.IsPlatformSupported() && SpellChecker.IsLanguageSupported(Settings.SpellingLang);
                this.spellCheckEnabled = enable;

                if (enable)
                {
                    spellChecker?.Dispose();
                    spellChecker = new SpellChecker(Settings.SpellingLang);
                    foreach (string word in Settings.SpellingWordsToIgnore)
                    {
                        spellChecker.Ignore(word);
                    }

                    SpellCheck();
                }
                else
                {
                    this.IndicatorCurrent = Indicator.Spelling;
                    this.IndicatorClearRange(0, this.TextLength);
                }
            }
        }

        internal bool SuppressContextMenu
        {
            get => this.suppressContextMenu;
        }

        [Category(nameof(CategoryAttribute.Appearance))]
        [RefreshProperties(RefreshProperties.All)]
        [DefaultValue(Theme.Light)]
        public Theme Theme
        {
            get
            {
                return theme;
            }
            set
            {
                theme = value;
                switch (value)
                {
                    case Theme.Dark:
                        switch (this.Lexer)
                        {
                            case Lexer.Xml:
                                SetXMLDarkStyles();
                                break;
                            case Lexer.Cpp:
                            default:
                                SetCSharpDarkStyles();
                                break;
                        }

                        // Line Numbers
                        this.Styles[Style.LineNumber].ForeColor = Color.FromArgb(43, 145, 175);
                        this.Styles[Style.LineNumber].BackColor = Color.FromArgb(30, 30, 30);

                        // Code Folding
                        this.SetFoldMarginColor(true, Color.FromArgb(30, 30, 30));
                        this.SetFoldMarginHighlightColor(true, Color.FromArgb(30, 30, 30));

                        // Code Fold Ellipsis
                        this.Styles[Style.FoldDisplayText].ForeColor = Color.Gray;
                        this.Styles[Style.FoldDisplayText].BackColor = Color.FromArgb(30, 30, 30);

                        // Code Folding markers
                        for (int i = 25; i <= 31; i++)
                        {
                            this.Markers[i].SetForeColor(Color.FromArgb(30, 30, 30));
                            this.Markers[i].SetBackColor(Color.FromArgb(165, 165, 165));
                        }

                        // Braces & Brackets
                        this.Styles[Style.BraceLight].BackColor = Color.FromArgb(17, 61, 111);
                        this.Styles[Style.BraceLight].ForeColor = Color.Gainsboro;
                        this.Styles[Style.BraceBad].BackColor = Color.FromArgb(17, 61, 111);
                        this.Styles[Style.BraceBad].ForeColor = Color.Red;

                        // White Space
                        this.SetWhitespaceForeColor(true, Color.FromArgb(20, 72, 82));

                        // Object Highlight
                        this.Indicators[Indicator.ObjectHighlight].ForeColor = Color.FromArgb(17, 61, 111);
                        this.Indicators[Indicator.ObjectHighlightDef].ForeColor = Color.FromArgb(64, 111, 17);

                        // Find
                        this.Indicators[Indicator.Find].ForeColor = Color.FromArgb(101, 51, 6);

                        // Error
                        this.Indicators[Indicator.Error].ForeColor = Color.FromArgb(252, 62, 54);
                        this.Indicators[Indicator.Warning].ForeColor = Color.FromArgb(149, 219, 125);

                        // Spelling
                        this.Indicators[Indicator.Spelling].ForeColor = Color.Magenta;

                        // Selection
                        this.SetSelectionBackColor(true, Color.FromArgb(38, 79, 120));

                        // Current Line Highlight
                        SetCaretLineDarkColor();

                        // Caret
                        this.CaretForeColor = Color.Gainsboro;


                        // IntelliTip back color
                        intelliTip.UpdateTheme(Color.White, Color.FromArgb(66, 66, 66));
                        iBox.UpdateTheme(Color.White, Color.FromArgb(66, 66, 66));
                        break;

                    case Theme.Light:
                    default:
                        switch (this.Lexer)
                        {
                            case Lexer.Xml:
                                SetXMLLightStyles();
                                break;
                            case Lexer.Cpp:
                            default:
                                SetCSharpLightStyles();
                                break;
                        }

                        // Line Numbers
                        this.Styles[Style.LineNumber].ForeColor = Color.DimGray;
                        this.Styles[Style.LineNumber].BackColor = Color.White;

                        // Code Folding
                        this.SetFoldMarginColor(true, Color.White);
                        this.SetFoldMarginHighlightColor(true, Color.White);

                        // Code Fold Ellipsis
                        this.Styles[Style.FoldDisplayText].ForeColor = Color.Gray;
                        this.Styles[Style.FoldDisplayText].BackColor = Color.White;

                        // Code Folding markers
                        for (int i = 25; i <= 31; i++)
                        {
                            this.Markers[i].SetForeColor(Color.White);
                            this.Markers[i].SetBackColor(Color.LightGray);
                        }

                        // Braces & Brackets
                        this.Styles[Style.BraceLight].BackColor = Color.Gainsboro;
                        this.Styles[Style.BraceLight].ForeColor = Color.Black;
                        this.Styles[Style.BraceBad].BackColor = Color.Gainsboro;
                        this.Styles[Style.BraceBad].ForeColor = Color.Red;

                        // White Space
                        this.SetWhitespaceForeColor(true, Color.FromArgb(43, 145, 175));

                        // Object Highlight
                        this.Indicators[Indicator.ObjectHighlight].ForeColor = Color.Gainsboro;
                        this.Indicators[Indicator.ObjectHighlightDef].ForeColor = Color.Gainsboro;

                        // Find
                        this.Indicators[Indicator.Find].ForeColor = Color.FromArgb(246, 185, 77);

                        // Error
                        this.Indicators[Indicator.Error].ForeColor = Color.Red;
                        this.Indicators[Indicator.Warning].ForeColor = Color.Green;

                        // Spelling
                        this.Indicators[Indicator.Spelling].ForeColor = Color.Magenta;

                        // Selection
                        this.SetSelectionBackColor(true, Color.FromArgb(173, 214, 255));

                        // Current Line Highlight
                        SetCaretLineLightColor();

                        // Caret
                        this.CaretForeColor = Color.Black;


                        // IntelliTip colors
                        intelliTip.UpdateTheme(Color.FromArgb(30, 30, 30), Color.WhiteSmoke);
                        iBox.UpdateTheme(Color.FromArgb(30, 30, 30), Color.WhiteSmoke);
                        break;
                }

                findPanel.UpdateTheme();
                lightBulbMenu.Renderer = PdnTheme.Renderer;
                indicatorBar.Theme = value;
            }
        }

        private void SetCSharpDarkStyles()
        {
            Color backColor = Color.FromArgb(30, 30, 30);
            this.Styles[Style.Default].BackColor = backColor;

            // Configure the CPP (C#) lexer styles
            for (int i = 0; i <= Preprocessor; i += Preprocessor)
            {
                this.Styles[Style.Cpp.Default + i].ForeColor = Color.White;
                this.Styles[Style.Cpp.Default + i].BackColor = backColor;
                this.Styles[Style.Cpp.Identifier + i].ForeColor = Color.Gainsboro;
                this.Styles[Style.Cpp.Identifier + i].BackColor = backColor;
                this.Styles[Style.Cpp.Comment + i].ForeColor = Color.FromArgb(87, 166, 74); // Green
                this.Styles[Style.Cpp.Comment + i].BackColor = backColor;
                this.Styles[Style.Cpp.CommentLine + i].ForeColor = Color.FromArgb(87, 166, 74); // Green
                this.Styles[Style.Cpp.CommentLine + i].BackColor = backColor;
                this.Styles[Style.Cpp.CommentLineDoc + i].ForeColor = Color.Gray;
                this.Styles[Style.Cpp.CommentLineDoc + i].BackColor = backColor;
                this.Styles[Style.Cpp.Number + i].ForeColor = Color.FromArgb(181, 206, 168);
                this.Styles[Style.Cpp.Number + i].BackColor = backColor;
                this.Styles[Style.Cpp.Word2 + i].ForeColor = Color.FromArgb(78, 201, 176);
                this.Styles[Style.Cpp.Word2 + i].BackColor = backColor;
                this.Styles[Style.Cpp.Word + i].ForeColor = Color.FromArgb(86, 156, 214);
                this.Styles[Style.Cpp.Word + i].BackColor = backColor;
                this.Styles[Style.Cpp.String + i].ForeColor = Color.FromArgb(214, 157, 133); // Red
                this.Styles[Style.Cpp.String + i].BackColor = backColor;
                this.Styles[Style.Cpp.Character + i].ForeColor = Color.FromArgb(214, 157, 133); // Red
                this.Styles[Style.Cpp.Character + i].BackColor = backColor;
                this.Styles[Style.Cpp.Verbatim + i].ForeColor = Color.FromArgb(214, 157, 133); // Red
                this.Styles[Style.Cpp.Verbatim + i].BackColor = backColor;
                this.Styles[Style.Cpp.StringEol + i].ForeColor = Color.FromArgb(214, 157, 133); // Red
                this.Styles[Style.Cpp.StringEol + i].BackColor = backColor;
                this.Styles[Style.Cpp.Operator + i].ForeColor = Color.FromArgb(180, 180, 180);
                this.Styles[Style.Cpp.Operator + i].BackColor = backColor;
                this.Styles[Style.Cpp.Preprocessor + i].ForeColor = Color.FromArgb(155, 155, 155);
                this.Styles[Style.Cpp.Preprocessor + i].BackColor = backColor;
                this.Styles[Style.Cpp.Regex + i].ForeColor = Color.Gainsboro;
                this.Styles[Style.Cpp.Regex + i].BackColor = backColor;
                this.Styles[Style.Cpp.EscapeSequence + i].ForeColor = Color.FromArgb(255, 214, 143);
                this.Styles[Style.Cpp.EscapeSequence + i].BackColor = backColor;

                this.Styles[Substyle.Keyword + i].ForeColor = Color.FromArgb(216, 160, 223);
                this.Styles[Substyle.Keyword + i].BackColor = Color.FromArgb(30, 30, 30);
                this.Styles[Substyle.Method + i].ForeColor = Color.FromArgb(220, 220, 170);
                this.Styles[Substyle.Method + i].BackColor = Color.FromArgb(30, 30, 30);
                this.Styles[Substyle.ParamAndVar + i].ForeColor = Color.FromArgb(156, 220, 254);
                this.Styles[Substyle.ParamAndVar + i].BackColor = Color.FromArgb(30, 30, 30);
                this.Styles[Substyle.Struct + i].ForeColor = Color.FromArgb(134, 198, 145);
                this.Styles[Substyle.Struct + i].BackColor = Color.FromArgb(30, 30, 30);
                this.Styles[Substyle.Enum + i].ForeColor = Color.FromArgb(184, 215, 163);
                this.Styles[Substyle.Enum + i].BackColor = Color.FromArgb(30, 30, 30);
                this.Styles[Substyle.Interface + i].ForeColor = Color.FromArgb(184, 215, 163);
                this.Styles[Substyle.Interface + i].BackColor = Color.FromArgb(30, 30, 30);
            }
        }

        private void SetCSharpLightStyles()
        {
            Color backColor = Color.White;
            this.Styles[Style.Default].BackColor = backColor;

            // Configure the CPP (C#) lexer styles
            for (int i = 0; i <= Preprocessor; i += Preprocessor)
            {
                this.Styles[Style.Cpp.Default + i].ForeColor = Color.Black;
                this.Styles[Style.Cpp.Default + i].BackColor = backColor;
                this.Styles[Style.Cpp.Identifier + i].ForeColor = Color.Black;
                this.Styles[Style.Cpp.Identifier + i].BackColor = backColor;
                this.Styles[Style.Cpp.Comment + i].ForeColor = Color.Green;
                this.Styles[Style.Cpp.Comment + i].BackColor = backColor;
                this.Styles[Style.Cpp.CommentLine + i].ForeColor = Color.Green;
                this.Styles[Style.Cpp.CommentLine + i].BackColor = backColor;
                this.Styles[Style.Cpp.CommentLineDoc + i].ForeColor = Color.Gray;
                this.Styles[Style.Cpp.CommentLineDoc + i].BackColor = backColor;
                this.Styles[Style.Cpp.Number + i].ForeColor = Color.Black;
                this.Styles[Style.Cpp.Number + i].BackColor = backColor;
                this.Styles[Style.Cpp.Word2 + i].ForeColor = Color.FromArgb(43, 145, 175);
                this.Styles[Style.Cpp.Word2 + i].BackColor = backColor;
                this.Styles[Style.Cpp.Word + i].ForeColor = Color.Blue;
                this.Styles[Style.Cpp.Word + i].BackColor = backColor;
                this.Styles[Style.Cpp.String + i].ForeColor = Color.FromArgb(163, 21, 21); // Red
                this.Styles[Style.Cpp.String + i].BackColor = backColor;
                this.Styles[Style.Cpp.Character + i].ForeColor = Color.FromArgb(163, 21, 21); // Red
                this.Styles[Style.Cpp.Character + i].BackColor = backColor;
                this.Styles[Style.Cpp.Verbatim + i].ForeColor = Color.FromArgb(128, 0, 0); // Red
                this.Styles[Style.Cpp.Verbatim + i].BackColor = backColor;
                this.Styles[Style.Cpp.StringEol + i].ForeColor = Color.FromArgb(163, 21, 21); // Red
                this.Styles[Style.Cpp.StringEol + i].BackColor = backColor;
                this.Styles[Style.Cpp.Operator + i].ForeColor = Color.Black;
                this.Styles[Style.Cpp.Operator + i].BackColor = backColor;
                this.Styles[Style.Cpp.Preprocessor + i].ForeColor = Color.Gray;
                this.Styles[Style.Cpp.Preprocessor + i].BackColor = backColor;
                this.Styles[Style.Cpp.Regex + i].ForeColor = Color.Black;
                this.Styles[Style.Cpp.Regex + i].BackColor = backColor;
                this.Styles[Style.Cpp.EscapeSequence + i].ForeColor = Color.FromArgb(183, 118, 251);
                this.Styles[Style.Cpp.EscapeSequence + i].BackColor = backColor;

                this.Styles[Substyle.Keyword + i].ForeColor = Color.FromArgb(143, 8, 196);
                this.Styles[Substyle.Keyword + i].BackColor = Color.White;
                this.Styles[Substyle.Method + i].ForeColor = Color.FromArgb(116, 83, 31);
                this.Styles[Substyle.Method + i].BackColor = Color.White;
                this.Styles[Substyle.ParamAndVar + i].ForeColor = Color.FromArgb(31, 55, 127);
                this.Styles[Substyle.ParamAndVar + i].BackColor = Color.White;
                this.Styles[Substyle.Struct + i].ForeColor = Color.FromArgb(43, 145, 175);
                this.Styles[Substyle.Struct + i].BackColor = Color.White;
                this.Styles[Substyle.Enum + i].ForeColor = Color.FromArgb(43, 145, 175);
                this.Styles[Substyle.Enum + i].BackColor = Color.White;
                this.Styles[Substyle.Interface + i].ForeColor = Color.FromArgb(43, 145, 175);
                this.Styles[Substyle.Interface + i].BackColor = Color.White;
            }
        }

        private void SetXMLDarkStyles()
        {
            Color backColor = Color.FromArgb(30, 30, 30);
            this.Styles[Style.Default].BackColor = backColor;

            // Configure the XML lexer styles
            this.Styles[Style.Xml.Default].ForeColor = Color.White;
            this.Styles[Style.Xml.Default].BackColor = backColor;
            this.Styles[Style.Xml.Tag].ForeColor = Color.FromArgb(86, 156, 214);
            this.Styles[Style.Xml.Tag].BackColor = backColor;
            this.Styles[Style.Xml.TagUnknown].ForeColor = Color.SkyBlue;
            this.Styles[Style.Xml.TagUnknown].BackColor = backColor;
            this.Styles[Style.Xml.Attribute].ForeColor = Color.FromArgb(146, 202, 244);
            this.Styles[Style.Xml.Attribute].BackColor = backColor;
            this.Styles[Style.Xml.AttributeUnknown].ForeColor = Color.PowderBlue;
            this.Styles[Style.Xml.AttributeUnknown].BackColor = backColor;
            this.Styles[Style.Xml.Number].ForeColor = Color.FromArgb(181, 206, 168);
            this.Styles[Style.Xml.Number].BackColor = backColor;
            this.Styles[Style.Xml.DoubleString].ForeColor = Color.FromArgb(200, 200, 200);
            this.Styles[Style.Xml.DoubleString].BackColor = backColor;
            this.Styles[Style.Xml.SingleString].ForeColor = Color.FromArgb(200, 200, 200);
            this.Styles[Style.Xml.SingleString].BackColor = backColor;
            this.Styles[Style.Xml.Other].ForeColor = Color.Gray;
            this.Styles[Style.Xml.Other].BackColor = backColor;
            this.Styles[Style.Xml.Comment].ForeColor = Color.FromArgb(87, 166, 74);
            this.Styles[Style.Xml.Comment].BackColor = backColor;
            this.Styles[Style.Xml.Entity].ForeColor = Color.LimeGreen;
            this.Styles[Style.Xml.Entity].BackColor = backColor;
            this.Styles[Style.Xml.TagEnd].ForeColor = Color.FromArgb(86, 156, 214);
            this.Styles[Style.Xml.TagEnd].BackColor = backColor;
            this.Styles[Style.Xml.XmlStart].ForeColor = Color.FromArgb(86, 156, 214);
            this.Styles[Style.Xml.XmlStart].BackColor = backColor;
            this.Styles[Style.Xml.XmlEnd].ForeColor = Color.FromArgb(86, 156, 214);
            this.Styles[Style.Xml.XmlEnd].BackColor = backColor;
            this.Styles[Style.Xml.Script].ForeColor = Color.Green;
            this.Styles[Style.Xml.Script].BackColor = backColor;
            this.Styles[Style.Xml.Asp].ForeColor = Color.Orange;
            this.Styles[Style.Xml.Asp].BackColor = backColor;
            this.Styles[Style.Xml.AspAt].ForeColor = Color.Purple;
            this.Styles[Style.Xml.AspAt].BackColor = backColor;
            this.Styles[Style.Xml.CData].ForeColor = Color.FromArgb(233, 213, 133);
            this.Styles[Style.Xml.CData].BackColor = backColor;
            this.Styles[Style.Xml.Question].ForeColor = Color.Salmon;
            this.Styles[Style.Xml.Question].BackColor = backColor;
            this.Styles[Style.Xml.Value].ForeColor = Color.Crimson;
            this.Styles[Style.Xml.Value].BackColor = backColor;
            this.Styles[Style.Xml.XcComment].ForeColor = Color.FromArgb(87, 166, 74);
            this.Styles[Style.Xml.XcComment].BackColor = backColor;

            const int SgmlTag = 21;
            const int SgmlError = 26;
            this.Styles[SgmlTag].ForeColor = Color.FromArgb(86, 156, 214);
            this.Styles[SgmlTag].BackColor = backColor;
            this.Styles[SgmlError].ForeColor = Color.White;
            this.Styles[SgmlError].BackColor = backColor;
        }

        private void SetXMLLightStyles()
        {
            Color backColor = Color.White;
            this.Styles[Style.Default].BackColor = backColor;

            // Configure the XML lexer styles
            this.Styles[Style.Xml.Default].ForeColor = Color.Black;
            this.Styles[Style.Xml.Default].BackColor = backColor;
            this.Styles[Style.Xml.Tag].ForeColor = Color.FromArgb(163, 21, 21);
            this.Styles[Style.Xml.Tag].BackColor = backColor;
            this.Styles[Style.Xml.TagUnknown].ForeColor = Color.SkyBlue;
            this.Styles[Style.Xml.TagUnknown].BackColor = backColor;
            this.Styles[Style.Xml.Attribute].ForeColor = Color.Red;
            this.Styles[Style.Xml.Attribute].BackColor = backColor;
            this.Styles[Style.Xml.AttributeUnknown].ForeColor = Color.PowderBlue;
            this.Styles[Style.Xml.AttributeUnknown].BackColor = backColor;
            this.Styles[Style.Xml.Number].ForeColor = Color.FromArgb(181, 206, 168);
            this.Styles[Style.Xml.Number].BackColor = backColor;
            this.Styles[Style.Xml.DoubleString].ForeColor = Color.Blue;
            this.Styles[Style.Xml.DoubleString].BackColor = backColor;
            this.Styles[Style.Xml.SingleString].ForeColor = Color.Blue;
            this.Styles[Style.Xml.SingleString].BackColor = backColor;
            this.Styles[Style.Xml.Other].ForeColor = Color.Blue;
            this.Styles[Style.Xml.Other].BackColor = backColor;
            this.Styles[Style.Xml.Comment].ForeColor = Color.Green;
            this.Styles[Style.Xml.Comment].BackColor = backColor;
            this.Styles[Style.Xml.Entity].ForeColor = Color.LimeGreen;
            this.Styles[Style.Xml.Entity].BackColor = backColor;
            this.Styles[Style.Xml.TagEnd].ForeColor = Color.FromArgb(163, 21, 21);
            this.Styles[Style.Xml.TagEnd].BackColor = backColor;
            this.Styles[Style.Xml.XmlStart].ForeColor = Color.FromArgb(163, 21, 21);
            this.Styles[Style.Xml.XmlStart].BackColor = backColor;
            this.Styles[Style.Xml.XmlEnd].ForeColor = Color.FromArgb(163, 21, 21);
            this.Styles[Style.Xml.XmlEnd].BackColor = backColor;
            this.Styles[Style.Xml.Script].ForeColor = Color.Green;
            this.Styles[Style.Xml.Script].BackColor = backColor;
            this.Styles[Style.Xml.Asp].ForeColor = Color.Orange;
            this.Styles[Style.Xml.Asp].BackColor = backColor;
            this.Styles[Style.Xml.AspAt].ForeColor = Color.Purple;
            this.Styles[Style.Xml.AspAt].BackColor = backColor;
            this.Styles[Style.Xml.CData].ForeColor = Color.Gray;
            this.Styles[Style.Xml.CData].BackColor = backColor;
            this.Styles[Style.Xml.Question].ForeColor = Color.Salmon;
            this.Styles[Style.Xml.Question].BackColor = backColor;
            this.Styles[Style.Xml.Value].ForeColor = Color.Crimson;
            this.Styles[Style.Xml.Value].BackColor = backColor;
            this.Styles[Style.Xml.XcComment].ForeColor = Color.Green;
            this.Styles[Style.Xml.XcComment].BackColor = backColor;

            const int SgmlTag = 21;
            const int SgmlError = 26;
            this.Styles[SgmlTag].ForeColor = Color.FromArgb(163, 21, 21);
            this.Styles[SgmlTag].BackColor = backColor;
            this.Styles[SgmlError].ForeColor = Color.Black;
            this.Styles[SgmlError].BackColor = backColor;
        }

        private void SetCaretLineDarkColor()
        {
            this.CaretLineBackColor = CaretLineFrameEnabled
                ? Color.FromArgb(70, 70, 70)
                : Color.FromArgb(40, 40, 40);
        }

        private void SetCaretLineLightColor()
        {
            this.CaretLineBackColor = CaretLineFrameEnabled
                ? Color.FromArgb(234, 234, 242)
                : Color.GhostWhite;
        }
        #endregion

        #region Event Handlers
        public event EventHandler BuildNeeded;
        private void OnBuildNeeded()
        {
            this.BuildNeeded?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler<NewTabEventArgs> DefTabNeeded;
        private void OnDefTabNeeded(string name, string path)
        {
            this.DefTabNeeded?.Invoke(this, new NewTabEventArgs(name, path));
        }
        #endregion

        public CodeTextBox()
        {
            if (!this.DesignMode)
            {
                timer.Interval = 1000;
                timer.Tick += (sender, e) =>
                {
                    if (this.Lexer == Lexer.Cpp)
                    {
                        if (this.useExtendedColors)
                        {
                            ColorizeMethods();
                        }
                        ParseVariables(this.CurrentPosition);
                    }
                };
                timer.Start();
            }

            InitializeComponent();

            // Hide icon margin in Light Bulb menu
            ((ToolStripDropDownMenu)bulbIcon.DropDown).ShowImageMargin = false;

            docCollection.Add(Guid.Empty, this.Document);
        }

        private void InitializeComponent()
        {
            this.lightBulbMenu.SuspendLayout();
            this.SuspendLayout();

            iBox.Visible = false;

            findPanel.Visible = false;
            findPanel.VisibleChanged += FindPanel_VisibleChanged;
            findPanel.ParametersChanged += FindPanel_ParametersChanged;
            findPanel.ReplaceAllClicked += FindPanel_ReplaceAllClicked;
            findPanel.FindNextClicked += FindPanel_FindNextClicked;

            indicatorBar.Visible = false;
            indicatorBar.Scroll += IndicatorBar_Scroll;

            // 
            // lightBulbMenu
            // 
            this.lightBulbMenu.Dock = DockStyle.None;
            this.lightBulbMenu.GripStyle = ToolStripGripStyle.Hidden;
            this.lightBulbMenu.Items.Add(this.bulbIcon);
            this.lightBulbMenu.Location = new Point(50, 100);
            this.lightBulbMenu.Name = "lightBulbMenu";
            this.lightBulbMenu.Size = new Size(63, 25);
            this.lightBulbMenu.TabIndex = 20;
            this.lightBulbMenu.Text = "Light Bulb";
            this.lightBulbMenu.Visible = false;
            this.lightBulbMenu.Cursor = Cursors.Default;
            // 
            // bulbIcon
            // 
            this.bulbIcon.AutoToolTip = false;
            this.bulbIcon.DisplayStyle = ToolStripItemDisplayStyle.Image;
            this.bulbIcon.ImageName = "Bulb";
            this.bulbIcon.Name = "bulbIcon";
            this.bulbIcon.Size = new Size(29, 22);
            this.bulbIcon.Text = "Bulb Icon";

            #region ScintillaNET Initializers
            this.Lexer = Lexer.Cpp;

            int substyleStart = this.AllocateSubstyles(Style.Cpp.Identifier, Substyle.NormStyleCount);
            Substyle.SetStyles(substyleStart);

            // Set the keywords for Syntax Highlighting
            UpdateSyntaxHighlighting();

            this.StyleResetDefault();
            this.Theme = Theme.Light;
            this.Styles[Style.Default].Font = "Consolas";
            this.Styles[Style.Default].Size = 10;

            // Set the styles for Ctrl+F Find
            this.Indicators[Indicator.Find].Style = IndicatorStyle.StraightBox;
            this.Indicators[Indicator.Find].Under = true;
            this.Indicators[Indicator.Find].OutlineAlpha = 153;
            this.Indicators[Indicator.Find].Alpha = 204;

            // Set the styles for Errors underlines
            this.Indicators[Indicator.Error].Style = IndicatorStyle.SquigglePixmap;
            this.Indicators[Indicator.Warning].Style = IndicatorStyle.SquigglePixmap;

            // Set the styles for Errors underlines
            this.Indicators[Indicator.Spelling].Style = IndicatorStyle.SquigglePixmap;

            // Set the styles for focused Object
            this.Indicators[Indicator.ObjectHighlight].Style = IndicatorStyle.StraightBox;
            this.Indicators[Indicator.ObjectHighlight].Under = true;
            this.Indicators[Indicator.ObjectHighlight].Alpha = 204;
            this.Indicators[Indicator.ObjectHighlight].OutlineAlpha = 255;

            // Set the styles for variable rename
            this.Indicators[Indicator.Rename].Style = IndicatorStyle.DotBox;
            this.Indicators[Indicator.Rename].Under = true;
            this.Indicators[Indicator.Rename].ForeColor = Color.DimGray;
            this.Indicators[Indicator.Rename].OutlineAlpha = 255;
            this.Indicators[Indicator.Rename].Alpha = 0;

            // Set the styles for focused Object Definition
            this.Indicators[Indicator.ObjectHighlightDef].Style = IndicatorStyle.StraightBox;
            this.Indicators[Indicator.ObjectHighlightDef].Under = true;
            this.Indicators[Indicator.ObjectHighlightDef].Alpha = 204;
            this.Indicators[Indicator.ObjectHighlightDef].OutlineAlpha = 255;

            // Rainbow Braces
            this.Indicators[Indicator.BraceColor0].Style = IndicatorStyle.TextFore;
            this.Indicators[Indicator.BraceColor0].ForeColor = Color.HotPink;
            this.Indicators[Indicator.BraceColor1].Style = IndicatorStyle.TextFore;
            this.Indicators[Indicator.BraceColor1].ForeColor = Color.DarkOrchid;
            this.Indicators[Indicator.BraceColor2].Style = IndicatorStyle.TextFore;
            this.Indicators[Indicator.BraceColor2].ForeColor = Color.Cyan;
            this.Indicators[Indicator.BraceColor3].Style = IndicatorStyle.TextFore;
            this.Indicators[Indicator.BraceColor3].ForeColor = Color.Gold;
            this.Indicators[Indicator.BraceColor4].Style = IndicatorStyle.TextFore;
            this.Indicators[Indicator.BraceColor4].ForeColor = Color.PaleGreen;

            // Vertical indent guides
            this.IndentationGuides = IndentView.None;

            // Configure a margin to use as padding between Line Numbers and the document
            this.Margins[LeftMargin.Padding].Type = MarginType.BackColor;
            this.Margins[LeftMargin.Padding].Sensitive = false;
            this.Margins[LeftMargin.Padding].Width = 2;
            this.Margins[LeftMargin.Padding].Mask = 0;

            // Configure a margin to display Line Numbers
            this.Margins[LeftMargin.LineNumbers].Type = MarginType.Number;
            this.Margins[LeftMargin.LineNumbers].Sensitive = false;
            this.Margins[LeftMargin.LineNumbers].Width = 0;
            this.Margins[LeftMargin.LineNumbers].Mask = 0;

            // Configure a margin to display Bookmark symbols
            this.Margins[LeftMargin.Bookmarks].Width = 0;
            this.Margins[LeftMargin.Bookmarks].Sensitive = true;
            this.Margins[LeftMargin.Bookmarks].Type = MarginType.Symbol;
            this.Margins[LeftMargin.Bookmarks].Mask = BookmarkMargin.Mask;
            this.Margins[LeftMargin.Bookmarks].Cursor = MarginCursor.Arrow;

            this.Markers[BookmarkMargin.Marker].Symbol = MarkerSymbol.Empty;
            this.Markers[BookmarkMargin.Marker].SetBackColor(Color.DeepSkyBlue);
            this.Markers[BookmarkMargin.Marker].SetForeColor(Color.DeepSkyBlue);

            // Instruct the lexer to calculate folding
            this.SetProperty("fold", "1");
            this.SetProperty("fold.compact", "0");
            this.SetProperty("lexer.cpp.allow.dollars", "0");
            this.SetProperty("lexer.cpp.escape.sequence", "1");

            // Configure a margin to display folding symbols
            this.Margins[LeftMargin.CodeFolding].Type = MarginType.Symbol;
            this.Margins[LeftMargin.CodeFolding].Mask = Marker.MaskFolders;
            this.Margins[LeftMargin.CodeFolding].Sensitive = true;
            this.Margins[LeftMargin.CodeFolding].Width = 0;
            this.Margins[LeftMargin.CodeFolding].Cursor = MarginCursor.Arrow;

            // Configure folding markers with respective symbols
            this.Markers[Marker.Folder].Symbol = MarkerSymbol.BoxPlus;
            this.Markers[Marker.FolderOpen].Symbol = MarkerSymbol.BoxMinus;
            this.Markers[Marker.FolderEnd].Symbol = MarkerSymbol.BoxPlusConnected;
            this.Markers[Marker.FolderMidTail].Symbol = MarkerSymbol.TCorner;
            this.Markers[Marker.FolderOpenMid].Symbol = MarkerSymbol.BoxMinusConnected;
            this.Markers[Marker.FolderSub].Symbol = MarkerSymbol.VLine;
            this.Markers[Marker.FolderTail].Symbol = MarkerSymbol.LCorner;

            // Enable automatic folding
            this.AutomaticFold = (AutomaticFold.Show | AutomaticFold.Change);

            // Fold Ellipsis
            this.FoldDisplayTextSetStyle(FoldDisplayText.Boxed);

            // Display whitespace
            this.WhitespaceSize = 1;
            this.ViewWhitespace = WhitespaceMode.Invisible;

            // Current Line Highlight
            this.CaretLineVisible = true;
            this.CaretLineVisibleAlways = true;

            // Word Wrap
            this.WrapMode = WrapMode.None;
            this.WrapIndentMode = WrapIndentMode.Indent;
            this.WrapVisualFlags = WrapVisualFlags.None;

            // Zoom
            this.Zoom = 0;

            // ToolTip Delay (ms)
            this.MouseDwellTime = 250;

            // Free up default HotKeys, so they can be used for other things
            // or just to disable undesired features (see comments for defaults)
            this.ClearCmdKey(Keys.Control | Keys.B); // Insert u002
            this.ClearCmdKey(Keys.Control | Keys.C); // Copy as Text
            this.ClearCmdKey(Keys.Control | Keys.E); // Insert u005
            this.ClearCmdKey(Keys.Control | Keys.F); // Insert u006
            this.ClearCmdKey(Keys.Control | Keys.G); // Insert u007
            this.ClearCmdKey(Keys.Control | Keys.H); // Insert u008
            this.ClearCmdKey(Keys.Control | Keys.I); // Insert u009 (Tab)
            this.ClearCmdKey(Keys.Control | Keys.J); // Insert u00A (Line Feed)
            this.ClearCmdKey(Keys.Control | Keys.K); // Insert u00B
            this.ClearCmdKey(Keys.Control | Keys.L); // Remove Line
            this.ClearCmdKey(Keys.Control | Keys.M); // Insert u00D (Carriage Return)
            this.ClearCmdKey(Keys.Control | Keys.N); // Insert u00E
            this.ClearCmdKey(Keys.Control | Keys.O); // Insert u00F
            this.ClearCmdKey(Keys.Control | Keys.P); // Insert u010
            this.ClearCmdKey(Keys.Control | Keys.Q); // Insert u011
            this.ClearCmdKey(Keys.Control | Keys.R); // Insert u012
            this.ClearCmdKey(Keys.Control | Keys.S); // Insert u0013
            this.ClearCmdKey(Keys.Control | Keys.T); // Swap Lines
            this.ClearCmdKey(Keys.Control | Keys.W); // Insert u0017
            this.ClearCmdKey(Keys.Control | Keys.OemOpenBrackets); // Go To Previous Code Block
            this.ClearCmdKey(Keys.Control | Keys.OemCloseBrackets); // Go To Next Code Block

            // Disable scintilla's own context menu
            this.UsePopup(false);

            // Perform lexer styling for the whole document; not just the scrolled region
            this.IdleStyling = IdleStyling.All;

            // Use DirectWrite text rendering
            this.Technology = Technology.DirectWrite;
            #endregion

            this.Controls.Add(iBox);
            this.Controls.Add(findPanel);
            this.Controls.Add(lightBulbMenu);
            this.Controls.Add(indicatorBar);
            this.lightBulbMenu.ResumeLayout(true);
            this.ResumeLayout(true);
        }

        #region Intelligent Assistance functions
        private string GetLastWords(int position)
        {
            int wordEndPos = this.WordEndPosition(position);
            string strippedText = this.GetTextRange(0, wordEndPos).StripParens().StripAngleBrackets().StripSquareBrackets();

            int posIndex = strippedText.Length;
            bool isWhiteSpaceAllowed = false;

            while (posIndex > 0)
            {
                char c = strippedText[posIndex - 1];

                if (isWhiteSpaceAllowed && char.IsWhiteSpace(c))
                {
                    // no-op
                }
                else if (!(char.IsLetterOrDigit(c) || c == '_' || c == '.'))
                {
                    return strippedText.Substring(posIndex);
                }
                else if (posIndex - 1 == 0)
                {
                    return strippedText.Substring(posIndex - 1);
                }
                else if (c == '.')
                {
                    isWhiteSpaceAllowed = true;
                }
                else
                {
                    isWhiteSpaceAllowed = false;
                }

                posIndex--;
            }

            return string.Empty;
        }

        private int[] GetLastWordsPos(int position)
        {
            List<int> tokenPositions = new List<int>();
            int tokenPos = position;
            bool isWhiteSpaceAllowed = false;

            while (tokenPos > 0)
            {
                char c = this.GetCharAt(tokenPos - 1);

                if (isWhiteSpaceAllowed && char.IsWhiteSpace(c))
                {
                    // no-op
                }
                else if (c == ')' || c == '>')
                {
                    tokenPos = this.BraceMatch(tokenPos - 1) + 1;
                    isWhiteSpaceAllowed = false;
                }
                else if (c == '.')
                {
                    tokenPositions.Add(tokenPos);
                    isWhiteSpaceAllowed = true;
                }
                else if (!(char.IsLetterOrDigit(c) || c == '_'))
                {
                    tokenPositions.Add(tokenPos);
                    break;
                }
                else if (tokenPos - 1 == 0)
                {
                    tokenPositions.Add(tokenPos - 1);
                    break;
                }
                else
                {
                    isWhiteSpaceAllowed = false;
                }

                tokenPos--;
            }

            return tokenPositions.Reverse<int>().ToArray();
        }

        private Type GetNumberType(int position)
        {
            int numStart = position;
            while (numStart > InvalidPosition)
            {
                int style = this.GetStyleAt(numStart - 1);
                if (style != Style.Cpp.Number && style != Style.Cpp.Number + Preprocessor)
                {
                    break;
                }

                numStart--;
            }

            bool foundDecimal = false;
            int numEnd = numStart;
            while (numEnd <= this.TextLength)
            {
                char curChar = this.GetCharAt(numEnd);

                if (curChar == '.')
                {
                    foundDecimal = true;
                }
                else if (!char.IsDigit(curChar))
                {
                    numEnd--;
                    break;
                }

                numEnd++;
            }

            int startStyle = this.GetStyleAt(numStart);
            int endStyle = this.GetStyleAt(numEnd);
            if ((startStyle != Style.Cpp.Number && startStyle != Style.Cpp.Number + Preprocessor) ||
                (endStyle != Style.Cpp.Number && endStyle != Style.Cpp.Number + Preprocessor))
            {
                return null;
            }

            char suffix = this.GetCharAt(numEnd + 1).ToUpperInvariant();
            switch (suffix)
            {
                case 'F':
                    return typeof(float);
                case 'L':
                    return typeof(long);
                case 'D':
                    return typeof(double);
                case 'M':
                    return typeof(decimal);
                case 'U':
                    return (this.GetCharAt(numEnd + 2).ToUpperInvariant() == 'L') ? typeof(ulong) : typeof(uint);
                default:
                    return (foundDecimal) ? typeof(double) : typeof(int);
            }
        }

        private Tuple<int, int> GetMethodBounds(int position)
        {
            if (IsInClassRoot(position))
            {
                return new Tuple<int, int>(InvalidPosition, InvalidPosition);
            }

            this.SetTargetRange(position, this.TextLength);
            while (this.SearchInTarget("}") != InvalidPosition)
            {
                int targetStart = this.TargetStart;
                int targetEnd = this.TargetEnd;

                int openBrace = this.BraceMatch(targetStart);
                if (!IsNestedBraceSet(openBrace, targetStart))
                {
                    return new Tuple<int, int>(openBrace, targetStart);
                }

                this.SetTargetRange(targetEnd, this.TextLength);
            }

            return new Tuple<int, int>(InvalidPosition, InvalidPosition);
        }

        private bool IsNestedBraceSet(int openPos, int closePos)
        {
            if (openPos >= closePos)
            {
                throw new ArgumentException(nameof(openPos) + " is greater or equal to " + nameof(closePos));
            }

            this.SetTargetRange(closePos, this.TextLength);
            while (this.SearchInTarget("}") != InvalidPosition)
            {
                if (this.BraceMatch(this.TargetStart) < openPos)
                {
                    return true;
                }

                this.SetTargetRange(this.TargetEnd, this.TextLength);
            }

            return false;
        }

        private bool IsInClassRoot(int position)
        {
            this.SetTargetRange(0, position);
            while (this.SearchInTarget("{") != InvalidPosition)
            {
                int openBrace = this.TargetStart;
                int closeBrace = this.BraceMatch(this.TargetStart);

                if (closeBrace == InvalidPosition)
                {
                    return true;
                }

                if (position > openBrace && position < closeBrace)
                {
                    return false;
                }

                if (closeBrace >= position)
                {
                    return true;
                }

                this.SetTargetRange(closeBrace, position);
            }

            return true;
        }

        private void ParseVariables(int position, bool localOnly = true)
        {
            Intelli.Parameters.Clear();
            Intelli.Variables.Clear();
            Intelli.VarPos.Clear();

            if (this.ReadOnly)
            {
                // We can use ReadOnly to indicate the document is a Type Definition
                return;
            }

            int rangeStart = 0;
            int rangeEnd = this.TextLength;
            IEnumerable<MethodInfo> methods = Intelli.UserScript.GetMethods(userScriptBindingFlags);

            if (localOnly)
            {
                Tuple<int, int> methodBounds = GetMethodBounds(position);
                if (methodBounds.Item1 == InvalidPosition || methodBounds.Item2 == InvalidPosition)
                {
                    return;
                }

                // Gather parameters of method
                int closeParenPos = methodBounds.Item1 - 1;
                while (closeParenPos > InvalidPosition && this.GetCharAt(closeParenPos) != ')')
                {
                    closeParenPos--;
                }

                int openParenPos = this.BraceMatch(closeParenPos);
                if (openParenPos != InvalidPosition)
                {
                    string methodName = this.GetWordFromPosition(openParenPos);
                    IEnumerable<MethodInfo> methodMatches = methods
                        .Where(m => m.Name.Equals(methodName, StringComparison.Ordinal));

                    methods = methodMatches.Any()
                        ? new[] { GetOverload(methodMatches, openParenPos) }
                        : Array.Empty<MethodInfo>();
                }
                else
                {
                    methods = Array.Empty<MethodInfo>();
                }

                rangeStart = methodBounds.Item1 + 1;
                rangeEnd = methodBounds.Item2 - 1;
            }

            foreach (ParameterInfo parameter in methods.SelectMany(method => method.GetParameters()))
            {
                if (!Intelli.Parameters.ContainsKey(parameter.Name))
                {
                    Intelli.Parameters.Add(parameter.Name, parameter.ParameterType);
                }
            }

            string bodyText = this.GetTextRange(rangeStart, rangeEnd - rangeStart);
            IEnumerable<string> bodyWords = bodyText.Split(new char[] { ' ', '(', '{', '<', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).Distinct();

            this.SearchFlags = SearchFlags.MatchCase | SearchFlags.WholeWord;
            foreach (string word in bodyWords)
            {
                bool isArray = word.EndsWith("[]", StringComparison.Ordinal);
                string typeStr = isArray ? word.Replace("[]", string.Empty) : word;

                Type type;
                if (Intelli.AllTypes.ContainsKey(typeStr))
                {
                    type = isArray ? Intelli.AllTypes[typeStr].MakeArrayType() : Intelli.AllTypes[typeStr];
                }
                else if (Intelli.UserDefinedTypes.ContainsKey(typeStr))
                {
                    type = isArray ? Intelli.UserDefinedTypes[typeStr].MakeArrayType() : Intelli.UserDefinedTypes[typeStr];
                }
                else
                {
                    continue;
                }

                this.SetTargetRange(rangeStart, rangeEnd);
                while (this.SearchInTarget(word) != InvalidPosition)
                {
                    int varPos = this.TargetEnd;

                    if (!localOnly && IsInClassRoot(varPos))
                    {
                        this.SetTargetRange(varPos, rangeEnd);
                        continue;
                    }

                    this.SetTargetRange(varPos, rangeEnd);

                    if (type.IsGenericType)
                    {
                        if (this.GetCharAt(varPos) != '<')
                        {
                            continue;
                        }

                        if (type.IsConstructedGenericType)
                        {
                            type = type.GetGenericTypeDefinition();
                        }

                        string args = GetGenericArgs(varPos);
                        type = type.MakeGenericType(args);

                        while (this.GetCharAt(varPos - 1) != '>' && varPos <= rangeEnd)
                        {
                            varPos++;
                        }
                    }

                    // Ensure there's at least one space after the type
                    if (!char.IsWhiteSpace(this.GetCharAt(varPos)))
                    {
                        continue;
                    }

                    // Skip over white space
                    while (char.IsWhiteSpace(this.GetCharAt(varPos)) && varPos <= rangeEnd)
                    {
                        varPos++;
                    }

                    // find the semi-colon
                    int semiColonPos = varPos;
                    while (this.GetCharAt(semiColonPos) != ';' && semiColonPos <= rangeEnd)
                    {
                        semiColonPos++;
                    }

                    string varRange = this.GetTextRange(varPos, semiColonPos - varPos);
                    string[] possibleVars = varRange.StripBraces().StripParens().Split(new char[] { ',' });
                    MatchCollection braceMatches = Regex.Matches(varRange, @"\{(?:\{[^{}]*\}|[^{}])*\}");

                    int varCount = possibleVars.Length;
                    if (braceMatches.Count > 0 && possibleVars.Length != braceMatches.Count)
                    {
                        varCount = 1;
                    }

                    for (int i = 0; i < varCount; i++)
                    {
                        while (char.IsWhiteSpace(this.GetCharAt(varPos)) && varPos <= rangeEnd)
                        {
                            varPos++;
                        }

                        int thisVarPos = varPos;
                        if (varCount > 1)
                        {
                            int braceLength = (braceMatches.Count > i) ? braceMatches[i].Groups[0].Length : 0;
                            varPos += possibleVars[i].Length + braceLength + 1;
                        }

                        int style = this.GetStyleAt(thisVarPos);
                        if (style != Style.Cpp.Identifier && style != Style.Cpp.Identifier + Preprocessor &&
                            style != Substyle.ParamAndVar && style != Substyle.ParamAndVar + Preprocessor)
                        {
                            continue;
                        }

                        string varName = this.GetWordFromPosition(thisVarPos);

                        // Ensure the variable doesn't contain illegal characters
                        if (!varName.IsCSharpIdentifier())
                        {
                            continue;
                        }

                        if (Intelli.AllTypes.ContainsKey(varName) || Intelli.Keywords.Contains(varName) || Intelli.Snippets.ContainsKey(varName) ||
                            Intelli.UserDefinedTypes.ContainsKey(varName) || Intelli.UserScript.Contains(varName, false))
                        {
                            continue;
                        }

                        Intelli.VarPos[varName] = thisVarPos;

                        if (localOnly && Intelli.Parameters.ContainsKey(varName))
                        {
                            continue;
                        }

                        Intelli.Variables[varName] = type;
                    }
                }
            }
        }

        private void ColorizeMethods()
        {
            ParseVariables(0, false);

            HashSet<string> methodNames = new HashSet<string>();
            int pos = 0;
            while (pos < this.TextLength)
            {
                int style = this.GetStyleAt(pos);
                if ((style == Style.Cpp.Identifier || style == Style.Cpp.Identifier + Preprocessor ||
                    style == Substyle.Method || style == Substyle.Method + Preprocessor) &&
                    this.GetIntelliType(pos) == IntelliType.Method)
                {
                    methodNames.Add(this.GetWordFromPosition(pos));
                }

                int endPos = this.WordEndPosition(pos);
                pos = (endPos > pos) ? endPos : pos + 1;
            }

            IEnumerable<string> paramNames = Intelli.UserScript.GetMethods(userScriptBindingFlags)
                .SelectMany(m => m.GetParameters())
                .Select(p => p.Name)
                .Distinct();

            this.updatingStyles = true;
            this.SetIdentifiers(Substyle.Method, methodNames.Join(" "));
            this.SetIdentifiers(Substyle.ParamAndVar, paramNames.Concat(Intelli.Variables.Keys).Join(" "));
        }

        private void ColorizeBraces()
        {
            if (this.Lexer != Lexer.Cpp)
            {
                return;
            }

            for (int i = Indicator.BraceColor0; i <= Indicator.BraceColor4; i++)
            {
                this.IndicatorCurrent = GetRainbowIndicator(i);
                this.IndicatorClearRange(0, this.TextLength);
            }

            int braceLevel = -1;

            for (int pos = 0; pos < this.TextLength; pos++)
            {
                int style = this.GetStyleAt(pos);
                if (style != Style.Cpp.Operator && style != Style.Cpp.Operator + Preprocessor)
                {
                    continue;
                }

                char c = this.GetCharAt(pos);
                if (c.IsBrace(false))
                {
                    if (braceLevel >= 0)
                    {
                        braceLevel--;
                    }

                    continue;
                }

                if (!c.IsBrace(true))
                {
                    continue;
                }

                int bracePos2 = this.BraceMatch(pos);
                if (bracePos2 == InvalidPosition)
                {
                    continue;
                }

                braceLevel++;

                int rainbowIndicator = GetRainbowIndicator(braceLevel);
                if (rainbowIndicator == -1)
                {
                    continue;
                }

                this.IndicatorCurrent = rainbowIndicator;
                this.IndicatorFillRange(pos, 1);
                this.IndicatorFillRange(bracePos2, 1);
            }
        }

        private static int GetRainbowIndicator(int braceLevel)
        {
            if (braceLevel < 0)
            {
                return -1;
            }

            return (braceLevel % 5) + Indicator.BraceColor0;
        }

        private IntelliType GetIntelliType(int position)
        {
            int style = this.GetStyleAt(position);
            if (style != Style.Cpp.Word && style != Style.Cpp.Word + Preprocessor &&
                style != Style.Cpp.Word2 && style != Style.Cpp.Word2 + Preprocessor &&
                style != Substyle.Enum && style != Substyle.Enum + Preprocessor &&
                style != Substyle.Interface && style != Substyle.Interface + Preprocessor &&
                style != Substyle.Struct && style != Substyle.Struct + Preprocessor &&
                style != Style.Cpp.Identifier && style != Style.Cpp.Identifier + Preprocessor &&
                style != Substyle.Method && style != Substyle.Method + Preprocessor &&
                style != Substyle.ParamAndVar && style != Substyle.ParamAndVar + Preprocessor)
            {
                return IntelliType.None;
            }

            string lastWords = GetLastWords(position);
            if (lastWords.Length == 0)
            {
                return IntelliType.None;
            }

            if (Intelli.Variables.ContainsKey(lastWords))
            {
                return IntelliType.Variable;
            }

            if (Intelli.Parameters.ContainsKey(lastWords))
            {
                return IntelliType.Parameter;
            }

            if (Intelli.AllTypes.ContainsKey(lastWords) || Intelli.UserDefinedTypes.ContainsKey(lastWords))
            {
                return IntelliType.Type;
            }

            MemberInfo memberInfo = GetMember(position, out _);
            if (memberInfo == null)
            {
                return IntelliType.None;
            }

            switch (memberInfo.MemberType)
            {
                case MemberTypes.Property:
                    return IntelliType.Property;
                case MemberTypes.Method:
                    return IntelliType.Method;
                case MemberTypes.Field:
                    return IntelliType.Field;
                case MemberTypes.Event:
                    return IntelliType.Event;
                case MemberTypes.NestedType:
                    return IntelliType.Type;
            }

            return IntelliType.None;
        }

        private void HighlightWordUsage()
        {
            if (this.Lexer != Lexer.Cpp)
            {
                return;
            }

            this.IndicatorCurrent = Indicator.ObjectHighlight;
            this.IndicatorClearRange(0, this.TextLength);

            this.IndicatorCurrent = Indicator.ObjectHighlightDef;
            this.IndicatorClearRange(0, this.TextLength);

            int position = this.WordStartPosition(this.CurrentPosition);
            ParseVariables(position);

            IntelliType currentType = GetIntelliType(position);
            if (currentType == IntelliType.None)
            {
                return;
            }

            string word = this.GetWordFromPosition(position);
            if (word.Length == 0)
            {
                return;
            }

            // Get declaring type for when working type members
            Type declaringType = GetDeclaringType(position);

            // Search the document
            int rangeEnd = this.TextLength;
            if (currentType == IntelliType.Variable)
            {
                Tuple<int, int> methodBounds = GetMethodBounds(position);
                if (methodBounds.Item1 == InvalidPosition || methodBounds.Item2 == InvalidPosition)
                {
                    this.TargetWholeDocument();
                }
                else
                {
                    this.SetTargetRange(methodBounds.Item1 + 1, methodBounds.Item2 - 1);
                    rangeEnd = methodBounds.Item2 - 1;
                }
            }
            else
            {
                this.TargetWholeDocument();
            }

            this.SearchFlags = SearchFlags.MatchCase | SearchFlags.WholeWord;

            while (this.SearchInTarget(word) != InvalidPosition)
            {
                if (GetIntelliType(this.TargetStart) == currentType)
                {
                    if (currentType == IntelliType.Property || currentType == IntelliType.Method || currentType == IntelliType.EnumItem ||
                        currentType == IntelliType.Constant || currentType == IntelliType.Field)
                    {
                        if (GetDeclaringType(this.TargetStart) != declaringType)
                        {
                            this.SetTargetRange(this.TargetEnd, rangeEnd);
                            continue;
                        }
                    }

                    this.IndicatorCurrent = Indicator.ObjectHighlight;

                    #region Word Definition Highlight
                    if ((currentType == IntelliType.Variable || currentType == IntelliType.Parameter) && Intelli.VarPos.ContainsKey(word) && Intelli.VarPos[word] == this.TargetStart)
                    {
                        this.IndicatorCurrent = Indicator.ObjectHighlightDef;
                    }
                    else if (currentType == IntelliType.Type && Intelli.UserDefinedTypes.ContainsKey(word))
                    {
                        int typePos = this.TargetStart - 1;

                        // Skip over white space
                        while (char.IsWhiteSpace(this.GetCharAt(typePos)) && typePos > InvalidPosition)
                        {
                            typePos--;
                        }

                        int style = this.GetStyleAt(typePos);
                        if (style == Style.Cpp.Word || style == Style.Cpp.Word + Preprocessor)
                        {
                            string foundType = this.GetWordFromPosition(typePos);

                            Type t = Intelli.UserDefinedTypes[word];
                            string baseType = t.GetObjectType();

                            if (baseType == foundType)
                            {
                                this.IndicatorCurrent = Indicator.ObjectHighlightDef;
                            }
                        }
                    }
                    else if (Intelli.UserScript.Contains(word, true))
                    {
                        int typePos = this.TargetStart - 1;

                        // Skip over white space
                        while (char.IsWhiteSpace(this.GetCharAt(typePos)) && typePos > InvalidPosition)
                        {
                            typePos--;
                        }

                        int style = this.GetStyleAt(typePos);
                        if (style == Style.Cpp.Word || style == Style.Cpp.Word + Preprocessor ||
                            style == Style.Cpp.Word2 || style == Style.Cpp.Word2 + Preprocessor ||
                            style == Substyle.Enum || style == Substyle.Enum + Preprocessor ||
                            style == Substyle.Interface || style == Substyle.Interface + Preprocessor ||
                            style == Substyle.Struct || style == Substyle.Struct + Preprocessor ||
                            style == Style.Cpp.Identifier || style == Style.Cpp.Identifier + Preprocessor ||
                            style == Substyle.Method || style == Substyle.Method + Preprocessor ||
                            style == Substyle.ParamAndVar || style == Substyle.ParamAndVar + Preprocessor)
                        {
                            string foundType = this.GetWordFromPosition(typePos);
                            Type t = null;
                            if (Intelli.AllTypes.ContainsKey(foundType))
                            {
                                t = Intelli.AllTypes[foundType];
                            }
                            else if (Intelli.UserDefinedTypes.ContainsKey(foundType))
                            {
                                t = Intelli.UserDefinedTypes[foundType];
                            }
                            else if (foundType == typeof(void).GetDisplayName())
                            {
                                t = typeof(void);
                            }

                            if (t != null)
                            {
                                MemberInfo member = Intelli.UserScript.GetMember(word, userScriptBindingFlags)[0];
                                string returnType = member.GetReturnType()?.GetDisplayName();

                                if (returnType?.Length > 0 && t.GetDisplayName() == returnType)
                                {
                                    this.IndicatorCurrent = Indicator.ObjectHighlightDef;
                                }
                            }
                        }
                    }
                    #endregion

                    this.IndicatorFillRange(this.TargetStart, this.TargetEnd - this.TargetStart);
                }

                // Search the remainder of the document
                this.SetTargetRange(this.TargetEnd, rangeEnd);
            }
        }

        private string GetIntelliTipCSharp(int position)
        {
            int style = this.GetStyleAt(position);
            if (style == Style.Cpp.Comment || style == Style.Cpp.Comment + Preprocessor ||
                style == Style.Cpp.CommentLine || style == Style.Cpp.CommentLine + Preprocessor ||
                style == Style.Cpp.Preprocessor || style == Style.Cpp.Preprocessor + Preprocessor ||
                style == Style.Cpp.Operator || style == Style.Cpp.Operator + Preprocessor ||
                style == Style.Cpp.Default || style == Style.Cpp.Default + Preprocessor)
            {
                return string.Empty;
            }
            if (style == Style.Cpp.String || style == Style.Cpp.String + Preprocessor ||
                style == Style.Cpp.StringEol || style == Style.Cpp.StringEol + Preprocessor ||
                style == Style.Cpp.Verbatim || style == Style.Cpp.Verbatim + Preprocessor)
            {
                Type stringType = typeof(string);
                string summary = stringType.GetDocCommentForToolTip();
                return $"{stringType.GetObjectType()} - {stringType.Namespace}.{stringType.Name}{summary}";
            }
            if (style == Style.Cpp.Character || style == Style.Cpp.Character + Preprocessor)
            {
                Type charType = typeof(char);
                string summary = charType.GetDocCommentForToolTip();
                return $"{charType.GetObjectType()} - {charType.Namespace}.{charType.Name}{summary}";
            }
            if (style == Style.Cpp.Number || style == Style.Cpp.Number + Preprocessor)
            {
                Type numType = GetNumberType(position);
                if (numType == null)
                {
                    return string.Empty;
                }
                string summary = numType.GetDocCommentForToolTip();
                return $"{numType.GetObjectType()} - {numType.Namespace}.{numType.Name}{summary}";
            }

            string lastWords = GetLastWords(position);
            if (lastWords.Length == 0)
            {
                return string.Empty;
            }

            ParseVariables(position);

            Type type;
            if (Intelli.Variables.ContainsKey(lastWords))
            {
                type = Intelli.Variables[lastWords];
                string typeName = type.GetDisplayName();

                return $"{typeName} - {lastWords}\nLocal Variable";
            }

            if (Intelli.Parameters.ContainsKey(lastWords))
            {
                type = Intelli.Parameters[lastWords];
                string typeName = type.GetDisplayName();

                return $"{typeName} - {lastWords}\nParameter";
            }

            if (Intelli.AllTypes.ContainsKey(lastWords))
            {
                type = Intelli.AllTypes[lastWords];

                if (type.IsClass || type.IsValueType)
                {
                    int newPos = this.WordStartPosition(position) - 1;
                    int parenPos = this.WordEndPosition(position);
                    if (newPos > InvalidPosition && this.GetWordFromPosition(newPos).Equals("new", StringComparison.Ordinal) && this.GetCharAt(parenPos).Equals('('))
                    {
                        // Constructor
                        ConstructorInfo[] ctors = type.GetConstructors();

                        if (type.IsValueType && this.GetCharAt(parenPos + 1).Equals(')'))
                        {
                            string overloads = (ctors.Length > 0) ? $" (+ {ctors.Length} overloads)" : string.Empty;
                            string summary = type.GetDocCommentForToolTip();
                            return $"{type.Name}.{type.Name}(){overloads}\nConstructor{summary}";
                        }

                        if (ctors.Length > 0)
                        {
                            int otherOverloads = type.IsValueType ? ctors.Length : ctors.Length - 1;
                            string overloads1 = (otherOverloads > 0) ? $" (+ {otherOverloads} overloads)" : string.Empty;

                            ConstructorInfo constructor = GetOverload(ctors, position);
                            string summary = constructor.GetDocCommentForToolTip();
                            return $"{constructor.DeclaringType.Name}.{type.Name}({constructor.Params()}){overloads1}\nConstructor{summary}";
                        }
                    }
                }

                string typeName = type.GetObjectType();

                string name = (type.IsGenericType) ? type.GetGenericName() : type.Name;
                string typeSummary = type.GetDocCommentForToolTip();

                return $"{typeName} - {type.Namespace}.{name}{typeSummary}";
            }

            if (Intelli.UserDefinedTypes.ContainsKey(lastWords))
            {
                type = Intelli.UserDefinedTypes[lastWords];
                string typeName = type.GetObjectType();

                string name = (type.IsGenericType) ? type.GetGenericName() : type.Name;
                string summary = type.GetDocCommentForToolTip();

                return $"{typeName} - {type.DeclaringType.Name}.{name}{summary}";
            }

            MemberInfo memberInfo = GetMember(position, out int length);
            if (memberInfo == null)
            {
                return string.Empty;
            }

            Type declaringType = memberInfo.DeclaringType;
            string precedingType = declaringType.GetDisplayName();
            string returnType;

            switch (memberInfo.MemberType)
            {
                case MemberTypes.Property:
                    PropertyInfo property = (PropertyInfo)memberInfo;
                    returnType = property.PropertyType.GetDisplayName();
                    string getSet = property.GetterSetter();
                    string propSummary = property.GetDocCommentForToolTip();

                    return $"{returnType} - {precedingType}.{property.Name}{getSet}\n{property.MemberType}{propSummary}";
                case MemberTypes.Method:
                    if (declaringType == Intelli.UserScript)
                    {
                        string member = this.GetWordFromPosition(position);
                        MemberInfo[] members = declaringType.GetMember(member, userScriptBindingFlags);
                        length = members.Length;
                        if (length == 0)
                        {
                            return string.Empty;
                        }

                        memberInfo = this.GetOverload(members.OfType<MethodInfo>(), position);
                    }

                    MethodInfo method = (MethodInfo)memberInfo;

                    string ext = string.Empty;
                    if (method.IsOrHasExtension())
                    {
                        precedingType = method.ExtendingType().GetDisplayName();
                        ext = "Extension ";
                    }

                    string genericArgs = string.Empty;
                    string genericConstraints = string.Empty;

                    if (method.IsGenericMethod)
                    {
                        Type[] args = method.GetGenericArguments();
                        genericArgs = $"<{args.Select(t => t.GetDisplayName()).Join(", ")}>";

                        if (method.IsGenericMethodDefinition)
                        {
                            string constraints = args.GetConstraints().Join("\r\n    ");
                            if (constraints.Length > 0)
                            {
                                genericConstraints = "\r\n    " + constraints;
                            }
                        }
                    }

                    returnType = method.ReturnType.GetDisplayName();
                    string nullable = method.IsNullable() ? "?" : string.Empty;
                    string byRef = method.ReturnType.IsByRef ? "ref " : string.Empty;
                    string overloads = (length > 1) ? $" (+ {length - 1} overloads)" : string.Empty;
                    string methodSummary = method.GetDocCommentForToolTip();

                    return $"{byRef}{returnType}{nullable} - {precedingType}.{method.Name}{genericArgs}({method.Params()}){overloads}{genericConstraints}\n{ext}{method.MemberType}{methodSummary}";
                case MemberTypes.Field:
                    FieldInfo field = (FieldInfo)memberInfo;
                    returnType = field.FieldType.GetDisplayName();

                    string fieldTypeName;
                    string fieldValue;
                    if (!field.IsStatic)
                    {
                        fieldTypeName = "Field";
                        fieldValue = string.Empty;
                    }
                    else if (field.FieldType.IsEnum)
                    {
                        fieldTypeName = "Enum Value";
                        fieldValue = $" ({field.GetEnumValue()})";
                    }
                    else if (field.IsLiteral && !field.IsInitOnly)
                    {
                        fieldTypeName = "Constant";
                        fieldValue = $" ({field.GetConstValue()})";
                    }
                    else
                    {
                        fieldTypeName = "Field";
                        string value = field.GetValue(null)?.ToString();
                        fieldValue = (value?.Length > 0) ? $" ( {value} )" : string.Empty;
                    }

                    string fieldSummary = field.GetDocCommentForToolTip();

                    return $"{returnType} - {precedingType}.{field.Name}{fieldValue}\n{fieldTypeName}{fieldSummary}";
                case MemberTypes.Event:
                    EventInfo eventInfo = (EventInfo)memberInfo;
                    returnType = eventInfo.EventHandlerType.GetDisplayName();
                    string eventSummary = eventInfo.GetDocCommentForToolTip();

                    return $"{returnType} - {precedingType}.{eventInfo.Name}\n{eventInfo.MemberType}{eventSummary}";
                case MemberTypes.NestedType:
                    type = (Type)memberInfo;
                    string typeName = type.GetObjectType();

                    string name = (type.IsGenericType) ? type.GetGenericName() : type.Name;
                    string nestedSummary = type.GetDocCommentForToolTip();

                    return $"{typeName} - {type.Namespace}.{precedingType}.{name}\nNested Type{nestedSummary}";
            }

            return string.Empty;
        }

        private string GetIntelliTipXaml(int position)
        {
            int style = this.GetStyleAt(position);
            if (style != Style.Xml.Tag && style != Style.Xml.Attribute)
            {
                return string.Empty;
            }

            Type type = GetXamlTag(position);
            if (type == null)
            {
                return string.Empty;
            }

            if (style == Style.Xml.Tag)
            {
                string typeName = type.GetObjectType();

                return $"{typeName} - {type.Namespace}.{type.Name}";
            }
            else if (style == Style.Xml.Attribute)
            {
                string attribute = this.GetWordFromPosition(position);
                PropertyInfo property = type.GetProperty(attribute, BindingFlags.Instance | BindingFlags.Public);
                if (property == null)
                {
                    return string.Empty;
                }

                string returnType = property.PropertyType.GetDisplayName();

                return $"{returnType} - {property.DeclaringType.Name}.{property.Name}";
            }

            return string.Empty;
        }

        private string GetGenericArgs(int position)
        {
            int openPos = this.WordEndPosition(position);
            if (this.GetCharAt(openPos) != '<')
            {
                return string.Empty;
            }

            int closePos = this.BraceMatch(openPos);
            if (closePos == InvalidPosition || closePos <= openPos + 1)
            {
                return string.Empty;
            }

            return this.GetTextRange(openPos + 1, closePos - openPos - 1);
        }

        private T GetOverload<T>(IEnumerable<T> mi, int position)
            where T : MethodBase
        {
            T defaultOverload = mi.First();

            bool isExplicitGeneric = false;
            string genericArgs = null;
            int paramStart = this.WordEndPosition(position);
            char openBrace = this.GetCharAt(paramStart);
            if (openBrace == '<')
            {
                isExplicitGeneric = true;
                genericArgs = GetGenericArgs(position);
                paramStart = this.BraceMatch(paramStart) + 1;
                openBrace = this.GetCharAt(paramStart);
            }

            if (isExplicitGeneric && defaultOverload is MethodInfo defaultMethod && defaultMethod.IsGenericMethodDefinition)
            {
                defaultOverload = defaultMethod.MakeGenericMethod(genericArgs) as T;
            }

            int miCount = mi.Count();

            if (miCount == 1 && !(defaultOverload is MethodInfo onlyMethod && onlyMethod.IsGenericMethodDefinition))
            {
                return defaultOverload;
            }

            if (openBrace != '(')
            {
                return defaultOverload;
            }

            int paramEnd = this.BraceMatch(paramStart);
            if (paramEnd == InvalidPosition)
            {
                return defaultOverload;
            }
            paramStart++;

            string[] paramWords = this.GetTextRange(paramStart, paramEnd - paramStart).Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            Tuple<int, int> oldRange = new Tuple<int, int>(this.TargetStart, this.TargetEnd);
            this.SearchFlags = SearchFlags.MatchCase;
            this.SetTargetRange(paramStart, paramEnd);
            List<Type> paramTypes = new List<Type>(paramWords.Length);
            foreach (string paramName in paramWords)
            {
                int paramPos = InvalidPosition;
                if (this.SearchInTarget(paramName) != InvalidPosition)
                {
                    paramPos = this.TargetEnd;
                    this.SetTargetRange(this.TargetEnd, paramEnd);
                }

                if (paramPos == InvalidPosition)
                {
                    this.SetTargetRange(oldRange.Item1, oldRange.Item2);
                    return defaultOverload;
                }

                Type paramType = GetReturnType(paramPos);
                if (paramType == null)
                {
                    this.SetTargetRange(oldRange.Item1, oldRange.Item2);
                    return defaultOverload;
                }

                if (!paramType.IsByRef && (paramName.StartsWith("ref", StringComparison.Ordinal) || paramName.StartsWith("out", StringComparison.Ordinal)))
                {
                    paramType = paramType.MakeByRefType();
                }

                paramTypes.Add(paramType);
            }
            this.SetTargetRange(oldRange.Item1, oldRange.Item2);

            for (int i = 0; i < miCount; i++)
            {
                T method = mi.ElementAt(i);
                if (method.IsGenericMethod)
                {
                    if (!isExplicitGeneric)
                    {
                        // Inferred Generic ?
                        ParameterInfo[] paramDefinitions = method.GetParameters();
                        if (paramDefinitions.Length != paramTypes.Count)
                        {
                            continue;
                        }

                        Type[] argDefinitions = method.GetGenericArguments();
                        List<Type> argTypes = new List<Type>();
                        bool nameMatch = false;
                        foreach (Type type in argDefinitions)
                        {
                            nameMatch = false;
                            string typeName = type.Name;
                            for (int i2 = 0; i2 < paramDefinitions.Length; i2++)
                            {
                                if (paramDefinitions[i2].ParameterType.Name.Equals(typeName, StringComparison.Ordinal))
                                {
                                    argTypes.Add(paramTypes[i2]);
                                    nameMatch = true;
                                    break;
                                }
                            }

                            if (!nameMatch)
                            {
                                break;
                            }
                        }

                        if (!nameMatch)
                        {
                            continue;
                        }

                        if (method.IsGenericMethodDefinition && method is MethodInfo methodInfo2)
                        {
                            method = methodInfo2.MakeGenericMethod(argTypes.ToArray()) as T;
                        }
                    }
                    else if (method.IsGenericMethodDefinition && method is MethodInfo methodInfo)
                    {
                        method = methodInfo.MakeGenericMethod(genericArgs) as T;

                        if (method.IsGenericMethodDefinition)
                        {
                            continue;
                        }
                    }
                }

                ParameterInfo[] paramInfo = method.GetParameters();
                if (method.IsOrHasExtension())
                {
                    paramInfo = paramInfo.Skip(1).ToArray();
                }

                if (paramInfo.Length != paramTypes.Count)
                {
                    continue;
                }

                bool match = true;
                for (int j = 0; j < paramInfo.Length; j++)
                {
                    ParameterInfo param = paramInfo[j];
                    Type paramType = paramTypes[j];
                    if (!(paramType == param.ParameterType || paramType.IsSubclassOf(param.ParameterType)))
                    {
                        match = false;
                        break;
                    }
                }

                if (!match)
                {
                    continue;
                }

                return method;
            }

            return defaultOverload;
        }

        private Type GetReturnType(int position)
        {
            int style = this.GetStyleAt(position - 1);
            if (style == Style.Cpp.Comment || style == Style.Cpp.Comment + Preprocessor ||
                style == Style.Cpp.CommentLine || style == Style.Cpp.CommentLine + Preprocessor ||
                style == Style.Cpp.Preprocessor || style == Style.Cpp.Preprocessor + Preprocessor ||
                style == Style.Cpp.Default || style == Style.Cpp.Default + Preprocessor)
            {
                return null;
            }

            if (style == Style.Cpp.String || style == Style.Cpp.String + Preprocessor ||
                style == Style.Cpp.StringEol || style == Style.Cpp.StringEol + Preprocessor ||
                style == Style.Cpp.Verbatim || style == Style.Cpp.Verbatim + Preprocessor)
            {
                return typeof(string);
            }

            if (style == Style.Cpp.Character || style == Style.Cpp.Character + Preprocessor)
            {
                return typeof(char);
            }

            if (style == Style.Cpp.Number || style == Style.Cpp.Number + Preprocessor)
            {
                return GetNumberType(position - 1);
            }

            string lastWords = GetLastWords(position);
            if (lastWords.Length == 0)
            {
                return null;
            }

            if (Intelli.Variables.ContainsKey(lastWords))
            {
                return Intelli.Variables[lastWords];
            }

            if (Intelli.Parameters.ContainsKey(lastWords))
            {
                return Intelli.Parameters[lastWords];
            }

            if (Intelli.AllTypes.ContainsKey(lastWords))
            {
                return Intelli.AllTypes[lastWords];
            }

            if (Intelli.UserDefinedTypes.ContainsKey(lastWords))
            {
                return Intelli.UserDefinedTypes[lastWords];
            }

            MemberInfo memberInfo = GetMember(position, out _);
            if (memberInfo == null)
            {
                return null;
            }

            return memberInfo.GetReturnType();
        }

        private Type GetDeclaringType(int position)
        {
            int style = this.GetStyleAt(position);
            if (style != Style.Cpp.Word && style != Style.Cpp.Word + Preprocessor &&
                style != Style.Cpp.Word2 && style != Style.Cpp.Word2 + Preprocessor &&
                style != Substyle.Enum && style != Substyle.Enum + Preprocessor &&
                style != Substyle.Interface && style != Substyle.Interface + Preprocessor &&
                style != Substyle.Struct && style != Substyle.Struct + Preprocessor &&
                style != Style.Cpp.Identifier && style != Style.Cpp.Identifier + Preprocessor &&
                style != Substyle.Method && style != Substyle.Method + Preprocessor &&
                style != Substyle.ParamAndVar && style != Substyle.ParamAndVar + Preprocessor)
            {
                return null;
            }

            string lastWords = GetLastWords(position);
            if (lastWords.Length == 0)
            {
                return null;
            }

            // We only care about member of types; not types
            if (Intelli.Variables.ContainsKey(lastWords) || Intelli.Parameters.ContainsKey(lastWords) ||
                Intelli.AllTypes.ContainsKey(lastWords) || Intelli.UserDefinedTypes.ContainsKey(lastWords))
            {
                return null;
            }

            MemberInfo memberInfo = GetMember(position, out _);
            if (memberInfo == null)
            {
                return null;
            }

            return memberInfo.DeclaringType;
        }

        private MemberInfo GetMember(int position, out int length)
        {
            length = 0;
            string lastWords = GetLastWords(position);
            string[] tokens = lastWords.Split('.', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (tokens.Length == 0)
            {
                return null;
            }

            int[] tokenPos = GetLastWordsPos(position);

            if (tokens.Length != tokenPos.Length)
            {
                return null;
            }

            Type type;
            bool isStatic = false;
            if (Intelli.Variables.ContainsKey(tokens[0]))
            {
                type = Intelli.Variables[tokens[0]];
            }
            else if (Intelli.Parameters.ContainsKey(tokens[0]))
            {
                type = Intelli.Parameters[tokens[0]];
            }
            else if (Intelli.AllTypes.ContainsKey(tokens[0]))
            {
                type = Intelli.AllTypes[tokens[0]];
                isStatic = true;
            }
            else if (Intelli.UserDefinedTypes.ContainsKey(tokens[0]))
            {
                type = Intelli.UserDefinedTypes[tokens[0]];
                isStatic = true;
            }
            else if (Intelli.UserScript.Contains(tokens[0], false))
            {
                type = Intelli.UserScript;
                tokens = tokens.Prepend(type.Name).ToArray();
                tokenPos = tokenPos.Prepend(InvalidPosition).ToArray();
            }
            else
            {
                return null;
            }

            for (int i = 1; i < tokens.Length; i++)
            {
                // get the member information for this token
                BindingFlags bindingFlags = (i == 1 && type == Intelli.UserScript) ?
                    BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic :
                    (isStatic ? BindingFlags.Static : BindingFlags.Instance) | BindingFlags.Public;

                MemberInfo[] mi = type.GetMember(tokens[i], bindingFlags);

                if (mi.Length == 0 || mi[0].MemberType == MemberTypes.Method)
                {
                    IEnumerable<MemberInfo> ext = type.GetExtensionMethod(tokens[i]);
                    mi = mi.Concat(ext).ToArray();
                }

                if (mi.Length == 0)
                {
                    return null;
                }

                if (i == tokens.Length - 1)
                {
                    // We're at the last iteration. Return the MemberInfo.
                    length = mi.Length;
                    return (mi[0].MemberType == MemberTypes.Method) ?
                        GetOverload(mi.OfType<MethodInfo>(), tokenPos[i]) :
                        mi[0];
                }

                type = (mi[0].MemberType == MemberTypes.Method) ?
                    GetOverload(mi.OfType<MethodInfo>(), tokenPos[i]).ReturnType :
                    mi[0].GetReturnType();

                if (type == null)
                {
                    return null;
                }

                isStatic = (mi[0].MemberType == MemberTypes.NestedType);
            }

            return null;
        }

        private Type GetXamlTag(int position)
        {
            int pos = position;
            bool foundTagStart = false;
            while (pos != InvalidPosition)
            {
                char charAtPos = this.GetCharAt(pos);
                if (charAtPos == '<')
                {
                    foundTagStart = true;
                    pos++;
                    break;
                }
                else if (charAtPos == '>')
                {
                    return null;
                }

                pos--;
            }

            if (!foundTagStart || this.GetStyleAt(pos) != Style.Xml.Tag)
            {
                return null;
            }

            string tagName = this.GetWordFromPosition(pos);
            if (!Intelli.XamlAutoCompleteTypes.TryGetValue(tagName, out Type tag) || tag == null)
            {
                return null;
            }

            return tag;
        }

        private void OpenDefinitionTab(MemberInfo memberInfo)
        {
            Type type;
            switch (memberInfo.MemberType)
            {
                case MemberTypes.TypeInfo:
                    type = (Type)memberInfo;
                    break;
                default:
                    type = memberInfo.DeclaringType;
                    if (type.IsNested)
                    {
                        type = type.DeclaringType;
                    }
                    break;
            }

            if (type.IsConstructedGenericType)
            {
                type = type.GetGenericTypeDefinition();
            }

            string defRef = DefinitionGenerator.Generate(type);
            string name = type.GetDisplayNameWithExclusion(type);

            OnDefTabNeeded(name, type.Namespace + "." + name);
            OnBuildNeeded();
            this.Text = defRef;
            this.ReadOnly = true;

            this.TargetWholeDocument();
            this.SearchFlags = SearchFlags.MatchCase | SearchFlags.WholeWord;
            string wordToFind = memberInfo.Name;

            if (this.SearchInTarget(wordToFind) != InvalidPosition)
            {
                this.SetSel(this.TargetStart, this.TargetEnd);
                this.delayedOperation |= DelayedOperation.ScrollCaret;
            }

            this.EmptyUndoBuffer();
            this.SetSavePoint();
        }

        internal void GoToDefinition(bool msDocs)
        {
            bool success = false;

            switch (this.Lexer)
            {
                case Lexer.Cpp:
                    success = GoToDefinitionCSharp(msDocs);
                    break;
                case Lexer.Xml:
                    success = GoToDefinitionXaml();
                    break;
            }

            if (!success)
            {
                FlexibleMessageBox.Show("Cannot navigate to the symbol under the caret.", "CodeLab", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private bool GoToDefinitionXaml()
        {
            int position = this.WordStartPosition(this.CurrentPosition);
            int style = this.GetStyleAt(position);

            if (style == Style.Xml.Attribute)
            {
                Type tag = GetXamlTag(position);
                if (tag == null)
                {
                    return false;
                }

                string attribute = this.GetWordFromPosition(position);
                PropertyInfo property = tag.GetProperty(attribute, BindingFlags.Instance | BindingFlags.Public);
                if (property == null)
                {
                    return false;
                }

                string fullName = $"{property.DeclaringType.FullName}.{property.Name}";
                OpenMsDocs(fullName);
                return true;
            }

            if (style == Style.Xml.Tag)
            {
                Type tag = GetXamlTag(position);
                if (tag == null)
                {
                    return false;
                }

                OpenMsDocs(tag.FullName);
                return true;
            }

            return false;
        }

        private bool GoToDefinitionCSharp(bool apiDocs)
        {
            int position = this.WordStartPosition(this.CurrentPosition);

            int style = this.GetStyleAt(position);
            if (style == Style.Cpp.Comment || style == Style.Cpp.Comment + Preprocessor ||
                style == Style.Cpp.CommentLine || style == Style.Cpp.CommentLine + Preprocessor ||
                style == Style.Cpp.Preprocessor || style == Style.Cpp.Preprocessor + Preprocessor ||
                style == Style.Cpp.Operator || style == Style.Cpp.Operator + Preprocessor ||
                style == Style.Cpp.Default || style == Style.Cpp.Default + Preprocessor)
            {
                return false;
            }

            if (style == Style.Cpp.String || style == Style.Cpp.String + Preprocessor ||
                style == Style.Cpp.StringEol || style == Style.Cpp.StringEol + Preprocessor ||
                style == Style.Cpp.Verbatim || style == Style.Cpp.Verbatim + Preprocessor)
            {
                if (apiDocs)
                {
                    OpenMsDocs(typeof(string).FullName);
                }
                else
                {
                    OpenDefinitionTab(typeof(string));
                }

                return true;
            }

            if (style == Style.Cpp.Character || style == Style.Cpp.Character + Preprocessor)
            {
                if (apiDocs)
                {
                    OpenMsDocs(typeof(char).FullName);
                }
                else
                {
                    OpenDefinitionTab(typeof(char));
                }

                return true;
            }

            if (style == Style.Cpp.Number || style == Style.Cpp.Number + Preprocessor)
            {
                Type numType = GetNumberType(position);
                if (numType == null)
                {
                    return false;
                }

                if (apiDocs)
                {
                    OpenMsDocs(numType.FullName);
                }
                else
                {
                    OpenDefinitionTab(numType);
                }

                return true;
            }

            string lastWords = GetLastWords(position);
            if (lastWords.Length == 0)
            {
                return false;
            }

            if (!apiDocs && (Intelli.Variables.ContainsKey(lastWords) || Intelli.Parameters.ContainsKey(lastWords)) && Intelli.VarPos.ContainsKey(lastWords))
            {
                this.SelectionStart = Intelli.VarPos[lastWords];
                this.SelectionEnd = this.WordEndPosition(Intelli.VarPos[lastWords]);
                this.ScrollCaret();
                return true;
            }

            if (!apiDocs && Intelli.UserDefinedTypes.ContainsKey(lastWords))
            {
                Type t = Intelli.UserDefinedTypes[lastWords];

                string baseType = t.GetObjectType();

                bool found = false;
                this.TargetWholeDocument();
                this.SearchFlags = SearchFlags.WholeWord;
                while (this.SearchInTarget(baseType) != InvalidPosition)
                {
                    int typePos = this.TargetEnd;
                    this.SetTargetRange(this.TargetEnd, this.TextLength);

                    // Skip over white space
                    while (char.IsWhiteSpace(this.GetCharAt(typePos)) && typePos <= this.TextLength)
                    {
                        typePos++;
                    }

                    if (this.GetWordFromPosition(typePos) != lastWords)
                    {
                        continue;
                    }

                    int style2 = this.GetStyleAt(typePos);
                    if (style2 != Style.Cpp.Word && style2 != Style.Cpp.Word + Preprocessor &&
                        style2 != Style.Cpp.Word2 && style2 != Style.Cpp.Word2 + Preprocessor &&
                        style2 != Substyle.Enum && style2 != Substyle.Enum + Preprocessor &&
                        style2 != Substyle.Interface && style2 != Substyle.Interface + Preprocessor &&
                        style2 != Substyle.Struct && style2 != Substyle.Struct + Preprocessor)
                    {
                        continue;
                    }

                    found = true;
                    this.SelectionStart = typePos;
                    this.SelectionEnd = this.WordEndPosition(typePos);
                    this.ScrollCaret();

                    break;
                }

                return found;
            }

            if (Intelli.AllTypes.ContainsKey(lastWords))
            {
                Type t = Intelli.AllTypes[lastWords];

                if (apiDocs)
                {
                    string typeName = (t.IsGenericType) ? t.Name.Replace("`", "-") : t.Name;
                    string fullName = $"{t.Namespace}.{typeName}";

                    if (t.Namespace.Equals(nameof(PdnCodeLab), StringComparison.Ordinal))
                    {
                        return false; // UserScript
                    }
                    else if (t.Namespace.StartsWith("PaintDotNet", StringComparison.Ordinal))
                    {
                        OpenPdnDocs(fullName);
                    }
                    else
                    {
                        OpenMsDocs(fullName);
                    }
                }
                else
                {
                    OpenDefinitionTab(t);
                }

                return true;
            }

            MemberInfo memberInfo = GetMember(position, out _);
            if (memberInfo == null)
            {
                return false;
            }

            if (!apiDocs && memberInfo.DeclaringType == Intelli.UserScript)
            {
                string returnType = memberInfo.GetReturnType()?.GetDisplayName();

                if (returnType?.Length == 0)
                {
                    return false;
                }

                bool found = false;
                this.TargetWholeDocument();
                this.SearchFlags = SearchFlags.MatchCase | SearchFlags.WholeWord;
                while (this.SearchInTarget(lastWords) != InvalidPosition)
                {
                    int typePos = this.TargetStart - 1;
                    int memberPos = this.TargetStart;
                    this.SetTargetRange(this.TargetEnd, this.TextLength);

                    // Skip over white space
                    while (char.IsWhiteSpace(this.GetCharAt(typePos)) && typePos > InvalidPosition)
                    {
                        typePos--;
                    }

                    string foundType = this.GetWordFromPosition(typePos);
                    Type t;
                    if (Intelli.AllTypes.ContainsKey(foundType))
                    {
                        t = Intelli.AllTypes[foundType];
                    }
                    else if (Intelli.UserDefinedTypes.ContainsKey(foundType))
                    {
                        t = Intelli.UserDefinedTypes[foundType];
                    }
                    else if (foundType == typeof(void).GetDisplayName())
                    {
                        t = typeof(void);
                    }
                    else
                    {
                        continue;
                    }

                    if (t.GetDisplayName() != returnType)
                    {
                        continue;
                    }

                    // Don't parse variables in comments
                    int style2 = this.GetStyleAt(typePos);
                    if (style2 != Style.Cpp.Word && style2 != Style.Cpp.Word + Preprocessor &&
                        style2 != Style.Cpp.Word2 && style2 != Style.Cpp.Word2 + Preprocessor &&
                        style2 != Substyle.Enum && style2 != Substyle.Enum + Preprocessor &&
                        style2 != Substyle.Interface && style2 != Substyle.Interface + Preprocessor &&
                        style2 != Substyle.Struct && style2 != Substyle.Struct + Preprocessor &&
                        style2 != Style.Cpp.Identifier && style2 != Style.Cpp.Identifier + Preprocessor &&
                        style2 != Substyle.Method && style2 != Substyle.Method + Preprocessor &&
                        style2 != Substyle.ParamAndVar && style2 != Substyle.ParamAndVar + Preprocessor)
                    {
                        continue;
                    }

                    found = true;
                    this.SelectionStart = memberPos;
                    this.SelectionEnd = this.WordEndPosition(memberPos);
                    this.ScrollCaret();

                    break;
                }

                return found;
            }

            if (apiDocs)
            {
                Type declaringType = memberInfo.DeclaringType;
                string typeName = (declaringType.IsGenericType) ? declaringType.Name.Replace("`", "-") : declaringType.Name;
                string fullName = $"{declaringType.Namespace}.{typeName}.{memberInfo.Name}";

                if (declaringType.Namespace.Equals(nameof(PdnCodeLab), StringComparison.Ordinal))
                {
                    return false; // UserScript
                }
                else if (declaringType.Namespace.StartsWith("PaintDotNet", StringComparison.Ordinal))
                {
                    OpenPdnDocs(fullName);
                }
                else
                {
                    OpenMsDocs(fullName);
                }
            }
            else
            {
                OpenDefinitionTab(memberInfo);
            }

            return true;
        }

        private void OpenMsDocs(string fullName)
        {
            string dotnetVer = Environment.Version.ToString(2);
            UIUtil.LaunchUrl(this, $"https://learn.microsoft.com/dotnet/api/{fullName}?view=net-{dotnetVer}");
        }

        private void OpenPdnDocs(string fullName)
        {
            UIUtil.LaunchUrl(this, $"https://paintdotnet.github.io/apidocs/api/{fullName}.html");
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            if (iBox.Visible && iBox.MouseOver && e is HandledMouseEventArgs args)
            {
                args.Handled = true;
                iBox.ScrollItems(e.Delta);
            }

            base.OnMouseWheel(e);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (!iBox.Visible)
            {
                if (e.KeyCode == Keys.F12)
                {
                    GoToDefinition(false);
                }
                else if (e.KeyCode == Keys.F1)
                {
                    GoToDefinition(true);
                }
                else if (this.ReadOnly)
                {
                    // no-op
                }
                else if (e.KeyCode == Keys.Escape)
                {
                    // Clear indicator for variable renaming
                    ClearRenaming();
                }
                else if (e.KeyCode == Keys.Tab)
                {
                    string prevWord = this.GetWordFromPosition(this.CurrentPosition);
                    int prevWordStartPos = this.WordStartPosition(this.CurrentPosition);
                    int prevWordEndPos = this.WordEndPosition(this.CurrentPosition);

                    if (this.GetCharAt(prevWordStartPos - 1).Equals('#'))
                    {
                        prevWord = "#" + prevWord;
                        prevWordStartPos--;
                    }

                    if (Intelli.Snippets.ContainsKey(prevWord) && prevWordEndPos == this.CurrentPosition)
                    {
                        e.Handled = true;
                        e.SuppressKeyPress = true;

                        this.BeginUndoAction();

                        this.DeleteRange(prevWordStartPos, prevWordEndPos - prevWordStartPos);

                        // Insert the snippet
                        string indent = new string(' ', this.Lines[this.CurrentLine].Indentation);
                        int startPos = this.CurrentPosition;
                        string[] lines = Intelli.Snippets[prevWord].Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                        for (int i = 0; i < lines.Length; i++)
                        {
                            string line = (i == lines.Length - 1) ? lines[i] : lines[i] + "\r\n" + indent;
                            this.AddText(line);
                        }
                        int endPos = this.CurrentPosition;

                        // move caret/selection to the correct location
                        this.SearchFlags = SearchFlags.None;
                        this.SetTargetRange(startPos, endPos);
                        if (this.SearchInTarget("$") != InvalidPosition)
                        {
                            this.DeleteRange(this.TargetStart, 1);
                            this.SelectionStart = this.WordStartPosition(this.TargetStart);
                            this.SelectionEnd = this.WordEndPosition(this.TargetStart);
                        }

                        this.EndUndoAction();
                        OnBuildNeeded();
                    }
                }
                else if (e.Control && e.KeyCode == Keys.J)
                {
                    int startPos = this.WordStartPosition(this.CurrentPosition);
                    if (this.GetCharAt(startPos - 1).Equals('.'))
                    {
                        MemberIntelliBox(startPos - 1);
                        if (iBox.Visible)
                        {
                            this.SetEmptySelection(startPos);
                            string memberName = this.GetWordFromPosition(this.CurrentPosition);
                            iBox.FindAndSelect(memberName);
                        }
                    }
                    else
                    {
                        NonMemberIntelliBox(this.CurrentPosition);
                    }
                }
                else if (e.Control && e.KeyCode == Keys.OemCloseBrackets)
                {
                    int bracePos1 = InvalidPosition;
                    int caretPos = this.CurrentPosition;

                    if (caretPos > 0 && (this.GetCharAt(caretPos - 1).IsBrace(true) || this.GetCharAt(caretPos - 1).IsBrace(false)))
                    {
                        bracePos1 = caretPos - 1;
                    }
                    else if (this.GetCharAt(caretPos).IsBrace(true) || this.GetCharAt(caretPos).IsBrace(false))
                    {
                        bracePos1 = caretPos;
                    }

                    int style = this.GetStyleAt(bracePos1);

                    if (bracePos1 > InvalidPosition && (style == Style.Cpp.Operator || style == Style.Cpp.Operator + Preprocessor))
                    {
                        int bracePos2 = this.BraceMatch(bracePos1);
                        if (bracePos2 != InvalidPosition)
                        {
                            if (this.GetCharAt(bracePos2).IsBrace(false))
                            {
                                bracePos2++;
                            }

                            this.SetEmptySelection(bracePos2);
                            this.ScrollCaret();
                        }
                    }
                }
                else if (e.Alt && e.KeyCode == Keys.Up)
                {
                    this.ExecuteCmd(Command.MoveSelectedLinesUp);
                }
                else if (e.Alt && e.KeyCode == Keys.Down)
                {
                    this.ExecuteCmd(Command.MoveSelectedLinesDown);
                }
                else if (e.KeyCode == Keys.Back)
                {
                    if (this.autoBraceOpenPos != InvalidPosition &&
                        this.CurrentPosition == this.autoBraceOpenPos + 1 &&
                        this.BraceMatch(this.autoBraceOpenPos) - 1 == this.autoBraceOpenPos)
                    {
                        e.SuppressKeyPress = true;
                        e.Handled = true;

                        this.DeleteRange(this.CurrentPosition - 1, 2);
                    }
                }
            }
            else if (this.ReadOnly)
            {
                // no-op
            }
            else if (e.Alt && e.KeyCode == Keys.L)
            {
                e.SuppressKeyPress = true;
                e.Handled = true;
                iBox.Filter(IntelliType.Variable);
            }
            else if (e.Alt && e.KeyCode == Keys.O)
            {
                e.SuppressKeyPress = true;
                e.Handled = true;
                iBox.Filter(IntelliType.Constant);
            }
            else if (e.Alt && e.KeyCode == Keys.P)
            {
                e.SuppressKeyPress = true;
                e.Handled = true;
                iBox.Filter(IntelliType.Property);
            }
            else if (e.Alt && e.KeyCode == Keys.F)
            {
                e.SuppressKeyPress = true;
                e.Handled = true;
                iBox.Filter(IntelliType.Field);
            }
            else if (e.Alt && e.KeyCode == Keys.M)
            {
                e.SuppressKeyPress = true;
                e.Handled = true;
                iBox.Filter(IntelliType.Method);
            }
            else if (e.Alt && e.KeyCode == Keys.C)
            {
                e.SuppressKeyPress = true;
                e.Handled = true;
                iBox.Filter(IntelliType.Class);
            }
            else if (e.Alt && e.KeyCode == Keys.S)
            {
                e.SuppressKeyPress = true;
                e.Handled = true;
                iBox.Filter(IntelliType.Struct);
            }
            else if (e.Alt && e.KeyCode == Keys.E)
            {
                e.SuppressKeyPress = true;
                e.Handled = true;
                iBox.Filter(IntelliType.Enum);
            }
            else if (e.Alt && e.KeyCode == Keys.K)
            {
                e.SuppressKeyPress = true;
                e.Handled = true;
                iBox.Filter(IntelliType.Keyword);
            }
            else if (e.Alt && e.KeyCode == Keys.T)
            {
                e.SuppressKeyPress = true;
                e.Handled = true;
                iBox.Filter(IntelliType.Snippet);
            }
            else if (e.Alt && e.KeyCode == Keys.I)
            {
                e.SuppressKeyPress = true;
                e.Handled = true;
                iBox.Filter(IntelliType.Interface);
            }
            else if (e.Alt && e.KeyCode == Keys.D)
            {
                e.SuppressKeyPress = true;
                e.Handled = true;
                iBox.Filter(IntelliType.Delegate);
            }
            else if (e.Alt && e.KeyCode == Keys.V)
            {
                e.SuppressKeyPress = true;
                e.Handled = true;
                iBox.Filter(IntelliType.Event);
            }
            else if (e.KeyCode == Keys.OemPeriod)
            {
                if (iBox.AutoComplete)
                {
                    e.Handled = true;
                    ConfirmIntelliBox();
                }
                else
                {
                    iBox.Visible = false;
                }
            }
            else if (e.KeyCode == Keys.OemSemicolon)
            {
                if (iBox.AutoComplete)
                {
                    e.Handled = true;
                    ConfirmIntelliBox();
                }
                else
                {
                    iBox.Visible = false;
                }
            }
            else if (e.KeyCode == Keys.Oemcomma)
            {
                if (iBox.AutoComplete)
                {
                    e.Handled = true;
                    ConfirmIntelliBox();
                }
                else
                {
                    iBox.Visible = false;
                }
            }
            else if (e.KeyCode == Keys.Tab)
            {
                e.SuppressKeyPress = true;
                e.Handled = true;
                ConfirmIntelliBox();
            }
            else if (e.KeyCode == Keys.Space)
            {
                if (iBox.AutoComplete)
                {
                    ConfirmIntelliBox();
                }
                else
                {
                    iBox.Visible = false;
                }
            }
            else if (e.KeyCode == Keys.Back)
            {
                int wordStartPos = this.WordStartPosition(this.CurrentPosition - 1);
                if (wordStartPos != posAtIBox)
                {
                    iBox.Visible = false;
                }
                else if (e.Control)
                {
                    iBox.Filter(string.Empty);
                }
                else
                {
                    string charsEntered = this.GetTextRange(wordStartPos, this.CurrentPosition - 1 - wordStartPos);
                    iBox.Filter(charsEntered);
                }
            }
            else if (e.KeyCode == Keys.Left)
            {
                int wordStartPos = this.WordStartPosition(this.CurrentPosition - 1);
                if (wordStartPos != posAtIBox)
                {
                    iBox.Visible = false;
                }
            }
            else if (e.KeyCode == Keys.Right)
            {
                int wordStartPos = this.WordStartPosition(this.CurrentPosition + 1);
                if (wordStartPos != posAtIBox)
                {
                    iBox.Visible = false;
                }
            }
            else if (e.KeyCode == Keys.Up)
            {
                iBox.SelectPrev();

                e.Handled = true;
                this.Focus();
            }
            else if (e.KeyCode == Keys.Down)
            {
                iBox.SelectNext();

                e.Handled = true;
                this.Focus();
            }
            else if (e.KeyCode == Keys.PageDown)
            {
                iBox.PageDown();

                e.Handled = true;
                this.Focus();
            }
            else if (e.KeyCode == Keys.PageUp)
            {
                iBox.PageUp();

                e.Handled = true;
                this.Focus();
            }
            else if (e.KeyCode == Keys.End)
            {
                iBox.SelectLast();
                e.Handled = true;
                this.Focus();
            }
            else if (e.KeyCode == Keys.Home)
            {
                iBox.SelectFirst();
                e.Handled = true;
                this.Focus();
            }
            else if (e.KeyCode == Keys.Return)
            {
                if (iBox.AutoComplete)
                {
                    e.SuppressKeyPress = true;
                    e.Handled = true;
                    ConfirmIntelliBox();
                }
                else
                {
                    iBox.Visible = false;
                }
            }
            else if (e.KeyCode == Keys.Escape)
            {
                iBox.Visible = false;
            }

            base.OnKeyDown(e);
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            if (iBox.Visible && IntelliBox.IsConfirmationChar(e.KeyChar))
            {
                if (iBox.AutoComplete)
                {
                    ConfirmIntelliBox();
                }
                else
                {
                    iBox.Visible = false;
                }
            }

            base.OnKeyPress(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (iBox.Visible)
            {
                iBox.Visible = false;
            }

            if (lightBulbMenu.Visible)
            {
                lightBulbMenu.Hide();
            }

            if (intelliTip.Visible)
            {
                intelliTip.Hide(this);
                disableIntelliTipPos = this.CharPositionFromPointClose(e.X, e.Y);
            }

            this.suppressContextMenu = e.Button != MouseButtons.Right;

            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            this.suppressContextMenu = true;

            base.OnMouseUp(e);
        }

        private void MemberIntelliBox(int position)
        {
            int prevCharPos = position - 1;
            while (char.IsWhiteSpace(this.GetCharAt(prevCharPos)) && prevCharPos > 0)
            {
                prevCharPos--;
            }

            int style = this.GetStyleAt(prevCharPos);
            if (style != Style.Cpp.Word && style != Style.Cpp.Word + Preprocessor &&
                style != Style.Cpp.Word2 && style != Style.Cpp.Word2 + Preprocessor &&
                style != Substyle.Enum && style != Substyle.Enum + Preprocessor &&
                style != Substyle.Interface && style != Substyle.Interface + Preprocessor &&
                style != Substyle.Struct && style != Substyle.Struct + Preprocessor &&
                style != Style.Cpp.Identifier && style != Style.Cpp.Identifier + Preprocessor &&
                style != Substyle.Method && style != Substyle.Method + Preprocessor &&
                style != Substyle.ParamAndVar && style != Substyle.ParamAndVar + Preprocessor &&
                style != Style.Cpp.Operator && style != Style.Cpp.Operator + Preprocessor)
            {
                return;
            }

            Type type = GetReturnType(prevCharPos + 1);

            if (type == null || type == typeof(void))
            {
                return;
            }

            bool isStatic = GetIntelliType(prevCharPos) == IntelliType.Type;

            iBox.PopulateMembers(type, isStatic);

            ShowIntelliBox(position);
        }

        private void NonMemberIntelliBox(int position)
        {
            // Ensure there's no words immediately to the left and right
            if ((position > 0 && char.IsLetterOrDigit(this.GetCharAt(position - 1))) || char.IsLetterOrDigit(this.GetCharAt(position + 1)))
            {
                return;
            }

            int prevCharPos = position - 1;
            while (char.IsWhiteSpace(this.GetCharAt(prevCharPos)) && prevCharPos > 0)
            {
                prevCharPos--;
            }

            int style = this.GetStyleAt(position - 1);
            if (prevCharPos < this.Lines[this.LineFromPosition(position)].Position)
            {
                int prevStyle = this.GetStyleAt(prevCharPos);
                if (prevStyle == Style.Cpp.Comment || prevStyle == Style.Cpp.Comment + Preprocessor ||
                    prevStyle == Style.Cpp.Verbatim || prevStyle == Style.Cpp.Verbatim + Preprocessor)
                {
                    return;
                }
            }
            else if (style != Style.Cpp.Default && style != Style.Cpp.Default + Preprocessor &&
                     style != Style.Cpp.Operator && style != Style.Cpp.Operator + Preprocessor)
            {
                return;
            }

            if (this.GetCharAt(prevCharPos) == '>' || this.GetCharAt(prevCharPos) == ']')
            {
                prevCharPos = this.BraceMatch(prevCharPos) - 1;
            }

            IntelliType prevType = this.GetIntelliType(prevCharPos);
            string preWord = this.GetWordFromPosition(prevCharPos);
            if (prevType != IntelliType.None || preWord == "void" || preWord == "var")
            {
                return;
            }

            bool inClassRoot = IsInClassRoot(position);

            iBox.PopulateNonMembers(this.GetCharAt(position), inClassRoot);
            ShowIntelliBox(position);
        }

        private void ConstructorIntelliBox(int position)
        {
            int style = this.GetStyleAt(this.WordStartPosition(position));
            if (style != Style.Cpp.Word && style != Style.Cpp.Word + Preprocessor &&
                style != Style.Cpp.Word2 && style != Style.Cpp.Word2 + Preprocessor &&
                style != Substyle.Enum && style != Substyle.Enum + Preprocessor &&
                style != Substyle.Interface && style != Substyle.Interface + Preprocessor &&
                style != Substyle.Struct && style != Substyle.Struct + Preprocessor)
            {
                return;
            }

            if (GetIntelliType(position - 1) != IntelliType.Type)
            {
                return;
            }

            string word = this.GetWordFromPosition(position);
            if (!Intelli.AllTypes.ContainsKey(word) && !Intelli.UserDefinedTypes.ContainsKey(word))
            {
                return;
            }

            Type type = Intelli.AllTypes.ContainsKey(word) ? Intelli.AllTypes[word] : Intelli.UserDefinedTypes[word];
            if (type.IsEnum || type.IsInterface || !(type.IsClass || type.IsValueType))
            {
                return;
            }

            iBox.PopulateConstructors(type);
            ShowIntelliBox(position);
        }

        private void SuggestionIntelliBox(int position)
        {
            int style = this.GetStyleAt(position - 1);
            if (style != Style.Cpp.Word2 && style != Style.Cpp.Word2 + Preprocessor &&
                style != Substyle.Enum && style != Substyle.Enum + Preprocessor &&
                style != Substyle.Interface && style != Substyle.Interface + Preprocessor &&
                style != Substyle.Struct && style != Substyle.Struct + Preprocessor &&
                style != Style.Cpp.Operator && style != Style.Cpp.Operator + Preprocessor)
            {
                return;
            }

            Type type = GetReturnType(position);

            if (type == null || type == typeof(void) || Intelli.TypeAliases.ContainsKey(type.Name))
            {
                return;
            }

            if (position > 1 && this.GetCharAt(position - 1) == ']' && this.GetCharAt(position - 2) == '[')
            {
                type = type.MakeArrayType();
            }
            else if (type.IsGenericType && this.GetCharAt(position - 1) == '>')
            {
                int openBrace = this.BraceMatch(position - 1);
                if (openBrace != InvalidPosition)
                {
                    string args = GetGenericArgs(openBrace);
                    type = type.MakeGenericType(args);
                }
            }

            iBox.PopulateSuggestions(type);

            ShowIntelliBox(position);
        }

        private void XamlTagIntelliBox(int position)
        {
            iBox.PopulateXamlTags();
            ShowIntelliBox(position);
        }

        private void XamlAttributeIntelliBox(int position)
        {
            Type tag = GetXamlTag(position);
            if (tag == null)
            {
                return;
            }

            iBox.PopulateXamlAttributes(tag);
            ShowIntelliBox(position);
        }

        private void ShowIntelliBox(int position)
        {
            if (iBox.IsEmpty)
            {
                return; // For some reason, the box is empty... don't bother showing it.
            }

            posAtIBox = iBox.ExtraSpace ? position + 1 : position;

            int lineHeight = this.Lines[this.CurrentLine].Height;
            Point topLeft = new Point
            {
                X = PointXFromPosition(posAtIBox) - iBox.IconWidth,
                Y = PointYFromPosition(posAtIBox) + lineHeight
            };

            if (this.ClientSize.Height < (topLeft.Y + iBox.Height))
            {
                topLeft.Offset(0, -lineHeight - iBox.Height);
            }

            if (this.ClientSize.Width < (topLeft.X + iBox.Width))
            {
                topLeft.Offset(iBox.IconWidth - iBox.Width, 0);
            }

            if (topLeft.X < 0)
            {
                topLeft.X = 0;
            }

            if (topLeft.Y < 0)
            {
                topLeft.Y = 0;
            }

            iBox.Location = topLeft;
            iBox.Visible = true;
        }

        internal void ConfirmIntelliBox()
        {
            int startPos = this.WordStartPosition(this.CurrentPosition);
            int endPos = this.WordEndPosition(this.CurrentPosition);
            string fill = iBox.AutoCompleteCode;

            if (this.GetCharAt(startPos - 1).Equals('#'))
            {
                startPos--;
            }

            this.SetTargetRange(startPos, endPos);
            this.ReplaceTarget(fill);
            this.Colorize(startPos, startPos + fill.Length);
            this.SetEmptySelection(startPos + fill.Length);

            iBox.SaveUsedItem();

            iBox.Visible = false;
            posAtIBox = InvalidPosition;

            this.Focus();
        }
        #endregion

        #region Find and Replace functions
        internal void FindAndReplace(bool showReplace)
        {
            findPanel.Term = (this.SelectionStart != this.SelectionEnd) ? this.SelectedText : this.GetWordFromPosition(this.CurrentPosition);
            findPanel.ShowReplace = showReplace;
            if (!findPanel.Visible)
            {
                findPanel.Location = new Point(this.ClientRectangle.Right - this.Margins.Right - findPanel.Width, 0);
                findPanel.Visible = true;
            }
            findPanel.Focus();
            Find(findPanel.Term, findPanel.Flags);
        }

        private void FindPanel_VisibleChanged(object sender, EventArgs e)
        {
            if (!findPanel.Visible)
            {
                Find(string.Empty, SearchFlags.None);
                this.Focus();
            }
        }

        private void FindPanel_ParametersChanged(object sender, EventArgs e)
        {
            Find(findPanel.Term, findPanel.Flags);
        }

        private void FindPanel_ReplaceAllClicked(object sender, EventArgs e)
        {
            Replace(findPanel.Term, findPanel.Replacement, findPanel.Flags);
            OnBuildNeeded();
        }

        private void FindPanel_FindNextClicked(object sender, EventArgs e)
        {
            if (findPanel.Term.Length == 0)
            {
                return;
            }

            this.SearchFlags = findPanel.Flags;

            this.SetTargetRange(this.CurrentPosition, this.TextLength);
            if (this.SearchInTarget(findPanel.Term) != InvalidPosition)
            {
                this.SetSel(this.TargetStart, this.TargetEnd);
                this.Lines[this.LineFromPosition(this.TargetStart)].EnsureVisible();
                this.ScrollCaret();

                return;
            }

            this.SetTargetRange(0, this.CurrentPosition);
            if (this.SearchInTarget(findPanel.Term) != InvalidPosition)
            {
                this.SetSel(this.TargetStart, this.TargetEnd);
                this.Lines[this.LineFromPosition(this.TargetStart)].EnsureVisible();
                this.ScrollCaret();
            }
        }

        private void Find(string term, SearchFlags searchFlags)
        {
            // Remove all highlights from the last time
            this.IndicatorCurrent = Indicator.Find;
            this.IndicatorClearRange(0, this.TextLength);
            this.matchLines.Clear();

            if (term.Length == 0)
            {
                findPanel.Matches = 0;
                UpdateIndicatorBar();
                return;
            }

            // Search the document
            this.TargetWholeDocument();
            this.SearchFlags = searchFlags;

            int matches = 0;
            while (this.SearchInTarget(term) != InvalidPosition)
            {
                matches++;

                this.matchLines.Add(this.LineFromPosition(this.TargetStart));

                // Mark the search results with the current indicator
                this.IndicatorCurrent = Indicator.Find;
                this.IndicatorFillRange(this.TargetStart, this.TargetEnd - this.TargetStart);

                // Search the remainder of the document
                this.SetTargetRange(this.TargetEnd, this.TextLength);
            }

            findPanel.Matches = matches;

            UpdateIndicatorBar();
        }

        private void Replace(string oldTerm, string newTerm, SearchFlags searchFlags)
        {
            if (this.ReadOnly || oldTerm.Length == 0)
            {
                return;
            }

            // Search the document
            this.TargetWholeDocument();
            this.SearchFlags = searchFlags;

            Replacing = true;
            this.BeginUndoAction();
            while (this.SearchInTarget(oldTerm) != InvalidPosition)
            {
                // Replace the instance with new string
                this.ReplaceTarget(newTerm);

                // Search the remainder of the document
                this.SetTargetRange(this.TargetEnd, this.TextLength);
            }
            this.EndUndoAction();
            Replacing = false;

            Find(string.Empty, SearchFlags.None);
        }
        #endregion

        #region Editor Event Override functions
        protected override void Dispose(bool disposing)
        {
            timer.Stop();
            base.Dispose(disposing);
        }

        protected override void OnUpdateUI(UpdateUIEventArgs e)
        {
            base.OnUpdateUI(e);

            bool hScroll = e.Change.HasFlag(UpdateChange.HScroll);
            bool vScroll = e.Change.HasFlag(UpdateChange.VScroll);
            bool content = e.Change.HasFlag(UpdateChange.Content);
            bool selection = e.Change.HasFlag(UpdateChange.Selection);

            bool styles = this.updatingStyles;
            this.updatingStyles = false;

            if (hScroll)
            {
                if (iBox.Visible)
                {
                    Point newLocation = new Point
                    {
                        X = PointXFromPosition(posAtIBox) - iBox.IconWidth,
                        Y = iBox.Location.Y
                    };

                    // Don't cover up the left margins
                    int marginWidth = this.Margins[LeftMargin.LineNumbers].Width + this.Margins[LeftMargin.Bookmarks].Width + this.Margins[LeftMargin.CodeFolding].Width + this.Margins[LeftMargin.Padding].Width;
                    if (newLocation.X < marginWidth)
                    {
                        newLocation.X = -iBox.Width;
                    }

                    iBox.Location = newLocation;
                }
            }

            if (vScroll)
            {
                if (iBox.Visible)
                {
                    int lineHeight = this.Lines[this.CurrentLine].Height;
                    iBox.Location = new Point
                    {
                        X = iBox.Location.X,
                        Y = PointYFromPosition(posAtIBox) + lineHeight
                    };
                }

                if (!mapScroll)
                {
                    indicatorBar.Value = this.FirstVisibleLine;
                }
                else
                {
                    mapScroll = false;
                }

                EnqueueSpellCheck();
            }

            if (hScroll || vScroll || selection)
            {
                if (lightBulbMenu.Visible)
                {
                    bulbIcon.HideDropDown();
                    lightBulbMenu.Hide();
                }
            }

            if (content)
            {
                if (MapEnabled)
                {
                    indicatorBar.Maximum = CountUILines();
                    indicatorBar.Value = this.FirstVisibleLine;
                }

                ColorizeBraces();
            }

            if (selection)
            {
                if (this.CurrentLine != previousLine)
                {
                    if (this.SelectionStart == this.SelectionEnd)
                    {
                        string lineText = this.Lines[this.CurrentLine].Text;

                        if (lineText.TrimEnd('\r', '\n').Length == 0)
                        {
                            // Move to an empty (no whitespace) line
                            // Click or Up/Down Keys or Enter/Return
                            int indent = GetIndentFromPrevLine(this.CurrentLine);
                            this.Selections[0].CaretVirtualSpace = indent;
                            this.Selections[0].AnchorVirtualSpace = indent;
                            if (content)
                            {
                                // Only with Enter/Return
                                this.ChooseCaretX();
                            }
                        }
                        else if (content && this.CurrentLine - previousLine == 1)
                        {
                            // Enter/Return with characters to right of caret
                            int indent = GetIndentFromPrevLine(this.CurrentLine);

                            if (lineText.Trim().Equals("}"))
                            {
                                this.BeginUndoAction();

                                this.ExecuteCmd(Command.NewLine);
                                this.Lines[this.CurrentLine].Indentation = indent - this.TabWidth;
                                this.ExecuteCmd(Command.LineUp);
                                this.Selections[0].CaretVirtualSpace = indent;
                                this.Selections[0].AnchorVirtualSpace = indent;

                                this.EndUndoAction();
                            }
                            else
                            {
                                this.Lines[this.CurrentLine].Indentation = indent;
                                this.SetEmptySelection(this.Lines[this.CurrentLine].Position + indent);
                            }
                        }
                    }
                    else
                    {
                        // Non-empty selection
                        this.Selections[0].CaretVirtualSpace = 0;
                        this.Selections[0].AnchorVirtualSpace = 0;
                    }

                    previousLine = this.CurrentLine;
                }
                else if (this.Selections[0].AnchorVirtualSpace != this.Selections[0].CaretVirtualSpace)
                {
                    // CLick onto an empty (no whitespace) line
                    this.Selections[0].CaretVirtualSpace = this.Selections[0].AnchorVirtualSpace;
                    this.ChooseCaretX();
                }
                else if (this.Selections[0].AnchorVirtualSpace > 0 || this.Selections[0].CaretVirtualSpace > 0)
                {
                    // Press Left Arrow key on an empty (no whitespace) line
                    this.Selections[0].CaretVirtualSpace = 0;
                    this.Selections[0].AnchorVirtualSpace = 0;
                    this.ChooseCaretX();
                }
            }

            if (content || selection)
            {
                if (intelliTip.Visible && !styles)
                {
                    intelliTip.Hide(this);
                    disableIntelliTipPos = InvalidPosition;
                }

                if (MapEnabled)
                {
                    int curLine = GetVisibleLine(this.CurrentLine);
                    indicatorBar.Caret = CountVisibleLines(curLine);
                }

                int caretPos = this.CurrentPosition;

                if (this.autoBraceOpenPos != InvalidPosition)
                {
                    if (caretPos <= this.autoBraceOpenPos || caretPos > this.BraceMatch(this.autoBraceOpenPos))
                    {
                        this.autoBraceOpenPos = InvalidPosition;
                    }
                }

                // Has the caret changed position?
                if (this.Lexer != Lexer.Null && lastCaretPos != caretPos)
                {
                    lastCaretPos = caretPos;

                    HighlightWordUsage();

                    int bracePos1 = InvalidPosition;

                    // Is there a closed brace to the left or an open brace to the right?
                    if (caretPos > 0 && this.GetCharAt(caretPos - 1).IsBrace(false))
                    {
                        bracePos1 = (caretPos - 1);
                    }
                    else if (this.GetCharAt(caretPos).IsBrace(true))
                    {
                        bracePos1 = caretPos;
                    }

                    int style = this.GetStyleAt(bracePos1);
                    bool correctStyle = false;
                    switch (this.Lexer)
                    {
                        case Lexer.Cpp:
                            correctStyle = (style == Style.Cpp.Operator || style == Style.Cpp.Operator + Preprocessor);
                            break;
                        case Lexer.Xml:
                            correctStyle = (style == Style.Xml.Tag || style == Style.Xml.TagUnknown);
                            break;
                    }

                    if (bracePos1 > InvalidPosition && correctStyle)
                    {
                        // Find the matching brace
                        int bracePos2 = this.BraceMatch(bracePos1);
                        if (bracePos2 == InvalidPosition)
                        {
                            if (this.Lexer == Lexer.Cpp)
                            {
                                this.BraceBadLight(bracePos1);
                            }
                            else
                            {
                                // Turn off brace matching
                                this.BraceHighlight(InvalidPosition, InvalidPosition);
                            }

                            this.HighlightGuide = 0;
                        }
                        else
                        {
                            this.BraceHighlight(bracePos1, bracePos2);
                            this.HighlightGuide = this.GetColumn(bracePos1);
                        }
                    }
                    else
                    {
                        // Turn off brace matching
                        this.BraceHighlight(InvalidPosition, InvalidPosition);
                        this.HighlightGuide = 0;
                    }
                }
            }
        }

        protected override void OnCharAdded(CharAddedEventArgs e)
        {
            if (this.ReadOnly)
            {
                // no-op
            }
            else if (iBox.Visible)
            {
                string word = this.GetWordFromPosition(this.CurrentPosition);
                if (this.Lexer == Lexer.Xml && e.Char == '/' && this.GetCharAt(this.CurrentPosition - 2) == '<')
                {
                    // Do nothing
                }
                else if (word.IsCSharpIdentifier())
                {
                    int wordStartPos = this.WordStartPosition(this.CurrentPosition);
                    if (wordStartPos > 0 && this.GetCharAt(wordStartPos - 1).Equals('#'))
                    {
                        word = "#" + word;
                    }

                    iBox.Filter(word);
                }
                else
                {
                    iBox.Visible = false;
                }
            }
            else if (this.Lexer == Lexer.Cpp)
            {
                if (e.Char == '}')
                {
                    if (this.autoBraceOpenPos != InvalidPosition &&
                        base.GetCharAt(this.CurrentPosition) == '}')
                    {
                        this.DeleteRange(this.CurrentPosition, 1);
                    }
                    else if (this.Lines[this.CurrentLine].Text.Trim() == "}") //Check whether the bracket is the only thing on the line.. For cases like "if() { }".
                    {
                        this.Lines[this.CurrentLine].Indentation -= this.TabWidth;
                    }
                }
                else if (e.Char == ')')
                {
                    if (this.autoBraceOpenPos != InvalidPosition &&
                        base.GetCharAt(this.CurrentPosition) == ')')
                    {
                        this.DeleteRange(this.CurrentPosition, 1);
                    }
                }
                else if (e.Char == ']')
                {
                    if (this.autoBraceOpenPos != InvalidPosition &&
                        base.GetCharAt(this.CurrentPosition) == ']')
                    {
                        this.DeleteRange(this.CurrentPosition, 1);
                    }
                }
                else if (e.Char == '>')
                {
                    if (this.autoBraceOpenPos != InvalidPosition &&
                        base.GetCharAt(this.CurrentPosition) == '>')
                    {
                        this.DeleteRange(this.CurrentPosition, 1);
                    }
                }
                else if (e.Char == '.')
                {
                    if (!this.DisableAutoComplete)
                    {
                        MemberIntelliBox(this.CurrentPosition - 1);
                    }
                }
                else if (e.Char == '(')
                {
                    if (IsRightOfCaretEmpty(this.CurrentPosition))
                    {
                        this.InsertText(this.CurrentPosition, ")");
                        this.autoBraceOpenPos = this.CurrentPosition - 1;
                    }

                    if (!this.DisableAutoComplete)
                    {
                        ConstructorIntelliBox(this.CurrentPosition - 1);
                    }
                }
                else if (e.Char == '[')
                {
                    if (IsRightOfCaretEmpty(this.CurrentPosition))
                    {
                        this.InsertText(this.CurrentPosition, "]");
                        this.autoBraceOpenPos = this.CurrentPosition - 1;
                    }
                }
                else if (e.Char == '{')
                {
                    if (IsRightOfCaretEmpty(this.CurrentPosition))
                    {
                        this.InsertText(this.CurrentPosition, "}");
                        this.autoBraceOpenPos = this.CurrentPosition - 1;
                    }
                }
                else if (e.Char == '<')
                {
                    if (IsRightOfCaretEmpty(this.CurrentPosition) &&
                        GetReturnType(this.CurrentPosition - 1)?.IsGenericType == true)
                    {
                        this.InsertText(this.CurrentPosition, ">");
                        this.autoBraceOpenPos = this.CurrentPosition - 1;
                    }
                }
                else if (this.DisableAutoComplete)
                {
                    // no-op
                }
                else if (char.IsLetter(e.Char.ToChar()) || e.Char.Equals('#'))
                {
                    if (this.GetCharAt(this.CurrentPosition - 2) == '.')
                    {
                        MemberIntelliBox(this.CurrentPosition - 2);
                        if (iBox.Visible)
                        {
                            iBox.Filter(this.GetTextRange(this.CurrentPosition - 1, 1));
                        }
                    }
                    else
                    {
                        bool proceedWithBox = true;

                        // Test for when declaring multiple variables in series.
                        // Ex: double a, b;
                        int testPos = this.CurrentPosition - 2;
                        while (char.IsWhiteSpace(this.GetCharAt(testPos)) && testPos > InvalidPosition)
                        {
                            testPos--;
                        }
                        if (this.GetCharAt(testPos).Equals(','))
                        {
                            while (testPos > InvalidPosition)
                            {
                                testPos--;

                                char c = this.GetCharAt(testPos);
                                if (c.Equals('(') || c.Equals('['))
                                {
                                    break;
                                }
                                else if (c.Equals(';') || c.Equals('{') || c.Equals(')'))
                                {
                                    proceedWithBox = false;
                                    break;
                                }
                            }
                        }

                        if (proceedWithBox)
                        {
                            NonMemberIntelliBox(this.CurrentPosition - 1);
                        }
                    }
                }
                else if (e.Char == ' ')
                {
                    if (this.GetWordFromPosition(this.CurrentPosition - 1).Equals("new", StringComparison.Ordinal))
                    {
                        NonMemberIntelliBox(this.CurrentPosition);
                    }
                    else
                    {
                        int testPos = this.CurrentPosition - 2;
                        char c = this.GetCharAt(testPos);
                        if (c == ']' || c == '>')
                        {
                            int openBrace = this.BraceMatch(testPos);
                            if (openBrace != InvalidPosition &&
                                this.LineFromPosition(openBrace) == this.LineFromPosition(testPos))
                            {
                                testPos = openBrace;
                            }
                        }

                        if (GetIntelliType(testPos - 1) == IntelliType.Type)
                        {
                            SuggestionIntelliBox(this.CurrentPosition - 1);
                        }
                    }
                }
            }
            else if (this.Lexer == Lexer.Xml)
            {
                if (this.DisableAutoComplete)
                {
                    // no-op
                }
                else if (e.Char == '<')
                {
                    XamlTagIntelliBox(this.CurrentPosition - 1);
                }
                else if (e.Char == ' ')
                {
                    XamlAttributeIntelliBox(this.CurrentPosition - 1);
                }
            }

            base.OnCharAdded(e);
        }

        protected override void OnZoomChanged(EventArgs e)
        {
            base.OnZoomChanged(e);

            UpdateMarginWidths();

            this.WhitespaceSize = UIUtil.Scale((this.Zoom >= 2) ? 2 : 1);

            UpdateIndicatorBar();

            if (iBox.Visible)
            {
                iBox.Location = new Point(
                    PointXFromPosition(posAtIBox) - iBox.IconWidth,
                    PointYFromPosition(posAtIBox) + this.Lines[this.CurrentLine].Height);
            }
        }

        protected override void OnPainted(EventArgs e)
        {
            base.OnPainted(e);

            // Sometimes the native Scintilla control takes too long to paint itself,
            // and consecutive calls don't work properly. So, we'll execute those
            // calls after Scintilla has reported it has finished re-painting itself.
            // As far as I can tell, this is just thread safety issue that ScintillaNET
            // should handle, but it doesn't.

            if (this.delayedOperation == DelayedOperation.None)
            {
                return;
            }

            if (this.delayedOperation.HasFlag(DelayedOperation.UpdateIndicatorBar))
            {
                UpdateIndicatorBar();
            }

            if (this.delayedOperation.HasFlag(DelayedOperation.ScrollCaret))
            {
                this.ScrollCaret();
            }

            if (this.delayedOperation.HasFlag(DelayedOperation.Spellcheck))
            {
                SpellCheck();
            }

            this.delayedOperation = DelayedOperation.None;
        }

        protected override void OnTextChanged(EventArgs e)
        {
            AdjustLineNumbersWidth();

            // Make sure scrollbar does cover the Find panel
            if (findPanel.Visible)
            {
                findPanel.Location = new Point(this.ClientRectangle.Right - this.Margins.Right - findPanel.Width, 0);
            }

            //Update Find Highlighting
            if (findPanel.Visible && findPanel.Term.Length > 0 && !Replacing)
            {
                Tuple<int, int> oldRange = new Tuple<int, int>(this.TargetStart, this.TargetEnd);
                Find(findPanel.Term, findPanel.Flags);
                this.SetTargetRange(oldRange.Item1, oldRange.Item2);
            }

            EnqueueSpellCheck();

            AdjustRenaming();

            base.OnTextChanged(e);
        }

        protected override void OnBeforeDelete(BeforeModificationEventArgs e)
        {
            if (this.Lexer == Lexer.Cpp && e.Source == ModificationSource.User && e.Text.Trim().Length > 0)
            {
                SetUpRenaming(e.Position);
            }

            base.OnBeforeDelete(e);
        }

        protected override void OnBeforeInsert(BeforeModificationEventArgs e)
        {
            if (this.Lexer == Lexer.Cpp && e.Source == ModificationSource.User && e.Text.Trim().Length > 0)
            {
                SetUpRenaming(e.Position);
            }

            base.OnBeforeInsert(e);
        }

        protected override void OnClientSizeChanged(EventArgs e)
        {
            base.OnClientSizeChanged(e);

            if (findPanel.Visible)
            {
                findPanel.Location = new Point(this.ClientRectangle.Right - this.Margins.Right - findPanel.Width, 0);
            }

            if (iBox.Visible)
            {
                iBox.Visible = false;
            }

            this.delayedOperation |= DelayedOperation.UpdateIndicatorBar;
        }

        protected override void OnLostFocus(EventArgs e)
        {
            // Scintilla doesn't hide its caret, if it loses focus to a child control
            if (this.ContainsFocus)
            {
                this.InternalFocusFlag = false;
            }

            base.OnLostFocus(e);
        }

        protected override void OnMarginClick(MarginClickEventArgs e)
        {
            Line line = this.Lines[this.LineFromPosition(e.Position)];

            if (e.Margin == LeftMargin.Bookmarks)
            {
                // Do we have a marker for this line?
                if ((line.MarkerGet() & BookmarkMargin.Mask) > 0)
                {
                    // Remove existing bookmark
                    line.MarkerDelete(BookmarkMargin.Marker);
                }
                else
                {
                    // Add bookmark
                    line.MarkerAdd(BookmarkMargin.Marker);
                }
            }
            else if (e.Margin == LeftMargin.CodeFolding)
            {
                if (line.FoldLevelFlags.HasFlag(FoldLevelFlags.Header))
                {
                    line.ToggleFoldShowText("...");
                }
            }

            UpdateIndicatorBar();

            base.OnMarginClick(e);
        }
        #endregion

        #region Editor Command functions
        internal void Comment()
        {
            int startLine = this.LineFromPosition(this.SelectionStart);
            int endLine = this.LineFromPosition(this.SelectionEnd);

            int minIndent = int.MaxValue;
            for (int line = startLine; line < endLine + 1; line++)
            {
                if (this.Lines[line].Text.Trim().Length > 0 && this.Lines[line].Indentation < minIndent)
                {
                    minIndent = this.Lines[line].Indentation;
                }
            }

            this.BeginUndoAction();
            for (int line = startLine; line < endLine + 1; line++)
            {
                if (this.Lines[line].Text.Trim().Length > 0)
                {
                    this.InsertText(this.Lines[line].Position + minIndent, "//");
                }
            }
            this.EndUndoAction();
        }

        internal void UnComment()
        {
            this.BeginUndoAction();
            for (int line = this.LineFromPosition(this.SelectionStart); line < this.LineFromPosition(this.SelectionEnd) + 1; line++)
            {
                if (this.Lines[line].Text.Trim().StartsWith("//", StringComparison.Ordinal))
                {
                    this.DeleteRange(this.Lines[line].Position + this.Lines[line].Indentation, 2);
                }
            }
            this.EndUndoAction();
        }

        internal void Indent()
        {
            this.ExecuteCmd(Command.Tab);
        }

        internal void UnIndent()
        {
            this.ExecuteCmd(Command.BackTab);
        }

        internal void UpdateSyntaxHighlighting()
        {
            if (this.Lexer != Lexer.Cpp)
            {
                return;
            }

            this.updatingStyles = true;

            this.SetKeywords(0, "abstract as base bool byte char checked class const decimal delegate double enum event explicit extern "
                + "false fixed float get implicit in int interface internal is lock long namespace new not null object operator out override "
                + "params partial private protected public readonly ref sbyte sealed set short sizeof stackalloc static string struct "
                + "this true typeof uint unchecked unsafe ulong ushort using var virtual void volatile where with");

            this.SetIdentifiers(Substyle.Keyword, "break case catch continue default do else finally for foreach goto if in return throw try switch while");

            if (!this.useExtendedColors)
            {
                this.SetKeywords(1, Intelli.ClassList + " " + Intelli.StructList + " " + Intelli.EnumList + " " + Intelli.InterfaceList + " " + Intelli.UserDefinedTypes.Keys);
            }
            else
            {
                string classWords = Intelli.ClassList;
                string structWords = Intelli.StructList;
                string enumWords = Intelli.EnumList;
                string interfaceWords = Intelli.InterfaceList;

                if (Intelli.UserDefinedTypes.Count > 0)
                {
                    HashSet<string> userEnums = new HashSet<string>();
                    HashSet<string> userInterfaces = new HashSet<string>();
                    HashSet<string> userStructs = new HashSet<string>();
                    HashSet<string> userClasses = new HashSet<string>();
                    foreach (KeyValuePair<string, Type> kvp in Intelli.UserDefinedTypes)
                    {
                        if (kvp.Value.IsEnum)
                        {
                            userEnums.Add(kvp.Key);
                        }
                        else if (kvp.Value.IsValueType)
                        {
                            userStructs.Add(kvp.Key);
                        }
                        else if (kvp.Value.IsClass)
                        {
                            userClasses.Add(kvp.Key);
                        }
                        else if (kvp.Value.IsInterface)
                        {
                            userInterfaces.Add(kvp.Key);
                        }
                    }

                    if (userClasses.Count > 0)
                    {
                        classWords += " " + userClasses.Join(" ");
                    }
                    if (userStructs.Count > 0)
                    {
                        structWords += " " + userStructs.Join(" ");
                    }
                    if (userEnums.Count > 0)
                    {
                        enumWords += " " + userEnums.Join(" ");
                    }
                    if (userInterfaces.Count > 0)
                    {
                        interfaceWords += " " + userInterfaces.Join(" ");
                    }
                }

                this.SetKeywords(1, classWords);
                this.SetIdentifiers(Substyle.Struct, structWords);
                this.SetIdentifiers(Substyle.Enum, enumWords);
                this.SetIdentifiers(Substyle.Interface, interfaceWords);
            }
        }

        internal void FormatDocument()
        {
            string[] linesNoComments = this.Text.StripComments().Split('\n');
            if (linesNoComments.Length != this.Lines.Count)
            {
                return;
            }

            int lineIndent = 0;
            bool codeBlock = false;
            bool braceLess = false;

            int withinCase = 0;
            bool caseStart = false;

            this.BeginUndoAction();
            for (int line = 0; line < this.Lines.Count; line++)
            {
                // Adjust Line Indentation
                string trimmedLine = linesNoComments[line].Trim();

                if (codeBlock && !trimmedLine.StartsWith("{", StringComparison.Ordinal) && !trimmedLine.StartsWith("using", StringComparison.Ordinal))
                {
                    braceLess = true;
                    lineIndent += this.TabWidth;
                }
                else if (trimmedLine.Equals("}", StringComparison.Ordinal) || trimmedLine.Equals("};", StringComparison.Ordinal))
                {
                    lineIndent -= this.TabWidth;
                }
                else if (caseStart && (trimmedLine.StartsWith("case ", StringComparison.Ordinal) || trimmedLine.StartsWith("default:", StringComparison.Ordinal)))
                {
                    withinCase--;
                    lineIndent -= this.TabWidth;
                }

                if (trimmedLine.Length > 0)
                {
                    caseStart = false;
                }

                this.Lines[line].Indentation = lineIndent;
                lineIndent = this.Lines[line].Indentation; // make sure they're in sync, as scintilla may clamp

                codeBlock = false;
                if (braceLess)
                {
                    braceLess = false;
                    lineIndent -= this.TabWidth;
                }
                if (trimmedLine.StartsWith("case ", StringComparison.Ordinal) || trimmedLine.StartsWith("default:", StringComparison.Ordinal))
                {
                    caseStart = true;
                    withinCase++;
                    lineIndent += this.TabWidth;
                }
                else if (withinCase > 0 && (trimmedLine.EndsWith("break;", StringComparison.Ordinal) || trimmedLine.EndsWith("return;", StringComparison.Ordinal)))
                {
                    withinCase--;
                    lineIndent -= this.TabWidth;
                }
                else if (trimmedLine.EndsWith("{", StringComparison.Ordinal))
                {
                    lineIndent += this.TabWidth;
                }
                else if (trimmedLine.EndsWith("}", StringComparison.Ordinal) && !trimmedLine.Equals("}", StringComparison.Ordinal))
                {
                    int closeBrace = this.Lines[line].Position + this.Lines[line].Indentation + trimmedLine.Length - 1;
                    int openBrace = this.BraceMatch(closeBrace);

                    if (openBrace != InvalidPosition && this.LineFromPosition(openBrace) != line)
                    {
                        lineIndent -= this.TabWidth;
                    }
                }
                else if (trimmedLine.EndsWith("};", StringComparison.Ordinal) && !trimmedLine.Equals("};", StringComparison.Ordinal))
                {
                    int closeBrace = this.Lines[line].Position + this.Lines[line].Indentation + trimmedLine.Length - 2;
                    int openBrace = this.BraceMatch(closeBrace);

                    if (openBrace != InvalidPosition && this.LineFromPosition(openBrace) != line)
                    {
                        lineIndent -= this.TabWidth;
                    }
                }
                else if (trimmedLine.StartsWith("using", StringComparison.Ordinal) || trimmedLine.StartsWith("else", StringComparison.Ordinal) ||
                        (trimmedLine.StartsWith("if", StringComparison.Ordinal) && !trimmedLine.EndsWith(";", StringComparison.Ordinal)))
                {
                    codeBlock = true;
                }

                // Trim Line End
                int trimLength = this.Lines[line].Length - 2 - this.Lines[line].Text.TrimEnd().Length;
                if (trimLength > 0)
                {
                    this.DeleteRange(this.Lines[line].EndPosition - 2 - trimLength, trimLength);
                }
            }
            this.EndUndoAction();
        }
        #endregion

        #region Renaming
        private void SetUpRenaming(int position)
        {
            if (this.Replacing || iBox.Visible)
            {
                return;
            }

            int wordStartPos = this.WordStartPosition(position);
            IntelliType intelliType = GetIntelliType(wordStartPos);
            if (IsIndicatorOn(Indicator.Rename, wordStartPos) ||
                (intelliType != IntelliType.Variable && intelliType != IntelliType.Field))
            {
                return;
            }

            if (intelliType == IntelliType.Field && GetDeclaringType(wordStartPos)?.FullName != Intelli.UserScriptFullName)
            {
                return;
            }

            RenameInfo.Position = wordStartPos;
            RenameInfo.Identifier = this.GetWordFromPosition(position);
            RenameInfo.IntelliType = intelliType;

            int length = this.WordEndPosition(position) - wordStartPos;

            this.IndicatorCurrent = Indicator.Rename;
            this.IndicatorFillRange(wordStartPos, length);
        }

        private void AdjustRenaming()
        {
            int wordStartPos = this.WordStartPosition(this.CurrentPosition);
            int wordEndPos = this.WordEndPosition(this.CurrentPosition) - 1;
            if (IsIndicatorOn(Indicator.Rename, wordStartPos) || IsIndicatorOn(Indicator.Rename, wordEndPos) || this.CurrentPosition == RenameInfo.Position)
            {
                int endPos = wordStartPos;
                while (IsIndicatorOn(Indicator.Rename, endPos) && endPos <= this.TextLength)
                {
                    endPos++;
                }

                int nonSpacePos = wordStartPos;
                while (!char.IsWhiteSpace(this.GetCharAt(nonSpacePos)) && nonSpacePos <= endPos)
                {
                    nonSpacePos++;
                }

                if ((endPos == nonSpacePos - 1 || endPos == nonSpacePos) && this.GetWordFromPosition(this.CurrentPosition) != RenameInfo.Identifier)
                {
                    int length = this.WordEndPosition(this.CurrentPosition) - wordStartPos;

                    this.IndicatorCurrent = Indicator.Rename;
                    this.IndicatorFillRange(wordStartPos, length);
                }
                else
                {
                    ClearRenaming();
                }
            }
            else
            {
                ClearRenaming();
            }
        }

        private void ClearRenaming()
        {
            RenameInfo.Clear();

            this.IndicatorCurrent = Indicator.Rename;
            this.IndicatorClearRange(0, this.TextLength);
        }

        private void DoRename()
        {
            if (!RenameInfo.IsValid)
            {
                return;
            }

            this.SetEmptySelection(RenameInfo.Position);
            string newName = this.GetWordFromPosition(RenameInfo.Position);

            // re-add the old variable to dictionary.
            // it will be automatically removed during the next ParseVariables();
            if (RenameInfo.IntelliType == IntelliType.Variable)
            {
                ParseVariables(this.CurrentPosition);
                if (!Intelli.Variables.ContainsKey(RenameInfo.Identifier))
                {
                    Intelli.Variables.Add(RenameInfo.Identifier, Intelli.Variables[newName]);
                }
            }

            // Search the document
            this.TargetWholeDocument();
            this.SearchFlags = SearchFlags.MatchCase | SearchFlags.WholeWord;

            this.BeginUndoAction();
            while (this.SearchInTarget(RenameInfo.Identifier) != InvalidPosition)
            {
                if (GetIntelliType(this.TargetStart) == RenameInfo.IntelliType)
                {
                    // Replace the instance with new string
                    this.ReplaceTarget(newName);
                }

                // Search the remainder of the document
                this.SetTargetRange(this.TargetEnd, this.TextLength);
            }
            this.EndUndoAction();

            ClearRenaming();

            if (RenameInfo.IntelliType == IntelliType.Variable)
            {
                ParseVariables(this.CurrentPosition);
            }

            HighlightWordUsage();
        }

        private void RenameButton_Click(object sender, EventArgs e)
        {
            lightBulbMenu.Hide();
            DoRename();
            OnBuildNeeded();
        }

        private static class RenameInfo
        {
            internal static int Position = InvalidPosition;
            internal static string Identifier = string.Empty;
            internal static IntelliType IntelliType = IntelliType.None;

            internal static bool IsValid => Position > InvalidPosition && Identifier.Length > 0 && IntelliType != IntelliType.None;

            internal static void Clear()
            {
                Position = InvalidPosition;
                Identifier = string.Empty;
                IntelliType = IntelliType.None;
            }
        }
        #endregion

        #region Helper functions
        private int GetVisibleLine(int line)
        {
            line = Math.Clamp(line, 0, this.Lines.Count - 1);

            while (!this.Lines[line].Visible)
            {
                if (line != this.Lines[line].FoldParent && this.Lines[line].FoldParent != -1)
                {
                    line = this.Lines[line].FoldParent;
                }
                else
                {
                    break;
                }
            }

            return line;
        }

        private int CountUILines()
        {
            return CountVisibleLines(this.Lines.Count);
        }

        private int CountVisibleLines(int untilLine)
        {
            bool wordWrapDisabled = this.WrapMode == WrapMode.None;
            bool allLinesVisible = this.Lines.AllLinesVisible;

            if (wordWrapDisabled && allLinesVisible)
            {
                return untilLine;
            }

            int count = 0;
            for (int i = 0; i < untilLine; i++)
            {
                if (allLinesVisible || this.Lines[i].Visible)
                {
                    count += wordWrapDisabled ? 1 : this.Lines[i].WrapCount;
                }
            }

            return count;
        }

        private bool IsIndicatorOn(int indicator, int position)
        {
            uint bitmask = this.IndicatorAllOnFor(position);
            int flag = (1 << indicator);
            return ((bitmask & flag) == flag);
        }

        private new char GetCharAt(int position)
        {
            return base.GetCharAt(position).ToChar();
        }

        private int WordStartPosition(int position)
        {
            return this.WordStartPosition(position, true);
        }

        private int WordEndPosition(int position)
        {
            return this.WordEndPosition(position, true);
        }

        private bool IsRightOfCaretEmpty(int position)
        {
            Line line = this.Lines[this.LineFromPosition(position)];
            return line.Text.Substring(position - line.Position).Trim().Length == 0;
        }

        private int GetIndentFromPrevLine(int line)
        {
            int indent = 0;
            int lineIndex = line - 1;
            while (lineIndex >= 0)
            {
                string lineText = this.Lines[lineIndex].Text.Trim();
                if (lineText.Length > 0)
                {
                    indent = lineText.EndsWith("{", StringComparison.Ordinal) ?
                        this.Lines[lineIndex].Indentation + this.TabWidth :
                        this.Lines[lineIndex].Indentation;

                    break;
                }

                lineIndex--;
            }

            return indent;
        }

        private new int GetStyleAt(int position)
        {
            int style = base.GetStyleAt(position);
            return (style >= 0) ? style : Substyle.SubstyleCorrection(style);
        }

        private void AdjustLineNumbersWidth()
        {
            if (this.Margins[LeftMargin.LineNumbers].Width > 0) // Line Numbers Visible/Enabled?
            {
                // Did the number of characters in the line number display change?
                // i.e. nnn VS nn, or nnnn VS nn, etc...
                int newLineNumberCharLength = this.Lines.Count.ToString().Length;
                if (newLineNumberCharLength != this.maxLineNumberCharLength)
                {
                    // Calculate the width required to display the last line number
                    // and include some padding for good measure.
                    this.Margins[LeftMargin.LineNumbers].Width = this.TextWidth(Style.LineNumber, new string('9', newLineNumberCharLength + 1)) + 2;
                    this.maxLineNumberCharLength = newLineNumberCharLength;
                }
            }
        }

        internal void UpdateMarginWidths()
        {
            if (this.Margins[LeftMargin.LineNumbers].Width > 0) // Line Numbers Visible/Enabled?
            {
                this.Margins[LeftMargin.LineNumbers].Width = this.TextWidth(Style.LineNumber, new string('9', maxLineNumberCharLength + 1)) + 2;
            }

            if (this.Margins[LeftMargin.Bookmarks].Width > 0) // Bookmarks Visible/Enabled?
            {
                this.Margins[LeftMargin.Bookmarks].Width = this.Lines[0].Height;
            }

            if (this.Margins[LeftMargin.CodeFolding].Width > 0) // Folding Visible/Enabled?
            {
                this.Margins[LeftMargin.CodeFolding].Width = this.Lines[0].Height;
            }
        }

        private void UpdateSubstyleAllocations()
        {
            int correctCount = this.useExtendedColors ? Substyle.ExtStyleCount : Substyle.NormStyleCount;
            if (this.GetSubstylesLength(Style.Cpp.Identifier) != correctCount)
            {
                this.FreeSubstyles();
                this.AllocateSubstyles(Style.Cpp.Identifier, correctCount);
            }
        }
        #endregion

        #region Editor ToolTip Functions
        protected override void OnDwellStart(DwellEventArgs e)
        {
            base.OnDwellStart(e);

            if (intelliTip.Visible ||
                e.Position == disableIntelliTipPos ||
                e.Position == disableIntelliTipPos + 1)
            {
                return;
            }

            if (lightBulbMenu.Visible)
            {
                lightBulbMenu.Hide();
            }

            if (this.Lexer != Lexer.Null)
            {
                dwellWordPos = this.WordStartPosition(e.Position);

                string tooltipText = null;

                if (this.Lexer == Lexer.Cpp)
                {
                    // If there's an error here, we'll show that instead
                    bool isError = this.IsIndicatorOn(Indicator.Error, e.Position);
                    bool isWarning = this.IsIndicatorOn(Indicator.Warning, e.Position);
                    if (isError || isWarning)
                    {
                        int indicator = isError ? Indicator.Error : Indicator.Warning;
                        int indicatorPosition = this.Indicators[indicator].Start(e.Position);
                        int indicatorLength = this.Indicators[indicator].End(e.Position) - indicatorPosition;

                        foreach (Error error in ScriptBuilder.Errors)
                        {
                            if (error.StartLine < 0)
                            {
                                continue;
                            }

                            var (errorPosition, errorLength) = GetErrorPosition(error.StartLine, error.StartColumn, error.EndLine, error.EndColumn);
                            if (errorPosition == indicatorPosition && errorLength == indicatorLength)
                            {
                                tooltipText = error.ErrorText.InsertLineBreaks(100);
                                break;
                            }
                        }
                    }
                }

                if (tooltipText == null)
                {
                    switch (this.Lexer)
                    {
                        case Lexer.Cpp:
                            tooltipText = this.GetIntelliTipCSharp(e.Position);
                            break;
                        case Lexer.Xml:
                            tooltipText = this.GetIntelliTipXaml(e.Position);
                            break;
                        default:
                            tooltipText = string.Empty;
                            break;
                    }
                }

                if (tooltipText.Length > 0)
                {
                    int y = this.PointYFromPosition(e.Position) + this.Lines[this.CurrentLine].Height;
                    intelliTip.Show(tooltipText, this, e.X, y);
                }
            }

            bool showLightBulb = false;
            if (this.IsIndicatorOn(Indicator.Rename, e.Position))
            {
                bulbIcon.DropDownItems.Clear();

                string rename = $"Rename '{RenameInfo.Identifier}' to '{this.GetWordFromPosition(e.Position)}'";
                bulbIcon.DropDownItems.Add(new ToolStripMenuItem(rename, null, RenameButton_Click));

                showLightBulb = true;
            }
            else if (spellCheckEnabled && this.IsIndicatorOn(Indicator.Spelling, e.Position))
            {
                SetupSpellingLightBulb(e.Position);

                showLightBulb = true;
            }

            if (showLightBulb)
            {
                lightBulbMenu.Location = new Point(
                    this.PointXFromPosition(e.Position) - lightBulbMenu.Width - 10,
                    this.PointYFromPosition(e.Position) + this.Lines[this.CurrentLine].Height);

                lightBulbMenu.Show();
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (intelliTip.Visible)
            {
                int pos = this.CharPositionFromPointClose(e.X, e.Y);
                int wordStartPos = this.WordStartPosition(pos);
                if (wordStartPos != dwellWordPos)
                {
                    intelliTip.Hide(this);
                    disableIntelliTipPos = InvalidPosition;
                }
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            if (intelliTip.Visible)
            {
                intelliTip.Hide(this);
                disableIntelliTipPos = InvalidPosition;
            }
        }
        #endregion

        #region Document Tabs functions
        internal void CreateNewDocument(Guid guid, ProjectType projectType)
        {
            this.findPanel.Hide();
            this.iBox.Hide();

            this.docMetaCollection[this.docGuid] = this.DocumentMeta;
            this.docGuid = guid;

            var document = this.Document;
            this.AddRefDocument(document);

            // Replace the current document with a new one
            ScintillaNET.Document newDocument = this.CreateDocument();
            this.docCollection.Add(guid, newDocument);

            this.Document = newDocument;

            switch (projectType)
            {
                case ProjectType.None:
                    this.Lexer = Lexer.Null;
                    break;
                case ProjectType.ClassicEffect:
                case ProjectType.GpuEffect:
                case ProjectType.BitmapEffect:
                case ProjectType.FileType:
                case ProjectType.Reference:
                    this.Lexer = Lexer.Cpp;
                    this.AllocateSubstyles(Style.Cpp.Identifier, this.useExtendedColors ? Substyle.ExtStyleCount : Substyle.NormStyleCount);
                    this.UpdateSyntaxHighlighting();

                    this.SetProperty("fold", "1");
                    this.SetProperty("fold.compact", "0");
                    this.SetProperty("lexer.cpp.allow.dollars", "0");
                    this.SetProperty("lexer.cpp.escape.sequence", "1");

                    switch (this.theme)
                    {
                        case Theme.Light:
                            SetCSharpLightStyles();
                            break;
                        case Theme.Dark:
                            SetCSharpDarkStyles();
                            break;
                    }
                    break;
                case ProjectType.Shape:
                    this.Lexer = Lexer.Xml;

                    this.SetProperty("fold", "1");
                    this.SetProperty("fold.html", "1");
                    this.SetProperty("fold.compact", "0");

                    switch (this.theme)
                    {
                        case Theme.Light:
                            SetXMLLightStyles();
                            break;
                        case Theme.Dark:
                            SetXMLDarkStyles();
                            break;
                    }
                    break;
            }

            AdjustLineNumbersWidth();
            ClearErrors();
            UpdateIndicatorBar();
        }

        internal void SwitchToDocument(Guid guid)
        {
            if (!docCollection.ContainsKey(guid))
            {
                return;
            }

            this.findPanel.Hide();
            this.iBox.Hide();

            this.docMetaCollection[this.docGuid] = this.DocumentMeta;
            this.docGuid = guid;

            var prevDocument = this.Document;
            this.AddRefDocument(prevDocument);

            this.Document = this.docCollection[guid];
            this.ReleaseDocument(this.docCollection[guid]);

            this.DocumentMeta = this.docMetaCollection[guid];

            if (!this.spellCheckEnabled)
            {
                this.IndicatorCurrent = Indicator.Spelling;
                this.IndicatorClearRange(0, this.TextLength);
            }

            switch (this.Lexer)
            {
                case Lexer.Cpp:
                    UpdateSubstyleAllocations();

                    switch (this.theme)
                    {
                        case Theme.Light:
                            SetCSharpLightStyles();
                            break;
                        case Theme.Dark:
                            SetCSharpDarkStyles();
                            break;
                    }
                    break;
                case Lexer.Xml:
                    switch (this.theme)
                    {
                        case Theme.Light:
                            SetXMLLightStyles();
                            break;
                        case Theme.Dark:
                            SetXMLDarkStyles();
                            break;
                    }
                    break;
            }

            AdjustLineNumbersWidth();
            ClearErrors();
            UpdateIndicatorBar();
        }

        internal void CloseDocument(Guid guid)
        {
            if (!docCollection.ContainsKey(guid))
            {
                return;
            }

            this.ReleaseDocument(this.docCollection[guid]);
            this.docCollection.Remove(guid);
            this.docMetaCollection.Remove(guid);
        }
        #endregion

        #region Indicator Map functions
        internal void UpdateIndicatorBar()
        {
            if (!MapEnabled)
            {
                return;
            }

            List<int> visibleLine = new List<int>();

            bool wordWrapDisabled = this.WrapMode == WrapMode.None;
            bool allLinesVisible = this.Lines.AllLinesVisible;

            int count = 0;
            for (int i = 0; i < this.Lines.Count; i++)
            {
                visibleLine.Add(count);

                if (allLinesVisible || this.Lines[i].Visible)
                {
                    count += wordWrapDisabled ? 1 : this.Lines[i].WrapCount;
                }
            }

            indicatorBar.Maximum = count;
            indicatorBar.LargeChange = this.LinesOnScreen;
            indicatorBar.Value = this.FirstVisibleLine;

            int curLine = GetVisibleLine(this.CurrentLine);
            indicatorBar.Caret = visibleLine[curLine];

            if (this.Bookmarks.Count == 0)
            {
                indicatorBar.Bookmarks = Array.Empty<int>();
            }
            else
            {
                List<int> bkmks = new List<int>();
                foreach (int line in this.Bookmarks)
                {
                    int bkmkLine = GetVisibleLine(line);
                    bkmks.Add(visibleLine[bkmkLine]);
                }
                indicatorBar.Bookmarks = bkmks;
            }

            if (matchLines.Count == 0)
            {
                indicatorBar.Matches = Array.Empty<int>();
            }
            else
            {
                List<int> matches = new List<int>();
                foreach (int line in matchLines)
                {
                    int matchLine = GetVisibleLine(line);
                    matches.Add(visibleLine[matchLine]);
                }
                indicatorBar.Matches = matches;
            }

            if (errorLines.Count == 0)
            {
                indicatorBar.Errors = Array.Empty<int>();
            }
            else
            {
                List<int> errors = new List<int>();
                foreach (int line in errorLines)
                {
                    int errorLine = GetVisibleLine(line);
                    errors.Add(visibleLine[errorLine]);
                }
                indicatorBar.Errors = errors;
            }

            if (warningLines.Count == 0)
            {
                indicatorBar.Warnings = Array.Empty<int>();
            }
            else
            {
                List<int> warnings = new List<int>();
                foreach (int line in warningLines)
                {
                    int warningLine = GetVisibleLine(line);
                    warnings.Add(visibleLine[warningLine]);
                }
                indicatorBar.Warnings = warnings;
            }
        }

        private void IndicatorBar_Scroll(object sender, ScrollEventArgs e)
        {
            mapScroll = true;
            this.LineScroll(e.NewValue - this.FirstVisibleLine, 0);
        }
        #endregion

        #region Spellcheck
        private void SpellCheck()
        {
            int firstLine = this.DocLineFromVisible(this.FirstVisibleLine);
            int lastLine = this.DocLineFromVisible(this.FirstVisibleLine + this.LinesOnScreen);

            int startPos = this.Lines[firstLine].Position;
            int length = this.Lines[lastLine].EndPosition - startPos;

            string textRange = this.GetTextRange(startPos, length);
            if (textRange.Length == 0)
            {
                return;
            }

            SpellingError[] spellingErrors = spellChecker.Check(textRange).ToArray();
            bool isCSharp = this.Lexer == Lexer.Cpp;
            bool isXaml = this.Lexer == Lexer.Xml;

            this.IndicatorCurrent = Indicator.Spelling;
            this.IndicatorClearRange(startPos, length);

            foreach (SpellingError spellingError in spellingErrors)
            {
                int errorPos = startPos + (int)spellingError.StartIndex;
                int errorLength = (int)spellingError.Length;

                if (isCSharp || isXaml)
                {
                    if (spellingError.RecommendedAction == RecommendedAction.Delete)
                    {
                        continue;
                    }

                    int style = this.GetStyleAt(errorPos);

                    bool isComment;
                    if (isCSharp)
                    {
                        isComment =
                            style == Style.Cpp.Comment || style == Style.Cpp.Comment + Preprocessor ||
                            style == Style.Cpp.CommentLine || style == Style.Cpp.CommentLine + Preprocessor;
                    }
                    else
                    {
                        isComment = style == Style.Xml.Comment || style == Style.Xml.XcComment;
                    }

                    if (isComment)
                    {
                        string commentText = this.GetTextRange(errorPos, errorLength);

                        if (commentText.Contains('.'))
                        {
                            continue;
                        }

                        if (commentText.Contains('|'))
                        {
                            SpellingError[] commentsErrors = spellChecker.Check(commentText.Replace('|', ' ')).ToArray();
                            foreach (SpellingError commentError in commentsErrors)
                            {
                                if (commentError.RecommendedAction == RecommendedAction.Delete)
                                {
                                    continue;
                                }

                                this.IndicatorFillRange(errorPos + (int)commentError.StartIndex, (int)commentError.Length);
                            }

                            continue;
                        }
                    }
                    else
                    {
                        if (isXaml)
                        {
                            continue;
                        }

                        bool isString =
                            style == Style.Cpp.String || style == Style.Cpp.String + Preprocessor ||
                            style == Style.Cpp.Verbatim || style == Style.Cpp.Verbatim + Preprocessor;

                        if (!isString)
                        {
                            continue;
                        }
                    }
                }

                this.IndicatorFillRange(errorPos, errorLength);
            }
        }

        private void EnqueueSpellCheck()
        {
            if (spellCheckEnabled)
            {
                delayedOperation |= DelayedOperation.Spellcheck;
            }
        }

        private void SetupSpellingLightBulb(int position)
        {
            int indicatorStart = this.Indicators[Indicator.Spelling].Start(position);
            int indicatorLength = this.Indicators[Indicator.Spelling].End(position) - indicatorStart;

            RecommendedAction recommendedAction = RecommendedAction.None;
            if (this.Lexer == Lexer.Null)
            {
                Line line = this.Lines[this.LineFromPosition(position)];
                recommendedAction = spellChecker.Check(line.Text)
                    .FirstOrDefault(error => line.Position + (int)error.StartIndex == indicatorStart && (int)error.Length == indicatorLength)
                    .RecommendedAction;
            }

            string misspelledWord = this.GetTextRange(indicatorStart, indicatorLength);

            bulbIcon.DropDownItems.Clear();

            if (recommendedAction == RecommendedAction.Delete)
            {
                ToolStripMenuItem deleteWord = new ToolStripMenuItem($"Delete '{misspelledWord}'", null, (sender, eventArgs) =>
                {
                    lightBulbMenu.Hide();
                    this.DeleteRange(indicatorStart - 1, indicatorLength + 1);
                });

                bulbIcon.DropDownItems.Add(deleteWord);
            }
            else
            {
                ToolStripMenuItem[] suggestedWords = spellChecker.Suggestions(misspelledWord)
                    .Select(word => new ToolStripMenuItem(word, null, (sender, eventArgs) =>
                    {
                        lightBulbMenu.Hide();
                        this.SetTargetRange(indicatorStart, indicatorStart + indicatorLength);
                        this.ReplaceTarget(word);
                    }))
                    .ToArray();

                ToolStripMenuItem ignoreWord = new ToolStripMenuItem($"Ignore '{misspelledWord}'", null, (sender, eventArgs) =>
                {
                    spellChecker.Ignore(misspelledWord);
                    EnqueueSpellCheck();
                    lightBulbMenu.Hide();
                    Settings.SpellingWordsToIgnore = Settings.SpellingWordsToIgnore.Append(misspelledWord);
                });

                if (suggestedWords.Length > 0)
                {
                    bulbIcon.DropDownItems.AddRange(suggestedWords);
                    bulbIcon.DropDownItems.Add(new ToolStripSeparator());
                    bulbIcon.DropDownItems.Add(ignoreWord);
                }
                else
                {
                    bulbIcon.DropDownItems.Add(ignoreWord);
                }
            }
        }
        #endregion

        #region Errors
        internal void ClearErrors()
        {
            errorLines.Clear();
            warningLines.Clear();

            // Clear underlines from the previous time
            this.IndicatorCurrent = Indicator.Error;
            this.IndicatorClearRange(0, this.TextLength);

            this.IndicatorCurrent = Indicator.Warning;
            this.IndicatorClearRange(0, this.TextLength);
        }

        internal void AddError(int startLine, int startColumn, int endLine, int endColumn, bool isWarning)
        {
            if (isWarning)
            {
                warningLines.Add(startLine);
            }
            else
            {
                errorLines.Add(startLine);
            }

            var (position, length) = GetErrorPosition(startLine, startColumn, endLine, endColumn);

            // Underline the error
            this.IndicatorCurrent = isWarning ? Indicator.Warning : Indicator.Error;
            this.IndicatorFillRange(position, length);
        }

        private Tuple<int, int> GetErrorPosition(int startLine, int startColumn, int endLine, int endColumn)
        {
            int errorPosition;
            int errorLength;

            if (endLine < 0 || endColumn < 0)
            {
                errorPosition = this.WordStartPosition(this.Lines[startLine].Position + startColumn);
                errorLength = this.GetWordFromPosition(errorPosition).Length;
            }
            else
            {
                errorPosition = this.Lines[startLine].Position + startColumn;
                errorLength = this.Lines[endLine].Position + endColumn - errorPosition;
            }

            // if error is at the end of the line (missing semi-colon), or is a stray '.'
            if (errorLength == 0 || errorPosition == this.Lines[startLine].EndPosition - 2)
            {
                errorPosition--;
                errorLength = 1;
            }

            return new Tuple<int, int>(errorPosition, errorLength);
        }
        #endregion

        #region Constants
        private static class Indicator
        {
            // 0 - 7 are reserved by Scintilla internally... used by the lexers
            internal const int Error = 8;
            internal const int Warning = 13;
            internal const int ObjectHighlight = 9;
            internal const int ObjectHighlightDef = 10;
            internal const int Rename = 11;
            internal const int Find = 12;
            internal const int Spelling = 14;

            // Rainbow Braces
            internal const int BraceColor0 = 15;
            internal const int BraceColor1 = 16;
            internal const int BraceColor2 = 17;
            internal const int BraceColor3 = 18;
            internal const int BraceColor4 = 19;
        }

        private static class LeftMargin
        {
            internal const int LineNumbers = 0;
            internal const int Bookmarks = 1;
            internal const int CodeFolding = 2;
            internal const int Padding = 3;
        }

        private static class BookmarkMargin
        {
            internal const int Marker = 3;
            internal const uint Mask = (1 << 3);
        }

        private static class Substyle
        {
            internal const int NormStyleCount = 1;
            internal const int ExtStyleCount = 6;

            internal static int Enum => enums;
            internal static int Method => methods;
            internal static int Struct => structs;
            internal static int Keyword => keywords;
            internal static int Interface => interfaces;
            internal static int ParamAndVar => paramsAndVars;

            private static int enums = -1;
            private static int methods = -1;
            private static int structs = -1;
            private static int keywords = -1;
            private static int interfaces = -1;
            private static int paramsAndVars = -1;
            private static int correction = 0;

            internal static void SetStyles(int startIndex)
            {
                keywords = startIndex;
                methods = startIndex + 1;
                structs = startIndex + 2;
                enums = startIndex + 3;
                interfaces = startIndex + 4;
                paramsAndVars = startIndex + 5;

                correction = startIndex * 2;
            }

            internal static int SubstyleCorrection(int style)
            {
                return style + correction;
            }
        }
        #endregion

        private class DocMeta
        {
            internal int ScrollPos { get; }
            internal int AnchorPos { get; }
            internal int CaretPos { get; }
            internal IEnumerable<int> FoldedLines { get; }

            internal DocMeta(int scrollPos, int anchorPos, int caretPos, IEnumerable<int> foldedLines)
            {
                this.ScrollPos = scrollPos;
                this.AnchorPos = anchorPos;
                this.CaretPos = caretPos;
                this.FoldedLines = foldedLines;
            }
        }
    }
}
