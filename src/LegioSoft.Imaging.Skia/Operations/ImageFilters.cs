using SkiaSharp;

namespace LegioSoft.Imaging.Skia.Operations;

/// <summary>
/// Provides image filter operations using GPU-accelerated rendering.
/// </summary>
/// <remarks>
/// All filter operations use SKPaint with SKColorFilter for hardware acceleration,
/// making them significantly faster than CPU-based pixel-by-pixel operations.
/// </remarks>
internal static class ImageFilters
{
    /// <summary>
    /// Converts an image to grayscale.
    /// </summary>
    /// <param name="source">Source bitmap. Caller is responsible for disposal.</param>
    /// <returns>New grayscale bitmap. Caller is responsible for disposal.</returns>
    /// <exception cref="ArgumentNullException">Thrown when source is null.</exception>
    /// <remarks>
    /// Uses weighted RGB luminance formula for perceptually correct gray scaling.
    /// </remarks>
    public static SKBitmap ApplyGrayscale(SKBitmap source)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        var grayscaleBitmap = new SKBitmap(source.Info);
        if (grayscaleBitmap.Handle == IntPtr.Zero)
            throw new InvalidOperationException("Failed to allocate bitmap memory.");

        using var canvas = new SKCanvas(grayscaleBitmap);
        using var paint = new SKPaint();

        paint.ColorFilter = SKColorFilter.CreateColorMatrix(new float[]
        {
            0.299f, 0.587f, 0.114f, 0, 0,
            0.299f, 0.587f, 0.114f, 0, 0,
            0.299f, 0.587f, 0.114f, 0, 0,
            0,      0,      0,      1, 0
        });

        canvas.DrawBitmap(source, 0, 0, paint);
        canvas.Flush();

        return grayscaleBitmap;
    }

    /// <summary>
    /// Applies sepia toning to an image.
    /// </summary>
    /// <param name="source">Source bitmap. Caller is responsible for disposal.</param>
    /// <returns>New sepia-toned bitmap. Caller is responsible for disposal.</returns>
    /// <exception cref="ArgumentNullException">Thrown when source is null.</exception>
    public static SKBitmap ApplySepia(SKBitmap source)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        var sepiaBitmap = new SKBitmap(source.Info);
        if (sepiaBitmap.Handle == IntPtr.Zero)
            throw new InvalidOperationException("Failed to allocate bitmap memory.");

        using var canvas = new SKCanvas(sepiaBitmap);
        using var paint = new SKPaint();

        paint.ColorFilter = SKColorFilter.CreateColorMatrix(new float[]
        {
            0.393f, 0.769f, 0.189f, 0, 0,
            0.349f, 0.686f, 0.168f, 0, 0,
            0.272f, 0.534f, 0.131f, 0, 0,
            0,      0,      0,      1, 0
        });

        canvas.DrawBitmap(source, 0, 0, paint);
        canvas.Flush();

        return sepiaBitmap;
    }

    /// <summary>
    /// Applies Gaussian blur to an image.
    /// </summary>
    /// <param name="source">Source bitmap. Caller is responsible for disposal.</param>
    /// <param name="radius">Blur radius in pixels. Range 1-100.</param>
    /// <returns>New blurred bitmap. Caller is responsible for disposal.</returns>
    /// <exception cref="ArgumentNullException">Thrown when source is null.</exception>
    /// <exception cref="ArgumentException">Thrown when radius is outside of valid range.</exception>
    /// <remarks>
    /// Uses hardware-accelerated blur filter. Larger values create softer blur
    /// but increase processing time significantly.
    /// </remarks>
    public static SKBitmap ApplyBlur(SKBitmap source, int radius = 5)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        if (radius is <= 0 or > 100)
            throw new ArgumentException("Blur radius must be between 1 and 100", nameof(radius));

        var blurBitmap = new SKBitmap(source.Info);
        if (blurBitmap.Handle == IntPtr.Zero)
            throw new InvalidOperationException("Failed to allocate bitmap memory.");

        using var canvas = new SKCanvas(blurBitmap);
        using var paint = new SKPaint();
        using var filter = SKImageFilter.CreateBlur((float)radius, (float)radius);

        paint.ImageFilter = filter;
        canvas.DrawBitmap(source, 0, 0, paint);
        canvas.Flush();

        return blurBitmap;
    }

    /// <summary>
    /// Sharpens an image by enhancing edges.
    /// </summary>
    /// <param name="source">Source bitmap. Caller is responsible for disposal.</param>
    /// <param name="amount">Sharpening intensity. Range 0-100, default 50.</param>
    /// <returns>New sharpened bitmap. Caller is responsible for disposal.</returns>
    /// <exception cref="ArgumentNullException">Thrown when source is null.</exception>
    /// <exception cref="ArgumentException">Thrown when amount is outside of valid range.</exception>
    /// <remarks>
    /// Uses hardware-accelerated 3x3 convolution matrix for fast sharpening.
    /// </remarks>
    public static SKBitmap ApplySharpen(SKBitmap source, int amount = 50)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        if (amount is < 0 or > 100)
            throw new ArgumentException("Sharpen amount must be between 0 and 100", nameof(amount));

        var factor = amount / 100f;
        var centerValue = 1 + 4 * factor;
        var edgeValue = -factor;

        var sharpenedBitmap = new SKBitmap(source.Info);
        if (sharpenedBitmap.Handle == IntPtr.Zero)
            throw new InvalidOperationException("Failed to allocate bitmap memory.");

        using var canvas = new SKCanvas(sharpenedBitmap);
        using var paint = new SKPaint();
        using var filter = SKImageFilter.CreateMatrixConvolution(
            new SKSizeI(3, 3),
            new float[]
            {
                0, edgeValue, 0,
                edgeValue, centerValue, edgeValue,
                0, edgeValue, 0
            },
            1, 0, new SKPointI(1, 1), SKShaderTileMode.Clamp, true);

        paint.ImageFilter = filter;
        canvas.DrawBitmap(source, 0, 0, paint);
        canvas.Flush();

        return sharpenedBitmap;
    }
}
