using System.Drawing;
using System.IO;
using System.Reflection;

namespace PaintDotNet.Effects
{
    internal static class ResUtil
    {
        private static readonly bool hiDpi = UIScaleFactor.Current.Scale > 1;
        private static readonly Assembly assembly = Assembly.GetExecutingAssembly();
        private static readonly Image emptyImage = new Bitmap(16, 16);

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

            resource = $"PaintDotNet.Effects.Icons.{resName}.png";

            using (Stream imageStream = assembly.GetManifestResourceStream(resource))
            {
                if (imageStream != null)
                {
                    return Image.FromStream(imageStream);
                }
            }

            return emptyImage;
        }
    }
}
