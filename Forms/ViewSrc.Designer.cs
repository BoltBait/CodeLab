namespace PdnCodeLab
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
            components = new System.ComponentModel.Container();
            TextSrcBox = new CodeTextBox();
            ButtonClose = new System.Windows.Forms.Button();
            toolTip1 = new System.Windows.Forms.ToolTip(components);
            CopyButton = new ScaledButton();
            SaveButton = new ScaledButton();
            SuspendLayout();
            // 
            // TextSrcBox
            // 
            TextSrcBox.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            TextSrcBox.AutoCMaxHeight = 9;
            TextSrcBox.AutomaticFold = ScintillaNET.AutomaticFold.Show | ScintillaNET.AutomaticFold.Change;
            TextSrcBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            TextSrcBox.CaretLineBackColor = System.Drawing.Color.GhostWhite;
            TextSrcBox.CaretLineVisible = true;
            TextSrcBox.CaretLineVisibleAlways = true;
            TextSrcBox.IdleStyling = ScintillaNET.IdleStyling.All;
            TextSrcBox.Lexer = ScintillaNET.Lexer.Cpp;
            TextSrcBox.Location = new System.Drawing.Point(13, 13);
            TextSrcBox.MouseDwellTime = 250;
            TextSrcBox.Name = "TextSrcBox";
            TextSrcBox.Size = new System.Drawing.Size(759, 304);
            TextSrcBox.TabIndex = 1;
            TextSrcBox.Technology = ScintillaNET.Technology.DirectWrite;
            TextSrcBox.WrapIndentMode = ScintillaNET.WrapIndentMode.Indent;
            // 
            // ButtonClose
            // 
            ButtonClose.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            ButtonClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            ButtonClose.FlatStyle = System.Windows.Forms.FlatStyle.System;
            ButtonClose.Location = new System.Drawing.Point(697, 325);
            ButtonClose.Name = "ButtonClose";
            ButtonClose.Size = new System.Drawing.Size(75, 24);
            ButtonClose.TabIndex = 0;
            ButtonClose.Text = "Close";
            ButtonClose.UseVisualStyleBackColor = true;
            ButtonClose.Click += ButtonClose_Click;
            // 
            // CopyButton
            // 
            CopyButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            CopyButton.FlatAppearance.BorderSize = 0;
            CopyButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            CopyButton.ImageName = "Copy";
            CopyButton.Location = new System.Drawing.Point(13, 324);
            CopyButton.Name = "CopyButton";
            CopyButton.Size = new System.Drawing.Size(27, 27);
            CopyButton.TabIndex = 3;
            CopyButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            toolTip1.SetToolTip(CopyButton, "Copy all to clipboard");
            CopyButton.UseVisualStyleBackColor = true;
            CopyButton.Click += CopyButton_Click;
            // 
            // SaveButton
            // 
            SaveButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            SaveButton.FlatAppearance.BorderSize = 0;
            SaveButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            SaveButton.ImageName = "Save";
            SaveButton.Location = new System.Drawing.Point(46, 324);
            SaveButton.Name = "SaveButton";
            SaveButton.Size = new System.Drawing.Size(27, 27);
            SaveButton.TabIndex = 2;
            SaveButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            toolTip1.SetToolTip(SaveButton, "Save to file");
            SaveButton.UseVisualStyleBackColor = true;
            SaveButton.Click += SaveButton_Click;
            // 
            // ViewSrc
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            CancelButton = ButtonClose;
            ClientSize = new System.Drawing.Size(784, 361);
            Controls.Add(CopyButton);
            Controls.Add(SaveButton);
            Controls.Add(ButtonClose);
            Controls.Add(TextSrcBox);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            Location = new System.Drawing.Point(0, 0);
            MaximizeBox = true;
            MinimumSize = new System.Drawing.Size(300, 200);
            Name = "ViewSrc";
            ShowIcon = false;
            ResumeLayout(false);
        }

        #endregion

        private PdnCodeLab.CodeTextBox TextSrcBox;
        private System.Windows.Forms.Button ButtonClose;
        private ScaledButton SaveButton;
        private ScaledButton CopyButton;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}