using PaintDotNet.Effects;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace PdnCodeLab
{
    internal partial class SettingsForm : ChildFormBase
    {
        #region Initialize
        private readonly bool Initializing;

        internal SettingsForm(SettingsPage defaultPage = SettingsPage.UI)
        {
            InitializeComponent();

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
            showWhiteSpaceCheckbox.Checked = Settings.WhiteSpace;
            indentSpacesComboBox.SelectedIndex = Settings.IndentSpaces == 4 ? 1 : 0;
            largeFontCheckbox.Checked = Settings.LargeFonts;
            fontCombobox.Items.AddRange(FontUtil.FontList.ToArray());
            fontCombobox.SelectedIndex = fontCombobox.FindStringExact(Settings.FontFamily);
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

            // RenderOptions page
            presetComboBox.Items.AddRange(PresentBoxItem.Items);
            presetComboBox.SelectedIndex = (int)Settings.RenderPreset;
            optionsTabControl.BackColor = Color.White;
            optionsTabControl.ForeColor = Color.Black;

            // Assistance Page
            disableAutoCompCheckBox.Checked = Settings.DisableAutoComplete;
            DocCommentOptions options = Settings.DocCommentOptions;
            dcEnabledCheckBox.Checked = options.HasFlag(DocCommentOptions.Enabled);
            dcToolTipsCheckBox.Checked = options.HasFlag(DocCommentOptions.ToolTips);
            dcDefsCheckBox.Checked = options.HasFlag(DocCommentOptions.Definitions);
            dcBclCheckBox.Checked = options.HasFlag(DocCommentOptions.BCL);

            settingsList.SelectedPage = defaultPage;

            Initializing = false;
        }

        private void settingsList_SelectedIndexChanged(object sender, EventArgs e)
        {
            // If you're trying to figure out why your new panel isn't showing, it's because you've dropped your panel into another panel.
            // All panels should be in the form, not inside of other panels. You'll probably have to go into ...Designer.cs to fix that
            // as the WYSIWYG editor makes this difficult.
            SettingsPage selectedPage = settingsList.SelectedPage;
            panelUpdates.Visible = selectedPage == SettingsPage.Updates;
            panelSnippet.Visible = selectedPage == SettingsPage.Snippet;
            panelUI.Visible = selectedPage == SettingsPage.UI;
            panelSpelling.Visible = selectedPage == SettingsPage.Spelling;
            panelCompiler.Visible = selectedPage == SettingsPage.Compiler;
            panelRenderOptions.Visible = selectedPage == SettingsPage.RenderOptions;
            panelAssist.Visible = selectedPage == SettingsPage.Assistance;
        }
        #endregion

        #region Updates Page
        private void checkForUpdates_CheckedChanged(object sender, EventArgs e)
        {
            if (Initializing) { return; }
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
            if (Initializing) { return; }
            Settings.ToolBar = toolbarCheckbox.Checked;
        }
        private void lineNumbersCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (Initializing) { return; }
            Settings.LineNumbers = lineNumbersCheckbox.Checked;
        }

        private void bookMarksCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (Initializing) { return; }
            Settings.Bookmarks = bookMarksCheckbox.Checked;
        }

        private void codeFoldingCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (Initializing) { return; }
            Settings.CodeFolding = codeFoldingCheckbox.Checked;
        }

        private void indicatorMapCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (Initializing) { return; }
            Settings.Map = indicatorMapCheckbox.Checked;
        }

        private void wordWrapCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (Initializing) { return; }
            Settings.WordWrap = wordWrapCheckbox.Checked;
        }

        private void showWhiteSpaceCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (Initializing) { return; }
            Settings.WhiteSpace = showWhiteSpaceCheckbox.Checked;
        }

        private void indentSpacesComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Initializing) { return; }
            Settings.IndentSpaces = indentSpacesComboBox.SelectedIndex == 0 ? 2 : 4;
        }

        private void fontCombobox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Initializing) { return; }
            string SelectedFont = fontCombobox.Text;
            if (SelectedFont.EndsWith('*'))
            {
                string previousFont = Settings.FontFamily;
                fontCombobox.SelectedIndex = fontCombobox.FindStringExact(previousFont);

                string notInstalled = $"{SelectedFont[..^1]} is not installed on your system.\r\n\r\nSwitching back to {previousFont}.";
                FlexibleMessageBox.Show(notInstalled, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                Settings.FontFamily = SelectedFont;
            }
        }

        private void fontCombobox_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index == -1)
            {
                return;
            }

            e.DrawBackground();

            string fontName = fontCombobox.Items[e.Index].ToString();
            bool installed = !fontName.EndsWith('*');

            if (installed)
            {
                int notEqualCharWidth = e.Bounds.Height + 2;
                using Font font = new Font(fontName, e.Font.SizeInPoints);

                Rectangle fontNameRect = Rectangle.FromLTRB(e.Bounds.Left, e.Bounds.Top, e.Bounds.Right - notEqualCharWidth, e.Bounds.Bottom);
                TextRenderer.DrawText(e.Graphics, fontName, font, fontNameRect, e.ForeColor, TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);

                Rectangle notEqualRect = Rectangle.FromLTRB(e.Bounds.Right - notEqualCharWidth, e.Bounds.Top, e.Bounds.Right, e.Bounds.Bottom);
                TextRenderer.DrawText(e.Graphics, "!=", font, notEqualRect, e.ForeColor, TextFormatFlags.VerticalCenter | TextFormatFlags.Right);
            }
            else
            {
                TextRenderer.DrawText(e.Graphics, fontName, e.Font, e.Bounds, e.ForeColor, TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
            }

            e.DrawFocusRectangle();
        }

        private void largeFontCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (Initializing) { return; }
            Settings.LargeFonts = largeFontCheckbox.Checked;
        }

        private void themeCombobox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Initializing) { return; }
            if (themeCombobox.Text == "Auto") { Settings.EditorTheme = Theme.Auto; }
            if (themeCombobox.Text == "Dark") { Settings.EditorTheme = Theme.Dark; }
            if (themeCombobox.Text == "Light") { Settings.EditorTheme = Theme.Light; }
        }

        private void extendedColorsCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (Initializing) { return; }
            Settings.ExtendedColors = extendedColorsCheckBox.Checked;
        }

        private void wordWrapTextFilesCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (Initializing) { return; }
            Settings.WordWrapPlainText = wordWrapTextFilesCheckbox.Checked;
        }

        private void caretLineFrameCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (Initializing) { return; }
            Settings.CaretLineFrame = caretLineFrameCheckBox.Checked;
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
            if (Initializing) { return; }
            Settings.Spellcheck = enableSpellcheckCheckBox.Checked;
        }

        private void spellLangComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Initializing) { return; }
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
            if (Initializing) { return; }
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
                string url = Error.GetErrorUrl(warningsToIgnoreList.Text);
                UIUtil.LaunchUrl(this, url);
            }
        }
        #endregion

        #region RenderOptions Page
        private void presetComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (presetComboBox.SelectedItem is PresentBoxItem item)
            {
                RenderPreset selectedPreset = item.Preset;
                optionsTabControl.Enabled = selectedPreset == RenderPreset.UserDefined;

                switch (selectedPreset)
                {
                    case RenderPreset.Regular:
                        noneRadioButton.Checked = false;
                        horizontalStripsRadioButton.Checked = false;
                        squareTilesRadioButton.Checked = true;
                        singleThreadedCheckBox.Checked = false;
                        noClipCheckBox.Checked = false;
                        aliasedSelectionCheckBox.Checked = false;
                        break;
                    case RenderPreset.LegacyROI:
                        noneRadioButton.Checked = false;
                        horizontalStripsRadioButton.Checked = true;
                        squareTilesRadioButton.Checked = false;
                        singleThreadedCheckBox.Checked = false;
                        noClipCheckBox.Checked = false;
                        aliasedSelectionCheckBox.Checked = false;
                        break;
                    case RenderPreset.AliasedSelection:
                        noneRadioButton.Checked = false;
                        horizontalStripsRadioButton.Checked = false;
                        squareTilesRadioButton.Checked = true;
                        singleThreadedCheckBox.Checked = false;
                        noClipCheckBox.Checked = false;
                        aliasedSelectionCheckBox.Checked = true;
                        break;
                    case RenderPreset.SingleRenderCall:
                        noneRadioButton.Checked = true;
                        horizontalStripsRadioButton.Checked = false;
                        squareTilesRadioButton.Checked = false;
                        singleThreadedCheckBox.Checked = true;
                        noClipCheckBox.Checked = false;
                        aliasedSelectionCheckBox.Checked = false;
                        break;
                    case RenderPreset.NoSelectionClip:
                        noneRadioButton.Checked = false;
                        horizontalStripsRadioButton.Checked = false;
                        squareTilesRadioButton.Checked = true;
                        singleThreadedCheckBox.Checked = false;
                        noClipCheckBox.Checked = true;
                        aliasedSelectionCheckBox.Checked = false;
                        break;
                    case RenderPreset.UserDefined:
                        BitmapEffectRenderingFlags renderFlags = Settings.RenderingFlags;
                        BitmapEffectRenderingSchedule schedule = Settings.RenderingSchedule;

                        noneRadioButton.Checked = schedule == BitmapEffectRenderingSchedule.None;
                        horizontalStripsRadioButton.Checked = schedule == BitmapEffectRenderingSchedule.HorizontalStrips;
                        squareTilesRadioButton.Checked = schedule == BitmapEffectRenderingSchedule.SquareTiles;
                        singleThreadedCheckBox.Checked = renderFlags.HasFlag(BitmapEffectRenderingFlags.SingleThreaded);
                        noClipCheckBox.Checked = renderFlags.HasFlag(BitmapEffectRenderingFlags.DisableSelectionClipping);
                        aliasedSelectionCheckBox.Checked = renderFlags.HasFlag(BitmapEffectRenderingFlags.ForceAliasedSelectionQuality);
                        break;
                }

                if (!Initializing)
                {
                    Settings.RenderPreset = selectedPreset;
                }
            }
        }

        private void RenderOption_CheckedChanged(object sender, EventArgs e)
        {
            if (!Initializing &&
                presetComboBox.SelectedItem is PresentBoxItem item &&
                item.Preset == RenderPreset.UserDefined)
            {
                BitmapEffectRenderingFlags flags = BitmapEffectRenderingFlags.None;
                if (noClipCheckBox.Checked) { flags |= BitmapEffectRenderingFlags.DisableSelectionClipping; }
                if (aliasedSelectionCheckBox.Checked) { flags |= BitmapEffectRenderingFlags.ForceAliasedSelectionQuality; }
                if (singleThreadedCheckBox.Checked) { flags |= BitmapEffectRenderingFlags.SingleThreaded; }

                Settings.RenderingFlags = flags;

                Settings.RenderingSchedule = squareTilesRadioButton.Checked
                    ? BitmapEffectRenderingSchedule.SquareTiles : horizontalStripsRadioButton.Checked
                    ? BitmapEffectRenderingSchedule.HorizontalStrips : noneRadioButton.Checked
                    ? BitmapEffectRenderingSchedule.None
                    : BitmapEffectRenderingSchedule.SquareTiles;
            }
        }

        private class PresentBoxItem
        {
            internal RenderPreset Preset { get; }

            private PresentBoxItem(RenderPreset renderPresetOption)
            {
                Preset = renderPresetOption;
            }

            public override string ToString()
            {
                return Preset.GetName();
            }

            internal static PresentBoxItem[] Items { get; } = Enum.GetValues<RenderPreset>().Select(x => new PresentBoxItem(x)).ToArray();
        }
        #endregion

        #region Assistance
        private void disableAutoCompCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            bool noAutoComplete = disableAutoCompCheckBox.Checked;

            if (!Initializing)
            {
                Settings.DisableAutoComplete = noAutoComplete;
            }

            noAutoCompleteInfoLabel.Visible = noAutoComplete;
        }

        private void docCommentCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (!Initializing)
            {
                DocCommentOptions flags = DocCommentOptions.None;
                if (dcEnabledCheckBox.Checked) { flags |= DocCommentOptions.Enabled; }
                if (dcToolTipsCheckBox.Checked) { flags |= DocCommentOptions.ToolTips; }
                if (dcDefsCheckBox.Checked) { flags |= DocCommentOptions.Definitions; }
                if (dcBclCheckBox.Checked) { flags |= DocCommentOptions.BCL; }

                Settings.DocCommentOptions = flags;
            }

            bool enabled = dcEnabledCheckBox.Checked;
            dcToolTipsCheckBox.Enabled = enabled;
            dcDefsCheckBox.Enabled = enabled;
            dcBclCheckBox.Enabled = enabled;

            noSdkWarnLabel.Visible = enabled && dcBclCheckBox.Checked && !DocComment.TryGetSdkDirectory(out _);
        }
        #endregion
    }
}
