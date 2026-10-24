/////////////////////////////////////////////////////////////////////////////////
// CodeLab for Paint.NET
// Copyright 2018 Jason Wendt. All Rights Reserved.
// Portions Copyright ©Microsoft Corporation. All Rights Reserved.
//
// THE DEVELOPERS MAKE NO WARRANTY OF ANY KIND REGARDING THE CODE. THEY
// SPECIFICALLY DISCLAIM ANY WARRANTY OF FITNESS FOR ANY PARTICULAR PURPOSE OR
// ANY OTHER WARRANTY.  THE CODELAB DEVELOPERS DISCLAIM ALL LIABILITY RELATING
// TO THE USE OF THIS CODE.  NO LICENSE, EXPRESS OR IMPLIED, BY ESTOPPEL OR
// OTHERWISE, TO ANY INTELLECTUAL PROPERTY RIGHTS IS GRANTED HEREIN.
//
// Latest distribution: https://www.BoltBait.com/pdn/codelab
/////////////////////////////////////////////////////////////////////////////////

using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace PdnCodeLab
{
    internal static class Solution
    {
        internal static string Generate(string slnPath, string name, string effectSource, string iconPath, string resourcePath, bool isClassicInstall)
        {
            string effectName = Regex.Replace(name, @"[^\w]", "");
            string projectName = effectName + "Effect";
            string projPath = Path.Combine(slnPath, projectName);
            string propPath = Path.Combine(projPath, "Properties");
            bool iconExists = File.Exists(iconPath);
            string samplePath = Path.ChangeExtension(resourcePath, ".sample.png");
            bool sampleExists = File.Exists(samplePath);
            string rtfPath = Path.ChangeExtension(resourcePath, ".rtz");
            bool rtfExists = File.Exists(rtfPath);

            // Two space indent on these files

            StringBuilder slnxFile = new StringBuilder()
                .AppendLine("<Solution>")
                .AppendLine($"  <Project Path=\"{projectName}/{projectName}.csproj\" />")
                .AppendLine("</Solution>");

            StringBuilder csprojFile = new StringBuilder();
            csprojFile.AppendLine("<Project Sdk=\"Microsoft.NET.Sdk\">");
            csprojFile.AppendLine();
            csprojFile.AppendLine("  <PropertyGroup>");
            csprojFile.AppendLine($"    <TargetFramework>net{Environment.Version.ToString(2)}-windows</TargetFramework>");
            csprojFile.AppendLine("    <GenerateAssemblyInfo>false</GenerateAssemblyInfo>");
            csprojFile.AppendLine("    <UseWindowsForms>true</UseWindowsForms>");
            csprojFile.AppendLine("    <AllowUnsafeBlocks>true</AllowUnsafeBlocks>");
            csprojFile.AppendLine($"    <RootNamespace>{projectName}</RootNamespace>");
            csprojFile.AppendLine($"    <AssemblyName>{effectName}</AssemblyName>");
            csprojFile.AppendLine("    <Deterministic>false</Deterministic>");
            csprojFile.AppendLine("  </PropertyGroup>");
            csprojFile.AppendLine();
            if (iconExists || sampleExists || rtfExists)
            {
                csprojFile.AppendLine("  <ItemGroup>");
                if (iconExists)
                {
                    csprojFile.AppendLine($"    <EmbeddedResource Include=\"{effectName}.png\" />");
                }
                if (sampleExists)
                {
                    csprojFile.AppendLine($"    <EmbeddedResource Include=\"{effectName}.sample.png\" />");
                }
                if (rtfExists)
                {
                    csprojFile.AppendLine($"    <EmbeddedResource Include=\"{effectName}.rtz\" />");
                }
                csprojFile.AppendLine("  </ItemGroup>");
                csprojFile.AppendLine();
            }
            csprojFile.AppendLine("  <ItemGroup>");
            // https://learn.microsoft.com/en-us/nuget/concepts/dependency-resolution#floating-versions
            csprojFile.AppendLine("    <PackageReference Include=\"Paint.NET.Plugin.SDK\" Version=\"*-*\" />");
            csprojFile.AppendLine("  </ItemGroup>");
            csprojFile.AppendLine();
            csprojFile.AppendLine("  <Target Name=\"PostBuild\" AfterTargets=\"PostBuildEvent\">");
            csprojFile.AppendLine("    <Exec Command=\"cmd /c explorer &quot;$(TargetDir)&quot;&#xD;&#xA;exit 0\" />");
            csprojFile.AppendLine("  </Target>");
            csprojFile.Append("</Project>"); // no end-of-line at the end of this file

            StringBuilder launchSettingsFile = new StringBuilder();
            if (isClassicInstall)
            {
                string pdnPath = Application.StartupPath;

                launchSettingsFile
                    .AppendLine("{")
                    .AppendLine("  \"profiles\": {")
                    .AppendLine("    \"" + projectName + "\": {")
                    .AppendLine("      \"commandName\": \"Executable\",")
                    .AppendLine($"      \"executablePath\": \"{Path.Combine(pdnPath, "PaintDotNet.exe").Replace(@"\", @"\\")}\",")
                    .AppendLine($"      \"workingDirectory\": \"{pdnPath.Replace(@"\", @"\\")}\"")
                    .AppendLine("    }")
                    .AppendLine("  }")
                    .Append("}"); // no end-of-line at the end of this file
            }

            try
            {
                File.WriteAllText(Path.Combine(slnPath, projectName + ".slnx"), slnxFile.ToString());

                if (!Directory.Exists(projPath))
                {
                    Directory.CreateDirectory(projPath);
                }

                File.WriteAllText(Path.Combine(projPath, projectName + ".csproj"), csprojFile.ToString());
                File.WriteAllText(Path.Combine(projPath, effectName + ".cs"), effectSource);

                if (iconExists)
                {
                    File.Copy(iconPath, Path.Combine(projPath, Path.GetFileName(iconPath)), true);
                }
                if (sampleExists)
                {
                    File.Copy(samplePath, Path.Combine(projPath, Path.GetFileName(samplePath)), true);
                }
                if (rtfExists)
                {
                    File.Copy(rtfPath, Path.Combine(projPath, Path.GetFileName(rtfPath)), true);
                }

                if (isClassicInstall)
                {
                    if (!Directory.Exists(propPath))
                    {
                        Directory.CreateDirectory(propPath);
                    }

                    File.WriteAllText(Path.Combine(propPath, "launchSettings.json"), launchSettingsFile.ToString());
                }
            }
            catch (Exception ex)
            {
                FlexibleMessageBox.Show("Solution generated failed.\r\n\r\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }

            return Path.Combine(slnPath, projectName + ".sln");
        }
    }
}
