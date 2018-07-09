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
// Latest distribution: http://www.BoltBait.com/pdn/codelab
/////////////////////////////////////////////////////////////////////////////////

using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

namespace PaintDotNet.Effects
{
    internal static class ScriptBuilder
    {
        private static readonly CSharpCodeProvider cscp = new CSharpCodeProvider();
        private static readonly CompilerParameters param = new CompilerParameters();
        private static CompilerResults result;
        private static Assembly userAssembly;
        private static Effect userScriptObject;
        private static int lineOffset;
        private static string internalError;

        #region Properties
        internal static Effect UserScriptObject => userScriptObject;
        internal static int LineOffset => lineOffset;
        internal static int ColumnOffset => 9;
        internal static List<ScriptError> Errors
        {
            get
            {
                List<ScriptError> errorList = new List<ScriptError>();
                if (result.Errors.HasErrors)
                {
                    foreach (CompilerError err in result.Errors)
                    {
                        errorList.Add(new ScriptError(err));
                    }
                }
                if (internalError != string.Empty)
                {
                    errorList.Add(new ScriptError(internalError));
                }
                return errorList;
            }
        }
        #endregion

        static ScriptBuilder()
        {
            param.GenerateInMemory = true;
            param.IncludeDebugInformation = false;
            param.GenerateExecutable = false;
            param.WarningLevel = 0;     // Turn off all warnings
            param.CompilerOptions = param.CompilerOptions + " /unsafe /optimize";

            // Get all loaded assemblies
            Assembly[] allLoadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();

            // Locate the dir where codelab.dll is
            string effectsPluginsPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string fileTypePluignsPath = Path.Combine(Directory.GetParent(effectsPluginsPath).FullName, "FileTypes");

            foreach (Assembly activeAssembly in allLoadedAssemblies)
            {
                try
                {
                    // Ignore assemblies in the Effects and FileTypes directories
                    if (!activeAssembly.Location.StartsWith(effectsPluginsPath, StringComparison.Ordinal) && !activeAssembly.Location.StartsWith(fileTypePluignsPath, StringComparison.Ordinal))
                    {
                        // Make assembly accessible to the plugin being written in the editor
                        param.ReferencedAssemblies.Add(activeAssembly.Location);
                    }
                }
                catch
                {
                    // Just don't crash. It wasn't that important anyway.
                }
            }

            lineOffset = (ScriptWriter.UsingPartCode + ScriptWriter.prepend_code).CountLines();
        }

        internal static bool Build(string scriptText, bool debug)
        {
            Regex preRenderRegex = new Regex(@"void PreRender\(Surface dst, Surface src\)(\s)*{(.|\s)*}", RegexOptions.Singleline);

            // Generate code
            string SourceCode =
                ScriptWriter.UsingPartCode +
                ScriptWriter.prepend_code +
                ScriptWriter.SetRenderPart(new List<UIElement>(), false, preRenderRegex.IsMatch(scriptText)) +
                ScriptWriter.UserEnteredPart(scriptText) +
                ScriptWriter.append_code;

            internalError = string.Empty;
            userScriptObject = null;
            // Compile code
            try
            {
                param.IncludeDebugInformation = debug;
                result = cscp.CompileAssemblyFromSource(param, SourceCode);
                param.IncludeDebugInformation = false;
                lineOffset = (SourceCode.Substring(0, SourceCode.IndexOf("#region User Entered Code", StringComparison.Ordinal)) + "\r\n").CountLines();

                if (result.Errors.HasErrors)
                {
                    return false;
                }

                userAssembly = result.CompiledAssembly;
                Intelli.UserDefinedTypes.Clear();

                foreach (Type type in userAssembly.GetTypes())
                {
                    if (type.IsSubclassOf(typeof(Effect)) && !type.IsAbstract)
                    {
                        userScriptObject = (Effect)type.GetConstructor(Type.EmptyTypes).Invoke(null);
                        Intelli.UserScript = type;
                    }
                    else if (type.DeclaringType != null && type.DeclaringType.FullName.StartsWith("PaintDotNet.Effects.UserScript", StringComparison.Ordinal) && !Intelli.UserDefinedTypes.ContainsKey(type.Name))
                    {
                        Intelli.UserDefinedTypes.Add(type.Name, type);
                    }
                }

                return (userScriptObject != null);
            }
            catch (Exception ex)
            {
                internalError = ex.Message;
            }
            return false;
        }

