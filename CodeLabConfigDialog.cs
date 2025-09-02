/////////////////////////////////////////////////////////////////////////////////
// CodeLab for Paint.NET
// Copyright ©2006 Rick Brewster, Tom Jackson. All Rights Reserved.
// Portions Copyright ©2007-2024 BoltBait. All Rights Reserved.
// Portions Copyright ©2016-2024 Jason Wendt. All Rights Reserved.
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

using PaintDotNet;
using PaintDotNet.AppModel;
using PaintDotNet.Clipboard;
using PaintDotNet.Effects;
using PaintDotNet.Effects.Gpu;
using ScintillaNET;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Document = PaintDotNet.Document;

namespace PdnCodeLab
{
    internal partial class CodeLabConfigDialog
#if FASTDEBUG
        : PdnBaseForm, IToolTipHost
#else
        : EffectConfigForm2, IToolTipHost
#endif
    {
        #region Constructor
#if RELEASE
        private const string WindowTitle = "CodeLab v" + CodeLab.Version;
#else
        private const string WindowTitle = "CodeLab Debug";
#endif
        private EffectConfigToken previewToken = null;
        private readonly RenderPreset renderPreset;
        private const string showErrorList = "Show Error List";

        public CodeLabConfigDialog(RenderPreset renderPreset)
        {
            Task.Run(() => Intelli.Keywords); // Forces the Intelli class to start initializing in the background
            InitializeComponent();

            FormBorderStyle = FormBorderStyle.Sizable;
            MaximizeBox = true;

#if FASTDEBUG
            this.Icon = UIUtil.CreateIcon("CodeLab");
            this.ShowInTaskbar = true;
            this.UseAppThemeColors = true;
            UpdateWindowTitle();
#endif
            ThemeUtil.Initialize(IsAppThemeDark, this.ForeColor, this.BackColor, Settings.EditorTheme);
            this.UpdateTheme();

            LoadSettingsFromRegistry();

            this.Opacity = 1.00;
            opacity50MenuItem.Checked = false;
            opacity75MenuItem.Checked = false;
            opacity90MenuItem.Checked = false;
            opacity100MenuItem.Checked = true;
            transparencyToolStripMenuItem.Enabled = EnableOpacity;

#if !RELEASE
            ScaledToolStripButton debugButton = new ScaledToolStripButton();
            debugButton.ImageName = "InsertPicture";
            debugButton.Click += (object sender, EventArgs e) =>
            {
                // test code here
                ProjectType projType = tabStrip1.SelectedTabProjType;
                if (!projType.IsCSharp())
                {
                    return;
                }

                string userCode = txtCode.Text;
                bool debugMode = OutputTextBox.Visible;

                string runCode = projType switch
                {
                    ProjectType.BitmapEffect => BitmapEffectWriter.Run(userCode, debugMode),
                    ProjectType.GpuImageEffect => GPUEffectWriter.Run(userCode, debugMode),
                    ProjectType.GpuDrawEffect => GPUDrawWriter.Run(userCode, debugMode),
                    ProjectType.FileType => FileTypeWriter.Run(userCode, debugMode),
                    _ => string.Empty,
                };

                using ViewSrc viewSrc = new ViewSrc("Run Code", runCode, true, true);
                viewSrc.ShowDialog();
            };

            toolStrip1.Items.Add(new ToolStripSeparator());
            toolStrip1.Items.Add(debugButton);
#endif

            this.renderPreset = renderPreset;
        }

#if !FASTDEBUG
        protected override void OnLoading()
        {
            base.OnLoading();

            UIUtil.SetIShellService(this.Services.GetService<IShellService>());
        }
#else
        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            this.txtCode.Text = DefaultCode.Default;
        }
#endif

        private void LoadSettingsFromRegistry()
        {
            toolStrip1.Visible = Settings.ToolBar;

            SetBottomPanes(Settings.ErrorBox, Settings.Output);

            this.UpdateTheme(Settings.EditorTheme);
            if (errorList.HasErrors)
            {
                ShowErrors.ForeColor = Color.Red;
            }

            txtCode.ApplyUserSettings();

            if (Settings.CheckForUpdates)
            {
                Freshness.GoCheckForUpdates(true, false);
            }

            string editorFont = Settings.FontFamily;
            OutputTextBox.Font = new Font(editorFont, OutputTextBox.Font.Size);
            errorList.Font = new Font(editorFont, errorList.Font.Size);

            ScriptBuilder.SetWarningLevel(Settings.WarningLevel);
            ScriptBuilder.SetWarningsToIgnore(Settings.WarningsToIgnore);
        }
        #endregion

        #region Token functions
#if !FASTDEBUG
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new CodeLabConfigToken Token
        {
            get => (CodeLabConfigToken)base.Token;

            set => base.Token = value;
        }

        protected override void OnUpdateTokenFromDialog(EffectConfigToken dstToken)
        {
            OnUpdateTokenFromDialog((CodeLabConfigToken)dstToken);
        }

        private void OnUpdateTokenFromDialog(CodeLabConfigToken dstToken)
        {
            dstToken.UserCode = txtCode.Text;
            dstToken.UserScriptObject = ScriptBuilder.BuiltEffect;
            dstToken.ScriptName = tabStrip1.SelectedTabTitle;
            dstToken.ScriptPath = tabStrip1.SelectedTabPath;
            dstToken.Dirty = tabStrip1.SelectedTabIsDirty;
            dstToken.PreviewToken = previewToken;
            dstToken.Bookmarks = txtCode.Bookmarks;
            dstToken.ProjectType = tabStrip1.SelectedTabProjType;
        }

