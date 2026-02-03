using LegioSoft.Imaging.Skia.SVG.Interfaces;
using SkiaSharp;

namespace LegioSoft.Imaging.Skia.SVG.Models;

public class SvgTitle : SvgElement
{
    public string? Text { get; set; }
    public string Class { get; set; } = string.Empty;
    public string Style { get; set; } = string.Empty;
    public string XmlSpace { get; set; } = string.Empty;
    public string XmlLang { get; set; } = string.Empty;
    public string XmlBase { get; set; } = string.Empty;

    public override void Accept(ISvgElementVisitor visitor, SKCanvas canvas, SKMatrix transform)
    {
        visitor.Visit(this, canvas, transform);
    }
}
