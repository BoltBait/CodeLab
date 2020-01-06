using System;
using System.Windows.Forms;

namespace PaintDotNet.Effects
{
    internal partial class NewShape : ChildFormBase
    {
        internal string ShapeCode;

        internal NewShape()
        {
            InitializeComponent();
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
            "    DisplayName=\"Square\"\r\n" +
            "    Geometry=\"F0 \" />\r\n";

        private const string streamGeometrySample = "" +
            "<ps:SimpleGeometryShape\r\n" +
            "    xmlns=\"clr-namespace:PaintDotNet.UI.Media;assembly=PaintDotNet.Framework\"\r\n" +
            "    xmlns:ps=\"clr-namespace:PaintDotNet.Shapes;assembly=PaintDotNet.Framework\"\r\n" +
            "    DisplayName=\"Square\"\r\n" +
            "    Geometry=\"F0 M 0,0 L 100,0 L 100,100 L 0,100 Z\" />\r\n";

        private const string pathGeometry = "" +
            "<ps:SimpleGeometryShape\r\n" +
            "    xmlns=\"clr-namespace:PaintDotNet.UI.Media;assembly=PaintDotNet.Framework\"\r\n" +
            "    xmlns:ps=\"clr-namespace:PaintDotNet.Shapes;assembly=PaintDotNet.Framework\"\r\n" +
            "    DisplayName=\"Cube\">\r\n" +
            "    <PathGeometry FillRule=\"Nonzero\">\r\n\r\n" +
            "    </PathGeometry>\r\n" +
            "</ps:SimpleGeometryShape>\r\n";

        private const string pathGeometrySample = "" +
            "<ps:SimpleGeometryShape\r\n" +
            "    xmlns=\"clr-namespace:PaintDotNet.UI.Media;assembly=PaintDotNet.Framework\"\r\n" +
            "    xmlns:ps=\"clr-namespace:PaintDotNet.Shapes;assembly=PaintDotNet.Framework\"\r\n" +
            "    DisplayName=\"Cube\">\r\n" +
            "    <PathGeometry FillRule=\"Nonzero\">\r\n" +
            "        <PathFigure IsClosed=\"True\" IsFilled=\"True\" StartPoint=\"0,22\">\r\n" +
            "            <LineSegment Point=\"58,0\" IsSmoothJoin=\"True\" />\r\n" +
            "            <LineSegment Point=\"116,22\" IsSmoothJoin=\"True\" />\r\n" +
            "            <LineSegment Point=\"116,93\" IsSmoothJoin=\"True\" />\r\n" +
            "            <LineSegment Point=\"58,116\" IsSmoothJoin=\"True\" />\r\n" +
            "            <LineSegment Point=\"0,93\" IsSmoothJoin=\"True\" />\r\n" +
            "            <LineSegment Point=\"0,22\" IsSmoothJoin=\"True\" />\r\n" +
            "        </PathFigure>\r\n" +
            "        <PathFigure IsClosed=\"False\" IsFilled=\"False\" StartPoint=\"58,46\">\r\n" +
            "            <LineSegment Point=\"0,22\" />\r\n" +
            "        </PathFigure>\r\n" +
            "        <PathFigure IsClosed=\"False\" IsFilled=\"False\" StartPoint=\"58,46\">\r\n" +
            "            <LineSegment Point=\"58,116\" />\r\n" +
            "        </PathFigure>\r\n" +
            "        <PathFigure IsClosed=\"False\" IsFilled=\"False\" StartPoint=\"58,46\">\r\n" +
            "            <LineSegment Point=\"116,22\" />\r\n" +
            "        </PathFigure>\r\n" +
            "    </PathGeometry>\r\n" +
            "</ps:SimpleGeometryShape>\r\n";

        private const string geometryGroup = "" +
            "<ps:SimpleGeometryShape\r\n" +
            "    xmlns=\"clr-namespace:PaintDotNet.UI.Media;assembly=PaintDotNet.Framework\"\r\n" +
            "    xmlns:ps=\"clr-namespace:PaintDotNet.Shapes;assembly=PaintDotNet.Framework\"\r\n" +
            "    DisplayName=\"Rounded Rectangle\">\r\n" +
            "        <GeometryGroup>\r\n\r\n" +
            "        </GeometryGroup>\r\n" +
            "</ps:SimpleGeometryShape>\r\n";

        private const string geometryGroupSample = "" +
            "<ps:SimpleGeometryShape\r\n" +
            "    xmlns=\"clr-namespace:PaintDotNet.UI.Media;assembly=PaintDotNet.Framework\"\r\n" +
            "    xmlns:ps=\"clr-namespace:PaintDotNet.Shapes;assembly=PaintDotNet.Framework\"\r\n" +
            "    DisplayName=\"Rounded Rectangle\">\r\n" +
            "        <GeometryGroup>\r\n" +
            "            <RectangleGeometry Rect=\"0,0,100,160\" RadiusX=\"5\" RadiusY=\"5\" />\r\n" +
            "            <RectangleGeometry Rect=\"2,2,96,156\" RadiusX=\"5\" RadiusY=\"5\" />\r\n" +
            "        </GeometryGroup>\r\n" +
            "</ps:SimpleGeometryShape>\r\n";

        private void RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (sgRadioButton.Checked)
            {
                samplePicture.BackgroundImage = Properties.Resources.Shape0;
            }
            else if (pgRadioButton.Checked)
            {
                samplePicture.BackgroundImage = Properties.Resources.Shape1;
            }
            else
            {
                samplePicture.BackgroundImage = Properties.Resources.Shape2;
            }
        }
    }
}
