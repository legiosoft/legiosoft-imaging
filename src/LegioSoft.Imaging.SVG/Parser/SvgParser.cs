using System.Globalization;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using LegioSoft.Imaging.SVG.Helpers;
using LegioSoft.Imaging.SVG.Models;
using SkiaSharp;

namespace LegioSoft.Imaging.SVG.Parser;

public class SvgParser
{
    private static readonly Regex FillStyleRegex = new(@"fill\s*:\s*([^;]+)", RegexOptions.Compiled);
    private static readonly Regex StrokeStyleRegex = new(@"stroke\s*:\s*([^;]+)", RegexOptions.Compiled);
    private static readonly XNamespace xlinkNamespace = "http://www.w3.org/1999/xlink";

    private readonly SvgParseOptions _options;
    private int _elementCount;
    private int _currentDepth;
    private readonly Dictionary<string, (SvgStyle? Fill, SvgStyle? Stroke)> _cssClasses = new();

    public SvgParser() : this(new SvgParseOptions())
    {
    }

    public SvgParser(SvgParseOptions options)
    {
        _options = options;
    }

    public SvgDocument Parse(string svgFilePath)
    {
        _elementCount = 0;
        _currentDepth = 0;
        _cssClasses.Clear();

        var settings = new XmlReaderSettings
        {
            DtdProcessing = DtdProcessing.Prohibit,
            XmlResolver = null,
            MaxCharactersFromEntities = 1024,
            IgnoreProcessingInstructions = true,
            IgnoreComments = true
        };

        using var reader = XmlReader.Create(svgFilePath, settings);
        var doc = XDocument.Load(reader);
        var root = doc.Root!;

        ParseCssStyles(root);

        var document = new SvgDocument
        {
            ViewBox = ParseViewBox(root),
            Width = ParseDimension(root, "width", 512),
            Height = ParseDimension(root, "height", 512),
            Xmlns = root.Attribute("xmlns")?.Value ?? string.Empty,
            XmlnsXlink = root.GetNamespaceOfPrefix("xlink")?.NamespaceName ?? string.Empty,
            Version = root.Attribute("version")?.Value ?? string.Empty,
            BaseProfile = root.Attribute("baseProfile")?.Value ?? string.Empty,
            X = GetFloat(root, "x"),
            Y = GetFloat(root, "y"),
            PreserveAspectRatio = root.Attribute("preserveAspectRatio")?.Value ?? string.Empty,
            ContentScriptType = root.Attribute("contentScriptType")?.Value ?? string.Empty,
            ContentStyleType = root.Attribute("contentStyleType")?.Value ?? string.Empty,
            ZoomAndPan = root.Attribute("zoomAndPan")?.Value ?? string.Empty,
            Class = root.Attribute("class")?.Value ?? string.Empty,
            Style = root.Attribute("style")?.Value ?? string.Empty,
            Id = root.Attribute("id")?.Value ?? string.Empty,
            XmlSpace = root.Attribute(XNamespace.Xml + "space")?.Value ?? string.Empty,
            XmlLang = root.Attribute(XNamespace.Xml + "lang")?.Value ?? string.Empty,
            XmlBase = root.Attribute(XNamespace.Xml + "base")?.Value ?? string.Empty
        };

        var group = new SvgGroup();
        ParseChildren(root, group, root.Name.Namespace);
        document.RootElement = group;

        PopulateElementsById(document);

        return document;
    }

    private void PopulateElementsById(SvgDocument document)
    {
        void AddElement(SvgElement element)
        {
            if (!string.IsNullOrEmpty(element.Id))
            {
                document.ElementsById[element.Id] = element;
            }

            if (element is SvgGroup group)
            {
                foreach (var child in group.Children ?? Enumerable.Empty<SvgElement>())
                {
                    AddElement(child);
                }
            }
            else if (element is SvgDefs defs)
            {
                foreach (var child in defs.Children ?? Enumerable.Empty<SvgElement>())
                {
                    AddElement(child);
                }
            }
            else if (element is SvgSwitch sw)
            {
                foreach (var child in sw.Children ?? Enumerable.Empty<SvgElement>())
                {
                    AddElement(child);
                }
            }
            else if (element is SvgSymbol sym)
            {
                foreach (var child in sym.Children ?? Enumerable.Empty<SvgElement>())
                {
                    AddElement(child);
                }
            }
            else if (element is SvgAnchor anchor)
            {
                foreach (var child in anchor.Children ?? Enumerable.Empty<SvgElement>())
                {
                    AddElement(child);
                }
            }
            else if (element is SvgText txt)
            {
                foreach (var child in txt.Children ?? Enumerable.Empty<SvgElement>())
                {
                    AddElement(child);
                }
            }
            else if (element is SvgTSpan tspan)
            {
                foreach (var child in tspan.Children ?? Enumerable.Empty<SvgElement>())
                {
                    AddElement(child);
                }
            }
            else if (element is SvgMarker marker)
            {
                foreach (var child in marker.Children ?? Enumerable.Empty<SvgElement>())
                {
                    AddElement(child);
                }
            }
            else if (element is SvgClipPath clipPath)
            {
                foreach (var child in clipPath.Children ?? Enumerable.Empty<SvgElement>())
                {
                    AddElement(child);
                }
            }
            else if (element is SvgMask mask)
            {
                foreach (var child in mask.Children ?? Enumerable.Empty<SvgElement>())
                {
                    AddElement(child);
                }
            }
        }

        if (document.RootElement != null)
        {
            AddElement(document.RootElement);
        }
    }

    private void ParseChildren(XElement parent, SvgGroup group, XNamespace ns)
    {
        if (++_currentDepth > _options.MaxNestingDepth)
        {
            throw new InvalidOperationException($"Maximum nesting depth of {_options.MaxNestingDepth} exceeded");
        }

        foreach (var child in parent.Elements())
        {
            if (++_elementCount > _options.MaxElements)
            {
                throw new InvalidOperationException($"Maximum element count of {_options.MaxElements} exceeded");
            }

            string tagName = child.Name.LocalName.ToLower();

            SvgElement? element = tagName switch
            {
                "g" => ParseGroup(child, ns),
                "a" => ParseAnchor(child, ns),
                "use" => ParseUse(child),
                "rect" => ParseRect(child),
                "circle" => ParseCircle(child),
                "ellipse" => ParseEllipse(child),
                "line" => ParseLine(child),
                "polyline" => ParsePolyline(child),
                "polygon" => ParsePolygon(child),
                "path" => ParsePath(child),
                "image" => ParseImage(child),
                "text" => ParseText(child, ns),
                "tspan" => ParseTSpan(child, ns),
                "textpath" => ParseTextPath(child),
                "switch" => ParseSwitch(child, ns),
                "foreignobject" => ParseForeignObject(child),
                "marker" => ParseMarker(child, ns),
                "clippath" => ParseClipPath(child, ns),
                "mask" => ParseMask(child, ns),
                "style" => ParseStyleElement(child),
                "animatemotion" => ParseAnimateMotion(child),
                "animatetransform" => ParseAnimateTransform(child),
                "mpath" => ParseMPath(child),
                "set" => ParseSet(child),
                "view" => ParseView(child),
                "title" => ParseTitle(child),
                "desc" => ParseDesc(child),
                "script" => ParseScript(child),
                "metadata" => ParseMetadata(child),
                "cursor" => ParseCursor(child),
                "solidcolor" => ParseSolidColor(child),
                "lineargradient" => ParseLinearGradient(child, ns),
                "radialgradient" => ParseRadialGradient(child, ns),
                "pattern" => ParsePattern(child, ns),
                "filter" => ParseFilter(child, ns),
                "font" => ParseFont(child),
                "font-face" => ParseFontFace(child),
                "glyph" => ParseGlyph(child),
                "missing-glyph" => ParseMissingGlyph(child),
                "hkern" => ParseHKern(child),
                "vkern" => ParseVKern(child),
                "font-face-src" => ParseFontFaceSrc(child),
                "font-face-uri" => ParseFontFaceUri(child),
                "font-face-format" => ParseFontFaceFormat(child),
                "font-face-name" => ParseFontFaceName(child),
                "color-profile" => ParseColorProfile(child),
                _ => null
            };

            if (element != null)
            {
                element.Id = child.Attribute("id")?.Value ?? string.Empty;
                element.Transform = ParseTransform(child.Attribute("transform")?.Value);
                var classAttr = child.Attribute("class")?.Value;

                element.FillStyle = ParseStyle(child, classAttr, true);
                element.StrokeStyle = ParseStyle(child, classAttr, false);

                group.Children ??= new List<SvgElement>();
                group.Children.Add(element);
            }
        }

        _currentDepth--;
    }

    private void ParseCssStyles(XElement root)
    {
        foreach (var styleElement in root.Descendants(root.Name.Namespace + "style"))
        {
            var cssText = styleElement.Value;
            var classRegex = new Regex(@"\.([a-zA-Z0-9_-]+)\s*\{([^}]+)\}");
            var matches = classRegex.Matches(cssText);

            foreach (Match match in matches)
            {
                string className = match.Groups[1].Value;
                string classContent = match.Groups[2].Value;

                var fillMatch = FillStyleRegex.Match(classContent);
                var strokeMatch = StrokeStyleRegex.Match(classContent);

                SvgStyle? fillStyle = null;
                SvgStyle? strokeStyle = null;

                if (fillMatch.Success)
                {
                    string colorVal = fillMatch.Groups[1].Value.Trim();
                    if (!string.IsNullOrEmpty(colorVal) && !colorVal.Equals("none", StringComparison.OrdinalIgnoreCase))
                    {
                        if (SvgColorHelper.TryParse(colorVal, out SKColor color))
                        {
                            fillStyle = new SvgStyle { Color = color };
                        }
                    }
                }

                if (strokeMatch.Success)
                {
                    string colorVal = strokeMatch.Groups[1].Value.Trim();
                    if (!string.IsNullOrEmpty(colorVal) && !colorVal.Equals("none", StringComparison.OrdinalIgnoreCase))
                    {
                        if (SvgColorHelper.TryParse(colorVal, out SKColor color))
                        {
                            strokeStyle = new SvgStyle { Color = color };
                        }
                    }
                }

                _cssClasses[className] = (fillStyle: fillStyle, strokeStyle: strokeStyle);
            }
        }
    }

