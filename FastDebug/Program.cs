﻿#if FASTDEBUG
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
            Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new CodeLabConfigDialog());
        }
    }
}
#endif
