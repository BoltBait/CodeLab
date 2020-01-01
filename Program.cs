#if FASTDEBUG
using System;
using System.Windows.Forms;

namespace PaintDotNet.Effects
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            ShapeBuilder.SetEnviromentParams(100, 100, 0, 0, 100, 100, ColorBgra.Black, ColorBgra.White, 2);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new CodeLabConfigDialog());
        }
    }
}
#endif
