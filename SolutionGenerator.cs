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

namespace PaintDotNet.Effects
{
    internal static class Solution
    {
        internal static string Generate(string slnPath, string name, string effectSource, string iconPath, string resourcePath)
        {
            string effectName = Regex.Replace(name, @"[^\w]", "");
            string projectName = effectName + "Effect";
            string slnGuid = "{" + Guid.NewGuid().ToString() + "}";
            string projGroupGuid = "{" + Guid.NewGuid().ToString() + "}";
            string projGuid = "{" + Guid.NewGuid().ToString() + "}";
            string pdnPath = Application.StartupPath;
            string projPath = Path.Combine(slnPath, projectName);
            string propPath = Path.Combine(projPath, "Properties");
            bool iconExists = File.Exists(iconPath);
            string samplePath = Path.ChangeExtension(resourcePath, ".sample.png");
            bool sampleExists = File.Exists(samplePath);
            string rtfPath = Path.ChangeExtension(resourcePath, ".rtz");
            bool rtfExists = File.Exists(rtfPath);

            // Tab indent
            StringBuilder slnFile = new StringBuilder();
            slnFile.AppendLine();
            slnFile.AppendLine("Microsoft Visual Studio Solution File, Format Version 12.00");
            slnFile.AppendLine("# Visual Studio 16");
            slnFile.AppendLine("VisualStudioVersion = 16.0.31515.178");
            slnFile.AppendLine("MinimumVisualStudioVersion = 10.0.40219.1");
            slnFile.AppendLine($"Project(\"{projGroupGuid}\") = \"{projectName}\", \"{projectName}\\{projectName}.csproj\", \"{projGuid}\"");
            slnFile.AppendLine("EndProject");
            slnFile.AppendLine("Global");
            slnFile.AppendLine("\tGlobalSection(SolutionConfigurationPlatforms) = preSolution");
            slnFile.AppendLine("\t\tDebug|Any CPU = Debug|Any CPU");
            slnFile.AppendLine("\t\tRelease|Any CPU = Release|Any CPU");
            slnFile.AppendLine("\tEndGlobalSection");
            slnFile.AppendLine("\tGlobalSection(ProjectConfigurationPlatforms) = postSolution");
            slnFile.AppendLine($"\t\t{projGuid}.Debug|Any CPU.ActiveCfg = Debug|Any CPU");
            slnFile.AppendLine($"\t\t{projGuid}.Debug|Any CPU.Build.0 = Debug|Any CPU");
            slnFile.AppendLine($"\t\t{projGuid}.Release|Any CPU.ActiveCfg = Release|Any CPU");
            slnFile.AppendLine($"\t\t{projGuid}.Release|Any CPU.Build.0 = Release|Any CPU");
            slnFile.AppendLine("\tEndGlobalSection");
            slnFile.AppendLine("\tGlobalSection(SolutionProperties) = preSolution");
            slnFile.AppendLine("\t\tHideSolutionNode = FALSE");
            slnFile.AppendLine("\tEndGlobalSection");
            slnFile.AppendLine("\tGlobalSection(ExtensibilityGlobals) = postSolution");
            slnFile.AppendLine($"\t\tSolutionGuid = {slnGuid}");
            slnFile.AppendLine("\tEndGlobalSection");
            slnFile.AppendLine("EndGlobal");

            // Two space indent
            StringBuilder csprojFile = new StringBuilder();
            csprojFile.AppendLine("<Project Sdk=\"Microsoft.NET.Sdk\">");
            csprojFile.AppendLine();
            csprojFile.AppendLine("  <PropertyGroup>");
            csprojFile.AppendLine("    <TargetFramework>net6.0-windows</TargetFramework>");
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
            csprojFile.AppendLine("    <Reference Include=\"PaintDotNet.Base\">");
            csprojFile.AppendLine($"      <HintPath>{Path.Combine(pdnPath, "PaintDotNet.Base.dll")}</HintPath>");
            csprojFile.AppendLine("    </Reference>");
            csprojFile.AppendLine("    <Reference Include=\"PaintDotNet.Core\">");
            csprojFile.AppendLine($"      <HintPath>{Path.Combine(pdnPath, "PaintDotNet.Core.dll")}</HintPath>");
            csprojFile.AppendLine("    </Reference>");
            csprojFile.AppendLine("    <Reference Include=\"PaintDotNet.Data\">");
            csprojFile.AppendLine($"      <HintPath>{Path.Combine(pdnPath, "PaintDotNet.Data.dll")}</HintPath>");
            csprojFile.AppendLine("    </Reference>");
            csprojFile.AppendLine("    <Reference Include=\"PaintDotNet.Effects\">");
            csprojFile.AppendLine($"      <HintPath>{Path.Combine(pdnPath, "PaintDotNet.Effects.dll")}</HintPath>");
            csprojFile.AppendLine("    </Reference>");
            csprojFile.AppendLine("  </ItemGroup>");
            csprojFile.AppendLine();
            csprojFile.AppendLine("  <Target Name=\"PostBuild\" AfterTargets=\"PostBuildEvent\">");
            csprojFile.AppendLine("    <Exec Command=\"cmd /c explorer &quot;$(TargetDir)&quot;&#xD;&#xA;exit 0\" />");
            csprojFile.AppendLine("  </Target>");
            csprojFile.Append("</Project>"); // no end-of-line at the end of this file

            // Two space indent
            StringBuilder launchSettingsFile = new StringBuilder();
            launchSettingsFile.AppendLine("{");
            launchSettingsFile.AppendLine("  \"profiles\": {");
            launchSettingsFile.AppendLine("    \"" + projectName + "\": {");
            launchSettingsFile.AppendLine("      \"commandName\": \"Executable\",");
            launchSettingsFile.AppendLine($"      \"executablePath\": \"{Path.Combine(pdnPath, "PaintDotNet.exe").Replace(@"\", @"\\")}\",");
            launchSettingsFile.AppendLine($"      \"workingDirectory\": \"{pdnPath.Replace(@"\", @"\\")}\"");
            launchSettingsFile.AppendLine("    }");
            launchSettingsFile.AppendLine("  }");
            launchSettingsFile.Append("}"); // no end-of-line at the end of this file

            try
            {
                File.WriteAllText(Path.Combine(slnPath, projectName + ".sln"), slnFile.ToString());

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

                if (!Directory.Exists(propPath))
                {
                    Directory.CreateDirectory(propPath);
                }

                File.WriteAllText(Path.Combine(propPath, "launchSettings.json"), launchSettingsFile.ToString());
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
