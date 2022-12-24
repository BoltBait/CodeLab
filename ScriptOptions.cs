using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaintDotNet.Effects
{
    internal enum ScriptRenderingSchedule
    {
        None = 0,               // SingleRenderCall
        HorizontalStrips = 1,   // Legacy
        SquareTiles = 2         // Default
    }

    [Flags]
    internal enum ScriptRenderingFlags
    {
        None = 0,
        SingleThreaded = 1,
        NoSelectionClipping = 2,
        AliasedSelection = 4
    }

    internal enum HelpType
    {
        None,
        URL,
        PlainText,
        RichText,
        Custom
    }
}