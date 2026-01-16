using LegioSoft.Imaging.Core.Classes;
using LegioSoft.Imaging.Skia.Core;
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
        using var bitmap = ImageLoader.LoadBitmap(imageData);
        using var cropped = CropBitmap(bitmap, x, y, width, height);
        var format = FormatDetector.DetectFormat(imageData);
        return ImageSaver.SaveBitmap(cropped, format);
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

        // Create destination bitmap
        var cropInfo = new SKImageInfo(width, height, source.ColorType, source.AlphaType);
        var finalBitmap = new SKBitmap(cropInfo);
        if (finalBitmap.Handle == IntPtr.Zero)
            throw new InvalidOperationException("Failed to allocate bitmap memory.");

        // Draw source bitmap with negative offset to position crop region at (0,0)
        // This is the most reliable approach - creates independent pixels via canvas
        using var canvas = new SKCanvas(finalBitmap);
        using var paint = new SKPaint();

        canvas.DrawBitmap(source, -x, -y, paint);
        canvas.Flush();

        return finalBitmap;
    }
}