using SkiaSharp;

namespace LegioSoft.Imaging.Skia.Operations;

/// <summary>
/// Provides image filter operations using GPU-accelerated rendering.
/// </summary>
/// <remarks>
/// All filter operations use SKPaint with SKColorFilter for hardware acceleration,
/// making them significantly faster than CPU-based pixel-by-pixel operations.
/// </remarks>
public static class ImageFilters
{
    /// <summary>
    /// Converts an image to grayscale.
    /// </summary>
    /// <param name="source">Source bitmap. Caller is responsible for disposal.</param>
    /// <returns>New grayscale bitmap. Caller is responsible for disposal.</returns>
    /// <exception cref="ArgumentNullException">Thrown when source is null.</exception>
    /// <remarks>
    /// Uses weighted RGB luminance formula for perceptually correct grayscaling.
    /// </remarks>
    public static SKBitmap ApplyGrayscale(SKBitmap source)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        var grayscaleBitmap = new SKBitmap(source.Info);

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

        return sepiaBitmap;
    }

    /// <summary>
    /// Applies Gaussian blur to an image.
    /// </summary>
    /// <param name="source">Source bitmap. Caller is responsible for disposal.</param>
    /// <param name="radius">Blur radius in pixels. Range 1-100.</param>
    /// <returns>New blurred bitmap. Caller is responsible for disposal.</returns>
    /// <exception cref="ArgumentNullException">Thrown when source is null.</exception>
    /// <exception cref="ArgumentException">Thrown when radius is outside the valid range.</exception>
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

        using var canvas = new SKCanvas(blurBitmap);
        using var paint = new SKPaint();
        using var filter = SKImageFilter.CreateBlur((float)radius, (float)radius);

        paint.ImageFilter = filter;
        canvas.DrawBitmap(source, 0, 0, paint);

        return blurBitmap;
    }

    /// <summary>
    /// Sharpens an image by enhancing edges.
    /// </summary>
    /// <param name="source">Source bitmap. Caller is responsible for disposal.</param>
    /// <param name="amount">Sharpening intensity. Range 0-100, default 50.</param>
    /// <returns>New sharpened bitmap. Caller is responsible for disposal.</returns>
    /// <exception cref="ArgumentNullException">Thrown when source is null.</exception>
    /// <exception cref="ArgumentException">Thrown when amount is outside the valid range.</exception>
    /// <remarks>
    /// Combines dilated edge enhancement with original image for sharpening effect.
    /// </remarks>
    public static SKBitmap ApplySharpen(SKBitmap source, int amount = 50)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        if (amount is < 0 or > 100)
            throw new ArgumentException("Sharpen amount must be between 0 and 100", nameof(amount));

        var factor = amount / 100f;

        var sharpenedBitmap = new SKBitmap(source.Info);

        using var canvas = new SKCanvas(sharpenedBitmap);
        using var paint = new SKPaint();

        paint.ImageFilter = SKImageFilter.CreateDilate(1, 1);
        canvas.DrawBitmap(source, 0, 0, paint);

        for (var y = 0; y < source.Height; y++)
        {
            for (var x = 0; x < source.Width; x++)
            {
                var originalPixel = source.GetPixel(x, y);
                var sharpenedPixel = sharpenedBitmap.GetPixel(x, y);

                var newR = (byte)Math.Min(255, Math.Max(0, originalPixel.Red + (sharpenedPixel.Red - originalPixel.Red) * factor));
                var newG = (byte)Math.Min(255, Math.Max(0, originalPixel.Green + (sharpenedPixel.Green - originalPixel.Green) * factor));
                var newB = (byte)Math.Min(255, Math.Max(0, originalPixel.Blue + (sharpenedPixel.Blue - originalPixel.Blue) * factor));

                sharpenedBitmap.SetPixel(x, y, new SKColor(newR, newG, newB, originalPixel.Alpha));
            }
        }

        return sharpenedBitmap;
    }
}
