using LegioSoft.Imaging.Skia.SVG.Interfaces;
using SkiaSharp;

namespace LegioSoft.Imaging.Skia.SVG.Models;

public class SvgFeFuncR : SvgElement
{
    public string? Type { get; set; }
    public string? TableValues { get; set; }
    public float? Slope { get; set; }
    public float? Intercept { get; set; }
    public float? Amplitude { get; set; }
    public float? Exponent { get; set; }
    public float? Offset { get; set; }

    public override void Accept(ISvgElementVisitor visitor, SKCanvas canvas, SKMatrix transform)
    {
        visitor.Visit(this, canvas, transform);
    }
}

public class SvgFeFuncG : SvgElement
{
    public string? Type { get; set; }
    public string? TableValues { get; set; }
    public float? Slope { get; set; }
    public float? Intercept { get; set; }
    public float? Amplitude { get; set; }
    public float? Exponent { get; set; }
    public float? Offset { get; set; }

    public override void Accept(ISvgElementVisitor visitor, SKCanvas canvas, SKMatrix transform)
    {
        visitor.Visit(this, canvas, transform);
    }
}

public class SvgFeFuncB : SvgElement
{
    public string? Type { get; set; }
    public string? TableValues { get; set; }
    public float? Slope { get; set; }
    public float? Intercept { get; set; }
    public float? Amplitude { get; set; }
    public float? Exponent { get; set; }
    public float? Offset { get; set; }

    public override void Accept(ISvgElementVisitor visitor, SKCanvas canvas, SKMatrix transform)
    {
        visitor.Visit(this, canvas, transform);
    }
}

public class SvgFeFuncA : SvgElement
{
    public string? Type { get; set; }
    public string? TableValues { get; set; }
    public float? Slope { get; set; }
    public float? Intercept { get; set; }
    public float? Amplitude { get; set; }
    public float? Exponent { get; set; }
    public float? Offset { get; set; }

    public override void Accept(ISvgElementVisitor visitor, SKCanvas canvas, SKMatrix transform)
    {
        visitor.Visit(this, canvas, transform);
    }
}
