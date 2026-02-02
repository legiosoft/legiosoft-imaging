using LegioSoft.Imaging.Skia.SVG.Interfaces;
using LegioSoft.Imaging.Skia.SVG.Models;
using SkiaSharp;

namespace LegioSoft.Imaging.Skia.SVG.Converter;

/// <summary>
/// Converts SVG documents to SKBitmap using the Visitor pattern.
/// </summary>
/// <remarks>
/// <b>Thread Safety Warning:</b> This class is stateful and not thread-safe.
/// Do not share instances across threads. Always create a new SvgToSkiaConverter for each conversion:
/// <code>
/// var converter = new SvgToSkiaConverter();
/// var bitmap = converter.Convert(document, width, height);
/// </code>
/// 
/// State maintained during conversion:
/// - <see cref="_fillStack"/> and <see cref="_strokeStack"/> for cascading style inheritance
/// - <see cref="_sharedPaint"/> for efficient drawing operations
/// </remarks>
public class SvgToSkiaConverter : ISvgElementVisitor
{
    private readonly Stack<SvgStyle> _fillStack = new Stack<SvgStyle>();
    private readonly Stack<SvgStyle> _strokeStack = new Stack<SvgStyle>();
    private readonly SKPaint _sharedPaint = new SKPaint { IsAntialias = true };
    private SvgStyle? _defaultFillStyle;

    public SKBitmap Convert(SvgDocument document, int width, int height)
    {
        if (document.RootElement == null)
            throw new InvalidOperationException("SVG document has no root element");
        if (document.ViewBox.Width <= 0 || document.ViewBox.Height <= 0)
            throw new InvalidOperationException($"Invalid ViewBox: {document.ViewBox}");

        var bitmap = new SKBitmap(width, height);
        using var canvas = new SKCanvas(bitmap);

        canvas.Clear(SKColors.Transparent);

        float scaleX = (float)width / document.ViewBox.Width;
        float scaleY = (float)height / document.ViewBox.Height;
        SKMatrix initialMatrix = SKMatrix.CreateScale(scaleX, scaleY);
        initialMatrix = initialMatrix.PostConcat(SKMatrix.CreateTranslation(
            -document.ViewBox.Left * scaleX,
            -document.ViewBox.Top * scaleY
        ));

        DebugPrint(document.RootElement);

        document.RootElement?.Accept(this, canvas, initialMatrix);

        return bitmap;

        void DebugPrint(SvgElement? element, int depth = 0)
        {
            if (element == null) return;

            string indent = new string(' ', depth * 2);
            System.Diagnostics.Debug.WriteLine($"{indent}{element.GetType().Name}: fill={element.FillStyle?.Color.ToString() ?? "null"}, stroke={element.StrokeStyle?.Color.ToString() ?? "null"}");

            if (element is SvgGroup group)
            {
                foreach (var child in group.Children ?? Enumerable.Empty<SvgElement>())
                {
                    DebugPrint(child, depth + 1);
                }
            }
        }
    }

    private SvgStyle? GetEffectiveFillStyle(SvgStyle? elementFillStyle)
    {
        return elementFillStyle ?? (_fillStack.Count > 0 ? _fillStack.Peek() : null);
    }

    private SvgStyle GetDefaultFillStyle()
    {
        return _defaultFillStyle ??= new SvgStyle { Color = SKColors.Black };
    }

    private SvgStyle? GetEffectiveStrokeStyle(SvgStyle? elementStrokeStyle)
    {
        return elementStrokeStyle ?? (_strokeStack.Count > 0 ? _strokeStack.Peek() : null);
    }

    private void ConfigurePaint(SvgStyle style, bool isFill)
    {
        _sharedPaint.Color = style.Color;
        _sharedPaint.Style = isFill ? SKPaintStyle.Fill : SKPaintStyle.Stroke;
        _sharedPaint.StrokeWidth = isFill ? 1.0f : style.StrokeWidth;
    }

