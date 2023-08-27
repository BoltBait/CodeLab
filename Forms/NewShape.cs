using System;
using System.Windows.Forms;

namespace PdnCodeLab
{
    internal partial class NewShape : ChildFormBase
    {
        internal string ShapeCode;

        internal NewShape()
        {
            InitializeComponent();

            samplePicture.BackgroundImage = UIUtil.GetImage("Shape0", "Resources");
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            if (includeSample.Checked)
            {
                if (this.ggRadioButton.Checked)
                {
                    this.ShapeCode = geometryGroupSample;
                }
                else if (this.pgRadioButton.Checked)
                {
                    this.ShapeCode = pathGeometrySample;
                }
                else
                {
                    this.ShapeCode = streamGeometrySample;
                }
            }
            else
            {
                if (this.ggRadioButton.Checked)
                {
                    this.ShapeCode = geometryGroup;
                }
                else if (this.pgRadioButton.Checked)
                {
                    this.ShapeCode = pathGeometry;
                }
                else
                {
                    this.ShapeCode = streamGeometry;
                }
            }
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private const string streamGeometry = "" +
            "<ps:SimpleGeometryShape\r\n" +
            "    xmlns=\"clr-namespace:PaintDotNet.UI.Media;assembly=PaintDotNet.Framework\"\r\n" +
            "    xmlns:ps=\"clr-namespace:PaintDotNet.Shapes;assembly=PaintDotNet.Framework\"\r\n" +
            "    DisplayName=\"\"\r\n" +
            "    Geometry=\"F0 \" />\r\n";

        private const string streamGeometrySample = "" +
            "<ps:SimpleGeometryShape\r\n" +
            "    xmlns=\"clr-namespace:PaintDotNet.UI.Media;assembly=PaintDotNet.Framework\"\r\n" +
            "    xmlns:ps=\"clr-namespace:PaintDotNet.Shapes;assembly=PaintDotNet.Framework\"\r\n" +
            "    DisplayName=\"House\"\r\n" +
            "    Geometry=\"F1 M 21,142 L 160,22 L 300,142 L 300,318 L 21,318 Z\" />\r\n";

        private const string pathGeometry = "" +
            "<ps:SimpleGeometryShape\r\n" +
            "    xmlns=\"clr-namespace:PaintDotNet.UI.Media;assembly=PaintDotNet.Framework\"\r\n" +
            "    xmlns:ps=\"clr-namespace:PaintDotNet.Shapes;assembly=PaintDotNet.Framework\"\r\n" +
            "    DisplayName=\"\">\r\n" +
            "    <PathGeometry FillRule=\"Nonzero\">\r\n\r\n" +
            "    </PathGeometry>\r\n" +
            "</ps:SimpleGeometryShape>\r\n";

        private const string pathGeometrySample = "" +
            "<ps:SimpleGeometryShape\r\n" +
            "    xmlns=\"clr-namespace:PaintDotNet.UI.Media;assembly=PaintDotNet.Framework\"\r\n" +
            "    xmlns:ps=\"clr-namespace:PaintDotNet.Shapes;assembly=PaintDotNet.Framework\"\r\n" +
            "    DisplayName=\"House\">\r\n" +
            "    <PathGeometry FillRule=\"Nonzero\">\r\n" +
            "        <PathFigure IsClosed=\"True\" IsFilled=\"True\" StartPoint=\"21,142\">\r\n" +
            "            <LineSegment Point=\"160,22\" IsSmoothJoin=\"True\" />\r\n" +
            "            <LineSegment Point=\"300,142\" IsSmoothJoin=\"True\" />\r\n" +
            "            <LineSegment Point=\"300,318\" IsSmoothJoin=\"True\" />\r\n" +
            "            <LineSegment Point=\"21,318\" IsSmoothJoin=\"True\" />\r\n" +
            "        </PathFigure>\r\n" +
            "    </PathGeometry>\r\n" +
            "</ps:SimpleGeometryShape>\r\n";

        private const string geometryGroup = "" +
            "<ps:SimpleGeometryShape\r\n" +
            "    xmlns=\"clr-namespace:PaintDotNet.UI.Media;assembly=PaintDotNet.Framework\"\r\n" +
            "    xmlns:ps=\"clr-namespace:PaintDotNet.Shapes;assembly=PaintDotNet.Framework\"\r\n" +
            "    DisplayName=\"\">\r\n" +
            "        <GeometryGroup>\r\n\r\n" +
            "        </GeometryGroup>\r\n" +
            "</ps:SimpleGeometryShape>\r\n";

        private const string geometryGroupSample = "" +
            "<ps:SimpleGeometryShape\r\n" +
            "    xmlns=\"clr-namespace:PaintDotNet.UI.Media;assembly=PaintDotNet.Framework\"\r\n" +
            "    xmlns:ps=\"clr-namespace:PaintDotNet.Shapes;assembly=PaintDotNet.Framework\"\r\n" +
            "    DisplayName=\"Cell Phone\">\r\n" +
            "        <GeometryGroup>\r\n" +
            "            <RectangleGeometry Rect=\"0,0,100,160\" RadiusX=\"5\" RadiusY=\"5\" />\r\n" +
            "            <RectangleGeometry Rect=\"2,2,96,156\" RadiusX=\"5\" RadiusY=\"5\" />\r\n" +
            "            <RectangleGeometry Rect=\"42,5,16,3\" RadiusX=\"2\" RadiusY=\"2\" />\r\n" +
            "            <EllipseGeometry Center=\"50,150\" RadiusX=\"6\" RadiusY=\"6\" />\r\n" +
            "            <PathGeometry Figures=\"M 20,72 L 50,52 L 80,72 L 80,110 L 20,110 Z\" />\r\n" +
            "        </GeometryGroup>\r\n" +
            "</ps:SimpleGeometryShape>\r\n";

        private void RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (sgRadioButton.Checked)
            {
                samplePicture.BackgroundImage = UIUtil.GetImage("Shape0", "Resources");
            }
            else if (pgRadioButton.Checked)
            {
                samplePicture.BackgroundImage = UIUtil.GetImage("Shape1", "Resources");
            }
            else
            {
                samplePicture.BackgroundImage = UIUtil.GetImage("Shape2", "Resources");
            }
        }
    }
}
