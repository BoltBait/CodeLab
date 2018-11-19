/////////////////////////////////////////////////////////////////////////////////
// CodeLab for Paint.NET
// Portions Copyright ©2007-2017 BoltBait. All Rights Reserved.
// Portions Copyright ©Microsoft Corporation. All Rights Reserved.
//
// THE CODELAB DEVELOPERS MAKE NO WARRANTY OF ANY KIND REGARDING THE CODE. THEY
// SPECIFICALLY DISCLAIM ANY WARRANTY OF FITNESS FOR ANY PARTICULAR PURPOSE OR
// ANY OTHER WARRANTY.  THE CODELAB DEVELOPERS DISCLAIM ALL LIABILITY RELATING
// TO THE USE OF THIS CODE.  NO LICENSE, EXPRESS OR IMPLIED, BY ESTOPPEL OR
// OTHERWISE, TO ANY INTELLECTUAL PROPERTY RIGHTS IS GRANTED HEREIN.
//
// Latest distribution: http://www.BoltBait.com/pdn/codelab
/////////////////////////////////////////////////////////////////////////////////

using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
using System.IO.Compression;
using System.Text;
using System.Diagnostics;

namespace PaintDotNet.Effects
{
    internal partial class BuildForm : Form
    {
        #region Constructor
        internal string IconPathStr = "";
        internal string SubMenuStr = "";
        internal string MenuStr = "";
        internal string CreateConfigOverride = "";
        internal string WindowTitleTextStr = "";
        internal int ConfigType = 0;
        internal string Author = "";
        internal string Support = "";
        internal int MajorVer = 0;
        internal int MinorVer = 0;
        internal bool isAdjustment = false;
        internal string Description = "";
        internal string KeyWords = "";
        internal bool ForceAliasSelection = false;
        internal bool ForceSingleThreaded = false;
        internal HelpType HelpType = 0;
        internal string HelpStr = "";
        private string HelpFileName = "";
        internal string RTZPath = "";
        string FullScriptText = "";
        string FileName = "";

