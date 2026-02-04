using LegioSoft.Imaging.SVG.Interfaces;
using SkiaSharp;

namespace LegioSoft.Imaging.SVG.Models;

public class SvgFeConvolveMatrix : SvgElement
{
    public string? In { get; set; }
    public string? Order { get; set; }
    public string? KernelMatrix { get; set; }
    public float? Divisor { get; set; }
    public float? Bias { get; set; }
    public string? TargetX { get; set; }
    public string? TargetY { get; set; }
    public string? EdgeMode { get; set; }
    public string? KernelUnitLength { get; set; }
    public bool? PreserveAlpha { get; set; }

    public override void Accept(ISvgElementVisitor visitor, SKCanvas canvas, SKMatrix transform)
    {
        visitor.Visit(this, canvas, transform);
    }
}
