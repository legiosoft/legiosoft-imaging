using LegioSoft.Imaging.Skia.SVG.Interfaces;
using SkiaSharp;

namespace LegioSoft.Imaging.Skia.SVG.Models;

public class SvgFeComposite : SvgElement
{
    public string? In { get; set; }
    public string? In2 { get; set; }
    public string? Operator { get; set; }
    public string? K1 { get; set; }
    public string? K2 { get; set; }
    public string? K3 { get; set; }
    public string? K4 { get; set; }

    public override void Accept(ISvgElementVisitor visitor, SKCanvas canvas, SKMatrix transform)
    {
        visitor.Visit(this, canvas, transform);
    }
}
