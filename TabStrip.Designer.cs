namespace PaintDotNet.Effects
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
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.untitledTab = new PaintDotNet.Effects.TabStrip.Tab();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.AllowItemReorder = true;
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.untitledTab});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Padding = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.toolStrip1.Size = new System.Drawing.Size(300, 25);
            this.toolStrip1.Stretch = true;
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "Tabs";
            // 
            // untitledTab
            // 
            this.untitledTab.AutoToolTip = false;
            this.untitledTab.Checked = true;
            this.untitledTab.CheckState = System.Windows.Forms.CheckState.Checked;
            this.untitledTab.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.untitledTab.ImageName = "Untitled";
            this.untitledTab.Margin = new System.Windows.Forms.Padding(0, 5, 3, 0);
            this.untitledTab.Name = "untitledTab";
            this.untitledTab.Padding = new System.Windows.Forms.Padding(0, 0, 20, 0);
            this.untitledTab.Size = new System.Drawing.Size(89, 20);
            this.untitledTab.Text = "Untitled";
            this.untitledTab.ToolTipText = "Untitled";
            this.untitledTab.Click += new System.EventHandler(this.Tab_Click);
            this.untitledTab.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Tab_MouseDown);
            this.untitledTab.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Tab_MouseUp);
            // 
            // TabStrip
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoSize = true;
            this.Controls.Add(this.toolStrip1);
            this.Name = "TabStrip";
            this.Size = new System.Drawing.Size(300, 25);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private Effects.TabStrip.Tab untitledTab;
    }
}
