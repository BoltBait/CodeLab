using System.Drawing;
using System.IO;
using System.Reflection;
using System.Resources;

namespace PaintDotNet.Effects
{
    internal static class ResUtil
    {
        private static readonly bool hiDpi = UIScaleFactor.Current.Scale > 1;
        private static readonly Assembly assembly = Assembly.GetExecutingAssembly();
        private static readonly Image badIcon;
        private static ResourceManager resManager = new ResourceManager("PaintDotNet.Effects.Properties.Resources", assembly);

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

            resource = hiDpi ?
                $"{resName}.32" :
                $"{resName}";

            object obj = resManager.GetObject(resource);

            if (obj != null && obj is Bitmap bitmap)
            {
                return bitmap;
            }

            return badIcon;
        }

        static ResUtil()
        {
            string resource = hiDpi ?
                $"PaintDotNet.Effects.Icons.BadIcon.32.png" :
                $"PaintDotNet.Effects.Icons.BadIcon.png";

            using (Stream imageStream = assembly.GetManifestResourceStream(resource))
            {
                badIcon = Image.FromStream(imageStream);
            }
        }

    }
}
