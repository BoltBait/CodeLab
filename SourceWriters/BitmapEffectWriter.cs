using PaintDotNet.Effects.Gpu;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PaintDotNet.Effects
{
    internal static class BitmapEffectWriter
    {
        private const string UsingStatements = ""
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
            + "using PaintDotNet.Direct2D1;\r\n"
            + "using PaintDotNet.Effects;\r\n"
            + "using PaintDotNet.Clipboard;\r\n"
            + "using PaintDotNet.Imaging;\r\n"
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

        private const string prepend_code = "\r\n"
            + "namespace PaintDotNet.Effects\r\n"
            + "{\r\n"
            + "public class UserScript : BitmapEffect\r\n"
            + "{\r\n"
            + "    public UserScript()\r\n"
            + "        : base(\"UserScript\", string.Empty, new BitmapEffectOptions())\r\n";

        private static string ConstructorPart(UIElement[] UserControls, string FileName, string submenuname, string menuname, string iconpath)
        {
            menuname = menuname.Trim().Replace('"', '\'');
            if (menuname.Length == 0)
            {
                menuname = FileName;
            }

            string className = Regex.Replace(FileName, @"[^\w]", "") + "EffectPlugin";

            string icon = File.Exists(iconpath)
                ? "new Bitmap(typeof(" + className + "), \"" + Path.GetFileName(iconpath) + "\")"
                : "null";

            string configurable = (UserControls.Length != 0).ToString().ToLowerInvariant();

            string EffectPart = "";
            EffectPart += "    public class " + className + " : PropertyBasedBitmapEffect\r\n";
            EffectPart += "    {\r\n";
            EffectPart += "        public static string StaticName => \"" + menuname + "\";\r\n";
            EffectPart += "        public static Image StaticIcon => " + icon + ";\r\n";
            EffectPart += "        public static string SubmenuName => " + CommonWriter.SubmenuPart(submenuname) + ";\r\n";
            EffectPart += "\r\n";
            EffectPart += "        public " + className + "()\r\n";
            EffectPart += "            : base(StaticName, StaticIcon, SubmenuName, new BitmapEffectOptions { IsConfigurable = " + configurable + " })\r\n";
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

        private static string InitializeRenderInfoPart(ScriptRenderingFlags renderingFlags, ScriptRenderingSchedule renderingSchedule)
        {
            string renderInfo =
                "        protected override void OnInitializeRenderInfo(IBitmapEffectRenderInfo renderInfo)\r\n" +
                "        {\r\n";

            if (renderingFlags != ScriptRenderingFlags.None)
            {
                List<string> flags= new List<string>();
                if (renderingFlags.HasFlag(ScriptRenderingFlags.SingleThreaded))
                {
                    flags.Add("BitmapEffectRenderingFlags.SingleThreaded");
                }
                if (renderingFlags.HasFlag(ScriptRenderingFlags.NoSelectionClipping))
                {
                    flags.Add("BitmapEffectRenderingFlags.DisableSelectionClipping");
                }
                if (renderingFlags.HasFlag(ScriptRenderingFlags.AliasedSelection))
                {
                    flags.Add("BitmapEffectRenderingFlags.ForceAliasedSelectionQuality");
                }

                renderInfo += "         renderInfo.Flags = " + flags.Join(" | ") + ";\r\n";
            }

            if (renderingSchedule != ScriptRenderingSchedule.SquareTiles)
            {
                string schedule = renderingSchedule switch
                {
                    ScriptRenderingSchedule.None => "BitmapEffectRenderingSchedule.None",
                    ScriptRenderingSchedule.HorizontalStrips => "BitmapEffectRenderingSchedule.HorizontalStrips",
                    _ => "BitmapEffectRenderingSchedule.SquareTiles",
                };
                renderInfo += "         renderInfo.Schedule = " + schedule + ";\r\n";
            }

            renderInfo +=
                "         base.OnInitializeRenderInfo(renderInfo);\r\n" +
                "    }\r\n";

            return renderInfo;
        }

        private static string SetTokenPart(UIElement[] UserControls)
        {
            string setToken =
                "        protected override void OnSetToken(PropertyBasedEffectConfigToken newToken)\r\n" +
                "        {\r\n";

            if (UserControls.Length > 0)
            {
                setToken += CommonWriter.TokenValuesPart(UserControls, "newToken");
            }

            setToken +=
                "         base.OnSetToken(newToken);\r\n" +
                "    }\r\n";

            return setToken;
        }

        internal static string FullSourceCode(string SourceCode, string FileName, bool isAdjustment, string submenuname, string menuname, string iconpath, string SupportURL, ScriptRenderingFlags renderingFlags, ScriptRenderingSchedule renderingSchedule, string Author, int MajorVersion, int MinorVersion, string Description, string KeyWords, string WindowTitleStr, HelpType HelpType, string HelpText)
        {
            UIElement[] UserControls = UIElement.ProcessUIControls(SourceCode);

            return
                BitmapEffectWriter.UsingStatements +
                CommonWriter.AssemblyInfoPart(FileName, menuname, Author, MajorVersion, MinorVersion, Description, KeyWords) +
                CommonWriter.NamespacePart(FileName) +
                CommonWriter.SupportInfoPart(menuname, SupportURL) +
                CommonWriter.CategoryPart(isAdjustment) +
                ConstructorPart(UserControls, FileName, submenuname, menuname, iconpath) +
                CommonWriter.HelpPart(HelpType, HelpText) +
                CommonWriter.PropertyPart(UserControls, FileName, WindowTitleStr, HelpType, HelpText, ProjectType.BitmapEffect) +
                BitmapEffectWriter.InitializeRenderInfoPart(renderingFlags, renderingSchedule) +
                BitmapEffectWriter.SetTokenPart(UserControls) +
                CommonWriter.UserEnteredPart(SourceCode) +
                CommonWriter.EndPart();
        }

        internal static string FullPreview(string scriptText)
        {
            const string FileName = "PreviewEffect";
            UIElement[] UserControls = UIElement.ProcessUIControls(scriptText);

            return
                BitmapEffectWriter.UsingStatements +
                CommonWriter.NamespacePart("UserScript") +
                BitmapEffectWriter.ConstructorPart(UserControls, FileName, string.Empty, FileName, string.Empty) +
                CommonWriter.PropertyPart(UserControls, FileName, "FULL UI PREVIEW - Temporarily renders to canvas", HelpType.None, string.Empty, ProjectType.BitmapEffect) +
                BitmapEffectWriter.SetTokenPart(UserControls) +
                CommonWriter.UserEnteredPart(scriptText) +
                CommonWriter.EndPart();
        }

        internal static string Run(string scriptText, bool debug)
        {
            return
                BitmapEffectWriter.UsingStatements +
                BitmapEffectWriter.prepend_code +
                CommonWriter.ConstructorBodyPart(debug) +
                CommonWriter.UserEnteredPart(scriptText) +
                CommonWriter.EndPart();
        }
    }
}
