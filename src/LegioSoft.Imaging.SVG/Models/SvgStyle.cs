using SkiaSharp;

namespace LegioSoft.Imaging.SVG.Models;

public class SvgStyle
{
    public SKColor Color { get; set; } = SKColors.Black;
    public float Opacity { get; set; } = 1.0f;
    public float StrokeWidth { get; set; } = 1.0f;
    public float FillOpacity { get; set; } = 1.0f;
    public SKPathFillType FillRule { get; set; } = SKPathFillType.Winding;
    public float[] StrokeDashArray { get; set; } = Array.Empty<float>();
    public float StrokeDashOffset { get; set; } = 0;
    public SKStrokeCap StrokeLineCap { get; set; } = SKStrokeCap.Butt;
    public SKStrokeJoin StrokeLineJoin { get; set; } = SKStrokeJoin.Miter;
    public float StrokeMiterLimit { get; set; } = 4.0f;
    public float StrokeOpacity { get; set; } = 1.0f;
}
