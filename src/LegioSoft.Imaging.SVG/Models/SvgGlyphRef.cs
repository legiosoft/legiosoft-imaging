using LegioSoft.Imaging.SVG.Interfaces;
using SkiaSharp;

namespace LegioSoft.Imaging.SVG.Models;

public class SvgGlyphRef : SvgElement
{
    public string? Href { get; set; }
    public string? GlyphRef { get; set; }
    public string? Format { get; set; }
    public float? X { get; set; }
    public float? Y { get; set; }
    public float? Dx { get; set; }
    public float? Dy { get; set; }

    public override void Accept(ISvgElementVisitor visitor, SKCanvas canvas, SKMatrix transform)
    {
        visitor.Visit(this, canvas, transform);
    }
}