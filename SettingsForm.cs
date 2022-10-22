using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace PaintDotNet.Effects
{
    public partial class SettingsForm : ChildFormBase
    {
        #region Initialize
        private readonly bool Initializing;
        private readonly IReadOnlyList<Image> pageIcons;

        public SettingsForm()
        {
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

            pageIcons = new Image[]
            {
                UIUtil.GetImage("UI"),
                UIUtil.GetImage("Snippet"),
                UIUtil.GetImage("Spelling"),
                UIUtil.GetImage("Compiler"),
                UIUtil.GetImage("Updates")
            };

            settingsList.ItemHeight = UIUtil.Scale(32);
            linkLabel1.LinkColor = this.ForeColor;
            settingsList.SelectedIndex = 0;

            Initializing = true;

            // User Interface page
            toolbarCheckbox.Checked = Settings.ToolBar;
            lineNumbersCheckbox.Checked = Settings.LineNumbers;
            bookMarksCheckbox.Checked = Settings.Bookmarks;
            codeFoldingCheckbox.Checked = Settings.CodeFolding;
            indicatorMapCheckbox.Checked = Settings.Map;
            wordWrapCheckbox.Checked = Settings.WordWrap;
            wordWrapTextFilesCheckbox.Checked = Settings.WordWrapPlainText;
            caretLineFrameCheckBox.Checked = Settings.CaretLineFrame;
            disableAutoCompCheckBox.Checked = Settings.DisableAutoComplete;
            showWhiteSpaceCheckbox.Checked = Settings.WhiteSpace;
            indentSpacesComboBox.SelectedIndex = Settings.IndentSpaces == 4 ? 1 : 0;
            largeFontCheckbox.Checked = Settings.LargeFonts;
            for (int i = 0; i < fontCombobox.Items.Count; i++)
            {
                if (!UIUtil.IsFontInstalled(fontCombobox.Items[i].ToString()))
                {
                    fontCombobox.Items[i] = fontCombobox.Items[i].ToString() + "*";
                }
            }
            fontCombobox.SelectedIndex = fontCombobox.FindString(Settings.FontFamily);
            themeCombobox.Text = Settings.EditorTheme.ToString();
            extendedColorsCheckBox.Checked = Settings.ExtendedColors;

            // Spellcheck page
            enableSpellcheckCheckBox.Checked = Settings.Spellcheck;
            spellLangComboBox.Items.AddRange(PlatformSpellCheck.SpellChecker.SupportedLanguages.ToArray());
            int langIndex = spellLangComboBox.FindString(Settings.SpellingLang);
            spellLangComboBox.SelectedIndex = langIndex > -1 ? langIndex : 0;
            wordsToIgnoreListBox.Items.AddRange(Settings.SpellingWordsToIgnore
                .Distinct()
                .OrderBy(s => s, StringComparer.OrdinalIgnoreCase)
                .ToArray());

            // Compiler page
            warningLevelCombobox.SelectedIndex = Settings.WarningLevel;
            warningsToIgnoreList.Items.AddRange(Settings.WarningsToIgnore
                .Distinct()
                .OrderBy(s => s, StringComparer.OrdinalIgnoreCase)
                .ToArray());

            if (warningsToIgnoreList.Items.Count > 0)
            {
                warningsToIgnoreList.SelectedIndex = 0;
            }

            // Updates page
            checkForUpdates.Checked = Settings.CheckForUpdates;

            Initializing = false;
        }

        private void settingsList_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index == -1)
            {
                return;
            }

            e.DrawBackground();

            Rectangle iconRect = new Rectangle(e.Bounds.X + UIUtil.Scale(4), e.Bounds.Y + UIUtil.Scale(4), UIUtil.Scale(24), UIUtil.Scale(24));
            e.Graphics.DrawImage(pageIcons[e.Index], iconRect);

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
                panelUpdates.Visible = false;
                panelSnippet.Visible = false;
                panelUI.Visible = true;
                panelSpelling.Visible = false;
                panelCompiler.Visible = false;
            }
            else if (item == "Snippets")
            {
                panelUpdates.Visible = false;
                panelSnippet.Visible = true;
                panelUI.Visible = false;
                panelSpelling.Visible = false;
                panelCompiler.Visible = false;
            }
            else if (item == "Updates")
            {
                panelUpdates.Visible = true;
                panelSnippet.Visible = false;
                panelUI.Visible = false;
                panelSpelling.Visible = false;
                panelCompiler.Visible = false;
            }
            else if (item == "Compiler")
            {
                panelUpdates.Visible = false;
                panelSnippet.Visible = false;
                panelUI.Visible = false;
                panelSpelling.Visible = false;
                panelCompiler.Visible = true;
            }
            else if (item == "Spellcheck")
            {
                panelUpdates.Visible = false;
                panelSnippet.Visible = false;
                panelUI.Visible = false;
                panelSpelling.Visible = true;
                panelCompiler.Visible = false;
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
            if (SelectedFont.EndsWith("*", StringComparison.Ordinal))
            {
                SelectedFont = SelectedFont[0..^1];
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

        private void extendedColorsCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (Initializing) return;
            Settings.ExtendedColors = extendedColorsCheckBox.Checked;
        }

        private void wordWrapTextFilesCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (Initializing) return;
            Settings.WordWrapPlainText = wordWrapTextFilesCheckbox.Checked;
        }

        private void caretLineFrameCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (Initializing) return;
            Settings.CaretLineFrame = caretLineFrameCheckBox.Checked;
        }

        private void disableAutoCompCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (Initializing) return;
            Settings.DisableAutoComplete = disableAutoCompCheckBox.Checked;
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

        #region Spellcheck Page
        private void addLangsButton_Click(object sender, EventArgs e)
        {
            UIUtil.LaunchUrl(this, "ms-settings:regionlanguage");
        }

        private void enableSpellcheckCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (Initializing) return;
            Settings.Spellcheck = enableSpellcheckCheckBox.Checked;
        }

        private void spellLangComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Initializing) return;
            Settings.SpellingLang = spellLangComboBox.Text;
        }

        private void removeIgnoreWordButton_Click(object sender, EventArgs e)
        {
            if (wordsToIgnoreListBox.SelectedIndex > -1)
            {
                wordsToIgnoreListBox.Items.RemoveAt(wordsToIgnoreListBox.SelectedIndex);
                Settings.SpellingWordsToIgnore = wordsToIgnoreListBox.Items.OfType<string>();
            }
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
                string url = $"https://msdn.microsoft.com/query/roslyn.query?appId=roslyn&k=k({warningsToIgnoreList.SelectedItem})";
                UIUtil.LaunchUrl(this, url);
            }
        }
        #endregion
    }
}
