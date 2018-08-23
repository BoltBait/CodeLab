namespace PaintDotNet.Effects
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
            this.button1 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.BasicStyle = new System.Windows.Forms.RadioButton();
            this.AdvancedStyle = new System.Windows.Forms.RadioButton();
            this.label2 = new System.Windows.Forms.Label();
            this.CenterCode = new System.Windows.Forms.CheckBox();
            this.PrimaryColorCode = new System.Windows.Forms.CheckBox();
            this.PenWidthCode = new System.Windows.Forms.CheckBox();
            this.SelectionCode = new System.Windows.Forms.CheckBox();
            this.BlendingCode = new System.Windows.Forms.ComboBox();
            this.EffectCode = new System.Windows.Forms.ComboBox();
            this.DoIt = new System.Windows.Forms.Button();
            this.PixelOpCode = new System.Windows.Forms.ComboBox();
            this.textBox1 = new System.Windows.Forms.Label();
            this.HsvColorMode = new System.Windows.Forms.CheckBox();
            this.FinalPixelOpCode = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.srcLabel = new System.Windows.Forms.Label();
            this.dstLabel = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.BlendArrow = new System.Windows.Forms.PictureBox();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.pictureBox4 = new System.Windows.Forms.PictureBox();
            this.pictureBox5 = new System.Windows.Forms.PictureBox();
            this.pictureBox6 = new System.Windows.Forms.PictureBox();
            this.pictureBox7 = new System.Windows.Forms.PictureBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.CustomHelp = new System.Windows.Forms.CheckBox();
            this.PaletteCode = new System.Windows.Forms.CheckBox();
            this.label10 = new System.Windows.Forms.Label();
            this.FAS = new System.Windows.Forms.CheckBox();
            this.ST = new System.Windows.Forms.CheckBox();
            this.SurfaceCode = new System.Windows.Forms.CheckBox();
            this.NoStyle = new System.Windows.Forms.RadioButton();
            ((System.ComponentModel.ISupportInitialize)(this.BlendArrow)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox7)).BeginInit();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.button1.Location = new System.Drawing.Point(287, 499);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 29;
            this.button1.Text = "Cancel";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(16, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(64, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Loop style:";
            // 
            // BasicStyle
            // 
            this.BasicStyle.AutoSize = true;
            this.BasicStyle.Checked = true;
            this.BasicStyle.Location = new System.Drawing.Point(35, 30);
            this.BasicStyle.Name = "BasicStyle";
            this.BasicStyle.Size = new System.Drawing.Size(51, 17);
            this.BasicStyle.TabIndex = 2;
            this.BasicStyle.TabStop = true;
            this.BasicStyle.Text = "Basic";
            this.BasicStyle.UseVisualStyleBackColor = true;
            // 
            // AdvancedStyle
            // 
            this.AdvancedStyle.AutoSize = true;
            this.AdvancedStyle.Location = new System.Drawing.Point(112, 30);
            this.AdvancedStyle.Name = "AdvancedStyle";
            this.AdvancedStyle.Size = new System.Drawing.Size(155, 17);
            this.AdvancedStyle.TabIndex = 3;
            this.AdvancedStyle.Text = "Advanced (unsafe pointers)";
            this.AdvancedStyle.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(16, 60);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(204, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Choose the variables you might need:";
            // 
            // CenterCode
            // 
            this.CenterCode.AutoSize = true;
            this.CenterCode.Location = new System.Drawing.Point(35, 77);
            this.CenterCode.Name = "CenterCode";
            this.CenterCode.Size = new System.Drawing.Size(223, 17);
            this.CenterCode.TabIndex = 6;
            this.CenterCode.Text = "Center of the selection (CenterX, CenterY)";
            this.CenterCode.UseVisualStyleBackColor = true;
            // 
            // PrimaryColorCode
            // 
            this.PrimaryColorCode.AutoSize = true;
            this.PrimaryColorCode.Location = new System.Drawing.Point(35, 101);
            this.PrimaryColorCode.Name = "PrimaryColorCode";
            this.PrimaryColorCode.Size = new System.Drawing.Size(189, 17);
            this.PrimaryColorCode.TabIndex = 7;
            this.PrimaryColorCode.Text = "Primary Color and Secondary Color";
            this.PrimaryColorCode.UseVisualStyleBackColor = true;
            // 
            // PenWidthCode
            // 
            this.PenWidthCode.AutoSize = true;
            this.PenWidthCode.Location = new System.Drawing.Point(230, 124);
            this.PenWidthCode.Name = "PenWidthCode";
            this.PenWidthCode.Size = new System.Drawing.Size(84, 17);
            this.PenWidthCode.TabIndex = 9;
            this.PenWidthCode.Text = "Brush Width";
            this.PenWidthCode.UseVisualStyleBackColor = true;
            // 
            // SelectionCode
            // 
            this.SelectionCode.AutoSize = true;
            this.SelectionCode.Location = new System.Drawing.Point(35, 420);
            this.SelectionCode.Name = "SelectionCode";
            this.SelectionCode.Size = new System.Drawing.Size(192, 17);
            this.SelectionCode.TabIndex = 17;
            this.SelectionCode.Text = "Selection boundary (marching ants)";
            this.SelectionCode.UseVisualStyleBackColor = true;
            // 
            // BlendingCode
            // 
            this.BlendingCode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.BlendingCode.FormattingEnabled = true;
            this.BlendingCode.Items.AddRange(new object[] {
            "Pass Through",
            "User selected blending mode",
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
            this.BlendingCode.Location = new System.Drawing.Point(199, 317);
            this.BlendingCode.Name = "BlendingCode";
            this.BlendingCode.Size = new System.Drawing.Size(163, 21);
            this.BlendingCode.TabIndex = 13;
            this.BlendingCode.SelectedIndexChanged += new System.EventHandler(this.BlendingCode_SelectedIndexChanged);
            // 
            // EffectCode
            // 
            this.EffectCode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.EffectCode.FormattingEnabled = true;
            this.EffectCode.Items.AddRange(new object[] {
            "---------Copy--------->",
            "         Empty------->",
            "     Clipboard------>",
            "        Clouds------->",
            "            Julia------->",
            "     Mandelbrot--->",
            "----Add Noise----->",
            "-------Bulge-------->",
            "-------Contrast------->",
            "------Crystalize------>",
            "--------Dents-------->",
            "----Edge Detect---->",
            "-------Emboss------>",
            "---Frosted Glass--->",
            "---Gaussian Blur-->",
            "--------Glow-------->",
            "-----Ink Sketch------>",
            "------Median------>",
            "----Motion Blur---->",
            "----Oil Painting---->",
            "------Outline------->",
            "---Pencil Sketch--->",
            "------Pixelate------>",
            "--Polar Inversion-->",
            "-----Posterize------>",
            "-----Radial Blur----->",
            "---Reduce Noise--->",
            "--------Relief-------->",
            "--------Sepia-------->",
            "------Sharpen------->",
            "---Soften Portrait--->",
            "----Surface Blur---->",
            "---Tile Reflection--->",
            "-------Twist------->",
            "------Unfocus------>",
            "------Vignette------>",
            "-----Zoom Blur----->"});
            this.EffectCode.Location = new System.Drawing.Point(126, 189);
            this.EffectCode.Name = "EffectCode";
            this.EffectCode.Size = new System.Drawing.Size(141, 21);
            this.EffectCode.TabIndex = 11;
            // 
            // DoIt
            // 
            this.DoIt.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.DoIt.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.DoIt.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DoIt.Location = new System.Drawing.Point(166, 499);
            this.DoIt.Name = "DoIt";
            this.DoIt.Size = new System.Drawing.Size(115, 23);
            this.DoIt.TabIndex = 0;
            this.DoIt.Text = "Generate Code";
            this.DoIt.UseVisualStyleBackColor = true;
            this.DoIt.Click += new System.EventHandler(this.DoIt_Click);
            // 
            // PixelOpCode
            // 
            this.PixelOpCode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.PixelOpCode.FormattingEnabled = true;
            this.PixelOpCode.Items.AddRange(new object[] {
            "Pass Through",
            "Desaturate",
            "Invert"});
            this.PixelOpCode.Location = new System.Drawing.Point(245, 266);
            this.PixelOpCode.Name = "PixelOpCode";
            this.PixelOpCode.Size = new System.Drawing.Size(117, 21);
            this.PixelOpCode.TabIndex = 12;
            // 
            // textBox1
            // 
            this.textBox1.AutoSize = true;
            this.textBox1.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox1.Location = new System.Drawing.Point(19, 398);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(105, 13);
            this.textBox1.TabIndex = 16;
            this.textBox1.Text = "Additional editing:";
            // 
            // HsvColorMode
            // 
            this.HsvColorMode.AutoSize = true;
            this.HsvColorMode.Location = new System.Drawing.Point(35, 444);
            this.HsvColorMode.Name = "HsvColorMode";
            this.HsvColorMode.Size = new System.Drawing.Size(107, 17);
            this.HsvColorMode.TabIndex = 18;
            this.HsvColorMode.Text = "Hsv Color Editing";
            this.HsvColorMode.UseVisualStyleBackColor = true;
            // 
            // FinalPixelOpCode
            // 
            this.FinalPixelOpCode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.FinalPixelOpCode.FormattingEnabled = true;
            this.FinalPixelOpCode.Items.AddRange(new object[] {
            "Pass Through",
            "Desaturate",
            "Invert"});
            this.FinalPixelOpCode.Location = new System.Drawing.Point(245, 368);
            this.FinalPixelOpCode.Name = "FinalPixelOpCode";
            this.FinalPixelOpCode.Size = new System.Drawing.Size(117, 21);
            this.FinalPixelOpCode.TabIndex = 15;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(16, 155);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(163, 13);
            this.label4.TabIndex = 10;
            this.label4.Text = "Pixel flow for complex effects:";
            // 
            // srcLabel
            // 
            this.srcLabel.AutoSize = true;
            this.srcLabel.BackColor = System.Drawing.Color.Black;
            this.srcLabel.ForeColor = System.Drawing.Color.White;
            this.srcLabel.Location = new System.Drawing.Point(57, 189);
            this.srcLabel.Name = "srcLabel";
            this.srcLabel.Size = new System.Drawing.Size(41, 26);
            this.srcLabel.TabIndex = 23;
            this.srcLabel.Text = "SRC\r\nIMAGE";
            this.srcLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // dstLabel
            // 
            this.dstLabel.AutoSize = true;
            this.dstLabel.BackColor = System.Drawing.Color.Black;
            this.dstLabel.ForeColor = System.Drawing.Color.White;
            this.dstLabel.Location = new System.Drawing.Point(300, 189);
            this.dstLabel.Name = "dstLabel";
            this.dstLabel.Size = new System.Drawing.Size(41, 26);
            this.dstLabel.TabIndex = 24;
            this.dstLabel.Text = "DST\r\nIMAGE";
            this.dstLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.BackColor = System.Drawing.Color.Black;
            this.label6.ForeColor = System.Drawing.Color.White;
            this.label6.Location = new System.Drawing.Point(300, 440);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(41, 26);
            this.label6.TabIndex = 28;
            this.label6.Text = "FINAL\r\nIMAGE";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // BlendArrow
            // 
            this.BlendArrow.Image = global::PaintDotNet.Effects.Properties.Resources.BlendArrow;
            this.BlendArrow.Location = new System.Drawing.Point(35, 238);
            this.BlendArrow.Name = "BlendArrow";
            this.BlendArrow.Size = new System.Drawing.Size(153, 104);
            this.BlendArrow.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.BlendArrow.TabIndex = 26;
            this.BlendArrow.TabStop = false;
            this.BlendArrow.Visible = false;
            // 
            // pictureBox3
            // 
            this.pictureBox3.Image = global::PaintDotNet.Effects.Properties.Resources.Photo;
            this.pictureBox3.Location = new System.Drawing.Point(276, 419);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(85, 64);
            this.pictureBox3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox3.TabIndex = 22;
            this.pictureBox3.TabStop = false;
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = global::PaintDotNet.Effects.Properties.Resources.Photo;
            this.pictureBox2.Location = new System.Drawing.Point(276, 172);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(85, 64);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox2.TabIndex = 21;
            this.pictureBox2.TabStop = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::PaintDotNet.Effects.Properties.Resources.Photo;
            this.pictureBox1.Location = new System.Drawing.Point(35, 172);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(85, 64);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 20;
            this.pictureBox1.TabStop = false;
            // 
            // pictureBox4
            // 
            this.pictureBox4.Image = global::PaintDotNet.Effects.Properties.Resources.DownArrow;
            this.pictureBox4.Location = new System.Drawing.Point(308, 240);
            this.pictureBox4.Name = "pictureBox4";
            this.pictureBox4.Size = new System.Drawing.Size(24, 24);
            this.pictureBox4.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox4.TabIndex = 27;
            this.pictureBox4.TabStop = false;
            // 
            // pictureBox5
            // 
            this.pictureBox5.Image = global::PaintDotNet.Effects.Properties.Resources.DownArrow;
            this.pictureBox5.Location = new System.Drawing.Point(308, 291);
            this.pictureBox5.Name = "pictureBox5";
            this.pictureBox5.Size = new System.Drawing.Size(24, 24);
            this.pictureBox5.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox5.TabIndex = 28;
            this.pictureBox5.TabStop = false;
            // 
            // pictureBox6
            // 
            this.pictureBox6.Image = global::PaintDotNet.Effects.Properties.Resources.DownArrow;
            this.pictureBox6.Location = new System.Drawing.Point(308, 342);
            this.pictureBox6.Name = "pictureBox6";
            this.pictureBox6.Size = new System.Drawing.Size(24, 24);
            this.pictureBox6.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox6.TabIndex = 29;
            this.pictureBox6.TabStop = false;
            // 
            // pictureBox7
            // 
            this.pictureBox7.Image = global::PaintDotNet.Effects.Properties.Resources.DownArrow;
            this.pictureBox7.Location = new System.Drawing.Point(308, 393);
            this.pictureBox7.Name = "pictureBox7";
            this.pictureBox7.Size = new System.Drawing.Size(24, 24);
            this.pictureBox7.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox7.TabIndex = 30;
            this.pictureBox7.TabStop = false;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(150, 322);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(34, 13);
            this.label7.TabIndex = 26;
            this.label7.Text = "Blend";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(196, 269);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(43, 13);
            this.label8.TabIndex = 25;
            this.label8.Text = "PixelOp";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(196, 371);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(43, 13);
            this.label9.TabIndex = 27;
            this.label9.Text = "PixelOp";
            // 
            // CustomHelp
            // 
            this.CustomHelp.AutoSize = true;
            this.CustomHelp.Location = new System.Drawing.Point(169, 444);
            this.CustomHelp.Name = "CustomHelp";
            this.CustomHelp.Size = new System.Drawing.Size(86, 17);
            this.CustomHelp.TabIndex = 19;
            this.CustomHelp.Text = "Custom Help";
            this.CustomHelp.UseVisualStyleBackColor = true;
            // 
            // PaletteCode
            // 
            this.PaletteCode.AutoSize = true;
            this.PaletteCode.Location = new System.Drawing.Point(35, 124);
            this.PaletteCode.Name = "PaletteCode";
            this.PaletteCode.Size = new System.Drawing.Size(154, 17);
            this.PaletteCode.TabIndex = 8;
            this.PaletteCode.Text = "Current and Default Palette";
            this.PaletteCode.UseVisualStyleBackColor = true;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(367, 521);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(10, 13);
            this.label10.TabIndex = 22;
            this.label10.Text = " ";
            // 
            // FAS
            // 
            this.FAS.AutoSize = true;
            this.FAS.Location = new System.Drawing.Point(35, 468);
            this.FAS.Name = "FAS";
            this.FAS.Size = new System.Drawing.Size(137, 17);
            this.FAS.TabIndex = 20;
            this.FAS.Text = "Force Aliased Selection";
            this.FAS.UseVisualStyleBackColor = true;
            // 
            // ST
            // 
            this.ST.AutoSize = true;
            this.ST.Location = new System.Drawing.Point(169, 468);
            this.ST.Name = "ST";
            this.ST.Size = new System.Drawing.Size(104, 17);
            this.ST.TabIndex = 21;
            this.ST.Text = "Single Threaded";
            this.ST.UseVisualStyleBackColor = true;
            // 
            // SurfaceCode
            // 
            this.SurfaceCode.AutoSize = true;
            this.SurfaceCode.Location = new System.Drawing.Point(35, 370);
            this.SurfaceCode.Name = "SurfaceCode";
            this.SurfaceCode.Size = new System.Drawing.Size(133, 17);
            this.SurfaceCode.TabIndex = 14;
            this.SurfaceCode.Text = "Create a WRK surface";
            this.SurfaceCode.UseVisualStyleBackColor = true;
            this.SurfaceCode.CheckedChanged += new System.EventHandler(this.SurfaceCode_CheckedChanged);
            // 
            // NoStyle
            // 
            this.NoStyle.AutoSize = true;
            this.NoStyle.Location = new System.Drawing.Point(292, 30);
            this.NoStyle.Name = "NoStyle";
            this.NoStyle.Size = new System.Drawing.Size(51, 17);
            this.NoStyle.TabIndex = 4;
            this.NoStyle.TabStop = true;
            this.NoStyle.Text = "None";
            this.NoStyle.UseVisualStyleBackColor = true;
            this.NoStyle.CheckedChanged += new System.EventHandler(this.NoStyle_CheckedChanged);
            // 
            // FileNew
            // 
            this.AcceptButton = this.DoIt;
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoSize = true;
            this.CancelButton = this.button1;
            this.ClientSize = new System.Drawing.Size(374, 543);
            this.Controls.Add(this.NoStyle);
            this.Controls.Add(this.SurfaceCode);
            this.Controls.Add(this.ST);
            this.Controls.Add(this.FAS);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.PaletteCode);
            this.Controls.Add(this.CustomHelp);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.BlendArrow);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.dstLabel);
            this.Controls.Add(this.srcLabel);
            this.Controls.Add(this.pictureBox3);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.FinalPixelOpCode);
            this.Controls.Add(this.HsvColorMode);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.PixelOpCode);
            this.Controls.Add(this.DoIt);
            this.Controls.Add(this.EffectCode);
            this.Controls.Add(this.BlendingCode);
            this.Controls.Add(this.SelectionCode);
            this.Controls.Add(this.PenWidthCode);
            this.Controls.Add(this.PrimaryColorCode);
            this.Controls.Add(this.CenterCode);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.AdvancedStyle);
            this.Controls.Add(this.BasicStyle);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.pictureBox4);
            this.Controls.Add(this.pictureBox5);
            this.Controls.Add(this.pictureBox6);
            this.Controls.Add(this.pictureBox7);
            this.Controls.Add(this.label7);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FileNew";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "New Source (Template)";
            ((System.ComponentModel.ISupportInitialize)(this.BlendArrow)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox7)).EndInit();
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
        private System.Windows.Forms.ComboBox BlendingCode;
        private System.Windows.Forms.ComboBox EffectCode;
        private System.Windows.Forms.Button DoIt;
        private System.Windows.Forms.ComboBox PixelOpCode;
        private System.Windows.Forms.Label textBox1;
        private System.Windows.Forms.CheckBox HsvColorMode;
        private System.Windows.Forms.ComboBox FinalPixelOpCode;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.PictureBox pictureBox3;
        private System.Windows.Forms.Label srcLabel;
        private System.Windows.Forms.Label dstLabel;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.PictureBox BlendArrow;
        private System.Windows.Forms.PictureBox pictureBox4;
        private System.Windows.Forms.PictureBox pictureBox5;
        private System.Windows.Forms.PictureBox pictureBox6;
        private System.Windows.Forms.PictureBox pictureBox7;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.CheckBox CustomHelp;
        private System.Windows.Forms.CheckBox PaletteCode;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.CheckBox FAS;
        private System.Windows.Forms.CheckBox ST;
        private System.Windows.Forms.CheckBox SurfaceCode;
        private System.Windows.Forms.RadioButton NoStyle;
    }
}