/////////////////////////////////////////////////////////////////////////////////
// CodeLab for Paint.NET
// Copyright �2006 Rick Brewster, Tom Jackson. All Rights Reserved.
// Portions Copyright �2007-2020 BoltBait. All Rights Reserved.
// Portions Copyright �2016-2020 Jason Wendt. All Rights Reserved.
// Portions Copyright �Microsoft Corporation. All Rights Reserved.
//
// THE CODELAB DEVELOPERS MAKE NO WARRANTY OF ANY KIND REGARDING THE CODE. THEY
// SPECIFICALLY DISCLAIM ANY WARRANTY OF FITNESS FOR ANY PARTICULAR PURPOSE OR
// ANY OTHER WARRANTY.  THE CODELAB DEVELOPERS DISCLAIM ALL LIABILITY RELATING
// TO THE USE OF THIS CODE.  NO LICENSE, EXPRESS OR IMPLIED, BY ESTOPPEL OR
// OTHERWISE, TO ANY INTELLECTUAL PROPERTY RIGHTS IS GRANTED HEREIN.
//
// Latest distribution: https://www.BoltBait.com/pdn/codelab
/////////////////////////////////////////////////////////////////////////////////

using PaintDotNet.Direct2D1;
using PaintDotNet.Imaging;
using PaintDotNet.Rendering;
using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using WpfGeometry = System.Windows.Media.Geometry;
using IDeviceContext = PaintDotNet.Direct2D1.IDeviceContext;

[assembly: AssemblyTitle("CodeLab plugin for Paint.NET")]
[assembly: AssemblyDescription("C# Code Editor for Paint.NET Plugin Development")]
[assembly: AssemblyConfiguration("C#|development|plugin|build|builder|code|coding|script|scripting")]
[assembly: AssemblyCompany("BoltBait")]
[assembly: AssemblyProduct("CodeLab")]
[assembly: AssemblyCopyright("Copyright �2022 BoltBait")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: ComVisible(false)]
[assembly: SupportedOSPlatform("Windows")]
[assembly: AssemblyVersion(PaintDotNet.Effects.CodeLab.Version + ".*")]
// The ScintillaNET text editor requires the assembly to have a Guid.
[assembly: Guid("b908a26a-45e2-4d24-9681-e6f2020e68a8")]

namespace PaintDotNet.Effects
{
    public class CodeLabSupportInfo : IPluginSupportInfo
    {
        public string Author => base.GetType().Assembly.GetCustomAttribute<AssemblyCopyrightAttribute>().Copyright;
        public string Copyright => base.GetType().Assembly.GetCustomAttribute<AssemblyDescriptionAttribute>().Description;
        public string DisplayName => base.GetType().Assembly.GetCustomAttribute<AssemblyProductAttribute>().Product;
        public Version Version => base.GetType().Assembly.GetName().Version;
        public Uri WebsiteUri => new Uri("https://www.boltbait.com/pdn/CodeLab/");
    }

    public abstract class CodeLab : BitmapEffect<CodeLabConfigToken>
    {
        internal const string Version = "6.8";

        // Includes the Build and Revision fields that are generated by the compiler
        internal static string VersionFull => typeof(CodeLab).Assembly.GetName().Version.ToString();

        protected CodeLab(string extendedName, BitmapEffectRenderingFlags renderingFlags, BitmapEffectRenderingSchedule renderingSchedule)
            : base("CodeLab" + extendedName, UIUtil.GetImage("CodeLab"), "Advanced", BitmapEffectOptions.Create() with { IsConfigurable = true })
        {
            this.renderingFlags = renderingFlags;
            this.renderingSchedule = renderingSchedule;
        }

        private readonly BitmapEffectRenderingFlags renderingFlags;
        private readonly BitmapEffectRenderingSchedule renderingSchedule;

        protected override IEffectConfigForm OnCreateConfigForm()
        {
            return new CodeLabConfigDialog();
        }

