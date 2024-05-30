using Microsoft.Win32;
using PaintDotNet;
using PaintDotNet.Effects;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace PdnCodeLab
{
    internal static class Settings
    {
        private static RegistryKey regKey = null;
        private static readonly string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
        private static readonly string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

        internal static bool Bookmarks
        {
            get => GetRegValue("Bookmarks", false);
            set => SetRegValue("Bookmarks", value);
        }

        internal static bool CheckForUpdates
        {
            get => GetRegValue("CheckForUpdates", false);
            set => SetRegValue("CheckForUpdates", value);
        }

        internal static bool CodeFolding
        {
            get => GetRegValue("CodeFolding", false);
            set => SetRegValue("CodeFolding", value);
        }

        internal static bool DisableAutoComplete
        {
            get => GetRegValue("DisableAutoComplete", false);
            set => SetRegValue("DisableAutoComplete", value);
        }

        internal static Theme EditorTheme
        {
            get => GetRegValue("EditorTheme", Theme.Auto);
            set => SetRegValue("EditorTheme", value);
        }

        internal static bool ErrorBox
        {
            get => GetRegValue("ErrorBox", false);
            set => SetRegValue("ErrorBox", value);
        }

        internal static string FontFamily
        {
            get
            {
                string fontFamily = GetRegValue("FontFamily", "Cascadia Mono");
                if (!UIUtil.IsFontInstalled(fontFamily))
                {
                    fontFamily = "Consolas";
                    if (!UIUtil.IsFontInstalled(fontFamily))
                    {
                        fontFamily = "Courier New";
                        if (!UIUtil.IsFontInstalled(fontFamily))
                        {
                            fontFamily = "Verdana";
                        }
                    }
                }

                return fontFamily;
            }
            set => SetRegValue("FontFamily", value);
        }

        internal static bool LargeFonts
        {
            get => GetRegValue("LargeFonts", false);
            set => SetRegValue("LargeFonts", value);
        }

        internal static string LastSlnDirectory
        {
            get => GetRegValue("LastSlnDir", documentsPath);
            set => SetRegValue("LastSlnDir", value);
        }

        internal static string LastSourceDirectory
        {
            get => GetRegValue("LastSourceDir", desktopPath);
            set => SetRegValue("LastSourceDir", value);
        }

        internal static DateTime LatestUpdateCheck
        {
            get
            {
                string regValue = GetRegValue("LatestUpdateCheck", string.Empty);
                if (DateTime.TryParseExact(regValue, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateTime))
                {
                    return dateTime;
                }
                else
                {
                    string newRegValue = DateTime.Now.ToString("yyyy-MM-dd");
                    SetRegValue("LatestUpdateCheck", newRegValue);
                    return DateTime.Now - TimeSpan.FromDays(8);
                }
            }
            set
            {
                string regValue = value.ToString("yyyy-MM-dd");
                SetRegValue("LatestUpdateCheck", regValue);
            }
        }

        internal static bool LineNumbers
        {
            get => GetRegValue("LineNumbers", false);
            set => SetRegValue("LineNumbers", value);
        }

        internal static bool Map
        {
            get => GetRegValue("Map", false);
            set => SetRegValue("Map", value);
        }

        internal static bool Output
        {
            get => GetRegValue("Output", false);
            set => SetRegValue("Output", value);
        }

        internal static string RecentDocs
        {
            get => GetRegValue("RecentDocs", string.Empty);
            set => SetRegValue("RecentDocs", value);
        }

        internal static bool ToolBar
        {
            get => GetRegValue("ToolBar", true);
            set => SetRegValue("ToolBar", value);
        }

        internal static bool WhiteSpace
        {
            get => GetRegValue("WhiteSpace", false);
            set => SetRegValue("WhiteSpace", value);
        }

        internal static bool WordWrap
        {
            get => GetRegValue("WordWrap", false);
            set => SetRegValue("WordWrap", value);
        }

        internal static bool WordWrapPlainText
        {
            get => GetRegValue("WordWrapPlainText", true);
            set => SetRegValue("WordWrapPlainText", value);
        }

        internal static string Snippets
        {
            get => GetRegValue("Snippets", string.Empty);
            set => SetRegValue("Snippets", value);
        }

        internal static int WarningLevel
        {
            get => GetRegValue("WarningLevel", 4);
            set => SetRegValue("WarningLevel", value);
        }

        internal static IEnumerable<string> WarningsToIgnore
        {
            get => GetRegValue("WarningsToIgnore", "CS0414").Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            set => SetRegValue("WarningsToIgnore", value.Join("|"));
        }

        internal static int IndentSpaces
        {
            get => GetRegValue("IndentSpaces", 4);
            set => SetRegValue("IndentSpaces", value);
        }

        internal static bool CaretLineFrame
        {
            get => GetRegValue("CaretLineFrame", false);
            set => SetRegValue("CaretLineFrame", value);
        }

        internal static bool ExtendedColors
        {
            get => GetRegValue("ExtendedColors", true);
            set => SetRegValue("ExtendedColors", value);
        }

        internal static bool Spellcheck
        {
            get => GetRegValue("Spellcheck", false);
            set => SetRegValue("Spellcheck", value);
        }

        internal static string SpellingLang
        {
            get => GetRegValue("SpellingLang", CultureInfo.CurrentUICulture.Name);
            set => SetRegValue("SpellingLang", value);
        }

        internal static IEnumerable<string> SpellingWordsToIgnore
        {
            get => GetRegValue("SpellingWordsToIgnore", "Desc").Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            set => SetRegValue("SpellingWordsToIgnore", value.Join("|"));
        }

        internal static RenderPreset RenderPreset
        {
            get => GetRegValue(nameof(RenderPreset), RenderPreset.Regular);
            set => SetRegValue(nameof(RenderPreset), value);
        }

        internal static BitmapEffectRenderingFlags RenderingFlags
        {
            get => GetRegValue(nameof(RenderingFlags), BitmapEffectRenderingFlags.None);
            set => SetRegValue(nameof(RenderingFlags), value);
        }

        internal static BitmapEffectRenderingSchedule RenderingSchedule
        {
            get => GetRegValue(nameof(RenderingSchedule), BitmapEffectRenderingSchedule.SquareTiles);
            set => SetRegValue(nameof(RenderingSchedule), value);
        }

        internal static DocCommentOptions DocCommentOptions
        {
            get => GetRegValue(nameof(DocCommentOptions), DocCommentOptions.Default);
            set => SetRegValue(nameof(DocCommentOptions), value);
        }

        private static void OpenRegKey()
        {
            if (regKey == null)
            {
                regKey = Registry.CurrentUser.OpenSubKey("Software\\CodeLab", true);
                if (regKey == null)
                {
                    Registry.CurrentUser.CreateSubKey("Software\\CodeLab").Flush();
                    regKey = Registry.CurrentUser.OpenSubKey("Software\\CodeLab", true);
                }
            }
        }

        private static T GetRegValue<T>(string valueName, T defaultValue)
        {
            if (regKey == null)
            {
                OpenRegKey();
            }

            if (defaultValue is bool b)
            {
                int defaultInt = b ? 1 : 0;
                int regValue = (int)regKey.GetValue(valueName, defaultInt);
                object boolObj = regValue == 1;
                return (T)boolObj;
            }
            else if (defaultValue is int integer)
            {
                return (T)regKey.GetValue(valueName, integer);
            }
            else if (defaultValue is string str)
            {
                return (T)regKey.GetValue(valueName, str);
            }
            else if (defaultValue is Enum)
            {
                Type enumType = defaultValue.GetType();
                Type underlyingType = Enum.GetUnderlyingType(enumType);
                object defaultValueAsNum = Convert.ChangeType(defaultValue, underlyingType);
                object regValue = regKey.GetValue(valueName, defaultValueAsNum);
                object regValueAsNum = Convert.ChangeType(regValue, underlyingType);
                return (T)regValueAsNum;
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        private static void SetRegValue<T>(string valueName, T value)
        {
            if (regKey == null)
            {
                OpenRegKey();
            }

            if (value is bool b)
            {
                int regValue = b ? 1 : 0;
                regKey.SetValue(valueName, regValue, RegistryValueKind.DWord);
            }
            else if (value is int integer)
            {
                regKey.SetValue(valueName, integer, RegistryValueKind.DWord);
            }
            else if (value is string str)
            {
                regKey.SetValue(valueName, str, RegistryValueKind.String);
            }
            else if (value is Enum)
            {
                Type enumType = value.GetType();
                Type underlyingType = Enum.GetUnderlyingType(enumType);
                object valueAsNum = Convert.ChangeType(value, underlyingType);
                RegistryValueKind valueKind = (underlyingType == typeof(ulong) || underlyingType == typeof(long)) ? RegistryValueKind.QWord : RegistryValueKind.DWord;
                regKey.SetValue(valueName, valueAsNum, valueKind);
            }
            else
            {
                throw new NotImplementedException();
            }

            regKey.Flush();
        }

        internal static void CloseRegKey()
        {
            if (regKey != null)
            {
                regKey.Close();
                regKey.Dispose();
                regKey = null;
            }
        }
    }
}
