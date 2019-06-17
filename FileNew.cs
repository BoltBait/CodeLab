/////////////////////////////////////////////////////////////////////////////////
// CodeLab for Paint.NET
// Portions Copyright ©2007-2018 BoltBait. All Rights Reserved.
// Portions Copyright ©2018 Jason Wendt. All Rights Reserved.
// Portions Copyright ©2019 Nicholas Hayes. All Rights Reserved.
// Portions Copyright ©Microsoft Corporation. All Rights Reserved.
//
// THE CODELAB DEVELOPERS MAKE NO WARRANTY OF ANY KIND REGARDING THE CODE. THEY
// SPECIFICALLY DISCLAIM ANY WARRANTY OF FITNESS FOR ANY PARTICULAR PURPOSE OR
// ANY OTHER WARRANTY.  THE CODELAB DEVELOPERS DISCLAIM ALL LIABILITY RELATING
// TO THE USE OF THIS CODE.  NO LICENSE, EXPRESS OR IMPLIED, BY ESTOPPEL OR
// OTHERWISE, TO ANY INTELLECTUAL PROPERTY RIGHTS IS GRANTED HEREIN.
//
// Latest distribution: https://www.BoltBait.com/pdn/codelab
/////////////////////////////////////////////////////////////////////////////////

using System;
using System.Windows.Forms;

namespace PaintDotNet.Effects
{
    internal partial class FileNew : Form
    {
        internal string CodeTemplate;

        internal FileNew(string EffectFlag)
        {
            InitializeComponent();

            // PDN Theme
            this.ForeColor = PdnTheme.ForeColor;
            this.BackColor = PdnTheme.BackColor;
            foreach (Control control in this.Controls)
            {
                if (control is ComboBox)
                {
                    control.ForeColor = PdnTheme.ForeColor;
                    control.BackColor = PdnTheme.BackColor;
                }
            }

            BlendingCode.Text = "Pass Through";
            PixelOpCode.Text = "Pass Through";
            FinalPixelOpCode.Text = "Pass Through";
            EffectCode.Text = "---------Copy--------->";
            dstLabel.Text = "DST\r\nIMAGE";

            if (EffectFlag.Contains("Aliased Selection"))
            {
                FAS.Checked = true;
            }
            else if (EffectFlag.Contains("Legacy ROI"))
            {
                LROI.Checked = true;
            }
            else if (EffectFlag.Contains("Single Render Call"))
            {
                SRC.Checked = true;
            }
        }