        internal BuildForm(string ScriptName, string ScriptText, string ScriptPath)
        {
            InitializeComponent();

            // PDN Theme
            this.ForeColor = PdnTheme.ForeColor;
            this.BackColor = PdnTheme.BackColor;
            toolStrip1.Renderer = PdnTheme.Renderer;
            SubMenuName.ForeColor = PdnTheme.ForeColor;
            SubMenuName.BackColor = PdnTheme.BackColor;
            ButtonIcon.LinkColor = PdnTheme.ForeColor;
            ButtonIcon.ActiveLinkColor = PdnTheme.ForeColor;
            foreach (Control control in this.Controls)
            {
                if (control is TextBox || control is NumericUpDown)
                {
                    control.ForeColor = PdnTheme.ForeColor;
                    control.BackColor = PdnTheme.BackColor;
                }
            }

            WarningLabel.Visible = false;

            // Set dialog box title
            this.Text = "Building " + ScriptName + ".dll";
            this.HelpFileName = ScriptName + ".txt";
            this.RTZPath = Path.ChangeExtension(ScriptPath, ".rtz");
            DecimalSymbol.Text = System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator;

            #region Populate fields from script comments
            FullScriptText = ScriptText;
            FileName = ScriptName;
            // Preload submenu name
            Regex RESubMenu = new Regex(@"//[\s-[\r\n]]*SubMenu[\s-[\r\n]]*:[\s-[\r\n]]*(?<sublabel>.*)(?=\r?\n|$)", RegexOptions.IgnoreCase);
            Match msm = RESubMenu.Match(ScriptText);
            if (msm.Success)
            {
                SubMenuName.Text = msm.Groups["sublabel"].Value.Trim();
                if ((SubMenuName.Text.ToLower() == "adjustments") || (SubMenuName.Text.ToLower() == "adj"))
                {
                    AdjustmentRadio.Checked = true;
                    SubMenuName.Text = "";
                }
            }
            // Preload menu name
            Regex REName = new Regex(@"//[\s-[\r\n]]*Name[\s-[\r\n]]*:[\s-[\r\n]]*(?<menulabel>.*)(?=\r?\n|$)", RegexOptions.IgnoreCase);
            Match mmn = REName.Match(ScriptText);
            if (mmn.Success)
            {
                string menuName = mmn.Groups["menulabel"].Value.Trim();
                MenuName.Text = (menuName.Length > 0) ? menuName : ScriptName;
            }
            else
            {
                MenuName.Text = ScriptName;
            }
            // Preload window title
            Regex RETitle = new Regex(@"//[\s-[\r\n]]*Title[\s-[\r\n]]*:[\s-[\r\n]]*(?<titlelabel>.*)(?=\r?\n|$)", RegexOptions.IgnoreCase);
            Match wtn = RETitle.Match(ScriptText);
            if (wtn.Success)
            {
                WindowTitleText.Text = wtn.Groups["titlelabel"].Value.Trim();
            }
            // Preload version checking for period
            Regex REVersion = new Regex(@"//[\s-[\r\n]]*Version[\s-[\r\n]]*:[\s-[\r\n]]*(?<majorversionlabel>\d+)\.(?<minorversionlabel>\d+)(?=\r?\n|$)", RegexOptions.IgnoreCase);
            Match vsn = REVersion.Match(ScriptText);
            if (!vsn.Success)
            {
                // Preload version checking for comma
                REVersion = new Regex(@"//[\s-[\r\n]]*Version[\s-[\r\n]]*:[\s-[\r\n]]*(?<majorversionlabel>\d+)\,(?<minorversionlabel>\d+)(?=\r?\n|$)", RegexOptions.IgnoreCase);
                vsn = REVersion.Match(ScriptText);
            }
            if (vsn.Success)
            {
                decimal majorv = 0;
                decimal minorv = 0;
                if (decimal.TryParse(vsn.Groups["majorversionlabel"].Value.Trim(), out majorv))
                {
                    MajorVersion.Value = majorv.Clamp(MajorVersion.Minimum, MajorVersion.Maximum);
                }
                if (decimal.TryParse(vsn.Groups["minorversionlabel"].Value.Trim(), out minorv))
                {
                    MinorVersion.Value = minorv.Clamp(MinorVersion.Minimum, MinorVersion.Maximum);
                }
            }
            // Preload author's name
            Regex REAuthor = new Regex(@"//[\s-[\r\n]]*Author[\s-[\r\n]]*:[\s-[\r\n]]*(?<authorlabel>.*)(?=\r?\n|$)", RegexOptions.IgnoreCase);
            Match mau = REAuthor.Match(ScriptText);
            if (mau.Success)
            {
                AuthorName.Text = mau.Groups["authorlabel"].Value.Trim();
            }
            // Preload Description
            Regex REDesc = new Regex(@"//[\s-[\r\n]]*Desc[\s-[\r\n]]*:[\s-[\r\n]]*(?<desclabel>.*)(?=\r?\n|$)", RegexOptions.IgnoreCase);
            Match mds = REDesc.Match(ScriptText);
            if (mds.Success)
            {
                DescriptionBox.Text = mds.Groups["desclabel"].Value.Trim();
            }
            else
            {
                DescriptionBox.Text = ScriptName + " selected pixels";
            }
            // Preload Keywords
            Regex REWords = new Regex(@"//[\s-[\r\n]]*KeyWords[\s-[\r\n]]*:[\s-[\r\n]]*(?<wordslabel>.*)(?=\r?\n|$)", RegexOptions.IgnoreCase);
            Match mkw = REWords.Match(ScriptText);
            if (mkw.Success)
            {
                KeyWordsBox.Text = mkw.Groups["wordslabel"].Value.Trim();
            }
            else
            {
                KeyWordsBox.Text = ScriptName;
            }
            // Preload Support URL
            Regex RESupport = new Regex(@"//[\s-[\r\n]]*URL[\s-[\r\n]]*:[\s-[\r\n]]*(?<urllabel>.*)(?=\r?\n|$)", RegexOptions.IgnoreCase);
            Match msu = RESupport.Match(ScriptText);
            if (msu.Success)
            {
                SupportURL.Text = msu.Groups["urllabel"].Value.Trim();
            }
            // Preload Force Aliased Selection
            ForceAliasSelectionBox.Checked = false;
            Regex REAlias = new Regex(@"//[\s-[\r\n]]*(Force\s*Aliased\s*Selection|FAS)[\s-[\r\n]]*(?=\r?\n|$)", RegexOptions.IgnoreCase);
            Match mas = REAlias.Match(ScriptText);
            if (mas.Success)
            {
                ForceAliasSelectionBox.Checked = true;
            }
            // Preload Force Single Threaded
            ForceSingleThreadedBox.Checked = false;
            Regex RESingle = new Regex(@"//[\s-[\r\n]]*(Force\s*Single\s*Threaded|FST)[\s-[\r\n]]*(?=\r?\n|$)", RegexOptions.IgnoreCase);
            Match mst = RESingle.Match(ScriptText);
            if (mst.Success)
            {
                ForceSingleThreadedBox.Checked = true;
            }
            #endregion

            #region Load Help Text
            // Preload help text
            Regex REHelp = new Regex(@"//[\s-[\r\n]]*Help[\s-[\r\n]]*:[\s-[\r\n]]*(?<helptext>.*)(?=\r?\n|$)", RegexOptions.IgnoreCase);
            Match hlp = REHelp.Match(ScriptText);
            if (hlp.Success)
            {
                HelpStr = hlp.Groups["helptext"].Value.Trim();
                if (HelpStr.StartsWith("http://", StringComparison.OrdinalIgnoreCase) || HelpStr.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                {
                    HelpURL.Text = HelpStr;
                    radioButtonURL.Checked = true;
                }
                else
                {
                    HelpPlainText.Text = HelpStr.Replace("\\\\t", "[t]").Replace("\\\\n", "[n]").Replace("\\n", "\r\n").Replace("\\t", "\t").Replace("[t]", "\\\\t").Replace("[n]", "\\\\n");
                    radioButtonPlain.Checked = true;
                }
            }

            if (HelpPlainText.Text == "")
            {
                HelpPlainText.Text = $"{MenuName.Text} v{MajorVersion.Value}{DecimalSymbol.Text}{MinorVersion.Value}\r\nCopyright ©{DateTime.Now.Year} by {AuthorName.Text}\r\nAll rights reserved.";
                if (radioButtonNone.Checked)
                {
                    radioButtonPlain.Checked = true;
                }
            }

            string resourcePath = Path.Combine(Path.GetDirectoryName(ScriptPath), ScriptName);

            // See if a help file exists
            try
            {
                string rtfPath = Path.ChangeExtension(resourcePath, ".rtf");
                if (File.Exists(rtfPath))
                {
                    RichHelpContent.Rtf = File.ReadAllText(rtfPath);
                    radioButtonRich.Checked = true;
                }
            }
            catch
            {
                // If something went wrong, don't crash, just assume the file is invalid
            }

            if (RichHelpContent.Text.Length == 0)
            {
                try
                {
                    string rtzPath = Path.ChangeExtension(resourcePath, ".rtz");
                    if (File.Exists(rtzPath))
                    {
                        string compressedContents = File.ReadAllText(rtzPath);
                        RichHelpContent.Rtf = DecompressString(compressedContents);
                        radioButtonRich.Checked = true;
                    }
                }
                catch
                {
                    // If something went wrong, don't crash, just assume the file is invalid
                }
            }

            if (RichHelpContent.Text.Length == 0)
            {
                try
                {
                    string txtPath = Path.ChangeExtension(resourcePath, ".txt");
                    if (File.Exists(txtPath))
                    {
                        RichHelpContent.Text = File.ReadAllText(txtPath);
                        ChangeUBBtoRTF();
                        radioButtonRich.Checked = true;
                    }
                }
                catch
                {
                    // If something went wrong, don't crash, just assume the file is invalid
                }
            }
            #endregion

            #region Load default icon
            // See if a default icon exists
            string iconPath = Path.ChangeExtension(resourcePath, ".png");
            SetIcon(iconPath);
            #endregion

            UpdateReadOnlyFields();
        }
        #endregion

        #region Build / Cancel buttons
        private void ButtonCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private bool UpdateAllValues()
        {
            MajorVer = (int)MajorVersion.Value;
            MinorVer = (int)MinorVersion.Value;
            Author = AuthorName.Text.Replace('\\', '/');
            Support = SupportURL.Text.Replace('\\', '/');
            WindowTitleTextStr = WindowTitleText.Text.Trim().Replace('\\', '/');
            isAdjustment = AdjustmentRadio.Checked;
            Description = DescriptionBox.Text.Trim().Replace('\\', '/');
            KeyWords = KeyWordsBox.Text.Trim().Replace('\\', '/');
            ForceAliasSelection = ForceAliasSelectionBox.Checked;
            ForceSingleThreaded = ForceSingleThreadedBox.Checked;
            if (radioButtonNone.Checked)
            {
                HelpStr = "";
                HelpType = HelpType.None;
            }
            if (radioButtonURL.Checked)
            {
                HelpStr = HelpURL.Text;
                HelpType = HelpType.URL;
            }
            if (radioButtonPlain.Checked)
            {
                HelpStr = HelpPlainText.Text.Replace("\n", "\\n").Replace("\r", "").Replace("\t", "\\t");
                HelpType = HelpType.PlainText;
            }
            if (radioButtonRich.Checked)
            {
                HelpStr = Path.GetFileName(RTZPath);
                HelpType = HelpType.RichText;
            }

            if (MenuName.Text.Trim() != "")
            {
                MenuStr = MenuName.Text.Trim().Replace('\\', '/');
                SubMenuStr = SubMenuName.Text.Trim().Replace('\\','/');
                CreateConfigOverride = "";
            }
            else
            {
                MessageBox.Show("Please enter a menu name.", "Error");
                MenuName.Focus();
                return false;
            }
            return true;
        }

        private void ButtonSave_Click(object sender, EventArgs e)
        {
            if (UpdateAllValues())
            {
                if (radioButtonRich.Checked)
                {
                    HelpType = HelpType.RichText;
                    // save rtz file where the cs file is stored: RTZPath
                    string CompressedOutput = CompressString(RichHelpContent.Rtf);
                    File.WriteAllText(RTZPath, CompressedOutput);
                    // return filename
                    HelpStr = Path.GetFileName(RTZPath);
                }
                DialogResult = DialogResult.OK;
                this.Close();
            }
        }
        #endregion

        #region Select Icon
        private void SetIcon(string filePath)
        {
            if (!File.Exists(filePath))
            {
                MenuIcon.Image = null;
                IconPathStr = "";
                return;
            }

            Bitmap newicon;
            try
            {
                // Load the file as a bitmap
                newicon = new Bitmap(filePath);
            }
            catch
            {
                // If any errors happen, assume the file is invalid.
                MenuIcon.Image = null;
                IconPathStr = "";
                return;
            }

            // Make sure the icon is 16 x 16
            if ((newicon.Width != 16) || (newicon.Height != 16))
            {
                MenuIcon.Image = null;
                IconPathStr = "";
                MessageBox.Show("PNG file must be 16 x 16 pixels", "Improper File Selected");
                return;
            }

            // Load the icon to the message box
            MenuIcon.Image = newicon;
            IconPathStr = filePath;
        }

        private void ButtonIcon_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Load 16x16 PNG Graphic";       // File Open dialog box title
            ofd.Filter = "Icon Files (*.PNG)|*.png";    // Only PNG files are allowed
            ofd.DefaultExt = ".png";
            ofd.Multiselect = false;                    // only 1 file at a time is allowed

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                SetIcon(ofd.FileName);
            }
        }
        #endregion

