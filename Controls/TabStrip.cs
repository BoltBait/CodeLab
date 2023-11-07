/////////////////////////////////////////////////////////////////////////////////
// CodeLab for Paint.NET
// Copyright 2018 Jason Wendt. All Rights Reserved.
// Portions Copyright ©Microsoft Corporation. All Rights Reserved.
//
// THE DEVELOPERS MAKE NO WARRANTY OF ANY KIND REGARDING THE CODE. THEY
// SPECIFICALLY DISCLAIM ANY WARRANTY OF FITNESS FOR ANY PARTICULAR PURPOSE OR
// ANY OTHER WARRANTY.  THE CODELAB DEVELOPERS DISCLAIM ALL LIABILITY RELATING
// TO THE USE OF THIS CODE.  NO LICENSE, EXPRESS OR IMPLIED, BY ESTOPPEL OR
// OTHERWISE, TO ANY INTELLECTUAL PROPERTY RIGHTS IS GRANTED HEREIN.
//
// Latest distribution: https://www.BoltBait.com/pdn/codelab
/////////////////////////////////////////////////////////////////////////////////

using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace PdnCodeLab
{
    [DefaultEvent(nameof(SelectedTabChanged))]
    public partial class TabStrip : UserControl
    {
        private Tab activeTab;
        private int closingTabIndex = -1;

        public TabStrip()
        {
            InitializeComponent();

            activeTab = untitledTab;
        }

        #region Properties
        internal int TabCount
        {
            get => this.toolStrip1.Items.Count;
        }

        internal bool AnyDirtyTabs
        {
            get => this.toolStrip1.Items
                .OfType<Tab>()
                .Any(tab => tab.IsDirty);
        }

        internal Guid SelectedTabGuid
        {
            get => activeTab.Guid;
        }

        internal ProjectType SelectedTabProjType
        {
            get => activeTab.ProjectType;
        }

        internal string SelectedTabTitle
        {
            get => activeTab.Title;
            set
            {
                activeTab.Title = value;
                activeTab.Text = activeTab.IsDirty ? value + "*" : value;
            }
        }

        internal string SelectedTabPath
        {
            get => activeTab.Path;
            set
            {
                if (value == activeTab.Path)
                {
                    return;
                }

                activeTab.Path = value;
                activeTab.ToolTipText = (value.IsNullOrEmpty()) ? activeTab.Title : value;

                if (!activeTab.ProjectType.IsEffect())
                {
                    return;
                }

                string imagePath = Path.ChangeExtension(value, ".png");
                if (File.Exists(imagePath))
                {
                    activeTab.Image = UIUtil.GetBitmapFromFile(imagePath);
                }
                else
                {
                    switch (activeTab.ProjectType)
                    {
                        case ProjectType.ClassicEffect:
                            activeTab.ImageName = "ClassicEffect";
                            break;
                        case ProjectType.GpuDrawEffect:
                        case ProjectType.GpuEffect:
                            activeTab.ImageName = "GpuEffect";
                            break;
                        case ProjectType.BitmapEffect:
                            activeTab.ImageName = "BitmapEffect";
                            break;
                    }
                }
            }
        }

        internal bool SelectedTabIsDirty
        {
            get => activeTab.IsDirty;
            set
            {
                activeTab.IsDirty = value;
                activeTab.Text = value ? activeTab.Title + "*" : activeTab.Title;
            }
        }

        internal bool SelectedTabIsInitial
        {
            get => activeTab.Guid == Guid.Empty;
        }
        #endregion

        #region Event Handlers
        public event EventHandler SelectedTabChanged;
        protected void OnSelectedTabChanged()
        {
            this.SelectedTabChanged?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler NewTabCreated;
        protected void OnNewTabCreated()
        {
            this.NewTabCreated?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler<CancelEventArgs> TabClosingAndDirty;
        protected void OnTabClosingAndDirty(CancelEventArgs args)
        {
            this.TabClosingAndDirty?.Invoke(this, args);
        }

        public event EventHandler<TabClosedEventArgs> TabClosed;
        protected void OnTabClosed(TabClosedEventArgs args)
        {
            this.TabClosed?.Invoke(this, args);
        }
        #endregion

        internal void NextTab()
        {
            if (this.toolStrip1.Items.Count <= 1)
            {
                return;
            }

            int nextIndex = (activeTab.Index >= this.toolStrip1.Items.Count - 1) ? 0 : activeTab.Index + 1;
            SwitchToTab(nextIndex);
        }

        internal void PrevTab()
        {
            if (this.toolStrip1.Items.Count <= 2)
            {
                return;
            }

            int prevIndex = (activeTab.Index <= 0) ? this.toolStrip1.Items.Count - 1 : activeTab.Index - 1;
            SwitchToTab(prevIndex);
        }

        private void SwitchToTab(Tab tab)
        {
            activeTab.Checked = false;
            activeTab.Overflow = ToolStripItemOverflow.AsNeeded;

            activeTab = tab;
            activeTab.Checked = true;
            activeTab.Overflow = ToolStripItemOverflow.Never;

            OnSelectedTabChanged();
        }

        private void SwitchToTab(int tabIndex)
        {
            if (tabIndex < 0 || tabIndex >= this.toolStrip1.Items.Count)
            {
                return;
            }

            if (this.toolStrip1.Items[tabIndex] is Tab tab)
            {
                SwitchToTab(tab);
            }
        }

        internal void CloseFirstTab()
        {
            if (this.toolStrip1.Items.Count > 1 && this.toolStrip1.Items[0] is Tab tab)
            {
                CloseTab(tab);
            }
        }

        internal void CloseTab()
        {
            CloseTab(activeTab);
        }

        private void CloseTab(Tab tab)
        {
            if (this.toolStrip1.Items.Count == 1)
            {
                // there's only one tab, so don't close it
                return;
            }

            if (tab.IsDirty)
            {
                if (tab.Index != activeTab.Index && !tab.Checked)
                {
                    SwitchToTab(tab);
                }

                CancelEventArgs cancelEventArgs = new CancelEventArgs();
                OnTabClosingAndDirty(cancelEventArgs);

                if (cancelEventArgs.Cancel)
                {
                    return;
                }
            }

            Guid guidOfClosedTab = tab.Guid;
            int tabIndexToSwitchTo = (tab.Index >= this.toolStrip1.Items.Count - 1) ? tab.Index - 1 : tab.Index;
            this.toolStrip1.Items.Remove(tab);
            UpdateIndexes();

            if (tab.Index == activeTab.Index && tab.Checked)
            {
                SwitchToTab(tabIndexToSwitchTo);
            }

            OnTabClosed(new TabClosedEventArgs(guidOfClosedTab));
        }

        internal void NewTab(string title, string path, ProjectType projectType)
        {
            int tabIndex = this.toolStrip1.Items.Count;

            Tab newTab = new Tab(title, path, projectType);
            newTab.Index = tabIndex;
            newTab.Click += Tab_Click;
            newTab.MouseDown += Tab_MouseDown;
            newTab.MouseUp += Tab_MouseUp;

            this.toolStrip1.Items.Add(newTab);

            activeTab.Checked = false;
            activeTab.Overflow = ToolStripItemOverflow.AsNeeded;

            activeTab = newTab;
            activeTab.Checked = true;
            activeTab.Overflow = ToolStripItemOverflow.Never;

            OnNewTabCreated();
        }

        private void UpdateIndexes()
        {
            int index = 0;
            foreach (ToolStripItem item in this.toolStrip1.Items)
            {
                if (item is Tab tab)
                {
                    tab.Index = index;
                    index++;
                }
            }
        }

        private void Tab_Click(object sender, EventArgs e)
        {
            if (sender is Tab clickedTab &&
                clickedTab.Index != activeTab.Index &&
                clickedTab.Index != this.closingTabIndex)
            {
                SwitchToTab(clickedTab);
            }
        }

        private void Tab_MouseUp(object sender, MouseEventArgs e)
        {
            if (sender is Tab tab &&
                (e.Button == MouseButtons.Middle || tab.CloseRect.Contains(e.Location)) &&
                tab.Index == this.closingTabIndex)
            {
                CloseTab(tab);
            }

            this.closingTabIndex = -1;
        }

        private void Tab_MouseDown(object sender, MouseEventArgs e)
        {
            this.closingTabIndex = -1;

            if (sender is Tab tab &&
                (e.Button == MouseButtons.Middle || tab.CloseRect.Contains(e.Location)))
            {
                this.closingTabIndex = tab.Index;
            }
        }

        private sealed class Tab : ScaledToolStripButton
        {
            internal int Index { get; set; }
            internal Guid Guid { get; }
            internal ProjectType ProjectType { get; }
            internal bool IsDirty { get; set; }
            internal string Title { get; set; }
            internal string Path { get; set; }
            internal Rectangle CloseRect => closeRect;

            [Browsable(false)]
            [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
            public new string ImageName
            {
                get => base.ImageName;
                set => base.ImageName = value;
            }

            private Rectangle closeRect = Rectangle.Empty;
            private bool closeRectHiLite = false;

            /// <summary>
            /// Do NOT USE. For Initial Tab only.
            /// </summary>
            public Tab()
                : this("Untitled", string.Empty, ProjectType.Default)
            {
                this.Guid = Guid.Empty;
            }

            internal Tab(string title, string path, ProjectType projectType)
            {
                this.ImageAlign = ContentAlignment.MiddleLeft;
                this.Margin = new Padding(0, 5, 3, 0);
                this.AutoToolTip = false;

                string imagePath = System.IO.Path.ChangeExtension(path, ".png");
                if (projectType.IsEffect() && File.Exists(imagePath))
                {
                    this.Image = UIUtil.GetBitmapFromFile(imagePath);
                }
                else
                {
                    this.ImageName = projectType switch
                    {
                        ProjectType.ClassicEffect => "ClassicEffect",
                        ProjectType.GpuEffect => "GpuEffect",
                        ProjectType.GpuDrawEffect => "GpuEffect",
                        ProjectType.BitmapEffect => "BitmapEffect",
                        ProjectType.FileType => "Save",
                        ProjectType.Reference => "Book",
                        ProjectType.Shape => "Shape",
                        _ => "PlainText",
                    };
                }

                this.Text = title;
                this.ToolTipText = path.IsNullOrEmpty() ? title : path;
                this.Guid = Guid.NewGuid();
                this.ProjectType = projectType;
                this.IsDirty = false;
                this.Title = title;
                this.Path = path;

                closeRect = Rectangle.Empty;
            }

            protected override void OnTextChanged(EventArgs e)
            {
                base.OnTextChanged(e);

                UpdateCloseRect();
            }

            private void UpdateCloseRect()
            {
                int size = this.Height / 2;
                int padding = this.Height / 4;
                closeRect = new Rectangle
                {
                    X = this.Width - closeRect.Width - padding,
                    Y = padding,
                    Width = size,
                    Height = size
                };
            }

            protected override void OnLayout(LayoutEventArgs e)
            {
                base.OnLayout(e);

                if (this.Padding.Right != this.Height)
                {
                    this.Padding = new Padding(0, 0, this.Height, 0);
                }
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                base.OnPaint(e);

                if (this.Parent.Items.Count <= 1)
                {
                    return;
                }

                if (!e.ClipRectangle.Contains(closeRect) || closeRect.IsEmpty)
                {
                    UpdateCloseRect();
                }

                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
                e.Graphics.FillRectangle(closeRectHiLite ? Brushes.DarkRed : Brushes.Crimson, closeRect);
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                using (Pen whitePen = new Pen(Color.White, 1.6f))
                {
                    e.Graphics.DrawLine(whitePen, closeRect.Left + 2, closeRect.Top + 2, closeRect.Right - 3, closeRect.Bottom - 3);
                    e.Graphics.DrawLine(whitePen, closeRect.Left + 2, closeRect.Bottom - 3, closeRect.Right - 3, closeRect.Top + 2);
                }
            }

            protected override void OnMouseMove(MouseEventArgs mea)
            {
                if (closeRect.Contains(mea.Location))
                {
                    if (!closeRectHiLite)
                    {
                        closeRectHiLite = true;
                        this.Invalidate();
                    }
                }
                else if (closeRectHiLite)
                {
                    closeRectHiLite = false;
                    this.Invalidate();
                }

                base.OnMouseMove(mea);
            }

            protected override void OnMouseLeave(EventArgs e)
            {
                if (closeRectHiLite)
                {
                    closeRectHiLite = false;
                    this.Invalidate();
                }

                base.OnMouseLeave(e);
            }
        }
    }

    public class TabClosedEventArgs : EventArgs
    {
        public Guid TabGuid { get; }

        public TabClosedEventArgs(Guid tabGuid)
        {
            this.TabGuid = tabGuid;
        }
    }

    public class NewTabEventArgs : EventArgs
    {
        public string Name { get; }
        public string Path { get; }

        public NewTabEventArgs(string name, string path)
        {
            this.Name = name;
            this.Path = path;
        }
    }
}
