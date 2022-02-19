using PaintDotNet.AppModel;
using PaintDotNet.Drawing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
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

        internal static Image GetImage(string resName, string directory = "Icons")
        {
            string resource = hiDpi ?
                $"PaintDotNet.Effects.{directory}.{resName}.32.png" :
                $"PaintDotNet.Effects.{directory}.{resName}.png";

            using (Stream imageStream = assembly.GetManifestResourceStream(resource))
            {
                if (imageStream != null)
                {
                    return Image.FromStream(imageStream);
                }
            }

            if (hiDpi)
            {
                resource = $"PaintDotNet.Effects.{directory}.{resName}.png";

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
            using (Image resImage = GetImage(resName))
            using (Surface surface = Surface.CopyFromGdipImage(resImage, false))
            using (Bitmap resBmp = surface.CreateAliasedGdipBitmap(BitmapAlphaMode.Premultiplied))
            {
                IntPtr hIcon = resBmp.GetHicon();
                return Icon.FromHandle(hIcon);
            }
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

        [DllImport("uxtheme.dll", SetLastError = true, ExactSpelling = true, CharSet = CharSet.Unicode)]
        private static extern int SetWindowTheme(IntPtr hWnd, string pszSubAppName, string pszSubIdList);

        /// <summary>
        /// Use to set dark scroll bars
        /// </summary>
        internal static void EnableUxThemeDarkMode(this Control control, bool enable)
        {
            if (!OperatingSystem.IsWindowsVersionAtLeast(10, 0, 17763))
            {
                return;
            }

            string themeName = enable ? "DarkMode_Explorer" : null;
            SetWindowTheme(control.Handle, themeName, null);
        }

        #region Copied from internal WinForms Code
        internal static Image CreateDisabledImage(Image normalImage)
        {
            ArgumentNullException.ThrowIfNull(normalImage);

            ImageAttributes imgAttrib = new ImageAttributes();

            imgAttrib.ClearColorKey();
            imgAttrib.SetColorMatrix(DisabledImageColorMatrix);

            Size size = normalImage.Size;
            Bitmap disabledBitmap = new Bitmap(size.Width, size.Height);
            using (Graphics graphics = Graphics.FromImage(disabledBitmap))
            {
                graphics.DrawImage(normalImage,
                                   new Rectangle(0, 0, size.Width, size.Height),
                                   0, 0, size.Width, size.Height,
                                   GraphicsUnit.Pixel,
                                   imgAttrib);
            }

            return disabledBitmap;
        }

        private static ColorMatrix s_disabledImageColorMatrix;

        private static ColorMatrix DisabledImageColorMatrix
        {
            get
            {
                if (s_disabledImageColorMatrix is null)
                {
                    // this is the result of a GreyScale matrix multiplied by a transparency matrix of .5

                    float[][] greyscale = new float[5][];
                    greyscale[0] = new float[5] { 0.2125f, 0.2125f, 0.2125f, 0, 0 };
                    greyscale[1] = new float[5] { 0.2577f, 0.2577f, 0.2577f, 0, 0 };
                    greyscale[2] = new float[5] { 0.0361f, 0.0361f, 0.0361f, 0, 0 };
                    greyscale[3] = new float[5] { 0, 0, 0, 1, 0 };
                    greyscale[4] = new float[5] { 0.38f, 0.38f, 0.38f, 0, 1 };

                    float[][] transparency = new float[5][];
                    transparency[0] = new float[5] { 1, 0, 0, 0, 0 };
                    transparency[1] = new float[5] { 0, 1, 0, 0, 0 };
                    transparency[2] = new float[5] { 0, 0, 1, 0, 0 };
                    transparency[3] = new float[5] { 0, 0, 0, .7F, 0 };
                    transparency[4] = new float[5] { 0, 0, 0, 0, 0 };

                    s_disabledImageColorMatrix = MultiplyColorMatrix(transparency, greyscale);
                }

                return s_disabledImageColorMatrix;
            }
        }

        /// <summary>
        ///  Multiply two 5x5 color matrices.
        /// </summary>
        private static ColorMatrix MultiplyColorMatrix(float[][] matrix1, float[][] matrix2)
        {
            const int Size = 5;

            // Build up an empty 5x5 array for results.
            float[][] result = new float[Size][];
            for (int row = 0; row < Size; row++)
            {
                result[row] = new float[Size];
            }

            float[] column = new float[Size];
            for (int j = 0; j < Size; j++)
            {
                for (int k = 0; k < Size; k++)
                {
                    column[k] = matrix1[k][j];
                }

                for (int i = 0; i < Size; i++)
                {
                    float[] row = matrix2[i];
                    float s = 0;
                    for (int k = 0; k < Size; k++)
                    {
                        s += row[k] * column[k];
                    }

                    result[i][j] = s;
                }
            }

            return new ColorMatrix(result);
        }
        #endregion
    }
}
