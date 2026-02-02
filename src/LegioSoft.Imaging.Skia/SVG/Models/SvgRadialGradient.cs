using LegioSoft.Imaging.Skia.SVG.Interfaces;
using SkiaSharp;

namespace LegioSoft.Imaging.Skia.SVG.Models;

public class SvgRadialGradient : SvgElement
{
    public float? Cx { get; set; }
    public float? Cy { get; set; }
    public float? R { get; set; }
    public float? Fx { get; set; }
    public float? Fy { get; set; }
    public string? GradientUnits { get; set; }
    public string? GradientTransform { get; set; }
    public string? SpreadMethod { get; set; }
    public List<SvgGradientStop>? Stops { get; set; }
    public string? Href { get; set; }

    public override void Accept(ISvgElementVisitor visitor, SKCanvas canvas, SKMatrix transform)
    {
        visitor.Visit(this, canvas, transform);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            Stops?.Clear();
        }
        base.Dispose(disposing);
    }

    ~SvgRadialGradient()
    {
        Dispose(false);
    }
}
