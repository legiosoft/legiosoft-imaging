using LegioSoft.Imaging.Core;
using LegioSoft.Imaging.Skia.Core;
using LegioSoft.Imaging.Skia.Operations;
using SkiaSharp;

namespace LegioSoft.Imaging.Skia;

/// <summary>
/// Fluent builder for image processing operations with automatic memory management.
/// </summary>
/// <remarks>
/// Operations are executed in exact order they're chained. For example,
/// .Rotate(90).Crop(10, 10, 100, 100) rotates the original image first,
/// then crops the rotated result. All intermediate bitmaps are automatically disposed
/// via swap-and-dispose pattern. Only call .Save() or .SaveAs() to ensure
/// the final bitmap is properly released.
/// </remarks>
/// <example>
/// <code>
/// // Resize and enhance photo
/// var result = LegioImageBuilder.Load("photo.jpg")
///     .Resize(1920, 1080, LegioScaleMode.Fit)
///     .Brightness(30)
///     .Contrast(20)
///     .Sharpen(60)
///     .Save("enhanced.jpg", quality: 90);
/// </code>
/// </example>
public class LegioImageBuilder
{
    private readonly byte[] _imageData;
    private readonly LegioImageFormat _format;
    private readonly List<Func<SKBitmap, SKBitmap>> _operations;
    private int _saveQuality = 75;

    private readonly int _virtualWidth;
    private readonly int _virtualHeight;

    private LegioImageBuilder(byte[] imageData)
    {
        _imageData = imageData ?? throw new ArgumentNullException(nameof(imageData));
        _format = FormatDetector.DetectFormat(imageData);
        _operations = new List<Func<SKBitmap, SKBitmap>>();
        _virtualWidth = 0;
        _virtualHeight = 0;
    }

    /// <summary>
    /// Creates a builder from an image byte array.
    /// </summary>
    /// <param name="imageData">Image data in a supported format (PNG, JPEG, WebP, BMP, GIF).</param>
    /// <returns>A new builder instance configured with the image data.</returns>
    /// <exception cref="ArgumentNullException">Thrown when imageData is null.</exception>
    public static LegioImageBuilder Load(byte[] imageData)
    {
        return new LegioImageBuilder(imageData);
    }

