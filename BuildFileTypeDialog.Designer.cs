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
            ((System.ComponentModel.ISupportInitialize)(this.minorBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.majorBox)).BeginInit();
            this.SuspendLayout();
            // 
            // cancelButton
            // 
            this.cancelButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.cancelButton.Location = new System.Drawing.Point(49, 487);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 24);
            this.cancelButton.TabIndex = 0;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // sourceButton
            // 
            this.sourceButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.sourceButton.Location = new System.Drawing.Point(39, 439);
            this.sourceButton.Name = "sourceButton";
            this.sourceButton.Size = new System.Drawing.Size(85, 24);
            this.sourceButton.TabIndex = 1;
            this.sourceButton.Text = "View Source";
            this.sourceButton.UseVisualStyleBackColor = true;
            this.sourceButton.Click += new System.EventHandler(this.sourceButton_Click);
            // 
            // solutionButton
            // 
            this.solutionButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.solutionButton.Location = new System.Drawing.Point(130, 439);
            this.solutionButton.Name = "solutionButton";
            this.solutionButton.Size = new System.Drawing.Size(131, 24);
            this.solutionButton.TabIndex = 2;
            this.solutionButton.Text = "Generate VS Solution";
            this.solutionButton.UseVisualStyleBackColor = true;
            this.solutionButton.Click += new System.EventHandler(this.solutionButton_Click);
            // 
            // buildButton
            // 
            this.buildButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buildButton.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buildButton.Location = new System.Drawing.Point(130, 487);
            this.buildButton.Name = "buildButton";
            this.buildButton.Size = new System.Drawing.Size(75, 24);
            this.buildButton.TabIndex = 3;
            this.buildButton.Text = "Build";
            this.buildButton.UseVisualStyleBackColor = true;
            this.buildButton.Click += new System.EventHandler(this.buildButton_Click);
            // 
            // authorBox
            // 
            this.authorBox.Location = new System.Drawing.Point(122, 77);
            this.authorBox.Name = "authorBox";
            this.authorBox.Size = new System.Drawing.Size(150, 23);
            this.authorBox.TabIndex = 4;
            // 
            // minorBox
            // 
            this.minorBox.Location = new System.Drawing.Point(222, 120);
            this.minorBox.Name = "minorBox";
            this.minorBox.Size = new System.Drawing.Size(50, 23);
            this.minorBox.TabIndex = 9;
            // 
            // majorBox
            // 
            this.majorBox.Location = new System.Drawing.Point(122, 120);
            this.majorBox.Name = "majorBox";
            this.majorBox.Size = new System.Drawing.Size(50, 23);
            this.majorBox.TabIndex = 10;
            // 
            // authorLabel
            // 
            this.authorLabel.AutoSize = true;
            this.authorLabel.Location = new System.Drawing.Point(12, 80);
            this.authorLabel.Name = "authorLabel";
            this.authorLabel.Size = new System.Drawing.Size(44, 15);
            this.authorLabel.TabIndex = 11;
            this.authorLabel.Text = "Author";
            // 
            // versionLabel
            // 
            this.versionLabel.AutoSize = true;
            this.versionLabel.Location = new System.Drawing.Point(12, 128);
            this.versionLabel.Name = "versionLabel";
            this.versionLabel.Size = new System.Drawing.Size(46, 15);
            this.versionLabel.TabIndex = 12;
            this.versionLabel.Text = "Version";
            // 
            // urlLabel
            // 
            this.urlLabel.AutoSize = true;
            this.urlLabel.Location = new System.Drawing.Point(12, 170);
            this.urlLabel.Name = "urlLabel";
            this.urlLabel.Size = new System.Drawing.Size(28, 15);
            this.urlLabel.TabIndex = 13;
            this.urlLabel.Text = "URL";
            // 
            // urlBox
            // 
            this.urlBox.Location = new System.Drawing.Point(122, 167);
            this.urlBox.Name = "urlBox";
            this.urlBox.Size = new System.Drawing.Size(150, 23);
            this.urlBox.TabIndex = 14;
            // 
            // nameBox
            // 
            this.nameBox.Location = new System.Drawing.Point(122, 27);
            this.nameBox.Name = "nameBox";
            this.nameBox.Size = new System.Drawing.Size(150, 23);
            this.nameBox.TabIndex = 15;
            // 
            // nameLabel
            // 
            this.nameLabel.AutoSize = true;
            this.nameLabel.Location = new System.Drawing.Point(12, 30);
            this.nameLabel.Name = "nameLabel";
            this.nameLabel.Size = new System.Drawing.Size(76, 15);
            this.nameLabel.TabIndex = 16;
            this.nameLabel.Text = "Plugin Name";
            // 
            // descriptionBox
            // 
            this.descriptionBox.Location = new System.Drawing.Point(122, 261);
            this.descriptionBox.Name = "descriptionBox";
            this.descriptionBox.Size = new System.Drawing.Size(150, 23);
            this.descriptionBox.TabIndex = 19;
            // 
            // descriptionLabel
            // 
            this.descriptionLabel.AutoSize = true;
            this.descriptionLabel.Location = new System.Drawing.Point(12, 264);
            this.descriptionLabel.Name = "descriptionLabel";
            this.descriptionLabel.Size = new System.Drawing.Size(67, 15);
            this.descriptionLabel.TabIndex = 20;
            this.descriptionLabel.Text = "Description";
            // 
            // layersCheckBox
            // 
            this.layersCheckBox.AutoSize = true;
            this.layersCheckBox.Location = new System.Drawing.Point(15, 397);
            this.layersCheckBox.Name = "layersCheckBox";
            this.layersCheckBox.Size = new System.Drawing.Size(109, 19);
            this.layersCheckBox.TabIndex = 21;
            this.layersCheckBox.Text = "Supports Layers";
            this.layersCheckBox.UseVisualStyleBackColor = true;
            // 
            // saveExtBox
            // 
            this.saveExtBox.Location = new System.Drawing.Point(122, 350);
            this.saveExtBox.Name = "saveExtBox";
            this.saveExtBox.Size = new System.Drawing.Size(150, 23);
            this.saveExtBox.TabIndex = 23;
            // 
            // loadExtBox
            // 
            this.loadExtBox.Location = new System.Drawing.Point(122, 303);
            this.loadExtBox.Name = "loadExtBox";
            this.loadExtBox.Size = new System.Drawing.Size(150, 23);
            this.loadExtBox.TabIndex = 22;
            // 
            // loadLabel
            // 
            this.loadLabel.AutoSize = true;
            this.loadLabel.Location = new System.Drawing.Point(12, 306);
            this.loadLabel.Name = "loadLabel";
            this.loadLabel.Size = new System.Drawing.Size(91, 15);
            this.loadLabel.TabIndex = 24;
            this.loadLabel.Text = "Load Extensions";
            // 
            // saveLabel
            // 
            this.saveLabel.AutoSize = true;
            this.saveLabel.Location = new System.Drawing.Point(12, 353);
            this.saveLabel.Name = "saveLabel";
            this.saveLabel.Size = new System.Drawing.Size(89, 15);
            this.saveLabel.TabIndex = 25;
            this.saveLabel.Text = "Save Extensions";
            // 
            // BuildFileTypeDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.ClientSize = new System.Drawing.Size(284, 522);
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
            this.IconName = "SaveAsDll";
            this.Name = "BuildFileTypeDialog";
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
    }
}