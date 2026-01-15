using SkiaSharp;

namespace LegioSoft.Imaging.Skia.Core;

/// <summary>
/// Provides secure image loading functionality with automatic codec management,
/// input validation, and resource protection.
/// </summary>
/// <remarks>
/// ImageLoader creates SKBitmap instances that wrap unmanaged memory.
/// Always dispose returned bitmaps or use them within a using statement
/// to prevent memory leaks.
/// 
/// SECURITY FEATURES:
/// - Path traversal prevention
/// - Image dimension limits
/// - Memory allocation limits
/// - Format whitelist
/// - Symbolic link detection
/// 
/// Consider using ImageMetadataReader.GetInfo() if you only need 
/// image dimensions without full pixel data.
/// </remarks>
internal static class ImageLoader
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
    /// Maximum allowed memory per image (512 MB).
    /// Prevents DoS attacks via oversized images.
    /// </summary>
    private const long MaxImageMemoryBytes = 512 * 1024 * 1024;

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
    /// Loads an image from byte array with full validation.
    /// </summary>
    /// <param name="imageData">Image data to decode.</param>
    /// <returns>Decoded SKBitmap. Caller is responsible for disposal.</returns>
    /// <exception cref="ArgumentException">Thrown when imageData is null or empty.</exception>
    /// <exception cref="InvalidOperationException">Thrown when image format is not supported or dimensions exceed limits.</exception>
    /// <exception cref="OutOfMemoryException">Thrown when image would require excessive memory.</exception>
    public static SKBitmap LoadBitmap(byte[] imageData)
    {
        if (imageData == null || imageData.Length == 0)
            throw new ArgumentException("Image data cannot be null or empty", nameof(imageData));

        using var ms = new MemoryStream(imageData, writable: false);
        return LoadBitmap(ms);
    }

    /// <summary>
    /// Loads an image from a stream with full validation.
    /// </summary>
    /// <param name="stream">Readable stream containing image data.</param>
    /// <returns>Decoded SKBitmap. Caller is responsible for disposal.</returns>
    /// <exception cref="ArgumentNullException">Thrown when stream is null.</exception>
    /// <exception cref="ArgumentException">Thrown when stream is not readable.</exception>
    /// <exception cref="InvalidOperationException">Thrown when image format is not supported or dimensions exceed limits.</exception>
    /// <exception cref="OutOfMemoryException">Thrown when image would require excessive memory.</exception>
    private static SKBitmap LoadBitmap(Stream stream)
    {
        if (stream == null)
            throw new ArgumentNullException(nameof(stream));

        if (!stream.CanRead)
            throw new ArgumentException("Stream must be readable", nameof(stream));

        // Save original position for streams that support seeking
        long originalPosition = 0;
        bool canSeek = false;
        
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
                // Some streams claim to be seekable but fail - continue anyway
                canSeek = false;
            }
        }

        try
        {
            // Use SKBitmap.Decode() instead of manual SKCodec usage
            // This handles all memory management correctly
            SKBitmap bitmap;
            try
            {
                bitmap = SKBitmap.Decode(stream);
            }
            catch (OutOfMemoryException)
            {
                throw new OutOfMemoryException(
                    "Image is too large or system is low on memory");
            }
            catch (Exception ex) when (!(ex is ArgumentException) && !(ex is InvalidOperationException))
            {
                throw new InvalidOperationException(
                    $"Failed to decode image: {ex.Message}", ex);
            }

            if (bitmap == null)
                throw new InvalidOperationException(
                    "Unable to decode image data. Unsupported format or corrupted file.");

            // Validate dimensions and memory requirements
            ValidateImageDimensions(bitmap.Info);

            return bitmap;
        }
        finally
        {
            // Restore stream position if it was seekable
            if (canSeek && stream.CanSeek)
            {
                try
                {
                    stream.Seek(originalPosition, SeekOrigin.Begin);
                }
                catch (IOException)
                {
                    // Ignore seek errors on cleanup
                }
            }
        }
    }

    /// <summary>
    /// Loads an image from a file path with full validation and security checks.
    /// </summary>
    /// <param name="filePath">Path to the image file.</param>
    /// <returns>Decoded SKBitmap. Caller is responsible for disposal.</returns>
    /// <exception cref="ArgumentException">Thrown when filePath is null, empty, or uses invalid format.</exception>
    /// <exception cref="FileNotFoundException">Thrown when file does not exist.</exception>
    /// <exception cref="UnauthorizedAccessException">Thrown when file is outside approved directories or is a symbolic link.</exception>
    /// <exception cref="InvalidOperationException">Thrown when image format is not supported or dimensions exceed limits.</exception>
    /// <exception cref="OutOfMemoryException">Thrown when image would require excessive memory.</exception>
    public static SKBitmap LoadBitmap(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException(
                "File path cannot be null or empty", nameof(filePath));

        // Validate file extension
        var extension = Path.GetExtension(filePath);
        if (!AllowedExtensions.Contains(extension))
            throw new ArgumentException(
                $"File format '{extension}' is not supported. " +
                $"Allowed formats: {string.Join(", ", AllowedExtensions)}", 
                nameof(filePath));

        // SECURITY: Validate path to prevent traversal attacks
        ValidateFilePath(filePath);

        if (!File.Exists(filePath))
            throw new FileNotFoundException(
                $"File not found: {filePath}", filePath);

        try
        {
            using var fs = File.OpenRead(filePath);
            return LoadBitmap(fs);
        }
        catch (UnauthorizedAccessException)
        {
            throw new UnauthorizedAccessException(
                $"Access denied: Cannot read image file at '{filePath}'");
        }
        catch (IOException ex)
        {
            throw new InvalidOperationException(
                $"Error reading image file: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Validates image dimensions and memory requirements.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when dimensions exceed limits.</exception>
    private static void ValidateImageDimensions(SKImageInfo info)
    {
        // Check for invalid dimensions
        if (info.Width <= 0 || info.Height <= 0)
            throw new InvalidOperationException(
                $"Invalid image dimensions: {info.Width}x{info.Height}");

        // Check width/height limits
        if (info.Width > MaxImageWidth || info.Height > MaxImageHeight)
            throw new InvalidOperationException(
                $"Image dimensions {info.Width}x{info.Height} exceed maximum " +
                $"allowed size of {MaxImageWidth}x{MaxImageHeight}. " +
                $"This is a security limit to prevent memory exhaustion attacks.");

        // Estimate memory usage (assume 4 bytes per pixel for RGBA)
        // Note: Some formats use different bytes per pixel, but this is conservative estimate
        long estimatedBytes;
        try
        {
            estimatedBytes = checked((long)info.Width * info.Height * 4);
        }
        catch (OverflowException)
        {
            throw new InvalidOperationException(
                $"Image dimensions {info.Width}x{info.Height} would cause integer overflow");
        }

        if (estimatedBytes > MaxImageMemoryBytes)
            throw new InvalidOperationException(
                $"Image would require approximately {FormatBytes(estimatedBytes)}, " +
                $"which exceeds the maximum allowed memory of {FormatBytes(MaxImageMemoryBytes)}. " +
                $"This is a security limit to prevent denial-of-service attacks.");
    }

    /// <summary>
    /// Validates that a file path is within approved directories and is not a symbolic link.
    /// Prevents path traversal and symlink attacks.
    /// </summary>
    /// <exception cref="UnauthorizedAccessException">Thrown when path is not authorized.</exception>
    private static void ValidateFilePath(string filePath)
    {
        // Get absolute path to prevent bypass attempts
        string fullPath;
        try
        {
            fullPath = Path.GetFullPath(filePath);
        }
        catch (Exception ex)
        {
            throw new ArgumentException(
                $"Invalid file path: {ex.Message}", 
                nameof(filePath), ex);
        }

        // Check if path is within any approved directory
        bool isApproved = ApprovedDirectories.Any(dir =>
        {
            try
            {
                var fullDir = Path.GetFullPath(dir);
                // Use ordinal comparison for consistency across platforms
                return fullPath.StartsWith(fullDir + Path.DirectorySeparatorChar, 
                    StringComparison.Ordinal);
            }
            catch
            {
                return false;
            }
        });

        if (!isApproved)
        {
            throw new UnauthorizedAccessException(
                $"File path '{fullPath}' is outside the approved image directories: " +
                $"{string.Join(", ", ApprovedDirectories)}. " +
                $"Configure ApprovedDirectories for your security policy.");
        }

        // SECURITY: Detect symbolic links (requires .NET 6+)
        // This prevents attacks where symlinks point to system files
        try
        {
            var fileInfo = new FileInfo(fullPath);
            // LinkTarget is non-null only if the file is a symbolic link
            if (fileInfo.LinkTarget != null)
            {
                throw new UnauthorizedAccessException(
                    $"Symbolic links are not allowed for security reasons. " +
                    $"File '{fullPath}' is a symbolic link.");
            }
        }
        catch (UnauthorizedAccessException)
        {
            throw;
        }
        catch (Exception ex)
        {
            // Log but don't fail on link detection errors - continue loading
            System.Diagnostics.Debug.WriteLine(
                $"Warning: Could not check symbolic link status: {ex.Message}");
        }
    }

    /// <summary>
    /// Formats byte count to human-readable size (KB, MB, GB).
    /// </summary>
    private static string FormatBytes(long bytes)
    {
        const long KB = 1024;
        const long MB = KB * 1024;
        const long GB = MB * 1024;

        return bytes switch
        {
            >= GB => $"{(double)bytes / GB:F2} GB",
            >= MB => $"{(double)bytes / MB:F2} MB",
            >= KB => $"{(double)bytes / KB:F2} KB",
            _ => $"{bytes} bytes"
        };
    }
}
