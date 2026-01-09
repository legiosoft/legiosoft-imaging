namespace LegioSoft.Imaging.Core;

/// <summary>
/// Interface for resizing images.
/// </summary>
public interface ILegioImageResizer
{
    /// <summary>
    /// Resizes an image to the specified dimensions.
    /// </summary>
    /// <param name="imageData">The image data to resize.</param>
    /// <param name="width">Target width in pixels.</param>
    /// <param name="height">Target height in pixels.</param>
    /// <param name="mode">Scale mode (Fit, Fill, or Stretch).</param>
    /// <param name="quality">Resize quality (default: High).</param>
    /// <returns>Resized image data.</returns>
    byte[] Resize(byte[] imageData, int width, int height, LegioScaleMode mode, LegioResizeQuality quality = LegioResizeQuality.High);

    /// <summary>
    /// Resizes an image stream to the specified dimensions.
    /// </summary>
    /// <param name="inputStream">The input stream containing image data.</param>
    /// <param name="width">Target width in pixels.</param>
    /// <param name="height">Target height in pixels.</param>
    /// <param name="mode">Scale mode (Fit, Fill, or Stretch).</param>
    /// <param name="quality">Resize quality (default: High).</param>
    /// <param name="outputStream">Optional output stream. If null, a new stream is created.</param>
    /// <returns>Stream containing the resized image.</returns>
    Stream Resize(Stream inputStream, int width, int height, LegioScaleMode mode, LegioResizeQuality quality = LegioResizeQuality.High, Stream? outputStream = null);
}