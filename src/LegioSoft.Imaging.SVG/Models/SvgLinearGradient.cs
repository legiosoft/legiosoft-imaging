using LegioSoft.Imaging.SVG.Interfaces;
using SkiaSharp;

namespace LegioSoft.Imaging.SVG.Models;

public class SvgLinearGradient : SvgElement
{
    public float? X1 { get; set; }
    public float? Y1 { get; set; }
    public float? X2 { get; set; }
    public float? Y2 { get; set; }
    public string? GradientUnits { get; set; }
    public string? GradientTransform { get; set; }
    public string? SpreadMethod { get; set; }
    public List<SvgGradientStop>? Stops { get; set; }
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
            if (Stops != null)
            {
                Stops.Clear();
            }
        }
        base.Dispose(disposing);
    }

    ~SvgLinearGradient()
    {
        Dispose(false);
    }
}

public class SvgGradientStop
{
    public float Offset { get; set; }
    public string? StopColor { get; set; }
    public float? StopOpacity { get; set; }
    public string? Class { get; set; }
    public string? Style { get; set; }
    public string? Id { get; set; }
}