﻿namespace PaintDotNet.Effects
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
            this.closeButton = new System.Windows.Forms.Button();
            this.updatesPanel = new System.Windows.Forms.Panel();
            this.checkNowButton = new System.Windows.Forms.Button();
            this.checkForUpdates = new System.Windows.Forms.CheckBox();
            this.compilerPanel = new System.Windows.Forms.Panel();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.warningLevelCombobox = new System.Windows.Forms.ComboBox();
            this.uiPanel = new System.Windows.Forms.Panel();
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
            this.updatesPanel.SuspendLayout();
            this.compilerPanel.SuspendLayout();
            this.uiPanel.SuspendLayout();
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
            // updatesPanel
            // 
            this.updatesPanel.Controls.Add(this.checkNowButton);
            this.updatesPanel.Controls.Add(this.checkForUpdates);
            this.updatesPanel.Location = new System.Drawing.Point(202, 12);
            this.updatesPanel.Name = "updatesPanel";
            this.updatesPanel.Size = new System.Drawing.Size(476, 405);
            this.updatesPanel.TabIndex = 2;
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
            // compilerPanel
            // 
            this.compilerPanel.Controls.Add(this.label9);
            this.compilerPanel.Controls.Add(this.label8);
            this.compilerPanel.Controls.Add(this.warningLevelCombobox);
            this.compilerPanel.Location = new System.Drawing.Point(202, 12);
            this.compilerPanel.Name = "compilerPanel";
            this.compilerPanel.Size = new System.Drawing.Size(476, 405);
            this.compilerPanel.TabIndex = 0;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(44, 50);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(82, 15);
            this.label9.TabIndex = 1;
            this.label9.Text = "Warning level:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.label8.Location = new System.Drawing.Point(23, 23);
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
            this.warningLevelCombobox.Location = new System.Drawing.Point(132, 45);
            this.warningLevelCombobox.Name = "warningLevelCombobox";
            this.warningLevelCombobox.Size = new System.Drawing.Size(76, 23);
            this.warningLevelCombobox.TabIndex = 2;
            this.warningLevelCombobox.SelectedIndexChanged += new System.EventHandler(this.warningLevelCombobox_SelectedIndexChanged);
            // 
            // uiPanel
            // 
            this.uiPanel.Controls.Add(this.toolbarCheckbox);
            this.uiPanel.Controls.Add(this.label5);
            this.uiPanel.Controls.Add(this.linkLabel1);
            this.uiPanel.Controls.Add(this.label4);
            this.uiPanel.Controls.Add(this.wordWrapTextFilesCheckbox);
            this.uiPanel.Controls.Add(this.themeCombobox);
            this.uiPanel.Controls.Add(this.label3);
            this.uiPanel.Controls.Add(this.largeFontCheckbox);
            this.uiPanel.Controls.Add(this.fontCombobox);
            this.uiPanel.Controls.Add(this.label2);
            this.uiPanel.Controls.Add(this.showWhiteSpaceCheckbox);
            this.uiPanel.Controls.Add(this.label1);
            this.uiPanel.Controls.Add(this.wordWrapCheckbox);
            this.uiPanel.Controls.Add(this.indicatorMapCheckbox);
            this.uiPanel.Controls.Add(this.codeFoldingCheckbox);
            this.uiPanel.Controls.Add(this.bookMarksCheckbox);
            this.uiPanel.Controls.Add(this.lineNumbersCheckbox);
            this.uiPanel.Location = new System.Drawing.Point(202, 12);
            this.uiPanel.Name = "uiPanel";
            this.uiPanel.Size = new System.Drawing.Size(476, 405);
            this.uiPanel.TabIndex = 1;
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
            this.label5.Location = new System.Drawing.Point(228, 369);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(190, 15);
            this.label5.TabIndex = 15;
            this.label5.Text = "\"Auto\" matches Paint.NET\'s theme";
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Location = new System.Drawing.Point(234, 298);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(88, 15);
            this.linkLabel1.TabIndex = 13;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "Help with fonts";
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(225, 272);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(177, 15);
            this.label4.TabIndex = 11;
            this.label4.Text = "* = not available on your system";
            // 
            // wordWrapTextFilesCheckbox
            // 
            this.wordWrapTextFilesCheckbox.AutoSize = true;
            this.wordWrapTextFilesCheckbox.Location = new System.Drawing.Point(32, 182);
            this.wordWrapTextFilesCheckbox.Name = "wordWrapTextFilesCheckbox";
            this.wordWrapTextFilesCheckbox.Size = new System.Drawing.Size(150, 19);
            this.wordWrapTextFilesCheckbox.TabIndex = 7;
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
            this.themeCombobox.Location = new System.Drawing.Point(32, 364);
            this.themeCombobox.Name = "themeCombobox";
            this.themeCombobox.Size = new System.Drawing.Size(166, 23);
            this.themeCombobox.TabIndex = 14;
            this.themeCombobox.SelectedIndexChanged += new System.EventHandler(this.themeCombobox_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.label3.Location = new System.Drawing.Point(13, 345);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(49, 15);
            this.label3.TabIndex = 14;
            this.label3.Text = "Theme:";
            // 
            // largeFontCheckbox
            // 
            this.largeFontCheckbox.AutoSize = true;
            this.largeFontCheckbox.Location = new System.Drawing.Point(32, 297);
            this.largeFontCheckbox.Name = "largeFontCheckbox";
            this.largeFontCheckbox.Size = new System.Drawing.Size(85, 19);
            this.largeFontCheckbox.TabIndex = 12;
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
            this.fontCombobox.Location = new System.Drawing.Point(32, 267);
            this.fontCombobox.Name = "fontCombobox";
            this.fontCombobox.Size = new System.Drawing.Size(166, 23);
            this.fontCombobox.TabIndex = 10;
            this.fontCombobox.SelectedIndexChanged += new System.EventHandler(this.fontCombobox_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.label2.Location = new System.Drawing.Point(13, 248);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(40, 15);
            this.label2.TabIndex = 9;
            this.label2.Text = "Fonts:";
            // 
            // showWhiteSpaceCheckbox
            // 
            this.showWhiteSpaceCheckbox.AutoSize = true;
            this.showWhiteSpaceCheckbox.Location = new System.Drawing.Point(32, 207);
            this.showWhiteSpaceCheckbox.Name = "showWhiteSpaceCheckbox";
            this.showWhiteSpaceCheckbox.Size = new System.Drawing.Size(129, 19);
            this.showWhiteSpaceCheckbox.TabIndex = 8;
            this.showWhiteSpaceCheckbox.Text = "Show whitespace ···";
            this.showWhiteSpaceCheckbox.UseVisualStyleBackColor = true;
            this.showWhiteSpaceCheckbox.CheckedChanged += new System.EventHandler(this.showWhiteSpaceCheckbox_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.label1.Location = new System.Drawing.Point(13, 4);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(116, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "Editor functionality:";
            // 
            // wordWrapCheckbox
            // 
            this.wordWrapCheckbox.AutoSize = true;
            this.wordWrapCheckbox.Location = new System.Drawing.Point(32, 156);
            this.wordWrapCheckbox.Name = "wordWrapCheckbox";
            this.wordWrapCheckbox.Size = new System.Drawing.Size(155, 19);
            this.wordWrapCheckbox.TabIndex = 6;
            this.wordWrapCheckbox.Text = "Word wrap C# code files";
            this.wordWrapCheckbox.UseVisualStyleBackColor = true;
            this.wordWrapCheckbox.CheckedChanged += new System.EventHandler(this.wordWrapCheckbox_CheckedChanged);
            // 
            // indicatorMapCheckbox
            // 
            this.indicatorMapCheckbox.AutoSize = true;
            this.indicatorMapCheckbox.Location = new System.Drawing.Point(32, 130);
            this.indicatorMapCheckbox.Name = "indicatorMapCheckbox";
            this.indicatorMapCheckbox.Size = new System.Drawing.Size(156, 19);
            this.indicatorMapCheckbox.TabIndex = 5;
            this.indicatorMapCheckbox.Text = "Indicator map (scrollbar)";
            this.indicatorMapCheckbox.UseVisualStyleBackColor = true;
            this.indicatorMapCheckbox.CheckedChanged += new System.EventHandler(this.indicatorMapCheckbox_CheckedChanged);
            // 
            // codeFoldingCheckbox
            // 
            this.codeFoldingCheckbox.AutoSize = true;
            this.codeFoldingCheckbox.Location = new System.Drawing.Point(32, 104);
            this.codeFoldingCheckbox.Name = "codeFoldingCheckbox";
            this.codeFoldingCheckbox.Size = new System.Drawing.Size(114, 19);
            this.codeFoldingCheckbox.TabIndex = 4;
            this.codeFoldingCheckbox.Text = "Code folding [+]";
            this.codeFoldingCheckbox.UseVisualStyleBackColor = true;
            this.codeFoldingCheckbox.CheckedChanged += new System.EventHandler(this.codeFoldingCheckbox_CheckedChanged);
            // 
            // bookMarksCheckbox
            // 
            this.bookMarksCheckbox.AutoSize = true;
            this.bookMarksCheckbox.Location = new System.Drawing.Point(32, 78);
            this.bookMarksCheckbox.Name = "bookMarksCheckbox";
            this.bookMarksCheckbox.Size = new System.Drawing.Size(101, 19);
            this.bookMarksCheckbox.TabIndex = 3;
            this.bookMarksCheckbox.Text = "Book marks ■";
            this.bookMarksCheckbox.UseVisualStyleBackColor = true;
            this.bookMarksCheckbox.CheckedChanged += new System.EventHandler(this.bookMarksCheckbox_CheckedChanged);
            // 
            // lineNumbersCheckbox
            // 
            this.lineNumbersCheckbox.AutoSize = true;
            this.lineNumbersCheckbox.Location = new System.Drawing.Point(32, 52);
            this.lineNumbersCheckbox.Name = "lineNumbersCheckbox";
            this.lineNumbersCheckbox.Size = new System.Drawing.Size(98, 19);
            this.lineNumbersCheckbox.TabIndex = 2;
            this.lineNumbersCheckbox.Text = "Line numbers";
            this.lineNumbersCheckbox.UseVisualStyleBackColor = true;
            this.lineNumbersCheckbox.CheckedChanged += new System.EventHandler(this.lineNumbersCheckbox_CheckedChanged);
            // 
            // settingsList
            // 
            this.settingsList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.settingsList.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.settingsList.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.settingsList.FormattingEnabled = true;
            this.settingsList.IntegralHeight = false;
            this.settingsList.ItemHeight = 32;
            this.settingsList.Items.AddRange(new object[] {
            "User Interface",
            "Compiler",
            "Updates"});
            this.settingsList.Location = new System.Drawing.Point(12, 12);
            this.settingsList.Name = "settingsList";
            this.settingsList.Size = new System.Drawing.Size(184, 406);
            this.settingsList.TabIndex = 0;
            this.settingsList.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.settingsList_DrawItem);
            this.settingsList.SelectedIndexChanged += new System.EventHandler(this.settingsList_SelectedIndexChanged);
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.CancelButton = this.closeButton;
            this.ClientSize = new System.Drawing.Size(691, 468);
            this.Controls.Add(this.compilerPanel);
            this.Controls.Add(this.updatesPanel);
            this.Controls.Add(this.uiPanel);
            this.Controls.Add(this.settingsList);
            this.Controls.Add(this.closeButton);
            this.IconName = "Settings";
            this.Name = "SettingsForm";
            this.Text = "CodeLab Settings";
            this.updatesPanel.ResumeLayout(false);
            this.updatesPanel.PerformLayout();
            this.compilerPanel.ResumeLayout(false);
            this.compilerPanel.PerformLayout();
            this.uiPanel.ResumeLayout(false);
            this.uiPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button closeButton;
        private System.Windows.Forms.Panel updatesPanel;
        private System.Windows.Forms.CheckBox checkForUpdates;
        private System.Windows.Forms.Button checkNowButton;
        private System.Windows.Forms.ListBox settingsList;
        private System.Windows.Forms.Panel uiPanel;
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
        private System.Windows.Forms.Panel compilerPanel;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ComboBox warningLevelCombobox;
        private System.Windows.Forms.CheckBox toolbarCheckbox;
    }
}