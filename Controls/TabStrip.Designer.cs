namespace PdnCodeLab
{
    partial class TabStrip
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
            untitledTab = new Tab();
            toolStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // toolStrip1
            // 
            toolStrip1.AllowClickThrough = true;
            toolStrip1.AllowItemReorder = true;
            toolStrip1.Dock = System.Windows.Forms.DockStyle.Fill;
            toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { untitledTab });
            toolStrip1.Location = new System.Drawing.Point(0, 0);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.Padding = new System.Windows.Forms.Padding(5, 0, 5, 0);
            toolStrip1.Size = new System.Drawing.Size(300, 25);
            toolStrip1.Stretch = true;
            toolStrip1.TabIndex = 0;
            toolStrip1.Text = "Tabs";
            // 
            // untitledTab
            // 
            untitledTab.AutoToolTip = false;
            untitledTab.Checked = true;
            untitledTab.CheckState = System.Windows.Forms.CheckState.Checked;
            untitledTab.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            untitledTab.Margin = new System.Windows.Forms.Padding(0, 5, 3, 0);
            untitledTab.Name = "untitledTab";
            untitledTab.Padding = new System.Windows.Forms.Padding(0, 0, 20, 0);
            untitledTab.Size = new System.Drawing.Size(89, 20);
            untitledTab.Text = "Untitled";
            untitledTab.Click += Tab_Click;
            untitledTab.MouseDown += Tab_MouseDown;
            untitledTab.MouseUp += Tab_MouseUp;
            // 
            // TabStrip
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            AutoSize = true;
            Controls.Add(toolStrip1);
            Name = "TabStrip";
            Size = new System.Drawing.Size(300, 25);
            toolStrip1.ResumeLayout(false);
            toolStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private PdnCodeLab.TabStrip.Tab untitledTab;
    }
}
