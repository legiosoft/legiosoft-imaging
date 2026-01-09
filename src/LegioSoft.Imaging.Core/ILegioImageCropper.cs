namespace LegioSoft.Imaging.Core;

/// <summary>
/// Interface for cropping images.
/// </summary>
public interface ILegioImageCropper
{
    /// <summary>
    /// Crops an image to the specified rectangle.
    /// </summary>
    /// <param name="imageData">The image data to crop.</param>
    /// <param name="x">X coordinate of the top-left corner.</param>
    /// <param name="y">Y coordinate of the top-left corner.</param>
    /// <param name="width">Width of the crop rectangle.</param>
    /// <param name="height">Height of the crop rectangle.</param>
    /// <returns>Cropped image data.</returns>
    byte[] Crop(byte[] imageData, int x, int y, int width, int height);

    /// <summary>
    /// Crops an image stream to the specified rectangle.
    /// </summary>
    /// <param name="inputStream">The input stream containing image data.</param>
    /// <param name="x">X coordinate of the top-left corner.</param>
    /// <param name="y">Y coordinate of the top-left corner.</param>
    /// <param name="width">Width of the crop rectangle.</param>
    /// <param name="height">Height of the crop rectangle.</param>
    /// <param name="outputStream">Optional output stream. If null, a new stream is created.</param>
    /// <returns>Stream containing the cropped image.</returns>
    Stream Crop(Stream inputStream, int x, int y, int width, int height, Stream? outputStream = null);
}