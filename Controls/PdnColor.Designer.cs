namespace PdnCodeLab
{
    partial class PdnColor
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.colorWheelBox = new System.Windows.Forms.PictureBox();
            this.headerPanel3 = new System.Windows.Forms.Panel();
            this.headerPanel2 = new System.Windows.Forms.Panel();
            this.headerPanel1 = new System.Windows.Forms.Panel();
            this.opacityLabel = new System.Windows.Forms.Label();
            this.sLabel = new System.Windows.Forms.Label();
            this.hlabel = new System.Windows.Forms.Label();
            this.hexLabel = new System.Windows.Forms.Label();
            this.vLabel = new System.Windows.Forms.Label();
            this.bLabel = new System.Windows.Forms.Label();
            this.gLabel = new System.Windows.Forms.Label();
            this.rLabel = new System.Windows.Forms.Label();
            this.alphaBox = new System.Windows.Forms.NumericUpDown();
            this.blueBox = new System.Windows.Forms.NumericUpDown();
            this.greenBox = new System.Windows.Forms.NumericUpDown();
            this.redBox = new System.Windows.Forms.NumericUpDown();
            this.hexBox = new System.Windows.Forms.TextBox();
            this.hueBox = new System.Windows.Forms.NumericUpDown();
            this.satBox = new System.Windows.Forms.NumericUpDown();
            this.valBox = new System.Windows.Forms.NumericUpDown();
            this.RGBlabel = new System.Windows.Forms.Label();
            this.hsvLabel = new System.Windows.Forms.Label();
            this.vColorSlider = new PdnCodeLab.ColorSlider();
            this.sColorSlider = new PdnCodeLab.ColorSlider();
            this.hColorSlider = new PdnCodeLab.ColorSlider();
            this.bColorSlider = new PdnCodeLab.ColorSlider();
            this.gColorSlider = new PdnCodeLab.ColorSlider();
            this.rColorSlider = new PdnCodeLab.ColorSlider();
            this.aColorSlider = new PdnCodeLab.ColorSlider();
            ((System.ComponentModel.ISupportInitialize)(this.colorWheelBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.alphaBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.blueBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.greenBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.redBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.hueBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.satBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.valBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.vColorSlider)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.sColorSlider)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.hColorSlider)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bColorSlider)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gColorSlider)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rColorSlider)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.aColorSlider)).BeginInit();
            this.SuspendLayout();
            // 
            // colorWheelBox
            // 
            this.colorWheelBox.BackColor = System.Drawing.Color.Transparent;
            this.colorWheelBox.Location = new System.Drawing.Point(3, 3);
            this.colorWheelBox.Name = "colorWheelBox";
            this.colorWheelBox.Size = new System.Drawing.Size(210, 210);
            this.colorWheelBox.TabIndex = 17;
            this.colorWheelBox.TabStop = false;
            this.colorWheelBox.Tag = "0";
            this.colorWheelBox.Paint += new System.Windows.Forms.PaintEventHandler(this.colorWheel_Paint);
            this.colorWheelBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ColorWheel_MouseDown);
            this.colorWheelBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.ColorWheel_MouseMove);
            this.colorWheelBox.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ColorWheel_MouseUp);
            // 
            // headerPanel3
            // 
            this.headerPanel3.BackColor = System.Drawing.SystemColors.ControlLight;
            this.headerPanel3.Location = new System.Drawing.Point(318, 238);
            this.headerPanel3.Name = "headerPanel3";
            this.headerPanel3.Size = new System.Drawing.Size(64, 1);
            this.headerPanel3.TabIndex = 39;
            // 
            // headerPanel2
            // 
            this.headerPanel2.BackColor = System.Drawing.SystemColors.ControlLight;
            this.headerPanel2.Location = new System.Drawing.Point(261, 137);
            this.headerPanel2.Name = "headerPanel2";
            this.headerPanel2.Size = new System.Drawing.Size(120, 1);
            this.headerPanel2.TabIndex = 38;
            // 
            // headerPanel1
            // 
            this.headerPanel1.BackColor = System.Drawing.SystemColors.ControlLight;
            this.headerPanel1.Location = new System.Drawing.Point(261, 9);
            this.headerPanel1.Name = "headerPanel1";
            this.headerPanel1.Size = new System.Drawing.Size(120, 1);
            this.headerPanel1.TabIndex = 37;
            // 
            // opacityLabel
            // 
            this.opacityLabel.AutoSize = true;
            this.opacityLabel.Location = new System.Drawing.Point(230, 233);
            this.opacityLabel.Name = "opacityLabel";
            this.opacityLabel.Size = new System.Drawing.Size(90, 15);
            this.opacityLabel.TabIndex = 36;
            this.opacityLabel.Text = "Opacity - Alpha";
            // 
            // sLabel
            // 
            this.sLabel.AutoSize = true;
            this.sLabel.Location = new System.Drawing.Point(230, 174);
            this.sLabel.Name = "sLabel";
            this.sLabel.Size = new System.Drawing.Size(16, 15);
            this.sLabel.TabIndex = 31;
            this.sLabel.Text = "S:";
            // 
            // hlabel
            // 
            this.hlabel.AutoSize = true;
            this.hlabel.Location = new System.Drawing.Point(230, 150);
            this.hlabel.Name = "hlabel";
            this.hlabel.Size = new System.Drawing.Size(19, 15);
            this.hlabel.TabIndex = 32;
            this.hlabel.Text = "H:";
            // 
            // hexLabel
            // 
            this.hexLabel.AutoSize = true;
            this.hexLabel.Location = new System.Drawing.Point(230, 101);
            this.hexLabel.Name = "hexLabel";
            this.hexLabel.Size = new System.Drawing.Size(31, 15);
            this.hexLabel.TabIndex = 30;
            this.hexLabel.Text = "Hex:";
            // 
            // vLabel
            // 
            this.vLabel.AutoSize = true;
            this.vLabel.Location = new System.Drawing.Point(230, 198);
            this.vLabel.Name = "vLabel";
            this.vLabel.Size = new System.Drawing.Size(17, 15);
            this.vLabel.TabIndex = 30;
            this.vLabel.Text = "V:";
            // 
            // bLabel
            // 
            this.bLabel.AutoSize = true;
            this.bLabel.Location = new System.Drawing.Point(230, 78);
            this.bLabel.Name = "bLabel";
            this.bLabel.Size = new System.Drawing.Size(17, 15);
            this.bLabel.TabIndex = 30;
            this.bLabel.Text = "B:";
            // 
            // gLabel
            // 
            this.gLabel.AutoSize = true;
            this.gLabel.Location = new System.Drawing.Point(230, 52);
            this.gLabel.Name = "gLabel";
            this.gLabel.Size = new System.Drawing.Size(18, 15);
            this.gLabel.TabIndex = 31;
            this.gLabel.Text = "G:";
            // 
            // rLabel
            // 
            this.rLabel.AutoSize = true;
            this.rLabel.Location = new System.Drawing.Point(230, 28);
            this.rLabel.Name = "rLabel";
            this.rLabel.Size = new System.Drawing.Size(17, 15);
            this.rLabel.TabIndex = 32;
            this.rLabel.Text = "R:";
            // 
            // alphaBox
            // 
            this.alphaBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.alphaBox.Location = new System.Drawing.Point(333, 249);
            this.alphaBox.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.alphaBox.Name = "alphaBox";
            this.alphaBox.Size = new System.Drawing.Size(49, 23);
            this.alphaBox.TabIndex = 8;
            this.alphaBox.Tag = "0";
            this.alphaBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.alphaBox.ValueChanged += new System.EventHandler(this.ARGB_ValueChanged);
            this.alphaBox.Leave += new System.EventHandler(this.ARGB_Leave);
            this.alphaBox.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ARGB_MouseUp);
            // 
            // blueBox
            // 
            this.blueBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.blueBox.Location = new System.Drawing.Point(333, 74);
            this.blueBox.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.blueBox.Name = "blueBox";
            this.blueBox.Size = new System.Drawing.Size(49, 23);
            this.blueBox.TabIndex = 3;
            this.blueBox.Tag = "0";
            this.blueBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.blueBox.ValueChanged += new System.EventHandler(this.ARGB_ValueChanged);
            this.blueBox.Leave += new System.EventHandler(this.ARGB_Leave);
            this.blueBox.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ARGB_MouseUp);
            // 
            // greenBox
            // 
            this.greenBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.greenBox.Location = new System.Drawing.Point(333, 50);
            this.greenBox.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.greenBox.Name = "greenBox";
            this.greenBox.Size = new System.Drawing.Size(49, 23);
            this.greenBox.TabIndex = 2;
            this.greenBox.Tag = "0";
            this.greenBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.greenBox.ValueChanged += new System.EventHandler(this.ARGB_ValueChanged);
            this.greenBox.Leave += new System.EventHandler(this.ARGB_Leave);
            this.greenBox.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ARGB_MouseUp);
            // 
            // redBox
            // 
            this.redBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.redBox.Location = new System.Drawing.Point(333, 26);
            this.redBox.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.redBox.Name = "redBox";
            this.redBox.Size = new System.Drawing.Size(49, 23);
            this.redBox.TabIndex = 1;
            this.redBox.Tag = "0";
            this.redBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.redBox.ValueChanged += new System.EventHandler(this.ARGB_ValueChanged);
            this.redBox.Leave += new System.EventHandler(this.ARGB_Leave);
            this.redBox.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ARGB_MouseUp);
            // 
            // hexBox
            // 
            this.hexBox.Location = new System.Drawing.Point(322, 98);
            this.hexBox.Name = "hexBox";
            this.hexBox.Size = new System.Drawing.Size(60, 23);
            this.hexBox.TabIndex = 4;
            this.hexBox.Text = "FFFFFFFF";
            this.hexBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.hexBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.hexBox_KeyPress);
            this.hexBox.Leave += new System.EventHandler(this.hexBox_Leave);
            // 
            // hueBox
            // 
            this.hueBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.hueBox.DecimalPlaces = 1;
            this.hueBox.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.hueBox.Location = new System.Drawing.Point(333, 148);
            this.hueBox.Maximum = new decimal(new int[] {
            360,
            0,
            0,
            0});
            this.hueBox.Name = "hueBox";
            this.hueBox.Size = new System.Drawing.Size(49, 23);
            this.hueBox.TabIndex = 5;
            this.hueBox.Tag = "0";
            this.hueBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.hueBox.ValueChanged += new System.EventHandler(this.HSV_ValueChanged);
            this.hueBox.Leave += new System.EventHandler(this.HSV_Leave);
            this.hueBox.MouseUp += new System.Windows.Forms.MouseEventHandler(this.HSV_MouseUp);
            // 
            // satBox
            // 
            this.satBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.satBox.DecimalPlaces = 1;
            this.satBox.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.satBox.Location = new System.Drawing.Point(333, 172);
            this.satBox.Name = "satBox";
            this.satBox.Size = new System.Drawing.Size(49, 23);
            this.satBox.TabIndex = 6;
            this.satBox.Tag = "0";
            this.satBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.satBox.ValueChanged += new System.EventHandler(this.HSV_ValueChanged);
            this.satBox.Leave += new System.EventHandler(this.HSV_Leave);
            this.satBox.MouseUp += new System.Windows.Forms.MouseEventHandler(this.HSV_MouseUp);
            // 
            // valBox
            // 
            this.valBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.valBox.DecimalPlaces = 1;
            this.valBox.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.valBox.Location = new System.Drawing.Point(333, 196);
            this.valBox.Name = "valBox";
            this.valBox.Size = new System.Drawing.Size(49, 23);
            this.valBox.TabIndex = 7;
            this.valBox.Tag = "0";
            this.valBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.valBox.ValueChanged += new System.EventHandler(this.HSV_ValueChanged);
            this.valBox.Leave += new System.EventHandler(this.HSV_Leave);
            this.valBox.MouseUp += new System.Windows.Forms.MouseEventHandler(this.HSV_MouseUp);
            // 
            // RGBlabel
            // 
            this.RGBlabel.AutoSize = true;
            this.RGBlabel.Location = new System.Drawing.Point(230, 4);
            this.RGBlabel.Name = "RGBlabel";
            this.RGBlabel.Size = new System.Drawing.Size(29, 15);
            this.RGBlabel.TabIndex = 0;
            this.RGBlabel.Text = "RGB";
            // 
            // hsvLabel
            // 
            this.hsvLabel.AutoSize = true;
            this.hsvLabel.Location = new System.Drawing.Point(230, 132);
            this.hsvLabel.Name = "hsvLabel";
            this.hsvLabel.Size = new System.Drawing.Size(29, 15);
            this.hsvLabel.TabIndex = 35;
            this.hsvLabel.Text = "HSV";
            // 
            // vColorSlider
            // 
            this.vColorSlider.Colors = new System.Drawing.Color[] {
        System.Drawing.Color.White,
        System.Drawing.Color.Black};
            this.vColorSlider.Location = new System.Drawing.Point(254, 199);
            this.vColorSlider.MaxValue = 100;
            this.vColorSlider.Name = "vColorSlider";
            this.vColorSlider.Size = new System.Drawing.Size(73, 15);
            this.vColorSlider.TabIndex = 46;
            this.vColorSlider.TabStop = false;
            this.vColorSlider.Value = 0F;
            this.vColorSlider.ValueChanged += new System.EventHandler(this.HSV_Sliders_ValueChanged);
            // 
            // sColorSlider
            // 
            this.sColorSlider.Colors = new System.Drawing.Color[] {
        System.Drawing.Color.White,
        System.Drawing.Color.Black};
            this.sColorSlider.Location = new System.Drawing.Point(254, 175);
            this.sColorSlider.MaxValue = 100;
            this.sColorSlider.Name = "sColorSlider";
            this.sColorSlider.Size = new System.Drawing.Size(73, 15);
            this.sColorSlider.TabIndex = 45;
            this.sColorSlider.TabStop = false;
            this.sColorSlider.Value = 0F;
            this.sColorSlider.ValueChanged += new System.EventHandler(this.HSV_Sliders_ValueChanged);
            // 
            // hColorSlider
            // 
            this.hColorSlider.Colors = new System.Drawing.Color[] {
        System.Drawing.Color.White,
        System.Drawing.Color.Black};
            this.hColorSlider.Location = new System.Drawing.Point(254, 151);
            this.hColorSlider.MaxValue = 360;
            this.hColorSlider.Name = "hColorSlider";
            this.hColorSlider.Size = new System.Drawing.Size(73, 15);
            this.hColorSlider.TabIndex = 44;
            this.hColorSlider.TabStop = false;
            this.hColorSlider.Value = 0F;
            this.hColorSlider.ValueChanged += new System.EventHandler(this.HSV_Sliders_ValueChanged);
            // 
            // bColorSlider
            // 
            this.bColorSlider.Colors = new System.Drawing.Color[] {
        System.Drawing.Color.White,
        System.Drawing.Color.Black};
            this.bColorSlider.Location = new System.Drawing.Point(254, 77);
            this.bColorSlider.MaxValue = 255;
            this.bColorSlider.Name = "bColorSlider";
            this.bColorSlider.Size = new System.Drawing.Size(73, 15);
            this.bColorSlider.TabIndex = 43;
            this.bColorSlider.TabStop = false;
            this.bColorSlider.Value = 0F;
            this.bColorSlider.ValueChanged += new System.EventHandler(this.RGB_Sliders_ValueChanged);
            // 
            // gColorSlider
            // 
            this.gColorSlider.Colors = new System.Drawing.Color[] {
        System.Drawing.Color.White,
        System.Drawing.Color.Black};
            this.gColorSlider.Location = new System.Drawing.Point(254, 53);
            this.gColorSlider.MaxValue = 255;
            this.gColorSlider.Name = "gColorSlider";
            this.gColorSlider.Size = new System.Drawing.Size(73, 15);
            this.gColorSlider.TabIndex = 42;
            this.gColorSlider.TabStop = false;
            this.gColorSlider.Value = 0F;
            this.gColorSlider.ValueChanged += new System.EventHandler(this.RGB_Sliders_ValueChanged);
            // 
            // rColorSlider
            // 
            this.rColorSlider.Colors = new System.Drawing.Color[] {
        System.Drawing.Color.White,
        System.Drawing.Color.Black};
            this.rColorSlider.Location = new System.Drawing.Point(254, 29);
            this.rColorSlider.MaxValue = 255;
            this.rColorSlider.Name = "rColorSlider";
            this.rColorSlider.Size = new System.Drawing.Size(73, 15);
            this.rColorSlider.TabIndex = 41;
            this.rColorSlider.TabStop = false;
            this.rColorSlider.Value = 0F;
            this.rColorSlider.ValueChanged += new System.EventHandler(this.RGB_Sliders_ValueChanged);
            // 
            // aColorSlider
            // 
            this.aColorSlider.Colors = new System.Drawing.Color[] {
        System.Drawing.Color.White,
        System.Drawing.Color.Black};
            this.aColorSlider.Location = new System.Drawing.Point(254, 252);
            this.aColorSlider.MaxValue = 255;
            this.aColorSlider.Name = "aColorSlider";
            this.aColorSlider.Size = new System.Drawing.Size(73, 15);
            this.aColorSlider.TabIndex = 40;
            this.aColorSlider.TabStop = false;
            this.aColorSlider.Value = 0F;
            this.aColorSlider.ValueChanged += new System.EventHandler(this.HSV_Sliders_ValueChanged);
            // 
            // PdnColor
            // 
            this.Controls.Add(this.vColorSlider);
            this.Controls.Add(this.hsvLabel);
            this.Controls.Add(this.sColorSlider);
            this.Controls.Add(this.RGBlabel);
            this.Controls.Add(this.hColorSlider);
            this.Controls.Add(this.hexBox);
            this.Controls.Add(this.bColorSlider);
            this.Controls.Add(this.valBox);
            this.Controls.Add(this.gColorSlider);
            this.Controls.Add(this.alphaBox);
            this.Controls.Add(this.rColorSlider);
            this.Controls.Add(this.satBox);
            this.Controls.Add(this.aColorSlider);
            this.Controls.Add(this.blueBox);
            this.Controls.Add(this.headerPanel3);
            this.Controls.Add(this.hueBox);
            this.Controls.Add(this.headerPanel2);
            this.Controls.Add(this.greenBox);
            this.Controls.Add(this.headerPanel1);
            this.Controls.Add(this.opacityLabel);
            this.Controls.Add(this.redBox);
            this.Controls.Add(this.sLabel);
            this.Controls.Add(this.colorWheelBox);
            this.Controls.Add(this.hlabel);
            this.Controls.Add(this.rLabel);
            this.Controls.Add(this.hexLabel);
            this.Controls.Add(this.gLabel);
            this.Controls.Add(this.vLabel);
            this.Controls.Add(this.bLabel);
            this.Name = "PdnColor";
            this.Size = new System.Drawing.Size(385, 275);
            ((System.ComponentModel.ISupportInitialize)(this.colorWheelBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.alphaBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.blueBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.greenBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.redBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.hueBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.satBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.valBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.vColorSlider)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.sColorSlider)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.hColorSlider)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bColorSlider)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gColorSlider)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rColorSlider)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.aColorSlider)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox colorWheelBox;
        private System.Windows.Forms.NumericUpDown alphaBox;
        private System.Windows.Forms.NumericUpDown blueBox;
        private System.Windows.Forms.NumericUpDown greenBox;
        private System.Windows.Forms.NumericUpDown redBox;
        private System.Windows.Forms.TextBox hexBox;
        private System.Windows.Forms.NumericUpDown hueBox;
        private System.Windows.Forms.NumericUpDown satBox;
        private System.Windows.Forms.NumericUpDown valBox;
        private System.Windows.Forms.Label hlabel;
        private System.Windows.Forms.Label sLabel;
        private System.Windows.Forms.Label vLabel;
        private System.Windows.Forms.Label hexLabel;
        private System.Windows.Forms.Label rLabel;
        private System.Windows.Forms.Label gLabel;
        private System.Windows.Forms.Label bLabel;
        private System.Windows.Forms.Label RGBlabel;
        private System.Windows.Forms.Label hsvLabel;
        private System.Windows.Forms.Label opacityLabel;
        private System.Windows.Forms.Panel headerPanel1;
        private System.Windows.Forms.Panel headerPanel2;
        private System.Windows.Forms.Panel headerPanel3;
        private ColorSlider aColorSlider;
        private ColorSlider rColorSlider;
        private ColorSlider vColorSlider;
        private ColorSlider sColorSlider;
        private ColorSlider hColorSlider;
        private ColorSlider bColorSlider;
        private ColorSlider gColorSlider;
    }
}
