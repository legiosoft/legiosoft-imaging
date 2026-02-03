using LegioSoft.Imaging.Skia.SVG.Interfaces;
using SkiaSharp;

namespace LegioSoft.Imaging.Skia.SVG.Models;

public class SvgPath : SvgElement
{
    public SKPath? Path { get; set; }
    public float PathLength { get; set; }
    public float Opacity { get; set; } = 1.0f;
    public string Display { get; set; } = string.Empty;
    public string Visibility { get; set; } = string.Empty;
    public string ClipPath { get; set; } = string.Empty;
    public string Mask { get; set; } = string.Empty;
    public string MarkerStart { get; set; } = string.Empty;
    public string MarkerMid { get; set; } = string.Empty;
    public string MarkerEnd { get; set; } = string.Empty;
    public string Class { get; set; } = string.Empty;
    public string Style { get; set; } = string.Empty;

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