        private IEffect userEffect;
        private bool fetchDebugMsg;
        private ProjectType projectType;
        private string shapeCode;
        private IBitmapEffectRenderer renderer;
        private IBitmapSource<ColorBgra32> sourceBitmap;

        protected override void OnInitializeRenderInfo(IBitmapEffectRenderInfo renderInfo)
        {
            this.sourceBitmap = this.Environment.GetSourceBitmapBgra32();

            renderInfo.Flags = this.renderingFlags;
            renderInfo.Schedule = this.renderingSchedule;
            base.OnInitializeRenderInfo(renderInfo);
        }

        protected override void OnSetToken(CodeLabConfigToken newToken)
        {
            userEffect = newToken.UserScriptObject;
            projectType = newToken.ProjectType;
            fetchDebugMsg = true;
            shapeCode = (projectType == ProjectType.Shape) ? newToken.UserCode : null;

            if (projectType.IsEffect() && userEffect != null)
            {
                using (IEffect effect = userEffect.EffectInfo.CreateInstance(this.Services, this.Environment))
                {
                    this.renderer = effect.CreateRenderer<IBitmapEffectRenderer>();
                }
                BitmapEffectInitializeInfo initializeInfo = new BitmapEffectInitializeInfo();
                this.renderer.Initialize(initializeInfo);
                this.renderer.SetToken(newToken.PreviewToken);
            }

            base.OnSetToken(newToken);
        }

        protected override void OnRender(IBitmapEffectOutput output)
        {
            using IBitmapLock<ColorBgra32> outputLock = output.LockBgra32();

            if (projectType == ProjectType.Shape)
            {
                WpfGeometry wpfGeometry = ShapeBuilder.GeometryFromRawString(shapeCode);
                if (wpfGeometry == null)
                {
                    this.sourceBitmap.CopyPixels(outputLock, output.Bounds.Location);
                    return;
                }

                IDirect2DFactory d2dFactory = this.Services.GetService<IDirect2DFactory>();
                using IGeometry d2dGeometry = d2dFactory.CreateGeometryFromWpfGeometry(wpfGeometry);

                RectFloat geoBounds = d2dGeometry.GetWidenedBounds(this.Environment.BrushSize);
                RectInt32 selBounds = this.Environment.Selection.RenderBounds;

                float scale = (selBounds.Width - geoBounds.Width) < (selBounds.Height - geoBounds.Height)
                    ? (selBounds.Width - 10) / geoBounds.Width
                    : (selBounds.Height - 10) / geoBounds.Height;

                float selCenterX = (selBounds.Right - selBounds.Left) / 2f + selBounds.Left;
                float selCenterY = (selBounds.Bottom - selBounds.Top) / 2f + selBounds.Top;

                Matrix3x2Float matrix = Matrix3x2Float.Translation(
                    (selBounds.Width - geoBounds.Width) / 2f - geoBounds.Left + selBounds.Left,
                    (selBounds.Height - geoBounds.Height) / 2f - geoBounds.Top + selBounds.Top);

                matrix.ScaleAt(scale, scale, selCenterX, selCenterY);

                using ITransformedGeometry transformedGeometry = d2dFactory.CreateTransformedGeometry(d2dGeometry, matrix);

                using IBitmap<ColorBgra32> outputBitmap = outputLock.CreateSharedBitmap();
                using IBitmap<ColorPbgra32> outputBitmapP = outputBitmap.CreatePremultipliedAdapter(PremultipliedAdapterOptions.UnPremultiplyOnDispose | PremultipliedAdapterOptions.PremultiplyOnCreate);
                using IDeviceContext outputDC = d2dFactory.CreateBitmapDeviceContext(outputBitmapP);
                using ISolidColorBrush strokeBrush = outputDC.CreateSolidColorBrush(this.Environment.PrimaryColor);
                using ISolidColorBrush solidBrush = outputDC.CreateSolidColorBrush(this.Environment.SecondaryColor);

                using (outputDC.UseBeginDraw())
                {
                    outputDC.Clear();

                    ICommandList commandList = outputDC.CreateCommandList();
                    using (commandList.UseBeginDraw(outputDC))
                    {
                        using IBitmapImage srcImage = outputDC.CreateImageFromBitmap(this.Environment.GetSourceBitmapBgra32());
                        outputDC.DrawImage(srcImage);

                        using (outputDC.UseTranslateTransform(0.5f, 0.5f))
                        {
                            outputDC.FillGeometry(transformedGeometry, solidBrush);
                            outputDC.DrawGeometry(transformedGeometry, strokeBrush, this.Environment.BrushSize);
                        }
                    }

                    using (outputDC.UseTranslateTransform(-output.Bounds.Location))
                    {
                        outputDC.DrawImage(commandList);
                    }
                }
            }
            else if (projectType.IsEffect() && userEffect != null)
            {
                try
                {
                    if (this.renderer == null || this.renderer.IsDisposed)
                    {
                        this.sourceBitmap.CopyPixels(outputLock, output.Bounds.Location);
                    }
                    else
                    {
                        this.renderer.Render(outputLock, output.Bounds.Location);
                    }
                }
                catch (Exception exc)
                {
                    this.Token.LastExceptions.Add(exc);
                    this.sourceBitmap.CopyPixels(outputLock, output.Bounds.Location);
                    this.Token.UserScriptObject = null;
                    userEffect?.Dispose();
                    userEffect = null;
                }

                if (fetchDebugMsg)
                {
                    fetchDebugMsg = false;
                    try
                    {
                        string debugOutput = userEffect?.GetType()
                            .GetProperty("__DebugMsgs", typeof(string))?
                            .GetValue(userEffect)?
                            .ToString();

                        if (!debugOutput.IsNullOrEmpty())
                        {
                            this.Token.Output.Add(debugOutput);
                        }
                    }
                    catch
                    {
                        // just fail silently
                    }
                }
            }
        }

