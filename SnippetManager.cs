using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using ScintillaNET;

namespace PaintDotNet.Effects
{
    public partial class SnippetManager : UserControl
    {
        private string currentSnippet = string.Empty;
        private int currentIndex = -1;
        private bool loadData = true;
        private bool dirty = false;

        public SnippetManager()
        {
            InitializeComponent();

            this.SnippetList.ForeColor = PdnTheme.ForeColor;
            this.SnippetList.BackColor = PdnTheme.BackColor;
            this.SnippetName.ForeColor = PdnTheme.ForeColor;
            this.SnippetName.BackColor = PdnTheme.BackColor;
            this.toolStrip1.Renderer = PdnTheme.Renderer;

            this.SnippetList.Items.AddRange(Intelli.Snippets.Keys.ToArray());
            this.SnippetList.Height = this.SnippetBody.Height; // HiDPI Fix
            this.SnippetBody.Theme = PdnTheme.Theme;
            this.SnippetBody.LineNumbersEnabled = Settings.LineNumbers;
            this.SnippetBody.WrapMode = Settings.WordWrap ? WrapMode.Whitespace : WrapMode.None;

            if (this.SnippetList.Items.Count > 0)
            {
                this.SnippetList.SelectedIndex = 0;
            }
            else
            {
                this.SnippetName.Enabled = false;
                this.SnippetBody.Enabled = false;
                this.DeleteButton.Enabled = false;
                this.UpdateButton.Enabled = false;
            }
        }

        private void SnippetList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.SnippetList.SelectedIndex == -1)
            {
                this.SnippetName.Enabled = false;
                this.SnippetBody.Enabled = false;
                this.DeleteButton.Enabled = false;
                this.UpdateButton.Enabled = false;

                this.SnippetName.Text = string.Empty;
                this.SnippetBody.Text = string.Empty;
                this.dirty = false;

                return;
            }

            this.SnippetName.Enabled = true;
            this.SnippetBody.Enabled = true;
            this.DeleteButton.Enabled = true;
            this.UpdateButton.Enabled = true;

            if (this.dirty && (int)FlexibleMessageBox.Show($"There are unsaved Changes to {this.currentSnippet}. Discard?", "Unsaved Changes", new string[] { "Discard", "Return to Editor" }, MessageBoxIcon.Question) == 2)
            {
                this.loadData = false;
                this.dirty = false;
                this.SnippetList.SelectedIndex = this.currentIndex;
                this.dirty = true;
                return;
            }

