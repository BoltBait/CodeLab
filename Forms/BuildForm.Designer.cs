namespace PdnCodeLab
{
    partial class BuildForm
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
            components = new System.ComponentModel.Container();
            ButtonCancel = new System.Windows.Forms.Button();
            ButtonSave = new System.Windows.Forms.Button();
            MenuName = new System.Windows.Forms.TextBox();
            SubMenuName = new System.Windows.Forms.ComboBox();
            label1 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            ButtonIcon = new System.Windows.Forms.LinkLabel();
            toolTip1 = new System.Windows.Forms.ToolTip(components);
            MajorVersion = new System.Windows.Forms.NumericUpDown();
            MinorVersion = new System.Windows.Forms.NumericUpDown();
            WindowTitleText = new System.Windows.Forms.TextBox();
            DescriptionBox = new System.Windows.Forms.TextBox();
            KeyWordsBox = new System.Windows.Forms.TextBox();
            ForceAliasSelectionBox = new System.Windows.Forms.CheckBox();
            PreviewHelpButton = new System.Windows.Forms.Button();
            ViewSourceButton = new System.Windows.Forms.Button();
            StraightAlphaBox = new System.Windows.Forms.CheckBox();
            WorkingSpaceColorContextBox = new System.Windows.Forms.CheckBox();
            EffectRadio = new System.Windows.Forms.RadioButton();
            AdjustmentRadio = new System.Windows.Forms.RadioButton();
            AuthorName = new System.Windows.Forms.TextBox();
            label16 = new System.Windows.Forms.Label();
            label17 = new System.Windows.Forms.Label();
            DecimalSymbol = new System.Windows.Forms.Label();
            SupportURL = new System.Windows.Forms.TextBox();
            label19 = new System.Windows.Forms.Label();
            label20 = new System.Windows.Forms.Label();
            label21 = new System.Windows.Forms.Label();
            label23 = new System.Windows.Forms.Label();
            label24 = new System.Windows.Forms.Label();
            label3 = new System.Windows.Forms.Label();
            label4 = new System.Windows.Forms.Label();
            label5 = new System.Windows.Forms.Label();
            label6 = new System.Windows.Forms.Label();
            label7 = new System.Windows.Forms.Label();
            ForceSingleThreadedBox = new System.Windows.Forms.CheckBox();
            label8 = new System.Windows.Forms.Label();
            label9 = new System.Windows.Forms.Label();
            radioButtonNone = new System.Windows.Forms.RadioButton();
            radioButtonURL = new System.Windows.Forms.RadioButton();
            radioButtonPlain = new System.Windows.Forms.RadioButton();
            radioButtonRich = new System.Windows.Forms.RadioButton();
            HelpURL = new System.Windows.Forms.TextBox();
            HelpPlainText = new System.Windows.Forms.TextBox();
            RichHelpContent = new System.Windows.Forms.RichTextBox();
            toolStrip1 = new System.Windows.Forms.ToolStrip();
            OpenButton = new ScaledToolStripButton();
            SaveButton = new ScaledToolStripButton();
            WordPadButton = new ScaledToolStripButton();
            toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            BoldButton = new ScaledToolStripButton();
            ItalicsButton = new ScaledToolStripButton();
            UnderlineButton = new ScaledToolStripButton();
            SuperScriptButton = new ScaledToolStripButton();
            SubScriptButton = new ScaledToolStripButton();
            LargeFontButton = new ScaledToolStripButton();
            SmallFontButton = new ScaledToolStripButton();
            ColorButton = new ScaledToolStripButton();
            toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            InsertImageButton = new ScaledToolStripButton();
            BulletButton = new ScaledToolStripButton();
            IndentButton = new ScaledToolStripButton();
            UnindentButton = new ScaledToolStripButton();
            ParagraphLeftButton = new ScaledToolStripButton();
            CenterButton = new ScaledToolStripButton();
            panel1 = new System.Windows.Forms.Panel();
            PreviewLabel = new System.Windows.Forms.Label();
            PlainTextLabel = new System.Windows.Forms.Label();
            WarningLabel = new System.Windows.Forms.Label();
            MenuIcon = new System.Windows.Forms.PictureBox();
            GenSlnButton = new System.Windows.Forms.Button();
            sampleImage = new System.Windows.Forms.PictureBox();
            sampleLabel = new System.Windows.Forms.Label();
            forceLegacyRoiBox = new System.Windows.Forms.CheckBox();
            forceSingleRenderBox = new System.Windows.Forms.CheckBox();
            NoSelectionClippingBox = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)MajorVersion).BeginInit();
            ((System.ComponentModel.ISupportInitialize)MinorVersion).BeginInit();
            toolStrip1.SuspendLayout();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)MenuIcon).BeginInit();
            ((System.ComponentModel.ISupportInitialize)sampleImage).BeginInit();
            SuspendLayout();
            // 
            // ButtonCancel
            // 
            ButtonCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            ButtonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            ButtonCancel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            ButtonCancel.Location = new System.Drawing.Point(635, 670);
            ButtonCancel.Name = "ButtonCancel";
            ButtonCancel.Size = new System.Drawing.Size(75, 24);
            ButtonCancel.TabIndex = 28;
            ButtonCancel.Text = "Cancel";
            ButtonCancel.UseVisualStyleBackColor = true;
            ButtonCancel.Click += ButtonCancel_Click;
            // 
            // ButtonSave
            // 
            ButtonSave.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            ButtonSave.FlatStyle = System.Windows.Forms.FlatStyle.System;
            ButtonSave.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            ButtonSave.Location = new System.Drawing.Point(554, 670);
            ButtonSave.Name = "ButtonSave";
            ButtonSave.Size = new System.Drawing.Size(75, 24);
            ButtonSave.TabIndex = 27;
            ButtonSave.Text = "Build";
            ButtonSave.UseVisualStyleBackColor = true;
            ButtonSave.Click += ButtonSave_Click;
            // 
            // MenuName
            // 
            MenuName.Location = new System.Drawing.Point(110, 100);
            MenuName.MaxLength = 50;
            MenuName.Name = "MenuName";
            MenuName.Size = new System.Drawing.Size(150, 23);
            MenuName.TabIndex = 2;
            toolTip1.SetToolTip(MenuName, "Enter a name for your effect\r\nas it will show up in Paint.NET");
            // 
            // SubMenuName
            // 
            SubMenuName.FormattingEnabled = true;
            SubMenuName.Items.AddRange(new object[] { "", "Advanced", "Artistic", "Blurs", "Color", "Distort", "Noise", "Object", "Photo", "Render", "Stylize" });
            SubMenuName.Location = new System.Drawing.Point(110, 63);
            SubMenuName.MaxDropDownItems = 12;
            SubMenuName.Name = "SubMenuName";
            SubMenuName.Size = new System.Drawing.Size(150, 23);
            SubMenuName.TabIndex = 1;
            toolTip1.SetToolTip(SubMenuName, "Select a submenu or leave blank for the main Effects menu.");
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(16, 67);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(61, 15);
            label1.TabIndex = 5;
            label1.Text = "Submenu:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(16, 104);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(65, 15);
            label2.TabIndex = 6;
            label2.Text = "Menu Text:";
            // 
            // ButtonIcon
            // 
            ButtonIcon.AutoSize = true;
            ButtonIcon.Location = new System.Drawing.Point(198, 123);
            ButtonIcon.Name = "ButtonIcon";
            ButtonIcon.Size = new System.Drawing.Size(64, 15);
            ButtonIcon.TabIndex = 3;
            ButtonIcon.TabStop = true;
            ButtonIcon.Text = "Select Icon";
            toolTip1.SetToolTip(ButtonIcon, "Select a 16x16 PNG file for display in the Effects menu.");
            ButtonIcon.LinkClicked += ButtonIcon_LinkClicked;
            // 
            // MajorVersion
            // 
            MajorVersion.Location = new System.Drawing.Point(133, 243);
            MajorVersion.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
            MajorVersion.Name = "MajorVersion";
            MajorVersion.Size = new System.Drawing.Size(50, 23);
            MajorVersion.TabIndex = 6;
            toolTip1.SetToolTip(MajorVersion, "Major Version");
            MajorVersion.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // MinorVersion
            // 
            MinorVersion.Location = new System.Drawing.Point(206, 243);
            MinorVersion.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
            MinorVersion.Name = "MinorVersion";
            MinorVersion.Size = new System.Drawing.Size(54, 23);
            MinorVersion.TabIndex = 7;
            toolTip1.SetToolTip(MinorVersion, "Minor Version");
            // 
            // WindowTitleText
            // 
            WindowTitleText.Location = new System.Drawing.Point(110, 146);
            WindowTitleText.MaxLength = 255;
            WindowTitleText.Name = "WindowTitleText";
            WindowTitleText.Size = new System.Drawing.Size(150, 23);
            WindowTitleText.TabIndex = 4;
            toolTip1.SetToolTip(WindowTitleText, "Enter a title for your effect UI window.");
            // 
            // DescriptionBox
            // 
            DescriptionBox.Location = new System.Drawing.Point(60, 277);
            DescriptionBox.MaxLength = 75;
            DescriptionBox.Name = "DescriptionBox";
            DescriptionBox.Size = new System.Drawing.Size(200, 23);
            DescriptionBox.TabIndex = 8;
            toolTip1.SetToolTip(DescriptionBox, "Describe your effect in 5 words or less.");
            // 
            // KeyWordsBox
            // 
            KeyWordsBox.Location = new System.Drawing.Point(90, 311);
            KeyWordsBox.MaxLength = 50;
            KeyWordsBox.Name = "KeyWordsBox";
            KeyWordsBox.Size = new System.Drawing.Size(170, 23);
            KeyWordsBox.TabIndex = 9;
            toolTip1.SetToolTip(KeyWordsBox, "If you wish your effect to be searchable via keywords,\r\nenter them separated by the pipe symbol.");
            // 
            // ForceAliasSelectionBox
            // 
            ForceAliasSelectionBox.AutoSize = true;
            ForceAliasSelectionBox.Location = new System.Drawing.Point(19, 405);
            ForceAliasSelectionBox.Name = "ForceAliasSelectionBox";
            ForceAliasSelectionBox.Size = new System.Drawing.Size(115, 19);
            ForceAliasSelectionBox.TabIndex = 11;
            ForceAliasSelectionBox.Text = "Aliased Selection";
            toolTip1.SetToolTip(ForceAliasSelectionBox, "Normally, selections are anti-aliased.\r\nThis will force a selection to be aliased.\r\nIt is useful if you are modifying the alpha values\r\nalong the marching ants boundary.");
            ForceAliasSelectionBox.UseVisualStyleBackColor = true;
            // 
            // PreviewHelpButton
            // 
            PreviewHelpButton.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            PreviewHelpButton.BackColor = System.Drawing.SystemColors.ActiveCaption;
            PreviewHelpButton.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            PreviewHelpButton.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            PreviewHelpButton.Location = new System.Drawing.Point(684, 25);
            PreviewHelpButton.Name = "PreviewHelpButton";
            PreviewHelpButton.Size = new System.Drawing.Size(26, 23);
            PreviewHelpButton.TabIndex = 18;
            PreviewHelpButton.Text = "?";
            toolTip1.SetToolTip(PreviewHelpButton, "Preview Help Content");
            PreviewHelpButton.UseVisualStyleBackColor = false;
            PreviewHelpButton.Click += PreviewHelp_Click;
            // 
            // ViewSourceButton
            // 
            ViewSourceButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            ViewSourceButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            ViewSourceButton.Location = new System.Drawing.Point(288, 670);
            ViewSourceButton.Name = "ViewSourceButton";
            ViewSourceButton.Size = new System.Drawing.Size(85, 24);
            ViewSourceButton.TabIndex = 25;
            ViewSourceButton.Text = "View Source";
            toolTip1.SetToolTip(ViewSourceButton, "View complete source code that will be built");
            ViewSourceButton.UseVisualStyleBackColor = true;
            ViewSourceButton.Click += ViewSourceButton_Click;
            // 
            // StraightAlphaBox
            // 
            StraightAlphaBox.AutoSize = true;
            StraightAlphaBox.Location = new System.Drawing.Point(19, 451);
            StraightAlphaBox.Name = "StraightAlphaBox";
            StraightAlphaBox.Size = new System.Drawing.Size(101, 19);
            StraightAlphaBox.TabIndex = 15;
            StraightAlphaBox.Text = "Straight Alpha";
            toolTip1.SetToolTip(StraightAlphaBox, "If unchecked Premultiplied Alpha is supplied to GPU accelerated effects");
            StraightAlphaBox.UseVisualStyleBackColor = true;
            // 
            // WorkingSpaceColorContextBox
            // 
            WorkingSpaceColorContextBox.AutoSize = true;
            WorkingSpaceColorContextBox.Location = new System.Drawing.Point(19, 473);
            WorkingSpaceColorContextBox.Margin = new System.Windows.Forms.Padding(2);
            WorkingSpaceColorContextBox.Name = "WorkingSpaceColorContextBox";
            WorkingSpaceColorContextBox.Size = new System.Drawing.Size(188, 19);
            WorkingSpaceColorContextBox.TabIndex = 91;
            WorkingSpaceColorContextBox.Text = "Working Space Gamma (sRGB 2.2)";
            toolTip1.SetToolTip(WorkingSpaceColorContextBox, "If unchecked then the source images are supplied in linear (1.0) gamma to GPU accelerated effects. It is usually best to use linear gamma, but sRGB can be more comfortable or convenient in some cases.");
            WorkingSpaceColorContextBox.UseVisualStyleBackColor = true;
            // 
            // EffectRadio
            // 
            EffectRadio.AutoSize = true;
            EffectRadio.Checked = true;
            EffectRadio.Location = new System.Drawing.Point(147, 4);
            EffectRadio.Name = "EffectRadio";
            EffectRadio.Size = new System.Drawing.Size(89, 19);
            EffectRadio.TabIndex = 1;
            EffectRadio.TabStop = true;
            EffectRadio.Text = "Effect Menu";
            EffectRadio.UseVisualStyleBackColor = true;
            // 
            // AdjustmentRadio
            // 
            AdjustmentRadio.AutoSize = true;
            AdjustmentRadio.Location = new System.Drawing.Point(12, 3);
            AdjustmentRadio.Name = "AdjustmentRadio";
            AdjustmentRadio.Size = new System.Drawing.Size(121, 19);
            AdjustmentRadio.TabIndex = 0;
            AdjustmentRadio.Text = "Adjustment Menu";
            AdjustmentRadio.UseVisualStyleBackColor = true;
            // 
            // AuthorName
            // 
            AuthorName.Location = new System.Drawing.Point(110, 209);
            AuthorName.MaxLength = 50;
            AuthorName.Name = "AuthorName";
            AuthorName.Size = new System.Drawing.Size(150, 23);
            AuthorName.TabIndex = 5;
            // 
            // label16
            // 
            label16.AutoSize = true;
            label16.Location = new System.Drawing.Point(16, 212);
            label16.Name = "label16";
            label16.Size = new System.Drawing.Size(90, 15);
            label16.TabIndex = 37;
            label16.Text = "Author's Name:";
            // 
            // label17
            // 
            label17.AutoSize = true;
            label17.Location = new System.Drawing.Point(16, 246);
            label17.Name = "label17";
            label17.Size = new System.Drawing.Size(71, 15);
            label17.TabIndex = 40;
            label17.Text = "DLL Version:";
            // 
            // DecimalSymbol
            // 
            DecimalSymbol.AutoSize = true;
            DecimalSymbol.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            DecimalSymbol.Location = new System.Drawing.Point(185, 233);
            DecimalSymbol.Name = "DecimalSymbol";
            DecimalSymbol.Size = new System.Drawing.Size(20, 29);
            DecimalSymbol.TabIndex = 41;
            DecimalSymbol.Text = ".";
            // 
            // SupportURL
            // 
            SupportURL.Location = new System.Drawing.Point(60, 345);
            SupportURL.MaxLength = 255;
            SupportURL.Name = "SupportURL";
            SupportURL.Size = new System.Drawing.Size(200, 23);
            SupportURL.TabIndex = 10;
            SupportURL.Text = "https://www.getpaint.net/redirect/plugins.html";
            // 
            // label19
            // 
            label19.AutoSize = true;
            label19.Location = new System.Drawing.Point(16, 348);
            label19.Name = "label19";
            label19.Size = new System.Drawing.Size(31, 15);
            label19.TabIndex = 43;
            label19.Text = "URL:";
            // 
            // label20
            // 
            label20.AutoSize = true;
            label20.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            label20.Location = new System.Drawing.Point(6, 184);
            label20.Name = "label20";
            label20.Size = new System.Drawing.Size(117, 13);
            label20.TabIndex = 44;
            label20.Text = "Support Information:";
            // 
            // label21
            // 
            label21.AutoSize = true;
            label21.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            label21.Location = new System.Drawing.Point(6, 10);
            label21.Name = "label21";
            label21.Size = new System.Drawing.Size(91, 15);
            label21.TabIndex = 45;
            label21.Text = "Menu Settings:";
            // 
            // label23
            // 
            label23.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            label23.Location = new System.Drawing.Point(22, 191);
            label23.Name = "label23";
            label23.Size = new System.Drawing.Size(222, 2);
            label23.TabIndex = 48;
            // 
            // label24
            // 
            label24.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            label24.Location = new System.Drawing.Point(22, 17);
            label24.Name = "label24";
            label24.Size = new System.Drawing.Size(238, 2);
            label24.TabIndex = 49;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(16, 150);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(77, 15);
            label3.TabIndex = 52;
            label3.Text = "Window title:";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new System.Drawing.Point(16, 280);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(35, 15);
            label4.TabIndex = 53;
            label4.Text = "Desc:";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new System.Drawing.Point(16, 314);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(66, 15);
            label5.TabIndex = 55;
            label5.Text = "Key|Words:";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            label6.Location = new System.Drawing.Point(6, 382);
            label6.Name = "label6";
            label6.Size = new System.Drawing.Size(161, 13);
            label6.TabIndex = 56;
            label6.Text = "Forced Settings: (Rarely used)";
            // 
            // label7
            // 
            label7.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            label7.Location = new System.Drawing.Point(22, 390);
            label7.Name = "label7";
            label7.Size = new System.Drawing.Size(238, 2);
            label7.TabIndex = 57;
            // 
            // ForceSingleThreadedBox
            // 
            ForceSingleThreadedBox.AutoSize = true;
            ForceSingleThreadedBox.Location = new System.Drawing.Point(141, 405);
            ForceSingleThreadedBox.Name = "ForceSingleThreadedBox";
            ForceSingleThreadedBox.Size = new System.Drawing.Size(110, 19);
            ForceSingleThreadedBox.TabIndex = 12;
            ForceSingleThreadedBox.Text = "Single Threaded";
            ForceSingleThreadedBox.UseVisualStyleBackColor = true;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            label8.Location = new System.Drawing.Point(285, 10);
            label8.Name = "label8";
            label8.Size = new System.Drawing.Size(79, 13);
            label8.TabIndex = 59;
            label8.Text = "Help Content:";
            // 
            // label9
            // 
            label9.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            label9.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            label9.Location = new System.Drawing.Point(328, 17);
            label9.Name = "label9";
            label9.Size = new System.Drawing.Size(385, 2);
            label9.TabIndex = 60;
            // 
            // radioButtonNone
            // 
            radioButtonNone.AutoSize = true;
            radioButtonNone.Checked = true;
            radioButtonNone.Location = new System.Drawing.Point(298, 27);
            radioButtonNone.Name = "radioButtonNone";
            radioButtonNone.Size = new System.Drawing.Size(54, 19);
            radioButtonNone.TabIndex = 17;
            radioButtonNone.TabStop = true;
            radioButtonNone.Text = "None";
            radioButtonNone.UseVisualStyleBackColor = true;
            radioButtonNone.CheckedChanged += radioButtonNone_CheckedChanged;
            // 
            // radioButtonURL
            // 
            radioButtonURL.AutoSize = true;
            radioButtonURL.Location = new System.Drawing.Point(298, 51);
            radioButtonURL.Name = "radioButtonURL";
            radioButtonURL.Size = new System.Drawing.Size(77, 19);
            radioButtonURL.TabIndex = 19;
            radioButtonURL.TabStop = true;
            radioButtonURL.Text = "Help URL:";
            radioButtonURL.UseVisualStyleBackColor = true;
            radioButtonURL.CheckedChanged += radioButtonURL_CheckedChanged;
            // 
            // radioButtonPlain
            // 
            radioButtonPlain.AutoSize = true;
            radioButtonPlain.Location = new System.Drawing.Point(298, 78);
            radioButtonPlain.Name = "radioButtonPlain";
            radioButtonPlain.Size = new System.Drawing.Size(77, 19);
            radioButtonPlain.TabIndex = 21;
            radioButtonPlain.TabStop = true;
            radioButtonPlain.Text = "Plain text:";
            radioButtonPlain.UseVisualStyleBackColor = true;
            radioButtonPlain.CheckedChanged += radioButtonPlain_CheckedChanged;
            // 
            // radioButtonRich
            // 
            radioButtonRich.AutoSize = true;
            radioButtonRich.Location = new System.Drawing.Point(298, 134);
            radioButtonRich.Name = "radioButtonRich";
            radioButtonRich.Size = new System.Drawing.Size(74, 19);
            radioButtonRich.TabIndex = 23;
            radioButtonRich.TabStop = true;
            radioButtonRich.Text = "Rich text:";
            radioButtonRich.UseVisualStyleBackColor = true;
            radioButtonRich.CheckedChanged += radioButtonRich_CheckedChanged;
            // 
            // HelpURL
            // 
            HelpURL.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            HelpURL.Location = new System.Drawing.Point(385, 50);
            HelpURL.Name = "HelpURL";
            HelpURL.Size = new System.Drawing.Size(325, 23);
            HelpURL.TabIndex = 20;
            HelpURL.Text = "https://www.getpaint.net/redirect/plugins.html";
            // 
            // HelpPlainText
            // 
            HelpPlainText.AcceptsReturn = true;
            HelpPlainText.AcceptsTab = true;
            HelpPlainText.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            HelpPlainText.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            HelpPlainText.Location = new System.Drawing.Point(385, 77);
            HelpPlainText.Multiline = true;
            HelpPlainText.Name = "HelpPlainText";
            HelpPlainText.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            HelpPlainText.Size = new System.Drawing.Size(325, 53);
            HelpPlainText.TabIndex = 22;
            // 
            // RichHelpContent
            // 
            RichHelpContent.AcceptsTab = true;
            RichHelpContent.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            RichHelpContent.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            RichHelpContent.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            RichHelpContent.Location = new System.Drawing.Point(288, 182);
            RichHelpContent.Name = "RichHelpContent";
            RichHelpContent.Size = new System.Drawing.Size(422, 433);
            RichHelpContent.TabIndex = 24;
            RichHelpContent.Text = "";
            RichHelpContent.KeyDown += RichHelpContent_KeyDown;
            // 
            // toolStrip1
            // 
            toolStrip1.AllowClickThrough = true;
            toolStrip1.Dock = System.Windows.Forms.DockStyle.None;
            toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { OpenButton, SaveButton, WordPadButton, toolStripSeparator2, BoldButton, ItalicsButton, UnderlineButton, SuperScriptButton, SubScriptButton, LargeFontButton, SmallFontButton, ColorButton, toolStripSeparator3, InsertImageButton, BulletButton, IndentButton, UnindentButton, ParagraphLeftButton, CenterButton });
            toolStrip1.Location = new System.Drawing.Point(288, 155);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.Padding = new System.Windows.Forms.Padding(0);
            toolStrip1.Size = new System.Drawing.Size(422, 27);
            toolStrip1.TabIndex = 0;
            // 
            // OpenButton
            // 
            OpenButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            OpenButton.ImageName = "Open";
            OpenButton.Name = "OpenButton";
            OpenButton.Padding = new System.Windows.Forms.Padding(2);
            OpenButton.Size = new System.Drawing.Size(24, 24);
            OpenButton.Text = "Open";
            OpenButton.ToolTipText = "Open (Ctrl+O)";
            OpenButton.Click += OpenButton_Click;
            // 
            // SaveButton
            // 
            SaveButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            SaveButton.ImageName = "Save";
            SaveButton.Name = "SaveButton";
            SaveButton.Padding = new System.Windows.Forms.Padding(2);
            SaveButton.Size = new System.Drawing.Size(24, 24);
            SaveButton.Text = "Save";
            SaveButton.ToolTipText = "Save (Ctrl+S)";
            SaveButton.Click += SaveButton_Click_1;
            // 
            // WordPadButton
            // 
            WordPadButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            WordPadButton.ImageName = "WordPad";
            WordPadButton.Name = "WordPadButton";
            WordPadButton.Padding = new System.Windows.Forms.Padding(2);
            WordPadButton.Size = new System.Drawing.Size(24, 24);
            WordPadButton.Text = "Edit in WordPad";
            WordPadButton.ToolTipText = "Edit in WordPad (Ctrl+W)";
            WordPadButton.Click += WordPadButton_Click;
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new System.Drawing.Size(6, 27);
            // 
            // BoldButton
            // 
            BoldButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            BoldButton.ImageName = "Bold";
            BoldButton.Name = "BoldButton";
            BoldButton.Padding = new System.Windows.Forms.Padding(2);
            BoldButton.Size = new System.Drawing.Size(24, 24);
            BoldButton.Text = "Bold";
            BoldButton.ToolTipText = "Bold (Ctrl+B)";
            BoldButton.Click += BoldButton_Click;
            // 
            // ItalicsButton
            // 
            ItalicsButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            ItalicsButton.ImageName = "Italic";
            ItalicsButton.Name = "ItalicsButton";
            ItalicsButton.Padding = new System.Windows.Forms.Padding(2);
            ItalicsButton.Size = new System.Drawing.Size(24, 24);
            ItalicsButton.Text = "Italics";
            ItalicsButton.ToolTipText = "Italics (Ctrl+I)";
            ItalicsButton.Click += ItalicsButton_Click;
            // 
            // UnderlineButton
            // 
            UnderlineButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            UnderlineButton.ImageName = "Underline";
            UnderlineButton.Name = "UnderlineButton";
            UnderlineButton.Padding = new System.Windows.Forms.Padding(2);
            UnderlineButton.Size = new System.Drawing.Size(24, 24);
            UnderlineButton.Text = "Underline";
            UnderlineButton.ToolTipText = "Underline (Ctrl+U)";
            UnderlineButton.Click += UnderlineButton_Click;
            // 
            // SuperScriptButton
            // 
            SuperScriptButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            SuperScriptButton.ImageName = "SuperScript";
            SuperScriptButton.Name = "SuperScriptButton";
            SuperScriptButton.Padding = new System.Windows.Forms.Padding(2);
            SuperScriptButton.Size = new System.Drawing.Size(24, 24);
            SuperScriptButton.Text = "SuperScript";
            SuperScriptButton.ToolTipText = "Superscript";
            SuperScriptButton.Click += SuperScriptButton_Click;
            // 
            // SubScriptButton
            // 
            SubScriptButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            SubScriptButton.ImageName = "SubScript";
            SubScriptButton.Name = "SubScriptButton";
            SubScriptButton.Padding = new System.Windows.Forms.Padding(2);
            SubScriptButton.Size = new System.Drawing.Size(24, 24);
            SubScriptButton.Text = "Subscript";
            SubScriptButton.Click += SubScriptButton_Click;
            // 
            // LargeFontButton
            // 
            LargeFontButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            LargeFontButton.ImageName = "IncreaseFontSize";
            LargeFontButton.Name = "LargeFontButton";
            LargeFontButton.Padding = new System.Windows.Forms.Padding(2);
            LargeFontButton.Size = new System.Drawing.Size(24, 24);
            LargeFontButton.Text = "Increase Font Size";
            LargeFontButton.Click += LargeFontButton_Click;
            // 
            // SmallFontButton
            // 
            SmallFontButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            SmallFontButton.ImageName = "DecreaseFontSize";
            SmallFontButton.Name = "SmallFontButton";
            SmallFontButton.Padding = new System.Windows.Forms.Padding(2);
            SmallFontButton.Size = new System.Drawing.Size(24, 24);
            SmallFontButton.Text = "Decrease Font Size";
            SmallFontButton.Click += SmallFontButton_Click;
            // 
            // ColorButton
            // 
            ColorButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            ColorButton.ImageName = "02ColorWheel";
            ColorButton.Name = "ColorButton";
            ColorButton.Padding = new System.Windows.Forms.Padding(2);
            ColorButton.Size = new System.Drawing.Size(24, 24);
            ColorButton.Text = "Color";
            ColorButton.ToolTipText = "Color (F8)";
            ColorButton.Click += ColorButton_Click;
            // 
            // toolStripSeparator3
            // 
            toolStripSeparator3.Name = "toolStripSeparator3";
            toolStripSeparator3.Size = new System.Drawing.Size(6, 27);
            // 
            // InsertImageButton
            // 
            InsertImageButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            InsertImageButton.ImageName = "InsertPicture";
            InsertImageButton.Name = "InsertImageButton";
            InsertImageButton.Padding = new System.Windows.Forms.Padding(2);
            InsertImageButton.Size = new System.Drawing.Size(24, 24);
            InsertImageButton.Text = "Insert Image";
            InsertImageButton.Click += InsertImageButton_Click;
            // 
            // BulletButton
            // 
            BulletButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            BulletButton.ImageName = "Bullets";
            BulletButton.Name = "BulletButton";
            BulletButton.Padding = new System.Windows.Forms.Padding(2);
            BulletButton.Size = new System.Drawing.Size(24, 24);
            BulletButton.Text = "Bullets";
            BulletButton.Click += BulletButton_Click;
            // 
            // IndentButton
            // 
            IndentButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            IndentButton.ImageName = "Indent";
            IndentButton.Name = "IndentButton";
            IndentButton.Padding = new System.Windows.Forms.Padding(2);
            IndentButton.Size = new System.Drawing.Size(24, 24);
            IndentButton.Text = "Indent";
            IndentButton.Click += IndentButton_Click;
            // 
            // UnindentButton
            // 
            UnindentButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            UnindentButton.ImageName = "Unindent";
            UnindentButton.Name = "UnindentButton";
            UnindentButton.Padding = new System.Windows.Forms.Padding(2);
            UnindentButton.Size = new System.Drawing.Size(24, 24);
            UnindentButton.Text = "Unindent";
            UnindentButton.Click += UnindentButton_Click;
            // 
            // ParagraphLeftButton
            // 
            ParagraphLeftButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            ParagraphLeftButton.ImageName = "ParagraphLeft";
            ParagraphLeftButton.Name = "ParagraphLeftButton";
            ParagraphLeftButton.Padding = new System.Windows.Forms.Padding(2);
            ParagraphLeftButton.Size = new System.Drawing.Size(24, 24);
            ParagraphLeftButton.Text = "Align Left";
            ParagraphLeftButton.Click += ParagraphLeftButton_Click;
            // 
            // CenterButton
            // 
            CenterButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            CenterButton.ImageName = "ParagraphCenter";
            CenterButton.Name = "CenterButton";
            CenterButton.Padding = new System.Windows.Forms.Padding(2);
            CenterButton.Size = new System.Drawing.Size(24, 24);
            CenterButton.Text = "Center";
            CenterButton.Click += CenterButton_Click;
            // 
            // panel1
            // 
            panel1.Controls.Add(AdjustmentRadio);
            panel1.Controls.Add(EffectRadio);
            panel1.Location = new System.Drawing.Point(9, 30);
            panel1.Name = "panel1";
            panel1.Size = new System.Drawing.Size(240, 28);
            panel1.TabIndex = 0;
            // 
            // PreviewLabel
            // 
            PreviewLabel.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            PreviewLabel.AutoSize = true;
            PreviewLabel.Location = new System.Drawing.Point(553, 29);
            PreviewLabel.Name = "PreviewLabel";
            PreviewLabel.Size = new System.Drawing.Size(121, 15);
            PreviewLabel.TabIndex = 85;
            PreviewLabel.Text = "Preview help content:";
            // 
            // PlainTextLabel
            // 
            PlainTextLabel.AutoSize = true;
            PlainTextLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            PlainTextLabel.Location = new System.Drawing.Point(319, 97);
            PlainTextLabel.Name = "PlainTextLabel";
            PlainTextLabel.Size = new System.Drawing.Size(42, 24);
            PlainTextLabel.TabIndex = 80;
            PlainTextLabel.Text = "(Use Tab\r\nor Enter)";
            // 
            // WarningLabel
            // 
            WarningLabel.AutoSize = true;
            WarningLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            WarningLabel.ForeColor = System.Drawing.Color.Red;
            WarningLabel.Location = new System.Drawing.Point(376, 136);
            WarningLabel.Name = "WarningLabel";
            WarningLabel.Size = new System.Drawing.Size(154, 13);
            WarningLabel.TabIndex = 86;
            WarningLabel.Text = "Close WordPad to continue!";
            // 
            // MenuIcon
            // 
            MenuIcon.Location = new System.Drawing.Point(89, 104);
            MenuIcon.Name = "MenuIcon";
            MenuIcon.Size = new System.Drawing.Size(16, 16);
            MenuIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            MenuIcon.TabIndex = 3;
            MenuIcon.TabStop = false;
            // 
            // GenSlnButton
            // 
            GenSlnButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            GenSlnButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            GenSlnButton.Location = new System.Drawing.Point(379, 670);
            GenSlnButton.Name = "GenSlnButton";
            GenSlnButton.Size = new System.Drawing.Size(131, 24);
            GenSlnButton.TabIndex = 26;
            GenSlnButton.Text = "Generate VS Solution";
            GenSlnButton.UseVisualStyleBackColor = true;
            GenSlnButton.Click += GenSlnButton_Click;
            // 
            // sampleImage
            // 
            sampleImage.Location = new System.Drawing.Point(35, 529);
            sampleImage.Name = "sampleImage";
            sampleImage.Size = new System.Drawing.Size(200, 150);
            sampleImage.TabIndex = 89;
            sampleImage.TabStop = false;
            // 
            // sampleLabel
            // 
            sampleLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            sampleLabel.Location = new System.Drawing.Point(6, 498);
            sampleLabel.Name = "sampleLabel";
            sampleLabel.Size = new System.Drawing.Size(254, 124);
            sampleLabel.TabIndex = 90;
            sampleLabel.Text = "Sample Image Detected:";
            // 
            // forceLegacyRoiBox
            // 
            forceLegacyRoiBox.AutoSize = true;
            forceLegacyRoiBox.Location = new System.Drawing.Point(19, 428);
            forceLegacyRoiBox.Name = "forceLegacyRoiBox";
            forceLegacyRoiBox.Size = new System.Drawing.Size(85, 19);
            forceLegacyRoiBox.TabIndex = 13;
            forceLegacyRoiBox.Text = "Legacy ROI";
            forceLegacyRoiBox.UseVisualStyleBackColor = true;
            forceLegacyRoiBox.CheckedChanged += ForceROI_CheckedChanged;
            // 
            // forceSingleRenderBox
            // 
            forceSingleRenderBox.AutoSize = true;
            forceSingleRenderBox.Location = new System.Drawing.Point(141, 428);
            forceSingleRenderBox.Name = "forceSingleRenderBox";
            forceSingleRenderBox.Size = new System.Drawing.Size(121, 19);
            forceSingleRenderBox.TabIndex = 14;
            forceSingleRenderBox.Text = "Single Render Call";
            forceSingleRenderBox.UseVisualStyleBackColor = true;
            forceSingleRenderBox.CheckedChanged += ForceROI_CheckedChanged;
            // 
            // NoSelectionClippingBox
            // 
            NoSelectionClippingBox.AutoSize = true;
            NoSelectionClippingBox.Location = new System.Drawing.Point(141, 451);
            NoSelectionClippingBox.Name = "NoSelectionClippingBox";
            NoSelectionClippingBox.Size = new System.Drawing.Size(117, 19);
            NoSelectionClippingBox.TabIndex = 16;
            NoSelectionClippingBox.Text = "No Selection Clip";
            NoSelectionClippingBox.UseVisualStyleBackColor = true;
            // 
            // BuildForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            CancelButton = ButtonCancel;
            ClientSize = new System.Drawing.Size(730, 709);
            Controls.Add(WorkingSpaceColorContextBox);
            Controls.Add(NoSelectionClippingBox);
            Controls.Add(StraightAlphaBox);
            Controls.Add(forceSingleRenderBox);
            Controls.Add(forceLegacyRoiBox);
            Controls.Add(sampleImage);
            Controls.Add(sampleLabel);
            Controls.Add(GenSlnButton);
            Controls.Add(ViewSourceButton);
            Controls.Add(toolStrip1);
            Controls.Add(WarningLabel);
            Controls.Add(PreviewLabel);
            Controls.Add(panel1);
            Controls.Add(RichHelpContent);
            Controls.Add(HelpPlainText);
            Controls.Add(PlainTextLabel);
            Controls.Add(HelpURL);
            Controls.Add(radioButtonRich);
            Controls.Add(radioButtonPlain);
            Controls.Add(radioButtonURL);
            Controls.Add(radioButtonNone);
            Controls.Add(PreviewHelpButton);
            Controls.Add(label8);
            Controls.Add(label9);
            Controls.Add(ForceSingleThreadedBox);
            Controls.Add(label6);
            Controls.Add(label7);
            Controls.Add(ForceAliasSelectionBox);
            Controls.Add(KeyWordsBox);
            Controls.Add(label5);
            Controls.Add(DescriptionBox);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(WindowTitleText);
            Controls.Add(label20);
            Controls.Add(label21);
            Controls.Add(label24);
            Controls.Add(label23);
            Controls.Add(label19);
            Controls.Add(SupportURL);
            Controls.Add(label17);
            Controls.Add(MinorVersion);
            Controls.Add(MajorVersion);
            Controls.Add(label16);
            Controls.Add(AuthorName);
            Controls.Add(ButtonIcon);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(SubMenuName);
            Controls.Add(MenuIcon);
            Controls.Add(MenuName);
            Controls.Add(ButtonSave);
            Controls.Add(ButtonCancel);
            Controls.Add(DecimalSymbol);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            IconName = "SaveAsDll";
            Location = new System.Drawing.Point(0, 0);
            MinimizeBox = true;
            MinimumSize = new System.Drawing.Size(743, 675);
            Name = "BuildForm";
            Text = "Building DLL";
            ((System.ComponentModel.ISupportInitialize)MajorVersion).EndInit();
            ((System.ComponentModel.ISupportInitialize)MinorVersion).EndInit();
            toolStrip1.ResumeLayout(false);
            toolStrip1.PerformLayout();
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)MenuIcon).EndInit();
            ((System.ComponentModel.ISupportInitialize)sampleImage).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Button ButtonCancel;
        private System.Windows.Forms.Button ButtonSave;
        private System.Windows.Forms.TextBox MenuName;
        private System.Windows.Forms.PictureBox MenuIcon;
        private System.Windows.Forms.ComboBox SubMenuName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.LinkLabel ButtonIcon;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.RadioButton EffectRadio;
        private System.Windows.Forms.RadioButton AdjustmentRadio;
        private System.Windows.Forms.TextBox AuthorName;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.NumericUpDown MajorVersion;
        private System.Windows.Forms.NumericUpDown MinorVersion;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label DecimalSymbol;
        private System.Windows.Forms.TextBox SupportURL;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.TextBox WindowTitleText;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox DescriptionBox;
        private System.Windows.Forms.TextBox KeyWordsBox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.CheckBox ForceAliasSelectionBox;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.CheckBox ForceSingleThreadedBox;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button PreviewHelpButton;
        private System.Windows.Forms.RadioButton radioButtonNone;
        private System.Windows.Forms.RadioButton radioButtonURL;
        private System.Windows.Forms.RadioButton radioButtonPlain;
        private System.Windows.Forms.RadioButton radioButtonRich;
        private System.Windows.Forms.TextBox HelpURL;
        private System.Windows.Forms.TextBox HelpPlainText;
        private System.Windows.Forms.RichTextBox RichHelpContent;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private ScaledToolStripButton BoldButton;
        private ScaledToolStripButton ItalicsButton;
        private ScaledToolStripButton UnderlineButton;
        private ScaledToolStripButton SuperScriptButton;
        private ScaledToolStripButton SubScriptButton;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label PreviewLabel;
        private ScaledToolStripButton LargeFontButton;
        private ScaledToolStripButton SmallFontButton;
        private ScaledToolStripButton IndentButton;
        private ScaledToolStripButton UnindentButton;
        private ScaledToolStripButton ParagraphLeftButton;
        private ScaledToolStripButton CenterButton;
        private ScaledToolStripButton BulletButton;
        private ScaledToolStripButton OpenButton;
        private ScaledToolStripButton SaveButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.Label PlainTextLabel;
        private ScaledToolStripButton WordPadButton;
        private ScaledToolStripButton InsertImageButton;
        private ScaledToolStripButton ColorButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.Label WarningLabel;
        private System.Windows.Forms.Button ViewSourceButton;
        private System.Windows.Forms.Button GenSlnButton;
        private System.Windows.Forms.PictureBox sampleImage;
        private System.Windows.Forms.Label sampleLabel;
        private System.Windows.Forms.CheckBox forceLegacyRoiBox;
        private System.Windows.Forms.CheckBox forceSingleRenderBox;
        private System.Windows.Forms.CheckBox StraightAlphaBox;
        private System.Windows.Forms.CheckBox NoSelectionClippingBox;
        private System.Windows.Forms.CheckBox WorkingSpaceColorContextBox;
    }
}