        protected override void OnDispose(bool disposing)
        {
            if (disposing)
            {
                userEffect?.Dispose();
                userEffect = null;
            }

            base.OnDispose(disposing);
        }
    }

    [PluginSupportInfo<CodeLabSupportInfo>]
    public class CodeLabRegular : CodeLab
    {
        public CodeLabRegular() : base(string.Empty, BitmapEffectRenderingFlags.None, BitmapEffectRenderingSchedule.SquareTiles)
        {
        }
    }

    [PluginSupportInfo<CodeLabSupportInfo>]
    public class CodeLabLegacyROI : CodeLab
    {
        public CodeLabLegacyROI() : base(" - Legacy ROI", BitmapEffectRenderingFlags.None, BitmapEffectRenderingSchedule.HorizontalStrips)
        {
        }
    }

    [PluginSupportInfo<CodeLabSupportInfo>]
    public class CodeLabAliased : CodeLab
    {
        public CodeLabAliased() : base(" - Aliased Selection", BitmapEffectRenderingFlags.ForceAliasedSelectionQuality, BitmapEffectRenderingSchedule.SquareTiles)
        {
        }
    }

    [PluginSupportInfo<CodeLabSupportInfo>]
    public class CodeLabSingleRender : CodeLab
    {
        public CodeLabSingleRender() : base(" - Single Render Call", BitmapEffectRenderingFlags.None, BitmapEffectRenderingSchedule.None)
        {
        }
    }

    [PluginSupportInfo<CodeLabSupportInfo>]
    public class CodeLabNoClip : CodeLab
    {
        public CodeLabNoClip() : base(" - No Selection Clip", BitmapEffectRenderingFlags.DisableSelectionClipping, BitmapEffectRenderingSchedule.SquareTiles)
        {
        }
    }
}
