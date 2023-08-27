using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace PaintDotNet.Effects
{
    internal sealed class SaveConfigDialog : ChildFormBase
    {
        private FileType fileType;
        private Surface srcSurface;
        private readonly Size imageSize;

        private SaveConfigWidget widget;
        private Button defaultsButton;
        private CheckBox testLayersCheckBox;
        private Button saveButton;
        private Panel viewPort;
        private PictureBox canvas;

        internal SaveConfigDialog(FileType fileType, Surface src)
        {
            this.fileType = fileType;
            this.srcSurface = src;
            this.imageSize = src.Size;

            this.SuspendLayout();

            this.canvas = new PictureBox();
            using (Surface backImage = new Surface(16, 16))
            {
                backImage.ClearWithCheckerboardPattern();
                canvas.BackgroundImage = new Bitmap(backImage.CreateAliasedBitmap());
            }
            this.canvas.Image = src.CreateAliasedBitmap();
            this.canvas.Size = this.imageSize;

            this.viewPort = new Panel();
            this.viewPort.Anchor = AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom | AnchorStyles.Left;
            this.viewPort.AutoScroll = true;
            this.viewPort.Size = new Size(551, 548);
            this.viewPort.Location = new Point(225, 7);
            this.viewPort.Controls.Add(canvas);

            this.testLayersCheckBox = new CheckBox();
            this.testLayersCheckBox.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            this.testLayersCheckBox.AutoSize = true;
            this.testLayersCheckBox.Text = "Add a test layer";
            this.testLayersCheckBox.Location = new Point(16, 495);
            this.testLayersCheckBox.CheckedChanged += (object sender, EventArgs e) => UpdatePreview();

            this.widget = fileType.CreateSaveConfigWidget();
            this.widget.Location = new Point(7, 7);
            this.widget.Width = 180;
            this.widget.TokenChanged += (object sender, EventArgs e) => UpdatePreview();

            this.saveButton = new Button();
            this.saveButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            this.saveButton.AutoSize = true;
            this.saveButton.FlatStyle = FlatStyle.System;
            this.saveButton.Text = "Save";
            this.saveButton.Location = new Point(14, 525);
            this.saveButton.Click += (object sender, EventArgs e) => SaveToFile();

            this.defaultsButton = new Button();
            this.defaultsButton.AutoSize = true;
            this.defaultsButton.FlatStyle = FlatStyle.System;
            this.defaultsButton.Text = "Defaults";
            this.defaultsButton.Click += (object sender, EventArgs e) => ResetToken();

            this.IconName = "Save";
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.MaximizeBox = true;
            this.MinimumSize = new Size(600, 380);
            this.Size = new Size(800, 600);
            this.Text = "Save Configuration";
            this.Controls.AddRange(new Control[]
            {
                widget,
                defaultsButton,
                testLayersCheckBox,
                saveButton,
                viewPort
            });

            this.ResumeLayout(false);

            this.defaultsButton.Location = new Point((widget.Right - widget.Left) / 2 + widget.Left - defaultsButton.Width / 2, widget.Bottom + 16);
        }

        private void UpdatePreview()
        {
            using (Document document = new Document(this.imageSize))
            using (MemoryStream stream = new MemoryStream())
            using (Surface scratchSurface = new Surface(this.imageSize))
            {
                document.Layers.Add(new BitmapLayer(this.srcSurface));
                if (this.testLayersCheckBox.Checked)
                {
                    document.Layers.Add(new BitmapLayer(this.imageSize, ColorBgra.DodgerBlue.NewAlpha(85)));
                }

                ProgressEventHandler progress = (object s1, ProgressEventArgs e1) =>
                {
                };

                try
                {
                    fileType.Save(document, stream, this.widget.Token, scratchSurface, progress, false);

                    stream.Seek(0, SeekOrigin.Begin);

                    using (Document savedDoc = fileType.Load(stream))
                    {
                        savedDoc.Flatten(scratchSurface);
                    }
                }
                catch
                {
                    scratchSurface.ClearWithCheckerboardPattern();
                }

                this.canvas.Image = new Bitmap(scratchSurface.CreateAliasedBitmap());

                document.Layers.DisposeAll();
            }
        }

        private void SaveToFile()
        {
            string filePath = null;

            using (SaveFileDialog sfd = new SaveFileDialog
                {
                    Title = "Save As",
                    DefaultExt = this.fileType.Options.DefaultSaveExtension,
                    Filter = this.fileType.Name + "|*" + this.fileType.Options.SaveExtensions.Join("; *"),
                    OverwritePrompt = true,
                    AddExtension = true,
                    FileName = "Untitled" + this.fileType.Options.DefaultSaveExtension,
                    InitialDirectory = Settings.LastSourceDirectory
                })
            {
                if (sfd.ShowDialog() != DialogResult.OK)
                {
                    return;
                }

                filePath = sfd.FileName;
            }

            if (filePath == null)
            {
                return;
            }

            using (Document document = new Document(this.imageSize))
            using (FileStream stream = new FileStream(filePath, FileMode.OpenOrCreate))
            using (Surface scratchSurface = new Surface(this.imageSize))
            {
                document.Layers.Add(new BitmapLayer(this.srcSurface));
                if (testLayersCheckBox.Checked)
                {
                    document.Layers.Add(new BitmapLayer(this.imageSize, ColorBgra.DodgerBlue.NewAlpha(85)));
                }

                ProgressEventHandler progress = (object s1, ProgressEventArgs e1) =>
                {
                };

                fileType.Save(document, stream, widget.Token, scratchSurface, progress, false);

                document.Layers.DisposeAll();
            }
        }

        private void ResetToken()
        {
            widget.Token = fileType.CreateDefaultSaveConfigToken();
        }
    }

    internal sealed class SaveWidgetDialog : ChildFormBase
    {
        internal SaveWidgetDialog(SaveConfigWidget widget, SaveConfigToken defaultToken)
        {
            this.SuspendLayout();

            SaveConfigWidget configWidget = widget;
            configWidget.Location = new Point(7, 7);
            configWidget.Width = 180;

            Button defaultsButton = new Button();
            defaultsButton.AutoSize = true;
            defaultsButton.FlatStyle = FlatStyle.System;
            defaultsButton.Text = "Defaults";
            defaultsButton.Click += (object sender, EventArgs e) => configWidget.Token = defaultToken;

            this.IconName = "Save";
            this.Text = "Save Configuration";
            this.Controls.AddRange(new Control[]
            {
                configWidget,
                defaultsButton
            });

            this.ResumeLayout(false);

            defaultsButton.Location = new Point((configWidget.Right - configWidget.Left) / 2 + configWidget.Left - defaultsButton.Width / 2, configWidget.Bottom + 16);
            this.ClientSize = new Size(configWidget.Right + 7, defaultsButton.Bottom + 7);
        }
    }
}
