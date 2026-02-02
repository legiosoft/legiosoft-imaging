using LegioSoft.Imaging.Skia.SVG.Interfaces;
using SkiaSharp;

namespace LegioSoft.Imaging.Skia.SVG.Models;

public class SvgTSpan : SvgElement
{
    public float X { get; set; }
    public float Y { get; set; }
    public float Dx { get; set; }
    public float Dy { get; set; }
    public string? Text { get; set; }
    public string? FontFamily { get; set; }
    public float FontSize { get; set; } = 16f;
    public string? FontWeight { get; set; }
    public string? FontStyle { get; set; }
    public string? TextAnchor { get; set; }
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

    ~SvgTSpan()
    {
        Dispose(false);
    }
}