    private SvgGroup ParseGroup(XElement element, XNamespace ns)
    {
        var group = new SvgGroup
        {
            Opacity = GetFloat(element, "opacity", 1.0f),
            Display = element.Attribute("display")?.Value ?? string.Empty,
            Visibility = element.Attribute("visibility")?.Value ?? string.Empty,
            ClipPath = element.Attribute("clip-path")?.Value ?? string.Empty,
            Mask = element.Attribute("mask")?.Value ?? string.Empty,
            Filter = element.Attribute("filter")?.Value ?? string.Empty,
            XmlSpace = element.Attribute(XNamespace.Xml + "space")?.Value ?? string.Empty,
            XmlLang = element.Attribute(XNamespace.Xml + "lang")?.Value ?? string.Empty,
            XmlBase = element.Attribute(XNamespace.Xml + "base")?.Value ?? string.Empty
        };

        ParseStyleAttributes(group, element);

        ParseChildren(element, group, ns);
        return group;
    }

    private SvgDefs ParseDefs(XElement element, XNamespace ns)
    {
        var defs = new SvgDefs
        {
            Class = element.Attribute("class")?.Value ?? string.Empty,
            Style = element.Attribute("style")?.Value ?? string.Empty,
            XmlSpace = element.Attribute(XNamespace.Xml + "space")?.Value ?? string.Empty,
            XmlLang = element.Attribute(XNamespace.Xml + "lang")?.Value ?? string.Empty,
            XmlBase = element.Attribute(XNamespace.Xml + "base")?.Value ?? string.Empty
        };

        defs.Id = element.Attribute("id")?.Value ?? string.Empty;
        defs.Transform = ParseTransform(element.Attribute("transform")?.Value);

        ParseChildrenToDefs(element, defs, ns);
        return defs;
    }

    private SvgSymbol ParseSymbol(XElement element, XNamespace ns)
    {
        var symbol = new SvgSymbol
        {
            ViewBox = ParseViewBoxAttribute(element),
            PreserveAspectRatio = element.Attribute("preserveAspectRatio")?.Value ?? string.Empty,
            Width = ParseDimension(element, "width", 0),
            Height = ParseDimension(element, "height", 0),
            X = GetFloat(element, "x"),
            Y = GetFloat(element, "y"),
            RefX = GetFloat(element, "refX"),
            RefY = GetFloat(element, "refY"),
            Class = element.Attribute("class")?.Value ?? string.Empty,
            Style = element.Attribute("style")?.Value ?? string.Empty,
            XmlSpace = element.Attribute(XNamespace.Xml + "space")?.Value ?? string.Empty,
            XmlLang = element.Attribute(XNamespace.Xml + "lang")?.Value ?? string.Empty,
            XmlBase = element.Attribute(XNamespace.Xml + "base")?.Value ?? string.Empty
        };

        ParseChildrenToSymbol(element, symbol, ns);
        return symbol;
    }

    private void ParseChildrenToSymbol(XElement parent, SvgSymbol symbol, XNamespace ns)
    {
        if (++_currentDepth > _options.MaxNestingDepth)
        {
            throw new InvalidOperationException($"Maximum nesting depth of {_options.MaxNestingDepth} exceeded");
        }

        foreach (var child in parent.Elements())
        {
            if (++_elementCount > _options.MaxElements)
            {
                throw new InvalidOperationException($"Maximum element count of {_options.MaxElements} exceeded");
            }

            string tagName = child.Name.LocalName.ToLower();

            SvgElement? element = tagName switch
            {
                "g" => ParseGroup(child, ns),
                "a" => ParseAnchor(child, ns),
                "rect" => ParseRect(child),
                "circle" => ParseCircle(child),
                "ellipse" => ParseEllipse(child),
                "line" => ParseLine(child),
                "polyline" => ParsePolyline(child),
                "polygon" => ParsePolygon(child),
                "path" => ParsePath(child),
                "image" => ParseImage(child),
                "text" => ParseText(child, ns),
                "tspan" => ParseTSpan(child, ns),
                "textpath" => ParseTextPath(child),
                "switch" => ParseSwitch(child, ns),
                "foreignobject" => ParseForeignObject(child),
                "marker" => ParseMarker(child, ns),
                "clippath" => ParseClipPath(child, ns),
                "mask" => ParseMask(child, ns),
                "style" => ParseStyleElement(child),
                "animatemotion" => ParseAnimateMotion(child),
                "animatetransform" => ParseAnimateTransform(child),
                "mpath" => ParseMPath(child),
                "set" => ParseSet(child),
                "view" => ParseView(child),
                "title" => ParseTitle(child),
                "desc" => ParseDesc(child),
                "script" => ParseScript(child),
                "metadata" => ParseMetadata(child),
                "cursor" => ParseCursor(child),
                "solidcolor" => ParseSolidColor(child),
                "lineargradient" => ParseLinearGradient(child, ns),
                "radialgradient" => ParseRadialGradient(child, ns),
                "pattern" => ParsePattern(child, ns),
                "font" => ParseFont(child),
                "font-face" => ParseFontFace(child),
                "glyph" => ParseGlyph(child),
                "missing-glyph" => ParseMissingGlyph(child),
                "hkern" => ParseHKern(child),
                "vkern" => ParseVKern(child),
                "font-face-src" => ParseFontFaceSrc(child),
                "font-face-uri" => ParseFontFaceUri(child),
                "font-face-format" => ParseFontFaceFormat(child),
                "font-face-name" => ParseFontFaceName(child),
                "color-profile" => ParseColorProfile(child),
                _ => null
            };

            if (element != null)
            {
                element.Id = child.Attribute("id")?.Value ?? string.Empty;
                element.Transform = ParseTransform(child.Attribute("transform")?.Value);
                var classAttr = child.Attribute("class")?.Value;

                element.FillStyle = ParseStyle(child, classAttr, true);
                element.StrokeStyle = ParseStyle(child, classAttr, false);

                symbol.Children ??= new List<SvgElement>();
                symbol.Children.Add(element);
            }
        }

        _currentDepth--;
    }

    private void ParseChildrenToDefs(XElement parent, SvgDefs defs, XNamespace ns)
    {
        if (++_currentDepth > _options.MaxNestingDepth)
        {
            throw new InvalidOperationException($"Maximum nesting depth of {_options.MaxNestingDepth} exceeded");
        }

        foreach (var child in parent.Elements())
        {
            if (++_elementCount > _options.MaxElements)
            {
                throw new InvalidOperationException($"Maximum element count of {_options.MaxElements} exceeded");
            }

            string tagName = child.Name.LocalName.ToLower();

            SvgElement? element = tagName switch
            {
                "g" => ParseGroup(child, ns),
                "a" => ParseAnchor(child, ns),
                "rect" => ParseRect(child),
                "circle" => ParseCircle(child),
                "ellipse" => ParseEllipse(child),
                "line" => ParseLine(child),
                "polyline" => ParsePolyline(child),
                "polygon" => ParsePolygon(child),
                "path" => ParsePath(child),
                "image" => ParseImage(child),
                "text" => ParseText(child, ns),
                "tspan" => ParseTSpan(child, ns),
                "textpath" => ParseTextPath(child),
                "switch" => ParseSwitch(child, ns),
                "foreignobject" => ParseForeignObject(child),
                "marker" => ParseMarker(child, ns),
                "clippath" => ParseClipPath(child, ns),
                "mask" => ParseMask(child, ns),
                "style" => ParseStyleElement(child),
                "animatemotion" => ParseAnimateMotion(child),
                "animatetransform" => ParseAnimateTransform(child),
                "mpath" => ParseMPath(child),
                "set" => ParseSet(child),
                "view" => ParseView(child),
                "title" => ParseTitle(child),
                "desc" => ParseDesc(child),
                "script" => ParseScript(child),
                "metadata" => ParseMetadata(child),
                "cursor" => ParseCursor(child),
                "solidcolor" => ParseSolidColor(child),
                "lineargradient" => ParseLinearGradient(child, ns),
                "radialgradient" => ParseRadialGradient(child, ns),
                "pattern" => ParsePattern(child, ns),
                "font" => ParseFont(child),
                "font-face" => ParseFontFace(child),
                "glyph" => ParseGlyph(child),
                "missing-glyph" => ParseMissingGlyph(child),
                "hkern" => ParseHKern(child),
                "vkern" => ParseVKern(child),
                "font-face-src" => ParseFontFaceSrc(child),
                "font-face-uri" => ParseFontFaceUri(child),
                "font-face-format" => ParseFontFaceFormat(child),
                "font-face-name" => ParseFontFaceName(child),
                "color-profile" => ParseColorProfile(child),
                _ => null
            };

            if (element != null)
            {
                element.Id = child.Attribute("id")?.Value ?? string.Empty;
                element.Transform = ParseTransform(child.Attribute("transform")?.Value);
                var classAttr = child.Attribute("class")?.Value;

                element.FillStyle = ParseStyle(child, classAttr, true);
                element.StrokeStyle = ParseStyle(child, classAttr, false);

                defs.Children ??= new List<SvgElement>();
                defs.Children.Add(element);
            }
        }

        _currentDepth--;
    }

    private void ParseStyleAttributes(SvgGroup group, XElement element)
    {
        var fillStyle = new SvgStyle();
        var strokeStyle = new SvgStyle();

        string? fill = element.Attribute("fill")?.Value;
        if (!string.IsNullOrEmpty(fill) && !fill.Equals("none", StringComparison.OrdinalIgnoreCase))
        {
            if (SvgColorHelper.TryParse(fill, out SKColor fillColor))
            {
                fillStyle.Color = fillColor;
            }
        }

        fillStyle.FillOpacity = GetFloat(element, "fill-opacity", 1.0f);
        fillStyle.FillRule = ParseFillRule(element.Attribute("fill-rule")?.Value);

        string? stroke = element.Attribute("stroke")?.Value;
        if (!string.IsNullOrEmpty(stroke) && !stroke.Equals("none", StringComparison.OrdinalIgnoreCase))
        {
            if (SvgColorHelper.TryParse(stroke, out SKColor strokeColor))
            {
                strokeStyle.Color = strokeColor;
            }
        }

        strokeStyle.StrokeWidth = GetFloat(element, "stroke-width", 1.0f);
        strokeStyle.StrokeOpacity = GetFloat(element, "stroke-opacity", 1.0f);
        strokeStyle.StrokeDashOffset = GetFloat(element, "stroke-dashoffset", 0);
        strokeStyle.StrokeMiterLimit = GetFloat(element, "stroke-miterlimit", 4.0f);
        strokeStyle.StrokeDashArray = ParseStrokeDashArray(element.Attribute("stroke-dasharray")?.Value);
        strokeStyle.StrokeLineCap = ParseStrokeCap(element.Attribute("stroke-linecap")?.Value);
        strokeStyle.StrokeLineJoin = ParseStrokeJoin(element.Attribute("stroke-linejoin")?.Value);

        if (fillStyle.Color != SKColors.Black || fillStyle.FillOpacity != 1.0f || fillStyle.FillRule != SKPathFillType.Winding)
        {
            group.FillStyle = fillStyle;
        }

        if (strokeStyle.Color != SKColors.Black || strokeStyle.StrokeWidth != 1.0f || strokeStyle.StrokeOpacity != 1.0f ||
            strokeStyle.StrokeDashArray.Length > 0 || strokeStyle.StrokeDashOffset != 0 || strokeStyle.StrokeMiterLimit != 4.0f ||
            strokeStyle.StrokeLineCap != SKStrokeCap.Butt || strokeStyle.StrokeLineJoin != SKStrokeJoin.Miter)
        {
            group.StrokeStyle = strokeStyle;
        }
    }