        internal static bool BuildDll(string scriptText, string scriptPath, string submenuname, string menuname, string iconpath, string Author, int MajorVersion, int MinorVersion, string SupportURL, string WindowTitleStr, bool isAdjustment, string Description, string KeyWords, bool ForceAliasSelection, bool ForceSingleThreaded, int HelpType, string HelpText)
        {
            string FileName = Path.GetFileNameWithoutExtension(scriptPath);
            string NameSpace = FileName;

            // Calculate output path
            string fullPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            fullPath = Path.Combine(fullPath, FileName);
            fullPath = Path.ChangeExtension(fullPath, ".dll");
            string iconResName = Path.GetFileName(iconpath);
            string installPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            installPath = Path.Combine(installPath, "Install_" + Regex.Replace(FileName, @"[^\w]", ""));
            installPath = Path.ChangeExtension(installPath, ".bat");

            // Calculate some default Attributes
            SupportURL = SupportURL.Trim();
            if (SupportURL == "")
            {
                SupportURL = "http://www.getpaint.net/redirect/plugins.html";
            }
            Description = Description.Trim();
            if (Description == "")
            {
                Description = menuname + " selected pixels";
            }
            if (KeyWords == "")
            {
                KeyWords = menuname;
            }

            // Remove non-alpha characters from namespace
            NameSpace = Regex.Replace(NameSpace, @"[^\w]", "") + "Effect";

            // Generate code
            string SourceCode = ScriptWriter.FullSourceCode(scriptText, FileName, isAdjustment, submenuname, menuname, iconpath, SupportURL, ForceAliasSelection, ForceSingleThreaded, Author, MajorVersion, MinorVersion, Description, KeyWords, WindowTitleStr, HelpType, HelpText);

            internalError = string.Empty;
            // Compile code
            try
            {
                string oldargs = param.CompilerOptions;
                if (File.Exists(iconpath))
                {
                    // If an icon is specified and exists, add it to the build as an imbedded resource to the same namespace as the effect.
                    param.CompilerOptions = param.CompilerOptions + " /res:\"" + iconpath + "\",\"" + NameSpace + "." + iconResName + "\" ";

                    // If an icon exists, see if a sample image exists
                    string samplepath;
                    samplepath = Path.ChangeExtension(iconpath, ".sample.png");
                    if (File.Exists(samplepath))
                    {
                        // If an image exists in the icon directory with a ".sample.png" extension, add it to the build as an imbedded resource.
                        param.CompilerOptions = param.CompilerOptions + " /res:\"" + samplepath + "\",\"" + NameSpace + "." + Path.GetFileName(samplepath) + "\" ";
                    }
                }
                string HelpPath = Path.ChangeExtension(scriptPath, ".rtz");
                if (HelpType == 3 && File.Exists(HelpPath))
                {
                    // If an help file exists in the source directory with a ".rtz" extension, add it to the build as an imbedded resource.
                    param.CompilerOptions = param.CompilerOptions + " /res:\"" + HelpPath + "\",\"" + NameSpace + "." + Path.GetFileName(HelpPath) + "\" ";
                }

                param.CompilerOptions = param.CompilerOptions + " /debug- /target:library /out:\"" + fullPath + "\"";

                try
                {
                    result = cscp.CompileAssemblyFromSource(param, SourceCode);
                }
                catch (Exception x)
                {
                    if (x.Message.Contains("Could not find file"))
                    {
                        // ignore this error
                    }
                    else
                    {
                        throw new Exception(x.Message, x);
                    }
                }

                lineOffset = (SourceCode.Substring(0, SourceCode.IndexOf("#region User Entered Code", StringComparison.Ordinal)) + "\r\n").CountLines();

                param.CompilerOptions = oldargs;


                if (result.Errors.HasErrors)
                {
                    return false;
                }

                // Create install bat file
                if (File.Exists(installPath))
                {
                    File.Delete(installPath);
                }

                // Try this in Russian for outputting Russian characters to the install batch file:

                //System.Text.Encoding encoding = System.Text.Encoding.GetEncoding("windows-1251");
                //var stream = new FileStream(installPath, FileMode.Create);
                //StreamWriter sw = new StreamWriter(stream, encoding);

                StreamWriter sw = new StreamWriter(installPath);

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
                sw.WriteLine(":: Read registry to find paint.net install directory");
                sw.WriteLine("reg query HKLM\\SOFTWARE\\Paint.NET /v TARGETDIR 2>nul || (echo Sorry, I can't find paint.net! & goto fail)");
                sw.WriteLine("set PDN_DIR=");
                sw.WriteLine("for /f \"tokens=2,*\" %%a in ('reg query HKLM\\SOFTWARE\\Paint.NET /v TARGETDIR ^| findstr TARGETDIR') do (");
                sw.WriteLine("  set PDN_DIR=%%b");
                sw.WriteLine(")");
                sw.WriteLine("if not defined PDN_DIR (echo Sorry, I can't find paint.net! & goto fail)");
                sw.WriteLine(":: End read registry");
                sw.WriteLine(":: Now do install");
                sw.WriteLine("@echo off");
                sw.WriteLine("cls");
                sw.WriteLine("echo.");
                sw.WriteLine("echo Installing " + Path.GetFileName(fullPath) + " to %PDN_DIR%\\Effects\\");
                sw.WriteLine("echo.");
                sw.WriteLine("copy /y \"" + Path.GetFileName(fullPath) + "\" \"%PDN_DIR%\\Effects\\\"");
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

                return true;
            }
            catch (Exception ex)
            {
                fullPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                fullPath = Path.Combine(fullPath, FileName);
                fullPath = Path.ChangeExtension(fullPath, ".dll");
                if (!File.Exists(fullPath))
                {
                    //userScriptObject = null;
                    internalError = ex.Message;
                }
                else
                {
                    return true;
                }
            }
            return false;
        }

