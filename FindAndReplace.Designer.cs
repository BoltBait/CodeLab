namespace PaintDotNet.Effects
{
    partial class FindAndReplace
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
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.Toggle = new System.Windows.Forms.ToolStripButton();
            this.FindBox = new System.Windows.Forms.ToolStripTextBox();
            this.HitCount = new System.Windows.Forms.ToolStripLabel();
            this.Close = new System.Windows.Forms.ToolStripButton();
            this.MatchCase = new System.Windows.Forms.ToolStripButton();
            this.MatchWord = new System.Windows.Forms.ToolStripButton();
            this.Regex = new System.Windows.Forms.ToolStripButton();
            this.EscChars = new System.Windows.Forms.ToolStripButton();
            this.ReplaceBox = new System.Windows.Forms.ToolStripTextBox();
            this.ReplaceAll = new System.Windows.Forms.ToolStripButton();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Toggle,
            this.FindBox,
            this.HitCount,
            this.Close,
            this.MatchCase,
            this.MatchWord,
            this.Regex,
            this.EscChars,
            this.ReplaceBox,
            this.ReplaceAll});
            this.toolStrip1.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Flow;
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Padding = new System.Windows.Forms.Padding(3);
            this.toolStrip1.Size = new System.Drawing.Size(234, 78);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            this.toolStrip1.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.toolStrip1_ItemClicked);
            // 
            // Toggle
            // 
            this.Toggle.AutoToolTip = false;
            this.Toggle.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.Toggle.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Toggle.Margin = new System.Windows.Forms.Padding(0, 1, 3, 2);
            this.Toggle.Name = "Toggle";
            this.Toggle.Size = new System.Drawing.Size(23, 19);
            this.Toggle.Text = "▲";
            this.Toggle.ToolTipText = "Find / Replace";
            this.Toggle.Click += new System.EventHandler(this.Toggle_Click);
            // 
            // FindBox
            // 
            this.FindBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.FindBox.Name = "FindBox";
            this.FindBox.Size = new System.Drawing.Size(150, 23);
            this.FindBox.TextChanged += new System.EventHandler(this.FindBox_TextChanged);
            // 
            // HitCount
            // 
            this.HitCount.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.HitCount.Margin = new System.Windows.Forms.Padding(0, 3, 0, 2);
            this.HitCount.Name = "HitCount";
            this.HitCount.Size = new System.Drawing.Size(13, 15);
            this.HitCount.Text = "0";
            this.HitCount.ToolTipText = "Matches";
            // 
            // Close
            // 
            this.Close.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.Close.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Close.Name = "Close";
            this.Close.Size = new System.Drawing.Size(23, 19);
            this.Close.Text = "X";
            this.Close.ToolTipText = "Close (Esc)";
            this.Close.Click += new System.EventHandler(this.Close_Click);
            // 
            // MatchCase
            // 
            this.MatchCase.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.MatchCase.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MatchCase.Margin = new System.Windows.Forms.Padding(27, 4, 0, 4);
            this.MatchCase.Name = "MatchCase";
            this.MatchCase.Size = new System.Drawing.Size(46, 19);
            this.MatchCase.Text = "Casing";
            this.MatchCase.ToolTipText = "Match Casing";
            this.MatchCase.Click += new System.EventHandler(this.MatchCase_Click);
            // 
            // MatchWord
            // 
            this.MatchWord.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.MatchWord.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MatchWord.Margin = new System.Windows.Forms.Padding(4, 4, 0, 4);
            this.MatchWord.Name = "MatchWord";
            this.MatchWord.Size = new System.Drawing.Size(48, 19);
            this.MatchWord.Text = "[word]";
            this.MatchWord.ToolTipText = "Match Whole Word";
            this.MatchWord.Click += new System.EventHandler(this.MatchWord_Click);
            // 
            // Regex
            // 
            this.Regex.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.Regex.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Regex.Margin = new System.Windows.Forms.Padding(4, 4, 0, 4);
            this.Regex.Name = "Regex";
            this.Regex.Size = new System.Drawing.Size(47, 19);
            this.Regex.Text = "Regex";
            this.Regex.ToolTipText = "Use Regular Expressions";
            this.Regex.Click += new System.EventHandler(this.Regex_Click);
            // 
            // EscChars
            // 
            this.EscChars.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.EscChars.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.EscChars.Margin = new System.Windows.Forms.Padding(4, 4, 0, 4);
            this.EscChars.Name = "EscChars";
            this.EscChars.Size = new System.Drawing.Size(46, 19);
            this.EscChars.Text = "\\Chars";
            this.EscChars.ToolTipText = "Interpret Escape Characters";
            this.EscChars.Click += new System.EventHandler(this.EscChars_Click);
            // 
            // ReplaceBox
            // 
            this.ReplaceBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ReplaceBox.Margin = new System.Windows.Forms.Padding(27, 0, 1, 0);
            this.ReplaceBox.Name = "ReplaceBox";
            this.ReplaceBox.Size = new System.Drawing.Size(150, 23);
            // 
            // ReplaceAll
            // 
            this.ReplaceAll.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.ReplaceAll.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ReplaceAll.Margin = new System.Windows.Forms.Padding(4, 1, 0, 2);
            this.ReplaceAll.Name = "ReplaceAll";
            this.ReplaceAll.Size = new System.Drawing.Size(40, 19);
            this.ReplaceAll.Text = "A➝B";
            this.ReplaceAll.ToolTipText = "Replace All";
            this.ReplaceAll.Click += new System.EventHandler(this.ReplaceAll_Click);
            // 
            // FindAndReplace
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.toolStrip1);
            this.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.MaximumSize = new System.Drawing.Size(236, 80);
            this.MinimumSize = new System.Drawing.Size(236, 52);
            this.Name = "FindAndReplace";
            this.Size = new System.Drawing.Size(234, 78);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton Toggle;
        private System.Windows.Forms.ToolStripTextBox FindBox;
        private System.Windows.Forms.ToolStripButton Close;
        private System.Windows.Forms.ToolStripButton MatchCase;
        private System.Windows.Forms.ToolStripButton MatchWord;
        private System.Windows.Forms.ToolStripButton EscChars;
        private System.Windows.Forms.ToolStripTextBox ReplaceBox;
        private System.Windows.Forms.ToolStripButton Regex;
        private System.Windows.Forms.ToolStripButton ReplaceAll;
        private System.Windows.Forms.ToolStripLabel HitCount;
    }
}
