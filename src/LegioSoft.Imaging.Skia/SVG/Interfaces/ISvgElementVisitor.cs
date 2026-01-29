using LegioSoft.Imaging.Skia.SVG.Models;
using SkiaSharp;

namespace LegioSoft.Imaging.Skia.SVG.Interfaces;

public interface ISvgElementVisitor
{
    void Visit(SvgRect element, SKCanvas canvas, SKMatrix transform);
    void Visit(SvgCircle element, SKCanvas canvas, SKMatrix transform);
    void Visit(SvgEllipse element, SKCanvas canvas, SKMatrix transform);
    void Visit(SvgLine element, SKCanvas canvas, SKMatrix transform);
    void Visit(SvgPolyline element, SKCanvas canvas, SKMatrix transform);
    void Visit(SvgPolygon element, SKCanvas canvas, SKMatrix transform);
    void Visit(SvgPath element, SKCanvas canvas, SKMatrix transform);
    void Visit(SvgGroup element, SKCanvas canvas, SKMatrix transform);
}
