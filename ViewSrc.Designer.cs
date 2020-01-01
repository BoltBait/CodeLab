namespace PaintDotNet.Effects
{
    partial class ViewSrc
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
            this.TextSrcBox = new System.Windows.Forms.TextBox();
            this.ButtonClose = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.CopyButton = new PaintDotNet.Effects.ScaledButton();
            this.SaveButton = new PaintDotNet.Effects.ScaledButton();
            this.SuspendLayout();
            // 
            // TextSrcBox
            // 
            this.TextSrcBox.AcceptsReturn = true;
            this.TextSrcBox.AcceptsTab = true;
            this.TextSrcBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TextSrcBox.Location = new System.Drawing.Point(13, 13);
            this.TextSrcBox.Multiline = true;
            this.TextSrcBox.Name = "TextSrcBox";
            this.TextSrcBox.ReadOnly = true;
            this.TextSrcBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.TextSrcBox.Size = new System.Drawing.Size(559, 305);
            this.TextSrcBox.TabIndex = 1;
            this.TextSrcBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TextSrcBox_KeyDown);
            // 
            // ButtonClose
            // 
            this.ButtonClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ButtonClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.ButtonClose.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.ButtonClose.Location = new System.Drawing.Point(497, 326);
            this.ButtonClose.Name = "ButtonClose";
            this.ButtonClose.Size = new System.Drawing.Size(75, 24);
            this.ButtonClose.TabIndex = 0;
            this.ButtonClose.Text = "Close";
            this.ButtonClose.UseVisualStyleBackColor = true;
            this.ButtonClose.Click += new System.EventHandler(this.ButtonClose_Click);
            // 
            // CopyButton
            // 
            this.CopyButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.CopyButton.FlatAppearance.BorderSize = 0;
            this.CopyButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.CopyButton.ImageName = "Copy";
            this.CopyButton.Location = new System.Drawing.Point(13, 325);
            this.CopyButton.Name = "CopyButton";
            this.CopyButton.Size = new System.Drawing.Size(27, 23);
            this.CopyButton.TabIndex = 3;
            this.CopyButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.toolTip1.SetToolTip(this.CopyButton, "Copy all to clipboard");
            this.CopyButton.UseVisualStyleBackColor = true;
            this.CopyButton.Click += new System.EventHandler(this.CopyButton_Click);
            // 
            // SaveButton
            // 
            this.SaveButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.SaveButton.FlatAppearance.BorderSize = 0;
            this.SaveButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.SaveButton.ImageName = "Save";
            this.SaveButton.Location = new System.Drawing.Point(46, 325);
            this.SaveButton.Name = "SaveButton";
            this.SaveButton.Size = new System.Drawing.Size(27, 23);
            this.SaveButton.TabIndex = 2;
            this.SaveButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.toolTip1.SetToolTip(this.SaveButton, "Save to file");
            this.SaveButton.UseVisualStyleBackColor = true;
            this.SaveButton.Click += new System.EventHandler(this.SaveButton_Click);
            // 
            // ViewSrc
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.CancelButton = this.ButtonClose;
            this.ClientSize = new System.Drawing.Size(584, 362);
            this.Controls.Add(this.CopyButton);
            this.Controls.Add(this.SaveButton);
            this.Controls.Add(this.ButtonClose);
            this.Controls.Add(this.TextSrcBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            this.MaximizeBox = true;
            this.MinimumSize = new System.Drawing.Size(300, 200);
            this.Name = "ViewSrc";
            this.ShowIcon = false;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox TextSrcBox;
        private System.Windows.Forms.Button ButtonClose;
        private ScaledButton SaveButton;
        private ScaledButton CopyButton;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}