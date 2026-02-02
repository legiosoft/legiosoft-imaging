using LegioSoft.Imaging.Skia.SVG.Interfaces;
using SkiaSharp;

namespace LegioSoft.Imaging.Skia.SVG.Models;

public class SvgView : SvgElement
{
    public SKRect? ViewBox { get; set; }
    public float? ZoomAndPan { get; set; }
    public float? ViewTargetX { get; set; }
    public float? ViewTargetY { get; set; }

    public override void Accept(ISvgElementVisitor visitor, SKCanvas canvas, SKMatrix transform)
    {
        visitor.Visit(this, canvas, transform);
    }
}
