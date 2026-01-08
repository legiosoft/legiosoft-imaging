namespace LegioSoft.Imaging.Core;

public enum LegioImageFormat
{
    Png,
    Jpeg,
    WebP,
    Bmp,
    Gif
}

public enum LegioScaleMode
{
    Fit,
    Fill,
    Stretch
}

public class LegioImageInfo
{
    public int Width { get; set; }
    public int Height { get; set; }
    public LegioImageFormat Format { get; set; }
    public bool HasAlpha { get; set; }
    public int ByteSize { get; set; }
}

public enum LegioTransformType
{
    None,
    Rotate90,
    Rotate180,
    Rotate270,
    FlipHorizontal,
    FlipVertical
}

public enum LegioFilterType
{
    None,
    Grayscale,
    Sepia,
    Blur,
    Sharpen
}

public enum LegioResizeQuality
{
    Low,
    Medium,
    High,
    Maximum
}

public enum LegioEncodingQuality
{
    Low = 0,
    Medium = 50,
    High = 75,
    Maximum = 100
}
