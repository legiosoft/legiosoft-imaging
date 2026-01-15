using LegioSoft.Imaging.Core.Classes;
using SkiaSharp;

namespace LegioSoft.Imaging.Skia.Operations;

/// <summary>
/// Provides image cropping functionality.
/// </summary>
internal static class ImageCropper
{
    /// <summary>
    /// Crops an image from byte array and saves it.
    /// </summary>
    /// <param name="imageData">Image data to process.</param>
    /// <param name="x">X coordinate of the crop origin (top-left corner).</param>
    /// <param name="y">Y coordinate of the crop origin (top-left corner).</param>
    /// <param name="width">Width of the crop area in pixels.</param>
    /// <param name="height">Height of the crop area in pixels.</param>
    /// <returns>Cropped image data in the original format.</returns>
    public static byte[] Crop(byte[] imageData, int x, int y, int width, int height)
    {
        using var bitmap = Core.ImageLoader.LoadBitmap(imageData);
        var cropped = CropBitmap(bitmap, x, y, width, height);
        var format = FormatDetector.DetectFormat(imageData);
        return Core.ImageSaver.SaveBitmap(cropped, format);
    }

    /// <summary>
    /// Crops a bitmap to the specified rectangle.
    /// </summary>
    /// <param name="source">Source bitmap to crop. Caller is responsible for disposal.</param>
    /// <param name="x">X coordinate of the crop origin (top-left corner).</param>
    /// <param name="y">Y coordinate of the crop origin (top-left corner).</param>
    /// <param name="width">Width of the crop area in pixels.</param>
    /// <param name="height">Height of the crop area in pixels.</param>
    /// <returns>New cropped bitmap. Caller is responsible for disposal.</returns>
    /// <exception cref="ArgumentNullException">Thrown when source is null.</exception>
    /// <exception cref="ArgumentException">Thrown when crop parameters are invalid.</exception>
    /// <exception cref="ArgumentException">Thrown when crop area extends beyond image bounds.</exception>
    public static SKBitmap CropBitmap(SKBitmap source, int x, int y, int width, int height)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        if (x < 0 || y < 0 || width <= 0 || height <= 0)
            throw new ArgumentException("Invalid crop parameters");

        if (x + width > source.Width || y + height > source.Height)
            throw new ArgumentException("Crop area extends beyond image bounds");

        var croppedBitmap = new SKBitmap(width, height);

        using var canvas = new SKCanvas(croppedBitmap);
        canvas.DrawBitmap(source, new SKRect(x, y, x + width, y + height), new SKPaint());
        canvas.Flush();

        return croppedBitmap;
    }
}