    public void Visit(SvgRect element, SKCanvas canvas, SKMatrix transform)
    {
        SKRect rect = SKRect.Create(element.X, element.Y, element.Width, element.Height);

        if (element.Rx > 0 || element.Ry > 0)
        {
            using var path = new SKPath();
            path.AddRoundRect(rect, element.Rx, element.Ry);
            RenderPath(path, element, canvas, transform, shouldDisposePath: true);
        }
        else
        {
            int saveCount = canvas.Save();
            canvas.SetMatrix(transform);

            var effectiveFillStyle = GetEffectiveFillStyle(element.FillStyle);
            if (effectiveFillStyle != null)
            {
                ConfigurePaint(effectiveFillStyle, true);
                canvas.DrawRect(rect, _sharedPaint);
            }
            else
            {
                ConfigurePaint(GetDefaultFillStyle(), true);
                canvas.DrawRect(rect, _sharedPaint);
            }

            var effectiveStrokeStyle = GetEffectiveStrokeStyle(element.StrokeStyle);
            if (effectiveStrokeStyle != null)
            {
                ConfigurePaint(effectiveStrokeStyle, false);
                canvas.DrawRect(rect, _sharedPaint);
            }

            canvas.RestoreToCount(saveCount);
        }
    }

    public void Visit(SvgCircle element, SKCanvas canvas, SKMatrix transform)
    {
        int saveCount = canvas.Save();
        canvas.SetMatrix(transform);

        var effectiveFillStyle = GetEffectiveFillStyle(element.FillStyle);
        if (effectiveFillStyle != null)
        {
            ConfigurePaint(effectiveFillStyle, true);
            canvas.DrawCircle(element.Cx, element.Cy, element.Radius, _sharedPaint);
        }
        else
        {
            ConfigurePaint(GetDefaultFillStyle(), true);
            canvas.DrawCircle(element.Cx, element.Cy, element.Radius, _sharedPaint);
        }

        var effectiveStrokeStyle = GetEffectiveStrokeStyle(element.StrokeStyle);
        if (effectiveStrokeStyle != null)
        {
            ConfigurePaint(effectiveStrokeStyle, false);
            canvas.DrawCircle(element.Cx, element.Cy, element.Radius, _sharedPaint);
        }

        canvas.RestoreToCount(saveCount);
    }

    public void Visit(SvgEllipse element, SKCanvas canvas, SKMatrix transform)
    {
        SKRect rect = new SKRect(
            element.Cx - element.Rx,
            element.Cy - element.Ry,
            element.Cx + element.Rx,
            element.Cy + element.Ry
        );

        int saveCount = canvas.Save();
        canvas.SetMatrix(transform);

        var effectiveFillStyle = GetEffectiveFillStyle(element.FillStyle);
        if (effectiveFillStyle != null)
        {
            ConfigurePaint(effectiveFillStyle, true);
            canvas.DrawOval(rect, _sharedPaint);
        }
        else
        {
            ConfigurePaint(GetDefaultFillStyle(), true);
            canvas.DrawOval(rect, _sharedPaint);
        }

        var effectiveStrokeStyle = GetEffectiveStrokeStyle(element.StrokeStyle);
        if (effectiveStrokeStyle != null)
        {
            ConfigurePaint(effectiveStrokeStyle, false);
            canvas.DrawOval(rect, _sharedPaint);
        }

        canvas.RestoreToCount(saveCount);
    }

    public void Visit(SvgLine element, SKCanvas canvas, SKMatrix transform)
    {
        int saveCount = canvas.Save();
        canvas.SetMatrix(transform);

        var effectiveStrokeStyle = GetEffectiveStrokeStyle(element.StrokeStyle);
        if (effectiveStrokeStyle != null)
        {
            ConfigurePaint(effectiveStrokeStyle, false);
            canvas.DrawLine(element.X1, element.Y1, element.X2, element.Y2, _sharedPaint);
        }

        canvas.RestoreToCount(saveCount);
    }

    public void Visit(SvgPolyline element, SKCanvas canvas, SKMatrix transform)
    {
        if (element.Points == null || element.Points.Count < 2) return;

        var path = new SKPath();
        path.MoveTo(element.Points[0]);
        for (int i = 1; i < element.Points.Count; i++)
        {
            path.LineTo(element.Points[i]);
        }
        RenderPath(path, element, canvas, transform, shouldDisposePath: true);
    }

