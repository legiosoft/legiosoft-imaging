using LegioSoft.Imaging.Skia.SVG.Interfaces;
using SkiaSharp;

namespace LegioSoft.Imaging.Skia.SVG.Models;

public class SvgColorProfile : SvgElement
{
    public string? Name { get; set; }
    public string? Local { get; set; }
    public string? RenderingIntent { get; set; }
    public string? Href { get; set; }

    public override void Accept(ISvgElementVisitor visitor, SKCanvas canvas, SKMatrix transform)
    {
        visitor.Visit(this, canvas, transform);
    }
}
