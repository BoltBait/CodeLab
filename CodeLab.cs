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

using System;
using System.Drawing;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;

[assembly: AssemblyTitle("CodeLab plugin for Paint.NET")]
[assembly: AssemblyDescription("C# Code Editor for Paint.NET Plugin Development")]
[assembly: AssemblyConfiguration("C#|development|plugin|build|builder|code|coding|script|scripting")]
[assembly: AssemblyCompany("BoltBait")]
[assembly: AssemblyProduct("CodeLab")]
[assembly: AssemblyCopyright("Copyright ©2020 BoltBait")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: ComVisible(false)]
[assembly: AssemblyVersion(PaintDotNet.Effects.CodeLab.Version + ".*")]
// The next line is for the ScintillaNET text editor control.  This way you don't have to use the "copy attributes" option in IL Merge.
[assembly: Guid("f8ac48e7-9378-482d-8c7f-92c8408dd4f2")]

namespace PaintDotNet.Effects
{
    public class PluginSupportInfo : IPluginSupportInfo
    {
        public string Author => base.GetType().Assembly.GetCustomAttribute<AssemblyCopyrightAttribute>().Copyright;
        public string Copyright => base.GetType().Assembly.GetCustomAttribute<AssemblyDescriptionAttribute>().Description;
        public string DisplayName => base.GetType().Assembly.GetCustomAttribute<AssemblyProductAttribute>().Product;
        public Version Version => base.GetType().Assembly.GetName().Version;
        public Uri WebsiteUri => new Uri("https://www.boltbait.com/pdn/CodeLab/");
    }

    public abstract class CodeLab : Effect
    {
        internal const string Version = "6.0";

        private static Image StaticImage => UIUtil.GetImage("CodeLab");

        protected CodeLab(string extendedName, EffectFlags flags, EffectRenderingSchedule renderingSchedule)
            : base("CodeLab" + extendedName, StaticImage, "Advanced", new EffectOptions() { Flags = EffectFlags.Configurable | flags, RenderingSchedule = renderingSchedule })
        {
        }

        public override EffectConfigDialog CreateConfigDialog()
        {
            return new CodeLabConfigDialog();
        }

        private Effect userEffect;
        private bool fetchDebugMsg;
        private ProjectType projectType;
        private Surface shapeSurface;

        private readonly BinaryPixelOp normalOp = LayerBlendModeUtil.CreateCompositionOp(LayerBlendMode.Normal);

        protected override void OnSetRenderInfo(EffectConfigToken parameters, RenderArgs dstArgs, RenderArgs srcArgs)
        {
            CodeLabConfigToken sect = (CodeLabConfigToken)parameters;
            userEffect = sect.UserScriptObject;
            projectType = sect.ProjectType;

            if (projectType == ProjectType.Shape)
            {
                Size srcSize = EnvironmentParameters.SourceSurface.Size;
                Rectangle selection = EnvironmentParameters.SelectionBounds;
                ColorBgra strokeColor = EnvironmentParameters.PrimaryColor;
                ColorBgra fillColor = EnvironmentParameters.SecondaryColor;
                double strokeThickness = EnvironmentParameters.BrushWidth;

                Thread t = new Thread(() =>
                {
                    ShapeBuilder.SetEnviromentParams(srcSize.Width, srcSize.Height, selection.X, selection.Y, selection.Width, selection.Height, strokeColor, fillColor, strokeThickness);
                    ShapeBuilder.RenderShape(sect.UserCode);
                });
                t.SetApartmentState(ApartmentState.STA);
                t.Start();
                t.Join();

                shapeSurface?.Dispose();
                shapeSurface = (ShapeBuilder.Shape != null) ?
                    Surface.CopyFromBitmap(ShapeBuilder.Shape) :
                    null;
            }
            else if (projectType == ProjectType.Effect && userEffect != null)
            {
                userEffect.EnvironmentParameters = this.EnvironmentParameters;

                try
                {
                    userEffect.SetRenderInfo(sect.PreviewToken, dstArgs, srcArgs);
                    fetchDebugMsg = true;
                }
                catch (Exception exc)
                {
                    sect.LastExceptions.Add(exc);
                    dstArgs.Surface.CopySurface(srcArgs.Surface);
                    sect.UserScriptObject = null;
                    userEffect?.Dispose();
                    userEffect = null;
                }
            }

            base.OnSetRenderInfo(parameters, dstArgs, srcArgs);
        }

        public override void Render(EffectConfigToken parameters, RenderArgs dstArgs, RenderArgs srcArgs, Rectangle[] rois, int startIndex, int length)
        {
            if (projectType == ProjectType.Shape && shapeSurface != null)
            {
                dstArgs.Surface.CopySurface(srcArgs.Surface, rois, startIndex, length);
                normalOp.Apply(dstArgs.Surface, shapeSurface, rois, startIndex, length);
            }
            else if (projectType == ProjectType.Effect && userEffect != null)
            {
                CodeLabConfigToken sect = (CodeLabConfigToken)parameters;
                try
                {
                    userEffect.Render(sect.PreviewToken, dstArgs, srcArgs, rois, startIndex, length);
                }
                catch (Exception exc)
                {
                    sect.LastExceptions.Add(exc);
                    dstArgs.Surface.CopySurface(srcArgs.Surface);
                    sect.UserScriptObject = null;
                    userEffect?.Dispose();
                    userEffect = null;
                }

                if (fetchDebugMsg)
                {
                    fetchDebugMsg = false;
                    try
                    {
                        string output = userEffect?.GetType().GetProperty("__DebugMsgs", typeof(string))?.GetValue(userEffect)?.ToString();
                        if (!output.IsNullOrEmpty())
                        {
                            sect.Output.Add(output);
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

                shapeSurface?.Dispose();
                shapeSurface = null;
            }

            base.OnDispose(disposing);
        }
    }

    [PluginSupportInfo(typeof(PluginSupportInfo), DisplayName = "CodeLab")]
    public class CodeLabRegular : CodeLab
    {
        public CodeLabRegular() : base(string.Empty, EffectFlags.None, EffectRenderingSchedule.DefaultTilesForCpuRendering)
        {
        }
    }

    [PluginSupportInfo(typeof(PluginSupportInfo), DisplayName = "CodeLab")]
    public class CodeLabLegacyROI : CodeLab
    {
        public CodeLabLegacyROI() : base(" - Legacy ROI", EffectFlags.None, EffectRenderingSchedule.SmallHorizontalStrips)
        {
        }
    }

    [PluginSupportInfo(typeof(PluginSupportInfo), DisplayName = "CodeLab")]
    public class CodeLabAliased : CodeLab
    {
        public CodeLabAliased() : base(" - Aliased Selection", EffectFlags.ForceAliasedSelectionQuality, EffectRenderingSchedule.DefaultTilesForCpuRendering)
        {
        }
    }

    [PluginSupportInfo(typeof(PluginSupportInfo), DisplayName = "CodeLab")]
    public class CodeLabSingleRender : CodeLab
    {
        public CodeLabSingleRender() : base(" - Single Render Call", EffectFlags.None, EffectRenderingSchedule.None)
        {
        }
    }
}