    private SKPathFillType ParseFillRule(string? fillRule)
    {
        if (string.IsNullOrEmpty(fillRule))
            return SKPathFillType.Winding;

        return fillRule.ToLowerInvariant() switch
        {
            "nonzero" => SKPathFillType.Winding,
            "evenodd" => SKPathFillType.EvenOdd,
            _ => SKPathFillType.Winding
        };
    }

    private SKStrokeCap ParseStrokeCap(string? strokeCap)
    {
        if (string.IsNullOrEmpty(strokeCap))
            return SKStrokeCap.Butt;

        return strokeCap.ToLowerInvariant() switch
        {
            "butt" => SKStrokeCap.Butt,
            "round" => SKStrokeCap.Round,
            "square" => SKStrokeCap.Square,
            _ => SKStrokeCap.Butt
        };
    }

    private SKStrokeJoin ParseStrokeJoin(string? strokeJoin)
    {
        if (string.IsNullOrEmpty(strokeJoin))
            return SKStrokeJoin.Miter;

        return strokeJoin.ToLowerInvariant() switch
        {
            "miter" => SKStrokeJoin.Miter,
            "round" => SKStrokeJoin.Round,
            "bevel" => SKStrokeJoin.Bevel,
            _ => SKStrokeJoin.Miter
        };
    }

