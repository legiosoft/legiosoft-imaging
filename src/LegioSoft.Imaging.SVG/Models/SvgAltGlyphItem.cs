using LegioSoft.Imaging.SVG.Interfaces;
using SkiaSharp;

namespace LegioSoft.Imaging.SVG.Models;

public class SvgAltGlyphItem : SvgElement
{
    public List<SvgGlyphRef>? GlyphRefs { get; set; }

    public override void Accept(ISvgElementVisitor visitor, SKCanvas canvas, SKMatrix transform)
    {
        visitor.Visit(this, canvas, transform);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            if (GlyphRefs != null)
            {
                foreach (var glyphRef in GlyphRefs)
                {
                    glyphRef?.Dispose();
                }
                GlyphRefs.Clear();
            }
        }
        base.Dispose(disposing);
    }

    ~SvgAltGlyphItem()
    {
        Dispose(false);
    }
}