    public void Visit(SvgPolygon element, SKCanvas canvas, SKMatrix transform)
    {
        if (element.Points == null || element.Points.Count < 2) return;

        var path = new SKPath();
        path.MoveTo(element.Points[0]);
        for (int i = 1; i < element.Points.Count; i++)
        {
            path.LineTo(element.Points[i]);
        }
        path.Close();
        RenderPath(path, element, canvas, transform, shouldDisposePath: true);
    }

    public void Visit(SvgPath element, SKCanvas canvas, SKMatrix transform)
    {
        if (element.Path == null) return;
        RenderPath(element.Path, element, canvas, transform, shouldDisposePath: false);
    }

    public void Visit(SvgGroup element, SKCanvas canvas, SKMatrix transform)
    {
        if (element.Children == null || element.Children.Count == 0) return;

        SKMatrix groupMatrix = element.Transform == null || element.Transform.Value.IsIdentity
            ? transform
            : transform.PostConcat(element.Transform.Value);

        if (element.FillStyle != null)
        {
            _fillStack.Push(element.FillStyle);
        }

        if (element.StrokeStyle != null)
        {
            _strokeStack.Push(element.StrokeStyle);
        }

        foreach (var child in element.Children)
        {
            child.Accept(this, canvas, groupMatrix);
        }

        if (element.StrokeStyle != null)
        {
            _strokeStack.Pop();
        }

        if (element.FillStyle != null)
        {
            _fillStack.Pop();
        }
    }

    public void Visit(SvgDefs element, SKCanvas canvas, SKMatrix transform)
    {
    }

    public void Visit(SvgDesc element, SKCanvas canvas, SKMatrix transform)
    {
    }

    public void Visit(SvgTitle element, SKCanvas canvas, SKMatrix transform)
    {
    }

    public void Visit(SvgSymbol element, SKCanvas canvas, SKMatrix transform)
    {
        if (element.Children == null || element.Children.Count == 0) return;

        SKMatrix symbolMatrix = element.Transform == null || element.Transform.Value.IsIdentity
            ? transform
            : transform.PostConcat(element.Transform.Value);

        if (element.FillStyle != null)
        {
            _fillStack.Push(element.FillStyle);
        }

        if (element.StrokeStyle != null)
        {
            _strokeStack.Push(element.StrokeStyle);
        }

        foreach (var child in element.Children)
        {
            child.Accept(this, canvas, symbolMatrix);
        }

        if (element.StrokeStyle != null)
        {
            _strokeStack.Pop();
        }

        if (element.FillStyle != null)
        {
            _fillStack.Pop();
        }
    }

    public void Visit(SvgUse element, SKCanvas canvas, SKMatrix transform)
    {
    }

    public void Visit(SvgImage element, SKCanvas canvas, SKMatrix transform)
    {
        if (element.Bitmap == null) return;

        SKRect dest = SKRect.Create(element.X, element.Y, element.Width, element.Height);

        int saveCount = canvas.Save();
        canvas.SetMatrix(transform);

        canvas.DrawBitmap(element.Bitmap, dest);

        canvas.RestoreToCount(saveCount);
    }

    public void Visit(SvgSwitch element, SKCanvas canvas, SKMatrix transform)
    {
        if (element.Children == null || element.Children.Count == 0) return;

        SKMatrix switchMatrix = element.Transform == null || element.Transform.Value.IsIdentity
            ? transform
            : transform.PostConcat(element.Transform.Value);

        if (element.FillStyle != null)
        {
            _fillStack.Push(element.FillStyle);
        }

        if (element.StrokeStyle != null)
        {
            _strokeStack.Push(element.StrokeStyle);
        }

        foreach (var child in element.Children)
        {
            child.Accept(this, canvas, switchMatrix);
            break;
        }

        if (element.StrokeStyle != null)
        {
            _strokeStack.Pop();
        }

        if (element.FillStyle != null)
        {
            _fillStack.Pop();
        }
    }

