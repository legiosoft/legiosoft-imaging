using LegioSoft.Imaging.SVG.Interfaces;
using SkiaSharp;

namespace LegioSoft.Imaging.SVG.Models;

public class SvgAnimateTransform : SvgElement
{
    public string? Type { get; set; }
    public string? From { get; set; }
    public string? To { get; set; }
    public string? By { get; set; }
    public string? Begin { get; set; }
    public string? Dur { get; set; }
    public string? RepeatCount { get; set; }
    public string? Additive { get; set; }
    public string? Accumulate { get; set; }

    public override void Accept(ISvgElementVisitor visitor, SKCanvas canvas, SKMatrix transform)
    {
        visitor.Visit(this, canvas, transform);
    }
}
