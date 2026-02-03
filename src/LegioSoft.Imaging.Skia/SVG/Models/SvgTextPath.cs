using LegioSoft.Imaging.Skia.SVG.Interfaces;
using SkiaSharp;

namespace LegioSoft.Imaging.Skia.SVG.Models;

public class SvgTextPath : SvgElement
{
    public string? Href { get; set; }
    public float StartOffset { get; set; }
    public string? Text { get; set; }
    public string? Method { get; set; }
    public string? Spacing { get; set; }
    public string? Side { get; set; }
    public string? LengthAdjust { get; set; }
    public float TextLength { get; set; }
    public string? FontFamily { get; set; }
    public float FontSize { get; set; } = 16f;
    public string Class { get; set; } = string.Empty;
    public string Style { get; set; } = string.Empty;

    public override void Accept(ISvgElementVisitor visitor, SKCanvas canvas, SKMatrix transform)
    {
        visitor.Visit(this, canvas, transform);
    }
}