    public void Visit(SvgStyleElement element, SKCanvas canvas, SKMatrix transform)
    {
    }

    public void Visit(SvgForeignObject element, SKCanvas canvas, SKMatrix transform)
    {
    }

    public void Visit(SvgText element, SKCanvas canvas, SKMatrix transform)
    {
        if (string.IsNullOrEmpty(element.Text)) return;

        int saveCount = canvas.Save();
        canvas.SetMatrix(transform);

        var effectiveFillStyle = GetEffectiveFillStyle(element.FillStyle);
        var textStyle = effectiveFillStyle ?? GetDefaultFillStyle();

        ConfigurePaint(textStyle, true);

        SKTypeface typeface = SKTypeface.Default;
        if (!string.IsNullOrEmpty(element.FontFamily))
        {
            typeface = SKTypeface.FromFamilyName(element.FontFamily);
        }

        SKFontStyle fontStyle = SKFontStyle.Normal;
        if (!string.IsNullOrEmpty(element.FontWeight) && element.FontWeight.Equals("bold", StringComparison.OrdinalIgnoreCase))
        {
            fontStyle = SKFontStyle.Bold;
        }
        else if (!string.IsNullOrEmpty(element.FontStyle) && element.FontStyle.Equals("italic", StringComparison.OrdinalIgnoreCase))
        {
            fontStyle = SKFontStyle.Italic;
        }

        var font = new SKFont(typeface ?? SKTypeface.Default, element.FontSize)
        {
            Edging = SKFontEdging.Antialias
        };

        var paint = new SKPaint
        {
            Color = textStyle.Color,
            IsAntialias = true
        };

        SKPoint position = new SKPoint(element.X, element.Y);
        canvas.DrawText(element.Text, position.X, position.Y, font, paint);

        if (element.Children != null)
        {
            foreach (var child in element.Children)
            {
                child.Accept(this, canvas, transform);
            }
        }

        canvas.RestoreToCount(saveCount);

        typeface?.Dispose();
        font.Dispose();
        paint.Dispose();
    }

    public void Visit(SvgTSpan element, SKCanvas canvas, SKMatrix transform)
    {
        if (string.IsNullOrEmpty(element.Text)) return;

        int saveCount = canvas.Save();
        canvas.SetMatrix(transform);

        var effectiveFillStyle = GetEffectiveFillStyle(element.FillStyle);
        var textStyle = effectiveFillStyle ?? GetDefaultFillStyle();

        ConfigurePaint(textStyle, true);

        SKTypeface typeface = SKTypeface.Default;
        if (!string.IsNullOrEmpty(element.FontFamily))
        {
            typeface = SKTypeface.FromFamilyName(element.FontFamily);
        }

        SKFontStyle fontStyle = SKFontStyle.Normal;
        if (!string.IsNullOrEmpty(element.FontWeight) && element.FontWeight.Equals("bold", StringComparison.OrdinalIgnoreCase))
        {
            fontStyle = SKFontStyle.Bold;
        }
        else if (!string.IsNullOrEmpty(element.FontStyle) && element.FontStyle.Equals("italic", StringComparison.OrdinalIgnoreCase))
        {
            fontStyle = SKFontStyle.Italic;
        }

        var font = new SKFont(typeface ?? SKTypeface.Default, element.FontSize)
        {
            Edging = SKFontEdging.Antialias
        };

        var paint = new SKPaint
        {
            Color = textStyle.Color,
            IsAntialias = true
        };

        SKPoint position = new SKPoint(element.X + element.Dx, element.Y + element.Dy);
        canvas.DrawText(element.Text, position.X, position.Y, font, paint);

        if (element.Children != null)
        {
            foreach (var child in element.Children)
            {
                child.Accept(this, canvas, transform);
            }
        }

        canvas.RestoreToCount(saveCount);

        typeface?.Dispose();
        font.Dispose();
        paint.Dispose();
    }

    public void Visit(SvgTextPath element, SKCanvas canvas, SKMatrix transform)
    {
    }