        private void DoIt_Click(object sender, EventArgs e)
        {
            const string cr = "\r\n";
            string wrksurface = "dst";
            string code = "// Name:\r\n// Submenu:\r\n// Author:\r\n// Title:\r\n// Version:\r\n// Desc:\r\n// Keywords:\r\n// URL:\r\n// Help:\r\n";
            if (FAS.Checked)
            {
                code += "// Force Aliased Selection\r\n";
            }
            if (ST.Checked)
            {
                code += "// Force Single Threaded\r\n";
            }
            if (LROI.Checked)
            {
                code += "// Force Legacy ROI\r\n";
            }
            if (SRC.Checked)
            {
                code += "// Force Single Render Call\r\n";
            }
            string controls = "1";
            string destcode = "dst[x,y]";
            string srccode = "src[x,y]";
            string wrkcode = "wrk[x,y]";
            string blendtop = "";
            string EffectName = "";
            /*
            ---------Copy--------->
                        Empty------->
                    Clipboard------>
                    Clouds------->
            ---Edge Detect--->
            -------Emboss------>
            ---Gaussian Blur-->
            ----Motion Blur---->
            ----Oil Painting---->
            --------Relief-------->
            --------Sepia-------->
            -------Contrast------->
            ------Outline------->
            ------Sharpen------->
            ---Pencil Sketch--->
            ---Frosted Glass--->
            ----Add Noise----->
            ---Reduce Noise--->
            ------Median------>
            -----Posterize------>
            */

            if (AdvancedStyle.Checked)
            {
                destcode = "*dstPtr";
                srccode = "*srcPtr";
                wrkcode = "*wrkPtr";
            }

            // Let's write some code!
            code += "#region UICode" + cr;

            // Add controls for selected options
            if (EffectCode.Text.Contains("Gaussian Blur"))
            {
                code += "IntSliderControl Amount1=10; // [0,100] Radius" + cr;
                controls = "2";
            }
            else if (EffectCode.Text.Contains("Contrast"))
            {
                code += "IntSliderControl Amount1=10; // [-100,100] Brightness" + cr;
                code += "IntSliderControl Amount2=10; // [-100,100] Contrast" + cr;
                controls = "3";
            }
            else if (EffectCode.Text.Contains("Motion Blur"))
            {
                code += "AngleControl Amount1 = 45; // [-180,180] Angle" + cr;
                code += "CheckboxControl Amount2 = true; // [0,1] Centered" + cr;
                code += "IntSliderControl Amount3 = 10; // [1,200] Distance" + cr;
                controls = "4";
            }
            else if (EffectCode.Text.Contains("Frosted"))
            {
                code += "DoubleSliderControl Amount1 = 3; // [0,200] Maximum Scatter Radius" + cr;
                code += "DoubleSliderControl Amount2 = 0; // [0,200] Minimum Scatter Radius" + cr;
                code += "IntSliderControl Amount3 = 2; // [1,8] Smoothness" + cr;
                controls = "4";
            }
            else if (EffectCode.Text.Contains("Add Noise"))
            {
                code += "IntSliderControl Amount1 = 64; // [0,100] Intensity" + cr;
                code += "IntSliderControl Amount2 = 100; // [0,400] Color Saturation" + cr;
                code += "DoubleSliderControl Amount3 = 100; // [0,100] Coverage" + cr;
                controls = "4";
            }
            else if (EffectCode.Text.Contains("Clouds"))
            {
                code += "IntSliderControl Amount1 = 250; // [2,1000] Scale" + cr;
                code += "DoubleSliderControl Amount2 = 0.5; // [0,1] Roughness" + cr;
                code += "BinaryPixelOp Amount3 = LayerBlendModeUtil.CreateCompositionOp(LayerBlendMode.Normal); // Blend Mode" + cr;
                code += "ReseedButtonControl Amount4 = 0; // [255] Reseed" + cr;
                controls = "5";
            }
            else if (EffectCode.Text.Contains("Oil Painting"))
            {
                code += "IntSliderControl Amount1=3; // [1,8] Brush Size" + cr;
                code += "IntSliderControl Amount2=50; // [3,255] Coarseness" + cr;
                controls = "3";
            }
            else if (EffectCode.Text.Contains("Reduce Noise"))
            {
                code += "IntSliderControl Amount1=10; // [0,200] Radius" + cr;
                code += "DoubleSliderControl Amount2=0.4; // [0,1] Strength" + cr;
                controls = "3";
            }
            else if (EffectCode.Text.Contains("Median"))
            {
                code += "IntSliderControl Amount1=10; // [1,200] Radius" + cr;
                code += "IntSliderControl Amount2=50; // [0,100] Percentile" + cr;
                controls = "3";
            }
            else if (EffectCode.Text.Contains("Sharpen"))
            {
                code += "IntSliderControl Amount1=2; // [1,20] Amount" + cr;
                controls = "2";
            }
            else if (EffectCode.Text.Contains("Edge Detect"))
            {
                code += "AngleControl Amount1 = 45; // [-180,180] Angle" + cr;
                controls = "2";
            }
            else if (EffectCode.Text.Contains("Emboss"))
            {
                code += "AngleControl Amount1 = 0; // [-180,180] Angle" + cr;
                controls = "2";
            }
            else if (EffectCode.Text.Contains("Relief"))
            {
                code += "AngleControl Amount1 = 45; // [-180,180] Angle" + cr;
                controls = "2";
            }
            else if (EffectCode.Text.Contains("Outline"))
            {
                code += "IntSliderControl Amount1=3; // [1,200] Thickness" + cr;
                code += "IntSliderControl Amount2=50; // [0,100] Intensity" + cr;
                controls = "3";
            }
            else if (EffectCode.Text.Contains("Pencil Sketch"))
            {
                code += "IntSliderControl Amount1=2; // [1,20] Pencil Tip Size" + cr;
                code += "IntSliderControl Amount2=0; // [-20,20] Range" + cr;
                controls = "3";
            }
            else if (EffectCode.Text.Contains("Posterize"))
            {
                code += "IntSliderControl Amount1=16; // [2,64] Red" + cr;
                code += "IntSliderControl Amount2=16; // [2,64] Green" + cr;
                code += "IntSliderControl Amount3=16; // [2,64] Blue" + cr;
                code += "CheckboxControl Amount4 = true; // [0,1] Linked" + cr;
                controls = "5";
            }
            else if (EffectCode.Text.Contains("Ink Sketch"))
            {
                code += "IntSliderControl Amount1 = 50; // [0,99] Ink Outline" + cr;
                code += "IntSliderControl Amount2 = 50; // [0,100] Coloring" + cr;
                controls = "3";
            }
            else if (EffectCode.Text.Contains("Radial Blur"))
            {
                code += "AngleControl Amount1 = 2; // [-180,180] Angle" + cr;
                code += "PanSliderControl Amount2 = Pair.Create(0.000,0.000); // Center" + cr;
                code += "IntSliderControl Amount3 = 2; // [1,5] Quality" + cr;
                controls = "4";
            }
            else if (EffectCode.Text.Contains("Surface Blur"))
            {
                code += "IntSliderControl Amount1 = 6; // [1,100] Radius" + cr;
                code += "IntSliderControl Amount2 = 15; // [1,100] Threshold" + cr;
                controls = "3";
            }
            else if (EffectCode.Text.Contains("Unfocus"))
            {
                code += "IntSliderControl Amount1 = 4; // [1,200] Radius" + cr;
                controls = "2";
            }
            else if (EffectCode.Text.Contains("Zoom Blur"))
            {
                code += "IntSliderControl Amount1 = 10; // [0,100] Zoom Amount" + cr;
                code += "PanSliderControl Amount2 = Pair.Create(0.000,0.000); // Center" + cr;
                controls = "3";
            }
            else if (EffectCode.Text.Contains("Bulge"))
            {
                code += "IntSliderControl Amount1 = 45; // [-200,100] Bulge" + cr;
                code += "PanSliderControl Amount2 = Pair.Create(0.000,0.000); // Center" + cr;
                controls = "3";
            }
            else if (EffectCode.Text.Contains("Crystalize"))
            {
                code += "IntSliderControl Amount1 = 8; // [2,250] Cell Size" + cr;
                code += "IntSliderControl Amount2 = 2; // [1,5] Quality" + cr;
                code += "ReseedButtonControl Amount3 = 0; // [255] Reseed" + cr;
                controls = "4";
            }
            else if (EffectCode.Text.Contains("Dents"))
            {
                code += "DoubleSliderControl Amount1 = 25; // [1,200] Scale" + cr;
                code += "DoubleSliderControl Amount2 = 50; // [0,200] Refraction" + cr;
                code += "DoubleSliderControl Amount3 = 10; // [0,100] Roughness" + cr;
                code += "DoubleSliderControl Amount4 = 10; // [0,100] Tension" + cr;
                code += "IntSliderControl Amount5 = 2; // [1,5] Quality" + cr;
                code += "ReseedButtonControl Amount6 = 0; // [255] Reseed" + cr;
                controls = "7";
            }
            else if (EffectCode.Text.Contains("Pixelate"))
            {
                code += "IntSliderControl Amount1 = 2; // [1,100] Cell size" + cr;
                controls = "2";
            }
            else if (EffectCode.Text.Contains("Polar Inversion"))
            {
                code += "DoubleSliderControl Amount1 = 3; // [0,200] Amount" + cr;
                code += "PanSliderControl Amount2 = Pair.Create(0.000,0.000); // Offset" + cr;
                code += "ListBoxControl Amount3 = 2; // Edge Behavior|Clamp|Reflect|Wrap" + cr;
                code += "IntSliderControl Amount4 = 2; // [1,5] Quality" + cr;
                controls = "5";
            }
            else if (EffectCode.Text.Contains("Tile Reflection"))
            {
                code += "AngleControl Amount1 = 30; // [-180,180] Angle" + cr;
                code += "DoubleSliderControl Amount2 = 40; // [1,800] Tile Size" + cr;
                code += "DoubleSliderControl Amount3 = 8; // [-100,100] Curvature" + cr;
                code += "IntSliderControl Amount4 = 2; // [1,5] Quality" + cr;
                controls = "5";
            }
            else if (EffectCode.Text.Contains("Twist"))
            {
                code += "DoubleSliderControl Amount1 = 30; // [-200,200] Amount / Direction" + cr;
                code += "DoubleSliderControl Amount2 = 1; // [0.01,2] Size" + cr;
                code += "PanSliderControl Amount3 = Pair.Create(0.000,0.000); // Center" + cr;
                code += "IntSliderControl Amount4 = 2; // [1,5] Quality" + cr;
                controls = "5";
            }
            else if (EffectCode.Text.Contains("Glow"))
            {
                code += "IntSliderControl Amount1 = 6; // [1,20] Radius" + cr;
                code += "IntSliderControl Amount2 = 10; // [-100,100] Brightness" + cr;
                code += "IntSliderControl Amount3 = 10; // [-100,100] Contrast" + cr;
                controls = "4";
            }
            else if (EffectCode.Text.Contains("Soften Portrait"))
            {
                code += "IntSliderControl Amount1 = 5; // [0,10] Softness" + cr;
                code += "IntSliderControl Amount2 = 0; // [-20,20] Lighting" + cr;
                code += "IntSliderControl Amount3 = 10; // [0,20] Warmth" + cr;
                controls = "4";
            }
            else if (EffectCode.Text.Contains("Vignette"))
            {
                code += "PanSliderControl Amount1 = Pair.Create(0.000,0.000); // Center" + cr;
                code += "DoubleSliderControl Amount2 = 0.5; // [0.1,4] Radius" + cr;
                code += "DoubleSliderControl Amount3 = 1; // [0,1] Density" + cr;
                controls = "4";
            }
            else if (EffectCode.Text.Contains("Julia"))
            {
                code += "DoubleSliderControl Amount1 = 4; // [1,10] Factor" + cr;
                code += "DoubleSliderControl Amount2 = 1; // [0.1,50] Zoom" + cr;
                code += "AngleControl Amount3 = 0; // [-180,180] Angle" + cr;
                code += "IntSliderControl Amount4 = 2; // [1,5] Quality" + cr;
                controls = "5";
            }
            else if (EffectCode.Text.Contains("Mandelbrot"))
            {
                code += "IntSliderControl Amount1 = 1; // [1,10] Factor" + cr;
                code += "DoubleSliderControl Amount2 = 10; // [0,100] Zoom" + cr;
                code += "AngleControl Amount3 = 0; // [-180,180] Angle" + cr;
                code += "IntSliderControl Amount4 = 2; // [1,5] Quality" + cr;
                code += "CheckboxControl Amount5 = false; // [0,1] Invert Colors" + cr;
                controls = "6";
            }
            else if (EffectCode.Text.Contains("Sepia"))
            {
                controls = "1";
            }
            else if (EffectCode.Text.Contains("Copy"))
            {
                controls = "1";
            }
            else if (EffectCode.Text.Contains("Empty"))
            {
                controls = "1";
            }

            if (BlendingCode.Text == "User selected blending mode")
            {
                code += "BinaryPixelOp Amount" + controls + " = LayerBlendModeUtil.CreateCompositionOp(LayerBlendMode.Normal); // Blending Mode" + cr;
            }
            code += "#endregion" + cr;
            code += cr;
            if (SurfaceCode.Checked)
            {
                code += "// Working surface" + cr;
                code += "Surface wrk = null;" + cr;
                code += cr;
                // we will be using the wrk surface for the destination of the complex effects
                wrksurface = "wrk";
            }

            // setup for calling complex effects
            if (EffectCode.Text.Contains("Gaussian Blur"))
            {
                code += "// Setup for calling the Gaussian Blur effect" + cr;
                code += "GaussianBlurEffect blurEffect = new GaussianBlurEffect();" + cr;
                code += "PropertyCollection blurProps;" + cr;
                code += cr;
                EffectName = "blurEffect";
            }
            else if (EffectCode.Text.Contains("Contrast"))
            {
                code += "// Setup for calling the Brightness and Contrast Adjustment function" + cr;
                code += "BrightnessAndContrastAdjustment bacAdjustment = new BrightnessAndContrastAdjustment();" + cr;
                code += "PropertyCollection bacProps;" + cr;
                code += cr;
                EffectName = "bacAdjustment";
            }
            else if (EffectCode.Text.Contains("Frosted"))
            {
                code += "// Setup for calling the Frosted Glass effect" + cr;
                code += "FrostedGlassEffect frostedEffect = new FrostedGlassEffect();" + cr;
                code += "PropertyCollection frostedProps;" + cr;
                code += cr;
                EffectName = "frostedEffect";
            }
            else if (EffectCode.Text.Contains("Add Noise"))
            {
                code += "// Setup for calling the Add Noise effect" + cr;
                code += "AddNoiseEffect noiseEffect = new AddNoiseEffect();" + cr;
                code += "PropertyCollection noiseProps;" + cr;
                code += cr;
                EffectName = "noiseEffect";
            }
            else if (EffectCode.Text.Contains("Motion Blur"))
            {
                code += "// Setup for calling the Motion Blur effect" + cr;
                code += "MotionBlurEffect blurEffect = new MotionBlurEffect();" + cr;
                code += "PropertyCollection blurProps;" + cr;
                code += cr;
                EffectName = "blurEffect";
            }
            else if (EffectCode.Text.Contains("Clouds"))
            {
                code += "// Setup for calling the Render Clouds function" + cr;
                code += "CloudsEffect cloudsEffect = new CloudsEffect();" + cr;
                code += "PropertyCollection cloudsProps;" + cr;
                code += cr;
                EffectName = "cloudsEffect";
            }
            else if (EffectCode.Text.Contains("Oil Painting"))
            {
                code += "// Setup for calling the Oil Painting effect" + cr;
                code += "OilPaintingEffect oilpaintEffect = new OilPaintingEffect();" + cr;
                code += "PropertyCollection oilpaintProps;" + cr;
                code += cr;
                EffectName = "oilpaintEffect";
            }
            else if (EffectCode.Text.Contains("Reduce Noise"))
            {
                code += "// Setup for calling the Reduce Noise effect" + cr;
                code += "ReduceNoiseEffect reducenoiseEffect = new ReduceNoiseEffect();" + cr;
                code += "PropertyCollection reducenoiseProps;" + cr;
                code += cr;
                EffectName = "reducenoiseEffect";
            }
            else if (EffectCode.Text.Contains("Median"))
            {
                code += "// Setup for calling the Median effect" + cr;
                code += "MedianEffect medianEffect = new MedianEffect();" + cr;
                code += "PropertyCollection medianProps;" + cr;
                code += cr;
                EffectName = "medianEffect";
            }
            else if (EffectCode.Text.Contains("Edge Detect"))
            {
                code += "// Setup for calling the Edge Detect effect" + cr;
                code += "EdgeDetectEffect edgedetectEffect = new EdgeDetectEffect();" + cr;
                code += "PropertyCollection edgeProps;" + cr;
                code += cr;
                EffectName = "edgedetectEffect";
            }
            else if (EffectCode.Text.Contains("Emboss"))
            {
                code += "// Setup for calling the Emboss effect" + cr;
                code += "EmbossEffect embossEffect = new EmbossEffect();" + cr;
                code += "PropertyCollection embossProps;" + cr;
                code += cr;
                EffectName = "embossEffect";
            }
            else if (EffectCode.Text.Contains("Relief"))
            {
                code += "// Setup for calling the Relief effect" + cr;
                code += "ReliefEffect reliefEffect = new ReliefEffect();" + cr;
                code += "PropertyCollection reliefProps;" + cr;
                code += cr;
                EffectName = "reliefEffect";
            }
            else if (EffectCode.Text.Contains("Outline"))
            {
                code += "// Setup for calling the Outline effect" + cr;
                code += "OutlineEffect outlineEffect = new OutlineEffect();" + cr;
                code += "PropertyCollection outlineProps;" + cr;
                code += cr;
                EffectName = "outlineEffect";
            }
            else if (EffectCode.Text.Contains("Sharpen"))
            {
                code += "// Setup for calling the Sharpen effect" + cr;
                code += "SharpenEffect sharpenEffect = new SharpenEffect();" + cr;
                code += "PropertyCollection sharpenProps;" + cr;
                code += cr;
                EffectName = "sharpenEffect";
            }
            else if (EffectCode.Text.Contains("Pencil Sketch"))
            {
                code += "// Setup for calling the Pencil Sketch effect" + cr;
                code += "PencilSketchEffect pencilSketchEffect = new PencilSketchEffect();" + cr;
                code += "PropertyCollection pencilSketchProps;" + cr;
                code += cr;
                EffectName = "pencilSketchEffect";
            }
            else if (EffectCode.Text.Contains("Posterize"))
            {
                code += "// Setup for calling the Posterize adjustment" + cr;
                code += "PosterizeAdjustment posterizeEffect = new PosterizeAdjustment();" + cr;
                code += "PropertyCollection posterizeProps;" + cr;
                code += cr;
                EffectName = "posterizeEffect";
            }
            else if (EffectCode.Text.Contains("Sepia"))
            {
                code += "// Setup for calling the Sepia effect" + cr;
                code += "SepiaEffect sepiaEffect = new SepiaEffect();" + cr;
                code += "PropertyCollection sepiaProps;" + cr;
                code += cr;
                EffectName = "sepiaEffect";
            }
            else if (EffectCode.Text.Contains("Ink Sketch"))
            {
                code += "// Setup for calling the Ink Sketch effect" + cr;
                code += "InkSketchEffect inkEffect = new InkSketchEffect();" + cr;
                code += "PropertyCollection inkProps;" + cr;
                code += cr;
                EffectName = "inkEffect";
            }
            else if (EffectCode.Text.Contains("Radial Blur"))
            {
                code += "// Setup for calling the Radial Blur effect" + cr;
                code += "RadialBlurEffect radialEffect = new RadialBlurEffect();" + cr;
                code += "PropertyCollection radialProps;" + cr;
                code += cr;
                EffectName = "radialEffect";
            }
            else if (EffectCode.Text.Contains("Surface Blur"))
            {
                code += "// Setup for calling the Surface Blur effect" + cr;
                code += "SurfaceBlurEffect surfaceEffect = new SurfaceBlurEffect();" + cr;
                code += "PropertyCollection surfaceProps;" + cr;
                code += cr;
                EffectName = "surfaceEffect";
            }
            else if (EffectCode.Text.Contains("Unfocus"))
            {
                code += "// Setup for calling the Unfocus effect" + cr;
                code += "UnfocusEffect unfocusEffect = new UnfocusEffect();" + cr;
                code += "PropertyCollection unfocusProps;" + cr;
                code += cr;
                EffectName = "unfocusEffect";
            }
            else if (EffectCode.Text.Contains("Zoom Blur"))
            {
                code += "// Setup for calling the Zoom Blur effect" + cr;
                code += "ZoomBlurEffect zoomEffect = new ZoomBlurEffect();" + cr;
                code += "PropertyCollection zoomProps;" + cr;
                code += cr;
                EffectName = "zoomEffect";
            }
            else if (EffectCode.Text.Contains("Bulge"))
            {
                code += "// Setup for calling the Bulge effect" + cr;
                code += "BulgeEffect bulgeEffect = new BulgeEffect();" + cr;
                code += "PropertyCollection bulgeProps;" + cr;
                code += cr;
                EffectName = "bulgeEffect";
            }
            else if (EffectCode.Text.Contains("Crystalize"))
            {
                code += "// Setup for calling the Crystalize effect" + cr;
                code += "CrystalizeEffect crystalizeEffect = new CrystalizeEffect();" + cr;
                code += "PropertyCollection crystalizeProps;" + cr;
                code += cr;
                EffectName = "crystalizeEffect";
            }
            else if (EffectCode.Text.Contains("Dents"))
            {
                code += "// Setup for calling the Dents effect" + cr;
                code += "DentsEffect dentsEffect = new DentsEffect();" + cr;
                code += "PropertyCollection dentsProps;" + cr;
                code += cr;
                EffectName = "dentsEffect";
            }
            else if (EffectCode.Text.Contains("Pixelate"))
            {
                code += "// Setup for calling the Pixelate effect" + cr;
                code += "PixelateEffect pixelateEffect = new PixelateEffect();" + cr;
                code += "PropertyCollection pixelateProps;" + cr;
                code += cr;
                EffectName = "pixelateEffect";
            }
            else if (EffectCode.Text.Contains("Polar Inversion"))
            {
                code += "// Setup for calling the Polar Inversion effect" + cr;
                code += "PolarInversionEffect polarEffect = new PolarInversionEffect();" + cr;
                code += "PropertyCollection polarProps;" + cr;
                code += cr;
                EffectName = "polarEffect";
            }
            else if (EffectCode.Text.Contains("Tile Reflection"))
            {
                code += "// Setup for calling the Tile Reflection effect" + cr;
                code += "TileEffect tileEffect = new TileEffect();" + cr;
                code += "PropertyCollection tileProps;" + cr;
                code += cr;
                EffectName = "tileEffect";
            }
            else if (EffectCode.Text.Contains("Twist"))
            {
                code += "// Setup for calling the Twist effect" + cr;
                code += "TwistEffect twistEffect = new TwistEffect();" + cr;
                code += "PropertyCollection twistProps;" + cr;
                code += cr;
                EffectName = "twistEffect";
            }
            else if (EffectCode.Text.Contains("Glow"))
            {
                code += "// Setup for calling the Glow effect" + cr;
                code += "GlowEffect glowEffect = new GlowEffect();" + cr;
                code += "PropertyCollection glowProps;" + cr;
                code += cr;
                EffectName = "glowEffect";
            }
            else if (EffectCode.Text.Contains("Soften Portrait"))
            {
                code += "// Setup for calling the Soften Portrait effect" + cr;
                code += "SoftenPortraitEffect portraitEffect = new SoftenPortraitEffect();" + cr;
                code += "PropertyCollection portraitProps;" + cr;
                code += cr;
                EffectName = "portraitEffect";
            }
            else if (EffectCode.Text.Contains("Vignette"))
            {
                code += "// Setup for calling the Vignette effect" + cr;
                code += "VignetteEffect vignetteEffect = new VignetteEffect();" + cr;
                code += "PropertyCollection vignetteProps;" + cr;
                code += cr;
                EffectName = "vignetteEffect";
            }
            else if (EffectCode.Text.Contains("Julia"))
            {
                code += "// Setup for calling the Julia effect" + cr;
                code += "JuliaFractalEffect juliaEffect = new JuliaFractalEffect();" + cr;
                code += "PropertyCollection juliaProps;" + cr;
                code += cr;
                EffectName = "juliaEffect";
            }
            else if (EffectCode.Text.Contains("Mandelbrot"))
            {
                code += "// Setup for calling the Mandelbrot effect" + cr;
                code += "MandelbrotFractalEffect mandelbrotEffect = new MandelbrotFractalEffect();" + cr;
                code += "PropertyCollection mandelbrotProps;" + cr;
                code += cr;
                EffectName = "mandelbrotEffect";
            }

            // setup for selected pixel op
            if (!PixelOpCode.Text.Contains("Pass Through") || !FinalPixelOpCode.Text.Contains("Pass Through"))
            {
                code += "// Setup for using pixel op" + cr;
                if (PixelOpCode.Text == "Desaturate" || FinalPixelOpCode.Text == "Desaturate") code += "private UnaryPixelOps.Desaturate desaturateOp = new UnaryPixelOps.Desaturate();" + cr;
                if (PixelOpCode.Text == "Invert" || FinalPixelOpCode.Text == "Invert")         code += "private UnaryPixelOps.Invert invertOp = new UnaryPixelOps.Invert();" + cr;
                code += cr;
            }

            // setup for selected blending op
            if (!BlendingCode.Text.Contains("Pass Through"))
            {
                if (BlendingCode.Text != "User selected blending mode") code += "// Setup for using " + BlendingCode.Text + " blend op" + cr;
                if (BlendingCode.Text == "Normal") code += "private BinaryPixelOp normalOp = LayerBlendModeUtil.CreateCompositionOp(LayerBlendMode.Normal);" + cr;
                if (BlendingCode.Text == "Multiply") code += "private BinaryPixelOp multiplyOp = LayerBlendModeUtil.CreateCompositionOp(LayerBlendMode.Multiply);" + cr;
                if (BlendingCode.Text == "Darken") code += "private BinaryPixelOp darkenOp = LayerBlendModeUtil.CreateCompositionOp(LayerBlendMode.Darken);" + cr;
                if (BlendingCode.Text == "Additive") code += "private BinaryPixelOp additiveOp = LayerBlendModeUtil.CreateCompositionOp(LayerBlendMode.Additive);" + cr;
                if (BlendingCode.Text == "ColorBurn") code += "private BinaryPixelOp colorburnOp = LayerBlendModeUtil.CreateCompositionOp(LayerBlendMode.ColorBurn);" + cr;
                if (BlendingCode.Text == "ColorDodge") code += "private BinaryPixelOp colordodgeOp = LayerBlendModeUtil.CreateCompositionOp(LayerBlendMode.ColorDodge);" + cr;
                if (BlendingCode.Text == "Difference") code += "private BinaryPixelOp differenceOp = LayerBlendModeUtil.CreateCompositionOp(LayerBlendMode.Difference);" + cr;
                if (BlendingCode.Text == "Glow") code += "private BinaryPixelOp glowOp = LayerBlendModeUtil.CreateCompositionOp(LayerBlendMode.Glow);" + cr;
                if (BlendingCode.Text == "Lighten") code += "private BinaryPixelOp lightenOp = LayerBlendModeUtil.CreateCompositionOp(LayerBlendMode.Lighten);" + cr;
                if (BlendingCode.Text == "Negation") code += "private BinaryPixelOp negationOp = LayerBlendModeUtil.CreateCompositionOp(LayerBlendMode.Negation);" + cr;
                if (BlendingCode.Text == "Overlay") code += "private BinaryPixelOp overlayOp = LayerBlendModeUtil.CreateCompositionOp(LayerBlendMode.Overlay);" + cr;
                if (BlendingCode.Text == "Reflect") code += "private BinaryPixelOp reflectOp = LayerBlendModeUtil.CreateCompositionOp(LayerBlendMode.Reflect);" + cr;
                if (BlendingCode.Text == "Screen") code += "private BinaryPixelOp screenOp = LayerBlendModeUtil.CreateCompositionOp(LayerBlendMode.Screen);" + cr;
                if (BlendingCode.Text == "Xor") code += "private BinaryPixelOp xorOp = LayerBlendModeUtil.CreateCompositionOp(LayerBlendMode.Xor);" + cr;
                code += cr;
            }
            else if (NoStyle.Checked)
            {
                code += "// Setup for using " + BlendingCode.Text + " blend op" + cr;
                if ((BlendingCode.Text == "Normal") || (BlendingCode.Text == "Pass Through")) code += "private BinaryPixelOp normalOp = LayerBlendModeUtil.CreateCompositionOp(LayerBlendMode.Normal);" + cr;
                if (BlendingCode.Text == "Multiply") code += "private BinaryPixelOp multiplyOp = LayerBlendModeUtil.CreateCompositionOp(LayerBlendMode.Multiply);" + cr;
                if (BlendingCode.Text == "Darken") code += "private BinaryPixelOp darkenOp = LayerBlendModeUtil.CreateCompositionOp(LayerBlendMode.Darken);" + cr;
                if (BlendingCode.Text == "Additive") code += "private BinaryPixelOp additiveOp = LayerBlendModeUtil.CreateCompositionOp(LayerBlendMode.Additive);" + cr;
                if (BlendingCode.Text == "ColorBurn") code += "private BinaryPixelOp colorburnOp = LayerBlendModeUtil.CreateCompositionOp(LayerBlendMode.ColorBurn);" + cr;
                if (BlendingCode.Text == "ColorDodge") code += "private BinaryPixelOp colordodgeOp = LayerBlendModeUtil.CreateCompositionOp(LayerBlendMode.ColorDodge);" + cr;
                if (BlendingCode.Text == "Difference") code += "private BinaryPixelOp differenceOp = LayerBlendModeUtil.CreateCompositionOp(LayerBlendMode.Difference);" + cr;
                if (BlendingCode.Text == "Glow") code += "private BinaryPixelOp glowOp = LayerBlendModeUtil.CreateCompositionOp(LayerBlendMode.Glow);" + cr;
                if (BlendingCode.Text == "Lighten") code += "private BinaryPixelOp lightenOp = LayerBlendModeUtil.CreateCompositionOp(LayerBlendMode.Lighten);" + cr;
                if (BlendingCode.Text == "Negation") code += "private BinaryPixelOp negationOp = LayerBlendModeUtil.CreateCompositionOp(LayerBlendMode.Negation);" + cr;
                if (BlendingCode.Text == "Overlay") code += "private BinaryPixelOp overlayOp = LayerBlendModeUtil.CreateCompositionOp(LayerBlendMode.Overlay);" + cr;
                if (BlendingCode.Text == "Reflect") code += "private BinaryPixelOp reflectOp = LayerBlendModeUtil.CreateCompositionOp(LayerBlendMode.Reflect);" + cr;
                if (BlendingCode.Text == "Screen") code += "private BinaryPixelOp screenOp = LayerBlendModeUtil.CreateCompositionOp(LayerBlendMode.Screen);" + cr;
                if (BlendingCode.Text == "Xor") code += "private BinaryPixelOp xorOp = LayerBlendModeUtil.CreateCompositionOp(LayerBlendMode.Xor);" + cr;
                code += cr;
            }

            // setup for using the clipboard
            if (EffectCode.Text.Contains("Clipboard"))
            {
                code += cr;
                code += "// Setup for getting an image from the clipboard" + cr;
                code += "protected Surface img" + cr;
                code += "{" + cr;
                code += "    get" + cr;
                code += "    {" + cr;
                code += "        if (!readClipboard)" + cr;
                code += "        {" + cr;
                code += "            readClipboard = true;" + cr;
                code += "            _img = Services.GetService<IClipboardService>().TryGetSurface();" + cr;
                code += "        }" + cr;
                code += cr;
                code += "        return _img;" + cr;
                code += "    }" + cr;
                code += "}" + cr;
                code += "private Surface _img = null;" + cr;
                code += "private bool readClipboard = false;" + cr;
                code += cr;
            }

            if (SurfaceCode.Checked || EffectName.Length > 0 || EffectCode.Text.Contains("Clipboard"))
            {
                code += "protected override void OnDispose(bool disposing)" + cr;
                code += "{" + cr;
                code += "    if (disposing)" + cr;
                code += "    {" + cr;
                code += "        // Release any surfaces or effects you've created." + cr;
                if (SurfaceCode.Checked)
                {
                    code += "        if (wrk != null) wrk.Dispose();" + cr;
                    code += "        wrk = null;" + cr;
                }
                if (EffectName.Length > 0)
                {
                    code += "        if (" + EffectName + " != null) " + EffectName + ".Dispose();" + cr;
                    code += "        " + EffectName + " = null;" + cr;
                }
                if (EffectCode.Text.Contains("Clipboard"))
                {
                    code += "        if (_img != null) _img.Dispose();" + cr;
                    code += "        _img = null;" + cr;
                }
                code += "    }" + cr;
                code += cr;
                code += "    base.OnDispose(disposing);" + cr;
                code += "}" + cr;
                code += cr;
            }

            // build the PreRender code
            code += "void PreRender(Surface dst, Surface src)" + cr;
            code += "{" + cr;
            if (SurfaceCode.Checked)
            {
                code += "    if (wrk == null)" + cr;
                code += "    {" + cr;
                code += "        wrk = new Surface(src.Size);" + cr;
                code += "    }" + cr;
                code += cr;
            }

            if (EffectCode.Text.Contains("Gaussian Blur"))
            {
                code += "    blurProps = blurEffect.CreatePropertyCollection();" + cr;
                code += "    PropertyBasedEffectConfigToken BlurParameters = new PropertyBasedEffectConfigToken(blurProps);" + cr;
                code += "    BlurParameters.SetPropertyValue(GaussianBlurEffect.PropertyNames.Radius, Amount1);" + cr;
                code += "    blurEffect.SetRenderInfo(BlurParameters, new RenderArgs(" + wrksurface + "), new RenderArgs(src));" + cr;
            }
            else if (EffectCode.Text.Contains("Contrast"))
            {
                code += "    bacProps = bacAdjustment.CreatePropertyCollection();" + cr;
                code += "    PropertyBasedEffectConfigToken bacParameters = new PropertyBasedEffectConfigToken(bacProps);" + cr;
                code += "    bacParameters.SetPropertyValue(BrightnessAndContrastAdjustment.PropertyNames.Brightness, Amount1);" + cr;
                code += "    bacParameters.SetPropertyValue(BrightnessAndContrastAdjustment.PropertyNames.Contrast, Amount2);" + cr;
                code += "    bacAdjustment.SetRenderInfo(bacParameters, new RenderArgs(" + wrksurface + "), new RenderArgs(src));" + cr;
            }
            else if (EffectCode.Text.Contains("Frosted"))
            {
                code += "    frostedProps = frostedEffect.CreatePropertyCollection();" + cr;
                code += "    PropertyBasedEffectConfigToken FrostedParameters = new PropertyBasedEffectConfigToken(frostedProps);" + cr;
                code += "    FrostedParameters.SetPropertyValue(FrostedGlassEffect.PropertyNames.MaxScatterRadius, Amount1);" + cr;
                code += "    FrostedParameters.SetPropertyValue(FrostedGlassEffect.PropertyNames.MinScatterRadius, Amount2);" + cr;
                code += "    FrostedParameters.SetPropertyValue(FrostedGlassEffect.PropertyNames.NumSamples, Amount3);" + cr;
                code += "    frostedEffect.SetRenderInfo(FrostedParameters, new RenderArgs(" + wrksurface + "), new RenderArgs(src));" + cr;
            }
            else if (EffectCode.Text.Contains("Add Noise"))
            {
                code += "    noiseProps = noiseEffect.CreatePropertyCollection();" + cr;
                code += "    PropertyBasedEffectConfigToken NoiseParameters = new PropertyBasedEffectConfigToken(noiseProps);" + cr;
                code += "    NoiseParameters.SetPropertyValue(AddNoiseEffect.PropertyNames.Intensity, Amount1);" + cr;
                code += "    NoiseParameters.SetPropertyValue(AddNoiseEffect.PropertyNames.Saturation, Amount2);" + cr;
                code += "    NoiseParameters.SetPropertyValue(AddNoiseEffect.PropertyNames.Coverage, Amount3);" + cr;
                code += "    noiseEffect.SetRenderInfo(NoiseParameters, new RenderArgs(" + wrksurface + "), new RenderArgs(src));" + cr;
            }
            else if (EffectCode.Text.Contains("Motion Blur"))
            {
                code += "    blurProps = blurEffect.CreatePropertyCollection();" + cr;
                code += "    PropertyBasedEffectConfigToken BlurParameters = new PropertyBasedEffectConfigToken(blurProps);" + cr;
                code += "    BlurParameters.SetPropertyValue(MotionBlurEffect.PropertyNames.Angle, Amount1);" + cr;
                code += "    BlurParameters.SetPropertyValue(MotionBlurEffect.PropertyNames.Centered, Amount2);" + cr;
                code += "    BlurParameters.SetPropertyValue(MotionBlurEffect.PropertyNames.Distance, Amount3);" + cr;
                code += "    blurEffect.SetRenderInfo(BlurParameters, new RenderArgs(" + wrksurface + "), new RenderArgs(src));" + cr;
            }
            else if (EffectCode.Text.Contains("Clouds"))
            {
                code += "    cloudsProps = cloudsEffect.CreatePropertyCollection();" + cr;
                code += "    PropertyBasedEffectConfigToken CloudsParameters = new PropertyBasedEffectConfigToken(cloudsProps);" + cr;
                code += "    CloudsParameters.SetPropertyValue(CloudsEffect.PropertyNames.Scale, Amount1);" + cr;
                code += "    CloudsParameters.SetPropertyValue(CloudsEffect.PropertyNames.Power, Amount2);" + cr;
                code += "    CloudsParameters.SetPropertyValue(CloudsEffect.PropertyNames.BlendMode, Amount3);" + cr;
                code += "    CloudsParameters.SetPropertyValue(CloudsEffect.PropertyNames.Seed, (int)Amount4);" + cr;
                code += "    cloudsEffect.SetRenderInfo(CloudsParameters, new RenderArgs(" + wrksurface + "), new RenderArgs(src));" + cr;
            }
            else if (EffectCode.Text.Contains("Oil Painting"))
            {
                code += "    oilpaintProps = oilpaintEffect.CreatePropertyCollection();" + cr;
                code += "    PropertyBasedEffectConfigToken oilpaintParameters = new PropertyBasedEffectConfigToken(oilpaintProps);" + cr;
                code += "    oilpaintParameters.SetPropertyValue(OilPaintingEffect.PropertyNames.BrushSize, Amount1);" + cr;
                code += "    oilpaintParameters.SetPropertyValue(OilPaintingEffect.PropertyNames.Coarseness, Amount2);" + cr;
                code += "    oilpaintEffect.SetRenderInfo(oilpaintParameters, new RenderArgs(" + wrksurface + "), new RenderArgs(src));" + cr;
            }
            else if (EffectCode.Text.Contains("Reduce Noise"))
            {
                code += "    reducenoiseProps = reducenoiseEffect.CreatePropertyCollection();" + cr;
                code += "    PropertyBasedEffectConfigToken reducenoiseParameters = new PropertyBasedEffectConfigToken(reducenoiseProps);" + cr;
                code += "    reducenoiseParameters.SetPropertyValue(ReduceNoiseEffect.PropertyNames.Radius, Amount1);" + cr;
                code += "    reducenoiseParameters.SetPropertyValue(ReduceNoiseEffect.PropertyNames.Strength, Amount2);" + cr;
                code += "    reducenoiseEffect.SetRenderInfo(reducenoiseParameters, new RenderArgs(" + wrksurface + "), new RenderArgs(src));" + cr;
            }
            else if (EffectCode.Text.Contains("Median"))
            {
                code += "    medianProps = medianEffect.CreatePropertyCollection();" + cr;
                code += "    PropertyBasedEffectConfigToken medianParameters = new PropertyBasedEffectConfigToken(medianProps);" + cr;
                code += "    medianParameters.SetPropertyValue(MedianEffect.PropertyNames.Radius, Amount1);" + cr;
                code += "    medianParameters.SetPropertyValue(MedianEffect.PropertyNames.Percentile, Amount2);" + cr;
                code += "    medianEffect.SetRenderInfo(medianParameters, new RenderArgs(" + wrksurface + "), new RenderArgs(src));" + cr;
            }
            else if (EffectCode.Text.Contains("Edge Detect"))
            {
                code += "    edgeProps = edgedetectEffect.CreatePropertyCollection();" + cr;
                code += "    PropertyBasedEffectConfigToken EdgeParameters = new PropertyBasedEffectConfigToken(edgeProps);" + cr;
                code += "    EdgeParameters.SetPropertyValue(EdgeDetectEffect.PropertyNames.Angle, Amount1);" + cr;
                code += "    edgedetectEffect.SetRenderInfo(EdgeParameters, new RenderArgs(" + wrksurface + "), new RenderArgs(src));" + cr;
            }
            else if (EffectCode.Text.Contains("Emboss"))
            {
                code += "    embossProps = embossEffect.CreatePropertyCollection();" + cr;
                code += "    PropertyBasedEffectConfigToken EmbossParameters = new PropertyBasedEffectConfigToken(embossProps);" + cr;
                code += "    EmbossParameters.SetPropertyValue(EmbossEffect.PropertyNames.Angle, Amount1);" + cr;
                code += "    embossEffect.SetRenderInfo(EmbossParameters, new RenderArgs(" + wrksurface + "), new RenderArgs(src));" + cr;
            }
            else if (EffectCode.Text.Contains("Relief"))
            {
                code += "    reliefProps = reliefEffect.CreatePropertyCollection();" + cr;
                code += "    PropertyBasedEffectConfigToken ReliefParameters = new PropertyBasedEffectConfigToken(reliefProps);" + cr;
                code += "    ReliefParameters.SetPropertyValue(ReliefEffect.PropertyNames.Angle, Amount1);" + cr;
                code += "    reliefEffect.SetRenderInfo(ReliefParameters, new RenderArgs(" + wrksurface + "), new RenderArgs(src));" + cr;
            }
            else if (EffectCode.Text.Contains("Posterize"))
            {
                code += "    posterizeProps = posterizeEffect.CreatePropertyCollection();" + cr;
                code += "    PropertyBasedEffectConfigToken PosterizeParameters = new PropertyBasedEffectConfigToken(posterizeProps);" + cr;
                code += "    PosterizeParameters.SetPropertyValue(PosterizeAdjustment.PropertyNames.RedLevels, Amount1);" + cr;
                code += "    PosterizeParameters.SetPropertyValue(PosterizeAdjustment.PropertyNames.GreenLevels, Amount2);" + cr;
                code += "    PosterizeParameters.SetPropertyValue(PosterizeAdjustment.PropertyNames.BlueLevels, Amount3);" + cr;
                code += "    PosterizeParameters.SetPropertyValue(PosterizeAdjustment.PropertyNames.LinkLevels, Amount4);" + cr;
                code += "    posterizeEffect.SetRenderInfo(PosterizeParameters, new RenderArgs(" + wrksurface + "), new RenderArgs(src));" + cr;
            }
            else if (EffectCode.Text.Contains("Outline"))
            {
                code += "    outlineProps = outlineEffect.CreatePropertyCollection();" + cr;
                code += "    PropertyBasedEffectConfigToken outlineParameters = new PropertyBasedEffectConfigToken(outlineProps);" + cr;
                code += "    outlineParameters.SetPropertyValue(OutlineEffect.PropertyNames.Thickness, Amount1);" + cr;
                code += "    outlineParameters.SetPropertyValue(OutlineEffect.PropertyNames.Intensity, Amount2);" + cr;
                code += "    outlineEffect.SetRenderInfo(outlineParameters, new RenderArgs(" + wrksurface + "), new RenderArgs(src));" + cr;
            }
            else if (EffectCode.Text.Contains("Sharpen"))
            {
                code += "    sharpenProps = sharpenEffect.CreatePropertyCollection();" + cr;
                code += "    PropertyBasedEffectConfigToken SharpenParameters = new PropertyBasedEffectConfigToken(sharpenProps);" + cr;
                code += "    SharpenParameters.SetPropertyValue(SharpenEffect.PropertyNames.Amount, Amount1);" + cr;
                code += "    sharpenEffect.SetRenderInfo(SharpenParameters, new RenderArgs(" + wrksurface + "), new RenderArgs(src));" + cr;
            }
            else if (EffectCode.Text.Contains("Pencil Sketch"))
            {
                code += "    pencilSketchProps = pencilSketchEffect.CreatePropertyCollection();" + cr;
                code += "    PropertyBasedEffectConfigToken pencilSketchParameters = new PropertyBasedEffectConfigToken(pencilSketchProps);" + cr;
                code += "    pencilSketchParameters.SetPropertyValue(PencilSketchEffect.PropertyNames.PencilTipSize, Amount1);" + cr;
                code += "    pencilSketchParameters.SetPropertyValue(PencilSketchEffect.PropertyNames.ColorRange, Amount2);" + cr;
                code += "    pencilSketchEffect.SetRenderInfo(pencilSketchParameters, new RenderArgs(" + wrksurface + "), new RenderArgs(src));" + cr;
            }
            else if (EffectCode.Text.Contains("Sepia"))
            {
                code += "    sepiaProps = sepiaEffect.CreatePropertyCollection();" + cr;
                code += "    PropertyBasedEffectConfigToken SepiaParameters = new PropertyBasedEffectConfigToken(sepiaProps);" + cr;
                code += "    sepiaEffect.SetRenderInfo(SepiaParameters, new RenderArgs(" + wrksurface + "), new RenderArgs(src));" + cr;
            }
            else if (EffectCode.Text.Contains("Ink Sketch"))
            {
                code += "    inkProps = inkEffect.CreatePropertyCollection();" + cr;
                code += "    PropertyBasedEffectConfigToken InkParameters = new PropertyBasedEffectConfigToken(inkProps);" + cr;
                code += "    InkParameters.SetPropertyValue(InkSketchEffect.PropertyNames.InkOutline, Amount1);" + cr;
                code += "    InkParameters.SetPropertyValue(InkSketchEffect.PropertyNames.Coloring, Amount2);" + cr;
                code += "    inkEffect.SetRenderInfo(InkParameters, new RenderArgs(" + wrksurface + "), new RenderArgs(src));" + cr;
            }
            else if (EffectCode.Text.Contains("Radial Blur"))
            {
                code += "    radialProps = radialEffect.CreatePropertyCollection();" + cr;
                code += "    PropertyBasedEffectConfigToken RadialParameters = new PropertyBasedEffectConfigToken(radialProps);" + cr;
                code += "    RadialParameters.SetPropertyValue(RadialBlurEffect.PropertyNames.Angle, Amount1);" + cr;
                code += "    RadialParameters.SetPropertyValue(RadialBlurEffect.PropertyNames.Offset, Amount2);" + cr;
                code += "    RadialParameters.SetPropertyValue(RadialBlurEffect.PropertyNames.Quality, Amount3);" + cr;
                code += "    radialEffect.SetRenderInfo(RadialParameters, new RenderArgs(" + wrksurface + "), new RenderArgs(src));" + cr;
            }
            else if (EffectCode.Text.Contains("Surface Blur"))
            {
                code += "    surfaceProps = surfaceEffect.CreatePropertyCollection();" + cr;
                code += "    PropertyBasedEffectConfigToken SurfaceParameters = new PropertyBasedEffectConfigToken(surfaceProps);" + cr;
                code += "    SurfaceParameters.SetPropertyValue(SurfaceBlurEffect.PropertyName.Radius, Amount1);" + cr;
                code += "    SurfaceParameters.SetPropertyValue(SurfaceBlurEffect.PropertyName.Threshold, Amount2);" + cr;
                code += "    surfaceEffect.SetRenderInfo(SurfaceParameters, new RenderArgs(" + wrksurface + "), new RenderArgs(src));" + cr;
            }
            else if (EffectCode.Text.Contains("Unfocus"))
            {
                code += "    unfocusProps = unfocusEffect.CreatePropertyCollection();" + cr;
                code += "    PropertyBasedEffectConfigToken UnfocusParameters = new PropertyBasedEffectConfigToken(unfocusProps);" + cr;
                code += "    UnfocusParameters.SetPropertyValue(UnfocusEffect.PropertyNames.Radius, Amount1);" + cr;
                code += "    unfocusEffect.SetRenderInfo(UnfocusParameters, new RenderArgs(" + wrksurface + "), new RenderArgs(src));" + cr;
            }
            else if (EffectCode.Text.Contains("Zoom Blur"))
            {
                code += "    zoomProps = zoomEffect.CreatePropertyCollection();" + cr;
                code += "    PropertyBasedEffectConfigToken ZoomParameters = new PropertyBasedEffectConfigToken(zoomProps);" + cr;
                code += "    ZoomParameters.SetPropertyValue(ZoomBlurEffect.PropertyNames.Amount, Amount1);" + cr;
                code += "    ZoomParameters.SetPropertyValue(ZoomBlurEffect.PropertyNames.Offset, Amount2);" + cr;
                code += "    zoomEffect.SetRenderInfo(ZoomParameters, new RenderArgs(" + wrksurface + "), new RenderArgs(src));" + cr;
            }
            else if (EffectCode.Text.Contains("Bulge"))
            {
                code += "    bulgeProps = bulgeEffect.CreatePropertyCollection();" + cr;
                code += "    PropertyBasedEffectConfigToken BulgeParameters = new PropertyBasedEffectConfigToken(bulgeProps);" + cr;
                code += "    BulgeParameters.SetPropertyValue(BulgeEffect.PropertyNames.Amount, Amount1);" + cr;
                code += "    BulgeParameters.SetPropertyValue(BulgeEffect.PropertyNames.Offset, Amount2);" + cr;
                code += "    bulgeEffect.SetRenderInfo(BulgeParameters, new RenderArgs(" + wrksurface + "), new RenderArgs(src));" + cr;
            }
            else if (EffectCode.Text.Contains("Crystalize"))
            {
                code += "    crystalizeProps = crystalizeEffect.CreatePropertyCollection();" + cr;
                code += "    PropertyBasedEffectConfigToken CrystalizeParameters = new PropertyBasedEffectConfigToken(crystalizeProps);" + cr;
                code += "    CrystalizeParameters.SetPropertyValue(CrystalizeEffect.PropertyNames.Size, Amount1);" + cr;
                code += "    CrystalizeParameters.SetPropertyValue(CrystalizeEffect.PropertyNames.Quality, Amount2);" + cr;
                code += "    CrystalizeParameters.SetPropertyValue(CrystalizeEffect.PropertyNames.Seed, (int)Amount3);" + cr;
                code += "    crystalizeEffect.SetRenderInfo(CrystalizeParameters, new RenderArgs(" + wrksurface + "), new RenderArgs(src));" + cr;
            }
            else if (EffectCode.Text.Contains("Dents"))
            {
                code += "    dentsProps = dentsEffect.CreatePropertyCollection();" + cr;
                code += "    PropertyBasedEffectConfigToken DentsParameters = new PropertyBasedEffectConfigToken(dentsProps);" + cr;
                code += "    DentsParameters.SetPropertyValue(DentsEffect.PropertyNames.Scale, Amount1);" + cr;
                code += "    DentsParameters.SetPropertyValue(DentsEffect.PropertyNames.Refraction, Amount2);" + cr;
                code += "    DentsParameters.SetPropertyValue(DentsEffect.PropertyNames.Roughness, Amount3);" + cr;
                code += "    DentsParameters.SetPropertyValue(DentsEffect.PropertyNames.Tension, Amount4);" + cr;
                code += "    DentsParameters.SetPropertyValue(DentsEffect.PropertyNames.Quality, Amount5);" + cr;
                code += "    DentsParameters.SetPropertyValue(DentsEffect.PropertyNames.Seed, (int)Amount6);" + cr;
                code += "    dentsEffect.SetRenderInfo(DentsParameters, new RenderArgs(" + wrksurface + "), new RenderArgs(src));" + cr;
            }
            else if (EffectCode.Text.Contains("Pixelate"))
            {
                code += "    pixelateProps = pixelateEffect.CreatePropertyCollection();" + cr;
                code += "    PropertyBasedEffectConfigToken PixelateParameters = new PropertyBasedEffectConfigToken(pixelateProps);" + cr;
                code += "    PixelateParameters.SetPropertyValue(PixelateEffect.PropertyNames.CellSize, Amount1);" + cr;
                code += "    pixelateEffect.SetRenderInfo(PixelateParameters, new RenderArgs(" + wrksurface + "), new RenderArgs(src));" + cr;
            }
            else if (EffectCode.Text.Contains("Polar Inversion"))
            {
                code += "    polarProps = polarEffect.CreatePropertyCollection();" + cr;
                code += "    PropertyBasedEffectConfigToken PolarParameters = new PropertyBasedEffectConfigToken(polarProps);" + cr;
                code += "    PolarParameters.SetPropertyValue(PolarInversionEffect.PropertyNames.Amount, Amount1);" + cr;
                code += "    PolarParameters.SetPropertyValue(PolarInversionEffect.PropertyNames.Offset, Amount2);" + cr;
                code += "    PolarParameters.SetPropertyValue(PolarInversionEffect.PropertyNames.EdgeBehavior, Amount3);" + cr;
                code += "    PolarParameters.SetPropertyValue(PolarInversionEffect.PropertyNames.Quality, Amount4);" + cr;
                code += "    polarEffect.SetRenderInfo(PolarParameters, new RenderArgs(" + wrksurface + "), new RenderArgs(src));" + cr;
            }
            else if (EffectCode.Text.Contains("Tile Reflection"))
            {
                code += "    tileProps = tileEffect.CreatePropertyCollection();" + cr;
                code += "    PropertyBasedEffectConfigToken TileParameters = new PropertyBasedEffectConfigToken(tileProps);" + cr;
                code += "    TileParameters.SetPropertyValue(TileEffect.PropertyNames.Rotation, Amount1);" + cr;
                code += "    TileParameters.SetPropertyValue(TileEffect.PropertyNames.SquareSize, Amount2);" + cr;
                code += "    TileParameters.SetPropertyValue(TileEffect.PropertyNames.Curvature, Amount3);" + cr;
                code += "    TileParameters.SetPropertyValue(TileEffect.PropertyNames.Quality, Amount4);" + cr;
                code += "    tileEffect.SetRenderInfo(TileParameters, new RenderArgs(" + wrksurface + "), new RenderArgs(src));" + cr;
            }
            else if (EffectCode.Text.Contains("Twist"))
            {
                code += "    twistProps = twistEffect.CreatePropertyCollection();" + cr;
                code += "    PropertyBasedEffectConfigToken TwistParameters = new PropertyBasedEffectConfigToken(twistProps);" + cr;
                code += "    TwistParameters.SetPropertyValue(TwistEffect.PropertyNames.Amount, Amount1);" + cr;
                code += "    TwistParameters.SetPropertyValue(TwistEffect.PropertyNames.Size, Amount2);" + cr;
                code += "    TwistParameters.SetPropertyValue(TwistEffect.PropertyNames.Offset, Amount3);" + cr;
                code += "    TwistParameters.SetPropertyValue(TwistEffect.PropertyNames.Quality, Amount4);" + cr;
                code += "    twistEffect.SetRenderInfo(TwistParameters, new RenderArgs(" + wrksurface + "), new RenderArgs(src));" + cr;
            }
            else if (EffectCode.Text.Contains("Glow"))
            {
                code += "    glowProps = glowEffect.CreatePropertyCollection();" + cr;
                code += "    PropertyBasedEffectConfigToken GlowParameters = new PropertyBasedEffectConfigToken(glowProps);" + cr;
                code += "    GlowParameters.SetPropertyValue(GlowEffect.PropertyNames.Radius, Amount1);" + cr;
                code += "    GlowParameters.SetPropertyValue(GlowEffect.PropertyNames.Brightness, Amount2);" + cr;
                code += "    GlowParameters.SetPropertyValue(GlowEffect.PropertyNames.Contrast, Amount3);" + cr;
                code += "    glowEffect.SetRenderInfo(GlowParameters, new RenderArgs(" + wrksurface + "), new RenderArgs(src));" + cr;
            }
            else if (EffectCode.Text.Contains("Soften Portrait"))
            {
                code += "    portraitProps = portraitEffect.CreatePropertyCollection();" + cr;
                code += "    PropertyBasedEffectConfigToken PortraitParameters = new PropertyBasedEffectConfigToken(portraitProps);" + cr;
                code += "    PortraitParameters.SetPropertyValue(SoftenPortraitEffect.PropertyNames.Softness, Amount1);" + cr;
                code += "    PortraitParameters.SetPropertyValue(SoftenPortraitEffect.PropertyNames.Lighting, Amount2);" + cr;
                code += "    PortraitParameters.SetPropertyValue(SoftenPortraitEffect.PropertyNames.Warmth, Amount3);" + cr;
                code += "    softenEffect.SetRenderInfo(PortraitParameters, new RenderArgs(" + wrksurface + "), new RenderArgs(src));" + cr;
            }
            else if (EffectCode.Text.Contains("Vignette"))
            {
                code += "    vignetteProps = vignetteEffect.CreatePropertyCollection();" + cr;
                code += "    PropertyBasedEffectConfigToken VignetteParameters = new PropertyBasedEffectConfigToken(vignetteProps);" + cr;
                code += "    VignetteParameters.SetPropertyValue(VignetteEffect.PropertyNames.Offset, Amount1);" + cr;
                code += "    VignetteParameters.SetPropertyValue(VignetteEffect.PropertyNames.Radius, Amount2);" + cr;
                code += "    VignetteParameters.SetPropertyValue(VignetteEffect.PropertyNames.Amount, Amount3);" + cr;
                code += "    vignetteEffect.SetRenderInfo(VignetteParameters, new RenderArgs(" + wrksurface + "), new RenderArgs(src));" + cr;
            }
            else if (EffectCode.Text.Contains("Julia"))
            {
                code += "    juliaProps = juliaEffect.CreatePropertyCollection();" + cr;
                code += "    PropertyBasedEffectConfigToken JuliaParameters = new PropertyBasedEffectConfigToken(juliaProps);" + cr;
                code += "    JuliaParameters.SetPropertyValue(JuliaFractalEffect.PropertyNames.Factor, Amount1);" + cr;
                code += "    JuliaParameters.SetPropertyValue(JuliaFractalEffect.PropertyNames.Zoom, Amount2);" + cr;
                code += "    JuliaParameters.SetPropertyValue(JuliaFractalEffect.PropertyNames.Angle, Amount3);" + cr;
                code += "    JuliaParameters.SetPropertyValue(JuliaFractalEffect.PropertyNames.Quality, Amount4);" + cr;
                code += "    juliaEffect.SetRenderInfo(JuliaParameters, new RenderArgs(" + wrksurface + "), new RenderArgs(src));" + cr;
            }
            else if (EffectCode.Text.Contains("Mandelbrot"))
            {
                code += "    mandelbrotProps = mandelbrotEffect.CreatePropertyCollection();" + cr;
                code += "    PropertyBasedEffectConfigToken MandelbrotParameters = new PropertyBasedEffectConfigToken(mandelbrotProps);" + cr;
                code += "    MandelbrotParameters.SetPropertyValue(MandelbrotFractalEffect.PropertyNames.Factor, Amount1);" + cr;
                code += "    MandelbrotParameters.SetPropertyValue(MandelbrotFractalEffect.PropertyNames.Zoom, Amount2);" + cr;
                code += "    MandelbrotParameters.SetPropertyValue(MandelbrotFractalEffect.PropertyNames.Angle, Amount3);" + cr;
                code += "    MandelbrotParameters.SetPropertyValue(MandelbrotFractalEffect.PropertyNames.Quality, Amount4);" + cr;
                code += "    MandelbrotParameters.SetPropertyValue(MandelbrotFractalEffect.PropertyNames.InvertColors, Amount5);" + cr;
                code += "    mandelbrotEffect.SetRenderInfo(MandelbrotParameters, new RenderArgs(" + wrksurface + "), new RenderArgs(src));" + cr;
            }

            if (EffectCode.Text.Contains("Empty") || NoStyle.Checked)
            {
                code += "    " + wrksurface + ".Clear(ColorBgra.Transparent);" + cr;
            }
            if (EffectCode.Text.Contains("Copy") && wrksurface != "dst")
            {
                code += "    " + wrksurface + ".CopySurface(src);" + cr;
            }

            // finish PreRender
            code += "}" + cr + cr;
            code += "// Here is the main render loop function" + cr;
            // if we're using pointers, the render function must be marked 'unsafe'
            if (AdvancedStyle.Checked)
            {
                code += "unsafe ";
            }
            code += "void Render(Surface dst, Surface src, Rectangle rect)" + cr;
            code += "{" + cr;

            // Add in code for the desired variables the user selected
            if (CenterCode.Checked || SelectionCode.Checked)
            {
                code += "    Rectangle selection = EnvironmentParameters.GetSelection(src.Bounds).GetBoundsInt();" + cr;
                if (SelectionCode.Checked)
                {
                    code += "    PdnRegion selectionRegion = EnvironmentParameters.GetSelection(src.Bounds);" + cr;
                }
                code += cr;
            }
            if (CenterCode.Checked)
            {
                code += "    // Delete these 2 lines if you don't need to know the center point of the current selection" + cr;
                code += "    int CenterX = ((selection.Right - selection.Left) / 2) + selection.Left;" + cr;
                code += "    int CenterY = ((selection.Bottom - selection.Top) / 2) + selection.Top;" + cr;
                code += cr;
            }
            if (PrimaryColorCode.Checked)
            {
                code += "    // Delete these lines if you don't need the primary or secondary color" + cr;
                code += "    ColorBgra PrimaryColor = (ColorBgra)EnvironmentParameters.PrimaryColor;" + cr;
                code += "    ColorBgra SecondaryColor = (ColorBgra)EnvironmentParameters.SecondaryColor;" + cr;
                code += cr;
            }
            if (PaletteCode.Checked)
            {
                code += "    // Delete these lines if you don't need the current or default palette" + cr;
                code += "    IReadOnlyList<ColorBgra> DefaultColors = PaintDotNet.ServiceProviderExtensions.GetService<IPalettesService>(Services).DefaultPalette;" + cr;
                code += "    IReadOnlyList<ColorBgra> CurrentColors = PaintDotNet.ServiceProviderExtensions.GetService<IPalettesService>(Services).CurrentPalette; " + cr;
                code += cr;
            }
            if (PenWidthCode.Checked)
            {
                code += "    // Delete the next line if you don't need the brush width" + cr;
                code += "    int BrushWidth = (int)EnvironmentParameters.BrushWidth;" + cr;
                code += cr;
            }

            // Now, call the actual function if this is a complex effect
            if (EffectCode.Text.Contains("Copy") && !BlendingCode.Text.Contains("Pass Through"))
            {
                code += "    // Call the copy function" + cr;
                code += "    " + wrksurface + ".CopySurface(src,rect.Location,rect);" + cr;
                code += cr;
                code += "    // Now in the main render loop, the " + wrksurface + " canvas has a copy of the src canvas" + cr;
            }

            if (EffectCode.Text.Contains("Gaussian Blur"))
            {
                code += "    // Call the Gaussian Blur function" + cr;
                code += "    blurEffect.Render(new Rectangle[1] {rect},0,1);" + cr;
                code += cr;
                code += "    // Now in the main render loop, the " + wrksurface + " canvas has a blurred version of the src canvas" + cr;
            }
            else if (EffectCode.Text.Contains("Contrast"))
            {
                code += "    // Call the Brightness and Contrast Adjustment function" + cr;
                code += "    bacAdjustment.Render(new Rectangle[1] {rect},0,1);" + cr;
                code += cr;
                code += "    // Now in the main render loop, the " + wrksurface + " canvas has an adjusted version of the src canvas" + cr;
            }
            else if (EffectCode.Text.Contains("Frosted"))
            {
                code += "    // Call the Frosted Glass function" + cr;
                code += "    frostedEffect.Render(new Rectangle[1] { rect }, 0, 1);" + cr;
                code += cr;
                code += "    // Now in the main render loop, the " + wrksurface + " canvas has a frosted version of the src canvas" + cr;
            }
            else if (EffectCode.Text.Contains("Add Noise"))
            {
                code += "    // Call the Add Noise function" + cr;
                code += "    noiseEffect.Render(new Rectangle[1] { rect }, 0, 1);" + cr;
                code += cr;
                code += "    // Now in the main render loop, the " + wrksurface + " canvas has a noisy version of the src canvas" + cr;
            }
            else if (EffectCode.Text.Contains("Motion Blur"))
            {
                code += "    // Call the Motion Blur function" + cr;
                code += "    blurEffect.Render(new Rectangle[1] {rect},0,1);" + cr;
                code += cr;
                code += "    // Now in the main render loop, the " + wrksurface + " canvas has a blurred version of the src canvas" + cr;
            }
            else if (EffectCode.Text.Contains("Clouds"))
            {
                code += "    // Call the Clouds function using Black and White" + cr;
                code += "    cloudsEffect.Render(new Rectangle[1] {rect},0,1);" + cr;
                code += cr;
                code += "    // Now in the main render loop, the " + wrksurface + " canvas has a render of clouds" + cr;
            }
            else if (EffectCode.Text.Contains("Oil Painting"))
            {
                code += "    // Call the Oil Painting function" + cr;
                code += "    oilpaintEffect.Render(new Rectangle[1] {rect},0,1);" + cr;
                code += cr;
                code += "    // Now in the main render loop, the " + wrksurface + " canvas has an oil painted version of the src canvas" + cr;
            }
            else if (EffectCode.Text.Contains("Reduce Noise"))
            {
                code += "    // Call the Reduce Noise function" + cr;
                code += "    reducenoiseEffect.Render(new Rectangle[1] {rect},0,1);" + cr;
                code += cr;
                code += "    // Now in the main render loop, the " + wrksurface + " canvas has a less noisy version of the src canvas" + cr;
            }
            else if (EffectCode.Text.Contains("Median"))
            {
                code += "    // Call the Median function" + cr;
                code += "    medianEffect.Render(new Rectangle[1] {rect},0,1);" + cr;
                code += cr;
                code += "    // Now in the main render loop, the " + wrksurface + " canvas has a median version of the src canvas" + cr;
            }
            else if (EffectCode.Text.Contains("Edge Detect"))
            {
                code += "    // Call the Edge Detect function" + cr;
                code += "    edgedetectEffect.Render(new Rectangle[1] {rect},0,1);" + cr;
                code += cr;
                code += "    // Now in the main render loop, the " + wrksurface + " canvas has an edge detect version of the src canvas" + cr;
            }
            else if (EffectCode.Text.Contains("Emboss"))
            {
                code += "    // Call the Emboss function" + cr;
                code += "    embossEffect.Render(new Rectangle[1] {rect},0,1);" + cr;
                code += cr;
                code += "    // Now in the main render loop, the " + wrksurface + " canvas has an embossed version of the src canvas" + cr;
            }
            else if (EffectCode.Text.Contains("Relief"))
            {
                code += "    // Call the Relief function" + cr;
                code += "    reliefEffect.Render(new Rectangle[1] {rect},0,1);" + cr;
                code += cr;
                code += "    // Now in the main render loop, the " + wrksurface + " canvas has a relief version of the src canvas" + cr;
            }
            else if (EffectCode.Text.Contains("Posterize"))
            {
                code += "    // Call the Posterize function" + cr;
                code += "    posterizeEffect.Render(new Rectangle[1] {rect},0,1);" + cr;
                code += cr;
                code += "    // Now in the main render loop, the " + wrksurface + " canvas has a posterized version of the src canvas" + cr;
            }
            else if (EffectCode.Text.Contains("Outline"))
            {
                code += "    // Call the Outline function" + cr;
                code += "    outlineEffect.Render(new Rectangle[1] { rect }, 0, 1);" + cr;
                code += cr;
                code += "    // Now in the main render loop, the " + wrksurface + " canvas has an outlined version of the src canvas" + cr;
            }
            else if (EffectCode.Text.Contains("Sharpen"))
            {
                code += "    // Call the Sharpen function" + cr;
                code += "    sharpenEffect.Render(new Rectangle[1] { rect }, 0, 1);" + cr;
                code += cr;
                code += "    // Now in the main render loop, the " + wrksurface + " canvas has a sharpened version of the src canvas" + cr;
            }
            else if (EffectCode.Text.Contains("Pencil Sketch"))
            {
                code += "    // Call the Pencil Sketch function" + cr;
                code += "    pencilSketchEffect.Render(new Rectangle[1] { rect }, 0, 1);" + cr;
                code += cr;
                code += "    // Now in the main render loop, the " + wrksurface + " canvas has an sketched version of the src canvas" + cr;
            }
            else if (EffectCode.Text.Contains("Sepia"))
            {
                code += "    // Call the Sepia function" + cr;
                code += "    sepiaEffect.Render(new Rectangle[1] {rect},0,1);" + cr;
                code += cr;
                code += "    // Now in the main render loop, the " + wrksurface + " canvas has a sepia version of the src canvas" + cr;
            }
            else if (EffectCode.Text.Contains("Bulge"))
            {
                code += "    // Call the Bulge function" + cr;
                code += "    bulgeEffect.Render(new Rectangle[1] {rect},0,1);" + cr;
                code += cr;
                code += "    // Now in the main render loop, the " + wrksurface + " canvas is Bulged" + cr;
            }
            else if (EffectCode.Text.Contains("Crystalize"))
            {
                code += "    // Call the Crystalize function" + cr;
                code += "    crystalizeEffect.Render(new Rectangle[1] {rect},0,1);" + cr;
                code += cr;
                code += "    // Now in the main render loop, the " + wrksurface + " canvas is Crystalized" + cr;
            }
            else if (EffectCode.Text.Contains("Dents"))
            {
                code += "    // Call the Dents function" + cr;
                code += "    dentsEffect.Render(new Rectangle[1] {rect},0,1);" + cr;
                code += cr;
                code += "    // Now in the main render loop, the " + wrksurface + " canvas has Dents" + cr;
            }
            else if (EffectCode.Text.Contains("Glow"))
            {
                code += "    // Call the Glow function" + cr;
                code += "    glowEffect.Render(new Rectangle[1] {rect},0,1);" + cr;
                code += cr;
                code += "    // Now in the main render loop, the " + wrksurface + " canvas has a Glow" + cr;
            }
            else if (EffectCode.Text.Contains("Ink Sketch"))
            {
                code += "    // Call the Ink Sketch function" + cr;
                code += "    inkEffect.Render(new Rectangle[1] {rect},0,1);" + cr;
                code += cr;
                code += "    // Now in the main render loop, the " + wrksurface + " canvas has an Ink Sketch" + cr;
            }
            else if (EffectCode.Text.Contains("Julia"))
            {
                code += "    // Call the Julia function" + cr;
                code += "    juliaEffect.Render(new Rectangle[1] {rect},0,1);" + cr;
                code += cr;
                code += "    // Now in the main render loop, the " + wrksurface + " canvas has a render of the Julia fractal" + cr;
            }
            else if (EffectCode.Text.Contains("Mandelbrot"))
            {
                code += "    // Call the Mandelbrot function" + cr;
                code += "    mandelbrotEffect.Render(new Rectangle[1] {rect},0,1);" + cr;
                code += cr;
                code += "    // Now in the main render loop, the " + wrksurface + " canvas has a render of the Mandelbrot fractal" + cr;
            }
            else if (EffectCode.Text.Contains("Pixelate"))
            {
                code += "    // Call the Pixelate function" + cr;
                code += "    pixelateEffect.Render(new Rectangle[1] {rect},0,1);" + cr;
                code += cr;
                code += "    // Now in the main render loop, the " + wrksurface + " canvas is Pixelated" + cr;
            }
            else if (EffectCode.Text.Contains("Polar Inversion"))
            {
                code += "    // Call the Polar Inversion function" + cr;
                code += "    polarEffect.Render(new Rectangle[1] {rect},0,1);" + cr;
                code += cr;
                code += "    // Now in the main render loop, the " + wrksurface + " canvas has a Polar Inversion" + cr;
            }
            else if (EffectCode.Text.Contains("Radial Blur"))
            {
                code += "    // Call the Radial Blur function" + cr;
                code += "    radialEffect.Render(new Rectangle[1] {rect},0,1);" + cr;
                code += cr;
                code += "    // Now in the main render loop, the " + wrksurface + " canvas has a Radial Blur" + cr;
            }
            else if (EffectCode.Text.Contains("Soften Portrait"))
            {
                code += "    // Call the Soften Portrait function" + cr;
                code += "    portraitEffect.Render(new Rectangle[1] {rect},0,1);" + cr;
                code += cr;
                code += "    // Now in the main render loop, the " + wrksurface + " canvas has a Softend Portrait" + cr;
            }
            else if (EffectCode.Text.Contains("Surface Blur"))
            {
                code += "    // Call the Surface Blur function" + cr;
                code += "    surfaceEffect.Render(new Rectangle[1] {rect},0,1);" + cr;
                code += cr;
                code += "    // Now in the main render loop, the " + wrksurface + " canvas is blurred" + cr;
            }
            else if (EffectCode.Text.Contains("Tile Reflection"))
            {
                code += "    // Call the Tile Reflection function" + cr;
                code += "    tileEffect.Render(new Rectangle[1] {rect},0,1);" + cr;
                code += cr;
                code += "    // Now in the main render loop, the " + wrksurface + " canvas has a Tile Reflection" + cr;
            }
            else if (EffectCode.Text.Contains("Twist"))
            {
                code += "    // Call the Twist function" + cr;
                code += "    twistEffect.Render(new Rectangle[1] {rect},0,1);" + cr;
                code += cr;
                code += "    // Now in the main render loop, the " + wrksurface + " canvas is Twisted" + cr;
            }
            else if (EffectCode.Text.Contains("Unfocus"))
            {
                code += "    // Call the Unfocus function" + cr;
                code += "    unfocusEffect.Render(new Rectangle[1] {rect},0,1);" + cr;
                code += cr;
                code += "    // Now in the main render loop, the " + wrksurface + " canvas is Unfocused" + cr;
            }
            else if (EffectCode.Text.Contains("Vignette"))
            {
                code += "    // Call the Vignette function" + cr;
                code += "    vignetteEffect.Render(new Rectangle[1] {rect},0,1);" + cr;
                code += cr;
                code += "    // Now in the main render loop, the " + wrksurface + " canvas has a Vignette" + cr;
            }
            else if (EffectCode.Text.Contains("Zoom Blur"))
            {
                code += "    // Call the Zoom Blur function" + cr;
                code += "    zoomEffect.Render(new Rectangle[1] {rect},0,1);" + cr;
                code += cr;
                code += "    // Now in the main render loop, the " + wrksurface + " canvas is Zoom Blurred" + cr;
            }

            if (NoStyle.Checked)
            {
                code += "    using (Graphics g = new RenderArgs(" + wrksurface + ").Graphics)" + cr;
                code += "    using (Region gClipRegion = new Region(rect))" + cr;
                code += "    using (Pen pen = new Pen(ColorBgra.Black, 1))" + cr;
                code += "    using (GraphicsPath path = new GraphicsPath())" + cr;
                code += "    using (SolidBrush brush = new SolidBrush(ColorBgra.Black))" + cr;
                code += "    using (Font font = new Font(\"Arial\", 12))" + cr;
                code += "    {" + cr;
                code += "        g.Clip = gClipRegion;" + cr;
                code += "        g.SmoothingMode = SmoothingMode.AntiAlias;" + cr;
                code += "        g.TextRenderingHint = TextRenderingHint.AntiAlias;" + cr;
                code += "        pen.LineJoin = LineJoin.Round;" + cr;
                code += cr;
                code += "        // add additional GDI+ commands here" + cr;
                code += cr;
                code += "    }" + cr;
                string blendOp = "normalOp";
                if (BlendingCode.Text == "Multiply") blendOp = "multiplyOp";
                else if (BlendingCode.Text == "Darken") blendOp = "darkenOp";
                else if (BlendingCode.Text == "Additive") blendOp = "additiveOp";
                else if (BlendingCode.Text == "ColorBurn") blendOp = "colorburnOp";
                else if (BlendingCode.Text == "ColorDodge") blendOp = "colordodgeOp";
                else if (BlendingCode.Text == "Difference") blendOp = "differenceOp";
                else if (BlendingCode.Text == "Glow") blendOp = "glowOp";
                else if (BlendingCode.Text == "Lighten") blendOp = "lightenOp";
                else if (BlendingCode.Text == "Negation") blendOp = "negationOp";
                else if (BlendingCode.Text == "Overlay") blendOp = "overlayOp";
                else if (BlendingCode.Text == "Reflect") blendOp = "reflectOp";
                else if (BlendingCode.Text == "Screen") blendOp = "screenOp";
                else if (BlendingCode.Text == "Xor") blendOp = "xorOp";
                else if (BlendingCode.Text == "User selected blending mode") blendOp = "Amount" + controls;
                code += "    " + blendOp + ".Apply(dst, src, " + wrksurface + ", rect);" + cr;
            }
            else
            {
                // On to the main render loop!
                code += "    for (int y = rect.Top; y < rect.Bottom; y++)" + cr;
                code += "    {" + cr;
                code += "        if (IsCancelRequested) return;" + cr;
                if (AdvancedStyle.Checked)
                {
                    code += "        ColorBgra* srcPtr = src.GetPointAddressUnchecked(rect.Left, y);" + cr;
                    code += "        ColorBgra* dstPtr = dst.GetPointAddressUnchecked(rect.Left, y);" + cr;
                    if (SurfaceCode.Checked)
                    {
                        code += "        ColorBgra* wrkPtr = wrk.GetPointAddressUnchecked(rect.Left, y);" + cr;
                    }
                }
                code += "        for (int x = rect.Left; x < rect.Right; x++)" + cr;
                code += "        {" + cr;
                if (AdvancedStyle.Checked)
                {
                    if ((EffectCode.Text.Contains("Copy") || (EffectCode.Text.Contains("Clipboard"))) && (BlendingCode.Text.Contains("Pass Through")))
                    {
                        code += "            ColorBgra CurrentPixel = *srcPtr;" + cr;
                        blendtop = destcode;
                        if (SurfaceCode.Checked)
                        {
                            blendtop = wrkcode;
                        }
                    }
                    else
                    {
                        code += "            ColorBgra CurrentPixel = *" + wrksurface + "Ptr;" + cr;
                        blendtop = srccode;
                    }
                }
                else
                {
                    if ((EffectCode.Text.Contains("Copy") || (EffectCode.Text.Contains("Clipboard"))) && (BlendingCode.Text.Contains("Pass Through")))
                    {
                        code += "            ColorBgra CurrentPixel = src[x,y];" + cr;
                        blendtop = destcode;
                        if (SurfaceCode.Checked)
                        {
                            blendtop = wrkcode;
                        }
                    }
                    else
                    {
                        code += "            ColorBgra CurrentPixel = " + wrksurface + "[x,y];" + cr;
                        blendtop = srccode;
                    }
                }
                if (EffectCode.Text.Contains("Clipboard"))
                {
                    code += "            if (IsCancelRequested) return;" + cr;
                    code += "            // If clipboard has an image, get it" + cr;
                    code += "            if (img != null)" + cr;
                    code += "            {" + cr;
                    code += "                CurrentPixel = img.GetBilinearSampleWrapped(x, y);" + cr;
                    code += "            }" + cr;
                }
                code += cr;

                string additionalindent = "";

                // Are we near a selection outline?
                if (SelectionCode.Checked)
                {
                    code += "            if ( (!selectionRegion.IsVisible(x-1,y)) || (!selectionRegion.IsVisible(x,y-1)) || (!selectionRegion.IsVisible(x+1,y)) || (!selectionRegion.IsVisible(x,y+1)) )" + cr;
                    code += "            {" + cr;
                    code += "                // This pixel is next to the marching ants" + cr;
                    additionalindent = "    ";
                }

                code += additionalindent + "            // TODO: Add additional pixel processing code here" + cr;

                // Add selected Pixel Op here
                if (PixelOpCode.Text == "Desaturate") code += additionalindent + "            CurrentPixel = desaturateOp.Apply(CurrentPixel);" + cr;
                else if (PixelOpCode.Text == "Invert") code += additionalindent + "            CurrentPixel = invertOp.Apply(CurrentPixel);" + cr;

                code += cr;

                // Add selected Blend Op here
                if (BlendingCode.Text == "Normal") code += additionalindent + "            CurrentPixel = normalOp.Apply(" + blendtop + ", CurrentPixel);" + cr;
                else if (BlendingCode.Text == "Multiply") code += additionalindent + "            CurrentPixel = multiplyOp.Apply(" + blendtop + ", CurrentPixel);" + cr;
                else if (BlendingCode.Text == "Darken") code += additionalindent + "            CurrentPixel = darkenOp.Apply(" + blendtop + ", CurrentPixel);" + cr;
                else if (BlendingCode.Text == "Additive") code += additionalindent + "            CurrentPixel = additiveOp.Apply(" + blendtop + ", CurrentPixel);" + cr;
                else if (BlendingCode.Text == "ColorBurn") code += additionalindent + "            CurrentPixel = colorburnOp.Apply(" + blendtop + ", CurrentPixel);" + cr;
                else if (BlendingCode.Text == "ColorDodge") code += additionalindent + "            CurrentPixel = colordodgeOp.Apply(" + blendtop + ", CurrentPixel);" + cr;
                else if (BlendingCode.Text == "Difference") code += additionalindent + "            CurrentPixel = differenceOp.Apply(" + blendtop + ", CurrentPixel);" + cr;
                else if (BlendingCode.Text == "Glow") code += additionalindent + "            CurrentPixel = glowOp.Apply(" + blendtop + ", CurrentPixel);" + cr;
                else if (BlendingCode.Text == "Lighten") code += additionalindent + "            CurrentPixel = lightenOp.Apply(" + blendtop + ", CurrentPixel);" + cr;
                else if (BlendingCode.Text == "Negation") code += additionalindent + "            CurrentPixel = negationOp.Apply(" + blendtop + ", CurrentPixel);" + cr;
                else if (BlendingCode.Text == "Overlay") code += additionalindent + "            CurrentPixel = overlayOp.Apply(" + blendtop + ", CurrentPixel);" + cr;
                else if (BlendingCode.Text == "Reflect") code += additionalindent + "            CurrentPixel = reflectOp.Apply(" + blendtop + ", CurrentPixel);" + cr;
                else if (BlendingCode.Text == "Screen") code += additionalindent + "            CurrentPixel = screenOp.Apply(" + blendtop + ", CurrentPixel);" + cr;
                else if (BlendingCode.Text == "Xor") code += additionalindent + "            CurrentPixel = xorOp.Apply(" + blendtop + ", CurrentPixel);" + cr;
                else if (BlendingCode.Text == "User selected blending mode") code += additionalindent + "            CurrentPixel = Amount" + controls + ".Apply(" + blendtop + ", CurrentPixel);" + cr;

                code += cr;

                // Add selected Pixel Op here
                if (FinalPixelOpCode.Text == "Desaturate") code += additionalindent + "            CurrentPixel = desaturateOp.Apply(CurrentPixel);" + cr;
                else if (FinalPixelOpCode.Text == "Invert") code += additionalindent + "            CurrentPixel = invertOp.Apply(CurrentPixel);" + cr;

                // HSV Color mode
                if (HsvColorMode.Checked)
                {
                    code += cr;
                    code += additionalindent + "            HsvColor hsv = HsvColor.FromColor(CurrentPixel.ToColor());" + cr;
                    code += additionalindent + "            int H = hsv.Hue;" + cr;
                    code += additionalindent + "            int S = hsv.Saturation;" + cr;
                    code += additionalindent + "            int V = hsv.Value;" + cr;
                    code += additionalindent + "            byte A = CurrentPixel.A;" + cr;
                    code += additionalindent + cr;
                    code += additionalindent + "            // TODO:  Modify H, S, V, and A according to some formula here" + cr;
                    code += additionalindent + cr;
                    code += additionalindent + "            CurrentPixel = ColorBgra.FromColor(new HsvColor(H,S,V).ToColor());" + cr;
                    code += additionalindent + "            CurrentPixel.A = A;" + cr;
                }

                code += cr;
                if (SelectionCode.Checked)
                {
                    code += "            }" + cr;
                    code += "            else" + cr;
                    code += "            {" + cr;
                    code += "                // This pixel is NOT next to the marching ants" + cr;
                    code += "                // TODO: Add additional pixel processing code here" + cr;
                    code += "            }" + cr;
                }

                if (AdvancedStyle.Checked)
                {
                    code += "            *dstPtr = CurrentPixel;" + cr;
                    code += "            srcPtr++;" + cr;
                    code += "            dstPtr++;" + cr;
                    if (SurfaceCode.Checked)
                    {
                        code += "            wrkPtr++;" + cr;
                    }
                }
                else
                {
                    code += "            dst[x,y] = CurrentPixel;" + cr;
                }

                code += "        }" + cr;
                code += "    }" + cr;
            }
            code += "}" + cr;
            code += cr;

            if (CustomHelp.Checked)
            {
                code += cr;
                code += "private void OnWindowHelpButtonClicked(IWin32Window owner, string helpContent)" + cr;
                code += "{" + cr;
                code += "    // The help content supplied in the 'help' comment in the header shows up in the 'helpContent' variable." + cr;
                code += "    // The 'owner' variable is the handle to the effect UI window that is opening a help window." + cr;
                code += "    // Replace the following line with your custom help code" + cr;
                code += "    MessageBox.Show(owner, helpContent, \"Help\", MessageBoxButtons.OK, MessageBoxIcon.Information);" + cr;
                code += "}" + cr;
                code += cr;
            }

            // Make template available to calling function and we're done!
            CodeTemplate = code;
        }

