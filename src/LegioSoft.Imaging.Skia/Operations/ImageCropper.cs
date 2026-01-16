using System.Diagnostics;
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
    /// <param name="x">X coordinate of crop origin (top-left corner).</param>
    /// <param name="y">Y coordinate of crop origin (top-left corner).</param>
    /// <param name="width">Width of the crop area in pixels.</param>
    /// <param name="height">Height of the crop area in pixels.</param>
    /// <returns>Cropped image data in original format.</returns>
    public static byte[] Crop(byte[] imageData, int x, int y, int width, int height)
    {
        using var bitmap = ImageLoader.LoadBitmap(imageData);
        using var cropped = CropBitmap(bitmap, x, y, width, height);
        var format = FormatDetector.DetectFormat(imageData);
        return ImageSaver.SaveBitmap(cropped, format);
    }

    /// <summary>
    /// Crops a bitmap to a specified rectangle.
    /// </summary>
    /// <param name="source">Source bitmap to crop. Caller is responsible for disposal.</param>
    /// <param name="x">X coordinate of crop origin (top-left corner).</param>
    /// <param name="y">Y coordinate of crop origin (top-left corner).</param>
    /// <param name="width">Width of the crop area in pixels.</param>
    /// <param name="height">Height of the crop area in pixels.</param>
    /// <returns>New cropped bitmap. Caller is responsible for disposal.</returns>
    /// <exception cref="ArgumentNullException">Thrown when source is null.</exception>
    /// <exception cref="ArgumentException">Thrown when crop parameters are invalid.</exception>
    /// <exception cref="ArgumentException">Thrown when crop area extends beyond image bounds.</exception>
    public static SKBitmap CropBitmap(SKBitmap source, int x, int y, int width, int height)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));

        if (x < 0 || y < 0 || width <= 0 || height <= 0)
            throw new ArgumentException("Invalid crop parameters");

        if (x >= source.Width || y >= source.Height)
            throw new ArgumentException("Crop origin is outside image bounds");
        if (width > source.Width - x || height > source.Height - y)
            throw new ArgumentException("Crop area extends beyond image bounds");

        var cropped = new SKBitmap(width, height);

        if (cropped.Handle == IntPtr.Zero)
            throw new InvalidOperationException("Failed to allocate memory for cropped bitmap");

        using var canvas = new SKCanvas(cropped);
        canvas.Clear(SKColors.Transparent);
        canvas.DrawBitmap(source, new SKPoint(-x, -y));

        return cropped;
    }
}
