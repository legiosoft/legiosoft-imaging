using LegioSoft.Imaging.SVG.Interfaces;
using SkiaSharp;

namespace LegioSoft.Imaging.SVG.Models;

public class SvgPattern : SvgElement
{
    public float? X { get; set; }
    public float? Y { get; set; }
    public float? Width { get; set; }
    public float? Height { get; set; }
    public string? PatternUnits { get; set; }
    public string? PatternContentUnits { get; set; }
    public string? PatternTransform { get; set; }
    public SKRect? ViewBox { get; set; }
    public string? PreserveAspectRatio { get; set; }
    public List<SvgElement>? Children { get; set; }
    public string? Href { get; set; }
    public string? XlinkHref { get; set; }
    public string? Class { get; set; }
    public string? Style { get; set; }

    public override void Accept(ISvgElementVisitor visitor, SKCanvas canvas, SKMatrix transform)
    {
        visitor.Visit(this, canvas, transform);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            foreach (var child in Children ?? Enumerable.Empty<SvgElement>())
            {
                child.Dispose();
            }
            Children?.Clear();
        }
        base.Dispose(disposing);
    }

    ~SvgPattern()
    {
        Dispose(false);
    }
}
