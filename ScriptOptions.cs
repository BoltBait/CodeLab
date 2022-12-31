using System;

namespace PaintDotNet.Effects
{
    internal enum ScriptRenderingSchedule
    {
        None,               // SingleRenderCall
        HorizontalStrips,   // Legacy
        SquareTiles,

        Default = SquareTiles
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