            if (this.loadData)
            {
                string snippetName = this.SnippetList.SelectedItem.ToString();
                if (Intelli.Snippets.ContainsKey(snippetName))
                {
                    this.currentSnippet = snippetName;
                    this.SnippetName.Text = snippetName;
                    this.SnippetBody.Text = Intelli.Snippets[snippetName];
                    this.SnippetBody.ScrollCaret();
                    this.SnippetBody.Focus();
                    this.currentIndex = this.SnippetList.SelectedIndex;
                    this.dirty = false;
                }
            }
            else
            {
                this.loadData = true;
            }
        }

        private void AddNewButton_Click(object sender, EventArgs e)
        {
            int nextNum = this.SnippetList.Items.Count + 1;
            while (Intelli.Snippets.ContainsKey($"snippet{nextNum}"))
            {
                nextNum++;
            }

            string snippetName = $"snippet{nextNum}";
            Intelli.Snippets.Add(snippetName, "Enter your snippet here.");
            this.SnippetList.Items.Add(snippetName);
            SaveSnippets();
            int newIndex = this.SnippetList.Items.Count - 1;
            this.SnippetList.SelectedIndex = newIndex;
            if (this.SnippetList.SelectedIndex == newIndex)
            {
                this.SnippetName.Focus();
                this.SnippetName.SelectAll();
            }
        }

        private void UpdateButton_Click(object sender, EventArgs e)
        {
            if (this.SnippetList.SelectedIndex == -1)
            {
                return;
            }

            string snippetName = this.SnippetName.Text.Trim();
            if (!IsValidSnippetName(snippetName))
            {
                FlexibleMessageBox.Show("Invalid Snippet Name.", "Snippet Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string snippetBody = this.SnippetBody.Text;
            if (string.IsNullOrWhiteSpace(snippetBody))
            {
                FlexibleMessageBox.Show("Snippet Body is empty.", "Snippet Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            System.Diagnostics.Debug.Assert(Intelli.Snippets.ContainsKey(this.currentSnippet));

            if (snippetName != this.currentSnippet && Intelli.Snippets.ContainsKey(snippetName))
            {
                FlexibleMessageBox.Show($"There is already another Snippet with the name of {snippetName}.", "Snippet Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            this.dirty = false;

            Intelli.Snippets.Remove(this.currentSnippet);
            Intelli.Snippets.Add(snippetName, snippetBody);

            SaveSnippets();

            int itemIndex = this.SnippetList.SelectedIndex;
            this.SnippetList.Items.Insert(this.SnippetList.SelectedIndex, snippetName);
            this.SnippetList.Items.RemoveAt(this.SnippetList.SelectedIndex);
            this.SnippetList.SelectedIndex = itemIndex;
        }

        private void SaveSnippets()
        {
            Settings.Snippets = JsonSerializer.Serialize(Intelli.Snippets);
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            int itemIndex = this.SnippetList.SelectedIndex;
            if (itemIndex == -1)
            {
                return;
            }

            string snippetName = this.SnippetList.SelectedItem.ToString();
            if (!Intelli.Snippets.ContainsKey(snippetName))
            {
                return;
            }

            Intelli.Snippets.Remove(snippetName);

            SaveSnippets();

            this.dirty = false;
            this.SnippetList.Items.RemoveAt(this.SnippetList.SelectedIndex);
            if (this.SnippetList.Items.Count > 0)
            {
                if (itemIndex >= this.SnippetList.Items.Count)
                {
                    itemIndex = this.SnippetList.Items.Count - 1;
                }
                this.SnippetList.SelectedIndex = itemIndex;
            }
        }

        private void SnippetButton_TextChanged(object sender, EventArgs e)
        {
            this.dirty = true;
        }

        private void SnippetName_TextChanged(object sender, EventArgs e)
        {
            this.dirty = true;
            string newName = this.SnippetName.Text.Trim();
            bool error = (!IsValidSnippetName(newName) || (newName != this.currentSnippet && Intelli.Snippets.ContainsKey(newName)));
            this.SnippetName.ForeColor = (this.SnippetName.Enabled && error) ? Color.Black : PdnTheme.ForeColor;
            this.SnippetName.BackColor = (this.SnippetName.Enabled && error) ? Color.FromArgb(246, 97, 81) : PdnTheme.BackColor;
        }

        private void ImportFromFileButton_Click(object sender, EventArgs e)
        {
            string fileContents;

            using (
                OpenFileDialog ofd = new OpenFileDialog
                {
                    Title = "Import JSON File",
                    DefaultExt = ".json",
                    Filter = "JSON Files (*.JSON)|*.json",
                    Multiselect = false,
                })
            {
                if (ofd.ShowDialog() != DialogResult.OK)
                {
                    return;
                }

                try
                {
                    fileContents = File.ReadAllText(ofd.FileName);
                }
                catch
                {
                    return;
                }
            }

            ImportJson(fileContents);
        }

        private void ImportFromClipButton_Click(object sender, EventArgs e)
        {
            string clipboardContents;
            try
            {
                clipboardContents = System.Windows.Forms.Clipboard.GetText();
            }
            catch
            {
                return;
            }

            ImportJson(clipboardContents);
        }

        private void ExportToFileButton_Click(object sender, EventArgs e)
        {
            string json = ExportJson();
            if (json == null)
            {
                return;
            }

            using (
                SaveFileDialog sfd = new SaveFileDialog
                {
                    Title = "Export JSON File",
                    DefaultExt = ".json",
                    Filter = "JSON Files (*.JSON)|*.json",
                    OverwritePrompt = true,
                    AddExtension = true,
                    FileName = "CodeLabSnippets.json",
                })
            {
                if (sfd.ShowDialog() != DialogResult.OK)
                {
                    return;
                }

                try
                {
                    File.WriteAllText(sfd.FileName, json);
                }
                catch
                {
                }
            }
        }

        private void ExportToClipButton_Click(object sender, EventArgs e)
        {
            string json = ExportJson();
            if (json == null)
            {
                return;
            }

            try
            {
                System.Windows.Forms.Clipboard.SetText(json);
            }
            catch
            {
            }
        }

        private void ImportJson(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                FlexibleMessageBox.Show("No JSON data.", "Import Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Dictionary<string, string> importedSnippets;
            try
            {
                importedSnippets = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
            }
            catch
            {
                FlexibleMessageBox.Show("Invalid JSON data.", "Import Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (importedSnippets == null || importedSnippets.Count == 0)
            {
                FlexibleMessageBox.Show("No JSON data.", "Import Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int importCount = 0;
            foreach (KeyValuePair<string, string> kvp in importedSnippets)
            {
                if (IsValidSnippetName(kvp.Key) && !Intelli.Snippets.ContainsKey(kvp.Key))
                {
                    Intelli.Snippets.Add(kvp.Key, kvp.Value);
                    importCount++;
                }
            }

            if (importCount > 0)
            {
                SaveSnippets();
                this.SnippetList.Items.Clear();
                this.SnippetList.Items.AddRange(Intelli.Snippets.Keys.ToArray());
                this.SnippetList.SelectedIndex = this.SnippetList.Items.Count - 1;
            }

            FlexibleMessageBox.Show($"Imported {importCount} of {importedSnippets.Count} {(importedSnippets.Count == 1 ? "Snippet" : "Snippets")}.", "Import Report", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private static string ExportJson()
        {
            if (Intelli.Snippets.Count == 0)
            {
                FlexibleMessageBox.Show("No snippets to export.", "Export Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }

            string json = null;
            try
            {
                json = JsonSerializer.Serialize(Intelli.Snippets);
            }
            catch
            {
            }

            return FormatJson(json);
        }

        private static bool IsValidSnippetName(string value)
        {
            if (value.Length == 0)
            {
                return false;
            }

            int index = 0;

            if (value[0] == '#')
            {
                index++;
            }

            for (int i = index; i < value.Length; i++)
            {
                char c = value[i];
                if (!(char.IsLetterOrDigit(c) || c == '_'))
                {
                    return false;
                }
            }

            return true;
        }

        // Based on https://stackoverflow.com/a/6237866
        private static string FormatJson(string str)
        {
            str = Regex.Unescape(str).Replace("\r", "\\r").Replace("\n", "\\n");

            const string INDENT_STRING = "    ";
            int indent = 0;
            bool quoted = false;
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < str.Length; i++)
            {
                char ch = str[i];
                switch (ch)
                {
                    case '{':
                    case '[':
                        sb.Append(ch);
                        if (!quoted)
                        {
                            sb.AppendLine();
                            Enumerable.Range(0, ++indent).ForEach(_ => sb.Append(INDENT_STRING));
                        }
                        break;
                    case '}':
                    case ']':
                        if (!quoted)
                        {
                            sb.AppendLine();
                            Enumerable.Range(0, --indent).ForEach(_ => sb.Append(INDENT_STRING));
                        }
                        sb.Append(ch);
                        break;
                    case '"':
                        sb.Append(ch);
                        bool escaped = false;
                        int index = i;
                        while (index > 0 && str[--index] == '\\')
                        {
                            escaped = !escaped;
                        }

                        if (!escaped)
                        {
                            quoted = !quoted;
                        }

                        break;
                    case ',':
                        sb.Append(ch);
                        if (!quoted)
                        {
                            sb.AppendLine();
                            Enumerable.Range(0, indent).ForEach(_ => sb.Append(INDENT_STRING));
                        }
                        break;
                    case ':':
                        sb.Append(ch);
                        if (!quoted)
                        {
                            sb.Append(" ");
                        }

                        break;
                    default:
                        sb.Append(ch);
                        break;
                }
            }
            return sb.ToString();
        }
    }
}
