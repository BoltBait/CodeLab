/////////////////////////////////////////////////////////////////////////////////
// CodeLab for Paint.NET
// Copyright ©2006 Rick Brewster, Tom Jackson. All Rights Reserved.
// Portions Copyright ©2018 BoltBait. All Rights Reserved.
// Portions Copyright ©2018 Jason Wendt. All Rights Reserved.
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

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text.RegularExpressions;

namespace PaintDotNet.Effects
{
    internal static class ScriptBuilder
    {
        private static Effect builtEffect;
        private static FileType builtFileType;
        private static int lineOffset;
        private static string exceptionMsg;
        private static readonly Regex preRenderRegex = new Regex(@"void\s+PreRender\s*\(\s*Surface\s+dst\s*,\s*Surface\s+src\s*\)\s*{(.|\s)*}", RegexOptions.Singleline);

        private static readonly IEnumerable<MetadataReference> references = Intelli.ReferenceAssemblies.Select(a => MetadataReference.CreateFromFile(a.Location));
        private static readonly CSharpParseOptions parseOptions = CSharpParseOptions.Default
            .WithLanguageVersion(LanguageVersion.CSharp9); // https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/configure-language-version
        private static CSharpCompilationOptions compilationOptions = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, allowUnsafe: true, optimizationLevel: OptimizationLevel.Release);
        private static IEnumerable<string> warningsToIgnore = Array.Empty<string>();

        private static IEnumerable<Diagnostic> errors;

        #region Properties
        internal static Effect BuiltEffect => builtEffect;
        internal static FileType BuiltFileType => builtFileType;
        internal static int LineOffset => lineOffset;
        internal static int ColumnOffset => 8;
        internal static string Exception => exceptionMsg;
        internal static IReadOnlyCollection<Error> Errors
        {
            get
            {
                List<Error> errorList = new List<Error>();
                if (!exceptionMsg.IsNullOrEmpty())
                {
                    errorList.Add(Error.NewInternalError(exceptionMsg));
                }
                if (errors != null)
                {
                    errorList.AddRange(errors.Select(diag => Error.NewCodeError(diag)));
                }

                errorList.Sort();
                return errorList;
            }
        }
        #endregion

        internal static void SetWarningLevel(int level)
        {
            compilationOptions = compilationOptions.WithWarningLevel(level);
        }

        internal static void SetWarningsToIgnore(IEnumerable<string> warnings)
        {
            warningsToIgnore = warnings;
        }

        internal static bool Build(string scriptText, bool debug)
        {
            builtEffect = null;

            string SourceCode =
                ScriptWriter.UsingPartCode(ProjectType.Effect) +
                ScriptWriter.prepend_code +
                ScriptWriter.ConstructorPart(debug) +
                ScriptWriter.SetRenderPart(Array.Empty<UIElement>(), false, preRenderRegex.IsMatch(scriptText)) +
                ScriptWriter.UserEnteredPart(scriptText) +
                ScriptWriter.append_code;

            Assembly assembly = CreateAssembly(SourceCode, debug);
            if (assembly == null)
            {
                return false;
            }

            Intelli.UserDefinedTypes.Clear();

            foreach (Type type in assembly.GetTypes())
            {
                if (type.IsSubclassOf(typeof(Effect)) && !type.IsAbstract)
                {
                    builtEffect = (Effect)type.GetConstructor(Type.EmptyTypes).Invoke(null);
                    Intelli.UserScript = type;
                }
                else if (type.DeclaringType != null && type.DeclaringType.FullName.StartsWith(Intelli.UserScriptFullName, StringComparison.Ordinal) && !Intelli.UserDefinedTypes.ContainsKey(type.Name))
                {
                    Intelli.UserDefinedTypes.Add(type.Name, type);
                }
            }

            return builtEffect != null;
        }

