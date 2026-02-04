using LegioSoft.Imaging.SVG.Interfaces;
using SkiaSharp;

namespace LegioSoft.Imaging.SVG.Models;

public class SvgLine : SvgElement
{
    public float X1 { get; set; }
    public float Y1 { get; set; }
    public float X2 { get; set; }
    public float Y2 { get; set; }
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
