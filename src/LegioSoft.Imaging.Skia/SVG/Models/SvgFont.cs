using LegioSoft.Imaging.Skia.SVG.Interfaces;
using SkiaSharp;

namespace LegioSoft.Imaging.Skia.SVG.Models;

public class SvgFont : SvgElement
{
    public string? FontFamily { get; set; }
    public string? FontStyle { get; set; }
    public string? FontWeight { get; set; }
    public float? FontSize { get; set; }
    public string? HorizAdvX { get; set; }
    public string? VertOriginY { get; set; }
    public string? VertAdvY { get; set; }

    public override void Accept(ISvgElementVisitor visitor, SKCanvas canvas, SKMatrix transform)
    {
        visitor.Visit(this, canvas, transform);
    }
}
