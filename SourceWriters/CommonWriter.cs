using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;

namespace PaintDotNet.Effects
{
    internal static class CommonWriter
    {
        internal static string AssemblyInfoPart(string FileName, string menuName, string Author, int MajorVersion, int MinorVersion, string Description, string KeyWords)
        {
            // Replace quotes with single quotes from Attribute fields
            Description = Description.Replace('"', '\'');
            KeyWords = KeyWords.Replace('"', '\'');
            Author = Author.Replace('"', '\'');
            menuName = menuName.Replace('"', '\'');
            Description = Description.Trim();
            if (Description.Length == 0)
            {
                Description = menuName + " selected pixels";
            }
            if (KeyWords.Length == 0)
            {
                KeyWords = menuName;
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
            AssemblyInfoPart += "[assembly: AssemblyMetadata(\"BuiltByCodeLab\", \"Version=" + CodeLab.VersionFull + "\")]\r\n";
            AssemblyInfoPart += "[assembly: SupportedOSPlatform(\"Windows\")]\r\n";
            AssemblyInfoPart += "\r\n";
            return AssemblyInfoPart;
        }

        internal static string NamespacePart(string FileName, bool isFileType = false)
        {
            // Remove non-alpha characters from namespace
            string pluginType = (!isFileType) ? "Effect" : "FileType";
            string NameSpace = Regex.Replace(FileName, @"[^\w]", "") + pluginType;

            string NamespacePart = "";
            NamespacePart += "namespace " + NameSpace + "\r\n";
            NamespacePart += "{\r\n";
            return NamespacePart;
        }

        internal static string SupportInfoPart(string menuName, string SupportURL)
        {
            menuName = menuName.Replace('"', '\'');
            SupportURL = SupportURL.Trim();
            if (!SupportURL.IsWebAddress())
            {
                SupportURL = "https://www.getpaint.net/redirect/plugins.html";
            }

            string SupportInfoPart = "";
            SupportInfoPart += "    public class PluginSupportInfo : IPluginSupportInfo\r\n";
            SupportInfoPart += "    {\r\n";
            SupportInfoPart += "        public string Author => base.GetType().Assembly.GetCustomAttribute<AssemblyCopyrightAttribute>().Copyright;\r\n";
            SupportInfoPart += "        public string Copyright => base.GetType().Assembly.GetCustomAttribute<AssemblyDescriptionAttribute>().Description;\r\n";
            SupportInfoPart += "        public string DisplayName => base.GetType().Assembly.GetCustomAttribute<AssemblyProductAttribute>().Product;\r\n";
            SupportInfoPart += "        public Version Version => base.GetType().Assembly.GetName().Version;\r\n";
            SupportInfoPart += "        public Uri WebsiteUri => new Uri(\"" + SupportURL + "\");\r\n";
            SupportInfoPart += "    }\r\n";
            SupportInfoPart += "\r\n";
            SupportInfoPart += "    [PluginSupportInfo<PluginSupportInfo>(DisplayName = \"" + menuName + "\")]\r\n";
            return SupportInfoPart;
        }

        internal static string CategoryPart(bool isAdjustment)
        {
            return isAdjustment ? "    [EffectCategory(EffectCategory.Adjustment)]\r\n" : string.Empty;
        }

        internal static string PropertyPart(UIElement[] UserControls, string FileName, string WindowTitleStr, HelpType HelpType, string HelpText, ProjectType projectType)
        {
            string PropertyPart = "";
            if (UserControls.Length == 0)
            {
                // No controls, so no User Interface. Generate an empty OnCreatePropertyCollection()
                if (projectType == ProjectType.FileType)
                {
                    PropertyPart += "        public override PropertyCollection OnCreateSavePropertyCollection()\r\n";
                }
                else
                {
                    PropertyPart += "        protected override PropertyCollection OnCreatePropertyCollection()\r\n";
                }
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
                if ((u.ElementType == ElementType.DropDown || u.ElementType == ElementType.RadioButtons) && !Intelli.IsEnum(u.TEnum))
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

            if (projectType == ProjectType.FileType)
            {
                PropertyPart += "        public override PropertyCollection OnCreateSavePropertyCollection()\r\n";
            }
            else
            {
                PropertyPart += "        protected override PropertyCollection OnCreatePropertyCollection()\r\n";
            }

            PropertyPart += "        {\r\n";

            // Check to see if we're including a color wheel without an alpha slider
            if (UserControls.Any(u => u.ElementType == ElementType.ColorWheel && !u.ColorWheelOptions.HasFlag(ColorWheelOptions.Alpha)))
            {
                if (projectType.Is5Effect())
                {
                    PropertyPart += "            ColorBgra32 PrimaryColor = Environment.PrimaryColor with { A = byte.MaxValue };\r\n";
                    PropertyPart += "            ColorBgra32 SecondaryColor = Environment.SecondaryColor with { A = byte.MaxValue };\r\n";
                }
                else
                {
                    PropertyPart += "            ColorBgra PrimaryColor = EnvironmentParameters.PrimaryColor.NewAlpha(byte.MaxValue);\r\n";
                    PropertyPart += "            ColorBgra SecondaryColor = EnvironmentParameters.SecondaryColor.NewAlpha(byte.MaxValue);\r\n";
                }

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

            if (UserControls.Any(u => u.ElementType == ElementType.FontFamily))
            {
                PropertyPart += "            FontFamily[] installedFontFamilies = new InstalledFontCollection().Families;\r\n";
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
                                string environment = projectType.Is5Effect() ? "Environment" : "EnvironmentParameters";

                                if (ColorControlCount < 2)
                                {
                                    // First color wheel defaults to Primary Color
                                    PropertyPart += "            props.Add(new Int32Property(PropertyNames." + propertyName + ", unchecked((int)" + environment + ".PrimaryColor.Bgra), Int32.MinValue, Int32.MaxValue));\r\n";
                                }
                                else
                                {
                                    // Second color wheel (and beyond) defaults to Secondary Color
                                    PropertyPart += "            props.Add(new Int32Property(PropertyNames." + propertyName + ", unchecked((int)" + environment + ".SecondaryColor.Bgra), Int32.MinValue, Int32.MaxValue));\r\n";
                                }
                            }
                        }
                        break;
                    case ElementType.AngleChooser:
                    case ElementType.DoubleSlider:
                        PropertyPart += "            props.Add(new DoubleProperty(PropertyNames." + propertyName + ", " + u.dDefault.ToString(CultureInfo.InvariantCulture) + ", " + u.dMin.ToString(CultureInfo.InvariantCulture) + ", " + u.dMax.ToString(CultureInfo.InvariantCulture) + "));\r\n";
                        break;
                    case ElementType.PanSlider:
                        PropertyPart += "            props.Add(new DoubleVectorProperty(PropertyNames." + propertyName + ", new Vector2Double(" + u.StrDefault + "), new Vector2Double(-1.0, -1.0), new Vector2Double(+1.0, +1.0)));\r\n";
                        break;
                    case ElementType.Textbox:
                    case ElementType.MultiLineTextbox:
                        PropertyPart += "            props.Add(new StringProperty(PropertyNames." + propertyName + ", \"\", " + u.Max.ToString() + "));\r\n";
                        break;
                    case ElementType.DropDown:
                    case ElementType.RadioButtons:
                        if (Intelli.IsEnum(u.TEnum))
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
                        PropertyPart += "            int " + propertyName + "DefaultValueIndex = Array.FindIndex(installedFontFamilies, ff => ff.Name.Equals(\"" + u.StrDefault + "\", StringComparison.OrdinalIgnoreCase));\r\n";
                        PropertyPart += "            if (" + propertyName + "DefaultValueIndex < 0)\r\n";
                        PropertyPart += "            {\r\n";
                        PropertyPart += "                " + propertyName + "DefaultValueIndex = 0;\r\n";
                        PropertyPart += "            }\r\n";
                        PropertyPart += "            props.Add(new StaticListChoiceProperty(PropertyNames." + propertyName + ", installedFontFamilies, " + propertyName + "DefaultValueIndex, false));\r\n";
                        break;
                    case ElementType.ReseedButton:
                        PropertyPart += "            props.Add(new Int32Property(PropertyNames." + propertyName + ", 0, 0, 255));\r\n";
                        break;
                    case ElementType.RollBall:
                        PropertyPart += "            props.Add(new DoubleVector3Property(PropertyNames." + propertyName + ", new Vector3Double(0.0, 0.0, 0.0), new Vector3Double(-180.0, -180.0, 0.0), new Vector3Double(180.0, 180.0, 90.0)));\r\n";
                        break;
                    case ElementType.Filename:
                    case ElementType.Folder:
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
                    string targetPropName = u.Identifier.FirstCharToUpper();
                    string sourcePropName = source.Identifier.FirstCharToUpper();
                    string inverse = (!u.EnableSwap).ToString().ToLower();

                    switch (source.ElementType)
                    {
                        case ElementType.ReseedButton:
                        case ElementType.IntSlider:
                            PropertyPart += "            propRules.Add(new ReadOnlyBoundToValueRule<int, Int32Property>(PropertyNames." + targetPropName + ", PropertyNames." + sourcePropName + ", new[] { 0 }, " + inverse + "));\r\n";
                            break;
                        case ElementType.Checkbox:
                            PropertyPart += "            propRules.Add(new ReadOnlyBoundToBooleanRule(PropertyNames." + targetPropName + ", PropertyNames." + sourcePropName + ", " + inverse + "));\r\n";
                            break;
                        case ElementType.ColorWheel:
                            PropertyPart += "            propRules.Add(new ReadOnlyBoundToValueRule<int, Int32Property>PropertyNames." + targetPropName + ", PropertyNames." + sourcePropName + ", ColorBgra.ToOpaqueInt32(Color.White), " + inverse + "));\r\n";
                            break;
                        case ElementType.DoubleSlider:
                        case ElementType.AngleChooser:
                            PropertyPart += "            propRules.Add(new ReadOnlyBoundToValueRule<double, DoubleProperty>(PropertyNames." + targetPropName + ", PropertyNames." + sourcePropName + ", new[] { 0d }, " + inverse + "));\r\n";
                            break;
                        case ElementType.PanSlider:
                            PropertyPart += "            propRules.Add(new ReadOnlyBoundToValueRule<Vector2Double, DoubleVectorProperty>(PropertyNames." + targetPropName + ", PropertyNames." + sourcePropName + ", new Vector2Double(0d,0d), " + inverse + "));\r\n";
                            break;
                        case ElementType.Filename:
                        case ElementType.Folder:
                        case ElementType.Textbox:
                        case ElementType.MultiLineTextbox:
                            PropertyPart += "            propRules.Add(new ReadOnlyBoundToValueRule<string, StringProperty>(PropertyNames." + targetPropName + ", PropertyNames." + sourcePropName + ", new[] { \"\" }, " + inverse + "));\r\n";
                            break;
                        case ElementType.DropDown:
                        case ElementType.RadioButtons:
                            string sourceEnumName = Intelli.TryGetEnumNames(source.TEnum, out string[] enumNames) ?
                                enumNames[0] :
                                sourcePropName + "Options." + sourcePropName + "Option1";

                            PropertyPart += "            propRules.Add(new ReadOnlyBoundToValueRule<object, StaticListChoiceProperty>(PropertyNames." + targetPropName + ", PropertyNames." + sourcePropName + ", " + sourceEnumName + ", " + inverse + "));\r\n";
                            break;
                        case ElementType.RollBall:
                            PropertyPart += "            propRules.Add(new ReadOnlyBoundToValueRule<Vector3Double, DoubleVector3Property>(PropertyNames." + targetPropName + ", PropertyNames." + sourcePropName + ", new Vector3Double(0d,0d,0d), " + inverse + "));\r\n";
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

            if (projectType == ProjectType.FileType)
            {
                PropertyPart += "        public override ControlInfo OnCreateSaveConfigUI(PropertyCollection props)\r\n";
            }
            else
            {
                PropertyPart += "        protected override ControlInfo OnCreateConfigUI(PropertyCollection props)\r\n";
            }

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
                if (projectType.Is5Effect())
                {
                    PropertyPart += "            RectInt32 selection = Environment.Selection.RenderBounds;\r\n";
                    PropertyPart += "            IBitmapSource<ColorBgra32> panImage = Environment.GetSourceBitmapBgra32().CreateClipper(selection);\r\n";
                }
                else
                {
                    PropertyPart += "            Rectangle selection = EnvironmentParameters.SelectionBounds;\r\n";
                    PropertyPart += "            ImageResource panImage = ImageResource.FromImage(EnvironmentParameters.SourceSurface.CreateAliasedBitmap(selection));\r\n";
                }

                PropertyPart += "\r\n";
            }

            if (projectType == ProjectType.FileType)
            {
                PropertyPart += "            ControlInfo configUI = CreateDefaultSaveConfigUI(props);\r\n";
            }
            else
            {
                PropertyPart += "            ControlInfo configUI = CreateDefaultConfigUI(props);\r\n";
            }

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
                        PropertyPart += "            configUI.SetPropertyControlValue(PropertyNames." + propertyName + ", ControlInfoPropertyNames.StaticImageUnderlay, panImage);\r\n";
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
                        if (Intelli.TryGetEnumNames(u.TEnum, out string[] enumNames))
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
                    case ElementType.Folder:
                        PropertyPart += "            configUI.SetPropertyControlType(PropertyNames." + propertyName + ", PropertyControlType.FolderChooser);\r\n";
                        break;
                }
                PropertyPart += "            configUI.SetPropertyControlValue(PropertyNames." + propertyName + ", ControlInfoPropertyNames.ShowHeaderLine, false);\r\n";
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
            if (HelpType == HelpType.PlainText || HelpType == HelpType.None || HelpType == HelpType.Custom || HelpText.Length == 0)
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
                HelpPart += "                    gZipStream.ReadExactly(buffer, 0, buffer.Length);\r\n";
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

        internal static string TokenValuesPart(UIElement[] userControls, string tokenName)
        {
            string tokenValues = "";
            foreach (UIElement u in userControls)
            {
                string propertyName = u.Identifier.FirstCharToUpper();
                switch (u.ElementType)
                {
                    case ElementType.IntSlider:
                        tokenValues += $"            {u.Identifier} = {tokenName}.GetProperty<Int32Property>(PropertyNames.{propertyName}).Value;\r\n";
                        break;
                    case ElementType.Checkbox:
                        tokenValues += $"            {u.Identifier} = {tokenName}.GetProperty<BooleanProperty>(PropertyNames.{propertyName}).Value;\r\n";
                        break;
                    case ElementType.ColorWheel:
                        if (!u.ColorWheelOptions.HasFlag(ColorWheelOptions.Alpha))
                        {
                            tokenValues += $"            {u.Identifier} = ColorBgra.FromOpaqueInt32({tokenName}.GetProperty<Int32Property>(PropertyNames.{propertyName}).Value);\r\n";
                        }
                        else
                        {
                            tokenValues += $"            {u.Identifier} = ColorBgra.FromUInt32(unchecked((uint){tokenName}.GetProperty<Int32Property>(PropertyNames.{propertyName}).Value));\r\n";
                        }
                        break;
                    case ElementType.AngleChooser:
                    case ElementType.DoubleSlider:
                        tokenValues += $"            {u.Identifier} = {tokenName}.GetProperty<DoubleProperty>(PropertyNames.{propertyName}).Value;\r\n";
                        break;
                    case ElementType.PanSlider:
                        tokenValues += $"            {u.Identifier} = {tokenName}.GetProperty<DoubleVectorProperty>(PropertyNames.{propertyName}).Value;\r\n";
                        break;
                    case ElementType.Filename:
                    case ElementType.Folder:
                    case ElementType.Textbox:
                    case ElementType.MultiLineTextbox:
                        tokenValues += $"            {u.Identifier} = {tokenName}.GetProperty<StringProperty>(PropertyNames.{propertyName}).Value;\r\n";
                        break;
                    case ElementType.DropDown:
                    case ElementType.RadioButtons:
                        string typeCast = Intelli.IsEnum(u.TEnum) ? u.TEnum : "byte)(int";
                        tokenValues += $"            {u.Identifier} = ({typeCast}){tokenName}.GetProperty<StaticListChoiceProperty>(PropertyNames.{propertyName}).Value;\r\n";
                        break;
                    case ElementType.BinaryPixelOp:
                        tokenValues += $"            {u.Identifier} = LayerBlendModeUtil.CreateCompositionOp((LayerBlendMode){tokenName}.GetProperty<StaticListChoiceProperty>(PropertyNames.{propertyName}).Value);\r\n";
                        break;
                    case ElementType.FontFamily:
                        tokenValues += $"            {u.Identifier} = (FontFamily){tokenName}.GetProperty<StaticListChoiceProperty>(PropertyNames.{propertyName}).Value;\r\n";
                        break;
                    case ElementType.ReseedButton:
                        tokenValues += $"            {u.Identifier} = (byte){tokenName}.GetProperty<Int32Property>(PropertyNames.{propertyName}).Value;\r\n";
                        tokenValues += $"            randomSeed = {u.Identifier};\r\n";
                        break;
                    case ElementType.RollBall:
                        tokenValues += $"            {u.Identifier} = {tokenName}.GetProperty<DoubleVector3Property>(PropertyNames.{propertyName}).Value;\r\n";
                        break;
                }
            }
            tokenValues += "\r\n";

            return tokenValues;
        }

        internal static string ConstructorBodyPart(bool debug)
        {
            if (!debug)
            {
                return ""
                    + "        {\r\n"
                    + "        }\r\n"
                    + "\r\n";
            }

            return ""
                + "        {\r\n"
                + "            __listener = new TextWriterTraceListener(__debugWriter);\r\n"
                + "            Trace.Listeners.Add(__listener);\r\n"
                + "        }\r\n"
                + "\r\n"
                + "        public StringWriter __debugWriter = new StringWriter();\r\n"
                + "        TextWriterTraceListener __listener;\r\n"
                + "\r\n"
                + "        public string __DebugMsgs\r\n"
                + "        {\r\n"
                + "            get\r\n"
                + "            {\r\n"
                + "                return __debugWriter.ToString();\r\n"
                + "            }\r\n"
                + "        }\r\n"
                + "\r\n";
        }

        internal static string UserEnteredPart(string SourceCode)
        {
            SourceCode = Regex.Replace(SourceCode, @"\b(?:RadioButtonControl|ListBoxControl)<(?<TEnum>\S+)>\s+", match => match.Groups["TEnum"].Value + " ");

            string UserEnteredPart = "";
            UserEnteredPart += "        #region User Entered Code\r\n        ";
            UserEnteredPart += SourceCode.Replace("\n", "\n        ");
            UserEnteredPart += "\r\n";
            UserEnteredPart += "        #endregion\r\n";
            return UserEnteredPart;
        }

        internal static string SubmenuPart(string submenuName)
        {
            submenuName = submenuName.Trim();
            if (submenuName.Length == 0)
            {
                return "null";
            }

            string builtinSubmenu = typeof(SubmenuNames)
                .GetProperties(BindingFlags.Public | BindingFlags.Static)
                .Where(x => x.PropertyType == typeof(string))
                .Select(x => x.Name)
                .FirstOrDefault(x => x.Equals(submenuName, StringComparison.OrdinalIgnoreCase));

            return (builtinSubmenu != null)
                ? nameof(SubmenuNames) + "." + builtinSubmenu
                : "\"" + submenuName.Replace('"', '\'') + "\"";
        }

        internal static string EndPart()
        {
            return ""
                + "    }\r\n"
                + "}\r\n";
        }
    }
}
