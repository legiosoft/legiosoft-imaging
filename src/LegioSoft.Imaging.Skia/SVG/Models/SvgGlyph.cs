using LegioSoft.Imaging.Skia.SVG.Interfaces;
using SkiaSharp;

namespace LegioSoft.Imaging.Skia.SVG.Models;

public class SvgGlyph : SvgElement
{
    public string? Unicode { get; set; }
    public string? GlyphName { get; set; }
    public string? HorizAdvX { get; set; }
    public string? VertAdvY { get; set; }
    public string? VertOriginX { get; set; }
    public string? VertOriginY { get; set; }
    public SKPath? Path { get; set; }
    public string? D { get; set; }

    public override void Accept(ISvgElementVisitor visitor, SKCanvas canvas, SKMatrix transform)
    {
        visitor.Visit(this, canvas, transform);
    }
}
