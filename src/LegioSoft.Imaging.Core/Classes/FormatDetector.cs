using LegioSoft.Imaging.Core.Enums;

using System;
using System.IO;
using System.Text;

namespace LegioSoft.Imaging.Core.Classes;

public static class FormatDetector
{
    public static LegioImageFormat DetectFormat(byte[] imageData)
    {
        ArgumentNullException.ThrowIfNull(imageData);

        if (imageData.Length == 0)
            throw new ArgumentException("Image data cannot be empty", nameof(imageData));

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

        if (IsBmp(imageData))
            return LegioImageFormat.Bmp;

        throw new NotSupportedException(
            "Unsupported or unrecognized image format. Supported formats: PNG, JPEG, WebP, BMP, GIF");
    }

    private static bool IsWebP(ReadOnlySpan<byte> imageData)
    {
        if (imageData.Length < 12) return false;

        return imageData[0] == 0x52 && imageData[1] == 0x49 && imageData[2] == 0x46 && imageData[3] == 0x46 &&
               imageData[8] == 0x57 && imageData[9] == 0x45 && imageData[10] == 0x42 && imageData[11] == 0x50;
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
}