    private float[] ParseStrokeDashArray(string? dashArray)
    {
        if (string.IsNullOrEmpty(dashArray))
            return Array.Empty<float>();

        var values = dashArray.Split(new[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);
        var result = new List<float>();

        foreach (var val in values)
        {
            if (float.TryParse(val, NumberStyles.Float, CultureInfo.InvariantCulture, out float num))
            {
                result.Add(num);
            }
        }

        return result.ToArray();
    }

    private float GetFloat(XElement element, string attrName, float defaultValue)
    {
        string? val = element.Attribute(attrName)?.Value;
        if (string.IsNullOrEmpty(val)) return defaultValue;

        float.TryParse(val.Replace("px", ""), NumberStyles.Any, CultureInfo.InvariantCulture, out float result);
        return result;
    }

    private int GetInt(XElement element, string attrName, int defaultValue = 0)
    {
        string? val = element.Attribute(attrName)?.Value;
        if (string.IsNullOrEmpty(val)) return defaultValue;

        int.TryParse(val, NumberStyles.Any, CultureInfo.InvariantCulture, out int result);
        return result;
    }

    private SvgRect ParseRect(XElement element)
    {
        return new SvgRect
        {
            X = GetFloat(element, "x"),
            Y = GetFloat(element, "y"),
            Width = GetFloat(element, "width"),
            Height = GetFloat(element, "height"),
            Rx = GetFloat(element, "rx"),
            Ry = GetFloat(element, "ry"),
            PathLength = GetFloat(element, "pathLength"),
            Opacity = GetFloat(element, "opacity", 1.0f),
            Display = element.Attribute("display")?.Value ?? string.Empty,
            Visibility = element.Attribute("visibility")?.Value ?? string.Empty,
            ClipPath = element.Attribute("clip-path")?.Value ?? string.Empty,
            Mask = element.Attribute("mask")?.Value ?? string.Empty,
            Class = element.Attribute("class")?.Value ?? string.Empty,
            Style = element.Attribute("style")?.Value ?? string.Empty
        };
    }

    private SvgUse ParseUse(XElement element)
    {
        return new SvgUse
        {
            Href = element.Attribute("href")?.Value ?? element.Attribute(xlinkNamespace + "href")?.Value,
            X = GetFloat(element, "x"),
            Y = GetFloat(element, "y"),
            Width = ParseDimension(element, "width", 0),
            Height = ParseDimension(element, "height", 0),
            Opacity = GetFloat(element, "opacity", 1.0f),
            Display = element.Attribute("display")?.Value ?? string.Empty,
            Visibility = element.Attribute("visibility")?.Value ?? string.Empty,
            ClipPath = element.Attribute("clip-path")?.Value ?? string.Empty,
            Mask = element.Attribute("mask")?.Value ?? string.Empty,
            Filter = element.Attribute("filter")?.Value ?? string.Empty,
            Class = element.Attribute("class")?.Value ?? string.Empty,
            Style = element.Attribute("style")?.Value ?? string.Empty
        };
    }

    private SvgCircle ParseCircle(XElement element)
    {
        return new SvgCircle
        {
            Cx = GetFloat(element, "cx"),
            Cy = GetFloat(element, "cy"),
            Radius = GetFloat(element, "r"),
            PathLength = GetFloat(element, "pathLength"),
            Opacity = GetFloat(element, "opacity", 1.0f),
            Display = element.Attribute("display")?.Value ?? string.Empty,
            Visibility = element.Attribute("visibility")?.Value ?? string.Empty,
            ClipPath = element.Attribute("clip-path")?.Value ?? string.Empty,
            Mask = element.Attribute("mask")?.Value ?? string.Empty,
            Class = element.Attribute("class")?.Value ?? string.Empty,
            Style = element.Attribute("style")?.Value ?? string.Empty
        };
    }

    private SvgEllipse ParseEllipse(XElement element)
    {
        return new SvgEllipse
        {
            Cx = GetFloat(element, "cx"),
            Cy = GetFloat(element, "cy"),
            Rx = GetFloat(element, "rx"),
            Ry = GetFloat(element, "ry"),
            PathLength = GetFloat(element, "pathLength"),
            Opacity = GetFloat(element, "opacity", 1.0f),
            Display = element.Attribute("display")?.Value ?? string.Empty,
            Visibility = element.Attribute("visibility")?.Value ?? string.Empty,
            ClipPath = element.Attribute("clip-path")?.Value ?? string.Empty,
            Mask = element.Attribute("mask")?.Value ?? string.Empty,
            Class = element.Attribute("class")?.Value ?? string.Empty,
            Style = element.Attribute("style")?.Value ?? string.Empty
        };
    }

    private SvgLine ParseLine(XElement element)
    {
        return new SvgLine
        {
            X1 = GetFloat(element, "x1"),
            Y1 = GetFloat(element, "y1"),
            X2 = GetFloat(element, "x2"),
            Y2 = GetFloat(element, "y2"),
            PathLength = GetFloat(element, "pathLength"),
            Opacity = GetFloat(element, "opacity", 1.0f),
            Display = element.Attribute("display")?.Value ?? string.Empty,
            Visibility = element.Attribute("visibility")?.Value ?? string.Empty,
            ClipPath = element.Attribute("clip-path")?.Value ?? string.Empty,
            Mask = element.Attribute("mask")?.Value ?? string.Empty,
            Class = element.Attribute("class")?.Value ?? string.Empty,
            Style = element.Attribute("style")?.Value ?? string.Empty
        };
    }

    private SvgPolyline ParsePolyline(XElement element)
    {
        return new SvgPolyline
        {
            Points = ParsePoints(element.Attribute("points")?.Value),
            PathLength = GetFloat(element, "pathLength"),
            Opacity = GetFloat(element, "opacity", 1.0f),
            Display = element.Attribute("display")?.Value ?? string.Empty,
            Visibility = element.Attribute("visibility")?.Value ?? string.Empty,
            ClipPath = element.Attribute("clip-path")?.Value ?? string.Empty,
            Mask = element.Attribute("mask")?.Value ?? string.Empty,
            Class = element.Attribute("class")?.Value ?? string.Empty,
            Style = element.Attribute("style")?.Value ?? string.Empty
        };
    }

    private SvgPolygon ParsePolygon(XElement element)
    {
        return new SvgPolygon
        {
            Points = ParsePoints(element.Attribute("points")?.Value),
            PathLength = GetFloat(element, "pathLength"),
            Opacity = GetFloat(element, "opacity", 1.0f),
            Display = element.Attribute("display")?.Value ?? string.Empty,
            Visibility = element.Attribute("visibility")?.Value ?? string.Empty,
            ClipPath = element.Attribute("clip-path")?.Value ?? string.Empty,
            Mask = element.Attribute("mask")?.Value ?? string.Empty,
            Class = element.Attribute("class")?.Value ?? string.Empty,
            Style = element.Attribute("style")?.Value ?? string.Empty
        };
    }

    private SvgPath ParsePath(XElement element)
    {
        var pathData = element.Attribute("d")?.Value ?? string.Empty;
        var path = SKPath.ParseSvgPathData(pathData);

        return new SvgPath
        {
            Path = path,
            PathLength = GetFloat(element, "pathLength"),
            Opacity = GetFloat(element, "opacity", 1.0f),
            Display = element.Attribute("display")?.Value ?? string.Empty,
            Visibility = element.Attribute("visibility")?.Value ?? string.Empty,
            ClipPath = element.Attribute("clip-path")?.Value ?? string.Empty,
            Mask = element.Attribute("mask")?.Value ?? string.Empty,
            MarkerStart = element.Attribute("marker-start")?.Value ?? string.Empty,
            MarkerMid = element.Attribute("marker-mid")?.Value ?? string.Empty,
            MarkerEnd = element.Attribute("marker-end")?.Value ?? string.Empty,
            Class = element.Attribute("class")?.Value ?? string.Empty,
            Style = element.Attribute("style")?.Value ?? string.Empty
        };
    }

    private SvgImage ParseImage(XElement element)
    {
        return new SvgImage
        {
            Href = element.Attribute("href")?.Value ?? string.Empty,
            XlinkHref = element.Attribute(xlinkNamespace + "href")?.Value ?? string.Empty,
            X = GetFloat(element, "x"),
            Y = GetFloat(element, "y"),
            Width = ParseDimension(element, "width", 0),
            Height = ParseDimension(element, "height", 0),
            PreserveAspectRatio = element.Attribute("preserveAspectRatio")?.Value ?? string.Empty,
            Opacity = GetFloat(element, "opacity", 1.0f),
            Display = element.Attribute("display")?.Value ?? string.Empty,
            Visibility = element.Attribute("visibility")?.Value ?? string.Empty,
            ClipPath = element.Attribute("clip-path")?.Value ?? string.Empty,
            Mask = element.Attribute("mask")?.Value ?? string.Empty,
            Filter = element.Attribute("filter")?.Value ?? string.Empty,
            CrossOrigin = element.Attribute("crossOrigin")?.Value ?? string.Empty,
            Class = element.Attribute("class")?.Value ?? string.Empty,
            Style = element.Attribute("style")?.Value ?? string.Empty
        };
    }

    private SvgAnimateMotion ParseAnimateMotion(XElement element)
    {
        return new SvgAnimateMotion
        {
            Path = element.Attribute("path")?.Value,
            KeyPoints = element.Attribute("keyPoints")?.Value,
            KeyTimes = element.Attribute("keyTimes")?.Value,
            Rotate = element.Attribute("rotate")?.Value,
            From = element.Attribute("from")?.Value,
            To = element.Attribute("to")?.Value,
            By = element.Attribute("by")?.Value,
            Begin = element.Attribute("begin")?.Value,
            Dur = element.Attribute("dur")?.Value,
            RepeatCount = element.Attribute("repeatCount")?.Value,
            CalcMode = element.Attribute("calcMode")?.Value
        };
    }

    private SvgAnimateTransform ParseAnimateTransform(XElement element)
    {
        return new SvgAnimateTransform
        {
            Type = element.Attribute("type")?.Value,
            From = element.Attribute("from")?.Value,
            To = element.Attribute("to")?.Value,
            By = element.Attribute("by")?.Value,
            Begin = element.Attribute("begin")?.Value,
            Dur = element.Attribute("dur")?.Value,
            RepeatCount = element.Attribute("repeatCount")?.Value,
            Additive = element.Attribute("additive")?.Value,
            Accumulate = element.Attribute("accumulate")?.Value
        };
    }

    private SvgMPath ParseMPath(XElement element)
    {
        return new SvgMPath
        {
            Path = element.Attribute("path")?.Value,
            Href = element.Attribute("href")?.Value ?? element.Attribute(xlinkNamespace + "href")?.Value
        };
    }

    private SvgSet ParseSet(XElement element)
    {
        return new SvgSet
        {
            AttributeName = element.Attribute("attributeName")?.Value,
            To = element.Attribute("to")?.Value,
            Begin = element.Attribute("begin")?.Value,
            Dur = element.Attribute("dur")?.Value
        };
    }

    private SvgView ParseView(XElement element)
    {
        return new SvgView
        {
            ViewBox = ParseViewBoxAttribute(element),
            ZoomAndPan = GetFloat(element, "zoomAndPan"),
            ViewTargetX = GetFloat(element, "viewTargetX"),
            ViewTargetY = GetFloat(element, "viewTargetY")
        };
    }

    private SKRect? ParseViewBoxAttribute(XElement element)
    {
        var vb = element.Attribute("viewBox");
        if (vb != null)
        {
            var values = new float[4];
            int valueIndex = 0;
            int i = 0;
            string vbValue = vb.Value;

            while (i < vbValue.Length && valueIndex < 4)
            {
                while (i < vbValue.Length && (vbValue[i] == ' ' || vbValue[i] == ',')) i++;
                if (i >= vbValue.Length) break;

                int start = i;
                while (i < vbValue.Length && (char.IsDigit(vbValue[i]) || vbValue[i] == '.' || vbValue[i] == '-' || vbValue[i] == '+' || vbValue[i] == 'e' || vbValue[i] == 'E')) i++;

                if (float.TryParse(vbValue.AsSpan(start, i - start), NumberStyles.Float, CultureInfo.InvariantCulture, out float num))
                {
                    values[valueIndex++] = num;
                }
            }

            if (valueIndex == 4)
                return SKRect.Create(values[0], values[1], values[2], values[3]);
        }

        return null;
    }

    private SvgScript ParseScript(XElement element)
    {
        return new SvgScript
        {
            Type = element.Attribute("type")?.Value,
            Content = element.Value
        };
    }

    private SvgMetadata ParseMetadata(XElement element)
    {
        return new SvgMetadata
        {
            Content = element.Value
        };
    }

    private SvgDesc ParseDesc(XElement element)
    {
        return new SvgDesc
        {
            Class = element.Attribute("class")?.Value ?? string.Empty,
            Style = element.Attribute("style")?.Value ?? string.Empty,
            XmlSpace = element.Attribute(XNamespace.Xml + "space")?.Value ?? string.Empty,
            XmlLang = element.Attribute(XNamespace.Xml + "lang")?.Value ?? string.Empty,
            XmlBase = element.Attribute(XNamespace.Xml + "base")?.Value ?? string.Empty,
            Text = element.Value
        };
    }

    private SvgTitle ParseTitle(XElement element)
    {
        return new SvgTitle
        {
            Class = element.Attribute("class")?.Value ?? string.Empty,
            Style = element.Attribute("style")?.Value ?? string.Empty,
            XmlSpace = element.Attribute(XNamespace.Xml + "space")?.Value ?? string.Empty,
            XmlLang = element.Attribute(XNamespace.Xml + "lang")?.Value ?? string.Empty,
            XmlBase = element.Attribute(XNamespace.Xml + "base")?.Value ?? string.Empty,
            Text = element.Value
        };
    }

    private SvgCursor ParseCursor(XElement element)
    {
        return new SvgCursor
        {
            X = GetFloat(element, "x"),
            Y = GetFloat(element, "y"),
            Href = element.Attribute("href")?.Value ?? element.Attribute(xlinkNamespace + "href")?.Value
        };
    }

    private SvgSolidColor ParseSolidColor(XElement element)
    {
        return new SvgSolidColor
        {
            SolidColor = element.Attribute("solid-color")?.Value,
            SolidOpacity = GetFloat(element, "solid-opacity"),
            Class = element.Attribute("class")?.Value ?? string.Empty,
            Style = element.Attribute("style")?.Value ?? string.Empty
        };
    }

    private SvgLinearGradient ParseLinearGradient(XElement element, XNamespace ns)
    {
        var gradient = new SvgLinearGradient
        {
            X1 = GetFloat(element, "x1"),
            Y1 = GetFloat(element, "y1"),
            X2 = GetFloat(element, "x2"),
            Y2 = GetFloat(element, "y2"),
            GradientUnits = element.Attribute("gradientUnits")?.Value,
            GradientTransform = element.Attribute("gradientTransform")?.Value,
            SpreadMethod = element.Attribute("spreadMethod")?.Value,
            Href = element.Attribute("href")?.Value,
            XlinkHref = element.Attribute(xlinkNamespace + "href")?.Value,
            Class = element.Attribute("class")?.Value ?? string.Empty,
            Style = element.Attribute("style")?.Value ?? string.Empty
        };

        gradient.Stops = new List<SvgGradientStop>();
        foreach (var child in element.Elements())
        {
            if (child.Name.LocalName.ToLower() == "stop")
            {
                gradient.Stops.Add(ParseStop(child));
            }
        }

        return gradient;
    }

    private SvgRadialGradient ParseRadialGradient(XElement element, XNamespace ns)
    {
        var gradient = new SvgRadialGradient
        {
            Cx = GetFloat(element, "cx"),
            Cy = GetFloat(element, "cy"),
            R = GetFloat(element, "r"),
            Fx = GetFloat(element, "fx"),
            Fy = GetFloat(element, "fy"),
            Fr = GetFloat(element, "fr"),
            GradientUnits = element.Attribute("gradientUnits")?.Value,
            GradientTransform = element.Attribute("gradientTransform")?.Value,
            SpreadMethod = element.Attribute("spreadMethod")?.Value,
            Href = element.Attribute("href")?.Value,
            XlinkHref = element.Attribute(xlinkNamespace + "href")?.Value,
            Class = element.Attribute("class")?.Value ?? string.Empty,
            Style = element.Attribute("style")?.Value ?? string.Empty
        };

        gradient.Stops = new List<SvgGradientStop>();
        foreach (var child in element.Elements())
        {
            if (child.Name.LocalName.ToLower() == "stop")
            {
                gradient.Stops.Add(ParseStop(child));
            }
        }

        return gradient;
    }

    private SvgPattern ParsePattern(XElement element, XNamespace ns)
    {
        var pattern = new SvgPattern
        {
            X = GetFloat(element, "x"),
            Y = GetFloat(element, "y"),
            Width = ParseDimension(element, "width", 0),
            Height = ParseDimension(element, "height", 0),
            PatternUnits = element.Attribute("patternUnits")?.Value,
            PatternContentUnits = element.Attribute("patternContentUnits")?.Value,
            PatternTransform = element.Attribute("patternTransform")?.Value,
            ViewBox = ParseViewBoxAttribute(element),
            PreserveAspectRatio = element.Attribute("preserveAspectRatio")?.Value ?? string.Empty,
            Href = element.Attribute("href")?.Value,
            XlinkHref = element.Attribute(xlinkNamespace + "href")?.Value,
            Class = element.Attribute("class")?.Value ?? string.Empty,
            Style = element.Attribute("style")?.Value ?? string.Empty
        };

        pattern.Id = element.Attribute("id")?.Value ?? string.Empty;
        pattern.Transform = ParseTransform(element.Attribute("transform")?.Value);

        ParseChildrenToPattern(element, pattern, ns);
        return pattern;
    }

    private SvgGradientStop ParseStop(XElement element)
    {
        return new SvgGradientStop
        {
            Offset = GetFloat(element, "offset"),
            StopColor = element.Attribute("stop-color")?.Value,
            StopOpacity = GetFloat(element, "stop-opacity", 1.0f),
            Class = element.Attribute("class")?.Value ?? string.Empty,
            Style = element.Attribute("style")?.Value ?? string.Empty,
            Id = element.Attribute("id")?.Value ?? string.Empty
        };
    }

    private void ParseChildrenToPattern(XElement parent, SvgPattern pattern, XNamespace ns)
    {
        if (++_currentDepth > _options.MaxNestingDepth)
        {
            throw new InvalidOperationException($"Maximum nesting depth of {_options.MaxNestingDepth} exceeded");
        }

        foreach (var child in parent.Elements())
        {
            if (++_elementCount > _options.MaxElements)
            {
                throw new InvalidOperationException($"Maximum element count of {_options.MaxElements} exceeded");
            }

            string tagName = child.Name.LocalName.ToLower();

            SvgElement? element = tagName switch
            {
                "g" => ParseGroup(child, ns),
                "a" => ParseAnchor(child, ns),
                "rect" => ParseRect(child),
                "circle" => ParseCircle(child),
                "ellipse" => ParseEllipse(child),
                "line" => ParseLine(child),
                "polyline" => ParsePolyline(child),
                "polygon" => ParsePolygon(child),
                "path" => ParsePath(child),
                "image" => ParseImage(child),
                "text" => ParseText(child, ns),
                "tspan" => ParseTSpan(child, ns),
                "textpath" => ParseTextPath(child),
                "switch" => ParseSwitch(child, ns),
                "foreignobject" => ParseForeignObject(child),
                "use" => ParseUse(child),
                _ => null
            };

            if (element != null)
            {
                element.Id = child.Attribute("id")?.Value ?? string.Empty;
                element.Transform = ParseTransform(child.Attribute("transform")?.Value);
                var classAttr = child.Attribute("class")?.Value;

                element.FillStyle = ParseStyle(child, classAttr, true);
                element.StrokeStyle = ParseStyle(child, classAttr, false);

                pattern.Children ??= new List<SvgElement>();
                pattern.Children.Add(element);
            }
        }

        _currentDepth--;
    }

    private SvgFont ParseFont(XElement element)
    {
        return new SvgFont
        {
            FontFamily = element.Attribute("font-family")?.Value,
            FontStyle = element.Attribute("font-style")?.Value,
            FontWeight = element.Attribute("font-weight")?.Value,
            FontSize = GetFloat(element, "font-size"),
            HorizAdvX = element.Attribute("horiz-adv-x")?.Value,
            VertOriginY = element.Attribute("vert-origin-y")?.Value,
            VertAdvY = element.Attribute("vert-adv-y")?.Value
        };
    }

    private SvgFontFace ParseFontFace(XElement element)
    {
        return new SvgFontFace
        {
            FontFamily = element.Attribute("font-family")?.Value,
            UnitsPerEm = element.Attribute("units-per-em")?.Value,
            Panose1 = element.Attribute("panose-1")?.Value,
            Ascent = element.Attribute("ascent")?.Value,
            Descent = element.Attribute("descent")?.Value,
            CapHeight = element.Attribute("cap-height")?.Value,
            XHeight = element.Attribute("x-height")?.Value,
            AccentHeight = element.Attribute("accent-height")?.Value,
            StemH = element.Attribute("stemh")?.Value,
            StemV = element.Attribute("stemv")?.Value,
            Slope = element.Attribute("slope")?.Value,
            FontStretch = element.Attribute("font-stretch")?.Value,
            FontWeight = element.Attribute("font-weight")?.Value,
            UnderlinePosition = element.Attribute("underline-position")?.Value,
            UnderlineThickness = element.Attribute("underline-thickness")?.Value,
            StrikethroughPosition = element.Attribute("strikethrough-position")?.Value,
            StrikethroughThickness = element.Attribute("strikethrough-thickness")?.Value,
            OverlinePosition = element.Attribute("overline-position")?.Value,
            OverlineThickness = element.Attribute("overline-thickness")?.Value,
            FontStyle = element.Attribute("font-style")?.Value,
            FontVariant = element.Attribute("font-variant")?.Value
        };
    }

    private SvgGlyph ParseGlyph(XElement element)
    {
        var pathData = element.Attribute("d")?.Value ?? string.Empty;
        var path = SKPath.ParseSvgPathData(pathData);

        return new SvgGlyph
        {
            Unicode = element.Attribute("unicode")?.Value,
            GlyphName = element.Attribute("glyph-name")?.Value,
            HorizAdvX = element.Attribute("horiz-adv-x")?.Value,
            VertAdvY = element.Attribute("vert-adv-y")?.Value,
            VertOriginX = element.Attribute("vert-origin-x")?.Value,
            VertOriginY = element.Attribute("vert-origin-y")?.Value,
            D = pathData,
            Path = path
        };
    }

    private SvgMissingGlyph ParseMissingGlyph(XElement element)
    {
        var pathData = element.Attribute("d")?.Value ?? string.Empty;
        var path = SKPath.ParseSvgPathData(pathData);

        return new SvgMissingGlyph
        {
            HorizAdvX = element.Attribute("horiz-adv-x")?.Value,
            D = pathData,
            Path = path
        };
    }

    private SvgHKern ParseHKern(XElement element)
    {
        return new SvgHKern
        {
            G1 = element.Attribute("g1")?.Value,
            G2 = element.Attribute("g2")?.Value,
            K = element.Attribute("k")?.Value,
            U1 = element.Attribute("u1")?.Value,
            U2 = element.Attribute("u2")?.Value
        };
    }

    private SvgVKern ParseVKern(XElement element)
    {
        return new SvgVKern
        {
            G1 = element.Attribute("g1")?.Value,
            G2 = element.Attribute("g2")?.Value,
            K = element.Attribute("k")?.Value,
            U1 = element.Attribute("u1")?.Value,
            U2 = element.Attribute("u2")?.Value
        };
    }

    private SvgFontFaceSrc ParseFontFaceSrc(XElement element)
    {
        return new SvgFontFaceSrc();
    }

    private SvgFontFaceUri ParseFontFaceUri(XElement element)
    {
        return new SvgFontFaceUri
        {
            Href = element.Attribute("href")?.Value ?? element.Attribute(xlinkNamespace + "href")?.Value
        };
    }

    private SvgFontFaceFormat ParseFontFaceFormat(XElement element)
    {
        return new SvgFontFaceFormat
        {
            String = element.Attribute("string")?.Value
        };
    }

    private SvgFontFaceName ParseFontFaceName(XElement element)
    {
        return new SvgFontFaceName
        {
            Name = element.Attribute("name")?.Value
        };
    }

    private SvgColorProfile ParseColorProfile(XElement element)
    {
        return new SvgColorProfile
        {
            Name = element.Attribute("name")?.Value,
            Local = element.Attribute("local")?.Value,
            RenderingIntent = element.Attribute("rendering-intent")?.Value,
            Href = element.Attribute("href")?.Value ?? element.Attribute(xlinkNamespace + "href")?.Value
        };
    }

    private SvgFilter ParseFilter(XElement element, XNamespace ns)
    {
        var filter = new SvgFilter
        {
            X = GetFloat(element, "x"),
            Y = GetFloat(element, "y"),
            Width = ParseDimension(element, "width", 0),
            Height = ParseDimension(element, "height", 0),
            FilterUnits = element.Attribute("filterUnits")?.Value,
            PrimitiveUnits = element.Attribute("primitiveUnits")?.Value,
            Href = element.Attribute("href")?.Value,
            XlinkHref = element.Attribute(xlinkNamespace + "href")?.Value,
            Class = element.Attribute("class")?.Value ?? string.Empty,
            Style = element.Attribute("style")?.Value ?? string.Empty
        };

        filter.Id = element.Attribute("id")?.Value ?? string.Empty;
        filter.Transform = ParseTransform(element.Attribute("transform")?.Value);

        ParseChildrenToFilter(element, filter, ns);
        return filter;
    }

    private SvgFeBlend ParseFeBlend(XElement element)
    {
        return new SvgFeBlend
        {
            In = element.Attribute("in")?.Value,
            In2 = element.Attribute("in2")?.Value,
            Mode = element.Attribute("mode")?.Value,
            X = GetFloat(element, "x"),
            Y = GetFloat(element, "y"),
            Width = GetFloat(element, "width"),
            Height = GetFloat(element, "height"),
            Result = element.Attribute("result")?.Value,
            Class = element.Attribute("class")?.Value ?? string.Empty,
            Style = element.Attribute("style")?.Value ?? string.Empty
        };
    }

    private SvgFeColorMatrix ParseFeColorMatrix(XElement element)
    {
        return new SvgFeColorMatrix
        {
            In = element.Attribute("in")?.Value,
            Type = element.Attribute("type")?.Value,
            Values = element.Attribute("values")?.Value,
            X = GetFloat(element, "x"),
            Y = GetFloat(element, "y"),
            Width = GetFloat(element, "width"),
            Height = GetFloat(element, "height"),
            Result = element.Attribute("result")?.Value,
            Class = element.Attribute("class")?.Value ?? string.Empty,
            Style = element.Attribute("style")?.Value ?? string.Empty
        };
    }

    private SvgFeComponentTransfer ParseFeComponentTransfer(XElement element)
    {
        var transfer = new SvgFeComponentTransfer
        {
            In = element.Attribute("in")?.Value,
            X = GetFloat(element, "x"),
            Y = GetFloat(element, "y"),
            Width = GetFloat(element, "width"),
            Height = GetFloat(element, "height"),
            Result = element.Attribute("result")?.Value,
            Class = element.Attribute("class")?.Value ?? string.Empty,
            Style = element.Attribute("style")?.Value ?? string.Empty
        };

        transfer.Id = element.Attribute("id")?.Value ?? string.Empty;

        foreach (var child in element.Elements())
        {
            string tagName = child.Name.LocalName.ToLower();
            if (tagName == "fefuncr")
            {
                transfer.FuncR = ParseFeFuncR(child);
            }
            else if (tagName == "fefuncg")
            {
                transfer.FuncG = ParseFeFuncG(child);
            }
            else if (tagName == "fefuncb")
            {
                transfer.FuncB = ParseFeFuncB(child);
            }
            else if (tagName == "fefunca")
            {
                transfer.FuncA = ParseFeFuncA(child);
            }
        }

        return transfer;
    }

    private SvgFeFuncR ParseFeFuncR(XElement element)
    {
        return new SvgFeFuncR
        {
            Type = element.Attribute("type")?.Value,
            TableValues = element.Attribute("tableValues")?.Value,
            Slope = GetFloat(element, "slope", 1.0f),
            Intercept = GetFloat(element, "intercept", 0.0f),
            Amplitude = GetFloat(element, "amplitude", 1.0f),
            Exponent = GetFloat(element, "exponent", 1.0f),
            Offset = GetFloat(element, "offset", 0.0f),
            Class = element.Attribute("class")?.Value ?? string.Empty,
            Style = element.Attribute("style")?.Value ?? string.Empty
        };
    }

    private SvgFeFuncG ParseFeFuncG(XElement element)
    {
        return new SvgFeFuncG
        {
            Type = element.Attribute("type")?.Value,
            TableValues = element.Attribute("tableValues")?.Value,
            Slope = GetFloat(element, "slope", 1.0f),
            Intercept = GetFloat(element, "intercept", 0.0f),
            Amplitude = GetFloat(element, "amplitude", 1.0f),
            Exponent = GetFloat(element, "exponent", 1.0f),
            Offset = GetFloat(element, "offset", 0.0f),
            Class = element.Attribute("class")?.Value ?? string.Empty,
            Style = element.Attribute("style")?.Value ?? string.Empty
        };
    }

    private SvgFeFuncB ParseFeFuncB(XElement element)
    {
        return new SvgFeFuncB
        {
            Type = element.Attribute("type")?.Value,
            TableValues = element.Attribute("tableValues")?.Value,
            Slope = GetFloat(element, "slope", 1.0f),
            Intercept = GetFloat(element, "intercept", 0.0f),
            Amplitude = GetFloat(element, "amplitude", 1.0f),
            Exponent = GetFloat(element, "exponent", 1.0f),
            Offset = GetFloat(element, "offset", 0.0f),
            Class = element.Attribute("class")?.Value ?? string.Empty,
            Style = element.Attribute("style")?.Value ?? string.Empty
        };
    }

    private SvgFeFuncA ParseFeFuncA(XElement element)
    {
        return new SvgFeFuncA
        {
            Type = element.Attribute("type")?.Value,
            TableValues = element.Attribute("tableValues")?.Value,
            Slope = GetFloat(element, "slope", 1.0f),
            Intercept = GetFloat(element, "intercept", 0.0f),
            Amplitude = GetFloat(element, "amplitude", 1.0f),
            Exponent = GetFloat(element, "exponent", 1.0f),
            Offset = GetFloat(element, "offset", 0.0f),
            Class = element.Attribute("class")?.Value ?? string.Empty,
            Style = element.Attribute("style")?.Value ?? string.Empty
        };
    }

    private SvgFeComposite ParseFeComposite(XElement element)
    {
        return new SvgFeComposite
        {
            In = element.Attribute("in")?.Value,
            In2 = element.Attribute("in2")?.Value,
            Operator = element.Attribute("operator")?.Value,
            K1 = element.Attribute("k1")?.Value,
            K2 = element.Attribute("k2")?.Value,
            K3 = element.Attribute("k3")?.Value,
            K4 = element.Attribute("k4")?.Value
        };
    }

    private SvgFeConvolveMatrix ParseFeConvolveMatrix(XElement element)
    {
        return new SvgFeConvolveMatrix
        {
            In = element.Attribute("in")?.Value,
            Order = element.Attribute("order")?.Value,
            KernelMatrix = element.Attribute("kernelMatrix")?.Value,
            Divisor = GetFloat(element, "divisor"),
            Bias = GetFloat(element, "bias"),
            TargetX = element.Attribute("targetX")?.Value,
            TargetY = element.Attribute("targetY")?.Value,
            EdgeMode = element.Attribute("edgeMode")?.Value,
            KernelUnitLength = element.Attribute("kernelUnitLength")?.Value,
            PreserveAlpha = element.Attribute("preserveAlpha")?.Value == "true"
        };
    }

    private SvgFeDiffuseLighting ParseFeDiffuseLighting(XElement element)
    {
        return new SvgFeDiffuseLighting
        {
            In = element.Attribute("in")?.Value,
            SurfaceScale = GetFloat(element, "surfaceScale"),
            DiffuseConstant = GetFloat(element, "diffuseConstant"),
            KernelUnitLength = element.Attribute("kernelUnitLength")?.Value
        };
    }

    private SvgFeDisplacementMap ParseFeDisplacementMap(XElement element)
    {
        return new SvgFeDisplacementMap
        {
            In = element.Attribute("in")?.Value,
            In2 = element.Attribute("in2")?.Value,
            Scale = element.Attribute("scale")?.Value,
            XChannelSelector = element.Attribute("xChannelSelector")?.Value,
            YChannelSelector = element.Attribute("yChannelSelector")?.Value
        };
    }

    private SvgFeDistantLight ParseFeDistantLight(XElement element)
    {
        return new SvgFeDistantLight
        {
            Azimuth = GetFloat(element, "azimuth"),
            Elevation = GetFloat(element, "elevation")
        };
    }

    private SvgFeFlood ParseFeFlood(XElement element)
    {
        return new SvgFeFlood
        {
            X = GetFloat(element, "x"),
            Y = GetFloat(element, "y"),
            Width = GetFloat(element, "width"),
            Height = GetFloat(element, "height"),
            FloodColor = element.Attribute("flood-color")?.Value,
            FloodOpacity = GetFloat(element, "flood-opacity", 1.0f)
        };
    }

    private SvgFeGaussianBlur ParseFeGaussianBlur(XElement element)
    {
        return new SvgFeGaussianBlur
        {
            In = element.Attribute("in")?.Value,
            StdDeviationX = GetFloat(element, "stdDeviation"),
            StdDeviationY = GetFloat(element, "stdDeviation"),
            EdgeMode = element.Attribute("edgeMode")?.Value
        };
    }

    private SvgFeImage ParseFeImage(XElement element)
    {
        return new SvgFeImage
        {
            In = element.Attribute("in")?.Value,
            Href = element.Attribute("href")?.Value ?? element.Attribute(xlinkNamespace + "href")?.Value,
            X = GetFloat(element, "x"),
            Y = GetFloat(element, "y"),
            Width = GetFloat(element, "width"),
            Height = GetFloat(element, "height"),
            PreserveAspectRatio = element.Attribute("preserveAspectRatio")?.Value
        };
    }

    private SvgFeMerge ParseFeMerge(XElement element)
    {
        var merge = new SvgFeMerge();
        merge.Nodes ??= new List<SvgFeMergeNode>();

        foreach (var child in element.Elements())
        {
            if (child.Name.LocalName.ToLower() == "femergenode")
            {
                merge.Nodes.Add(new SvgFeMergeNode
                {
                    In = child.Attribute("in")?.Value
                });
            }
        }

        return merge;
    }

    private SvgFeMorphology ParseFeMorphology(XElement element)
    {
        return new SvgFeMorphology
        {
            In = element.Attribute("in")?.Value,
            Operator = element.Attribute("operator")?.Value,
            RadiusX = GetFloat(element, "radius"),
            RadiusY = GetFloat(element, "radius")
        };
    }

    private SvgFeOffset ParseFeOffset(XElement element)
    {
        return new SvgFeOffset
        {
            In = element.Attribute("in")?.Value,
            Dx = GetFloat(element, "dx"),
            Dy = GetFloat(element, "dy")
        };
    }

    private SvgFePointLight ParseFePointLight(XElement element)
    {
        return new SvgFePointLight
        {
            X = GetFloat(element, "x"),
            Y = GetFloat(element, "y"),
            Z = GetFloat(element, "z")
        };
    }

    private SvgFeSpecularLighting ParseFeSpecularLighting(XElement element)
    {
        return new SvgFeSpecularLighting
        {
            In = element.Attribute("in")?.Value,
            SurfaceScale = GetFloat(element, "surfaceScale"),
            SpecularConstant = GetFloat(element, "specularConstant"),
            SpecularExponent = GetFloat(element, "specularExponent"),
            KernelUnitLength = element.Attribute("kernelUnitLength")?.Value
        };
    }

    private SvgFeSpotLight ParseFeSpotLight(XElement element)
    {
        return new SvgFeSpotLight
        {
            X = GetFloat(element, "x"),
            Y = GetFloat(element, "y"),
            Z = GetFloat(element, "z"),
            PointsAtX = GetFloat(element, "pointsAtX"),
            PointsAtY = GetFloat(element, "pointsAtY"),
            PointsAtZ = GetFloat(element, "pointsAtZ"),
            SpecularExponent = GetFloat(element, "specularExponent"),
            LimitingConeAngle = GetFloat(element, "limitingConeAngle")
        };
    }

    private SvgFeTile ParseFeTile(XElement element)
    {
        return new SvgFeTile
        {
            In = element.Attribute("in")?.Value
        };
    }

    private SvgFeTurbulence ParseFeTurbulence(XElement element)
    {
        return new SvgFeTurbulence
        {
            Type = element.Attribute("type")?.Value,
            BaseFrequency = element.Attribute("baseFrequency")?.Value,
            NumOctaves = GetInt(element, "numOctaves"),
            Seed = GetInt(element, "seed")
        };
    }

    private SvgFeDropShadow ParseFeDropShadow(XElement element)
    {
        return new SvgFeDropShadow
        {
            In = element.Attribute("in")?.Value,
            Dx = GetFloat(element, "dx"),
            Dy = GetFloat(element, "dy"),
            StdDeviationX = GetFloat(element, "stdDeviation"),
            StdDeviationY = GetFloat(element, "stdDeviation"),
            FloodColor = element.Attribute("flood-color")?.Value,
            FloodOpacity = GetFloat(element, "flood-opacity", 1.0f)
        };
    }

    private void ParseChildrenToFilter(XElement parent, SvgFilter filter, XNamespace ns)
    {
        if (++_currentDepth > _options.MaxNestingDepth)
        {
            throw new InvalidOperationException($"Maximum nesting depth of {_options.MaxNestingDepth} exceeded");
        }

        foreach (var child in parent.Elements())
        {
            if (++_elementCount > _options.MaxElements)
            {
                throw new InvalidOperationException($"Maximum element count of {_options.MaxElements} exceeded");
            }

            string tagName = child.Name.LocalName.ToLower();

            SvgElement? element = tagName switch
            {
                "feblend" => ParseFeBlend(child),
                "fecolormatrix" => ParseFeColorMatrix(child),
                "fecomponenttransfer" => ParseFeComponentTransfer(child),
                "fecomposite" => ParseFeComposite(child),
                "feconvolvematrix" => ParseFeConvolveMatrix(child),
                "fediffuselighting" => ParseFeDiffuseLighting(child),
                "fedisplacementmap" => ParseFeDisplacementMap(child),
                "fedistantlight" => ParseFeDistantLight(child),
                "feflood" => ParseFeFlood(child),
                "fegaussianblur" => ParseFeGaussianBlur(child),
                "feimage" => ParseFeImage(child),
                "femerge" => ParseFeMerge(child),
                "femorphology" => ParseFeMorphology(child),
                "feoffset" => ParseFeOffset(child),
                "fepointlight" => ParseFePointLight(child),
                "fespecularlighting" => ParseFeSpecularLighting(child),
                "fespotlight" => ParseFeSpotLight(child),
                "fetile" => ParseFeTile(child),
                "feturbulence" => ParseFeTurbulence(child),
                "fedropshadow" => ParseFeDropShadow(child),
                _ => null
            };

            if (element != null)
            {
                filter.FilterPrimitives ??= new List<SvgElement>();
                filter.FilterPrimitives.Add(element);
            }
        }

        _currentDepth--;
    }

    private SvgText ParseText(XElement element, XNamespace ns)
    {
        var text = new SvgText
        {
            X = GetFloat(element, "x"),
            Y = GetFloat(element, "y"),
            Dx = GetFloat(element, "dx"),
            Dy = GetFloat(element, "dy"),
            Rotate = element.Attribute("rotate")?.Value,
            LengthAdjust = element.Attribute("lengthAdjust")?.Value,
            TextLength = GetFloat(element, "textLength"),
            Opacity = GetFloat(element, "opacity", 1.0f),
            Display = element.Attribute("display")?.Value ?? string.Empty,
            Visibility = element.Attribute("visibility")?.Value ?? string.Empty,
            ClipPath = element.Attribute("clip-path")?.Value ?? string.Empty,
            Mask = element.Attribute("mask")?.Value ?? string.Empty,
            Text = element.Value,
            FontFamily = element.Attribute("font-family")?.Value,
            FontSize = GetFloat(element, "font-size", 16f),
            FontWeight = element.Attribute("font-weight")?.Value,
            FontStyle = element.Attribute("font-style")?.Value,
            TextAnchor = element.Attribute("text-anchor")?.Value,
            DominantBaseline = element.Attribute("dominant-baseline")?.Value,
            LetterSpacing = GetFloat(element, "letter-spacing"),
            WordSpacing = GetFloat(element, "word-spacing"),
            WritingMode = element.Attribute("writing-mode")?.Value,
            TextDecoration = element.Attribute("text-decoration")?.Value,
            Class = element.Attribute("class")?.Value ?? string.Empty,
            Style = element.Attribute("style")?.Value ?? string.Empty
        };

        foreach (var child in element.Elements())
        {
            string tagName = child.Name.LocalName.ToLower();

            SvgElement? childElement = tagName switch
            {
                "tspan" => ParseTSpan(child, ns),
                "textpath" => ParseTextPath(child),
                _ => null
            };

            if (childElement != null)
            {
                text.Children ??= new List<SvgElement>();
                text.Children.Add(childElement);
            }
        }

        return text;
    }

    private SvgTSpan ParseTSpan(XElement element, XNamespace ns)
    {
        var tspan = new SvgTSpan
        {
            X = GetFloat(element, "x"),
            Y = GetFloat(element, "y"),
            Dx = GetFloat(element, "dx"),
            Dy = GetFloat(element, "dy"),
            Rotate = element.Attribute("rotate")?.Value,
            LengthAdjust = element.Attribute("lengthAdjust")?.Value,
            TextLength = GetFloat(element, "textLength"),
            Text = element.Value,
            FontFamily = element.Attribute("font-family")?.Value,
            FontSize = GetFloat(element, "font-size", 16f),
            FontWeight = element.Attribute("font-weight")?.Value,
            FontStyle = element.Attribute("font-style")?.Value,
            TextAnchor = element.Attribute("text-anchor")?.Value,
            DominantBaseline = element.Attribute("dominant-baseline")?.Value,
            LetterSpacing = GetFloat(element, "letter-spacing"),
            WordSpacing = GetFloat(element, "word-spacing"),
            TextDecoration = element.Attribute("text-decoration")?.Value,
            Class = element.Attribute("class")?.Value ?? string.Empty,
            Style = element.Attribute("style")?.Value ?? string.Empty
        };

        foreach (var child in element.Elements())
        {
            string tagName = child.Name.LocalName.ToLower();

            SvgElement? childElement = tagName switch
            {
                "tspan" => ParseTSpan(child, ns),
                "textpath" => ParseTextPath(child),
                _ => null
            };

            if (childElement != null)
            {
                tspan.Children ??= new List<SvgElement>();
                tspan.Children.Add(childElement);
            }
        }

        return tspan;
    }

    private SvgTextPath ParseTextPath(XElement element)
    {
        return new SvgTextPath
        {
            Href = element.Attribute("href")?.Value ?? element.Attribute(xlinkNamespace + "href")?.Value,
            StartOffset = GetFloat(element, "startOffset"),
            Text = element.Value,
            Method = element.Attribute("method")?.Value,
            Spacing = element.Attribute("spacing")?.Value,
            Side = element.Attribute("side")?.Value,
            LengthAdjust = element.Attribute("lengthAdjust")?.Value,
            TextLength = GetFloat(element, "textLength"),
            FillStyle = ParseStyle(element, null, true),
            StrokeStyle = ParseStyle(element, null, false),
            FontFamily = element.Attribute("font-family")?.Value,
            FontSize = GetFloat(element, "font-size", 16f),
            Class = element.Attribute("class")?.Value ?? string.Empty,
            Style = element.Attribute("style")?.Value ?? string.Empty
        };
    }

    private SvgSwitch ParseSwitch(XElement element, XNamespace ns)
    {
        var sw = new SvgSwitch
        {
            Opacity = GetFloat(element, "opacity", 1.0f),
            Display = element.Attribute("display")?.Value ?? string.Empty,
            Visibility = element.Attribute("visibility")?.Value ?? string.Empty,
            RequiredFeatures = element.Attribute("requiredFeatures")?.Value,
            RequiredExtensions = element.Attribute("requiredExtensions")?.Value,
            SystemLanguage = element.Attribute("systemLanguage")?.Value,
            Class = element.Attribute("class")?.Value ?? string.Empty,
            Style = element.Attribute("style")?.Value ?? string.Empty
        };

        foreach (var child in element.Elements())
        {
            string tagName = child.Name.LocalName.ToLower();

            SvgElement? childElement = tagName switch
            {
                "g" => ParseGroup(child, ns),
                "a" => ParseAnchor(child, ns),
                "rect" => ParseRect(child),
                "circle" => ParseCircle(child),
                "ellipse" => ParseEllipse(child),
                "line" => ParseLine(child),
                "polyline" => ParsePolyline(child),
                "polygon" => ParsePolygon(child),
                "path" => ParsePath(child),
                "image" => ParseImage(child),
                "text" => ParseText(child, ns),
                "tspan" => ParseTSpan(child, ns),
                "use" => ParseUse(child),
                "foreignobject" => ParseForeignObject(child),
                _ => null
            };

            if (childElement != null)
            {
                sw.Children ??= new List<SvgElement>();
                sw.Children.Add(childElement);
            }
        }

        return sw;
    }

    private SvgForeignObject ParseForeignObject(XElement element)
    {
        return new SvgForeignObject
        {
            X = GetFloat(element, "x"),
            Y = GetFloat(element, "y"),
            Width = ParseDimension(element, "width", 0),
            Height = ParseDimension(element, "height", 0),
            Opacity = GetFloat(element, "opacity", 1.0f),
            Display = element.Attribute("display")?.Value ?? string.Empty,
            Visibility = element.Attribute("visibility")?.Value ?? string.Empty,
            RequiredFeatures = element.Attribute("requiredFeatures")?.Value,
            RequiredExtensions = element.Attribute("requiredExtensions")?.Value,
            SystemLanguage = element.Attribute("systemLanguage")?.Value,
            Class = element.Attribute("class")?.Value ?? string.Empty,
            Style = element.Attribute("style")?.Value ?? string.Empty,
            Content = element.ToString()
        };
    }

    private SKMatrix ParseTransform(string? transform)
    {
        if (string.IsNullOrEmpty(transform)) return SKMatrix.Identity;

        SKMatrix matrix = SKMatrix.Identity;
        var args = new float[6];
        
        for (int i = 0; i < transform.Length;)
        {
            while (i < transform.Length && char.IsWhiteSpace(transform[i])) i++;
            if (i >= transform.Length) break;

            int typeStart = i;
            while (i < transform.Length && char.IsLetter(transform[i])) i++;
            string type = transform.Substring(typeStart, i - typeStart).ToLowerInvariant();

            while (i < transform.Length && transform[i] != '(') i++;
            if (i >= transform.Length) break;
            i++;

            int argCount = 0;
            while (i < transform.Length && transform[i] != ')')
            {
                while (i < transform.Length && (char.IsWhiteSpace(transform[i]) || transform[i] == ',')) i++;
                if (i >= transform.Length || transform[i] == ')') break;

                int numStart = i;
                while (i < transform.Length && (char.IsDigit(transform[i]) || transform[i] == '.' || transform[i] == '-' || transform[i] == '+' || transform[i] == 'e' || transform[i] == 'E')) i++;
                
                if (argCount < 6 && float.TryParse(transform.AsSpan(numStart, i - numStart), NumberStyles.Float, CultureInfo.InvariantCulture, out float num))
                {
                    args[argCount++] = num;
                }
            }

            while (i < transform.Length && transform[i] != ')') i++;
            if (i < transform.Length) i++;

            SKMatrix transformMatrix = type switch
            {
                "matrix" when argCount >= 6 => new SKMatrix(args[0], args[2], args[4], args[1], args[3], args[5], 0, 0, 1),
                "translate" when argCount >= 1 => SKMatrix.CreateTranslation(args[0], argCount > 1 ? args[1] : 0),
                "scale" when argCount >= 1 => SKMatrix.CreateScale(args[0], argCount > 1 ? args[1] : args[0]),
                "rotate" when argCount >= 1 => argCount == 3
                    ? SKMatrix.CreateTranslation(args[1], args[2])
                        .PostConcat(SKMatrix.CreateRotationDegrees(args[0]))
                        .PostConcat(SKMatrix.CreateTranslation(-args[1], -args[2]))
                    : SKMatrix.CreateRotationDegrees(args[0]),
                _ => SKMatrix.Identity
            };

            matrix = matrix.PostConcat(transformMatrix);
        }

        return matrix;
    }

    private SvgStyle? ParseStyle(XElement element, string? classAttr, bool isFill)
    {
        string? val = element.Attribute(isFill ? "fill" : "stroke")?.Value;

        if (string.IsNullOrEmpty(val))
        {
            var style = element.Attribute("style")?.Value;
            if (!string.IsNullOrEmpty(style))
            {
                var match = isFill ? FillStyleRegex.Match(style) : StrokeStyleRegex.Match(style);
                if (match.Success) val = match.Groups[1].Value.Trim();
            }
        }

        if (string.IsNullOrEmpty(val))
        {
            if (!string.IsNullOrEmpty(classAttr))
            {
                var classes = classAttr.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var className in classes)
                {
                    if (_cssClasses.TryGetValue(className, out var classStyles))
                    {
                        var style = isFill ? classStyles.Fill : classStyles.Stroke;
                        if (style != null) return style;
                    }
                }
            }
            return null;
        }

        if (val.Equals("none", StringComparison.OrdinalIgnoreCase))
            return null;

        if (!SvgColorHelper.TryParse(val, out SKColor color))
            return null;

        var styleObj = new SvgStyle
        {
            Color = color
        };

        if (!isFill)
        {
            string? wVal = element.Attribute("stroke-width")?.Value;
            if (!string.IsNullOrEmpty(wVal) && float.TryParse(wVal.Replace("px", ""), NumberStyles.Any, CultureInfo.InvariantCulture, out float width))
            {
                styleObj.StrokeWidth = width;
            }
        }

        return styleObj;
    }

