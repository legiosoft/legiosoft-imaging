using LegioSoft.Imaging.Skia.SVG.Interfaces;
using SkiaSharp;

namespace LegioSoft.Imaging.Skia.SVG.Models;

public class SvgHKern : SvgElement
{
    public string? G1 { get; set; }
    public string? G2 { get; set; }
    public string? K { get; set; }
    public string? U1 { get; set; }
    public string? U2 { get; set; }

    public override void Accept(ISvgElementVisitor visitor, SKCanvas canvas, SKMatrix transform)
    {
        visitor.Visit(this, canvas, transform);
    }
}
