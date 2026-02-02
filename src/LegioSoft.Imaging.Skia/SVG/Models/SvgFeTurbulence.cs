using LegioSoft.Imaging.Skia.SVG.Interfaces;
using SkiaSharp;

namespace LegioSoft.Imaging.Skia.SVG.Models;

public class SvgFeTurbulence : SvgElement
{
    public string? Type { get; set; }
    public string? BaseFrequency { get; set; }
    public int? NumOctaves { get; set; }
    public float? Seed { get; set; }
    public string? StitchTiles { get; set; }

    public override void Accept(ISvgElementVisitor visitor, SKCanvas canvas, SKMatrix transform)
    {
        visitor.Visit(this, canvas, transform);
    }
}
