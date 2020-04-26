/////////////////////////////////////////////////////////////////////////////////
// CodeLab for Paint.NET
// Copyright ©2006 Rick Brewster, Tom Jackson. All Rights Reserved.
// Portions Copyright ©2007-2020 BoltBait. All Rights Reserved.
// Portions Copyright ©2016-2020 Jason Wendt. All Rights Reserved.
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

using PaintDotNet.AppModel;
using ScintillaNET;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PaintDotNet.Effects
{
    internal partial class CodeLabConfigDialog : EffectConfigDialog
    {
        private const string ThisVersion = "5.2"; // Remember to change it in CodeLab.cs too!
        private const string WebUpdateFile = "https://www.boltbait.com/versions.txt"; // The web site to check for updates
        private const string ThisApplication = "1"; // in the WebUpadteFile, CodeLab is application #1
        // format of the versions.txt file:  application number;current version;URL to download current version
        // for example: 1;2.13;https://boltbait.com/pdn/CodeLab/CodeLab213.zip
        // each application on its own line
        #region Constructor
        private const string WindowTitle = "CodeLab v" + ThisVersion;
        private string FileName = "Untitled";
        private string FullScriptPath = "";
        private bool CheckForUpdates;
        private string UpdateURL = "";
        private string UpdateVER = "";
        private EffectConfigToken previewToken = null;
        private readonly Color OriginalForeColor;
        private readonly Color OriginalBackColor;
        private string effectFlag = null;

        private string EffectFlag
        {
            get
            {
                if (this.effectFlag != null)
                {
                    return this.effectFlag;
                }

                if (this.Effect != null)
                {
                    string name = this.Effect.Name;
                    int dashIndex = name.IndexOf('-');
                    this.effectFlag = dashIndex > 0 ?
                        " (" + name.Substring(dashIndex + 2) + ")" :
                        string.Empty;
                    return this.effectFlag;
                }

                return string.Empty;
            }
        }

        public CodeLabConfigDialog()
        {
            Task.Run(() => Intelli.Keywords); // Forces the Intelli class to start initializing in the background
            InitializeComponent();

#if FASTDEBUG
            this.Icon = ResUtil.CreateIcon("CodeLab");
            this.ShowInTaskbar = true;
#endif

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

            PopulateFontSubMenu(editorFont);
            txtCode.Styles[Style.Default].Font = editorFont;
            OutputTextBox.Font = new Font(editorFont, OutputTextBox.Font.Size);
            errorList.Font = new Font(editorFont, errorList.Font.Size);
            #endregion

            this.Opacity = 1.00;
            opacity50MenuItem.Checked = false;
            opacity75MenuItem.Checked = false;
            opacity90MenuItem.Checked = false;
            opacity100MenuItem.Checked = true;

            // PDN Theme
            ApplyTheme();
            txtCode.Theme = PdnTheme.Theme;

            ResetScript();
            BuildAsync();
            txtCode.Focus();
        }
        #endregion

        #region Token functions
        protected override void InitTokenFromDialog()
        {
            CodeLabConfigToken sect = (CodeLabConfigToken)theEffectToken;
            sect.UserCode = txtCode.Text;
            sect.UserScriptObject = ScriptBuilder.BuiltEffect;
            sect.ScriptName = FileName;
            sect.ScriptPath = FullScriptPath;
            sect.Dirty = tabStrip1.SelectedTabIsDirty;
            sect.PreviewToken = previewToken;
            sect.Bookmarks = txtCode.Bookmarks;
            sect.ProjectType = tabStrip1.SelectedTabProjType;
        }

        protected override void InitDialogFromToken(EffectConfigToken effectTokenCopy)
        {
            if (effectTokenCopy is CodeLabConfigToken token)
            {
                FileName = token.ScriptName;
                FullScriptPath = token.ScriptPath;
                if (token.ProjectType != ProjectType.Effect)
                {
                    tabStrip1.SelectedTabIsDirty = false;
                    tabStrip1.NewTab(FileName, FullScriptPath, token.ProjectType);
                    tabStrip1.CloseFirstTab();
                }
                txtCode.Text = token.UserCode;
                txtCode.EmptyUndoBuffer();
                if (!token.Dirty)
                {
                    txtCode.SetSavePoint();
                }
                txtCode.Bookmarks = token.Bookmarks;

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
                PreviewToken = null,
                Bookmarks = Array.Empty<int>()
            };
        }
        #endregion

        #region Build Script actions
        private void Build()
        {
            ClearErrorList();

            ProjectType projType = tabStrip1.SelectedTabProjType;
            if (projType == ProjectType.None || projType == ProjectType.Reference)
            {
                return;
            }

            switch (projType)
            {
                case ProjectType.Effect:
                    ScriptBuilder.Build(txtCode.Text, OutputTextBox.Visible);
                    DisplayErrors();
                    txtCode.UpdateSyntaxHighlighting();
                    FinishTokenUpdate();
                    break;
                case ProjectType.FileType:
                    ScriptBuilder.BuildFileType(txtCode.Text, OutputTextBox.Visible);
                    DisplayErrors();
                    txtCode.UpdateSyntaxHighlighting();
                    RunFileType();
                    break;
                case ProjectType.Shape:
                    ShapeBuilder.TryParseShapeCode(txtCode.Text);
                    DisplayErrors();
                    FinishTokenUpdate();
                    break;
            }
        }

        private async Task BuildAsync()
        {
            ProjectType projType = tabStrip1.SelectedTabProjType;
            if (projType == ProjectType.None || projType == ProjectType.Shape)
            {
                return;
            }

            tmrCompile.Enabled = false;
            string code = txtCode.Text;
            bool debug = OutputTextBox.Visible;
            await Task.Run(() =>
            {
                switch (projType)
                {
                    case ProjectType.Effect:
                        ScriptBuilder.Build(code, debug);
                        break;
                    case ProjectType.FileType:
                        ScriptBuilder.BuildFileType(code, debug);
                        break;
                }
            });

            if (this.IsDisposed)
            {
                return;
            }

            DisplayErrors();

            switch (projType)
            {
                case ProjectType.Effect:
                    txtCode.UpdateSyntaxHighlighting();
                    FinishTokenUpdate();
                    break;
                case ProjectType.FileType:
                    txtCode.UpdateSyntaxHighlighting();
                    RunFileType();
                    break;
            }

            tmrCompile.Enabled = true;
        }

        private void RunEffectWithDialog()
        {
            if (errorList.Items.Count != 0)
            {
                FlexibleMessageBox.Show("Before you can preview your Effect, you must resolve all code errors.", "Build Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!ScriptBuilder.BuildFullPreview(txtCode.Text))
            {
                FlexibleMessageBox.Show("Something went wrong, and the Preview can't be run.", "Preview Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                DisplayErrors();
                return;
            }

            if (!ScriptBuilder.BuiltEffect.Options.Flags.HasFlag(EffectFlags.Configurable))
            {
                FlexibleMessageBox.Show("There are no UI controls, so the Preview can't be displayed.", "Preview Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            ScriptBuilder.BuiltEffect.EnvironmentParameters = this.Effect.EnvironmentParameters;
            using (EffectConfigDialog previewDialog = ScriptBuilder.BuiltEffect.CreateConfigDialog())
            {
                previewToken = previewDialog.EffectToken;
                previewDialog.EffectTokenChanged += (sender, e) => FinishTokenUpdate();

                previewDialog.ShowDialog();
            }

            previewToken = null;
            Build();
        }

        private void RunFileTypeWithDialog()
        {
            if (errorList.Items.Count != 0)
            {
                FlexibleMessageBox.Show("Before you can preview your FileType, you must resolve all code errors.", "Build Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            //if (!ScriptBuilder.BuiltFileType.SupportsConfiguration)
            //{
            //    FlexibleMessageBox.Show("There are no UI controls, so the Preview can't be displayed.", "Preview Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    return;
            //}

            using (SaveConfigDialog saveDialog = new SaveConfigDialog(ScriptBuilder.BuiltFileType, EnvironmentParameters.SourceSurface))
            {
                saveDialog.ShowDialog();
            }
        }

        private void RunFileType()
        {
            if (ScriptBuilder.BuiltFileType == null)
            {
                return;
            }

            Size size = new Size(400, 300);
            using (Document document = new Document(size))
            using (MemoryStream stream = new MemoryStream())
            using (Surface scratchSurface = new Surface(size))
            {
                document.Layers.Add(new BitmapLayer(size, ColorBgra.DodgerBlue.NewAlpha(85)));

                SaveConfigToken token = ScriptBuilder.BuiltFileType.CreateDefaultSaveConfigToken();
                ProgressEventHandler progress = (object s1, ProgressEventArgs e1) =>
                {
                };

                try
                {
                    ScriptBuilder.BuiltFileType.Save(document, stream, token, scratchSurface, progress, false);

                    stream.Seek(0, SeekOrigin.Begin);

                    using (Document savedDoc = ScriptBuilder.BuiltFileType.Load(stream))
                    {
                        savedDoc.Flatten(scratchSurface);
                    }

                    // Debug Output capture
                    if (OutputTextBox.Visible)
                    {
                        string output = ScriptBuilder.BuiltFileType.GetType().GetProperty("__DebugMsgs", typeof(string))?.GetValue(ScriptBuilder.BuiltFileType)?.ToString();
                        if (!output.IsNullOrEmpty() && output.Trim().Length > 0)
                        {
                            OutputTextBox.AppendText(output);
                        }
                    }
                }
                catch (Exception ex)
                {
                    errorList.Items.Add(ex.ToString());
                    ShowErrors.Text = $"Show Errors List ({errorList.Items.Count})";
                    ShowErrors.ForeColor = Color.Red;
                }

                document.Layers.DisposeAll();
            }
        }

        private void DisplayErrors()
        {
            ClearErrorList();
            txtCode.ClearErrors();

            switch (tabStrip1.SelectedTabProjType)
            {
                case ProjectType.Effect:
                case ProjectType.FileType:
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

                    break;
                case ProjectType.Shape:
                    if (ShapeBuilder.Error == null)
                    {
                        txtCode.UpdateIndicatorBar();
                        return;
                    }

                    errorList.Items.Add(ShapeBuilder.Error);

                    if (ShapeBuilder.Error.Line > 0)
                    {
                        txtCode.AddError(ShapeBuilder.Error.Line - 1, ShapeBuilder.Error.Column);
                    }

                    break;
            }

            txtCode.UpdateIndicatorBar();

            ShowErrors.Text = $"Show Errors List ({errorList.Items.Count})";
            ShowErrors.ForeColor = Color.Red;
        }

        private void ClearErrorList()
        {
            errorList.Items.Clear();
            toolTips.SetToolTip(errorList, "");

            ShowErrors.Text = "Show Errors List";
            ShowErrors.ForeColor = this.ForeColor;
        }

        private void ResetScript()
        {
            InitialInitToken();
            InitDialogFromToken();
            FinishTokenUpdate();
        }

        private void LoadFile(string filePath)
        {
            string fileContents = null;
            try
            {
                fileContents = File.ReadAllText(filePath);
            }
            catch
            {
                return;
            }

            if (fileContents == null)
            {
                return;
            }

            AddToRecents(filePath);

            FullScriptPath = filePath;
            FileName = Path.GetFileNameWithoutExtension(filePath);

            ProjectType projType;
            switch (Path.GetExtension(filePath).ToLowerInvariant())
            {
                case ".cs":
                    if (Regex.IsMatch(fileContents, @"void\s+Render\s*\(\s*Surface\s+dst\s*,\s*Surface\s+src\s*,\s*Rectangle\s+rect\s*\)\s*{(.|\s)*}", RegexOptions.Singleline))
                    {
                        projType = ProjectType.Effect;
                    }
                    else if (Regex.IsMatch(fileContents, @"void\s+SaveImage\s*\(\s*Document\s+input\s*,\s*Stream\s+output\s*,\s*PropertyBasedSaveConfigToken\s+token\s*,\s*Surface\s+scratchSurface\s*,\s*ProgressEventHandler\s+progressCallback\s*\)\s*{(.|\s)*}", RegexOptions.Singleline))
                    {
                        projType = ProjectType.FileType;
                    }
                    else
                    {
                        projType = ProjectType.None;
                    }
                    break;
                case ".xaml":
                    projType = ProjectType.Shape;
                    break;
                case ".txt":
                default:
                    projType = ProjectType.None;
                    break;
            }

            if (tabStrip1.SelectedTabGuid == Guid.Empty && tabStrip1.SelectedTabProjType == projType && txtCode.IsVirgin)
            {
                UpdateTabProperties();
            }
            else
            {
                tabStrip1.NewTab(FileName, FullScriptPath, projType);
            }

            txtCode.Text = fileContents;
            txtCode.EmptyUndoBuffer();
            txtCode.SetSavePoint();
        }

        private DialogResult PromptToSave()
        {
            int buttonPressed = (int)FlexibleMessageBox.Show(this, $"Do you want to save changes to '{FileName}'?", "Unsaved Changes", new string[] { "Save","Discard","Cancel" }, MessageBoxIcon.Question);
            switch (buttonPressed)
            {
                case 1: // Save
                    if (!SaveFile())
                    {
                        txtCode.Focus();
                        return DialogResult.Cancel;
                    }
                    txtCode.SetSavePoint();
                    return DialogResult.None;
                case 2: // Discard
                    return DialogResult.None;
                case 3: // Cancel
                    txtCode.Focus();
                    return DialogResult.Cancel;
            }
            return DialogResult.None;
        }

        private bool SaveFile()
        {
            if (FullScriptPath.IsNullOrEmpty() || !File.Exists(FullScriptPath))
            {
                return SaveAsFile();
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

        private bool SaveAsFile()
        {
            string fileName = this.FileName;
            string fileExt;
            string description;
            bool isCSharp;
            switch (tabStrip1.SelectedTabProjType)
            {
                case ProjectType.None:
                    fileExt = ".txt";
                    description = "Plain Text";
                    isCSharp = false;
                    break;
                case ProjectType.Shape:
                    fileExt = ".xaml";
                    description = "XAML Shape";
                    isCSharp = false;
                    break;
                case ProjectType.Effect:
                case ProjectType.FileType:
                default:
                    fileExt = ".cs";
                    description = "C# Code";
                    isCSharp = true;
                    break;
            }

            if (isCSharp && fileName.Equals("Untitled", StringComparison.Ordinal))
            {
                Match wtn = Regex.Match(txtCode.Text, @"//[\s-[\r\n]]*Name[\s-[\r\n]]*:[\s-[\r\n]]*(?<scriptName>.*)(?=\r?\n|$)", RegexOptions.IgnoreCase);
                if (wtn.Success)
                {
                    string scriptName = wtn.Groups["scriptName"].Value.Trim();
                    if (scriptName.Length > 0)
                    {
                        fileName = scriptName;
                    }
                }

                if (fileName.Equals("Untitled", StringComparison.Ordinal))
                {
                    fileName = "MyScript";
                }
            }

            SaveFileDialog sfd = new SaveFileDialog
            {
                Title = "Save User Script",
                DefaultExt = fileExt,
                Filter = description + " Files (*" + fileExt.ToUpperInvariant() + ")|*" + fileExt,
                OverwritePrompt = true,
                AddExtension = true,
                FileName = fileName + fileExt,
                InitialDirectory = Settings.LastSourceDirectory
            };

            sfd.FileOk += (sender, e) =>
            {
                if (isCSharp && !char.IsLetter(Path.GetFileName(sfd.FileName), 0))
                {
                    e.Cancel = true;
                    FlexibleMessageBox.Show("The filename must begin with a letter.", "Save User Script", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

            ErrorCodeMenuItem.Visible = (errorList.SelectedItem is ScriptError error && error.ErrorNumber.Length > 0);
        }

        private void CopyErrorMenuItem_Click(object sender, EventArgs e)
        {
            if (errorList.SelectedIndex > -1)
            {
                string errorMsg = (errorList.SelectedItem is ScriptError error) ? error.ErrorText : errorList.SelectedItem.ToString();
                if (!errorMsg.IsNullOrEmpty())
                {
                    System.Windows.Forms.Clipboard.SetText(errorMsg);
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
                LaunchUrl($"https://docs.microsoft.com/dotnet/csharp/language-reference/compiler-messages/{error.ErrorNumber}");
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

                errorList.Items.Add(exc);
                ShowErrors.Text = $"Show Errors List ({errorList.Items.Count})";
                ShowErrors.ForeColor = Color.Red;
            }

            if (sect.Output.Count > 0)
            {
                string output = sect.Output[0];
                sect.Output.Clear();

                if (output.Trim().Length > 0)
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
                e.Cancel = (tabStrip1.TabCount == 1) ?
                    PromptToSave() == DialogResult.Cancel :
                    (int)FlexibleMessageBox.Show("There are one or more Tabs with unsaved changes. These changes will be lost if CodeLab is closed.\r\n\r\nWould you like to return to CodeLab, so these files can be saved?", "Unsaved changes", new string[] { "Return to CodeLab", "Discard changes" }, MessageBoxIcon.Warning) == 1;
            }

            Settings.CloseRegKey();

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
#if FASTDEBUG
            this.Close();
#endif
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            tmrExceptionCheck.Enabled = false;
            tmrCompile.Enabled = false;
#if FASTDEBUG
            this.Close();
#endif
        }

        private static bool IsFontInstalled(string fontName)
        {
            using (Font font = new Font(fontName, 12f))
            {
                return font.Name == fontName;
            }
        }

        private void PopulateFontSubMenu(string fontToHaveChecked)
        {
            string[] monoFonts = { "Cascadia Code", "Consolas", "Courier New", "Envy Code R", "Fira Code", "Hack", "JetBrains Mono", "Verdana" };
            List<ToolStripMenuItem> fontMenuItems = new List<ToolStripMenuItem>(monoFonts.Length);
            foreach (string fontName in monoFonts)
            {
                ToolStripMenuItem fontMenuItem = new ToolStripMenuItem();
                fontMenuItem.CheckState = (fontName == fontToHaveChecked) ? CheckState.Checked : CheckState.Unchecked;
                fontMenuItem.Text = fontName;
                fontMenuItem.Enabled = IsFontInstalled(fontName);
                if (fontMenuItem.Enabled)
                {
                    fontMenuItem.Font = new Font(fontName, fontMenuItem.Font.Size);
                }
                fontMenuItem.Click += FontMenuItem_Click;

                fontMenuItems.Add(fontMenuItem);
            }

            fontsToolStripMenuItem.DropDownItems.AddRange(fontMenuItems.ToArray());
            fontsToolStripMenuItem.DropDownItems.Add(new ToolStripSeparator());
            fontsToolStripMenuItem.DropDownItems.Add("Help with Fonts", null, HelpWithFonts_Click);
        }

        private void ApplyTheme()
        {
            PdnTheme.SetColors(this.ForeColor, this.BackColor);
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

        private void LaunchUrl(string url)
        {
#if FASTDEBUG
            System.Diagnostics.Process.Start(url);
#else
            this.Services.GetService<IShellService>().LaunchUrl(null, url);
#endif
        }
        #endregion

        #region Freshness Check functions
        private void DisplayUpdates(bool silentMode)
        {
            if (UpdateURL != "")
            {
                if (txtCode.Focused) // only popup if code editor has focus (otherwise, we might be doing something that we shouldn't interrupt)
                {
                    if (FlexibleMessageBox.Show("An update to CodeLab is available.\n\nWould you like to download CodeLab v" + UpdateVER + "?\n\n(This will not close your current CodeLab session.)", "CodeLab Updater", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
                    {
                        LaunchUrl(UpdateURL);
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
                    FlexibleMessageBox.Show("You are up-to-date!", "CodeLab Updater", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    FlexibleMessageBox.Show("I'm not sure if you are up-to-date.\n\nI was not able to reach the update website.\n\nTry again later.", "CodeLab Updater", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void GoCheckForUpdates(bool silentMode, bool force)
        {
            UpdateVER = "";
            UpdateURL = "";

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
        private void CreateNewEffect()
        {
            FileNew fn = new FileNew(this.EffectFlag);
            if (fn.ShowDialog() == DialogResult.OK)
            {
                FileName = "Untitled";
                FullScriptPath = "";

                if (tabStrip1.SelectedTabGuid == Guid.Empty && tabStrip1.SelectedTabProjType == ProjectType.Effect && txtCode.IsVirgin)
                {
                    UpdateTabProperties();
                }
                else
                {
                    tabStrip1.NewTab(FileName, FullScriptPath, ProjectType.Effect);
                }

                txtCode.Text = fn.CodeTemplate;
                txtCode.EmptyUndoBuffer();
                Build();
            }
            fn.Dispose();

            txtCode.Focus();
        }

        private void CreateNewPlainText()
        {
            FileName = "Untitled";
            FullScriptPath = string.Empty;

            tabStrip1.NewTab(FileName, FullScriptPath, ProjectType.None);

            //txtCode.Text = string.Empty;
            txtCode.EmptyUndoBuffer();
            Build();
            txtCode.Focus();
        }

        private void CreateNewFileType()
        {
            FileName = "Untitled";
            FullScriptPath = string.Empty;

            tabStrip1.NewTab(FileName, FullScriptPath, ProjectType.FileType);

            txtCode.Text = "" +
                "// Name:\r\n" +
                "// Author:\r\n" +
                "// Version:\r\n" +
                "// Desc:\r\n" +
                "// URL:\r\n" +
                "// LoadExtns: .foo, .bar\r\n" +
                "// SaveExtns: .foo, .bar\r\n" +
                "// Flattened: false\r\n" +
                "#region UICode\r\n" +
                "CheckboxControl Amount1 = false; // Checkbox Description\r\n" +
                "#endregion\r\n" +
                "\r\n" +
                "private const string HeaderSignature = \".PDN\";\r\n" +
                "\r\n" +
                "void SaveImage(Document input, Stream output, PropertyBasedSaveConfigToken token, Surface scratchSurface, ProgressEventHandler progressCallback)\r\n" +
                "{\r\n" +
                "    using (RenderArgs args = new RenderArgs(scratchSurface))\r\n" +
                "    {\r\n" +
                "        // Render a flattened view of the Document to the scratch surface.\r\n" +
                "        input.Render(args, true);\r\n" +
                "    }\r\n" +
                "\r\n" +
                "    if (Amount1)\r\n" +
                "    {\r\n" +
                "        new UnaryPixelOps.Invert().Apply(scratchSurface, scratchSurface.Bounds);\r\n" +
                "    }\r\n" +
                "\r\n" +
                "    // The stream paint.net hands us must not be closed.\r\n" +
                "    using (BinaryWriter writer = new BinaryWriter(output, Encoding.UTF8, leaveOpen: true))\r\n" +
                "    {\r\n" +
                "        // Write the file header.\r\n" +
                "        writer.Write(Encoding.ASCII.GetBytes(HeaderSignature));\r\n" +
                "        writer.Write(scratchSurface.Width);\r\n" +
                "        writer.Write(scratchSurface.Height);\r\n" +
                "\r\n" +
                "        for (int y = 0; y < scratchSurface.Height; y++)\r\n" +
                "        {\r\n" +
                "            // Report progress if the callback is not null.\r\n" +
                "            if (progressCallback != null)\r\n" +
                "            {\r\n" +
                "                double percent = (double)y / scratchSurface.Height;\r\n" +
                "\r\n" +
                "                progressCallback(null, new ProgressEventArgs(percent));\r\n" +
                "            }\r\n" +
                "\r\n" +
                "            for (int x = 0; x < scratchSurface.Width; x++)\r\n" +
                "            {\r\n" +
                "                // Write the pixel values.\r\n" +
                "                ColorBgra color = scratchSurface[x, y];\r\n" +
                "\r\n" +
                "                writer.Write(color.Bgra);\r\n" +
                "            }\r\n" +
                "        }\r\n" +
                "    }\r\n" +
                "}\r\n" +
                "\r\n" +
                "Document LoadImage(Stream input)\r\n" +
                "{\r\n" +
                "    Document doc = null;\r\n" +
                "\r\n" +
                "    // The stream paint.net hands us must not be closed.\r\n" +
                "    using (BinaryReader reader = new BinaryReader(input, Encoding.UTF8, leaveOpen: true))\r\n" +
                "    {\r\n" +
                "        // Read and validate the file header.\r\n" +
                "        byte[] headerSignature = reader.ReadBytes(4);\r\n" +
                "\r\n" +
                "        if (Encoding.ASCII.GetString(headerSignature) != HeaderSignature)\r\n" +
                "        {\r\n" +
                "            throw new FormatException(\"Invalid file signature.\");\r\n" +
                "        }\r\n" +
                "\r\n" +
                "        int width = reader.ReadInt32();\r\n" +
                "        int height = reader.ReadInt32();\r\n" +
                "\r\n" +
                "        // Create a new Document.\r\n" +
                "        doc = new Document(width, height);\r\n" +
                "\r\n" +
                "        // Create a background layer.\r\n" +
                "        BitmapLayer layer = Layer.CreateBackgroundLayer(width, height);\r\n" +
                "\r\n" +
                "        for (int y = 0; y < height; y++)\r\n" +
                "        {\r\n" +
                "            for (int x = 0; x < width; x++)\r\n" +
                "            {\r\n" +
                "                // Read the pixel values from the file.\r\n" +
                "                uint bgraColor = reader.ReadUInt32();\r\n" +
                "\r\n" +
                "                layer.Surface[x, y] = ColorBgra.FromUInt32(bgraColor);\r\n" +
                "            }\r\n" +
                "        }\r\n" +
                "\r\n" +
                "        // Add the new layer to the Document.\r\n" +
                "        doc.Layers.Add(layer);\r\n" +
                "    }\r\n" +
                "\r\n" +
                "    return doc;\r\n" +
                "}\r\n" +
                "\r\n";

            txtCode.EmptyUndoBuffer();
            Build();
            txtCode.Focus();
        }

        private void CreateNewShape()
        {
            NewShape newShape = new NewShape();
            if (newShape.ShowDialog() == DialogResult.OK)
            {
                FileName = "Untitled";
                FullScriptPath = string.Empty;

                tabStrip1.NewTab(FileName, FullScriptPath, ProjectType.Shape);

                txtCode.Text = newShape.ShapeCode;
                txtCode.EmptyUndoBuffer();
                Build();
            }
            newShape.Dispose();

            txtCode.Focus();
        }

        private void Open()
        {
            OpenFileDialog ofd = new OpenFileDialog
            {
                Title = "Load User Script",
                DefaultExt = ".cs",
                Filter = "All Support Files (*.CS; *.XAML; *.TXT;)|*.cs;*.xaml;*.txt;|C# Code Files (*.CS)|*.cs|XAML Shape Files (*.XAML)|*.xaml|Plain Text Files (*.TXT)|*.txt",
                Multiselect = false,
                InitialDirectory = Settings.LastSourceDirectory
            };

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                Settings.LastSourceDirectory = Path.GetDirectoryName(ofd.FileName);

                LoadFile(ofd.FileName);
            }

            ofd.Dispose();

            txtCode.Focus();
            Build();
        }

        private void Save()
        {
            if (SaveFile())
            {
                txtCode.SetSavePoint();
            }
            txtCode.Focus();
        }

        private void SaveAs()
        {
            if (SaveAsFile())
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
                FlexibleMessageBox.Show("Before you can build a DLL, you must resolve all code errors.", "Build Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string fileName = FileName.Trim();
            if (fileName.Length == 0 || fileName == "Untitled")
            {
                FlexibleMessageBox.Show("Before you can build a DLL, you must first save your source file using the File > Save as... menu.", "Build Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            bool buildSucceeded = false;
#if FASTDEBUG
                    const bool isClassic = false;
#else
            bool isClassic = this.Services.GetService<IAppInfoService>().InstallType == AppInstallType.Classic;
#endif

            switch (tabStrip1.SelectedTabProjType)
            {
                case ProjectType.Effect:
                    BuildForm buildForm = new BuildForm(fileName, txtCode.Text, FullScriptPath, isClassic);
                    if (buildForm.ShowDialog() != DialogResult.OK)
                    {
                        return;
                    }

                    buildSucceeded = ScriptBuilder.BuildEffectDll(
                        txtCode.Text, FullScriptPath, buildForm.SubMenu, buildForm.MenuItemName, buildForm.IconPath, buildForm.Author, buildForm.MajorVer, buildForm.MinorVer, buildForm.URL,
                        buildForm.WindowTitle, buildForm.isAdjustment, buildForm.Description, buildForm.KeyWords, buildForm.EffectFlags, buildForm.RenderingSchedule, buildForm.HelpType, buildForm.HelpStr);

                    buildForm.Dispose();
                    break;
                case ProjectType.FileType:
                    BuildFileTypeDialog buildFileTypeDialog = new BuildFileTypeDialog(fileName, txtCode.Text, isClassic);
                    if (buildFileTypeDialog.ShowDialog() != DialogResult.OK)
                    {
                        return;
                    }

                    buildSucceeded = ScriptBuilder.BuildFileTypeDll(txtCode.Text, FullScriptPath, buildFileTypeDialog.Author, buildFileTypeDialog.Major, buildFileTypeDialog.Minor,
                        buildFileTypeDialog.URL, buildFileTypeDialog.Description, buildFileTypeDialog.LoadExt, buildFileTypeDialog.SaveExt, buildFileTypeDialog.Layers, buildFileTypeDialog.PluginName);

                    buildFileTypeDialog.Dispose();
                    break;
            }

            if (buildSucceeded)
            {
                string fullPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                fullPath = Path.Combine(fullPath, fileName);
                string zipPath = Path.ChangeExtension(fullPath, ".zip");

                string succeeded = "Build succeeded!\r\n\r\n" +
                    "\"" + fileName + ".zip\" has been created on your Desktop.\r\n" +
                    zipPath + "\r\n\r\n" +
                    "You will need to right-click 'Extract All...' the Zip to run the install.bat file\r\n" +
                    "Restart Paint.NET to make the plugin available.";

                FlexibleMessageBox.Show(succeeded, "Build Finished", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                DisplayErrors();
                string error = (ScriptBuilder.Exception != null) ? ScriptBuilder.Exception.InsertLineBreaks(100) : "Please check for build errors in the Errors List.";
                FlexibleMessageBox.Show("I'm sorry, I was not able to build the DLL.\r\n\r\n" + error, "Build Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UIDesigner()
        {
            // User Interface Designer
            UIBuilder myUIBuilderForm = new UIBuilder(txtCode.Text, tabStrip1.SelectedTabProjType, ColorBgra.Black);  // This should be the current Primary color
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

        private void FormatDocument()
        {
            txtCode.FormatDocument();
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
#if FASTDEBUG
            return;
#endif
            double SaveOpacitySetting = Opacity;
            Opacity = 0;
            tmrCompile.Enabled = false;
            Build();

            switch (this.tabStrip1.SelectedTabProjType)
            {
                case ProjectType.Effect:
                    RunEffectWithDialog();
                    break;
                case ProjectType.FileType:
                    RunFileTypeWithDialog();
                    break;
            }

            tmrCompile.Enabled = true;
            Opacity = SaveOpacitySetting;
        }
        #endregion

        #region File Menu Event functions
        private void NewEffectMenuItem_Click(object sender, EventArgs e)
        {
            CreateNewEffect();
        }

        private void NewFileTypeMenuItem_Click(object sender, EventArgs e)
        {
            CreateNewFileType();
        }

        private void NewShapeMenuItem_Click(object sender, EventArgs e)
        {
            CreateNewShape();
        }

        private void NewTextMenuItem_Click(object sender, EventArgs e)
        {
            CreateNewPlainText();
        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Open();
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
            bool hasText = txtCode.TextLength > 0;
            bool isTextSelected = hasText && txtCode.SelectedText.Length > 0;
            bool cSharp = tabStrip1.SelectedTabProjType == ProjectType.Effect || tabStrip1.SelectedTabProjType == ProjectType.FileType;

            this.cutToolStripMenuItem1.Enabled = isTextSelected;
            this.copyToolStripMenuItem1.Enabled = isTextSelected;
            this.selectAllToolStripMenuItem1.Enabled = hasText;
            this.indentToolStripMenuItem.Enabled = true;
            this.unindentToolStripMenuItem.Enabled = true;
            this.pasteToolStripMenuItem1.Enabled = txtCode.CanPaste;
            this.commentSelectionToolStripMenuItem.Enabled = cSharp && hasText;
            this.uncommentSelectionToolStripMenuItem.Enabled = cSharp && hasText;
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

        private void formatDocMenuItem_Click(object sender, EventArgs e)
        {
            FormatDocument();
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
                txtCode.Height -= toolStrip1.Height;
            }
            else
            {
                Settings.ToolBar = false;
                toolBarToolStripMenuItem.CheckState = CheckState.Unchecked;
                toolStrip1.Visible = false;
                txtCode.Location = new Point(txtCode.Left, tabStrip1.Bottom);
                txtCode.Height += toolStrip1.Height;
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
            bool inverseValue = txtCode.WrapMode == WrapMode.None;
            EnableWordWrap(inverseValue);

            if (tabStrip1.SelectedTabProjType == ProjectType.None)
            {
                Settings.WordWrapPlainText = inverseValue;
            }
            else
            {
                Settings.WordWrap = inverseValue;
            }

            txtCode.Focus();
        }

        private void EnableWordWrap(bool enable)
        {
            wordWrapToolStripMenuItem.CheckState = enable ? CheckState.Checked : CheckState.Unchecked;
            txtCode.WrapMode = enable ? WrapMode.Whitespace : WrapMode.None;
            txtCode.WrapVisualFlags = enable ? WrapVisualFlags.Start : WrapVisualFlags.None;
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

        private void FontMenuItem_Click(object sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem fontMenuItem)
            {
                string fontName = fontMenuItem.Text;

                foreach (ToolStripMenuItem menuItem in fontsToolStripMenuItem.DropDownItems.OfType<ToolStripMenuItem>())
                {
                    menuItem.Checked = menuItem.Text == fontName;
                }

                Settings.FontFamily = fontName;
                txtCode.Styles[Style.Default].Font = fontName;
                txtCode.UpdateMarginWidths();
                OutputTextBox.Font = new Font(fontName, OutputTextBox.Font.Size);
                errorList.Font = new Font(fontName, errorList.Font.Size);

                txtCode.Focus();
            }
        }

        private void HelpWithFonts_Click(object sender, EventArgs e)
        {
            FlexibleMessageBox.Show(
                "You may choose between Consolas, Courier New, and Verdana fonts to view your code.\n\n" +
                "These fonts are built-in to Windows. If they are not available, you may need to download them.\n\n" +
                "Or, you may download and install these popular programming fonts from the following web sites:\n\n" + 
                "▪ Envy Code R\n    http://damieng.com/blog/2008/05/26/envy-code-r-preview-7-coding-font-released \n\n" + 
                "▪ Hack\n    http://sourcefoundry.org/hack/ \n\n" + 
                "The following fonts support Programming Ligatures: (Showing  ≤  instead of  <=  etc.)\n\n" + 
                "▪ Cascadia Code\n    https://devblogs.microsoft.com/commandline/cascadia-code/ \n\n" + 
                "▪ Fira Code\n    https://github.com/tonsky/FiraCode \n\n" + 
                "▪ JetBrains Mono\n    https://www.jetbrains.com/lp/mono/ \n\n" + 
                "Once downloaded and installed on your system, you may choose these fonts from the menu.", 
                "Help With Fonts", MessageBoxButtons.OK,MessageBoxIcon.Information);
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
            txtCode.Theme = PdnTheme.Theme;
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
            LaunchUrl("https://www.BoltBait.com/pdn/codelab/help");
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
            LaunchUrl("https://www.boltbait.com/pdn/codelab/history/#v" + ThisVersion);
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FlexibleMessageBox.Show(WindowTitle + "\nCopyright © 2006-2020, All Rights Reserved.\n\nTom Jackson:\tInitial Code, Compile to DLL\n\nDavid Issel:\tEffect UI Creation, Effect Icons, Effect Help\n\t\tSystem, Editor Enhancements (including\n\t\tFile New Templates, CodeLab Updater,\n\t\tBug Fixes), Coding Tutorials and Installer\n\nJason Wendt:\tMigration to ScintillaNET editor control,\n\t\tIntelligent Assistance (including code\n\t\tcompletion with snippets and tips),\n\t\tDebug Output, Dark Theme, Bug Fixes,\n\t\tHiDPI icons, Live Effect Preview,\n\t\tFiletype editing, and Shape editing\n\nJörg Reichert:\tFlexibleMessageBox", "About CodeLab", MessageBoxButtons.OK, MessageBoxIcon.Information);
            txtCode.Focus();
        }
        #endregion

        #region Context menu Event functions
        private void contextMenuStrip1_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            bool hasText = txtCode.TextLength > 0;
            bool isTextSelected = hasText && txtCode.SelectedText.Length > 0;
            bool cSharp = tabStrip1.SelectedTabProjType == ProjectType.Effect || tabStrip1.SelectedTabProjType == ProjectType.FileType;

            this.cutToolStripMenuItem.Enabled = isTextSelected;
            this.copyToolStripMenuItem.Enabled = isTextSelected;
            this.selectAllToolStripMenuItem.Enabled = txtCode.TextLength > 0;
            this.searchToolStripMenuItem.Enabled = true;
            this.indentToolStripMenuItem1.Enabled = true;
            this.unindentToolStripMenuItem1.Enabled = true;
            this.pasteToolStripMenuItem.Enabled = txtCode.CanPaste;
            this.commentSelectionToolStripMenuItem1.Enabled = cSharp && hasText;
            this.uncommentSelectionToolStripMenuItem1.Enabled = cSharp && hasText;
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
            CreateNewEffect();
        }

        private void OpenButton_Click(object sender, EventArgs e)
        {
            Open();
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

        private void FormatDocButton_Click(object sender, EventArgs e)
        {
            FormatDocument();
        }

        private void FindButton_Click(object sender, EventArgs e)
        {
            FindCommand();
        }

        private void ReplaceButton_Click(object sender, EventArgs e)
        {
            ReplaceCommand();
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

        private void SnippetManButton_Click(object sender, EventArgs e)
        {
            using (SnippetManager snippetManager = new SnippetManager())
            {
                snippetManager.ShowDialog();
            }
        }
        #endregion

        #region Recent Items functions
        private void AddToRecents(string filePath)
        {
            string recents = Settings.RecentDocs;

            if (recents.Length == 0)
            {
                recents = filePath;
            }
            else
            {
                recents = filePath + "|" + recents;

                string[] paths = recents.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries).Distinct().ToArray();

                int length = Math.Min(8, paths.Length);
                recents = string.Join("|", paths, 0, length);
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
                    Match wtn = Regex.Match(File.ReadAllText(itemPath), @"//[\s-[\r\n]]*Name[\s-[\r\n]]*:[\s-[\r\n]]*(?<menulabel>.*)(?=\r?\n|$)", RegexOptions.IgnoreCase);
                    if (wtn.Success)
                    {
                        string scriptName = wtn.Groups["menulabel"].Value.Trim();
                        if (scriptName.Length > 0)
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
                    recentItem.Image = ResUtil.GetBitmapFromFile(imagePath);
                }

                recentsList.Add(recentItem);
                count++;
            }

            if (recentsList.Count > 0)
            {
                ToolStripSeparator toolStripSeparator = new ToolStripSeparator();
                recentsList.Add(toolStripSeparator);

                ToolStripMenuItem clearRecents = new ToolStripMenuItem
                {
                    Text = "&Clear List"
                };
                clearRecents.Click += ClearRecents_Click;
                recentsList.Add(clearRecents);

                this.openRecentToolStripMenuItem.DropDownItems.AddRange(recentsList.ToArray());
            }
            else
            {
                ToolStripMenuItem noRecents = new ToolStripMenuItem
                {
                    Text = "No Recent Items",
                    Enabled = false
                };

                this.openRecentToolStripMenuItem.DropDownItems.Add(noRecents);
            }
        }

        private void ClearRecents_Click(object sender, EventArgs e)
        {
            if (FlexibleMessageBox.Show("Are you sure you want to clear the Open Recent list?", "CodeLab", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Settings.RecentDocs = string.Empty;
            }
        }

        private void RecentItem_Click(object sender, EventArgs e)
        {
            string filePath = (sender as ToolStripMenuItem)?.ToolTipText;
            if (!File.Exists(filePath))
            {
                FlexibleMessageBox.Show("File not found.\n" + filePath, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtCode.Focus();
                return;
            }

            LoadFile(filePath);
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
            ProjectType projectType = tabStrip1.SelectedTabProjType;
            UpdateWindowTitle();
            txtCode.SwitchToDocument(tabStrip1.SelectedTabGuid);
            EnableWordWrap(projectType == ProjectType.None ? Settings.WordWrapPlainText : Settings.WordWrap);
            UpdateToolBarButtons();

            if (projectType == ProjectType.Effect ||
                projectType == ProjectType.FileType)
            {
                EnableCSharpButtons(true);
                BuildAsync();
            }
            else
            {
                EnableCSharpButtons(false);
                Build();
            }
        }

        private void tabStrip1_NewTabCreated(object sender, EventArgs e)
        {
            txtCode.CreateNewDocument(tabStrip1.SelectedTabGuid, tabStrip1.SelectedTabProjType);
            EnableWordWrap(tabStrip1.SelectedTabProjType == ProjectType.None ? Settings.WordWrapPlainText : Settings.WordWrap);
            UpdateWindowTitle();
            UpdateToolBarButtons();
            EnableCSharpButtons(tabStrip1.SelectedTabProjType == ProjectType.Effect || tabStrip1.SelectedTabProjType == ProjectType.FileType);
        }

        private void EnableCSharpButtons(bool enable)
        {
            SaveDLLButton.Enabled = enable;
            saveAsDLLToolStripMenuItem.Enabled = enable;
            UIDesignerButton.Enabled = enable;
            userInterfaceDesignerToolStripMenuItem.Enabled = enable;
            RunButton.Enabled = enable;
            previewEffectMenuItem.Enabled = enable;
            FormatDocButton.Enabled = enable;
            formatDocMenuItem.Enabled = enable;
            SnippetManButton.Enabled = enable;
            CommentButton.Enabled = enable;
            commentSelectionToolStripMenuItem.Enabled = enable;
            commentSelectionToolStripMenuItem1.Enabled = enable;
            UnCommentButton.Enabled = enable;
            uncommentSelectionToolStripMenuItem.Enabled = enable;
            uncommentSelectionToolStripMenuItem1.Enabled = enable;
        }

        private void tabStrip1_TabClosingAndDirty(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (PromptToSave() == DialogResult.Cancel)
            {
                e.Cancel = true;
            }
        }

        private void txtCode_DefTabNeeded(object sender, NewTabEventArgs e)
        {
            FileName = e.Name;
            FullScriptPath = string.Empty;

            tabStrip1.NewTab(FileName, FullScriptPath, ProjectType.Reference);
        }

        private void tabStrip1_TabClosed(object sender, TabClosedEventArgs e)
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
            this.Text = FileName + " - " + WindowTitle + this.EffectFlag;
        }
        #endregion

        #region Drag-and-Drop
        protected override void OnDragEnter(DragEventArgs drgevent)
        {
            base.OnDragEnter(drgevent);

            if (drgevent.Data.GetDataPresent(DataFormats.FileDrop))
            {
                drgevent.Effect = DragDropEffects.Copy;
            }
        }

        protected override void OnDragDrop(DragEventArgs drgevent)
        {
            base.OnDragDrop(drgevent);

            if (drgevent.Data.GetDataPresent(DataFormats.FileDrop) &&
                drgevent.Data.GetData(DataFormats.FileDrop) is string[] droppedFiles)
            {
                foreach (string filePath in droppedFiles)
                {
                    string fileExt = Path.GetExtension(filePath);
                    if (File.Exists(filePath) &&
                        (fileExt.Equals(".cs", StringComparison.OrdinalIgnoreCase) ||
                        fileExt.Equals(".txt", StringComparison.OrdinalIgnoreCase) ||
                        fileExt.Equals(".xaml", StringComparison.OrdinalIgnoreCase)))
                    {
                        LoadFile(filePath);
                    }
                }

                txtCode.Focus();
                Build();
            }
        }
        #endregion
    }

    public enum ProjectType
    {
        None,
        Effect,
        FileType,
        Reference,
        Shape
    }
}
