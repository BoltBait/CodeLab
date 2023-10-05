using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PdnCodeLab
{
    public enum RenderPreset
    {
        Regular,
        LegacyROI,
        AliasedSelection,
        SingleRenderCall,
        NoSelectionClip,
        UserDefined
    }

    internal static class RenderPresetExtensions
    {
        internal static string GetExTitle(this RenderPreset launchOption)
        {
            return launchOption switch
            {
                RenderPreset.Regular => string.Empty,
                RenderPreset.LegacyROI => " (Legacy ROI)",
                RenderPreset.AliasedSelection => " (Aliased Selection)",
                RenderPreset.SingleRenderCall => " (Single Render Call)",
                RenderPreset.NoSelectionClip => " (No Selection Clip)",
                RenderPreset.UserDefined => " (User Defined)",
                _ => string.Empty,
            };
        }

        internal static string GetName(this RenderPreset launchOption)
        {
            return launchOption switch
            {
                RenderPreset.Regular => "Regular",
                RenderPreset.LegacyROI => "Legacy ROI",
                RenderPreset.AliasedSelection => "Aliased Selection",
                RenderPreset.SingleRenderCall => "Single Render Call",
                RenderPreset.NoSelectionClip => "No Selection Clip",
                RenderPreset.UserDefined => "User Defined",
                _ => string.Empty,
            };
        }
    }
}
