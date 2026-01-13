namespace LegioSoft.Imaging.Core;

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
