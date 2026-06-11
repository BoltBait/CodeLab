using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Windows.Forms;

namespace PdnCodeLab;

public class RtfHelpEditor : RichTextBox
{
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    internal string RtfCompressed => CompressString(this.Rtf);

    internal bool LoadFileFromPath(string filePath)
    {
        if (!File.Exists(filePath))
        {
            return false;
        }

        string fileContents;

        try
        {
            fileContents = File.ReadAllText(filePath);
        }
        catch
        {
            return false;
        }

        this.ResetText();

        string fileExtension = Path.GetExtension(filePath);

        if (fileExtension.Equals(".rtf", StringComparison.OrdinalIgnoreCase))
        {
            this.Rtf = fileContents;
        }
        else if (fileExtension.Equals(".rtz", StringComparison.OrdinalIgnoreCase))
        {
            this.Rtf = DecompressString(fileContents);
        }
        else if (fileExtension.Equals(".txt", StringComparison.OrdinalIgnoreCase))
        {
            this.Text = fileContents;
            ChangeUBBtoRTF();
        }

        return true;
    }

    #region Compression
    private static string CompressString(string text)
    {
        byte[] buffer = Encoding.UTF8.GetBytes(text);
        using MemoryStream memoryStream = new MemoryStream();
        using (GZipStream gZipStream = new GZipStream(memoryStream, CompressionMode.Compress, true))
        {
            gZipStream.Write(buffer, 0, buffer.Length);
        }

        memoryStream.Position = 0;

        byte[] compressedData = new byte[memoryStream.Length];
        memoryStream.ReadExactly(compressedData, 0, compressedData.Length);

        byte[] gZipBuffer = new byte[compressedData.Length + 4];
        Buffer.BlockCopy(compressedData, 0, gZipBuffer, 4, compressedData.Length);
        Buffer.BlockCopy(BitConverter.GetBytes(buffer.Length), 0, gZipBuffer, 0, 4);
        return Convert.ToBase64String(gZipBuffer);
    }

