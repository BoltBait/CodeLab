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
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace PaintDotNet.Effects
{
    internal static class ScriptWriter
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
            + "    Rectangle selection = EnvironmentParameters.GetSelection(src.Bounds).GetBoundsInt();\r\n"
            + "    int CenterX = ((selection.Right - selection.Left) / 2) + selection.Left;\r\n"
            + "    int CenterY = ((selection.Bottom - selection.Top) / 2) + selection.Top;\r\n"
            + "    ColorBgra PrimaryColor = EnvironmentParameters.PrimaryColor;\r\n"
            + "    ColorBgra SecondaryColor = EnvironmentParameters.SecondaryColor;\r\n"
            + "    int BrushWidth = (int)EnvironmentParameters.BrushWidth;\r\n"
            + "\r\n"
            + "    ColorBgra CurrentPixel;\r\n"
            + "    for (int y = rect.Top; y < rect.Bottom; y++)\r\n"
            + "    {\r\n"
            + "        if (IsCancelRequested) return;\r\n"
            + "        for (int x = rect.Left; x < rect.Right; x++)\r\n"
            + "        {\r\n"
            + "            CurrentPixel = src[x,y];\r\n"
            + "            // TODO: Add pixel processing code here\r\n"
            + "            // Access RGBA values this way, for example:\r\n"
            + "            // CurrentPixel.R = PrimaryColor.R;\r\n"
            + "            // CurrentPixel.G = PrimaryColor.G;\r\n"
            + "            // CurrentPixel.B = PrimaryColor.B;\r\n"
            + "            // CurrentPixel.A = PrimaryColor.A;\r\n"
            + "            dst[x,y] = CurrentPixel;\r\n"
            + "        }\r\n"
            + "    }\r\n"
            + "}\r\n";
        internal const string EmptyCode = "\r\n"
            + "void Render(Surface dst, Surface src, Rectangle rect) { }\r\n";
        internal const string UsingPartCode = ""
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
            + "using System.Text.RegularExpressions;\r\n"
            + "using System.Runtime.InteropServices;\r\n"
            + "using Registry = Microsoft.Win32.Registry;\r\n"
            + "using RegistryKey = Microsoft.Win32.RegistryKey;\r\n"
            + "using PaintDotNet;\r\n"
            + "using PaintDotNet.Effects;\r\n"
            + "using PaintDotNet.AppModel;\r\n"
            + "using PaintDotNet.Clipboard;\r\n"
            + "using PaintDotNet.IndirectUI;\r\n"
            + "using PaintDotNet.Collections;\r\n"
            + "using PaintDotNet.PropertySystem;\r\n"
            + "using IntSliderControl = System.Int32;\r\n"
            + "using CheckboxControl = System.Boolean;\r\n"
            + "using ColorWheelControl = PaintDotNet.ColorBgra;\r\n"
            + "using AngleControl = System.Double;\r\n"
            + "using PanSliderControl = PaintDotNet.Pair<double,double>;\r\n"
            + "using TextboxControl = System.String;\r\n"
            + "using FilenameControl = System.String;\r\n"
            + "using DoubleSliderControl = System.Double;\r\n"
            + "using ListBoxControl = System.Byte;\r\n"
            + "using RadioButtonControl = System.Byte;\r\n"
            + "using ReseedButtonControl = System.Byte;\r\n"
            + "using MultiLineTextboxControl = System.String;\r\n"
            + "using RollControl = System.Tuple<double, double, double>;\r\n"
            + "\r\n";
        internal const string prepend_code = "\r\n"
            + "namespace PaintDotNet.Effects\r\n"
            + "{\r\n"
            + "public class UserScript : Effect\r\n"
            + "{\r\n"
            + "    public StringWriter __debugWriter = new StringWriter();\r\n"
            + "    TextWriterTraceListener __listener;\r\n"
            + "\r\n"
            + "    public string __DebugMsgs\r\n"
            + "    {\r\n"
            + "        get\r\n"
            + "        {\r\n"
            + "            return __debugWriter.ToString();\r\n"
            + "        }\r\n"
            + "    }\r\n"
            + "\r\n"
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
            + "        : base(\"UserScript\", null, string.Empty, new EffectOptions())\r\n"
            + "    {\r\n"
            + "        __listener = new TextWriterTraceListener(__debugWriter);\r\n"
            + "        Debug.Listeners.Add(__listener);\r\n"
            + "    }\r\n"
            + "\r\n";
        internal const string append_code = ""
            + "    }\r\n"
            + "}\r\n";

        internal static string AssemblyInfoPart(string FileName, string menuname, string Author, int MajorVersion, int MinorVersion, string Description, string KeyWords)
        {
            // Replace quotes with single quotes from Attribute fields
            Description = Description.Replace('"', '\'');
            KeyWords = KeyWords.Replace('"', '\'');
            Author = Author.Replace('"', '\'');
            menuname = menuname.Replace('"', '\'');
            Description = Description.Trim();
            if (Description.Length == 0)
            {
                Description = menuname + " selected pixels";
            }
            if (KeyWords.Length == 0)
            {
                KeyWords = menuname;
            }

            string AssemblyInfoPart = "";
            AssemblyInfoPart += "[assembly: AssemblyTitle(\"" + FileName + " plugin for Paint.NET\")]\r\n";
            AssemblyInfoPart += "[assembly: AssemblyDescription(\"" + Description + "\")]\r\n";
            AssemblyInfoPart += "[assembly: AssemblyConfiguration(\"" + KeyWords.ToLower() + "\")]\r\n";
            AssemblyInfoPart += "[assembly: AssemblyCompany(\"" + Author + "\")]\r\n";
            AssemblyInfoPart += "[assembly: AssemblyProduct(\"" + FileName + "\")]\r\n";
            AssemblyInfoPart += "[assembly: AssemblyCopyright(\"Copyright ©" + DateTime.Now.Year.ToString();
            if (Author != "")
            {
                AssemblyInfoPart += " by " + Author;
            }
            AssemblyInfoPart += "\")]\r\n";
            AssemblyInfoPart += "[assembly: AssemblyTrademark(\"\")]\r\n";
            AssemblyInfoPart += "[assembly: AssemblyCulture(\"\")]\r\n";
            AssemblyInfoPart += "[assembly: ComVisible(false)]\r\n";
            AssemblyInfoPart += "[assembly: AssemblyVersion(\"" + MajorVersion.ToString() + "." + MinorVersion.ToString() + ".*\")]\r\n";
            AssemblyInfoPart += "\r\n";
            return AssemblyInfoPart;
        }

        internal static string NamespacePart(string FileName)
        {
            // Remove non-alpha characters from namespace
            string NameSpace = Regex.Replace(FileName, @"[^\w]", "") + "Effect";

            string NamespacePart = "";
            NamespacePart += "namespace " + NameSpace + "\r\n";
            NamespacePart += "{\r\n";
            return NamespacePart;
        }

        internal static string SupportInfoPart(string menuname, string SupportURL)
        {
            menuname = menuname.Replace('"', '\'');
            SupportURL = SupportURL.Trim();
            if (!SupportURL.IsWebAddress())
            {
                SupportURL = "https://www.getpaint.net/redirect/plugins.html";
            }

            string SupportInfoPart = "";
            SupportInfoPart += "    public class PluginSupportInfo : IPluginSupportInfo\r\n";
            SupportInfoPart += "    {\r\n";
            SupportInfoPart += "        public string Author\r\n";
            SupportInfoPart += "        {\r\n";
            SupportInfoPart += "            get\r\n";
            SupportInfoPart += "            {\r\n";
            SupportInfoPart += "                return base.GetType().Assembly.GetCustomAttribute<AssemblyCopyrightAttribute>().Copyright;\r\n";
            SupportInfoPart += "            }\r\n";
            SupportInfoPart += "        }\r\n";
            SupportInfoPart += "\r\n";
            SupportInfoPart += "        public string Copyright\r\n";
            SupportInfoPart += "        {\r\n";
            SupportInfoPart += "            get\r\n";
            SupportInfoPart += "            {\r\n";
            SupportInfoPart += "                return base.GetType().Assembly.GetCustomAttribute<AssemblyDescriptionAttribute>().Description;\r\n";
            SupportInfoPart += "            }\r\n";
            SupportInfoPart += "        }\r\n";
            SupportInfoPart += "\r\n";
            SupportInfoPart += "        public string DisplayName\r\n";
            SupportInfoPart += "        {\r\n";
            SupportInfoPart += "            get\r\n";
            SupportInfoPart += "            {\r\n";
            SupportInfoPart += "                return base.GetType().Assembly.GetCustomAttribute<AssemblyProductAttribute>().Product;\r\n";
            SupportInfoPart += "            }\r\n";
            SupportInfoPart += "        }\r\n";
            SupportInfoPart += "\r\n";
            SupportInfoPart += "        public Version Version\r\n";
            SupportInfoPart += "        {\r\n";
            SupportInfoPart += "            get\r\n";
            SupportInfoPart += "            {\r\n";
            SupportInfoPart += "                return base.GetType().Assembly.GetName().Version;\r\n";
            SupportInfoPart += "            }\r\n";
            SupportInfoPart += "        }\r\n";
            SupportInfoPart += "\r\n";
            SupportInfoPart += "        public Uri WebsiteUri\r\n";
            SupportInfoPart += "        {\r\n";
            SupportInfoPart += "            get\r\n";
            SupportInfoPart += "            {\r\n";
            SupportInfoPart += "                return new Uri(\"" + SupportURL + "\");\r\n";
            SupportInfoPart += "            }\r\n";
            SupportInfoPart += "        }\r\n";
            SupportInfoPart += "    }\r\n";
            SupportInfoPart += "\r\n";
            SupportInfoPart += "    [PluginSupportInfo(typeof(PluginSupportInfo), DisplayName = \"" + menuname + "\")]\r\n";
            return SupportInfoPart;
        }

        internal static string CategoryPart(bool isAdjustment)
        {
            string CategoryPart = "";  // Default is to place our plugin in the Effects menu
            if (isAdjustment)
            {
                // Place our plugin in the Adjustments menu
                CategoryPart = "    [EffectCategory(EffectCategory.Adjustment)]\r\n";
            }
            return CategoryPart;
        }

        internal static string EffectPart(UIElement[] UserControls, string FileName, string submenuname, string menuname, string iconpath, EffectFlags effectFlags, EffectRenderingSchedule renderingSchedule)
        {
            menuname = menuname.Trim().Replace('"', '\'');
            if (menuname.Length == 0)
            {
                menuname = FileName;
            }
            string NameSpace = Regex.Replace(FileName, @"[^\w]", "") + "Effect";
            string iconResName = Path.GetFileName(iconpath);

            string EffectPart = "";
            EffectPart += "    public class " + NameSpace + "Plugin : PropertyBasedEffect\r\n";
            EffectPart += "    {\r\n";
            EffectPart += "        public static string StaticName\r\n";
            EffectPart += "        {\r\n";
            EffectPart += "            get\r\n";
            EffectPart += "            {\r\n";
            EffectPart += "                return \"" + menuname + "\";\r\n";
            EffectPart += "            }\r\n";
            EffectPart += "        }\r\n";
            EffectPart += "\r\n";
            EffectPart += "        public static Image StaticIcon\r\n";
            EffectPart += "        {\r\n";
            EffectPart += "            get\r\n";
            EffectPart += "            {\r\n";
            if (File.Exists(iconpath))
            {
                EffectPart += "                return new Bitmap(typeof(" + NameSpace + "Plugin), \"" + iconResName + "\");\r\n";
            }
            else
            {
                EffectPart += "                return null;\r\n";
            }
            EffectPart += "            }\r\n";
            EffectPart += "        }\r\n";

            EffectPart += "\r\n";
            EffectPart += "        public static string SubmenuName\r\n";
            EffectPart += "        {\r\n";
            EffectPart += "            get\r\n";
            EffectPart += "            {\r\n";
            if (submenuname.Trim().Length == 0)
            {
                EffectPart += "                return null;\r\n";
            }
            else
            {
                EffectPart += "                return ";
                switch (submenuname.Trim().ToLower())
                {
                    case "artistic":
                        EffectPart += "SubmenuNames.Artistic;\r\n";
                        break;
                    case "blurs":
                        EffectPart += "SubmenuNames.Blurs;\r\n";
                        break;
                    case "distort":
                        EffectPart += "SubmenuNames.Distort;\r\n";
                        break;
                    case "noise":
                        EffectPart += "SubmenuNames.Noise;\r\n";
                        break;
                    case "photo":
                        EffectPart += "SubmenuNames.Photo;\r\n";
                        break;
                    case "render":
                        EffectPart += "SubmenuNames.Render;\r\n";
                        break;
                    case "stylize":
                        EffectPart += "SubmenuNames.Stylize;\r\n";
                        break;
                    default:
                        // If a non-builtin submenu name is specified, add that in
                        EffectPart += "\"" + submenuname.Trim().Replace('"', '\'') + "\";\r\n";
                        break;
                }
            }
            EffectPart += "            }\r\n";
            EffectPart += "        }\r\n";
            EffectPart += "\r\n";
            EffectPart += "        public " + NameSpace + "Plugin()\r\n";

            // Set Effect Flags
            string flags = (UserControls.Length != 0) ? "EffectFlags.Configurable" : "EffectFlags.None";
            if (effectFlags.HasFlag(EffectFlags.ForceAliasedSelectionQuality)) flags += " | EffectFlags.ForceAliasedSelectionQuality";
            if (effectFlags.HasFlag(EffectFlags.SingleThreaded)) flags += " | EffectFlags.SingleThreaded";

            // Rendering Schedule
            string schedule = string.Empty;
            if (renderingSchedule == EffectRenderingSchedule.SmallHorizontalStrips)
            {
                schedule = ", RenderingSchedule = EffectRenderingSchedule.SmallHorizontalStrips";
            }
            else if (renderingSchedule == EffectRenderingSchedule.None)
            {
                schedule = ", RenderingSchedule = EffectRenderingSchedule.None";
            }

            EffectPart += "            : base(StaticName, StaticIcon, SubmenuName, new EffectOptions() { Flags = " + flags + schedule + " })\r\n";
            EffectPart += "        {\r\n";
            foreach (UIElement u in UserControls)
            {
                if (u.ElementType == ElementType.ReseedButton)
                {
                    // if we have a random number generator control, include the following line...
                    EffectPart += "            instanceSeed = unchecked((int)DateTime.Now.Ticks);\r\n";
                    // ... only once!
                    break;
                }
            }
            EffectPart += "        }\r\n";
            EffectPart += "\r\n";
            return EffectPart;
        }

        internal static string PropertyPart(UIElement[] UserControls, string FileName, string WindowTitleStr, HelpType HelpType, string HelpText)
        {
            string PropertyPart = "";
            if (UserControls.Length == 0)
            {
                // No controls, so no User Interface. Generate an empty OnCreatePropertyCollection()
                PropertyPart += "        protected override PropertyCollection OnCreatePropertyCollection()\r\n";
                PropertyPart += "        {\r\n";
                PropertyPart += "            return PropertyCollection.CreateEmpty();\r\n";
                PropertyPart += "        }\r\n";
                PropertyPart += "\r\n";

                return PropertyPart;
            }

            // Enumerate the user controls
            PropertyPart += "        public enum PropertyNames\r\n";
            PropertyPart += "        {\r\n";
            int x = 0;
            foreach (UIElement u in UserControls)
            {
                x++;
                if (x != 1)
                {
                    PropertyPart += ",\r\n";
                }
                PropertyPart += "            " + u.Identifier.FirstCharToUpper();
            }
            PropertyPart += "\r\n";

            PropertyPart += "        }\r\n";
            PropertyPart += "\r\n";

            foreach (UIElement u in UserControls)
            {
                if ((u.ElementType == ElementType.DropDown || u.ElementType == ElementType.RadioButtons) &&
                    (u.TEnum == null || !Intelli.IsUserDefinedEnum(u.TEnum)))
                {
                    string identifier = u.Identifier.FirstCharToUpper();
                    PropertyPart += "        public enum " + identifier + "Options\r\n";
                    PropertyPart += "        {\r\n";
                    int z = 0;
                    foreach (string Option in u.ToOptionArray())
                    {
                        z++;
                        if (z != 1)
                        {
                            PropertyPart += ",\r\n";
                        }
                        PropertyPart += "            " + identifier + "Option" + z.ToString();
                    }
                    PropertyPart += "\r\n";
                    PropertyPart += "        }\r\n";
                    PropertyPart += "\r\n";
                }
            }

            // if we have a random number generator control, include the following lines...
            if (UserControls.Any(u => u.ElementType == ElementType.ReseedButton))
            {
                PropertyPart += "        [ThreadStatic]\r\n";
                PropertyPart += "        private static Random RandomNumber;\r\n";
                PropertyPart += "\r\n";
                PropertyPart += "        private int randomSeed;\r\n";
                PropertyPart += "        private int instanceSeed;\r\n";
                PropertyPart += "\r\n";
            }

            // generate OnCreatePropertyCollection()
            PropertyPart += "\r\n";
            PropertyPart += "        protected override PropertyCollection OnCreatePropertyCollection()\r\n";
            PropertyPart += "        {\r\n";

            // Check to see if we're including a color wheel without an alpha slider
            if (UserControls.Any(u => u.ElementType == ElementType.ColorWheel && !u.ColorWheelOptions.HasFlag(ColorWheelOptions.Alpha)))
            {
                PropertyPart += "            ColorBgra PrimaryColor = EnvironmentParameters.PrimaryColor.NewAlpha(byte.MaxValue);\r\n";
                PropertyPart += "            ColorBgra SecondaryColor = EnvironmentParameters.SecondaryColor.NewAlpha(byte.MaxValue);\r\n";
                PropertyPart += "\r\n";
            }

            // Check to see if we're including a user selectable blending mode
            if (UserControls.Any(u => u.ElementType == ElementType.BinaryPixelOp))
            {
                PropertyPart += "            // setup for a user selected blend mode\r\n";
                PropertyPart += "            IEnumLocalizerFactory factory = Services.GetService<IEnumLocalizerFactory>();\r\n";
                PropertyPart += "            IEnumLocalizer blendModeLocalizer = factory.Create(typeof(LayerBlendMode));\r\n";
                PropertyPart += "            IList<ILocalizedEnumValue> blendModes = blendModeLocalizer.GetLocalizedEnumValues();\r\n";
                PropertyPart += "            object[] blendModesArray = blendModes.Select(lev => lev.EnumValue).ToArrayEx();\r\n";
                PropertyPart += "            int defaultBlendModeIndex = blendModesArray.IndexOf(LayerBlendMode.Normal);\r\n";
                PropertyPart += "\r\n";
            }

            PropertyPart += "            List<Property> props = new List<Property>();\r\n";
            PropertyPart += "\r\n";

            int ColorControlCount = 0;

            foreach (UIElement u in UserControls)
            {
                string propertyName = u.Identifier.FirstCharToUpper();

                switch (u.ElementType)
                {
                    case ElementType.IntSlider:
                        PropertyPart += "            props.Add(new Int32Property(PropertyNames." + propertyName + ", " + u.Default.ToString() + ", " + u.Min.ToString() + ", " + u.Max.ToString() + "));\r\n";
                        break;
                    case ElementType.Checkbox:
                        PropertyPart += "            props.Add(new BooleanProperty(PropertyNames." + propertyName + ", " + ((u.Default == 0) ? "false" : "true") + "));\r\n";
                        break;
                    case ElementType.ColorWheel:
                        ColorControlCount++;
                        bool includeAlpha = u.ColorWheelOptions.HasFlag(ColorWheelOptions.Alpha);
                        if (u.StrDefault != "")
                        {
                            if (!includeAlpha) // no alpha slider
                            {
                                if (u.StrDefault == "PrimaryColor" || u.StrDefault == "SecondaryColor")
                                {
                                    PropertyPart += "            props.Add(new Int32Property(PropertyNames." + propertyName + ", ColorBgra.ToOpaqueInt32(" + u.StrDefault + "), 0, 0xffffff));\r\n";
                                }
                                else
                                {
                                    PropertyPart += "            props.Add(new Int32Property(PropertyNames." + propertyName + ", ColorBgra.ToOpaqueInt32(Color." + u.StrDefault + "), 0, 0xffffff));\r\n";
                                }
                            }
                            else // include alpha slider
                            {
                                if (u.StrDefault == "PrimaryColor" || u.StrDefault == "SecondaryColor")
                                {
                                    PropertyPart += "            props.Add(new Int32Property(PropertyNames." + propertyName + ", unchecked((int)EnvironmentParameters." + u.StrDefault + ".Bgra), Int32.MinValue, Int32.MaxValue));\r\n";
                                }
                                else
                                {
                                    PropertyPart += "            props.Add(new Int32Property(PropertyNames." + propertyName + ", unchecked((int)ColorBgra." + u.StrDefault + ".Bgra), Int32.MinValue, Int32.MaxValue));\r\n";
                                }
                            }
                        }
                        else
                        {
                            if (!includeAlpha) // no alpha slider
                            {
                                if (ColorControlCount < 2)
                                {
                                    // First color wheel defaults to Primary Color
                                    PropertyPart += "            props.Add(new Int32Property(PropertyNames." + propertyName + ", ColorBgra.ToOpaqueInt32(PrimaryColor), 0, 0xffffff));\r\n";
                                }
                                else
                                {
                                    // Second color wheel (and beyond) defaults to Secondary Color
                                    PropertyPart += "            props.Add(new Int32Property(PropertyNames." + propertyName + ", ColorBgra.ToOpaqueInt32(SecondaryColor), 0, 0xffffff));\r\n";
                                }
                            }
                            else  // include alpha slider
                            {
                                if (ColorControlCount < 2)
                                {
                                    // First color wheel defaults to Primary Color
                                    PropertyPart += "            props.Add(new Int32Property(PropertyNames." + propertyName + ", unchecked((int)EnvironmentParameters.PrimaryColor.Bgra), Int32.MinValue, Int32.MaxValue));\r\n";
                                }
                                else
                                {
                                    // Second color wheel (and beyond) defaults to Secondary Color
                                    PropertyPart += "            props.Add(new Int32Property(PropertyNames." + propertyName + ", unchecked((int)EnvironmentParameters.SecondaryColor.Bgra), Int32.MinValue, Int32.MaxValue));\r\n";
                                }
                            }
                        }
                        break;
                    case ElementType.AngleChooser:
                    case ElementType.DoubleSlider:
                        PropertyPart += "            props.Add(new DoubleProperty(PropertyNames." + propertyName + ", " + u.dDefault.ToString(CultureInfo.InvariantCulture) + ", " + u.dMin.ToString(CultureInfo.InvariantCulture) + ", " + u.dMax.ToString(CultureInfo.InvariantCulture) + "));\r\n";
                        break;
                    case ElementType.PanSlider:
                        PropertyPart += "            props.Add(new DoubleVectorProperty(PropertyNames." + propertyName + ", Pair.Create(" + u.dMin.ToString("F3", CultureInfo.InvariantCulture) + ", " + u.dMax.ToString("F3", CultureInfo.InvariantCulture) + "), Pair.Create(-1.0, -1.0), Pair.Create(+1.0, +1.0)));\r\n";
                        break;
                    case ElementType.Textbox:
                    case ElementType.MultiLineTextbox:
                        PropertyPart += "            props.Add(new StringProperty(PropertyNames." + propertyName + ", \"\", " + u.Max.ToString() + "));\r\n";
                        break;
                    case ElementType.DropDown:
                    case ElementType.RadioButtons:
                        if (u.TEnum != null && Intelli.IsUserDefinedEnum(u.TEnum))
                        {
                            PropertyPart += "            props.Add(StaticListChoiceProperty.CreateForEnum<" + u.TEnum + ">(PropertyNames." + propertyName + ", " + u.StrDefault + ", false));\r\n";
                        }
                        else if (u.Default > 0)
                        {
                            PropertyPart += "            " + propertyName + "Options " + propertyName + "Default = (Enum.IsDefined(typeof(" + propertyName + "Options), " + u.Default.ToString() + ")) ? (" + propertyName + "Options)" + u.Default.ToString() + " : 0;\r\n";
                            PropertyPart += "            props.Add(StaticListChoiceProperty.CreateForEnum<" + propertyName + "Options>(PropertyNames." + propertyName + ", " + propertyName + "Default, false));\r\n";
                        }
                        else
                        {
                            PropertyPart += "            props.Add(StaticListChoiceProperty.CreateForEnum<" + propertyName + "Options>(PropertyNames." + propertyName + ", 0, false));\r\n";
                        }
                        break;
                    case ElementType.BinaryPixelOp:
                        PropertyPart += "            props.Add(new StaticListChoiceProperty(PropertyNames." + propertyName + ", blendModesArray, defaultBlendModeIndex, false));\r\n";
                        break;
                    case ElementType.FontFamily:
                        PropertyPart += "            FontFamily[] " + propertyName + "FontFamilies = new InstalledFontCollection().Families;\r\n";
                        PropertyPart += "            props.Add(new StaticListChoiceProperty(PropertyNames." + propertyName + ", " + propertyName + "FontFamilies, 0, false));\r\n";
                        break;
                    case ElementType.ReseedButton:
                        PropertyPart += "            props.Add(new Int32Property(PropertyNames." + propertyName + ", 0, 0, 255));\r\n";
                        break;
                    case ElementType.RollBall:
                        PropertyPart += "            props.Add(new DoubleVector3Property(PropertyNames." + propertyName + ", Tuple.Create<double, double, double>(0.0, 0.0, 0.0), Tuple.Create<double, double, double>(-180.0, -180.0, 0.0), Tuple.Create<double, double, double>(180.0, 180.0, 90.0)));\r\n";
                        break;
                    case ElementType.Filename:
                        PropertyPart += "            props.Add(new StringProperty(PropertyNames." + propertyName + ", \"\"));\r\n";
                        break;
                    case ElementType.Uri:
                        PropertyPart += "            props.Add(new UriProperty(PropertyNames." + propertyName + ", new Uri(\"" + u.StrDefault + "\")));\r\n";
                        break;
                }
            }

            if (UserControls.Any(u => u.EnabledWhen))
            {
                PropertyPart += "\r\n";
                PropertyPart += "            List<PropertyCollectionRule> propRules = new List<PropertyCollectionRule>();\r\n";
                PropertyPart += "\r\n";

                foreach (UIElement u in UserControls)
                {
                    if (!u.EnabledWhen || u.EnableIdentifier == u.Identifier) // don't allow pointing to itself
                    {
                        continue;
                    }

                    int index = Array.FindIndex(UserControls, element => element.Identifier == u.EnableIdentifier);

                    if (index < 0)
                    {
                        continue;
                    }

                    UIElement source = UserControls[index];
                    string sourceName = source.Identifier.FirstCharToUpper();
                    string targetPropName = "PropertyNames." + u.Identifier.FirstCharToUpper();
                    string sourcePropName = "PropertyNames." + sourceName;
                    string inverse = (!u.EnableSwap).ToString().ToLower();

                    switch (source.ElementType)
                    {
                        case ElementType.ReseedButton:
                        case ElementType.IntSlider:
                            PropertyPart += "            propRules.Add(new ReadOnlyBoundToValueRule<int, Int32Property>(" + targetPropName + ", " + sourcePropName + ", new[] { 0 }, " + inverse + "));\r\n";
                            break;
                        case ElementType.Checkbox:
                            PropertyPart += "            propRules.Add(new ReadOnlyBoundToBooleanRule(" + targetPropName + ", " + sourcePropName + ", " + inverse + "));\r\n";
                            break;
                        case ElementType.ColorWheel:
                            PropertyPart += "            propRules.Add(new ReadOnlyBoundToValueRule<int, Int32Property>" + targetPropName + ", " + sourcePropName + ", ColorBgra.ToOpaqueInt32(Color.White), " + inverse + "));\r\n";
                            break;
                        case ElementType.DoubleSlider:
                        case ElementType.AngleChooser:
                            PropertyPart += "            propRules.Add(new ReadOnlyBoundToValueRule<double, DoubleProperty>(" + targetPropName + ", " + sourcePropName + ", new[] { 0d }, " + inverse + "));\r\n";
                            break;
                        case ElementType.PanSlider:
                            PropertyPart += "            propRules.Add(new ReadOnlyBoundToValueRule<Pair<double, double>, DoubleVectorProperty>(" + targetPropName + ", " + sourcePropName + ", Pair.Create(0d,0d), " + inverse + "));\r\n";
                            break;
                        case ElementType.Filename:
                        case ElementType.Textbox:
                        case ElementType.MultiLineTextbox:
                            PropertyPart += "            propRules.Add(new ReadOnlyBoundToValueRule<string, StringProperty>(" + targetPropName + ", " + sourcePropName + ", new[] { \"\" }, " + inverse + "));\r\n";
                            break;
                        case ElementType.DropDown:
                        case ElementType.RadioButtons:
                            string sourceEnumName = (source.TEnum != null && Intelli.TryGetEnumNames(source.TEnum, out string[] enumNames)) ?
                                enumNames[0] :
                                sourceName + "Options." + sourceName + "Option1";

                            PropertyPart += "            propRules.Add(new ReadOnlyBoundToValueRule<object, StaticListChoiceProperty>(" + targetPropName + ", " + sourcePropName + ", " + sourceEnumName + ", " + inverse + "));\r\n";
                            break;
                        case ElementType.RollBall:
                            PropertyPart += "            propRules.Add(new ReadOnlyBoundToValueRule<Tuple<double, double, double>, DoubleVector3Property>(" + targetPropName + ", " + sourcePropName + ", Tuple.Create(0d,0d,0d), " + inverse + "));\r\n";
                            break;
                        //BinaryPixelOp
                        //FontFamily
                        //Uri
                    }
                }
                PropertyPart += "\r\n";
                PropertyPart += "            return new PropertyCollection(props, propRules);\r\n";
            }
            else
            {
                PropertyPart += "\r\n";
                PropertyPart += "            return new PropertyCollection(props);\r\n";
            }
            PropertyPart += "        }\r\n";

            // generate OnCreateConfigUI()
            PropertyPart += "\r\n";
            PropertyPart += "        protected override ControlInfo OnCreateConfigUI(PropertyCollection props)\r\n";
            PropertyPart += "        {\r\n";

            if (UserControls.Any(u => u.ElementType == ElementType.BinaryPixelOp))
            {
                PropertyPart += "            // setup for a user selected blend mode\r\n";
                PropertyPart += "            IEnumLocalizerFactory factory = Services.GetService<IEnumLocalizerFactory>();\r\n";
                PropertyPart += "            IEnumLocalizer blendModeLocalizer = factory.Create(typeof(LayerBlendMode));\r\n";
                PropertyPart += "            IList<ILocalizedEnumValue> blendModes = blendModeLocalizer.GetLocalizedEnumValues();\r\n";
                PropertyPart += "\r\n";
            }

            if (UserControls.Any(u => u.ElementType == ElementType.PanSlider))
            {
                PropertyPart += "            Rectangle selection = EnvironmentParameters.GetSelection(EnvironmentParameters.SourceSurface.Bounds).GetBoundsInt();\r\n";
                PropertyPart += "            ImageResource imageResource = ImageResource.FromImage(EnvironmentParameters.SourceSurface.CreateAliasedBitmap(selection));\r\n";
                PropertyPart += "\r\n";
            }

            PropertyPart += "            ControlInfo configUI = CreateDefaultConfigUI(props);\r\n";
            PropertyPart += "\r\n";

            foreach (UIElement u in UserControls)
            {
                string propertyName = u.Identifier.FirstCharToUpper();

                if (u.ElementType == ElementType.Checkbox ||
                    u.ElementType == ElementType.ReseedButton ||
                    u.ElementType == ElementType.Uri)
                {
                    PropertyPart += "            configUI.SetPropertyControlValue(PropertyNames." + propertyName + ", ControlInfoPropertyNames.DisplayName, string.Empty);\r\n";
                }
                else
                {
                    PropertyPart += "            configUI.SetPropertyControlValue(PropertyNames." + propertyName + ", ControlInfoPropertyNames.DisplayName, \"" + u.ToShortName() + "\");\r\n";
                }

                if ((u.ElementType == ElementType.IntSlider || u.ElementType == ElementType.DoubleSlider) && u.Style > 0)
                {
                    switch (u.Style)
                    {
                        case SliderStyle.Hue:
                            PropertyPart += "            configUI.SetPropertyControlValue(PropertyNames." + propertyName + ", ControlInfoPropertyNames.ControlStyle, SliderControlStyle.Hue);\r\n";
                            PropertyPart += "            configUI.SetPropertyControlValue(PropertyNames." + propertyName + ", ControlInfoPropertyNames.RangeWraps, true);\r\n";
                            break;
                        case SliderStyle.HueCentered:
                            PropertyPart += "            configUI.SetPropertyControlValue(PropertyNames." + propertyName + ", ControlInfoPropertyNames.ControlStyle, SliderControlStyle.HueCentered);\r\n";
                            PropertyPart += "            configUI.SetPropertyControlValue(PropertyNames." + propertyName + ", ControlInfoPropertyNames.RangeWraps, true);\r\n";
                            break;
                        case SliderStyle.Saturation:
                            PropertyPart += "            configUI.SetPropertyControlValue(PropertyNames." + propertyName + ", ControlInfoPropertyNames.ControlStyle, SliderControlStyle.SaturationHue);\r\n";
                            break;
                        case SliderStyle.WhiteBlack:
                            PropertyPart += "            configUI.SetPropertyControlValue(PropertyNames." + propertyName + ", ControlInfoPropertyNames.ControlColors, new ColorBgra[] { ColorBgra.White, ColorBgra.Black });\r\n";
                            break;
                        case SliderStyle.BlackWhite:
                            PropertyPart += "            configUI.SetPropertyControlValue(PropertyNames." + propertyName + ", ControlInfoPropertyNames.ControlColors, new ColorBgra[] { ColorBgra.Black, ColorBgra.White });\r\n";
                            break;
                        case SliderStyle.CyanRed:
                            PropertyPart += "            configUI.SetPropertyControlValue(PropertyNames." + propertyName + ", ControlInfoPropertyNames.ControlColors, new ColorBgra[] { ColorBgra.Cyan, ColorBgra.White, ColorBgra.Red });\r\n";
                            break;
                        case SliderStyle.MagentaGreen:
                            PropertyPart += "            configUI.SetPropertyControlValue(PropertyNames." + propertyName + ", ControlInfoPropertyNames.ControlColors, new ColorBgra[] { ColorBgra.Magenta, ColorBgra.White, ColorBgra.Green });\r\n";
                            break;
                        case SliderStyle.YellowBlue:
                            PropertyPart += "            configUI.SetPropertyControlValue(PropertyNames." + propertyName + ", ControlInfoPropertyNames.ControlColors, new ColorBgra[] { ColorBgra.Yellow, ColorBgra.White, ColorBgra.Blue });\r\n";
                            break;
                        case SliderStyle.CyanOrange:
                            PropertyPart += "            configUI.SetPropertyControlValue(PropertyNames." + propertyName + ", ControlInfoPropertyNames.ControlColors, new ColorBgra[] { ColorBgra.Cyan, ColorBgra.White, ColorBgra.Orange });\r\n";
                            break;
                        case SliderStyle.WhiteRed:
                            PropertyPart += "            configUI.SetPropertyControlValue(PropertyNames." + propertyName + ", ControlInfoPropertyNames.ControlColors, new ColorBgra[] { ColorBgra.White, ColorBgra.Red });\r\n";
                            break;
                        case SliderStyle.WhiteGreen:
                            PropertyPart += "            configUI.SetPropertyControlValue(PropertyNames." + propertyName + ", ControlInfoPropertyNames.ControlColors, new ColorBgra[] { ColorBgra.White, ColorBgra.Green });\r\n";
                            break;
                        case SliderStyle.WhiteBlue:
                            PropertyPart += "            configUI.SetPropertyControlValue(PropertyNames." + propertyName + ", ControlInfoPropertyNames.ControlColors, new ColorBgra[] { ColorBgra.White, ColorBgra.Blue });\r\n";
                            break;
                    }
                }

                switch (u.ElementType)
                {
                    case ElementType.Checkbox:
                        PropertyPart += "            configUI.SetPropertyControlValue(PropertyNames." + propertyName + ", ControlInfoPropertyNames.Description, \"" + u.Name + "\");\r\n";
                        break;
                    case ElementType.ColorWheel:
                        PropertyPart += "            configUI.SetPropertyControlType(PropertyNames." + propertyName + ", PropertyControlType.ColorWheel);\r\n";
                        if (u.ColorWheelOptions.HasFlag(ColorWheelOptions.NoReset))
                        {
                            PropertyPart += "            configUI.SetPropertyControlValue(PropertyNames." + propertyName + ", ControlInfoPropertyNames.ShowResetButton, false);\r\n";
                        }
                        break;
                    case ElementType.AngleChooser:
                        PropertyPart += "            configUI.SetPropertyControlType(PropertyNames." + propertyName + ", PropertyControlType.AngleChooser);\r\n";
                        PropertyPart += "            configUI.SetPropertyControlValue(PropertyNames." + propertyName + ", ControlInfoPropertyNames.DecimalPlaces, 3);\r\n";
                        break;
                    case ElementType.PanSlider:
                        PropertyPart += "            configUI.SetPropertyControlValue(PropertyNames." + propertyName + ", ControlInfoPropertyNames.SliderSmallChangeX, 0.05);\r\n";
                        PropertyPart += "            configUI.SetPropertyControlValue(PropertyNames." + propertyName + ", ControlInfoPropertyNames.SliderLargeChangeX, 0.25);\r\n";
                        PropertyPart += "            configUI.SetPropertyControlValue(PropertyNames." + propertyName + ", ControlInfoPropertyNames.UpDownIncrementX, 0.01);\r\n";
                        PropertyPart += "            configUI.SetPropertyControlValue(PropertyNames." + propertyName + ", ControlInfoPropertyNames.SliderSmallChangeY, 0.05);\r\n";
                        PropertyPart += "            configUI.SetPropertyControlValue(PropertyNames." + propertyName + ", ControlInfoPropertyNames.SliderLargeChangeY, 0.25);\r\n";
                        PropertyPart += "            configUI.SetPropertyControlValue(PropertyNames." + propertyName + ", ControlInfoPropertyNames.UpDownIncrementY, 0.01);\r\n";
                        PropertyPart += "            configUI.SetPropertyControlValue(PropertyNames." + propertyName + ", ControlInfoPropertyNames.DecimalPlaces, 3);\r\n";
                        PropertyPart += "            configUI.SetPropertyControlValue(PropertyNames." + propertyName + ", ControlInfoPropertyNames.StaticImageUnderlay, imageResource);\r\n";
                        break;
                    case ElementType.DoubleSlider:
                        PropertyPart += "            configUI.SetPropertyControlValue(PropertyNames." + propertyName + ", ControlInfoPropertyNames.SliderLargeChange, 0.25);\r\n";
                        PropertyPart += "            configUI.SetPropertyControlValue(PropertyNames." + propertyName + ", ControlInfoPropertyNames.SliderSmallChange, 0.05);\r\n";
                        PropertyPart += "            configUI.SetPropertyControlValue(PropertyNames." + propertyName + ", ControlInfoPropertyNames.UpDownIncrement, 0.01);\r\n";
                        if (u.Max < 1000)
                        {
                            PropertyPart += "            configUI.SetPropertyControlValue(PropertyNames." + propertyName + ", ControlInfoPropertyNames.DecimalPlaces, 3);\r\n";
                        }
                        break;
                    case ElementType.RadioButtons:
                        PropertyPart += "            configUI.SetPropertyControlType(PropertyNames." + propertyName + ", PropertyControlType.RadioButton);\r\n";
                        goto case ElementType.DropDown; // Fall Through
                    case ElementType.DropDown:
                        PropertyPart += "            PropertyControlInfo " + propertyName + "Control = configUI.FindControlForPropertyName(PropertyNames." + propertyName + ");\r\n";
                        if (u.TEnum != null && Intelli.TryGetEnumNames(u.TEnum, out string[] enumNames))
                        {
                            string[] displayNames = u.ToOptionArray();
                            int length = Math.Min(enumNames.Length, displayNames.Length);

                            for (int i = 0; i < length; i++)
                            {
                                PropertyPart += "            " + propertyName + "Control.SetValueDisplayName(" + enumNames[i] + ", \"" + displayNames[i].Trim() + "\");\r\n";
                            }
                        }
                        else
                        {
                            byte OptionCount = 0;
                            foreach (string entry in u.ToOptionArray())
                            {
                                OptionCount++;
                                PropertyPart += "            " + propertyName + "Control.SetValueDisplayName(" + propertyName + "Options." + propertyName + "Option" + OptionCount.ToString() + ", \"" + entry.Trim() + "\");\r\n";
                            }
                        }
                        break;
                    case ElementType.BinaryPixelOp:
                        PropertyPart += "            PropertyControlInfo " + propertyName + "blendOpControl = configUI.FindControlForPropertyName(PropertyNames." + propertyName + ");\r\n";
                        PropertyPart += "            foreach (ILocalizedEnumValue blendOpValue in blendModes)\r\n";
                        PropertyPart += "            {\r\n";
                        PropertyPart += "                " + propertyName + "blendOpControl.SetValueDisplayName(blendOpValue.EnumValue, blendOpValue.LocalizedName);\r\n";
                        PropertyPart += "            }\r\n";
                        break;
                    case ElementType.FontFamily:
                        PropertyPart += "            PropertyControlInfo " + propertyName + "FontFamilyControl = configUI.FindControlForPropertyName(PropertyNames." + propertyName + ");\r\n";
                        PropertyPart += "            FontFamily[] " + propertyName + "FontFamilies = new InstalledFontCollection().Families;\r\n";
                        PropertyPart += "            foreach (FontFamily ff in " + propertyName + "FontFamilies)\r\n";
                        PropertyPart += "            {\r\n";
                        PropertyPart += "                " + propertyName + "FontFamilyControl.SetValueDisplayName(ff, ff.Name);\r\n";
                        PropertyPart += "            }\r\n";
                        break;
                    case ElementType.ReseedButton:
                        PropertyPart += "            configUI.SetPropertyControlType(PropertyNames." + propertyName + ", PropertyControlType.IncrementButton);\r\n";
                        PropertyPart += "            configUI.SetPropertyControlValue(PropertyNames." + propertyName + ", ControlInfoPropertyNames.ButtonText, \"" + u.Name + "\");\r\n";
                        break;
                    case ElementType.MultiLineTextbox:
                        PropertyPart += "            configUI.SetPropertyControlType(PropertyNames." + propertyName + ", PropertyControlType.TextBox);\r\n";
                        PropertyPart += "            configUI.SetPropertyControlValue(PropertyNames." + propertyName + ", ControlInfoPropertyNames.Multiline, true);\r\n";
                        break;
                    case ElementType.RollBall:
                        PropertyPart += "            configUI.SetPropertyControlType(PropertyNames." + propertyName + ", PropertyControlType.RollBallAndSliders);\r\n";
                        PropertyPart += "            configUI.SetPropertyControlValue(PropertyNames." + propertyName + ", ControlInfoPropertyNames.UpDownIncrementX, 0.01);\r\n";
                        PropertyPart += "            configUI.SetPropertyControlValue(PropertyNames." + propertyName + ", ControlInfoPropertyNames.UpDownIncrementY, 0.01);\r\n";
                        PropertyPart += "            configUI.SetPropertyControlValue(PropertyNames." + propertyName + ", ControlInfoPropertyNames.UpDownIncrementZ, 0.01);\r\n";
                        PropertyPart += "            configUI.SetPropertyControlValue(PropertyNames." + propertyName + ", ControlInfoPropertyNames.DecimalPlaces, 3);\r\n";
                        break;
                    case ElementType.Filename:
                        PropertyPart += "            configUI.SetPropertyControlType(PropertyNames." + propertyName + ", PropertyControlType.FileChooser);\r\n";
                        PropertyPart += "            configUI.SetPropertyControlValue(PropertyNames." + propertyName + ", ControlInfoPropertyNames.FileTypes, new string[] { " + u.ToAllowableFileTypes() + " });\r\n";
                        PropertyPart += "            configUI.SetPropertyControlValue(PropertyNames." + propertyName + ", ControlInfoPropertyNames.AllowAllFiles, true);\r\n";
                        break;
                    case ElementType.Uri:
                        PropertyPart += "            configUI.SetPropertyControlValue(PropertyNames." + propertyName + ", ControlInfoPropertyNames.Description, \"" + u.Name + "\");\r\n";
                        break;
                }
            }
            PropertyPart += "\r\n";
            PropertyPart += "            return configUI;\r\n";
            PropertyPart += "        }\r\n";
            PropertyPart += "\r\n";

            // generate OnCustomizeConfigUIWindowProperties()
            if (WindowTitleStr.Length > 0 || HelpType == HelpType.Custom || ((HelpText.Length > 0) && (HelpType != HelpType.None)))
            {
                PropertyPart += "        protected override void OnCustomizeConfigUIWindowProperties(PropertyCollection props)\r\n";
                PropertyPart += "        {\r\n";
                if (WindowTitleStr.Length > 0)
                {
                    PropertyPart += "            // Change the effect's window title\r\n";
                    PropertyPart += "            props[ControlInfoPropertyNames.WindowTitle].Value = \"" + WindowTitleStr.Replace("\"", "''") + "\";\r\n";
                }
                if (HelpType == HelpType.Custom)
                {
                    PropertyPart += "            props[ControlInfoPropertyNames.WindowHelpContentType].Value = WindowHelpContentType.CustomViaCallback;\r\n";
                }
                else if ((HelpText.Length > 0) && (HelpType != HelpType.None))
                {
                    string helpContent = HelpText.Replace('"', '\'').Replace("\n", "\\n").Replace("\r", "").Replace("\t", "\\t").Replace("\"", "\\\"");

                    PropertyPart += "            // Add help button to effect UI\r\n";
                    if (HelpType == HelpType.URL)
                    {
                        PropertyPart += "            props[ControlInfoPropertyNames.WindowHelpContentType].Value = WindowHelpContentType.CustomViaCallback;\r\n";
                        PropertyPart += "            props[ControlInfoPropertyNames.WindowHelpContent].Value = \"" + helpContent.ToLower() + "\";\r\n";
                    }
                    else if (HelpType == HelpType.PlainText)
                    {
                        PropertyPart += "            props[ControlInfoPropertyNames.WindowHelpContentType].Value = WindowHelpContentType.PlainText;\r\n";
                        PropertyPart += "            props[ControlInfoPropertyNames.WindowHelpContent].Value = \"" + helpContent + "\";\r\n";
                    }
                    else if (HelpType == HelpType.RichText)
                    {
                        // We have to add custom help
                        string NameSpace = Regex.Replace(FileName, @"[^\w]", "") + "Effect";
                        PropertyPart += "            props[ControlInfoPropertyNames.WindowHelpContentType].Value = WindowHelpContentType.CustomViaCallback;\r\n";
                        PropertyPart += "            props[ControlInfoPropertyNames.WindowHelpContent].Value = \"" + NameSpace + "." + helpContent + "\";\r\n";
                    }
                }
                PropertyPart += "            base.OnCustomizeConfigUIWindowProperties(props);\r\n";
                PropertyPart += "        }\r\n";
                PropertyPart += "\r\n";
            }

            return PropertyPart;
        }

        internal static string HelpPart(HelpType HelpType, string HelpText)
        {
            if (HelpType == HelpType.PlainText || HelpType == HelpType.None || HelpType == HelpType.Custom ||  HelpText.Length == 0)
            {
                return string.Empty;
            }

            string HelpPart = "";
            if (HelpType == HelpType.URL)
            {
                HelpPart += "        private void OnWindowHelpButtonClicked(IWin32Window owner, string helpContent)\r\n";
                HelpPart += "        {\r\n";
                HelpPart += "            if (helpContent.StartsWith(\"http://\", StringComparison.OrdinalIgnoreCase) || helpContent.StartsWith(\"https://\", StringComparison.OrdinalIgnoreCase))\r\n";
                HelpPart += "            {\r\n";
                HelpPart += "                Services.GetService<IShellService>().LaunchUrl(null, helpContent);\r\n";
                HelpPart += "            }\r\n";
                HelpPart += "        }\r\n";
                HelpPart += "\r\n";
            }
            else if (HelpType == HelpType.RichText)
            {
                HelpPart += "        RichTextBox rtb_HelpBox;\r\n";
                HelpPart += "        Button btn_HelpBoxOKButton;\r\n";
                HelpPart += "\r\n";
                HelpPart += "        private void rtb_LinkClicked(object sender, System.Windows.Forms.LinkClickedEventArgs e)\r\n";
                HelpPart += "        {\r\n";
                HelpPart += "            Services.GetService<IShellService>().LaunchUrl(null, e.LinkText);\r\n";
                HelpPart += "            btn_HelpBoxOKButton.Focus();\r\n";
                HelpPart += "        }\r\n";
                HelpPart += "\r\n";
                HelpPart += "        private static string LoadLocalizedString(string libraryName, uint ident, string defaultText)\r\n";
                HelpPart += "        {\r\n";
                HelpPart += "            IntPtr libraryHandle = GetModuleHandle(libraryName);\r\n";
                HelpPart += "            if (libraryHandle != IntPtr.Zero)\r\n";
                HelpPart += "            {\r\n";
                HelpPart += "                System.Text.StringBuilder sb = new System.Text.StringBuilder(1024);\r\n";
                HelpPart += "                if (LoadString(libraryHandle, ident, sb, 1024) > 0)\r\n";
                HelpPart += "                {\r\n";
                HelpPart += "                    return sb.ToString();\r\n";
                HelpPart += "                }\r\n";
                HelpPart += "            }\r\n";
                HelpPart += "            return defaultText;\r\n";
                HelpPart += "        }\r\n";
                HelpPart += "\r\n";
                HelpPart += "        [DllImport(\"kernel32.dll\", CharSet = CharSet.Unicode)]\r\n";
                HelpPart += "        private static extern IntPtr GetModuleHandle(string lpModuleName);\r\n";
                HelpPart += "\r\n";
                HelpPart += "        [DllImport(\"user32.dll\", CharSet = CharSet.Unicode)]\r\n";
                HelpPart += "        private static extern int LoadString(IntPtr hInstance, uint uID, System.Text.StringBuilder lpBuffer, int nBufferMax);\r\n";
                HelpPart += "\r\n";
                HelpPart += "        public static string DecompressString(string compressedText)\r\n";
                HelpPart += "        {\r\n";
                HelpPart += "            byte[] gZipBuffer = Convert.FromBase64String(compressedText);\r\n";
                HelpPart += "            using (var memoryStream = new MemoryStream())\r\n";
                HelpPart += "            {\r\n";
                HelpPart += "                int dataLength = BitConverter.ToInt32(gZipBuffer, 0);\r\n";
                HelpPart += "                memoryStream.Write(gZipBuffer, 4, gZipBuffer.Length - 4);\r\n";
                HelpPart += "\r\n";
                HelpPart += "                var buffer = new byte[dataLength];\r\n";
                HelpPart += "\r\n";
                HelpPart += "                memoryStream.Position = 0;\r\n";
                HelpPart += "                using (var gZipStream = new GZipStream(memoryStream, CompressionMode.Decompress))\r\n";
                HelpPart += "                {\r\n";
                HelpPart += "                    gZipStream.Read(buffer, 0, buffer.Length);\r\n";
                HelpPart += "                }\r\n";
                HelpPart += "\r\n";
                HelpPart += "                return System.Text.Encoding.UTF8.GetString(buffer);\r\n";
                HelpPart += "            }\r\n";
                HelpPart += "        }\r\n";
                HelpPart += "\r\n";
                HelpPart += "        private void OnWindowHelpButtonClicked(IWin32Window owner, string helpContent)\r\n";
                HelpPart += "        {\r\n";
                HelpPart += "            var assembly = Assembly.GetExecutingAssembly();\r\n";
                HelpPart += "            var resourceName = helpContent; // helpContent has the resource name in it.\r\n";
                HelpPart += "            string result = \"\";\r\n";
                HelpPart += "            using (Stream stream = assembly.GetManifestResourceStream(resourceName))\r\n";
                HelpPart += "            using (StreamReader reader = new StreamReader(stream))\r\n";
                HelpPart += "            {\r\n";
                HelpPart += "                result = reader.ReadToEnd();\r\n";
                HelpPart += "            }\r\n";
                HelpPart += "\r\n";
                HelpPart += "            using (Form form = new Form())\r\n";
                HelpPart += "            {\r\n";
                HelpPart += "                form.SuspendLayout();\r\n";
                HelpPart += "                form.AutoScaleDimensions = new SizeF(96F, 96F);\r\n";
                HelpPart += "                form.AutoScaleMode = AutoScaleMode.Dpi;\r\n";
                HelpPart += "                form.Text = StaticName + \" - \" + LoadLocalizedString(\"user32.dll\",808,\"Help\");\r\n";
                HelpPart += "                form.AutoSize = false;\r\n";
                HelpPart += "                form.ClientSize = new Size(564, 392);\r\n";
                HelpPart += "                form.MinimumSize = new Size(330, 282);\r\n";
                HelpPart += "                form.FormBorderStyle = FormBorderStyle.Sizable;\r\n";
                HelpPart += "                form.ShowInTaskbar = false;\r\n";
                HelpPart += "                form.MinimizeBox = false;\r\n";
                HelpPart += "                form.StartPosition = FormStartPosition.CenterParent;\r\n";
                HelpPart += "                if (StaticIcon != null)\r\n";
                HelpPart += "                {\r\n";
                HelpPart += "                    form.Icon = Icon.FromHandle(((Bitmap)StaticIcon).GetHicon());\r\n";
                HelpPart += "                }\r\n";
                HelpPart += "                else\r\n";
                HelpPart += "                {\r\n";
                HelpPart += "                    form.ShowIcon = false;\r\n";
                HelpPart += "                }\r\n";
                HelpPart += "                btn_HelpBoxOKButton = new Button();\r\n";
                HelpPart += "                btn_HelpBoxOKButton.AutoSize = true;\r\n";
                HelpPart += "                btn_HelpBoxOKButton.Text = LoadLocalizedString(\"user32.dll\",800,\"OK\");\r\n";
                HelpPart += "                btn_HelpBoxOKButton.DialogResult = DialogResult.Cancel;\r\n";
                HelpPart += "                btn_HelpBoxOKButton.Size = new Size(84, 24);\r\n";
                HelpPart += "                btn_HelpBoxOKButton.Location = new Point(472, 359);\r\n";
                HelpPart += "                btn_HelpBoxOKButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;\r\n";
                HelpPart += "                form.Controls.Add(btn_HelpBoxOKButton);\r\n";
                HelpPart += "                rtb_HelpBox = new RichTextBox();\r\n";
                HelpPart += "                rtb_HelpBox.Size = new Size(564, 350);\r\n";
                HelpPart += "                rtb_HelpBox.Location = new Point(0, 0);\r\n";
                HelpPart += "                rtb_HelpBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Right;\r\n";
                HelpPart += "                rtb_HelpBox.DetectUrls = true;\r\n";
                HelpPart += "                rtb_HelpBox.WordWrap = true;\r\n";
                HelpPart += "                rtb_HelpBox.ScrollBars = RichTextBoxScrollBars.ForcedVertical;\r\n";
                HelpPart += "                rtb_HelpBox.BorderStyle = BorderStyle.None;\r\n";
                HelpPart += "                rtb_HelpBox.Font = new Font(rtb_HelpBox.SelectionFont.Name, 10f);\r\n";
                HelpPart += "                rtb_HelpBox.LinkClicked += new LinkClickedEventHandler(rtb_LinkClicked);\r\n";
                HelpPart += "                rtb_HelpBox.ReadOnly = false;\r\n";
                HelpPart += "                rtb_HelpBox.Rtf = DecompressString(result);\r\n";
                HelpPart += "                rtb_HelpBox.ReadOnly = true;\r\n";
                HelpPart += "                form.Controls.Add(rtb_HelpBox);\r\n";
                HelpPart += "                form.ResumeLayout();\r\n";
                HelpPart += "                form.ShowDialog();\r\n";
                HelpPart += "            }\r\n";
                HelpPart += "        }\r\n";
                HelpPart += "\r\n";
            }

            return HelpPart;
        }

        internal static string SetRenderPart(UIElement[] UserControls, bool toDll, bool PreRenderExists)
        {
            string SetRenderPart = "";
            if (!toDll && !PreRenderExists)
            {
                return SetRenderPart;
            }

            string tokenType = (toDll) ? "PropertyBasedEffectConfigToken" : "EffectConfigToken";
            SetRenderPart += "        protected override void OnSetRenderInfo(" + tokenType + " newToken, RenderArgs dstArgs, RenderArgs srcArgs)\r\n";
            SetRenderPart += "        {\r\n";

            if (toDll && UserControls.Length > 0)
            {
                foreach (UIElement u in UserControls)
                {
                    string propertyName = u.Identifier.FirstCharToUpper();
                    switch (u.ElementType)
                    {
                        case ElementType.IntSlider:
                            SetRenderPart += "            " + u.Identifier + " = newToken.GetProperty<Int32Property>(PropertyNames." + propertyName + ").Value;\r\n";
                            break;
                        case ElementType.Checkbox:
                            SetRenderPart += "            " + u.Identifier + " = newToken.GetProperty<BooleanProperty>(PropertyNames." + propertyName + ").Value;\r\n";
                            break;
                        case ElementType.ColorWheel:
                            if (!u.ColorWheelOptions.HasFlag(ColorWheelOptions.Alpha))
                            {
                                SetRenderPart += "            " + u.Identifier + " = ColorBgra.FromOpaqueInt32(newToken.GetProperty<Int32Property>(PropertyNames." + propertyName + ").Value);\r\n";
                            }
                            else
                            {
                                SetRenderPart += "            " + u.Identifier + " = ColorBgra.FromUInt32(unchecked((uint)newToken.GetProperty<Int32Property>(PropertyNames." + propertyName + ").Value));\r\n";
                            }
                            break;
                        case ElementType.AngleChooser:
                        case ElementType.DoubleSlider:
                            SetRenderPart += "            " + u.Identifier + " = newToken.GetProperty<DoubleProperty>(PropertyNames." + propertyName + ").Value;\r\n";
                            break;
                        case ElementType.PanSlider:
                            SetRenderPart += "            " + u.Identifier + " = newToken.GetProperty<DoubleVectorProperty>(PropertyNames." + propertyName + ").Value;\r\n";
                            break;
                        case ElementType.Filename:
                        case ElementType.Textbox:
                        case ElementType.MultiLineTextbox:
                            SetRenderPart += "            " + u.Identifier + " = newToken.GetProperty<StringProperty>(PropertyNames." + propertyName + ").Value;\r\n";
                            break;
                        case ElementType.DropDown:
                        case ElementType.RadioButtons:
                            string typeCast = (u.TEnum != null && Intelli.IsUserDefinedEnum(u.TEnum)) ? u.TEnum : "byte)(int";
                            SetRenderPart += "            " + u.Identifier + " = (" + typeCast + ")newToken.GetProperty<StaticListChoiceProperty>(PropertyNames." + propertyName + ").Value;\r\n";
                            break;
                        case ElementType.BinaryPixelOp:
                            SetRenderPart += "            " + u.Identifier + " = LayerBlendModeUtil.CreateCompositionOp((LayerBlendMode)newToken.GetProperty<StaticListChoiceProperty>(PropertyNames." + propertyName + ").Value);\r\n";
                            break;
                        case ElementType.FontFamily:
                            SetRenderPart += "            " + u.Identifier + " = (FontFamily)newToken.GetProperty<StaticListChoiceProperty>(PropertyNames." + propertyName + ").Value;\r\n";
                            break;
                        case ElementType.ReseedButton:
                            SetRenderPart += "            " + u.Identifier + " = (byte)newToken.GetProperty<Int32Property>(PropertyNames." + propertyName + ").Value;\r\n";
                            SetRenderPart += "            randomSeed = " + u.Identifier + ";\r\n";
                            break;
                        case ElementType.RollBall:
                            SetRenderPart += "            " + u.Identifier + " = newToken.GetProperty<DoubleVector3Property>(PropertyNames." + propertyName + ").Value;\r\n";
                            break;
                    }
                }
                SetRenderPart += "\r\n";
            }

            if (PreRenderExists)
            {
                SetRenderPart += "            PreRender(dstArgs.Surface, srcArgs.Surface);\r\n";
                SetRenderPart += "\r\n";
            }

            SetRenderPart += "            base.OnSetRenderInfo(newToken, dstArgs, srcArgs);\r\n";
            SetRenderPart += "        }\r\n";
            SetRenderPart += "\r\n";
            return SetRenderPart;
        }

        internal static string RenderLoopPart(UIElement[] UserControls)
        {
            string RenderLoopPart = "";
            RenderLoopPart += "        protected override unsafe void OnRender(Rectangle[] rois, int startIndex, int length)\r\n";
            RenderLoopPart += "        {\r\n";
            RenderLoopPart += "            if (length == 0) return;\r\n";

            foreach (UIElement u in UserControls)
            {
                if (u.ElementType == ElementType.ReseedButton)
                {
                    // if we have a random number generator control, include the following line...
                    RenderLoopPart += "            RandomNumber = GetRandomNumberGenerator(rois, startIndex);\r\n";
                    // ... only once!
                    break;
                }
            }

            RenderLoopPart += "            for (int i = startIndex; i < startIndex + length; ++i)\r\n";
            RenderLoopPart += "            {\r\n";
            RenderLoopPart += "                Render(DstArgs.Surface,SrcArgs.Surface,rois[i]);\r\n";
            RenderLoopPart += "            }\r\n";
            RenderLoopPart += "        }\r\n";
            RenderLoopPart += "\r\n";

            foreach (UIElement u in UserControls)
            {
                if (u.ElementType == ElementType.ReseedButton)
                {
                    // if we have a random number generator control, include the following line...
                    RenderLoopPart += "        private Random GetRandomNumberGenerator(Rectangle[] rois, int startIndex)\r\n";
                    RenderLoopPart += "        {\r\n";
                    RenderLoopPart += "            Rectangle roi = rois[startIndex];\r\n";
                    RenderLoopPart += "            return new Random(instanceSeed ^ (randomSeed << 16) ^ (roi.X << 8) ^ roi.Y);\r\n";
                    RenderLoopPart += "        }\r\n";
                    RenderLoopPart += "\r\n";
                    // ... only once!
                    break;
                }
            }
            return RenderLoopPart;
        }

        internal static string UserEnteredPart(string SourceCode)
        {
            SourceCode = Regex.Replace(SourceCode, @"\bRadioButtonControl<(?<TEnum>\S+)>\s+", match => match.Groups["TEnum"].Value + " ");
            SourceCode = Regex.Replace(SourceCode, @"\bListBoxControl<(?<TEnum>\S+)>\s+", match => match.Groups["TEnum"].Value + " ");

            string UserEnteredPart = "";
            UserEnteredPart += "        #region User Entered Code\r\n        ";
            UserEnteredPart += SourceCode.Replace("\n", "\n        ");
            UserEnteredPart += "\r\n";
            UserEnteredPart += "        #endregion\r\n";
            return UserEnteredPart;
        }

        internal static string EndPart()
        {
            return append_code;
        }

        internal static string FullSourceCode(string SourceCode, string FileName, bool isAdjustment, string submenuname, string menuname, string iconpath, string SupportURL, EffectFlags effectFlags, EffectRenderingSchedule renderingSchedule, string Author, int MajorVersion, int MinorVersion, string Description, string KeyWords, string WindowTitleStr, HelpType HelpType, string HelpText)
        {
            UIElement[] UserControls = UIElement.ProcessUIControls(SourceCode);
            Regex preRenderRegex = new Regex(@"void PreRender\(Surface dst, Surface src\)(\s)*{(.|\s)*}", RegexOptions.Singleline);

            string sUsingPart = UsingPartCode;
            string sAssemblyInfoPart = AssemblyInfoPart(FileName, menuname, Author, MajorVersion, MinorVersion, Description, KeyWords);
            string sNamespacePart = NamespacePart(FileName);
            string sSupportInfoPart = SupportInfoPart(menuname, SupportURL);
            string sCategoryPart = CategoryPart(isAdjustment);
            string sEffectPart = EffectPart(UserControls, FileName, submenuname, menuname, iconpath, effectFlags, renderingSchedule);
            string sHelpPart = HelpPart(HelpType, HelpText);
            string sPropertyPart = PropertyPart(UserControls, FileName, WindowTitleStr, HelpType, HelpText);
            string sSetRenderPart = SetRenderPart(UserControls, true, preRenderRegex.IsMatch(SourceCode));
            string sRenderLoopPart = RenderLoopPart(UserControls);
            string sUserEnteredPart = UserEnteredPart(SourceCode);
            string sEndPart = EndPart();

            return sUsingPart + sAssemblyInfoPart + sNamespacePart + sSupportInfoPart + sCategoryPart + sEffectPart + sHelpPart + sPropertyPart + sSetRenderPart + sRenderLoopPart + sUserEnteredPart + sEndPart;
        }
    }
}