    public void Visit(SvgAnchor element, SKCanvas canvas, SKMatrix transform)
    {
        if (element.Children == null || element.Children.Count == 0) return;

        SKMatrix anchorMatrix = element.Transform == null || element.Transform.Value.IsIdentity
            ? transform
            : transform.PostConcat(element.Transform.Value);

        if (element.FillStyle != null)
        {
            _fillStack.Push(element.FillStyle);
        }

        if (element.StrokeStyle != null)
        {
            _strokeStack.Push(element.StrokeStyle);
        }

        foreach (var child in element.Children)
        {
            child.Accept(this, canvas, anchorMatrix);
        }

        if (element.StrokeStyle != null)
        {
            _strokeStack.Pop();
        }

        if (element.FillStyle != null)
        {
            _fillStack.Pop();
        }
    }

    public void Visit(SvgAltGlyph element, SKCanvas canvas, SKMatrix transform)
    {
        if (element.Children == null || element.Children.Count == 0) return;

        SKMatrix altGlyphMatrix = element.Transform == null || element.Transform.Value.IsIdentity
            ? transform
            : transform.PostConcat(element.Transform.Value);

        if (element.FillStyle != null)
        {
            _fillStack.Push(element.FillStyle);
        }

        if (element.StrokeStyle != null)
        {
            _strokeStack.Push(element.StrokeStyle);
        }

        foreach (var child in element.Children)
        {
            child.Accept(this, canvas, altGlyphMatrix);
        }

        if (element.StrokeStyle != null)
        {
            _strokeStack.Pop();
        }

        if (element.FillStyle != null)
        {
            _fillStack.Pop();
        }
    }

    public void Visit(SvgAltGlyphDef element, SKCanvas canvas, SKMatrix transform)
    {
        if (element.Children == null || element.Children.Count == 0) return;

        SKMatrix altGlyphDefMatrix = element.Transform == null || element.Transform.Value.IsIdentity
            ? transform
            : transform.PostConcat(element.Transform.Value);

        if (element.FillStyle != null)
        {
            _fillStack.Push(element.FillStyle);
        }

        if (element.StrokeStyle != null)
        {
            _strokeStack.Push(element.StrokeStyle);
        }

        foreach (var child in element.Children)
        {
            child.Accept(this, canvas, altGlyphDefMatrix);
        }

        if (element.StrokeStyle != null)
        {
            _strokeStack.Pop();
        }

        if (element.FillStyle != null)
        {
            _fillStack.Pop();
        }
    }

    public void Visit(SvgAltGlyphItem element, SKCanvas canvas, SKMatrix transform)
    {
        if (element.GlyphRefs == null || element.GlyphRefs.Count == 0) return;

        SKMatrix altGlyphItemMatrix = element.Transform == null || element.Transform.Value.IsIdentity
            ? transform
            : transform.PostConcat(element.Transform.Value);

        if (element.FillStyle != null)
        {
            _fillStack.Push(element.FillStyle);
        }

        if (element.StrokeStyle != null)
        {
            _strokeStack.Push(element.StrokeStyle);
        }

        foreach (var glyphRef in element.GlyphRefs)
        {
            glyphRef.Accept(this, canvas, altGlyphItemMatrix);
        }

        if (element.StrokeStyle != null)
        {
            _strokeStack.Pop();
        }

        if (element.FillStyle != null)
        {
            _fillStack.Pop();
        }
    }

    public void Visit(SvgGlyphRef element, SKCanvas canvas, SKMatrix transform)
    {
    }

    public void Visit(SvgMarker element, SKCanvas canvas, SKMatrix transform)
    {
        if (element.Children == null || element.Children.Count == 0) return;

        SKMatrix markerMatrix = element.Transform == null || element.Transform.Value.IsIdentity
            ? transform
            : transform.PostConcat(element.Transform.Value);

        if (element.FillStyle != null)
        {
            _fillStack.Push(element.FillStyle);
        }

        if (element.StrokeStyle != null)
        {
            _strokeStack.Push(element.StrokeStyle);
        }

        foreach (var child in element.Children)
        {
            child.Accept(this, canvas, markerMatrix);
        }

        if (element.StrokeStyle != null)
        {
            _strokeStack.Pop();
        }

        if (element.FillStyle != null)
        {
            _fillStack.Pop();
        }
    }

