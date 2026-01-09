namespace LegioSoft.Imaging.Core;

/// <summary>
/// Supported image formats.
/// </summary>
public enum LegioImageFormat
{
    Png,
    Jpeg,
    WebP,
    Bmp,
    Gif
}

/// <summary>
/// Scaling modes for image resizing.
/// </summary>
public enum LegioScaleMode
{
    /// <summary>
    /// Scales the image to fit within the specified dimensions while maintaining aspect ratio. May have empty space.
    /// </summary>
    Fit,
    /// <summary>
    /// Scales the image to fill the specified dimensions while maintaining aspect ratio. May crop the image.
    /// </summary>
    Fill,
    /// <summary>
    /// Stretches the image to exactly match the specified dimensions, ignoring aspect ratio.
    /// </summary>
    Stretch
}

/// <summary>
/// Types of image transformations.
/// </summary>
public enum LegioTransformType
{
    None,
    Rotate90,
    Rotate180,
    Rotate270,
    FlipHorizontal,
    FlipVertical
}

/// <summary>
/// Types of image filters.
/// </summary>
public enum LegioFilterType
{
    None,
    Grayscale,
    Sepia,
    Blur,
    Sharpen
}

/// <summary>
/// Quality levels for image resizing operations.
/// </summary>
public enum LegioResizeQuality
{
    Low,
    Medium,
    High,
    Maximum
}

/// <summary>
/// Quality levels for image encoding operations.
/// Values represent percentage quality (0-100).
/// </summary>
public enum LegioEncodingQuality
{
    /// <summary>
    /// Minimum quality (0%).
    /// </summary>
    Low = 0,
    /// <summary>
    /// Medium quality (50%).
    /// </summary>
    Medium = 50,
    /// <summary>
    /// High quality (75%).
    /// </summary>
    High = 75,
    /// <summary>
    /// Maximum quality (100%).
    /// </summary>
    Maximum = 100
}