        protected override void OnUpdateDialogFromToken(EffectConfigToken token)
        {
            OnUpdateDialogFromToken((CodeLabConfigToken)token);
        }

        private void OnUpdateDialogFromToken(CodeLabConfigToken token)
        {
            if (token.ProjectType != ProjectType.Default)
            {
                tabStrip1.SelectedTabIsDirty = false;
                tabStrip1.NewTab(token.ScriptName, token.ScriptPath, token.ProjectType);
                tabStrip1.CloseFirstTab();
            }
            else if (token.ProjectType.IsCSharp())
            {
                Intelli.SetReferences(token.ProjectType);
            }
            txtCode.Text = token.UserCode;
            txtCode.EmptyUndoBuffer();
            if (!token.Dirty)
            {
                txtCode.SetSavePoint();
            }
            txtCode.Bookmarks = token.Bookmarks;

            UpdateWindowTitle();

            BuildAsync();
            txtCode.Focus();
        }

        protected override EffectConfigToken OnCreateInitialToken()
        {
            return new CodeLabConfigToken
            {
                UserCode = DefaultCode.Default,
                UserScriptObject = null,
                ScriptName = "Untitled",
                ScriptPath = "",
                Dirty = false,
                PreviewToken = null,
                Bookmarks = Array.Empty<int>()
            };
        }
#else
        private void UpdateTokenFromDialog()
        {
        }
#endif
        #endregion

        #region Build Script actions
        private void Build()
        {
            ClearErrorList();

            ProjectType projType = tabStrip1.SelectedTabProjType;
            if (projType == ProjectType.PlainText || projType == ProjectType.Reference)
            {
                return;
            }

            string userCode = txtCode.Text;
            bool debugMode = OutputTextBox.Visible;

            switch (projType)
            {
                case ProjectType.BitmapEffect:
                    string bitmapSourceCode = BitmapEffectWriter.Run(userCode, debugMode);
                    ScriptBuilder.BuildEffect<BitmapEffect>(bitmapSourceCode, debugMode);
                    DisplayErrors();
                    txtCode.UpdateSyntaxHighlighting();
                    UpdateTokenFromDialog();
                    break;
                case ProjectType.GpuImageEffect:
                    string gpuSourceCode = GPUEffectWriter.Run(userCode, debugMode);
                    ScriptBuilder.BuildEffect<GpuImageEffect>(gpuSourceCode, debugMode);
                    DisplayErrors();
                    txtCode.UpdateSyntaxHighlighting();
                    UpdateTokenFromDialog();
                    break;
                case ProjectType.GpuDrawEffect:
                    string gpuDrawSourceCode = GPUDrawWriter.Run(userCode, debugMode);
                    ScriptBuilder.BuildEffect<GpuDrawingEffect>(gpuDrawSourceCode, debugMode);
                    DisplayErrors();
                    txtCode.UpdateSyntaxHighlighting();
                    UpdateTokenFromDialog();
                    break;
                case ProjectType.FileType:
                    string fileTypeSourceCode = FileTypeWriter.Run(userCode, debugMode);
                    ScriptBuilder.BuildFileType(fileTypeSourceCode, debugMode);
                    DisplayErrors();
                    txtCode.UpdateSyntaxHighlighting();
                    RunFileType();
                    break;
                case ProjectType.Shape:
                    ShapeBuilder.TryParseShapeCode(userCode);
                    DisplayErrors();
                    UpdateTokenFromDialog();
                    break;
            }
        }

