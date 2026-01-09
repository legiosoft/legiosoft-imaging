using SkiaSharp;
using LegioSoft.Imaging.Core;

namespace LegioSoft.Imaging.Skia.Core;

/// <summary>
/// Provides efficient image metadata extraction without full pixel decoding.
/// </summary>
/// <remarks>
/// Uses SKCodec to read image headers only, making it significantly faster
/// than loading the full bitmap when you only need dimensions or format information.
/// </remarks>
public static class ImageMetadataReader
{
    /// <summary>
    /// Reads image metadata from byte array without full decoding.
    /// </summary>
    /// <param name="imageData">Image data to analyze.</param>
    /// <returns>Image metadata including dimensions, format, and size.</returns>
    /// <exception cref="ArgumentException">Thrown when imageData is null or empty.</exception>
    /// <exception cref="InvalidOperationException">Thrown when image format is not supported.</exception>
    /// <example>
    /// <code>
    /// var imageData = File.ReadAllBytes("photo.jpg");
    /// var info = ImageMetadataReader.GetInfo(imageData);
    /// Console.WriteLine($"{info.Width}x{info.Height}");
    /// </code>
    /// </example>
    public static LegioImageInfo GetInfo(byte[] imageData)
    {
        if (imageData == null || imageData.Length == 0)
            throw new ArgumentException("Image data cannot be null or empty", nameof(imageData));

        using var ms = new MemoryStream(imageData);
        using var codec = SKCodec.Create(ms);

        if (codec == null)
            throw new InvalidOperationException("Unable to decode image data. Unsupported format.");

        var format = FormatDetector.DetectFormat(imageData);

        return new LegioImageInfo
        {
            Width = codec.Info.Width,
            Height = codec.Info.Height,
            Format = format,
            HasAlpha = codec.Info.AlphaType != SKAlphaType.Opaque,
            ByteSize = imageData.Length
        };
    }

    /// <summary>
    /// Reads image metadata from a stream without full decoding.
    /// </summary>
    /// <param name="stream">Readable stream containing image data.</param>
    /// <returns>Image metadata including dimensions, format, and size.</returns>
    /// <exception cref="ArgumentNullException">Thrown when stream is null.</exception>
    /// <exception cref="ArgumentException">Thrown when stream is not readable.</exception>
    /// <exception cref="InvalidOperationException">Thrown when image format is not supported.</exception>
    /// <remarks>
    /// For non-seekable streams, ByteSize will be 0. Consider using byte[] overload
    /// if you need accurate file size information.
    /// </remarks>
    public static LegioImageInfo GetInfo(Stream stream)
    {
        if (stream == null)
            throw new ArgumentNullException(nameof(stream));

        if (!stream.CanRead)
            throw new ArgumentException("Stream must be readable", nameof(stream));

        var codec = SKCodec.Create(stream);

        if (codec == null)
            throw new InvalidOperationException("Unable to decode image data. Unsupported format.");

        try
        {
            var format = DetectFormatFromCodec(codec);

            return new LegioImageInfo
            {
                Width = codec.Info.Width,
                Height = codec.Info.Height,
                Format = format,
                HasAlpha = codec.Info.AlphaType != SKAlphaType.Opaque,
                ByteSize = stream.CanSeek ? (int)stream.Length : 0
            };
        }
        finally
        {
            codec.Dispose();
        }
    }

    /// <summary>
    /// Reads image metadata from a file without full decoding.
    /// </summary>
    /// <param name="filePath">Path to the image file.</param>
    /// <returns>Image metadata including dimensions, format, and size.</returns>
    /// <exception cref="FileNotFoundException">Thrown when file does not exist.</exception>
    /// <exception cref="InvalidOperationException">Thrown when image format is not supported.</exception>
    public static LegioImageInfo GetInfo(string filePath)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"File not found: {filePath}", filePath);

        using var fs = File.OpenRead(filePath);
        return GetInfo(fs);
    }

    private static LegioImageFormat DetectFormatFromCodec(SKCodec codec)
    {
        return codec.EncodedFormat switch
        {
            SKEncodedImageFormat.Png => LegioImageFormat.Png,
            SKEncodedImageFormat.Jpeg => LegioImageFormat.Jpeg,
            SKEncodedImageFormat.Webp => LegioImageFormat.WebP,
            SKEncodedImageFormat.Bmp => LegioImageFormat.Bmp,
            SKEncodedImageFormat.Gif => LegioImageFormat.Gif,
            _ => LegioImageFormat.Png
        };
    }
}
