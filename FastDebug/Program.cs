#if FASTDEBUG
using System;
using System.Windows.Forms;
using TerraFX.Interop.Windows;
using TerraFX.Interop.WinRT;
using Windows.System;

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
            DispatcherQueueController dispatcherQueueController = CreateForCurrentThread();

            Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new CodeLabConfigDialog(RenderPreset.Regular));

            dispatcherQueueController.ShutdownQueueAsync();
        }

        private static unsafe DispatcherQueueController CreateForCurrentThread()
        {
            using ComPtr<IDispatcherQueueController> spController = default;
            HRESULT hr = TerraFX.Interop.Windows.Windows.CreateDispatcherQueueController(
                new DispatcherQueueOptions()
                {
                    dwSize = (uint)sizeof(DispatcherQueueOptions),
                    apartmentType = DISPATCHERQUEUE_THREAD_APARTMENTTYPE.DQTAT_COM_STA,
                    threadType = DISPATCHERQUEUE_THREAD_TYPE.DQTYPE_THREAD_CURRENT
                },
                spController.GetAddressOf());

            return DispatcherQueueController.FromAbi((IntPtr)spController.Get());
        }
    }
}
#endif
