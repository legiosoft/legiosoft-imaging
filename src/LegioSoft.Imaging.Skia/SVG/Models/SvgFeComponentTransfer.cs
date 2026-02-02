using LegioSoft.Imaging.Skia.SVG.Interfaces;
using SkiaSharp;

namespace LegioSoft.Imaging.Skia.SVG.Models;

public class SvgFeComponentTransfer : SvgElement
{
    public string? In { get; set; }
    public SvgFeFuncR? FuncR { get; set; }
    public SvgFeFuncG? FuncG { get; set; }
    public SvgFeFuncB? FuncB { get; set; }
    public SvgFeFuncA? FuncA { get; set; }

    public override void Accept(ISvgElementVisitor visitor, SKCanvas canvas, SKMatrix transform)
    {
        visitor.Visit(this, canvas, transform);
    }
}
