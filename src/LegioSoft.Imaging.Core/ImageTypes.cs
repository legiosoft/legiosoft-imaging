namespace LegioSoft.Imaging.Core;

public enum ImageFormat
{
    Png,
    Jpeg,
    WebP,
    Bmp,
    Gif
}

public enum ScaleMode
{
    Fit,
    Fill,
    Stretch
}

public class ImageInfo
{
    public int Width { get; set; }
    public int Height { get; set; }
    public ImageFormat Format { get; set; }
    public bool HasAlpha { get; set; }
    public int ByteSize { get; set; }
}

public enum TransformType
{
    None,
    Rotate90,
    Rotate180,
    Rotate270,
    FlipHorizontal,
    FlipVertical
}

public enum FilterType
{
    None,
    Grayscale,
    Sepia,
    Blur
}

public enum ResizeQuality
{
    Low,
    Medium,
    High,
    Maximum
}