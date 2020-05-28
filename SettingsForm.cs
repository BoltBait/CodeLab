using System;
using System.Drawing;
using System.Linq;
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
            // PDN Theme
            foreach (Control control in this.Controls.OfType<Control>()
                .Concat(this.Controls.OfType<Panel>().SelectMany(panel => panel.Controls.OfType<Control>()))
                )
            {
                if ((control is ComboBox) || (control is ListBox))
                {
                    control.ForeColor = this.ForeColor;
                    control.BackColor = this.BackColor;
                }
            }

            imageList.ImageSize = UIUtil.ScaleSize(24, 24);
            imageList.Images.AddRange(new Image[]
            {
                UIUtil.GetImage("UI"),
                UIUtil.GetImage("Snippet"),
                UIUtil.GetImage("Compiler"),
                UIUtil.GetImage("Updates")
            });

            settingsList.ItemHeight = UIUtil.Scale(32);
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
            indentSpacesComboBox.SelectedIndex = Settings.IndentSpaces == 4 ? 1 : 0;
            largeFontCheckbox.Checked = Settings.LargeFonts;
            for(int i=0; i<fontCombobox.Items.Count; i++)
            {
                if (!UIUtil.IsFontInstalled(fontCombobox.Items[i].ToString()))
                {
                    fontCombobox.Items[i] = fontCombobox.Items[i].ToString() + "*";
                }
            }
            fontCombobox.SelectedIndex = fontCombobox.FindString(Settings.FontFamily);
            themeCombobox.Text = Settings.EditorTheme.ToString();
            warningLevelCombobox.SelectedIndex = Settings.WarningLevel;
            warningsToIgnoreList.Items.AddRange(Settings.WarningsToIgnore.ToArray());
            if (warningsToIgnoreList.Items.Count > 0)
            {
                warningsToIgnoreList.SelectedIndex = 0;
            }
            Initializing = false;
        }

        private void settingsList_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index == -1)
            {
                return;
            }

            e.DrawBackground(); 

            Point iconLocation = new Point(e.Bounds.X + UIUtil.Scale(4), e.Bounds.Y + UIUtil.Scale(4));
            imageList.Draw(e.Graphics, iconLocation, e.Index);

            string itemName = settingsList.Items[e.Index].ToString();
            Rectangle textBounds = new Rectangle(e.Bounds.X + e.Bounds.Height, e.Bounds.Y, e.Bounds.Width - e.Bounds.Height, e.Bounds.Height);
            TextRenderer.DrawText(e.Graphics, itemName, e.Font, textBounds, e.ForeColor, TextFormatFlags.VerticalCenter);

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
                snippetPanel.Visible = false;
                uiPanel.Visible = true;
                compilerPanel.Visible = false;
            }
            else if (item == "Snippets")
            {
                updatesPanel.Visible = false;
                snippetPanel.Visible = true;
                uiPanel.Visible = false;
                compilerPanel.Visible = false;
            }
            else if (item == "Updates")
            {
                updatesPanel.Visible = true;
                snippetPanel.Visible = false;
                uiPanel.Visible = false;
                compilerPanel.Visible = false;
            }
            else if (item == "Compiler")
            {
                updatesPanel.Visible = false;
                snippetPanel.Visible = false;
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

        private void indentSpacesComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Initializing) return;
            Settings.IndentSpaces = indentSpacesComboBox.SelectedIndex == 0 ? 2 : 4;
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

        private void removeWarningButton_Click(object sender, EventArgs e)
        {
            if (warningsToIgnoreList.SelectedIndex > -1)
            {
                warningsToIgnoreList.Items.RemoveAt(warningsToIgnoreList.SelectedIndex);
                Settings.WarningsToIgnore = warningsToIgnoreList.Items.OfType<string>();
            }
        }

        private void lookupWarningButton_Click(object sender, EventArgs e)
        {
            if (warningsToIgnoreList.SelectedIndex > -1)
            {
                string url = "https://docs.microsoft.com/dotnet/csharp/misc/" + warningsToIgnoreList.SelectedItem.ToString();
                System.Diagnostics.Process.Start(url);
            }
        }
        #endregion
    }
}
