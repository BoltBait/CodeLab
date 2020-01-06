using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
            this.userCode = scriptText;
            this.isClassic = isClassic;
            DecimalSymbol.Text = System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator;

            this.Text = $"Building {this.fileName}.DLL";

            // Preload menu name
            Match mmn = Regex.Match(scriptText, @"//[\s-[\r\n]]*Name[\s-[\r\n]]*:[\s-[\r\n]]*(?<menulabel>.*)(?=\r?\n|$)", RegexOptions.IgnoreCase);
            if (mmn.Success)
            {
                string menuName = mmn.Groups["menulabel"].Value.Trim();
                nameBox.Text = (menuName.Length > 0) ? menuName : this.fileName;
            }
            else
            {
                this.nameBox.Text = this.fileName;
            }

            // Preload version checking for period
            Match vsn = Regex.Match(scriptText, @"//[\s-[\r\n]]*Version[\s-[\r\n]]*:[\s-[\r\n]]*(?<majorversionlabel>\d+)\.(?<minorversionlabel>\d+)(?=\r?\n|$)", RegexOptions.IgnoreCase);
            if (!vsn.Success)
            {
                // Preload version checking for comma
                vsn = Regex.Match(scriptText, @"//[\s-[\r\n]]*Version[\s-[\r\n]]*:[\s-[\r\n]]*(?<majorversionlabel>\d+)\,(?<minorversionlabel>\d+)(?=\r?\n|$)", RegexOptions.IgnoreCase);
            }
            if (vsn.Success)
            {
                if (decimal.TryParse(vsn.Groups["majorversionlabel"].Value.Trim(), out decimal majorv))
                {
                    this.majorBox.Value = majorv.Clamp(this.majorBox.Minimum, this.majorBox.Maximum);
                }
                if (decimal.TryParse(vsn.Groups["minorversionlabel"].Value.Trim(), out decimal minorv))
                {
                    this.minorBox.Value = minorv.Clamp(this.majorBox.Minimum, this.majorBox.Maximum);
                }
            }

            // Preload author's name
            Match mau = Regex.Match(scriptText, @"//[\s-[\r\n]]*Author[\s-[\r\n]]*:[\s-[\r\n]]*(?<authorlabel>.*)(?=\r?\n|$)", RegexOptions.IgnoreCase);
            if (mau.Success)
            {
                this.authorBox.Text = mau.Groups["authorlabel"].Value.Trim();
            }

            // Preload Description
            Match mds = Regex.Match(scriptText, @"//[\s-[\r\n]]*Desc[\s-[\r\n]]*:[\s-[\r\n]]*(?<desclabel>.*)(?=\r?\n|$)", RegexOptions.IgnoreCase);
            if (mds.Success && mds.Groups["desclabel"].Value.Trim() != "")
            {
                this.descriptionBox.Text = mds.Groups["desclabel"].Value.Trim();
            }
            else
            {
                this.descriptionBox.Text = this.fileName + " FileType";
            }

            // Preload Support URL
            Match msu = Regex.Match(scriptText, @"//[\s-[\r\n]]*URL[\s-[\r\n]]*:[\s-[\r\n]]*(?<urllabel>.*)(?=\r?\n|$)", RegexOptions.IgnoreCase);
            if (msu.Success)
            {
                this.urlBox.Text = msu.Groups["urllabel"].Value.Trim();
            }

            // Preload Load Extensions
            Match mle = Regex.Match(scriptText, @"//[\s-[\r\n]]*LoadExtns[\s-[\r\n]]*:[\s-[\r\n]]*(?<loadlabel>.*)(?=\r?\n|$)", RegexOptions.IgnoreCase);
            if (mle.Success)
            {
                this.loadExtBox.Text = mle.Groups["loadlabel"].Value.Trim();
            }

            // Preload Save Extensions
            Match mse = Regex.Match(scriptText, @"//[\s-[\r\n]]*SaveExtns[\s-[\r\n]]*:[\s-[\r\n]]*(?<savelabel>.*)(?=\r?\n|$)", RegexOptions.IgnoreCase);
            if (mse.Success)
            {
                this.saveExtBox.Text = mse.Groups["savelabel"].Value.Trim();
            }

            // Preload Supports Layers
            Match msl = Regex.Match(scriptText, @"//[\s-[\r\n]]*Flattened[\s-[\r\n]]*:[\s-[\r\n]]*(?<flattenlabel>.*)(?=\r?\n|$)", RegexOptions.IgnoreCase);
            if (msl.Success)
            {
                this.layersCheckBox.Checked = msl.Groups["flattenlabel"].Value.Trim().Equals("true", StringComparison.OrdinalIgnoreCase);
            }
        }

        private void sourceButton_Click(object sender, EventArgs e)
        {
            UpdateAllValues();

            string sourceCode = ScriptWriter.FullFileTypeSourceCode(this.userCode, this.fileName, this.Author, this.Major, this.Minor, this.URL, this.Description, this.LoadExt, this.SaveExt, this.Layers, this.PluginName);
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
                    string sourceCode = ScriptWriter.FullFileTypeSourceCode(this.userCode, this.fileName, this.Author, this.Major, this.Minor, this.URL, this.Description, this.LoadExt, this.SaveExt, this.Layers, this.PluginName);
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
            this.LoadExt = "\"" + this.loadExtBox.Text.Split('|', ',', ';').Select(ext => ext.Trim()).Join("\", \"") + "\"";
            this.SaveExt = "\"" + this.saveExtBox.Text.Split('|', ',', ';').Select(ext => ext.Trim()).Join("\", \"") + "\"";
            this.Layers = this.layersCheckBox.Checked;
        }
    }
}
