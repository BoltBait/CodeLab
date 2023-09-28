namespace PdnCodeLab
{
    partial class SettingsForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsForm));
            closeButton = new System.Windows.Forms.Button();
            panelUpdates = new System.Windows.Forms.Panel();
            checkNowButton = new System.Windows.Forms.Button();
            checkForUpdates = new System.Windows.Forms.CheckBox();
            panelCompiler = new System.Windows.Forms.Panel();
            label7 = new System.Windows.Forms.Label();
            label6 = new System.Windows.Forms.Label();
            lookupWarningButton = new System.Windows.Forms.Button();
            warningToIgnoreLabel = new System.Windows.Forms.Label();
            warningsToIgnoreList = new System.Windows.Forms.ListBox();
            removeWarningButton = new System.Windows.Forms.Button();
            label9 = new System.Windows.Forms.Label();
            label8 = new System.Windows.Forms.Label();
            warningLevelCombobox = new System.Windows.Forms.ComboBox();
            panelUI = new System.Windows.Forms.Panel();
            label10 = new System.Windows.Forms.Label();
            disableAutoCompCheckBox = new System.Windows.Forms.CheckBox();
            extendedColorsCheckBox = new System.Windows.Forms.CheckBox();
            caretLineFrameCheckBox = new System.Windows.Forms.CheckBox();
            indentSpacesLabel = new System.Windows.Forms.Label();
            indentSpacesComboBox = new System.Windows.Forms.ComboBox();
            toolbarCheckbox = new System.Windows.Forms.CheckBox();
            label5 = new System.Windows.Forms.Label();
            linkLabel1 = new System.Windows.Forms.LinkLabel();
            label4 = new System.Windows.Forms.Label();
            wordWrapTextFilesCheckbox = new System.Windows.Forms.CheckBox();
            themeCombobox = new System.Windows.Forms.ComboBox();
            label3 = new System.Windows.Forms.Label();
            largeFontCheckbox = new System.Windows.Forms.CheckBox();
            fontCombobox = new System.Windows.Forms.ComboBox();
            label2 = new System.Windows.Forms.Label();
            showWhiteSpaceCheckbox = new System.Windows.Forms.CheckBox();
            label1 = new System.Windows.Forms.Label();
            wordWrapCheckbox = new System.Windows.Forms.CheckBox();
            indicatorMapCheckbox = new System.Windows.Forms.CheckBox();
            codeFoldingCheckbox = new System.Windows.Forms.CheckBox();
            bookMarksCheckbox = new System.Windows.Forms.CheckBox();
            lineNumbersCheckbox = new System.Windows.Forms.CheckBox();
            settingsList = new System.Windows.Forms.ListBox();
            panelSnippet = new SnippetManager();
            panelSpelling = new System.Windows.Forms.Panel();
            addLangsButton = new System.Windows.Forms.Button();
            enableSpellcheckCheckBox = new System.Windows.Forms.CheckBox();
            addWordsToIgnoreLabel = new System.Windows.Forms.Label();
            wordsToIgnoreLabel = new System.Windows.Forms.Label();
            wordsToIgnoreListBox = new System.Windows.Forms.ListBox();
            removeIgnoreWordButton = new System.Windows.Forms.Button();
            label11 = new System.Windows.Forms.Label();
            spellcheckOptionsLabel = new System.Windows.Forms.Label();
            spellLangComboBox = new System.Windows.Forms.ComboBox();
            panelRenderOptions = new System.Windows.Forms.Panel();
            presetLabel = new System.Windows.Forms.Label();
            panelRenderOptionsSub = new System.Windows.Forms.Panel();
            flagsLabel = new System.Windows.Forms.Label();
            scheduleLabel = new System.Windows.Forms.Label();
            horizontalStripsRadioButton = new System.Windows.Forms.RadioButton();
            squareTilesRadioButton = new System.Windows.Forms.RadioButton();
            aliasedSelectionCheckBox = new System.Windows.Forms.CheckBox();
            noneRadioButton = new System.Windows.Forms.RadioButton();
            noClipCheckBox = new System.Windows.Forms.CheckBox();
            singleThreadedCheckBox = new System.Windows.Forms.CheckBox();
            presetComboBox = new System.Windows.Forms.ComboBox();
            panelUpdates.SuspendLayout();
            panelCompiler.SuspendLayout();
            panelUI.SuspendLayout();
            panelSpelling.SuspendLayout();
            panelRenderOptions.SuspendLayout();
            panelRenderOptionsSub.SuspendLayout();
            SuspendLayout();
            // 
            // closeButton
            // 
            closeButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            closeButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            closeButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            closeButton.Location = new System.Drawing.Point(604, 433);
            closeButton.Name = "closeButton";
            closeButton.Size = new System.Drawing.Size(75, 23);
            closeButton.TabIndex = 3;
            closeButton.Text = "Close";
            closeButton.UseVisualStyleBackColor = true;
            // 
            // panelUpdates
            // 
            panelUpdates.Controls.Add(checkNowButton);
            panelUpdates.Controls.Add(checkForUpdates);
            panelUpdates.Location = new System.Drawing.Point(202, 12);
            panelUpdates.Name = "panelUpdates";
            panelUpdates.Size = new System.Drawing.Size(476, 405);
            panelUpdates.TabIndex = 2;
            // 
            // checkNowButton
            // 
            checkNowButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            checkNowButton.Location = new System.Drawing.Point(16, 53);
            checkNowButton.Name = "checkNowButton";
            checkNowButton.Size = new System.Drawing.Size(88, 23);
            checkNowButton.TabIndex = 1;
            checkNowButton.Text = "Check Now";
            checkNowButton.UseVisualStyleBackColor = true;
            checkNowButton.Click += checkNowButton_Click;
            // 
            // checkForUpdates
            // 
            checkForUpdates.AutoSize = true;
            checkForUpdates.Location = new System.Drawing.Point(16, 17);
            checkForUpdates.Name = "checkForUpdates";
            checkForUpdates.Size = new System.Drawing.Size(310, 19);
            checkForUpdates.TabIndex = 0;
            checkForUpdates.Text = "Automatically check for updates when CodeLab starts";
            checkForUpdates.UseVisualStyleBackColor = true;
            checkForUpdates.CheckedChanged += checkForUpdates_CheckedChanged;
            // 
            // panelCompiler
            // 
            panelCompiler.Controls.Add(label7);
            panelCompiler.Controls.Add(label6);
            panelCompiler.Controls.Add(lookupWarningButton);
            panelCompiler.Controls.Add(warningToIgnoreLabel);
            panelCompiler.Controls.Add(warningsToIgnoreList);
            panelCompiler.Controls.Add(removeWarningButton);
            panelCompiler.Controls.Add(label9);
            panelCompiler.Controls.Add(label8);
            panelCompiler.Controls.Add(warningLevelCombobox);
            panelCompiler.Location = new System.Drawing.Point(202, 12);
            panelCompiler.Name = "panelCompiler";
            panelCompiler.Size = new System.Drawing.Size(476, 405);
            panelCompiler.TabIndex = 0;
            // 
            // label7
            // 
            label7.Location = new System.Drawing.Point(141, 44);
            label7.Name = "label7";
            label7.Size = new System.Drawing.Size(323, 99);
            label7.TabIndex = 8;
            label7.Text = resources.GetString("label7.Text");
            // 
            // label6
            // 
            label6.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            label6.Location = new System.Drawing.Point(133, 247);
            label6.Name = "label6";
            label6.Size = new System.Drawing.Size(313, 75);
            label6.TabIndex = 7;
            label6.Text = "To add a specific warning to the list of warnings to ignore, when you see a warning in the error's list below the code window, right-click on the warning and choose \"Ignore this Warning\".";
            // 
            // lookupWarningButton
            // 
            lookupWarningButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            lookupWarningButton.Location = new System.Drawing.Point(133, 211);
            lookupWarningButton.Name = "lookupWarningButton";
            lookupWarningButton.Size = new System.Drawing.Size(108, 25);
            lookupWarningButton.TabIndex = 6;
            lookupWarningButton.Text = "Lookup Warning";
            lookupWarningButton.UseVisualStyleBackColor = true;
            lookupWarningButton.Click += lookupWarningButton_Click;
            // 
            // warningToIgnoreLabel
            // 
            warningToIgnoreLabel.AutoSize = true;
            warningToIgnoreLabel.Location = new System.Drawing.Point(35, 155);
            warningToIgnoreLabel.Name = "warningToIgnoreLabel";
            warningToIgnoreLabel.Size = new System.Drawing.Size(111, 15);
            warningToIgnoreLabel.TabIndex = 3;
            warningToIgnoreLabel.Text = "Warnings to ignore:";
            // 
            // warningsToIgnoreList
            // 
            warningsToIgnoreList.FormattingEnabled = true;
            warningsToIgnoreList.ItemHeight = 15;
            warningsToIgnoreList.Location = new System.Drawing.Point(48, 180);
            warningsToIgnoreList.Name = "warningsToIgnoreList";
            warningsToIgnoreList.Size = new System.Drawing.Size(75, 139);
            warningsToIgnoreList.TabIndex = 4;
            // 
            // removeWarningButton
            // 
            removeWarningButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            removeWarningButton.Location = new System.Drawing.Point(133, 180);
            removeWarningButton.Name = "removeWarningButton";
            removeWarningButton.Size = new System.Drawing.Size(108, 25);
            removeWarningButton.TabIndex = 5;
            removeWarningButton.Text = "Remove Warning";
            removeWarningButton.UseVisualStyleBackColor = true;
            removeWarningButton.Click += removeWarningButton_Click;
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Location = new System.Drawing.Point(35, 44);
            label9.Name = "label9";
            label9.Size = new System.Drawing.Size(82, 15);
            label9.TabIndex = 1;
            label9.Text = "Warning level:";
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            label8.Location = new System.Drawing.Point(14, 17);
            label8.Name = "label8";
            label8.Size = new System.Drawing.Size(120, 15);
            label8.TabIndex = 0;
            label8.Text = "C# compiler options:";
            // 
            // warningLevelCombobox
            // 
            warningLevelCombobox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            warningLevelCombobox.FormattingEnabled = true;
            warningLevelCombobox.Items.AddRange(new object[] { "0", "1", "2", "3", "4" });
            warningLevelCombobox.Location = new System.Drawing.Point(48, 68);
            warningLevelCombobox.Name = "warningLevelCombobox";
            warningLevelCombobox.Size = new System.Drawing.Size(75, 23);
            warningLevelCombobox.TabIndex = 2;
            warningLevelCombobox.SelectedIndexChanged += warningLevelCombobox_SelectedIndexChanged;
            // 
            // panelUI
            // 
            panelUI.Controls.Add(label10);
            panelUI.Controls.Add(disableAutoCompCheckBox);
            panelUI.Controls.Add(extendedColorsCheckBox);
            panelUI.Controls.Add(caretLineFrameCheckBox);
            panelUI.Controls.Add(indentSpacesLabel);
            panelUI.Controls.Add(indentSpacesComboBox);
            panelUI.Controls.Add(toolbarCheckbox);
            panelUI.Controls.Add(label5);
            panelUI.Controls.Add(linkLabel1);
            panelUI.Controls.Add(label4);
            panelUI.Controls.Add(wordWrapTextFilesCheckbox);
            panelUI.Controls.Add(themeCombobox);
            panelUI.Controls.Add(label3);
            panelUI.Controls.Add(largeFontCheckbox);
            panelUI.Controls.Add(fontCombobox);
            panelUI.Controls.Add(label2);
            panelUI.Controls.Add(showWhiteSpaceCheckbox);
            panelUI.Controls.Add(label1);
            panelUI.Controls.Add(wordWrapCheckbox);
            panelUI.Controls.Add(indicatorMapCheckbox);
            panelUI.Controls.Add(codeFoldingCheckbox);
            panelUI.Controls.Add(bookMarksCheckbox);
            panelUI.Controls.Add(lineNumbersCheckbox);
            panelUI.Location = new System.Drawing.Point(202, 12);
            panelUI.Name = "panelUI";
            panelUI.Size = new System.Drawing.Size(476, 405);
            panelUI.TabIndex = 1;
            // 
            // label10
            // 
            label10.Location = new System.Drawing.Point(225, 132);
            label10.Name = "label10";
            label10.Size = new System.Drawing.Size(239, 51);
            label10.TabIndex = 21;
            label10.Text = "When disabled, you can still trigger the\r\nautocomplete window to open after\r\ntyping period by pressing Ctrl-J";
            // 
            // disableAutoCompCheckBox
            // 
            disableAutoCompCheckBox.AutoSize = true;
            disableAutoCompCheckBox.Location = new System.Drawing.Point(32, 131);
            disableAutoCompCheckBox.Name = "disableAutoCompCheckBox";
            disableAutoCompCheckBox.Size = new System.Drawing.Size(157, 19);
            disableAutoCompCheckBox.TabIndex = 20;
            disableAutoCompCheckBox.Text = "Disable Auto Complete...";
            disableAutoCompCheckBox.UseVisualStyleBackColor = true;
            disableAutoCompCheckBox.CheckedChanged += disableAutoCompCheckBox_CheckedChanged;
            // 
            // extendedColorsCheckBox
            // 
            extendedColorsCheckBox.AutoSize = true;
            extendedColorsCheckBox.Location = new System.Drawing.Point(32, 378);
            extendedColorsCheckBox.Name = "extendedColorsCheckBox";
            extendedColorsCheckBox.Size = new System.Drawing.Size(192, 19);
            extendedColorsCheckBox.TabIndex = 19;
            extendedColorsCheckBox.Text = "Extended Colors (Experimental)";
            extendedColorsCheckBox.UseVisualStyleBackColor = true;
            extendedColorsCheckBox.CheckedChanged += extendedColorsCheckBox_CheckedChanged;
            // 
            // caretLineFrameCheckBox
            // 
            caretLineFrameCheckBox.AutoSize = true;
            caretLineFrameCheckBox.Location = new System.Drawing.Point(237, 105);
            caretLineFrameCheckBox.Name = "caretLineFrameCheckBox";
            caretLineFrameCheckBox.Size = new System.Drawing.Size(122, 19);
            caretLineFrameCheckBox.TabIndex = 8;
            caretLineFrameCheckBox.Text = "Current line frame";
            caretLineFrameCheckBox.UseVisualStyleBackColor = true;
            caretLineFrameCheckBox.CheckedChanged += caretLineFrameCheckBox_CheckedChanged;
            // 
            // indentSpacesLabel
            // 
            indentSpacesLabel.AutoSize = true;
            indentSpacesLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            indentSpacesLabel.Location = new System.Drawing.Point(13, 160);
            indentSpacesLabel.Name = "indentSpacesLabel";
            indentSpacesLabel.Size = new System.Drawing.Size(88, 15);
            indentSpacesLabel.TabIndex = 9;
            indentSpacesLabel.Text = "Indent Spaces:";
            // 
            // indentSpacesComboBox
            // 
            indentSpacesComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            indentSpacesComboBox.FormattingEnabled = true;
            indentSpacesComboBox.Items.AddRange(new object[] { "2", "4" });
            indentSpacesComboBox.Location = new System.Drawing.Point(32, 186);
            indentSpacesComboBox.Name = "indentSpacesComboBox";
            indentSpacesComboBox.Size = new System.Drawing.Size(50, 23);
            indentSpacesComboBox.TabIndex = 10;
            indentSpacesComboBox.SelectedIndexChanged += indentSpacesComboBox_SelectedIndexChanged;
            // 
            // toolbarCheckbox
            // 
            toolbarCheckbox.AutoSize = true;
            toolbarCheckbox.Location = new System.Drawing.Point(32, 28);
            toolbarCheckbox.Name = "toolbarCheckbox";
            toolbarCheckbox.Size = new System.Drawing.Size(65, 19);
            toolbarCheckbox.TabIndex = 1;
            toolbarCheckbox.Text = "Toolbar";
            toolbarCheckbox.UseVisualStyleBackColor = true;
            toolbarCheckbox.CheckedChanged += toolbarCheckbox_CheckedChanged;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new System.Drawing.Point(228, 354);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(190, 15);
            label5.TabIndex = 18;
            label5.Text = "\"Auto\" matches Paint.NET's theme";
            // 
            // linkLabel1
            // 
            linkLabel1.AutoSize = true;
            linkLabel1.Location = new System.Drawing.Point(234, 301);
            linkLabel1.Name = "linkLabel1";
            linkLabel1.Size = new System.Drawing.Size(88, 15);
            linkLabel1.TabIndex = 15;
            linkLabel1.TabStop = true;
            linkLabel1.Text = "Help with fonts";
            linkLabel1.LinkClicked += linkLabel1_LinkClicked;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new System.Drawing.Point(225, 275);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(177, 15);
            label4.TabIndex = 13;
            label4.Text = "* = not available on your system";
            // 
            // wordWrapTextFilesCheckbox
            // 
            wordWrapTextFilesCheckbox.AutoSize = true;
            wordWrapTextFilesCheckbox.Location = new System.Drawing.Point(237, 80);
            wordWrapTextFilesCheckbox.Name = "wordWrapTextFilesCheckbox";
            wordWrapTextFilesCheckbox.Size = new System.Drawing.Size(150, 19);
            wordWrapTextFilesCheckbox.TabIndex = 6;
            wordWrapTextFilesCheckbox.Text = "Word wrap text files  ‹‒'";
            wordWrapTextFilesCheckbox.UseVisualStyleBackColor = true;
            wordWrapTextFilesCheckbox.CheckedChanged += wordWrapTextFilesCheckbox_CheckedChanged;
            // 
            // themeCombobox
            // 
            themeCombobox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            themeCombobox.FormattingEnabled = true;
            themeCombobox.Items.AddRange(new object[] { "Auto", "Light", "Dark" });
            themeCombobox.Location = new System.Drawing.Point(32, 349);
            themeCombobox.Name = "themeCombobox";
            themeCombobox.Size = new System.Drawing.Size(166, 23);
            themeCombobox.TabIndex = 17;
            themeCombobox.SelectedIndexChanged += themeCombobox_SelectedIndexChanged;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            label3.Location = new System.Drawing.Point(13, 330);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(49, 15);
            label3.TabIndex = 16;
            label3.Text = "Theme:";
            // 
            // largeFontCheckbox
            // 
            largeFontCheckbox.AutoSize = true;
            largeFontCheckbox.Location = new System.Drawing.Point(32, 300);
            largeFontCheckbox.Name = "largeFontCheckbox";
            largeFontCheckbox.Size = new System.Drawing.Size(85, 19);
            largeFontCheckbox.TabIndex = 14;
            largeFontCheckbox.Text = "Large fonts";
            largeFontCheckbox.UseVisualStyleBackColor = true;
            largeFontCheckbox.CheckedChanged += largeFontCheckbox_CheckedChanged;
            // 
            // fontCombobox
            // 
            fontCombobox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            fontCombobox.FormattingEnabled = true;
            fontCombobox.Items.AddRange(new object[] { "Cascadia Code", "Consolas", "Courier New", "Envy Code R", "Fira Code", "Hack", "JetBrains Mono", "Verdana" });
            fontCombobox.Location = new System.Drawing.Point(32, 270);
            fontCombobox.Name = "fontCombobox";
            fontCombobox.Size = new System.Drawing.Size(166, 23);
            fontCombobox.TabIndex = 12;
            fontCombobox.SelectedIndexChanged += fontCombobox_SelectedIndexChanged;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            label2.Location = new System.Drawing.Point(13, 251);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(40, 15);
            label2.TabIndex = 11;
            label2.Text = "Fonts:";
            // 
            // showWhiteSpaceCheckbox
            // 
            showWhiteSpaceCheckbox.AutoSize = true;
            showWhiteSpaceCheckbox.Location = new System.Drawing.Point(32, 216);
            showWhiteSpaceCheckbox.Name = "showWhiteSpaceCheckbox";
            showWhiteSpaceCheckbox.Size = new System.Drawing.Size(129, 19);
            showWhiteSpaceCheckbox.TabIndex = 11;
            showWhiteSpaceCheckbox.Text = "Show whitespace ···";
            showWhiteSpaceCheckbox.UseVisualStyleBackColor = true;
            showWhiteSpaceCheckbox.CheckedChanged += showWhiteSpaceCheckbox_CheckedChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            label1.Location = new System.Drawing.Point(13, 4);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(116, 15);
            label1.TabIndex = 0;
            label1.Text = "Editor functionality:";
            // 
            // wordWrapCheckbox
            // 
            wordWrapCheckbox.AutoSize = true;
            wordWrapCheckbox.Location = new System.Drawing.Point(237, 54);
            wordWrapCheckbox.Name = "wordWrapCheckbox";
            wordWrapCheckbox.Size = new System.Drawing.Size(156, 19);
            wordWrapCheckbox.TabIndex = 4;
            wordWrapCheckbox.Text = "Word wrap code files  ‹‒'";
            wordWrapCheckbox.UseVisualStyleBackColor = true;
            wordWrapCheckbox.CheckedChanged += wordWrapCheckbox_CheckedChanged;
            // 
            // indicatorMapCheckbox
            // 
            indicatorMapCheckbox.AutoSize = true;
            indicatorMapCheckbox.Location = new System.Drawing.Point(237, 28);
            indicatorMapCheckbox.Name = "indicatorMapCheckbox";
            indicatorMapCheckbox.Size = new System.Drawing.Size(156, 19);
            indicatorMapCheckbox.TabIndex = 2;
            indicatorMapCheckbox.Text = "Indicator map (scrollbar)";
            indicatorMapCheckbox.UseVisualStyleBackColor = true;
            indicatorMapCheckbox.CheckedChanged += indicatorMapCheckbox_CheckedChanged;
            // 
            // codeFoldingCheckbox
            // 
            codeFoldingCheckbox.AutoSize = true;
            codeFoldingCheckbox.Location = new System.Drawing.Point(32, 105);
            codeFoldingCheckbox.Name = "codeFoldingCheckbox";
            codeFoldingCheckbox.Size = new System.Drawing.Size(114, 19);
            codeFoldingCheckbox.TabIndex = 7;
            codeFoldingCheckbox.Text = "Code folding [+]";
            codeFoldingCheckbox.UseVisualStyleBackColor = true;
            codeFoldingCheckbox.CheckedChanged += codeFoldingCheckbox_CheckedChanged;
            // 
            // bookMarksCheckbox
            // 
            bookMarksCheckbox.AutoSize = true;
            bookMarksCheckbox.Location = new System.Drawing.Point(32, 79);
            bookMarksCheckbox.Name = "bookMarksCheckbox";
            bookMarksCheckbox.Size = new System.Drawing.Size(88, 19);
            bookMarksCheckbox.TabIndex = 5;
            bookMarksCheckbox.Text = "Book marks";
            bookMarksCheckbox.UseVisualStyleBackColor = true;
            bookMarksCheckbox.CheckedChanged += bookMarksCheckbox_CheckedChanged;
            // 
            // lineNumbersCheckbox
            // 
            lineNumbersCheckbox.AutoSize = true;
            lineNumbersCheckbox.Location = new System.Drawing.Point(32, 53);
            lineNumbersCheckbox.Name = "lineNumbersCheckbox";
            lineNumbersCheckbox.Size = new System.Drawing.Size(98, 19);
            lineNumbersCheckbox.TabIndex = 3;
            lineNumbersCheckbox.Text = "Line numbers";
            lineNumbersCheckbox.UseVisualStyleBackColor = true;
            lineNumbersCheckbox.CheckedChanged += lineNumbersCheckbox_CheckedChanged;
            // 
            // settingsList
            // 
            settingsList.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            settingsList.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            settingsList.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            settingsList.FormattingEnabled = true;
            settingsList.IntegralHeight = false;
            settingsList.ItemHeight = 32;
            settingsList.Items.AddRange(new object[] { "User Interface", "Snippets", "Spellcheck", "Compiler", "Updates", "Render Options" });
            settingsList.Location = new System.Drawing.Point(12, 12);
            settingsList.Name = "settingsList";
            settingsList.Size = new System.Drawing.Size(184, 406);
            settingsList.TabIndex = 0;
            settingsList.DrawItem += settingsList_DrawItem;
            settingsList.SelectedIndexChanged += settingsList_SelectedIndexChanged;
            // 
            // panelSnippet
            // 
            panelSnippet.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            panelSnippet.Location = new System.Drawing.Point(202, 12);
            panelSnippet.Name = "panelSnippet";
            panelSnippet.Size = new System.Drawing.Size(476, 405);
            panelSnippet.TabIndex = 1;
            // 
            // panelSpelling
            // 
            panelSpelling.Controls.Add(addLangsButton);
            panelSpelling.Controls.Add(enableSpellcheckCheckBox);
            panelSpelling.Controls.Add(addWordsToIgnoreLabel);
            panelSpelling.Controls.Add(wordsToIgnoreLabel);
            panelSpelling.Controls.Add(wordsToIgnoreListBox);
            panelSpelling.Controls.Add(removeIgnoreWordButton);
            panelSpelling.Controls.Add(label11);
            panelSpelling.Controls.Add(spellcheckOptionsLabel);
            panelSpelling.Controls.Add(spellLangComboBox);
            panelSpelling.Location = new System.Drawing.Point(202, 12);
            panelSpelling.Name = "panelSpelling";
            panelSpelling.Size = new System.Drawing.Size(476, 405);
            panelSpelling.TabIndex = 8;
            // 
            // addLangsButton
            // 
            addLangsButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            addLangsButton.Location = new System.Drawing.Point(157, 111);
            addLangsButton.Name = "addLangsButton";
            addLangsButton.Size = new System.Drawing.Size(118, 25);
            addLangsButton.TabIndex = 4;
            addLangsButton.Text = "Add Languages";
            addLangsButton.UseVisualStyleBackColor = true;
            addLangsButton.Click += addLangsButton_Click;
            // 
            // enableSpellcheckCheckBox
            // 
            enableSpellcheckCheckBox.AutoSize = true;
            enableSpellcheckCheckBox.Location = new System.Drawing.Point(38, 52);
            enableSpellcheckCheckBox.Name = "enableSpellcheckCheckBox";
            enableSpellcheckCheckBox.Size = new System.Drawing.Size(120, 19);
            enableSpellcheckCheckBox.TabIndex = 1;
            enableSpellcheckCheckBox.Text = "Enable Spellcheck";
            enableSpellcheckCheckBox.UseVisualStyleBackColor = true;
            enableSpellcheckCheckBox.CheckedChanged += enableSpellcheckCheckBox_CheckedChanged;
            // 
            // addWordsToIgnoreLabel
            // 
            addWordsToIgnoreLabel.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            addWordsToIgnoreLabel.Location = new System.Drawing.Point(154, 208);
            addWordsToIgnoreLabel.Name = "addWordsToIgnoreLabel";
            addWordsToIgnoreLabel.Size = new System.Drawing.Size(310, 75);
            addWordsToIgnoreLabel.TabIndex = 8;
            addWordsToIgnoreLabel.Text = "To add a specific word to the list of words to ignore, when you see a red underlined word in the code/text window, hover over the word and choose \"Ignore Word\" from the light bulb menu.";
            // 
            // wordsToIgnoreLabel
            // 
            wordsToIgnoreLabel.AutoSize = true;
            wordsToIgnoreLabel.Location = new System.Drawing.Point(35, 155);
            wordsToIgnoreLabel.Name = "wordsToIgnoreLabel";
            wordsToIgnoreLabel.Size = new System.Drawing.Size(95, 15);
            wordsToIgnoreLabel.TabIndex = 5;
            wordsToIgnoreLabel.Text = "Words to ignore:";
            // 
            // wordsToIgnoreListBox
            // 
            wordsToIgnoreListBox.FormattingEnabled = true;
            wordsToIgnoreListBox.ItemHeight = 15;
            wordsToIgnoreListBox.Location = new System.Drawing.Point(48, 180);
            wordsToIgnoreListBox.Name = "wordsToIgnoreListBox";
            wordsToIgnoreListBox.Size = new System.Drawing.Size(100, 139);
            wordsToIgnoreListBox.TabIndex = 6;
            // 
            // removeIgnoreWordButton
            // 
            removeIgnoreWordButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            removeIgnoreWordButton.Location = new System.Drawing.Point(157, 180);
            removeIgnoreWordButton.Name = "removeIgnoreWordButton";
            removeIgnoreWordButton.Size = new System.Drawing.Size(118, 24);
            removeIgnoreWordButton.TabIndex = 7;
            removeIgnoreWordButton.Text = "Remove Word";
            removeIgnoreWordButton.UseVisualStyleBackColor = true;
            removeIgnoreWordButton.Click += removeIgnoreWordButton_Click;
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.Location = new System.Drawing.Point(35, 88);
            label11.Name = "label11";
            label11.Size = new System.Drawing.Size(62, 15);
            label11.TabIndex = 2;
            label11.Text = "Language:";
            // 
            // spellcheckOptionsLabel
            // 
            spellcheckOptionsLabel.AutoSize = true;
            spellcheckOptionsLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            spellcheckOptionsLabel.Location = new System.Drawing.Point(14, 17);
            spellcheckOptionsLabel.Name = "spellcheckOptionsLabel";
            spellcheckOptionsLabel.Size = new System.Drawing.Size(114, 15);
            spellcheckOptionsLabel.TabIndex = 0;
            spellcheckOptionsLabel.Text = "Spellcheck options:";
            // 
            // spellLangComboBox
            // 
            spellLangComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            spellLangComboBox.FormattingEnabled = true;
            spellLangComboBox.Location = new System.Drawing.Point(48, 112);
            spellLangComboBox.Name = "spellLangComboBox";
            spellLangComboBox.Size = new System.Drawing.Size(100, 23);
            spellLangComboBox.TabIndex = 3;
            spellLangComboBox.SelectedIndexChanged += spellLangComboBox_SelectedIndexChanged;
            // 
            // panelRenderOptions
            // 
            panelRenderOptions.Controls.Add(presetLabel);
            panelRenderOptions.Controls.Add(panelRenderOptionsSub);
            panelRenderOptions.Controls.Add(presetComboBox);
            panelRenderOptions.Location = new System.Drawing.Point(202, 12);
            panelRenderOptions.Name = "panelRenderOptions";
            panelRenderOptions.Size = new System.Drawing.Size(476, 405);
            panelRenderOptions.TabIndex = 9;
            // 
            // presetLabel
            // 
            presetLabel.AutoSize = true;
            presetLabel.Location = new System.Drawing.Point(17, 24);
            presetLabel.Name = "presetLabel";
            presetLabel.Size = new System.Drawing.Size(39, 15);
            presetLabel.TabIndex = 13;
            presetLabel.Text = "Preset";
            // 
            // panelRenderOptionsSub
            // 
            panelRenderOptionsSub.Controls.Add(flagsLabel);
            panelRenderOptionsSub.Controls.Add(scheduleLabel);
            panelRenderOptionsSub.Controls.Add(horizontalStripsRadioButton);
            panelRenderOptionsSub.Controls.Add(squareTilesRadioButton);
            panelRenderOptionsSub.Controls.Add(aliasedSelectionCheckBox);
            panelRenderOptionsSub.Controls.Add(noneRadioButton);
            panelRenderOptionsSub.Controls.Add(noClipCheckBox);
            panelRenderOptionsSub.Controls.Add(singleThreadedCheckBox);
            panelRenderOptionsSub.Location = new System.Drawing.Point(17, 50);
            panelRenderOptionsSub.Name = "panelRenderOptionsSub";
            panelRenderOptionsSub.Size = new System.Drawing.Size(398, 141);
            panelRenderOptionsSub.TabIndex = 12;
            // 
            // flagsLabel
            // 
            flagsLabel.AutoSize = true;
            flagsLabel.Location = new System.Drawing.Point(193, 23);
            flagsLabel.Name = "flagsLabel";
            flagsLabel.Size = new System.Drawing.Size(81, 15);
            flagsLabel.TabIndex = 8;
            flagsLabel.Text = "Rending Flags";
            // 
            // scheduleLabel
            // 
            scheduleLabel.AutoSize = true;
            scheduleLabel.Location = new System.Drawing.Point(11, 23);
            scheduleLabel.Name = "scheduleLabel";
            scheduleLabel.Size = new System.Drawing.Size(102, 15);
            scheduleLabel.TabIndex = 7;
            scheduleLabel.Text = "Rending Schedule";
            // 
            // horizontalStripsRadioButton
            // 
            horizontalStripsRadioButton.AutoSize = true;
            horizontalStripsRadioButton.Location = new System.Drawing.Point(14, 65);
            horizontalStripsRadioButton.Name = "horizontalStripsRadioButton";
            horizontalStripsRadioButton.Size = new System.Drawing.Size(112, 19);
            horizontalStripsRadioButton.TabIndex = 5;
            horizontalStripsRadioButton.TabStop = true;
            horizontalStripsRadioButton.Text = "Horizontal Strips";
            horizontalStripsRadioButton.UseVisualStyleBackColor = true;
            horizontalStripsRadioButton.CheckedChanged += RenderOption_CheckedChanged;
            // 
            // squareTilesRadioButton
            // 
            squareTilesRadioButton.AutoSize = true;
            squareTilesRadioButton.Location = new System.Drawing.Point(14, 40);
            squareTilesRadioButton.Name = "squareTilesRadioButton";
            squareTilesRadioButton.Size = new System.Drawing.Size(87, 19);
            squareTilesRadioButton.TabIndex = 4;
            squareTilesRadioButton.TabStop = true;
            squareTilesRadioButton.Text = "Square Tiles";
            squareTilesRadioButton.UseVisualStyleBackColor = true;
            squareTilesRadioButton.CheckedChanged += RenderOption_CheckedChanged;
            // 
            // aliasedSelectionCheckBox
            // 
            aliasedSelectionCheckBox.AutoSize = true;
            aliasedSelectionCheckBox.Location = new System.Drawing.Point(196, 66);
            aliasedSelectionCheckBox.Name = "aliasedSelectionCheckBox";
            aliasedSelectionCheckBox.Size = new System.Drawing.Size(188, 19);
            aliasedSelectionCheckBox.TabIndex = 2;
            aliasedSelectionCheckBox.Text = "Force Aliased Selection Quality";
            aliasedSelectionCheckBox.UseVisualStyleBackColor = true;
            aliasedSelectionCheckBox.CheckedChanged += RenderOption_CheckedChanged;
            // 
            // noneRadioButton
            // 
            noneRadioButton.AutoSize = true;
            noneRadioButton.Location = new System.Drawing.Point(14, 90);
            noneRadioButton.Name = "noneRadioButton";
            noneRadioButton.Size = new System.Drawing.Size(54, 19);
            noneRadioButton.TabIndex = 6;
            noneRadioButton.TabStop = true;
            noneRadioButton.Text = "None";
            noneRadioButton.UseVisualStyleBackColor = true;
            noneRadioButton.CheckedChanged += RenderOption_CheckedChanged;
            // 
            // noClipCheckBox
            // 
            noClipCheckBox.AutoSize = true;
            noClipCheckBox.Location = new System.Drawing.Point(196, 41);
            noClipCheckBox.Name = "noClipCheckBox";
            noClipCheckBox.Size = new System.Drawing.Size(163, 19);
            noClipCheckBox.TabIndex = 1;
            noClipCheckBox.Text = "Disable Selection Clipping";
            noClipCheckBox.UseVisualStyleBackColor = true;
            noClipCheckBox.CheckedChanged += RenderOption_CheckedChanged;
            // 
            // singleThreadedCheckBox
            // 
            singleThreadedCheckBox.AutoSize = true;
            singleThreadedCheckBox.Location = new System.Drawing.Point(196, 91);
            singleThreadedCheckBox.Name = "singleThreadedCheckBox";
            singleThreadedCheckBox.Size = new System.Drawing.Size(110, 19);
            singleThreadedCheckBox.TabIndex = 0;
            singleThreadedCheckBox.Text = "Single Threaded";
            singleThreadedCheckBox.UseVisualStyleBackColor = true;
            singleThreadedCheckBox.CheckedChanged += RenderOption_CheckedChanged;
            // 
            // presetComboBox
            // 
            presetComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            presetComboBox.FormattingEnabled = true;
            presetComboBox.Location = new System.Drawing.Point(62, 21);
            presetComboBox.Name = "presetComboBox";
            presetComboBox.Size = new System.Drawing.Size(353, 23);
            presetComboBox.TabIndex = 11;
            presetComboBox.SelectedIndexChanged += presetComboBox_SelectedIndexChanged;
            // 
            // SettingsForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            CancelButton = closeButton;
            ClientSize = new System.Drawing.Size(691, 468);
            Controls.Add(panelRenderOptions);
            Controls.Add(settingsList);
            Controls.Add(closeButton);
            Controls.Add(panelUI);
            Controls.Add(panelSnippet);
            Controls.Add(panelSpelling);
            Controls.Add(panelUpdates);
            Controls.Add(panelCompiler);
            IconName = "Settings";
            Location = new System.Drawing.Point(0, 0);
            Name = "SettingsForm";
            Text = "CodeLab Settings";
            panelUpdates.ResumeLayout(false);
            panelUpdates.PerformLayout();
            panelCompiler.ResumeLayout(false);
            panelCompiler.PerformLayout();
            panelUI.ResumeLayout(false);
            panelUI.PerformLayout();
            panelSpelling.ResumeLayout(false);
            panelSpelling.PerformLayout();
            panelRenderOptions.ResumeLayout(false);
            panelRenderOptions.PerformLayout();
            panelRenderOptionsSub.ResumeLayout(false);
            panelRenderOptionsSub.PerformLayout();
            ResumeLayout(false);
        }

        #endregion
        private System.Windows.Forms.Button closeButton;
        private System.Windows.Forms.Panel panelUpdates;
        private System.Windows.Forms.CheckBox checkForUpdates;
        private System.Windows.Forms.Button checkNowButton;
        private System.Windows.Forms.ListBox settingsList;
        private System.Windows.Forms.Panel panelUI;
        private System.Windows.Forms.ComboBox themeCombobox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox largeFontCheckbox;
        private System.Windows.Forms.ComboBox fontCombobox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox showWhiteSpaceCheckbox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox wordWrapCheckbox;
        private System.Windows.Forms.CheckBox indicatorMapCheckbox;
        private System.Windows.Forms.CheckBox codeFoldingCheckbox;
        private System.Windows.Forms.CheckBox bookMarksCheckbox;
        private System.Windows.Forms.CheckBox lineNumbersCheckbox;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox wordWrapTextFilesCheckbox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Panel panelCompiler;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ComboBox warningLevelCombobox;
        private System.Windows.Forms.CheckBox toolbarCheckbox;
        private SnippetManager panelSnippet;
        private System.Windows.Forms.Label indentSpacesLabel;
        private System.Windows.Forms.ComboBox indentSpacesComboBox;
        private System.Windows.Forms.Button removeWarningButton;
        private System.Windows.Forms.Label warningToIgnoreLabel;
        private System.Windows.Forms.ListBox warningsToIgnoreList;
        private System.Windows.Forms.Button lookupWarningButton;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.CheckBox caretLineFrameCheckBox;
        private System.Windows.Forms.CheckBox extendedColorsCheckBox;
        private System.Windows.Forms.Panel panelSpelling;
        private System.Windows.Forms.CheckBox enableSpellcheckCheckBox;
        private System.Windows.Forms.Label addWordsToIgnoreLabel;
        private System.Windows.Forms.Label wordsToIgnoreLabel;
        private System.Windows.Forms.ListBox wordsToIgnoreListBox;
        private System.Windows.Forms.Button removeIgnoreWordButton;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label spellcheckOptionsLabel;
        private System.Windows.Forms.ComboBox spellLangComboBox;
        private System.Windows.Forms.Button addLangsButton;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.CheckBox disableAutoCompCheckBox;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Panel panelRenderOptions;
        private System.Windows.Forms.Label presetLabel;
        private System.Windows.Forms.Panel panelRenderOptionsSub;
        private System.Windows.Forms.Label flagsLabel;
        private System.Windows.Forms.Label scheduleLabel;
        private System.Windows.Forms.RadioButton horizontalStripsRadioButton;
        private System.Windows.Forms.RadioButton squareTilesRadioButton;
        private System.Windows.Forms.CheckBox aliasedSelectionCheckBox;
        private System.Windows.Forms.RadioButton noneRadioButton;
        private System.Windows.Forms.CheckBox noClipCheckBox;
        private System.Windows.Forms.CheckBox singleThreadedCheckBox;
        private System.Windows.Forms.ComboBox presetComboBox;
    }
}