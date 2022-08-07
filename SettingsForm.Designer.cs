namespace PaintDotNet.Effects
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
            this.closeButton = new System.Windows.Forms.Button();
            this.panelUpdates = new System.Windows.Forms.Panel();
            this.checkNowButton = new System.Windows.Forms.Button();
            this.checkForUpdates = new System.Windows.Forms.CheckBox();
            this.panelCompiler = new System.Windows.Forms.Panel();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.lookupWarningButton = new System.Windows.Forms.Button();
            this.warningToIgnoreLabel = new System.Windows.Forms.Label();
            this.warningsToIgnoreList = new System.Windows.Forms.ListBox();
            this.removeWarningButton = new System.Windows.Forms.Button();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.warningLevelCombobox = new System.Windows.Forms.ComboBox();
            this.panelUI = new System.Windows.Forms.Panel();
            this.disableAutoCompCheckBox = new System.Windows.Forms.CheckBox();
            this.extendedColorsCheckBox = new System.Windows.Forms.CheckBox();
            this.caretLineFrameCheckBox = new System.Windows.Forms.CheckBox();
            this.indentSpacesLabel = new System.Windows.Forms.Label();
            this.indentSpacesComboBox = new System.Windows.Forms.ComboBox();
            this.toolbarCheckbox = new System.Windows.Forms.CheckBox();
            this.label5 = new System.Windows.Forms.Label();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.label4 = new System.Windows.Forms.Label();
            this.wordWrapTextFilesCheckbox = new System.Windows.Forms.CheckBox();
            this.themeCombobox = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.largeFontCheckbox = new System.Windows.Forms.CheckBox();
            this.fontCombobox = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.showWhiteSpaceCheckbox = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.wordWrapCheckbox = new System.Windows.Forms.CheckBox();
            this.indicatorMapCheckbox = new System.Windows.Forms.CheckBox();
            this.codeFoldingCheckbox = new System.Windows.Forms.CheckBox();
            this.bookMarksCheckbox = new System.Windows.Forms.CheckBox();
            this.lineNumbersCheckbox = new System.Windows.Forms.CheckBox();
            this.settingsList = new System.Windows.Forms.ListBox();
            this.panelSnippet = new PaintDotNet.Effects.SnippetManager();
            this.panelSpelling = new System.Windows.Forms.Panel();
            this.addLangsButton = new System.Windows.Forms.Button();
            this.enableSpellcheckCheckBox = new System.Windows.Forms.CheckBox();
            this.addWordsToIgnoreLabel = new System.Windows.Forms.Label();
            this.wordsToIgnoreLabel = new System.Windows.Forms.Label();
            this.wordsToIgnoreListBox = new System.Windows.Forms.ListBox();
            this.removeIgnoreWordButton = new System.Windows.Forms.Button();
            this.label11 = new System.Windows.Forms.Label();
            this.spellcheckOptionsLabel = new System.Windows.Forms.Label();
            this.spellLangComboBox = new System.Windows.Forms.ComboBox();
            this.panelUpdates.SuspendLayout();
            this.panelCompiler.SuspendLayout();
            this.panelUI.SuspendLayout();
            this.panelSpelling.SuspendLayout();
            this.SuspendLayout();
            // 
            // closeButton
            // 
            this.closeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.closeButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.closeButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.closeButton.Location = new System.Drawing.Point(604, 433);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(75, 23);
            this.closeButton.TabIndex = 3;
            this.closeButton.Text = "Close";
            this.closeButton.UseVisualStyleBackColor = true;
            // 
            // panelUpdates
            // 
            this.panelUpdates.Controls.Add(this.checkNowButton);
            this.panelUpdates.Controls.Add(this.checkForUpdates);
            this.panelUpdates.Location = new System.Drawing.Point(202, 12);
            this.panelUpdates.Name = "panelUpdates";
            this.panelUpdates.Size = new System.Drawing.Size(476, 405);
            this.panelUpdates.TabIndex = 2;
            // 
            // checkNowButton
            // 
            this.checkNowButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.checkNowButton.Location = new System.Drawing.Point(16, 53);
            this.checkNowButton.Name = "checkNowButton";
            this.checkNowButton.Size = new System.Drawing.Size(88, 23);
            this.checkNowButton.TabIndex = 1;
            this.checkNowButton.Text = "Check Now";
            this.checkNowButton.UseVisualStyleBackColor = true;
            this.checkNowButton.Click += new System.EventHandler(this.checkNowButton_Click);
            // 
            // checkForUpdates
            // 
            this.checkForUpdates.AutoSize = true;
            this.checkForUpdates.Location = new System.Drawing.Point(16, 17);
            this.checkForUpdates.Name = "checkForUpdates";
            this.checkForUpdates.Size = new System.Drawing.Size(310, 19);
            this.checkForUpdates.TabIndex = 0;
            this.checkForUpdates.Text = "Automatically check for updates when CodeLab starts";
            this.checkForUpdates.UseVisualStyleBackColor = true;
            this.checkForUpdates.CheckedChanged += new System.EventHandler(this.checkForUpdates_CheckedChanged);
            // 
            // panelCompiler
            // 
            this.panelCompiler.Controls.Add(this.label7);
            this.panelCompiler.Controls.Add(this.label6);
            this.panelCompiler.Controls.Add(this.lookupWarningButton);
            this.panelCompiler.Controls.Add(this.warningToIgnoreLabel);
            this.panelCompiler.Controls.Add(this.warningsToIgnoreList);
            this.panelCompiler.Controls.Add(this.removeWarningButton);
            this.panelCompiler.Controls.Add(this.label9);
            this.panelCompiler.Controls.Add(this.label8);
            this.panelCompiler.Controls.Add(this.warningLevelCombobox);
            this.panelCompiler.Location = new System.Drawing.Point(202, 12);
            this.panelCompiler.Name = "panelCompiler";
            this.panelCompiler.Size = new System.Drawing.Size(476, 405);
            this.panelCompiler.TabIndex = 0;
            // 
            // label7
            // 
            this.label7.Location = new System.Drawing.Point(141, 44);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(323, 99);
            this.label7.TabIndex = 8;
            this.label7.Text = resources.GetString("label7.Text");
            // 
            // label6
            // 
            this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label6.Location = new System.Drawing.Point(133, 247);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(313, 75);
            this.label6.TabIndex = 7;
            this.label6.Text = "To add a specific warning to the list of warnings to ignore, when you see a warni" +
    "ng in the error\'s list below the code window, right-click on the warning and cho" +
    "ose \"Ignore this Warning\".";
            // 
            // lookupWarningButton
            // 
            this.lookupWarningButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.lookupWarningButton.Location = new System.Drawing.Point(133, 211);
            this.lookupWarningButton.Name = "lookupWarningButton";
            this.lookupWarningButton.Size = new System.Drawing.Size(108, 25);
            this.lookupWarningButton.TabIndex = 6;
            this.lookupWarningButton.Text = "Lookup Warning";
            this.lookupWarningButton.UseVisualStyleBackColor = true;
            this.lookupWarningButton.Click += new System.EventHandler(this.lookupWarningButton_Click);
            // 
            // warningToIgnoreLabel
            // 
            this.warningToIgnoreLabel.AutoSize = true;
            this.warningToIgnoreLabel.Location = new System.Drawing.Point(35, 155);
            this.warningToIgnoreLabel.Name = "warningToIgnoreLabel";
            this.warningToIgnoreLabel.Size = new System.Drawing.Size(111, 15);
            this.warningToIgnoreLabel.TabIndex = 3;
            this.warningToIgnoreLabel.Text = "Warnings to ignore:";
            // 
            // warningsToIgnoreList
            // 
            this.warningsToIgnoreList.FormattingEnabled = true;
            this.warningsToIgnoreList.ItemHeight = 15;
            this.warningsToIgnoreList.Location = new System.Drawing.Point(48, 180);
            this.warningsToIgnoreList.Name = "warningsToIgnoreList";
            this.warningsToIgnoreList.Size = new System.Drawing.Size(75, 139);
            this.warningsToIgnoreList.TabIndex = 4;
            // 
            // removeWarningButton
            // 
            this.removeWarningButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.removeWarningButton.Location = new System.Drawing.Point(133, 180);
            this.removeWarningButton.Name = "removeWarningButton";
            this.removeWarningButton.Size = new System.Drawing.Size(108, 25);
            this.removeWarningButton.TabIndex = 5;
            this.removeWarningButton.Text = "Remove Warning";
            this.removeWarningButton.UseVisualStyleBackColor = true;
            this.removeWarningButton.Click += new System.EventHandler(this.removeWarningButton_Click);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(35, 44);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(82, 15);
            this.label9.TabIndex = 1;
            this.label9.Text = "Warning level:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label8.Location = new System.Drawing.Point(14, 17);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(120, 15);
            this.label8.TabIndex = 0;
            this.label8.Text = "C# compiler options:";
            // 
            // warningLevelCombobox
            // 
            this.warningLevelCombobox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.warningLevelCombobox.FormattingEnabled = true;
            this.warningLevelCombobox.Items.AddRange(new object[] {
            "0",
            "1",
            "2",
            "3",
            "4"});
            this.warningLevelCombobox.Location = new System.Drawing.Point(48, 68);
            this.warningLevelCombobox.Name = "warningLevelCombobox";
            this.warningLevelCombobox.Size = new System.Drawing.Size(75, 23);
            this.warningLevelCombobox.TabIndex = 2;
            this.warningLevelCombobox.SelectedIndexChanged += new System.EventHandler(this.warningLevelCombobox_SelectedIndexChanged);
            // 
            // panelUI
            // 
            this.panelUI.Controls.Add(this.disableAutoCompCheckBox);
            this.panelUI.Controls.Add(this.extendedColorsCheckBox);
            this.panelUI.Controls.Add(this.caretLineFrameCheckBox);
            this.panelUI.Controls.Add(this.indentSpacesLabel);
            this.panelUI.Controls.Add(this.indentSpacesComboBox);
            this.panelUI.Controls.Add(this.toolbarCheckbox);
            this.panelUI.Controls.Add(this.label5);
            this.panelUI.Controls.Add(this.linkLabel1);
            this.panelUI.Controls.Add(this.label4);
            this.panelUI.Controls.Add(this.wordWrapTextFilesCheckbox);
            this.panelUI.Controls.Add(this.themeCombobox);
            this.panelUI.Controls.Add(this.label3);
            this.panelUI.Controls.Add(this.largeFontCheckbox);
            this.panelUI.Controls.Add(this.fontCombobox);
            this.panelUI.Controls.Add(this.label2);
            this.panelUI.Controls.Add(this.showWhiteSpaceCheckbox);
            this.panelUI.Controls.Add(this.label1);
            this.panelUI.Controls.Add(this.wordWrapCheckbox);
            this.panelUI.Controls.Add(this.indicatorMapCheckbox);
            this.panelUI.Controls.Add(this.codeFoldingCheckbox);
            this.panelUI.Controls.Add(this.bookMarksCheckbox);
            this.panelUI.Controls.Add(this.lineNumbersCheckbox);
            this.panelUI.Location = new System.Drawing.Point(202, 12);
            this.panelUI.Name = "panelUI";
            this.panelUI.Size = new System.Drawing.Size(476, 405);
            this.panelUI.TabIndex = 1;
            // 
            // disableAutoCompCheckBox
            // 
            this.disableAutoCompCheckBox.AutoSize = true;
            this.disableAutoCompCheckBox.Location = new System.Drawing.Point(32, 131);
            this.disableAutoCompCheckBox.Name = "disableAutoCompCheckBox";
            this.disableAutoCompCheckBox.Size = new System.Drawing.Size(148, 19);
            this.disableAutoCompCheckBox.TabIndex = 20;
            this.disableAutoCompCheckBox.Text = "Disable Auto Complete";
            this.disableAutoCompCheckBox.UseVisualStyleBackColor = true;
            this.disableAutoCompCheckBox.CheckedChanged += new System.EventHandler(this.disableAutoCompCheckBox_CheckedChanged);
            // 
            // extendedColorsCheckBox
            // 
            this.extendedColorsCheckBox.AutoSize = true;
            this.extendedColorsCheckBox.Location = new System.Drawing.Point(32, 378);
            this.extendedColorsCheckBox.Name = "extendedColorsCheckBox";
            this.extendedColorsCheckBox.Size = new System.Drawing.Size(192, 19);
            this.extendedColorsCheckBox.TabIndex = 19;
            this.extendedColorsCheckBox.Text = "Extended Colors (Experimental)";
            this.extendedColorsCheckBox.UseVisualStyleBackColor = true;
            this.extendedColorsCheckBox.CheckedChanged += new System.EventHandler(this.extendedColorsCheckBox_CheckedChanged);
            // 
            // caretLineFrameCheckBox
            // 
            this.caretLineFrameCheckBox.AutoSize = true;
            this.caretLineFrameCheckBox.Location = new System.Drawing.Point(237, 105);
            this.caretLineFrameCheckBox.Name = "caretLineFrameCheckBox";
            this.caretLineFrameCheckBox.Size = new System.Drawing.Size(122, 19);
            this.caretLineFrameCheckBox.TabIndex = 8;
            this.caretLineFrameCheckBox.Text = "Current line frame";
            this.caretLineFrameCheckBox.UseVisualStyleBackColor = true;
            this.caretLineFrameCheckBox.CheckedChanged += new System.EventHandler(this.caretLineFrameCheckBox_CheckedChanged);
            // 
            // indentSpacesLabel
            // 
            this.indentSpacesLabel.AutoSize = true;
            this.indentSpacesLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.indentSpacesLabel.Location = new System.Drawing.Point(13, 160);
            this.indentSpacesLabel.Name = "indentSpacesLabel";
            this.indentSpacesLabel.Size = new System.Drawing.Size(88, 15);
            this.indentSpacesLabel.TabIndex = 9;
            this.indentSpacesLabel.Text = "Indent Spaces:";
            // 
            // indentSpacesComboBox
            // 
            this.indentSpacesComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.indentSpacesComboBox.FormattingEnabled = true;
            this.indentSpacesComboBox.Items.AddRange(new object[] {
            "2",
            "4"});
            this.indentSpacesComboBox.Location = new System.Drawing.Point(32, 186);
            this.indentSpacesComboBox.Name = "indentSpacesComboBox";
            this.indentSpacesComboBox.Size = new System.Drawing.Size(50, 23);
            this.indentSpacesComboBox.TabIndex = 10;
            this.indentSpacesComboBox.SelectedIndexChanged += new System.EventHandler(this.indentSpacesComboBox_SelectedIndexChanged);
            // 
            // toolbarCheckbox
            // 
            this.toolbarCheckbox.AutoSize = true;
            this.toolbarCheckbox.Location = new System.Drawing.Point(32, 28);
            this.toolbarCheckbox.Name = "toolbarCheckbox";
            this.toolbarCheckbox.Size = new System.Drawing.Size(65, 19);
            this.toolbarCheckbox.TabIndex = 1;
            this.toolbarCheckbox.Text = "Toolbar";
            this.toolbarCheckbox.UseVisualStyleBackColor = true;
            this.toolbarCheckbox.CheckedChanged += new System.EventHandler(this.toolbarCheckbox_CheckedChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(228, 354);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(190, 15);
            this.label5.TabIndex = 18;
            this.label5.Text = "\"Auto\" matches Paint.NET\'s theme";
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Location = new System.Drawing.Point(234, 301);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(88, 15);
            this.linkLabel1.TabIndex = 15;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "Help with fonts";
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(225, 275);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(177, 15);
            this.label4.TabIndex = 13;
            this.label4.Text = "* = not available on your system";
            // 
            // wordWrapTextFilesCheckbox
            // 
            this.wordWrapTextFilesCheckbox.AutoSize = true;
            this.wordWrapTextFilesCheckbox.Location = new System.Drawing.Point(237, 80);
            this.wordWrapTextFilesCheckbox.Name = "wordWrapTextFilesCheckbox";
            this.wordWrapTextFilesCheckbox.Size = new System.Drawing.Size(150, 19);
            this.wordWrapTextFilesCheckbox.TabIndex = 6;
            this.wordWrapTextFilesCheckbox.Text = "Word wrap text files  ‹‒\'";
            this.wordWrapTextFilesCheckbox.UseVisualStyleBackColor = true;
            this.wordWrapTextFilesCheckbox.CheckedChanged += new System.EventHandler(this.wordWrapTextFilesCheckbox_CheckedChanged);
            // 
            // themeCombobox
            // 
            this.themeCombobox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.themeCombobox.FormattingEnabled = true;
            this.themeCombobox.Items.AddRange(new object[] {
            "Auto",
            "Light",
            "Dark"});
            this.themeCombobox.Location = new System.Drawing.Point(32, 349);
            this.themeCombobox.Name = "themeCombobox";
            this.themeCombobox.Size = new System.Drawing.Size(166, 23);
            this.themeCombobox.TabIndex = 17;
            this.themeCombobox.SelectedIndexChanged += new System.EventHandler(this.themeCombobox_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label3.Location = new System.Drawing.Point(13, 330);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(49, 15);
            this.label3.TabIndex = 16;
            this.label3.Text = "Theme:";
            // 
            // largeFontCheckbox
            // 
            this.largeFontCheckbox.AutoSize = true;
            this.largeFontCheckbox.Location = new System.Drawing.Point(32, 300);
            this.largeFontCheckbox.Name = "largeFontCheckbox";
            this.largeFontCheckbox.Size = new System.Drawing.Size(85, 19);
            this.largeFontCheckbox.TabIndex = 14;
            this.largeFontCheckbox.Text = "Large fonts";
            this.largeFontCheckbox.UseVisualStyleBackColor = true;
            this.largeFontCheckbox.CheckedChanged += new System.EventHandler(this.largeFontCheckbox_CheckedChanged);
            // 
            // fontCombobox
            // 
            this.fontCombobox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.fontCombobox.FormattingEnabled = true;
            this.fontCombobox.Items.AddRange(new object[] {
            "Cascadia Code",
            "Consolas",
            "Courier New",
            "Envy Code R",
            "Fira Code",
            "Hack",
            "JetBrains Mono",
            "Verdana"});
            this.fontCombobox.Location = new System.Drawing.Point(32, 270);
            this.fontCombobox.Name = "fontCombobox";
            this.fontCombobox.Size = new System.Drawing.Size(166, 23);
            this.fontCombobox.TabIndex = 12;
            this.fontCombobox.SelectedIndexChanged += new System.EventHandler(this.fontCombobox_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label2.Location = new System.Drawing.Point(13, 251);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(40, 15);
            this.label2.TabIndex = 11;
            this.label2.Text = "Fonts:";
            // 
            // showWhiteSpaceCheckbox
            // 
            this.showWhiteSpaceCheckbox.AutoSize = true;
            this.showWhiteSpaceCheckbox.Location = new System.Drawing.Point(32, 216);
            this.showWhiteSpaceCheckbox.Name = "showWhiteSpaceCheckbox";
            this.showWhiteSpaceCheckbox.Size = new System.Drawing.Size(129, 19);
            this.showWhiteSpaceCheckbox.TabIndex = 11;
            this.showWhiteSpaceCheckbox.Text = "Show whitespace ···";
            this.showWhiteSpaceCheckbox.UseVisualStyleBackColor = true;
            this.showWhiteSpaceCheckbox.CheckedChanged += new System.EventHandler(this.showWhiteSpaceCheckbox_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label1.Location = new System.Drawing.Point(13, 4);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(116, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "Editor functionality:";
            // 
            // wordWrapCheckbox
            // 
            this.wordWrapCheckbox.AutoSize = true;
            this.wordWrapCheckbox.Location = new System.Drawing.Point(237, 54);
            this.wordWrapCheckbox.Name = "wordWrapCheckbox";
            this.wordWrapCheckbox.Size = new System.Drawing.Size(156, 19);
            this.wordWrapCheckbox.TabIndex = 4;
            this.wordWrapCheckbox.Text = "Word wrap code files  ‹‒\'";
            this.wordWrapCheckbox.UseVisualStyleBackColor = true;
            this.wordWrapCheckbox.CheckedChanged += new System.EventHandler(this.wordWrapCheckbox_CheckedChanged);
            // 
            // indicatorMapCheckbox
            // 
            this.indicatorMapCheckbox.AutoSize = true;
            this.indicatorMapCheckbox.Location = new System.Drawing.Point(237, 28);
            this.indicatorMapCheckbox.Name = "indicatorMapCheckbox";
            this.indicatorMapCheckbox.Size = new System.Drawing.Size(156, 19);
            this.indicatorMapCheckbox.TabIndex = 2;
            this.indicatorMapCheckbox.Text = "Indicator map (scrollbar)";
            this.indicatorMapCheckbox.UseVisualStyleBackColor = true;
            this.indicatorMapCheckbox.CheckedChanged += new System.EventHandler(this.indicatorMapCheckbox_CheckedChanged);
            // 
            // codeFoldingCheckbox
            // 
            this.codeFoldingCheckbox.AutoSize = true;
            this.codeFoldingCheckbox.Location = new System.Drawing.Point(32, 105);
            this.codeFoldingCheckbox.Name = "codeFoldingCheckbox";
            this.codeFoldingCheckbox.Size = new System.Drawing.Size(114, 19);
            this.codeFoldingCheckbox.TabIndex = 7;
            this.codeFoldingCheckbox.Text = "Code folding [+]";
            this.codeFoldingCheckbox.UseVisualStyleBackColor = true;
            this.codeFoldingCheckbox.CheckedChanged += new System.EventHandler(this.codeFoldingCheckbox_CheckedChanged);
            // 
            // bookMarksCheckbox
            // 
            this.bookMarksCheckbox.AutoSize = true;
            this.bookMarksCheckbox.Location = new System.Drawing.Point(32, 79);
            this.bookMarksCheckbox.Name = "bookMarksCheckbox";
            this.bookMarksCheckbox.Size = new System.Drawing.Size(88, 19);
            this.bookMarksCheckbox.TabIndex = 5;
            this.bookMarksCheckbox.Text = "Book marks";
            this.bookMarksCheckbox.UseVisualStyleBackColor = true;
            this.bookMarksCheckbox.CheckedChanged += new System.EventHandler(this.bookMarksCheckbox_CheckedChanged);
            // 
            // lineNumbersCheckbox
            // 
            this.lineNumbersCheckbox.AutoSize = true;
            this.lineNumbersCheckbox.Location = new System.Drawing.Point(32, 53);
            this.lineNumbersCheckbox.Name = "lineNumbersCheckbox";
            this.lineNumbersCheckbox.Size = new System.Drawing.Size(98, 19);
            this.lineNumbersCheckbox.TabIndex = 3;
            this.lineNumbersCheckbox.Text = "Line numbers";
            this.lineNumbersCheckbox.UseVisualStyleBackColor = true;
            this.lineNumbersCheckbox.CheckedChanged += new System.EventHandler(this.lineNumbersCheckbox_CheckedChanged);
            // 
            // settingsList
            // 
            this.settingsList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.settingsList.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.settingsList.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.settingsList.FormattingEnabled = true;
            this.settingsList.IntegralHeight = false;
            this.settingsList.ItemHeight = 32;
            this.settingsList.Items.AddRange(new object[] {
            "User Interface",
            "Snippets",
            "Spellcheck",
            "Compiler",
            "Updates"});
            this.settingsList.Location = new System.Drawing.Point(12, 12);
            this.settingsList.Name = "settingsList";
            this.settingsList.Size = new System.Drawing.Size(184, 406);
            this.settingsList.TabIndex = 0;
            this.settingsList.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.settingsList_DrawItem);
            this.settingsList.SelectedIndexChanged += new System.EventHandler(this.settingsList_SelectedIndexChanged);
            // 
            // panelSnippet
            // 
            this.panelSnippet.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.panelSnippet.Location = new System.Drawing.Point(202, 12);
            this.panelSnippet.Name = "panelSnippet";
            this.panelSnippet.Size = new System.Drawing.Size(476, 405);
            this.panelSnippet.TabIndex = 1;
            // 
            // panelSpelling
            // 
            this.panelSpelling.Controls.Add(this.addLangsButton);
            this.panelSpelling.Controls.Add(this.enableSpellcheckCheckBox);
            this.panelSpelling.Controls.Add(this.addWordsToIgnoreLabel);
            this.panelSpelling.Controls.Add(this.wordsToIgnoreLabel);
            this.panelSpelling.Controls.Add(this.wordsToIgnoreListBox);
            this.panelSpelling.Controls.Add(this.removeIgnoreWordButton);
            this.panelSpelling.Controls.Add(this.label11);
            this.panelSpelling.Controls.Add(this.spellcheckOptionsLabel);
            this.panelSpelling.Controls.Add(this.spellLangComboBox);
            this.panelSpelling.Location = new System.Drawing.Point(202, 12);
            this.panelSpelling.Name = "panelSpelling";
            this.panelSpelling.Size = new System.Drawing.Size(476, 405);
            this.panelSpelling.TabIndex = 8;
            // 
            // addLangsButton
            // 
            this.addLangsButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.addLangsButton.Location = new System.Drawing.Point(157, 111);
            this.addLangsButton.Name = "addLangsButton";
            this.addLangsButton.Size = new System.Drawing.Size(118, 25);
            this.addLangsButton.TabIndex = 4;
            this.addLangsButton.Text = "Add Languages";
            this.addLangsButton.UseVisualStyleBackColor = true;
            this.addLangsButton.Click += new System.EventHandler(this.addLangsButton_Click);
            // 
            // enableSpellcheckCheckBox
            // 
            this.enableSpellcheckCheckBox.AutoSize = true;
            this.enableSpellcheckCheckBox.Location = new System.Drawing.Point(38, 52);
            this.enableSpellcheckCheckBox.Name = "enableSpellcheckCheckBox";
            this.enableSpellcheckCheckBox.Size = new System.Drawing.Size(120, 19);
            this.enableSpellcheckCheckBox.TabIndex = 1;
            this.enableSpellcheckCheckBox.Text = "Enable Spellcheck";
            this.enableSpellcheckCheckBox.UseVisualStyleBackColor = true;
            this.enableSpellcheckCheckBox.CheckedChanged += new System.EventHandler(this.enableSpellcheckCheckBox_CheckedChanged);
            // 
            // addWordsToIgnoreLabel
            // 
            this.addWordsToIgnoreLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.addWordsToIgnoreLabel.Location = new System.Drawing.Point(154, 208);
            this.addWordsToIgnoreLabel.Name = "addWordsToIgnoreLabel";
            this.addWordsToIgnoreLabel.Size = new System.Drawing.Size(310, 75);
            this.addWordsToIgnoreLabel.TabIndex = 8;
            this.addWordsToIgnoreLabel.Text = "To add a specific word to the list of words to ignore, when you see a red underli" +
    "ned word in the code/text window, hover over the word and choose \"Ignore Word\" f" +
    "rom the light bulb menu.";
            // 
            // wordsToIgnoreLabel
            // 
            this.wordsToIgnoreLabel.AutoSize = true;
            this.wordsToIgnoreLabel.Location = new System.Drawing.Point(35, 155);
            this.wordsToIgnoreLabel.Name = "wordsToIgnoreLabel";
            this.wordsToIgnoreLabel.Size = new System.Drawing.Size(95, 15);
            this.wordsToIgnoreLabel.TabIndex = 5;
            this.wordsToIgnoreLabel.Text = "Words to ignore:";
            // 
            // wordsToIgnoreListBox
            // 
            this.wordsToIgnoreListBox.FormattingEnabled = true;
            this.wordsToIgnoreListBox.ItemHeight = 15;
            this.wordsToIgnoreListBox.Location = new System.Drawing.Point(48, 180);
            this.wordsToIgnoreListBox.Name = "wordsToIgnoreListBox";
            this.wordsToIgnoreListBox.Size = new System.Drawing.Size(100, 139);
            this.wordsToIgnoreListBox.TabIndex = 6;
            // 
            // removeIgnoreWordButton
            // 
            this.removeIgnoreWordButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.removeIgnoreWordButton.Location = new System.Drawing.Point(157, 180);
            this.removeIgnoreWordButton.Name = "removeIgnoreWordButton";
            this.removeIgnoreWordButton.Size = new System.Drawing.Size(118, 24);
            this.removeIgnoreWordButton.TabIndex = 7;
            this.removeIgnoreWordButton.Text = "Remove Word";
            this.removeIgnoreWordButton.UseVisualStyleBackColor = true;
            this.removeIgnoreWordButton.Click += new System.EventHandler(this.removeIgnoreWordButton_Click);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(35, 88);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(62, 15);
            this.label11.TabIndex = 2;
            this.label11.Text = "Language:";
            // 
            // spellcheckOptionsLabel
            // 
            this.spellcheckOptionsLabel.AutoSize = true;
            this.spellcheckOptionsLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.spellcheckOptionsLabel.Location = new System.Drawing.Point(14, 17);
            this.spellcheckOptionsLabel.Name = "spellcheckOptionsLabel";
            this.spellcheckOptionsLabel.Size = new System.Drawing.Size(114, 15);
            this.spellcheckOptionsLabel.TabIndex = 0;
            this.spellcheckOptionsLabel.Text = "Spellcheck options:";
            // 
            // spellLangComboBox
            // 
            this.spellLangComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.spellLangComboBox.FormattingEnabled = true;
            this.spellLangComboBox.Location = new System.Drawing.Point(48, 112);
            this.spellLangComboBox.Name = "spellLangComboBox";
            this.spellLangComboBox.Size = new System.Drawing.Size(100, 23);
            this.spellLangComboBox.TabIndex = 3;
            this.spellLangComboBox.SelectedIndexChanged += new System.EventHandler(this.spellLangComboBox_SelectedIndexChanged);
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.CancelButton = this.closeButton;
            this.ClientSize = new System.Drawing.Size(691, 468);
            this.Controls.Add(this.settingsList);
            this.Controls.Add(this.closeButton);
            this.Controls.Add(this.panelUI);
            this.Controls.Add(this.panelSnippet);
            this.Controls.Add(this.panelSpelling);
            this.Controls.Add(this.panelUpdates);
            this.Controls.Add(this.panelCompiler);
            this.IconName = "Settings";
            this.Name = "SettingsForm";
            this.Text = "CodeLab Settings";
            this.panelUpdates.ResumeLayout(false);
            this.panelUpdates.PerformLayout();
            this.panelCompiler.ResumeLayout(false);
            this.panelCompiler.PerformLayout();
            this.panelUI.ResumeLayout(false);
            this.panelUI.PerformLayout();
            this.panelSpelling.ResumeLayout(false);
            this.panelSpelling.PerformLayout();
            this.ResumeLayout(false);

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
    }
}