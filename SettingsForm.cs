using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PaintDotNet.Effects
{
    public partial class SettingsForm : ChildFormBase
    {
        #region Initialize
        bool Initializing;
        public SettingsForm()
        {
            Initializing = true;
            InitializeComponent();
            this.Icon = ResUtil.CreateIcon("Settings");
            // PDN Theme
            foreach (Control control in this.Controls)
            {
                if ((control is ComboBox) || (control is ListBox))
                {
                    control.ForeColor = this.ForeColor;
                    control.BackColor = this.BackColor;
                }
            }
            linkLabel1.LinkColor = this.ForeColor;
            settingsList.SelectedIndex = 0;
            toolbarCheckbox.Checked = Settings.ToolBar;
            checkForUpdates.Checked = Settings.CheckForUpdates;
            lineNumbersCheckbox.Checked = Settings.LineNumbers;
            bookMarksCheckbox.Checked = Settings.Bookmarks;
            codeFoldingCheckbox.Checked = Settings.CodeFolding;
            indicatorMapCheckbox.Checked = Settings.Map;
            wordWrapCheckbox.Checked = Settings.WordWrap;
            wordWrapTextFilesCheckbox.Checked = Settings.WordWrapPlainText;
            showWhiteSpaceCheckbox.Checked = Settings.WhiteSpace;
            largeFontCheckbox.Checked = Settings.LargeFonts;
            for(int i=0; i<fontCombobox.Items.Count; i++)
            {
                if (!IsFontInstalled(fontCombobox.Items[i].ToString()))
                {
                    fontCombobox.Items[i] = fontCombobox.Items[i].ToString() + "*";
                }
            }
            fontCombobox.SelectedIndex = fontCombobox.FindString(Settings.FontFamily);
            themeCombobox.Text = Settings.EditorTheme.ToString();
            warningLevelCombobox.SelectedIndex = Settings.WarningLevel;
            Initializing = false;
        }

        private void settingsList_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index == -1)
            {
                return;
            }
            float UIfactor = e.Graphics.DpiY / 96;
            e.DrawBackground();
            string item = settingsList.Items[e.Index].ToString();
            string graphicName = "UI";
            if (item == "Updates") { graphicName = "Updates"; }
            if (item == "Compiler") { graphicName = "Compiler"; }
            Image iconImage = ResUtil.GetImage(graphicName);
            e.Graphics.DrawImage(iconImage, new Rectangle(e.Bounds.X + 1, e.Bounds.Y + 1, e.Bounds.Height - 2, e.Bounds.Height - 2));
            using (SolidBrush solidBrush = new SolidBrush(e.ForeColor))
            using (SolidBrush foreBrush = new SolidBrush(e.ForeColor))
            using (SolidBrush backBrush = new SolidBrush(Color.Gray))
            using (Font bigfont = new Font(e.Font.FontFamily, e.Font.Size, FontStyle.Bold))
            {
                e.Graphics.DrawString(item, bigfont, foreBrush, new Rectangle(e.Bounds.X + e.Bounds.Height, e.Bounds.Y + (int)(7 * UIfactor), e.Bounds.Width - e.Bounds.Height, e.Bounds.Height));
            }
            e.DrawFocusRectangle();
        }

        private void settingsList_SelectedIndexChanged(object sender, EventArgs e)
        {
            // If you're trying to figure out why your new panel isn't showing, it's because you've dropped your panel into another panel.
            // All panels should be in the form, not inside of other panels. You'll probably have to go into ...Designer.cs to fix that
            // as the WYSIWYG editor makes this difficult.
            string item = settingsList.SelectedItem.ToString();
            if (item == "User Interface")
            {
                updatesPanel.Visible = false;
                uiPanel.Visible = true;
                compilerPanel.Visible = false;
            }
            else if (item == "Updates")
            {
                updatesPanel.Visible = true;
                uiPanel.Visible = false;
                compilerPanel.Visible = false;
            }
            else if (item == "Compiler")
            {
                updatesPanel.Visible = false;
                uiPanel.Visible = false;
                compilerPanel.Visible = true;
            }
        }
        #endregion

        #region Updates Page
        private void checkForUpdates_CheckedChanged(object sender, EventArgs e)
        {
            if (Initializing) return;
            Settings.CheckForUpdates = checkForUpdates.Checked;
        }

        private void checkNowButton_Click(object sender, EventArgs e)
        {
            Freshness.GoCheckForUpdates(false, true);
        }
        #endregion

        #region UI Page
        private void toolbarCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (Initializing) return;
            Settings.ToolBar = toolbarCheckbox.Checked;
        }
        private void lineNumbersCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (Initializing) return;
            Settings.LineNumbers = lineNumbersCheckbox.Checked;
        }

        private void bookMarksCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (Initializing) return;
            Settings.Bookmarks = bookMarksCheckbox.Checked;
        }

        private void codeFoldingCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (Initializing) return;
            Settings.CodeFolding = codeFoldingCheckbox.Checked;
        }

        private void indicatorMapCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (Initializing) return;
            Settings.Map = indicatorMapCheckbox.Checked;
        }

        private void wordWrapCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (Initializing) return;
            Settings.WordWrap = wordWrapCheckbox.Checked;
        }

        private void showWhiteSpaceCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (Initializing) return;
            Settings.WhiteSpace = showWhiteSpaceCheckbox.Checked;
        }

        private void fontCombobox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Initializing) return;
            string SelectedFont = fontCombobox.Text;
            if (SelectedFont.EndsWith("*"))
            {
                SelectedFont = SelectedFont.Substring(0, SelectedFont.Length - 1);
                FlexibleMessageBox.Show(SelectedFont + " is not installed on your system.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            Settings.FontFamily = SelectedFont;
        }

        private void largeFontCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (Initializing) return;
            Settings.LargeFonts = largeFontCheckbox.Checked;
        }

        private void themeCombobox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Initializing) return;
            if (themeCombobox.Text == "Auto") Settings.EditorTheme = Theme.Auto;
            if (themeCombobox.Text == "Dark") Settings.EditorTheme = Theme.Dark;
            if (themeCombobox.Text == "Light") Settings.EditorTheme = Theme.Light;
        }

        private void wordWrapTextFilesCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (Initializing) return;
            Settings.WordWrapPlainText = wordWrapTextFilesCheckbox.Checked;
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            FlexibleMessageBox.Show(
                "You may choose between Consolas, Courier New, and Verdana fonts to view your code.\n\n" +
                "These fonts are built-in to Windows. If they are not available, you may need to download them.\n\n" +
                "Or, you may download and install these popular programming fonts from the following web sites:\n\n" +
                "▪ Envy Code R\n    http://damieng.com/blog/2008/05/26/envy-code-r-preview-7-coding-font-released \n\n" +
                "▪ Hack\n    http://sourcefoundry.org/hack/ \n\n" +
                "The following fonts support Programming Ligatures: (Showing  ≤  instead of  <=  etc.)\n\n" +
                "▪ Cascadia Code\n    https://devblogs.microsoft.com/commandline/cascadia-code/ \n\n" +
                "▪ Fira Code\n    https://github.com/tonsky/FiraCode \n\n" +
                "▪ JetBrains Mono\n    https://www.jetbrains.com/lp/mono/ \n\n" +
                "Once downloaded and installed on your system, you may choose these fonts from the menu.",
                "Help With Fonts", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        #endregion

        #region Compiler Page
        private void warningLevelCombobox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Initializing) return;
            Settings.WarningLevel = warningLevelCombobox.SelectedIndex;
        }
        #endregion

        private static bool IsFontInstalled(string fontName)
        {
            using (Font font = new Font(fontName, 12f))
            {
                return font.Name == fontName;
            }
        }
    }
}