        private async Task BuildAsync()
        {
            ProjectType projType = tabStrip1.SelectedTabProjType;
            if (projType == ProjectType.PlainText || projType == ProjectType.Shape)
            {
                return;
            }

            tmrCompile.Enabled = false;
            string userCode = txtCode.Text;
            bool debugMode = OutputTextBox.Visible;
            await Task.Run(() =>
            {
                switch (projType)
                {
                    case ProjectType.BitmapEffect:
                        string bitmapSourceCode = BitmapEffectWriter.Run(userCode, debugMode);
                        ScriptBuilder.BuildEffect<BitmapEffect>(bitmapSourceCode, debugMode);
                        break;
                    case ProjectType.GpuImageEffect:
                        string gpuSourceCode = GPUEffectWriter.Run(userCode, debugMode);
                        ScriptBuilder.BuildEffect<GpuImageEffect>(gpuSourceCode, debugMode);
                        break;
                    case ProjectType.GpuDrawEffect:
                        string gpuDrawSourceCode = GPUDrawWriter.Run(userCode, debugMode);
                        ScriptBuilder.BuildEffect<GpuDrawingEffect>(gpuDrawSourceCode, debugMode);
                        break;
                    case ProjectType.FileType:
                        string fileTypeSourceCode = FileTypeWriter.Run(userCode, debugMode);
                        ScriptBuilder.BuildFileType(fileTypeSourceCode, debugMode);
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
                case ProjectType.GpuImageEffect:
                case ProjectType.GpuDrawEffect:
                case ProjectType.BitmapEffect:
                    txtCode.UpdateSyntaxHighlighting();
                    UpdateTokenFromDialog();
                    break;
                case ProjectType.FileType:
                    txtCode.UpdateSyntaxHighlighting();
                    RunFileType();
                    break;
            }

            tmrCompile.Enabled = true;
        }

        private void RunEffectWithDialog(ProjectType projectType)
        {
            if (errorList.HasErrors)
            {
                FlexibleMessageBox.Show("Before you can preview your Effect, you must resolve all code errors.", "Build Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            bool built = false;
            switch (projectType)
            {
                case ProjectType.BitmapEffect:
                    string bitmapSourceCode = BitmapEffectWriter.FullPreview(txtCode.Text);
                    built = ScriptBuilder.BuildEffect<BitmapEffect>(bitmapSourceCode);
                    break;
                case ProjectType.GpuImageEffect:
                    string gpuSourceCode = GPUEffectWriter.FullPreview(txtCode.Text);
                    built = ScriptBuilder.BuildEffect<GpuImageEffect>(gpuSourceCode);
                    break;
                case ProjectType.GpuDrawEffect:
                    string gpuDrawSourceCode = GPUDrawWriter.FullPreview(txtCode.Text);
                    built = ScriptBuilder.BuildEffect<GpuDrawingEffect>(gpuDrawSourceCode);
                    break;
            }

            if (!built)
            {
                FlexibleMessageBox.Show("Something went wrong, and the Preview can't be run.", "Preview Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                DisplayErrors();
                return;
            }

            if (!ScriptBuilder.BuiltEffect.Options.IsConfigurable)
            {
                FlexibleMessageBox.Show("There are no UI controls, so the Preview can't be displayed.", "Preview Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

#if !FASTDEBUG
            using IEffect effect = ScriptBuilder.BuiltEffect.EffectInfo.CreateInstance(new ServiceProviderWrapper(this.Services), this.Environment);
            using IEffectConfigForm previewDialog = effect.CreateConfigForm();
            {
                previewToken = previewDialog.Token;
                previewDialog.TokenChanged += (sender, e) => UpdateTokenFromDialog();

                previewDialog.ShowDialog(this);
            }
#endif

            previewToken = null;
            Build();
        }

        private void RunFileTypeWithDialog()
        {
            if (errorList.HasErrors)
            {
                FlexibleMessageBox.Show("Before you can preview your FileType, you must resolve all code errors.", "Build Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!ScriptBuilder.BuiltFileType.SupportsConfiguration)
            {
                const string noControls = "There are no UI controls!\r\n\r\n" +
                    "Be aware that FileTypes with no controls will not display a Save Configuration dialog in Paint.NET.\r\n\r\n" +
                    "However, a Save Configuration dialog will still display here in CodeLab for testing/debugging purposes.\r\n" +
                    "It contains an image preview, which can provide visual feedback if the FileType is working correctly.";

                FlexibleMessageBox.Show(noControls, "Preview Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            using Surface srcSurface = new Surface(800, 600);
            using Graphics g = new RenderArgs(srcSurface).Graphics;
            using System.Drawing.Drawing2D.LinearGradientBrush gradientBrush = new System.Drawing.Drawing2D.LinearGradientBrush(srcSurface.Bounds, Color.Black, Color.White, 0f);
            g.FillRectangle(gradientBrush, srcSurface.Bounds);

            using SaveConfigDialog saveDialog = new SaveConfigDialog(ScriptBuilder.BuiltFileType, srcSurface);
            saveDialog.ShowDialog();
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
                    errorList.AddError(Error.NewExceptionError(ex));
                    ShowErrors.Text = $"{showErrorList} ({errorList.ErrorCount})";
                    ShowErrors.ForeColor = Color.Red;
                }

                document.Layers.DisposeAll();
            }
        }

        private void DisplayErrors()
        {
            ClearErrorList();
            txtCode.ClearErrors();

            ProjectType projectType = tabStrip1.SelectedTabProjType;
            if (projectType.IsCSharp() && ScriptBuilder.Errors.Count > 0)
            {
                bool projectTypeMismatch = ScriptBuilder.Errors
                    .Any(x =>
                    {
                        return
                            (x.ErrorNumber.Equals("CS0534", StringComparison.Ordinal) || x.ErrorNumber.Equals("CS0115", StringComparison.Ordinal)) &&
                            (x.ErrorText.Contains(".OnDraw(IDeviceContext)", StringComparison.Ordinal) ||
                            x.ErrorText.Contains(".OnCreateOutput(IDeviceContext)", StringComparison.Ordinal) ||
                            x.ErrorText.Contains(".OnRender(IBitmapEffectOutput)", StringComparison.Ordinal));
                    });

                if (projectTypeMismatch)
                {
                    ProjectType detectedProjectType = ProjectTypeUtil.FromContents(txtCode.Text, null);
                    if (detectedProjectType != projectType && detectedProjectType.IsCSharp())
                    {
                        tabStrip1.SelectedTabProjType = detectedProjectType;
                        UpdateForProjectType();
                        FlexibleMessageBox.Show($"Project Type has been automatically changed from {projectType} to {detectedProjectType}.", "Project Type Mismatch", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }
                }

                errorList.AddErrors(ScriptBuilder.Errors);
                txtCode.AddErrors(ScriptBuilder.Errors);
            }
            else if (projectType == ProjectType.Shape && ShapeBuilder.Error != null)
            {
                errorList.AddError(ShapeBuilder.Error);
            }

            txtCode.UpdateIndicatorBar();

            int errorCount = errorList.ErrorCount;
            if (errorCount > 0)
            {
                ShowErrors.Text = $"{showErrorList} ({errorCount})";
                ShowErrors.ForeColor = Color.Red;
            }
        }

        private void ClearErrorList()
        {
            errorList.ClearErrors();
            toolTips.SetToolTip(errorList, "");

            ShowErrors.Text = showErrorList;
            ShowErrors.ForeColor = this.ForeColor;
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

            ProjectType projType = ProjectTypeUtil.FromContents(fileContents, Path.GetExtension(filePath));

            bool removedInitTab = tabStrip1.SelectedTabIsInitial && txtCode.IsVirgin;

            tabStrip1.NewTab(Path.GetFileNameWithoutExtension(filePath), filePath, projType);

            if (removedInitTab)
            {
                tabStrip1.CloseFirstTab();
            }

            txtCode.Text = fileContents;
            txtCode.EmptyUndoBuffer();
            txtCode.SetSavePoint();
        }

        private DialogResult PromptToSave()
        {
            int buttonPressed = (int)FlexibleMessageBox.Show(this, $"Do you want to save changes to '{tabStrip1.SelectedTabTitle}'?", "Unsaved Changes", new string[] { "Save", "Discard", "Cancel" }, MessageBoxIcon.Question);
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
            string filePath = tabStrip1.SelectedTabPath;
            if (filePath.IsNullOrEmpty() || !File.Exists(filePath))
            {
                return SaveAsFile();
            }

            bool saved = false;
            try
            {
                File.WriteAllText(filePath, txtCode.Text);
                saved = true;
            }
            catch
            {
            }

            return saved;
        }

        private bool SaveAsFile()
        {
            string fileName = tabStrip1.SelectedTabTitle;
            string fileExt;
            string description;

            ProjectType projectType = tabStrip1.SelectedTabProjType;
            switch (projectType)
            {
                case ProjectType.PlainText:
                    fileExt = ".txt";
                    description = "Plain Text";
                    break;
                case ProjectType.Shape:
                    fileExt = ".xaml";
                    description = "XAML Shape";
                    break;
                default:
                    fileExt = ".cs";
                    description = "C# Code";
                    break;
            }

            if (fileName.Equals("Untitled", StringComparison.Ordinal))
            {
                fileName = TryExtractName(txtCode.Text, projectType, out string extractedName)
                    ? extractedName
                    : $"My{projectType}";
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
                if (projectType.IsCSharp() && !char.IsLetter(Path.GetFileName(sfd.FileName), 0))
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
                    Settings.LastSourceDirectory = Path.GetDirectoryName(sfd.FileName);
                    tabStrip1.SelectedTabPath = sfd.FileName;
                    tabStrip1.SelectedTabTitle = Path.GetFileNameWithoutExtension(sfd.FileName);
                    UpdateWindowTitle();
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

            ErrorCodeMenuItem.Visible = errorList.SelectedError.ErrorUrl?.Length > 0;
            ignoreWarningMenuItem.Visible = errorList.SelectedError.IsWarning;
        }

        private void CopyErrorMenuItem_Click(object sender, EventArgs e)
        {
            if (errorList.SelectedIndex > -1)
            {
                string errorMsg = errorList.SelectedError.ErrorText;
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
                using (ViewSrc VSW = new ViewSrc("Full Error Message", errorList.SelectedError.FullError, false))
                {
                    VSW.ShowDialog();
                }
            }
        }

        private void ErrorCodeMenuItem_Click(object sender, EventArgs e)
        {
            if (errorList.SelectedIndex > -1)
            {
                string url = errorList.SelectedError.ErrorUrl;
                LaunchUrl(url);
            }
        }

        private void ignoreWarningMenuItem_Click(object sender, EventArgs e)
        {
            if (errorList.SelectedIndex > -1)
            {
                IEnumerable<string> warnings = Settings.WarningsToIgnore.Append(errorList.SelectedError.ErrorNumber);
                Settings.WarningsToIgnore = warnings;
                ScriptBuilder.SetWarningsToIgnore(warnings);
                BuildAsync();
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
                using (ViewSrc VSW = new ViewSrc("Full Error Message", errorList.SelectedError.FullError, false))
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
            if (errorList.SelectedIndex < 0)
            {
                return;
            }

            Error error = errorList.SelectedError;
            if (error.StartLine >= 0)
            {
                txtCode.SetEmptySelection(txtCode.Lines[error.StartLine].Position + error.StartColumn);
                txtCode.Lines[error.StartLine].EnsureVisible();
                txtCode.ScrollCaret();    // Make error visible by scrolling to it
                txtCode.Focus();
            }

            toolTips.SetToolTip(errorList, error.FullError.InsertLineBreaks(100));
        }
        #endregion

        #region Timer tick Event functions
        private void tmrExceptionCheck_Tick(object sender, EventArgs e)
        {
#if !FASTDEBUG
            CodeLabConfigToken sect = Token;

            if (Token.LastExceptions.Count > 0)
            {
                Error exc = Error.NewExceptionError(sect.LastExceptions[0]);
                Token.LastExceptions.Clear();

                errorList.AddError(exc);
                ShowErrors.Text = $"{showErrorList} ({errorList.ErrorCount})";
                ShowErrors.ForeColor = Color.Red;
            }

            if (Token.Output.Count > 0)
            {
                string output = Token.Output[0];
                Token.Output.Clear();

                if (output.Trim().Length > 0)
                {
                    OutputTextBox.AppendText(output);
                }
            }
#endif
        }

        private void tmrCompile_Tick(object sender, EventArgs e)
        {
            tmrCompile.Enabled = false;
            DisplayUpdates();
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

        protected override void OnHelpRequested(HelpEventArgs hevent)
        {
            // no-op
            // Prevents PDN Docs from opening when pressing F1
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
                case (Keys.Control | Keys.N):
                    fileToolStripMenuItem.ShowDropDown();
                    newToolStripMenuItem.ShowDropDown();
                    NewGpuEffectMenuItem.Select();
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
            if (e.KeyCode == Keys.F6)
            {
                btnBuild_Click(sender, EventArgs.Empty);
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

        private void LaunchUrl(string url)
        {
            UIUtil.LaunchUrl(this, url);
        }

        void IToolTipHost.ThemeToolTip()
        {
            toolTips.UpdateTheme();
        }
        #endregion

        #region Freshness Check functions
        private void DisplayUpdates()
        {
            if (txtCode.Focused)
            {
                Freshness.DisplayUpdateNotification();
            }
        }
        #endregion

        #region Common functions for button/menu events
        private void CreateNewBitmapEffect()
        {
            CreateNewProjectTab(ProjectType.BitmapEffect);
        }

        private void CreateNewGPUEffect()
        {
            CreateNewProjectTab(ProjectType.GpuImageEffect);
        }

        private void CreateNewGPUDrawEffect()
        {
            CreateNewProjectTab(ProjectType.GpuDrawEffect);
        }

        private void CreateNewPlainText()
        {
            CreateNewProjectTab(ProjectType.PlainText);
        }

        private void CreateNewFileType()
        {
            CreateNewProjectTab(ProjectType.FileType);
        }

        private void CreateNewShape()
        {
            using NewShape newShape = new NewShape();
            if (newShape.ShowDialog() == DialogResult.OK)
            {
                CreateNewProjectTab(ProjectType.Shape, newShape.ShapeCode);
            }
        }

        private void CreateNewProjectTab(ProjectType projectType)
        {
            CreateNewProjectTab(projectType, DefaultCode.ForProjectType(projectType));
        }

        private void CreateNewProjectTab(ProjectType projectType, string userCode)
        {
            bool removedInitTab = tabStrip1.TabCount == 1 && tabStrip1.SelectedTabIsInitial && txtCode.IsVirgin;

            tabStrip1.NewTab("Untitled", string.Empty, projectType);

            if (removedInitTab)
            {
                tabStrip1.CloseFirstTab();
            }

            txtCode.Text = userCode;
            txtCode.EmptyUndoBuffer();
            Build();
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
            if (errorList.HasErrors)
            {
                FlexibleMessageBox.Show("Before you can build a DLL, you must resolve all code errors.", "Build Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string fileName = tabStrip1.SelectedTabTitle.Trim();
            if (fileName.Length == 0 || fileName == "Untitled")
            {
                FlexibleMessageBox.Show("Before you can build a DLL, you must first save your source file using the File > Save as... menu.", "Build Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            bool buildSucceeded = false;
#if FASTDEBUG
            const bool canCreateSln = false;
#else
            bool canCreateSln = this.Services.GetService<IAppInfoService>().InstallType == AppInstallType.Classic;
#endif

            string scriptPath = tabStrip1.SelectedTabPath;
            string userCode = txtCode.Text;
            string projectName = Path.GetFileNameWithoutExtension(scriptPath);
            ProjectType projectType = tabStrip1.SelectedTabProjType;

            if (projectType.IsEffect())
            {
                using BuildForm buildForm = new BuildForm(fileName, userCode, scriptPath, projectType, canCreateSln);
                if (buildForm.ShowDialog() != DialogResult.OK)
                {
                    return;
                }

                string effectSourceCode = projectType switch
                {
                    ProjectType.BitmapEffect =>
                        BitmapEffectWriter.FullSourceCode(
                            userCode, projectName, buildForm.isAdjustment, buildForm.SubMenu, buildForm.MenuItemName, buildForm.IconPath, buildForm.URL,
                            buildForm.RenderingFlags, buildForm.RenderingSchedule, buildForm.Author, buildForm.MajorVer, buildForm.MinorVer,
                            buildForm.Description, buildForm.KeyWords, buildForm.WindowTitle, buildForm.HelpType, buildForm.HelpStr),
                    ProjectType.GpuDrawEffect =>
                        GPUDrawWriter.FullSourceCode(
                            userCode, projectName, buildForm.isAdjustment, buildForm.SubMenu, buildForm.MenuItemName, buildForm.IconPath, buildForm.URL,
                            buildForm.RenderingFlags, buildForm.RenderingSchedule, buildForm.Author, buildForm.MajorVer, buildForm.MinorVer,
                            buildForm.Description, buildForm.KeyWords, buildForm.WindowTitle, buildForm.HelpType, buildForm.HelpStr),
                    ProjectType.GpuImageEffect =>
                        GPUEffectWriter.FullSourceCode(
                            userCode, projectName, buildForm.isAdjustment, buildForm.SubMenu, buildForm.MenuItemName, buildForm.IconPath, buildForm.URL,
                            buildForm.RenderingFlags, buildForm.RenderingSchedule, buildForm.Author, buildForm.MajorVer, buildForm.MinorVer,
                            buildForm.Description, buildForm.KeyWords, buildForm.WindowTitle, buildForm.HelpType, buildForm.HelpStr)
                };

                buildSucceeded = ScriptBuilder.BuildEffectDll(effectSourceCode, scriptPath, buildForm.IconPath, buildForm.HelpType);
            }
            else if (projectType == ProjectType.FileType)
            {
                using BuildFileTypeDialog buildFileTypeDialog = new BuildFileTypeDialog(fileName, userCode, canCreateSln);
                if (buildFileTypeDialog.ShowDialog() != DialogResult.OK)
                {
                    return;
                }

                string fileTypeSourceCode = FileTypeWriter.FullSourceCode(
                    userCode, projectName, buildFileTypeDialog.Author, buildFileTypeDialog.Major, buildFileTypeDialog.Minor,
                    buildFileTypeDialog.URL, buildFileTypeDialog.Description, buildFileTypeDialog.LoadExt, buildFileTypeDialog.SaveExt,
                    buildFileTypeDialog.Layers, buildFileTypeDialog.PluginName);

                buildSucceeded = ScriptBuilder.BuildFileTypeDll(fileTypeSourceCode, projectName);
            }

            if (buildSucceeded)
            {
                string zipPath = Path.Combine(
                    System.Environment.GetFolderPath(System.Environment.SpecialFolder.DesktopDirectory),
                    Path.ChangeExtension(fileName, ".zip"));

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
                string error = (ScriptBuilder.Exception != null) ? ScriptBuilder.Exception.InsertLineBreaks(100) : "Please check for build errors in the Error List.";
                FlexibleMessageBox.Show("I'm sorry, I was not able to build the DLL.\r\n\r\n" + error, "Build Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UIDesigner()
        {
            // User Interface Designer
            using UIBuilder myUIBuilderForm = new UIBuilder(txtCode.Text, tabStrip1.SelectedTabProjType,
#if FASTDEBUG
                null, null
#else
                this.Services, this.Environment
#endif
                );

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
            txtCode.CopyAsRtf();
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
#if !FASTDEBUG
            double SaveOpacitySetting = Opacity;
            Opacity = 0;
            tmrCompile.Enabled = false;
            Build();

            ProjectType projectType = this.tabStrip1.SelectedTabProjType;
            switch (projectType)
            {
                case ProjectType.BitmapEffect:
                case ProjectType.GpuImageEffect:
                case ProjectType.GpuDrawEffect:
                    RunEffectWithDialog(projectType);
                    break;
                case ProjectType.FileType:
                    RunFileTypeWithDialog();
                    break;
            }

            tmrCompile.Enabled = true;
            Opacity = SaveOpacitySetting;
#endif
        }

        private static bool TryExtractName(string userText, ProjectType projectType, out string extractedName)
        {
            extractedName = null;

            if (projectType.IsCSharp())
            {
                Match wtn = Regex.Match(userText, @"//[\s-[\r\n]]*Name[\s-[\r\n]]*:[\s-[\r\n]]*(?<scriptName>.*)(?=\r?\n|$)", RegexOptions.IgnoreCase);
                if (wtn.Success)
                {
                    string scriptName = wtn.Groups["scriptName"].Value.Trim();
                    if (scriptName.Length > 0)
                    {
                        extractedName = scriptName;
                    }
                }
            }
            else if (projectType == ProjectType.Shape)
            {
                if (ShapeBuilder.TryExtractShapeName(userText, out string shapeName))
                {
                    extractedName = shapeName;
                }
            }

            return extractedName != null;
        }
        #endregion

        #region File Menu Event functions
        private void fileToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            ProjectType projectType = tabStrip1.SelectedTabProjType;
            bool notRef = projectType != ProjectType.Reference;
            bool cSharp = projectType.IsCSharp();

            saveToolStripMenuItem.Enabled = notRef;
            saveAsToolStripMenuItem.Enabled = notRef;
            saveAsDLLToolStripMenuItem.Enabled = notRef;
            previewEffectMenuItem.Enabled = cSharp;
            userInterfaceDesignerToolStripMenuItem.Enabled = cSharp;
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
            bool cSharp = tabStrip1.SelectedTabProjType.IsCSharp();

            this.cutToolStripMenuItem1.Enabled = isTextSelected;
            this.copyToolStripMenuItem1.Enabled = isTextSelected;
            this.selectAllToolStripMenuItem1.Enabled = hasText;
            this.indentToolStripMenuItem.Enabled = !txtCode.ReadOnly;
            this.unindentToolStripMenuItem.Enabled = !txtCode.ReadOnly;
            this.pasteToolStripMenuItem1.Enabled = txtCode.CanPaste;
            this.commentSelectionToolStripMenuItem.Enabled = cSharp && hasText;
            this.uncommentSelectionToolStripMenuItem.Enabled = cSharp && hasText;
            this.undoToolStripMenuItem1.Enabled = txtCode.CanUndo;
            this.redoToolStripMenuItem1.Enabled = txtCode.CanRedo;
            this.commentSelectionToolStripMenuItem.Enabled = cSharp;
            this.uncommentSelectionToolStripMenuItem.Enabled = cSharp;
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
        private void SetBottomPanes(bool ErrorsVisible, bool DebugVisible)
        {
            bottomPaneSplitContainer.Panel1Collapsed = !ErrorsVisible;
            bottomPaneSplitContainer.Panel2Collapsed = !DebugVisible;

            mainSplitContainer.Panel2Collapsed = !ErrorsVisible && !DebugVisible;

            ClearOutput.Enabled = DebugVisible;
            viewErrorsToolStripMenuItem.Checked = ErrorsVisible;
            viewDebugToolStripMenuItem.Checked = DebugVisible;
            ShowErrors.Checked = ErrorsVisible;
            ShowOutput.Checked = DebugVisible;
            Settings.ErrorBox = ErrorsVisible;
            Settings.Output = DebugVisible;
        }

        private void viewErrorsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetBottomPanes(!ShowErrors.Checked, ShowOutput.Checked);
            txtCode.Focus();
        }
        private void viewDebugToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetBottomPanes(ShowErrors.Checked, !ShowOutput.Checked);
            txtCode.Focus();
        }
        private void ShowErrors_Click(object sender, EventArgs e)
        {
            SetBottomPanes(ShowErrors.Checked, ShowOutput.Checked);
            txtCode.Focus();
        }
        private void ShowOutput_Click(object sender, EventArgs e)
        {
            SetBottomPanes(ShowErrors.Checked, ShowOutput.Checked);
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

        private void apiDocMenuItem_Click(object sender, EventArgs e)
        {
            LaunchUrl("https://paintdotnet.github.io/apidocs/");
        }

        private void changesInThisVersionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LaunchUrl("https://www.boltbait.com/pdn/codelab/history/#v" + CodeLab.Version);
        }

        private void discussToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LaunchUrl("https://forums.getpaint.net/forum/17-plugin");
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (SettingsForm sf = new SettingsForm())
            {
                sf.ShowDialog();
            }
            LoadSettingsFromRegistry();
            DocComment.ReInstantiate();
            BuildAsync();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StringBuilder aboutText = new StringBuilder()
                .AppendLine(WindowTitle)
                .AppendLine("Copyright ©2006-2024, All Rights Reserved.")
                .AppendLine()
                .AppendLine("―――――――――――  Developers  ――――――――――")
                .AppendLine()
                .AppendLine("Tom Jackson:\tConcept, Initial Code, Compile to DLL")
                .AppendLine()
                .AppendLine("David Issel:\tEffect UI Creation, Effect Icons, Effect Help")
                .AppendLine("\t\tSystem, File New Complex Pixel Flow Code")
                .AppendLine("\t\tGeneration, GPU Image and Drawing")
                .AppendLine("\t\tEffects, CodeLab Updater, Settings")
                .AppendLine("\t\tScreen, Bug Fixes, Tutorials and Installer.")
                .AppendLine()
                .AppendLine("Jason Wendt:\tMigration to ScintillaNET editor control,")
                .AppendLine("\t\tmodern .NET, and the \"Roslyn\" Compiler.")
                .AppendLine("\t\tIntelligent Assistance (including code")
                .AppendLine("\t\tcompletion, tips, snippets, and variable")
                .AppendLine("\t\tname suggestions), Bitmap Effects, Debug")
                .AppendLine("\t\tOutput, Dark Theme, HiDPI icons, Live")
                .AppendLine("\t\tEffect Preview, Spellcheck, Filetype")
                .AppendLine("\t\tplugin creation, and Shape editing.")
                .AppendLine()
                .AppendLine("―――――――――――  Libraries  ―――――――――――")
                .AppendLine()
                .AppendLine("Jörg Reichert:\tFlexibleMessageBox")
                .AppendLine("Jacob Slusser:\tScintillaNET")
                .AppendLine("Microsoft:\tRoslyn")
                .AppendLine()
                .AppendLine("―――――――――――  Runtime  ―――――――――――")
                .AppendLine()
                .AppendLine(".NET version:\t" + System.Environment.Version.ToString(2))
                .Append("C# version:\t" + ScriptBuilder.CSharpVersion);

            FlexibleMessageBox.Show(this, aboutText.ToString(), "About CodeLab", MessageBoxButtons.OK, MessageBoxIcon.Information);
            txtCode.Focus();
        }
        #endregion

        #region Context menu Event functions
        private void contextMenuStrip1_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (txtCode.SuppressContextMenu)
            {
                e.Cancel = true;
                return;
            }

            bool hasText = txtCode.TextLength > 0;
            bool isTextSelected = hasText && txtCode.SelectedText.Length > 0;
            bool cSharp = tabStrip1.SelectedTabProjType.IsCSharp();
            bool showDef = tabStrip1.SelectedTabProjType != ProjectType.PlainText;

            this.cutToolStripMenuItem.Enabled = isTextSelected;
            this.copyToolStripMenuItem.Enabled = isTextSelected;
            this.selectAllToolStripMenuItem.Enabled = txtCode.TextLength > 0;
            this.searchToolStripMenuItem.Enabled = true;
            this.indentToolStripMenuItem1.Enabled = !txtCode.ReadOnly;
            this.unindentToolStripMenuItem1.Enabled = !txtCode.ReadOnly;
            this.pasteToolStripMenuItem.Enabled = txtCode.CanPaste;
            this.commentSelectionToolStripMenuItem1.Enabled = cSharp && hasText;
            this.uncommentSelectionToolStripMenuItem1.Enabled = cSharp && hasText;
            this.undoToolStripMenuItem.Enabled = txtCode.CanUndo;
            this.redoToolStripMenuItem.Enabled = txtCode.CanRedo;
            this.commentSelectionToolStripMenuItem1.Enabled = cSharp;
            this.uncommentSelectionToolStripMenuItem1.Enabled = cSharp;
            this.GoToDefMenuItem.Enabled = showDef;
            this.LookUpDefMenuItem.Enabled = showDef;
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

        private void GoToDefMenuItem_Click(object sender, EventArgs e)
        {
            txtCode.GoToDefinition(false);
        }

        private void LookUpDefMenuItem_Click(object sender, EventArgs e)
        {
            txtCode.GoToDefinition(true);
        }
        #endregion

        #region Toolbar Event functions
        private void NewBitmapEffect_Click(object sender, EventArgs e)
        {
            CreateNewBitmapEffect();
        }

        private void NewGpuEffect_Click(object sender, EventArgs e)
        {
            CreateNewGPUEffect();
        }

        private void NewGpuDrawEffect_Click(object sender, EventArgs e)
        {
            CreateNewGPUDrawEffect();
        }

        private void NewFromClipboard_Click(object sender, EventArgs e)
        {
#if FASTDEBUG
            string clipboardContents = Clipboard.GetText();
#else
            string clipboardContents = this.Services.GetService<IClipboardService>().TryGetText();
#endif
            if (clipboardContents is null)
            {
                return;
            }

            ProjectType projectType = ProjectTypeUtil.FromContents(clipboardContents, null);

            CreateNewProjectTab(projectType, clipboardContents);
        }

        private void NewButton_DropDownOpening(object sender, EventArgs e)
        {
#if FASTDEBUG
            bool containsText = Clipboard.ContainsText();
#else
            bool containsText = this.Services.GetService<IClipboardService>().ContainsText();
#endif
            this.NewFromClipboardMenuItem.Enabled = containsText;
            this.NewFromClipboardMenuItem1.Enabled = containsText;
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
        #endregion

        #region Recent Items functions
        private static void AddToRecents(string filePath)
        {
            string[] paths = Settings.RecentDocs
                .Split('|', StringSplitOptions.RemoveEmptyEntries)
                .Prepend(filePath)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();

            int length = Math.Min(8, paths.Length);

            Settings.RecentDocs = string.Join("|", paths, 0, length);
        }

        private void openRecentToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            if (sender is not ToolStripDropDownItem menu)
            {
                return;
            }

            menu.DropDownItems.Clear();

            string recents = Settings.RecentDocs;

            List<ToolStripItem> recentsList = new List<ToolStripItem>();
            string[] paths = recents.Split('|', StringSplitOptions.RemoveEmptyEntries);
            int count = 1;
            foreach (string itemPath in paths)
            {
                if (!File.Exists(itemPath))
                {
                    continue;
                }

                ToolStripMenuItem recentItem = new ToolStripMenuItem();

                string fileName = Path.GetFileName(itemPath);
                string menuText = $"&{count} {fileName}";
                try
                {
                    string fileExtension = Path.GetExtension(itemPath);
                    string fileContents = File.ReadAllText(itemPath);
                    ProjectType projectType = ProjectTypeUtil.FromContents(fileContents, fileExtension);
                    if (TryExtractName(fileContents, projectType, out string extractedName))
                    {
                        menuText = $"&{count} {extractedName} ({fileName})";
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
                    recentItem.Image = UIUtil.GetBitmapFromFile(imagePath);
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

                menu.DropDownItems.AddRange(recentsList.ToArray());
            }
            else
            {
                ToolStripMenuItem noRecents = new ToolStripMenuItem
                {
                    Text = "No Recent Items",
                    Enabled = false
                };

                menu.DropDownItems.Add(noRecents);
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
            txtCode.SwitchToDocument(tabStrip1.SelectedTabGuid);
            UpdateWindowTitle();
            UpdateToolBarButtons();
            UpdateForProjectType();
        }

        private void tabStrip1_NewTabCreated(object sender, EventArgs e)
        {
            txtCode.CreateNewDocument(tabStrip1.SelectedTabGuid, tabStrip1.SelectedTabProjType);
            UpdateWindowTitle();
            UpdateToolBarButtons();
            UpdateForProjectType();
        }

        private void UpdateForProjectType()
        {
            ProjectType projectType = tabStrip1.SelectedTabProjType;

            bool useWordWrap = (projectType == ProjectType.PlainText) ? Settings.WordWrapPlainText : Settings.WordWrap;
            txtCode.WrapMode = useWordWrap ? WrapMode.Whitespace : WrapMode.None;

            DisableButtonsForRef(projectType == ProjectType.Reference);

            if (projectType.IsCSharp())
            {
                EnableCSharpButtons(true);
                Intelli.SetReferences(projectType);
                BuildAsync();
            }
            else
            {
                EnableCSharpButtons(false);
                Build();
            }
        }

        private void EnableCSharpButtons(bool enable)
        {
            SaveDLLButton.Enabled = enable;
            UIDesignerButton.Enabled = enable;
            RunButton.Enabled = enable;
            FormatDocButton.Enabled = enable;
            formatDocMenuItem.Enabled = enable;
            CommentButton.Enabled = enable;
            UnCommentButton.Enabled = enable;
        }

        private void DisableButtonsForRef(bool disable)
        {
            bool enable = !disable;
            SaveButton.Enabled = enable;
            saveToolStripMenuItem.Enabled = enable;
            IndentButton.Enabled = enable;
            UndentButton.Enabled = enable;
            PasteButton.Enabled = enable;
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
            tabStrip1.NewTab(e.Name, e.Path, ProjectType.Reference);
        }

        private void tabStrip1_TabClosed(object sender, TabClosedEventArgs e)
        {
            this.txtCode.CloseDocument(e.TabGuid);
        }

        private void UpdateWindowTitle()
        {
            this.Text = tabStrip1.SelectedTabTitle + " - " + WindowTitle + renderPreset.GetExTitle();
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
}
