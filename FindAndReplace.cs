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
// Latest distribution: http://www.BoltBait.com/pdn/codelab
/////////////////////////////////////////////////////////////////////////////////

using ScintillaNET;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace PaintDotNet.Effects
{
    internal partial class FindAndReplace : UserControl
    {
        private bool replaceVisible = true;

        internal FindAndReplace()
        {
            InitializeComponent();

            // HiDPI Fixes
            int leftMargin = Toggle.Width + Toggle.Margin.Horizontal + FindBox.Margin.Left;
            MatchCase.Margin = new Padding(leftMargin, MatchCase.Margin.Top, MatchCase.Margin.Right, MatchCase.Margin.Bottom);
            ReplaceBox.Margin = new Padding(leftMargin, ReplaceBox.Margin.Top, ReplaceBox.Margin.Right, ReplaceBox.Margin.Bottom);

            int emptySpace = toolStrip1.ClientSize.Width - (toolStrip1.Padding.Horizontal + Toggle.Width + Toggle.Margin.Horizontal +
                             FindBox.Width + FindBox.Margin.Horizontal + Next.Width + Next.Margin.Horizontal + Close.Width + Close.Margin.Horizontal);
            Close.Margin = new Padding(emptySpace, Close.Margin.Top, Close.Margin.Right, Close.Margin.Bottom);
        }

        #region Properties
        internal string Term
        {
            get
            {
                return (!EscChars.Checked) ? FindBox.Text : FindBox.Text.Replace("\\t", "\t").Replace("\\n", "\n").Replace("\\r", "\r");
            }
            set
            {
                if (value.Contains("\r") || value.Contains("\n") || value.Contains("\t"))
                {
                    value = value.Replace("\t", "\\t").Replace("\n", "\\n").Replace("\r", "\\r");
                    EscChars.Checked = true;
                }
                FindBox.Text = value;
            }
        }

        internal string Replacement
        {
            get
            {
                return (!EscChars.Checked) ? ReplaceBox.Text : ReplaceBox.Text.Replace("\\t", "\t").Replace("\\n", "\n").Replace("\\r", "\r");
            }
        }

        internal bool ShowReplace
        {
            set
            {
                replaceVisible = !value;
                ToggleReplace();
            }
        }

        internal SearchFlags Flags
        {
            get
            {
                SearchFlags flags = SearchFlags.None;

                if (MatchCase.Checked)
                {
                    flags |= SearchFlags.MatchCase;
                }

                if (MatchWord.Checked)
                {
                    flags |= SearchFlags.WholeWord;
                }

                if (Regex.Checked)
                {
                    flags |= SearchFlags.Regex;
                }

                return flags;
            }
        }

        internal int Matches
        {
            set
            {
                string matches = (value == 1) ? "Match" : "Matches";
                this.Next.ToolTipText = $"Find Next\r\n({value} {matches})";
            }
        }
        #endregion

        #region Event Handlers
        internal event EventHandler ParametersChanged;
        protected void OnParametersChanged()
        {
            this.ParametersChanged?.Invoke(this, EventArgs.Empty);
        }

        internal event EventHandler ReplaceAllClicked;
        protected void OnReplaceAllClicked()
        {
            this.ReplaceAllClicked?.Invoke(this, EventArgs.Empty);
        }

        internal event EventHandler FindNextClicked;
        protected void OnFindNextClicked()
        {
            this.FindNextClicked?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        internal void UpdateTheme()
        {
            toolStrip1.Renderer = PdnTheme.Renderer;
        }

        private void Close_Click(object sender, EventArgs e)
        {
            this.Visible = false;
        }

        private void MatchCase_Click(object sender, EventArgs e)
        {
            MatchCase.Checked = !MatchCase.Checked;
            OnParametersChanged();
        }

        private void MatchWord_Click(object sender, EventArgs e)
        {
            MatchWord.Checked = !MatchWord.Checked;
            OnParametersChanged();
        }

        private void Regex_Click(object sender, EventArgs e)
        {
            Regex.Checked = !Regex.Checked;
            OnParametersChanged();
        }

        private void EscChars_Click(object sender, EventArgs e)
        {
            EscChars.Checked = !EscChars.Checked;
            OnParametersChanged();
        }

        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);

            FindBox.Focus();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                this.Visible = false;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void Toggle_Click(object sender, EventArgs e)
        {
            ToggleReplace();
        }

        private void ToggleReplace()
        {
            if (replaceVisible)
            {
                Toggle.Text = "▼";
                ReplaceBox.Visible = false;
                ReplaceAll.Visible = false;
                this.Height = ReplaceBox.Bounds.Top;
                replaceVisible = false;
            }
            else
            {
                Toggle.Text = "▲";
                ReplaceBox.Visible = true;
                ReplaceAll.Visible = true;
                this.Height = ReplaceBox.Bounds.Bottom + FindBox.Bounds.Top;
                replaceVisible = true;
            }
        }

        private void FindBox_TextChanged(object sender, EventArgs e)
        {
            OnParametersChanged();
        }

        private void ReplaceAll_Click(object sender, EventArgs e)
        {
            OnReplaceAllClicked();
        }

        private void toolStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            toolStrip1.Focus();
        }

        private void Next_Click(object sender, EventArgs e)
        {
            OnFindNextClicked();
        }
    }
}
