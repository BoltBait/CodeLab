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
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace PaintDotNet.Effects
{
    internal partial class FileNew : ChildFormBase
    {
        internal string CodeTemplate;

        // The following effects are handled differently as they only have DST surface and not a SRC surface:
        private static readonly IReadOnlyCollection<string> renderedEffects = new string[] { "Clipboard", "Clouds", "Julia", "Mandelbrot" };

        internal FileNew(string EffectFlag)
        {
            InitializeComponent();

            foreach (Control control in this.Controls)
            {
                if ((control is ComboBox) || (control is ListBox))
                {
                    control.ForeColor = this.ForeColor;
                    control.BackColor = this.BackColor;
                }
            }

            float UIfactor;
            using (Graphics g = flowList.CreateGraphics())
            {
                UIfactor = g.DpiY / 96;
            }
            flowList.ItemHeight = (int)(64 * UIfactor);
            flowList.Font = this.Font;
            DefaultColorComboBox.Items.Add("Primary");
            DefaultColorComboBox.Items.Add("Secondary");
            DefaultColorComboBox.Items.Add("User selected");
            DefaultColorComboBox.Items.AddRange(GetColorNames());
            DefaultColorComboBox.SelectedIndex = 0;
            categoryBox.SelectedIndex = 0;
            pixelOpBox.SelectedIndex = 0;
            effectBox.SelectedIndex = effectBox.FindString("Gaussian");
            sourceBox.SelectedIndex = 0;
            blendBox.SelectedIndex = 1;
            destinationBox.SelectedIndex = 1;
            bottomBox.SelectedIndex = 2;
            updateScreen();

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

        private static string[] GetColorNames()
        {
            List<string> names = typeof(Color).GetProperties(BindingFlags.Public | BindingFlags.Static)
                     .Where(prop => prop.PropertyType == typeof(Color))
                     .Select(prop => prop.Name).ToList();

            names.Sort();

            return names.ToArray();
        }

        private static string getLowerName(string name)
        {
            return getName(name).ToLower();
        }

        private static string getName(string name)
        {
            // remove spaces and slashes from the effect name
            return name.Replace(" ", "").Replace("Brightness/", "").Replace("Hue/", "");
        }

        private static string getEffectPropCode(string effect, string src, string dst, int eCount, bool isLastItem, bool effectPreviouslySent, ref string renderCode)
        {
            const string cr = "\r\n";
            string propCode = "";
            string lowerName = getLowerName(effect);

            if (effect == "Clipboard")
            {
                renderCode = "";
                propCode += "    // Copy from the Clipboard to the " + dst + " surface" + cr;
                propCode += "    for (int y = 0; y < " + dst + ".Size.Height; y++)" + cr;
                propCode += "    {" + cr;
                propCode += "        if (IsCancelRequested) return;" + cr;
                propCode += "        for (int x = 0; x < " + dst + ".Size.Width; x++)" + cr;
                propCode += "        {" + cr;
                propCode += "            if (clipboardSurface != null)" + cr;
                propCode += "            {" + cr;
                propCode += "                //" + dst + "[x,y] = clipboardSurface.GetBilinearSample(x, y);" + cr;
                propCode += "                //" + dst + "[x,y] = clipboardSurface.GetBilinearSampleClamped(x, y);" + cr;
                propCode += "                " + dst + "[x,y] = clipboardSurface.GetBilinearSampleWrapped(x, y);" + cr;
                propCode += "            }" + cr;
                propCode += "            else" + cr;
                propCode += "            {" + cr;
                propCode += "                " + dst + "[x,y] = Color.Transparent;" + cr;
                propCode += "            }" + cr;
                propCode += "        }" + cr;
                propCode += "    }" + cr;
            }
            else
            {
                propCode += "    // " + effect + cr;
                if (lowerName == "clouds")
                {
                    propCode += "    // Use this line instead of the next for Black-and-White clouds:" + cr;
                    propCode += "    //" + lowerName + "Effect.EnvironmentParameters = new EffectEnvironmentParameters(ColorBgra.Black, ColorBgra.White, 0, EnvironmentParameters.GetSelectionAsPdnRegion(), " + src + ");" + cr;
                }
                propCode += "    " + lowerName + "Effect.EnvironmentParameters = EnvironmentParameters;" + cr;
                propCode += "    " + lowerName + "Props = " + lowerName + "Effect.CreatePropertyCollection();" + cr;
                propCode += "    ";
                if (!effectPreviouslySent)
                {
                    propCode += "PropertyBasedEffectConfigToken ";
                }
                propCode += lowerName + "Parameters = new PropertyBasedEffectConfigToken(" + lowerName + "Props);" + cr;
                if (effect.Contains("Gaussian"))
                {
                    propCode += "    " + lowerName + "Parameters.SetPropertyValue(GaussianBlurEffect.PropertyNames.Radius, Amount" + eCount.ToString() + ");" + cr;
                }
                else if (effect.Contains("Contrast"))
                {
                    propCode += "    " + lowerName + "Parameters.SetPropertyValue(BrightnessAndContrastAdjustment.PropertyNames.Brightness, Amount" + eCount.ToString() + ");" + cr;
                    eCount++;
                    propCode += "    " + lowerName + "Parameters.SetPropertyValue(BrightnessAndContrastAdjustment.PropertyNames.Contrast, Amount" + eCount.ToString() + ");" + cr;
                }
                else if (effect.Contains("Saturation"))
                {
                    propCode += "    " + lowerName + "Parameters.SetPropertyValue(HueAndSaturationAdjustment.PropertyNames.Hue, Amount" + eCount.ToString() + ");" + cr;
                    eCount++;
                    propCode += "    " + lowerName + "Parameters.SetPropertyValue(HueAndSaturationAdjustment.PropertyNames.Saturation, Amount" + eCount.ToString() + ");" + cr;
                    eCount++;
                    propCode += "    " + lowerName + "Parameters.SetPropertyValue(HueAndSaturationAdjustment.PropertyNames.Lightness, Amount" + eCount.ToString() + ");" + cr;
                }
                else if (effect.Contains("Motion"))
                {
                    propCode += "    " + lowerName + "Parameters.SetPropertyValue(MotionBlurEffect.PropertyNames.Angle, Amount" + eCount.ToString() + ");" + cr;
                    eCount++;
                    propCode += "    " + lowerName + "Parameters.SetPropertyValue(MotionBlurEffect.PropertyNames.Centered, Amount" + eCount.ToString() + ");" + cr;
                    eCount++;
                    propCode += "    " + lowerName + "Parameters.SetPropertyValue(MotionBlurEffect.PropertyNames.Distance, Amount" + eCount.ToString() + ");" + cr;
                }
                else if (effect.Contains("Frosted"))
                {
                    propCode += "    " + lowerName + "Parameters.SetPropertyValue(FrostedGlassEffect.PropertyNames.MaxScatterRadius, Amount" + eCount.ToString() + ");" + cr;
                    eCount++;
                    propCode += "    " + lowerName + "Parameters.SetPropertyValue(FrostedGlassEffect.PropertyNames.MinScatterRadius, Amount" + eCount.ToString() + ");" + cr;
                    eCount++;
                    propCode += "    " + lowerName + "Parameters.SetPropertyValue(FrostedGlassEffect.PropertyNames.NumSamples, Amount" + eCount.ToString() + ");" + cr;
                }
                else if (effect.Contains("Add Noise"))
                {
                    propCode += "    " + lowerName + "Parameters.SetPropertyValue(AddNoiseEffect.PropertyNames.Intensity, Amount" + eCount.ToString() + ");" + cr;
                    eCount++;
                    propCode += "    " + lowerName + "Parameters.SetPropertyValue(AddNoiseEffect.PropertyNames.Saturation, Amount" + eCount.ToString() + ");" + cr;
                    eCount++;
                    propCode += "    " + lowerName + "Parameters.SetPropertyValue(AddNoiseEffect.PropertyNames.Coverage, Amount" + eCount.ToString() + ");" + cr;
                }
                else if (effect.Contains("Clouds"))
                {
                    propCode += "    " + lowerName + "Parameters.SetPropertyValue(CloudsEffect.PropertyNames.Scale, Amount" + eCount.ToString() + ");" + cr;
                    eCount++;
                    propCode += "    " + lowerName + "Parameters.SetPropertyValue(CloudsEffect.PropertyNames.Power, Amount" + eCount.ToString() + ");" + cr;
                    eCount++;
                    propCode += "    " + lowerName + "Parameters.SetPropertyValue(CloudsEffect.PropertyNames.BlendMode, Amount" + eCount.ToString() + ");" + cr;
                    eCount++;
                    propCode += "    " + lowerName + "Parameters.SetPropertyValue(CloudsEffect.PropertyNames.Seed, (int)Amount" + eCount.ToString() + ");" + cr;
                }
                else if (effect.Contains("Oil Painting"))
                {
                    propCode += "    " + lowerName + "Parameters.SetPropertyValue(OilPaintingEffect.PropertyNames.BrushSize, Amount" + eCount.ToString() + ");" + cr;
                    eCount++;
                    propCode += "    " + lowerName + "Parameters.SetPropertyValue(OilPaintingEffect.PropertyNames.Coarseness, Amount" + eCount.ToString() + ");" + cr;
                }
                else if (effect.Contains("Reduce Noise"))
                {
                    propCode += "    " + lowerName + "Parameters.SetPropertyValue(ReduceNoiseEffect.PropertyNames.Radius, Amount" + eCount.ToString() + ");" + cr;
                    eCount++;
                    propCode += "    " + lowerName + "Parameters.SetPropertyValue(ReduceNoiseEffect.PropertyNames.Strength, Amount" + eCount.ToString() + ");" + cr;
                }
                else if (effect.Contains("Median"))
                {
                    propCode += "    " + lowerName + "Parameters.SetPropertyValue(MedianEffect.PropertyNames.Radius, Amount" + eCount.ToString() + ");" + cr;
                    eCount++;
                    propCode += "    " + lowerName + "Parameters.SetPropertyValue(MedianEffect.PropertyNames.Percentile, Amount" + eCount.ToString() + ");" + cr;
                }
                else if (effect.Contains("Sharpen"))
                {
                    propCode += "    " + lowerName + "Parameters.SetPropertyValue(SharpenEffect.PropertyNames.Amount, Amount" + eCount.ToString() + ");" + cr;
                }
                else if (effect.Contains("Edge Detect"))
                {
                    propCode += "    " + lowerName + "Parameters.SetPropertyValue(EdgeDetectEffect.PropertyNames.Angle, Amount" + eCount.ToString() + ");" + cr;
                }
                else if (effect.Contains("Emboss"))
                {
                    propCode += "    " + lowerName + "Parameters.SetPropertyValue(EmbossEffect.PropertyNames.Angle, Amount" + eCount.ToString() + ");" + cr;
                }
                else if (effect.Contains("Relief"))
                {
                    propCode += "    " + lowerName + "Parameters.SetPropertyValue(ReliefEffect.PropertyNames.Angle, Amount" + eCount.ToString() + ");" + cr;
                }
                else if (effect.Contains("Outline"))
                {
                    propCode += "    " + lowerName + "Parameters.SetPropertyValue(OutlineEffect.PropertyNames.Thickness, Amount" + eCount.ToString() + ");" + cr;
                    eCount++;
                    propCode += "    " + lowerName + "Parameters.SetPropertyValue(OutlineEffect.PropertyNames.Intensity, Amount" + eCount.ToString() + ");" + cr;
                }
                else if (effect.Contains("Pencil Sketch"))
                {
                    propCode += "    " + lowerName + "Parameters.SetPropertyValue(PencilSketchEffect.PropertyNames.PencilTipSize, Amount" + eCount.ToString() + ");" + cr;
                    eCount++;
                    propCode += "    " + lowerName + "Parameters.SetPropertyValue(PencilSketchEffect.PropertyNames.ColorRange, Amount" + eCount.ToString() + ");" + cr;
                }
                else if (effect.Contains("Posterize"))
                {
                    propCode += "    " + lowerName + "Parameters.SetPropertyValue(PosterizeAdjustment.PropertyNames.RedLevels, Amount" + eCount.ToString() + ");" + cr;
                    eCount++;
                    propCode += "    " + lowerName + "Parameters.SetPropertyValue(PosterizeAdjustment.PropertyNames.GreenLevels, Amount" + eCount.ToString() + ");" + cr;
                    eCount++;
                    propCode += "    " + lowerName + "Parameters.SetPropertyValue(PosterizeAdjustment.PropertyNames.BlueLevels, Amount" + eCount.ToString() + ");" + cr;
                    eCount++;
                    propCode += "    " + lowerName + "Parameters.SetPropertyValue(PosterizeAdjustment.PropertyNames.LinkLevels, Amount" + eCount.ToString() + ");" + cr;
                }
                else if (effect.Contains("Ink Sketch"))
                {
                    propCode += "    " + lowerName + "Parameters.SetPropertyValue(InkSketchEffect.PropertyNames.InkOutline, Amount" + eCount.ToString() + ");" + cr;
                    eCount++;
                    propCode += "    " + lowerName + "Parameters.SetPropertyValue(InkSketchEffect.PropertyNames.Coloring, Amount" + eCount.ToString() + ");" + cr;
                }
                else if (effect.Contains("Radial Blur"))
                {
                    propCode += "    " + lowerName + "Parameters.SetPropertyValue(RadialBlurEffect.PropertyNames.Angle, Amount" + eCount.ToString() + ");" + cr;
                    eCount++;
                    propCode += "    " + lowerName + "Parameters.SetPropertyValue(RadialBlurEffect.PropertyNames.Offset, Amount" + eCount.ToString() + ");" + cr;
                    eCount++;
                    propCode += "    " + lowerName + "Parameters.SetPropertyValue(RadialBlurEffect.PropertyNames.Quality, Amount" + eCount.ToString() + ");" + cr;
                }
                else if (effect.Contains("Surface Blur"))
                {
                    propCode += "    " + lowerName + "Parameters.SetPropertyValue(SurfaceBlurEffect.PropertyName.Radius, Amount" + eCount.ToString() + ");" + cr;
                    eCount++;
                    propCode += "    " + lowerName + "Parameters.SetPropertyValue(SurfaceBlurEffect.PropertyName.Threshold, Amount" + eCount.ToString() + ");" + cr;
                }
                else if (effect.Contains("Unfocus"))
                {
                    propCode += "    " + lowerName + "Parameters.SetPropertyValue(UnfocusEffect.PropertyNames.Radius, Amount" + eCount.ToString() + ");" + cr;
                }
                else if (effect.Contains("Zoom Blur"))
                {
                    propCode += "    " + lowerName + "Parameters.SetPropertyValue(ZoomBlurEffect.PropertyNames.Amount, Amount" + eCount.ToString() + ");" + cr;
                    eCount++;
                    propCode += "    " + lowerName + "Parameters.SetPropertyValue(ZoomBlurEffect.PropertyNames.Offset, Amount" + eCount.ToString() + ");" + cr;
                }
                else if (effect.Contains("Bulge"))
                {
                    propCode += "    " + lowerName + "Parameters.SetPropertyValue(BulgeEffect.PropertyNames.Amount, Amount" + eCount.ToString() + ");" + cr;
                    eCount++;
                    propCode += "    " + lowerName + "Parameters.SetPropertyValue(BulgeEffect.PropertyNames.Offset, Amount" + eCount.ToString() + ");" + cr;
                }
                else if (effect.Contains("Crystalize"))
                {
                    propCode += "    " + lowerName + "Parameters.SetPropertyValue(CrystalizeEffect.PropertyNames.Size, Amount" + eCount.ToString() + ");" + cr;
                    eCount++;
                    propCode += "    " + lowerName + "Parameters.SetPropertyValue(CrystalizeEffect.PropertyNames.Quality, Amount" + eCount.ToString() + ");" + cr;
                    eCount++;
                    propCode += "    " + lowerName + "Parameters.SetPropertyValue(CrystalizeEffect.PropertyNames.Seed, (int)Amount" + eCount.ToString() + ");" + cr;
                }
                else if (effect.Contains("Dents"))
                {
                    propCode += "    " + lowerName + "Parameters.SetPropertyValue(DentsEffect.PropertyNames.Scale, Amount" + eCount.ToString() + ");" + cr;
                    eCount++;
                    propCode += "    " + lowerName + "Parameters.SetPropertyValue(DentsEffect.PropertyNames.Refraction, Amount" + eCount.ToString() + ");" + cr;
                    eCount++;
                    propCode += "    " + lowerName + "Parameters.SetPropertyValue(DentsEffect.PropertyNames.Roughness, Amount" + eCount.ToString() + ");" + cr;
                    eCount++;
                    propCode += "    " + lowerName + "Parameters.SetPropertyValue(DentsEffect.PropertyNames.Tension, Amount" + eCount.ToString() + ");" + cr;
                    eCount++;
                    propCode += "    " + lowerName + "Parameters.SetPropertyValue(DentsEffect.PropertyNames.Quality, Amount" + eCount.ToString() + ");" + cr;
                    eCount++;
                    propCode += "    " + lowerName + "Parameters.SetPropertyValue(DentsEffect.PropertyNames.Seed, (int)Amount" + eCount.ToString() + ");" + cr;
                }
                else if (effect.Contains("Pixelate"))
                {
                    propCode += "    " + lowerName + "Parameters.SetPropertyValue(PixelateEffect.PropertyNames.CellSize, Amount" + eCount.ToString() + ");" + cr;
                }
                else if (effect.Contains("Polar Inversion"))
                {
                    propCode += "    " + lowerName + "Parameters.SetPropertyValue(PolarInversionEffect.PropertyNames.Amount, Amount" + eCount.ToString() + ");" + cr;
                    eCount++;
                    propCode += "    " + lowerName + "Parameters.SetPropertyValue(PolarInversionEffect.PropertyNames.Offset, Amount" + eCount.ToString() + ");" + cr;
                    eCount++;
                    propCode += "    " + lowerName + "Parameters.SetPropertyValue(PolarInversionEffect.PropertyNames.EdgeBehavior, Amount" + eCount.ToString() + ");" + cr;
                    eCount++;
                    propCode += "    " + lowerName + "Parameters.SetPropertyValue(PolarInversionEffect.PropertyNames.Quality, Amount" + eCount.ToString() + ");" + cr;
                }
                else if (effect.Contains("Tile"))
                {
                    propCode += "    " + lowerName + "Parameters.SetPropertyValue(TileEffect.PropertyNames.Rotation, Amount" + eCount.ToString() + ");" + cr;
                    eCount++;
                    propCode += "    " + lowerName + "Parameters.SetPropertyValue(TileEffect.PropertyNames.SquareSize, Amount" + eCount.ToString() + ");" + cr;
                    eCount++;
                    propCode += "    " + lowerName + "Parameters.SetPropertyValue(TileEffect.PropertyNames.Curvature, Amount" + eCount.ToString() + ");" + cr;
                    eCount++;
                    propCode += "    " + lowerName + "Parameters.SetPropertyValue(TileEffect.PropertyNames.Quality, Amount" + eCount.ToString() + ");" + cr;
                }
                else if (effect.Contains("Twist"))
                {
                    propCode += "    " + lowerName + "Parameters.SetPropertyValue(TwistEffect.PropertyNames.Amount, Amount" + eCount.ToString() + ");" + cr;
                    eCount++;
                    propCode += "    " + lowerName + "Parameters.SetPropertyValue(TwistEffect.PropertyNames.Size, Amount" + eCount.ToString() + ");" + cr;
                    eCount++;
                    propCode += "    " + lowerName + "Parameters.SetPropertyValue(TwistEffect.PropertyNames.Offset, Amount" + eCount.ToString() + ");" + cr;
                    eCount++;
                    propCode += "    " + lowerName + "Parameters.SetPropertyValue(TwistEffect.PropertyNames.Quality, Amount" + eCount.ToString() + ");" + cr;
                }
                else if (effect.Contains("Glow"))
                {
                    propCode += "    " + lowerName + "Parameters.SetPropertyValue(GlowEffect.PropertyNames.Radius, Amount" + eCount.ToString() + ");" + cr;
                    eCount++;
                    propCode += "    " + lowerName + "Parameters.SetPropertyValue(GlowEffect.PropertyNames.Brightness, Amount" + eCount.ToString() + ");" + cr;
                    eCount++;
                    propCode += "    " + lowerName + "Parameters.SetPropertyValue(GlowEffect.PropertyNames.Contrast, Amount" + eCount.ToString() + ");" + cr;
                }
                else if (effect.Contains("Soften Portrait"))
                {
                    propCode += "    " + lowerName + "Parameters.SetPropertyValue(SoftenPortraitEffect.PropertyNames.Softness, Amount" + eCount.ToString() + ");" + cr;
                    eCount++;
                    propCode += "    " + lowerName + "Parameters.SetPropertyValue(SoftenPortraitEffect.PropertyNames.Lighting, Amount" + eCount.ToString() + ");" + cr;
                    eCount++;
                    propCode += "    " + lowerName + "Parameters.SetPropertyValue(SoftenPortraitEffect.PropertyNames.Warmth, Amount" + eCount.ToString() + ");" + cr;
                }
                else if (effect.Contains("Vignette"))
                {
                    propCode += "    " + lowerName + "Parameters.SetPropertyValue(VignetteEffect.PropertyNames.Offset, Amount" + eCount.ToString() + ");" + cr;
                    eCount++;
                    propCode += "    " + lowerName + "Parameters.SetPropertyValue(VignetteEffect.PropertyNames.Radius, Amount" + eCount.ToString() + ");" + cr;
                    eCount++;
                    propCode += "    " + lowerName + "Parameters.SetPropertyValue(VignetteEffect.PropertyNames.Amount, Amount" + eCount.ToString() + ");" + cr;
                }
                else if (effect.Contains("Julia"))
                {
                    propCode += "    " + lowerName + "Parameters.SetPropertyValue(JuliaFractalEffect.PropertyNames.Factor, Amount" + eCount.ToString() + ");" + cr;
                    eCount++;
                    propCode += "    " + lowerName + "Parameters.SetPropertyValue(JuliaFractalEffect.PropertyNames.Zoom, Amount" + eCount.ToString() + ");" + cr;
                    eCount++;
                    propCode += "    " + lowerName + "Parameters.SetPropertyValue(JuliaFractalEffect.PropertyNames.Angle, Amount" + eCount.ToString() + ");" + cr;
                    eCount++;
                    propCode += "    " + lowerName + "Parameters.SetPropertyValue(JuliaFractalEffect.PropertyNames.Quality, Amount" + eCount.ToString() + ");" + cr;
                }
                else if (effect.Contains("Mandelbrot"))
                {
                    propCode += "    " + lowerName + "Parameters.SetPropertyValue(MandelbrotFractalEffect.PropertyNames.Factor, Amount" + eCount.ToString() + ");" + cr;
                    eCount++;
                    propCode += "    " + lowerName + "Parameters.SetPropertyValue(MandelbrotFractalEffect.PropertyNames.Zoom, Amount" + eCount.ToString() + ");" + cr;
                    eCount++;
                    propCode += "    " + lowerName + "Parameters.SetPropertyValue(MandelbrotFractalEffect.PropertyNames.Angle, Amount" + eCount.ToString() + ");" + cr;
                    eCount++;
                    propCode += "    " + lowerName + "Parameters.SetPropertyValue(MandelbrotFractalEffect.PropertyNames.Quality, Amount" + eCount.ToString() + ");" + cr;
                    eCount++;
                    propCode += "    " + lowerName + "Parameters.SetPropertyValue(MandelbrotFractalEffect.PropertyNames.InvertColors, Amount" + eCount.ToString() + ");" + cr;
                }
                propCode += "    " + lowerName + "Effect.SetRenderInfo(" + lowerName + "Parameters, new RenderArgs(" + dst + "), new RenderArgs(" + src + "));" + cr;
                if (isLastItem)
                {
                    renderCode = "    // Now call the " + effect + " function from " + src + " surface to " + dst + " surface" + cr;
                    renderCode += "    " + lowerName + "Effect.Render(new Rectangle[1] {rect},0,1);" + cr;
                }
                else
                {
                    propCode += "    if (IsCancelRequested) return;" + cr;
                    propCode += "    " + lowerName + "Effect.Render(new Rectangle[1] {" + dst + ".Bounds},0,1);" + cr;
                }
            }
            return propCode;
        }

        private static string getUIControls(string effect, ref int controlCount)
        {
            string code = "";
            const string cr = "\r\n";
            // Add controls for selected options
            if (effect.Contains("Gaussian"))
            {
                code += "IntSliderControl Amount" + controlCount.ToString() + " = 10; // [0,100] " + effect + " Radius" + cr;
                controlCount++;
            }
            else if (effect.Contains("Contrast"))
            {
                code += "IntSliderControl Amount" + controlCount.ToString() + " = 10; // [-100,100] " + effect + " Brightness" + cr;
                controlCount++;
                code += "IntSliderControl Amount" + controlCount.ToString() + " = 10; // [-100,100] " + effect + " Contrast" + cr;
                controlCount++;
            }
            else if (effect.Contains("Saturation"))
            {
                code += "IntSliderControl Amount" + controlCount.ToString() + " = 0; // [-180,180] " + effect + " Hue" + cr;
                controlCount++;
                code += "IntSliderControl Amount" + controlCount.ToString() + " = 100; // [0,200] " + effect + " Saturation" + cr;
                controlCount++;
                code += "IntSliderControl Amount" + controlCount.ToString() + " = 0; // [-100,100] " + effect + " Lightness" + cr;
                controlCount++;
            }
            else if (effect.Contains("Motion"))
            {
                code += "AngleControl Amount" + controlCount.ToString() + " = 45; // [-180,180] " + effect + " Angle" + cr;
                controlCount++;
                code += "CheckboxControl Amount" + controlCount.ToString() + " = true; // [0,1] " + effect + " Centered" + cr;
                controlCount++;
                code += "IntSliderControl Amount" + controlCount.ToString() + " = 10; // [1,200] " + effect + " Distance" + cr;
                controlCount++;
            }
            else if (effect.Contains("Frosted"))
            {
                code += "DoubleSliderControl Amount" + controlCount.ToString() + " = 3; // [0,200] " + effect + " Maximum Scatter Radius" + cr;
                controlCount++;
                code += "DoubleSliderControl Amount" + controlCount.ToString() + " = 0; // [0,200] " + effect + " Minimum Scatter Radius" + cr;
                controlCount++;
                code += "IntSliderControl Amount" + controlCount.ToString() + " = 2; // [1,8] " + effect + " Smoothness" + cr;
                controlCount++;
            }
            else if (effect.Contains("Add Noise"))
            {
                code += "IntSliderControl Amount" + controlCount.ToString() + " = 64; // [0,100] " + effect + " Intensity" + cr;
                controlCount++;
                code += "IntSliderControl Amount" + controlCount.ToString() + " = 100; // [0,400] " + effect + " Color Saturation" + cr;
                controlCount++;
                code += "DoubleSliderControl Amount" + controlCount.ToString() + " = 100; // [0,100] " + effect + " Coverage" + cr;
                controlCount++;
            }
            else if (effect.Contains("Clouds"))
            {
                code += "IntSliderControl Amount" + controlCount.ToString() + " = 250; // [2,1000] " + effect + " Scale" + cr;
                controlCount++;
                code += "DoubleSliderControl Amount" + controlCount.ToString() + " = 0.5; // [0,1] " + effect + " Roughness" + cr;
                controlCount++;
                code += "BinaryPixelOp Amount" + controlCount.ToString() + " = LayerBlendModeUtil.CreateCompositionOp(LayerBlendMode.Normal); // " + effect + " Blend Mode" + cr;
                controlCount++;
                code += "ReseedButtonControl Amount" + controlCount.ToString() + " = 0; // [255] " + effect + " Reseed" + cr;
                controlCount++;
            }
            else if (effect.Contains("Oil Painting"))
            {
                code += "IntSliderControl Amount" + controlCount.ToString() + " = 3; // [1,8] " + effect + " Brush Size" + cr;
                controlCount++;
                code += "IntSliderControl Amount" + controlCount.ToString() + " = 50; // [3,255] " + effect + " Coarseness" + cr;
                controlCount++;
            }
            else if (effect.Contains("Reduce Noise"))
            {
                code += "IntSliderControl Amount" + controlCount.ToString() + " = 10; // [0,200] " + effect + " Radius" + cr;
                controlCount++;
                code += "DoubleSliderControl Amount" + controlCount.ToString() + " = 0.4; // [0,1] " + effect + " Strength" + cr;
                controlCount++;
            }
            else if (effect.Contains("Median"))
            {
                code += "IntSliderControl Amount" + controlCount.ToString() + " = 10; // [1,200] " + effect + " Radius" + cr;
                controlCount++;
                code += "IntSliderControl Amount" + controlCount.ToString() + " = 50; // [0,100] " + effect + " Percentile" + cr;
                controlCount++;
            }
            else if (effect.Contains("Sharpen"))
            {
                code += "IntSliderControl Amount" + controlCount.ToString() + " = 2; // [1,20] " + effect + " Amount" + cr;
                controlCount++;
            }
            else if (effect.Contains("Edge Detect"))
            {
                code += "AngleControl Amount" + controlCount.ToString() + " = 45; // [-180,180] " + effect + " Angle" + cr;
                controlCount++;
            }
            else if (effect.Contains("Emboss"))
            {
                code += "AngleControl Amount" + controlCount.ToString() + " = 0; // [-180,180] " + effect + " Angle" + cr;
                controlCount++;
            }
            else if (effect.Contains("Relief"))
            {
                code += "AngleControl Amount" + controlCount.ToString() + " = 45; // [-180,180] " + effect + " Angle" + cr;
                controlCount++;
            }
            else if (effect.Contains("Outline"))
            {
                code += "IntSliderControl Amount" + controlCount.ToString() + " = 3; // [1,200] " + effect + " Thickness" + cr;
                controlCount++;
                code += "IntSliderControl Amount" + controlCount.ToString() + " = 50; // [0,100] " + effect + " Intensity" + cr;
                controlCount++;
            }
            else if (effect.Contains("Pencil Sketch"))
            {
                code += "IntSliderControl Amount" + controlCount.ToString() + " = 2; // [1,20] " + effect + " Pencil Tip Size" + cr;
                controlCount++;
                code += "IntSliderControl Amount" + controlCount.ToString() + " = 0; // [-20,20] " + effect + " Range" + cr;
                controlCount++;
            }
            else if (effect.Contains("Posterize"))
            {
                code += "IntSliderControl Amount" + controlCount.ToString() + " = 16; // [2,64] " + effect + " Red" + cr;
                controlCount++;
                code += "IntSliderControl Amount" + controlCount.ToString() + " = 16; // [2,64] " + effect + " Green" + cr;
                controlCount++;
                code += "IntSliderControl Amount" + controlCount.ToString() + " = 16; // [2,64] " + effect + " Blue" + cr;
                controlCount++;
                code += "CheckboxControl Amount" + controlCount.ToString() + " = true; // [0,1] " + effect + " Linked" + cr;
                controlCount++;
            }
            else if (effect.Contains("Ink Sketch"))
            {
                code += "IntSliderControl Amount" + controlCount.ToString() + " = 50; // [0,99] " + effect + " Ink Outline" + cr;
                controlCount++;
                code += "IntSliderControl Amount" + controlCount.ToString() + " = 50; // [0,100] " + effect + " Coloring" + cr;
                controlCount++;
            }
            else if (effect.Contains("Radial Blur"))
            {
                code += "AngleControl Amount" + controlCount.ToString() + " = 2; // [-180,180] " + effect + " Angle" + cr;
                controlCount++;
                code += "PanSliderControl Amount" + controlCount.ToString() + " = Pair.Create(0.000,0.000); // " + effect + " Center" + cr;
                controlCount++;
                code += "IntSliderControl Amount" + controlCount.ToString() + " = 2; // [1,5] " + effect + " Quality" + cr;
                controlCount++;
            }
            else if (effect.Contains("Surface Blur"))
            {
                code += "IntSliderControl Amount" + controlCount.ToString() + " = 6; // [1,100] " + effect + " Radius" + cr;
                controlCount++;
                code += "IntSliderControl Amount" + controlCount.ToString() + " = 15; // [1,100] " + effect + " Threshold" + cr;
                controlCount++;
            }
            else if (effect.Contains("Unfocus"))
            {
                code += "IntSliderControl Amount" + controlCount.ToString() + " = 4; // [1,200] " + effect + " Radius" + cr;
                controlCount++;
            }
            else if (effect.Contains("Zoom Blur"))
            {
                code += "IntSliderControl Amount" + controlCount.ToString() + " = 10; // [0,100] " + effect + " Zoom Amount" + cr;
                controlCount++;
                code += "PanSliderControl Amount" + controlCount.ToString() + " = Pair.Create(0.000,0.000); // " + effect + " Center" + cr;
                controlCount++;
            }
            else if (effect.Contains("Bulge"))
            {
                code += "IntSliderControl Amount" + controlCount.ToString() + " = 45; // [-200,100] " + effect + " Bulge" + cr;
                controlCount++;
                code += "PanSliderControl Amount" + controlCount.ToString() + " = Pair.Create(0.000,0.000); // " + effect + " Center" + cr;
                controlCount++;
            }
            else if (effect.Contains("Crystalize"))
            {
                code += "IntSliderControl Amount" + controlCount.ToString() + " = 8; // [2,250] " + effect + " Cell Size" + cr;
                controlCount++;
                code += "IntSliderControl Amount" + controlCount.ToString() + " = 2; // [1,5] " + effect + " Quality" + cr;
                controlCount++;
                code += "ReseedButtonControl Amount" + controlCount.ToString() + " = 0; // [255] " + effect + " Reseed" + cr;
                controlCount++;
            }
            else if (effect.Contains("Dents"))
            {
                code += "DoubleSliderControl Amount" + controlCount.ToString() + " = 25; // [1,200] " + effect + " Scale" + cr;
                controlCount++;
                code += "DoubleSliderControl Amount" + controlCount.ToString() + " = 50; // [0,200] " + effect + " Refraction" + cr;
                controlCount++;
                code += "DoubleSliderControl Amount" + controlCount.ToString() + " = 10; // [0,100] " + effect + " Roughness" + cr;
                controlCount++;
                code += "DoubleSliderControl Amount" + controlCount.ToString() + " = 10; // [0,100] " + effect + " Tension" + cr;
                controlCount++;
                code += "IntSliderControl Amount" + controlCount.ToString() + " = 2; // [1,5] " + effect + " Quality" + cr;
                controlCount++;
                code += "ReseedButtonControl Amount" + controlCount.ToString() + " = 0; // [255] " + effect + " Reseed" + cr;
                controlCount++;
            }
            else if (effect.Contains("Pixelate"))
            {
                code += "IntSliderControl Amount" + controlCount.ToString() + " = 2; // [1,100] " + effect + " Cell size" + cr;
                controlCount++;
            }
            else if (effect.Contains("Polar Inversion"))
            {
                code += "DoubleSliderControl Amount" + controlCount.ToString() + " = 3; // [0,200] " + effect + " Amount" + cr;
                controlCount++;
                code += "PanSliderControl Amount" + controlCount.ToString() + " = Pair.Create(0.000,0.000); // " + effect + " Offset" + cr;
                controlCount++;
                code += "ListBoxControl Amount" + controlCount.ToString() + " = 2; // Edge Behavior|Clamp|Reflect|Wrap" + cr;
                controlCount++;
                code += "IntSliderControl Amount" + controlCount.ToString() + " = 2; // [1,5] " + effect + " Quality" + cr;
                controlCount++;
            }
            else if (effect.Contains("Tile Reflection"))
            {
                code += "AngleControl Amount" + controlCount.ToString() + " = 30; // [-180,180] " + effect + " Angle" + cr;
                controlCount++;
                code += "DoubleSliderControl Amount" + controlCount.ToString() + " = 40; // [1,800] " + effect + " Tile Size" + cr;
                controlCount++;
                code += "DoubleSliderControl Amount" + controlCount.ToString() + " = 8; // [-100,100] " + effect + " Curvature" + cr;
                controlCount++;
                code += "IntSliderControl Amount" + controlCount.ToString() + " = 2; // [1,5] " + effect + " Quality" + cr;
                controlCount++;
            }
            else if (effect.Contains("Twist"))
            {
                code += "DoubleSliderControl Amount" + controlCount.ToString() + " = 30; // [-200,200] " + effect + " Amount / Direction" + cr;
                controlCount++;
                code += "DoubleSliderControl Amount" + controlCount.ToString() + " = 1; // [0.01,2] " + effect + " Size" + cr;
                controlCount++;
                code += "PanSliderControl Amount" + controlCount.ToString() + " = Pair.Create(0.000,0.000); // " + effect + " Center" + cr;
                controlCount++;
                code += "IntSliderControl Amount" + controlCount.ToString() + " = 2; // [1,5] " + effect + " Quality" + cr;
                controlCount++;
            }
            else if (effect.Contains("Glow"))
            {
                code += "IntSliderControl Amount" + controlCount.ToString() + " = 6; // [1,20] " + effect + " Radius" + cr;
                controlCount++;
                code += "IntSliderControl Amount" + controlCount.ToString() + " = 10; // [-100,100] " + effect + " Brightness" + cr;
                controlCount++;
                code += "IntSliderControl Amount" + controlCount.ToString() + " = 10; // [-100,100] " + effect + " Contrast" + cr;
                controlCount++;
            }
            else if (effect.Contains("Soften Portrait"))
            {
                code += "IntSliderControl Amount" + controlCount.ToString() + " = 5; // [0,10] " + effect + " Softness" + cr;
                controlCount++;
                code += "IntSliderControl Amount" + controlCount.ToString() + " = 0; // [-20,20] " + effect + " Lighting" + cr;
                controlCount++;
                code += "IntSliderControl Amount" + controlCount.ToString() + " = 10; // [0,20] " + effect + " Warmth" + cr;
                controlCount++;
            }
            else if (effect.Contains("Vignette"))
            {
                code += "PanSliderControl Amount" + controlCount.ToString() + " = Pair.Create(0.000,0.000); // " + effect + " Center" + cr;
                controlCount++;
                code += "DoubleSliderControl Amount" + controlCount.ToString() + " = 0.5; // [0.1,4] " + effect + " Radius" + cr;
                controlCount++;
                code += "DoubleSliderControl Amount" + controlCount.ToString() + " = 1; // [0,1] " + effect + " Density" + cr;
                controlCount++;
            }
            else if (effect.Contains("Julia"))
            {
                code += "DoubleSliderControl Amount" + controlCount.ToString() + " = 4; // [1,10] " + effect + " Factor" + cr;
                controlCount++;
                code += "DoubleSliderControl Amount" + controlCount.ToString() + " = 1; // [0.1,50] " + effect + " Zoom" + cr;
                controlCount++;
                code += "AngleControl Amount" + controlCount.ToString() + " = 0; // [-180,180] " + effect + " Angle" + cr;
                controlCount++;
                code += "IntSliderControl Amount" + controlCount.ToString() + " = 2; // [1,5] " + effect + " Quality" + cr;
                controlCount++;
            }
            else if (effect.Contains("Mandelbrot"))
            {
                code += "IntSliderControl Amount" + controlCount.ToString() + " = 1; // [1,10] " + effect + " Factor" + cr;
                controlCount++;
                code += "DoubleSliderControl Amount" + controlCount.ToString() + " = 10; // [0,100] " + effect + " Zoom" + cr;
                controlCount++;
                code += "AngleControl Amount" + controlCount.ToString() + " = 0; // [-180,180] " + effect + " Angle" + cr;
                controlCount++;
                code += "IntSliderControl Amount" + controlCount.ToString() + " = 2; // [1,5] " + effect + " Quality" + cr;
                controlCount++;
                code += "CheckboxControl Amount" + controlCount.ToString() + " = false; // [0,1] " + effect + " Invert Colors" + cr;
                controlCount++;
            }
            if (effect.Contains("User selected"))
            {
                code += "BinaryPixelOp Amount" + controlCount.ToString() + " = LayerBlendModeUtil.CreateCompositionOp(LayerBlendMode.Normal); // " + effect + " Blending Mode" + cr;
                controlCount++;
            }
            if (effect.Contains("Fill with Color"))
            {
                code += "ColorWheelControl Amount" + controlCount.ToString() + " = ColorBgra.FromBgra(0, 0, 0, 255); // [PrimaryColor?!] Fill with user selected color" + cr;
                controlCount++;
            }
            return code;
        }

        private void DoIt_Click(object sender, EventArgs e)
        {
            const string cr = "\r\n";
            string wrksurface = "dst";
            string dstsurface = "dst";
            string srcsurface = "src";
            string disposecode = "";
            string rendercode = "";
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
            string destcode = "dst[x,y]";
            string srccode = "src[x,y]";
            int currentUIcount = 1;
            bool workSurfaceNeeded = false;
            bool clipboardNeeded = false;
            string[] flowListArray = flowList.Items.OfType<string>().ToArray();

            if (AdvancedStyle.Checked)
            {
                destcode = "*dstPtr";
                srccode = "*srcPtr";
            }

            // Let's write some code!

            // Generate a list of required UI controls
            code += "#region UICode" + cr;
            for (int i = 0; i < flowListArray.Length; i++)
            {
                flowListArray[i] += "|" + currentUIcount.ToString();
                string[] elementDetails = flowListArray[i].Split('|');
                if (elementDetails[1] == "Effect" ||
                    (elementDetails[1] == "Blend" && elementDetails[2] == "User selected") ||
                    (elementDetails[1] == "Fill" && elementDetails[3] == "User selected"))
                {
                    code += getUIControls(elementDetails[2], ref currentUIcount);
                }
                if (elementDetails[0].Contains("W"))
                {
                    workSurfaceNeeded = true;
                }
                if (elementDetails[2] == "Clipboard")
                {
                    clipboardNeeded = true;
                }
            }
            code += "#endregion" + cr;
            code += cr;

            // setup for a work surface if one is needed
            if (workSurfaceNeeded)
            {
                code += "// Working surface" + cr;
                code += "Surface wrk = null;" + cr;
                code += cr;
            }

            // setup for calling complex effects
            if (flowListArray.Any(element => element.Contains("Gaussian")))
            {
                code += "// Setup for calling the Gaussian Blur effect" + cr;
                code += "GaussianBlurEffect gaussianblurEffect = new GaussianBlurEffect();" + cr;
                code += "PropertyCollection gaussianblurProps;" + cr;
                code += cr;
                disposecode += "        if (gaussianblurEffect != null) gaussianblurEffect.Dispose();" + cr;
                disposecode += "        gaussianblurEffect = null;" + cr;
            }
            if (flowListArray.Any(element => element.Contains("Contrast")))
            {
                code += "// Setup for calling the Brightness and Contrast Adjustment function" + cr;
                code += "BrightnessAndContrastAdjustment contrastEffect = new BrightnessAndContrastAdjustment();" + cr;
                code += "PropertyCollection contrastProps;" + cr;
                code += cr;
                disposecode += "        if (contrastEffect != null) contrastEffect.Dispose();" + cr;
                disposecode += "        contrastEffect = null;" + cr;
            }
            if (flowListArray.Any(element => element.Contains("Saturation")))
            {
                code += "// Setup for calling the Hue and Saturation Adjustment function" + cr;
                code += "HueAndSaturationAdjustment saturationEffect = new HueAndSaturationAdjustment();" + cr;
                code += "PropertyCollection saturationProps;" + cr;
                code += cr;
                disposecode += "        if (saturationEffect != null) saturationEffect.Dispose();" + cr;
                disposecode += "        saturationEffect = null;" + cr;
            }
            if (flowListArray.Any(element => element.Contains("Frosted")))
            {
                code += "// Setup for calling the Frosted Glass effect" + cr;
                code += "FrostedGlassEffect frostedglassEffect = new FrostedGlassEffect();" + cr;
                code += "PropertyCollection frostedglassProps;" + cr;
                code += cr;
                disposecode += "        if (frostedglassEffect != null) frostedglassEffect.Dispose();" + cr;
                disposecode += "        frostedglassEffect = null;" + cr;
            }
            if (flowListArray.Any(element => element.Contains("Add Noise")))
            {
                code += "// Setup for calling the Add Noise effect" + cr;
                code += "AddNoiseEffect addnoiseEffect = new AddNoiseEffect();" + cr;
                code += "PropertyCollection addnoiseProps;" + cr;
                code += cr;
                disposecode += "        if (addnoiseEffect != null) addnoiseEffect.Dispose();" + cr;
                disposecode += "        addnoiseEffect = null;" + cr;
            }
            if (flowListArray.Any(element => element.Contains("Motion Blur")))
            {
                code += "// Setup for calling the Motion Blur effect" + cr;
                code += "MotionBlurEffect motionblurEffect = new MotionBlurEffect();" + cr;
                code += "PropertyCollection motionblurProps;" + cr;
                code += cr;
                disposecode += "        if (motionblurEffect != null) motionblurEffect.Dispose();" + cr;
                disposecode += "        motionblurEffect = null;" + cr;
            }
            if (flowListArray.Any(element => element.Contains("Clouds")))
            {
                code += "// Setup for calling the Render Clouds function" + cr;
                code += "CloudsEffect cloudsEffect = new CloudsEffect();" + cr;
                code += "PropertyCollection cloudsProps;" + cr;
                code += cr;
                disposecode += "        if (cloudsEffect != null) cloudsEffect.Dispose();" + cr;
                disposecode += "        cloudsEffect = null;" + cr;
            }
            if (flowListArray.Any(element => element.Contains("Oil Painting")))
            {
                code += "// Setup for calling the Oil Painting effect" + cr;
                code += "OilPaintingEffect oilpaintingEffect = new OilPaintingEffect();" + cr;
                code += "PropertyCollection oilpaintingProps;" + cr;
                code += cr;
                disposecode += "        if (oilpaintingEffect != null) oilpaintingEffect.Dispose();" + cr;
                disposecode += "        oilpaintingEffect = null;" + cr;
            }
            if (flowListArray.Any(element => element.Contains("Reduce Noise")))
            {
                code += "// Setup for calling the Reduce Noise effect" + cr;
                code += "ReduceNoiseEffect reducenoiseEffect = new ReduceNoiseEffect();" + cr;
                code += "PropertyCollection reducenoiseProps;" + cr;
                code += cr;
                disposecode += "        if (reducenoiseEffect != null) reducenoiseEffect.Dispose();" + cr;
                disposecode += "        reducenoiseEffect = null;" + cr;
            }
            if (flowListArray.Any(element => element.Contains("Median")))
            {
                code += "// Setup for calling the Median effect" + cr;
                code += "MedianEffect medianEffect = new MedianEffect();" + cr;
                code += "PropertyCollection medianProps;" + cr;
                code += cr;
                disposecode += "        if (medianEffect != null) medianEffect.Dispose();" + cr;
                disposecode += "        medianEffect = null;" + cr;
            }
            if (flowListArray.Any(element => element.Contains("Edge Detect")))
            {
                code += "// Setup for calling the Edge Detect effect" + cr;
                code += "EdgeDetectEffect edgedetectEffect = new EdgeDetectEffect();" + cr;
                code += "PropertyCollection edgedetectProps;" + cr;
                code += cr;
                disposecode += "        if (edgedetectEffect != null) edgedetectEffect.Dispose();" + cr;
                disposecode += "        edgedetectEffect = null;" + cr;
            }
            if (flowListArray.Any(element => element.Contains("Emboss")))
            {
                code += "// Setup for calling the Emboss effect" + cr;
                code += "EmbossEffect embossEffect = new EmbossEffect();" + cr;
                code += "PropertyCollection embossProps;" + cr;
                code += cr;
                disposecode += "        if (embossEffect != null) embossEffect.Dispose();" + cr;
                disposecode += "        embossEffect = null;" + cr;
            }
            if (flowListArray.Any(element => element.Contains("Relief")))
            {
                code += "// Setup for calling the Relief effect" + cr;
                code += "ReliefEffect reliefEffect = new ReliefEffect();" + cr;
                code += "PropertyCollection reliefProps;" + cr;
                code += cr;
                disposecode += "        if (reliefEffect != null) reliefEffect.Dispose();" + cr;
                disposecode += "        reliefEffect = null;" + cr;
            }
            if (flowListArray.Any(element => element.Contains("Outline")))
            {
                code += "// Setup for calling the Outline effect" + cr;
                code += "OutlineEffect outlineEffect = new OutlineEffect();" + cr;
                code += "PropertyCollection outlineProps;" + cr;
                code += cr;
                disposecode += "        if (outlineEffect != null) outlineEffect.Dispose();" + cr;
                disposecode += "        outlineEffect = null;" + cr;
            }
            if (flowListArray.Any(element => element.Contains("Sharpen")))
            {
                code += "// Setup for calling the Sharpen effect" + cr;
                code += "SharpenEffect sharpenEffect = new SharpenEffect();" + cr;
                code += "PropertyCollection sharpenProps;" + cr;
                code += cr;
                disposecode += "        if (sharpenEffect != null) sharpenEffect.Dispose();" + cr;
                disposecode += "        sharpenEffect = null;" + cr;
            }
            if (flowListArray.Any(element => element.Contains("Pencil Sketch")))
            {
                code += "// Setup for calling the Pencil Sketch effect" + cr;
                code += "PencilSketchEffect pencilsketchEffect = new PencilSketchEffect();" + cr;
                code += "PropertyCollection pencilsketchProps;" + cr;
                code += cr;
                disposecode += "        if (pencilsketchEffect != null) pencilsketchEffect.Dispose();" + cr;
                disposecode += "        pencilsketchEffect = null;" + cr;
            }
            if (flowListArray.Any(element => element.Contains("Posterize")))
            {
                code += "// Setup for calling the Posterize adjustment" + cr;
                code += "PosterizeAdjustment posterizeEffect = new PosterizeAdjustment();" + cr;
                code += "PropertyCollection posterizeProps;" + cr;
                code += cr;
                disposecode += "        if (posterizeEffect != null) posterizeEffect.Dispose();" + cr;
                disposecode += "        posterizeEffect = null;" + cr;
            }
            if (flowListArray.Any(element => element.Contains("Sepia")))
            {
                code += "// Setup for calling the Sepia effect" + cr;
                code += "SepiaEffect sepiaEffect = new SepiaEffect();" + cr;
                code += "PropertyCollection sepiaProps;" + cr;
                code += cr;
                disposecode += "        if (sepiaEffect != null) sepiaEffect.Dispose();" + cr;
                disposecode += "        sepiaEffect = null;" + cr;
            }
            if (flowListArray.Any(element => element.Contains("Ink Sketch")))
            {
                code += "// Setup for calling the Ink Sketch effect" + cr;
                code += "InkSketchEffect inksketchEffect = new InkSketchEffect();" + cr;
                code += "PropertyCollection inksketchProps;" + cr;
                code += cr;
                disposecode += "        if (inksketchEffect != null) inksketchEffect.Dispose();" + cr;
                disposecode += "        inksketchEffect = null;" + cr;
            }
            if (flowListArray.Any(element => element.Contains("Radial Blur")))
            {
                code += "// Setup for calling the Radial Blur effect" + cr;
                code += "RadialBlurEffect radialblurEffect = new RadialBlurEffect();" + cr;
                code += "PropertyCollection radialblurProps;" + cr;
                code += cr;
                disposecode += "        if (radialblurEffect != null) radialblurEffect.Dispose();" + cr;
                disposecode += "        radialblurEffect = null;" + cr;
            }
            if (flowListArray.Any(element => element.Contains("Surface Blur")))
            {
                code += "// Setup for calling the Surface Blur effect" + cr;
                code += "SurfaceBlurEffect surfaceblurEffect = new SurfaceBlurEffect();" + cr;
                code += "PropertyCollection surfaceblurProps;" + cr;
                code += cr;
                disposecode += "        if (surfaceblurEffect != null) surfaceblurEffect.Dispose();" + cr;
                disposecode += "        surfaceblurEffect = null;" + cr;
            }
            if (flowListArray.Any(element => element.Contains("Unfocus")))
            {
                code += "// Setup for calling the Unfocus effect" + cr;
                code += "UnfocusEffect unfocusEffect = new UnfocusEffect();" + cr;
                code += "PropertyCollection unfocusProps;" + cr;
                code += cr;
                disposecode += "        if (unfocusEffect != null) unfocusEffect.Dispose();" + cr;
                disposecode += "        unfocusEffect = null;" + cr;
            }
            if (flowListArray.Any(element => element.Contains("Zoom Blur")))
            {
                code += "// Setup for calling the Zoom Blur effect" + cr;
                code += "ZoomBlurEffect zoomblurEffect = new ZoomBlurEffect();" + cr;
                code += "PropertyCollection zoomblurProps;" + cr;
                code += cr;
                disposecode += "        if (zoomblurEffect != null) zoomblurEffect.Dispose();" + cr;
                disposecode += "        zoomblurEffect = null;" + cr;
            }
            if (flowListArray.Any(element => element.Contains("Bulge")))
            {
                code += "// Setup for calling the Bulge effect" + cr;
                code += "BulgeEffect bulgeEffect = new BulgeEffect();" + cr;
                code += "PropertyCollection bulgeProps;" + cr;
                code += cr;
                disposecode += "        if (bulgeEffect != null) bulgeEffect.Dispose();" + cr;
                disposecode += "        bulgeEffect = null;" + cr;
            }
            if (flowListArray.Any(element => element.Contains("Crystalize")))
            {
                code += "// Setup for calling the Crystalize effect" + cr;
                code += "CrystalizeEffect crystalizeEffect = new CrystalizeEffect();" + cr;
                code += "PropertyCollection crystalizeProps;" + cr;
                code += cr;
                disposecode += "        if (crystalizeEffect != null) crystalizeEffect.Dispose();" + cr;
                disposecode += "        crystalizeEffect = null;" + cr;
            }
            if (flowListArray.Any(element => element.Contains("Dents")))
            {
                code += "// Setup for calling the Dents effect" + cr;
                code += "DentsEffect dentsEffect = new DentsEffect();" + cr;
                code += "PropertyCollection dentsProps;" + cr;
                code += cr;
                disposecode += "        if (dentsEffect != null) dentsEffect.Dispose();" + cr;
                disposecode += "        dentsEffect = null;" + cr;
            }
            if (flowListArray.Any(element => element.Contains("Pixelate")))
            {
                code += "// Setup for calling the Pixelate effect" + cr;
                code += "PixelateEffect pixelateEffect = new PixelateEffect();" + cr;
                code += "PropertyCollection pixelateProps;" + cr;
                code += cr;
                disposecode += "        if (pixelateEffect != null) pixelateEffect.Dispose();" + cr;
                disposecode += "        pixelateEffect = null;" + cr;
            }
            if (flowListArray.Any(element => element.Contains("Polar Inversion")))
            {
                code += "// Setup for calling the Polar Inversion effect" + cr;
                code += "PolarInversionEffect polarinversionEffect = new PolarInversionEffect();" + cr;
                code += "PropertyCollection polarinversionProps;" + cr;
                code += cr;
                disposecode += "        if (polarinversionEffect != null) polarinversionEffect.Dispose();" + cr;
                disposecode += "        polarinversionEffect = null;" + cr;
            }
            if (flowListArray.Any(element => element.Contains("Tile")))
            {
                code += "// Setup for calling the Tile Reflection effect" + cr;
                code += "TileEffect tilereflectionEffect = new TileEffect();" + cr;
                code += "PropertyCollection tilereflectionProps;" + cr;
                code += cr;
                disposecode += "        if (tilereflectionEffect != null) tilereflectionEffect.Dispose();" + cr;
                disposecode += "        tilereflectionEffect = null;" + cr;
            }
            if (flowListArray.Any(element => element.Contains("Twist")))
            {
                code += "// Setup for calling the Twist effect" + cr;
                code += "TwistEffect twistEffect = new TwistEffect();" + cr;
                code += "PropertyCollection twistProps;" + cr;
                code += cr;
                disposecode += "        if (twistEffect != null) twistEffect.Dispose();" + cr;
                disposecode += "        twistEffect = null;" + cr;
            }
            if (flowListArray.Any(element => element.Contains("Glow")))
            {
                code += "// Setup for calling the Glow effect" + cr;
                code += "GlowEffect glowEffect = new GlowEffect();" + cr;
                code += "PropertyCollection glowProps;" + cr;
                code += cr;
                disposecode += "        if (glowEffect != null) glowEffect.Dispose();" + cr;
                disposecode += "        glowEffect = null;" + cr;
            }
            if (flowListArray.Any(element => element.Contains("Soften Portrait")))
            {
                code += "// Setup for calling the Soften Portrait effect" + cr;
                code += "SoftenPortraitEffect softenportraitEffect = new SoftenPortraitEffect();" + cr;
                code += "PropertyCollection softenportraitProps;" + cr;
                code += cr;
                disposecode += "        if (softenportraitEffect != null) softenportraitEffect.Dispose();" + cr;
                disposecode += "        softenportraitEffect = null;" + cr;
            }
            if (flowListArray.Any(element => element.Contains("Vignette")))
            {
                code += "// Setup for calling the Vignette effect" + cr;
                code += "VignetteEffect vignetteEffect = new VignetteEffect();" + cr;
                code += "PropertyCollection vignetteProps;" + cr;
                code += cr;
                disposecode += "        if (vignetteEffect != null) vignetteEffect.Dispose();" + cr;
                disposecode += "        vignetteEffect = null;" + cr;
            }
            if (flowListArray.Any(element => element.Contains("Julia")))
            {
                code += "// Setup for calling the Julia effect" + cr;
                code += "JuliaFractalEffect juliaEffect = new JuliaFractalEffect();" + cr;
                code += "PropertyCollection juliaProps;" + cr;
                code += cr;
                disposecode += "        if (juliaEffect != null) juliaEffect.Dispose();" + cr;
                disposecode += "        juliaEffect = null;" + cr;
            }
            if (flowListArray.Any(element => element.Contains("Mandelbrot")))
            {
                code += "// Setup for calling the Mandelbrot effect" + cr;
                code += "MandelbrotFractalEffect mandelbrotEffect = new MandelbrotFractalEffect();" + cr;
                code += "PropertyCollection mandelbrotProps;" + cr;
                code += cr;
                disposecode += "        if (mandelbrotEffect != null) mandelbrotEffect.Dispose();" + cr;
                disposecode += "        mandelbrotEffect = null;" + cr;
            }
            // Pixel Ops
            if (flowListArray.Any(element => element.Contains("Pixel Op")))
            {
                code += "// Setup for using pixel op" + cr;
                if (flowListArray.Any(element => element.Contains("Desaturate")))
                {
                    code += "private UnaryPixelOps.Desaturate desaturateOp = new UnaryPixelOps.Desaturate();" + cr;
                }
                if (flowListArray.Any(element => element.Contains("Invert")))
                {
                    code += "private UnaryPixelOps.Invert invertOp = new UnaryPixelOps.Invert();" + cr;
                }
                code += cr;
            }
            // Blends
            if (flowListArray.Any(element => element.Contains("Blend") && !element.Contains("User selected")))
            {
                code += "// Setup for selected blending op" + cr;
                if (flowListArray.Any(element => element.Contains("Blend|Normal")))
                {
                    code += "private BinaryPixelOp normalOp = LayerBlendModeUtil.CreateCompositionOp(LayerBlendMode.Normal);" + cr;
                }
                if (flowListArray.Any(element => element.Contains("Blend|Multiply")))
                {
                    code += "private BinaryPixelOp multiplyOp = LayerBlendModeUtil.CreateCompositionOp(LayerBlendMode.Multiply);" + cr;
                }
                if (flowListArray.Any(element => element.Contains("Blend|Darken")))
                {
                    code += "private BinaryPixelOp darkenOp = LayerBlendModeUtil.CreateCompositionOp(LayerBlendMode.Darken);" + cr;
                }
                if (flowListArray.Any(element => element.Contains("Blend|Additive")))
                {
                    code += "private BinaryPixelOp additiveOp = LayerBlendModeUtil.CreateCompositionOp(LayerBlendMode.Additive);" + cr;
                }
                if (flowListArray.Any(element => element.Contains("Blend|ColorBurn")))
                {
                    code += "private BinaryPixelOp colorburnOp = LayerBlendModeUtil.CreateCompositionOp(LayerBlendMode.ColorBurn);" + cr;
                }
                if (flowListArray.Any(element => element.Contains("Blend|ColorDodge")))
                {
                    code += "private BinaryPixelOp colordodgeOp = LayerBlendModeUtil.CreateCompositionOp(LayerBlendMode.ColorDodge);" + cr;
                }
                if (flowListArray.Any(element => element.Contains("Blend|Difference")))
                {
                    code += "private BinaryPixelOp differenceOp = LayerBlendModeUtil.CreateCompositionOp(LayerBlendMode.Difference);" + cr;
                }
                if (flowListArray.Any(element => element.Contains("Blend|Glow")))
                {
                    code += "private BinaryPixelOp glowOp = LayerBlendModeUtil.CreateCompositionOp(LayerBlendMode.Glow);" + cr;
                }
                if (flowListArray.Any(element => element.Contains("Blend|Lighten")))
                {
                    code += "private BinaryPixelOp lightenOp = LayerBlendModeUtil.CreateCompositionOp(LayerBlendMode.Lighten);" + cr;
                }
                if (flowListArray.Any(element => element.Contains("Blend|Negation")))
                {
                    code += "private BinaryPixelOp negationOp = LayerBlendModeUtil.CreateCompositionOp(LayerBlendMode.Negation);" + cr;
                }
                if (flowListArray.Any(element => element.Contains("Blend|Overlay")))
                {
                    code += "private BinaryPixelOp overlayOp = LayerBlendModeUtil.CreateCompositionOp(LayerBlendMode.Overlay);" + cr;
                }
                if (flowListArray.Any(element => element.Contains("Blend|Reflect")))
                {
                    code += "private BinaryPixelOp reflectOp = LayerBlendModeUtil.CreateCompositionOp(LayerBlendMode.Reflect);" + cr;
                }
                if (flowListArray.Any(element => element.Contains("Blend|Screen")))
                {
                    code += "private BinaryPixelOp screenOp = LayerBlendModeUtil.CreateCompositionOp(LayerBlendMode.Screen);" + cr;
                }
                if (flowListArray.Any(element => element.Contains("Blend|Xor")))
                {
                    code += "private BinaryPixelOp xorOp = LayerBlendModeUtil.CreateCompositionOp(LayerBlendMode.Xor);" + cr;
                }
                code += cr;
            }
            // setup for using the clipboard
            if (clipboardNeeded)
            {
                code += "private Surface clipboardSurface = null;" + cr;
                code += "private bool readClipboard = false;" + cr;
                code += cr;
            }

            // OnDispose
            if (workSurfaceNeeded || disposecode.Length > 0 || clipboardNeeded)
            {
                code += "protected override void OnDispose(bool disposing)" + cr;
                code += "{" + cr;
                code += "    if (disposing)" + cr;
                code += "    {" + cr;
                code += "        // Release any surfaces or effects you've created" + cr;
                if (workSurfaceNeeded)
                {
                    code += "        if (wrk != null) wrk.Dispose();" + cr;
                    code += "        wrk = null;" + cr;
                }
                if (clipboardNeeded)
                {
                    code += "        if (clipboardSurface != null) clipboardSurface.Dispose();" + cr;
                    code += "        clipboardSurface = null;" + cr;
                }
                if (disposecode.Length > 0)
                {
                    code += disposecode;
                }
                code += "    }" + cr;
                code += cr;
                code += "    base.OnDispose(disposing);" + cr;
                code += "}" + cr;
                code += cr;
            }

            // build the PreRender code
            code += "// This single-threaded function is called after the UI changes and before the Render function is called" + cr;
            code += "// The purpose is to prepare anything you'll need in the Render function" + cr;
            code += "void PreRender(Surface dst, Surface src)" + cr;
            code += "{" + cr;

            // WRK surface
            if (workSurfaceNeeded)
            {
                code += "    if (wrk == null)" + cr;
                code += "    {" + cr;
                code += "        wrk = new Surface(src.Size);" + cr;
                code += "    }" + cr;
                code += cr;
            }

            // Clipboard
            if (clipboardNeeded)
            {
                code += "    if (!readClipboard)" + cr;
                code += "    {" + cr;
                code += "        readClipboard = true;" + cr;
                code += "        clipboardSurface = Services.GetService<IClipboardService>().TryGetSurface();" + cr;
                code += "    }" + cr;
            }

            if (flowListArray.Length > 0)
            {
                for (int i = 0; i < flowListArray.Length; i++)
                {
                    bool lastItem = i == flowListArray.Length - 1;
                    string[] elementDetails = flowListArray[i].Split('|');
                    //[0] = Dest, Src Dest, Top Bottom Dest
                    //[1] = Effect, Blend, Pixel, Fill, or Copy
                    //[2] = Effect / Blend / Pixel Op Name
                    //[3] = Fill color or comment
                    //[4] = Starting control name is AmountX
                    string targets = elementDetails[0];
                    string etype = elementDetails[1];
                    string ename = elementDetails[2];
                    string ecolor = elementDetails[3];
                    int elnum = int.Parse(elementDetails[4]);

                    dstsurface = "dst";
                    wrksurface = "wrk";
                    srcsurface = "src";
                    switch (targets.Length)
                    {
                        case 1:
                            if (targets[0] == 'W') dstsurface = "wrk";
                            break;
                        case 2:
                            if (targets[0] == 'W') srcsurface = "wrk";
                            if (targets[0] == 'D') srcsurface = "dst";
                            if (targets[1] == 'W') dstsurface = "wrk";
                            break;
                        case 3:
                            if (targets[0] == 'W') srcsurface = "wrk";
                            if (targets[0] == 'D') srcsurface = "dst";
                            if (targets[1] == 'S') wrksurface = "src";
                            if (targets[1] == 'D') wrksurface = "dst";
                            if (targets[2] == 'W') dstsurface = "wrk";
                            break;
                    }
                    // prepare for the Render loop
                    if (lastItem)
                    {
                        // it will be going from the destination of the last item in the pixel flow list to the DST canvas.
                        if (AdvancedStyle.Checked)
                        {
                            srccode = "*" + dstsurface + "Ptr";
                        }
                        else
                        {
                            srccode = dstsurface + "[x,y]";
                        }
                    }
                    // add code for this item in the pixel flow list
                    switch (etype)
                    {
                        case "Effect":
                            code += getEffectPropCode(ename, srcsurface, dstsurface, elnum, lastItem, code.Contains("PropertyBasedEffectConfigToken " + getLowerName(ename) + "Parameters ="), ref rendercode);
                            break;
                        case "Blend":
                            if (lastItem)
                            {
                                if (ename == "User selected")
                                {
                                    rendercode = "    // Blend the " + srcsurface + " surface and the " + wrksurface + " surface to the " + dstsurface + " surface" + cr;
                                    rendercode += "    Amount" + elnum.ToString() + ".Apply(" + dstsurface + ", " + srcsurface + ", " + wrksurface + ", rect);" + cr;
                                }
                                else
                                {
                                    rendercode = "    // " + ename + " Blend the " + srcsurface + " surface and the " + wrksurface + " surface to the " + dstsurface + " surface" + cr;
                                    rendercode += "    " + ename.ToLower() + "Op.Apply(" + dstsurface + ", " + srcsurface + ", " + wrksurface + ", rect);" + cr;
                                }
                            }
                            else
                            {
                               code += "    if (IsCancelRequested) return;" + cr;
                               if (ename == "User selected")
                                {
                                    code += "    // Blend the " + srcsurface + " surface and the " + wrksurface + " surface to the " + dstsurface + " surface" + cr;
                                    code += "    Amount" + elnum.ToString() + ".Apply(" + dstsurface + ", " + srcsurface + ", " + wrksurface + ");" + cr;
                                }
                                else
                                {
                                    code += "    // " + ename + " Blend the " + srcsurface + " surface and the " + wrksurface + " surface to the " + dstsurface + " surface" + cr;
                                    code += "    " + ename.ToLower() + "Op.Apply(" + dstsurface + ", " + srcsurface + ", " + wrksurface + ");" + cr;
                                }
                            }
                            break;
                        case "Pixel Op":
                            if (lastItem)
                            {
                                rendercode = "    // " + ename + " the " + srcsurface + " surface to the " + dstsurface + " surface" + cr;
                                rendercode += "    " + ename.ToLower() + "Op.Apply(" + dstsurface + ", " + srcsurface + ", rect);" + cr;
                            }
                            else
                            {
                                code += "    if (IsCancelRequested) return;" + cr;
                                code += "    // " + ename + " the " + srcsurface + " surface to the " + dstsurface + " surface" + cr;
                                code += "    " + ename.ToLower() + "Op.Apply(" + dstsurface + ", " + srcsurface + ",  new Rectangle[1] {" + dstsurface + ".Bounds},0,1);" + cr;
                            }
                            break;
                        case "Fill":
                            if (lastItem)
                            {
                                if (ecolor == "User selected")
                                {
                                    rendercode = "    // Fill the " + dstsurface + " surface with a user selected color" + cr;
                                    rendercode += "    " + dstsurface + ".Clear(rect,Amount" + elnum.ToString() + ");" + cr;
                                }
                                else
                                {
                                    rendercode = "    // Fill the " + dstsurface + " surface with " + ecolor + " color" + cr;
                                    if (ecolor == "Primary" || ecolor == "Secondary")
                                    {
                                        ecolor = "EnvironmentParameters." + ecolor + "Color";
                                    }
                                    else
                                    {
                                        ecolor = "ColorBgra." + ecolor;
                                    }
                                    rendercode += "    " + dstsurface + ".Clear(rect," + ecolor + ");" + cr;
                                }
                            }
                            else
                            {
                                code += "    if (IsCancelRequested) return;" + cr;
                                if (ecolor == "User selected")
                                {
                                    code += "    // Fill the " + dstsurface + " surface with a user selected color" + cr;
                                    code += "    " + dstsurface + ".Clear(Amount" + elnum.ToString() + ");" + cr;
                                }
                                else
                                {
                                    code += "    // Fill the " + dstsurface + " surface with " + ecolor + " color" + cr;
                                    if (ecolor == "Primary" || ecolor == "Secondary")
                                    {
                                        ecolor = "EnvironmentParameters." + ecolor + "Color";
                                    }
                                    else
                                    {
                                        ecolor = "ColorBgra." + ecolor;
                                    }
                                    code += "    " + dstsurface + ".Clear(" + ecolor + ");" + cr;
                                }
                            }
                            break;
                        case "Copy":
                            if (lastItem)
                            {
                                rendercode = "    // Copy the " + srcsurface + " surface to the " + dstsurface + " surface" + cr;
                                rendercode += "    " + dstsurface + ".CopySurface(" + srcsurface + ",rect.Location,rect);" + cr;
                            }
                            else
                            {
                                code += "    if (IsCancelRequested) return;" + cr;
                                code += "    // Copy the " + srcsurface + " surface to the " + dstsurface + " surface" + cr;
                                code += "    " + dstsurface + ".CopySurface(" + srcsurface + ");" + cr;
                            }
                            break;
                    }
                }
            }
            // finish PreRender
            code += "}" + cr + cr;
            code += "// Here is the main multi-threaded render function" + cr;
            code += "// The dst canvas is broken up into rectangles and" + cr;
            code += "// your job is to write to each pixel of that rectangle" + cr;
            // if we're using pointers, the render function must be marked 'unsafe'
            if (AdvancedStyle.Checked)
            {
                code += "unsafe ";
            }
            code += "void Render(Surface dst, Surface src, Rectangle rect)" + cr;
            code += "{" + cr;

            // Add in code for the desired variables the user wants
            if (SelectionCode.Checked)
            {
                code += "    // Delete this line if you don't need the selection outline shape" + cr;
                code += "    PdnRegion selectionRegion = EnvironmentParameters.GetSelectionAsPdnRegion();" + cr;
                code += cr;
            }
            if (CenterCode.Checked)
            {
                code += "    // Delete these 3 lines if you don't need to know the center point of the current selection" + cr;
                code += "    Rectangle selection = EnvironmentParameters.SelectionBounds;" + cr;
                code += "    int CenterX = ((selection.Right - selection.Left) / 2) + selection.Left;" + cr;
                code += "    int CenterY = ((selection.Bottom - selection.Top) / 2) + selection.Top;" + cr;
                code += cr;
            }
            if (PrimaryColorCode.Checked)
            {
                code += "    // Delete these lines if you don't need the primary or secondary color" + cr;
                code += "    ColorBgra PrimaryColor = EnvironmentParameters.PrimaryColor;" + cr;
                code += "    ColorBgra SecondaryColor = EnvironmentParameters.SecondaryColor;" + cr;
                code += cr;
            }
            if (PaletteCode.Checked)
            {
                code += "    // Delete these lines if you don't need the current or default palette" + cr;
                code += "    IReadOnlyList<ColorBgra> DefaultColors = Services.GetService<IPalettesService>().DefaultPalette;" + cr;
                code += "    IReadOnlyList<ColorBgra> CurrentColors = Services.GetService<IPalettesService>().CurrentPalette; " + cr;
                code += cr;
            }
            if (PenWidthCode.Checked)
            {
                code += "    // Delete the next line if you don't need the brush width" + cr;
                code += "    int BrushWidth = (int)EnvironmentParameters.BrushWidth;" + cr;
                code += cr;
            }

            if (rendercode.Length > 0)
            {
                code += rendercode;
                code += cr;
            }

            // Are we doing GDI+ commands style?
            if (NoStyle.Checked)
            {
                if (flowListArray.Length > 0)
                {
                    if (dstsurface != "dst")
                    {
                        code += "    // Copy the " + dstsurface + " surface to the dst surface" + cr;
                        code += "    dst.CopySurface(" + dstsurface + ", rect.Location, rect);" + cr;
                        code += cr;
                    }
                    // else we've already rendered to the DST surface.
                }
                else
                {
                    code += "    // Copy the src surface to the dst surface" + cr;
                    code += "    dst.CopySurface(src, rect.Location, rect);" + cr;
                    code += cr;
                }
                code += "    // Setup for drawing using GDI+ commands" + cr;
                code += "    using (Graphics g = new RenderArgs(dst).Graphics)" + cr;
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
                code += "        // TODO: add additional GDI+ commands here, such as:" + cr;
                code += "        //g.DrawString(\"CodeLab Rocks!\", font, brush, 0, 0);" + cr;
                code += "        //g.DrawLine(pen, 5, 15, 122, 15);" + cr;
                code += cr;
                code += "    }" + cr;
            }
            else
            {
                // On to the main render loop!
                code += cr;
                code += "    // Step through each row of the current rectangle" + cr;
                code += "    for (int y = rect.Top; y < rect.Bottom; y++)" + cr;
                code += "    {" + cr;
                code += "        if (IsCancelRequested) return;" + cr;
                if (AdvancedStyle.Checked)
                {
                    code += "        ColorBgra* srcPtr = src.GetPointAddressUnchecked(rect.Left, y);" + cr;
                    code += "        ColorBgra* dstPtr = dst.GetPointAddressUnchecked(rect.Left, y);" + cr;
                    if (workSurfaceNeeded)
                    {
                        code += "        ColorBgra* wrkPtr = wrk.GetPointAddressUnchecked(rect.Left, y);" + cr;
                    }
                }
                code += "        // Step through each pixel on the current row of the rectangle" + cr;
                code += "        for (int x = rect.Left; x < rect.Right; x++)" + cr;
                code += "        {" + cr;
                code += "            ColorBgra CurrentPixel = " + srccode + ";" + cr;
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

                code += cr;

                // HSV Color mode
                if (HsvColorMode.Checked)
                {
                    code += cr;
                    code += additionalindent + "            HsvColor hsv = HsvColor.FromColor(CurrentPixel.ToColor());" + cr;
                    code += additionalindent + "            int H = hsv.Hue;         // 0-360" + cr;
                    code += additionalindent + "            int S = hsv.Saturation;  // 0-100" + cr;
                    code += additionalindent + "            int V = hsv.Value;       // 0-100" + cr;
                    code += additionalindent + "            byte A = CurrentPixel.A; // 0-255" + cr;
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
                    code += cr;
                    code += "            }" + cr;
                }

                code += "            " + destcode + " = CurrentPixel;" + cr;
                if (AdvancedStyle.Checked)
                {
                    code += "            srcPtr++;" + cr;
                    code += "            dstPtr++;" + cr;
                    if (workSurfaceNeeded)
                    {
                        code += "            wrkPtr++;" + cr;
                    }
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

        private void NoStyle_CheckedChanged(object sender, EventArgs e)
        {
            if (NoStyle.Checked)
            {
                SelectionCode.Enabled = false;
                HsvColorMode.Enabled = false;
            }
            else
            {
                SelectionCode.Enabled = true;
                HsvColorMode.Enabled = true;
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

        private void flowList_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index == -1)
            {
                return;
            }
            float UIfactor = e.Graphics.DpiY / 96;
            e.DrawBackground();
            string listItemText = flowList.Items[e.Index].ToString();
            /*
                SW|Effect|Gaussian Blur|SRC to WRK
                SWD|Blend|Darken|SRC and WRK to DST
                DD|Pixel|Invert|DST to DST
                W|Fill|Fill with Color|Red
                SW|Copy|Copy Surface|SRC to WRK
            */
            string graphicName = listItemText.Substring(0, listItemText.IndexOf("|"));
            // This is to minimize the number of graphic files I had to include...
            if (graphicName.Length == 3)
            {
                string source = graphicName.Substring(0, 2);
                string destination = graphicName.Substring(2);
                if (source == "WS") source = "SW";
                if (source == "DW") source = "WD";
                if (source == "DS") source = "SD";
                if (source == "SS") source = "S";
                if (source == "WW") source = "W";
                if (source == "DD") source = "D";
                graphicName = source + destination;
                listItemText = listItemText.Substring(4);
            }
            else
            {
                listItemText = listItemText.Substring(graphicName.Length + 1);
            }
            string groupName = listItemText.Substring(0, listItemText.IndexOf("|"));
            listItemText = listItemText.Substring(groupName.Length + 1);
            string bigText = listItemText.Substring(0, listItemText.IndexOf("|"));
            string smallText = listItemText.Substring(listItemText.IndexOf("|") + 1);
            using (SolidBrush solidBrush = new SolidBrush(e.ForeColor))
            using (SolidBrush foreBrush = new SolidBrush(e.ForeColor))
            using (SolidBrush backBrush = new SolidBrush(Color.Gray))
            using (Font bigfont = new Font(e.Font.FontFamily, e.Font.Size * 2, FontStyle.Bold))
            using (Font smallfont = new Font(e.Font, FontStyle.Regular))
            {
                solidBrush.Color = Color.FromName(smallText);
                Image iconImage = ResUtil.GetImage(graphicName);
                e.Graphics.DrawImage(iconImage, new Rectangle(e.Bounds.X + 1, e.Bounds.Y + 1, e.Bounds.Height - 2, e.Bounds.Height - 2));

                if (groupName == "Fill")
                {
                    if ((smallText == "Transparent") || (smallText == "Primary") || (smallText == "Secondary") || (smallText == "User selected"))
                    {
                        string imageName = smallText == "User selected" ? "UserSelected" : smallText;
                        Image imageFill = ResUtil.GetImage(imageName);
                        e.Graphics.DrawImage(imageFill, e.Bounds.X + (int)(17 * UIfactor), e.Bounds.Y + (int)(16 * UIfactor), e.Bounds.Height - (int)(33 * UIfactor), e.Bounds.Height - (int)(34 * UIfactor));
                        if (smallText == "Transparent") smallText = "Clear " + ((graphicName[0] == 'W') ? "WRK" : "DST") + " surface";
                        if (smallText == "Primary") smallText = "Fill " + ((graphicName[0] == 'W') ? "WRK" : "DST") + " surface with Primary color";
                        if (smallText == "Secondary") smallText = "Fill " + ((graphicName[0] == 'W') ? "WRK" : "DST") + " surface with Secondary color";
                        if (smallText == "User selected") smallText = "Fill " + ((graphicName[0] == 'W') ? "WRK" : "DST") + " surface with User Selected color";
                    }
                    else
                    {
                        e.Graphics.FillRectangle(solidBrush, new Rectangle(e.Bounds.X + (int)(16 * UIfactor), e.Bounds.Y + (int)(16 * UIfactor), e.Bounds.Height - (int)(32 * UIfactor), e.Bounds.Height - (int)(34 * UIfactor)));
                        if (graphicName[0] == 'W')
                        {
                            smallText = "WRK with " + smallText;
                        }
                        else
                        {
                            smallText = "DST with " + smallText;
                        }
                    }
                }
                else if (groupName == "Copy")
                {
                    string fillName = "SRC";
                    if (graphicName[0] == 'W')
                    {
                        fillName = "WRK";
                    }
                    else if (graphicName[0] == 'D')
                    {
                        fillName = "DST";
                    }
                    if (graphicName[1] == 'W')
                    {
                        fillName += "WRK";
                    }
                    else
                    {
                        fillName += "DST";
                    }
                    Image imageFill = ResUtil.GetImage(fillName);
                    e.Graphics.DrawImage(imageFill, e.Bounds.X + (int)(16 * UIfactor), e.Bounds.Y + (int)(16 * UIfactor), e.Bounds.Height - (int)(32 * UIfactor), e.Bounds.Height - (int)(32 * UIfactor));
                }
                else if (groupName == "Blend")
                {
                    Image imageFill = ResUtil.GetImage("Blend");
                    e.Graphics.DrawImage(imageFill, e.Bounds.X + (int)(16 * UIfactor), e.Bounds.Y + (int)(16 * UIfactor), e.Bounds.Height - (int)(32 * UIfactor), e.Bounds.Height - (int)(32 * UIfactor));
                    bigText += " Blend";
                }
                else if (groupName == "Pixel Op")
                {
                    Image imageFill = ResUtil.GetImage(bigText);
                    e.Graphics.DrawImage(imageFill, e.Bounds.X + (int)(17 * UIfactor), e.Bounds.Y + (int)(16 * UIfactor), e.Bounds.Height - (int)(33 * UIfactor), e.Bounds.Height - (int)(34 * UIfactor));
                    bigText += " Pixels";
                }
                else // Effect
                {
                    Image imageFill = ResUtil.GetImage(getName(bigText));
                    e.Graphics.DrawImage(imageFill, e.Bounds.X + (int)(17 * UIfactor), e.Bounds.Y + (int)(15 * UIfactor), e.Bounds.Height - (int)(32 * UIfactor), e.Bounds.Height - (int)(32 * UIfactor));
                    bigText += " Effect";
                }

                e.Graphics.DrawString(bigText, bigfont, foreBrush, new Rectangle(e.Bounds.X + e.Bounds.Height, e.Bounds.Y + (int)(7 * UIfactor), e.Bounds.Width - e.Bounds.Height, e.Bounds.Height));
                e.Graphics.DrawString(smallText, smallfont, foreBrush, new Rectangle(e.Bounds.X + e.Bounds.Height + 2, e.Bounds.Y + (int)(bigfont.SizeInPoints / 72 * e.Graphics.DpiY) + (int)(14 * UIfactor), e.Bounds.Width - e.Bounds.Height, e.Bounds.Height));
            }
            e.DrawFocusRectangle();
        }

        private void MoveUp_Click(object sender, EventArgs e)
        {
            if (flowList.SelectedItems.Count == 0)
            {
                return;
            }
            int CurrentItem = flowList.SelectedIndex;
            if (CurrentItem > 0)
            {
                string TargetElement = flowList.Items[CurrentItem].ToString();
                flowList.Items.RemoveAt(CurrentItem);
                flowList.Items.Insert(CurrentItem - 1, TargetElement);
                flowList.SelectedIndex = CurrentItem - 1;
            }
        }

        private void MoveDown_Click(object sender, EventArgs e)
        {
            if (flowList.SelectedItems.Count == 0)
            {
                return;
            }
            int CurrentItem = flowList.SelectedIndex;
            if (CurrentItem >= 0 && CurrentItem < flowList.Items.Count - 1)
            {
                string TargetElement = flowList.Items[CurrentItem].ToString();
                flowList.Items.RemoveAt(CurrentItem);
                flowList.Items.Insert(CurrentItem + 1, TargetElement);
                flowList.SelectedIndex = CurrentItem + 1;
            }
        }

        private void Delete_Click(object sender, EventArgs e)
        {
            int CurrentItem = (flowList.SelectedItems.Count > 0) ? flowList.SelectedIndex : -1;
            if (CurrentItem > -1)
            {
                flowList.Items.RemoveAt(flowList.SelectedIndex);
            }
            if (CurrentItem >= flowList.Items.Count)
            {
                CurrentItem--;
            }
            flowList.SelectedIndex = CurrentItem;
        }

        private void flowList_Click(object sender, EventArgs e)
        {
            // on Ctrl+Click deselect selected item in the list
            if (ModifierKeys.HasFlag(Keys.Control))
            {
                flowList.SelectedIndex = -1;
                //catagoryBox.Text = "";
            }
        }

        private string assembleElement()
        {
            //[0] = Dest, Src Dest, Top Bottom Dest
            //[1] = Effect, Blend, Pixel, Fill, or Copy
            //[2] = Effect / Blend / Pixel Op Name
            //[3] = Fill color or comment
            // and added later:
            //[4] = Starting control name is AmountX

            // icon description
            string ret = "";
            if (sourceBox.Visible) ret += sourceBox.Text.Substring(0, 1);
            if (bottomBox.Visible) ret += bottomBox.Text.Substring(0, 1);
            ret += destinationBox.Text.Substring(0, 1); // always visible
            int iconLength = ret.Length;
            ret += "|";
            // catagory
            ret += categoryBox.Text;
            ret += "|";
            // big text
            switch (categoryBox.Text)
            {
                case "Effect":
                    ret += effectBox.Text;
                    break;
                case "Blend":
                    ret += blendBox.Text;
                    break;
                case "Pixel Op":
                    ret += pixelOpBox.Text;
                    break;
                case "Fill":
                    ret += "Fill with Color";
                    break;
                case "Copy":
                    ret += "Copy Surface";
                    break;
            }
            ret += "|";
            // small text
            if (categoryBox.Text == "Fill")
            {
                ret += DefaultColorComboBox.Text;
            }
            else
            {
                switch (iconLength)
                {
                    case 1:
                        ret += "Render to " + destinationBox.Text;
                        break;
                    case 2:
                        ret += sourceBox.Text + " to " + destinationBox.Text;
                        break;
                    case 3:
                        ret += sourceBox.Text + " and " + bottomBox.Text + " to " + destinationBox.Text;
                        break;
                }
            }
            return ret;
        }

        private void flowList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (flowList.SelectedItems.Count == 0)
            {
                //catagoryBox.Text = "";
                return;
            }
            string[] item = flowList.SelectedItem.ToString().Split('|');
            if (item.Length > 2)
            {
                categoryBox.Text = item[1];
            }
            if (item.Length > 3)
            {
                switch (item[1])
                {
                    /*
                        SW|Effect|Gaussian Blur|SRC to WRK
                        SWD|Blend|Darken|SRC and WRK to DST
                        DD|Pixel|Invert|DST to DST
                        W|Fill|Fill with Color|Red
                        SW|Copy|Copy Surface|SRC to WRK

                        // Code for each type:
                        Copy Surface: wrk.CopySurface(src);
                        Fill with color: wrk.Clear(ColorBgra.Gray);
                        PixelOp: desaturateOp.Apply(dst,src,rect);  invertOp.Apply(dst, src, rect);
                        BlendOp: multiplyOp.Apply(dst, lhs, rhs, rect);
                        Effect: blurEffect.SetRenderInfo(BlurParameters, new RenderArgs(dst), new RenderArgs(src));
                                blurEffect.Render(new Rectangle[1] {dst.Bounds},0,1); // this is how to do "rect" in PreRender()

                    */
                    case "Effect":
                        string effectName = item[2];
                        if (effectName.Contains(" ")) effectName = effectName.Substring(0, effectName.IndexOf(" "));
                        effectBox.SelectedIndex = effectBox.FindString(effectName);
                        break;
                    case "Blend":
                        string blendName = item[2];
                        if (blendName.Contains(" ")) blendName = blendName.Substring(0, blendName.IndexOf(" "));
                        blendBox.Text = blendName;
                        break;
                    case "Pixel Op":
                        string opName = item[2];
                        if (opName.Contains(" ")) opName = opName.Substring(0, opName.IndexOf(" "));
                        pixelOpBox.Text = opName;
                        break;
                    case "Fill":
                        string colorName = item[3];
                        if (colorName.Contains(" ")) colorName = colorName.Substring(0, colorName.IndexOf(" "));
                        DefaultColorComboBox.Text = colorName;
                        break;
                    case "Copy":
                        break;
                }
            }
            switch (item[0].Length)
            {
                case 1:
                    destinationBox.SelectedIndex = destinationBox.FindString(item[0][0].ToString());
                    break;
                case 2:
                    sourceBox.SelectedIndex = sourceBox.FindString(item[0][0].ToString());
                    destinationBox.SelectedIndex = destinationBox.FindString(item[0][1].ToString());
                    break;
                case 3:
                    sourceBox.SelectedIndex = sourceBox.FindString(item[0][0].ToString());
                    bottomBox.SelectedIndex = bottomBox.FindString(item[0][1].ToString());
                    destinationBox.SelectedIndex = destinationBox.FindString(item[0][2].ToString());
                    break;
            }
            updateScreen();
        }

        private void catagoryBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            updateScreen();
        }

        private void updateScreen()
        {
            sourceLabel.Text = "Source layer:";
            switch (categoryBox.Text)
            {
                case "Effect":
                    effectLabel.Visible = true;
                    effectBox.Visible = true;
                    bottomLabel.Visible = false;
                    bottomBox.Visible = false;
                    if (renderedEffects.Contains(effectBox.Text))
                    //if (effectBox.SelectedIndex < 4)
                    {
                        sourceLabel.Visible = false;
                        sourceBox.Visible = false;
                    }
                    else
                    {
                        sourceLabel.Visible = true;
                        sourceBox.Visible = true;
                    }
                    DefaultColorComboBox.Visible = false;
                    blendLabel.Visible = false;
                    blendBox.Visible = false;
                    pixelOpBox.Visible = false;
                    break;
                case "Blend":
                    effectLabel.Visible = false;
                    effectBox.Visible = false;
                    bottomLabel.Visible = true;
                    bottomBox.Visible = true;
                    sourceLabel.Visible = true;
                    sourceBox.Visible = true;
                    DefaultColorComboBox.Visible = false;
                    blendLabel.Visible = true;
                    blendBox.Visible = true;
                    pixelOpBox.Visible = false;
                    sourceLabel.Text = "Top layer:";
                    break;
                case "Pixel Op":
                    effectLabel.Visible = false;
                    effectBox.Visible = false;
                    bottomLabel.Visible = false;
                    bottomBox.Visible = false;
                    sourceLabel.Visible = true;
                    sourceBox.Visible = true;
                    DefaultColorComboBox.Visible = false;
                    blendLabel.Visible = false;
                    blendBox.Visible = false;
                    pixelOpBox.Visible = true;
                    break;
                case "Fill":
                    effectLabel.Visible = false;
                    effectBox.Visible = false;
                    bottomLabel.Visible = false;
                    bottomBox.Visible = false;
                    sourceLabel.Visible = false;
                    sourceBox.Visible = false;
                    DefaultColorComboBox.Visible = true;
                    blendLabel.Visible = false;
                    blendBox.Visible = false;
                    pixelOpBox.Visible = false;
                    break;
                case "Copy":
                    effectLabel.Visible = false;
                    effectBox.Visible = false;
                    bottomLabel.Visible = false;
                    bottomBox.Visible = false;
                    sourceLabel.Visible = true;
                    sourceBox.Visible = true;
                    DefaultColorComboBox.Visible = false;
                    blendLabel.Visible = false;
                    blendBox.Visible = false;
                    pixelOpBox.Visible = false;
                    break;
            }
        }

        private void effectBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            updateScreen();
        }

        private void DefaultColorComboBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index == -1)
            {
                return;
            }

            e.DrawBackground();
            string colorName = DefaultColorComboBox.Items[e.Index].ToString();

            using (SolidBrush solidBrush = new SolidBrush(e.ForeColor))
            using (Font font = new Font(e.Font, FontStyle.Regular))
            {
                if (colorName == "Transparent" || colorName == "Primary" || colorName == "Secondary" || colorName == "User selected")
                {
                    e.Graphics.DrawString(colorName, font, solidBrush, e.Bounds);
                }
                else
                {
                    solidBrush.Color = Color.FromName(colorName);
                    e.Graphics.FillRectangle(solidBrush, new Rectangle(e.Bounds.X + 1, e.Bounds.Y + 1, e.Bounds.Height - 2, e.Bounds.Height - 2));
                    e.Graphics.DrawString(colorName, font, solidBrush, new Rectangle(e.Bounds.X + e.Bounds.Height, e.Bounds.Y + 1, e.Bounds.Width - e.Bounds.Height, e.Bounds.Height));
                }
            }
            e.DrawFocusRectangle();
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            flowList.Items.Add(assembleElement());
            flowList.SelectedIndex = flowList.Items.Count - 1;
        }

        private void updateButton_Click(object sender, EventArgs e)
        {
            string element = assembleElement();
            if (flowList.SelectedItems.Count == 0)
            {
                flowList.Items.Add(element);
            }
            int CurrentItem = flowList.SelectedIndex;
            if (CurrentItem >= 0)
            {
                flowList.Items.RemoveAt(CurrentItem);
                flowList.Items.Insert(CurrentItem, element);
                flowList.SelectedIndex = CurrentItem;
            }
        }
    }
}
