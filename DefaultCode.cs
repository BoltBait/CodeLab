using PaintDotNet.Direct2D1;
using System;

namespace PdnCodeLab
{
    internal static class DefaultCode
    {
        internal static string Default => ProjectType.Default switch
        {
            ProjectType.ClassicEffect => ClassicEffect,
            ProjectType.BitmapEffect => BitmapEffect,
            ProjectType.GpuEffect => GPUEffect,
            _ => string.Empty,
        };

        internal const string ClassicEffect = ""
            + "// Name:\r\n"
            + "// Submenu:\r\n"
            + "// Author:\r\n"
            + "// Title:\r\n"
            + "// Version:\r\n"
            + "// Desc:\r\n"
            + "// Keywords:\r\n"
            + "// URL:\r\n"
            + "// Help:\r\n"
            + "#region UICode\r\n"
            + "IntSliderControl Amount1 = 0; // [0,100] Slider 1 Description\r\n"
            + "IntSliderControl Amount2 = 0; // [0,100] Slider 2 Description\r\n"
            + "IntSliderControl Amount3 = 0; // [0,100] Slider 3 Description\r\n"
            + "#endregion\r\n"
            + "\r\n"
            + "void Render(Surface dst, Surface src, Rectangle rect)\r\n"
            + "{\r\n"
            + "    // Delete any of these lines you don't need\r\n"
            + "    Rectangle selection = EnvironmentParameters.SelectionBounds;\r\n"
            + "    int centerX = ((selection.Right - selection.Left) / 2) + selection.Left;\r\n"
            + "    int centerY = ((selection.Bottom - selection.Top) / 2) + selection.Top;\r\n"
            + "    ColorBgra primaryColor = EnvironmentParameters.PrimaryColor;\r\n"
            + "    ColorBgra secondaryColor = EnvironmentParameters.SecondaryColor;\r\n"
            + "    int brushWidth = (int)EnvironmentParameters.BrushWidth;\r\n"
            + "\r\n"
            + "    ColorBgra currentPixel;\r\n"
            + "    for (int y = rect.Top; y < rect.Bottom; y++)\r\n"
            + "    {\r\n"
            + "        if (IsCancelRequested) return;\r\n"
            + "        for (int x = rect.Left; x < rect.Right; x++)\r\n"
            + "        {\r\n"
            + "            currentPixel = src[x,y];\r\n"
            + "            // TODO: Add pixel processing code here\r\n"
            + "            // Access RGBA values this way, for example:\r\n"
            + "            // currentPixel.R = primaryColor.R;\r\n"
            + "            // currentPixel.G = primaryColor.G;\r\n"
            + "            // currentPixel.B = primaryColor.B;\r\n"
            + "            // currentPixel.A = primaryColor.A;\r\n"
            + "            dst[x,y] = currentPixel;\r\n"
            + "        }\r\n"
            + "    }\r\n"
            + "}\r\n";

        internal const string BitmapEffect = ""
            + "// Name:\r\n"
            + "// Submenu:\r\n"
            + "// Author:\r\n"
            + "// Title:\r\n"
            + "// Version:\r\n"
            + "// Desc:\r\n"
            + "// Keywords:\r\n"
            + "// URL:\r\n"
            + "// Help:\r\n"
            + "#region UICode\r\n"
            + "IntSliderControl Amount1 = 0; // [0,100] Slider 1 Description\r\n"
            + "IntSliderControl Amount2 = 0; // [0,100] Slider 2 Description\r\n"
            + "IntSliderControl Amount3 = 0; // [0,100] Slider 3 Description\r\n"
            + "#endregion\r\n"
            + "\r\n"
            + "protected override void OnRender(IBitmapEffectOutput output)\r\n"
            + "{\r\n"
            + "    using IEffectInputBitmap<ColorBgra32> sourceBitmap = Environment.GetSourceBitmapBgra32();\r\n"
            + "    using IBitmapLock<ColorBgra32> sourceLock = Environment.GetSourceBitmapBgra32().Lock(new RectInt32(0, 0, sourceBitmap.Size));\r\n"
            + "    RegionPtr<ColorBgra32> sourceRegion = sourceLock.AsRegionPtr();\r\n"
            + "\r\n"
            + "    RectInt32 outputBounds = output.Bounds;\r\n"
            + "    using IBitmapLock<ColorBgra32> outputLock = output.LockBgra32();\r\n"
            + "    RegionPtr<ColorBgra32> outputSubRegion = outputLock.AsRegionPtr();\r\n"
            + "    var outputRegion = outputSubRegion.OffsetView(-outputBounds.Location);\r\n"
            + "\r\n"
            + "    // Delete any of these lines you don't need\r\n"
            + "    ColorBgra32 primaryColor = Environment.PrimaryColor;\r\n"
            + "    ColorBgra32 secondaryColor = Environment.SecondaryColor;\r\n"
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