        internal static bool BuildEffectDll(string scriptText, string scriptPath, string subMenuname, string menuName, string iconPath, string author, int majorVersion, int minorVersion, string supportURL, string windowTitle, bool isAdjustment, string description, string keyWords, EffectFlags effectFlags, EffectRenderingSchedule renderingSchedule, HelpType helpType, string helpText)
        {
            string projectName = Path.GetFileNameWithoutExtension(scriptPath);

            // Generate code
            string sourceCode = ScriptWriter.FullSourceCode(scriptText, projectName, isAdjustment, subMenuname, menuName, iconPath, supportURL, effectFlags, renderingSchedule, author, majorVersion, minorVersion, description, keyWords, windowTitle, helpType, helpText);

            List<string> resourceFiles = new List<string>();

            if (File.Exists(iconPath))
            {
                // If an icon is specified and exists, add it to the build as an embedded resource to the same namespace as the effect.
                resourceFiles.Add(iconPath);
            }

            string samplepath = Path.ChangeExtension(scriptPath, ".sample.png");
            if (File.Exists(samplepath))
            {
                // If an image exists in the icon directory with a ".sample.png" extension, add it to the build as an embedded resource.
                resourceFiles.Add(samplepath);
            }

            string helpPath = Path.ChangeExtension(scriptPath, ".rtz");
            if (helpType == HelpType.RichText && File.Exists(helpPath))
            {
                // If an help file exists in the source directory with a ".rtz" extension, add it to the build as an embedded resource.
                resourceFiles.Add(helpPath);
            }

            return BuildDll(projectName, resourceFiles, sourceCode, ProjectType.Effect);
        }

        internal static bool BuildFullPreview(string scriptText)
        {
            builtEffect = null;

            const string FileName = "PreviewEffect";

            UIElement[] UserControls = UIElement.ProcessUIControls(scriptText, ProjectType.Effect);

            string SourceCode =
                ScriptWriter.UsingPartCode(ProjectType.Effect) +
                ScriptWriter.NamespacePart(FileName, ProjectType.Effect) +
                ScriptWriter.EffectPart(UserControls, FileName, string.Empty, FileName, string.Empty, EffectFlags.None, EffectRenderingSchedule.DefaultTilesForCpuRendering) +
                ScriptWriter.PropertyPart(UserControls, FileName, "FULL UI PREVIEW - Temporarily renders to canvas", HelpType.None, string.Empty, ProjectType.Effect) +
                ScriptWriter.SetRenderPart(UserControls, true, preRenderRegex.IsMatch(scriptText)) +
                ScriptWriter.RenderLoopPart(UserControls) +
                ScriptWriter.UserEnteredPart(scriptText) +
                ScriptWriter.EndPart();

            Assembly assembly = CreateAssembly(SourceCode, false);
            if (assembly == null)
            {
                return false;
            }

            foreach (Type type in assembly.GetTypes())
            {
                if (type.IsSubclassOf(typeof(PropertyBasedEffect)) && !type.IsAbstract)
                {
                    builtEffect = (Effect)type.GetConstructor(Type.EmptyTypes).Invoke(null);
                    return true;
                }
            }

            return false;
        }

        internal static bool BuildUiPreview(string uiCode)
        {
            builtEffect = null;

            const string FileName = "UiPreviewEffect";
            uiCode = "#region UICode\r\n" + uiCode + "\r\n#endregion\r\n";

            UIElement[] UserControls = UIElement.ProcessUIControls(uiCode, ProjectType.Effect);

            string SourceCode =
                ScriptWriter.UsingPartCode(ProjectType.Effect) +
                ScriptWriter.NamespacePart(FileName, ProjectType.Effect) +
                ScriptWriter.EffectPart(UserControls, FileName, string.Empty, "UI PREVIEW - Does NOT Render to canvas", string.Empty, EffectFlags.None, EffectRenderingSchedule.DefaultTilesForCpuRendering) +
                ScriptWriter.PropertyPart(UserControls, FileName, string.Empty, HelpType.None, string.Empty, ProjectType.Effect) +
                ScriptWriter.RenderLoopPart(UserControls) +
                uiCode +
                ScriptWriter.EmptyCode +
                ScriptWriter.EndPart();

            Assembly assembly = CreateAssembly(SourceCode, false);
            if (assembly == null)
            {
                return false;
            }

            foreach (Type type in assembly.GetTypes())
            {
                if (type.IsSubclassOf(typeof(PropertyBasedEffect)) && !type.IsAbstract)
                {
                    builtEffect = (Effect)type.GetConstructor(Type.EmptyTypes).Invoke(null);
                    return true;
                }
            }

            return false;
        }

