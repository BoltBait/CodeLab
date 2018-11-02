/////////////////////////////////////////////////////////////////////////////////
// CodeLab for Paint.NET
// Copyright ©2006 Rick Brewster, Tom Jackson. All Rights Reserved.
// Portions Copyright ©2007-2018 BoltBait. All Rights Reserved.
// Portions Copyright ©2016-2018 Jason Wendt. All Rights Reserved.
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

using PaintDotNet.AppModel;
using ScintillaNET;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PaintDotNet.Effects
{
    internal partial class CodeLabConfigDialog : EffectConfigDialog
    {
        private const string ThisVersion = "4.1"; // Remember to change it in CodeLab.cs too!
        private const string WebUpdateFile = "http://www.boltbait.com/versions.txt"; // The web site to check for updates
        private const string ThisApplication = "1"; // in the WebUpadteFile, CodeLab is application #1
        // format of the versions.txt file:  application number;current version;URL to download current version
        // for example: 1;2.13;http://boltbait.com/pdn/CodeLab/CodeLab213.zip
        // each application on its own line
        #region Constructor
        private const string WindowTitle = "CodeLab v" + ThisVersion;
        private string FileName = "Untitled";
        private string FullScriptPath = "";
        private bool CheckForUpdates;
        private string UpdateURL = "";
        private string UpdateVER = "";
        private bool preview = false;
        private EffectConfigToken previewToken = null;
        private Color OriginalForeColor;
        private Color OriginalBackColor;
        private readonly string locale = CultureInfo.CurrentUICulture.Name.ToLowerInvariant();

        public CodeLabConfigDialog()
        {
            InitializeComponent();

            #region Load Settings from registry
            if (Settings.WordWrap)
            {
                txtCode.WrapMode = WrapMode.Whitespace;
                wordWrapToolStripMenuItem.CheckState = CheckState.Checked;
            }
            if (Settings.WhiteSpace)
            {
                txtCode.ViewWhitespace = WhitespaceMode.VisibleAlways;
                whiteSpaceToolStripMenuItem.CheckState = CheckState.Checked;
                txtCode.WrapVisualFlags = WrapVisualFlags.Start;
            }
            if (Settings.CodeFolding)
            {
                txtCode.CodeFoldingEnabled = true;
                codeFoldingToolStripMenuItem.CheckState = CheckState.Checked;
            }
            if (Settings.LineNumbers)
            {
                txtCode.LineNumbersEnabled = true;
                lineNumbersToolStripMenuItem.CheckState = CheckState.Checked;
            }
            if (Settings.Bookmarks)
            {
                txtCode.BookmarksEnabled = true;
                bookmarksToolStripMenuItem.CheckState = CheckState.Checked;
            }
            if (Settings.ToolBar)
            {
                toolBarToolStripMenuItem.CheckState = CheckState.Checked;
                toolStrip1.Visible = true;
                txtCode.Location = new Point(txtCode.Left, tabStrip1.Bottom);
            }
            else
            {
                toolBarToolStripMenuItem.CheckState = CheckState.Unchecked;
                toolStrip1.Visible = false;
                txtCode.Location = new Point(txtCode.Left, tabStrip1.Top);
            }
            if (Settings.ErrorBox)
            {
                viewCheckBoxes(true, false);
            }
            else if (Settings.Output)
            {
                viewCheckBoxes(false, true);
            }
            else
            {
                viewCheckBoxes(false, false);
            }
            OriginalForeColor = this.ForeColor;
            OriginalBackColor = this.BackColor;
            if (Settings.EditorTheme == Theme.Auto)
            {
                autoToolStripMenuItem.CheckState = CheckState.Checked;
                darkToolStripMenuItem.CheckState = CheckState.Unchecked;
                lightToolStripMenuItem.CheckState = CheckState.Unchecked;
            }
            else if (Settings.EditorTheme == Theme.Dark)
            {
                this.ForeColor = Color.White;
                this.BackColor = Color.FromArgb(40, 40, 40);
                txtCode.Theme = Theme.Dark;
                autoToolStripMenuItem.CheckState = CheckState.Unchecked;
                darkToolStripMenuItem.CheckState = CheckState.Checked;
                lightToolStripMenuItem.CheckState = CheckState.Unchecked;
            }
            else if (Settings.EditorTheme == Theme.Light)
            {
                this.ForeColor = Color.Black;
                this.BackColor = Color.White;
                txtCode.Theme = Theme.Light;
                autoToolStripMenuItem.CheckState = CheckState.Unchecked;
                darkToolStripMenuItem.CheckState = CheckState.Unchecked;
                lightToolStripMenuItem.CheckState = CheckState.Checked;
            }
            if (Settings.LargeFonts)
            {
                txtCode.Zoom = 2;
                largeFontToolStripMenuItem.CheckState = CheckState.Checked;
            }
            if (Settings.Map)
            {
                txtCode.MapEnabled = true;
                indicatorMapMenuItem.CheckState = CheckState.Checked;
            }
            if (Settings.CheckForUpdates)
            {
                CheckForUpdates = true;
                checkForUpdatesToolStripMenuItem.CheckState = CheckState.Checked;
                GoCheckForUpdates(true, false);
            }
            else
            {
                checkForUpdatesToolStripMenuItem.CheckState = CheckState.Unchecked;
            }
            string editorFont = Settings.FontFamily;
            if (!IsFontInstalled(editorFont))
            {
                editorFont = "Courier New";
            }
            if (!IsFontInstalled(editorFont))
            {
                editorFont = "Verdana";
            }
            fontsCourierMenuItem.CheckState = ("Courier New" == editorFont) ? CheckState.Checked : CheckState.Unchecked;
            fontsConsolasMenuItem.CheckState = ("Consolas" == editorFont) ? CheckState.Checked : CheckState.Unchecked;
            fontsEnvyRMenuItem.CheckState = ("Envy Code R" == editorFont) ? CheckState.Checked : CheckState.Unchecked;
            fontsHackMenuItem.CheckState = ("Hack" == editorFont) ? CheckState.Checked : CheckState.Unchecked;
            fontsVerdanaMenuItem.CheckState = ("Verdana" == editorFont) ? CheckState.Checked : CheckState.Unchecked;
            txtCode.Styles[Style.Default].Font = editorFont;
            OutputTextBox.Font = new Font(editorFont, OutputTextBox.Font.Size);
            errorList.Font = new Font(editorFont, errorList.Font.Size);
            #endregion

            this.Opacity = 1.00;
            opacity50MenuItem.Checked = false;
            opacity75MenuItem.Checked = false;
            opacity90MenuItem.Checked = false;
            opacity100MenuItem.Checked = true;

            // Disable menu items if they'll have no effect
            transparencyToolStripMenuItem.Enabled = EnableOpacity;
            fontsCourierMenuItem.Enabled = IsFontInstalled("Courier New");
            fontsConsolasMenuItem.Enabled = IsFontInstalled("Consolas");
            fontsEnvyRMenuItem.Enabled = IsFontInstalled("Envy Code R");
            fontsHackMenuItem.Enabled = IsFontInstalled("Hack");

            if (fontsCourierMenuItem.Enabled) fontsCourierMenuItem.Font = new Font("Courier New", fontsCourierMenuItem.Font.Size);
            if (fontsConsolasMenuItem.Enabled) fontsConsolasMenuItem.Font = new Font("Consolas", fontsConsolasMenuItem.Font.Size);
            if (fontsEnvyRMenuItem.Enabled) fontsEnvyRMenuItem.Font = new Font("Envy Code R", fontsEnvyRMenuItem.Font.Size);
            if (fontsHackMenuItem.Enabled) fontsHackMenuItem.Font = new Font("Hack", fontsHackMenuItem.Font.Size);
            if (fontsVerdanaMenuItem.Enabled) fontsVerdanaMenuItem.Font = new Font("Verdana", fontsVerdanaMenuItem.Font.Size);

            // PDN Theme
            ApplyTheme();
            txtCode.Theme = (PdnTheme.BackColor.R < 128 && PdnTheme.BackColor.G < 128 && PdnTheme.BackColor.B < 128) ? Theme.Dark : Theme.Light;

            ResetScript();
            Build();
            txtCode.Focus();
        }
        #endregion

        #region Token functions
        protected override void InitTokenFromDialog()
        {
            CodeLabConfigToken sect = (CodeLabConfigToken)theEffectToken;
            sect.UserCode = txtCode.Text;
            sect.UserScriptObject = ScriptBuilder.UserScriptObject;
            sect.ScriptName = FileName;
            sect.ScriptPath = FullScriptPath;
            sect.Dirty = tabStrip1.SelectedTabIsDirty;
            sect.Preview = preview;
            sect.PreviewToken = previewToken;
            sect.Bookmarks = txtCode.Bookmarks;
        }

        protected override void InitDialogFromToken(EffectConfigToken effectTokenCopy)
        {
            CodeLabConfigToken sect = (CodeLabConfigToken)effectTokenCopy;

            if (sect != null)
            {
                FileName = sect.ScriptName;
                FullScriptPath = sect.ScriptPath;
                txtCode.Text = sect.UserCode;
                txtCode.ExecuteCmd(Command.ScrollToEnd); // Workaround for a scintilla bug
                txtCode.ExecuteCmd(Command.ScrollToStart);
                txtCode.EmptyUndoBuffer();
                if (!sect.Dirty)
                {
                    txtCode.SetSavePoint();
                }
                txtCode.Bookmarks = sect.Bookmarks;

                UpdateTabProperties();
            }
        }

        protected override void InitialInitToken()
        {
            theEffectToken = new CodeLabConfigToken
            {
                UserCode = ScriptWriter.DefaultCode,
                UserScriptObject = null,
                ScriptName = "Untitled",
                ScriptPath = "",
                Dirty = false,
                Preview = false,
                PreviewToken = null,
                Bookmarks = Array.Empty<int>()
            };
        }
        #endregion

        #region Build Script actions
        private void Build()
        {
            ScriptBuilder.Build(txtCode.Text, OutputTextBox.Visible);

            DisplayErrors();

            txtCode.UpdateSyntaxHighlighting();

            FinishTokenUpdate();
        }

        private async Task BuildAsync()
        {
            tmrCompile.Enabled = false;
            string code = txtCode.Text;
            bool debug = OutputTextBox.Visible;
            await Task.Run(() => ScriptBuilder.Build(code, debug));

            if (this.IsDisposed)
            {
                return;
            }

            DisplayErrors();

            txtCode.UpdateSyntaxHighlighting();

            FinishTokenUpdate();
            tmrCompile.Enabled = true;
        }

        private void RunWithDialog()
        {
            tmrCompile.Enabled = false;
            Build();
            if (errorList.Items.Count != 0)
            {
                MessageBox.Show("Before you can preview your effect, you must resolve all code errors.", "Build Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                if (!ScriptBuilder.BuildFullPreview(txtCode.Text))
                {
                    MessageBox.Show("Something went wrong, and the Preview can't be run.", "Preview Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else if (!ScriptBuilder.UserScriptObject.Options.Flags.HasFlag(EffectFlags.Configurable))
                {
                    MessageBox.Show("There are no UI controls, so the Preview can't be displayed.", "Preview Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    ScriptBuilder.UserScriptObject.EnvironmentParameters = this.Effect.EnvironmentParameters;
                    using (EffectConfigDialog previewDialog = ScriptBuilder.UserScriptObject.CreateConfigDialog())
                    {
                        preview = true;
                        previewToken = previewDialog.EffectToken;
                        previewDialog.EffectTokenChanged += (sender, e) => FinishTokenUpdate();

                        previewDialog.ShowDialog();
                    }

                    preview = false;
                    previewToken = null;
                    Build();
                }
            }
            tmrCompile.Enabled = true;
        }

        private void DisplayErrors()
        {
            errorList.Items.Clear();
            toolTips.SetToolTip(errorList, "");

            txtCode.ClearErrors();

            ShowErrors.Text = "Show Errors List";
            ShowErrors.ForeColor = this.ForeColor;

            if (ScriptBuilder.Errors.Count == 0)
            {
                txtCode.UpdateIndicatorBar();
                return;
            }

            foreach (ScriptError err in ScriptBuilder.Errors)
            {
                errorList.Items.Add(err);

                if (err.Line > 0)
                {
                    txtCode.AddError(err.Line - 1, err.Column);
                }
            }

            txtCode.UpdateIndicatorBar();

            ShowErrors.Text = $"Show Errors List ({errorList.Items.Count})";
            ShowErrors.ForeColor = Color.Red;
        }

        private void ResetScript()
        {
            InitialInitToken();
            InitDialogFromToken();
            FinishTokenUpdate();
        }

        private DialogResult PromptToSave()
        {
            DialogResult dr = MessageBox.Show(this, $"Do you want to save changes to '{FileName}'?", "Script Editor", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
            switch (dr)
            {
                case DialogResult.Yes:
                    if (!SaveScript())
                    {
                        txtCode.Focus();
                        return DialogResult.Cancel;
                    }
                    txtCode.SetSavePoint();
                    return DialogResult.None;
                case DialogResult.No:
                    return DialogResult.None;
                case DialogResult.Cancel:
                    txtCode.Focus();
                    return DialogResult.Cancel;
            }
            return DialogResult.None;
        }

        private bool SaveScript()
        {
            if (FullScriptPath.IsNullOrEmpty() || !File.Exists(FullScriptPath))
            {
                return SaveAsScript();
            }

            bool saved = false;
            try
            {
                File.WriteAllText(FullScriptPath, txtCode.Text);
                saved = true;
            }
            catch
            {
            }

            return saved;
        }

        private bool SaveAsScript()
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Title = "Save User Script";
            sfd.DefaultExt = ".cs";
            sfd.Filter = "C# Code Files (*.CS)|*.cs";
            sfd.OverwritePrompt = true;
            sfd.AddExtension = true;
            sfd.FileName = (FileName == "Untitled") ? "MyScript.cs" : FileName + ".cs";
            sfd.InitialDirectory = Settings.LastSourceDirectory;
            sfd.FileOk += (object sender, System.ComponentModel.CancelEventArgs e) =>
            {
                if (!char.IsLetter(Path.GetFileName(sfd.FileName), 0))
                {
                    e.Cancel = true;
                    MessageBox.Show("The filename must begin with a letter.", "Save User Script", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };

            bool saved = false;
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    File.WriteAllText(sfd.FileName, txtCode.Text);
                    FullScriptPath = sfd.FileName;
                    Settings.LastSourceDirectory = Path.GetDirectoryName(sfd.FileName);
                    FileName = Path.GetFileNameWithoutExtension(sfd.FileName);
                    UpdateTabProperties();
                    AddToRecents(sfd.FileName);
                    saved = true;
                }
                catch
                {
                }
            }

            sfd.Dispose();
            return saved;
        }

        private bool LoadScript()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Load User Script";
            ofd.DefaultExt = ".cs";
            ofd.Filter = "C# Code Files (*.CS)|*.cs";
            ofd.DefaultExt = ".cs";
            ofd.Multiselect = false;
            ofd.InitialDirectory = Settings.LastSourceDirectory;

            bool loaded = false;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                AddToRecents(ofd.FileName);

                FullScriptPath = ofd.FileName;
                FileName = Path.GetFileNameWithoutExtension(ofd.FileName);

                if (tabStrip1.SelectedTabGuid == Guid.Empty && txtCode.IsVirgin)
                {
                    UpdateTabProperties();
                }
                else
                {
                    tabStrip1.NewTab(FileName, FullScriptPath);
                }

                try
                {
                    Settings.LastSourceDirectory = Path.GetDirectoryName(ofd.FileName);
                    txtCode.Text = File.ReadAllText(ofd.FileName);
                    txtCode.ExecuteCmd(Command.ScrollToEnd); // Workaround for a scintilla bug
                    txtCode.ExecuteCmd(Command.ScrollToStart);
                    txtCode.EmptyUndoBuffer();
                    loaded = true;
                }
                catch
                {
                }
            }

            ofd.Dispose();
            return loaded;
        }

        private void txtCode_BuildNeeded(object sender, EventArgs e)
        {
            Build();
        }
        #endregion

        #region Error listbox functions
        private void errorListMenu_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (errorList.SelectedIndex < 0)
            {
                e.Cancel = true;
                return;
            }

            ErrorCodeMenuItem.Visible = (errorList.SelectedItem is ScriptError error && error.ErrorNumber != string.Empty);
        }

        private void CopyErrorMenuItem_Click(object sender, EventArgs e)
        {
            if (errorList.SelectedIndex > -1)
            {
                string errorMsg = (errorList.SelectedItem is ScriptError error) ? error.ErrorText : errorList.SelectedItem.ToString();
                if (!errorMsg.IsNullOrEmpty())
                {
                    Clipboard.SetText(errorMsg);
                }
            }
        }

        private void FullErrorMenuItem_Click(object sender, EventArgs e)
        {
            if (errorList.SelectedIndex > -1)
            {
                using (ViewSrc VSW = new ViewSrc("Full Error Message", errorList.SelectedItem.ToString(), false))
                {
                    VSW.ShowDialog();
                }
            }
        }

        private void ErrorCodeMenuItem_Click(object sender, EventArgs e)
        {
            if (errorList.SelectedIndex > -1 && errorList.SelectedItem is ScriptError error)
            {
                Services.GetService<IShellService>().LaunchUrl(null, $"https://docs.microsoft.com/{locale}/dotnet/csharp/language-reference/compiler-messages/{error.ErrorNumber}");
            }
        }

        private void errorList_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                errorList.SelectedIndex = errorList.IndexFromPoint(e.Location);
            }
        }

        private void listErrors_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (errorList.SelectedIndex >= 0)
            {
                ScrollToError();
                using (ViewSrc VSW = new ViewSrc("Full Error Message", errorList.SelectedItem.ToString(), false))
                {
                    VSW.ShowDialog();
                }
            }
        }

        private void listErrors_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (errorList.SelectedIndex >= 0)
            {
                ScrollToError();
            }
        }

        private void ScrollToError()
        {
            if (errorList.SelectedIndex >= 0 && errorList.SelectedItem is ScriptError errw && errw.Line > 0)
            {
                txtCode.SetEmptySelection(txtCode.Lines[errw.Line - 1].Position + errw.Column);
                txtCode.Lines[errw.Line - 1].EnsureVisible();
                txtCode.ScrollCaret();    // Make error visible by scrolling to it
                txtCode.Focus();
            }
            toolTips.SetToolTip(errorList, errorList.SelectedItem.ToString().InsertLineBreaks(100));
        }
        #endregion

        #region Timer tick Event functions
        private void tmrExceptionCheck_Tick(object sender, EventArgs e)
        {
            CodeLabConfigToken sect = (CodeLabConfigToken)theEffectToken;

            if (sect.LastExceptions.Count > 0)
            {
                string exc = sect.LastExceptions[0].ToString();
                sect.LastExceptions.Clear();

                string numString = exc.Substring(exc.IndexOf(".0.cs:line ", StringComparison.Ordinal) + 11, 4).Trim();
                if (int.TryParse(numString, out int lineNum))
                {
                    lineNum -= ScriptBuilder.LineOffset;
                }

                errorList.Items.Add($"Unhandled Exception at line {lineNum}: \r\n{exc}");
                ShowErrors.Text = $"Show Errors List ({errorList.Items.Count})";
                ShowErrors.ForeColor = Color.Red;
            }

            if (sect.Output.Count > 0)
            {
                string output = sect.Output[0];
                sect.Output.Clear();

                if (output.Trim() != string.Empty)
                {
                    OutputTextBox.AppendText(output);
                }
            }
        }

        private void tmrCompile_Tick(object sender, EventArgs e)
        {
            tmrCompile.Enabled = false;
            DisplayUpdates(true);
            UpdateToolBarButtons();
            Build();
        }
        #endregion

        #region Dialog functions - Load Icons, keyboard events
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            tmrCompile.Enabled = false;
            tmrExceptionCheck.Enabled = false;

            if (tabStrip1.AnyDirtyTabs)
            {
                e.Cancel = MessageBox.Show("Tabs marked with \"*\" have unsaved changes. These changes will be lost if CodeLab is closed.\r\n\r\nWould you like to save these files?", "Unsaved changes", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes;
            }

            base.OnFormClosing(e);
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case (Keys.Control | Keys.Tab):
                    tabStrip1.NextTab();
                    return true;
                case (Keys.Control | Keys.Shift | Keys.Tab):
                    tabStrip1.PrevTab();
                    return true;
                case (Keys.Control | Keys.W):
                    tabStrip1.CloseTab();
                    return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void txtCode_KeyUp(object sender, KeyEventArgs e)
        {
            if (
                (e.KeyCode == Keys.Delete) ||
                (e.Control && (e.KeyCode == Keys.V)) ||
                (e.Shift && (e.KeyCode == Keys.Insert))
               )
            {
                // Reset idle timer
                tmrCompile.Enabled = false;
                tmrCompile.Enabled = true;
            }
            if (e.KeyCode == Keys.F1)
            {
                helpTopicsToolStripMenuItem_Click(sender, EventArgs.Empty);
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
            base.OnKeyUp(e);
            UpdateToolBarButtons();
        }

        private void txtCode_KeyPress(object sender, KeyPressEventArgs e)
        {
            base.OnKeyPress(e);
            // Reset idle timer
            tmrCompile.Enabled = false;
            tmrCompile.Enabled = true;
            UpdateToolBarButtons();
        }

        private void btnBuild_Click(object sender, EventArgs e)
        {
            Build();
            txtCode.Focus();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            tmrCompile.Enabled = false;
            Build();
            tmrExceptionCheck.Enabled = false;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            tmrExceptionCheck.Enabled = false;
            tmrCompile.Enabled = false;
        }

        private static bool IsFontInstalled(string fontName)
        {
            using (Font font = new Font(fontName, 12f))
            {
                return font.Name == fontName;
            }
        }

        private void ApplyTheme()
        {
            PdnTheme.ForeColor = this.ForeColor;
            PdnTheme.BackColor = this.BackColor;
            toolStrip1.Renderer = PdnTheme.Renderer;
            tabStrip1.Renderer = PdnTheme.TabRenderer;
            menuStrip1.Renderer = PdnTheme.Renderer;
            contextMenuStrip1.Renderer = PdnTheme.Renderer;
            errorList.ForeColor = PdnTheme.ForeColor;
            errorList.BackColor = PdnTheme.BackColor;
            OutputTextBox.ForeColor = PdnTheme.ForeColor;
            OutputTextBox.BackColor = PdnTheme.BackColor;
            ShowErrors.ForeColor = (ShowErrors.Text == "Show Errors List") ? this.ForeColor : Color.Red;
        }
        #endregion

        #region Freshness Check functions
        private void DisplayUpdates(bool silentMode)
        {
            if (UpdateURL != "")
            {
                if (txtCode.Focused) // only popup if code editor has focus (otherwise, we might be doing something that we shouldn't interrupt)
                {
                    if (MessageBox.Show("An update to CodeLab is available.\n\nWould you like to download CodeLab v" + UpdateVER + "?\n\n(This will not close your current CodeLab session.)", "CodeLab Updater", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
                    {
                        Services.GetService<IShellService>().LaunchUrl(null, UpdateURL);
                    }
                    else
                    {
                        UpdateURL = "";
                    }
                }
            }
            else if (!silentMode)
            {
                if (UpdateVER == ThisVersion)
                {
                    MessageBox.Show("You are up-to-date!", "CodeLab Updater", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("I'm not sure if you are up-to-date.\n\nI was not able to reach the update website.\n\nTry again later.", "CodeLab Updater", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void GoCheckForUpdates(bool silentMode, bool force)
        {
            UpdateVER = "";
            UpdateURL = "";

            if (WebUpdateFile == "") return;

            if (!force)
            {
                // only check for updates every 7 days
                if (Math.Abs((Settings.LatestUpdateCheck - DateTime.Today).TotalDays) < 7)
                {
                    return; // not time yet
                }
            }

            Random r = new Random(); // defeat any cache by appending a random number to the URL

            WebClient web = new WebClient();
            web.OpenReadAsync(new Uri(WebUpdateFile + "?r=" + r.Next(int.MaxValue).ToString()));

            web.OpenReadCompleted += (sender, e) =>
            {
                try
                {
                    string text = "";
                    Stream stream = e.Result;
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        text = reader.ReadToEnd();
                    }
                    string[] lines = text.Split('\n');
                    for (int i = 0; i < lines.Length; i++)
                    {
                        string[] data = lines[i].Split(';');
                        if (data.Length >= 2)
                        {
                            if (data[0].Trim() == ThisApplication.Trim())
                            {
                                UpdateVER = data[1].Trim();
                                if (data[1].Trim() != ThisVersion.Trim())
                                {
                                    UpdateURL = data[2].Trim();
                                }
                            }
                        }
                    }
                }
                catch
                {
                    UpdateVER = "";
                    UpdateURL = "";
                }

                Settings.LatestUpdateCheck = DateTime.Now;

                DisplayUpdates(silentMode);
            };
        }
        #endregion

        #region Common functions for button/menu events
        private void CreateNewFile()
        {
            FileNew fn = new FileNew();
            if (fn.ShowDialog() == DialogResult.OK)
            {
                FileName = "Untitled";
                FullScriptPath = "";

                if (tabStrip1.SelectedTabGuid == Guid.Empty && txtCode.IsVirgin)
                {
                    UpdateTabProperties();
                }
                else
                {
                    tabStrip1.NewTab(FileName, FullScriptPath);
                }

                txtCode.Text = fn.CodeTemplate;
                txtCode.ExecuteCmd(Command.ScrollToEnd); // Workaround for a scintilla bug
                txtCode.ExecuteCmd(Command.ScrollToStart);
                txtCode.EmptyUndoBuffer();
                Build();
            }
            fn.Dispose();

            txtCode.Focus();
        }

        private void OpenFile()
        {
            LoadScript();
            txtCode.SetSavePoint();
            txtCode.Focus();
            Build();
        }

        private void Save()
        {
            if (SaveScript())
            {
                txtCode.SetSavePoint();
            }
            txtCode.Focus();
        }

        private void SaveAs()
        {
            if (SaveAsScript())
            {
                txtCode.SetSavePoint();
            }
            txtCode.Focus();
        }

        private void SaveAsDLL()
        {
            Build();
            if (errorList.Items.Count != 0)
            {
                MessageBox.Show("Before you can build a DLL, you must resolve all code errors.", "Build Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (FileName == "Untitled" || FileName == "")
            {
                MessageBox.Show("Before you can build a DLL, you must first save your source file using the File > Save as... menu.", "Build Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                string fullPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                fullPath = Path.Combine(fullPath, FileName);
                fullPath = Path.ChangeExtension(fullPath, ".dll");
                // Let the user pick the submenu, menu name, and icon
                BuildForm myBuildForm = new BuildForm(FileName.Trim(), txtCode.Text, FullScriptPath);
                if (myBuildForm.ShowDialog() == DialogResult.OK)
                {
                    // Everything is OK, BUILD IT!
                    if (ScriptBuilder.BuildDll(txtCode.Text, FullScriptPath, myBuildForm.SubMenuStr, myBuildForm.MenuStr, myBuildForm.IconPathStr, myBuildForm.Author, myBuildForm.MajorVer, myBuildForm.MinorVer, myBuildForm.Support, myBuildForm.WindowTitleTextStr, myBuildForm.isAdjustment, myBuildForm.Description, myBuildForm.KeyWords, myBuildForm.ForceAliasSelection, myBuildForm.ForceSingleThreaded, myBuildForm.HelpType, myBuildForm.HelpStr))
                    {
                        string zipPath = Path.ChangeExtension(fullPath, ".zip");
                        MessageBox.Show("Build succeeded!\r\n\r\nFile \"" + zipPath.Trim() + "\" created.\r\n\r\nYou will need to right-click 'Extract All...' the file on your desktop to run the install.bat file and restart Paint.NET to see it in the Effects menu.", "Build Finished", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        DisplayErrors();
                        MessageBox.Show("I'm sorry, I was not able to build the DLL.\r\n\r\nPerhaps the file already exists and is marked 'read only' or is in use by Paint.NET.  There may be other build errors listed in the box below.", "Build Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void UIDesigner()
        {
            txtCode.ReNumberUIVariables();
            // User Interface Designer
            UIBuilder myUIBuilderForm = new UIBuilder(txtCode.Text, ColorBgra.Black);  // This should be the current Primary color
            if (myUIBuilderForm.ShowDialog() == DialogResult.OK)
            {
                // update generated code
                txtCode.BeginUndoAction();
                if (txtCode.Text.Contains("#region UICode"))
                {
                    int uiRegPos = txtCode.Text.IndexOf("#region UICode", StringComparison.Ordinal);
                    int startLine = txtCode.LineFromPosition(uiRegPos) + 1;
                    int startPos = txtCode.Lines[startLine].Position;
                    int endPos = txtCode.Text.IndexOf("#endregion", StringComparison.Ordinal);
                    txtCode.SetTargetRange(startPos, endPos);
                    txtCode.ReplaceTarget(myUIBuilderForm.UIControlsText);
                }
                else
                {
                    for (int i = 0; i < txtCode.Lines.Count; i++)
                    {
                        string lineText = txtCode.Lines[i].Text;
                        if (lineText.Contains("int Amount1") || lineText.Contains("int Amount2") || lineText.Contains("int Amount3"))
                        {
                            int startPos = txtCode.Lines[i].Position;
                            int length = txtCode.Lines[i].EndPosition - startPos;
                            txtCode.DeleteRange(startPos, length);
                            i--;
                        }
                    }
                    txtCode.InsertText(0, "#region UICode\r\n" + myUIBuilderForm.UIControlsText + "#endregion\r\n");
                }
                txtCode.EndUndoAction();
            }
            Build();
        }

        private void CutSelection()
        {
            txtCode.Cut();
            tmrCompile.Enabled = false;
            tmrCompile.Enabled = true;
        }

        private void CopySelection()
        {
            txtCode.Copy();
        }

        private void PasteSelection()
        {
            txtCode.Paste();
            tmrCompile.Enabled = false;
            tmrCompile.Enabled = true;
        }

        private void FindCommand()
        {
            txtCode.FindAndReplace(false);
        }

        private void ReplaceCommand()
        {
            txtCode.FindAndReplace(true);
        }

        private void UndoCommand()
        {
            txtCode.Undo();
            tmrCompile.Enabled = false;
            tmrCompile.Enabled = true;
        }

        private void RedoCommand()
        {
            txtCode.Redo();
            tmrCompile.Enabled = false;
            tmrCompile.Enabled = true;
        }

        private void SelectAllCommand()
        {
            txtCode.SelectAll();
        }

        private void IndentCommand()
        {
            txtCode.Indent();
        }

        private void UndentCommand()
        {
            txtCode.UnIndent();
        }

        private void CommentCommand()
        {
            txtCode.Comment();
            tmrCompile.Enabled = false;
            tmrCompile.Enabled = true;
        }

        private void UnCommentCommand()
        {
            txtCode.UnComment();
            tmrCompile.Enabled = false;
            tmrCompile.Enabled = true;
        }

        private void RunCommand()
        {
            double SaveOpacitySetting = Opacity;
            Opacity = 0;
            RunWithDialog();
            Opacity = SaveOpacitySetting;
        }
        #endregion

        #region File Menu Event functions
        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CreateNewFile();
        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFile();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Save();
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveAs();
        }

        private void saveAsDLLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveAsDLL();
        }

        private void userInterfaceDesignerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UIDesigner();
        }

        private void userInterfaceRenumberToolStripMenuItem_Click(object sender, EventArgs e)
        {
            txtCode.ReNumberUIVariables();
        }

        private void previewEffectMenuItem_Click(object sender, EventArgs e)
        {
            RunCommand();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tmrCompile.Enabled = false;
            this.Close();
        }
        #endregion

        #region Edit menu Event functions
        private void editToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            this.cutToolStripMenuItem1.Enabled = txtCode.SelectedText.Length > 0;
            this.copyToolStripMenuItem1.Enabled = txtCode.SelectedText.Length > 0;
            this.selectAllToolStripMenuItem1.Enabled = txtCode.TextLength > 0;
            this.indentToolStripMenuItem.Enabled = true;
            this.unindentToolStripMenuItem.Enabled = true;
            this.pasteToolStripMenuItem1.Enabled = txtCode.CanPaste;
            this.commentSelectionToolStripMenuItem.Enabled = true;
            this.uncommentSelectionToolStripMenuItem.Enabled = true;
            this.undoToolStripMenuItem1.Enabled = txtCode.CanUndo;
            this.redoToolStripMenuItem1.Enabled = txtCode.CanRedo;
        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UndoCommand();
        }

        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RedoCommand();
        }

        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SelectAllCommand();
        }

        private void searchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FindCommand();
        }

        private void replaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ReplaceCommand();
        }

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CutSelection();
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CopySelection();
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PasteSelection();
        }

        private void indentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            IndentCommand();
        }

        private void unindentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UndentCommand();
        }

        private void commentSelectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CommentCommand();
        }

        private void uncommentSelectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UnCommentCommand();
        }
        #endregion

        #region View menu Event functions
        private void toolBarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!toolStrip1.Visible)
            {
                Settings.ToolBar = true;
                toolBarToolStripMenuItem.CheckState = CheckState.Checked;
                toolStrip1.Visible = true;
                txtCode.Location = new Point(txtCode.Left, tabStrip1.Bottom);
                txtCode.Height = txtCode.Height - toolStrip1.Height;
            }
            else
            {
                Settings.ToolBar = false;
                toolBarToolStripMenuItem.CheckState = CheckState.Unchecked;
                toolStrip1.Visible = false;
                txtCode.Location = new Point(txtCode.Left, tabStrip1.Bottom);
                txtCode.Height = txtCode.Height + toolStrip1.Height;
            }
            txtCode.Focus();
        }

        private void viewCheckBoxes(bool ErrorsVisible, bool DebugVisible)
        {
            if (ErrorsVisible)
            {
                Settings.ErrorBox = true;
                Settings.Output = false;
                viewErrorsToolStripMenuItem.CheckState = CheckState.Checked;
                viewDebugToolStripMenuItem.CheckState = CheckState.Unchecked;
                ShowErrors.Checked = true;
                ShowOutput.Checked = false;
                errorList.Visible = true;
                OutputTextBox.Visible = false;
                ClearOutput.Enabled = false;
                txtCode.Height = errorList.Top - txtCode.Top + 1;
            }
            else if (DebugVisible)
            {
                Settings.ErrorBox = false;
                Settings.Output = true;
                viewErrorsToolStripMenuItem.CheckState = CheckState.Unchecked;
                viewDebugToolStripMenuItem.CheckState = CheckState.Checked;
                ShowErrors.Checked = false;
                ShowOutput.Checked = true;
                errorList.Visible = false;
                OutputTextBox.Visible = true;
                ClearOutput.Enabled = true;
                txtCode.Height = errorList.Top - txtCode.Top + 1;
            }
            else
            {
                Settings.ErrorBox = false;
                Settings.Output = false;
                viewErrorsToolStripMenuItem.CheckState = CheckState.Unchecked;
                viewDebugToolStripMenuItem.CheckState = CheckState.Unchecked;
                ShowErrors.Checked = false;
                ShowOutput.Checked = false;
                errorList.Visible = false;
                OutputTextBox.Visible = false;
                ClearOutput.Enabled = false;
                txtCode.Height = errorList.Bottom - txtCode.Top;
            }
        }

        private void viewErrorsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ShowErrors.Checked)
            {
                viewCheckBoxes(false, false);
            }
            else
            {
                viewCheckBoxes(true, false);
            }
            txtCode.Focus();
        }

        private void viewDebugToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ShowOutput.Checked)
            {
                viewCheckBoxes(false, false);
            }
            else
            {
                viewCheckBoxes(false, true);
            }
            txtCode.Focus();
        }

        private void ShowErrors_Click(object sender, EventArgs e)
        {
            if (ShowErrors.Checked)
            {
                viewCheckBoxes(true, false);
            }
            else
            {
                viewCheckBoxes(false, false);
            }
            txtCode.Focus();
        }

        private void ShowOutput_Click(object sender, EventArgs e)
        {
            if (ShowOutput.Checked)
            {
                viewCheckBoxes(false, true);
            }
            else
            {
                viewCheckBoxes(false, false);
            }
            txtCode.Focus();
        }

        private void largeFontToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (txtCode.Zoom != 2)
            {
                txtCode.Zoom = 2;
                Settings.LargeFonts = true;
                largeFontToolStripMenuItem.CheckState = CheckState.Checked;
            }
            else
            {
                txtCode.Zoom = 0;
                Settings.LargeFonts = false;
                largeFontToolStripMenuItem.CheckState = CheckState.Unchecked;
            }
            txtCode.Focus();
        }

        private void wordWrapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (txtCode.WrapMode == WrapMode.None)
            {
                txtCode.WrapMode = WrapMode.Whitespace;
                Settings.WordWrap = true;
                wordWrapToolStripMenuItem.CheckState = CheckState.Checked;
                txtCode.WrapVisualFlags = WrapVisualFlags.Start;
            }
            else
            {
                txtCode.WrapMode = WrapMode.None;
                Settings.WordWrap = false;
                wordWrapToolStripMenuItem.CheckState = CheckState.Unchecked;
                txtCode.WrapVisualFlags = WrapVisualFlags.None;
            }
            txtCode.Focus();
        }

        private void whiteSpaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (txtCode.ViewWhitespace == WhitespaceMode.Invisible)
            {
                txtCode.ViewWhitespace = WhitespaceMode.VisibleAlways;
                Settings.WhiteSpace = true;
                whiteSpaceToolStripMenuItem.CheckState = CheckState.Checked;
            }
            else
            {
                txtCode.ViewWhitespace = WhitespaceMode.Invisible;
                Settings.WhiteSpace = false;
                whiteSpaceToolStripMenuItem.CheckState = CheckState.Unchecked;
            }
            txtCode.Focus();
        }

        private void codeFoldingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!txtCode.CodeFoldingEnabled)
            {
                txtCode.CodeFoldingEnabled = true;
                Settings.CodeFolding = true;
                codeFoldingToolStripMenuItem.CheckState = CheckState.Checked;
            }
            else
            {
                txtCode.CodeFoldingEnabled = false;
                Settings.CodeFolding = false;
                codeFoldingToolStripMenuItem.CheckState = CheckState.Unchecked;
            }
            txtCode.Focus();
        }

        private void lineNumbersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!txtCode.LineNumbersEnabled)
            {
                txtCode.LineNumbersEnabled = true;
                Settings.LineNumbers = true;
                lineNumbersToolStripMenuItem.CheckState = CheckState.Checked;
            }
            else
            {
                txtCode.LineNumbersEnabled = false;
                Settings.LineNumbers = false;
                lineNumbersToolStripMenuItem.CheckState = CheckState.Unchecked;
            }
            txtCode.Focus();
        }

        private void bookmarksToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!txtCode.BookmarksEnabled)
            {
                txtCode.BookmarksEnabled = true;
                Settings.Bookmarks = true;
                bookmarksToolStripMenuItem.CheckState = CheckState.Checked;
            }
            else
            {
                txtCode.BookmarksEnabled = false;
                Settings.Bookmarks = false;
                bookmarksToolStripMenuItem.CheckState = CheckState.Unchecked;
            }
            txtCode.Focus();
        }

        private void indicatorMapMenuItem_Click(object sender, EventArgs e)
        {
            if (!txtCode.MapEnabled)
            {
                txtCode.MapEnabled = true;
                Settings.Map = true;
                indicatorMapMenuItem.CheckState = CheckState.Checked;
            }
            else
            {
                txtCode.MapEnabled = false;
                Settings.Map = false;
                indicatorMapMenuItem.CheckState = CheckState.Unchecked;
            }
            txtCode.Focus();
        }

        private void fontsCourierMenuItem_Click(object sender, EventArgs e)
        {
            fontsCourierMenuItem.Checked = true;
            fontsConsolasMenuItem.Checked = false;
            fontsEnvyRMenuItem.Checked = false;
            fontsHackMenuItem.Checked = false;
            fontsVerdanaMenuItem.Checked = false;
            Settings.FontFamily = "Courier New";
            txtCode.Styles[Style.Default].Font = "Courier New";
            OutputTextBox.Font = new Font("Courier New", OutputTextBox.Font.Size);
            errorList.Font = new Font("Courier New", errorList.Font.Size);
            txtCode.Focus();
        }

        private void fontsConsolasMenuItem_Click(object sender, EventArgs e)
        {
            fontsCourierMenuItem.Checked = false;
            fontsConsolasMenuItem.Checked = true;
            fontsEnvyRMenuItem.Checked = false;
            fontsHackMenuItem.Checked = false;
            fontsVerdanaMenuItem.Checked = false;
            Settings.FontFamily = "Consolas";
            txtCode.Styles[Style.Default].Font = "Consolas";
            OutputTextBox.Font = new Font("Consolas", OutputTextBox.Font.Size);
            errorList.Font = new Font("Consolas", errorList.Font.Size);
            txtCode.Focus();
        }

        private void fontsEnvyRMenuItem_Click(object sender, EventArgs e)
        {
            fontsCourierMenuItem.Checked = false;
            fontsConsolasMenuItem.Checked = false;
            fontsEnvyRMenuItem.Checked = true;
            fontsHackMenuItem.Checked = false;
            fontsVerdanaMenuItem.Checked = false;
            Settings.FontFamily = "Envy Code R";
            txtCode.Styles[Style.Default].Font = "Envy Code R";
            OutputTextBox.Font = new Font("Envy Code R", OutputTextBox.Font.Size);
            errorList.Font = new Font("Envy Code R", errorList.Font.Size);
            txtCode.Focus();
        }

        private void fontsHackMenuItem_Click(object sender, EventArgs e)
        {
            fontsCourierMenuItem.Checked = false;
            fontsConsolasMenuItem.Checked = false;
            fontsEnvyRMenuItem.Checked = false;
            fontsHackMenuItem.Checked = true;
            fontsVerdanaMenuItem.Checked = false;
            Settings.FontFamily = "Hack";
            txtCode.Styles[Style.Default].Font = "Hack";
            OutputTextBox.Font = new Font("Hack", OutputTextBox.Font.Size);
            errorList.Font = new Font("Hack", errorList.Font.Size);
            txtCode.Focus();
        }

        private void fontsVerdanaMenuItem_Click(object sender, EventArgs e)
        {
            fontsCourierMenuItem.Checked = false;
            fontsConsolasMenuItem.Checked = false;
            fontsEnvyRMenuItem.Checked = false;
            fontsHackMenuItem.Checked = false;
            fontsVerdanaMenuItem.Checked = true;
            Settings.FontFamily = "Verdana";
            txtCode.Styles[Style.Default].Font = "Verdana";
            OutputTextBox.Font = new Font("Verdana", OutputTextBox.Font.Size);
            errorList.Font = new Font("Verdana", errorList.Font.Size);
            txtCode.Focus();
        }

        private void opacity50MenuItem_Click(object sender, EventArgs e)
        {
            this.Opacity = 0.50;
            opacity50MenuItem.Checked = true;
            opacity75MenuItem.Checked = false;
            opacity90MenuItem.Checked = false;
            opacity100MenuItem.Checked = false;
            txtCode.Focus();
        }

        private void opacity100MenuItem_Click(object sender, EventArgs e)
        {
            this.Opacity = 1.00;
            opacity50MenuItem.Checked = false;
            opacity75MenuItem.Checked = false;
            opacity90MenuItem.Checked = false;
            opacity100MenuItem.Checked = true;
            txtCode.Focus();
        }

        private void opacity75MenuItem_Click(object sender, EventArgs e)
        {
            this.Opacity = 0.75;
            opacity50MenuItem.Checked = false;
            opacity75MenuItem.Checked = true;
            opacity90MenuItem.Checked = false;
            opacity100MenuItem.Checked = false;
            txtCode.Focus();
        }

        private void opacity90MenuItem_Click(object sender, EventArgs e)
        {
            this.Opacity = 0.90;
            opacity50MenuItem.Checked = false;
            opacity75MenuItem.Checked = false;
            opacity90MenuItem.Checked = true;
            opacity100MenuItem.Checked = false;
            txtCode.Focus();
        }

        private void lightToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.ForeColor = Color.Black;
            this.BackColor = Color.White;
            ApplyTheme();
            txtCode.Theme = Theme.Light;
            Settings.EditorTheme = Theme.Light;
            lightToolStripMenuItem.CheckState = CheckState.Checked;
            darkToolStripMenuItem.CheckState = CheckState.Unchecked;
            autoToolStripMenuItem.CheckState = CheckState.Unchecked;
            txtCode.Focus();
        }

        private void darkToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.ForeColor = Color.White;
            this.BackColor = Color.FromArgb(40, 40, 40);
            ApplyTheme();
            txtCode.Theme = Theme.Dark;
            Settings.EditorTheme = Theme.Dark;
            lightToolStripMenuItem.CheckState = CheckState.Unchecked;
            darkToolStripMenuItem.CheckState = CheckState.Checked;
            autoToolStripMenuItem.CheckState = CheckState.Unchecked;
            txtCode.Focus();
        }

        private void autoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.ForeColor = OriginalForeColor;
            this.BackColor = OriginalBackColor;
            ApplyTheme();
            txtCode.Theme = (PdnTheme.BackColor.R < 128 && PdnTheme.BackColor.G < 128 && PdnTheme.BackColor.B < 128) ? Theme.Dark : Theme.Light;
            Settings.EditorTheme = Theme.Auto;
            lightToolStripMenuItem.CheckState = CheckState.Unchecked;
            darkToolStripMenuItem.CheckState = CheckState.Unchecked;
            autoToolStripMenuItem.CheckState = CheckState.Checked;
            txtCode.Focus();
        }

        private void ClearOutput_Click(object sender, EventArgs e)
        {
            OutputTextBox.Clear();
            txtCode.Focus();
        }
        #endregion

        #region Help menu Event functions
        private void helpTopicsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Services.GetService<IShellService>().LaunchUrl(null, "http://www.BoltBait.com/pdn/codelab/help");
        }

        private void checkForUpdatesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!CheckForUpdates)
            {
                Settings.CheckForUpdates = true;
                checkForUpdatesToolStripMenuItem.CheckState = CheckState.Checked;
                CheckForUpdates = true;
                GoCheckForUpdates(false, true);
            }
            else
            {
                Settings.CheckForUpdates = false;
                checkForUpdatesToolStripMenuItem.CheckState = CheckState.Unchecked;
                CheckForUpdates = false;
            }
            txtCode.Focus();
        }

        private void changesInThisVersionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Services.GetService<IShellService>().LaunchUrl(null, "http://www.boltbait.com/pdn/codelab/history.asp#v" + ThisVersion);
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show(WindowTitle + "\nCopyright © 2006-2018, All Rights Reserved.\n\nTom Jackson:\tInitial Code, Compile to DLL\n\nDavid Issel:\tEffect UI Creation, Effect Icons, Effect Help\n\t\tSystem, Editor Enhancements (including\n\t\tCode Templates, CodeLab Updater, Bug\n\t\tFixes), and Coding Tutorials\n\nJason Wendt:\tMigration to ScintillaNET editor control,\n\t\tIntelligent Assistance (including code\n\t\tcompletion with snippets and tips),\n\t\tDebug Output, Dark Theme, Bug Fixes", "About CodeLab", MessageBoxButtons.OK, MessageBoxIcon.Information);
            txtCode.Focus();
        }
        #endregion

        #region Context menu Event functions
        private void contextMenuStrip1_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.cutToolStripMenuItem.Enabled = txtCode.SelectedText.Length > 0;
            this.copyToolStripMenuItem.Enabled = txtCode.SelectedText.Length > 0;
            this.selectAllToolStripMenuItem.Enabled = txtCode.TextLength > 0;
            this.searchToolStripMenuItem.Enabled = true;
            this.indentToolStripMenuItem1.Enabled = true;
            this.unindentToolStripMenuItem1.Enabled = true;
            this.pasteToolStripMenuItem.Enabled = txtCode.CanPaste;
            this.commentSelectionToolStripMenuItem1.Enabled = txtCode.TextLength > 0;
            this.uncommentSelectionToolStripMenuItem1.Enabled = txtCode.TextLength > 0;
            this.undoToolStripMenuItem.Enabled = txtCode.CanUndo;
            this.redoToolStripMenuItem.Enabled = txtCode.CanRedo;
        }

        private void undoToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            UndoCommand();
        }

        private void redoToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            RedoCommand();
        }

        private void selectAllToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            SelectAllCommand();
            txtCode.Focus();
        }

        private void searchToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            FindCommand();
        }

        private void replaceToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ReplaceCommand();
            txtCode.Focus();
        }

        private void cutToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            CutSelection();
        }

        private void copyToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            CopySelection();
        }

        private void pasteToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            PasteSelection();
        }

        private void indentToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            IndentCommand();
        }

        private void unindentToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            UndentCommand();
        }

        private void commentSelectionToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            CommentCommand();
        }

        private void uncommentSelectionToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            UnCommentCommand();
        }
        #endregion

        #region Toolbar Event functions
        private void NewButton_Click(object sender, EventArgs e)
        {
            CreateNewFile();
        }

        private void OpenButton_Click(object sender, EventArgs e)
        {
            OpenFile();
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            Save();
        }

        private void SaveDLLButton_Click(object sender, EventArgs e)
        {
            SaveAsDLL();
        }

        private void UIDesignerButton_Click(object sender, EventArgs e)
        {
            UIDesigner();
        }

        private void CutButton_Click(object sender, EventArgs e)
        {
            CutSelection();
        }

        private void CopyButton_Click(object sender, EventArgs e)
        {
            CopySelection();
        }

        private void PasteButton_Click(object sender, EventArgs e)
        {
            PasteSelection();
        }

        private void SelectAllButton_Click(object sender, EventArgs e)
        {
            SelectAllCommand();
        }

        private void UndoButton_Click(object sender, EventArgs e)
        {
            UndoCommand();
        }

        private void RedoButton_Click(object sender, EventArgs e)
        {
            RedoCommand();
        }

        private void IndentButton_Click(object sender, EventArgs e)
        {
            IndentCommand();
        }

        private void UndentButton_Click(object sender, EventArgs e)
        {
            UndentCommand();
        }

        private void CommentButton_Click(object sender, EventArgs e)
        {
            CommentCommand();
        }

        private void UnCommentButton_Click(object sender, EventArgs e)
        {
            UnCommentCommand();
        }

        private void formatDocMenuItem_Click(object sender, EventArgs e)
        {
            txtCode.FormatDocument();
        }

        private void UpdateToolBarButtons()
        {
            CutButton.Enabled = txtCode.SelectedText.Length > 0;
            CopyButton.Enabled = txtCode.SelectedText.Length > 0;
            UndoButton.Enabled = txtCode.CanUndo;
            RedoButton.Enabled = txtCode.CanRedo;
        }

        private void txtCode_UpdateUI(object sender, UpdateUIEventArgs e)
        {
            if (e.Change.HasFlag(UpdateChange.Selection) || e.Change.HasFlag(UpdateChange.Content))
            {
                UpdateToolBarButtons();
            }
        }

        private void RunButton_Click(object sender, EventArgs e)
        {
            RunCommand();
        }
        #endregion

        #region Recent Items functions
        private void AddToRecents(string filePath)
        {
            string recents = Settings.RecentDocs;

            if (recents == string.Empty)
            {
                recents = filePath;
            }
            else
            {
                recents = filePath + "|" + recents;

                var paths = recents.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                List<string> recentsList = new List<string>();
                foreach (string itemPath in paths)
                {
                    bool contains = false;
                    foreach (string listItem in recentsList)
                    {
                        if (listItem.Equals(itemPath, StringComparison.OrdinalIgnoreCase))
                        {
                            contains = true;
                            break;
                        }
                    }

                    if (!contains)
                    {
                        recentsList.Add(itemPath);
                    }
                }

                int length = Math.Min(8, recentsList.Count);
                recents = string.Join("|", recentsList.ToArray(), 0, length);
            }

            Settings.RecentDocs = recents;
        }

        private void openRecentToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            this.openRecentToolStripMenuItem.DropDownItems.Clear();

            string recents = Settings.RecentDocs;

            List<ToolStripItem> recentsList = new List<ToolStripItem>();
            string[] paths = recents.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            int count = 1;
            foreach (string itemPath in paths)
            {
                if (!File.Exists(itemPath))
                {
                    continue;
                }

                ToolStripMenuItem recentItem = new ToolStripMenuItem();

                string menuText = $"&{count} {Path.GetFileName(itemPath)}";
                try
                {
                    Regex REName = new Regex(@"//[\s-[\r\n]]*Name[\s-[\r\n]]*:[\s-[\r\n]]*(?<menulabel>.*)(?=\r?\n|$)", RegexOptions.IgnoreCase);
                    Match wtn = REName.Match(File.ReadAllText(itemPath));
                    if (wtn.Success)
                    {
                        string scriptName = wtn.Groups["menulabel"].Value.Trim();
                        if (scriptName != string.Empty)
                        {
                            menuText = $"&{count} {scriptName} ({Path.GetFileName(itemPath)})";
                        }
                    }
                }
                catch
                {
                }

                recentItem.Text = menuText;
                recentItem.ToolTipText = itemPath;
                recentItem.Click += RecentItem_Click;

                string imagePath = Path.ChangeExtension(itemPath, ".png");
                if (File.Exists(imagePath))
                {
                    recentItem.Image = new Bitmap(imagePath);
                }

                recentsList.Add(recentItem);
                count++;
            }

            if (recentsList.Count > 0)
            {
                ToolStripSeparator toolStripSeparator = new ToolStripSeparator();
                recentsList.Add(toolStripSeparator);

                ToolStripMenuItem clearRecents = new ToolStripMenuItem();
                clearRecents.Text = "&Clear List";
                clearRecents.Click += ClearRecents_Click;
                recentsList.Add(clearRecents);

                this.openRecentToolStripMenuItem.DropDownItems.AddRange(recentsList.ToArray());
            }
            else
            {
                ToolStripMenuItem noRecents = new ToolStripMenuItem();
                noRecents.Text = "No Recent Items";
                noRecents.Enabled = false;

                this.openRecentToolStripMenuItem.DropDownItems.Add(noRecents);
            }
        }

        private void ClearRecents_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to clear the Open Recent list?", "CodeLab", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Settings.RecentDocs = string.Empty;
            }
        }

        private void RecentItem_Click(object sender, EventArgs e)
        {
            string filePath = (sender as ToolStripMenuItem)?.ToolTipText;
            if (!File.Exists(filePath))
            {
                MessageBox.Show("File not found.\n" + filePath, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtCode.Focus();
                return;
            }

            AddToRecents(filePath);

            FullScriptPath = filePath;
            FileName = Path.GetFileNameWithoutExtension(filePath);

            if (tabStrip1.SelectedTabGuid == Guid.Empty && txtCode.IsVirgin)
            {
                UpdateTabProperties();
            }
            else
            {
                tabStrip1.NewTab(FileName, FullScriptPath);
            }

            txtCode.Text = File.ReadAllText(filePath);
            txtCode.ExecuteCmd(Command.ScrollToEnd); // Workaround for a scintilla bug
            txtCode.ExecuteCmd(Command.ScrollToStart);
            txtCode.EmptyUndoBuffer();
            txtCode.SetSavePoint();
            txtCode.Focus();
            Build();
        }
        #endregion

        #region Dirty Document functions
        private void txtCode_SavePointLeft(object sender, EventArgs e)
        {
            tabStrip1.SelectedTabIsDirty = true;
        }

        private void txtCode_SavePointReached(object sender, EventArgs e)
        {
            tabStrip1.SelectedTabIsDirty = false;
        }
        #endregion

        #region Document Tabs functions
        private void tabStrip1_SelectedIndexChanged(object sender, EventArgs e)
        {
            FileName = tabStrip1.SelectedTabTitle;
            FullScriptPath = tabStrip1.SelectedTabPath;
            UpdateWindowTitle();
            txtCode.SwitchToDocument(tabStrip1.SelectedTabGuid);
            UpdateToolBarButtons();
            BuildAsync();
        }

        private void tabStrip1_NewTabCreated(object sender, TabEventArgs e)
        {
            txtCode.CreateNewDocument(e.TabGuid);
            UpdateWindowTitle();
            UpdateToolBarButtons();
        }

        private void tabStrip1_TabClosingAndDirty(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (PromptToSave() == DialogResult.Cancel)
            {
                e.Cancel = true;
            }
        }

        private void tabStrip1_TabClosed(object sender, TabEventArgs e)
        {
            this.txtCode.CloseDocument(e.TabGuid);
        }

        private void UpdateTabProperties()
        {
            tabStrip1.SelectedTabTitle = FileName;
            tabStrip1.SelectedTabPath = FullScriptPath;

            UpdateWindowTitle();
        }

        private void UpdateWindowTitle()
        {
            this.Text = FileName + " - " + WindowTitle;
        }
        #endregion
    }
}
