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
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace PaintDotNet.Effects
{
    internal static class Solution
    {
        internal static bool Generate(string slnPath, string name, string effectSource, string iconPath, string resourcePath)
        {
            string effectName = Regex.Replace(name, @"[^\w]", "");
            string projectName = effectName + "Effect";
            string slnGuid = "{" + Guid.NewGuid().ToString() + "}";
            string projGroupGuid = "{" + Guid.NewGuid().ToString() + "}";
            string projGuid = "{" + Guid.NewGuid().ToString() + "}";
            string pdnPath = Application.StartupPath;
            string projPath = Path.Combine(slnPath, projectName);
            bool iconExists = File.Exists(iconPath);
            string samplePath = Path.ChangeExtension(resourcePath, ".sample.png");
            bool sampleExists = File.Exists(samplePath);
            string rtfPath = Path.ChangeExtension(resourcePath, ".rtz");
            bool rtfExists = File.Exists(rtfPath);

            // Tab indent
            StringBuilder slnFile = new StringBuilder();
            slnFile.AppendLine();
            slnFile.AppendLine("Microsoft Visual Studio Solution File, Format Version 12.00");
            slnFile.AppendLine("# Visual Studio 15");
            slnFile.AppendLine("VisualStudioVersion = 15.0.28010.0");
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
            csprojFile.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            csprojFile.AppendLine("<Project ToolsVersion=\"15.0\" xmlns=\"http://schemas.microsoft.com/developer/msbuild/2003\">");
            csprojFile.AppendLine("  <Import Project=\"$(MSBuildExtensionsPath)\\$(MSBuildToolsVersion)\\Microsoft.Common.props\" Condition=\"Exists('$(MSBuildExtensionsPath)\\$(MSBuildToolsVersion)\\Microsoft.Common.props')\" />");
            csprojFile.AppendLine("  <PropertyGroup>");
            csprojFile.AppendLine("    <Configuration Condition=\" '$(Configuration)' == '' \">Debug</Configuration>");
            csprojFile.AppendLine("    <Platform Condition=\" '$(Platform)' == '' \">AnyCPU</Platform>");
            csprojFile.AppendLine($"    <ProjectGuid>{projGuid}</ProjectGuid>");
            csprojFile.AppendLine("    <OutputType>Library</OutputType>");
            csprojFile.AppendLine("    <AppDesignerFolder>Properties</AppDesignerFolder>");
            csprojFile.AppendLine($"    <RootNamespace>{projectName}</RootNamespace>");
            csprojFile.AppendLine($"    <AssemblyName>{effectName}</AssemblyName>");
            csprojFile.AppendLine("    <TargetFrameworkVersion>v4.7</TargetFrameworkVersion>");
            csprojFile.AppendLine("    <FileAlignment>512</FileAlignment>");
            csprojFile.AppendLine("  </PropertyGroup>");
            csprojFile.AppendLine("  <PropertyGroup Condition=\" '$(Configuration)|$(Platform)' == 'Debug|AnyCPU' \">");
            csprojFile.AppendLine("    <DebugSymbols>true</DebugSymbols>");
            csprojFile.AppendLine("    <DebugType>full</DebugType>");
            csprojFile.AppendLine("    <Optimize>false</Optimize>");
            csprojFile.AppendLine("    <OutputPath>bin\\Debug\\</OutputPath>");
            csprojFile.AppendLine("    <DefineConstants>DEBUG;TRACE</DefineConstants>");
            csprojFile.AppendLine("    <ErrorReport>prompt</ErrorReport>");
            csprojFile.AppendLine("    <WarningLevel>4</WarningLevel>");
            csprojFile.AppendLine("    <AllowUnsafeBlocks>true</AllowUnsafeBlocks>");
            csprojFile.AppendLine("  </PropertyGroup>");
            csprojFile.AppendLine("  <PropertyGroup Condition=\" '$(Configuration)|$(Platform)' == 'Release|AnyCPU' \">");
            csprojFile.AppendLine("    <DebugType>pdbonly</DebugType>");
            csprojFile.AppendLine("    <Optimize>true</Optimize>");
            csprojFile.AppendLine("    <OutputPath>bin\\Release\\</OutputPath>");
            csprojFile.AppendLine("    <DefineConstants>TRACE</DefineConstants>");
            csprojFile.AppendLine("    <ErrorReport>prompt</ErrorReport>");
            csprojFile.AppendLine("    <WarningLevel>4</WarningLevel>");
            csprojFile.AppendLine("    <AllowUnsafeBlocks>true</AllowUnsafeBlocks>");
            csprojFile.AppendLine("  </PropertyGroup>");
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
            csprojFile.AppendLine("    <Reference Include=\"System\" />");
            csprojFile.AppendLine("    <Reference Include=\"System.Core\" />");
            csprojFile.AppendLine("    <Reference Include=\"System.Drawing\" />");
            csprojFile.AppendLine("    <Reference Include=\"System.Windows.Forms\" />");
            csprojFile.AppendLine("    <Reference Include=\"System.Xml.Linq\" />");
            csprojFile.AppendLine("    <Reference Include=\"System.Data.DataSetExtensions\" />");
            csprojFile.AppendLine("    <Reference Include=\"Microsoft.CSharp\" />");
            csprojFile.AppendLine("    <Reference Include=\"System.Data\" />");
            csprojFile.AppendLine("    <Reference Include=\"System.Net.Http\" />");
            csprojFile.AppendLine("    <Reference Include=\"System.Xml\" />");
            csprojFile.AppendLine("  </ItemGroup>");
            csprojFile.AppendLine("  <ItemGroup>");
            csprojFile.AppendLine($"    <Compile Include=\"{effectName}.cs\" />");
            csprojFile.AppendLine("  </ItemGroup>");
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
            }
            csprojFile.AppendLine("  <ItemGroup>");
            csprojFile.AppendLine("    <Folder Include=\"Properties\\\" />");
            csprojFile.AppendLine("  </ItemGroup>");
            csprojFile.AppendLine("  <Import Project=\"$(MSBuildToolsPath)\\Microsoft.CSharp.targets\" />");
            csprojFile.AppendLine("  <PropertyGroup>");
            csprojFile.AppendLine($"    <PostBuildEvent>cmd /c explorer \"$(TargetDir)\" \r\nexit 0</PostBuildEvent>");
            csprojFile.AppendLine("  </PropertyGroup>");
            csprojFile.Append("</Project>"); // no end-of-line at the end of this file

            // Two space indent
            StringBuilder csprojUserFile = new StringBuilder();
            csprojUserFile.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            csprojUserFile.AppendLine("<Project ToolsVersion=\"15.0\" xmlns=\"http://schemas.microsoft.com/developer/msbuild/2003\">");
            csprojUserFile.AppendLine("  <PropertyGroup Condition=\"'$(Configuration)|$(Platform)' == 'Debug|AnyCPU'\">");
            csprojUserFile.AppendLine("    <StartAction>Program</StartAction>");
            csprojUserFile.AppendLine($"    <StartProgram>{Path.Combine(pdnPath, "PaintDotNet.exe")}</StartProgram>");
            csprojUserFile.AppendLine($"    <StartWorkingDirectory>{pdnPath}\\</StartWorkingDirectory>");
            csprojUserFile.AppendLine("  </PropertyGroup>");
            csprojUserFile.Append("</Project>"); // no end-of-line at the end of this file


            try
            {
                File.WriteAllText(Path.Combine(slnPath, projectName + ".sln"), slnFile.ToString());

                if (!Directory.Exists(projPath))
                {
                    Directory.CreateDirectory(projPath);
                }

                File.WriteAllText(Path.Combine(projPath, projectName + ".csproj"), csprojFile.ToString());
                File.WriteAllText(Path.Combine(projPath, projectName + ".csproj.user"), csprojUserFile.ToString());
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
            }
            catch (Exception ex)
            {
                FlexibleMessageBox.Show("Solution generated failed.\r\n\r\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            try
            {
                Process.Start("explorer.exe", "/select," + Path.Combine(slnPath, projectName + ".sln"));
            }
            catch
            {
                FlexibleMessageBox.Show("Could not navigate to the generated Solution file.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return true;
        }
    }
}
