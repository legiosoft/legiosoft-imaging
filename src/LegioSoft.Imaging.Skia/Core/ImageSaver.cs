using LegioSoft.Imaging.Core.Enums;
using SkiaSharp;

namespace LegioSoft.Imaging.Skia.Core;

internal static class ImageSaver
{
    public static byte[] SaveBitmap(SKBitmap bitmap, LegioImageFormat format, int quality = 90)
    {
        if (bitmap == null)
            throw new ArgumentNullException(nameof(bitmap));

        if (bitmap.Width <= 0 || bitmap.Height <= 0)
            throw new ArgumentException("Bitmap must have valid dimensions", nameof(bitmap));

        var skiaFormat = ConvertToSkiaFormat(format, quality);
        var adjustedQuality = AdjustQualityForFormat(format, quality);

        using var ms = new MemoryStream();
        var success = bitmap.Encode(ms, skiaFormat, adjustedQuality);

        if (!success || ms.Length == 0)
            throw new InvalidOperationException($"Failed to encode image as {format}");

        return ms.ToArray();
    }

    public static void SaveBitmap(SKBitmap bitmap, Stream outputStream, LegioImageFormat format, int quality = 90)
    {
        if (bitmap == null)
            throw new ArgumentNullException(nameof(bitmap));

        if (outputStream == null)
            throw new ArgumentNullException(nameof(outputStream));

        if (bitmap.Width <= 0 || bitmap.Height <= 0)
            throw new ArgumentException("Bitmap must have valid dimensions", nameof(bitmap));

        var skiaFormat = ConvertToSkiaFormat(format, quality);
        var adjustedQuality = AdjustQualityForFormat(format, quality);

        var success = bitmap.Encode(outputStream, skiaFormat, adjustedQuality);

        if (!success)
            throw new InvalidOperationException($"Failed to encode image as {format}");
    }

    private static SKEncodedImageFormat ConvertToSkiaFormat(LegioImageFormat format, int quality)
    {
        return format switch
        {
            LegioImageFormat.Jpeg => SKEncodedImageFormat.Jpeg,
            LegioImageFormat.WebP => SKEncodedImageFormat.Webp,
            LegioImageFormat.Png => SKEncodedImageFormat.Png,
            _ => throw new NotSupportedException($"Unsupported image format: {format}")
        };
    }

    private static int AdjustQualityForFormat(LegioImageFormat format, int quality)
    {
        return format switch
        {
            LegioImageFormat.Png => 100,
            LegioImageFormat.WebP when quality == 100 => 100,
            _ => Math.Clamp(quality, 0, 100)
        };
    }
}
