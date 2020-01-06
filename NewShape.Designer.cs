namespace PaintDotNet.Effects
{
    partial class NewShape
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
            this.pgRadioButton = new System.Windows.Forms.RadioButton();
            this.ggRadioButton = new System.Windows.Forms.RadioButton();
            this.sgRadioButton = new System.Windows.Forms.RadioButton();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.samplePicture = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.samplePicture)).BeginInit();
            this.SuspendLayout();
            // 
            // pgRadioButton
            // 
            this.pgRadioButton.AutoSize = true;
            this.pgRadioButton.Location = new System.Drawing.Point(12, 66);
            this.pgRadioButton.Name = "pgRadioButton";
            this.pgRadioButton.Size = new System.Drawing.Size(104, 19);
            this.pgRadioButton.TabIndex = 5;
            this.pgRadioButton.TabStop = true;
            this.pgRadioButton.Text = "Path Geometry";
            this.pgRadioButton.UseVisualStyleBackColor = true;
            this.pgRadioButton.CheckedChanged += new System.EventHandler(this.RadioButton_CheckedChanged);
            // 
            // ggRadioButton
            // 
            this.ggRadioButton.AutoSize = true;
            this.ggRadioButton.Location = new System.Drawing.Point(12, 91);
            this.ggRadioButton.Name = "ggRadioButton";
            this.ggRadioButton.Size = new System.Drawing.Size(113, 19);
            this.ggRadioButton.TabIndex = 4;
            this.ggRadioButton.TabStop = true;
            this.ggRadioButton.Text = "Geometry Group";
            this.ggRadioButton.UseVisualStyleBackColor = true;
            this.ggRadioButton.CheckedChanged += new System.EventHandler(this.RadioButton_CheckedChanged);
            // 
            // sgRadioButton
            // 
            this.sgRadioButton.AutoSize = true;
            this.sgRadioButton.Location = new System.Drawing.Point(12, 41);
            this.sgRadioButton.Name = "sgRadioButton";
            this.sgRadioButton.Size = new System.Drawing.Size(156, 19);
            this.sgRadioButton.TabIndex = 3;
            this.sgRadioButton.TabStop = true;
            this.sgRadioButton.Text = "Simple Stream Geometry";
            this.sgRadioButton.UseVisualStyleBackColor = true;
            this.sgRadioButton.CheckedChanged += new System.EventHandler(this.RadioButton_CheckedChanged);
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.okButton.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.okButton.Location = new System.Drawing.Point(404, 156);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 24);
            this.okButton.TabIndex = 6;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.cancelButton.Location = new System.Drawing.Point(485, 156);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 24);
            this.cancelButton.TabIndex = 7;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // samplePicture
            // 
            this.samplePicture.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.samplePicture.BackgroundImage = global::PaintDotNet.Effects.Properties.Resources.Shape0;
            this.samplePicture.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.samplePicture.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.samplePicture.Location = new System.Drawing.Point(176, 19);
            this.samplePicture.Name = "samplePicture";
            this.samplePicture.Size = new System.Drawing.Size(383, 121);
            this.samplePicture.TabIndex = 8;
            this.samplePicture.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.label1.Location = new System.Drawing.Point(10, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(86, 15);
            this.label1.TabIndex = 9;
            this.label1.Text = "Shape format:";
            // 
            // NewShape
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.ClientSize = new System.Drawing.Size(572, 192);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.samplePicture);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.pgRadioButton);
            this.Controls.Add(this.ggRadioButton);
            this.Controls.Add(this.sgRadioButton);
            this.IconName = "Shape";
            this.Name = "NewShape";
            this.Text = "New Shape";
            ((System.ComponentModel.ISupportInitialize)(this.samplePicture)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton pgRadioButton;
        private System.Windows.Forms.RadioButton ggRadioButton;
        private System.Windows.Forms.RadioButton sgRadioButton;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.PictureBox samplePicture;
        private System.Windows.Forms.Label label1;
    }
}