using System.Drawing;
using System.IO;
using System.Reflection;

namespace PaintDotNet.Effects
{
    internal static class ResUtil
    {
        private static readonly bool hiDpi = UIScaleFactor.Current.Scale > 1;
        private static readonly Assembly assembly = Assembly.GetExecutingAssembly();
        internal static readonly Image EmptyImage = new Bitmap(16, 16);

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
    }
}
