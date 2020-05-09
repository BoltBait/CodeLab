// This file contains "patches" to Scintilla.NET that are not yet available in the NuGet release.
// To be deleted in the future. NOTE: The "Allow Unsafe Code" option can be disabled when this file is deleted.

// Substyles --- https://github.com/jacobslusser/ScintillaNET/pull/443
// CaretLineVisibleAlways --- https://github.com/jacobslusser/ScintillaNET/pull/389
// InternalFocusFlag --- https://github.com/jacobslusser/ScintillaNET/pull/448
// DocLineFromVisible Bugfix https://github.com/jacobslusser/ScintillaNET/pull/480

using System;
using System.Text;

namespace PaintDotNet.Effects
{
    public partial class CodeTextBox
    {
        internal bool InternalFocusFlag
        {
            get
            {
                return (DirectMessage(2381, IntPtr.Zero, IntPtr.Zero) != IntPtr.Zero);
            }
            set
            {
                var focus = (value ? new IntPtr(1) : IntPtr.Zero);
                DirectMessage(2380, focus, IntPtr.Zero);
            }
        }

        internal bool CaretLineVisibleAlways
        {
            get
            {
                return (DirectMessage(2654, IntPtr.Zero, IntPtr.Zero) != IntPtr.Zero);
            }
            set
            {
                var visibleAlways = (value ? new IntPtr(1) : IntPtr.Zero);
                DirectMessage(2655, visibleAlways, IntPtr.Zero);
            }
        }

        internal int AllocateSubstyles(int styleBase, int numberStyles)
        {
            return this.DirectMessage(4020, new IntPtr(styleBase), new IntPtr(numberStyles)).ToInt32();
        }

        internal unsafe void SetIdentifiers(int style, string identifiers)
        {
            int baseStyle = GetStyleFromSubstyle(style);
            int min = GetSubstylesStart(baseStyle);
            int length = GetSubstylesLength(baseStyle);
            int max = (length > 0) ? min + length - 1 : min;

            style = style.Clamp(min, max);
            byte[] bytes = GetBytes(identifiers ?? string.Empty, Encoding.ASCII, true);

            fixed (byte* bp = bytes)
            {
                DirectMessage(4024, new IntPtr(style), new IntPtr(bp));
            }
        }

        internal int GetStyleFromSubstyle(int subStyle)
        {
            return DirectMessage(4027, new IntPtr(subStyle), IntPtr.Zero).ToInt32();
        }

        internal int GetSubstylesLength(int styleBase)
        {
            return DirectMessage(4022, new IntPtr(styleBase), IntPtr.Zero).ToInt32();
        }

        internal int GetSubstylesStart(int styleBase)
        {
            return DirectMessage(4021, new IntPtr(styleBase), IntPtr.Zero).ToInt32();
        }

        internal new int DocLineFromVisible(int displayLine)
        {
            return DirectMessage(2221, new IntPtr(displayLine), IntPtr.Zero).ToInt32();
        }

        // Copied from Scinilla.NET's internal Helper class
        private static unsafe byte[] GetBytes(string text, Encoding encoding, bool zeroTerminated)
        {
            if (string.IsNullOrEmpty(text))
            {
                return (zeroTerminated ? new byte[] { 0 } : new byte[0]);
            }

            int count = encoding.GetByteCount(text);
            byte[] buffer = new byte[count + (zeroTerminated ? 1 : 0)];

            fixed (byte* bp = buffer)
            fixed (char* ch = text)
            {
                encoding.GetBytes(ch, text.Length, bp, count);
            }

            if (zeroTerminated)
            {
                buffer[buffer.Length - 1] = 0;
            }

            return buffer;
        }
    }
}
