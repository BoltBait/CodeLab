using Microsoft.Win32;
using System;
using System.Globalization;

namespace PaintDotNet.Effects
{
    internal static class Settings
    {
        private static RegistryKey regKey = null;
        private static readonly string desktopPath;
        private static readonly string documentsPath;

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
            get => GetRegValue("FontFamily", "Consolas");
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

        static Settings()
        {
            OpenRegKey();
            desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        }

        internal static void OpenRegKey()
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

        private static TType GetRegValue<TType>(string valueName, TType defaultValue)
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
                return (TType)boolObj;
            }
            else if (defaultValue is string str)
            {
                return (TType)regKey.GetValue(valueName, str);
            }
            else if (defaultValue is Theme theme)
            {
                int defaultTheme = (int)theme;
                int regValue = (int)regKey.GetValue(valueName, defaultTheme);
                object themeObj = Enum.IsDefined(typeof(Theme), regValue) ? (Theme)regValue : Theme.Auto;
                return (TType)themeObj;
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        private static void SetRegValue<TType>(string valueName, TType value)
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
            else if (value is string str)
            {
                regKey.SetValue(valueName, str, RegistryValueKind.String);
            }
            else if (value is Theme theme)
            {
                int regValue = (int)theme;
                regKey.SetValue(valueName, regValue, RegistryValueKind.DWord);
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
