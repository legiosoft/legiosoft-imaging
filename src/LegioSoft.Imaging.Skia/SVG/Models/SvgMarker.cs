using LegioSoft.Imaging.Skia.SVG.Interfaces;
using SkiaSharp;

namespace LegioSoft.Imaging.Skia.SVG.Models;

public class SvgMarker : SvgElement
{
    public SKRect? ViewBox { get; set; }
    public string? PreserveAspectRatio { get; set; }
    public float? RefX { get; set; }
    public float? RefY { get; set; }
    public float? MarkerWidth { get; set; }
    public float? MarkerHeight { get; set; }
    public string? MarkerUnits { get; set; }
    public float? Orient { get; set; }
    public string? OrientType { get; set; }
    public float Opacity { get; set; } = 1.0f;
    public string? Overflow { get; set; }
    public string? Clip { get; set; }
    public string Class { get; set; } = string.Empty;
    public string Style { get; set; } = string.Empty;
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