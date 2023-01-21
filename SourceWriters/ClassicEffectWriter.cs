/////////////////////////////////////////////////////////////////////////////////
// CodeLab for Paint.NET
// Copyright ©2006 Rick Brewster, Tom Jackson. All Rights Reserved.
// Portions Copyright ©2018 BoltBait. All Rights Reserved.
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
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace PaintDotNet.Effects
{
    internal static class ClassicEffectWriter
    {
        internal const string DefaultCode = ""
            + "// Name:\r\n"
            + "// Submenu:\r\n"
            + "// Author:\r\n"
            + "// Title:\r\n"
            + "// Version:\r\n"
            + "// Desc:\r\n"
            + "// Keywords:\r\n"
            + "// URL:\r\n"
            + "// Help:\r\n"
            + "#region UICode\r\n"
            + "IntSliderControl Amount1 = 0; // [0,100] Slider 1 Description\r\n"
            + "IntSliderControl Amount2 = 0; // [0,100] Slider 2 Description\r\n"
            + "IntSliderControl Amount3 = 0; // [0,100] Slider 3 Description\r\n"
            + "#endregion\r\n"
            + "\r\n"
            + "void Render(Surface dst, Surface src, Rectangle rect)\r\n"
            + "{\r\n"
            + "    // Delete any of these lines you don't need\r\n"
            + "    Rectangle selection = EnvironmentParameters.SelectionBounds;\r\n"
            + "    int centerX = ((selection.Right - selection.Left) / 2) + selection.Left;\r\n"
            + "    int centerY = ((selection.Bottom - selection.Top) / 2) + selection.Top;\r\n"
            + "    ColorBgra primaryColor = EnvironmentParameters.PrimaryColor;\r\n"
            + "    ColorBgra secondaryColor = EnvironmentParameters.SecondaryColor;\r\n"
            + "    int brushWidth = (int)EnvironmentParameters.BrushWidth;\r\n"
            + "\r\n"
            + "    ColorBgra currentPixel;\r\n"
            + "    for (int y = rect.Top; y < rect.Bottom; y++)\r\n"
            + "    {\r\n"
            + "        if (IsCancelRequested) return;\r\n"
            + "        for (int x = rect.Left; x < rect.Right; x++)\r\n"
            + "        {\r\n"
            + "            currentPixel = src[x,y];\r\n"
            + "            // TODO: Add pixel processing code here\r\n"
            + "            // Access RGBA values this way, for example:\r\n"
            + "            // currentPixel.R = primaryColor.R;\r\n"
            + "            // currentPixel.G = primaryColor.G;\r\n"
            + "            // currentPixel.B = primaryColor.B;\r\n"
            + "            // currentPixel.A = primaryColor.A;\r\n"
            + "            dst[x,y] = currentPixel;\r\n"
            + "        }\r\n"
            + "    }\r\n"
            + "}\r\n";

        private const string EmptyCode = "\r\n"
            + "#region User Entered Code\r\n"
            + "void Render(Surface dst, Surface src, Rectangle rect) { }\r\n"
            + "#endregion\r\n";

        private static string UsingPartCode()
        {
            return ""
                + "using System;\r\n"
                + "using System.IO;\r\n"
                + "using System.Linq;\r\n"
                + "using System.Diagnostics;\r\n"
                + "using System.Drawing;\r\n"
                + "using System.Threading;\r\n"
                + "using System.Reflection;\r\n"
                + "using System.Drawing.Text;\r\n"
                + "using System.Windows.Forms;\r\n"
                + "using System.IO.Compression;\r\n"
                + "using System.Drawing.Drawing2D;\r\n"
                + "using System.Collections.Generic;\r\n"
                + "using System.Text;\r\n"
                + "using System.Text.RegularExpressions;\r\n"
                + "using System.Runtime.InteropServices;\r\n"
                + "using System.Runtime.Versioning;\r\n"
                + "using Registry = Microsoft.Win32.Registry;\r\n"
                + "using RegistryKey = Microsoft.Win32.RegistryKey;\r\n"
                + "using PaintDotNet;\r\n"
                + "using PaintDotNet.AppModel;\r\n"
                + "using PaintDotNet.Effects;\r\n"
                + "using PaintDotNet.Clipboard;\r\n"
                + "using PaintDotNet.IndirectUI;\r\n"
                + "using PaintDotNet.Collections;\r\n"
                + "using PaintDotNet.PropertySystem;\r\n"
                + "using PaintDotNet.Rendering;\r\n"
                + "using ColorWheelControl = PaintDotNet.ColorBgra;\r\n"
                + "using AngleControl = System.Double;\r\n"
                + "using PanSliderControl = PaintDotNet.Rendering.Vector2Double;\r\n"
                + "using FolderControl = System.String;\r\n"
                + "using FilenameControl = System.String;\r\n"
                + "using ReseedButtonControl = System.Byte;\r\n"
                + "using RollControl = PaintDotNet.Rendering.Vector3Double;\r\n"
                + "using IntSliderControl = System.Int32;\r\n"
                + "using CheckboxControl = System.Boolean;\r\n"
                + "using TextboxControl = System.String;\r\n"
                + "using DoubleSliderControl = System.Double;\r\n"
                + "using ListBoxControl = System.Byte;\r\n"
                + "using RadioButtonControl = System.Byte;\r\n"
                + "using MultiLineTextboxControl = System.String;\r\n"
                + "\r\n";
        }

        private const string prepend_code = "\r\n"
            + "namespace PaintDotNet.Effects\r\n"
            + "{\r\n"
            + "public class UserScript : Effect\r\n"
            + "{\r\n"
            + "    [ThreadStatic]\r\n"
            + "    private static Random RandomNumber;\r\n"
            + "    private int instanceSeed = unchecked((int)DateTime.Now.Ticks);\r\n"
            + "\r\n"
            + "    public override void Render(EffectConfigToken parameters, RenderArgs dstArgs, RenderArgs srcArgs, Rectangle[] rois, int startIndex, int length)\r\n"
            + "    {\r\n"
            + "        if (length == 0) return;\r\n"
            + "        RandomNumber = new Random(instanceSeed ^ (1 << 16) ^ (rois[startIndex].X << 8) ^ rois[startIndex].Y);\r\n"
            + "        for (int i = startIndex; i < startIndex + length; ++i)\r\n"
            + "        {\r\n"
            + "            Render(dstArgs.Surface, srcArgs.Surface, rois[i]);\r\n"
            + "        }\r\n"
            + "    }\r\n"
            + "\r\n"
            + "    public UserScript()\r\n"
            + "        : base(\"UserScript\", string.Empty, new EffectOptions())\r\n";

        private static string ConstructorPart(UIElement[] UserControls, string FileName, string subMenuName, string menuName, string iconPath, ScriptRenderingFlags renderingFlags, ScriptRenderingSchedule renderingSchedule)
        {
            menuName = menuName.Trim().Replace('"', '\'');
            if (menuName.Length == 0)
            {
                menuName = FileName;
            }

            string className = Regex.Replace(FileName, @"[^\w]", "") + "EffectPlugin";

            string icon = File.Exists(iconPath)
                ? "new Bitmap(typeof(" + className + "), \"" + Path.GetFileName(iconPath) + "\")"
                : "null";

            string EffectPart = "";
            EffectPart += "    public class " + className + " : PropertyBasedEffect\r\n";
            EffectPart += "    {\r\n";
            EffectPart += "        public static string StaticName => \"" + menuName + "\";\r\n";
            EffectPart += "        public static Image StaticIcon => " + icon + ";\r\n";
            EffectPart += "        public static string SubmenuName => " + CommonWriter.SubmenuPart(subMenuName) + ";\r\n";
            EffectPart += "\r\n";
            EffectPart += "        public " + className + "()\r\n";

            // Set Effect Flags
            string flags = (UserControls.Length != 0) ? "EffectFlags.Configurable" : "EffectFlags.None";
            if (renderingFlags.HasFlag(ScriptRenderingFlags.AliasedSelection)) flags += " | EffectFlags.ForceAliasedSelectionQuality";
            if (renderingFlags.HasFlag(ScriptRenderingFlags.SingleThreaded)) flags += " | EffectFlags.SingleThreaded";

            // Rendering Schedule
            string schedule = string.Empty;
            if (renderingSchedule == ScriptRenderingSchedule.HorizontalStrips)
            {
                schedule = ", RenderingSchedule = EffectRenderingSchedule.SmallHorizontalStrips";
            }
            else if (renderingSchedule == ScriptRenderingSchedule.None)
            {
                schedule = ", RenderingSchedule = EffectRenderingSchedule.None";
            }

            EffectPart += "            : base(StaticName, StaticIcon, SubmenuName, new EffectOptions { Flags = " + flags + schedule + " })\r\n";
            EffectPart += "        {\r\n";

            if (UserControls.Any(u => u.ElementType == ElementType.ReseedButton))
            {
                // if we have a random number generator control, include the following line...
                EffectPart += "            instanceSeed = unchecked((int)DateTime.Now.Ticks);\r\n";
            }

            EffectPart += "        }\r\n";
            EffectPart += "\r\n";
            return EffectPart;
        }

        private static string SetRenderPart(UIElement[] UserControls, bool toDll, bool PreRenderExists)
        {
            string SetRenderPart = "";
            if (!toDll && !PreRenderExists)
            {
                return SetRenderPart;
            }

            string tokenType = (toDll) ? "PropertyBasedEffectConfigToken" : "EffectConfigToken";
            SetRenderPart += "        protected override void OnSetRenderInfo(" + tokenType + " token, RenderArgs dstArgs, RenderArgs srcArgs)\r\n";
            SetRenderPart += "        {\r\n";

            if (toDll && UserControls.Length > 0)
            {
                SetRenderPart += CommonWriter.TokenValuesPart(UserControls, "token");
            }

            if (PreRenderExists)
            {
                SetRenderPart += "            PreRender(dstArgs.Surface, srcArgs.Surface);\r\n";
                SetRenderPart += "\r\n";
            }

            SetRenderPart += "            base.OnSetRenderInfo(token, dstArgs, srcArgs);\r\n";
            SetRenderPart += "        }\r\n";
            SetRenderPart += "\r\n";
            return SetRenderPart;
        }

        private static string RenderLoopPart(UIElement[] UserControls)
        {
            bool hasReseed = UserControls.Any(u => u.ElementType == ElementType.ReseedButton);

            string RenderLoopPart = "";
            RenderLoopPart += "        protected override unsafe void OnRender(Rectangle[] rois, int startIndex, int length)\r\n";
            RenderLoopPart += "        {\r\n";
            RenderLoopPart += "            if (length == 0) return;\r\n";

            if (hasReseed)
            {
                // if we have a random number generator control, include the following line...
                RenderLoopPart += "            RandomNumber = GetRandomNumberGenerator(rois, startIndex);\r\n";
            }

            RenderLoopPart += "            for (int i = startIndex; i < startIndex + length; ++i)\r\n";
            RenderLoopPart += "            {\r\n";
            RenderLoopPart += "                Render(DstArgs.Surface,SrcArgs.Surface,rois[i]);\r\n";
            RenderLoopPart += "            }\r\n";
            RenderLoopPart += "        }\r\n";
            RenderLoopPart += "\r\n";

            if (hasReseed)
            {
                // if we have a random number generator control, include the following line...
                RenderLoopPart += "        private Random GetRandomNumberGenerator(Rectangle[] rois, int startIndex)\r\n";
                RenderLoopPart += "        {\r\n";
                RenderLoopPart += "            Rectangle roi = rois[startIndex];\r\n";
                RenderLoopPart += "            return new Random(instanceSeed ^ (randomSeed << 16) ^ (roi.X << 8) ^ roi.Y);\r\n";
                RenderLoopPart += "        }\r\n";
                RenderLoopPart += "\r\n";
            }

            return RenderLoopPart;
        }

        internal static string FullSourceCode(string SourceCode, string FileName, bool isAdjustment, string subMenuName, string menuName, string iconPath, string SupportURL, ScriptRenderingFlags renderingFlags, ScriptRenderingSchedule renderingSchedule, string Author, int MajorVersion, int MinorVersion, string Description, string KeyWords, string WindowTitleStr, HelpType HelpType, string HelpText)
        {
            UIElement[] UserControls = UIElement.ProcessUIControls(SourceCode);
            bool hasPreRender = HasPreRender(SourceCode);

            string sUsingPart = UsingPartCode();
            string sAssemblyInfoPart = CommonWriter.AssemblyInfoPart(FileName, menuName, Author, MajorVersion, MinorVersion, Description, KeyWords);
            string sNamespacePart = CommonWriter.NamespacePart(FileName);
            string sSupportInfoPart = CommonWriter.SupportInfoPart(menuName, SupportURL);
            string sCategoryPart = CommonWriter.CategoryPart(isAdjustment);
            string sEffectPart = ConstructorPart(UserControls, FileName, subMenuName, menuName, iconPath, renderingFlags, renderingSchedule);
            string sHelpPart = CommonWriter.HelpPart(HelpType, HelpText);
            string sPropertyPart = CommonWriter.PropertyPart(UserControls, FileName, WindowTitleStr, HelpType, HelpText, ProjectType.ClassicEffect);
            string sSetRenderPart = SetRenderPart(UserControls, true, hasPreRender);
            string sRenderLoopPart = RenderLoopPart(UserControls);
            string sUserEnteredPart = CommonWriter.UserEnteredPart(SourceCode);
            string sEndPart = CommonWriter.EndPart();

            return sUsingPart + sAssemblyInfoPart + sNamespacePart + sSupportInfoPart + sCategoryPart + sEffectPart + sHelpPart + sPropertyPart + sSetRenderPart + sRenderLoopPart + sUserEnteredPart + sEndPart;
        }

        internal static string FullPreview(string scriptText)
        {
            const string FileName = "PreviewEffect";
            UIElement[] UserControls = UIElement.ProcessUIControls(scriptText);

            return
                ClassicEffectWriter.UsingPartCode() +
                CommonWriter.NamespacePart(FileName) +
                ClassicEffectWriter.ConstructorPart(UserControls, FileName, "FULL UI PREVIEW - Temporarily renders to canvas", FileName, string.Empty, ScriptRenderingFlags.None, ScriptRenderingSchedule.Default) +
                CommonWriter.PropertyPart(UserControls, FileName, string.Empty, HelpType.None, string.Empty, ProjectType.ClassicEffect) +
                ClassicEffectWriter.SetRenderPart(UserControls, true, ClassicEffectWriter.HasPreRender(scriptText)) +
                ClassicEffectWriter.RenderLoopPart(UserControls) +
                CommonWriter.UserEnteredPart(scriptText) +
                CommonWriter.EndPart();
        }

        internal static string UiPreview(string uiCode)
        {
            const string FileName = "UiPreviewEffect";
            uiCode = "#region UICode\r\n" + uiCode + "\r\n#endregion\r\n";

            UIElement[] UserControls = UIElement.ProcessUIControls(uiCode);

            return
                ClassicEffectWriter.UsingPartCode() +
                CommonWriter.NamespacePart(FileName) +
                ClassicEffectWriter.ConstructorPart(UserControls, FileName, string.Empty, "UI PREVIEW - Does NOT Render to canvas", string.Empty, ScriptRenderingFlags.None, ScriptRenderingSchedule.Default) +
                CommonWriter.PropertyPart(UserControls, FileName, string.Empty, HelpType.None, string.Empty, ProjectType.ClassicEffect) +
                ClassicEffectWriter.RenderLoopPart(UserControls) +
                uiCode +
                ClassicEffectWriter.EmptyCode +
                CommonWriter.EndPart();
        }

        internal static string Run(string scriptText, bool debug)
        {
            return
                ClassicEffectWriter.UsingPartCode() +
                ClassicEffectWriter.prepend_code +
                CommonWriter.ConstructorBodyPart(debug) +
                ClassicEffectWriter.SetRenderPart(Array.Empty<UIElement>(), false, ClassicEffectWriter.HasPreRender(scriptText)) +
                CommonWriter.UserEnteredPart(scriptText) +
                CommonWriter.EndPart();
        }

        private static bool HasPreRender(string sourceCode)
        {
            return Regex.IsMatch(sourceCode, @"void\s+PreRender\s*\(\s*Surface\s+dst\s*,\s*Surface\s+src\s*\)\s*{(.|\s)*}", RegexOptions.Singleline);
        }
    }
}
