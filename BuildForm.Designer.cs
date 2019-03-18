namespace PaintDotNet.Effects
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
            this.components = new System.ComponentModel.Container();
            this.ButtonCancel = new System.Windows.Forms.Button();
            this.ButtonSave = new System.Windows.Forms.Button();
            this.MenuName = new System.Windows.Forms.TextBox();
            this.SubMenuName = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.ButtonIcon = new System.Windows.Forms.LinkLabel();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.MajorVersion = new System.Windows.Forms.NumericUpDown();
            this.MinorVersion = new System.Windows.Forms.NumericUpDown();
            this.WindowTitleText = new System.Windows.Forms.TextBox();
            this.DescriptionBox = new System.Windows.Forms.TextBox();
            this.KeyWordsBox = new System.Windows.Forms.TextBox();
            this.ForceAliasSelectionBox = new System.Windows.Forms.CheckBox();
            this.PreviewHelpButton = new System.Windows.Forms.Button();
            this.ViewSourceButton = new System.Windows.Forms.Button();
            this.EffectRadio = new System.Windows.Forms.RadioButton();
            this.AdjustmentRadio = new System.Windows.Forms.RadioButton();
            this.AuthorName = new System.Windows.Forms.TextBox();
            this.label16 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.DecimalSymbol = new System.Windows.Forms.Label();
            this.SupportURL = new System.Windows.Forms.TextBox();
            this.label19 = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.label21 = new System.Windows.Forms.Label();
            this.label23 = new System.Windows.Forms.Label();
            this.label24 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.ForceSingleThreadedBox = new System.Windows.Forms.CheckBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.radioButtonNone = new System.Windows.Forms.RadioButton();
            this.radioButtonURL = new System.Windows.Forms.RadioButton();
            this.radioButtonPlain = new System.Windows.Forms.RadioButton();
            this.radioButtonRich = new System.Windows.Forms.RadioButton();
            this.HelpURL = new System.Windows.Forms.TextBox();
            this.HelpPlainText = new System.Windows.Forms.TextBox();
            this.RichHelpContent = new System.Windows.Forms.RichTextBox();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.OpenButton = new System.Windows.Forms.ToolStripButton();
            this.SaveButton = new System.Windows.Forms.ToolStripButton();
            this.WordPadButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.BoldButton = new System.Windows.Forms.ToolStripButton();
            this.ItalicsButton = new System.Windows.Forms.ToolStripButton();
            this.UnderlineButton = new System.Windows.Forms.ToolStripButton();
            this.SuperScriptButton = new System.Windows.Forms.ToolStripButton();
            this.SubScriptButton = new System.Windows.Forms.ToolStripButton();
            this.LargeFontButton = new System.Windows.Forms.ToolStripButton();
            this.SmallFontButton = new System.Windows.Forms.ToolStripButton();
            this.ColorButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.InsertImageButton = new System.Windows.Forms.ToolStripButton();
            this.BulletButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.IndentButton = new System.Windows.Forms.ToolStripButton();
            this.UnindentButton = new System.Windows.Forms.ToolStripButton();
            this.ParagraphLeftButton = new System.Windows.Forms.ToolStripButton();
            this.CenterButton = new System.Windows.Forms.ToolStripButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.PreviewLabel = new System.Windows.Forms.Label();
            this.PlainTextLabel = new System.Windows.Forms.Label();
            this.WarningLabel = new System.Windows.Forms.Label();
            this.MenuIcon = new System.Windows.Forms.PictureBox();
            this.GenSlnButton = new System.Windows.Forms.Button();
            this.sampleImage = new System.Windows.Forms.PictureBox();
            this.sampleLabel = new System.Windows.Forms.Label();
            this.forceLegacyRoiBox = new System.Windows.Forms.CheckBox();
            this.forceSingleRenderBox = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.MajorVersion)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.MinorVersion)).BeginInit();
            this.toolStrip1.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.MenuIcon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.sampleImage)).BeginInit();
            this.SuspendLayout();
            // 
            // ButtonCancel
            // 
            this.ButtonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ButtonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.ButtonCancel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.ButtonCancel.Location = new System.Drawing.Point(618, 600);
            this.ButtonCancel.Name = "ButtonCancel";
            this.ButtonCancel.Size = new System.Drawing.Size(75, 23);
            this.ButtonCancel.TabIndex = 26;
            this.ButtonCancel.Text = "Cancel";
            this.ButtonCancel.UseVisualStyleBackColor = true;
            this.ButtonCancel.Click += new System.EventHandler(this.ButtonCancel_Click);
            // 
            // ButtonSave
            // 
            this.ButtonSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ButtonSave.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.ButtonSave.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ButtonSave.Location = new System.Drawing.Point(537, 600);
            this.ButtonSave.Name = "ButtonSave";
            this.ButtonSave.Size = new System.Drawing.Size(75, 23);
            this.ButtonSave.TabIndex = 25;
            this.ButtonSave.Text = "Build";
            this.ButtonSave.UseVisualStyleBackColor = true;
            this.ButtonSave.Click += new System.EventHandler(this.ButtonSave_Click);
            // 
            // MenuName
            // 
            this.MenuName.Location = new System.Drawing.Point(106, 100);
            this.MenuName.MaxLength = 50;
            this.MenuName.Name = "MenuName";
            this.MenuName.Size = new System.Drawing.Size(143, 20);
            this.MenuName.TabIndex = 2;
            this.toolTip1.SetToolTip(this.MenuName, "Enter a name for your effect\r\nas it will show up in Paint.NET");
            // 
            // SubMenuName
            // 
            this.SubMenuName.FormattingEnabled = true;
            this.SubMenuName.Items.AddRange(new object[] {
            "",
            "Advanced",
            "Artistic",
            "Blurs",
            "Color",
            "Distort",
            "Noise",
            "Object",
            "Photo",
            "Render",
            "Stylize"});
            this.SubMenuName.Location = new System.Drawing.Point(80, 63);
            this.SubMenuName.MaxDropDownItems = 12;
            this.SubMenuName.Name = "SubMenuName";
            this.SubMenuName.Size = new System.Drawing.Size(169, 21);
            this.SubMenuName.TabIndex = 1;
            this.toolTip1.SetToolTip(this.SubMenuName, "Select a submenu or leave blank for the main Effects menu.");
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 67);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(55, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Submenu:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(16, 104);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(61, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Menu Text:";
            // 
            // ButtonIcon
            // 
            this.ButtonIcon.AutoSize = true;
            this.ButtonIcon.Location = new System.Drawing.Point(188, 123);
            this.ButtonIcon.Name = "ButtonIcon";
            this.ButtonIcon.Size = new System.Drawing.Size(61, 13);
            this.ButtonIcon.TabIndex = 3;
            this.ButtonIcon.TabStop = true;
            this.ButtonIcon.Text = "Select Icon";
            this.toolTip1.SetToolTip(this.ButtonIcon, "Select a 16x16 PNG file for display in the Effects menu.");
            this.ButtonIcon.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.ButtonIcon_LinkClicked);
            // 
            // MajorVersion
            // 
            this.MajorVersion.Location = new System.Drawing.Point(122, 243);
            this.MajorVersion.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.MajorVersion.Name = "MajorVersion";
            this.MajorVersion.Size = new System.Drawing.Size(50, 20);
            this.MajorVersion.TabIndex = 6;
            this.toolTip1.SetToolTip(this.MajorVersion, "Major Version");
            this.MajorVersion.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // MinorVersion
            // 
            this.MinorVersion.Location = new System.Drawing.Point(195, 243);
            this.MinorVersion.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.MinorVersion.Name = "MinorVersion";
            this.MinorVersion.Size = new System.Drawing.Size(54, 20);
            this.MinorVersion.TabIndex = 7;
            this.toolTip1.SetToolTip(this.MinorVersion, "Minor Version");
            // 
            // WindowTitleText
            // 
            this.WindowTitleText.Location = new System.Drawing.Point(93, 146);
            this.WindowTitleText.MaxLength = 255;
            this.WindowTitleText.Name = "WindowTitleText";
            this.WindowTitleText.Size = new System.Drawing.Size(156, 20);
            this.WindowTitleText.TabIndex = 4;
            this.toolTip1.SetToolTip(this.WindowTitleText, "Enter a title for your effect UI window.");
            // 
            // DescriptionBox
            // 
            this.DescriptionBox.Location = new System.Drawing.Point(58, 277);
            this.DescriptionBox.MaxLength = 75;
            this.DescriptionBox.Name = "DescriptionBox";
            this.DescriptionBox.Size = new System.Drawing.Size(192, 20);
            this.DescriptionBox.TabIndex = 8;
            this.toolTip1.SetToolTip(this.DescriptionBox, "Describe your effect in 5 words or less.");
            // 
            // KeyWordsBox
            // 
            this.KeyWordsBox.Location = new System.Drawing.Point(80, 311);
            this.KeyWordsBox.MaxLength = 50;
            this.KeyWordsBox.Name = "KeyWordsBox";
            this.KeyWordsBox.Size = new System.Drawing.Size(170, 20);
            this.KeyWordsBox.TabIndex = 9;
            this.toolTip1.SetToolTip(this.KeyWordsBox, "If you wish your effect to be searchable via keywords,\r\nenter them separated by t" +
        "he pipe symbol.");
            // 
            // ForceAliasSelectionBox
            // 
            this.ForceAliasSelectionBox.AutoSize = true;
            this.ForceAliasSelectionBox.Location = new System.Drawing.Point(19, 405);
            this.ForceAliasSelectionBox.Name = "ForceAliasSelectionBox";
            this.ForceAliasSelectionBox.Size = new System.Drawing.Size(107, 17);
            this.ForceAliasSelectionBox.TabIndex = 11;
            this.ForceAliasSelectionBox.Text = "Aliased Selection";
            this.toolTip1.SetToolTip(this.ForceAliasSelectionBox, "Normally, selections are anti-aliased.\r\nThis will force a selection to be aliased" +
        ".\r\nIt is useful if you are modifying the alpha values\r\nalong the marching ants b" +
        "oundary.");
            this.ForceAliasSelectionBox.UseVisualStyleBackColor = true;
            // 
            // PreviewHelpButton
            // 
            this.PreviewHelpButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.PreviewHelpButton.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.PreviewHelpButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.PreviewHelpButton.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.PreviewHelpButton.Location = new System.Drawing.Point(667, 24);
            this.PreviewHelpButton.Name = "PreviewHelpButton";
            this.PreviewHelpButton.Size = new System.Drawing.Size(26, 23);
            this.PreviewHelpButton.TabIndex = 16;
            this.PreviewHelpButton.Text = "?";
            this.toolTip1.SetToolTip(this.PreviewHelpButton, "Preview Help Content");
            this.PreviewHelpButton.UseVisualStyleBackColor = false;
            this.PreviewHelpButton.Click += new System.EventHandler(this.PreviewHelp_Click);
            // 
            // ViewSourceButton
            // 
            this.ViewSourceButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ViewSourceButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.ViewSourceButton.Location = new System.Drawing.Point(279, 600);
            this.ViewSourceButton.Name = "ViewSourceButton";
            this.ViewSourceButton.Size = new System.Drawing.Size(75, 23);
            this.ViewSourceButton.TabIndex = 23;
            this.ViewSourceButton.Text = "View Source";
            this.toolTip1.SetToolTip(this.ViewSourceButton, "View complete source code that will be built");
            this.ViewSourceButton.UseVisualStyleBackColor = true;
            this.ViewSourceButton.Click += new System.EventHandler(this.ViewSourceButton_Click);
            // 
            // EffectRadio
            // 
            this.EffectRadio.AutoSize = true;
            this.EffectRadio.Checked = true;
            this.EffectRadio.Location = new System.Drawing.Point(152, 7);
            this.EffectRadio.Name = "EffectRadio";
            this.EffectRadio.Size = new System.Drawing.Size(83, 17);
            this.EffectRadio.TabIndex = 1;
            this.EffectRadio.TabStop = true;
            this.EffectRadio.Text = "Effect Menu";
            this.EffectRadio.UseVisualStyleBackColor = true;
            // 
            // AdjustmentRadio
            // 
            this.AdjustmentRadio.AutoSize = true;
            this.AdjustmentRadio.Location = new System.Drawing.Point(18, 7);
            this.AdjustmentRadio.Name = "AdjustmentRadio";
            this.AdjustmentRadio.Size = new System.Drawing.Size(107, 17);
            this.AdjustmentRadio.TabIndex = 0;
            this.AdjustmentRadio.Text = "Adjustment Menu";
            this.AdjustmentRadio.UseVisualStyleBackColor = true;
            // 
            // AuthorName
            // 
            this.AuthorName.Location = new System.Drawing.Point(101, 209);
            this.AuthorName.MaxLength = 50;
            this.AuthorName.Name = "AuthorName";
            this.AuthorName.Size = new System.Drawing.Size(148, 20);
            this.AuthorName.TabIndex = 5;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(16, 212);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(79, 13);
            this.label16.TabIndex = 37;
            this.label16.Text = "Author\'s Name:";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(16, 246);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(68, 13);
            this.label17.TabIndex = 40;
            this.label17.Text = "DLL Version:";
            // 
            // DecimalSymbol
            // 
            this.DecimalSymbol.AutoSize = true;
            this.DecimalSymbol.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DecimalSymbol.Location = new System.Drawing.Point(174, 233);
            this.DecimalSymbol.Name = "DecimalSymbol";
            this.DecimalSymbol.Size = new System.Drawing.Size(20, 29);
            this.DecimalSymbol.TabIndex = 41;
            this.DecimalSymbol.Text = ".";
            // 
            // SupportURL
            // 
            this.SupportURL.Location = new System.Drawing.Point(58, 345);
            this.SupportURL.MaxLength = 255;
            this.SupportURL.Name = "SupportURL";
            this.SupportURL.Size = new System.Drawing.Size(192, 20);
            this.SupportURL.TabIndex = 10;
            this.SupportURL.Text = "http://www.getpaint.net/redirect/plugins.html";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(16, 348);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(32, 13);
            this.label19.TabIndex = 43;
            this.label19.Text = "URL:";
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label20.Location = new System.Drawing.Point(6, 184);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(117, 13);
            this.label20.TabIndex = 44;
            this.label20.Text = "Support Information:";
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label21.Location = new System.Drawing.Point(6, 10);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(86, 13);
            this.label21.TabIndex = 45;
            this.label21.Text = "Menu Settings:";
            // 
            // label23
            // 
            this.label23.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label23.Location = new System.Drawing.Point(22, 191);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(222, 2);
            this.label23.TabIndex = 48;
            // 
            // label24
            // 
            this.label24.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label24.Location = new System.Drawing.Point(22, 17);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(222, 2);
            this.label24.TabIndex = 49;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(19, 150);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(68, 13);
            this.label3.TabIndex = 52;
            this.label3.Text = "Window title:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(16, 280);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(35, 13);
            this.label4.TabIndex = 53;
            this.label4.Text = "Desc:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(16, 314);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(61, 13);
            this.label5.TabIndex = 55;
            this.label5.Text = "Key|Words:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(6, 382);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(161, 13);
            this.label6.TabIndex = 56;
            this.label6.Text = "Forced Settings: (Rarely used)";
            // 
            // label7
            // 
            this.label7.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label7.Location = new System.Drawing.Point(27, 390);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(222, 2);
            this.label7.TabIndex = 57;
            // 
            // ForceSingleThreadedBox
            // 
            this.ForceSingleThreadedBox.AutoSize = true;
            this.ForceSingleThreadedBox.Location = new System.Drawing.Point(132, 405);
            this.ForceSingleThreadedBox.Name = "ForceSingleThreadedBox";
            this.ForceSingleThreadedBox.Size = new System.Drawing.Size(104, 17);
            this.ForceSingleThreadedBox.TabIndex = 12;
            this.ForceSingleThreadedBox.Text = "Single Threaded";
            this.ForceSingleThreadedBox.UseVisualStyleBackColor = true;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(275, 10);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(79, 13);
            this.label8.TabIndex = 59;
            this.label8.Text = "Help Content:";
            // 
            // label9
            // 
            this.label9.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label9.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label9.Location = new System.Drawing.Point(318, 17);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(382, 2);
            this.label9.TabIndex = 60;
            // 
            // radioButtonNone
            // 
            this.radioButtonNone.AutoSize = true;
            this.radioButtonNone.Checked = true;
            this.radioButtonNone.Location = new System.Drawing.Point(288, 27);
            this.radioButtonNone.Name = "radioButtonNone";
            this.radioButtonNone.Size = new System.Drawing.Size(51, 17);
            this.radioButtonNone.TabIndex = 15;
            this.radioButtonNone.TabStop = true;
            this.radioButtonNone.Text = "None";
            this.radioButtonNone.UseVisualStyleBackColor = true;
            this.radioButtonNone.CheckedChanged += new System.EventHandler(this.radioButtonNone_CheckedChanged);
            // 
            // radioButtonURL
            // 
            this.radioButtonURL.AutoSize = true;
            this.radioButtonURL.Location = new System.Drawing.Point(288, 51);
            this.radioButtonURL.Name = "radioButtonURL";
            this.radioButtonURL.Size = new System.Drawing.Size(75, 17);
            this.radioButtonURL.TabIndex = 17;
            this.radioButtonURL.TabStop = true;
            this.radioButtonURL.Text = "Help URL:";
            this.radioButtonURL.UseVisualStyleBackColor = true;
            this.radioButtonURL.CheckedChanged += new System.EventHandler(this.radioButtonURL_CheckedChanged);
            // 
            // radioButtonPlain
            // 
            this.radioButtonPlain.AutoSize = true;
            this.radioButtonPlain.Location = new System.Drawing.Point(288, 78);
            this.radioButtonPlain.Name = "radioButtonPlain";
            this.radioButtonPlain.Size = new System.Drawing.Size(71, 17);
            this.radioButtonPlain.TabIndex = 19;
            this.radioButtonPlain.TabStop = true;
            this.radioButtonPlain.Text = "Plain text:";
            this.radioButtonPlain.UseVisualStyleBackColor = true;
            this.radioButtonPlain.CheckedChanged += new System.EventHandler(this.radioButtonPlain_CheckedChanged);
            // 
            // radioButtonRich
            // 
            this.radioButtonRich.AutoSize = true;
            this.radioButtonRich.Location = new System.Drawing.Point(288, 134);
            this.radioButtonRich.Name = "radioButtonRich";
            this.radioButtonRich.Size = new System.Drawing.Size(70, 17);
            this.radioButtonRich.TabIndex = 21;
            this.radioButtonRich.TabStop = true;
            this.radioButtonRich.Text = "Rich text:";
            this.radioButtonRich.UseVisualStyleBackColor = true;
            this.radioButtonRich.CheckedChanged += new System.EventHandler(this.radioButtonRich_CheckedChanged);
            // 
            // HelpURL
            // 
            this.HelpURL.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.HelpURL.Location = new System.Drawing.Point(369, 50);
            this.HelpURL.Name = "HelpURL";
            this.HelpURL.Size = new System.Drawing.Size(323, 20);
            this.HelpURL.TabIndex = 18;
            this.HelpURL.Text = "http://www.getpaint.net/redirect/plugins.html";
            // 
            // HelpPlainText
            // 
            this.HelpPlainText.AcceptsReturn = true;
            this.HelpPlainText.AcceptsTab = true;
            this.HelpPlainText.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.HelpPlainText.Location = new System.Drawing.Point(369, 77);
            this.HelpPlainText.Multiline = true;
            this.HelpPlainText.Name = "HelpPlainText";
            this.HelpPlainText.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.HelpPlainText.Size = new System.Drawing.Size(323, 53);
            this.HelpPlainText.TabIndex = 20;
            // 
            // RichHelpContent
            // 
            this.RichHelpContent.AcceptsTab = true;
            this.RichHelpContent.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.RichHelpContent.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.RichHelpContent.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.RichHelpContent.Location = new System.Drawing.Point(278, 179);
            this.RichHelpContent.Name = "RichHelpContent";
            this.RichHelpContent.Size = new System.Drawing.Size(414, 401);
            this.RichHelpContent.TabIndex = 22;
            this.RichHelpContent.Text = "";
            this.RichHelpContent.KeyDown += new System.Windows.Forms.KeyEventHandler(this.RichHelpContent_KeyDown);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.OpenButton,
            this.SaveButton,
            this.WordPadButton,
            this.toolStripSeparator2,
            this.BoldButton,
            this.ItalicsButton,
            this.UnderlineButton,
            this.SuperScriptButton,
            this.SubScriptButton,
            this.LargeFontButton,
            this.SmallFontButton,
            this.ColorButton,
            this.toolStripSeparator3,
            this.InsertImageButton,
            this.BulletButton,
            this.toolStripSeparator1,
            this.IndentButton,
            this.UnindentButton,
            this.ParagraphLeftButton,
            this.CenterButton});
            this.toolStrip1.Location = new System.Drawing.Point(278, 155);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Padding = new System.Windows.Forms.Padding(0);
            this.toolStrip1.Size = new System.Drawing.Size(411, 25);
            this.toolStrip1.TabIndex = 0;
            // 
            // OpenButton
            // 
            this.OpenButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.OpenButton.Image = global::PaintDotNet.Effects.Properties.Resources.Open;
            this.OpenButton.Name = "OpenButton";
            this.OpenButton.Size = new System.Drawing.Size(23, 22);
            this.OpenButton.Text = "Open";
            this.OpenButton.ToolTipText = "Open (Ctrl+O)";
            this.OpenButton.Click += new System.EventHandler(this.OpenButton_Click);
            // 
            // SaveButton
            // 
            this.SaveButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.SaveButton.Image = global::PaintDotNet.Effects.Properties.Resources.Save;
            this.SaveButton.Name = "SaveButton";
            this.SaveButton.Size = new System.Drawing.Size(23, 22);
            this.SaveButton.Text = "Save";
            this.SaveButton.ToolTipText = "Save (Ctrl+S)";
            this.SaveButton.Click += new System.EventHandler(this.SaveButton_Click_1);
            // 
            // WordPadButton
            // 
            this.WordPadButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.WordPadButton.Image = global::PaintDotNet.Effects.Properties.Resources.WordPad;
            this.WordPadButton.Name = "WordPadButton";
            this.WordPadButton.Size = new System.Drawing.Size(23, 22);
            this.WordPadButton.Text = "Edit in WordPad";
            this.WordPadButton.ToolTipText = "Edit in WordPad (Ctrl+W)";
            this.WordPadButton.Click += new System.EventHandler(this.WordPadButton_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // BoldButton
            // 
            this.BoldButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.BoldButton.Image = global::PaintDotNet.Effects.Properties.Resources.Bold;
            this.BoldButton.Name = "BoldButton";
            this.BoldButton.Size = new System.Drawing.Size(23, 22);
            this.BoldButton.Text = "Bold";
            this.BoldButton.ToolTipText = "Bold (Ctrl+B)";
            this.BoldButton.Click += new System.EventHandler(this.BoldButton_Click);
            // 
            // ItalicsButton
            // 
            this.ItalicsButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.ItalicsButton.Image = global::PaintDotNet.Effects.Properties.Resources.Italic;
            this.ItalicsButton.Name = "ItalicsButton";
            this.ItalicsButton.Size = new System.Drawing.Size(23, 22);
            this.ItalicsButton.Text = "Italics";
            this.ItalicsButton.ToolTipText = "Italics (Ctrl+I)";
            this.ItalicsButton.Click += new System.EventHandler(this.ItalicsButton_Click);
            // 
            // UnderlineButton
            // 
            this.UnderlineButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.UnderlineButton.Image = global::PaintDotNet.Effects.Properties.Resources.Underline;
            this.UnderlineButton.Name = "UnderlineButton";
            this.UnderlineButton.Size = new System.Drawing.Size(23, 22);
            this.UnderlineButton.Text = "Underline";
            this.UnderlineButton.ToolTipText = "Underline (Ctrl+U)";
            this.UnderlineButton.Click += new System.EventHandler(this.UnderlineButton_Click);
            // 
            // SuperScriptButton
            // 
            this.SuperScriptButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.SuperScriptButton.Image = global::PaintDotNet.Effects.Properties.Resources.SuperScript;
            this.SuperScriptButton.Name = "SuperScriptButton";
            this.SuperScriptButton.Size = new System.Drawing.Size(23, 22);
            this.SuperScriptButton.Text = "SuperScript";
            this.SuperScriptButton.ToolTipText = "Superscript";
            this.SuperScriptButton.Click += new System.EventHandler(this.SuperScriptButton_Click);
            // 
            // SubScriptButton
            // 
            this.SubScriptButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.SubScriptButton.Image = global::PaintDotNet.Effects.Properties.Resources.Subscript;
            this.SubScriptButton.Name = "SubScriptButton";
            this.SubScriptButton.Size = new System.Drawing.Size(23, 22);
            this.SubScriptButton.Text = "Subscript";
            this.SubScriptButton.Click += new System.EventHandler(this.SubScriptButton_Click);
            // 
            // LargeFontButton
            // 
            this.LargeFontButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.LargeFontButton.Image = global::PaintDotNet.Effects.Properties.Resources.IncreaseFontSize;
            this.LargeFontButton.Name = "LargeFontButton";
            this.LargeFontButton.Size = new System.Drawing.Size(23, 22);
            this.LargeFontButton.Text = "Increase Font Size";
            this.LargeFontButton.Click += new System.EventHandler(this.LargeFontButton_Click);
            // 
            // SmallFontButton
            // 
            this.SmallFontButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.SmallFontButton.Image = global::PaintDotNet.Effects.Properties.Resources.DecreaseFontSize;
            this.SmallFontButton.Name = "SmallFontButton";
            this.SmallFontButton.Size = new System.Drawing.Size(23, 22);
            this.SmallFontButton.Text = "Decrease Font Size";
            this.SmallFontButton.Click += new System.EventHandler(this.SmallFontButton_Click);
            // 
            // ColorButton
            // 
            this.ColorButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.ColorButton.Image = global::PaintDotNet.Effects.Properties.Resources.ColorWheel;
            this.ColorButton.Name = "ColorButton";
            this.ColorButton.Size = new System.Drawing.Size(23, 22);
            this.ColorButton.Text = "Color";
            this.ColorButton.ToolTipText = "Color (F8)";
            this.ColorButton.Click += new System.EventHandler(this.ColorButton_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
            // 
            // InsertImageButton
            // 
            this.InsertImageButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.InsertImageButton.Image = global::PaintDotNet.Effects.Properties.Resources.InsertPicture;
            this.InsertImageButton.Name = "InsertImageButton";
            this.InsertImageButton.Size = new System.Drawing.Size(23, 22);
            this.InsertImageButton.Text = "Insert Image";
            this.InsertImageButton.Click += new System.EventHandler(this.InsertImageButton_Click);
            // 
            // BulletButton
            // 
            this.BulletButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.BulletButton.Image = global::PaintDotNet.Effects.Properties.Resources.Bullets;
            this.BulletButton.Name = "BulletButton";
            this.BulletButton.Size = new System.Drawing.Size(23, 22);
            this.BulletButton.Text = "Bullets";
            this.BulletButton.Click += new System.EventHandler(this.BulletButton_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // IndentButton
            // 
            this.IndentButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.IndentButton.Image = global::PaintDotNet.Effects.Properties.Resources.Indent;
            this.IndentButton.Name = "IndentButton";
            this.IndentButton.Size = new System.Drawing.Size(23, 22);
            this.IndentButton.Text = "Indent";
            this.IndentButton.Click += new System.EventHandler(this.IndentButton_Click);
            // 
            // UnindentButton
            // 
            this.UnindentButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.UnindentButton.Image = global::PaintDotNet.Effects.Properties.Resources.Unindent;
            this.UnindentButton.Name = "UnindentButton";
            this.UnindentButton.Size = new System.Drawing.Size(23, 22);
            this.UnindentButton.Text = "Unindent";
            this.UnindentButton.Click += new System.EventHandler(this.UnindentButton_Click);
            // 
            // ParagraphLeftButton
            // 
            this.ParagraphLeftButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.ParagraphLeftButton.Image = global::PaintDotNet.Effects.Properties.Resources.ParagraphLeft;
            this.ParagraphLeftButton.Name = "ParagraphLeftButton";
            this.ParagraphLeftButton.Size = new System.Drawing.Size(23, 22);
            this.ParagraphLeftButton.Text = "Align Left";
            this.ParagraphLeftButton.Click += new System.EventHandler(this.ParagraphLeftButton_Click);
            // 
            // CenterButton
            // 
            this.CenterButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.CenterButton.Image = global::PaintDotNet.Effects.Properties.Resources.ParagraphCenter;
            this.CenterButton.Name = "CenterButton";
            this.CenterButton.Size = new System.Drawing.Size(23, 22);
            this.CenterButton.Text = "Center";
            this.CenterButton.Click += new System.EventHandler(this.CenterButton_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.AdjustmentRadio);
            this.panel1.Controls.Add(this.EffectRadio);
            this.panel1.Location = new System.Drawing.Point(9, 27);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(240, 30);
            this.panel1.TabIndex = 0;
            // 
            // PreviewLabel
            // 
            this.PreviewLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.PreviewLabel.AutoSize = true;
            this.PreviewLabel.Location = new System.Drawing.Point(555, 29);
            this.PreviewLabel.Name = "PreviewLabel";
            this.PreviewLabel.Size = new System.Drawing.Size(110, 13);
            this.PreviewLabel.TabIndex = 85;
            this.PreviewLabel.Text = "Preview help content:";
            // 
            // PlainTextLabel
            // 
            this.PlainTextLabel.AutoSize = true;
            this.PlainTextLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.PlainTextLabel.Location = new System.Drawing.Point(309, 97);
            this.PlainTextLabel.Name = "PlainTextLabel";
            this.PlainTextLabel.Size = new System.Drawing.Size(42, 24);
            this.PlainTextLabel.TabIndex = 80;
            this.PlainTextLabel.Text = "(Use Tab\r\nor Enter)";
            // 
            // WarningLabel
            // 
            this.WarningLabel.AutoSize = true;
            this.WarningLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.WarningLabel.ForeColor = System.Drawing.Color.Red;
            this.WarningLabel.Location = new System.Drawing.Point(366, 136);
            this.WarningLabel.Name = "WarningLabel";
            this.WarningLabel.Size = new System.Drawing.Size(154, 13);
            this.WarningLabel.TabIndex = 86;
            this.WarningLabel.Text = "Close WordPad to continue!";
            // 
            // MenuIcon
            // 
            this.MenuIcon.Location = new System.Drawing.Point(86, 102);
            this.MenuIcon.Name = "MenuIcon";
            this.MenuIcon.Size = new System.Drawing.Size(16, 16);
            this.MenuIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.MenuIcon.TabIndex = 3;
            this.MenuIcon.TabStop = false;
            // 
            // GenSlnButton
            // 
            this.GenSlnButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.GenSlnButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.GenSlnButton.Location = new System.Drawing.Point(360, 600);
            this.GenSlnButton.Name = "GenSlnButton";
            this.GenSlnButton.Size = new System.Drawing.Size(125, 23);
            this.GenSlnButton.TabIndex = 24;
            this.GenSlnButton.Text = "Generate VS Solution";
            this.GenSlnButton.UseVisualStyleBackColor = true;
            this.GenSlnButton.Click += new System.EventHandler(this.GenSlnButton_Click);
            // 
            // sampleImage
            // 
            this.sampleImage.Location = new System.Drawing.Point(30, 472);
            this.sampleImage.Name = "sampleImage";
            this.sampleImage.Size = new System.Drawing.Size(200, 150);
            this.sampleImage.TabIndex = 89;
            this.sampleImage.TabStop = false;
            // 
            // sampleLabel
            // 
            this.sampleLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold);
            this.sampleLabel.Location = new System.Drawing.Point(6, 456);
            this.sampleLabel.Name = "sampleLabel";
            this.sampleLabel.Size = new System.Drawing.Size(243, 124);
            this.sampleLabel.TabIndex = 90;
            this.sampleLabel.Text = "Sample Image Detected:";
            // 
            // forceLegacyRoiBox
            // 
            this.forceLegacyRoiBox.AutoSize = true;
            this.forceLegacyRoiBox.Location = new System.Drawing.Point(19, 428);
            this.forceLegacyRoiBox.Name = "forceLegacyRoiBox";
            this.forceLegacyRoiBox.Size = new System.Drawing.Size(83, 17);
            this.forceLegacyRoiBox.TabIndex = 13;
            this.forceLegacyRoiBox.Text = "Legacy ROI";
            this.forceLegacyRoiBox.UseVisualStyleBackColor = true;
            this.forceLegacyRoiBox.CheckedChanged += new System.EventHandler(this.ForceROI_CheckedChanged);
            // 
            // forceSingleRenderBox
            // 
            this.forceSingleRenderBox.AutoSize = true;
            this.forceSingleRenderBox.Location = new System.Drawing.Point(132, 428);
            this.forceSingleRenderBox.Name = "forceSingleRenderBox";
            this.forceSingleRenderBox.Size = new System.Drawing.Size(113, 17);
            this.forceSingleRenderBox.TabIndex = 14;
            this.forceSingleRenderBox.Text = "Single Render Call";
            this.forceSingleRenderBox.UseVisualStyleBackColor = true;
            this.forceSingleRenderBox.CheckedChanged += new System.EventHandler(this.ForceROI_CheckedChanged);
            // 
            // BuildForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.ButtonCancel;
            this.ClientSize = new System.Drawing.Size(717, 637);
            this.Controls.Add(this.forceSingleRenderBox);
            this.Controls.Add(this.forceLegacyRoiBox);
            this.Controls.Add(this.sampleImage);
            this.Controls.Add(this.sampleLabel);
            this.Controls.Add(this.GenSlnButton);
            this.Controls.Add(this.ViewSourceButton);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.WarningLabel);
            this.Controls.Add(this.PreviewLabel);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.RichHelpContent);
            this.Controls.Add(this.HelpPlainText);
            this.Controls.Add(this.PlainTextLabel);
            this.Controls.Add(this.HelpURL);
            this.Controls.Add(this.radioButtonRich);
            this.Controls.Add(this.radioButtonPlain);
            this.Controls.Add(this.radioButtonURL);
            this.Controls.Add(this.radioButtonNone);
            this.Controls.Add(this.PreviewHelpButton);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.ForceSingleThreadedBox);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.ForceAliasSelectionBox);
            this.Controls.Add(this.KeyWordsBox);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.DescriptionBox);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.WindowTitleText);
            this.Controls.Add(this.label20);
            this.Controls.Add(this.label21);
            this.Controls.Add(this.label24);
            this.Controls.Add(this.label23);
            this.Controls.Add(this.label19);
            this.Controls.Add(this.SupportURL);
            this.Controls.Add(this.label17);
            this.Controls.Add(this.MinorVersion);
            this.Controls.Add(this.MajorVersion);
            this.Controls.Add(this.label16);
            this.Controls.Add(this.AuthorName);
            this.Controls.Add(this.ButtonIcon);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.SubMenuName);
            this.Controls.Add(this.MenuIcon);
            this.Controls.Add(this.MenuName);
            this.Controls.Add(this.ButtonSave);
            this.Controls.Add(this.ButtonCancel);
            this.Controls.Add(this.DecimalSymbol);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(733, 655);
            this.Name = "BuildForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Building DLL";
            ((System.ComponentModel.ISupportInitialize)(this.MajorVersion)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.MinorVersion)).EndInit();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.MenuIcon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.sampleImage)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

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
        private System.Windows.Forms.ToolStripButton BoldButton;
        private System.Windows.Forms.ToolStripButton ItalicsButton;
        private System.Windows.Forms.ToolStripButton UnderlineButton;
        private System.Windows.Forms.ToolStripButton SuperScriptButton;
        private System.Windows.Forms.ToolStripButton SubScriptButton;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label PreviewLabel;
        private System.Windows.Forms.ToolStripButton LargeFontButton;
        private System.Windows.Forms.ToolStripButton SmallFontButton;
        private System.Windows.Forms.ToolStripButton IndentButton;
        private System.Windows.Forms.ToolStripButton UnindentButton;
        private System.Windows.Forms.ToolStripButton ParagraphLeftButton;
        private System.Windows.Forms.ToolStripButton CenterButton;
        private System.Windows.Forms.ToolStripButton BulletButton;
        private System.Windows.Forms.ToolStripButton OpenButton;
        private System.Windows.Forms.ToolStripButton SaveButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.Label PlainTextLabel;
        private System.Windows.Forms.ToolStripButton WordPadButton;
        private System.Windows.Forms.ToolStripButton InsertImageButton;
        private System.Windows.Forms.ToolStripButton ColorButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.Label WarningLabel;
        private System.Windows.Forms.Button ViewSourceButton;
        private System.Windows.Forms.Button GenSlnButton;
        private System.Windows.Forms.PictureBox sampleImage;
        private System.Windows.Forms.Label sampleLabel;
        private System.Windows.Forms.CheckBox forceLegacyRoiBox;
        private System.Windows.Forms.CheckBox forceSingleRenderBox;
    }
}