    public void Visit(SvgMask element, SKCanvas canvas, SKMatrix transform)
    {
        if (element.Children == null || element.Children.Count == 0) return;

        SKMatrix maskMatrix = element.Transform == null || element.Transform.Value.IsIdentity
            ? transform
            : transform.PostConcat(element.Transform.Value);

        if (element.FillStyle != null)
        {
            _fillStack.Push(element.FillStyle);
        }

        if (element.StrokeStyle != null)
        {
            _strokeStack.Push(element.StrokeStyle);
        }

        foreach (var child in element.Children)
        {
            child.Accept(this, canvas, maskMatrix);
        }

        if (element.StrokeStyle != null)
        {
            _strokeStack.Pop();
        }

        if (element.FillStyle != null)
        {
            _fillStack.Pop();
        }
    }

    public void Visit(SvgClipPath element, SKCanvas canvas, SKMatrix transform)
    {
        if (element.Children == null || element.Children.Count == 0) return;

        SKMatrix clipPathMatrix = element.Transform == null || element.Transform.Value.IsIdentity
            ? transform
            : transform.PostConcat(element.Transform.Value);

        if (element.FillStyle != null)
        {
            _fillStack.Push(element.FillStyle);
        }

        if (element.StrokeStyle != null)
        {
            _strokeStack.Push(element.StrokeStyle);
        }

        foreach (var child in element.Children)
        {
            child.Accept(this, canvas, clipPathMatrix);
        }

        if (element.StrokeStyle != null)
        {
            _strokeStack.Pop();
        }

        if (element.FillStyle != null)
        {
            _fillStack.Pop();
        }
    }

    public void Visit(SvgLinearGradient element, SKCanvas canvas, SKMatrix transform)
    {
    }

    public void Visit(SvgRadialGradient element, SKCanvas canvas, SKMatrix transform)
    {
    }

    public void Visit(SvgPattern element, SKCanvas canvas, SKMatrix transform)
    {
    }

    public void Visit(SvgFilter element, SKCanvas canvas, SKMatrix transform)
    {
    }

    public void Visit(SvgFeBlend element, SKCanvas canvas, SKMatrix transform)
    {
    }

    public void Visit(SvgFeColorMatrix element, SKCanvas canvas, SKMatrix transform)
    {
    }

    public void Visit(SvgFeComponentTransfer element, SKCanvas canvas, SKMatrix transform)
    {
    }

    public void Visit(SvgFeFuncR element, SKCanvas canvas, SKMatrix transform)
    {
    }

    public void Visit(SvgFeFuncG element, SKCanvas canvas, SKMatrix transform)
    {
    }

    public void Visit(SvgFeFuncB element, SKCanvas canvas, SKMatrix transform)
    {
    }

    public void Visit(SvgFeFuncA element, SKCanvas canvas, SKMatrix transform)
    {
    }

    public void Visit(SvgFeComposite element, SKCanvas canvas, SKMatrix transform)
    {
    }

    public void Visit(SvgFeConvolveMatrix element, SKCanvas canvas, SKMatrix transform)
    {
    }

    public void Visit(SvgFeDiffuseLighting element, SKCanvas canvas, SKMatrix transform)
    {
    }

    public void Visit(SvgFeDisplacementMap element, SKCanvas canvas, SKMatrix transform)
    {
    }

    public void Visit(SvgFeDistantLight element, SKCanvas canvas, SKMatrix transform)
    {
    }

    public void Visit(SvgFeFlood element, SKCanvas canvas, SKMatrix transform)
    {
    }

    public void Visit(SvgFeGaussianBlur element, SKCanvas canvas, SKMatrix transform)
    {
    }

    public void Visit(SvgFeImage element, SKCanvas canvas, SKMatrix transform)
    {
    }

    public void Visit(SvgFeMerge element, SKCanvas canvas, SKMatrix transform)
    {
    }

    public void Visit(SvgFeMergeNode element, SKCanvas canvas, SKMatrix transform)
    {
    }

    public void Visit(SvgFeMorphology element, SKCanvas canvas, SKMatrix transform)
    {
    }

