using LegioSoft.Imaging.Skia.SVG.Interfaces;
using SkiaSharp;

namespace LegioSoft.Imaging.Skia.SVG.Models;

public class SvgMarker : SvgElement
{
    public float? RefX { get; set; }
    public float? RefY { get; set; }
    public float? MarkerWidth { get; set; }
    public float? MarkerHeight { get; set; }
    public string? MarkerUnits { get; set; }
    public float? Orient { get; set; }
    public string? OrientType { get; set; }
    public List<SvgElement>? Children { get; set; }

    public override void Accept(ISvgElementVisitor visitor, SKCanvas canvas, SKMatrix transform)
    {
        visitor.Visit(this, canvas, transform);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            if (Children != null)
            {
                foreach (var child in Children)
                {
                    child?.Dispose();
                }
                Children.Clear();
            }
        }
        base.Dispose(disposing);
    }

    ~SvgMarker()
    {
        Dispose(false);
    }
}