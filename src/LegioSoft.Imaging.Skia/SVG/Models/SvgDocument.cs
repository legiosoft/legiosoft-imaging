using SkiaSharp;

namespace LegioSoft.Imaging.Skia.SVG.Models;

public class SvgDocument : IDisposable
{
    public SKRect ViewBox { get; set; }
    public float Width { get; set; }
    public float Height { get; set; }
    public SvgElement? RootElement { get; set; }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            RootElement?.Dispose();
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
