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
// Latest distribution: https://www.BoltBait.com/pdn/codelab
/////////////////////////////////////////////////////////////////////////////////

using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
using System.IO.Compression;
using System.Text;

namespace PdnCodeLab
{
    internal partial class BuildForm : ChildFormBase, IToolTipHost
    {
        #region Constructor
        internal string IconPath = "";
        internal string SubMenu = "";
        internal string MenuItemName = "";
        internal string WindowTitle = "";
        internal string Author = "";
        internal string URL = "";
        internal int MajorVer = 0;
        internal int MinorVer = 0;
        internal bool isAdjustment = false;
        internal string Description = "";
        internal string KeyWords = "";
        internal ScriptRenderingFlags RenderingFlags = ScriptRenderingFlags.None;
        internal ScriptRenderingSchedule RenderingSchedule = ScriptRenderingSchedule.Default;
        internal HelpType HelpType = 0;
        internal string HelpStr = "";
        internal string RTZPath = "";
        private readonly string FullScriptText = "";
        private readonly string FileName = "";
        private readonly string resourcePath;
        private readonly bool canCreateSln;
        private readonly bool customHelp;
        private readonly ProjectType projectType;

        internal BuildForm(string ScriptName, string ScriptText, string ScriptPath, ProjectType projectType, bool canCreateSln)
        {
            InitializeComponent();

            HelpPlainText.Font = this.Font;

            this.projectType = projectType;
            this.canCreateSln = canCreateSln;

            WarningLabel.Visible = false;

            // Set dialog box title
            this.Text = "Building " + ScriptName + ".dll";
            this.RTZPath = Path.ChangeExtension(ScriptPath, ".rtz");
            DecimalSymbol.Text = System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator;

            #region Populate fields from script comments
            FullScriptText = ScriptText;
            FileName = ScriptName;

            // Will the plugin have a User Interface
            bool hasUI = UIElement.ProcessUIControls(ScriptText, projectType).Length > 0;

            // Preload submenu name
            Match msm = Regex.Match(ScriptText, @"//[\s-[\r\n]]*SubMenu[\s-[\r\n]]*:[\s-[\r\n]]*(?<subLabel>.*)(?=\r?\n|$)", RegexOptions.IgnoreCase);
            if (msm.Success)
            {
                SubMenuName.Text = msm.Groups["subLabel"].Value.Trim();
                if (SubMenuName.Text.Equals("adjustments", StringComparison.OrdinalIgnoreCase) ||
                    SubMenuName.Text.Equals("adj", StringComparison.OrdinalIgnoreCase))
                {
                    AdjustmentRadio.Checked = true;
                    SubMenuName.Text = string.Empty;
                }
            }

            // Preload menu name
            Match mmn = Regex.Match(ScriptText, @"//[\s-[\r\n]]*Name[\s-[\r\n]]*:[\s-[\r\n]]*(?<menuLabel>.*)(?=\r?\n|$)", RegexOptions.IgnoreCase);
            if (mmn.Success)
            {
                string menuName = mmn.Groups["menuLabel"].Value.Trim();
                MenuName.Text = (menuName.Length > 0) ? menuName : ScriptName;
            }
            else
            {
                MenuName.Text = ScriptName;
            }

            // Preload window title
            if (hasUI)
            {
                Match wtn = Regex.Match(ScriptText, @"//[\s-[\r\n]]*Title[\s-[\r\n]]*:[\s-[\r\n]]*(?<titleLabel>.*)(?=\r?\n|$)", RegexOptions.IgnoreCase);
                if (wtn.Success)
                {
                    WindowTitleText.Text = wtn.Groups["titleLabel"].Value.Trim();
                }
            }
            else
            {
                label3.Enabled = false;
                WindowTitleText.Enabled = false;
            }

            // Preload version checking for Major.Minor (period or comma)
            Match vsn = Regex.Match(ScriptText, @"//[\s-[\r\n]]*Version[\s-[\r\n]]*:[\s-[\r\n]]*(?<majorVersionLabel>\d+)[\.\,](?<minorVersionLabel>\d+)(?=\r?\n|$)", RegexOptions.IgnoreCase);
            if (!vsn.Success)
            {
                // Preload version checking for just Major
                vsn = Regex.Match(ScriptText, @"//[\s-[\r\n]]*Version[\s-[\r\n]]*:[\s-[\r\n]]*(?<majorVersionLabel>\d+)(?=\r?\n|$)", RegexOptions.IgnoreCase);
            }
            if (vsn.Success)
            {
                if (decimal.TryParse(vsn.Groups["majorVersionLabel"].Value.Trim(), out decimal majorVer))
                {
                    MajorVersion.Value = Math.Clamp(majorVer, MajorVersion.Minimum, MajorVersion.Maximum);
                }
                if (decimal.TryParse(vsn.Groups["minorVersionLabel"].Value.Trim(), out decimal minorVer))
                {
                    MinorVersion.Value = Math.Clamp(minorVer, MinorVersion.Minimum, MinorVersion.Maximum);
                }
            }

            // Preload author's name
            Match mau = Regex.Match(ScriptText, @"//[\s-[\r\n]]*Author[\s-[\r\n]]*:[\s-[\r\n]]*(?<authorLabel>.*)(?=\r?\n|$)", RegexOptions.IgnoreCase);
            if (mau.Success)
            {
                AuthorName.Text = mau.Groups["authorLabel"].Value.Trim();
            }

            // Preload Description
            Match mds = Regex.Match(ScriptText, @"//[\s-[\r\n]]*Desc[\s-[\r\n]]*:[\s-[\r\n]]*(?<descLabel>.*)(?=\r?\n|$)", RegexOptions.IgnoreCase);
            DescriptionBox.Text = mds.Success ?
                mds.Groups["descLabel"].Value.Trim() :
                ScriptName + " selected pixels";

            // Preload Keywords
            Match mkw = Regex.Match(ScriptText, @"//[\s-[\r\n]]*KeyWords[\s-[\r\n]]*:[\s-[\r\n]]*(?<wordsLabel>.*)(?=\r?\n|$)", RegexOptions.IgnoreCase);
            KeyWordsBox.Text = mkw.Success ?
                mkw.Groups["wordsLabel"].Value.Trim() :
                ScriptName;

            // Preload Support URL
            Match msu = Regex.Match(ScriptText, @"//[\s-[\r\n]]*URL[\s-[\r\n]]*:[\s-[\r\n]]*(?<urlLabel>.*)(?=\r?\n|$)", RegexOptions.IgnoreCase);
            if (msu.Success)
            {
                SupportURL.Text = msu.Groups["urlLabel"].Value.Trim();
            }

            // Preload Force Aliased Selection
            ForceAliasSelectionBox.Checked = Regex.IsMatch(ScriptText, @"//[\s-[\r\n]]*(Force\s*Aliased\s*Selection|FAS)[\s-[\r\n]]*(?=\r?\n|$)", RegexOptions.IgnoreCase);

            // Preload Force Single Threaded
            ForceSingleThreadedBox.Checked = Regex.IsMatch(ScriptText, @"//[\s-[\r\n]]*(Force\s*Single\s*Threaded|FST)[\s-[\r\n]]*(?=\r?\n|$)", RegexOptions.IgnoreCase);

            // Preload Force Legacy ROI
            forceLegacyRoiBox.Checked = Regex.IsMatch(ScriptText, @"//[\s-[\r\n]]*(Force\s*Legacy\s*ROI|FLR)[\s-[\r\n]]*(?=\r?\n|$)", RegexOptions.IgnoreCase);

            // Preload Single Render Call
            forceSingleRenderBox.Checked = Regex.IsMatch(ScriptText, @"//[\s-[\r\n]]*(Force\s*Single\s*Render\s*Call|FSR)[\s-[\r\n]]*(?=\r?\n|$)", RegexOptions.IgnoreCase);

            // Disable Selection Clipping
            NoSelectionClippingBox.Checked = Regex.IsMatch(ScriptText, @"//[\s-[\r\n]]*(No\s*Selection\s*Clipping|NSC)[\s-[\r\n]]*(?=\r?\n|$)", RegexOptions.IgnoreCase);

            // Premultiplied/Straight Alpha for GPU Effects
            StraightAlphaBox.Checked = Regex.IsMatch(ScriptText, @"//[\s-[\r\n]]*(Straight\s*Alpha|SA)[\s-[\r\n]]*(?=\r?\n|$)", RegexOptions.IgnoreCase);

            // WorkingSpace/WorkingSpaceLinear for GPU Effects
            WorkingSpaceColorContextBox.Checked = Regex.IsMatch(ScriptText, @"//[\s-[\r\n]]*(Working\s*Space\s*Color\s*Context|WSCC)[\s-[\r\n]]*(?=\r?\n|$)", RegexOptions.IgnoreCase);

            if (projectType == ProjectType.ClassicEffect)
            {
                NoSelectionClippingBox.Checked = false;
                StraightAlphaBox.Checked = false;
                WorkingSpaceColorContextBox.Checked = false;

                NoSelectionClippingBox.Enabled = false;
                StraightAlphaBox.Enabled = false;
                WorkingSpaceColorContextBox.Enabled = false;
            }
            #endregion

            resourcePath = Path.Combine(Path.GetDirectoryName(ScriptPath), ScriptName);

            #region Load Help Text
            if (hasUI)
            {
                // Preload help text
                Match hlp = Regex.Match(ScriptText, @"//[\s-[\r\n]]*Help[\s-[\r\n]]*:[\s-[\r\n]]*(?<helpText>.*)(?=\r?\n|$)", RegexOptions.IgnoreCase);
                if (hlp.Success)
                {
                    HelpStr = hlp.Groups["helpText"].Value.Trim();
                    if (HelpStr.IsWebAddress())
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

                if (HelpPlainText.Text.Length == 0)
                {
                    HelpPlainText.Text = $"{MenuName.Text} v{MajorVersion.Value}{DecimalSymbol.Text}{MinorVersion.Value}\r\nCopyright ©{DateTime.Now.Year} by {AuthorName.Text}\r\nAll rights reserved.";
                    if (radioButtonNone.Checked)
                    {
                        radioButtonPlain.Checked = true;
                    }
                }

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

                if (Regex.IsMatch(ScriptText, @"void OnWindowHelpButtonClicked\(IWin32Window owner, string helpContent\)(\s)*{(.|\s)*}", RegexOptions.Singleline))
                {
                    customHelp = true;
                    radioButtonNone.Checked = true;
                    radioButtonNone.Text = "Custom";

                    radioButtonURL.Enabled = false;
                    radioButtonPlain.Enabled = false;
                    radioButtonRich.Enabled = false;
                }
            }
            else
            {
                radioButtonNone.Text = "None - Plugin has no User Interface";

                radioButtonURL.Enabled = false;
                radioButtonPlain.Enabled = false;
                radioButtonRich.Enabled = false;
            }
            #endregion

            #region Load default icon
            // See if a default icon exists
            string iconPath = Path.ChangeExtension(resourcePath, ".png");
            SetIcon(iconPath);
            #endregion

            #region Load sample image
            // See if a sample image exists
            string samplePath = Path.ChangeExtension(resourcePath, ".sample.png");
            if (File.Exists(samplePath))
            {
                Bitmap SampleImage = UIUtil.GetBitmapFromFile(samplePath);
                if (SampleImage != null)
                {
                    if ((SampleImage.Width != 200) || (SampleImage.Height != 150))
                    {
                        sampleLabel.Text = "The sample image " + Path.GetFileName(samplePath) + " was detected, but it was the wrong size. PNG file must be 200x150 pixels. You may continue without a sample image.";
                        sampleLabel.Visible = true;
                        sampleImage.Visible = false;
                    }
                    else
                    {
                        sampleImage.Image = SampleImage;
                        sampleLabel.Text = "Sample Image Detected:";
                        sampleLabel.Visible = true;
                        sampleImage.Visible = true;
                    }
                }
                else
                {
                    sampleLabel.Text = "Something went wrong trying to load your sample image.";
                    sampleLabel.Visible = true;
                    sampleImage.Visible = false;
                }
            }
            else
            {
                sampleLabel.Text = Path.GetFileName(samplePath) + " not detected.  If you would like to include a sample image, create a PNG file of size 200x150 and place it in the same directory as your source file.";
                sampleLabel.Visible = true;
                sampleImage.Visible = false;
            }
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
            URL = SupportURL.Text.Replace('\\', '/');
            WindowTitle = WindowTitleText.Text.Trim().Replace('\\', '/');
            isAdjustment = AdjustmentRadio.Checked;
            Description = DescriptionBox.Text.Trim().Replace('\\', '/');
            KeyWords = KeyWordsBox.Text.Trim().Replace('\\', '/');

            this.RenderingSchedule = ScriptRenderingSchedule.Default;
            if (forceLegacyRoiBox.Checked)
            {
                this.RenderingSchedule = ScriptRenderingSchedule.HorizontalStrips;
            }
            else if (forceSingleRenderBox.Checked)
            {
                this.RenderingSchedule = ScriptRenderingSchedule.None;
            }

            this.RenderingFlags = ScriptRenderingFlags.None;
            if (ForceAliasSelectionBox.Checked) { this.RenderingFlags |= ScriptRenderingFlags.AliasedSelection; }
            if (ForceSingleThreadedBox.Checked) { this.RenderingFlags |= ScriptRenderingFlags.SingleThreaded; }
            if (NoSelectionClippingBox.Checked) { this.RenderingFlags |= ScriptRenderingFlags.NoSelectionClipping; }
            if (StraightAlphaBox.Checked) { this.RenderingFlags |= ScriptRenderingFlags.StraightAlpha; }
            if (WorkingSpaceColorContextBox.Checked) { this.RenderingFlags |= ScriptRenderingFlags.WorkingSpaceColorContext; }

            if (radioButtonNone.Checked)
            {
                HelpStr = "";
                HelpType = customHelp ? HelpType.Custom : HelpType.None;
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
                MenuItemName = MenuName.Text.Trim().Replace('\\', '/');
                SubMenu = SubMenuName.Text.Trim().Replace('\\','/');
            }
            else
            {
                FlexibleMessageBox.Show("Please enter a menu name.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                IconPath = "";
                return;
            }

            Bitmap newIcon = UIUtil.GetBitmapFromFile(filePath);
            if (newIcon is null)
            {
                MenuIcon.Image = null;
                IconPath = "";
                return;
            }

            // Make sure the icon is square, and is at least 16 x 16
            if (newIcon.Width != newIcon.Height || newIcon.Width < 16)
            {
                MenuIcon.Image = null;
                IconPath = "";
                FlexibleMessageBox.Show("PNG file must be square and at least 16 x 16 pixels", "Improper File Selected", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Load the icon to the message box
            MenuIcon.Image = newIcon;
            IconPath = filePath;
        }

        private void ButtonIcon_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            using OpenFileDialog ofd = new OpenFileDialog
            {
                Title = "Load Icon PNG Graphic",        // File Open dialog box title
                Filter = "Icon Files (*.PNG)|*.png",    // Only PNG files are allowed
                DefaultExt = ".png",
                Multiselect = false                     // only 1 file at a time is allowed
            };

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                SetIcon(ofd.FileName);
            }
        }
        #endregion

        #region UBB to RTF
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
                            RichHelpContent.SelectionIndent += 20;
                            break;
                        case StyleTypes.Alignment:
                            RichHelpContent.SelectionAlignment = HorizontalAlignment.Center;
                            break;
                        case StyleTypes.Size:
                            RichHelpContent.SelectionFont = new Font(RichHelpContent.SelectionFont.Name, SizeToApply, RichHelpContent.SelectionFont.Style);
                            break;
                        case StyleTypes.Baseline:
                            RichHelpContent.SelectionCharOffset = RichHelpContent.SelectionFont.Height / 3 * NewBaselineDirection;
                            RichHelpContent.SelectionFont = new Font(RichHelpContent.SelectionFont.Name, MathF.Max(RichHelpContent.SelectionFont.Size * 0.75f, SizeToApply), RichHelpContent.SelectionFont.Style);
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
        #endregion

        #region Help Preview
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

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern int LoadString(IntPtr hInstance, uint uID, StringBuilder lpBuffer, int nBufferMax);

        private void PreviewHelp_Click(object sender, EventArgs e)
        {
            if (radioButtonURL.Checked)
            {
                if (HelpURL.Text.IsWebAddress())
                {
                    UIUtil.LaunchUrl(this, HelpURL.Text);
                }
                else
                {
                    FlexibleMessageBox.Show("Specified URL should start with 'http://' or 'https://'\r\n\r\nFix your URL and try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else if (radioButtonPlain.Checked)
            {
                // This message box is not "Flexible" because we're trying to simulate what Paint.NET will be showing for a plain text help box.
                MessageBox.Show(HelpPlainText.Text, MenuName.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else if (radioButtonRich.Checked)
            {
                using Form form = new Form();
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

                Button btn_HelpBoxOKButton = new Button();
                btn_HelpBoxOKButton.AutoSize = true;
                btn_HelpBoxOKButton.Text = LoadLocalizedString("user32.dll", 800, "OK");
                btn_HelpBoxOKButton.DialogResult = DialogResult.Cancel;
                btn_HelpBoxOKButton.Size = new Size(84, 24);
                btn_HelpBoxOKButton.Location = new Point(472, 359);
                btn_HelpBoxOKButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

                RichTextBox rtb_HelpBox = new RichTextBox();
                rtb_HelpBox.Size = new Size(564, 350);
                rtb_HelpBox.Location = new Point(0, 0);
                rtb_HelpBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Right;
                rtb_HelpBox.DetectUrls = true;
                rtb_HelpBox.WordWrap = true;
                rtb_HelpBox.ScrollBars = RichTextBoxScrollBars.ForcedVertical;
                rtb_HelpBox.BorderStyle = BorderStyle.None;
                rtb_HelpBox.Font = new Font(rtb_HelpBox.SelectionFont.Name, 10f);
                rtb_HelpBox.ReadOnly = false;
                rtb_HelpBox.Rtf = RichHelpContent.Rtf;
                rtb_HelpBox.ReadOnly = true;
                rtb_HelpBox.LinkClicked += (obj, args) =>
                {
                    UIUtil.LaunchUrl(this, args.LinkText);
                    btn_HelpBoxOKButton.Focus();
                };

                form.Controls.AddRange(new Control[] { btn_HelpBoxOKButton, rtb_HelpBox });
                form.ResumeLayout();
                form.ShowDialog();
            }
        }
        #endregion

        #region Compression
        private static string CompressString(string text)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(text);
            using MemoryStream memoryStream = new MemoryStream();
            using (GZipStream gZipStream = new GZipStream(memoryStream, CompressionMode.Compress, true))
            {
                gZipStream.Write(buffer, 0, buffer.Length);
            }

            memoryStream.Position = 0;

            byte[] compressedData = new byte[memoryStream.Length];
            memoryStream.ReadExactly(compressedData, 0, compressedData.Length);

            byte[] gZipBuffer = new byte[compressedData.Length + 4];
            Buffer.BlockCopy(compressedData, 0, gZipBuffer, 4, compressedData.Length);
            Buffer.BlockCopy(BitConverter.GetBytes(buffer.Length), 0, gZipBuffer, 0, 4);
            return Convert.ToBase64String(gZipBuffer);
        }

        private static string DecompressString(string compressedText)
        {
            byte[] gZipBuffer = Convert.FromBase64String(compressedText);
            int dataLength = BitConverter.ToInt32(gZipBuffer, 0);

            using MemoryStream memoryStream = new MemoryStream();
            memoryStream.Write(gZipBuffer, 4, gZipBuffer.Length - 4);
            memoryStream.Position = 0;

            byte[] buffer = new byte[dataLength];

            using (GZipStream gZipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
            {
                gZipStream.ReadExactly(buffer, 0, buffer.Length);
            }

            return Encoding.UTF8.GetString(buffer);
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
            else if (radioButtonURL.Checked)
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
            else if (radioButtonPlain.Checked)
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
            else if (radioButtonRich.Checked)
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
            IDataObject obj = System.Windows.Forms.Clipboard.GetDataObject();
            System.Windows.Forms.Clipboard.Clear();

            System.Windows.Forms.Clipboard.SetImage(img);
            RichHelpContent.Paste();

            System.Windows.Forms.Clipboard.Clear();
            System.Windows.Forms.Clipboard.SetDataObject(obj);
        }

        private void DoColor()
        {
            ColorWindow colorDialog1 = new ColorWindow
            {
                Color = Color.FromArgb(255, RichHelpContent.SelectionColor),
                ShowAlpha = false
            };

            if (colorDialog1.ShowDialog() == DialogResult.OK && colorDialog1.Color != RichHelpContent.SelectionColor)
            {
                RichHelpContent.SelectionColor = colorDialog1.Color;
            }
        }

        private void DoBold()
        {
            RichHelpContent.SelectionFont = (RichHelpContent.SelectionFont.Bold) ?
                new Font(RichHelpContent.SelectionFont, RichHelpContent.SelectionFont.Style ^ FontStyle.Bold) :
                new Font(RichHelpContent.SelectionFont, RichHelpContent.SelectionFont.Style | FontStyle.Bold);
        }

        private void DoItalics()
        {
            RichHelpContent.SelectionFont = (RichHelpContent.SelectionFont.Italic) ?
                new Font(RichHelpContent.SelectionFont, RichHelpContent.SelectionFont.Style ^ FontStyle.Italic) :
                new Font(RichHelpContent.SelectionFont, RichHelpContent.SelectionFont.Style | FontStyle.Italic);
        }

        private void DoUnderline()
        {
            RichHelpContent.SelectionFont = (RichHelpContent.SelectionFont.Underline) ?
                new Font(RichHelpContent.SelectionFont, RichHelpContent.SelectionFont.Style ^ FontStyle.Underline) :
                new Font(RichHelpContent.SelectionFont, RichHelpContent.SelectionFont.Style | FontStyle.Underline);
        }

        private void DoOpen()
        {
            OpenFileDialog ofd = new OpenFileDialog
            {
                Title = "Open Help File",
                Filter = "Rich Text Format (*.RTF)|*.RTF|Compressed Rich Text Format (*.RTZ)|*.RTZ|Text Format with UBB Codes (*.TXT)|*.TXT",
                DefaultExt = ".rtf",
                Multiselect = false,
                InitialDirectory = Settings.LastSourceDirectory
            };

            if (ofd.ShowDialog() == DialogResult.OK && File.Exists(ofd.FileName))
            {
                try
                {
                    string fileExtension = Path.GetExtension(ofd.FileName);
                    if (fileExtension.Equals(".rtf", StringComparison.OrdinalIgnoreCase))
                    {
                        RichHelpContent.ResetText();
                        RichHelpContent.Rtf = File.ReadAllText(ofd.FileName);
                    }
                    else if (fileExtension.Equals(".rtz", StringComparison.OrdinalIgnoreCase))
                    {
                        string FileContents = File.ReadAllText(ofd.FileName);
                        string ExpandedContents = DecompressString(FileContents);
                        RichHelpContent.ResetText();
                        RichHelpContent.Rtf = ExpandedContents;
                    }
                    else if (fileExtension.Equals(".txt", StringComparison.OrdinalIgnoreCase))
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
            SaveFileDialog sfd = new SaveFileDialog
            {
                Title = "Save Help File",
                FileName = Path.ChangeExtension(FileName, ".rtf"),
                Filter = "Rich Text Format (*.RTF)|*.RTF",
                DefaultExt = ".rtf",
                InitialDirectory = Settings.LastSourceDirectory
            };

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    File.WriteAllText(sfd.FileName, RichHelpContent.Rtf);
                    if (OpenInWordPad)
                    {
                        WarningLabel.Visible = true;
                        Application.DoEvents();
                        ProcessUtil.Exec(sfd.FileName, string.Empty);
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
            RichHelpContent.SelectionCharOffset = (RichHelpContent.SelectionCharOffset == 0) ? 5 : 0;
        }

        private void DoSubscript()
        {
            RichHelpContent.SelectionCharOffset = (RichHelpContent.SelectionCharOffset == 0) ? -5 : 0;
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
            using OpenFileDialog ofd = new OpenFileDialog
            {
                Title = "Open Image File",
                Filter = "Image Files(*.PNG;*.BMP;*.JPG;*.GIF)|*.PNG;*.BMP;*.JPG;*.GIF|All files (*.*)|*.*",
                DefaultExt = ".png",
                Multiselect = false,
                InitialDirectory = Settings.LastSourceDirectory
            };

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                Bitmap aimg = UIUtil.GetBitmapFromFile(ofd.FileName);
                if (aimg != null)
                {
                    InsertImage(aimg);
                }
                else
                {
                    FlexibleMessageBox.Show("There was a problem opening the image.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

            string SourceCode = this.projectType switch
            {
                ProjectType.ClassicEffect => ClassicEffectWriter.FullSourceCode(FullScriptText, FileName, isAdjustment, SubMenuName.Text, MenuName.Text, IconPath, URL, RenderingFlags, RenderingSchedule, Author, MajorVer, MinorVer, Description, KeyWords, WindowTitle, HelpType, HelpStr),
                ProjectType.BitmapEffect => BitmapEffectWriter.FullSourceCode(FullScriptText, FileName, isAdjustment, SubMenuName.Text, MenuName.Text, IconPath, URL, RenderingFlags, RenderingSchedule, Author, MajorVer, MinorVer, Description, KeyWords, WindowTitle, HelpType, HelpStr),
                ProjectType.GpuImageEffect => GPUEffectWriter.FullSourceCode(FullScriptText, FileName, isAdjustment, SubMenuName.Text, MenuName.Text, IconPath, URL, RenderingFlags, RenderingSchedule, Author, MajorVer, MinorVer, Description, KeyWords, WindowTitle, HelpType, HelpStr),
                ProjectType.GpuDrawEffect => GPUDrawWriter.FullSourceCode(FullScriptText, FileName, isAdjustment, SubMenuName.Text, MenuName.Text, IconPath, URL, RenderingFlags, RenderingSchedule, Author, MajorVer, MinorVer, Description, KeyWords, WindowTitle, HelpType, HelpStr),
                _ => throw new NotImplementedException("Invalid Project Type"),
            };

            using ViewSrc VSW = new ViewSrc("Full Source Code", SourceCode, true);
            VSW.ShowDialog();
        }

        private void GenSlnButton_Click(object sender, EventArgs e)
        {
            if (!this.canCreateSln)
            {
                FlexibleMessageBox.Show("Due to technical reasons, this feature is only available on classic installations of Paint.NET.", "Generate Visual Studio Solution", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (!UpdateAllValues())
            {
                return;
            }

            if (radioButtonRich.Checked)
            {
                string CompressedOutput = CompressString(RichHelpContent.Rtf);
                File.WriteAllText(RTZPath, CompressedOutput);
            }

            using FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.SelectedPath = Settings.LastSlnDirectory;
            fbd.ShowNewFolderButton = true;
            fbd.Description = "Choose a Folder to place the generated Visual Studio Solution.";

            if (fbd.ShowDialog() == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
            {
                string SourceCode = this.projectType switch
                {
                    ProjectType.ClassicEffect => ClassicEffectWriter.FullSourceCode(FullScriptText, FileName, isAdjustment, SubMenuName.Text, MenuName.Text, IconPath, URL, RenderingFlags, RenderingSchedule, Author, MajorVer, MinorVer, Description, KeyWords, WindowTitle, HelpType, HelpStr),
                    ProjectType.BitmapEffect => BitmapEffectWriter.FullSourceCode(FullScriptText, FileName, isAdjustment, SubMenuName.Text, MenuName.Text, IconPath, URL, RenderingFlags, RenderingSchedule, Author, MajorVer, MinorVer, Description, KeyWords, WindowTitle, HelpType, HelpStr),
                    ProjectType.GpuImageEffect => GPUEffectWriter.FullSourceCode(FullScriptText, FileName, isAdjustment, SubMenuName.Text, MenuName.Text, IconPath, URL, RenderingFlags, RenderingSchedule, Author, MajorVer, MinorVer, Description, KeyWords, WindowTitle, HelpType, HelpStr),
                    ProjectType.GpuDrawEffect => GPUDrawWriter.FullSourceCode(FullScriptText, FileName, isAdjustment, SubMenuName.Text, MenuName.Text, IconPath, URL, RenderingFlags, RenderingSchedule, Author, MajorVer, MinorVer, Description, KeyWords, WindowTitle, HelpType, HelpStr),
                    _ => throw new NotImplementedException("Invalid Project Type"),
                };

                string slnFilePath = Solution.Generate(fbd.SelectedPath, FileName, SourceCode, IconPath, resourcePath);

                if (slnFilePath != null)
                {
                    bool success = File.Exists(slnFilePath) && UIUtil.LaunchFolderAndSelectFile(this, slnFilePath);
                    if (!success)
                    {
                        FlexibleMessageBox.Show("Could not navigate to the generated Solution file.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }

                Settings.LastSlnDirectory = fbd.SelectedPath;
            }
        }

        private void ForceROI_CheckedChanged(object sender, EventArgs e)
        {
            if (sender == forceLegacyRoiBox)
            {
                if (forceLegacyRoiBox.Checked && forceSingleRenderBox.Checked)
                {
                    forceSingleRenderBox.Checked = false;
                }
            }
            else if (sender == forceSingleRenderBox)
            {
                if (forceSingleRenderBox.Checked && forceLegacyRoiBox.Checked)
                {
                    forceLegacyRoiBox.Checked = false;
                }
            }
        }

        void IToolTipHost.ThemeToolTip()
        {
            toolTip1.UpdateTheme();
        }
    }
}
