using LegioSoft.Imaging.SVG.Interfaces;
using SkiaSharp;

namespace LegioSoft.Imaging.SVG.Models;

public class SvgFeMorphology : SvgElement
{
    public string? In { get; set; }
    public string? Operator { get; set; }
    public float? RadiusX { get; set; }
    public float? RadiusY { get; set; }

    public override void Accept(ISvgElementVisitor visitor, SKCanvas canvas, SKMatrix transform)
    {
        visitor.Visit(this, canvas, transform);
    }
}
