using LegioSoft.Imaging.Skia.SVG.Interfaces;
using SkiaSharp;

namespace LegioSoft.Imaging.Skia.SVG.Models;

public class SvgMask : SvgElement
{
    public float? X { get; set; }
    public float? Y { get; set; }
    public? Width { get; set; }
    public? Height { get; set; }
    public string? MaskUnits { get; set; }
    public string? MaskContentUnits { get; set; }
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

    ~SvgMask()
    {
        Dispose(false);
    }
}