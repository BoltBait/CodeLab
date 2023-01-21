using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaintDotNet.Effects
{
    internal static class FileTypeWriter
    {
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
                + "using PaintDotNet.Clipboard;\r\n"
                + "using PaintDotNet.IndirectUI;\r\n"
                + "using PaintDotNet.Collections;\r\n"
                + "using PaintDotNet.PropertySystem;\r\n"
                + "using PaintDotNet.Rendering;\r\n"
                + "using IntSliderControl = System.Int32;\r\n"
                + "using CheckboxControl = System.Boolean;\r\n"
                + "using TextboxControl = System.String;\r\n"
                + "using DoubleSliderControl = System.Double;\r\n"
                + "using ListBoxControl = System.Byte;\r\n"
                + "using RadioButtonControl = System.Byte;\r\n"
                + "using MultiLineTextboxControl = System.String;\r\n"
                + "\r\n";
        }

        private static string FileTypePart(string projectName, string loadExt, string saveExt, bool supportsLayers, string title)
        {
            string fileTypePart = "";
            fileTypePart += "    public sealed class " + projectName + "Factory : IFileTypeFactory\r\n";
            fileTypePart += "    {\r\n";
            fileTypePart += "        public FileType[] GetFileTypeInstances()\r\n";
            fileTypePart += "        {\r\n";
            fileTypePart += "            return new[] { new " + projectName + "Plugin() };\r\n";
            fileTypePart += "        }\r\n";
            fileTypePart += "    }\r\n";
            fileTypePart += "\r\n";
            fileTypePart += "    [PluginSupportInfo(typeof(PluginSupportInfo))]\r\n";
            fileTypePart += "    internal class " + projectName + "Plugin : PropertyBasedFileType\r\n";
            fileTypePart += "    {\r\n";
            fileTypePart += "        internal " + projectName + "Plugin()\r\n";
            fileTypePart += "            : base(\r\n";
            fileTypePart += "                \"" + title + "\",\r\n";
            fileTypePart += "                new FileTypeOptions\r\n";
            fileTypePart += "                {\r\n";
            fileTypePart += "                    LoadExtensions = new string[] { " + loadExt + " },\r\n";
            fileTypePart += "                    SaveExtensions = new string[] { " + saveExt + " },\r\n";
            fileTypePart += "                    SupportsCancellation = true,\r\n";
            fileTypePart += "                    SupportsLayers = " + supportsLayers.ToString().ToLowerInvariant() + "\r\n";
            fileTypePart += "                })\r\n";

            return fileTypePart;
        }

        private static string FileTypePart2(UIElement[] userControls)
        {
            string fileTypePart2 = "";
            //fileTypePart2 += "        protected override bool IsReflexive(PropertyBasedSaveConfigToken token)\r\n";
            //fileTypePart2 += "        {\r\n";
            //fileTypePart2 += "            return false;\r\n";
            //fileTypePart2 += "        }\r\n";
            //fileTypePart2 += "\r\n";
            //fileTypePart2 += "\r\n";
            fileTypePart2 += "        protected override void OnSaveT(Document input, Stream output, PropertyBasedSaveConfigToken token, Surface scratchSurface, ProgressEventHandler progressCallback)\r\n";
            fileTypePart2 += "        {\r\n";

            if (userControls.Length > 0)
            {
                fileTypePart2 += CommonWriter.TokenValuesPart(userControls, "token");
            }

            fileTypePart2 += "            SaveImage(input, output, token, scratchSurface, progressCallback);\r\n";
            fileTypePart2 += "        }\r\n";
            fileTypePart2 += "\r\n";
            fileTypePart2 += "        protected override Document OnLoad(Stream input)\r\n";
            fileTypePart2 += "        {\r\n";
            fileTypePart2 += "            return LoadImage(input);\r\n";
            fileTypePart2 += "        }\r\n";

            return fileTypePart2;
        }

        internal static string FullSourceCode(string scriptText, string projectName, string author, int majorVersion, int minorVersion, string supportURL, string description, string loadExt, string saveExt, bool supportsLayers, string title)
        {
            UIElement[] userControls = UIElement.ProcessUIControls(scriptText, false);

            return
                UsingPartCode() +
                CommonWriter.AssemblyInfoPart(projectName, projectName, author, majorVersion, minorVersion, description, string.Empty) +
                CommonWriter.NamespacePart(projectName, true) +
                CommonWriter.SupportInfoPart(title, supportURL) +
                FileTypePart(projectName, loadExt, saveExt, supportsLayers, title) +
                CommonWriter.ConstructorBodyPart(false) +
                CommonWriter.PropertyPart(userControls, projectName, string.Empty, HelpType.None, string.Empty, ProjectType.FileType) +
                FileTypePart2(userControls) +
                CommonWriter.UserEnteredPart(scriptText) +
                CommonWriter.EndPart();
        }

        internal static string Run(string fileTypeCode, bool debug)
        {
            const string projectName = "MyFileType";

            UIElement[] userControls = UIElement.ProcessUIControls(fileTypeCode, false);

            return
                FileTypeWriter.UsingPartCode() +
                CommonWriter.NamespacePart(projectName, true) +
                FileTypeWriter.FileTypePart(projectName, "\".foo\"", "\".foo\"", false, projectName) +
                CommonWriter.ConstructorBodyPart(debug) +
                CommonWriter.PropertyPart(userControls, projectName, string.Empty, HelpType.None, string.Empty, ProjectType.FileType) +
                FileTypeWriter.FileTypePart2(userControls) +
                CommonWriter.UserEnteredPart(fileTypeCode) +
                CommonWriter.EndPart();
        }
    }
}