    /// <summary>
    /// Creates a builder from a file path.
    /// </summary>
    /// <param name="filePath">Path to the image file.</param>
    /// <returns>A new builder instance loaded with the image file.</returns>
    /// <exception cref="FileNotFoundException">Thrown when file does not exist.</exception>
    public static LegioImageBuilder Load(string filePath)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"File not found: {filePath}", filePath);
        return Load(File.ReadAllBytes(filePath));
    }

    /// <summary>
    /// Creates a builder from a stream.
    /// </summary>
    /// <param name="stream">A readable stream containing image data.</param>
    /// <returns>A new builder instance loaded with the stream data.</returns>
    /// <exception cref="ArgumentNullException">Thrown when stream is null.</exception>
    /// <remarks>
    /// The entire stream is read into memory. For large files or streams that
    /// cannot be reused, consider loading to byte array first.
    /// </remarks>
    public static LegioImageBuilder Load(Stream stream)
    {
        if (stream == null) throw new ArgumentNullException(nameof(stream));
        using var ms = new MemoryStream();
        stream.CopyTo(ms);
        return Load(ms.ToArray());
    }

    /// <summary>
    /// Adds a resize operation to the processing chain.
    /// </summary>
    /// <param name="width">Target width in pixels.</param>
    /// <param name="height">Target height in pixels.</param>
    /// <param name="mode">How to handle aspect ratio. Defaults to Fit.</param>
    /// <param name="quality">Resize quality level. Defaults to High.</param>
    /// <returns>This builder for method chaining.</returns>
    /// <exception cref="ArgumentException">Thrown when width or height is not positive.</exception>
    /// <remarks>
    /// Use ResizeToWidth or ResizeToHeight to automatically maintain aspect ratio.
    /// </remarks>
    public LegioImageBuilder Resize(int width, int height, LegioScaleMode mode = LegioScaleMode.Fit, LegioResizeQuality quality = LegioResizeQuality.High)
    {
        if (width <= 0 || height <= 0)
            throw new ArgumentException("Width and height must be positive", nameof(width));
        
        _operations.Add(source => ImageResizer.ResizeBitmap(source, width, height, quality));
        return this;
    }

    /// <summary>
    /// Resizes image to a specified width while maintaining aspect ratio.
    /// </summary>
    /// <param name="width">Target width in pixels.</param>
    /// <param name="quality">Resize quality level. Defaults to High.</param>
    /// <returns>This builder for method chaining.</returns>
    /// <exception cref="ArgumentException">Thrown when width is not positive.</exception>
    /// <remarks>
    /// Uses virtual dimensions (updated by previous operations) for calculations instead of GetInfo().
    /// </remarks>
    public LegioImageBuilder ResizeToWidth(int width, LegioResizeQuality quality = LegioResizeQuality.High)
    {
        if (width <= 0)
            throw new ArgumentException("Width must be positive", nameof(width));
        
        var ratio = (double)width / _virtualWidth;
        var newHeight = (int)(_virtualHeight * ratio);
        
        _operations.Add(source => ImageResizer.ResizeBitmap(source, width, newHeight, quality));
        return this;
    }

    /// <summary>
    /// Resizes image to a specified height while maintaining aspect ratio.
    /// </summary>
    /// <param name="height">Target height in pixels.</param>
    /// <param name="quality">Resize quality level. Defaults to High.</param>
    /// <returns>This builder for method chaining.</returns>
    /// <exception cref="ArgumentException">Thrown when height is not positive.</exception>
    /// <remarks>
    /// Uses virtual dimensions (updated by previous operations) for calculations instead of GetInfo().
    /// </remarks>
    public LegioImageBuilder ResizeToHeight(int height, LegioResizeQuality quality = LegioResizeQuality.High)
    {
        if (height <= 0)
            throw new ArgumentException("Height must be positive", nameof(height));
        
        var ratio = (double)height / _virtualHeight;
        var newWidth = (int)(_virtualWidth * ratio);
        
        _operations.Add(source => ImageResizer.ResizeBitmap(source, newWidth, height, quality));
        return this;
    }

    /// <summary>
    /// Scales image by a factor while maintaining aspect ratio.
    /// </summary>
    /// <param name="factor">Scale multiplier. 1.0 = original size, 0.5 = half size.</param>
    /// <param name="quality">Resize quality level. Defaults to High.</param>
    /// <returns>This builder for method chaining.</returns>
    /// <exception cref="ArgumentException">Thrown when factor is not positive.</exception>
    /// <remarks>
    /// Uses virtual dimensions (updated by previous operations) for calculations instead of GetInfo().
    /// </remarks>
    public LegioImageBuilder Scale(double factor, LegioResizeQuality quality = LegioResizeQuality.High)
    {
        if (factor <= 0)
            throw new ArgumentException("Scale factor must be positive", nameof(factor));
        
        var newWidth = (int)(_virtualWidth * factor);
        var newHeight = (int)(_virtualHeight * factor);
        
        _operations.Add(source => ImageResizer.ResizeBitmap(source, newWidth, newHeight, quality));
        return this;
    }

    /// <summary>
    /// Adds a crop operation to the processing chain.
    /// </summary>
    /// <param name="x">X coordinate of the crop origin (top-left corner).</param>
    /// <param name="y">Y coordinate of the crop origin (top-left corner).</param>
    /// <param name="width">Width of the crop area in pixels.</param>
    /// <param name="height">Height of the crop area in pixels.</param>
    /// <returns>This builder for method chaining.</returns>
    /// <exception cref="ArgumentException">Thrown when width or height is not positive.</exception>
    public LegioImageBuilder Crop(int x, int y, int width, int height)
    {
        if (width <= 0 || height <= 0)
            throw new ArgumentException("Width and height must be positive", nameof(width));
        
        _operations.Add(source => ImageCropper.CropBitmap(source, x, y, width, height));
        return this;
    }

    /// <summary>
    /// Adds a rotation operation to the processing chain.
    /// </summary>
    /// <param name="degrees">Rotation angle. Must be 0, 90, 180, or 270.</param>
    /// <returns>This builder for method chaining.</returns>
    /// <exception cref="ArgumentException">Thrown when degrees is not a valid rotation value.</exception>
    public LegioImageBuilder Rotate(int degrees)
    {
        if (degrees != 0 && degrees != 90 && degrees != 180 && degrees != 270)
            throw new ArgumentException("Rotation must be 0, 90, 180, or 270 degrees", nameof(degrees));

        if (degrees != 0)
        {
            _operations.Add(source => ImageTransformer.Rotate(source, degrees));
        }
        
        return this;
    }

    /// <summary>
    /// Adds a flip operation to the processing chain.
    /// </summary>
    /// <param name="horizontal">Mirror horizontally. Defaults to true.</param>
    /// <param name="vertical">Mirror vertically. Defaults to false.</param>
    /// <returns>This builder for method chaining.</returns>
    public LegioImageBuilder Flip(bool horizontal = true, bool vertical = false)
    {
        if (horizontal || vertical)
        {
            _operations.Add(source => ImageTransformer.Flip(source, horizontal, vertical));
        }
        
        return this;
    }

    /// <summary>
    /// Converts an image to grayscale.
    /// </summary>
    /// <returns>This builder for method chaining.</returns>
    /// <remarks>
    /// Uses GPU-accelerated color matrix filtering for performance.
    /// </remarks>
    public LegioImageBuilder Grayscale()
    {
        _operations.Add(ImageFilters.ApplyGrayscale);
        return this;
    }

    /// <summary>
    /// Applies sepia toning to an image.
    /// </summary>
    /// <returns>This builder for method chaining.</returns>
    /// <remarks>
    /// Uses GPU-accelerated color matrix filtering for performance.
    /// </remarks>
    public LegioImageBuilder Sepia()
    {
        _operations.Add(ImageFilters.ApplySepia);
        return this;
    }

    /// <summary>
    /// Applies a Gaussian blur to an image.
    /// </summary>
    /// <param name="radius">Blur radius in pixels. Range 1-20, default 5.</param>
    /// <returns>This builder for method chaining.</returns>
    /// <exception cref="ArgumentException">Thrown when radius is outside the valid range.</exception>
    /// <remarks>
    /// Uses hardware-accelerated blur filtering. Larger values create softer blur
    /// but increase processing time.
    /// </remarks>
    public LegioImageBuilder Blur(int radius = 5)
    {
        if (radius <= 0 || radius > 20)
            throw new ArgumentException("Blur radius must be between 1 and 20", nameof(radius));

        _operations.Add(source => ImageFilters.ApplyBlur(source, radius));
        return this;
    }

    /// <summary>
    /// Sharpens an image.
    /// </summary>
    /// <param name="amount">Sharpening intensity. Range 0-100, default 50.</param>
    /// <returns>This builder for method chaining.</returns>
    /// <exception cref="ArgumentException">Thrown when amount is outside the valid range.</exception>
    public LegioImageBuilder Sharpen(int amount = 50)
    {
        if (amount < 0 || amount > 100)
            throw new ArgumentException("Sharpen amount must be between 0 and 100", nameof(amount));

        _operations.Add(source => ImageFilters.ApplySharpen(source, amount));
        return this;
    }

    /// <summary>
    /// Adjusts the image brightness.
    /// </summary>
    /// <param name="amount">Brightness adjustment. Range -255 to 255, default 0.</param>
    /// <returns>This builder for method chaining.</returns>
    /// <exception cref="ArgumentException">Thrown when amount is outside the valid range.</exception>
    /// <remarks>
    /// Positive values lighten the image, negative values darken it.
    /// Uses GPU-accelerated color matrix filtering for performance.
    /// </remarks>
    public LegioImageBuilder Brightness(int amount)
    {
        if (amount < -255 || amount > 255)
            throw new ArgumentException("Brightness must be between -255 and 255", nameof(amount));

        _operations.Add(source => ImageColorAdjustments.ApplyBrightness(source, amount));
        return this;
    }

    /// <summary>
    /// Adjusts the image contrast.
    /// </summary>
    /// <param name="amount">Contrast adjustment. Range -100 to 100, default 0.</param>
    /// <returns>This builder for method chaining.</returns>
    /// <exception cref="ArgumentException">Thrown when amount is outside the valid range.</exception>
    /// <remarks>
    /// Positive values increase contrast, negative values decrease it.
    /// Uses GPU-accelerated color matrix filtering for performance.
    /// </remarks>
    public LegioImageBuilder Contrast(int amount)
    {
        if (amount is < -100 || amount > 100)
            throw new ArgumentException("Contrast must be between -100 and 100", nameof(amount));

        _operations.Add(source => ImageColorAdjustments.ApplyContrast(source, amount));
        return this;
    }

    /// <summary>
    /// Inverts all colors in the image.
    /// </summary>
    /// <returns>This builder for method chaining.</returns>
    /// <remarks>
    /// Creates a negative image effect (photographic negative).
    /// Uses GPU-accelerated color matrix filtering for performance.
    /// </remarks>
    public LegioImageBuilder Invert()
    {
        _operations.Add(ImageColorAdjustments.ApplyInvert);
        return this;
    }

    /// <summary>
    /// Sets the quality level for subsequent save operations.
    /// </summary>
    /// <param name="quality">Encoding quality. Range 0-100, default 75.</param>
    /// <returns>This builder for method chaining.</returns>
    /// <exception cref="ArgumentException">Thrown when quality is outside the valid range.</exception>
    /// <remarks>
    /// Quality effects vary by format:
    /// - JPEG/WebP: Lower = smaller files with more artifacts
    /// - PNG/BMP/GIF: Always 100 (lossless), parameter ignored
    /// Recommended ranges: JPEG 70-85, WebP 80-90
    /// </remarks>
    public LegioImageBuilder Quality(int quality)
    {
        if (quality < 0 || quality > 100)
            throw new ArgumentException("Quality must be between 0 and 100", nameof(quality));

        _saveQuality = quality;
        return this;
    }

    /// <summary>
    /// Executes all operations and saves the result as a byte array.
    /// </summary>
    /// <param name="format">Target image format.</param>
    /// <param name="quality">Encoding quality (0-100). Overrides builder default.</param>
    /// <returns>Encoded image data.</returns>
    /// <remarks>
    /// The final bitmap is automatically disposed after encoding.
    /// </remarks>
    public byte[] SaveAs(LegioImageFormat format, int? quality = null)
    {
        var finalQuality = quality ?? _saveQuality;
        using var result = ApplyOperations();
        return ImageSaver.SaveBitmap(result, format, finalQuality);
    }

    /// <summary>
    /// Executes all operations and saves to a file.
    /// </summary>
    /// <param name="filePath">Output file path.</param>
    /// <param name="format">Target format. Defaults to the original format.</param>
    /// <param name="quality">Encoding quality (0-100). Overrides builder default.</param>
    public void Save(string filePath, LegioImageFormat? format = null, int? quality = null)
    {
        var targetFormat = format ?? _format;
        var data = SaveAs(targetFormat, quality);
        File.WriteAllBytes(filePath, data);
    }

    /// <summary>
    /// Executes all operations and returns an encoded stream.
    /// </summary>
    /// <param name="format">Target image format.</param>
    /// <param name="quality">Encoding quality (0-100). Overrides builder default.</param>
    /// <returns>A MemoryStream containing the encoded image data.</returns>
    /// <remarks>
    /// The caller is responsible for disposing the returned stream.
    /// </remarks>
    public Stream SaveAsStream(LegioImageFormat format, int? quality = null)
    {
        var data = SaveAs(format, quality);
        return new MemoryStream(data);
    }

    /// <summary>
    /// Gets image metadata without full decoding.
    /// </summary>
    /// <returns>Image information including dimensions, format, and size.</returns>
    /// <remarks>
    /// Uses SKCodec for efficient header-only reading, avoiding full pixel decoding.
    /// </remarks>
    public LegioImageInfo GetInfo()
    {
        return ImageMetadataReader.GetInfo(_imageData);
    }

    private SKBitmap ApplyOperations()
    {
        SKBitmap? currentBitmap = null;

        try
        {
            currentBitmap = ImageLoader.LoadBitmap(_imageData);

            foreach (var operation in _operations)
            {
                using var previousBitmap = currentBitmap;
                currentBitmap = operation(currentBitmap);
            }

            return currentBitmap;
        }
        catch
        {
            currentBitmap?.Dispose();
            throw;
        }
    }
}
