using LegioSoft.Imaging.Core.Classes;
using LegioSoft.Imaging.Core.Enums;
using LegioSoft.Imaging.Skia.Core;
using SkiaSharp;

namespace LegioSoft.Imaging.Skia.Operations;

    /// <summary>
    /// Provides image resizing functionality.
    /// </summary>
    internal static class ImageResizer
    {
        /// <summary>
        /// Calculates target dimensions based on scale mode.
        /// </summary>
        private static (int width, int height) CalculateTargetDimensions(int sourceWidth, int sourceHeight, int targetWidth, int targetHeight, LegioScaleMode mode)
        {
            return mode switch
            {
                LegioScaleMode.Fit => CalculateFitDimensions(sourceWidth, sourceHeight, targetWidth, targetHeight),
                LegioScaleMode.Fill => CalculateFillDimensions(sourceWidth, sourceHeight, targetWidth, targetHeight),
                LegioScaleMode.Stretch => (targetWidth, targetHeight),
                _ => (targetWidth, targetHeight)
            };
        }

        /// <summary>
        /// Calculates dimensions to fit within bounds while maintaining aspect ratio.
        /// </summary>
        private static (int width, int height) CalculateFitDimensions(int sourceWidth, int sourceHeight, int maxWidth, int maxHeight)
        {
            double sourceRatio = (double)sourceWidth / sourceHeight;
            double targetRatio = (double)maxWidth / maxHeight;

            if (sourceRatio > targetRatio)
            {
                return (maxWidth, (int)(maxWidth / sourceRatio));
            }
            else
            {
                return ((int)(maxHeight * sourceRatio), maxHeight);
            }
        }

        /// <summary>
        /// Calculates dimensions to fill the bounds while maintaining aspect ratio.
        /// </summary>
        private static (int width, int height) CalculateFillDimensions(int sourceWidth, int sourceHeight, int minWidth, int minHeight)
        {
            double sourceRatio = (double)sourceWidth / sourceHeight;
            double targetRatio = (double)minWidth / minHeight;

            if (sourceRatio < targetRatio)
            {
                return (minWidth, (int)(minWidth / sourceRatio));
            }
            else
            {
                return ((int)(minHeight * sourceRatio), minHeight);
            }
        }

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
        using var bitmap = ImageLoader.LoadBitmap(imageData);
        var (targetWidth, targetHeight) = CalculateTargetDimensions(bitmap.Width, bitmap.Height, width, height, mode);
        using var resized = ResizeBitmap(bitmap, targetWidth, targetHeight, quality);
        var format = FormatDetector.DetectFormat(imageData);
        return ImageSaver.SaveBitmap(resized, format, 90);
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

        if (scaledBitmap.Handle == IntPtr.Zero)
            throw new InvalidOperationException("Failed to allocate bitmap memory");

        source.ScalePixels(scaledBitmap, filterQuality);

        return scaledBitmap;
    }
}
