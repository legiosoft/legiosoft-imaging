namespace LegioSoft.Imaging.Core.Enums;

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