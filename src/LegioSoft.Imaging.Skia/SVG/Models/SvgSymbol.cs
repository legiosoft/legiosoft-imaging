using LegioSoft.Imaging.Skia.SVG.Interfaces;
using SkiaSharp;

namespace LegioSoft.Imaging.Skia.SVG.Models;

public class SvgSymbol : SvgElement
{
    public SKRect? ViewBox { get; set; }
    public string PreserveAspectRatio { get; set; } = string.Empty;
    public float Width { get; set; }
    public float Height { get; set; }
    public float X { get; set; }
    public float Y { get; set; }
    public float RefX { get; set; }
    public float RefY { get; set; }
    public string Class { get; set; } = string.Empty;
    public string Style { get; set; } = string.Empty;
    public string XmlSpace { get; set; } = string.Empty;
    public string XmlLang { get; set; } = string.Empty;
    public string XmlBase { get; set; } = string.Empty;
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

    ~SvgSymbol()
    {
        Dispose(false);
    }
}
