using SkiaSharp;
using LegioSoft.Imaging.Core.Classes;
using LegioSoft.Imaging.Core.Enums;

namespace LegioSoft.Imaging.Skia.Core;

/// <summary>
/// Provides secure image metadata extraction without full pixel decoding.
/// </summary>
/// <remarks>
/// Uses SKCodec to read image headers only, making it significantly faster
/// than loading the full bitmap when you only need dimensions or format information.
/// </remarks>
internal static class ImageMetadataReader
{
    /// <summary>
    /// Maximum allowed image width (16,384 pixels = 16K).
    /// </summary>
    private const int MaxImageWidth = 16384;

    /// <summary>
    /// Maximum allowed image height (16,384 pixels = 16K).
    /// </summary>
    private const int MaxImageHeight = 16384;

    /// <summary>
    /// Allowed image file extensions (case-insensitive).
    /// </summary>
    private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".jpg",
        ".jpeg",
        ".png",
        ".webp"
    };

    /// <summary>
    /// Approved base directories from which images can be loaded.
    /// Configure according to your security policy.
    /// </summary>
    private static readonly string[] ApprovedDirectories =
    [
        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "images"),
        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "assets")
    ];

    /// <summary>
    /// Reads image metadata from byte array with full validation.
    /// </summary>
    /// <param name="imageData">Image data to analyze.</param>
    /// <returns>Image metadata including dimensions, format, and size.</returns>
    /// <exception cref="ArgumentException">Thrown when imageData is null or empty.</exception>
    /// <exception cref="InvalidOperationException">Thrown when image format is not supported or dimensions exceed limits.</exception>
    public static LegioImageInfo GetInfo(byte[] imageData)
    {
        if (imageData == null || imageData.Length == 0)
            throw new ArgumentException("Image data cannot be null or empty", nameof(imageData));

        using var ms = new MemoryStream(imageData, writable: false);
        return GetInfo(ms, imageData.Length);
    }

    /// <summary>
    /// Reads image metadata from a stream with full validation.
    /// </summary>
    /// <param name="stream">Readable stream containing image data.</param>
    /// <returns>Image metadata including dimensions, format, and size.</returns>
    /// <exception cref="ArgumentNullException">Thrown when stream is null.</exception>
    /// <exception cref="ArgumentException">Thrown when stream is not readable.</exception>
    /// <exception cref="InvalidOperationException">Thrown when image format is not supported or dimensions exceed limits.</exception>
    /// <remarks>
    /// For non-seekable streams, ByteSize will be 0. Consider using byte[] overload
    /// if you need accurate file size information.
    /// </remarks>
    public static LegioImageInfo GetInfo(Stream stream)
    {
        if (stream == null)
            throw new ArgumentNullException(nameof(stream));

        var byteSize = stream.CanSeek
            ? (int)Math.Min(stream.Length, int.MaxValue)
            : 0;

        return GetInfo(stream, byteSize);
    }

    private static LegioImageInfo GetInfo(Stream stream, int byteSize)
    {
        if (stream == null)
            throw new ArgumentNullException(nameof(stream));

        if (!stream.CanRead)
            throw new ArgumentException("Stream must be readable", nameof(stream));

        long originalPosition = 0;
        var canSeek = false;

        if (stream.CanSeek)
        {
            try
            {
                originalPosition = stream.Position;
                stream.Seek(0, SeekOrigin.Begin);
                canSeek = true;
            }
            catch (IOException)
            {
                canSeek = false;
            }
        }

        try
        {
            using var codec = SKCodec.Create(stream);

            if (codec == null)
                throw new InvalidOperationException("Unable to decode image data. Unsupported format.");

            var format = DetectFormatFromCodec(codec);

            var info = new LegioImageInfo
            {
                Width = codec.Info.Width,
                Height = codec.Info.Height,
                Format = format,
                HasAlpha = codec.Info.AlphaType != SKAlphaType.Opaque,
                ByteSize = byteSize
            };

            ValidateImageDimensions(info.Width, info.Height);

            return info;
        }
        catch (OutOfMemoryException)
        {
            throw new OutOfMemoryException("Image is too large or system is low on memory");
        }
        finally
        {
            if (canSeek && stream.CanSeek)
            {
                try
                {
                    stream.Seek(originalPosition, SeekOrigin.Begin);
                }
                catch (IOException)
                {
                }
            }
        }
    }

    /// <summary>
    /// Reads image metadata from a file path with full validation and security checks.
    /// </summary>
    /// <param name="filePath">Path to the image file.</param>
    /// <returns>Image metadata including dimensions, format, and size.</returns>
    /// <exception cref="ArgumentException">Thrown when filePath is null, empty, or uses invalid format.</exception>
    /// <exception cref="FileNotFoundException">Thrown when file does not exist.</exception>
    /// <exception cref="UnauthorizedAccessException">Thrown when file is outside approved directories or is a symbolic link.</exception>
    /// <exception cref="InvalidOperationException">Thrown when image format is not supported or dimensions exceed limits.</exception>
    public static LegioImageInfo GetInfo(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("File path cannot be null or empty", nameof(filePath));

        var extension = Path.GetExtension(filePath);
        if (!AllowedExtensions.Contains(extension))
            throw new ArgumentException(
                $"File format '{extension}' is not supported. " +
                $"Allowed formats: {string.Join(", ", AllowedExtensions)}",
                nameof(filePath));

        FileStream fileStream;
        try
        {
            fileStream = File.OpenRead(filePath);
        }
        catch (UnauthorizedAccessException)
        {
            throw new UnauthorizedAccessException(
                $"Access denied: Cannot read image file at '{filePath}'");
        }
        catch (FileNotFoundException)
        {
            throw new FileNotFoundException(
                $"File not found: {filePath}", filePath);
        }
        catch (IOException ex)
        {
            throw new InvalidOperationException(
                $"Error opening image file: {ex.Message}", ex);
        }

        try
        {
            ImageSecurity.ValidateOpenedFile(fileStream, filePath, ApprovedDirectories, AllowedExtensions);
            return GetInfo(fileStream, (int)fileStream.Length);
        }
        finally
        {
            fileStream.Dispose();
        }
    }

    /// <summary>
    /// Detects image format from codec information.
    /// Throws for unsupported formats - no silent defaults.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when format is not supported.</exception>
    private static LegioImageFormat DetectFormatFromCodec(SKCodec codec)
    {
        return codec.EncodedFormat switch
        {
            SKEncodedImageFormat.Png => LegioImageFormat.Png,
            SKEncodedImageFormat.Jpeg => LegioImageFormat.Jpeg,
            SKEncodedImageFormat.Webp => LegioImageFormat.WebP,
            _ => throw new InvalidOperationException(
                $"Unsupported format: {codec.EncodedFormat}. " +
                $"Only PNG, JPEG, and WebP formats are supported.")
        };
    }

    /// <summary>
    /// Validates image dimensions and memory requirements.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when dimensions exceed limits.</exception>
    private static void ValidateImageDimensions(int width, int height)
    {
        if (width <= 0 || height <= 0)
            throw new InvalidOperationException(
                $"Invalid image dimensions: {width}x{height}");

        if (width > MaxImageWidth || height > MaxImageHeight)
            throw new InvalidOperationException(
                $"Image dimensions {width}x{height} exceed maximum " +
                $"allowed size of {MaxImageWidth}x{MaxImageHeight}. " +
                $"This is a security limit to prevent memory exhaustion attacks.");
    }
}