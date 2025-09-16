#if FASTDEBUG
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Windows.Forms;

namespace PdnCodeLab
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

            // hack to get Controls derived from Direct2DControl working in FastDebug
            {
                Control syncContextControl = new Control(); // this forces WinForms SyncContext to be installed

                MethodInfo syncContextInstall = AppDomain.CurrentDomain
                    .GetAssemblies()
                    .First(a => a.GetName().Name == "PaintDotNet.Base")
                    .GetTypes()
                    .First(a => a.Name == "PdnSynchronizationContext")
                    .GetMethod("Install", BindingFlags.NonPublic | BindingFlags.Static);

                Type delegateType = Expression.GetDelegateType(syncContextInstall.ReturnType);

                syncContextInstall.CreateDelegate(delegateType).DynamicInvoke();
            }

            Application.Run(new CodeLabConfigDialog(RenderPreset.Regular));
        }
    }
}
#endif
