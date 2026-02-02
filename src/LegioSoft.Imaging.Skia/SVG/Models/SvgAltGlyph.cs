using LegioSoft.Imaging.Skia.SVG.Interfaces;
using SkiaSharp;

namespace LegioSoft.Imaging.Skia.SVG.Models;

public class SvgAltGlyph : SvgElement
{
    public string? Href { get; set; }
    public string? GlyphRef { get; set; }
    public string? Format { get; set; }
    public float? X { get; set; }
    public float? Y { get; set; }
    public float? Dx { get; set; }
    public float? Dy { get; set; }
    public float? Rotate { get; set; }
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

    ~SvgAltGlyph()
    {
        Dispose(false);
    }
}