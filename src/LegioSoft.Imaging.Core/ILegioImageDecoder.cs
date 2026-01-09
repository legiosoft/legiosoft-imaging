namespace LegioSoft.Imaging.Core;

/// <summary>
/// Interface for decoding images and retrieving image information.
/// </summary>
public interface ILegioImageDecoder
{
    /// <summary>
    /// Decodes image data and returns the decoded bytes.
    /// </summary>
    /// <param name="imageData">The encoded image data to decode.</param>
    /// <param name="format">The detected format of the image.</param>
    /// <returns>Decoded image data.</returns>
    byte[] Decode(byte[] imageData, out LegioImageFormat format);

    /// <summary>
    /// Retrieves metadata information from image data.
    /// </summary>
    /// <param name="imageData">The image data to analyze.</param>
    /// <returns>Image metadata including dimensions, format, and size.</returns>
    LegioImageInfo GetImageInfo(byte[] imageData);

    /// <summary>
    /// Retrieves metadata information from an image stream.
    /// </summary>
    /// <param name="inputStream">The image stream to analyze.</param>
    /// <returns>Image metadata including dimensions, format, and size.</returns>
    LegioImageInfo GetImageInfo(Stream inputStream);
}