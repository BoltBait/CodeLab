namespace PaintDotNet.Effects
{
    partial class BuildFileTypeDialog
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
            this.cancelButton = new System.Windows.Forms.Button();
            this.sourceButton = new System.Windows.Forms.Button();
            this.solutionButton = new System.Windows.Forms.Button();
            this.buildButton = new System.Windows.Forms.Button();
            this.authorBox = new System.Windows.Forms.TextBox();
            this.minorBox = new System.Windows.Forms.NumericUpDown();
            this.majorBox = new System.Windows.Forms.NumericUpDown();
            this.authorLabel = new System.Windows.Forms.Label();
            this.versionLabel = new System.Windows.Forms.Label();
            this.urlLabel = new System.Windows.Forms.Label();
            this.urlBox = new System.Windows.Forms.TextBox();
            this.nameBox = new System.Windows.Forms.TextBox();
            this.nameLabel = new System.Windows.Forms.Label();
            this.descriptionBox = new System.Windows.Forms.TextBox();
            this.descriptionLabel = new System.Windows.Forms.Label();
            this.layersCheckBox = new System.Windows.Forms.CheckBox();
            this.saveExtBox = new System.Windows.Forms.TextBox();
            this.loadExtBox = new System.Windows.Forms.TextBox();
            this.loadLabel = new System.Windows.Forms.Label();
            this.saveLabel = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.DecimalSymbol = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.minorBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.majorBox)).BeginInit();
            this.SuspendLayout();
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.cancelButton.Location = new System.Drawing.Point(508, 185);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 24);
            this.cancelButton.TabIndex = 1;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // sourceButton
            // 
            this.sourceButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.sourceButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.sourceButton.Location = new System.Drawing.Point(12, 185);
            this.sourceButton.Name = "sourceButton";
            this.sourceButton.Size = new System.Drawing.Size(85, 24);
            this.sourceButton.TabIndex = 23;
            this.sourceButton.Text = "View Source";
            this.sourceButton.UseVisualStyleBackColor = true;
            this.sourceButton.Click += new System.EventHandler(this.sourceButton_Click);
            // 
            // solutionButton
            // 
            this.solutionButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.solutionButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.solutionButton.Location = new System.Drawing.Point(103, 185);
            this.solutionButton.Name = "solutionButton";
            this.solutionButton.Size = new System.Drawing.Size(131, 24);
            this.solutionButton.TabIndex = 24;
            this.solutionButton.Text = "Generate VS Solution";
            this.solutionButton.UseVisualStyleBackColor = true;
            this.solutionButton.Click += new System.EventHandler(this.solutionButton_Click);
            // 
            // buildButton
            // 
            this.buildButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buildButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buildButton.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buildButton.Location = new System.Drawing.Point(427, 185);
            this.buildButton.Name = "buildButton";
            this.buildButton.Size = new System.Drawing.Size(75, 24);
            this.buildButton.TabIndex = 0;
            this.buildButton.Text = "Build";
            this.buildButton.UseVisualStyleBackColor = true;
            this.buildButton.Click += new System.EventHandler(this.buildButton_Click);
            // 
            // authorBox
            // 
            this.authorBox.Location = new System.Drawing.Point(128, 66);
            this.authorBox.Name = "authorBox";
            this.authorBox.Size = new System.Drawing.Size(150, 23);
            this.authorBox.TabIndex = 7;
            // 
            // minorBox
            // 
            this.minorBox.Location = new System.Drawing.Point(228, 95);
            this.minorBox.Name = "minorBox";
            this.minorBox.Size = new System.Drawing.Size(50, 23);
            this.minorBox.TabIndex = 11;
            // 
            // majorBox
            // 
            this.majorBox.Location = new System.Drawing.Point(148, 95);
            this.majorBox.Name = "majorBox";
            this.majorBox.Size = new System.Drawing.Size(50, 23);
            this.majorBox.TabIndex = 9;
            // 
            // authorLabel
            // 
            this.authorLabel.AutoSize = true;
            this.authorLabel.Location = new System.Drawing.Point(26, 69);
            this.authorLabel.Name = "authorLabel";
            this.authorLabel.Size = new System.Drawing.Size(90, 15);
            this.authorLabel.TabIndex = 6;
            this.authorLabel.Text = "Author\'s Name:";
            // 
            // versionLabel
            // 
            this.versionLabel.AutoSize = true;
            this.versionLabel.Location = new System.Drawing.Point(26, 97);
            this.versionLabel.Name = "versionLabel";
            this.versionLabel.Size = new System.Drawing.Size(71, 15);
            this.versionLabel.TabIndex = 8;
            this.versionLabel.Text = "DLL Version:";
            // 
            // urlLabel
            // 
            this.urlLabel.AutoSize = true;
            this.urlLabel.Location = new System.Drawing.Point(26, 127);
            this.urlLabel.Name = "urlLabel";
            this.urlLabel.Size = new System.Drawing.Size(76, 15);
            this.urlLabel.TabIndex = 12;
            this.urlLabel.Text = "Support URL:";
            // 
            // urlBox
            // 
            this.urlBox.Location = new System.Drawing.Point(128, 124);
            this.urlBox.Name = "urlBox";
            this.urlBox.Size = new System.Drawing.Size(150, 23);
            this.urlBox.TabIndex = 13;
            // 
            // nameBox
            // 
            this.nameBox.Location = new System.Drawing.Point(128, 37);
            this.nameBox.Name = "nameBox";
            this.nameBox.Size = new System.Drawing.Size(150, 23);
            this.nameBox.TabIndex = 5;
            // 
            // nameLabel
            // 
            this.nameLabel.AutoSize = true;
            this.nameLabel.Location = new System.Drawing.Point(26, 40);
            this.nameLabel.Name = "nameLabel";
            this.nameLabel.Size = new System.Drawing.Size(79, 15);
            this.nameLabel.TabIndex = 4;
            this.nameLabel.Text = "Plugin Name:";
            // 
            // descriptionBox
            // 
            this.descriptionBox.Location = new System.Drawing.Point(432, 37);
            this.descriptionBox.Name = "descriptionBox";
            this.descriptionBox.Size = new System.Drawing.Size(150, 23);
            this.descriptionBox.TabIndex = 17;
            this.descriptionBox.Text = "Filetype Plugin";
            // 
            // descriptionLabel
            // 
            this.descriptionLabel.AutoSize = true;
            this.descriptionLabel.Location = new System.Drawing.Point(331, 40);
            this.descriptionLabel.Name = "descriptionLabel";
            this.descriptionLabel.Size = new System.Drawing.Size(70, 15);
            this.descriptionLabel.TabIndex = 16;
            this.descriptionLabel.Text = "Description:";
            // 
            // layersCheckBox
            // 
            this.layersCheckBox.AutoSize = true;
            this.layersCheckBox.Location = new System.Drawing.Point(432, 128);
            this.layersCheckBox.Name = "layersCheckBox";
            this.layersCheckBox.Size = new System.Drawing.Size(109, 19);
            this.layersCheckBox.TabIndex = 22;
            this.layersCheckBox.Text = "Supports Layers";
            this.layersCheckBox.UseVisualStyleBackColor = true;
            // 
            // saveExtBox
            // 
            this.saveExtBox.Location = new System.Drawing.Point(432, 95);
            this.saveExtBox.Name = "saveExtBox";
            this.saveExtBox.Size = new System.Drawing.Size(150, 23);
            this.saveExtBox.TabIndex = 21;
            // 
            // loadExtBox
            // 
            this.loadExtBox.Location = new System.Drawing.Point(432, 66);
            this.loadExtBox.Name = "loadExtBox";
            this.loadExtBox.Size = new System.Drawing.Size(150, 23);
            this.loadExtBox.TabIndex = 19;
            // 
            // loadLabel
            // 
            this.loadLabel.AutoSize = true;
            this.loadLabel.Location = new System.Drawing.Point(331, 69);
            this.loadLabel.Name = "loadLabel";
            this.loadLabel.Size = new System.Drawing.Size(95, 15);
            this.loadLabel.TabIndex = 18;
            this.loadLabel.Text = "Load Extensions:";
            // 
            // saveLabel
            // 
            this.saveLabel.AutoSize = true;
            this.saveLabel.Location = new System.Drawing.Point(331, 98);
            this.saveLabel.Name = "saveLabel";
            this.saveLabel.Size = new System.Drawing.Size(93, 15);
            this.saveLabel.TabIndex = 20;
            this.saveLabel.Text = "Save Extensions:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(11, 12);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(117, 13);
            this.label8.TabIndex = 2;
            this.label8.Text = "Support Information:";
            // 
            // label9
            // 
            this.label9.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label9.Location = new System.Drawing.Point(54, 19);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(225, 2);
            this.label9.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(314, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(94, 13);
            this.label1.TabIndex = 14;
            this.label1.Text = "File Type Details:";
            // 
            // label2
            // 
            this.label2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label2.Location = new System.Drawing.Point(357, 19);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(225, 2);
            this.label2.TabIndex = 15;
            // 
            // DecimalSymbol
            // 
            this.DecimalSymbol.AutoSize = true;
            this.DecimalSymbol.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DecimalSymbol.Location = new System.Drawing.Point(204, 89);
            this.DecimalSymbol.Name = "DecimalSymbol";
            this.DecimalSymbol.Size = new System.Drawing.Size(20, 29);
            this.DecimalSymbol.TabIndex = 10;
            this.DecimalSymbol.Text = ".";
            // 
            // BuildFileTypeDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.ClientSize = new System.Drawing.Size(595, 221);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.saveLabel);
            this.Controls.Add(this.loadLabel);
            this.Controls.Add(this.saveExtBox);
            this.Controls.Add(this.loadExtBox);
            this.Controls.Add(this.layersCheckBox);
            this.Controls.Add(this.descriptionLabel);
            this.Controls.Add(this.descriptionBox);
            this.Controls.Add(this.nameLabel);
            this.Controls.Add(this.nameBox);
            this.Controls.Add(this.urlBox);
            this.Controls.Add(this.urlLabel);
            this.Controls.Add(this.versionLabel);
            this.Controls.Add(this.authorLabel);
            this.Controls.Add(this.majorBox);
            this.Controls.Add(this.minorBox);
            this.Controls.Add(this.authorBox);
            this.Controls.Add(this.buildButton);
            this.Controls.Add(this.solutionButton);
            this.Controls.Add(this.sourceButton);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.DecimalSymbol);
            this.IconName = "SaveAsDll";
            this.Name = "BuildFileTypeDialog";
            this.Text = "Building Filetype DLL";
            ((System.ComponentModel.ISupportInitialize)(this.minorBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.majorBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button sourceButton;
        private System.Windows.Forms.Button solutionButton;
        private System.Windows.Forms.Button buildButton;
        private System.Windows.Forms.TextBox authorBox;
        private System.Windows.Forms.NumericUpDown minorBox;
        private System.Windows.Forms.NumericUpDown majorBox;
        private System.Windows.Forms.Label authorLabel;
        private System.Windows.Forms.Label versionLabel;
        private System.Windows.Forms.Label urlLabel;
        private System.Windows.Forms.TextBox urlBox;
        private System.Windows.Forms.TextBox nameBox;
        private System.Windows.Forms.Label nameLabel;
        private System.Windows.Forms.TextBox descriptionBox;
        private System.Windows.Forms.Label descriptionLabel;
        private System.Windows.Forms.CheckBox layersCheckBox;
        private System.Windows.Forms.TextBox saveExtBox;
        private System.Windows.Forms.TextBox loadExtBox;
        private System.Windows.Forms.Label loadLabel;
        private System.Windows.Forms.Label saveLabel;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label DecimalSymbol;
    }
}