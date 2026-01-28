using LegioSoft.Imaging.Core.Enums;

using System;
using System.Text;

namespace LegioSoft.Imaging.Core.Classes;

public static class FormatDetector
{
    public static LegioImageFormat DetectFormat(byte[] imageData)
    {
        ArgumentNullException.ThrowIfNull(imageData);

        if (imageData.Length == 0)
            throw new ArgumentException("Image data cannot be empty", nameof(imageData));

        if (imageData.Length < 3)
            throw new ArgumentException("Image data is too short to determine format (minimum 3 bytes required)",
                nameof(imageData));

        return IdentifyFormatBySignature(imageData);
    }

    public static LegioImageFormat DetectFormat(Stream stream)
    {
        ArgumentNullException.ThrowIfNull(stream);

        if (!stream.CanRead)
            throw new ArgumentException("Stream must be readable", nameof(stream));

        if (!stream.CanSeek)
            throw new NotSupportedException(
                "Stream must be seekable. Use DetectFormat(byte[]) by converting stream to bytes first if needed.");

        long originalPosition = stream.Position;
        stream.Seek(0, SeekOrigin.Begin);

        try
        {
            Span<byte> buffer = stackalloc byte[512];
            var totalRead = 0;

            while (totalRead < buffer.Length)
            {
                var bytesRead = stream.Read(buffer.Slice(totalRead));
                if (bytesRead == 0)
                    break;
                totalRead += bytesRead;
            }

            if (totalRead < 3)
                throw new ArgumentException("Stream is too short to determine format (minimum 3 bytes required)",
                    nameof(stream));

            return IdentifyFormatBySignature(buffer.Slice(0, totalRead));
        }
        finally
        {
            stream.Seek(originalPosition, SeekOrigin.Begin);
        }
    }

    private static LegioImageFormat IdentifyFormatBySignature(ReadOnlySpan<byte> imageData)
    {
        if (IsWebP(imageData))
            return LegioImageFormat.WebP;

        if (IsJpeg(imageData))
            return LegioImageFormat.Jpeg;

        if (IsGif(imageData))
            return LegioImageFormat.Gif;

        if (IsPng(imageData))
            return LegioImageFormat.Png;

        if (IsSvg(imageData))
            return LegioImageFormat.Svg;

        if (IsBmp(imageData))
            return LegioImageFormat.Bmp;

        throw new NotSupportedException(
            "Unsupported or unrecognized image format. Supported formats: PNG, JPEG, WebP, BMP, GIF, SVG");
    }

    private static bool IsWebP(ReadOnlySpan<byte> imageData)
    {
        if (imageData.Length < 12) return false;

        return imageData[0] == 0x52 && imageData[1] == 0x49 && imageData[2] == 0x46 && imageData[3] == 0x46 &&
               imageData[8] == 0x57 && imageData[9] == 0x45 && imageData[10] == 0x66 && imageData[11] == 0x50;
    }

    private static bool IsJpeg(ReadOnlySpan<byte> imageData)
    {
        if (imageData.Length < 3) return false;

        return imageData[0] == 0xFF && imageData[1] == 0xD8 && imageData[2] == 0xFF;
    }

    private static bool IsBmp(ReadOnlySpan<byte> imageData)
    {
        if (imageData.Length < 14) return false;

        if (imageData[0] != 0x42 || imageData[1] != 0x4D) return false;

        var dataOffset = (uint)imageData[10] |
                        ((uint)imageData[11] << 8) |
                        ((uint)imageData[12] << 16) |
                        ((uint)imageData[13] << 24);

        return dataOffset >= 14 && dataOffset <= 0x0FFFFFFF;
    }

    private static bool IsGif(ReadOnlySpan<byte> imageData)
    {
        if (imageData.Length < 6) return false;

        return imageData[0] == 0x47 && imageData[1] == 0x49 && imageData[2] == 0x46 &&
               imageData[3] == 0x38 &&
               (imageData[4] == 0x37 || imageData[4] == 0x39) &&
               imageData[5] == 0x61;
    }

    private static bool IsPng(ReadOnlySpan<byte> imageData)
    {
        if (imageData.Length < 8) return false;

        return imageData[0] == 0x89 && imageData[1] == 0x50 && imageData[2] == 0x4E && imageData[3] == 0x47 &&
               imageData[4] == 0x0D && imageData[5] == 0x0A && imageData[6] == 0x1A && imageData[7] == 0x0A;
    }

    private static bool IsSvg(ReadOnlySpan<byte> imageData)
    {
        if (imageData.Length < 5) return false;

        var dataSpan = imageData.Slice(0, Math.Min(imageData.Length, 512));
        var offset = 0;

        if (HasUtf8Bom(dataSpan))
        {
            offset = 3;
        }

        if (offset + 4 > dataSpan.Length) return false;

        var textSpan = Encoding.ASCII.GetString(dataSpan.Slice(offset)).AsSpan();

        var trimmed = textSpan.TrimStart();

        if (trimmed.StartsWith("<?xml", StringComparison.OrdinalIgnoreCase))
        {
            var xmlEndIndex = trimmed.IndexOf("?>", StringComparison.Ordinal);
            if (xmlEndIndex >= 0)
            {
                var afterXml = trimmed.Slice(xmlEndIndex + 2).TrimStart();

                var firstElementIndex = FindFirstElement(afterXml);
                if (firstElementIndex >= 0)
                {
                    var firstElement = afterXml.Slice(firstElementIndex);
                    return firstElement.StartsWith("<svg", StringComparison.OrdinalIgnoreCase);
                }
            }
            return false;
        }

        var rootElementIndex = FindFirstElement(trimmed);
        if (rootElementIndex >= 0)
        {
            var rootElement = trimmed.Slice(rootElementIndex);
            return rootElement.StartsWith("<svg", StringComparison.OrdinalIgnoreCase);
        }

        return false;
    }

    private static int FindFirstElement(ReadOnlySpan<char> text)
    {
        var i = 0;
        while (i < text.Length)
        {
            if (char.IsWhiteSpace(text[i]))
            {
                i++;
                continue;
            }

            if (text[i] != '<')
            {
                return -1;
            }

            if (i + 4 <= text.Length &&
                text[i + 1] == '!' &&
                text[i + 2] == '-' &&
                text[i + 3] == '-')
            {
                var commentEnd = text.Slice(i).IndexOf("-->", StringComparison.Ordinal);
                if (commentEnd < 0) return -1;
                i += commentEnd + 3;
                continue;
            }

            if (i + 9 <= text.Length &&
                text[i + 1] == '!' &&
                (text[i + 2] == 'D' || text[i + 2] == 'd') &&
                (text[i + 3] == 'O' || text[i + 3] == 'o') &&
                (text[i + 4] == 'C' || text[i + 4] == 'c') &&
                (text[i + 5] == 'T' || text[i + 5] == 't') &&
                (text[i + 6] == 'Y' || text[i + 6] == 'y') &&
                (text[i + 7] == 'P' || text[i + 7] == 'p') &&
                (text[i + 8] == 'E' || text[i + 8] == 'e'))
            {
                var doctypeEnd = text.Slice(i).IndexOf('>');
                if (doctypeEnd < 0) return -1;
                i += doctypeEnd + 1;
                continue;
            }

            return i;
        }
        return -1;
    }

    private static bool HasUtf8Bom(ReadOnlySpan<byte> data)
    {
        return data.Length >= 3 && data[0] == 0xEF && data[1] == 0xBB && data[2] == 0xBF;
    }
}