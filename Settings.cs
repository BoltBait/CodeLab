using Microsoft.Win32;
using System;
using System.Globalization;

namespace PaintDotNet.Effects
{
    internal static class Settings
    {
        private static readonly RegistryKey regKey;
        private static readonly string desktopPath;
        private static readonly string documentsPath;

        internal static bool Bookmarks
        {
            get
            {
                int regValue = (int)regKey.GetValue("Bookmarks", 0);
                return regValue == 1;
            }
            set
            {
                int regValue = value ? 1 : 0;
                regKey.SetValue("Bookmarks", regValue, RegistryValueKind.DWord);
                regKey.Flush();
            }
        }

        internal static bool CheckForUpdates
        {
            get
            {
                int regValue = (int)regKey.GetValue("CheckForUpdates", 0);
                return regValue == 1;
            }
            set
            {
                int regValue = value ? 1 : 0;
                regKey.SetValue("CheckForUpdates", regValue, RegistryValueKind.DWord);
                regKey.Flush();
            }
        }

        internal static bool CodeFolding
        {
            get
            {
                int regValue = (int)regKey.GetValue("CodeFolding", 0);
                return regValue == 1;
            }
            set
            {
                int regValue = value ? 1 : 0;
                regKey.SetValue("CodeFolding", regValue, RegistryValueKind.DWord);
                regKey.Flush();
            }
        }

        internal static Theme EditorTheme
        {
            get
            {
                int regValue = (int)regKey.GetValue("EditorTheme", 0);
                return Enum.IsDefined(typeof(Theme), regValue) ? (Theme)regValue : Theme.Auto;
            }
            set
            {
                int regValue = (int)value;
                regKey.SetValue("EditorTheme", regValue, RegistryValueKind.DWord);
                regKey.Flush();
            }
        }

        internal static bool ErrorBox
        {
            get
            {
                int regValue = (int)regKey.GetValue("ErrorBox", 1);
                return regValue == 1;
            }
            set
            {
                int regValue = value ? 1 : 0;
                regKey.SetValue("ErrorBox", regValue, RegistryValueKind.DWord);
                regKey.Flush();
            }
        }

        internal static string FontFamily
        {
            get
            {
                return (string)regKey.GetValue("FontFamily", "Courier New");
            }
            set
            {
                regKey.SetValue("FontFamily", value, RegistryValueKind.String);
                regKey.Flush();
            }
        }

        internal static bool LargeFonts
        {
            get
            {
                int regValue = (int)regKey.GetValue("LargeFonts", 0);
                return regValue == 1;
            }
            set
            {
                int regValue = value ? 1 : 0;
                regKey.SetValue("LargeFonts", regValue, RegistryValueKind.DWord);
                regKey.Flush();
            }
        }

        internal static string LastSlnDirectory
        {
            get
            {
                return (string)regKey.GetValue("LastSlnDir", documentsPath);
            }
            set
            {
                regKey.SetValue("LastSlnDir", value, RegistryValueKind.String);
                regKey.Flush();
            }
        }

        internal static string LastSourceDirectory
        {
            get
            {
                return (string)regKey.GetValue("LastSourceDir", desktopPath);
            }
            set
            {
                regKey.SetValue("LastSourceDir", value, RegistryValueKind.String);
                regKey.Flush();
            }
        }

        internal static DateTime LatestUpdateCheck
        {
            get
            {
                string regValue = (string)regKey.GetValue("LatestUpdateCheck", string.Empty);
                if (DateTime.TryParseExact(regValue, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateTime))
                {
                    return dateTime;
                }
                else
                {
                    string newRegValue = DateTime.Now.ToString("yyyy-MM-dd");
                    regKey.SetValue("LatestUpdateCheck", newRegValue, RegistryValueKind.String);
                    regKey.Flush();
                    return DateTime.Now - TimeSpan.FromDays(8);
                }
            }
            set
            {
                string regValue = value.ToString("yyyy-MM-dd");
                regKey.SetValue("LatestUpdateCheck", regValue, RegistryValueKind.String);
                regKey.Flush();
            }
        }

        internal static bool LineNumbers
        {
            get
            {
                int regValue = (int)regKey.GetValue("LineNumbers", 0);
                return regValue == 1;
            }
            set
            {
                int regValue = value ? 1 : 0;
                regKey.SetValue("LineNumbers", regValue, RegistryValueKind.DWord);
                regKey.Flush();
            }
        }

        internal static bool Map
        {
            get
            {
                int regValue = (int)regKey.GetValue("Map", 0);
                return regValue == 1;
            }
            set
            {
                int regValue = value ? 1 : 0;
                regKey.SetValue("Map", regValue, RegistryValueKind.DWord);
                regKey.Flush();
            }
        }

        internal static bool Output
        {
            get
            {
                int regValue = (int)regKey.GetValue("Output", 0);
                return regValue == 1;
            }
            set
            {
                int regValue = value ? 1 : 0;
                regKey.SetValue("Output", regValue, RegistryValueKind.DWord);
                regKey.Flush();
            }
        }

        internal static string RecentDocs
        {
            get
            {
                return (string)regKey.GetValue("RecentDocs", string.Empty);
            }

            set
            {
                regKey.SetValue("RecentDocs", value, RegistryValueKind.String);
                regKey.Flush();
            }
        }

        internal static bool ToolBar
        {
            get
            {
                int regValue = (int)regKey.GetValue("ToolBar", 0);
                return regValue == 1;
            }
            set
            {
                int regValue = value ? 1 : 0;
                regKey.SetValue("ToolBar", regValue, RegistryValueKind.DWord);
                regKey.Flush();
            }
        }

        internal static bool WhiteSpace
        {
            get
            {
                int regValue = (int)regKey.GetValue("WhiteSpace", 0);
                return regValue == 1;
            }
            set
            {
                int regValue = value ? 1 : 0;
                regKey.SetValue("WhiteSpace", regValue, RegistryValueKind.DWord);
                regKey.Flush();
            }
        }

        internal static bool WordWrap
        {
            get
            {
                int regValue = (int)regKey.GetValue("WordWrap", 0);
                return regValue == 1;
            }
            set
            {
                int regValue = value ? 1 : 0;
                regKey.SetValue("WordWrap", regValue, RegistryValueKind.DWord);
                regKey.Flush();
            }
        }

        static Settings()
        {
            regKey = Registry.CurrentUser.OpenSubKey("Software\\CodeLab", true);
            if (regKey == null)
            {
                Registry.CurrentUser.CreateSubKey("Software\\CodeLab").Flush();
                regKey = Registry.CurrentUser.OpenSubKey("Software\\CodeLab", true);
            }

            desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        }
    }
}
