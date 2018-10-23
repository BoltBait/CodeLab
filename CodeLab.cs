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

using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

[assembly: AssemblyTitle("CodeLab plugin for Paint.NET")]
[assembly: AssemblyDescription("C# Code Editor for Paint.NET Plugin Development")]
[assembly: AssemblyConfiguration("C#|development|plugin|build|builder|code|coding|script|scripting")]
[assembly: AssemblyCompany("BoltBait")]
[assembly: AssemblyProduct("CodeLab")]
[assembly: AssemblyCopyright("Copyright ©2018 BoltBait")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: ComVisible(false)]
[assembly: AssemblyVersion("4.1.*")]
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
        public Uri WebsiteUri => new Uri("http://www.boltbait.com/pdn/CodeLab/");
    }

    [PluginSupportInfo(typeof(PluginSupportInfo), DisplayName = "CodeLab")]
    public class CodeLab : Effect
    {
        private static Image StaticImage
        {
            get
            {
                using (Stream imageStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("PaintDotNet.Effects.Icons.CodeLab.png"))
                {
                    return Image.FromStream(imageStream);
                }
            }
        }

        public CodeLab() : base("CodeLab", StaticImage, "Advanced", EffectFlags.Configurable)
        {
        }

        public override EffectConfigDialog CreateConfigDialog()
        {
            return new CodeLabConfigDialog();
        }

        private Effect userEffect;
        private bool fetchDebugMsg;

        protected override void OnSetRenderInfo(EffectConfigToken parameters, RenderArgs dstArgs, RenderArgs srcArgs)
        {
            CodeLabConfigToken sect = (CodeLabConfigToken)parameters;
            userEffect = sect.UserScriptObject;

            if (userEffect != null)
            {
                userEffect.EnvironmentParameters = this.EnvironmentParameters;

                try
                {
                    userEffect.SetRenderInfo(sect.Preview ? sect.PreviewToken : null, dstArgs, srcArgs);
                    fetchDebugMsg = true;
                }
                catch (Exception exc)
                {
                    sect.LastExceptions.Add(exc);
                    dstArgs.Surface.CopySurface(srcArgs.Surface);
                    sect.UserScriptObject = null;
                    userEffect.Dispose();
                    userEffect = null;
                }
            }

            base.OnSetRenderInfo(parameters, dstArgs, srcArgs);
        }

        public override void Render(EffectConfigToken parameters, RenderArgs dstArgs, RenderArgs srcArgs, Rectangle[] rois, int startIndex, int length)
        {
            if (userEffect != null)
            {
                CodeLabConfigToken sect = (CodeLabConfigToken)parameters;
                try
                {
                    userEffect.Render(sect.Preview ? sect.PreviewToken : null, dstArgs, srcArgs, rois, startIndex, length);
                }
                catch (Exception exc)
                {
                    sect.LastExceptions.Add(exc);
                    dstArgs.Surface.CopySurface(srcArgs.Surface);
                    sect.UserScriptObject = null;
                    userEffect.Dispose();
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
            userEffect?.Dispose();
            base.OnDispose(disposing);
        }
    }
}
