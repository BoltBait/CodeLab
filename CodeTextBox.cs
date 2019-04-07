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
        private int posAtIBox = InvalidPosition;
        private int varToRenamePos = InvalidPosition;
        private string varToRename = string.Empty;
        private readonly List<int> errorLines = new List<int>();
        private readonly List<int> matchLines = new List<int>();
        private readonly SizeF dpi = new SizeF(1f, 1f);
        private Theme theme;
        private const int Preprocessor = 64;
        private int indexForPurpleWords = -1;

        private readonly ToolStrip lightBulbMenu = new ToolStrip();
        private readonly ScaledToolStripDropDownButton bulbIcon = new ScaledToolStripDropDownButton();
        private readonly ToolStripMenuItem renameVarMenuItem = new ToolStripMenuItem();

        private readonly Dictionary<Guid, ScintillaNET.Document> docCollection = new Dictionary<Guid, ScintillaNET.Document>();

        #region Properties
        internal int[] Bookmarks
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
                for (int i = 0; i < value.Length; i++)
                {
                    this.Lines[value[i]].MarkerAdd(BookmarkMargin.Marker);
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

        [Category("Appearance")]
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
                        this.Styles[Style.Default].BackColor = Color.FromArgb(30, 30, 30);

                        // Configure the CPP (C#) lexer styles
                        for (int i = 0; i <= Preprocessor; i += Preprocessor)
                        {
                            this.Styles[Style.Cpp.Default + i].ForeColor = Color.White;
                            this.Styles[Style.Cpp.Default + i].BackColor = Color.FromArgb(30, 30, 30);
                            this.Styles[Style.Cpp.Identifier + i].ForeColor = Color.Gainsboro;
                            this.Styles[Style.Cpp.Identifier + i].BackColor = Color.FromArgb(30, 30, 30);
                            this.Styles[Style.Cpp.Comment + i].ForeColor = Color.FromArgb(87, 166, 74); // Green
                            this.Styles[Style.Cpp.Comment + i].BackColor = Color.FromArgb(30, 30, 30);
                            this.Styles[Style.Cpp.CommentLine + i].ForeColor = Color.FromArgb(87, 166, 74); // Green
                            this.Styles[Style.Cpp.CommentLine + i].BackColor = Color.FromArgb(30, 30, 30);
                            this.Styles[Style.Cpp.CommentLineDoc + i].ForeColor = Color.Gray;
                            this.Styles[Style.Cpp.CommentLineDoc + i].BackColor = Color.FromArgb(30, 30, 30);
                            this.Styles[Style.Cpp.Number + i].ForeColor = Color.FromArgb(181, 206, 168);
                            this.Styles[Style.Cpp.Number + i].BackColor = Color.FromArgb(30, 30, 30);
                            this.Styles[Style.Cpp.Word2 + i].ForeColor = Color.FromArgb(78, 201, 176);
                            this.Styles[Style.Cpp.Word2 + i].BackColor = Color.FromArgb(30, 30, 30);
                            this.Styles[Style.Cpp.Word + i].ForeColor = Color.FromArgb(86, 156, 214);
                            this.Styles[Style.Cpp.Word + i].BackColor = Color.FromArgb(30, 30, 30);
                            this.Styles[Style.Cpp.String + i].ForeColor = Color.FromArgb(214, 157, 133); // Red
                            this.Styles[Style.Cpp.String + i].BackColor = Color.FromArgb(30, 30, 30);
                            this.Styles[Style.Cpp.Character + i].ForeColor = Color.FromArgb(214, 157, 133); // Red
                            this.Styles[Style.Cpp.Character + i].BackColor = Color.FromArgb(30, 30, 30);
                            this.Styles[Style.Cpp.Verbatim + i].ForeColor = Color.FromArgb(214, 157, 133); // Red
                            this.Styles[Style.Cpp.Verbatim + i].BackColor = Color.FromArgb(30, 30, 30);
                            this.Styles[Style.Cpp.StringEol + i].ForeColor = Color.FromArgb(214, 157, 133); // Red
                            this.Styles[Style.Cpp.StringEol + i].BackColor = Color.FromArgb(30, 30, 30);
                            this.Styles[Style.Cpp.Operator + i].ForeColor = Color.FromArgb(180, 180, 180);
                            this.Styles[Style.Cpp.Operator + i].BackColor = Color.FromArgb(30, 30, 30);
                            this.Styles[Style.Cpp.Preprocessor + i].ForeColor = Color.FromArgb(155, 155, 155);
                            this.Styles[Style.Cpp.Preprocessor + i].BackColor = Color.FromArgb(30, 30, 30);
                            this.Styles[Style.Cpp.Regex + i].ForeColor = Color.Gainsboro;
                            this.Styles[Style.Cpp.Regex + i].BackColor = Color.FromArgb(30, 30, 30);
                            if (indexForPurpleWords > 0)
                            {
                                this.Styles[indexForPurpleWords + i].ForeColor = Color.FromArgb(216, 160, 223);
                                this.Styles[indexForPurpleWords + i].BackColor = Color.FromArgb(30, 30, 30);
                            }
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
                        this.Styles[Style.Default].BackColor = Color.White;

                        // Configure the CPP (C#) lexer styles
                        for (int i = 0; i <= Preprocessor; i += Preprocessor)
                        {
                            this.Styles[Style.Cpp.Default + i].ForeColor = Color.Black;
                            this.Styles[Style.Cpp.Default + i].BackColor = Color.White;
                            this.Styles[Style.Cpp.Identifier + i].ForeColor = Color.Black;
                            this.Styles[Style.Cpp.Identifier + i].BackColor = Color.White;
                            this.Styles[Style.Cpp.Comment + i].ForeColor = Color.Green;
                            this.Styles[Style.Cpp.Comment + i].BackColor = Color.White;
                            this.Styles[Style.Cpp.CommentLine + i].ForeColor = Color.Green;
                            this.Styles[Style.Cpp.CommentLine + i].BackColor = Color.White;
                            this.Styles[Style.Cpp.CommentLineDoc + i].ForeColor = Color.Gray;
                            this.Styles[Style.Cpp.CommentLineDoc + i].BackColor = Color.White;
                            this.Styles[Style.Cpp.Number + i].ForeColor = Color.Black;
                            this.Styles[Style.Cpp.Number + i].BackColor = Color.White;
                            this.Styles[Style.Cpp.Word2 + i].ForeColor = Color.FromArgb(43, 145, 175);
                            this.Styles[Style.Cpp.Word2 + i].BackColor = Color.White;
                            this.Styles[Style.Cpp.Word + i].ForeColor = Color.Blue;
                            this.Styles[Style.Cpp.Word + i].BackColor = Color.White;
                            this.Styles[Style.Cpp.String + i].ForeColor = Color.FromArgb(163, 21, 21); // Red
                            this.Styles[Style.Cpp.String + i].BackColor = Color.White;
                            this.Styles[Style.Cpp.Character + i].ForeColor = Color.FromArgb(163, 21, 21); // Red
                            this.Styles[Style.Cpp.Character + i].BackColor = Color.White;
                            this.Styles[Style.Cpp.Verbatim + i].ForeColor = Color.FromArgb(128, 0, 0); // Red
                            this.Styles[Style.Cpp.Verbatim + i].BackColor = Color.White;
                            this.Styles[Style.Cpp.StringEol + i].ForeColor = Color.FromArgb(163, 21, 21); // Red
                            this.Styles[Style.Cpp.StringEol + i].BackColor = Color.White;
                            this.Styles[Style.Cpp.Operator + i].ForeColor = Color.Black;
                            this.Styles[Style.Cpp.Operator + i].BackColor = Color.White;
                            this.Styles[Style.Cpp.Preprocessor + i].ForeColor = Color.Gray;
                            this.Styles[Style.Cpp.Preprocessor + i].BackColor = Color.White;
                            this.Styles[Style.Cpp.Regex + i].ForeColor = Color.Black;
                            this.Styles[Style.Cpp.Regex + i].BackColor = Color.White;
                            if (indexForPurpleWords > 0)
                            {
                                this.Styles[indexForPurpleWords + i].ForeColor = Color.FromArgb(143, 8, 196);
                                this.Styles[indexForPurpleWords + i].BackColor = Color.White;
                            }
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
        #endregion

        #region Event Handlers
        public event EventHandler BuildNeeded;
        private void OnBuildNeeded()
        {
            this.BuildNeeded?.Invoke(this, EventArgs.Empty);
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
            this.renameVarMenuItem.Click += renameVar_Click;

            #region ScintillaNET Initializers
            this.StyleResetDefault();
            this.Theme = Theme.Light;
            this.Styles[Style.Default].Font = "Courier New";
            this.Styles[Style.Default].Size = 10;
            //this.StyleClearAll();

            this.Lexer = Lexer.Cpp;

            // Set the styles for Ctrl+F Find
            this.Indicators[Indicator.Find].Style = IndicatorStyle.StraightBox;
            this.Indicators[Indicator.Find].Under = true;
            this.Indicators[Indicator.Find].OutlineAlpha = 153;
            this.Indicators[Indicator.Find].Alpha = 204;

            // Set the styles for Errors underlines
            this.Indicators[Indicator.Error].Style = IndicatorStyle.SquiggleLow;

            // Set the styles for focused Object
            this.Indicators[Indicator.ObjectHighlight].Style = IndicatorStyle.StraightBox;
            this.Indicators[Indicator.ObjectHighlight].Under = true;
            this.Indicators[Indicator.ObjectHighlight].Alpha = 204;
            this.Indicators[Indicator.ObjectHighlight].OutlineAlpha = 255;

            // Set the styles for variable rename
            this.Indicators[Indicator.VariableRename].Style = IndicatorStyle.DotBox;
            this.Indicators[Indicator.VariableRename].Under = true;
            this.Indicators[Indicator.VariableRename].ForeColor = Color.DimGray;
            this.Indicators[Indicator.VariableRename].OutlineAlpha = 255;
            this.Indicators[Indicator.VariableRename].Alpha = 0;

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

            // Word Wrap
            this.WrapMode = WrapMode.None;
            this.WrapIndentMode = WrapIndentMode.Indent;
            this.WrapVisualFlags = WrapVisualFlags.None;

            // Zoom
            this.Zoom = 0;

            // ToolTip Delay (ms)
            this.MouseDwellTime = 250;

            // Free up default HotKeys, so they can be used for other things
            this.ClearCmdKey(Keys.Control | Keys.F); // Find
            this.ClearCmdKey(Keys.Control | Keys.H); // Find and Replace
            this.ClearCmdKey(Keys.Control | Keys.J); // Open IntelliBox
            this.ClearCmdKey(Keys.Control | Keys.Q); // Format Document
            this.ClearCmdKey(Keys.Control | Keys.G); // unassigned
            this.ClearCmdKey(Keys.Control | Keys.P); // Preview Effect

            // CaretLineVisibleAlways
            this.CaretLineVisibleAlways = true;

            // Disable scintilla's own context menu
            this.UsePopup(false);
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
            string strippedText = this.GetTextRange(0, position).StripParens();

            int posIndex = strippedText.Length;

            while (posIndex > 0)
            {
                char c = strippedText[posIndex - 1];

                if (!(char.IsLetterOrDigit(c) || c == '_' || c == '.'))
                {
                    return strippedText.Substring(posIndex, strippedText.Length - posIndex);
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
                char c = this.GetCharAt(tokenPos - 1).ToChar();

                if (c == ')')
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
                char curChar = this.GetCharAt(numEnd).ToChar();

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

            char suffix = this.GetCharAt(numEnd + 1).ToChar().ToUpperInvariant();
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
                    return (this.GetCharAt(numEnd + 2).ToChar().ToUpperInvariant() == 'L') ? typeof(ulong) : typeof(uint);
                default:
                    return (foundDecimal) ? typeof(double) : typeof(int);
            }
        }

        private Tuple<int, int> GetMethodBounds(int position)
        {
            bool insideMethod = false;
            this.SetTargetRange(0, position);
            while (this.SearchInTarget("{") != InvalidPosition)
            {
                int openBrace = this.TargetStart;
                int closeBrace = this.BraceMatch(this.TargetStart);

                if (closeBrace == InvalidPosition)
                {
                    break;
                }

                if (position > openBrace && position < closeBrace)
                {
                    insideMethod = true;
                    break;
                }

                if (closeBrace >= position)
                {
                    break;
                }

                this.SetTargetRange(closeBrace, position);
            }

            if (!insideMethod)
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

        private void ParseLocalVariables(int position)
        {
            Intelli.Variables.Clear();
            Intelli.VarPos.Clear();
            Tuple<int, int> methodBounds = GetMethodBounds(position);
            if (methodBounds.Item1 == InvalidPosition || methodBounds.Item2 == InvalidPosition)
            {
                return;
            }
            int methodStart = methodBounds.Item1 + 1;
            int methodEnd = methodBounds.Item2 - 1;

            string lastWords = this.GetTextRange(methodStart, methodEnd);
            var docWords = lastWords.Split(new char[] { ' ', '(', '{', '<', '\n' }, StringSplitOptions.RemoveEmptyEntries).Distinct();

            this.SearchFlags = SearchFlags.MatchCase | SearchFlags.WholeWord;
            foreach (string word in docWords)
            {
                Type type;
                if (Intelli.AllTypes.ContainsKey(word))
                {
                    type = Intelli.AllTypes[word];
                }
                else if (Intelli.UserDefinedTypes.ContainsKey(word))
                {
                    type = Intelli.UserDefinedTypes[word];
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
                        if (this.GetCharAt(varPos).ToChar() != '<')
                        {
                            continue;
                        }

                        while (this.GetCharAt(varPos - 1).ToChar() != '>' && varPos <= methodEnd)
                        {
                            varPos++;
                        }
                    }

                    // Ensure there's at least one space after the type
                    if (!char.IsWhiteSpace(this.GetCharAt(varPos).ToChar()))
                    {
                        continue;
                    }

                    // Skip over white space
                    while (char.IsWhiteSpace(this.GetCharAt(varPos).ToChar()) && varPos <= methodEnd)
                    {
                        varPos++;
                    }

                    // find the semi-colon
                    int semiColonPos = varPos;
                    while (this.GetCharAt(semiColonPos).ToChar() != ';' && semiColonPos <= methodEnd)
                    {
                        semiColonPos++;
                    }

                    string varRange = this.GetTextRange(varPos, semiColonPos - varPos);
                    string[] possibleVars = varRange.StripBraces().StripParens().Split(new char[] { ',' });
                    MatchCollection braceMatches = new Regex(@"\{(?:\{[^{}]*\}|[^{}])*\}").Matches(varRange);

                    int varCount = possibleVars.Length;
                    if (braceMatches.Count > 0 && possibleVars.Length != braceMatches.Count)
                    {
                        varCount = 1;
                    }

                    for (int i = 0; i < varCount; i++)
                    {
                        while (char.IsWhiteSpace(this.GetCharAt(varPos).ToChar()) && varPos <= methodEnd)
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

            string lastWords = GetLastWords(this.WordEndPosition(position, true));
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

            // since we have periods in our last works, let's see what we can do...
            char[] separator = { '.' };
            string[] tokens = lastWords.Split(separator, StringSplitOptions.RemoveEmptyEntries);
            if (tokens.Length == 0)
            {
                return IntelliType.None;
            }

            int[] tokenPos = GetLastWordsPos(position);

            if (tokens.Length != tokenPos.Length)
            {
                return IntelliType.None;
            }

            // if the first token is a variable of a known type
            Type type;
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
            }
            else if (Intelli.UserDefinedTypes.ContainsKey(tokens[0]))
            {
                type = Intelli.UserDefinedTypes[tokens[0]];
            }
            else if (Intelli.UserScript.Contains(tokens[0], false))
            {
                type = Intelli.UserScript;
                string newTokens = type.Name + "." + string.Join(".", tokens);
                tokens = newTokens.Split(separator, StringSplitOptions.RemoveEmptyEntries);

                int[] newPositions = new int[tokenPos.Length + 1];
                newPositions[0] = InvalidPosition;
                Array.Copy(tokenPos, 0, newPositions, 1, tokenPos.Length);
                tokenPos = newPositions;
            }
            else
            {
                return IntelliType.None;
            }

            for (int i = 1; i < tokens.Length; i++)
            {
                // get the member information for this token
                MemberInfo[] mi = (i == 1 && type == Intelli.UserScript) ?
                    type.GetMember(tokens[i], BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic) :
                    type.GetMember(tokens[i]);

                if (mi.Length == 0)
                {
                    return IntelliType.None;
                }

                if (i == tokens.Length - 1)
                {
                    // We at the last iteration. Get the IntelliType
                    switch (mi[0].MemberType)
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
                }
                else
                {
                    int memberIndex = 0;
                    if (mi[0].MemberType == MemberTypes.Method)
                    {
                        memberIndex = GetOverload(mi, tokenPos[i]).Item1;
                    }

                    type = mi[memberIndex].GetReturnType();
                    // stop to prevent null ref on next iteration
                    if (type == null)
                    {
                        return IntelliType.None;
                    }
                }
            }

            return IntelliType.None;
        }

        private void HighlightWordUsage()
        {
            this.IndicatorCurrent = Indicator.ObjectHighlight;
            this.IndicatorClearRange(0, this.TextLength);

            this.IndicatorCurrent = Indicator.ObjectHighlightDef;
            this.IndicatorClearRange(0, this.TextLength);

            int position = this.CurrentPosition;
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
                        int typePos = this.TargetStart;

                        // Skip over white space
                        while (char.IsWhiteSpace(this.GetCharAt(typePos - 1).ToChar()) && typePos > InvalidPosition)
                        {
                            typePos--;
                        }

                        //int style = this.GetStyleAt(typePos);
                        //if (style == Style.Cpp.Word || style == Style.Cpp.Word + Preprocessor ||
                        //    style == Style.Cpp.Word2 || style == Style.Cpp.Word2 + Preprocessor)
                        //{
                            string foundType = this.GetWordFromPosition(typePos);

                            Type t = Intelli.UserDefinedTypes[word];
                            string baseType = t.GetObjectType();

                            if (baseType == foundType)
                            {
                                this.IndicatorCurrent = Indicator.ObjectHighlightDef;
                            }
                        //}

                    }
                    else if (Intelli.UserScript.Contains(word, true))
                    {
                        int typePos = this.TargetStart;

                        // Skip over white space
                        while (char.IsWhiteSpace(this.GetCharAt(typePos - 1).ToChar()) && typePos > InvalidPosition)
                        {
                            typePos--;
                        }

                        //int style = this.GetStyleAt(typePos);
                        //if (style == Style.Cpp.Word || style == Style.Cpp.Word + Preprocessor ||
                        //    style == Style.Cpp.Word2 || style == Style.Cpp.Word2 + Preprocessor ||
                        //    style == Style.Cpp.Identifier || style == Style.Cpp.Identifier + Preprocessor)
                        //{
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

                                if (returnType.Length > 0 && t.GetDisplayName() == returnType)
                                {
                                    this.IndicatorCurrent = Indicator.ObjectHighlightDef;
                                }
                            }
                        //}
                    }
                    #endregion

                    this.IndicatorFillRange(this.TargetStart, this.TargetEnd - this.TargetStart);
                }

                // Search the remainder of the document
                this.SetTargetRange(this.TargetEnd, rangeEnd);
            }
        }

        private string GetIntelliTip(int position)
        {
            int style = this.GetStyleAt(position);
            if (style == Style.Cpp.Comment || style == Style.Cpp.Comment + Preprocessor ||
                style == Style.Cpp.CommentLine || style == Style.Cpp.CommentLine + Preprocessor ||
                style == Style.Cpp.Preprocessor || style == Style.Cpp.Preprocessor + Preprocessor ||
                style == Style.Cpp.Operator || style == Style.Cpp.Operator + Preprocessor)
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


            string lastWords = GetLastWords(this.WordEndPosition(position, true));
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

            // since we have periods in our last works, let's see what we can do...
            char[] separator = { '.' };
            string[] tokens = lastWords.Split(separator, StringSplitOptions.RemoveEmptyEntries);
            int[] tokenPos = GetLastWordsPos(position);

            if (tokens.Length != tokenPos.Length)
            {
                return string.Empty;
            }

            // if the first token is a variable of a known type
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
            }
            else if (Intelli.UserDefinedTypes.ContainsKey(tokens[0]))
            {
                type = Intelli.UserDefinedTypes[tokens[0]];
            }
            else if (Intelli.UserScript.Contains(tokens[0], false))
            {
                type = Intelli.UserScript;
                string newTokens = type.Name + "." + string.Join(".", tokens);
                tokens = newTokens.Split(separator, StringSplitOptions.RemoveEmptyEntries);

                int[] newPositions = new int[tokenPos.Length + 1];
                newPositions[0] = InvalidPosition;
                Array.Copy(tokenPos, 0, newPositions, 1, tokenPos.Length);
                tokenPos = newPositions;
            }
            else
            {
                return string.Empty;
            }

            for (int i = 1; i < tokens.Length; i++)
            {
                // get the member information for this token
                MemberInfo[] mi = (i == 1 && type == Intelli.UserScript) ?
                    type.GetMember(tokens[i], BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic) :
                    type.GetMember(tokens[i]);

                if (mi.Length == 0)
                {
                    return string.Empty;
                }

                if (i == tokens.Length - 1)
                {
                    // We at the last iteration. Get information for the toolTip

                    string precedingType = mi[0].DeclaringType.GetDisplayName();
                    string returnType = string.Empty;

                    switch (mi[0].MemberType)
                    {
                        case MemberTypes.Property:
                            returnType = ((PropertyInfo)mi[0]).PropertyType.GetDisplayName();
                            string getSet = ((PropertyInfo)mi[0]).GetterSetter();

                            return $"{returnType} - {precedingType}.{mi[0].Name}{getSet}\n{mi[0].MemberType}";
                        case MemberTypes.Method:
                            if (type == Intelli.UserScript)
                            {
                                mi = type.GetMember(tokens[i], BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
                                if (mi.Length == 0)
                                {
                                    return string.Empty;
                                }
                            }

                            Tuple<int, string> method = GetOverload(mi, position);
                            returnType = ((MethodInfo)mi[method.Item1]).ReturnType.GetDisplayName();
                            string overloads = (mi.Length > 1) ? $" (+ {mi.Length - 1} overloads)" : string.Empty;

                            return $"{returnType} - {precedingType}.{mi[method.Item1].Name}({method.Item2}){overloads}\n{mi[0].MemberType}";
                        case MemberTypes.Field:
                            FieldInfo field = (FieldInfo)mi[0];
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
                                fieldValue = $" ({field.GetValue(null)})";
                            }
                            else
                            {
                                fieldTypeName = "Field";
                                fieldValue = $" ( {field.GetValue(null)} )";
                            }

                            return $"{returnType} - {precedingType}.{mi[0].Name}{fieldValue}\n{fieldTypeName}";
                        case MemberTypes.Event:
                            returnType = ((EventInfo)mi[0]).EventHandlerType.GetDisplayName();

                            return $"{returnType} - {precedingType}.{mi[0].Name}\n{mi[0].MemberType}";
                        case MemberTypes.NestedType:
                            type = (Type)mi[0];
                            string typeName = type.GetObjectType();

                            string name = (type.IsGenericType) ? type.GetGenericName() : type.Name;

                            return $"{typeName} - {type.Namespace}.{precedingType}.{name}\nNested Type";
                    }
                }
                else
                {
                    int memberIndex = 0;
                    if (mi[0].MemberType == MemberTypes.Method)
                    {
                        memberIndex = GetOverload(mi, tokenPos[i]).Item1;
                    }

                    type = mi[memberIndex].GetReturnType();
                    if (type == null)
                    {
                        return string.Empty;
                    }
                }
            }

            return string.Empty;
        }

        private Tuple<int, string> GetOverload(MemberInfo[] mi, int position)
        {
            Tuple<int, string> defaultOverload = new Tuple<int, string>(0, ((MethodInfo)mi[0]).Params());

            if (mi.Length == 1)
            {
                return defaultOverload;
            }

            int paramStart = this.WordEndPosition(position, true);
            if (this.GetCharAt(paramStart).ToChar() != '(')
            {
                return defaultOverload;
            }

            int paramEnd = this.BraceMatch(paramStart);
            if (paramEnd == InvalidPosition)
            {
                return defaultOverload;
            }
            paramStart++;

            string[] paramArray = this.GetTextRange(paramStart, paramEnd - paramStart).Split(',');

            Tuple<int, int> oldRange = new Tuple<int, int>(this.TargetStart, this.TargetEnd);
            this.SearchFlags = SearchFlags.MatchCase;
            for (int i = 0; i < mi.Length; i++)
            {
                ParameterInfo[] paramInfo = ((MethodInfo)mi[i]).GetParameters();
                if (paramInfo.Length != paramArray.Length)
                {
                    continue;
                }

                bool match = true;
                List<string> paraNames = new List<string>();

                this.SetTargetRange(paramStart, paramEnd);
                for (int j = 0; j < paramInfo.Length; j++)
                {
                    int paramPos = InvalidPosition;
                    if (this.SearchInTarget(paramArray[j].Trim()) != InvalidPosition)
                    {
                        paramPos = this.TargetEnd;
                        this.SetTargetRange(this.TargetEnd, paramEnd);
                    }

                    if (paramPos == InvalidPosition)
                    {
                        match = false;
                        break;
                    }

                    Type varType = GetReturnType(paramPos);
                    if (varType == null || !(varType == paramInfo[j].ParameterType || varType.IsSubclassOf(paramInfo[j].ParameterType)))
                    {
                        match = false;
                        break;
                    }

                    ParameterInfo param = paramInfo[j];
                    paraNames.Add($"{(param.ParameterType.IsByRef ? "ref " : string.Empty)}{param.ParameterType.GetDisplayName()} {param.Name}");
                }

                if (!match)
                {
                    continue;
                }

                this.SetTargetRange(oldRange.Item1, oldRange.Item2);
                return new Tuple<int, string>(i, paraNames.Join(", "));
            }

            this.SetTargetRange(oldRange.Item1, oldRange.Item2);
            return defaultOverload;
        }

        private Type GetReturnType(int position)
        {
            int style = this.GetStyleAt(position - 1);
            if (style == Style.Cpp.Comment || style == Style.Cpp.Comment + Preprocessor ||
                style == Style.Cpp.CommentLine || style == Style.Cpp.CommentLine + Preprocessor ||
                style == Style.Cpp.Preprocessor || style == Style.Cpp.Preprocessor + Preprocessor)
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

            string[] tokens = lastWords.Split('.');
            int[] tokenPos = GetLastWordsPos(position);

            if (tokens.Length != tokenPos.Length)
            {
                return null;
            }

            Type type = null;
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
            }
            else if (Intelli.UserDefinedTypes.ContainsKey(tokens[0]))
            {
                type = Intelli.UserDefinedTypes[tokens[0]];
            }
            else if (Intelli.UserScript.Contains(tokens[0], false))
            {
                type = Intelli.UserScript;
                string newTokens = type.Name + "." + string.Join(".", tokens);
                tokens = newTokens.Split('.');

                int[] newPositions = new int[tokenPos.Length + 1];
                newPositions[0] = InvalidPosition;
                Array.Copy(tokenPos, 0, newPositions, 1, tokenPos.Length);
                tokenPos = newPositions;
            }
            else
            {
                return null;
            }

            for (int i = 1; i < tokens.Length; i++)
            {
                // get the member information for this token
                MemberInfo[] mi = (i == 1 && type == Intelli.UserScript) ?
                    type.GetMember(tokens[i], BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic) :
                    type.GetMember(tokens[i]);

                if (mi.Length == 0)
                {
                    return null;
                }

                int memberIndex = 0;
                if (mi[0].MemberType == MemberTypes.Method)
                {
                    memberIndex = GetOverload(mi, tokenPos[i]).Item1;
                }

                // extract type info for token
                type = mi[memberIndex].GetReturnType();
                if (type == null)
                {
                    return null;
                }
            }

            return type;
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

            string lastWords = GetLastWords(this.WordEndPosition(position, true));
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

            // since we have periods in our last works, let's see what we can do...
            char[] separator = { '.' };
            string[] tokens = lastWords.Split(separator, StringSplitOptions.RemoveEmptyEntries);
            if (tokens.Length == 0)
            {
                return null;
            }

            int[] tokenPos = GetLastWordsPos(position);

            if (tokens.Length != tokenPos.Length)
            {
                return null;
            }

            // if the first token is a variable of a known type
            Type type;
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
            }
            else if (Intelli.UserDefinedTypes.ContainsKey(tokens[0]))
            {
                type = Intelli.UserDefinedTypes[tokens[0]];
            }
            else if (Intelli.UserScript.Contains(tokens[0], false))
            {
                type = Intelli.UserScript;
                string newTokens = type.Name + "." + string.Join(".", tokens);
                tokens = newTokens.Split(separator, StringSplitOptions.RemoveEmptyEntries);

                int[] newPositions = new int[tokenPos.Length + 1];
                newPositions[0] = InvalidPosition;
                Array.Copy(tokenPos, 0, newPositions, 1, tokenPos.Length);
                tokenPos = newPositions;
            }
            else
            {
                return null;
            }

            for (int i = 1; i < tokens.Length; i++)
            {
                // get the member information for this token
                MemberInfo[] mi = (i == 1 && type == Intelli.UserScript) ?
                    type.GetMember(tokens[i], BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic) :
                    type.GetMember(tokens[i]);

                if (mi.Length == 0)
                {
                    return null;
                }

                if (i == tokens.Length - 1)
                {
                    // We at the last iteration. Get the DeclaringType
                    return mi[0].DeclaringType;
                }
                else
                {
                    int memberIndex = 0;
                    if (mi[0].MemberType == MemberTypes.Method)
                    {
                        memberIndex = GetOverload(mi, tokenPos[i]).Item1;
                    }

                    type = mi[memberIndex].GetReturnType();

                    // stop to prevent null ref on next iteration
                    if (type == null)
                    {
                        return null;
                    }
                }
            }

            return null;
        }

        private bool GoToDefinition()
        {
            int position = this.CurrentPosition;

            int style = this.GetStyleAt(position);
            if (style == Style.Cpp.Comment || style == Style.Cpp.Comment + Preprocessor ||
                style == Style.Cpp.CommentLine || style == Style.Cpp.CommentLine + Preprocessor ||
                style == Style.Cpp.Preprocessor || style == Style.Cpp.Preprocessor + Preprocessor ||
                style == Style.Cpp.Operator || style == Style.Cpp.Operator + Preprocessor)
            {
                return false;
            }
            if (style == Style.Cpp.String || style == Style.Cpp.String + Preprocessor ||
                style == Style.Cpp.StringEol || style == Style.Cpp.StringEol + Preprocessor ||
                style == Style.Cpp.Verbatim || style == Style.Cpp.Verbatim + Preprocessor)
            {
                Type stringType = typeof(string);
                string fullName = $"{stringType.Namespace}.{stringType.Name}";

                OpenMsDocs(fullName);
                return true;
            }
            if (style == Style.Cpp.Character || style == Style.Cpp.Character + Preprocessor)
            {
                Type charType = typeof(char);
                string fullName = $"{charType.Namespace}.{charType.Name}";

                OpenMsDocs(fullName);
                return true;
            }
            if (style == Style.Cpp.Number || style == Style.Cpp.Number + Preprocessor)
            {
                Type numType = GetNumberType(position);
                if (numType != null)
                {
                    string fullName = $"{numType.Namespace}.{numType.Name}";

                    OpenMsDocs(fullName);
                    return true;
                }

                return false;
            }

            string lastWords = GetLastWords(this.WordEndPosition(position, true));
            if (lastWords.Length == 0)
            {
                return false;
            }

            if ((Intelli.Variables.ContainsKey(lastWords) || Intelli.Parameters.ContainsKey(lastWords)) && Intelli.VarPos.ContainsKey(lastWords))
            {
                this.SelectionStart = Intelli.VarPos[lastWords];
                this.SelectionEnd = this.WordEndPosition(Intelli.VarPos[lastWords], true);
                this.ScrollCaret();
                return true;
            }

            if (Intelli.UserDefinedTypes.ContainsKey(lastWords))
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
                    while (char.IsWhiteSpace(this.GetCharAt(typePos).ToChar()) && typePos <= this.TextLength)
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
                    this.SelectionEnd = this.WordEndPosition(typePos, true);
                    this.ScrollCaret();

                    break;
                }

                return found;
            }

            if (Intelli.AllTypes.ContainsKey(lastWords))
            {
                Type t = Intelli.AllTypes[lastWords];
                string typeName = (t.IsGenericType) ? t.Name.Replace("`", "-") : t.Name;
                string fullName = $"{t.Namespace}.{typeName}";

                if (fullName.Length == 0 || fullName.StartsWith("PaintDotNet", StringComparison.Ordinal))
                {
                    return false;
                }

                OpenMsDocs(fullName);
                return true;
            }

            // since we have periods in our last works, let's see what we can do...
            char[] separator = { '.' };
            string[] tokens = lastWords.Split(separator, StringSplitOptions.RemoveEmptyEntries);
            int[] tokenPos = GetLastWordsPos(position);

            if (tokens.Length != tokenPos.Length)
            {
                return false;
            }

            Type type;
            // if the first token is a variable of a known type
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
            }
            else if (Intelli.UserScript.Contains(tokens[0], false))
            {
                type = Intelli.UserScript;
                string newTokens = type.Name + "." + string.Join(".", tokens);
                tokens = newTokens.Split(separator, StringSplitOptions.RemoveEmptyEntries);

                int[] newPositions = new int[tokenPos.Length + 1];
                newPositions[0] = InvalidPosition;
                Array.Copy(tokenPos, 0, newPositions, 1, tokenPos.Length);
                tokenPos = newPositions;
            }
            else
            {
                return false;
            }

            for (int i = 1; i < tokens.Length; i++)
            {
                // get the member information for this token
                MemberInfo[] mi = (i == 1 && type == Intelli.UserScript) ?
                    type.GetMember(tokens[i], BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic) :
                    type.GetMember(tokens[i]);

                if (mi.Length == 0)
                {
                    return false;
                }

                if (i == tokens.Length - 1)
                {
                    string typeName = (type.IsGenericType) ? mi[0].DeclaringType.Name.Replace("`", "-") : mi[0].DeclaringType.Name;
                    string fullName = $"{mi[0].DeclaringType.Namespace}.{typeName}.{mi[0].Name}";

                    if (fullName.Length == 0)
                    {
                        return false;
                    }

                    if (mi[0].DeclaringType == Intelli.UserScript)
                    {
                        string returnType = mi[0].GetReturnType()?.GetDisplayName();

                        if (returnType.Length == 0)
                        {
                            return false;
                        }

                        bool found = false;
                        this.TargetWholeDocument();
                        this.SearchFlags = SearchFlags.MatchCase | SearchFlags.WholeWord;
                        while (this.SearchInTarget(lastWords) != InvalidPosition)
                        {
                            int typePos = this.TargetStart;
                            int memberPos = this.TargetStart;
                            this.SetTargetRange(this.TargetEnd, this.TextLength);

                            // Skip over white space
                            while (char.IsWhiteSpace(this.GetCharAt(typePos - 1).ToChar()) && typePos > InvalidPosition)
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
                            this.SelectionEnd = this.WordEndPosition(memberPos, true);
                            this.ScrollCaret();

                            break;
                        }

                        return found;
                    }

                    if (fullName.StartsWith("PaintDotNet", StringComparison.Ordinal))
                    {
                        return false;
                    }

                    OpenMsDocs(fullName);
                    return true;
                }
                else
                {
                    int memberIndex = 0;
                    if (mi[0].MemberType == MemberTypes.Method)
                    {
                        memberIndex = GetOverload(mi, tokenPos[i]).Item1;
                    }

                    type = mi[memberIndex].GetReturnType();
                    if (type == null)
                    {
                        return false;
                    }
                }
            }

            return false;

            void OpenMsDocs(string fullName)
            {
                System.Diagnostics.Process.Start($"https://docs.microsoft.com/dotnet/api/{fullName}");
            }
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            if (iBox.Visible && iBox.MouseOver)
            {
                ((HandledMouseEventArgs)e).Handled = true;
                int scrollTo = iBox.SelectedIndex - Math.Sign(e.Delta) * SystemInformation.MouseWheelScrollLines;
                iBox.SelectedIndex = scrollTo.Clamp(0, iBox.Items.Count - 1);
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
                    ClearVarToRename();
                }
                else if (e.KeyCode == Keys.Tab)
                {
                    string prevWord = this.GetWordFromPosition(this.CurrentPosition);
                    int prevWordStartPos = this.WordStartPosition(this.CurrentPosition, true);
                    int prevWordEndPos = this.WordEndPosition(this.CurrentPosition, true);

                    if (this.GetCharAt(prevWordStartPos - 1).Equals('#'))
                    {
                        prevWord = "#" + prevWord;
                    }

                    if (Intelli.Snippets.ContainsKey(prevWord) && prevWordEndPos == this.CurrentPosition)
                    {
                        e.Handled = true;
                        e.SuppressKeyPress = true;

                        this.BeginUndoAction();

                        // Insert the snippet
                        int startPos = this.CurrentPosition;
                        string[] lines = Intelli.Snippets[prevWord].Split('\n');
                        for (int i = 0; i < lines.Length; i++)
                        {
                            string line = (i == lines.Length - 1) ? lines[i] : lines[i] + "\r\n";
                            this.AddText(line);
                        }
                        int endPos = this.CurrentPosition;

                        // move caret/selection to the correct location
                        this.SearchFlags = SearchFlags.None;
                        this.SetTargetRange(startPos, endPos);
                        if (this.SearchInTarget("&") != InvalidPosition)
                        {
                            this.DeleteRange(this.TargetStart, 1);
                            this.SelectionStart = this.WordStartPosition(this.TargetStart, true);
                            this.SelectionEnd = this.WordEndPosition(this.TargetStart, true);
                        }

                        this.EndUndoAction();
                        OnBuildNeeded();
                    }
                }
                else if (e.Control && e.KeyCode == Keys.J)
                {
                    int startPos = this.WordStartPosition(this.CurrentPosition, true);
                    if (this.GetTextRange(startPos - 1, 1).Equals("."))
                    {
                        MemberIntelliBox(startPos - 1);
                        if (iBox.Visible)
                        {
                            this.SetEmptySelection(startPos);
                        }
                    }
                    else
                    {
                        NonMemberIntelliBox(this.CurrentPosition - 1);
                    }
                }
                else if (e.KeyCode == Keys.F12)
                {
                    if (!GoToDefinition())
                    {
                        MessageBox.Show("Cannot navigate to the symbol under the caret.", "CodeLab", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
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
                int wordStartPos = this.WordStartPosition(this.CurrentPosition - 1, true);
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
                int wordStartPos = this.WordStartPosition(this.CurrentPosition - 1, true);
                if (wordStartPos != posAtIBox)
                {
                    iBox.Visible = false;
                }
            }
            else if (e.KeyCode == Keys.Right)
            {
                int wordStartPos = this.WordStartPosition(this.CurrentPosition + 1, true);
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
            else if ((e.KeyValue < 48) || (e.KeyValue >= 58 && e.KeyValue <= 64) || (e.KeyValue >= 91 && e.KeyValue <= 96) || e.KeyValue > 122)
            {
                // Use currently selected item
                if (e.KeyCode == Keys.Return)
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
                // Just close the box and don't use anything
                else if (e.KeyCode == Keys.Escape)
                {
                    iBox.Visible = false;
                }
            }
            else
            {
                // Letter or number typed, search for it in the listview
                int wordStartPos = this.WordStartPosition(this.CurrentPosition, true);

                if (this.GetCharAt(wordStartPos - 1).Equals('#'))
                {
                    wordStartPos--;
                }

                string charsEntered = this.GetTextRange(wordStartPos, this.CurrentPosition - wordStartPos) + e.KeyValue.ToChar().ToLowerInvariant();
                iBox.Filter(charsEntered);
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

            base.OnMouseDown(e);
        }

        private void MemberIntelliBox(int position)
        {
            Type type = GetReturnType(position);

            if (type == null)
            {
                return;
            }

            string[] lastWords = GetLastWords(position).Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            if (lastWords.Length == 0)
            {
                return;
            }

            string lastWord = lastWords.Last();
            bool isStatic = (Intelli.AllTypes.ContainsKey(lastWord) || Intelli.UserDefinedTypes.ContainsKey(lastWord));

            iBox.Populate(type, isStatic);

            ShowIntelliBox(position, true);
        }

        private void NonMemberIntelliBox(int position)
        {
            // Ensure there's no words immediately to the left and right
            if (char.IsLetterOrDigit(this.GetCharAt(position - 1).ToChar()) || char.IsLetterOrDigit(this.GetCharAt(position + 1).ToChar()))
            {
                return;
            }

            int prevCharPos = position - 1;
            while (char.IsWhiteSpace(this.GetCharAt(prevCharPos).ToChar()) && prevCharPos > 0)
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

            // Ensure previous word is not Type or Variable
            if (char.IsLetterOrDigit(this.GetCharAt(prevCharPos).ToChar()))
            {
                string preWord = this.GetWordFromPosition(prevCharPos);
                if (Intelli.AllTypes.ContainsKey(preWord) || Intelli.UserDefinedTypes.ContainsKey(preWord) || Intelli.Variables.ContainsKey(preWord) || preWord == "void" || preWord == "var")
                {
                    return;
                }
            }
            else if (this.GetCharAt(prevCharPos) == '>' || this.GetCharAt(prevCharPos) == ']')
            {
                if (this.GetCharAt(prevCharPos - 1) == '<' || this.GetCharAt(prevCharPos - 1) == '[')
                {
                    prevCharPos--;
                }

                string prevWord = GetLastWords(prevCharPos);

                if (Intelli.AllTypes.ContainsKey(prevWord) || Intelli.UserDefinedTypes.ContainsKey(prevWord) || Intelli.Variables.ContainsKey(prevWord))
                {
                    return;
                }
            }

            iBox.Populate(this.GetCharAt(position).ToChar());
            ShowIntelliBox(position, false);
        }

        private void ConstructorIntelliBox(int position)
        {
            string word = this.GetWordFromPosition(position);
            if (!Intelli.AllTypes.ContainsKey(word) && !Intelli.UserDefinedTypes.ContainsKey(word))
            {
                return;
            }

            int style = this.GetStyleAt(this.WordStartPosition(position, true));
            if (style != Style.Cpp.Word && style != Style.Cpp.Word + Preprocessor &&
                style != Style.Cpp.Word2 && style != Style.Cpp.Word2 + Preprocessor)
            {
                return;
            }

            Type type = Intelli.AllTypes.ContainsKey(word) ? Intelli.AllTypes[word] : Intelli.UserDefinedTypes[word];
            iBox.Populate(type);
            ShowIntelliBox(position, false);
        }

        private void ShowIntelliBox(int position, bool members)
        {
            if (iBox.Items.Count <= 0)
            {
                return; // For some reason, the box is empty... don't bother showing it.
            }

            posAtIBox = members ? position + 1 : position;

            int lineHeight = this.Lines[this.CurrentLine].Height;
            int periodWidth = members ? this.TextWidth(Style.Default, ".") : 0;
            Point topLeft = new Point
            {
                X = PointXFromPosition(position) + periodWidth - iBox.IconWidth,
                Y = PointYFromPosition(position) + lineHeight
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
            int startPos = this.WordStartPosition(this.CurrentPosition, true);
            int endPos = this.WordEndPosition(this.CurrentPosition, true);
            string fill = iBox.SelectedItem.ToString();

            if (this.GetCharAt(startPos - 1).Equals('#'))
            {
                startPos--;
            }

            this.SetTargetRange(startPos, endPos);
            this.ReplaceTarget(fill);
            this.SetEmptySelection(startPos + fill.Length);

            iBox.SaveUsedItem();

            iBox.Visible = false;
            posAtIBox = InvalidPosition;

            this.Focus();
        }

        private void ClearVarToRename()
        {
            varToRename = string.Empty;
            varToRenamePos = InvalidPosition;

            this.IndicatorCurrent = Indicator.VariableRename;
            this.IndicatorClearRange(0, this.TextLength);
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
            if (oldTerm.Length == 0)
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

        private int lastCaretPos = 0;
        private bool mapScroll = false;
        private static bool IsBrace(int c, bool openBrace)
        {
            if (openBrace)
            {
                switch (c)
                {
                    case '(':
                    case '[':
                    case '{':
                    case '<':
                        return true;
                }
            }
            else
            {
                switch (c)
                {
                    case ')':
                    case ']':
                    case '}':
                    case '>':
                        return true;
                }
            }
            return false;
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
                    iBox.HideToolTip();
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

                    iBox.HideToolTip();
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

            if (e.Change.HasFlag(UpdateChange.Selection) || e.Change.HasFlag(UpdateChange.Content))
            {
                if (MapEnabled)
                {
                    int curLine = GetVisibleLine(this.CurrentLine);
                    indicatorBar.Caret = CountVisibleLines(curLine);
                }

                // Has the caret changed position?
                int caretPos = this.CurrentPosition;
                if (lastCaretPos != caretPos)
                {
                    lastCaretPos = caretPos;

                    HighlightWordUsage();

                    int bracePos1 = InvalidPosition;
                    int bracePos2 = InvalidPosition;

                    // Is there a closed brace to the left or an open brace to the right?
                    if (caretPos > 0 && IsBrace(this.GetCharAt(caretPos - 1), false))
                    {
                        bracePos1 = (caretPos - 1);
                    }
                    else if (IsBrace(this.GetCharAt(caretPos), true))
                    {
                        bracePos1 = caretPos;
                    }

                    int style = this.GetStyleAt(bracePos1);

                    if (bracePos1 > InvalidPosition && (style == Style.Cpp.Operator || style == Style.Cpp.Operator + Preprocessor))
                    {
                        // Find the matching brace
                        bracePos2 = this.BraceMatch(bracePos1);
                        if (bracePos2 == InvalidPosition)
                        {
                            this.BraceBadLight(bracePos1);
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

        protected override void OnInsertCheck(InsertCheckEventArgs e)
        {
            if (e.Text.Equals("\r\n", StringComparison.Ordinal) ||
                e.Text.Equals("\r", StringComparison.Ordinal) ||
                e.Text.Equals("\n", StringComparison.Ordinal))
            {
                int line = this.LineFromPosition(e.Position);
                e.Text += new string(' ', this.Lines[line].Indentation);
                if (this.Lines[line].Text.Trim().EndsWith("{", StringComparison.Ordinal))
                {
                    e.Text += new string(' ', this.TabWidth);
                }
            }

            base.OnInsertCheck(e);
        }

        protected override void OnCharAdded(CharAddedEventArgs e)
        {
            if (e.Char == '}')
            {
                if (this.Lines[this.CurrentLine].Text.Trim() == "}") //Check whether the bracket is the only thing on the line.. For cases like "if() { }".
                {
                    this.Lines[this.CurrentLine].Indentation -= this.TabWidth;
                }
            }
            else if (!iBox.Visible)
            {
                if (e.Char == '.')
                {
                    MemberIntelliBox(this.CurrentPosition - 1);
                }
                else if (e.Char == '(')
                {
                    ConstructorIntelliBox(this.CurrentPosition - 1);
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
            }

            base.OnCharAdded(e);
        }

        protected override void OnZoomChanged(EventArgs e)
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

            this.WhitespaceSize = getDpiX((this.Zoom >= 2) ? 2 : 1);

            UpdateIndicatorBar();

            base.OnZoomChanged(e);
        }

        private bool Replacing;
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

            // Adjust or disable indicator for Variable Renaming
            int wordStartPos = this.WordStartPosition(this.CurrentPosition, true);
            int wordEndPos = this.WordEndPosition(this.CurrentPosition, true) - 1;
            if (IsIndicatorOn(Indicator.VariableRename, wordStartPos) || IsIndicatorOn(Indicator.VariableRename, wordEndPos) || this.CurrentPosition == varToRenamePos)
            {
                int endPos = wordStartPos;
                while (IsIndicatorOn(Indicator.VariableRename, endPos) && endPos <= this.TextLength)
                {
                    endPos++;
                }

                int nonSpacePos = wordStartPos;
                while (!char.IsWhiteSpace(this.GetCharAt(nonSpacePos).ToChar()) && nonSpacePos <= endPos)
                {
                    nonSpacePos++;
                }

                if ((endPos == nonSpacePos - 1 || endPos == nonSpacePos) && this.GetWordFromPosition(this.CurrentPosition) != varToRename)
                {
                    int length = this.WordEndPosition(this.CurrentPosition, true) - wordStartPos;

                    this.IndicatorCurrent = Indicator.VariableRename;
                    this.IndicatorFillRange(wordStartPos, length);
                }
                else
                {
                    ClearVarToRename();
                }
            }
            else
            {
                ClearVarToRename();
            }

            base.OnTextChanged(e);
        }

        protected override void OnBeforeDelete(BeforeModificationEventArgs e)
        {
            // Set indicator for Variable Renaming?
            int wordStartPos = this.WordStartPosition(e.Position, true);
            if (e.Source == ModificationSource.User && GetIntelliType(wordStartPos) == IntelliType.Variable &&
                !IsIndicatorOn(Indicator.VariableRename, wordStartPos) && e.Text.Trim().Length > 0 && !Replacing)
            {
                varToRenamePos = wordStartPos;
                varToRename = this.GetWordFromPosition(e.Position);
                int length = this.WordEndPosition(e.Position, true) - wordStartPos;

                this.IndicatorCurrent = Indicator.VariableRename;
                this.IndicatorFillRange(wordStartPos, length);
            }

            base.OnBeforeDelete(e);
        }

        protected override void OnBeforeInsert(BeforeModificationEventArgs e)
        {
            // Set indicator for Variable Renaming?
            int wordStartPos = this.WordStartPosition(e.Position, true);
            if (e.Source == ModificationSource.User && GetIntelliType(wordStartPos) == IntelliType.Variable &&
                !IsIndicatorOn(Indicator.VariableRename, wordStartPos) && e.Text.Trim().Length > 0 && !Replacing)
            {
                varToRenamePos = wordStartPos;
                varToRename = this.GetWordFromPosition(e.Position);
                int length = this.WordEndPosition(e.Position, true) - wordStartPos;

                this.IndicatorCurrent = Indicator.VariableRename;
                this.IndicatorFillRange(wordStartPos, length);
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

            UpdateIndicatorBar();
        }

        protected override void OnLostFocus(EventArgs e)
        {
            // Scintilla doesn't hide its caret, if it loses focus to a child control
            if (this.ContainsFocus)
            {
                Control focusedControl = this.FindFocus();
                this.Focus();
                this.Enabled = false; // Force it to lose focus to a non-child control
                this.Enabled = true;
                focusedControl.Focus();
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
            this.BeginUndoAction();
            for (int line = this.LineFromPosition(this.SelectionStart); line < this.LineFromPosition(this.SelectionEnd) + 1; line++)
            {
                this.Lines[line].Indentation += this.TabWidth;
            }
            this.EndUndoAction();
        }

        internal void UnIndent()
        {
            this.BeginUndoAction();
            for (int line = this.LineFromPosition(this.SelectionStart); line < this.LineFromPosition(this.SelectionEnd) + 1; line++)
            {
                this.Lines[line].Indentation -= this.TabWidth;
            }
            this.EndUndoAction();
        }

        internal void UpdateSyntaxHighlighting()
        {
            this.SetKeywords(1, string.Join(" ", Intelli.AllTypes.Keys) + " " + string.Join(" ", Intelli.UserDefinedTypes.Keys));
            this.SetKeywords(0, "abstract as base bool byte char checked class const decimal delegate double enum event explicit extern "
                + "false fixed float implicit in int interface internal is lock long namespace new null object operator out override "
                + "params partial private protected public readonly ref sbyte sealed short sizeof stackalloc static string struct "
                + "this true typeof uint unchecked unsafe ulong ushort using var virtual void volatile");
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
                else if (trimmedLine.StartsWith("using", StringComparison.Ordinal) || trimmedLine.StartsWith("if", StringComparison.Ordinal) || trimmedLine.StartsWith("else", StringComparison.Ordinal))
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

        internal void ReNumberUIVariables()
        {
            if (!this.Text.Contains("#region UICode") || !this.Text.Contains("#endregion"))
            {
                return;
            }

            int start = this.Text.IndexOf("#region UICode", StringComparison.Ordinal) + 16;
            int length = this.Text.IndexOf("#endregion", StringComparison.Ordinal) - start;
            string[] UiCodeText = this.GetTextRange(start, length).Split('\n');
            List<string> UIlist = new List<string>();
            foreach (string item in UiCodeText)
            {
                string trimmed = item.Trim();
                if (trimmed.Length > 0)
                {
                    UIlist.Add(trimmed);
                }
            }

            bool needReNumbering = false;
            for (int i = 0; i < UIlist.Count; i++)
            {
                if (!UIlist[i].Contains("Amount" + (i + 1).ToString()))
                {
                    needReNumbering = true;
                    break;
                }
            }

            if (!needReNumbering)
            {
                return;
            }

            Regex REAmt = new Regex(@"\s*(?<type>.*)\s+Amount(?<amt>\d+)\s*=\s*(?<default>.*);\s*\/{2}(?<rawcomment>.*)");
            List<KeyValuePair<int, int>> data = new List<KeyValuePair<int, int>>();
            for (int i = 0; i < UIlist.Count; i++)
            {
                Match m = REAmt.Match(UIlist[i]);
                if (!m.Success)
                {
                    continue;
                }

                if (int.TryParse(m.Groups["amt"].Value, out int fm))
                {
                    data.Add(new KeyValuePair<int, int>(fm, i + 1));
                }
            }

            const string modifier = "_R!e!N!u!M!b!E!r_";
            this.BeginUndoAction();
            for (int i = 0; i < data.Count; i++)
            {
                Replace("Amount" + data[i].Key.ToString(), "Amount" + modifier + data[i].Value.ToString(), SearchFlags.MatchCase | SearchFlags.WholeWord);
            }

            Replace(modifier, string.Empty, SearchFlags.None);
            this.EndUndoAction();

            OnBuildNeeded();


            void Replace(string oldValue, string newValue, SearchFlags searchFlags)
            {
                if (oldValue.Length == 0)
                {
                    return;
                }

                // Search the document
                this.TargetWholeDocument();
                this.SearchFlags = searchFlags;

                while (this.SearchInTarget(oldValue) != InvalidPosition)
                {
                    // Replace the instance with new string
                    this.ReplaceTarget(newValue);

                    // Search the remainder of the document
                    this.SetTargetRange(this.TargetEnd, this.TextLength);
                }
            }
        }

        internal void RenameVariable()
        {
            this.SetEmptySelection(varToRenamePos);
            string newVar = this.GetWordFromPosition(varToRenamePos);

            ParseLocalVariables(this.CurrentPosition);

            // re-add the old variable to dictionary.
            // it will be automatically removed during the next ParseVariables();
            if (!Intelli.Variables.ContainsKey(varToRename))
            {
                Intelli.Variables.Add(varToRename, Intelli.Variables[newVar]);
            }

            // Search the document
            this.TargetWholeDocument();
            this.SearchFlags = SearchFlags.MatchCase | SearchFlags.WholeWord;

            this.BeginUndoAction();
            while (this.SearchInTarget(varToRename) != InvalidPosition)
            {
                if (GetIntelliType(this.TargetStart) == IntelliType.Variable)
                {
                    // Replace the instance with new string
                    this.ReplaceTarget(newVar);
                }

                // Search the remainder of the document
                this.SetTargetRange(this.TargetEnd, this.TextLength);
            }
            this.EndUndoAction();

            ClearVarToRename();

            ParseLocalVariables(this.CurrentPosition);
            HighlightWordUsage();
        }

        private void renameVar_Click(object sender, EventArgs e)
        {
            RenameVariable();
            OnBuildNeeded();
        }
        #endregion

        #region Helper functions
        private int GetVisibleLine(int line)
        {
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
            int count = 0;
            for (int i = 0; i < untilLine; i++)
            {
                if (this.Lines[i].Visible)
                {
                    count += this.Lines[i].WrapCount;
                }
            }

            return count;
        }

        private int getDpiX(int value) => (int)Math.Round(value * dpi.Width);

        internal bool IsIndicatorOn(int indicator, int position)
        {
            uint bitmask = this.IndicatorAllOnFor(position);
            int flag = (1 << indicator);
            return ((bitmask & flag) == flag);
        }

        private int maxLineNumberCharLength;
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
        #endregion

        #region Editor ToolTip Functions
        protected override void OnDwellStart(DwellEventArgs e)
        {
            if (lightBulbMenu.Visible)
            {
                lightBulbMenu.Hide();
            }

            string tooltipText = this.GetIntelliTip(e.Position);

            // If there's an error here, we'll show that instead
            if (ScriptBuilder.Errors.Count > 0)
            {
                int wordStartPos = this.WordStartPosition(e.Position, false);
                int wordEndPos = this.WordEndPosition(e.Position, false);
                foreach (ScriptError error in ScriptBuilder.Errors)
                {
                    int errorPos = this.Lines[error.Line - 1].Position + error.Column;
                    if (errorPos == wordStartPos || errorPos == wordEndPos)
                    {
                        tooltipText = error.ErrorText.InsertLineBreaks(100);
                        break;
                    }
                }
            }

            if (tooltipText.Length > 0)
            {
                int y = this.PointYFromPosition(e.Position) + this.Lines[this.CurrentLine].Height;
                intelliTip.Show(tooltipText, this, e.X, y);
            }

            if (this.IsIndicatorOn(Indicator.VariableRename, e.Position))
            {
                renameVarMenuItem.Text = $"Rename '{varToRename}' to '{this.GetWordFromPosition(e.Position)}'";
                lightBulbMenu.Location = new Point(this.PointXFromPosition(e.Position) - lightBulbMenu.Width - 10,
                                                   this.PointYFromPosition(e.Position) + this.Lines[this.CurrentLine].Height);
                lightBulbMenu.Show();
            }

            base.OnDwellStart(e);
        }

        protected override void OnDwellEnd(DwellEventArgs e)
        {
            if (intelliTip.Active)
            {
                intelliTip.Hide(this);
            }

            base.OnDwellEnd(e);
        }
        #endregion

        #region Document Tabs functions
        internal void CreateNewDocument(Guid guid)
        {
            this.findPanel.Hide();

            var document = this.Document ;
            this.AddRefDocument(document);

            // Replace the current document with a new one
            ScintillaNET.Document newDocument = this.CreateDocument();
            this.docCollection.Add(guid, newDocument);

            this.Document = newDocument;

            this.Lexer = Lexer.Cpp;
            indexForPurpleWords = this.AllocateSubstyles(Style.Cpp.Identifier, 1);
            this.UpdateSyntaxHighlighting();

            this.SetProperty("fold", "1");
            this.SetProperty("fold.compact", "0");
            this.AutomaticFold = (AutomaticFold.Show | AutomaticFold.Click | AutomaticFold.Change);
        }

        internal void SwitchToDocument(Guid guid)
        {
            if (!docCollection.ContainsKey(guid))
            {
                return;
            }

            this.findPanel.Hide();

            var prevDocument = this.Document;
            this.AddRefDocument(prevDocument);

            this.Document = this.docCollection[guid];
            this.ReleaseDocument(this.docCollection[guid]);

            AdjustLineNumbersWidth();
        }

        internal void CloseDocument(Guid guid)
        {
            if (!docCollection.ContainsKey(guid))
            {
                return;
            }

            this.ReleaseDocument(this.docCollection[guid]);
            this.docCollection.Remove(guid);
        }
        #endregion

        #region Indicator Map functions
        internal void UpdateIndicatorBar()
        {
            if (!MapEnabled)
            {
                return;
            }

            indicatorBar.Maximum = CountUILines();
            indicatorBar.LargeChange = this.LinesOnScreen;
            indicatorBar.Value = this.FirstVisibleLine;

            int curLine = GetVisibleLine(this.CurrentLine);
            indicatorBar.Caret = CountVisibleLines(curLine);

            if (this.Bookmarks.Length == 0)
            {
                indicatorBar.Bookmarks = Array.Empty<int>();
            }
            else
            {
                List<int> bkmks = new List<int>();
                for (int i = 0; i < this.Bookmarks.Length; i++)
                {
                    int bkmkLine = GetVisibleLine(Bookmarks[i]);
                    bkmks.Add(CountVisibleLines(bkmkLine));
                }
                indicatorBar.Bookmarks = bkmks.ToArray();
            }

            if (matchLines.Count == 0)
            {
                indicatorBar.Matches = Array.Empty<int>();
            }
            else
            {
                List<int> matches = new List<int>();
                for (int i = 0; i < matchLines.Count; i++)
                {
                    int matchLine = GetVisibleLine(matchLines[i]);
                    matches.Add(CountVisibleLines(matchLine));
                }
                indicatorBar.Matches = matches.ToArray();
            }

            if (errorLines.Count == 0)
            {
                indicatorBar.Errors = Array.Empty<int>();
            }
            else
            {
                List<int> errors = new List<int>();
                for (int i = 0; i < errorLines.Count; i++)
                {
                    int errorLine = GetVisibleLine(errorLines[i]);
                    errors.Add(CountVisibleLines(errorLine));
                }
                indicatorBar.Errors = errors.ToArray();
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
            this.IndicatorCurrent = Indicator.Error;
            this.IndicatorClearRange(0, this.TextLength); // Clear underlines from the previous time
        }

        internal void AddError(int line, int column)
        {
            errorLines.Add(line);

            int errPosition = this.Lines[line].Position + column;
            int errorLength = this.GetWordFromPosition(errPosition).Length;

            // if error is at the end of the line (missing semi-colon), or is a stray '.'
            if (errorLength == 0 || errPosition == this.Lines[line].EndPosition - 2)
            {
                errPosition--;
                errorLength = 1;
            }

            // Underline the error
            this.IndicatorCurrent = Indicator.Error;
            this.IndicatorFillRange(errPosition, errorLength);
        }
        #endregion

        #region Constants
        private static class Indicator
        {
            // 0 - 7 are reserved by Scintilla internally... used by the lexers
            internal const int Error = 8;
            internal const int ObjectHighlight = 9;
            internal const int ObjectHighlightDef = 10;
            internal const int VariableRename = 11;
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
    }

    public enum Theme
    {
        Auto,
        Light,
        Dark
    }
}
