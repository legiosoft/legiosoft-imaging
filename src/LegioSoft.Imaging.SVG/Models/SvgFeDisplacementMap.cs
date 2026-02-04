using LegioSoft.Imaging.SVG.Interfaces;
using SkiaSharp;

namespace LegioSoft.Imaging.SVG.Models;

public class SvgFeDisplacementMap : SvgElement
{
    public string? In { get; set; }
    public string? In2 { get; set; }
    public string? Scale { get; set; }
    public string? XChannelSelector { get; set; }
    public string? YChannelSelector { get; set; }

    public override void Accept(ISvgElementVisitor visitor, SKCanvas canvas, SKMatrix transform)
    {
        visitor.Visit(this, canvas, transform);
    }
}
