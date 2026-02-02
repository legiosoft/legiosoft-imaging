using LegioSoft.Imaging.Skia.SVG.Interfaces;
using SkiaSharp;

namespace LegioSoft.Imaging.Skia.SVG.Models;

public class SvgFeDropShadow : SvgElement
{
    public string? In { get; set; }
    public float? Dx { get; set; }
    public float? Dy { get; set; }
    public float? StdDeviationX { get; set; }
    public float? StdDeviationY { get; set; }
    public string? FloodColor { get; set; }
    public float? FloodOpacity { get; set; }

    public override void Accept(ISvgElementVisitor visitor, SKCanvas canvas, SKMatrix transform)
    {
        visitor.Visit(this, canvas, transform);
    }
}
