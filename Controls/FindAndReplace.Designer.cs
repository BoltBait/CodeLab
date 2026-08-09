namespace PdnCodeLab
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
            toolStrip1 = new System.Windows.Forms.ToolStrip();
            Toggle = new System.Windows.Forms.ToolStripButton();
            FindBox = new System.Windows.Forms.ToolStripTextBox();
            Next = new System.Windows.Forms.ToolStripButton();
            Close = new System.Windows.Forms.ToolStripButton();
            MatchCase = new System.Windows.Forms.ToolStripButton();
            MatchWord = new System.Windows.Forms.ToolStripButton();
            Regex = new System.Windows.Forms.ToolStripButton();
            EscChars = new System.Windows.Forms.ToolStripButton();
            ReplaceBox = new System.Windows.Forms.ToolStripTextBox();
            ReplaceAll = new System.Windows.Forms.ToolStripButton();
            toolStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // toolStrip1
            // 
            toolStrip1.AllowClickThrough = true;
            toolStrip1.Dock = System.Windows.Forms.DockStyle.Fill;
            toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { Toggle, FindBox, Next, Close, MatchCase, MatchWord, Regex, EscChars, ReplaceBox, ReplaceAll });
            toolStrip1.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Flow;
            toolStrip1.Location = new System.Drawing.Point(0, 0);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.Padding = new System.Windows.Forms.Padding(3);
            toolStrip1.Size = new System.Drawing.Size(234, 78);
            toolStrip1.TabIndex = 0;
            toolStrip1.Text = "toolStrip1";
            toolStrip1.ItemClicked += toolStrip1_ItemClicked;
            toolStrip1.Paint += toolStrip1_Paint;
            // 
            // Toggle
            // 
            Toggle.AutoToolTip = false;
            Toggle.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            Toggle.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
            Toggle.Margin = new System.Windows.Forms.Padding(0, 1, 3, 2);
            Toggle.Name = "Toggle";
            Toggle.Size = new System.Drawing.Size(23, 19);
            Toggle.Text = "▲";
            Toggle.ToolTipText = "Find / Replace";
            Toggle.Click += Toggle_Click;
            // 
            // FindBox
            // 
            FindBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            FindBox.Name = "FindBox";
            FindBox.Size = new System.Drawing.Size(150, 23);
            FindBox.KeyPress += FindBox_KeyPress;
            FindBox.TextChanged += FindBox_TextChanged;
            // 
            // Next
            // 
            Next.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            Next.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
            Next.Name = "Next";
            Next.Size = new System.Drawing.Size(23, 19);
            Next.Text = "▶";
            Next.ToolTipText = "Find Next";
            Next.Click += Next_Click;
            // 
            // Close
            // 
            Close.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            Close.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
            Close.Name = "Close";
            Close.Size = new System.Drawing.Size(23, 19);
            Close.Text = "X";
            Close.ToolTipText = "Close (Esc)";
            Close.Click += Close_Click;
            // 
            // MatchCase
            // 
            MatchCase.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            MatchCase.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
            MatchCase.Margin = new System.Windows.Forms.Padding(27, 4, 0, 4);
            MatchCase.Name = "MatchCase";
            MatchCase.Size = new System.Drawing.Size(46, 19);
            MatchCase.Text = "Casing";
            MatchCase.ToolTipText = "Match Casing";
            MatchCase.Click += MatchCase_Click;
            // 
            // MatchWord
            // 
            MatchWord.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            MatchWord.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
            MatchWord.Margin = new System.Windows.Forms.Padding(4, 4, 0, 4);
            MatchWord.Name = "MatchWord";
            MatchWord.Size = new System.Drawing.Size(48, 19);
            MatchWord.Text = "[word]";
            MatchWord.ToolTipText = "Match Whole Word";
            MatchWord.Click += MatchWord_Click;
            // 
            // Regex
            // 
            Regex.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            Regex.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
            Regex.Margin = new System.Windows.Forms.Padding(4, 4, 0, 4);
            Regex.Name = "Regex";
            Regex.Size = new System.Drawing.Size(47, 19);
            Regex.Text = "Regex";
            Regex.ToolTipText = "Use Regular Expressions";
            Regex.Click += Regex_Click;
            // 
            // EscChars
            // 
            EscChars.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            EscChars.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
            EscChars.Margin = new System.Windows.Forms.Padding(4, 4, 0, 4);
            EscChars.Name = "EscChars";
            EscChars.Size = new System.Drawing.Size(46, 19);
            EscChars.Text = "\\Chars";
            EscChars.ToolTipText = "Interpret Escape Characters";
            EscChars.Click += EscChars_Click;
            // 
            // ReplaceBox
            // 
            ReplaceBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            ReplaceBox.Margin = new System.Windows.Forms.Padding(27, 0, 1, 0);
            ReplaceBox.Name = "ReplaceBox";
            ReplaceBox.Size = new System.Drawing.Size(150, 23);
            // 
            // ReplaceAll
            // 
            ReplaceAll.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            ReplaceAll.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
            ReplaceAll.Margin = new System.Windows.Forms.Padding(4, 1, 0, 2);
            ReplaceAll.Name = "ReplaceAll";
            ReplaceAll.Size = new System.Drawing.Size(40, 19);
            ReplaceAll.Text = "A➝B";
            ReplaceAll.ToolTipText = "Replace All";
            ReplaceAll.Click += ReplaceAll_Click;
            // 
            // FindAndReplace
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            Controls.Add(toolStrip1);
            Cursor = System.Windows.Forms.Cursors.Arrow;
            Name = "FindAndReplace";
            Size = new System.Drawing.Size(234, 78);
            toolStrip1.ResumeLayout(false);
            toolStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();

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
        private System.Windows.Forms.ToolStripButton Next;
    }
}
