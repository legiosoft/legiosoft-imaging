using LegioSoft.Imaging.SVG.Interfaces;
using SkiaSharp;

namespace LegioSoft.Imaging.SVG.Models;

public class SvgFeBlend : SvgElement
{
    public string? In { get; set; }
    public string? In2 { get; set; }
    public string? Mode { get; set; }
    public float? X { get; set; }
    public float? Y { get; set; }
    public float? Width { get; set; }
    public float? Height { get; set; }
    public string? Result { get; set; }
    public string? Class { get; set; }
    public string? Style { get; set; }

    public override void Accept(ISvgElementVisitor visitor, SKCanvas canvas, SKMatrix transform)
    {
        visitor.Visit(this, canvas, transform);
    }
}
