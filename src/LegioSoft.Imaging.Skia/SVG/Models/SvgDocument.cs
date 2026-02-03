using SkiaSharp;

namespace LegioSoft.Imaging.Skia.SVG.Models;

public class SvgDocument : IDisposable
{
    public SKRect ViewBox { get; set; }
    public float Width { get; set; }
    public float Height { get; set; }
    public SvgElement? RootElement { get; set; }
    public string Xmlns { get; set; } = string.Empty;
    public string XmlnsXlink { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public string BaseProfile { get; set; } = string.Empty;
    public float X { get; set; }
    public float Y { get; set; }
    public string PreserveAspectRatio { get; set; } = string.Empty;
    public string ContentScriptType { get; set; } = string.Empty;
    public string ContentStyleType { get; set; } = string.Empty;
    public string ZoomAndPan { get; set; } = string.Empty;
    public string Class { get; set; } = string.Empty;
    public string Style { get; set; } = string.Empty;
    public string Id { get; set; } = string.Empty;
    public string XmlSpace { get; set; } = string.Empty;
    public string XmlLang { get; set; } = string.Empty;
    public string XmlBase { get; set; } = string.Empty;

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            RootElement?.Dispose();
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
