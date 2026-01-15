using SkiaSharp;

namespace LegioSoft.Imaging.Skia.Operations;

/// <summary>
/// Provides image transformation functionality (rotate, flip).
/// </summary>
internal static class ImageTransformer
{
    /// <summary>
    /// Rotates a bitmap by 90, 180, or 270 degrees.
    /// </summary>
    /// <param name="source">Source bitmap to rotate. Caller is responsible for disposal.</param>
    /// <param name="degrees">Rotation angle. Must be 90, 180, or 270.</param>
    /// <returns>New rotated bitmap. Caller is responsible for disposal.</returns>
    /// <exception cref="ArgumentNullException">Thrown when source is null.</exception>
    /// <exception cref="ArgumentException">Thrown when degrees is not a valid rotation value.</exception>
    /// <remarks>
    /// For 90 and 270 degree rotations, width and height are swapped.
    /// </remarks>
    public static SKBitmap Rotate(SKBitmap source, int degrees)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        if (degrees != 90 && degrees != 180 && degrees != 270)
            throw new ArgumentException("Rotation must be 90, 180, or 270 degrees", nameof(degrees));

        var radians = degrees * Math.PI / 180;
        SKImageInfo rotatedInfo;

        if (degrees is 90 or 270)
        {
            rotatedInfo = new SKImageInfo(
                source.Height,
                source.Width,
                source.ColorType,
                source.AlphaType);
        }
        else
        {
            rotatedInfo = new SKImageInfo(
                source.Width,
                source.Height,
                source.ColorType,
                source.AlphaType);
        }

        var rotatedBitmap = new SKBitmap(rotatedInfo);

        using var canvas = new SKCanvas(rotatedBitmap);
        using var paint = new SKPaint();
        var matrix = SKMatrix.CreateRotation((float)radians);
        var center = new SKPoint(rotatedBitmap.Width / 2f, rotatedBitmap.Height / 2f);
        canvas.Translate(center.X, center.Y);
        canvas.Concat(ref matrix);
        canvas.DrawBitmap(source, -source.Width / 2f, -source.Height / 2f, paint);

        return rotatedBitmap;
    }

    /// <summary>
    /// Flips a bitmap horizontally and/or vertically.
    /// </summary>
    /// <param name="source">Source bitmap to flip. Caller is responsible for disposal.</param>
    /// <param name="horizontal">Mirror horizontally.</param>
    /// <param name="vertical">Mirror vertically.</param>
    /// <returns>New flipped bitmap. Caller is responsible for disposal.</returns>
    /// <exception cref="ArgumentNullException">Thrown when source is null.</exception>
    public static SKBitmap Flip(SKBitmap source, bool horizontal, bool vertical)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        var flippedBitmap = new SKBitmap(source.Info);

        using var canvas = new SKCanvas(flippedBitmap);
        if (horizontal && vertical)
        {
            canvas.DrawBitmap(source, -source.Width, -source.Height);
        }
        else if (horizontal)
        {
            canvas.DrawBitmap(source, -source.Width, 0);
        }
        else if (vertical)
        {
            canvas.DrawBitmap(source, 0, -source.Height);
        }
        else
        {
            canvas.DrawBitmap(source, 0, 0);
        }

        return flippedBitmap;
    }
}
