using LegioSoft.Imaging.Skia.SVG.Interfaces;
using SkiaSharp;

namespace LegioSoft.Imaging.Skia.SVG.Models;

public class SvgPath : SvgElement
{
    public SKPath? Path { get; set; }

    public override void Accept(ISvgElementVisitor visitor, SKCanvas canvas, SKMatrix transform)
    {
        visitor.Visit(this, canvas, transform);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            Path?.Dispose();
        }
        base.Dispose(disposing);
    }

    ~SvgPath()
    {
        Dispose(false);
    }
}