    public void Visit(SvgFeOffset element, SKCanvas canvas, SKMatrix transform)
    {
    }

    public void Visit(SvgFePointLight element, SKCanvas canvas, SKMatrix transform)
    {
    }

    public void Visit(SvgFeSpecularLighting element, SKCanvas canvas, SKMatrix transform)
    {
    }

    public void Visit(SvgFeSpotLight element, SKCanvas canvas, SKMatrix transform)
    {
    }

    public void Visit(SvgFeTile element, SKCanvas canvas, SKMatrix transform)
    {
    }

    public void Visit(SvgFeTurbulence element, SKCanvas canvas, SKMatrix transform)
    {
    }

    public void Visit(SvgFeDropShadow element, SKCanvas canvas, SKMatrix transform)
    {
    }

    public void Visit(SvgAnimate element, SKCanvas canvas, SKMatrix transform)
    {
    }

    public void Visit(SvgAnimateMotion element, SKCanvas canvas, SKMatrix transform)
    {
    }

    public void Visit(SvgAnimateTransform element, SKCanvas canvas, SKMatrix transform)
    {
    }

    public void Visit(SvgMPath element, SKCanvas canvas, SKMatrix transform)
    {
    }

    public void Visit(SvgSet element, SKCanvas canvas, SKMatrix transform)
    {
    }

    public void Visit(SvgView element, SKCanvas canvas, SKMatrix transform)
    {
    }

    public void Visit(SvgScript element, SKCanvas canvas, SKMatrix transform)
    {
    }

    public void Visit(SvgMetadata element, SKCanvas canvas, SKMatrix transform)
    {
    }

    public void Visit(SvgCursor element, SKCanvas canvas, SKMatrix transform)
    {
    }

    public void Visit(SvgSolidColor element, SKCanvas canvas, SKMatrix transform)
    {
    }

    public void Visit(SvgFont element, SKCanvas canvas, SKMatrix transform)
    {
    }

    public void Visit(SvgFontFace element, SKCanvas canvas, SKMatrix transform)
    {
    }

    public void Visit(SvgGlyph element, SKCanvas canvas, SKMatrix transform)
    {
    }

    public void Visit(SvgMissingGlyph element, SKCanvas canvas, SKMatrix transform)
    {
    }

    public void Visit(SvgHKern element, SKCanvas canvas, SKMatrix transform)
    {
    }

    public void Visit(SvgVKern element, SKCanvas canvas, SKMatrix transform)
    {
    }

    public void Visit(SvgFontFaceSrc element, SKCanvas canvas, SKMatrix transform)
    {
    }

    public void Visit(SvgFontFaceUri element, SKCanvas canvas, SKMatrix transform)
    {
    }

    public void Visit(SvgFontFaceFormat element, SKCanvas canvas, SKMatrix transform)
    {
    }

    public void Visit(SvgFontFaceName element, SKCanvas canvas, SKMatrix transform)
    {
    }

    public void Visit(SvgColorProfile element, SKCanvas canvas, SKMatrix transform)
    {
    }

    private void RenderPath(SKPath path, SvgElement element, SKCanvas canvas, SKMatrix transform, bool shouldDisposePath = true)
    {
        int saveCount = canvas.Save();
        canvas.SetMatrix(transform);

        var effectiveFillStyle = GetEffectiveFillStyle(element.FillStyle);
        if (effectiveFillStyle != null)
        {
            ConfigurePaint(effectiveFillStyle, true);
            canvas.DrawPath(path, _sharedPaint);
        }
        else
        {
            ConfigurePaint(GetDefaultFillStyle(), true);
            canvas.DrawPath(path, _sharedPaint);
        }

        var effectiveStrokeStyle = GetEffectiveStrokeStyle(element.StrokeStyle);
        if (effectiveStrokeStyle != null)
        {
            ConfigurePaint(effectiveStrokeStyle, false);
            canvas.DrawPath(path, _sharedPaint);
        }

        canvas.RestoreToCount(saveCount);

        if (shouldDisposePath)
        {
            path.Dispose();
        }
    }
}