    private static string DecompressString(string compressedText)
    {
        byte[] gZipBuffer = Convert.FromBase64String(compressedText);
        int dataLength = BitConverter.ToInt32(gZipBuffer, 0);

        using MemoryStream memoryStream = new MemoryStream();
        memoryStream.Write(gZipBuffer, 4, gZipBuffer.Length - 4);
        memoryStream.Position = 0;

        byte[] buffer = new byte[dataLength];

        using (GZipStream gZipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
        {
            gZipStream.ReadExactly(buffer, 0, buffer.Length);
        }

        return Encoding.UTF8.GetString(buffer);
    }
    #endregion

    #region RTF Editor functions
    internal void InsertImage(Image image)
    {
        IDataObject existingItem = Clipboard.GetDataObject();
        Clipboard.Clear();

        Clipboard.SetImage(image);
        this.Paste();

        Clipboard.Clear();
        Clipboard.SetDataObject(existingItem);
    }

    internal void ApplyColor()
    {
        using ColorWindow colorDialog1 = new ColorWindow
        {
            Color = Color.FromArgb(255, this.SelectionColor),
            ShowAlpha = false
        };

        if (colorDialog1.ShowDialog() == DialogResult.OK && colorDialog1.Color != this.SelectionColor)
        {
            this.SelectionColor = colorDialog1.Color;
        }
    }

    internal void ToggleBold()
    {
        FontStyle newBoldStyle = this.SelectionFont.Bold
            ? this.SelectionFont.Style ^ FontStyle.Bold
            : this.SelectionFont.Style | FontStyle.Bold;

        this.SelectionFont = new Font(this.SelectionFont, newBoldStyle);
    }

    internal void ToggleItalics()
    {
        FontStyle newItalicsStyle = this.SelectionFont.Italic
            ? this.SelectionFont.Style ^ FontStyle.Italic
            : this.SelectionFont.Style | FontStyle.Italic;

        this.SelectionFont = new Font(this.SelectionFont, newItalicsStyle);
    }

    internal void ToggleUnderline()
    {
        FontStyle newUnderlineStyle = this.SelectionFont.Underline
            ? this.SelectionFont.Style ^ FontStyle.Underline
            : this.SelectionFont.Style | FontStyle.Underline;

        this.SelectionFont = new Font(this.SelectionFont, newUnderlineStyle);
    }

    internal void ToggleSuperscript()
    {
        this.SelectionCharOffset = (this.SelectionCharOffset == 0) ? 5 : 0;
    }

    internal void ToggleSubscript()
    {
        this.SelectionCharOffset = (this.SelectionCharOffset == 0) ? -5 : 0;
    }

    internal void IncreaseFontSize()
    {
        float newFontSize = float.Min(this.SelectionFont.SizeInPoints + 2, 72);
        this.SelectionFont = new Font(this.SelectionFont.Name, newFontSize);
    }

    internal void DecreaseFontSize()
    {
        float newFontSize = float.Max(this.SelectionFont.SizeInPoints - 2, 2);
        this.SelectionFont = new Font(this.SelectionFont.Name, newFontSize);
    }

    internal void ToggleBullet()
    {
        this.SelectionBullet = !this.SelectionBullet;
    }

    internal void Indent()
    {
        this.SelectionIndent += 20;
    }

    internal void Unindent()
    {
        this.SelectionIndent -= 20;
    }

    internal void AlginLeft()
    {
        this.SelectionAlignment = HorizontalAlignment.Left;
    }

    internal void AlignCenter()
    {
        this.SelectionAlignment = HorizontalAlignment.Center;
    }
    #endregion

    #region UBB to RTF
    private enum StyleTypes
    {
        Style,
        Color,
        BackColor,
        Indent,
        Alignment,
        Size,
        Baseline
    }

    private void rtb_FindMatchingUBBPair(string OpenUBBcode, FontStyle NewFontStyle, Color NewColor, StyleTypes NewStyleType, float NewSize, int NewBaseLineDirection, ref int FirstCodeLocation, ref int FirstEndLocation, ref FontStyle FirstStyle, ref Color FirstColor, ref float FirstSize, ref int FirstBaselineDirection, ref StyleTypes FirstStyleType, ref int FirstOpenCodeLength)
    {
        int OpenCodePosition = this.Find(OpenUBBcode);
        int CloseCodePosition = this.Find(OpenUBBcode.Insert(1, "/"), int.Max(OpenCodePosition, 0), RichTextBoxFinds.NoHighlight);
        if ((OpenCodePosition != -1) && (CloseCodePosition != -1) && (OpenCodePosition < FirstCodeLocation))
        {
            FirstCodeLocation = OpenCodePosition;
            FirstEndLocation = CloseCodePosition;
            FirstStyle = NewFontStyle;
            FirstColor = NewColor;
            FirstSize = NewSize;
            FirstBaselineDirection = NewBaseLineDirection;
            FirstStyleType = NewStyleType;
            FirstOpenCodeLength = OpenUBBcode.Length;
        }
    }

    private void ChangeUBBtoRTF()
    {
        int EarliestTagFound = int.MaxValue;
        int MatchingEndTag = 0;
        FontStyle StyleToApply = FontStyle.Regular;
        StyleTypes StyleTypeToApply = StyleTypes.Style;
        int OpenCodeLength = 0;
        Color ColorToApply = Color.Black;
        float SizeToApply = 10f;
        int NewBaselineDirection = 0;
        this.SelectAll();
        this.SelectionIndent = 10;
        this.SelectionRightIndent = 10;
        this.Select(0, 0);
        this.SelectionFont = new Font(this.SelectionFont.Name, 5f, this.SelectionFont.Style);
        this.SelectedText = "\n";
        do
        {
            EarliestTagFound = int.MaxValue;
            rtb_FindMatchingUBBPair("[b]", FontStyle.Bold, Color.Black, StyleTypes.Style, 10f, 0, ref EarliestTagFound, ref MatchingEndTag, ref StyleToApply, ref ColorToApply, ref SizeToApply, ref NewBaselineDirection, ref StyleTypeToApply, ref OpenCodeLength);
            rtb_FindMatchingUBBPair("[i]", FontStyle.Italic, Color.Black, StyleTypes.Style, 10f, 0, ref EarliestTagFound, ref MatchingEndTag, ref StyleToApply, ref ColorToApply, ref SizeToApply, ref NewBaselineDirection, ref StyleTypeToApply, ref OpenCodeLength);
            rtb_FindMatchingUBBPair("[u]", FontStyle.Underline, Color.Black, StyleTypes.Style, 10f, 0, ref EarliestTagFound, ref MatchingEndTag, ref StyleToApply, ref ColorToApply, ref SizeToApply, ref NewBaselineDirection, ref StyleTypeToApply, ref OpenCodeLength);
            rtb_FindMatchingUBBPair("[s]", FontStyle.Strikeout, Color.Black, StyleTypes.Style, 10f, 0, ref EarliestTagFound, ref MatchingEndTag, ref StyleToApply, ref ColorToApply, ref SizeToApply, ref NewBaselineDirection, ref StyleTypeToApply, ref OpenCodeLength);
            rtb_FindMatchingUBBPair("[red]", FontStyle.Regular, Color.Red, StyleTypes.Color, 10f, 0, ref EarliestTagFound, ref MatchingEndTag, ref StyleToApply, ref ColorToApply, ref SizeToApply, ref NewBaselineDirection, ref StyleTypeToApply, ref OpenCodeLength);
            rtb_FindMatchingUBBPair("[blue]", FontStyle.Regular, Color.Blue, StyleTypes.Color, 10f, 0, ref EarliestTagFound, ref MatchingEndTag, ref StyleToApply, ref ColorToApply, ref SizeToApply, ref NewBaselineDirection, ref StyleTypeToApply, ref OpenCodeLength);
            rtb_FindMatchingUBBPair("[cyan]", FontStyle.Regular, Color.Cyan, StyleTypes.Color, 10f, 0, ref EarliestTagFound, ref MatchingEndTag, ref StyleToApply, ref ColorToApply, ref SizeToApply, ref NewBaselineDirection, ref StyleTypeToApply, ref OpenCodeLength);
            rtb_FindMatchingUBBPair("[green]", FontStyle.Regular, Color.Green, StyleTypes.Color, 10f, 0, ref EarliestTagFound, ref MatchingEndTag, ref StyleToApply, ref ColorToApply, ref SizeToApply, ref NewBaselineDirection, ref StyleTypeToApply, ref OpenCodeLength);
            rtb_FindMatchingUBBPair("[brown]", FontStyle.Regular, Color.Chocolate, StyleTypes.Color, 10f, 0, ref EarliestTagFound, ref MatchingEndTag, ref StyleToApply, ref ColorToApply, ref SizeToApply, ref NewBaselineDirection, ref StyleTypeToApply, ref OpenCodeLength);
            rtb_FindMatchingUBBPair("[white]", FontStyle.Regular, Color.White, StyleTypes.Color, 10f, 0, ref EarliestTagFound, ref MatchingEndTag, ref StyleToApply, ref ColorToApply, ref SizeToApply, ref NewBaselineDirection, ref StyleTypeToApply, ref OpenCodeLength);
            rtb_FindMatchingUBBPair("[yellow]", FontStyle.Regular, Color.Gold, StyleTypes.Color, 10f, 0, ref EarliestTagFound, ref MatchingEndTag, ref StyleToApply, ref ColorToApply, ref SizeToApply, ref NewBaselineDirection, ref StyleTypeToApply, ref OpenCodeLength);
            rtb_FindMatchingUBBPair("[purple]", FontStyle.Regular, Color.Purple, StyleTypes.Color, 10f, 0, ref EarliestTagFound, ref MatchingEndTag, ref StyleToApply, ref ColorToApply, ref SizeToApply, ref NewBaselineDirection, ref StyleTypeToApply, ref OpenCodeLength);
            rtb_FindMatchingUBBPair("[orange]", FontStyle.Regular, Color.DarkOrange, StyleTypes.Color, 10f, 0, ref EarliestTagFound, ref MatchingEndTag, ref StyleToApply, ref ColorToApply, ref SizeToApply, ref NewBaselineDirection, ref StyleTypeToApply, ref OpenCodeLength);
            rtb_FindMatchingUBBPair("[silver]", FontStyle.Regular, Color.Silver, StyleTypes.Color, 10f, 0, ref EarliestTagFound, ref MatchingEndTag, ref StyleToApply, ref ColorToApply, ref SizeToApply, ref NewBaselineDirection, ref StyleTypeToApply, ref OpenCodeLength);
            rtb_FindMatchingUBBPair("[sharpie]", FontStyle.Regular, Color.Black, StyleTypes.BackColor, 10f, 0, ref EarliestTagFound, ref MatchingEndTag, ref StyleToApply, ref ColorToApply, ref SizeToApply, ref NewBaselineDirection, ref StyleTypeToApply, ref OpenCodeLength);
            rtb_FindMatchingUBBPair("[highlighter]", FontStyle.Regular, Color.Gold, StyleTypes.BackColor, 10f, 0, ref EarliestTagFound, ref MatchingEndTag, ref StyleToApply, ref ColorToApply, ref SizeToApply, ref NewBaselineDirection, ref StyleTypeToApply, ref OpenCodeLength);
            rtb_FindMatchingUBBPair("[indent]", FontStyle.Regular, Color.Black, StyleTypes.Indent, 10f, 0, ref EarliestTagFound, ref MatchingEndTag, ref StyleToApply, ref ColorToApply, ref SizeToApply, ref NewBaselineDirection, ref StyleTypeToApply, ref OpenCodeLength);
            rtb_FindMatchingUBBPair("[center]", FontStyle.Regular, Color.Black, StyleTypes.Alignment, 10f, 0, ref EarliestTagFound, ref MatchingEndTag, ref StyleToApply, ref ColorToApply, ref SizeToApply, ref NewBaselineDirection, ref StyleTypeToApply, ref OpenCodeLength);
            rtb_FindMatchingUBBPair("[small]", FontStyle.Regular, Color.Black, StyleTypes.Size, 7.5f, 0, ref EarliestTagFound, ref MatchingEndTag, ref StyleToApply, ref ColorToApply, ref SizeToApply, ref NewBaselineDirection, ref StyleTypeToApply, ref OpenCodeLength);
            rtb_FindMatchingUBBPair("[big]", FontStyle.Regular, Color.Black, StyleTypes.Size, 13f, 0, ref EarliestTagFound, ref MatchingEndTag, ref StyleToApply, ref ColorToApply, ref SizeToApply, ref NewBaselineDirection, ref StyleTypeToApply, ref OpenCodeLength);
            rtb_FindMatchingUBBPair("[sup]", FontStyle.Regular, Color.Black, StyleTypes.Baseline, 7.5f, 1, ref EarliestTagFound, ref MatchingEndTag, ref StyleToApply, ref ColorToApply, ref SizeToApply, ref NewBaselineDirection, ref StyleTypeToApply, ref OpenCodeLength);
            rtb_FindMatchingUBBPair("[sub]", FontStyle.Regular, Color.Black, StyleTypes.Baseline, 7.5f, -1, ref EarliestTagFound, ref MatchingEndTag, ref StyleToApply, ref ColorToApply, ref SizeToApply, ref NewBaselineDirection, ref StyleTypeToApply, ref OpenCodeLength);
            if (EarliestTagFound < int.MaxValue)
            {
                this.Select(EarliestTagFound, MatchingEndTag - EarliestTagFound);
                switch (StyleTypeToApply)
                {
                    case StyleTypes.Style:
                        this.SelectionFont = new Font(this.SelectionFont, this.SelectionFont.Style | StyleToApply);
                        break;
                    case StyleTypes.Color:
                        this.SelectionColor = ColorToApply;
                        break;
                    case StyleTypes.BackColor:
                        this.SelectionBackColor = ColorToApply;
                        break;
                    case StyleTypes.Indent:
                        this.SelectionIndent += 20;
                        break;
                    case StyleTypes.Alignment:
                        this.SelectionAlignment = HorizontalAlignment.Center;
                        break;
                    case StyleTypes.Size:
                        this.SelectionFont = new Font(this.SelectionFont.Name, SizeToApply, this.SelectionFont.Style);
                        break;
                    case StyleTypes.Baseline:
                        this.SelectionCharOffset = this.SelectionFont.Height / 3 * NewBaselineDirection;
                        this.SelectionFont = new Font(this.SelectionFont.Name, float.Max(this.SelectionFont.Size * 0.75f, SizeToApply), this.SelectionFont.Style);
                        break;
                    default:
                        break;
                }
                this.Select(MatchingEndTag, OpenCodeLength + 1);
                this.SelectedText = "";
                this.Select(EarliestTagFound, OpenCodeLength);
                this.SelectedText = "";
            }
        } while (EarliestTagFound < int.MaxValue);
        int findt = this.Find("[t]");
        while (findt > -1)
        {
            this.Select(findt, 3);
            this.SelectedText = "\\t";
            findt = this.Find("[t]");
        }
        int findn = this.Find("[n]");
        while (findn > -1)
        {
            this.Select(findn, 3);
            this.SelectedText = "\\n";
            findn = this.Find("[n]");
        }
        this.Select(0, 0);
    }
    #endregion
}