        private void BlendingCode_SelectedIndexChanged(object sender, EventArgs e)
        {
            BlendArrow.Visible = (BlendingCode.Text != "Pass Through");
        }

        private void SurfaceCode_CheckedChanged(object sender, EventArgs e)
        {
            dstLabel.Text = SurfaceCode.Checked ? "WRK\r\nIMAGE" : "DST\r\nIMAGE";
        }

        private void NoStyle_CheckedChanged(object sender, EventArgs e)
        {
            if (NoStyle.Checked)
            {
                SelectionCode.Enabled = false;
                HsvColorMode.Enabled = false;
                PixelOpCode.Text = "Pass Through";
                PixelOpCode.Enabled = false;
                FinalPixelOpCode.Text = "Pass Through";
                FinalPixelOpCode.Enabled = false;
                BlendingCode.Text = "Normal";
                SurfaceCode.Checked = true;
                SurfaceCode.Enabled = false;
                EffectCode.Text = "         Empty------->";
            }
            else
            {
                SelectionCode.Enabled = true;
                HsvColorMode.Enabled = true;
                PixelOpCode.Enabled = true;
                FinalPixelOpCode.Enabled = true;
                BlendingCode.Text = "Pass Through";
                SurfaceCode.Enabled = true;
            }
        }

        private void ForceROI_CheckedChanged(object sender, EventArgs e)
        {
            if (sender == LROI)
            {
                if (LROI.Checked && SRC.Checked)
                {
                    SRC.Checked = false;
                }
            }
            else if (sender == SRC)
            {
                if (SRC.Checked && LROI.Checked)
                {
                    LROI.Checked = false;
                }
            }
        }
    }
}
