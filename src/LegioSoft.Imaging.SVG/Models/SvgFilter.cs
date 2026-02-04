using LegioSoft.Imaging.SVG.Interfaces;
using SkiaSharp;

namespace LegioSoft.Imaging.SVG.Models;

public class SvgFilter : SvgElement
{
    public float? X { get; set; }
    public float? Y { get; set; }
    public float? Width { get; set; }
    public float? Height { get; set; }
    public string? FilterUnits { get; set; }
    public string? PrimitiveUnits { get; set; }
    public List<SvgElement>? FilterPrimitives { get; set; }
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
            foreach (var primitive in FilterPrimitives ?? Enumerable.Empty<SvgElement>())
            {
                primitive.Dispose();
            }
            FilterPrimitives?.Clear();
        }
        base.Dispose(disposing);
    }

    ~SvgFilter()
    {
        Dispose(false);
    }
}