    private List<SKPoint>? ParsePoints(string? pointsStr)
    {
        if (string.IsNullOrEmpty(pointsStr))
            return null;

        var points = new List<SKPoint>();
        int i = 0;
        
        while (i < pointsStr.Length)
        {
            while (i < pointsStr.Length && (pointsStr[i] == ' ' || pointsStr[i] == ',')) i++;
            if (i >= pointsStr.Length) break;

            int xStart = i;
            while (i < pointsStr.Length && (char.IsDigit(pointsStr[i]) || pointsStr[i] == '.' || pointsStr[i] == '-' || pointsStr[i] == '+' || pointsStr[i] == 'e' || pointsStr[i] == 'E')) i++;
            
            if (float.TryParse(pointsStr.AsSpan(xStart, i - xStart), NumberStyles.Float, CultureInfo.InvariantCulture, out float x))
            {
                while (i < pointsStr.Length && (pointsStr[i] == ' ' || pointsStr[i] == ',')) i++;
                if (i >= pointsStr.Length) break;

                int yStart = i;
                while (i < pointsStr.Length && (char.IsDigit(pointsStr[i]) || pointsStr[i] == '.' || pointsStr[i] == '-' || pointsStr[i] == '+' || pointsStr[i] == 'e' || pointsStr[i] == 'E')) i++;
                
                if (float.TryParse(pointsStr.AsSpan(yStart, i - yStart), NumberStyles.Float, CultureInfo.InvariantCulture, out float y))
                {
                    points.Add(new SKPoint(x, y));
                }
            }
        }

        return points.Count > 0 ? points : null;
    }

