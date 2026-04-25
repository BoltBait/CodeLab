using PaintDotNet.Direct2D1;
using System;

namespace PdnCodeLab
{
    internal static class DefaultCode
    {
        internal static string Default => ForProjectType(ProjectType.Default);

        internal static string ForProjectType(ProjectType projectType) => projectType switch
        {
            ProjectType.BitmapEffect => BitmapEffect,
            ProjectType.GpuImageEffect => GPUEffect,
            ProjectType.GpuDrawEffect => GPUDrawEffect,
            _ => string.Empty,
        };

        private const string BitmapEffect = ""
            + "// Name:\r\n"
            + "// Submenu:\r\n"
            + "// Author:\r\n"
            + "// Title:\r\n"
            + "// Version:\r\n"
            + "// Desc:\r\n"
            + "// Keywords:\r\n"
            + "// URL:\r\n"
            + "// Help:\r\n"
            + "\r\n// For help writing a Bitmap plugin: https://boltbait.com/pdn/CodeLab/help/tutorial/bitmap/\r\n\r\n"
            + "#region UICode\r\n"
            + "IntSliderControl Amount1 = 0; // [0,100] Slider 1 Description\r\n"
            + "IntSliderControl Amount2 = 0; // [0,100] Slider 2 Description\r\n"
            + "IntSliderControl Amount3 = 0; // [0,100] Slider 3 Description\r\n"
            + "#endregion\r\n"
            + "\r\n"
            + "protected override void OnRender(IBitmapEffectOutput output)\r\n"
            + "{\r\n"
            + "    using IEffectInputBitmap<ColorBgra32> sourceBitmap = Environment.GetSourceBitmapBgra32();\r\n"
            + "    using IBitmapLock<ColorBgra32> sourceLock = sourceBitmap.Lock(new RectInt32(0, 0, sourceBitmap.Size));\r\n"
            + "    RegionPtr<ColorBgra32> sourceRegion = sourceLock.AsRegionPtr();\r\n"
            + "\r\n"
            + "    RectInt32 outputBounds = output.Bounds;\r\n"
            + "    using IBitmapLock<ColorBgra32> outputLock = output.LockBgra32();\r\n"
            + "    RegionPtr<ColorBgra32> outputSubRegion = outputLock.AsRegionPtr();\r\n"
            + "    var outputRegion = outputSubRegion.OffsetView(-outputBounds.Location);\r\n"
            + "    //uint seed = RandomNumber.InitializeSeed(RandomNumberRenderSeed, outputBounds.Location);\r\n"
            + "\r\n"
            + "    // Delete any of these lines you don't need\r\n"
            + "    ColorBgra32 primaryColor = Environment.PrimaryColor.GetBgra32(sourceBitmap.ColorContext);\r\n"
            + "    ColorBgra32 secondaryColor = Environment.SecondaryColor.GetBgra32(sourceBitmap.ColorContext);\r\n"
            + "    int canvasCenterX = Environment.Document.Size.Width / 2;\r\n"
            + "    int canvasCenterY = Environment.Document.Size.Height / 2;\r\n"
            + "    var selection = Environment.Selection.RenderBounds;\r\n"
            + "    int selectionCenterX = (selection.Right - selection.Left) / 2 + selection.Left;\r\n"
            + "    int selectionCenterY = (selection.Bottom - selection.Top) / 2 + selection.Top;\r\n"
            + "\r\n"
            + "    // Loop through the output canvas tile\r\n"
            + "    for (int y = outputBounds.Top; y < outputBounds.Bottom; ++y)\r\n"
            + "    {\r\n"
            + "        if (IsCancelRequested) return;\r\n"
            + "\r\n"
            + "        for (int x = outputBounds.Left; x < outputBounds.Right; ++x)\r\n"
            + "        {\r\n"
            + "            // Get your source pixel\r\n"
            + "            ColorBgra32 sourcePixel = sourceRegion[x,y];\r\n"
            + "\r\n"
            + "            // TODO: Change source pixel according to some algorithm\r\n"
            + "            //sourcePixel.B = (byte)(Amount1 * 255 / 100); // Blue\r\n"
            + "            //sourcePixel.G = (byte)(Amount2 * 255 / 100); // Green\r\n"
            + "            //sourcePixel.R = (byte)(Amount3 * 255 / 100); // Red\r\n"
            + "            //sourcePixel.A = 255; // Alpha Transparency\r\n"
            + "\r\n"
            + "            // Save your pixel to the output canvas\r\n"
            + "            outputRegion[x,y] = sourcePixel;\r\n"
            + "        }\r\n"
            + "    }\r\n"
            + "}\r\n";

        private const string GPUEffect = ""
            + "// Name:\r\n"
            + "// Submenu:\r\n"
            + "// Author:\r\n"
            + "// Title:\r\n"
            + "// Version:\r\n"
            + "// Desc:\r\n"
            + "// Keywords:\r\n"
            + "// URL:\r\n"
            + "// Help:\r\n"
            + "\r\n// For help writing a GPU Image plugin: https://boltbait.com/pdn/CodeLab/help/tutorial/image/\r\n\r\n"
            + "#region UICode\r\n"
            + "IntSliderControl Amount1 = 0; // [0,100] Slider 1 Description\r\n"
            + "IntSliderControl Amount2 = 0; // [0,100] Slider 2 Description\r\n"
            + "IntSliderControl Amount3 = 0; // [0,100] Slider 3 Description\r\n"
            + "#endregion\r\n"
            + "\r\n"
            + "protected override IDeviceImage OnCreateOutput(IDeviceContext deviceContext)\r\n"
            + "{\r\n"
            + "    // TODO: replace this return statement with your GPU pipeline algorithm\r\n"
            + "    return Environment.SourceImage;\r\n"
            + "}\r\n";

        private const string GPUDrawEffect = ""
            + "// Name:\r\n"
            + "// Submenu:\r\n"
            + "// Author:\r\n"
            + "// Title:\r\n"
            + "// Version:\r\n"
            + "// Desc:\r\n"
            + "// Keywords:\r\n"
            + "// URL:\r\n"
            + "// Help:\r\n"
            + "\r\n// For help writing a GPU Drawing plugin: https://boltbait.com/pdn/CodeLab/help/tutorial/drawing/\r\n\r\n"
            + "#region UICode\r\n"
            + "IntSliderControl Amount1 = 0; // [0,100] Slider 1 Description\r\n"
            + "IntSliderControl Amount2 = 0; // [0,100] Slider 2 Description\r\n"
            + "IntSliderControl Amount3 = 0; // [0,100] Slider 3 Description\r\n"
            + "#endregion\r\n"
            + "\r\n"
            + "protected override unsafe void OnDraw(IDeviceContext deviceContext)\r\n"
            + "{\r\n"
            + "    // TODO: replace this DrawImage statement with your GPU Drawing statements\r\n"
            + "    deviceContext.DrawImage(Environment.SourceImage);\r\n"
            + "}\r\n";
    }
}
