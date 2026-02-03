using LegioSoft.Imaging.Skia.SVG.Interfaces;
using SkiaSharp;

namespace LegioSoft.Imaging.Skia.SVG.Models;

public class SvgImage : SvgElement
{
    public string? Href { get; set; }
    public string? XlinkHref { get; set; }
    public float X { get; set; }
    public float Y { get; set; }
    public float Width { get; set; }
    public float Height { get; set; }
    public string PreserveAspectRatio { get; set; } = string.Empty;
    public float Opacity { get; set; } = 1.0f;
    public string Display { get; set; } = string.Empty;
    public string Visibility { get; set; } = string.Empty;
    public string ClipPath { get; set; } = string.Empty;
    public string Mask { get; set; } = string.Empty;
    public string Filter { get; set; } = string.Empty;
    public string CrossOrigin { get; set; } = string.Empty;
    public string Class { get; set; } = string.Empty;
    public string Style { get; set; } = string.Empty;
    public SKBitmap? Bitmap { get; set; }

    public override void Accept(ISvgElementVisitor visitor, SKCanvas canvas, SKMatrix transform)
    {
        visitor.Visit(this, canvas, transform);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            Bitmap?.Dispose();
        }
        base.Dispose(disposing);
    }

    ~SvgImage()
    {
        Dispose(false);
    }
}
