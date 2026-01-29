using LegioSoft.Imaging.Skia.SVG.Interfaces;
using SkiaSharp;

namespace LegioSoft.Imaging.Skia.SVG.Models;

public abstract class SvgElement : IDisposable
{
    public string Id { get; set; } = string.Empty;
    public SKMatrix? Transform { get; set; }
    public SvgStyle? FillStyle { get; set; }
    public SvgStyle? StrokeStyle { get; set; }

    public abstract void Accept(ISvgElementVisitor visitor, SKCanvas canvas, SKMatrix transform);

    protected virtual void Dispose(bool disposing)
    {
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
