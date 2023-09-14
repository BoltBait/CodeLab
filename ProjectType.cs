using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PdnCodeLab
{
    public enum ProjectType
    {
        None,
        ClassicEffect,
        GpuEffect,
        GpuDrawEffect,
        BitmapEffect,
        FileType,
        Reference,
        Shape,

        Default = ClassicEffect
    }

    internal static class ProjectTypeExtensions
    {
        internal static bool IsCSharp(this ProjectType projectType)
        {
            return
                projectType == ProjectType.ClassicEffect ||
                projectType == ProjectType.GpuEffect ||
                projectType == ProjectType.GpuDrawEffect ||
                projectType == ProjectType.BitmapEffect ||
                projectType == ProjectType.FileType;
        }

        internal static bool IsEffect(this ProjectType projectType)
        {
            return
                projectType == ProjectType.ClassicEffect ||
                projectType == ProjectType.GpuEffect ||
                projectType == ProjectType.GpuDrawEffect ||
                projectType == ProjectType.BitmapEffect;
        }

        internal static bool Is5Effect(this ProjectType projectType)
        {
            return
                projectType == ProjectType.GpuEffect ||
                projectType == ProjectType.GpuDrawEffect ||
                projectType == ProjectType.BitmapEffect;
        }
    }
}
