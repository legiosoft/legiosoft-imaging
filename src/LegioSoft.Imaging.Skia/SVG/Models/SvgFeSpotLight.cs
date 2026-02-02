using LegioSoft.Imaging.Skia.SVG.Interfaces;
using SkiaSharp;

namespace LegioSoft.Imaging.Skia.SVG.Models;

public class SvgFeSpotLight : SvgElement
{
    public float? X { get; set; }
    public float? Y { get; set; }
    public float? Z { get; set; }
    public float? PointsAtX { get; set; }
    public float? PointsAtY { get; set; }
    public float? PointsAtZ { get; set; }
    public float? SpecularExponent { get; set; }
    public float? LimitingConeAngle { get; set; }

    public override void Accept(ISvgElementVisitor visitor, SKCanvas canvas, SKMatrix transform)
    {
        visitor.Visit(this, canvas, transform);
    }
}
