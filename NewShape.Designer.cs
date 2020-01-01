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
            this.pgGadioButton = new System.Windows.Forms.RadioButton();
            this.ggRadioButton = new System.Windows.Forms.RadioButton();
            this.sgRadioButton = new System.Windows.Forms.RadioButton();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // pgGadioButton
            // 
            this.pgGadioButton.AutoSize = true;
            this.pgGadioButton.Location = new System.Drawing.Point(12, 37);
            this.pgGadioButton.Name = "pgGadioButton";
            this.pgGadioButton.Size = new System.Drawing.Size(235, 19);
            this.pgGadioButton.TabIndex = 5;
            this.pgGadioButton.TabStop = true;
            this.pgGadioButton.Text = "SimpleGeometryShape - PathGeometry ";
            this.pgGadioButton.UseVisualStyleBackColor = true;
            // 
            // ggRadioButton
            // 
            this.ggRadioButton.AutoSize = true;
            this.ggRadioButton.Location = new System.Drawing.Point(12, 62);
            this.ggRadioButton.Name = "ggRadioButton";
            this.ggRadioButton.Size = new System.Drawing.Size(241, 19);
            this.ggRadioButton.TabIndex = 4;
            this.ggRadioButton.TabStop = true;
            this.ggRadioButton.Text = "SimpleGeometryShape - GeometryGroup";
            this.ggRadioButton.UseVisualStyleBackColor = true;
            // 
            // sgRadioButton
            // 
            this.sgRadioButton.AutoSize = true;
            this.sgRadioButton.Location = new System.Drawing.Point(12, 12);
            this.sgRadioButton.Name = "sgRadioButton";
            this.sgRadioButton.Size = new System.Drawing.Size(245, 19);
            this.sgRadioButton.TabIndex = 3;
            this.sgRadioButton.TabStop = true;
            this.sgRadioButton.Text = "SimpleGeometryShape - StreamGeometry";
            this.sgRadioButton.UseVisualStyleBackColor = true;
            // 
            // okButton
            // 
            this.okButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.okButton.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.okButton.Location = new System.Drawing.Point(132, 114);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 24);
            this.okButton.TabIndex = 6;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.cancelButton.Location = new System.Drawing.Point(213, 114);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 24);
            this.cancelButton.TabIndex = 7;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // NewShape
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.ClientSize = new System.Drawing.Size(300, 150);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.pgGadioButton);
            this.Controls.Add(this.ggRadioButton);
            this.Controls.Add(this.sgRadioButton);
            this.IconName = "Shape";
            this.Name = "NewShape";
            this.Text = "New Shape";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton pgGadioButton;
        private System.Windows.Forms.RadioButton ggRadioButton;
        private System.Windows.Forms.RadioButton sgRadioButton;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
    }
}