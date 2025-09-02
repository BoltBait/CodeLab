using PaintDotNet.DirectWrite;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PdnCodeLab
{
    internal static class FontUtil
    {
        internal static IReadOnlyCollection<string> FontList { get; } = BuildFontList();

        private static IReadOnlyCollection<string> BuildFontList()
        {
            List<string> installedMonoFonts = new List<string>();
            using IDirectWriteFactory dwFactory = DirectWriteFactory.CreateRef();
            using IFontCollection fontCollection = dwFactory.GetSystemFontCollection(false);
            for (int familyIndex = 0; familyIndex < fontCollection.FontFamilyCount; familyIndex++)
            {
                using IFontFamily fontFamily = fontCollection.GetFontFamily(familyIndex);
                for (int fontIndex = 0; fontIndex < fontFamily.FontCount; fontIndex++)
                {
                    using IFont font = fontFamily.GetFont(fontIndex);
                    if (font.IsMonospacedFont)
                    {
                        using ILocalizedStrings langTags = font.TryGetInformationalStrings(InformationalStringID.DesignScriptLanguageTag);
                        if (langTags != null)
                        {
                            for (int i = 0; i < langTags.Count; i++)
                            {
                                if (langTags.GetString(i) == "Latn")
                                {
                                    installedMonoFonts.Add(fontFamily.GetFamilyName());
                                    break;
                                }
                            }
                        }
                        else
                        {
                            installedMonoFonts.Add(fontFamily.GetFamilyName());
                        }

                        break;
                    }
                }
            }

            string[] recommendedFonts = ["Cascadia Code", "Consolas", "Courier New", "Envy Code R", "Fira Code", "Hack", "JetBrains Mono", "Fake Font"];

            return recommendedFonts
                .Except(installedMonoFonts, StringComparer.Ordinal)
                .Select(x => x + '*')
                .Concat(installedMonoFonts)
                .Append("Verdana")
                .Order(StringComparer.OrdinalIgnoreCase)
                .ToList()
                .AsReadOnly();
        }

        internal static bool IsFontInstalled(string fontName)
        {
            return FontList.Contains(fontName, StringComparer.Ordinal);
        }
    }
}