        #region UBB and Help Preview
        RichTextBox rtb_HelpBox;
        Button btn_HelpBoxOKButton;

        private void rtb_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            Process.Start(e.LinkText);
            btn_HelpBoxOKButton.Focus();
        }

        private enum StyleTypes
        {
            Style,
            Color,
            BackColor,
            Indent,
            Alignment,
            Size,
            Baseline
        }

        private void rtb_FindMatchingUBBPair(string OpenUBBcode, FontStyle NewFontStyle, Color NewColor, StyleTypes NewStyleType, float NewSize, int NewBaseLineDirection, ref int FirstCodeLocation, ref int FirstEndLocation, ref FontStyle FirstStyle, ref Color FirstColor, ref float FirstSize, ref int FirstBaselineDirection, ref StyleTypes FirstStyleType, ref int FirstOpenCodeLength)
        {
            int OpenCodePosition = RichHelpContent.Find(OpenUBBcode);
            int CloseCodePosition = RichHelpContent.Find(OpenUBBcode.Insert(1, "/"), Math.Max(OpenCodePosition, 0), RichTextBoxFinds.NoHighlight);
            if ((OpenCodePosition != -1) && (CloseCodePosition != -1) && (OpenCodePosition < FirstCodeLocation))
            {
                FirstCodeLocation = OpenCodePosition;
                FirstEndLocation = CloseCodePosition;
                FirstStyle = NewFontStyle;
                FirstColor = NewColor;
                FirstSize = NewSize;
                FirstBaselineDirection = NewBaseLineDirection;
                FirstStyleType = NewStyleType;
                FirstOpenCodeLength = OpenUBBcode.Length;
            }
        }

        private void ChangeUBBtoRTF()
        {
            int EarliestTagFound = int.MaxValue;
            int MatchingEndTag = 0;
            FontStyle StyleToApply = FontStyle.Regular;
            StyleTypes StyleTypeToApply = StyleTypes.Style;
            int OpenCodeLength = 0;
            Color ColorToApply = Color.Black;
            float SizeToApply = 10f;
            int NewBaselineDirection = 0;
            RichHelpContent.SelectAll();
            RichHelpContent.SelectionIndent = 10;
            RichHelpContent.SelectionRightIndent = 10;
            RichHelpContent.Select(0, 0);
            RichHelpContent.SelectionFont = new Font(RichHelpContent.SelectionFont.Name, 5f, RichHelpContent.SelectionFont.Style);
            RichHelpContent.SelectedText = "\n";
            do
            {
                EarliestTagFound = int.MaxValue;
                rtb_FindMatchingUBBPair("[b]", FontStyle.Bold, Color.Black, StyleTypes.Style, 10f, 0, ref EarliestTagFound, ref MatchingEndTag, ref StyleToApply, ref ColorToApply, ref SizeToApply, ref NewBaselineDirection, ref StyleTypeToApply, ref OpenCodeLength);
                rtb_FindMatchingUBBPair("[i]", FontStyle.Italic, Color.Black, StyleTypes.Style, 10f, 0, ref EarliestTagFound, ref MatchingEndTag, ref StyleToApply, ref ColorToApply, ref SizeToApply, ref NewBaselineDirection, ref StyleTypeToApply, ref OpenCodeLength);
                rtb_FindMatchingUBBPair("[u]", FontStyle.Underline, Color.Black, StyleTypes.Style, 10f, 0, ref EarliestTagFound, ref MatchingEndTag, ref StyleToApply, ref ColorToApply, ref SizeToApply, ref NewBaselineDirection, ref StyleTypeToApply, ref OpenCodeLength);
                rtb_FindMatchingUBBPair("[s]", FontStyle.Strikeout, Color.Black, StyleTypes.Style, 10f, 0, ref EarliestTagFound, ref MatchingEndTag, ref StyleToApply, ref ColorToApply, ref SizeToApply, ref NewBaselineDirection, ref StyleTypeToApply, ref OpenCodeLength);
                rtb_FindMatchingUBBPair("[red]", FontStyle.Regular, Color.Red, StyleTypes.Color, 10f, 0, ref EarliestTagFound, ref MatchingEndTag, ref StyleToApply, ref ColorToApply, ref SizeToApply, ref NewBaselineDirection, ref StyleTypeToApply, ref OpenCodeLength);
                rtb_FindMatchingUBBPair("[blue]", FontStyle.Regular, Color.Blue, StyleTypes.Color, 10f, 0, ref EarliestTagFound, ref MatchingEndTag, ref StyleToApply, ref ColorToApply, ref SizeToApply, ref NewBaselineDirection, ref StyleTypeToApply, ref OpenCodeLength);
                rtb_FindMatchingUBBPair("[cyan]", FontStyle.Regular, Color.Cyan, StyleTypes.Color, 10f, 0, ref EarliestTagFound, ref MatchingEndTag, ref StyleToApply, ref ColorToApply, ref SizeToApply, ref NewBaselineDirection, ref StyleTypeToApply, ref OpenCodeLength);
                rtb_FindMatchingUBBPair("[green]", FontStyle.Regular, Color.Green, StyleTypes.Color, 10f, 0, ref EarliestTagFound, ref MatchingEndTag, ref StyleToApply, ref ColorToApply, ref SizeToApply, ref NewBaselineDirection, ref StyleTypeToApply, ref OpenCodeLength);
                rtb_FindMatchingUBBPair("[brown]", FontStyle.Regular, Color.Chocolate, StyleTypes.Color, 10f, 0, ref EarliestTagFound, ref MatchingEndTag, ref StyleToApply, ref ColorToApply, ref SizeToApply, ref NewBaselineDirection, ref StyleTypeToApply, ref OpenCodeLength);
                rtb_FindMatchingUBBPair("[white]", FontStyle.Regular, Color.White, StyleTypes.Color, 10f, 0, ref EarliestTagFound, ref MatchingEndTag, ref StyleToApply, ref ColorToApply, ref SizeToApply, ref NewBaselineDirection, ref StyleTypeToApply, ref OpenCodeLength);
                rtb_FindMatchingUBBPair("[yellow]", FontStyle.Regular, Color.Gold, StyleTypes.Color, 10f, 0, ref EarliestTagFound, ref MatchingEndTag, ref StyleToApply, ref ColorToApply, ref SizeToApply, ref NewBaselineDirection, ref StyleTypeToApply, ref OpenCodeLength);
                rtb_FindMatchingUBBPair("[purple]", FontStyle.Regular, Color.Purple, StyleTypes.Color, 10f, 0, ref EarliestTagFound, ref MatchingEndTag, ref StyleToApply, ref ColorToApply, ref SizeToApply, ref NewBaselineDirection, ref StyleTypeToApply, ref OpenCodeLength);
                rtb_FindMatchingUBBPair("[orange]", FontStyle.Regular, Color.DarkOrange, StyleTypes.Color, 10f, 0, ref EarliestTagFound, ref MatchingEndTag, ref StyleToApply, ref ColorToApply, ref SizeToApply, ref NewBaselineDirection, ref StyleTypeToApply, ref OpenCodeLength);
                rtb_FindMatchingUBBPair("[silver]", FontStyle.Regular, Color.Silver, StyleTypes.Color, 10f, 0, ref EarliestTagFound, ref MatchingEndTag, ref StyleToApply, ref ColorToApply, ref SizeToApply, ref NewBaselineDirection, ref StyleTypeToApply, ref OpenCodeLength);
                rtb_FindMatchingUBBPair("[sharpie]", FontStyle.Regular, Color.Black, StyleTypes.BackColor, 10f, 0, ref EarliestTagFound, ref MatchingEndTag, ref StyleToApply, ref ColorToApply, ref SizeToApply, ref NewBaselineDirection, ref StyleTypeToApply, ref OpenCodeLength);
                rtb_FindMatchingUBBPair("[highlighter]", FontStyle.Regular, Color.Gold, StyleTypes.BackColor, 10f, 0, ref EarliestTagFound, ref MatchingEndTag, ref StyleToApply, ref ColorToApply, ref SizeToApply, ref NewBaselineDirection, ref StyleTypeToApply, ref OpenCodeLength);
                rtb_FindMatchingUBBPair("[indent]", FontStyle.Regular, Color.Black, StyleTypes.Indent, 10f, 0, ref EarliestTagFound, ref MatchingEndTag, ref StyleToApply, ref ColorToApply, ref SizeToApply, ref NewBaselineDirection, ref StyleTypeToApply, ref OpenCodeLength);
                rtb_FindMatchingUBBPair("[center]", FontStyle.Regular, Color.Black, StyleTypes.Alignment, 10f, 0, ref EarliestTagFound, ref MatchingEndTag, ref StyleToApply, ref ColorToApply, ref SizeToApply, ref NewBaselineDirection, ref StyleTypeToApply, ref OpenCodeLength);
                rtb_FindMatchingUBBPair("[small]", FontStyle.Regular, Color.Black, StyleTypes.Size, 7.5f, 0, ref EarliestTagFound, ref MatchingEndTag, ref StyleToApply, ref ColorToApply, ref SizeToApply, ref NewBaselineDirection, ref StyleTypeToApply, ref OpenCodeLength);
                rtb_FindMatchingUBBPair("[big]", FontStyle.Regular, Color.Black, StyleTypes.Size, 13f, 0, ref EarliestTagFound, ref MatchingEndTag, ref StyleToApply, ref ColorToApply, ref SizeToApply, ref NewBaselineDirection, ref StyleTypeToApply, ref OpenCodeLength);
                rtb_FindMatchingUBBPair("[sup]", FontStyle.Regular, Color.Black, StyleTypes.Baseline, 7.5f, 1, ref EarliestTagFound, ref MatchingEndTag, ref StyleToApply, ref ColorToApply, ref SizeToApply, ref NewBaselineDirection, ref StyleTypeToApply, ref OpenCodeLength);
                rtb_FindMatchingUBBPair("[sub]", FontStyle.Regular, Color.Black, StyleTypes.Baseline, 7.5f, -1, ref EarliestTagFound, ref MatchingEndTag, ref StyleToApply, ref ColorToApply, ref SizeToApply, ref NewBaselineDirection, ref StyleTypeToApply, ref OpenCodeLength);
                if (EarliestTagFound < int.MaxValue)
                {
                    RichHelpContent.Select(EarliestTagFound, MatchingEndTag - EarliestTagFound);
                    switch (StyleTypeToApply)
                    {
                        case StyleTypes.Style:
                            RichHelpContent.SelectionFont = new Font(RichHelpContent.SelectionFont, RichHelpContent.SelectionFont.Style | StyleToApply);
                            break;
                        case StyleTypes.Color:
                            RichHelpContent.SelectionColor = ColorToApply;
                            break;
                        case StyleTypes.BackColor:
                            RichHelpContent.SelectionBackColor = ColorToApply;
                            break;
                        case StyleTypes.Indent:
                            RichHelpContent.SelectionIndent = RichHelpContent.SelectionIndent + 20;
                            break;
                        case StyleTypes.Alignment:
                            RichHelpContent.SelectionAlignment = HorizontalAlignment.Center;
                            break;
                        case StyleTypes.Size:
                            RichHelpContent.SelectionFont = new Font(RichHelpContent.SelectionFont.Name, SizeToApply, RichHelpContent.SelectionFont.Style);
                            break;
                        case StyleTypes.Baseline:
                            RichHelpContent.SelectionCharOffset = RichHelpContent.SelectionFont.Height / 3 * NewBaselineDirection;
                            RichHelpContent.SelectionFont = new Font(RichHelpContent.SelectionFont.Name, Math.Max(RichHelpContent.SelectionFont.Size * 0.75f, SizeToApply), RichHelpContent.SelectionFont.Style);
                            break;
                        default:
                            break;
                    }
                    RichHelpContent.Select(MatchingEndTag, OpenCodeLength + 1);
                    RichHelpContent.SelectedText = "";
                    RichHelpContent.Select(EarliestTagFound, OpenCodeLength);
                    RichHelpContent.SelectedText = "";
                }
            } while (EarliestTagFound < int.MaxValue);
            int findt = RichHelpContent.Find("[t]");
            while (findt > -1)
            {
                RichHelpContent.Select(findt, 3);
                RichHelpContent.SelectedText = "\\t";
                findt = RichHelpContent.Find("[t]");
            }
            int findn = RichHelpContent.Find("[n]");
            while (findn > -1)
            {
                RichHelpContent.Select(findn, 3);
                RichHelpContent.SelectedText = "\\n";
                findn = RichHelpContent.Find("[n]");
            }
            RichHelpContent.Select(0, 0);
        }

        private static string LoadLocalizedString(string libraryName, uint ident, string defaultText)
        {
            IntPtr libraryHandle = GetModuleHandle(libraryName);
            if (libraryHandle != IntPtr.Zero)
            {
                StringBuilder sb = new StringBuilder(1024);
                if (LoadString(libraryHandle, ident, sb, 1024) > 0)
                {
                    return sb.ToString();
                }
            }
            return defaultText;
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int LoadString(IntPtr hInstance, uint uID, StringBuilder lpBuffer, int nBufferMax);

        private void PreviewHelp_Click(object sender, EventArgs e)
        {
            if (radioButtonURL.Checked)
            {
                if (HelpURL.Text.StartsWith("http://", StringComparison.OrdinalIgnoreCase) || HelpURL.Text.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                {
                    Process.Start(HelpURL.Text);
                }
                else
                {
                    MessageBox.Show("Specified URL should start with 'http://' or 'https://'\r\n\r\nFix your URL and try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            if (radioButtonPlain.Checked)
            {
                MessageBox.Show(HelpPlainText.Text, MenuName.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            if (radioButtonRich.Checked)
            {
                using (Form form = new Form())
                {
                    form.SuspendLayout();
                    form.AutoScaleDimensions = new SizeF(96F, 96F);
                    form.AutoScaleMode = AutoScaleMode.Dpi;
                    form.Text = MenuName.Text + " - " + LoadLocalizedString("user32.dll", 808, "Help");
                    form.AutoSize = false;
                    form.ClientSize = new Size(564, 392);
                    form.MinimumSize = new Size(330, 282);
                    form.FormBorderStyle = FormBorderStyle.Sizable;
                    form.ShowInTaskbar = false;
                    form.MinimizeBox = false;
                    form.StartPosition = FormStartPosition.CenterParent;
                    if (MenuIcon.Image != null)
                    {
                        form.Icon = Icon.FromHandle(((Bitmap)MenuIcon.Image).GetHicon());
                    }
                    else
                    {
                        form.ShowIcon = false;
                    }
                    btn_HelpBoxOKButton = new Button();
                    btn_HelpBoxOKButton.AutoSize = true;
                    btn_HelpBoxOKButton.Text = LoadLocalizedString("user32.dll", 800, "OK");
                    btn_HelpBoxOKButton.DialogResult = DialogResult.Cancel;
                    btn_HelpBoxOKButton.Size = new Size(84, 24);
                    btn_HelpBoxOKButton.Location = new Point(472, 359);
                    btn_HelpBoxOKButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
                    form.Controls.Add(btn_HelpBoxOKButton);
                    rtb_HelpBox = new RichTextBox();
                    rtb_HelpBox.Size = new Size(564, 350);
                    rtb_HelpBox.Location = new Point(0, 0);
                    rtb_HelpBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Right;
                    rtb_HelpBox.DetectUrls = true;
                    rtb_HelpBox.WordWrap = true;
                    rtb_HelpBox.ScrollBars = RichTextBoxScrollBars.ForcedVertical;
                    rtb_HelpBox.BorderStyle = BorderStyle.None;
                    rtb_HelpBox.Font = new Font(rtb_HelpBox.SelectionFont.Name, 10f);
                    rtb_HelpBox.LinkClicked += new LinkClickedEventHandler(rtb_LinkClicked);
                    rtb_HelpBox.ReadOnly = false;
                    rtb_HelpBox.Rtf = RichHelpContent.Rtf;
                    //rtb_HelpBox.Text = RichHelpContent.Text.Replace("\\\\t", "[t]").Replace("\\\\n", "[n]");
                    //rtb_ChangeUBBtoRTF();
                    rtb_HelpBox.ReadOnly = true;
                    form.Controls.Add(rtb_HelpBox);
                    form.ResumeLayout();
                    form.ShowDialog();
                }
            }
        }
        #endregion

        #region Compression
        private static string CompressString(string text)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(text);
            var memoryStream = new MemoryStream();
            using (var gZipStream = new GZipStream(memoryStream, CompressionMode.Compress, true))
            {
                gZipStream.Write(buffer, 0, buffer.Length);
            }

            memoryStream.Position = 0;

            var compressedData = new byte[memoryStream.Length];
            memoryStream.Read(compressedData, 0, compressedData.Length);

            var gZipBuffer = new byte[compressedData.Length + 4];
            Buffer.BlockCopy(compressedData, 0, gZipBuffer, 4, compressedData.Length);
            Buffer.BlockCopy(BitConverter.GetBytes(buffer.Length), 0, gZipBuffer, 0, 4);
            return Convert.ToBase64String(gZipBuffer);
        }

        private static string DecompressString(string compressedText)
        {
            byte[] gZipBuffer = Convert.FromBase64String(compressedText);
            using (var memoryStream = new MemoryStream())
            {
                int dataLength = BitConverter.ToInt32(gZipBuffer, 0);
                memoryStream.Write(gZipBuffer, 4, gZipBuffer.Length - 4);

                var buffer = new byte[dataLength];

                memoryStream.Position = 0;
                using (var gZipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
                {
                    gZipStream.Read(buffer, 0, buffer.Length);
                }

                return Encoding.UTF8.GetString(buffer);
            }
        }
        #endregion

        #region RTF Editor functions
        private void UpdateReadOnlyFields()
        {
            if (radioButtonNone.Checked)
            {
                HelpURL.Enabled = false;
                HelpPlainText.Enabled = false;
                PlainTextLabel.Enabled = false;
                RichHelpContent.Enabled = false;
                toolStrip1.Enabled = false;
                PreviewLabel.Enabled = false;
                PreviewHelpButton.Enabled = false;
            }
            if (radioButtonURL.Checked)
            {
                HelpURL.Enabled = true;
                HelpURL.Focus();
                HelpURL.Select(0, 0);
                HelpPlainText.Enabled = false;
                PlainTextLabel.Enabled = false;
                RichHelpContent.Enabled = false;
                toolStrip1.Enabled = false;
                PreviewLabel.Enabled = true;
                PreviewHelpButton.Enabled = true;
            }
            if (radioButtonPlain.Checked)
            {
                HelpURL.Enabled = false;
                HelpPlainText.Enabled = true;
                PlainTextLabel.Enabled = true;
                HelpPlainText.Focus();
                HelpPlainText.Select(HelpPlainText.Text.Length, HelpPlainText.Text.Length);
                RichHelpContent.Enabled = false;
                toolStrip1.Enabled = false;
                PreviewLabel.Enabled = true;
                PreviewHelpButton.Enabled = true;
            }
            if (radioButtonRich.Checked)
            {
                HelpURL.Enabled = false;
                HelpPlainText.Enabled = false;
                PlainTextLabel.Enabled = false;
                RichHelpContent.Enabled = true;
                toolStrip1.Enabled = true;
                RichHelpContent.Focus();
                RichHelpContent.Select(0, 0);
                PreviewLabel.Enabled = true;
                PreviewHelpButton.Enabled = true;
            }
        }

        private void radioButtonNone_CheckedChanged(object sender, EventArgs e)
        {
            UpdateReadOnlyFields();
        }

        private void radioButtonURL_CheckedChanged(object sender, EventArgs e)
        {
            UpdateReadOnlyFields();
        }

        private void radioButtonPlain_CheckedChanged(object sender, EventArgs e)
        {
            UpdateReadOnlyFields();
        }

        private void radioButtonRich_CheckedChanged(object sender, EventArgs e)
        {
            UpdateReadOnlyFields();
        }

        private void InsertImage(Image img)
        {
            IDataObject obj = Clipboard.GetDataObject();
            Clipboard.Clear();

            Clipboard.SetImage(img);
            RichHelpContent.Paste();

            Clipboard.Clear();
            Clipboard.SetDataObject(obj);
        }

        private void DoColor()
        {
            ColorWindow colorDialog1 = new ColorWindow();
            Color CurrentColor = RichHelpContent.SelectionColor;
            CurrentColor = Color.FromArgb(255, CurrentColor);
            colorDialog1.Color = CurrentColor;
            colorDialog1.ShowAlpha = false;
            if (colorDialog1.ShowDialog() == DialogResult.OK && colorDialog1.Color != RichHelpContent.SelectionColor)
            {
                RichHelpContent.SelectionColor = colorDialog1.Color;
            }
        }

        private void DoBold()
        {
            if (RichHelpContent.SelectionFont.Bold)
            {
                RichHelpContent.SelectionFont = new Font(RichHelpContent.SelectionFont, RichHelpContent.SelectionFont.Style ^ FontStyle.Bold);
            }
            else
            {
                RichHelpContent.SelectionFont = new Font(RichHelpContent.SelectionFont, RichHelpContent.SelectionFont.Style | FontStyle.Bold);
            }
        }

        private void DoItalics()
        {
            if (RichHelpContent.SelectionFont.Italic)
            {
                RichHelpContent.SelectionFont = new Font(RichHelpContent.SelectionFont, RichHelpContent.SelectionFont.Style ^ FontStyle.Italic);
            }
            else
            {
                RichHelpContent.SelectionFont = new Font(RichHelpContent.SelectionFont, RichHelpContent.SelectionFont.Style | FontStyle.Italic);
            }
        }

        private void DoUnderline()
        {
            if (RichHelpContent.SelectionFont.Underline)
            {
                RichHelpContent.SelectionFont = new Font(RichHelpContent.SelectionFont, RichHelpContent.SelectionFont.Style ^ FontStyle.Underline);
            }
            else
            {
                RichHelpContent.SelectionFont = new Font(RichHelpContent.SelectionFont, RichHelpContent.SelectionFont.Style | FontStyle.Underline);
            }
        }

        private void DoOpen()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Open Help File";
            ofd.Filter = "Rich Text Format (*.RTF)|*.RTF|Compressed Rich Text Format (*.RTZ)|*.RTZ|Text Format with UBB Codes (*.TXT)|*.TXT";
            ofd.DefaultExt = ".rtf";
            ofd.Multiselect = false;
            ofd.InitialDirectory = Settings.LastSourceDirectory;
            if (ofd.ShowDialog() == DialogResult.OK && File.Exists(ofd.FileName))
            {
                try
                {
                    string fileExtenion = Path.GetExtension(ofd.FileName);
                    if (fileExtenion.Equals(".rtf", StringComparison.OrdinalIgnoreCase))
                    {
                        RichHelpContent.ResetText();
                        RichHelpContent.Rtf = File.ReadAllText(ofd.FileName);
                    }
                    else if (fileExtenion.Equals(".rtz", StringComparison.OrdinalIgnoreCase))
                    {
                        string FileContents = File.ReadAllText(ofd.FileName);
                        string ExpandedContents = DecompressString(FileContents);
                        RichHelpContent.ResetText();
                        RichHelpContent.Rtf = ExpandedContents;
                    }
                    else if (fileExtenion.Equals(".txt", StringComparison.OrdinalIgnoreCase))
                    {
                        RichHelpContent.ResetText();
                        RichHelpContent.Text = File.ReadAllText(ofd.FileName);
                        ChangeUBBtoRTF();
                    }
                }
                catch
                {
                }
            }
            RichHelpContent.Focus();
        }

        private void DoSave(bool OpenInWordPad)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Title = "Save Help File";
            sfd.FileName = Path.ChangeExtension(FileName,".rtf");
            sfd.Filter = "Rich Text Format (*.RTF)|*.RTF";
            sfd.DefaultExt = ".rtf";
            sfd.InitialDirectory = Settings.LastSourceDirectory;

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    File.WriteAllText(sfd.FileName, RichHelpContent.Rtf);
                    if (OpenInWordPad)
                    {
                        WarningLabel.Visible = true;
                        Application.DoEvents();
                        string sysFolder = Environment.GetFolderPath(Environment.SpecialFolder.System);
                        Process p = Process.Start("wordpad.exe", "\"" + sfd.FileName + "\"");
                        p.WaitForInputIdle();
                        p.WaitForExit();
                        WarningLabel.Visible = false;
                        RichHelpContent.Rtf = File.ReadAllText(sfd.FileName);
                    }
                }
                catch
                {
                }
            }
        }

        private void DoSuperscript()
        {
            if (RichHelpContent.SelectionCharOffset == 0)
            {
                RichHelpContent.SelectionCharOffset = 5;
            }
            else
            {
                RichHelpContent.SelectionCharOffset = 0;
            }
        }

        private void DoSubscript()
        {
            if (RichHelpContent.SelectionCharOffset == 0)
            {
                RichHelpContent.SelectionCharOffset = -5;
            }
            else
            {
                RichHelpContent.SelectionCharOffset = 0;
            }
        }

        private void DoLargeFont()
        {
            RichHelpContent.SelectionFont = new Font(RichHelpContent.SelectionFont.Name, (RichHelpContent.SelectionFont.SizeInPoints + 2 > 72) ? 72 : RichHelpContent.SelectionFont.SizeInPoints + 2);
        }

        private void DoSmallFont()
        {
            RichHelpContent.SelectionFont = new Font(RichHelpContent.SelectionFont.Name, (RichHelpContent.SelectionFont.SizeInPoints - 2 < 2) ? 2 : RichHelpContent.SelectionFont.SizeInPoints - 2);
        }

        private void DoBullet()
        {
            RichHelpContent.SelectionBullet = !RichHelpContent.SelectionBullet;
        }

        private void DoIndent()
        {
            RichHelpContent.SelectionIndent += 20;
        }

        private void DoUnindent()
        {
            RichHelpContent.SelectionIndent -= 20;
        }

        private void DoLeft()
        {
            RichHelpContent.SelectionAlignment = HorizontalAlignment.Left;
        }

        private void DoCenter()
        {
            RichHelpContent.SelectionAlignment = HorizontalAlignment.Center;
        }
        #endregion

        #region RTF Editor Toolbar Buttons
        private void BoldButton_Click(object sender, EventArgs e)
        {
            DoBold();
        }

        private void ItalicsButton_Click(object sender, EventArgs e)
        {
            DoItalics();
        }

        private void UnderlineButton_Click(object sender, EventArgs e)
        {
            DoUnderline();
        }

        private void OpenButton_Click(object sender, EventArgs e)
        {
            DoOpen();
        }

        private void SaveButton_Click_1(object sender, EventArgs e)
        {
            DoSave(false);
        }

        private void WordPadButton_Click(object sender, EventArgs e)
        {
            DoSave(true);
        }

        private void SuperScriptButton_Click(object sender, EventArgs e)
        {
            DoSuperscript();
        }

        private void SubScriptButton_Click(object sender, EventArgs e)
        {
            DoSubscript();
        }

        private void LargeFontButton_Click(object sender, EventArgs e)
        {
            DoLargeFont();
        }

        private void SmallFontButton_Click(object sender, EventArgs e)
        {
            DoSmallFont();
        }

        private void BulletButton_Click(object sender, EventArgs e)
        {
            DoBullet();
        }

        private void IndentButton_Click(object sender, EventArgs e)
        {
            DoIndent();
        }

        private void UnindentButton_Click(object sender, EventArgs e)
        {
            DoUnindent();
        }

        private void ParagraphLeftButton_Click(object sender, EventArgs e)
        {
            DoLeft();
        }

        private void CenterButton_Click(object sender, EventArgs e)
        {
            DoCenter();
        }

        private void InsertImageButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Open Image File";
            ofd.Filter = "Image Files(*.PNG;*.BMP;*.JPG;*.GIF)|*.PNG;*.BMP;*.JPG;*.GIF|All files (*.*)|*.*";
            ofd.DefaultExt = ".png";
            ofd.Multiselect = false;
            ofd.InitialDirectory = Settings.LastSourceDirectory;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                Bitmap aimg = null;
                try
                {
                    aimg = (Bitmap)Image.FromFile(ofd.FileName, false);
                }
                catch (Exception)
                {
                }
                if (aimg != null)
                {
                    InsertImage(aimg);
                }
                else
                {
                    MessageBox.Show("There was a problem opening the image.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

        }

        private void ColorButton_Click(object sender, EventArgs e)
        {
            DoColor();
        }
        #endregion

        #region RTF Editor Keys
        private void RichHelpContent_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Modifiers == Keys.Control && e.KeyCode == Keys.B)
            {
                DoBold();
                e.Handled = true;
            }
            if (e.Modifiers == Keys.Control && e.KeyCode == Keys.I)
            {
                DoItalics();
                e.Handled = true;
            }
            if (e.Modifiers == Keys.Control && e.KeyCode == Keys.U)
            {
                DoUnderline();
                e.Handled = true;
            }
            if (e.Modifiers == Keys.Control && e.KeyCode == Keys.O)
            {
                DoOpen();
                e.Handled = true;
            }
            if (e.Modifiers == Keys.Control && e.KeyCode == Keys.S)
            {
                DoSave(false);
                e.Handled = true;
            }
            if (e.Modifiers == Keys.Control && e.KeyCode == Keys.W)
            {
                DoSave(true);
                e.Handled = true;
            }
            if ((e.KeyCode == Keys.F8) || (e.Modifiers == Keys.Alt && e.KeyCode == Keys.C))
            {
                DoColor();
                e.Handled = true;
            }
        }
        #endregion

        private void ViewSourceButton_Click(object sender, EventArgs e)
        {
            if (!UpdateAllValues())
            {
                return;
            }

            string SourceCode = ScriptWriter.FullSourceCode(FullScriptText, FileName, isAdjustment, SubMenuName.Text, MenuName.Text, IconPathStr, Support, ForceAliasSelection, ForceSingleThreaded, Author, MajorVer, MinorVer, Description, KeyWords, WindowTitleTextStr, HelpType, HelpStr);
            using (ViewSrc VSW = new ViewSrc("Full Source Code", SourceCode, true))
            {
                VSW.ShowDialog();
            }
        }

        private void GenSlnButton_Click(object sender, EventArgs e)
        {
            if (!UpdateAllValues())
            {
                return;
            }

            if (radioButtonRich.Checked)
            {
                string CompressedOutput = CompressString(RichHelpContent.Rtf);
                File.WriteAllText(RTZPath, CompressedOutput);
            }

            using (FolderBrowserDialog fbd = new FolderBrowserDialog())
            {
                fbd.SelectedPath = Settings.LastSlnDirectory;
                fbd.ShowNewFolderButton = true;
                fbd.Description = "Choose a Folder to place the generated Visual Studio Solution.";

                if (fbd.ShowDialog() == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    string SourceCode = ScriptWriter.FullSourceCode(FullScriptText, FileName, isAdjustment, SubMenuName.Text, MenuName.Text, IconPathStr, Support, ForceAliasSelection, ForceSingleThreaded, Author, MajorVer, MinorVer, Description, KeyWords, WindowTitleTextStr, HelpType, HelpStr);
                    Solution.Generate(fbd.SelectedPath, FileName, SourceCode, IconPathStr);

                    Settings.LastSlnDirectory = fbd.SelectedPath;
                }
            }
        }
    }

    internal enum HelpType
    {
        None,
        URL,
        PlainText,
        RichText
    }
}
