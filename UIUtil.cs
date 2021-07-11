using PaintDotNet.AppModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace PaintDotNet.Effects
{
    internal static class UIUtil
    {
        private static readonly bool hiDpi = UIScaleFactor.Current.Scale > 1;
        private static readonly Assembly assembly = Assembly.GetExecutingAssembly();
        internal static readonly Image EmptyImage = new Bitmap(16, 16);
        private static IShellService iShellService;

        internal static void SetIShellService(IShellService shellService)
        {
            iShellService = shellService;
        }

        internal static Image GetImage(string resName)
        {
            string resource = hiDpi ?
                $"PaintDotNet.Effects.Icons.{resName}.32.png" :
                $"PaintDotNet.Effects.Icons.{resName}.png";

            using (Stream imageStream = assembly.GetManifestResourceStream(resource))
            {
                if (imageStream != null)
                {
                    return Image.FromStream(imageStream);
                }
            }

            if (hiDpi)
            {
                resource = $"PaintDotNet.Effects.Icons.{resName}.png";

                using (Stream imageStream = assembly.GetManifestResourceStream(resource))
                {
                    if (imageStream != null)
                    {
                        return Image.FromStream(imageStream);
                    }
                }
            }

            return EmptyImage;
        }

        internal static Bitmap GetBitmapFromFile(string filePath)
        {
            try
            {
                // This is the BKM for loading bitmaps from files without locking the file.
                // This pattern should always be used when loading a bitmap from a file.
                using (Bitmap bmpTemp = new Bitmap(filePath))
                {
                    return new Bitmap(bmpTemp);
                }
            }
            catch
            {
                return null;
            }
        }

        internal static Icon CreateIcon(string resName)
        {
            Bitmap resBmp = GetImage(resName) as Bitmap;
            IntPtr hIcon = resBmp.GetHicon();
            return Icon.FromHandle(hIcon);
        }

        internal static int Scale(int value)
        {
            return (int)Math.Round(value * UIScaleFactor.Current.Scale);
        }

        internal static Size ScaleSize(int width, int height)
        {
            return new Size(Scale(width), Scale(height));
        }

        internal static bool IsFontInstalled(string fontName)
        {
            using (Font font = new Font(fontName, 12f))
            {
                return font.Name == fontName;
            }
        }

        internal static string[] GetColorNames(bool includeTransparent)
        {
            List<string> names = typeof(Color).GetProperties(BindingFlags.Public | BindingFlags.Static)
                     .Where(prop => prop.PropertyType == typeof(Color))
                     .Select(prop => prop.Name).ToList();

            if (!includeTransparent)
            {
                names.Remove("Transparent");
            }

            names.Sort();

            return names.ToArray();
        }

        internal static void LaunchUrl(IWin32Window owner, string url)
        {
            if (iShellService != null)
            {
                iShellService.LaunchUrl(owner, url);
            }
            else
            {
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true
                };
                Process.Start(startInfo);
            }
        }

        internal static bool LaunchFolderAndSelectFile(IWin32Window owner, string filePath)
        {
            if (iShellService != null)
            {
                return iShellService.LaunchFolderAndSelectFile(owner, filePath);
            }
            else
            {
                return ProcessUtil.TryExec("explorer.exe", new[] { "/select," + filePath }) != 0;
            }
        }
    }
}
