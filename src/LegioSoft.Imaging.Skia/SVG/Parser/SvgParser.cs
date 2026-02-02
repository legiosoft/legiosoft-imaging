using System.Globalization;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using LegioSoft.Imaging.Skia.SVG.Helpers;
using LegioSoft.Imaging.Skia.SVG.Models;
using SkiaSharp;

namespace LegioSoft.Imaging.Skia.SVG.Parser;

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
            Height = ParseDimension(root, "height", 512)
        };

        var group = new SvgGroup();
        ParseChildren(root, group, root.Name.Namespace);
        document.RootElement = group;

        return document;
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

            if (tagName == "defs") continue;

            SvgElement? element = tagName switch
            {
                "g" => ParseGroup(child, ns),
                "rect" => ParseRect(child),
                "circle" => ParseCircle(child),
                "ellipse" => ParseEllipse(child),
                "line" => ParseLine(child),
                "polyline" => ParsePolyline(child),
                "polygon" => ParsePolygon(child),
                "path" => ParsePath(child),
                "animatemotion" => ParseAnimateMotion(child),
                "animatetransform" => ParseAnimateTransform(child),
                "mpath" => ParseMPath(child),
                "set" => ParseSet(child),
                "view" => ParseView(child),
                "script" => ParseScript(child),
                "metadata" => ParseMetadata(child),
                "cursor" => ParseCursor(child),
                "solidcolor" => ParseSolidColor(child),
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
        var group = new SvgGroup();
        ParseChildren(element, group, ns);
        return group;
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
            Ry = GetFloat(element, "ry")
        };
    }

    private SvgCircle ParseCircle(XElement element)
    {
        return new SvgCircle
        {
            Cx = GetFloat(element, "cx"),
            Cy = GetFloat(element, "cy"),
            Radius = GetFloat(element, "r")
        };
    }

    private SvgEllipse ParseEllipse(XElement element)
    {
        return new SvgEllipse
        {
            Cx = GetFloat(element, "cx"),
            Cy = GetFloat(element, "cy"),
            Rx = GetFloat(element, "rx"),
            Ry = GetFloat(element, "ry")
        };
    }

    private SvgLine ParseLine(XElement element)
    {
        return new SvgLine
        {
            X1 = GetFloat(element, "x1"),
            Y1 = GetFloat(element, "y1"),
            X2 = GetFloat(element, "x2"),
            Y2 = GetFloat(element, "y2")
        };
    }

    private SvgPolyline ParsePolyline(XElement element)
    {
        return new SvgPolyline
        {
            Points = ParsePoints(element.Attribute("points")?.Value)
        };
    }

    private SvgPolygon ParsePolygon(XElement element)
    {
        return new SvgPolygon
        {
            Points = ParsePoints(element.Attribute("points")?.Value)
        };
    }

    private SvgPath ParsePath(XElement element)
    {
        var pathData = element.Attribute("d")?.Value ?? string.Empty;
        var path = SKPath.ParseSvgPathData(pathData);
        
        return new SvgPath
        {
            Path = path
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
            Opacity = GetFloat(element, "solid-opacity")
        };
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
}
