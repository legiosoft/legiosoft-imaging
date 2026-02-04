using LegioSoft.Imaging.SVG.Interfaces;
using SkiaSharp;

namespace LegioSoft.Imaging.SVG.Models;

public class SvgFeGaussianBlur : SvgElement
{
    public string? In { get; set; }
    public float? StdDeviationX { get; set; }
    public float? StdDeviationY { get; set; }
    public string? EdgeMode { get; set; }

    public override void Accept(ISvgElementVisitor visitor, SKCanvas canvas, SKMatrix transform)
    {
        visitor.Visit(this, canvas, transform);
    }
}
