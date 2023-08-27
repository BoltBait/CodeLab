namespace PdnCodeLab
{
    partial class SnippetManager
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
            this.SnippetList = new System.Windows.Forms.ListBox();
            this.SnippetName = new System.Windows.Forms.TextBox();
            this.UpdateButton = new System.Windows.Forms.Button();
            this.SnippetBody = new PdnCodeLab.CodeTextBox();
            this.DeleteButton = new System.Windows.Forms.Button();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.AddNewButton = new PdnCodeLab.ScaledToolStripButton();
            this.ImportExportMenu = new PdnCodeLab.ScaledToolStripDropDownButton();
            this.ImportFromFileButton = new PdnCodeLab.ScaledToolStripMenuItem();
            this.ImportFromClipButton = new PdnCodeLab.ScaledToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.ExportToFileButton = new PdnCodeLab.ScaledToolStripMenuItem();
            this.ExportToClipButton = new PdnCodeLab.ScaledToolStripMenuItem();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // SnippetList
            // 
            this.SnippetList.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.SnippetList.FormattingEnabled = true;
            this.SnippetList.IntegralHeight = false;
            this.SnippetList.ItemHeight = 15;
            this.SnippetList.Location = new System.Drawing.Point(12, 41);
            this.SnippetList.Name = "SnippetList";
            this.SnippetList.Size = new System.Drawing.Size(75, 362);
            this.SnippetList.TabIndex = 0;
            this.SnippetList.SelectedIndexChanged += new System.EventHandler(this.SnippetList_SelectedIndexChanged);
            // 
            // SnippetName
            // 
            this.SnippetName.Location = new System.Drawing.Point(100, 14);
            this.SnippetName.Name = "SnippetName";
            this.SnippetName.Size = new System.Drawing.Size(100, 23);
            this.SnippetName.TabIndex = 1;
            this.SnippetName.TextChanged += new System.EventHandler(this.SnippetName_TextChanged);
            // 
            // UpdateButton
            // 
            this.UpdateButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.UpdateButton.Location = new System.Drawing.Point(401, 12);
            this.UpdateButton.Name = "UpdateButton";
            this.UpdateButton.Size = new System.Drawing.Size(75, 23);
            this.UpdateButton.TabIndex = 3;
            this.UpdateButton.Text = "Update";
            this.UpdateButton.UseVisualStyleBackColor = true;
            this.UpdateButton.Click += new System.EventHandler(this.UpdateButton_Click);
            // 
            // SnippetBody
            // 
            this.SnippetBody.AutomaticFold = ((ScintillaNET.AutomaticFold)((ScintillaNET.AutomaticFold.Show | ScintillaNET.AutomaticFold.Change)));
            this.SnippetBody.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.SnippetBody.CaretLineBackColor = System.Drawing.Color.GhostWhite;
            this.SnippetBody.CaretLineVisible = true;
            this.SnippetBody.IdleStyling = ScintillaNET.IdleStyling.All;
            this.SnippetBody.Lexer = ScintillaNET.Lexer.Cpp;
            this.SnippetBody.Location = new System.Drawing.Point(100, 41);
            this.SnippetBody.MouseDwellTime = 250;
            this.SnippetBody.Name = "SnippetBody";
            this.SnippetBody.Size = new System.Drawing.Size(376, 364);
            this.SnippetBody.TabIndex = 2;
            this.SnippetBody.Technology = ScintillaNET.Technology.DirectWrite;
            this.SnippetBody.WrapIndentMode = ScintillaNET.WrapIndentMode.Indent;
            this.SnippetBody.TextChanged += new System.EventHandler(this.SnippetButton_TextChanged);
            // 
            // DeleteButton
            // 
            this.DeleteButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.DeleteButton.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.DeleteButton.ForeColor = System.Drawing.Color.Red;
            this.DeleteButton.Location = new System.Drawing.Point(372, 12);
            this.DeleteButton.Name = "DeleteButton";
            this.DeleteButton.Size = new System.Drawing.Size(23, 23);
            this.DeleteButton.TabIndex = 4;
            this.DeleteButton.Text = "✗";
            this.DeleteButton.UseVisualStyleBackColor = true;
            this.DeleteButton.Click += new System.EventHandler(this.DeleteButton_Click);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.AddNewButton,
            this.ImportExportMenu});
            this.toolStrip1.Location = new System.Drawing.Point(9, 9);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(56, 27);
            this.toolStrip1.TabIndex = 5;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // AddNewButton
            // 
            this.AddNewButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.AddNewButton.ImageName = "Add";
            this.AddNewButton.Name = "AddNewButton";
            this.AddNewButton.Padding = new System.Windows.Forms.Padding(2);
            this.AddNewButton.Size = new System.Drawing.Size(24, 24);
            this.AddNewButton.Text = "Add New";
            this.AddNewButton.Click += new System.EventHandler(this.AddNewButton_Click);
            // 
            // ImportExportMenu
            // 
            this.ImportExportMenu.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.ImportExportMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ImportFromFileButton,
            this.ImportFromClipButton,
            this.toolStripSeparator1,
            this.ExportToFileButton,
            this.ExportToClipButton});
            this.ImportExportMenu.ImageName = "Json";
            this.ImportExportMenu.Name = "ImportExportMenu";
            this.ImportExportMenu.Size = new System.Drawing.Size(29, 24);
            this.ImportExportMenu.Text = "Import/Export";
            // 
            // ImportFromFileButton
            // 
            this.ImportFromFileButton.ImageName = "Open";
            this.ImportFromFileButton.Name = "ImportFromFileButton";
            this.ImportFromFileButton.Size = new System.Drawing.Size(225, 22);
            this.ImportFromFileButton.Text = "Import JSON from File";
            this.ImportFromFileButton.Click += new System.EventHandler(this.ImportFromFileButton_Click);
            // 
            // ImportFromClipButton
            // 
            this.ImportFromClipButton.ImageName = "Paste";
            this.ImportFromClipButton.Name = "ImportFromClipButton";
            this.ImportFromClipButton.Size = new System.Drawing.Size(225, 22);
            this.ImportFromClipButton.Text = "Import JSON from Clipboard";
            this.ImportFromClipButton.Click += new System.EventHandler(this.ImportFromClipButton_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(222, 6);
            // 
            // ExportToFileButton
            // 
            this.ExportToFileButton.ImageName = "Save";
            this.ExportToFileButton.Name = "ExportToFileButton";
            this.ExportToFileButton.Size = new System.Drawing.Size(225, 22);
            this.ExportToFileButton.Text = "Export JSON to File";
            this.ExportToFileButton.Click += new System.EventHandler(this.ExportToFileButton_Click);
            // 
            // ExportToClipButton
            // 
            this.ExportToClipButton.ImageName = "Copy";
            this.ExportToClipButton.Name = "ExportToClipButton";
            this.ExportToClipButton.Size = new System.Drawing.Size(225, 22);
            this.ExportToClipButton.Text = "Export JSON to Clipboard";
            this.ExportToClipButton.Click += new System.EventHandler(this.ExportToClipButton_Click);
            // 
            // SnippetManager
            // 
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.DeleteButton);
            this.Controls.Add(this.SnippetBody);
            this.Controls.Add(this.UpdateButton);
            this.Controls.Add(this.SnippetName);
            this.Controls.Add(this.SnippetList);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "SnippetManager";
            this.Size = new System.Drawing.Size(476, 405);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox SnippetList;
        private System.Windows.Forms.TextBox SnippetName;
        private System.Windows.Forms.Button UpdateButton;
        private CodeTextBox SnippetBody;
        private System.Windows.Forms.Button DeleteButton;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private ScaledToolStripButton AddNewButton;
        private ScaledToolStripDropDownButton ImportExportMenu;
        private ScaledToolStripMenuItem ImportFromClipButton;
        private ScaledToolStripMenuItem ImportFromFileButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private ScaledToolStripMenuItem ExportToFileButton;
        private ScaledToolStripMenuItem ExportToClipButton;
    }
}