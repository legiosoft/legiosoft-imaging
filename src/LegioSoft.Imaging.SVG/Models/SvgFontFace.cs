using LegioSoft.Imaging.SVG.Interfaces;
using SkiaSharp;

namespace LegioSoft.Imaging.SVG.Models;

public class SvgFontFace : SvgElement
{
    public string? FontFamily { get; set; }
    public string? UnitsPerEm { get; set; }
    public string? Panose1 { get; set; }
    public string? Ascent { get; set; }
    public string? Descent { get; set; }
    public string? CapHeight { get; set; }
    public string? XHeight { get; set; }
    public string? AccentHeight { get; set; }
    public string? StemH { get; set; }
    public string? StemV { get; set; }
    public string? Slope { get; set; }
    public string? FontStretch { get; set; }
    public string? FontWeight { get; set; }
    public string? UnderlinePosition { get; set; }
    public string? UnderlineThickness { get; set; }
    public string? StrikethroughPosition { get; set; }
    public string? StrikethroughThickness { get; set; }
    public string? OverlinePosition { get; set; }
    public string? OverlineThickness { get; set; }
    public string? FontStyle { get; set; }
    public string? FontVariant { get; set; }

    public override void Accept(ISvgElementVisitor visitor, SKCanvas canvas, SKMatrix transform)
    {
        visitor.Visit(this, canvas, transform);
    }
}