    private SKRect ParseViewBox(XElement svg)
    {
        var vb = svg.Attribute("viewBox");
        if (vb != null)
        {
            var values = new float[4];
            int valueIndex = 0;
            int i = 0;
            string vbValue = vb.Value;
            
            while (i < vbValue.Length && valueIndex < 4)
            {
                while (i < vbValue.Length && (vbValue[i] == ' ' || vbValue[i] == ',')) i++;
                if (i >= vbValue.Length) break;

                int start = i;
                while (i < vbValue.Length && (char.IsDigit(vbValue[i]) || vbValue[i] == '.' || vbValue[i] == '-' || vbValue[i] == '+' || vbValue[i] == 'e' || vbValue[i] == 'E')) i++;
                
                if (float.TryParse(vbValue.AsSpan(start, i - start), NumberStyles.Float, CultureInfo.InvariantCulture, out float num))
                {
                    values[valueIndex++] = num;
                }
            }

            if (valueIndex == 4)
                return SKRect.Create(values[0], values[1], values[2], values[3]);
        }

        float w = ParseDimension(svg, "width", 512);
        float h = ParseDimension(svg, "height", 512);
        return SKRect.Create(0, 0, w, h);
    }

    private float ParseDimension(XElement element, string attrName, float defaultValue)
    {
        string? val = element.Attribute(attrName)?.Value;
        if (string.IsNullOrEmpty(val)) return defaultValue;
        
        float.TryParse(val.Replace("px", ""), NumberStyles.Any, CultureInfo.InvariantCulture, out float result);
        return result > 0 ? result : defaultValue;
    }

