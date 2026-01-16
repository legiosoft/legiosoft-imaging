namespace LegioSoft.Imaging.Core.Interfaces;

/// <summary>
/// Interface for transforming images (rotation and flipping).
/// </summary>
public interface ILegioImageTransformer
{
    /// <summary>
    /// Rotates the image by the specified number of degrees.
    /// </summary>
    /// <param name="imageData">The image data to rotate.</param>
    /// <param name="degrees">Rotation angle in degrees. Common values: 90, 180, 270.</param>
    /// <returns>Rotated image data.</returns>
    byte[] Rotate(byte[] imageData, int degrees);

    /// <summary>
    /// Rotates the image stream by the specified number of degrees.
    /// </summary>
    /// <param name="inputStream">The input stream containing image data.</param>
    /// <param name="degrees">Rotation angle in degrees. Common values: 90, 180, 270.</param>
    /// <param name="outputStream">Optional output stream. If null, a new stream is created.</param>
    /// <returns>Stream containing the rotated image.</returns>
    Stream Rotate(Stream inputStream, int degrees, Stream? outputStream = null);

    /// <summary>
    /// Flips the image horizontally and/or vertically.
    /// </summary>
    /// <param name="imageData">The image data to flip.</param>
    /// <param name="horizontal">Whether to flip horizontally.</param>
    /// <param name="vertical">Whether to flip vertically.</param>
    /// <returns>Flipped image data.</returns>
    byte[] Flip(byte[] imageData, bool horizontal, bool vertical);

    /// <summary>
    /// Flips the image stream horizontally and/or vertically.
    /// </summary>
    /// <param name="inputStream">The input stream containing image data.</param>
    /// <param name="horizontal">Whether to flip horizontally.</param>
    /// <param name="vertical">Whether to flip vertically.</param>
    /// <param name="outputStream">Optional output stream. If null, a new stream is created.</param>
    /// <returns>Stream containing the flipped image.</returns>
    Stream Flip(Stream inputStream, bool horizontal, bool vertical, Stream? outputStream = null);
}