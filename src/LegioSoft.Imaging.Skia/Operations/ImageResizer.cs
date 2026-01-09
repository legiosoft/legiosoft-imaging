using LegioSoft.Imaging.Core;
using SkiaSharp;

namespace LegioSoft.Imaging.Skia.Operations;

/// <summary>
/// Provides image resizing functionality.
/// </summary>
public static class ImageResizer
{
    /// <summary>
    /// Resizes an image from byte array and saves it.
    /// </summary>
    /// <param name="imageData">Image data to process.</param>
    /// <param name="width">Target width in pixels.</param>
    /// <param name="height">Target height in pixels.</param>
    /// <param name="mode">How to handle aspect ratio. Defaults to Fit.</param>
    /// <param name="quality">Resize quality level. Defaults to High.</param>
    /// <returns>Resized image data in the original format.</returns>
    public static byte[] Resize(byte[] imageData, int width, int height, LegioScaleMode mode = LegioScaleMode.Fit, LegioResizeQuality quality = LegioResizeQuality.High)
    {
        using var bitmap = Core.ImageLoader.LoadBitmap(imageData);
        var resized = ResizeBitmap(bitmap, width, height, quality);
        var format = FormatDetector.DetectFormat(imageData);
        return Core.ImageSaver.SaveBitmap(resized, format, 75);
    }

    /// <summary>
    /// Resizes a bitmap to the specified dimensions.
    /// </summary>
    /// <param name="source">Source bitmap to resize. Caller is responsible for disposal.</param>
    /// <param name="width">Target width in pixels.</param>
    /// <param name="height">Target height in pixels.</param>
    /// <param name="quality">Resize quality level. Defaults to High.</param>
    /// <returns>New resized bitmap. Caller is responsible for disposal.</returns>
    /// <exception cref="ArgumentNullException">Thrown when source is null.</exception>
    /// <exception cref="ArgumentException">Thrown when width or height is not positive.</exception>
    /// <remarks>
    /// Uses ScalePixels for hardware-accelerated resizing based on quality setting.
    /// </remarks>
    public static SKBitmap ResizeBitmap(SKBitmap source, int width, int height, LegioResizeQuality quality = LegioResizeQuality.High)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        if (width <= 0 || height <= 0)
            throw new ArgumentException("Width and height must be positive", nameof(width));

        var filterQuality = quality switch
        {
            LegioResizeQuality.Low => SKFilterQuality.Low,
            LegioResizeQuality.Medium => SKFilterQuality.Medium,
            LegioResizeQuality.High => SKFilterQuality.High,
            LegioResizeQuality.Maximum => SKFilterQuality.High,
            _ => SKFilterQuality.High
        };

        var scaledInfo = new SKImageInfo(width, height, source.ColorType, source.AlphaType);
        var scaledBitmap = new SKBitmap(scaledInfo);
        source.ScalePixels(scaledBitmap, filterQuality);

        return scaledBitmap;
    }
}