    private float GetFloat(XElement element, string attrName)
    {
        string? val = element.Attribute(attrName)?.Value;
        if (string.IsNullOrEmpty(val)) return 0;
        
        float.TryParse(val.Replace("px", ""), NumberStyles.Any, CultureInfo.InvariantCulture, out float result);
        return result;
    }

    private SvgAnchor ParseAnchor(XElement element, XNamespace ns)
    {
        var anchor = new SvgAnchor
        {
            Href = element.Attribute("href")?.Value ?? element.Attribute(xlinkNamespace + "href")?.Value,
            Target = element.Attribute("target")?.Value,
            Download = element.Attribute("download")?.Value,
            Ping = element.Attribute("ping")?.Value,
            Rel = element.Attribute("rel")?.Value,
            Hreflang = element.Attribute("hreflang")?.Value,
            Type = element.Attribute("type")?.Value,
            Opacity = GetFloat(element, "opacity", 1.0f),
            Display = element.Attribute("display")?.Value ?? string.Empty,
            Visibility = element.Attribute("visibility")?.Value ?? string.Empty,
            Class = element.Attribute("class")?.Value ?? string.Empty,
            Style = element.Attribute("style")?.Value ?? string.Empty
        };

        foreach (var child in element.Elements())
        {
            string tagName = child.Name.LocalName.ToLower();

            SvgElement? childElement = tagName switch
            {
                "g" => ParseGroup(child, ns),
                "rect" => ParseRect(child),
                "circle" => ParseCircle(child),
                "ellipse" => ParseEllipse(child),
                "line" => ParseLine(child),
                "polyline" => ParsePolyline(child),
                "polygon" => ParsePolygon(child),
                "path" => ParsePath(child),
                "image" => ParseImage(child),
                "text" => ParseText(child, ns),
                "tspan" => ParseTSpan(child, ns),
                "use" => ParseUse(child),
                "foreignobject" => ParseForeignObject(child),
                _ => null
            };

            if (childElement != null)
            {
                anchor.Children ??= new List<SvgElement>();
                anchor.Children.Add(childElement);
            }
        }

        return anchor;
    }

