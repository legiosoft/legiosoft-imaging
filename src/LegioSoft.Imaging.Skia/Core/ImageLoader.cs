using SkiaSharp;

namespace LegioSoft.Imaging.Skia.Core;

/// <summary>
/// Provides image loading functionality with automatic codec management.
/// </summary>
/// <remarks>
/// ImageLoader creates SKBitmap instances that wrap unmanaged memory.
/// Always dispose returned bitmaps or use them within a using statement
/// to prevent memory leaks. Consider using ImageMetadataReader.GetInfo()
/// if you only need image dimensions without full pixel data.
/// </remarks>
public static class ImageLoader
{
    /// <summary>
    /// Loads an image from byte array.
    /// </summary>
    /// <param name="imageData">Image data to decode.</param>
    /// <returns>Decoded SKBitmap. Caller is responsible for disposal.</returns>
    /// <exception cref="ArgumentException">Thrown when imageData is null or empty.</exception>
    /// <exception cref="InvalidOperationException">Thrown when image format is not supported.</exception>
    public static SKBitmap LoadBitmap(byte[] imageData)
    {
        if (imageData == null || imageData.Length == 0)
            throw new ArgumentException("Image data cannot be null or empty", nameof(imageData));

        using var ms = new MemoryStream(imageData);
        using var codec = SKCodec.Create(ms);

        if (codec == null)
            throw new InvalidOperationException("Unable to decode image data. Unsupported format.");

        var bitmap = new SKBitmap(codec.Info);
        codec.GetPixels(bitmap.Info, bitmap.GetPixels());

        return bitmap;
    }

    /// <summary>
    /// Loads an image from a stream.
    /// </summary>
    /// <param name="stream">Readable stream containing image data.</param>
    /// <returns>Decoded SKBitmap. Caller is responsible for disposal.</returns>
    /// <exception cref="ArgumentNullException">Thrown when stream is null.</exception>
    /// <exception cref="ArgumentException">Thrown when stream is not readable.</exception>
    /// <exception cref="InvalidOperationException">Thrown when image format is not supported.</exception>
    public static SKBitmap LoadBitmap(Stream stream)
    {
        if (stream == null)
            throw new ArgumentNullException(nameof(stream));

        if (!stream.CanRead)
            throw new ArgumentException("Stream must be readable", nameof(stream));

        using var codec = SKCodec.Create(stream);

        if (codec == null)
            throw new InvalidOperationException("Unable to decode image data. Unsupported format.");

        var bitmap = new SKBitmap(codec.Info);
        codec.GetPixels(bitmap.Info, bitmap.GetPixels());

        return bitmap;
    }

    /// <summary>
    /// Loads an image from a file path.
    /// </summary>
    /// <param name="filePath">Path to the image file.</param>
    /// <returns>Decoded SKBitmap. Caller is responsible for disposal.</returns>
    /// <exception cref="FileNotFoundException">Thrown when file does not exist.</exception>
    /// <exception cref="InvalidOperationException">Thrown when image format is not supported.</exception>
    public static SKBitmap LoadBitmap(string filePath)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"File not found: {filePath}", filePath);

        using var fs = File.OpenRead(filePath);
        return LoadBitmap(fs);
    }
}
