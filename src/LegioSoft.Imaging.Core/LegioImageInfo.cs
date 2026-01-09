namespace LegioSoft.Imaging.Core;

/// <summary>
/// Contains metadata information about an image.
/// </summary>
public class LegioImageInfo
{
    /// <summary>
    /// Image width in pixels.
    /// </summary>
    public int Width { get; set; }

    /// <summary>
    /// Image height in pixels.
    /// </summary>
    public int Height { get; set; }

    /// <summary>
    /// Image format.
    /// </summary>
    public LegioImageFormat Format { get; set; }

    /// <summary>
    /// Whether the image has an alpha channel (transparency).
    /// </summary>
    public bool HasAlpha { get; set; }

    /// <summary>
    /// Size of the image data in bytes.
    /// </summary>
    public int ByteSize { get; set; }
}