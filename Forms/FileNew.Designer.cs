namespace PdnCodeLab
{
    partial class FileNew
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
            this.button1 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.BasicStyle = new System.Windows.Forms.RadioButton();
            this.AdvancedStyle = new System.Windows.Forms.RadioButton();
            this.label2 = new System.Windows.Forms.Label();
            this.CenterCode = new System.Windows.Forms.CheckBox();
            this.PrimaryColorCode = new System.Windows.Forms.CheckBox();
            this.PenWidthCode = new System.Windows.Forms.CheckBox();
            this.SelectionCode = new System.Windows.Forms.CheckBox();
            this.DoIt = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.Label();
            this.HsvColorMode = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.CustomHelp = new System.Windows.Forms.CheckBox();
            this.PaletteCode = new System.Windows.Forms.CheckBox();
            this.label10 = new System.Windows.Forms.Label();
            this.FAS = new System.Windows.Forms.CheckBox();
            this.ST = new System.Windows.Forms.CheckBox();
            this.NoStyle = new System.Windows.Forms.RadioButton();
            this.SRC = new System.Windows.Forms.CheckBox();
            this.LROI = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.flowList = new System.Windows.Forms.ListBox();
            this.Delete = new System.Windows.Forms.Button();
            this.MoveDown = new System.Windows.Forms.Button();
            this.MoveUp = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.categoryBox = new System.Windows.Forms.ComboBox();
            this.label11 = new System.Windows.Forms.Label();
            this.effectLabel = new System.Windows.Forms.Label();
            this.effectBox = new System.Windows.Forms.ComboBox();
            this.sourceLabel = new System.Windows.Forms.Label();
            this.sourceBox = new System.Windows.Forms.ComboBox();
            this.destinationLabel = new System.Windows.Forms.Label();
            this.destinationBox = new System.Windows.Forms.ComboBox();
            this.bottomLabel = new System.Windows.Forms.Label();
            this.bottomBox = new System.Windows.Forms.ComboBox();
            this.updateButton = new System.Windows.Forms.Button();
            this.addButton = new System.Windows.Forms.Button();
            this.DefaultColorComboBox = new System.Windows.Forms.ComboBox();
            this.blendLabel = new System.Windows.Forms.Label();
            this.blendBox = new System.Windows.Forms.ComboBox();
            this.pixelOpBox = new System.Windows.Forms.ComboBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.ElementTab = new System.Windows.Forms.TabPage();
            this.panel1 = new System.Windows.Forms.Panel();
            this.tabControl1.SuspendLayout();
            this.ElementTab.SuspendLayout();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.button1.Location = new System.Drawing.Point(684, 471);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 24);
            this.button1.TabIndex = 1;
            this.button1.Text = "Cancel";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label1.Location = new System.Drawing.Point(21, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(66, 15);
            this.label1.TabIndex = 2;
            this.label1.Text = "Loop style:";
            // 
            // BasicStyle
            // 
            this.BasicStyle.AutoSize = true;
            this.BasicStyle.Checked = true;
            this.BasicStyle.Location = new System.Drawing.Point(41, 35);
            this.BasicStyle.Name = "BasicStyle";
            this.BasicStyle.Size = new System.Drawing.Size(52, 19);
            this.BasicStyle.TabIndex = 3;
            this.BasicStyle.TabStop = true;
            this.BasicStyle.Text = "Basic";
            this.BasicStyle.UseVisualStyleBackColor = true;
            // 
            // AdvancedStyle
            // 
            this.AdvancedStyle.AutoSize = true;
            this.AdvancedStyle.Location = new System.Drawing.Point(41, 58);
            this.AdvancedStyle.Name = "AdvancedStyle";
            this.AdvancedStyle.Size = new System.Drawing.Size(170, 19);
            this.AdvancedStyle.TabIndex = 4;
            this.AdvancedStyle.Text = "Advanced (unsafe pointers)";
            this.AdvancedStyle.UseVisualStyleBackColor = true;
            this.AdvancedStyle.CheckedChanged += new System.EventHandler(this.AdvancedStyle_CheckedChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label2.Location = new System.Drawing.Point(21, 115);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(214, 15);
            this.label2.TabIndex = 6;
            this.label2.Text = "Choose the variables you might need:";
            // 
            // CenterCode
            // 
            this.CenterCode.AutoSize = true;
            this.CenterCode.Location = new System.Drawing.Point(40, 134);
            this.CenterCode.Name = "CenterCode";
            this.CenterCode.Size = new System.Drawing.Size(246, 19);
            this.CenterCode.TabIndex = 7;
            this.CenterCode.Text = "Center of the selection (CenterX, CenterY)";
            this.CenterCode.UseVisualStyleBackColor = true;
            // 
            // PrimaryColorCode
            // 
            this.PrimaryColorCode.AutoSize = true;
            this.PrimaryColorCode.Location = new System.Drawing.Point(40, 158);
            this.PrimaryColorCode.Name = "PrimaryColorCode";
            this.PrimaryColorCode.Size = new System.Drawing.Size(212, 19);
            this.PrimaryColorCode.TabIndex = 8;
            this.PrimaryColorCode.Text = "Primary Color and Secondary Color";
            this.PrimaryColorCode.UseVisualStyleBackColor = true;
            // 
            // PenWidthCode
            // 
            this.PenWidthCode.AutoSize = true;
            this.PenWidthCode.Location = new System.Drawing.Point(40, 204);
            this.PenWidthCode.Name = "PenWidthCode";
            this.PenWidthCode.Size = new System.Drawing.Size(91, 19);
            this.PenWidthCode.TabIndex = 10;
            this.PenWidthCode.Text = "Brush Width";
            this.PenWidthCode.UseVisualStyleBackColor = true;
            // 
            // SelectionCode
            // 
            this.SelectionCode.AutoSize = true;
            this.SelectionCode.Location = new System.Drawing.Point(40, 265);
            this.SelectionCode.Name = "SelectionCode";
            this.SelectionCode.Size = new System.Drawing.Size(215, 19);
            this.SelectionCode.TabIndex = 12;
            this.SelectionCode.Text = "Selection boundary (marching ants)";
            this.SelectionCode.UseVisualStyleBackColor = true;
            // 
            // DoIt
            // 
            this.DoIt.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.DoIt.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.DoIt.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.DoIt.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.DoIt.Location = new System.Drawing.Point(563, 471);
            this.DoIt.Name = "DoIt";
            this.DoIt.Size = new System.Drawing.Size(115, 24);
            this.DoIt.TabIndex = 0;
            this.DoIt.Text = "Generate Code";
            this.DoIt.UseVisualStyleBackColor = true;
            this.DoIt.Click += new System.EventHandler(this.DoIt_Click);
            // 
            // textBox1
            // 
            this.textBox1.AutoSize = true;
            this.textBox1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.textBox1.Location = new System.Drawing.Point(21, 244);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(85, 15);
            this.textBox1.TabIndex = 11;
            this.textBox1.Text = "Code samples:";
            // 
            // HsvColorMode
            // 
            this.HsvColorMode.AutoSize = true;
            this.HsvColorMode.Location = new System.Drawing.Point(40, 289);
            this.HsvColorMode.Name = "HsvColorMode";
            this.HsvColorMode.Size = new System.Drawing.Size(120, 19);
            this.HsvColorMode.TabIndex = 13;
            this.HsvColorMode.Text = "HSV Color Editing";
            this.HsvColorMode.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label4.Location = new System.Drawing.Point(298, 13);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(179, 15);
            this.label4.TabIndex = 20;
            this.label4.Text = "Pixel flow for complex effects:";
            // 
            // CustomHelp
            // 
            this.CustomHelp.AutoSize = true;
            this.CustomHelp.Location = new System.Drawing.Point(40, 312);
            this.CustomHelp.Name = "CustomHelp";
            this.CustomHelp.Size = new System.Drawing.Size(96, 19);
            this.CustomHelp.TabIndex = 14;
            this.CustomHelp.Text = "Custom Help";
            this.CustomHelp.UseVisualStyleBackColor = true;
            // 
            // PaletteCode
            // 
            this.PaletteCode.AutoSize = true;
            this.PaletteCode.Location = new System.Drawing.Point(40, 181);
            this.PaletteCode.Name = "PaletteCode";
            this.PaletteCode.Size = new System.Drawing.Size(169, 19);
            this.PaletteCode.TabIndex = 9;
            this.PaletteCode.Text = "Current and Default Palette";
            this.PaletteCode.UseVisualStyleBackColor = true;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(367, 541);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(10, 15);
            this.label10.TabIndex = 31;
            this.label10.Text = " ";
            // 
            // FAS
            // 
            this.FAS.AutoSize = true;
            this.FAS.Location = new System.Drawing.Point(40, 370);
            this.FAS.Name = "FAS";
            this.FAS.Size = new System.Drawing.Size(147, 19);
            this.FAS.TabIndex = 16;
            this.FAS.Text = "Force Aliased Selection";
            this.FAS.UseVisualStyleBackColor = true;
            // 
            // ST
            // 
            this.ST.AutoSize = true;
            this.ST.Location = new System.Drawing.Point(40, 417);
            this.ST.Name = "ST";
            this.ST.Size = new System.Drawing.Size(110, 19);
            this.ST.TabIndex = 18;
            this.ST.Text = "Single Threaded";
            this.ST.UseVisualStyleBackColor = true;
            // 
            // NoStyle
            // 
            this.NoStyle.AutoSize = true;
            this.NoStyle.Location = new System.Drawing.Point(41, 81);
            this.NoStyle.Name = "NoStyle";
            this.NoStyle.Size = new System.Drawing.Size(92, 19);
            this.NoStyle.TabIndex = 5;
            this.NoStyle.TabStop = true;
            this.NoStyle.Text = "None (GDI+)";
            this.NoStyle.UseVisualStyleBackColor = true;
            this.NoStyle.CheckedChanged += new System.EventHandler(this.NoStyle_CheckedChanged);
            // 
            // SRC
            // 
            this.SRC.AutoSize = true;
            this.SRC.Location = new System.Drawing.Point(40, 394);
            this.SRC.Name = "SRC";
            this.SRC.Size = new System.Drawing.Size(121, 19);
            this.SRC.TabIndex = 17;
            this.SRC.Text = "Single Render Call";
            this.SRC.UseVisualStyleBackColor = true;
            this.SRC.CheckedChanged += new System.EventHandler(this.ForceROI_CheckedChanged);
            // 
            // LROI
            // 
            this.LROI.AutoSize = true;
            this.LROI.Location = new System.Drawing.Point(40, 441);
            this.LROI.Name = "LROI";
            this.LROI.Size = new System.Drawing.Size(85, 19);
            this.LROI.TabIndex = 19;
            this.LROI.Text = "Legacy ROI";
            this.LROI.UseVisualStyleBackColor = true;
            this.LROI.CheckedChanged += new System.EventHandler(this.ForceROI_CheckedChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label3.Location = new System.Drawing.Point(21, 352);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(95, 15);
            this.label3.TabIndex = 15;
            this.label3.Text = "Render options:";
            // 
            // flowList
            // 
            this.flowList.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.flowList.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.flowList.FormattingEnabled = true;
            this.flowList.IntegralHeight = false;
            this.flowList.ItemHeight = 64;
            this.flowList.Location = new System.Drawing.Point(301, 35);
            this.flowList.Name = "flowList";
            this.flowList.Size = new System.Drawing.Size(427, 302);
            this.flowList.TabIndex = 21;
            this.flowList.Click += new System.EventHandler(this.flowList_Click);
            this.flowList.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.flowList_DrawItem);
            this.flowList.SelectedIndexChanged += new System.EventHandler(this.flowList_SelectedIndexChanged);
            // 
            // Delete
            // 
            this.Delete.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.Delete.ForeColor = System.Drawing.Color.Red;
            this.Delete.Location = new System.Drawing.Point(736, 106);
            this.Delete.Name = "Delete";
            this.Delete.Size = new System.Drawing.Size(23, 23);
            this.Delete.TabIndex = 24;
            this.Delete.Text = "✗";
            this.toolTip1.SetToolTip(this.Delete, "Delete");
            this.Delete.UseVisualStyleBackColor = true;
            this.Delete.Click += new System.EventHandler(this.Delete_Click);
            // 
            // MoveDown
            // 
            this.MoveDown.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.MoveDown.Location = new System.Drawing.Point(736, 62);
            this.MoveDown.Name = "MoveDown";
            this.MoveDown.Size = new System.Drawing.Size(23, 23);
            this.MoveDown.TabIndex = 23;
            this.MoveDown.Text = "▼";
            this.toolTip1.SetToolTip(this.MoveDown, "Move Down");
            this.MoveDown.UseVisualStyleBackColor = true;
            this.MoveDown.Click += new System.EventHandler(this.MoveDown_Click);
            // 
            // MoveUp
            // 
            this.MoveUp.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.MoveUp.Location = new System.Drawing.Point(736, 34);
            this.MoveUp.Name = "MoveUp";
            this.MoveUp.Size = new System.Drawing.Size(23, 23);
            this.MoveUp.TabIndex = 22;
            this.MoveUp.Text = "▲";
            this.toolTip1.SetToolTip(this.MoveUp, "Move Up");
            this.MoveUp.UseVisualStyleBackColor = true;
            this.MoveUp.Click += new System.EventHandler(this.MoveUp_Click);
            // 
            // categoryBox
            // 
            this.categoryBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.categoryBox.FormattingEnabled = true;
            this.categoryBox.Items.AddRange(new object[] {
            "Effect",
            "Blend",
            "Pixel Op",
            "Fill",
            "Copy"});
            this.categoryBox.Location = new System.Drawing.Point(64, 12);
            this.categoryBox.Name = "categoryBox";
            this.categoryBox.Size = new System.Drawing.Size(92, 23);
            this.categoryBox.TabIndex = 26;
            this.categoryBox.SelectedIndexChanged += new System.EventHandler(this.categoryBox_SelectedIndexChanged);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.ForeColor = System.Drawing.SystemColors.WindowText;
            this.label11.Location = new System.Drawing.Point(6, 16);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(58, 15);
            this.label11.TabIndex = 25;
            this.label11.Text = "Category:";
            // 
            // effectLabel
            // 
            this.effectLabel.AutoSize = true;
            this.effectLabel.ForeColor = System.Drawing.SystemColors.WindowText;
            this.effectLabel.Location = new System.Drawing.Point(174, 15);
            this.effectLabel.Name = "effectLabel";
            this.effectLabel.Size = new System.Drawing.Size(40, 15);
            this.effectLabel.TabIndex = 40;
            this.effectLabel.Text = "Effect:";
            // 
            // effectBox
            // 
            this.effectBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.effectBox.FormattingEnabled = true;
            this.effectBox.Items.AddRange(new object[] {
            "Add Noise",
            "Brightness/Contrast",
            "Bulge",
            "Clipboard",
            "Clouds",
            "Crystalize",
            "Dents",
            "Edge Detect",
            "Emboss",
            "Frosted Glass",
            "Gaussian Blur",
            "Glow",
            "Hue/Saturation",
            "Ink Sketch",
            "Julia",
            "Mandelbrot",
            "Median",
            "Motion Blur",
            "Oil Painting",
            "Outline",
            "Pencil Sketch",
            "Pixelate",
            "Polar Inversion",
            "Posterize",
            "Radial Blur",
            "Reduce Noise",
            "Relief",
            "Rotate Zoom",
            "Sepia",
            "Sharpen",
            "Soften Portrait",
            "Surface Blur",
            "Tile Reflection",
            "Twist",
            "Unfocus",
            "Vignette",
            "Zoom Blur"});
            this.effectBox.Location = new System.Drawing.Point(218, 12);
            this.effectBox.Name = "effectBox";
            this.effectBox.Size = new System.Drawing.Size(179, 23);
            this.effectBox.TabIndex = 30;
            this.effectBox.SelectedIndexChanged += new System.EventHandler(this.effectBox_SelectedIndexChanged);
            // 
            // sourceLabel
            // 
            this.sourceLabel.AutoSize = true;
            this.sourceLabel.ForeColor = System.Drawing.SystemColors.WindowText;
            this.sourceLabel.Location = new System.Drawing.Point(173, 52);
            this.sourceLabel.Name = "sourceLabel";
            this.sourceLabel.Size = new System.Drawing.Size(74, 15);
            this.sourceLabel.TabIndex = 33;
            this.sourceLabel.Text = "Source layer:";
            // 
            // sourceBox
            // 
            this.sourceBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.sourceBox.FormattingEnabled = true;
            this.sourceBox.Items.AddRange(new object[] {
            "SRC",
            "WRK",
            "AUX"});
            this.sourceBox.Location = new System.Drawing.Point(250, 49);
            this.sourceBox.Name = "sourceBox";
            this.sourceBox.Size = new System.Drawing.Size(51, 23);
            this.sourceBox.TabIndex = 34;
            // 
            // destinationLabel
            // 
            this.destinationLabel.AutoSize = true;
            this.destinationLabel.ForeColor = System.Drawing.SystemColors.WindowText;
            this.destinationLabel.Location = new System.Drawing.Point(6, 52);
            this.destinationLabel.Name = "destinationLabel";
            this.destinationLabel.Size = new System.Drawing.Size(98, 15);
            this.destinationLabel.TabIndex = 31;
            this.destinationLabel.Text = "Destination layer:";
            // 
            // destinationBox
            // 
            this.destinationBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.destinationBox.FormattingEnabled = true;
            this.destinationBox.Items.AddRange(new object[] {
            "WRK",
            "AUX",
            "DST"});
            this.destinationBox.Location = new System.Drawing.Point(105, 49);
            this.destinationBox.Name = "destinationBox";
            this.destinationBox.Size = new System.Drawing.Size(51, 23);
            this.destinationBox.TabIndex = 32;
            // 
            // bottomLabel
            // 
            this.bottomLabel.AutoSize = true;
            this.bottomLabel.ForeColor = System.Drawing.SystemColors.WindowText;
            this.bottomLabel.Location = new System.Drawing.Point(316, 52);
            this.bottomLabel.Name = "bottomLabel";
            this.bottomLabel.Size = new System.Drawing.Size(78, 15);
            this.bottomLabel.TabIndex = 35;
            this.bottomLabel.Text = "Bottom layer:";
            // 
            // bottomBox
            // 
            this.bottomBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.bottomBox.FormattingEnabled = true;
            this.bottomBox.Items.AddRange(new object[] {
            "SRC",
            "WRK",
            "AUX"});
            this.bottomBox.Location = new System.Drawing.Point(393, 49);
            this.bottomBox.Name = "bottomBox";
            this.bottomBox.Size = new System.Drawing.Size(51, 23);
            this.bottomBox.TabIndex = 36;
            // 
            // updateButton
            // 
            this.updateButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.updateButton.Location = new System.Drawing.Point(651, 343);
            this.updateButton.Name = "updateButton";
            this.updateButton.Size = new System.Drawing.Size(51, 23);
            this.updateButton.TabIndex = 37;
            this.updateButton.Text = "Update";
            this.updateButton.UseVisualStyleBackColor = true;
            this.updateButton.Click += new System.EventHandler(this.updateButton_Click);
            // 
            // addButton
            // 
            this.addButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.addButton.Location = new System.Drawing.Point(708, 343);
            this.addButton.Name = "addButton";
            this.addButton.Size = new System.Drawing.Size(51, 23);
            this.addButton.TabIndex = 38;
            this.addButton.Text = "Add";
            this.addButton.UseVisualStyleBackColor = true;
            this.addButton.Click += new System.EventHandler(this.addButton_Click);
            // 
            // DefaultColorComboBox
            // 
            this.DefaultColorComboBox.BackColor = System.Drawing.SystemColors.Window;
            this.DefaultColorComboBox.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.DefaultColorComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.DefaultColorComboBox.DropDownWidth = 150;
            this.DefaultColorComboBox.Location = new System.Drawing.Point(177, 12);
            this.DefaultColorComboBox.MaxDropDownItems = 10;
            this.DefaultColorComboBox.Name = "DefaultColorComboBox";
            this.DefaultColorComboBox.Size = new System.Drawing.Size(149, 24);
            this.DefaultColorComboBox.TabIndex = 50;
            this.DefaultColorComboBox.Visible = false;
            this.DefaultColorComboBox.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.DefaultColorComboBox_DrawItem);
            // 
            // blendLabel
            // 
            this.blendLabel.AutoSize = true;
            this.blendLabel.ForeColor = System.Drawing.SystemColors.WindowText;
            this.blendLabel.Location = new System.Drawing.Point(174, 16);
            this.blendLabel.Name = "blendLabel";
            this.blendLabel.Size = new System.Drawing.Size(41, 15);
            this.blendLabel.TabIndex = 28;
            this.blendLabel.Text = "Mode:";
            this.blendLabel.Visible = false;
            // 
            // blendBox
            // 
            this.blendBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.blendBox.FormattingEnabled = true;
            this.blendBox.Items.AddRange(new object[] {
            "User selected",
            "Normal",
            "Multiply",
            "Darken",
            "Additive",
            "ColorBurn",
            "ColorDodge",
            "Difference",
            "Glow",
            "Lighten",
            "Negation",
            "Overlay",
            "Reflect",
            "Screen",
            "Xor"});
            this.blendBox.Location = new System.Drawing.Point(220, 12);
            this.blendBox.Name = "blendBox";
            this.blendBox.Size = new System.Drawing.Size(106, 23);
            this.blendBox.TabIndex = 29;
            this.blendBox.Visible = false;
            // 
            // pixelOpBox
            // 
            this.pixelOpBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.pixelOpBox.FormattingEnabled = true;
            this.pixelOpBox.Items.AddRange(new object[] {
            "Desaturate",
            "Invert"});
            this.pixelOpBox.Location = new System.Drawing.Point(177, 12);
            this.pixelOpBox.Name = "pixelOpBox";
            this.pixelOpBox.Size = new System.Drawing.Size(149, 23);
            this.pixelOpBox.TabIndex = 27;
            this.pixelOpBox.Visible = false;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.ElementTab);
            this.tabControl1.Location = new System.Drawing.Point(301, 350);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(459, 107);
            this.tabControl1.TabIndex = 51;
            this.tabControl1.TabStop = false;
            // 
            // ElementTab
            // 
            this.ElementTab.Controls.Add(this.label11);
            this.ElementTab.Controls.Add(this.pixelOpBox);
            this.ElementTab.Controls.Add(this.blendBox);
            this.ElementTab.Controls.Add(this.bottomBox);
            this.ElementTab.Controls.Add(this.categoryBox);
            this.ElementTab.Controls.Add(this.bottomLabel);
            this.ElementTab.Controls.Add(this.blendLabel);
            this.ElementTab.Controls.Add(this.sourceBox);
            this.ElementTab.Controls.Add(this.destinationBox);
            this.ElementTab.Controls.Add(this.sourceLabel);
            this.ElementTab.Controls.Add(this.effectLabel);
            this.ElementTab.Controls.Add(this.destinationLabel);
            this.ElementTab.Controls.Add(this.DefaultColorComboBox);
            this.ElementTab.Controls.Add(this.effectBox);
            this.ElementTab.Location = new System.Drawing.Point(4, 24);
            this.ElementTab.Name = "ElementTab";
            this.ElementTab.Padding = new System.Windows.Forms.Padding(3);
            this.ElementTab.Size = new System.Drawing.Size(451, 79);
            this.ElementTab.TabIndex = 0;
            this.ElementTab.Text = "Element";
            this.ElementTab.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.Location = new System.Drawing.Point(758, 370);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(20, 90);
            this.panel1.TabIndex = 52;
            // 
            // FileNew
            // 
            this.AcceptButton = this.DoIt;
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.CancelButton = this.button1;
            this.ClientSize = new System.Drawing.Size(774, 506);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.addButton);
            this.Controls.Add(this.updateButton);
            this.Controls.Add(this.Delete);
            this.Controls.Add(this.MoveDown);
            this.Controls.Add(this.MoveUp);
            this.Controls.Add(this.flowList);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.LROI);
            this.Controls.Add(this.SRC);
            this.Controls.Add(this.NoStyle);
            this.Controls.Add(this.ST);
            this.Controls.Add(this.FAS);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.PaletteCode);
            this.Controls.Add(this.CustomHelp);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.HsvColorMode);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.DoIt);
            this.Controls.Add(this.SelectionCode);
            this.Controls.Add(this.PenWidthCode);
            this.Controls.Add(this.PrimaryColorCode);
            this.Controls.Add(this.CenterCode);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.AdvancedStyle);
            this.Controls.Add(this.BasicStyle);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.tabControl1);
            this.IconName = "New";
            this.Name = "FileNew";
            this.Text = "New Source (Template)";
            this.tabControl1.ResumeLayout(false);
            this.ElementTab.ResumeLayout(false);
            this.ElementTab.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RadioButton BasicStyle;
        private System.Windows.Forms.RadioButton AdvancedStyle;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox CenterCode;
        private System.Windows.Forms.CheckBox PrimaryColorCode;
        private System.Windows.Forms.CheckBox PenWidthCode;
        private System.Windows.Forms.CheckBox SelectionCode;
        private System.Windows.Forms.Button DoIt;
        private System.Windows.Forms.Label textBox1;
        private System.Windows.Forms.CheckBox HsvColorMode;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox CustomHelp;
        private System.Windows.Forms.CheckBox PaletteCode;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.CheckBox FAS;
        private System.Windows.Forms.CheckBox ST;
        private System.Windows.Forms.RadioButton NoStyle;
        private System.Windows.Forms.CheckBox SRC;
        private System.Windows.Forms.CheckBox LROI;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ListBox flowList;
        private System.Windows.Forms.Button Delete;
        private System.Windows.Forms.Button MoveDown;
        private System.Windows.Forms.Button MoveUp;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.ComboBox categoryBox;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label effectLabel;
        private System.Windows.Forms.ComboBox effectBox;
        private System.Windows.Forms.Label sourceLabel;
        private System.Windows.Forms.ComboBox sourceBox;
        private System.Windows.Forms.Label destinationLabel;
        private System.Windows.Forms.ComboBox destinationBox;
        private System.Windows.Forms.Label bottomLabel;
        private System.Windows.Forms.ComboBox bottomBox;
        private System.Windows.Forms.Button updateButton;
        private System.Windows.Forms.Button addButton;
        private System.Windows.Forms.ComboBox DefaultColorComboBox;
        private System.Windows.Forms.Label blendLabel;
        private System.Windows.Forms.ComboBox blendBox;
        private System.Windows.Forms.ComboBox pixelOpBox;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage ElementTab;
        private System.Windows.Forms.Panel panel1;
    }
}