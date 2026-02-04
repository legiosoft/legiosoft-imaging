using LegioSoft.Imaging.SVG.Interfaces;
using SkiaSharp;

namespace LegioSoft.Imaging.SVG.Models;

public class SvgFeDiffuseLighting : SvgElement
{
    public string? In { get; set; }
    public float? SurfaceScale { get; set; }
    public float? DiffuseConstant { get; set; }
    public string? KernelUnitLength { get; set; }
    public List<SvgElement>? LightingColors { get; set; }

    public override void Accept(ISvgElementVisitor visitor, SKCanvas canvas, SKMatrix transform)
    {
        visitor.Visit(this, canvas, transform);
    }
}
