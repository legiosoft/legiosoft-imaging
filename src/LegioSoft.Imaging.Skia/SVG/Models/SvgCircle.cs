using LegioSoft.Imaging.Skia.SVG.Interfaces;
using SkiaSharp;

namespace LegioSoft.Imaging.Skia.SVG.Models;

public class SvgCircle : SvgElement
{
    public float Cx { get; set; }
    public float Cy { get; set; }
    public float Radius { get; set; }
    public float PathLength { get; set; }
    public float Opacity { get; set; } = 1.0f;
    public string Display { get; set; } = string.Empty;
    public string Visibility { get; set; } = string.Empty;
    public string ClipPath { get; set; } = string.Empty;
    public string Mask { get; set; } = string.Empty;
    public string Class { get; set; } = string.Empty;
    public string Style { get; set; } = string.Empty;

    public override void Accept(ISvgElementVisitor visitor, SKCanvas canvas, SKMatrix transform)
    {
        visitor.Visit(this, canvas, transform);
    }
}
