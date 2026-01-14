namespace LegioSoft.Imaging.Core.Interfaces;

/// <summary>
/// Interface for applying filters to images.
/// </summary>
public interface ILegioImageFilter
{
    /// <summary>
    /// Applies a grayscale filter to the image.
    /// </summary>
    /// <param name="imageData">The image data to filter.</param>
    /// <returns>Grayscale image data.</returns>
    byte[] ApplyGrayscale(byte[] imageData);

    /// <summary>
    /// Applies a grayscale filter to the image stream.
    /// </summary>
    /// <param name="inputStream">The input stream containing image data.</param>
    /// <param name="outputStream">Optional output stream. If null, a new stream is created.</param>
    /// <returns>Stream containing the grayscale image.</returns>
    Stream ApplyGrayscale(Stream inputStream, Stream? outputStream = null);
    
    /// <summary>
    /// Applies a sepia tone filter to the image.
    /// </summary>
    /// <param name="imageData">The image data to filter.</param>
    /// <returns>Sepia-toned image data.</returns>
    byte[] ApplySepia(byte[] imageData);

    /// <summary>
    /// Applies a sepia tone filter to the image stream.
    /// </summary>
    /// <param name="inputStream">The input stream containing image data.</param>
    /// <param name="outputStream">Optional output stream. If null, a new stream is created.</param>
    /// <returns>Stream containing the sepia-toned image.</returns>
    Stream ApplySepia(Stream inputStream, Stream? outputStream = null);
    
    /// <summary>
    /// Applies a blur filter to the image.
    /// </summary>
    /// <param name="imageData">The image data to filter.</param>
    /// <param name="radius">Blur radius (default: 3). Higher values create more blur.</param>
    /// <returns>Blurred image data.</returns>
    byte[] ApplyBlur(byte[] imageData, int radius = 3);

    /// <summary>
    /// Applies a blur filter to the image stream.
    /// </summary>
    /// <param name="inputStream">The input stream containing image data.</param>
    /// <param name="radius">Blur radius (default: 3). Higher values create more blur.</param>
    /// <param name="outputStream">Optional output stream. If null, a new stream is created.</param>
    /// <returns>Stream containing the blurred image.</returns>
    Stream ApplyBlur(Stream inputStream, int radius = 3, Stream? outputStream = null);
    
    /// <summary>
    /// Applies a sharpening filter to the image.
    /// </summary>
    /// <param name="imageData">The image data to filter.</param>
    /// <param name="amount">Sharpen amount (default: 50). Range: 0-100.</param>
    /// <returns>Sharpened image data.</returns>
    byte[] ApplySharpen(byte[] imageData, int amount = 50);

    /// <summary>
    /// Applies a sharpening filter to the image stream.
    /// </summary>
    /// <param name="inputStream">The input stream containing image data.</param>
    /// <param name="amount">Sharpen amount (default: 50). Range: 0-100.</param>
    /// <param name="outputStream">Optional output stream. If null, a new stream is created.</param>
    /// <returns>Stream containing the sharpened image.</returns>
    Stream ApplySharpen(Stream inputStream, int amount = 50, Stream? outputStream = null);
}