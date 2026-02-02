using LegioSoft.Imaging.Skia.SVG.Interfaces;
using SkiaSharp;

namespace LegioSoft.Imaging.Skia.SVG.Models;

public class SvgAnimateMotion : SvgElement
{
    public string? Path { get; set; }
    public string? KeyPoints { get; set; }
    public string? KeyTimes { get; set; }
    public string? Rotate { get; set; }
    public string? From { get; set; }
    public string? To { get; set; }
    public string? By { get; set; }
    public string? Begin { get; set; }
    public string? Dur { get; set; }
    public string? RepeatCount { get; set; }
    public string? CalcMode { get; set; }

    public override void Accept(ISvgElementVisitor visitor, SKCanvas canvas, SKMatrix transform)
    {
        visitor.Visit(this, canvas, transform);
    }
}