        internal static bool BuildFileType(string fileTypeCode, bool debug)
        {
            builtFileType = null;

            const string projectName = "MyFileType";

            UIElement[] userControls = UIElement.ProcessUIControls(fileTypeCode, ProjectType.FileType);

            string sourceCode =
                ScriptWriter.UsingPartCode(ProjectType.FileType) +
                ScriptWriter.NamespacePart(projectName, ProjectType.FileType) +
                ScriptWriter.FileTypePart(projectName, "\".foo\"", "\".foo\"", false, projectName) +
                ScriptWriter.ConstructorPart(debug) +
                ScriptWriter.PropertyPart(userControls, projectName, string.Empty, HelpType.None, string.Empty, ProjectType.FileType) +
                ScriptWriter.FileTypePart2(userControls) +
                ScriptWriter.UserEnteredPart(fileTypeCode) +
                ScriptWriter.EndPart();

            Assembly assembly = CreateAssembly(sourceCode, debug);
            if (assembly == null)
            {
                return false;
            }

            Intelli.UserDefinedTypes.Clear();

            foreach (Type type in assembly.GetTypes())
            {
                if (type.IsSubclassOf(typeof(PropertyBasedFileType)) && !type.IsAbstract)
                {
                    builtFileType = (FileType)type.GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance)[0].Invoke(null);
                    return true;
                }
            }

