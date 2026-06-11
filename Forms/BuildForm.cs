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
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

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
                    MajorVersion.Value = decimal.Clamp(majorVer, MajorVersion.Minimum, MajorVersion.Maximum);
                }
                if (decimal.TryParse(vsn.Groups["minorVersionLabel"].Value.Trim(), out decimal minorVer))
                {
                    MinorVersion.Value = decimal.Clamp(minorVer, MinorVersion.Minimum, MinorVersion.Maximum);
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

                if (HelpPlainText.TextLength == 0)
                {
                    HelpPlainText.Text = $"{MenuName.Text} v{MajorVersion.Value}{DecimalSymbol.Text}{MinorVersion.Value}\r\nCopyright ©{DateTime.Now.Year} by {AuthorName.Text}\r\nAll rights reserved.";
                    if (radioButtonNone.Checked)
                    {
                        radioButtonPlain.Checked = true;
                    }
                }

                // See if a help file exists
                string[] possibleHelpPaths =
                {
                    Path.ChangeExtension(resourcePath, ".rtf"),
                    Path.ChangeExtension(resourcePath, ".rtz"),
                    Path.ChangeExtension(resourcePath, ".txt")
                };

                string helpPath = possibleHelpPaths.FirstOrDefault(path => File.Exists(path));
                bool rtfLoaded = RichHelpContent.LoadFileFromPath(helpPath);
                radioButtonRich.Checked = rtfLoaded;

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
                    string CompressedOutput = RichHelpContent.RtfCompressed;
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

        private void radioHelpType_CheckedChanged(object sender, EventArgs e)
        {
            UpdateReadOnlyFields();
        }

        private void OpenRtf()
        {
            using OpenFileDialog ofd = new OpenFileDialog
            {
                Title = "Open Help File",
                Filter = "Rich Text Format (*.RTF)|*.RTF|Compressed Rich Text Format (*.RTZ)|*.RTZ|Text Format with UBB Codes (*.TXT)|*.TXT",
                DefaultExt = ".rtf",
                Multiselect = false,
                InitialDirectory = Settings.LastSourceDirectory
            };

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                RichHelpContent.LoadFileFromPath(ofd.FileName);
            }

            RichHelpContent.Focus();
        }

        private void SaveRtf(bool OpenInWordPad)
        {
            using SaveFileDialog sfd = new SaveFileDialog
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
        #endregion

        #region RTF Editor Toolbar Buttons
        private void BoldButton_Click(object sender, EventArgs e)
        {
            RichHelpContent.ToggleBold();
        }

        private void ItalicsButton_Click(object sender, EventArgs e)
        {
            RichHelpContent.ToggleItalics();
        }

        private void UnderlineButton_Click(object sender, EventArgs e)
        {
            RichHelpContent.ToggleUnderline();
        }

        private void OpenButton_Click(object sender, EventArgs e)
        {
            OpenRtf();
        }

        private void SaveButton_Click_1(object sender, EventArgs e)
        {
            SaveRtf(false);
        }

        private void WordPadButton_Click(object sender, EventArgs e)
        {
            SaveRtf(true);
        }

        private void SuperScriptButton_Click(object sender, EventArgs e)
        {
            RichHelpContent.ToggleSuperscript();
        }

        private void SubScriptButton_Click(object sender, EventArgs e)
        {
            RichHelpContent.ToggleSubscript();
        }

        private void LargeFontButton_Click(object sender, EventArgs e)
        {
            RichHelpContent.IncreaseFontSize();
        }

        private void SmallFontButton_Click(object sender, EventArgs e)
        {
            RichHelpContent.DecreaseFontSize();
        }

        private void BulletButton_Click(object sender, EventArgs e)
        {
            RichHelpContent.ToggleBullet();
        }

        private void IndentButton_Click(object sender, EventArgs e)
        {
            RichHelpContent.Indent();
        }

        private void UnindentButton_Click(object sender, EventArgs e)
        {
            RichHelpContent.Unindent();
        }

        private void ParagraphLeftButton_Click(object sender, EventArgs e)
        {
            RichHelpContent.AlginLeft();
        }

        private void CenterButton_Click(object sender, EventArgs e)
        {
            RichHelpContent.AlignCenter();
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
                    RichHelpContent.InsertImage(aimg);
                }
                else
                {
                    FlexibleMessageBox.Show("There was a problem opening the image.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void ColorButton_Click(object sender, EventArgs e)
        {
            RichHelpContent.ApplyColor();
        }
        #endregion

        #region RTF Editor Keys
        private void RichHelpContent_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Modifiers == Keys.Control && e.KeyCode == Keys.B)
            {
                RichHelpContent.ToggleBold();
                e.Handled = true;
            }
            if (e.Modifiers == Keys.Control && e.KeyCode == Keys.I)
            {
                RichHelpContent.ToggleItalics();
                e.Handled = true;
            }
            if (e.Modifiers == Keys.Control && e.KeyCode == Keys.U)
            {
                RichHelpContent.ToggleUnderline();
                e.Handled = true;
            }
            if (e.Modifiers == Keys.Control && e.KeyCode == Keys.O)
            {
                OpenRtf();
                e.Handled = true;
            }
            if (e.Modifiers == Keys.Control && e.KeyCode == Keys.S)
            {
                SaveRtf(false);
                e.Handled = true;
            }
            if (e.Modifiers == Keys.Control && e.KeyCode == Keys.W)
            {
                SaveRtf(true);
                e.Handled = true;
            }
            if ((e.KeyCode == Keys.F8) || (e.Modifiers == Keys.Alt && e.KeyCode == Keys.C))
            {
                RichHelpContent.ApplyColor();
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
                string CompressedOutput = RichHelpContent.RtfCompressed;
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
