using System.Diagnostics;
using System.Text;
using SkiaSharp;

namespace LegioSoft.Imaging.Skia.Core;

/// <summary>
/// Loads and decodes images with automatic validation and resource protection.
/// </summary>
/// <remarks>
/// ImageLoader creates SKBitmap instances that wrap unmanaged memory.
/// Always dispose returned bitmaps or use them within a using statement.
/// 
/// Consider using ImageMetadataReader.GetInfo() if you only need 
/// image dimensions without loading full pixel data.
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
    /// Loads an image from a stream.
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
                throw new InvalidOperationException(
                    "Unable to decode image data. Unsupported format or corrupted file.");

            ValidateImageDimensions(codec.Info);

            var bitmap = new SKBitmap(codec.Info);
            
            if (bitmap.Handle == IntPtr.Zero)
            {
                throw new InvalidOperationException(
                    "Failed to allocate memory for image bitmap.");
            }

            try
            {
                var result = codec.GetPixels(codec.Info, bitmap.GetPixels());
                
                if (result != SKCodecResult.Success)
                {
                    throw new InvalidOperationException(
                        $"Failed to decode image: Codec returned {result}");
                }
            }
            catch
            {
                bitmap.Dispose();
                throw;
            }

            return bitmap;
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
                    // Best-effort cleanup: if restoring stream position fails, 
                    // the stream may be left at an unexpected position, but 
                    // we cannot safely propagate this exception from a finally block.
                }
            }
        }
    }

    /// <summary>
    /// Loads an image from a file path.
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
            ValidateOpenedFile(fileStream, filePath);
            return LoadBitmap(fileStream);
        }
        finally
        {
            fileStream.Dispose();
        }
    }

    /// <summary>
    /// Validates that an opened file is within approved directories and not a symlink.
    /// </summary>
    private static void ValidateOpenedFile(FileStream fileStream, string originalPath)
    {
        string? actualPath = null;

        // Attempt to resolve path via OS Handle (Secure)
        if (OperatingSystem.IsWindows())
        {
            try
            {
                var handle = fileStream.SafeFileHandle.DangerousGetHandle();
                var pathBuilder = new StringBuilder(512);
                var result = Windows.GetFinalPathNameByHandle(
                    handle,
                    pathBuilder,
                    (uint)pathBuilder.Capacity,
                    Windows.FILE_NAME_NORMALIZED);

                if (result != 0)
                {
                    actualPath = pathBuilder.ToString();
                    if (actualPath.StartsWith(@"\\?\"))
                        actualPath = actualPath.Substring(4);
                }
            }
            catch
            {
                // If OS path resolution fails, actualPath remains null.
                // The logic below handles this by failing closed on Windows 
                // (secure) or using a fallback on Linux.
            }
        }

        // Decision Logic: Fail Closed on Windows, Fail Open (Fallback) on Linux
        if (actualPath == null)
        {
            if (OperatingSystem.IsWindows())
            {
                throw new UnauthorizedAccessException(
                    "Security Check Failed: Unable to verify file path via OS handle.");
            }
            else
            {
                actualPath = Path.GetFullPath(originalPath);
            }
        }

        ValidatePathNotContainSymlinks(actualPath);

        var isApproved = ApprovedDirectories.Any(dir =>
        {
            try
            {
                var fullDir = Path.GetFullPath(dir);
                var searchPath = fullDir + Path.DirectorySeparatorChar;
                var comparison = OperatingSystem.IsWindows() 
                    ? StringComparison.OrdinalIgnoreCase 
                    : StringComparison.Ordinal;

                return actualPath.StartsWith(searchPath, comparison) ||
                       actualPath.Equals(fullDir, comparison);
            }
            catch
            {
                // If path validation fails (e.g., path is malformed), 
                // conservatively treat it as not approved for security.
                return false;
            }
        });

        if (!isApproved)
        {
            throw new UnauthorizedAccessException(
                $"File path '{actualPath}' is outside the approved image directories: " +
                $"{string.Join(", ", ApprovedDirectories)}.");
        }

        var extension = Path.GetExtension(actualPath);
        if (!AllowedExtensions.Contains(extension))
        {
            throw new UnauthorizedAccessException(
                $"File '{actualPath}' has extension '{extension}' which is not allowed.");
        }
    }

    /// <summary>
    /// Validates that no component of the path is a symbolic link.
    /// </summary>
    private static void ValidatePathNotContainSymlinks(string fullPath)
    {
        var directory = Path.GetDirectoryName(fullPath);
        if (string.IsNullOrEmpty(directory))
            return;

        var currentDir = directory;
        var visited = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var rootPath = Path.GetPathRoot(fullPath);

        while (!string.IsNullOrEmpty(currentDir) && 
               currentDir != rootPath && 
               !visited.Contains(currentDir))
        {
            visited.Add(currentDir);

            try
            {
                var dirInfo = new DirectoryInfo(currentDir);
                
                if (OperatingSystem.IsWindows())
                {
                    var attributes = dirInfo.Attributes;
                    if ((attributes & FileAttributes.ReparsePoint) != 0)
                    {
                        throw new UnauthorizedAccessException(
                            $"Directory '{currentDir}' is a symbolic link or junction.");
                    }
                }
                else
                {
                    if (dirInfo.LinkTarget != null)
                    {
                        throw new UnauthorizedAccessException(
                            $"Directory '{currentDir}' is a symbolic link.");
                    }
                }
            }
            catch (UnauthorizedAccessException)
            {
                throw;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(
                    $"Warning: Could not check symlink status for directory '{currentDir}': {ex.Message}");
            }

            currentDir = Path.GetDirectoryName(currentDir);
        }
    }

    /// <summary>
    /// Windows API interop for resolving file paths.
    /// </summary>
    private static class Windows
    {
        internal const uint FILE_NAME_NORMALIZED = 0x0;

        [System.Runtime.InteropServices.DllImport("kernel32.dll", SetLastError = true, CharSet = System.Runtime.InteropServices.CharSet.Unicode)]
        internal static extern uint GetFinalPathNameByHandle(
            IntPtr hFile,
            StringBuilder lpszFilePath,
            uint cchFilePath,
            uint dwFlags);
    }

    /// <summary>
    /// Validates image dimensions and memory requirements.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when dimensions exceed limits.</exception>
    private static void ValidateImageDimensions(SKImageInfo info)
    {
        if (info.Width <= 0 || info.Height <= 0)
            throw new InvalidOperationException(
                $"Invalid image dimensions: {info.Width}x{info.Height}");

        if (info.Width > MaxImageWidth || info.Height > MaxImageHeight)
            throw new InvalidOperationException(
                $"Image dimensions {info.Width}x{info.Height} exceed maximum " +
                $"allowed size of {MaxImageWidth}x{MaxImageHeight}.");

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
                $"which exceeds the maximum allowed memory of {FormatBytes(MaxImageMemoryBytes)}.");
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