        internal const string GPUEffect = ""
            + "// Name:\r\n"
            + "// Submenu:\r\n"
            + "// Author:\r\n"
            + "// Title:\r\n"
            + "// Version:\r\n"
            + "// Desc:\r\n"
            + "// Keywords:\r\n"
            + "// URL:\r\n"
            + "// Help:\r\n"
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

        internal const string FileType = "" +
            "// Name:\r\n" +
            "// Author:\r\n" +
            "// Version:\r\n" +
            "// Desc:\r\n" +
            "// URL:\r\n" +
            "// LoadExtns: .foo, .bar\r\n" +
            "// SaveExtns: .foo, .bar\r\n" +
            "// Flattened: false\r\n" +
            "#region UICode\r\n" +
            "CheckboxControl Amount1 = false; // Checkbox Description\r\n" +
            "#endregion\r\n" +
            "\r\n" +
            "private const string HeaderSignature = \".PDN\";\r\n" +
            "\r\n" +
            "void SaveImage(Document input, Stream output, PropertyBasedSaveConfigToken token, Surface scratchSurface, ProgressEventHandler progressCallback)\r\n" +
            "{\r\n" +
            "    using (RenderArgs args = new RenderArgs(scratchSurface))\r\n" +
            "    {\r\n" +
            "        // Render a flattened view of the Document to the scratch surface.\r\n" +
            "        scratchSurface.Clear();\r\n" +
            "        input.CreateRenderer().Render(scratchSurface);\r\n" +
            "    }\r\n" +
            "\r\n" +
            "    if (Amount1)\r\n" +
            "    {\r\n" +
            "        new UnaryPixelOps.Invert().Apply(scratchSurface, scratchSurface.Bounds);\r\n" +
            "    }\r\n" +
            "\r\n" +
            "    // The stream paint.net hands us must not be closed.\r\n" +
            "    using (BinaryWriter writer = new BinaryWriter(output, Encoding.UTF8, leaveOpen: true))\r\n" +
            "    {\r\n" +
            "        // Write the file header.\r\n" +
            "        writer.Write(Encoding.ASCII.GetBytes(HeaderSignature));\r\n" +
            "        writer.Write(scratchSurface.Width);\r\n" +
            "        writer.Write(scratchSurface.Height);\r\n" +
            "\r\n" +
            "        for (int y = 0; y < scratchSurface.Height; y++)\r\n" +
            "        {\r\n" +
            "            // Report progress if the callback is not null.\r\n" +
            "            if (progressCallback != null)\r\n" +
            "            {\r\n" +
            "                double percent = (double)y / scratchSurface.Height;\r\n" +
            "\r\n" +
            "                progressCallback(null, new ProgressEventArgs(percent));\r\n" +
            "            }\r\n" +
            "\r\n" +
            "            for (int x = 0; x < scratchSurface.Width; x++)\r\n" +
            "            {\r\n" +
            "                // Write the pixel values.\r\n" +
            "                ColorBgra color = scratchSurface[x, y];\r\n" +
            "\r\n" +
            "                writer.Write(color.Bgra);\r\n" +
            "            }\r\n" +
            "        }\r\n" +
            "    }\r\n" +
            "}\r\n" +
            "\r\n" +
            "Document LoadImage(Stream input)\r\n" +
            "{\r\n" +
            "    Document doc = null;\r\n" +
            "\r\n" +
            "    // The stream paint.net hands us must not be closed.\r\n" +
            "    using (BinaryReader reader = new BinaryReader(input, Encoding.UTF8, leaveOpen: true))\r\n" +
            "    {\r\n" +
            "        // Read and validate the file header.\r\n" +
            "        byte[] headerSignature = reader.ReadBytes(4);\r\n" +
            "\r\n" +
            "        if (Encoding.ASCII.GetString(headerSignature) != HeaderSignature)\r\n" +
            "        {\r\n" +
            "            throw new FormatException(\"Invalid file signature.\");\r\n" +
            "        }\r\n" +
            "\r\n" +
            "        int width = reader.ReadInt32();\r\n" +
            "        int height = reader.ReadInt32();\r\n" +
            "\r\n" +
            "        // Create a new Document.\r\n" +
            "        doc = new Document(width, height);\r\n" +
            "\r\n" +
            "        // Create a background layer.\r\n" +
            "        BitmapLayer layer = Layer.CreateBackgroundLayer(width, height);\r\n" +
            "\r\n" +
            "        for (int y = 0; y < height; y++)\r\n" +
            "        {\r\n" +
            "            for (int x = 0; x < width; x++)\r\n" +
            "            {\r\n" +
            "                // Read the pixel values from the file.\r\n" +
            "                uint bgraColor = reader.ReadUInt32();\r\n" +
            "\r\n" +
            "                layer.Surface[x, y] = ColorBgra.FromUInt32(bgraColor);\r\n" +
            "            }\r\n" +
            "        }\r\n" +
            "\r\n" +
            "        // Add the new layer to the Document.\r\n" +
            "        doc.Layers.Add(layer);\r\n" +
            "    }\r\n" +
            "\r\n" +
            "    return doc;\r\n" +
            "}\r\n" +
            "\r\n";
    }
}