        internal static bool BuildFullPreview(string scriptText)
        {
            string FileName = "PreviewEffect";
            Regex preRenderRegex = new Regex(@"void PreRender\(Surface dst, Surface src\)(\s)*{(.|\s)*}", RegexOptions.Singleline);

            // Generate code
            List<UIElement> UserControls = ScriptWriter.ProcessUIControls(scriptText);

            string SourceCode =
                ScriptWriter.UsingPartCode +
                ScriptWriter.NamespacePart(FileName) +
                ScriptWriter.EffectPart(UserControls, FileName, string.Empty, FileName, string.Empty, false, false) +
                ScriptWriter.PropertyPart(UserControls, false, FileName, "FULL UI PREVIEW - Temporarily renders to canvas", 0, string.Empty) +
                ScriptWriter.SetRenderPart(UserControls, true, preRenderRegex.IsMatch(scriptText)) +
                ScriptWriter.RenderLoopPart(UserControls) +
                ScriptWriter.UserEnteredPart(scriptText) +
                ScriptWriter.EndPart();

            internalError = string.Empty;
            userScriptObject = null;
            // Compile code
            try
            {
                result = cscp.CompileAssemblyFromSource(param, SourceCode);

                if (result.Errors.HasErrors)
                {
                    return false;
                }

                userAssembly = result.CompiledAssembly;
                foreach (Type type in userAssembly.GetTypes())
                {
                    if (type.IsSubclassOf(typeof(PropertyBasedEffect)) && !type.IsAbstract)
                    {
                        userScriptObject = (Effect)type.GetConstructor(Type.EmptyTypes).Invoke(null);
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                internalError = ex.Message;
            }
            return false;
        }

        internal static bool BuildUiPreview(string uiCode)
        {
            string FileName = "UiPreviewEffect";
            uiCode = "#region UICode\r\n" + uiCode + "\r\n#endregion\r\n";

            // Generate code
            List<UIElement> UserControls = ScriptWriter.ProcessUIControls(uiCode);

            string SourceCode =
                ScriptWriter.UsingPartCode +
                ScriptWriter.NamespacePart(FileName) +
                ScriptWriter.EffectPart(UserControls, FileName, string.Empty, "UI PREVIEW - Does NOT Render to canvas", string.Empty, false, false) +
                ScriptWriter.PropertyPart(UserControls, false, FileName, string.Empty, 0, string.Empty) +
                ScriptWriter.SetRenderPart(UserControls, true, false) +
                ScriptWriter.RenderLoopPart(UserControls) +
                uiCode +
                ScriptWriter.EmptyCode +
                ScriptWriter.EndPart();

            userScriptObject = null;
            // Compile code
            try
            {
                result = cscp.CompileAssemblyFromSource(param, SourceCode);

                if (result.Errors.HasErrors)
                {
                    return false;
                }

                userAssembly = result.CompiledAssembly;
                foreach (Type type in userAssembly.GetTypes())
                {
                    if (type.IsSubclassOf(typeof(PropertyBasedEffect)) && !type.IsAbstract)
                    {
                        userScriptObject = (Effect)type.GetConstructor(Type.EmptyTypes).Invoke(null);
                        return true;
                    }
                }
            }
            catch
            {
            }
            return false;
        }
    }
}