    private SvgMarker ParseMarker(XElement element, XNamespace ns)
    {
        var marker = new SvgMarker
        {
            ViewBox = ParseViewBoxAttribute(element),
            PreserveAspectRatio = element.Attribute("preserveAspectRatio")?.Value,
            RefX = GetFloat(element, "refX"),
            RefY = GetFloat(element, "refY"),
            MarkerWidth = GetFloat(element, "markerWidth"),
            MarkerHeight = GetFloat(element, "markerHeight"),
            MarkerUnits = element.Attribute("markerUnits")?.Value,
            Orient = GetFloat(element, "orient"),
            OrientType = element.Attribute("orient")?.Value,
            Opacity = GetFloat(element, "opacity", 1.0f),
            Overflow = element.Attribute("overflow")?.Value,
            Clip = element.Attribute("clip")?.Value,
            Class = element.Attribute("class")?.Value ?? string.Empty,
            Style = element.Attribute("style")?.Value ?? string.Empty
        };

        foreach (var child in element.Elements())
        {
            string tagName = child.Name.LocalName.ToLower();

            SvgElement? childElement = tagName switch
            {
                "path" => ParsePath(child),
                "rect" => ParseRect(child),
                "circle" => ParseCircle(child),
                "ellipse" => ParseEllipse(child),
                "line" => ParseLine(child),
                "polyline" => ParsePolyline(child),
                "polygon" => ParsePolygon(child),
                _ => null
            };

            if (childElement != null)
            {
                marker.Children ??= new List<SvgElement>();
                marker.Children.Add(childElement);
            }
        }

        return marker;
    }

    private SvgClipPath ParseClipPath(XElement element, XNamespace ns)
    {
        var clipPath = new SvgClipPath
        {
            ClipPathUnits = element.Attribute("clipPathUnits")?.Value,
            Class = element.Attribute("class")?.Value ?? string.Empty,
            Style = element.Attribute("style")?.Value ?? string.Empty,
            XmlSpace = element.Attribute(XNamespace.Xml + "space")?.Value ?? string.Empty,
            XmlLang = element.Attribute(XNamespace.Xml + "lang")?.Value ?? string.Empty,
            XmlBase = element.Attribute(XNamespace.Xml + "base")?.Value ?? string.Empty
        };

        foreach (var child in element.Elements())
        {
            string tagName = child.Name.LocalName.ToLower();

            SvgElement? childElement = tagName switch
            {
                "path" => ParsePath(child),
                "rect" => ParseRect(child),
                "circle" => ParseCircle(child),
                "ellipse" => ParseEllipse(child),
                "line" => ParseLine(child),
                "polyline" => ParsePolyline(child),
                "polygon" => ParsePolygon(child),
                "text" => ParseText(child, ns),
                "tspan" => ParseTSpan(child, ns),
                "use" => ParseUse(child),
                "g" => ParseGroup(child, ns),
                _ => null
            };

            if (childElement != null)
            {
                clipPath.Children ??= new List<SvgElement>();
                clipPath.Children.Add(childElement);
            }
        }

        return clipPath;
    }

    private SvgMask ParseMask(XElement element, XNamespace ns)
    {
        var mask = new SvgMask
        {
            X = GetFloat(element, "x"),
            Y = GetFloat(element, "y"),
            Width = ParseDimension(element, "width", 0),
            Height = ParseDimension(element, "height", 0),
            MaskUnits = element.Attribute("maskUnits")?.Value,
            MaskContentUnits = element.Attribute("maskContentUnits")?.Value,
            Class = element.Attribute("class")?.Value ?? string.Empty,
            Style = element.Attribute("style")?.Value ?? string.Empty,
            XmlSpace = element.Attribute(XNamespace.Xml + "space")?.Value ?? string.Empty,
            XmlLang = element.Attribute(XNamespace.Xml + "lang")?.Value ?? string.Empty,
            XmlBase = element.Attribute(XNamespace.Xml + "base")?.Value ?? string.Empty
        };

        foreach (var child in element.Elements())
        {
            string tagName = child.Name.LocalName.ToLower();

            SvgElement? childElement = tagName switch
            {
                "path" => ParsePath(child),
                "rect" => ParseRect(child),
                "circle" => ParseCircle(child),
                "ellipse" => ParseEllipse(child),
                "line" => ParseLine(child),
                "polyline" => ParsePolyline(child),
                "polygon" => ParsePolygon(child),
                "text" => ParseText(child, ns),
                "tspan" => ParseTSpan(child, ns),
                "use" => ParseUse(child),
                "g" => ParseGroup(child, ns),
                "image" => ParseImage(child),
                _ => null
            };

            if (childElement != null)
            {
                mask.Children ??= new List<SvgElement>();
                mask.Children.Add(childElement);
            }
        }

        return mask;
    }

    private SvgStyleElement ParseStyleElement(XElement element)
    {
        return new SvgStyleElement
        {
            Type = element.Attribute("type")?.Value,
            Media = element.Attribute("media")?.Value,
            Title = element.Attribute("title")?.Value,
            Class = element.Attribute("class")?.Value ?? string.Empty,
            Style = element.Attribute("style")?.Value ?? string.Empty,
            XmlSpace = element.Attribute(XNamespace.Xml + "space")?.Value ?? string.Empty,
            XmlLang = element.Attribute(XNamespace.Xml + "lang")?.Value ?? string.Empty,
            XmlBase = element.Attribute(XNamespace.Xml + "base")?.Value ?? string.Empty,
            Content = element.Value
        };
    }
}
