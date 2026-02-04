using LegioSoft.Imaging.SVG.Interfaces;
using SkiaSharp;

namespace LegioSoft.Imaging.SVG.Models;

public class SvgAnimate : SvgElement
{
    public string? AttributeName { get; set; }
    public string? Type { get; set; }
    public string? From { get; set; }
    public string? To { get; set; }
    public string? By { get; set; }
    public string? Begin { get; set; }
    public string? Dur { get; set; }
    public string? RepeatCount { get; set; }
    public string? Values { get; set; }
    public string? KeyTimes { get; set; }
    public string? KeySplines { get; set; }
    public string? CalcMode { get; set; }

    public override void Accept(ISvgElementVisitor visitor, SKCanvas canvas, SKMatrix transform)
    {
        visitor.Visit(this, canvas, transform);
    }
}
