using SkiaSharp;

namespace LegioSoft.Imaging.Skia.Operations;

/// <summary>
/// Provides image color adjustment operations using GPU-accelerated rendering.
/// </summary>
/// <remarks>
/// All adjustment operations use SKPaint with SKColorFilter for hardware acceleration,
/// making them significantly faster than CPU-based pixel-by-pixel operations.
/// </remarks>
internal static class ImageColorAdjustments
{
    /// <summary>
    /// Adjusts the brightness of an image.
    /// </summary>
    /// <param name="source">Source bitmap. Caller is responsible for disposal.</param>
    /// <param name="amount">Brightness adjustment. Range -255 to 255, default 0.</param>
    /// <returns>New brightness-adjusted bitmap. Caller is responsible for disposal.</returns>
    /// <exception cref="ArgumentNullException">Thrown when source is null.</exception>
    /// <exception cref="ArgumentException">Thrown when amount is outside the valid range.</exception>
    /// <remarks>
    /// Positive values lighten the image, negative values darken it.
    /// </remarks>
    public static SKBitmap ApplyBrightness(SKBitmap source, int amount)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        if (amount < -255 || amount > 255)
            throw new ArgumentException("Brightness amount must be between -255 and 255.", nameof(amount));

        if (amount == 0)
        {
            var copy = new SKBitmap(source.Info);
            if (copy.Handle == IntPtr.Zero)
                throw new InvalidOperationException("Failed to allocate bitmap memory.");
            source.CopyTo(copy, source.Info.ColorType);
            return copy;
        }

        var brightnessBitmap = new SKBitmap(source.Info);
        if (brightnessBitmap.Handle == IntPtr.Zero)
            throw new InvalidOperationException("Failed to allocate bitmap memory.");

        using var canvas = new SKCanvas(brightnessBitmap);
        using var paint = new SKPaint();

        float delta = amount;

        paint.ColorFilter = SKColorFilter.CreateColorMatrix(new float[]
        {
            1, 0, 0, 0, delta,
            0, 1, 0, 0, delta,
            0, 0, 1, 0, delta,
            0, 0, 0, 1, 0
        });

        canvas.DrawBitmap(source, 0, 0, paint);
        canvas.Flush();

        return brightnessBitmap;
    }

    /// <summary>
    /// Adjusts the contrast of an image.
    /// </summary>
    /// <param name="source">Source bitmap. Caller is responsible for disposal.</param>
    /// <param name="amount">Contrast adjustment. Range -100 to 100, default 0.</param>
    /// <returns>New contrast-adjusted bitmap. Caller is responsible for disposal.</returns>
    /// <exception cref="ArgumentNullException">Thrown when source is null.</exception>
    /// <exception cref="ArgumentException">Thrown when amount is outside the valid range.</exception>
    /// <remarks>
    /// Positive values increase contrast, negative values decrease it.
    /// </remarks>
    public static SKBitmap ApplyContrast(SKBitmap source, int amount)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        if (amount < -100 || amount > 100)
            throw new ArgumentException("Contrast amount must be between -100 and 100.", nameof(amount));

        if (amount == 0)
        {
            var copy = new SKBitmap(source.Info);
            if (copy.Handle == IntPtr.Zero)
                throw new InvalidOperationException("Failed to allocate bitmap memory.");
            source.CopyTo(copy, source.Info.ColorType);
            return copy;
        }

        var contrastBitmap = new SKBitmap(source.Info);
        if (contrastBitmap.Handle == IntPtr.Zero)
            throw new InvalidOperationException("Failed to allocate bitmap memory.");

        using var canvas = new SKCanvas(contrastBitmap);
        using var paint = new SKPaint();

        float factor = (259 * (amount + 255)) / (255 * (259 - amount));
        float intercept = 128 * (1 - factor);

        paint.ColorFilter = SKColorFilter.CreateColorMatrix(new float[]
        {
            factor, 0,      0,      0, intercept,
            0,      factor, 0,      0, intercept,
            0,      0,      factor, 0, intercept,
            0,      0,      0,      1, 0
        });

        canvas.DrawBitmap(source, 0, 0, paint);
        canvas.Flush();

        return contrastBitmap;
    }

    /// <summary>
    /// Inverts all colors in an image.
    /// </summary>
    /// <param name="source">Source bitmap. Caller is responsible for disposal.</param>
    /// <returns>New inverted bitmap. Caller is responsible for disposal.</returns>
    /// <exception cref="ArgumentNullException">Thrown when source is null.</exception>
    /// <remarks>
    /// Creates a negative image effect (photographic negative).
    /// </remarks>
    public static SKBitmap ApplyInvert(SKBitmap source)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        var invertBitmap = new SKBitmap(source.Info);
        if (invertBitmap.Handle == IntPtr.Zero)
            throw new InvalidOperationException("Failed to allocate bitmap memory.");

        using var canvas = new SKCanvas(invertBitmap);
        using var paint = new SKPaint();

        paint.ColorFilter = SKColorFilter.CreateColorMatrix(new float[]
        {
            -1, 0, 0, 0, 255,
            0, -1, 0, 0, 255,
            0, 0, -1, 0, 255,
            0, 0, 0, 1, 0
        });

        canvas.DrawBitmap(source, 0, 0, paint);
        canvas.Flush();

        return invertBitmap;
    }
}
