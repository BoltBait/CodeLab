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

            // HiDPI Fix
            int replaceBoxMargin = Toggle.Width + Toggle.Margin.Horizontal + FindBox.Margin.Left;
            ReplaceBox.Margin = new Padding(replaceBoxMargin, ReplaceBox.Margin.Top, ReplaceBox.Margin.Right, ReplaceBox.Margin.Bottom);
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
                List<SearchFlags> flagList = new List<SearchFlags>();
                if (MatchCase.Checked)
                {
                    flagList.Add(SearchFlags.MatchCase);
                }

                if (MatchWord.Checked)
                {
                    flagList.Add(SearchFlags.WholeWord);
                }

                if (Regex.Checked)
                {
                    flagList.Add(SearchFlags.Regex);
                }

                SearchFlags flags;
                switch (flagList.Count)
                {
                    case 0:
                        flags = SearchFlags.None;
                        break;
                    case 1:
                        flags = flagList[0];
                        break;
                    case 2:
                        flags = flagList[0] | flagList[1];
                        break;
                    case 3:
                        flags = flagList[0] | flagList[1] | flagList[2];
                        break;
                    default:
                        flags = SearchFlags.None;
                        break;
                }
                return flags;
            }
        }

        internal int Matches
        {
            set
            {
                HitCount.Text = value.ToString();

                // Adjust margins to keep close button on far right
                int emptySpace = toolStrip1.ClientSize.Width - (toolStrip1.Padding.Horizontal + Toggle.Width + Toggle.Margin.Horizontal +
                                 FindBox.Width + FindBox.Margin.Horizontal + HitCount.Width + Close.Width + Close.Margin.Horizontal);
                HitCount.Margin = new Padding(emptySpace / 2, HitCount.Margin.Top, emptySpace / 2, HitCount.Margin.Bottom);
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
                this.Height = this.MinimumSize.Height;
                replaceVisible = false;
            }
            else
            {
                Toggle.Text = "▲";
                ReplaceBox.Visible = true;
                ReplaceAll.Visible = true;
                this.Height = this.MaximumSize.Height;
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
    }
}
