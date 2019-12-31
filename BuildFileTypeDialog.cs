using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PaintDotNet.Effects
{
    internal partial class BuildFileTypeDialog : ChildFormBase
    {
        internal string Author;
        internal string URL;
        internal string PluginName;
        internal string Description;
        internal int Major;
        internal int Minor;
        internal string LoadExt;
        internal string SaveExt;
        internal bool Layers;

        private readonly bool isClassic;
        private readonly string userCode;
        private readonly string fileName;

        internal BuildFileTypeDialog(string scriptPath, string scriptText, bool isClassic)
        {
            InitializeComponent();

            foreach (Control control in this.Controls)
            {
                if (control is TextBox || control is NumericUpDown)
                {
                    control.ForeColor = this.ForeColor;
                    control.BackColor = this.BackColor;
                }
            }

            this.fileName = Path.GetFileNameWithoutExtension(scriptPath);
            this.nameBox.Text = this.fileName;
            this.userCode = scriptText;
            this.isClassic = isClassic;

            this.Text = $"Building {this.fileName}.DLL";
        }

        private void sourceButton_Click(object sender, EventArgs e)
        {
            UpdateAllValues();

            string sourceCode = ScriptWriter.FullFileTypeSourceCode(this.userCode, this.fileName, this.Author, this.Major, this.Minor, this.URL, this.Description, this.LoadExt, this.SaveExt, this.Layers);
            using (ViewSrc VSW = new ViewSrc("Full Source Code", sourceCode, true))
            {
                VSW.ShowDialog();
            }
        }

        private void solutionButton_Click(object sender, EventArgs e)
        {
            if (!this.isClassic)
            {
                FlexibleMessageBox.Show("Due to technical reasons, this feature is only available on classic installations of Paint.NET.", "Generate Visual Studio Solution", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            UpdateAllValues();

            using (FolderBrowserDialog fbd = new FolderBrowserDialog())
            {
                fbd.SelectedPath = Settings.LastSlnDirectory;
                fbd.ShowNewFolderButton = true;
                fbd.Description = "Choose a Folder to place the generated Visual Studio Solution.";

                if (fbd.ShowDialog() == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    string sourceCode = ScriptWriter.FullFileTypeSourceCode(this.userCode, this.fileName, this.Author, this.Major, this.Minor, this.URL, this.Description, this.LoadExt, this.SaveExt, this.Layers);
                    Solution.Generate(fbd.SelectedPath, this.fileName, sourceCode, string.Empty);

                    Settings.LastSlnDirectory = fbd.SelectedPath;
                }
            }
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void buildButton_Click(object sender, EventArgs e)
        {
            UpdateAllValues();
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void UpdateAllValues()
        {
            this.Author = this.authorBox.Text;
            this.URL = this.urlBox.Text;
            this.PluginName = this.nameBox.Text;
            this.Description = this.descriptionBox.Text;
            this.Major = (int)this.majorBox.Value;
            this.Minor = (int)this.minorBox.Value;
            this.LoadExt = "\"" + this.loadExtBox.Text.Split('|', ',', ';').Join("\", \"") + "\"";
            this.SaveExt = "\"" + this.saveExtBox.Text.Split('|', ',', ';').Join("\", \"") + "\"";
            this.Layers = this.layersCheckBox.Checked;
        }
    }
}
