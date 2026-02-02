using LegioSoft.Imaging.Skia.SVG.Interfaces;
using SkiaSharp;

namespace LegioSoft.Imaging.Skia.SVG.Models;

public class SvgFeSpecularLighting : SvgElement
{
    public string? In { get; set; }
    public float? SurfaceScale { get; set; }
    public float? SpecularConstant { get; set; }
    public float? SpecularExponent { get; set; }
    public string? KernelUnitLength { get; set; }
    public string? LightingColor { get; set; }
    public List<SvgElement>? LightingSources { get; set; }

    public override void Accept(ISvgElementVisitor visitor, SKCanvas canvas, SKMatrix transform)
    {
        visitor.Visit(this, canvas, transform);
    }
}
