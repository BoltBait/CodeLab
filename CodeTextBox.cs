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

using ScintillaNET;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace PaintDotNet.Effects
{
    public sealed partial class CodeTextBox : Scintilla
    {
        private readonly Timer timer = new Timer();
        private readonly IntelliBox iBox = new IntelliBox();
        private readonly FindAndReplace findPanel = new FindAndReplace();
        private readonly IntelliTip intelliTip = new IntelliTip();
        private readonly IndicatorBar indicatorBar = new IndicatorBar();
        private readonly List<int> errorLines = new List<int>();
        private readonly List<int> warningLines = new List<int>();
        private readonly List<int> matchLines = new List<int>();
        private readonly SizeF dpi = new SizeF(1f, 1f);
        private readonly ToolStrip lightBulbMenu = new ToolStrip();
        private readonly ScaledToolStripDropDownButton bulbIcon = new ScaledToolStripDropDownButton();
        private readonly ToolStripMenuItem renameVarMenuItem = new ToolStripMenuItem();
        private readonly Dictionary<Guid, ScintillaNET.Document> docCollection = new Dictionary<Guid, ScintillaNET.Document>();
        private readonly Dictionary<Guid, DocMeta> docMetaCollection = new Dictionary<Guid, DocMeta>();
        private const int Preprocessor = 64;

        private enum DelayedOperation
        {
            None,
            UpdateIndicatorBar,
            ScrollCaret
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
        private int indexForPurpleWords = -1;
        private int disableIntelliTipPos = InvalidPosition;
        private DelayedOperation delayedOperation = DelayedOperation.None;
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
            get => !(this.CanUndo || this.CanRedo) && this.Text.Equals(ScriptWriter.DefaultCode);
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

                        // Selection
                        this.SetSelectionBackColor(true, Color.FromArgb(38, 79, 120));

                        // Current Line Highlight
                        this.CaretLineBackColor = Color.FromArgb(40, 40, 40);

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
                        this.Indicators[Indicator.Warning].ForeColor = Color.Green;

                        // Error
                        this.Indicators[Indicator.Error].ForeColor = Color.Red;

                        // Selection
                        this.SetSelectionBackColor(true, Color.FromArgb(173, 214, 255));

                        // Current Line Highlight
                        this.CaretLineBackColor = Color.GhostWhite;

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
                if (indexForPurpleWords > 0)
                {
                    this.Styles[indexForPurpleWords + i].ForeColor = Color.FromArgb(216, 160, 223);
                    this.Styles[indexForPurpleWords + i].BackColor = backColor;
                }
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
                if (indexForPurpleWords > 0)
                {
                    this.Styles[indexForPurpleWords + i].ForeColor = Color.FromArgb(143, 8, 196);
                    this.Styles[indexForPurpleWords + i].BackColor = backColor;
                }
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
            this.Styles[Style.Xml.XcComment].ForeColor = Color.Aquamarine;
            this.Styles[Style.Xml.XcComment].BackColor = backColor;
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
            this.Styles[Style.Xml.XcComment].ForeColor = Color.Aquamarine;
            this.Styles[Style.Xml.XcComment].BackColor = backColor;
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
                timer.Tick += (sender, e) => ParseLocalVariables(this.CurrentPosition);
                timer.Start();
            }

            using (Graphics g = this.CreateGraphics())
            {
                dpi = new SizeF(g.DpiX / 96f, g.DpiY / 96f);
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

            iBox.Size = new Size(300, 100);
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
            this.bulbIcon.DropDownItems.Add(this.renameVarMenuItem);
            this.bulbIcon.ImageName = "Bulb";
            this.bulbIcon.Name = "bulbIcon";
            this.bulbIcon.Size = new Size(29, 22);
            this.bulbIcon.Text = "Bulb Icon";
            // 
            // renameVarMenuItem
            // 
            this.renameVarMenuItem.Name = "renameVarMenuItem";
            this.renameVarMenuItem.Size = new Size(152, 22);
            this.renameVarMenuItem.Text = "Rename";
            this.renameVarMenuItem.Click += RenameButton_Click;

            #region ScintillaNET Initializers
            this.StyleResetDefault();
            this.Theme = Theme.Light;
            this.Styles[Style.Default].Font = "Consolas";
            this.Styles[Style.Default].Size = 10;
            //this.StyleClearAll();

            this.Lexer = Lexer.Cpp;

            // Set the styles for Ctrl+F Find
            this.Indicators[Indicator.Find].Style = IndicatorStyle.StraightBox;
            this.Indicators[Indicator.Find].Under = true;
            this.Indicators[Indicator.Find].OutlineAlpha = 153;
            this.Indicators[Indicator.Find].Alpha = 204;

            // Set the styles for Errors underlines
            this.Indicators[Indicator.Error].Style = IndicatorStyle.Squiggle;
            this.Indicators[Indicator.Warning].Style = IndicatorStyle.Squiggle;

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

            indexForPurpleWords = this.AllocateSubstyles(Style.Cpp.Identifier, 1);

            // Set the keywords for Syntax Highlighting
            UpdateSyntaxHighlighting();

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

            while (posIndex > 0)
            {
                char c = strippedText[posIndex - 1];

                if (!(char.IsLetterOrDigit(c) || c == '_' || c == '.'))
                {
                    return strippedText.Substring(posIndex);
                }
                else if (posIndex - 1 == 0)
                {
                    return strippedText.Substring(posIndex - 1);
                }

                posIndex--;
            }

            return string.Empty;
        }

        private int[] GetLastWordsPos(int position)
        {
            List<int> tokenPositions = new List<int>();
            int tokenPos = position;
            while (tokenPos > 0)
            {
                char c = this.GetCharAt(tokenPos - 1);

                if (c == ')' || c == '>')
                {
                    tokenPos = this.BraceMatch(tokenPos - 1) + 1;
                }
                else if (c == '.')
                {
                    tokenPositions.Add(tokenPos);
                }
                else if (!(char.IsLetterOrDigit(c) || c == '_'))
                {
                    tokenPositions.Add(tokenPos);
                    break;
                }
                else if (tokenPos - 1 == 0)
                {
                    tokenPositions.Add(tokenPos - 1);
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
                throw new ArgumentException();
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

        private void ParseLocalVariables(int position)
        {
            Intelli.Parameters.Clear();
            Intelli.Variables.Clear();
            Intelli.VarPos.Clear();

            if (this.ReadOnly)
            {
                // We can use ReadOnly to indicate the document is a Type Definition
                return;
            }

            Tuple<int, int> methodBounds = GetMethodBounds(position);
            if (methodBounds.Item1 == InvalidPosition || methodBounds.Item2 == InvalidPosition)
            {
                return;
            }

            int closeParenPos = methodBounds.Item1 - 1;
            while (closeParenPos > InvalidPosition && this.GetCharAt(closeParenPos) != ')')
            {
                closeParenPos--;
            }

            int openParenPos = this.BraceMatch(closeParenPos);
            if (openParenPos != InvalidPosition)
            {
                string methodName = this.GetWordFromPosition(openParenPos);
                IEnumerable<MethodInfo> methods = Intelli.UserScript.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly)
                    .Where(m => !m.IsVirtual && m.Name.Equals(methodName, StringComparison.Ordinal));

                if (methods.Any())
                {
                    MethodInfo methodInfo = GetOverload(methods, openParenPos);

                    foreach (ParameterInfo parameter in methodInfo.GetParameters())
                    {
                        if (!Intelli.Parameters.ContainsKey(parameter.Name))
                        {
                            Intelli.Parameters.Add(parameter.Name, parameter.ParameterType);
                        }
                    }
                }
            }

            int methodStart = methodBounds.Item1 + 1;
            int methodEnd = methodBounds.Item2 - 1;

            string bodyText = this.GetTextRange(methodStart, methodEnd - methodStart);
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

                this.SetTargetRange(methodStart, methodEnd);
                while (this.SearchInTarget(word) != InvalidPosition)
                {
                    int varPos = this.TargetEnd;
                    this.SetTargetRange(this.TargetEnd, methodEnd);

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

                        while (this.GetCharAt(varPos - 1) != '>' && varPos <= methodEnd)
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
                    while (char.IsWhiteSpace(this.GetCharAt(varPos)) && varPos <= methodEnd)
                    {
                        varPos++;
                    }

                    // find the semi-colon
                    int semiColonPos = varPos;
                    while (this.GetCharAt(semiColonPos) != ';' && semiColonPos <= methodEnd)
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
                        while (char.IsWhiteSpace(this.GetCharAt(varPos)) && varPos <= methodEnd)
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
                        if (style != Style.Cpp.Identifier && style != Style.Cpp.Identifier + Preprocessor)
                        {
                            continue;
                        }

                        string varName = this.GetWordFromPosition(thisVarPos);

                        // Ensure the variable doesn't contain illegal characters
                        if (!varName.IsCSharpIndentifier())
                        {
                            continue;
                        }

                        if (Intelli.AllTypes.ContainsKey(varName) || Intelli.Keywords.Contains(varName) || Intelli.Snippets.ContainsKey(varName) ||
                            Intelli.UserDefinedTypes.ContainsKey(varName) || Intelli.UserScript.Contains(varName, false))
                        {
                            continue;
                        }

                        if (!Intelli.VarPos.ContainsKey(varName))
                        {
                            Intelli.VarPos.Add(varName, thisVarPos);
                        }
                        else
                        {
                            Intelli.VarPos[varName] = thisVarPos;
                        }

                        if (Intelli.Parameters.ContainsKey(varName))
                        {
                            continue;
                        }

                        if (!Intelli.Variables.ContainsKey(varName))
                        {
                            Intelli.Variables.Add(varName, type);
                        }
                        else
                        {
                            Intelli.Variables[varName] = type;
                        }
                    }
                }
            }
        }

        private IntelliType GetIntelliType(int position)
        {
            int style = this.GetStyleAt(position);
            if (style != Style.Cpp.Word && style != Style.Cpp.Word + Preprocessor &&
                style != Style.Cpp.Word2 && style != Style.Cpp.Word2 + Preprocessor &&
                style != Style.Cpp.Identifier && style != Style.Cpp.Identifier + Preprocessor)
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
            ParseLocalVariables(position);
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
                        if (style == Style.Cpp.Word || style == Style.Cpp.Word + Preprocessor ||
                            style == Style.Cpp.Word2 || style == Style.Cpp.Word2 + Preprocessor)
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
                            style == Style.Cpp.Identifier || style == Style.Cpp.Identifier + Preprocessor)
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
                                MemberInfo member = Intelli.UserScript.GetMember(word, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly)[0];
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
                return $"{stringType.GetObjectType()} - {stringType.Namespace}.{stringType.Name}";
            }
            if (style == Style.Cpp.Character || style == Style.Cpp.Character + Preprocessor)
            {
                Type charType = typeof(char);
                return $"{charType.GetObjectType()} - {charType.Namespace}.{charType.Name}";
            }
            if (style == Style.Cpp.Number || style == Style.Cpp.Number + Preprocessor)
            {
                Type numType = GetNumberType(position);
                return (numType != null) ? $"{numType.GetObjectType()} - {numType.Namespace}.{numType.Name}" : string.Empty;
            }

            string lastWords = GetLastWords(position);
            if (lastWords.Length == 0)
            {
                return string.Empty;
            }

            ParseLocalVariables(position);

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
                            return $"{type.Name}.{type.Name}(){overloads}\nConstructor";
                        }

                        if (ctors.Length > 0)
                        {
                            int otherOverloads = type.IsValueType ? ctors.Length : ctors.Length - 1;
                            string overloads1 = (otherOverloads > 0) ? $" (+ {otherOverloads} overloads)" : string.Empty;

                            ConstructorInfo constructor = GetOverload(ctors, position);
                            return $"{constructor.DeclaringType.Name}.{type.Name}({constructor.Params()}){overloads1}\nConstructor";
                        }
                    }
                }

                string typeName = type.GetObjectType();

                string name = (type.IsGenericType) ? type.GetGenericName() : type.Name;

                return $"{typeName} - {type.Namespace}.{name}";
            }

            if (Intelli.UserDefinedTypes.ContainsKey(lastWords))
            {
                type = Intelli.UserDefinedTypes[lastWords];
                string typeName = type.GetObjectType();

                string name = (type.IsGenericType) ? type.GetGenericName() : type.Name;

                return $"{typeName} - {type.DeclaringType.Name}.{name}";
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

                    return $"{returnType} - {precedingType}.{property.Name}{getSet}\n{property.MemberType}";
                case MemberTypes.Method:
                    if (declaringType == Intelli.UserScript)
                    {
                        string member = this.GetWordFromPosition(position);
                        MemberInfo[] members = declaringType.GetMember(member, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
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
                    string genericContraints = string.Empty;

                    if (method.IsGenericMethod)
                    {
                        Type[] args = method.GetGenericArguments();
                        genericArgs = $"<{args.Select(t => t.GetDisplayName()).Join(", ")}>";

                        if (method.IsGenericMethodDefinition)
                        {
                            string constraints = args.GetConstraints().Join("\r\n    ");
                            if (constraints.Length > 0)
                            {
                                genericContraints = "\r\n    " + constraints;
                            }
                        }
                    }

                    returnType = method.ReturnType.GetDisplayName();
                    string overloads = (length > 1) ? $" (+ {length - 1} overloads)" : string.Empty;

                    return $"{returnType} - {precedingType}.{method.Name}{genericArgs}({method.Params()}){overloads}{genericContraints}\n{ext}{method.MemberType}";
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

                    return $"{returnType} - {precedingType}.{field.Name}{fieldValue}\n{fieldTypeName}";
                case MemberTypes.Event:
                    EventInfo eventInfo = (EventInfo)memberInfo;
                    returnType = eventInfo.EventHandlerType.GetDisplayName();

                    return $"{returnType} - {precedingType}.{eventInfo.Name}\n{eventInfo.MemberType}";
                case MemberTypes.NestedType:
                    type = (Type)memberInfo;
                    string typeName = type.GetObjectType();

                    string name = (type.IsGenericType) ? type.GetGenericName() : type.Name;

                    return $"{typeName} - {type.Namespace}.{precedingType}.{name}\nNested Type";
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

            if (mi.Count() == 1 && !(defaultOverload is MethodInfo onlyMethod && onlyMethod.IsGenericMethodDefinition))
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

            string[] paramWords = this.GetTextRange(paramStart, paramEnd - paramStart).Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            Tuple<int, int> oldRange = new Tuple<int, int>(this.TargetStart, this.TargetEnd);
            this.SearchFlags = SearchFlags.MatchCase;
            this.SetTargetRange(paramStart, paramEnd);
            List<Type> paramTypes = new List<Type>(paramWords.Length);
            for (int i = 0; i < paramWords.Length; i++)
            {
                int paramPos = InvalidPosition;
                string paramName = paramWords[i].Trim();
                if (this.SearchInTarget(paramName) != InvalidPosition)
                {
                    paramPos = this.TargetEnd;
                    this.SetTargetRange(this.TargetEnd, paramEnd);
                }

                if (paramPos == InvalidPosition)
                {
                    return defaultOverload;
                }

                Type paramType = GetReturnType(paramPos);
                if (paramType == null)
                {
                    return defaultOverload;
                }

                if (!paramType.IsByRef && (paramName.StartsWith("ref", StringComparison.Ordinal) || paramName.StartsWith("out", StringComparison.Ordinal)))
                {
                    paramType = paramType.MakeByRefType();
                }

                paramTypes.Add(paramType);
            }
            this.SetTargetRange(oldRange.Item1, oldRange.Item2);

            for (int i = 0; i < mi.Count(); i++)
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
                style != Style.Cpp.Identifier && style != Style.Cpp.Identifier + Preprocessor)
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
            string[] tokens = lastWords.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
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

            string defRef = DefinitionGenerator.Generate(type);
            string name = type.GetDisplayNameWithExclusion(type);

            OnDefTabNeeded(name, type.Namespace + "." + name);
            OnBuildNeeded();
            this.Text = defRef;
            this.ReadOnly = true;

            this.TargetWholeDocument();
            this.SearchFlags = SearchFlags.MatchCase | SearchFlags.WholeWord | SearchFlags.WordStart;
            string wordToFind = memberInfo.Name;

            if (this.SearchInTarget(wordToFind) != InvalidPosition)
            {
                this.SetSel(this.TargetStart, this.TargetEnd);
                this.delayedOperation = DelayedOperation.ScrollCaret;
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

        private bool GoToDefinitionCSharp(bool msDocs)
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
                if (msDocs)
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
                if (msDocs)
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

                if (msDocs)
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

            if (!msDocs && (Intelli.Variables.ContainsKey(lastWords) || Intelli.Parameters.ContainsKey(lastWords)) && Intelli.VarPos.ContainsKey(lastWords))
            {
                this.SelectionStart = Intelli.VarPos[lastWords];
                this.SelectionEnd = this.WordEndPosition(Intelli.VarPos[lastWords]);
                this.ScrollCaret();
                return true;
            }

            if (!msDocs && Intelli.UserDefinedTypes.ContainsKey(lastWords))
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
                        style2 != Style.Cpp.Word2 && style2 != Style.Cpp.Word2 + Preprocessor)
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

                if (msDocs)
                {
                    if (t.Namespace.StartsWith("PaintDotNet", StringComparison.Ordinal))
                    {
                        return false;
                    }

                    string typeName = (t.IsGenericType) ? t.Name.Replace("`", "-") : t.Name;
                    string fullName = $"{t.Namespace}.{typeName}";

                    OpenMsDocs(fullName);
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

            if (!msDocs && memberInfo.DeclaringType == Intelli.UserScript)
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
                        style2 != Style.Cpp.Identifier && style2 != Style.Cpp.Identifier + Preprocessor)
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

            if (msDocs)
            {
                Type declaringType = memberInfo.DeclaringType;
                if (declaringType.Namespace.StartsWith("PaintDotNet", StringComparison.Ordinal))
                {
                    return false;
                }

                string typeName = (declaringType.IsGenericType) ? declaringType.Name.Replace("`", "-") : declaringType.Name;
                string fullName = $"{declaringType.Namespace}.{typeName}.{memberInfo.Name}";

                OpenMsDocs(fullName);
            }
            else
            {
                OpenDefinitionTab(memberInfo);
            }

            return true;
        }

        private static void OpenMsDocs(string fullName)
        {
            System.Diagnostics.Process.Start($"https://docs.microsoft.com/dotnet/api/{fullName}");
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            if (iBox.Visible && iBox.MouseOver && e is HandledMouseEventArgs args)
            {
                args.Handled = true;
                int newTopIndex = iBox.TopIndex - Math.Sign(e.Delta) * SystemInformation.MouseWheelScrollLines;
                iBox.TopIndex = newTopIndex.Clamp(0, iBox.Items.Count - 1);
            }

            base.OnMouseWheel(e);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (!iBox.Visible)
            {
                if (e.KeyCode == Keys.Escape)
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
                else if (e.KeyCode == Keys.F12)
                {
                    GoToDefinition(false);
                }
                else if (e.Alt && e.KeyCode == Keys.Up)
                {
                    this.ExecuteCmd(Command.MoveSelectedLinesUp);
                }
                else if (e.Alt && e.KeyCode == Keys.Down)
                {
                    this.ExecuteCmd(Command.MoveSelectedLinesDown);
                }
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
            else if (e.KeyCode == Keys.OemPeriod)
            {
                if (iBox.Matches)
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
                if (iBox.Matches)
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
                if (iBox.Matches)
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
                if (iBox.Matches)
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
                if (iBox.SelectedIndex > 0)
                {
                    iBox.SelectedIndex--;
                }

                e.Handled = true;
                this.Focus();
            }
            else if (e.KeyCode == Keys.Down)
            {
                if (iBox.SelectedIndex < iBox.Items.Count - 1)
                {
                    iBox.SelectedIndex++;
                }

                e.Handled = true;
                this.Focus();
            }
            else if (e.KeyCode == Keys.PageDown)
            {
                if (iBox.SelectedIndex < iBox.Items.Count - 10)
                {
                    iBox.SelectedIndex += 10;
                }
                else
                {
                    iBox.SelectedIndex = iBox.Items.Count - 1;
                }

                e.Handled = true;
                this.Focus();
            }
            else if (e.KeyCode == Keys.PageUp)
            {
                if (iBox.SelectedIndex > 10)
                {
                    iBox.SelectedIndex -= 10;
                }
                else
                {
                    iBox.SelectedIndex = 0;
                }

                e.Handled = true;
                this.Focus();
            }
            else if (e.KeyCode == Keys.End)
            {
                iBox.SelectedIndex = iBox.Items.Count - 1;
                e.Handled = true;
                this.Focus();
            }
            else if (e.KeyCode == Keys.Home)
            {
                iBox.SelectedIndex = 0;
                e.Handled = true;
                this.Focus();
            }
            else if (e.KeyCode == Keys.Return)
            {
                if (iBox.Matches)
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
            if (!iBox.Visible)
            {
                base.OnKeyPress(e);
                return;
            }

            if (e.KeyChar == ')' || e.KeyChar == '[' || e.KeyChar == ']' || e.KeyChar == '{' || e.KeyChar == '}')
            {
                ConfirmIntelliBox();
                base.OnKeyPress(e);
                return;
            }

            if (e.KeyChar == '(')
            {
                ConfirmIntelliBox();
                base.OnKeyPress(e);
                this.InsertText(this.CurrentPosition, ")");
                return;
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (iBox.Visible)
            {
                iBox.Visible = false;
            }

            if (intelliTip.Visible)
            {
                intelliTip.Hide(this);
                disableIntelliTipPos = this.CharPositionFromPointClose(e.X, e.Y);
            }

            base.OnMouseDown(e);
        }

        private void MemberIntelliBox(int position)
        {
            int style = this.GetStyleAt(position - 1);
            if (style != Style.Cpp.Word && style != Style.Cpp.Word + Preprocessor &&
                style != Style.Cpp.Word2 && style != Style.Cpp.Word2 + Preprocessor &&
                style != Style.Cpp.Identifier && style != Style.Cpp.Identifier + Preprocessor &&
                style != Style.Cpp.Operator && style != Style.Cpp.Operator + Preprocessor)
            {
                return;
            }

            Type type = GetReturnType(position);

            if (type == null || type == typeof(void))
            {
                return;
            }

            bool isStatic = GetIntelliType(position - 1) == IntelliType.Type;

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
                style != Style.Cpp.Word2 && style != Style.Cpp.Word2 + Preprocessor)
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
            if (iBox.Items.Count <= 0)
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

            if (e.Change.HasFlag(UpdateChange.HScroll))
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

                if (lightBulbMenu.Visible)
                {
                    lightBulbMenu.Hide();
                }
            }

            if (e.Change.HasFlag(UpdateChange.VScroll))
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

                if (lightBulbMenu.Visible)
                {
                    lightBulbMenu.Hide();
                }

                if (!mapScroll)
                {
                    indicatorBar.Value = this.FirstVisibleLine;
                }
                else
                {
                    mapScroll = false;
                }
            }

            if (e.Change.HasFlag(UpdateChange.Content))
            {
                if (MapEnabled)
                {
                    indicatorBar.Maximum = CountUILines();
                    indicatorBar.Value = this.FirstVisibleLine;
                }
            }

            if (e.Change.HasFlag(UpdateChange.Selection))
            {
                if (this.CurrentLine != previousLine)
                {
                    if (this.SelectionStart == this.SelectionEnd)
                    {
                        string lineText = this.Lines[this.CurrentLine].Text;
                        int trimmedEndLength = lineText.TrimEnd('\r', '\n').Length;

                        if (trimmedEndLength == 0)
                        {
                            // Move to an empty (no whitespace) line
                            // Click or Up/Down Keys or Enter/Return
                            int indent = GetIndentFromPrevLine(this.CurrentLine);
                            this.Selections[0].CaretVirtualSpace = indent;
                            this.Selections[0].AnchorVirtualSpace = indent;
                            if (e.Change.HasFlag(UpdateChange.Content))
                            {
                                // Only with Enter/Return
                                this.ChooseCaretX();
                            }
                        }
                        else if (e.Change.HasFlag(UpdateChange.Content) && Math.Abs(this.CurrentLine - previousLine) == 1)
                        {
                            // Enter/Return/Backspace with characters to right of caret
                            int indent = GetIndentFromPrevLine(this.CurrentLine);

                            if (lineText.Trim().Equals("}"))
                            {
                                indent -= this.TabWidth;
                            }

                            this.Lines[this.CurrentLine].Indentation = indent;

                            int newCaretPos = this.CurrentLine > previousLine
                                ? this.Lines[this.CurrentLine].Position + indent
                                : this.Lines[this.CurrentLine].Position + trimmedEndLength;

                            this.SetEmptySelection(newCaretPos);
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

            if (e.Change.HasFlag(UpdateChange.Selection) || e.Change.HasFlag(UpdateChange.Content))
            {
                if (intelliTip.Visible)
                {
                    intelliTip.Hide(this);
                    disableIntelliTipPos = InvalidPosition;
                }

                if (MapEnabled)
                {
                    int curLine = GetVisibleLine(this.CurrentLine);
                    indicatorBar.Caret = CountVisibleLines(curLine);
                }

                // Has the caret changed position?
                int caretPos = this.CurrentPosition;
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
            if (iBox.Visible)
            {
                string word = this.GetWordFromPosition(this.CurrentPosition);
                if (this.Lexer == Lexer.Xml && e.Char == '/' && this.GetCharAt(this.CurrentPosition - 2) == '<')
                {
                    // Do nothing
                }
                else if (word.IsCSharpIndentifier())
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
                    if (base.GetCharAt(this.CurrentPosition - 2) == '{' &&
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
                    if (base.GetCharAt(this.CurrentPosition - 2) == '(' &&
                        base.GetCharAt(this.CurrentPosition) == ')')
                    {
                        this.DeleteRange(this.CurrentPosition, 1);
                    }
                }
                else if (e.Char == ']')
                {
                    if (base.GetCharAt(this.CurrentPosition - 2) == '[' &&
                        base.GetCharAt(this.CurrentPosition) == ']')
                    {
                        this.DeleteRange(this.CurrentPosition, 1);
                    }
                }
                else if (e.Char == '>')
                {
                    if (base.GetCharAt(this.CurrentPosition - 2) == '<' &&
                        base.GetCharAt(this.CurrentPosition) == '>')
                    {
                        this.DeleteRange(this.CurrentPosition, 1);
                    }
                }
                else if (e.Char == '.')
                {
                    MemberIntelliBox(this.CurrentPosition - 1);
                }
                else if (e.Char == '(')
                {
                    if (IsRightOfCaretEmpty(this.CurrentPosition))
                    {
                        this.InsertText(this.CurrentPosition, ")");
                    }

                    ConstructorIntelliBox(this.CurrentPosition - 1);
                }
                else if (e.Char == '[')
                {
                    if (IsRightOfCaretEmpty(this.CurrentPosition))
                    {
                        this.InsertText(this.CurrentPosition, "]");
                    }
                }
                else if (e.Char == '{')
                {
                    if (IsRightOfCaretEmpty(this.CurrentPosition))
                    {
                        this.InsertText(this.CurrentPosition, "}");
                    }
                }
                else if (e.Char == '<')
                {
                    if (IsRightOfCaretEmpty(this.CurrentPosition) &&
                        GetReturnType(this.CurrentPosition - 1)?.IsGenericType == true)
                    {
                        this.InsertText(this.CurrentPosition, ">");
                    }
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
                        NonMemberIntelliBox(this.CurrentPosition - 1);
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
                if (e.Char == '<')
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

            this.WhitespaceSize = GetDpiX((this.Zoom >= 2) ? 2 : 1);

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

            if (this.delayedOperation == DelayedOperation.UpdateIndicatorBar)
            {
                UpdateIndicatorBar();
            }
            else if (this.delayedOperation == DelayedOperation.ScrollCaret)
            {
                this.ScrollCaret();
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

            AdjustRenaming();

            base.OnTextChanged(e);
        }

        protected override void OnBeforeDelete(BeforeModificationEventArgs e)
        {
            if (this.Lexer == Lexer.Cpp && e.Source == ModificationSource.User && e.Text.Trim().Length > 0 && !Replacing)
            {
                SetUpRenaming(e.Position);
            }

            base.OnBeforeDelete(e);
        }

        protected override void OnBeforeInsert(BeforeModificationEventArgs e)
        {
            if (this.Lexer == Lexer.Cpp && e.Source == ModificationSource.User && e.Text.Trim().Length > 0 && !Replacing)
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

            this.delayedOperation = DelayedOperation.UpdateIndicatorBar;
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

            this.SetKeywords(1, string.Join(" ", Intelli.AllTypes.Keys) + " " + string.Join(" ", Intelli.UserDefinedTypes.Keys));
            this.SetKeywords(0, "abstract as base bool byte char checked class const decimal delegate double enum event explicit extern "
                + "false fixed float get implicit in int interface internal is lock long namespace new null object operator out override "
                + "params partial private protected public readonly ref sbyte sealed set short sizeof stackalloc static string struct "
                + "this true typeof uint unchecked unsafe ulong ushort using var virtual void volatile where");
            this.SetIdentifiers(indexForPurpleWords, "break case catch continue default do else finally for foreach goto if return throw try switch while");
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
            bool braceless = false;

            int withinCase = 0;
            bool caseStart = false;

            this.BeginUndoAction();
            for (int line = 0; line < this.Lines.Count; line++)
            {
                // Adjust Line Indentation
                string trimmedLine = linesNoComments[line].Trim();

                if (codeBlock && !trimmedLine.StartsWith("{", StringComparison.Ordinal) && !trimmedLine.StartsWith("using", StringComparison.Ordinal))
                {
                    braceless = true;
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
                if (braceless)
                {
                    braceless = false;
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
                ParseLocalVariables(this.CurrentPosition);
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
                ParseLocalVariables(this.CurrentPosition);
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
            line = line.Clamp(0, this.Lines.Count - 1);

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

        private int GetDpiX(int value) => (int)Math.Round(value * dpi.Width);

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
        #endregion

        #region Editor ToolTip Functions
        protected override void OnDwellStart(DwellEventArgs e)
        {
            base.OnDwellStart(e);

            if (this.Lexer == Lexer.Null || intelliTip.Visible ||
                e.Position == disableIntelliTipPos || e.Position == disableIntelliTipPos + 1)
            {
                return;
            }

            if (lightBulbMenu.Visible)
            {
                lightBulbMenu.Hide();
            }

            dwellWordPos = this.WordStartPosition(e.Position);

            string tooltipText = null;

            // If there's an error here, we'll show that instead
            if (ScriptBuilder.Errors.Count > 0)
            {
                int wordStartPos = this.WordStartPosition(e.Position);
                int wordEndPos = this.WordEndPosition(e.Position);
                foreach (Error error in ScriptBuilder.Errors)
                {
                    int errorPos = this.Lines[error.Line - 1].Position + error.Column;
                    if (errorPos == wordStartPos || errorPos == wordEndPos)
                    {
                        tooltipText = error.ErrorText.InsertLineBreaks(100);
                        break;
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

            if (this.IsIndicatorOn(Indicator.Rename, e.Position))
            {
                renameVarMenuItem.Text = $"Rename '{RenameInfo.Identifier}' to '{this.GetWordFromPosition(e.Position)}'";
                lightBulbMenu.Location = new Point(this.PointXFromPosition(e.Position) - lightBulbMenu.Width - 10,
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
                case ProjectType.Effect:
                case ProjectType.FileType:
                case ProjectType.Reference:
                    this.Lexer = Lexer.Cpp;
                    indexForPurpleWords = this.AllocateSubstyles(Style.Cpp.Identifier, 1);
                    this.UpdateSyntaxHighlighting();

                    this.SetProperty("fold", "1");
                    this.SetProperty("fold.compact", "0");

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

            switch (this.Lexer)
            {
                case Lexer.Cpp:
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

            Dictionary<int, int> visibleLine = new Dictionary<int, int>();

            bool wordWrapDisabled = this.WrapMode == WrapMode.None;
            bool allLinesVisible = this.Lines.AllLinesVisible;

            int count = 0;
            for (int i = 0; i < this.Lines.Count; i++)
            {
                visibleLine.Add(i, count);

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

        internal void AddError(int line, int column, bool isWarning)
        {
            if (isWarning)
            {
                warningLines.Add(line);
            }
            else
            {
                errorLines.Add(line);
            }

            int errPosition = this.WordStartPosition(this.Lines[line].Position + column);
            int errorLength = this.GetWordFromPosition(errPosition).Length;

            // if error is at the end of the line (missing semi-colon), or is a stray '.'
            if (errorLength == 0 || errPosition == this.Lines[line].EndPosition - 2)
            {
                errPosition--;
                errorLength = 1;
            }

            // Underline the error
            this.IndicatorCurrent = isWarning ? Indicator.Warning : Indicator.Error;
            this.IndicatorFillRange(errPosition, errorLength);
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
