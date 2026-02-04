using LegioSoft.Imaging.SVG.Interfaces;
using SkiaSharp;

namespace LegioSoft.Imaging.SVG.Models;

public class SvgFontFaceSrc : SvgElement
{
    public override void Accept(ISvgElementVisitor visitor, SKCanvas canvas, SKMatrix transform)
    {
        visitor.Visit(this, canvas, transform);
    }
}
