using LegioSoft.Imaging.SVG.Converter;
using LegioSoft.Imaging.SVG.Parser;
using SkiaSharp;

namespace LegioSoft.Imaging.SVG;

public class SvgRenderer
{
    private readonly SvgParser _parser;
    private readonly SvgToSkiaConverter _converter;

    public SvgRenderer()
    {
        _parser = new SvgParser();
        _converter = new SvgToSkiaConverter();
    }

    public SvgRenderer(SvgParseOptions parseOptions)
    {
        _parser = new SvgParser(parseOptions);
        _converter = new SvgToSkiaConverter();
    }

    public SKBitmap Render(string svgPath, int width, int height)
    {
        using var document = _parser.Parse(svgPath);
        return _converter.Convert(document, width, height);
    }

    public void RenderToFile(string svgPath, string outputPath, int width, int height, SKEncodedImageFormat format = SKEncodedImageFormat.Png, int quality = 100)
    {
        using var bitmap = Render(svgPath, width, height);
        using var image = SKImage.FromBitmap(bitmap);
        using var data = image.Encode(format, quality);
        using var stream = File.OpenWrite(outputPath);
        data.SaveTo(stream);
    }
}
