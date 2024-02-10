using System;
using System.Text.RegularExpressions;

namespace PdnCodeLab
{
    public enum ProjectType
    {
        None,
        ClassicEffect,
        GpuImageEffect,
        GpuDrawEffect,
        BitmapEffect,
        FileType,
        Reference,
        Shape,

        Default = GpuImageEffect
    }

    internal static class ProjectTypeExtensions
    {
        internal static bool IsCSharp(this ProjectType projectType)
        {
            return
                projectType == ProjectType.ClassicEffect ||
                projectType == ProjectType.GpuImageEffect ||
                projectType == ProjectType.GpuDrawEffect ||
                projectType == ProjectType.BitmapEffect ||
                projectType == ProjectType.FileType;
        }

        internal static bool IsEffect(this ProjectType projectType)
        {
            return
                projectType == ProjectType.ClassicEffect ||
                projectType == ProjectType.GpuImageEffect ||
                projectType == ProjectType.GpuDrawEffect ||
                projectType == ProjectType.BitmapEffect;
        }

        internal static bool Is5Effect(this ProjectType projectType)
        {
            return
                projectType == ProjectType.GpuImageEffect ||
                projectType == ProjectType.GpuDrawEffect ||
                projectType == ProjectType.BitmapEffect;
        }
    }

    internal static class ProjectTypeUtil
    {
        internal static ProjectType FromContents(string textContents, string fileExtension)
        {
            ProjectType projectType = FromContentImpl(textContents);

            if (fileExtension is null ||
                projectType == ProjectType.None ||
                (fileExtension.Equals(".cs", StringComparison.OrdinalIgnoreCase) && projectType.IsCSharp()) ||
                (fileExtension.Equals(".xaml", StringComparison.OrdinalIgnoreCase) && projectType == ProjectType.Shape))
            {
                return projectType;
            }

            return ProjectType.None;
        }

        private static ProjectType FromContentImpl(string textContents)
        {
            if (Regex.IsMatch(textContents, @"void\s+Render\s*\(\s*Surface\s+dst\s*,\s*Surface\s+src\s*,\s*Rectangle\s+rect\s*\)\s*{(.|\s)*}", RegexOptions.Singleline))
            {
                return ProjectType.ClassicEffect;
            }

            if (Regex.IsMatch(textContents, @"protected\s+override\s+void\s+OnRender\s*\(\s*IBitmapEffectOutput\s+output\s*\)\s*{(.|\s)*}", RegexOptions.Singleline))
            {
                return ProjectType.BitmapEffect;
            }

            if (Regex.IsMatch(textContents, @"protected\s+override\s+IDeviceImage\s+OnCreateOutput\(IDeviceContext\s+deviceContext\s*\)\s*{(.|\s)*}", RegexOptions.Singleline))
            {
                return ProjectType.GpuImageEffect;
            }

            if (Regex.IsMatch(textContents, @"protected\s+override\s+unsafe\s+void\s+OnDraw\(IDeviceContext\s+deviceContext\s*\)\s*{(.|\s)*}", RegexOptions.Singleline))
            {
                return ProjectType.GpuDrawEffect;
            }

            if (Regex.IsMatch(textContents, @"void\s+SaveImage\s*\(\s*Document\s+input\s*,\s*Stream\s+output\s*,\s*PropertyBasedSaveConfigToken\s+token\s*,\s*Surface\s+scratchSurface\s*,\s*ProgressEventHandler\s+progressCallback\s*\)\s*{(.|\s)*}", RegexOptions.Singleline))
            {
                return ProjectType.FileType;
            }

            if (Regex.IsMatch(textContents, @"<ps:SimpleGeometryShape\s+xmlns=""clr-namespace:PaintDotNet\.UI\.Media;assembly=PaintDotNet\.Framework""\s+xmlns:ps=""clr-namespace:PaintDotNet\.Shapes;assembly=PaintDotNet\.Framework""", RegexOptions.Singleline))
            {
                return ProjectType.Shape;
            }

            return ProjectType.None;
        }
    }
}