            return false;
        }

        internal static bool BuildFileTypeDll(string scriptText, string scriptPath, string author, int majorVersion, int minorVersion, string supportURL, string description, string loadExt, string saveExt, bool supoortLayers, string title)
        {
            string projectName = Path.GetFileNameWithoutExtension(scriptPath);
            string sourceCode = ScriptWriter.FullFileTypeSourceCode(scriptText, projectName, author, majorVersion, minorVersion, supportURL, description, loadExt, saveExt, supoortLayers, title);

            return BuildDll(projectName, Array.Empty<string>(), sourceCode, ProjectType.FileType);
        }

        private static bool BuildDll(string projectName, IEnumerable<string> resources, string sourceCode, ProjectType projectType)
        {
            lineOffset = CalculateLineOffset(sourceCode);
            exceptionMsg = null;
            errors = null;

            try
            {
                string santizedProjectName = Regex.Replace(projectName, @"[^\w]", ""); // Remove non-alpha characters from namespace

                // Calculate output path
                string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                string dllPath = Path.Combine(desktopPath, santizedProjectName);
                dllPath = Path.ChangeExtension(dllPath, ".dll");

                IEnumerable<SyntaxTree> syntaxTree = new[] { CSharpSyntaxTree.ParseText(sourceCode, options: parseOptions) };

                CSharpCompilation compilation = CSharpCompilation.Create(santizedProjectName, syntaxTree, references, compilationOptions);

                IEnumerable<ResourceDescription> resourceDescriptions = resources.Select(res => new ResourceDescription(
                    santizedProjectName + "Effect." + Path.GetFileName(res),
                    () => File.OpenRead(res),
                    true));

                using (FileStream dllStream = new FileStream(dllPath, FileMode.Create))
                using (Stream win32resStream = compilation.CreateDefaultWin32Resources(true, true, null, null))
                {
                    EmitResult result = compilation.Emit(peStream: dllStream, manifestResources: resourceDescriptions, win32Resources: win32resStream);

                    errors = result.Diagnostics.Where(ErrorFilter);

                    if (!result.Success)
                    {
                        return false;
                    }
                }

                string zipPath = Path.ChangeExtension(dllPath, ".zip");
                string batPath = Path.Combine(desktopPath, "Install_" + santizedProjectName);
                batPath = Path.ChangeExtension(batPath, ".bat");

                // Create install bat file
                if (File.Exists(batPath))
                {
                    File.Delete(batPath);
                }
                if (File.Exists(zipPath))
                {
                    File.Delete(zipPath);
                }

                string pluginDirName = (projectType == ProjectType.Effect) ? "Effects" : "FileTypes";

                // Try this in Russian for outputting Russian characters to the install batch file:

                //System.Text.Encoding encoding = System.Text.Encoding.GetEncoding("windows-1251");
                //var stream = new FileStream(installPath, FileMode.Create);
                //StreamWriter sw = new StreamWriter(stream, encoding);

                StreamWriter sw = new StreamWriter(batPath);

                sw.WriteLine("@echo off");
                sw.WriteLine(":: Get ADMIN Privs");
                sw.WriteLine("mkdir \"%windir%\\BatchGotAdmin\"");
                sw.WriteLine("if '%errorlevel%' == '0' (");
                sw.WriteLine("  rmdir \"%windir%\\BatchGotAdmin\" & goto gotAdmin");
                sw.WriteLine(") else ( goto UACPrompt )");
                sw.WriteLine("");
                sw.WriteLine(":UACPrompt");
                sw.WriteLine("    echo Set UAC = CreateObject^(\"Shell.Application\"^) > \"%temp%\\getadmin.vbs\"");
                sw.WriteLine("    echo UAC.ShellExecute %0, \"\", \"\", \"runas\", 1 >> \"%temp%\\getadmin.vbs\"");
                sw.WriteLine("    \"%temp%\\getadmin.vbs\"");
                sw.WriteLine("    exit /B");
                sw.WriteLine("");
                sw.WriteLine(":gotAdmin");
                sw.WriteLine("    if exist \"%temp%\\getadmin.vbs\" ( del \"%temp%\\getadmin.vbs\" )");
                sw.WriteLine("    pushd \"%CD%\"");
                sw.WriteLine("    CD /D \"%~dp0\"");
                sw.WriteLine("");
                sw.WriteLine(":: End Get ADMIN Privs");
                sw.WriteLine(":: Read registry to find Paint.NET install directory");
                sw.WriteLine("reg query HKLM\\SOFTWARE\\Paint.NET /v TARGETDIR 2>nul || (echo Sorry, I can't find Paint.NET! & goto store)");
                sw.WriteLine("set PDN_DIR=");
                sw.WriteLine("for /f \"tokens=2,*\" %%a in ('reg query HKLM\\SOFTWARE\\Paint.NET /v TARGETDIR ^| findstr TARGETDIR') do (");
                sw.WriteLine("  set PDN_DIR=%%b");
                sw.WriteLine(")");
                sw.WriteLine("if not defined PDN_DIR (echo Sorry, I can't find Paint.NET! & goto store)");
                sw.WriteLine(":: End read registry");
                sw.WriteLine(":: Now do install");
                sw.WriteLine("@echo off");
                sw.WriteLine("cls");
                sw.WriteLine("echo.");
                sw.WriteLine("echo Installing " + Path.GetFileName(dllPath) + " to %PDN_DIR%\\" + pluginDirName + "\\");
                sw.WriteLine("echo.");
                sw.WriteLine("copy /y \"" + Path.GetFileName(dllPath) + "\" \"%PDN_DIR%\\" + pluginDirName + "\\\"");
                sw.WriteLine("if '%errorlevel%' == '0' (");
                sw.WriteLine("goto success");
                sw.WriteLine(") else (");
                sw.WriteLine("goto fail");
                sw.WriteLine(")");
                sw.WriteLine(":store");
                sw.WriteLine("reg query \"HKCU\\Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\User Shell Folders\" /v Personal 2>nul || (echo Sorry, I can't find Paint.NET! & goto fail)");
                sw.WriteLine("set PDN_DIR=");
                sw.WriteLine("for /f \"tokens=2,*\" %%a in ('reg query \"HKCU\\Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\User Shell Folders\" /v Personal ^| findstr Personal') do (");
                sw.WriteLine("  set PDN_DIR=%%b");
                sw.WriteLine(")");
                sw.WriteLine("if not defined PDN_DIR (echo Sorry, I can't find Paint.NET! & goto fail)");
                sw.WriteLine("@echo off");
                sw.WriteLine("cls");
                sw.WriteLine("setlocal enabledelayedexpansion");
                sw.WriteLine("set PDN_DIR=!PDN_DIR:%%USERPROFILE%%=%USERPROFILE%!");
                sw.WriteLine("setlocal disabledelayedexpansion");
                sw.WriteLine("echo.");
                sw.WriteLine("echo I could not find the standard installation of Paint.NET.");
                sw.WriteLine("echo I will install this effect in your Documents folder instead");
                sw.WriteLine("echo in case you are using the store version.");
                sw.WriteLine("echo.");
                sw.WriteLine("echo Installing " + Path.GetFileName(dllPath) + " to %PDN_DIR%\\paint.net App Files\\" + pluginDirName + "\\");
                sw.WriteLine("echo.");
                sw.WriteLine("mkdir \"%PDN_DIR%\\paint.net App Files\\" + pluginDirName + "\\\" 2>nul");
                sw.WriteLine("copy /y \"" + Path.GetFileName(dllPath) + "\" \"%PDN_DIR%\\paint.net App Files\\" + pluginDirName + "\\\"");
                sw.WriteLine("if '%errorlevel%' == '0' (");
                sw.WriteLine("goto success");
                sw.WriteLine(") else (");
                sw.WriteLine("goto fail");
                sw.WriteLine(")");
                sw.WriteLine(":success");
                sw.WriteLine("echo.");

                sw.WriteLine(@"echo    _____ _    _  _____ _____ ______  _____ _____  _ ");
                sw.WriteLine(@"echo   / ____) !  ! !/  ___)  ___)  ____)/ ____) ____)! !");
                sw.WriteLine(@"echo  ( (___ ! !  ! !  /  /  /   ! (__  ( (___( (___  ! !");
                sw.WriteLine(@"echo   \___ \! !  ! ! (  (  (    !  __)  \___ \\___ \ ! !");
                sw.WriteLine(@"echo   ____) ) !__! !  \__\  \___! (____ ____) )___) )!_!");
                sw.WriteLine(@"echo  (_____/ \____/ \_____)_____)______)_____/_____/ (_)");
                //sw.WriteLine(@"echo  _  _  ___  _____  ____  _  _  _ ");
                //sw.WriteLine(@"echo ( \/ )/ __)(  _  )(  __)( \/ )/ \");
                //sw.WriteLine(@"echo  )  /( (__  )( )(  ) _)  )  ( \_/");
                //sw.WriteLine(@"echo (__/  \___)(_) (_)(____)(_/\_)(_)");

                sw.WriteLine("goto done");
                sw.WriteLine(":fail");
                sw.WriteLine("echo.");

                sw.WriteLine(@"echo  _____       _____ _      _ ");
                sw.WriteLine(@"echo !  ___)/\   (_   _) !    ! !");
                sw.WriteLine(@"echo ! (__ /  \    ! ! ! !    ! !");
                sw.WriteLine(@"echo !  __) /\ \   ! ! ! !    ! !");
                sw.WriteLine(@"echo ! ! / ____ \ _! !_! !___ !_!");
                sw.WriteLine(@"echo !_!/_/    \_\_____)_____)(_)");
                //sw.WriteLine(@"echo   __  ____  __ _  ___  ____   _");
                //sw.WriteLine(@"echo  /  \(_  _)!  / /(_  \( __ ) / \");
                //sw.WriteLine(@"echo (  O ) )(  !   ( / _  )(__ \ \_/");
                //sw.WriteLine(@"echo  \__/ (__) !__\_\\___/(____/ (_)");

                sw.WriteLine("echo.");
                sw.WriteLine("echo Close Paint.NET and try installing again.");
                sw.WriteLine(":done");
                sw.WriteLine("echo.");
                sw.WriteLine("pause");

                sw.Flush();
                sw.Close();

                // Create .zip file on user's desktop
                using (ZipArchive archive = ZipFile.Open(zipPath, ZipArchiveMode.Create))
                {
                    archive.CreateEntryFromFile(dllPath, Path.GetFileName(dllPath)); // add .dll file
                    archive.CreateEntryFromFile(batPath, Path.GetFileName(batPath)); // add install.bat
                }

                if (File.Exists(zipPath))
                {
                    // if the zip file was successfully built, delete temp files
                    File.Delete(dllPath);
                    File.Delete(batPath);
                }

                return true;
            }
            catch (Exception ex)
            {
                exceptionMsg = ex.Message;
            }

            return false;
        }

        private static int CalculateLineOffset(string sourceCode)
        {
            return sourceCode.Substring(0, sourceCode.IndexOf("#region User Entered Code", StringComparison.Ordinal)).CountLines() + 1;
        }

        private static Assembly CreateAssembly(string sourceCode, bool debug)
        {
            lineOffset = CalculateLineOffset(sourceCode);
            exceptionMsg = null;
            errors = null;

            string assemblyName = Path.GetRandomFileName();
            IEnumerable<SyntaxTree> syntaxTree = new[] { CSharpSyntaxTree.ParseText(sourceCode, options: debug ? parseOptions.WithPreprocessorSymbols("DEBUG") : parseOptions) };

            CSharpCompilation compilation = CSharpCompilation.Create(
                assemblyName, syntaxTree, references,
                compilationOptions.WithOptimizationLevel(debug ? OptimizationLevel.Debug : OptimizationLevel.Release));

            using (MemoryStream ms = new MemoryStream())
            {
                EmitResult result = compilation.Emit(ms);

                errors = result.Diagnostics.Where(ErrorFilter);

                if (!result.Success)
                {
                    return null;
                }

                AssemblyLoadContext loadContext = new AssemblyLoadContext(null, true);
                ms.Seek(0, SeekOrigin.Begin);
                Assembly assembly = loadContext.LoadFromStream(ms);
                return assembly;
            }
        }

        private static bool ErrorFilter(Diagnostic diagnostic)
        {
            return !(diagnostic.Severity == DiagnosticSeverity.Hidden || warningsToIgnore.Contains(diagnostic.Id));
        }
    }
}
