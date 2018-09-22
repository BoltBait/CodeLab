/////////////////////////////////////////////////////////////////////////////////
// CodeLab for Paint.NET
// Portions Copyright ©2007-2011 BoltBait. All Rights Reserved.
// Portions Copyright ©Microsoft Corporation. All Rights Reserved.
//
// THE CODELAB DEVELOPERS MAKE NO WARRANTY OF ANY KIND REGARDING THE CODE. THEY
// SPECIFICALLY DISCLAIM ANY WARRANTY OF FITNESS FOR ANY PARTICULAR PURPOSE OR
// ANY OTHER WARRANTY.  THE CODELAB DEVELOPERS DISCLAIM ALL LIABILITY RELATING
// TO THE USE OF THIS CODE.  NO LICENSE, EXPRESS OR IMPLIED, BY ESTOPPEL OR
// OTHERWISE, TO ANY INTELLECTUAL PROPERTY RIGHTS IS GRANTED HEREIN.
//
// Latest distribution: http://www.BoltBait.com/pdn/codelab
/////////////////////////////////////////////////////////////////////////////////

using System;
using System.Windows.Forms;
using System.IO;
using System.Drawing;

namespace PaintDotNet.Effects
{
    internal partial class ViewSrc : Form
    {
        internal ViewSrc(string title, string SourceString, bool ShowSaveButton)
        {
            InitializeComponent();

            // PDN Theme
            this.ForeColor = PdnTheme.ForeColor;
            this.BackColor = PdnTheme.BackColor;
            TextSrcBox.ForeColor = PdnTheme.ForeColor;
            TextSrcBox.BackColor = PdnTheme.BackColor;

            // DPI fixes
            CopyButton.Image = CopyButton.Image.ResizeForDPI(this.AutoScaleDimensions);
            SaveButton.Image = SaveButton.Image.ResizeForDPI(this.AutoScaleDimensions);

            TextSrcBox.Font = new Font(Settings.FontFamily, TextSrcBox.Font.Size);
            TextSrcBox.Text = SourceString;
            this.Text = title;
            SaveButton.Visible = ShowSaveButton;
        }

        private void ButtonClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void TextSrcBox_KeyDown(object sender, KeyEventArgs e)
        {
            // Ctrl-A does a Select All in the editor window
            if (e.Control && (e.KeyCode == Keys.A))
            {
                TextSrcBox.SelectAll();
                e.Handled = true;
            }

        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Title = "Save Complete Source Code";
            sfd.DefaultExt = ".cs";
            sfd.Filter = "C# Code Files (*.CS)|*.cs";
            sfd.OverwritePrompt = true;
            sfd.AddExtension = true;
            sfd.FileName = "project.cs";
            sfd.InitialDirectory = Settings.LastSourceDirectory;

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    File.WriteAllText(sfd.FileName, TextSrcBox.Text);
                }
                catch
                {
                    MessageBox.Show("Error saving source file.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            sfd.Dispose();
        }

        private void CopyButton_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(TextSrcBox.Text);
        }
    }
}
