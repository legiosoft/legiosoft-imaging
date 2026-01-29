using SkiaSharp;

namespace LegioSoft.Imaging.Skia.SVG.Models;

public class SvgStyle
{
    public SKColor Color { get; set; } = SKColors.Black;
    public float Opacity { get; set; } = 1.0f;
    public float StrokeWidth { get; set; } = 1.0f;
}
