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
// Latest distribution: https://www.BoltBait.com/pdn/codelab
/////////////////////////////////////////////////////////////////////////////////

using System;
using System.Windows.Forms;
using System.IO;

namespace PdnCodeLab
{
    internal partial class ViewSrc : ChildFormBase
    {
        internal ViewSrc(string title, string SourceString, bool isSourceCode)
        {
            InitializeComponent();

            TextSrcBox.Lexer = isSourceCode ? ScintillaNET.Lexer.Cpp : ScintillaNET.Lexer.Null;
            TextSrcBox.Text = SourceString;
            TextSrcBox.ReadOnly = true;
            TextSrcBox.ApplyUserSettings();

            this.Text = title;
            SaveButton.Visible = isSourceCode;
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            TextSrcBox.Focus();
        }

        private void ButtonClose_Click(object sender, EventArgs e)
        {
            this.Close();
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
                    FlexibleMessageBox.Show("Error saving source file.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            sfd.Dispose();
        }

        private void CopyButton_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.Clipboard.SetText(TextSrcBox.Text);
        }
    }
}
