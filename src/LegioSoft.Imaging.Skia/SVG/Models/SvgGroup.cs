using LegioSoft.Imaging.Skia.SVG.Interfaces;
using SkiaSharp;

namespace LegioSoft.Imaging.Skia.SVG.Models;

public class SvgGroup : SvgElement
{
    public List<SvgElement>? Children { get; set; }
    public float Opacity { get; set; } = 1.0f;
    public string Display { get; set; } = string.Empty;
    public string Visibility { get; set; } = string.Empty;
    public string ClipPath { get; set; } = string.Empty;
    public string Mask { get; set; } = string.Empty;
    public string Filter { get; set; } = string.Empty;
    public string XmlSpace { get; set; } = string.Empty;
    public string XmlLang { get; set; } = string.Empty;
    public string XmlBase { get; set; } = string.Empty;

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

    ~SvgGroup()
    {
        Dispose(false);
    }
}
