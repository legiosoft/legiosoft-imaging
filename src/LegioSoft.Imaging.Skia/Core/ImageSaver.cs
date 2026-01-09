using LegioSoft.Imaging.Core;
using SkiaSharp;

namespace LegioSoft.Imaging.Skia.Core;

/// <summary>
/// Provides image encoding and saving functionality.
/// </summary>
/// <remarks>
/// Quality parameter has different effects depending on format:
/// - PNG/BMP/GIF: Always 100 (lossless), quality parameter is ignored
/// - JPEG: 0-100, lower = smaller file with more compression artifacts
/// - WebP: 0-100, balances size and quality
/// </remarks>
public static class ImageSaver
{
    /// <summary>
    /// Encodes a bitmap to the specified format and quality.
    /// </summary>
    /// <param name="bitmap">Bitmap to encode. Must have valid dimensions.</param>
    /// <param name="format">Target image format.</param>
    /// <param name="quality">Encoding quality 0-100. Defaults to 90.</param>
    /// <returns>Encoded image data as byte array.</returns>
    /// <exception cref="ArgumentNullException">Thrown when bitmap is null.</exception>
    /// <exception cref="ArgumentException">Thrown when bitmap has invalid dimensions.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when quality is outside 0-100 range.</exception>
    /// <exception cref="NotSupportedException">Thrown when format is not supported for encoding.</exception>
    /// <exception cref="InvalidOperationException">Thrown when encoding fails.</exception>
    /// <example>
    /// <code>
    /// using var bitmap = ImageLoader.LoadBitmap("photo.jpg");
    /// var pngData = ImageSaver.SaveBitmap(bitmap, LegioImageFormat.Png);
    /// File.WriteAllBytes("output.png", pngData);
    /// </code>
    /// </example>
    public static byte[] SaveBitmap(SKBitmap bitmap, LegioImageFormat format, int quality = 90)
    {
        if (bitmap == null)
            throw new ArgumentNullException(nameof(bitmap));

        if (bitmap.Width <= 0 || bitmap.Height <= 0)
            throw new ArgumentException("Bitmap must have valid dimensions", nameof(bitmap));

        ValidateQuality(format, quality);
        ValidateFormatSupport(format);

        var skiaFormat = ConvertToSkiaFormat(format);
        var adjustedQuality = AdjustQualityForFormat(format, quality);

        using var ms = new MemoryStream();
        var success = bitmap.Encode(ms, skiaFormat, adjustedQuality);

        if (!success || ms.Length == 0)
            throw new InvalidOperationException($"Failed to encode image as {format}");

        return ms.ToArray();
    }

    private static void ValidateQuality(LegioImageFormat format, int quality)
    {
        if (quality is < 0 or > 100)
            throw new ArgumentOutOfRangeException(nameof(quality), "Quality must be between 0 and 100");
    }

    private static void ValidateFormatSupport(LegioImageFormat format)
    {
        if (format == LegioImageFormat.Bmp)
            throw new NotSupportedException("BMP encoding is not supported in Skia implementation");

        if (format == LegioImageFormat.Gif)
            throw new NotSupportedException("GIF encoding is not supported in Skia implementation");
    }

    private static SKEncodedImageFormat ConvertToSkiaFormat(LegioImageFormat format)
    {
        return format switch
        {
            LegioImageFormat.Jpeg => SKEncodedImageFormat.Jpeg,
            LegioImageFormat.WebP => SKEncodedImageFormat.Webp,
            LegioImageFormat.Png => SKEncodedImageFormat.Png,
            LegioImageFormat.Bmp => SKEncodedImageFormat.Bmp,
            LegioImageFormat.Gif => SKEncodedImageFormat.Gif,
            _ => throw new NotSupportedException($"Unsupported image format: {format}")
        };
    }

    private static int AdjustQualityForFormat(LegioImageFormat format, int quality)
    {
        return format switch
        {
            LegioImageFormat.Png => 100,
            LegioImageFormat.Bmp => 100,
            LegioImageFormat.Gif => 100,
            _ => quality
        };
    }
}
