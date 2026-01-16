using LegioSoft.Imaging.Core.Enums;

namespace LegioSoft.Imaging.Core.Interfaces;

/// <summary>
/// Interface for encoding images to different formats.
/// </summary>
public interface ILegioImageEncoder
{
    /// <summary>
    /// Encodes image data to the specified format.
    /// </summary>
    /// <param name="imageData">The image data to encode.</param>
    /// <param name="format">The target format.</param>
    /// <param name="quality">The encoding quality (default: High).</param>
    /// <returns>Encoded image data.</returns>
    byte[] Encode(byte[] imageData, LegioImageFormat format, LegioEncodingQuality quality = LegioEncodingQuality.High);

    /// <summary>
    /// Encodes an image stream to the specified format.
    /// </summary>
    /// <param name="inputStream">The input stream containing image data.</param>
    /// <param name="format">The target format.</param>
    /// <param name="quality">The encoding quality (default: High).</param>
    /// <param name="outputStream">Optional output stream. If null, a new stream is created.</param>
    /// <returns>Stream containing the encoded image.</returns>
    Stream Encode(Stream inputStream, LegioImageFormat format, LegioEncodingQuality quality = LegioEncodingQuality.High,
        Stream? outputStream = null);
}