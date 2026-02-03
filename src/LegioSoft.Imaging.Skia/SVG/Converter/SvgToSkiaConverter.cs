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
    private SvgDocument? _document;
    private readonly Stack<SKPath> _clipPathStack = new Stack<SKPath>();

    public SKBitmap Convert(SvgDocument document, int width, int height)
    {
        if (document.RootElement == null)
            throw new InvalidOperationException("SVG document has no root element");
        if (document.ViewBox.Width <= 0 || document.ViewBox.Height <= 0)
            throw new InvalidOperationException($"Invalid ViewBox: {document.ViewBox}");

        _document = document;

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

        _document = null;

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

    private (SKMatrix matrix, SKRect viewBox) ComputeTransformAndViewport(SvgDocument document, int width, int height)
    {
        SKRect viewBox = document.ViewBox;
        
        float scaleX = (float)width / viewBox.Width;
        float scaleY = (float)height / viewBox.Height;
        
        if (!string.IsNullOrEmpty(document.PreserveAspectRatio))
        {
            var parts = document.PreserveAspectRatio.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            string alignment = parts.Length > 0 ? parts[0] : "xMidYMid";
            string meetOrSlice = parts.Length > 1 ? parts[1] : "meet";

            bool shouldPreserveAspectRatio = !string.Equals(alignment, "none", StringComparison.OrdinalIgnoreCase);
            
            if (shouldPreserveAspectRatio)
            {
                if (string.Equals(meetOrSlice, "slice", StringComparison.OrdinalIgnoreCase))
                {
                    scale = Math.Max(scaleX, scaleY);
                }
                else
                {
                    scale = Math.Min(scaleX, scaleY);
                }
                scaleX = scaleY = scale;
            }
        }

        float translateX = -viewBox.Left * scaleX;
        float translateY = -viewBox.Top * scaleY;
        
        if (!string.IsNullOrEmpty(document.PreserveAspectRatio) && !string.Equals(document.PreserveAspectRatio.Split(' ')[0], "none", StringComparison.OrdinalIgnoreCase))
        {
            var parts = document.PreserveAspectRatio.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            string alignment = parts.Length > 0 ? parts[0] : "xMidYMid";
            
            float contentWidth = viewBox.Width * scaleX;
            float contentHeight = viewBox.Height * scaleY;
            
            if (alignment.StartsWith("xMin", StringComparison.OrdinalIgnoreCase))
            {
                translateX = -viewBox.Left * scaleX;
            }
            else if (alignment.StartsWith("xMid", StringComparison.OrdinalIgnoreCase))
            {
                translateX = -viewBox.Left * scaleX + (width - contentWidth) / 2;
            }
            else if (alignment.StartsWith("xMax", StringComparison.OrdinalIgnoreCase))
            {
                translateX = -viewBox.Left * scaleX + (width - contentWidth);
            }
            
            if (alignment.Contains("YMin", StringComparison.OrdinalIgnoreCase))
            {
                translateY = -viewBox.Top * scaleY;
            }
            else if (alignment.Contains("YMid", StringComparison.OrdinalIgnoreCase))
            {
                translateY = -viewBox.Top * scaleY + (height - contentHeight) / 2;
            }
            else if (alignment.Contains("YMax", StringComparison.OrdinalIgnoreCase))
            {
                translateY = -viewBox.Top * scaleY + (height - contentHeight);
            }
        }
        
        translateX += document.X;
        translateY += document.Y;

        SKMatrix initialMatrix = SKMatrix.CreateScale(scaleX, scaleY);
        initialMatrix = initialMatrix.PostConcat(SKMatrix.CreateTranslation(translateX, translateY));

        return (initialMatrix, viewBox);
    }

    private float scale;

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

        if (!isFill)
        {
            _sharedPaint.StrokeCap = style.StrokeLineCap;
            _sharedPaint.StrokeJoin = style.StrokeLineJoin;
            _sharedPaint.StrokeMiter = style.StrokeMiterLimit;

            if (style.StrokeDashArray.Length > 0)
            {
                _sharedPaint.PathEffect = SKPathEffect.CreateDash(style.StrokeDashArray, style.StrokeDashOffset);
            }
            else
            {
                _sharedPaint.PathEffect = null;
            }
        }
        else
        {
            _sharedPaint.PathEffect = null;
        }
    }

    private int ApplyClipPath(SvgElement element, SKCanvas canvas)
    {
        string? clipPathRef = element switch
        {
            SvgRect r => r.ClipPath,
            SvgCircle c => c.ClipPath,
            SvgEllipse e => e.ClipPath,
            SvgLine l => l.ClipPath,
            SvgPolyline p => p.ClipPath,
            SvgPolygon pg => pg.ClipPath,
            SvgPath path => path.ClipPath,
            SvgImage i => i.ClipPath,
            SvgText t => t.ClipPath,
            SvgUse u => u.ClipPath,
            SvgGroup g => g.ClipPath,
            _ => null
        };

        if (string.IsNullOrEmpty(clipPathRef))
            return 0;

        clipPathRef = clipPathRef.Trim();
        if (clipPathRef.StartsWith("url(") && clipPathRef.EndsWith(")"))
        {
            string id = clipPathRef.Substring(4, clipPathRef.Length - 5).Trim('#');
            if (_document != null && _document.ElementsById.TryGetValue(id, out var clipPathElement) && clipPathElement is SvgClipPath clipPath)
            {
                SKPath clipPathSkia = BuildClipPath(clipPath);
                if (clipPathSkia != null)
                {
                    canvas.Save();
                    canvas.ClipPath(clipPathSkia);
                    _clipPathStack.Push(clipPathSkia);
                    return 1;
                }
            }
        }

        return 0;
    }

    private SKPath? BuildClipPath(SvgClipPath clipPathElement)
    {
        if (clipPathElement.Children == null || clipPathElement.Children.Count == 0)
            return null;

        var path = new SKPath();
        foreach (var child in clipPathElement.Children)
        {
            if (child is SvgPath svgPath && svgPath.Path != null)
            {
                path.AddPath(svgPath.Path);
            }
            else if (child is SvgRect rect)
            {
                path.AddRect(SKRect.Create(rect.X, rect.Y, rect.Width, rect.Height));
            }
            else if (child is SvgCircle circle)
            {
                path.AddCircle(circle.Cx, circle.Cy, circle.Radius);
            }
            else if (child is SvgEllipse ellipse)
            {
                path.AddOval(SKRect.Create(ellipse.Cx - ellipse.Rx, ellipse.Cy - ellipse.Ry, ellipse.Rx * 2, ellipse.Ry * 2));
            }
            else if (child is SvgPolyline polyline)
            {
                if (polyline.Points != null && polyline.Points.Count > 0)
                {
                    path.MoveTo(polyline.Points[0]);
                    for (int i = 1; i < polyline.Points.Count; i++)
                    {
                        path.LineTo(polyline.Points[i]);
                    }
                }
            }
            else if (child is SvgPolygon polygon)
            {
                if (polygon.Points != null && polygon.Points.Count > 0)
                {
                    path.MoveTo(polygon.Points[0]);
                    for (int i = 1; i < polygon.Points.Count; i++)
                    {
                        path.LineTo(polygon.Points[i]);
                    }
                    path.Close();
                }
            }
            else if (child is SvgLine line)
            {
                path.MoveTo(line.X1, line.Y1);
                path.LineTo(line.X2, line.Y2);
            }
            else if (child is SvgGroup group)
            {
                foreach (var groupChild in group.Children ?? Enumerable.Empty<SvgElement>())
                {
                    var childPath = BuildElementPath(groupChild);
                    if (childPath != null)
                    {
                        path.AddPath(childPath);
                    }
                }
            }
        }

        return path;
    }

    private SKPath? BuildElementPath(SvgElement element)
    {
        if (element is SvgPath svgPath && svgPath.Path != null)
            return svgPath.Path;
        else if (element is SvgRect rect)
        {
            var path = new SKPath();
            path.AddRect(SKRect.Create(rect.X, rect.Y, rect.Width, rect.Height));
            return path;
        }
        else if (element is SvgCircle circle)
        {
            var path = new SKPath();
            path.AddCircle(circle.Cx, circle.Cy, circle.Radius);
            return path;
        }
        else if (element is SvgEllipse ellipse)
        {
            var path = new SKPath();
            path.AddOval(SKRect.Create(ellipse.Cx - ellipse.Rx, ellipse.Cy - ellipse.Ry, ellipse.Rx * 2, ellipse.Ry * 2));
            return path;
        }
        else if (element is SvgPolyline polyline && polyline.Points != null && polyline.Points.Count > 0)
        {
            var path = new SKPath();
            path.MoveTo(polyline.Points[0]);
            for (int i = 1; i < polyline.Points.Count; i++)
            {
                path.LineTo(polyline.Points[i]);
            }
            return path;
        }
        else if (element is SvgPolygon polygon && polygon.Points != null && polygon.Points.Count > 0)
        {
            var path = new SKPath();
            path.MoveTo(polygon.Points[0]);
            for (int i = 1; i < polygon.Points.Count; i++)
            {
                path.LineTo(polygon.Points[i]);
            }
            path.Close();
            return path;
        }
        else if (element is SvgLine line)
        {
            var path = new SKPath();
            path.MoveTo(line.X1, line.Y1);
            path.LineTo(line.X2, line.Y2);
            return path;
        }

        return null;
    }

    private void RestoreClipPath(SKCanvas canvas)
    {
        if (_clipPathStack.Count > 0)
        {
            _clipPathStack.Pop();
            canvas.Restore();
        }
    }

    public void Visit(SvgRect element, SKCanvas canvas, SKMatrix transform)
    {
        if (string.Equals(element.Display, "none", StringComparison.OrdinalIgnoreCase))
            return;

        bool isVisible = string.IsNullOrEmpty(element.Visibility) ||
                         !string.Equals(element.Visibility, "hidden", StringComparison.OrdinalIgnoreCase) ||
                         !string.Equals(element.Visibility, "collapse", StringComparison.OrdinalIgnoreCase);

        if (!isVisible)
            return;

        SKRect rect = SKRect.Create(element.X, element.Y, element.Width, element.Height);

        SKMatrix rectMatrix = element.Transform == null || element.Transform.Value.IsIdentity
            ? transform
            : transform.PostConcat(element.Transform.Value);

        int saveCount = canvas.Save();
        canvas.SetMatrix(rectMatrix);

        int clipDepth = ApplyClipPath(element, canvas);

        if (element.Opacity < 1.0f)
        {
            _sharedPaint.Color = new SKColor(0, 0, 0, (byte)(255 * element.Opacity));
            _sharedPaint.Style = SKPaintStyle.Fill;
            canvas.SaveLayer(_sharedPaint);
        }

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

        if (element.Opacity < 1.0f)
        {
            canvas.Restore();
        }

        for (int i = 0; i < clipDepth; i++)
        {
            RestoreClipPath(canvas);
        }

        canvas.RestoreToCount(saveCount);
    }

    public void Visit(SvgCircle element, SKCanvas canvas, SKMatrix transform)
    {
        if (string.Equals(element.Display, "none", StringComparison.OrdinalIgnoreCase))
            return;

        bool isVisible = string.IsNullOrEmpty(element.Visibility) ||
                         !string.Equals(element.Visibility, "hidden", StringComparison.OrdinalIgnoreCase) ||
                         !string.Equals(element.Visibility, "collapse", StringComparison.OrdinalIgnoreCase);

        if (!isVisible)
            return;

        int saveCount = canvas.Save();

        SKMatrix circleMatrix = element.Transform == null || element.Transform.Value.IsIdentity
            ? transform
            : transform.PostConcat(element.Transform.Value);

        canvas.SetMatrix(circleMatrix);

        if (element.Opacity < 1.0f)
        {
            _sharedPaint.Color = new SKColor(0, 0, 0, (byte)(255 * element.Opacity));
            _sharedPaint.Style = SKPaintStyle.Fill;
            canvas.SaveLayer(_sharedPaint);
        }

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

        if (element.Opacity < 1.0f)
        {
            canvas.Restore();
        }

        canvas.RestoreToCount(saveCount);
    }

    public void Visit(SvgEllipse element, SKCanvas canvas, SKMatrix transform)
    {
        if (string.Equals(element.Display, "none", StringComparison.OrdinalIgnoreCase))
            return;

        bool isVisible = string.IsNullOrEmpty(element.Visibility) ||
                         !string.Equals(element.Visibility, "hidden", StringComparison.OrdinalIgnoreCase) ||
                         !string.Equals(element.Visibility, "collapse", StringComparison.OrdinalIgnoreCase);

        if (!isVisible)
            return;

        SKRect rect = new SKRect(
            element.Cx - element.Rx,
            element.Cy - element.Ry,
            element.Cx + element.Rx,
            element.Cy + element.Ry
        );

        int saveCount = canvas.Save();

        SKMatrix ellipseMatrix = element.Transform == null || element.Transform.Value.IsIdentity
            ? transform
            : transform.PostConcat(element.Transform.Value);

        canvas.SetMatrix(ellipseMatrix);

        if (element.Opacity < 1.0f)
        {
            _sharedPaint.Color = new SKColor(0, 0, 0, (byte)(255 * element.Opacity));
            _sharedPaint.Style = SKPaintStyle.Fill;
            canvas.SaveLayer(_sharedPaint);
        }

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

        if (element.Opacity < 1.0f)
        {
            canvas.Restore();
        }

        canvas.RestoreToCount(saveCount);
    }

    public void Visit(SvgLine element, SKCanvas canvas, SKMatrix transform)
    {
        if (string.Equals(element.Display, "none", StringComparison.OrdinalIgnoreCase))
            return;

        bool isVisible = string.IsNullOrEmpty(element.Visibility) ||
                         !string.Equals(element.Visibility, "hidden", StringComparison.OrdinalIgnoreCase) ||
                         !string.Equals(element.Visibility, "collapse", StringComparison.OrdinalIgnoreCase);

        if (!isVisible)
            return;

        int saveCount = canvas.Save();

        SKMatrix lineMatrix = element.Transform == null || element.Transform.Value.IsIdentity
            ? transform
            : transform.PostConcat(element.Transform.Value);

        canvas.SetMatrix(lineMatrix);

        if (element.Opacity < 1.0f)
        {
            _sharedPaint.Color = new SKColor(0, 0, 0, (byte)(255 * element.Opacity));
            _sharedPaint.Style = SKPaintStyle.Fill;
            canvas.SaveLayer(_sharedPaint);
        }

        var effectiveStrokeStyle = GetEffectiveStrokeStyle(element.StrokeStyle);
        if (effectiveStrokeStyle != null)
        {
            ConfigurePaint(effectiveStrokeStyle, false);
            canvas.DrawLine(element.X1, element.Y1, element.X2, element.Y2, _sharedPaint);
        }

        if (element.Opacity < 1.0f)
        {
            canvas.Restore();
        }

        canvas.RestoreToCount(saveCount);
    }

    public void Visit(SvgPolyline element, SKCanvas canvas, SKMatrix transform)
    {
        if (element.Points == null || element.Points.Count < 2) return;

        if (string.Equals(element.Display, "none", StringComparison.OrdinalIgnoreCase))
            return;

        bool isVisible = string.IsNullOrEmpty(element.Visibility) ||
                         !string.Equals(element.Visibility, "hidden", StringComparison.OrdinalIgnoreCase) ||
                         !string.Equals(element.Visibility, "collapse", StringComparison.OrdinalIgnoreCase);

        if (!isVisible)
            return;

        int saveCount = canvas.Save();

        SKMatrix polylineMatrix = element.Transform == null || element.Transform.Value.IsIdentity
            ? transform
            : transform.PostConcat(element.Transform.Value);

        canvas.SetMatrix(polylineMatrix);

        if (element.Opacity < 1.0f)
        {
            _sharedPaint.Color = new SKColor(0, 0, 0, (byte)(255 * element.Opacity));
            _sharedPaint.Style = SKPaintStyle.Fill;
            canvas.SaveLayer(_sharedPaint);
        }

        var path = new SKPath();
        path.MoveTo(element.Points[0]);
        for (int i = 1; i < element.Points.Count; i++)
        {
            path.LineTo(element.Points[i]);
        }
        RenderPath(path, element, canvas, SKMatrix.Identity, shouldDisposePath: true);

        if (element.Opacity < 1.0f)
        {
            canvas.Restore();
        }

        canvas.RestoreToCount(saveCount);
    }

    public void Visit(SvgPolygon element, SKCanvas canvas, SKMatrix transform)
    {
        if (element.Points == null || element.Points.Count < 2) return;

        if (string.Equals(element.Display, "none", StringComparison.OrdinalIgnoreCase))
            return;

        bool isVisible = string.IsNullOrEmpty(element.Visibility) ||
                         !string.Equals(element.Visibility, "hidden", StringComparison.OrdinalIgnoreCase) ||
                         !string.Equals(element.Visibility, "collapse", StringComparison.OrdinalIgnoreCase);

        if (!isVisible)
            return;

        int saveCount = canvas.Save();

        SKMatrix polygonMatrix = element.Transform == null || element.Transform.Value.IsIdentity
            ? transform
            : transform.PostConcat(element.Transform.Value);

        canvas.SetMatrix(polygonMatrix);

        if (element.Opacity < 1.0f)
        {
            _sharedPaint.Color = new SKColor(0, 0, 0, (byte)(255 * element.Opacity));
            _sharedPaint.Style = SKPaintStyle.Fill;
            canvas.SaveLayer(_sharedPaint);
        }

        var path = new SKPath();
        path.MoveTo(element.Points[0]);
        for (int i = 1; i < element.Points.Count; i++)
        {
            path.LineTo(element.Points[i]);
        }
        path.Close();
        RenderPath(path, element, canvas, SKMatrix.Identity, shouldDisposePath: true);

        if (element.Opacity < 1.0f)
        {
            canvas.Restore();
        }

        canvas.RestoreToCount(saveCount);
    }

    public void Visit(SvgPath element, SKCanvas canvas, SKMatrix transform)
    {
        if (element.Path == null) return;

        if (string.Equals(element.Display, "none", StringComparison.OrdinalIgnoreCase))
            return;

        bool isVisible = string.IsNullOrEmpty(element.Visibility) ||
                         !string.Equals(element.Visibility, "hidden", StringComparison.OrdinalIgnoreCase) ||
                         !string.Equals(element.Visibility, "collapse", StringComparison.OrdinalIgnoreCase);

        if (!isVisible)
            return;

        int saveCount = canvas.Save();

        SKMatrix pathMatrix = element.Transform == null || element.Transform.Value.IsIdentity
            ? transform
            : transform.PostConcat(element.Transform.Value);

        canvas.SetMatrix(pathMatrix);

        if (element.Opacity < 1.0f)
        {
            _sharedPaint.Color = new SKColor(0, 0, 0, (byte)(255 * element.Opacity));
            _sharedPaint.Style = SKPaintStyle.Fill;
            canvas.SaveLayer(_sharedPaint);
        }

        RenderPath(element.Path, element, canvas, SKMatrix.Identity, shouldDisposePath: false);

        if (element.Opacity < 1.0f)
        {
            canvas.Restore();
        }

        canvas.RestoreToCount(saveCount);
    }

    public void Visit(SvgGroup element, SKCanvas canvas, SKMatrix transform)
    {
        if (element.Children == null || element.Children.Count == 0) return;

        if (string.Equals(element.Display, "none", StringComparison.OrdinalIgnoreCase))
            return;

        bool isVisible = string.IsNullOrEmpty(element.Visibility) || 
                         !string.Equals(element.Visibility, "hidden", StringComparison.OrdinalIgnoreCase) ||
                         !string.Equals(element.Visibility, "collapse", StringComparison.OrdinalIgnoreCase);

        if (!isVisible)
            return;

        int saveCount = canvas.Save();

        SKMatrix groupMatrix = element.Transform == null || element.Transform.Value.IsIdentity
            ? transform
            : transform.PostConcat(element.Transform.Value);

        if (element.Opacity < 1.0f)
        {
            _sharedPaint.Color = new SKColor(0, 0, 0, (byte)(255 * element.Opacity));
            _sharedPaint.Style = SKPaintStyle.Fill;
            canvas.SaveLayer(_sharedPaint);
        }

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

        if (element.Opacity < 1.0f)
        {
            canvas.Restore();
        }

        canvas.RestoreToCount(saveCount);
    }

    public void Visit(SvgDefs element, SKCanvas canvas, SKMatrix transform)
    {
        if (element.Children == null || element.Children.Count == 0) return;

        SKMatrix defsMatrix = element.Transform == null || element.Transform.Value.IsIdentity
            ? transform
            : transform.PostConcat(element.Transform.Value);

        foreach (var child in element.Children)
        {
            child.Accept(this, canvas, defsMatrix);
        }
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

        int saveCount = canvas.Save();

        SKMatrix symbolMatrix = element.Transform == null || element.Transform.Value.IsIdentity
            ? transform
            : transform.PostConcat(element.Transform.Value);

        if (element.ViewBox.HasValue)
        {
            SKRect viewBox = element.ViewBox.Value;
            float width = element.Width > 0 ? element.Width : viewBox.Width;
            float height = element.Height > 0 ? element.Height : viewBox.Height;

            if (width > 0 && height > 0)
            {
                float scaleX = width / viewBox.Width;
                float scaleY = height / viewBox.Height;

                if (!string.IsNullOrEmpty(element.PreserveAspectRatio))
                {
                    var parts = element.PreserveAspectRatio.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    string alignment = parts.Length > 0 ? parts[0] : "xMidYMid";
                    string meetOrSlice = parts.Length > 1 ? parts[1] : "meet";

                    bool shouldPreserveAspectRatio = !string.Equals(alignment, "none", StringComparison.OrdinalIgnoreCase);

                    if (shouldPreserveAspectRatio)
                    {
                        if (string.Equals(meetOrSlice, "slice", StringComparison.OrdinalIgnoreCase))
                        {
                            scale = Math.Max(scaleX, scaleY);
                        }
                        else
                        {
                            scale = Math.Min(scaleX, scaleY);
                        }
                        scaleX = scaleY = scale;
                    }
                }

                float translateX = element.X - viewBox.Left * scaleX;
                float translateY = element.Y - viewBox.Top * scaleY;

                if (!string.IsNullOrEmpty(element.PreserveAspectRatio))
                {
                    var parts = element.PreserveAspectRatio.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    string alignment = parts.Length > 0 ? parts[0] : "xMidYMid";

                    float contentWidth = viewBox.Width * scaleX;
                    float contentHeight = viewBox.Height * scaleY;

                    if (alignment.StartsWith("xMin", StringComparison.OrdinalIgnoreCase))
                    {
                        translateX = element.X - viewBox.Left * scaleX;
                    }
                    else if (alignment.StartsWith("xMid", StringComparison.OrdinalIgnoreCase))
                    {
                        translateX = element.X - viewBox.Left * scaleX + (width - contentWidth) / 2;
                    }
                    else if (alignment.StartsWith("xMax", StringComparison.OrdinalIgnoreCase))
                    {
                        translateX = element.X - viewBox.Left * scaleX + (width - contentWidth);
                    }

                    if (alignment.Contains("YMin", StringComparison.OrdinalIgnoreCase))
                    {
                        translateY = element.Y - viewBox.Top * scaleY;
                    }
                    else if (alignment.Contains("YMid", StringComparison.OrdinalIgnoreCase))
                    {
                        translateY = element.Y - viewBox.Top * scaleY + (height - contentHeight) / 2;
                    }
                    else if (alignment.Contains("YMax", StringComparison.OrdinalIgnoreCase))
                    {
                        translateY = element.Y - viewBox.Top * scaleY + (height - contentHeight);
                    }
                }

                SKMatrix viewBoxMatrix = SKMatrix.CreateScale(scaleX, scaleY);
                viewBoxMatrix = viewBoxMatrix.PostConcat(SKMatrix.CreateTranslation(translateX, translateY));
                symbolMatrix = symbolMatrix.PostConcat(viewBoxMatrix);
            }
            else
            {
                SKMatrix translationMatrix = SKMatrix.CreateTranslation(element.X, element.Y);
                symbolMatrix = symbolMatrix.PostConcat(translationMatrix);
            }
        }
        else if (element.X != 0 || element.Y != 0)
        {
            SKMatrix translationMatrix = SKMatrix.CreateTranslation(element.X, element.Y);
            symbolMatrix = symbolMatrix.PostConcat(translationMatrix);
        }

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

        canvas.RestoreToCount(saveCount);
    }

    public void Visit(SvgUse element, SKCanvas canvas, SKMatrix transform)
    {
        if (string.IsNullOrEmpty(element.Href))
            return;

        if (string.Equals(element.Display, "none", StringComparison.OrdinalIgnoreCase))
            return;

        bool isVisible = string.IsNullOrEmpty(element.Visibility) ||
                         !string.Equals(element.Visibility, "hidden", StringComparison.OrdinalIgnoreCase) ||
                         !string.Equals(element.Visibility, "collapse", StringComparison.OrdinalIgnoreCase);

        if (!isVisible)
            return;

        int saveCount = canvas.Save();

        SKMatrix useMatrix = element.Transform == null || element.Transform.Value.IsIdentity
            ? transform
            : transform.PostConcat(element.Transform.Value);

        useMatrix = useMatrix.PostConcat(SKMatrix.CreateTranslation(element.X, element.Y));

        if (element.Opacity < 1.0f)
        {
            _sharedPaint.Color = new SKColor(0, 0, 0, (byte)(255 * element.Opacity));
            _sharedPaint.Style = SKPaintStyle.Fill;
            canvas.SaveLayer(_sharedPaint);
        }

        if (element.FillStyle != null)
        {
            _fillStack.Push(element.FillStyle);
        }

        if (element.StrokeStyle != null)
        {
            _strokeStack.Push(element.StrokeStyle);
        }

        if (element.Href.StartsWith("#"))
        {
            string targetId = element.Href.Substring(1);
        }

        if (element.StrokeStyle != null)
        {
            _strokeStack.Pop();
        }

        if (element.FillStyle != null)
        {
            _fillStack.Pop();
        }

        if (element.Opacity < 1.0f)
        {
            canvas.Restore();
        }

        canvas.RestoreToCount(saveCount);
    }

    public void Visit(SvgImage element, SKCanvas canvas, SKMatrix transform)
    {
        if (element.Bitmap == null) return;

        if (string.Equals(element.Display, "none", StringComparison.OrdinalIgnoreCase))
            return;

        bool isVisible = string.IsNullOrEmpty(element.Visibility) ||
                         !string.Equals(element.Visibility, "hidden", StringComparison.OrdinalIgnoreCase) ||
                         !string.Equals(element.Visibility, "collapse", StringComparison.OrdinalIgnoreCase);

        if (!isVisible)
            return;

        SKRect dest = SKRect.Create(element.X, element.Y, element.Width, element.Height);

        int saveCount = canvas.Save();

        SKMatrix imageMatrix = element.Transform == null || element.Transform.Value.IsIdentity
            ? transform
            : transform.PostConcat(element.Transform.Value);

        canvas.SetMatrix(imageMatrix);

        if (element.Opacity < 1.0f)
        {
            _sharedPaint.Color = new SKColor(0, 0, 0, (byte)(255 * element.Opacity));
            _sharedPaint.Style = SKPaintStyle.Fill;
            canvas.SaveLayer(_sharedPaint);
        }

        canvas.DrawBitmap(element.Bitmap, dest);

        if (element.Opacity < 1.0f)
        {
            canvas.Restore();
        }

        canvas.RestoreToCount(saveCount);
    }

    public void Visit(SvgSwitch element, SKCanvas canvas, SKMatrix transform)
    {
        if (element.Children == null || element.Children.Count == 0) return;

        if (string.Equals(element.Display, "none", StringComparison.OrdinalIgnoreCase))
            return;

        bool isVisible = string.IsNullOrEmpty(element.Visibility) ||
                         !string.Equals(element.Visibility, "hidden", StringComparison.OrdinalIgnoreCase) ||
                         !string.Equals(element.Visibility, "collapse", StringComparison.OrdinalIgnoreCase);

        if (!isVisible)
            return;

        int saveCount = canvas.Save();

        SKMatrix switchMatrix = element.Transform == null || element.Transform.Value.IsIdentity
            ? transform
            : transform.PostConcat(element.Transform.Value);

        if (element.Opacity < 1.0f)
        {
            _sharedPaint.Color = new SKColor(0, 0, 0, (byte)(255 * element.Opacity));
            _sharedPaint.Style = SKPaintStyle.Fill;
            canvas.SaveLayer(_sharedPaint);
        }

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

        if (element.Opacity < 1.0f)
        {
            canvas.Restore();
        }

        canvas.RestoreToCount(saveCount);
    }

    public void Visit(SvgStyleElement element, SKCanvas canvas, SKMatrix transform)
    {
    }

    public void Visit(SvgForeignObject element, SKCanvas canvas, SKMatrix transform)
    {
        if (string.Equals(element.Display, "none", StringComparison.OrdinalIgnoreCase))
            return;

        bool isVisible = string.IsNullOrEmpty(element.Visibility) ||
                         !string.Equals(element.Visibility, "hidden", StringComparison.OrdinalIgnoreCase) ||
                         !string.Equals(element.Visibility, "collapse", StringComparison.OrdinalIgnoreCase);

        if (!isVisible)
            return;

        int saveCount = canvas.Save();

        SKMatrix foreignObjectMatrix = element.Transform == null || element.Transform.Value.IsIdentity
            ? transform
            : transform.PostConcat(element.Transform.Value);

        canvas.SetMatrix(foreignObjectMatrix);

        if (element.Opacity < 1.0f)
        {
            _sharedPaint.Color = new SKColor(0, 0, 0, (byte)(255 * element.Opacity));
            _sharedPaint.Style = SKPaintStyle.Fill;
            canvas.SaveLayer(_sharedPaint);
        }

        SKRect rect = SKRect.Create(element.X, element.Y, element.Width, element.Height);
        canvas.DrawRect(rect, _sharedPaint);

        if (element.Opacity < 1.0f)
        {
            canvas.Restore();
        }

        canvas.RestoreToCount(saveCount);
    }

    public void Visit(SvgText element, SKCanvas canvas, SKMatrix transform)
    {
        if (string.IsNullOrEmpty(element.Text) && (element.Children == null || element.Children.Count == 0)) return;

        if (string.Equals(element.Display, "none", StringComparison.OrdinalIgnoreCase))
            return;

        bool isVisible = string.IsNullOrEmpty(element.Visibility) ||
                         !string.Equals(element.Visibility, "hidden", StringComparison.OrdinalIgnoreCase) ||
                         !string.Equals(element.Visibility, "collapse", StringComparison.OrdinalIgnoreCase);

        if (!isVisible)
            return;

        int saveCount = canvas.Save();

        SKMatrix textMatrix = element.Transform == null || element.Transform.Value.IsIdentity
            ? transform
            : transform.PostConcat(element.Transform.Value);

        canvas.SetMatrix(textMatrix);

        if (element.Opacity < 1.0f)
        {
            _sharedPaint.Color = new SKColor(0, 0, 0, (byte)(255 * element.Opacity));
            _sharedPaint.Style = SKPaintStyle.Fill;
            canvas.SaveLayer(_sharedPaint);
        }

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
                child.Accept(this, canvas, textMatrix);
            }
        }

        if (element.Opacity < 1.0f)
        {
            canvas.Restore();
        }

        canvas.RestoreToCount(saveCount);

        typeface?.Dispose();
        font.Dispose();
        paint.Dispose();
    }

    public void Visit(SvgTSpan element, SKCanvas canvas, SKMatrix transform)
    {
        if (string.IsNullOrEmpty(element.Text) && (element.Children == null || element.Children.Count == 0)) return;

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

        var font = new SKFont(typeface ?? SKTypeface.Default, element.FontSize)
        {
            Edging = SKFontEdging.Antialias
        };

        var paint = new SKPaint
        {
            Color = textStyle.Color,
            IsAntialias = true
        };

        canvas.DrawText(element.Text, 0, 0, font, paint);

        canvas.RestoreToCount(saveCount);

        typeface?.Dispose();
        font.Dispose();
        paint.Dispose();
    }

    public void Visit(SvgAnchor element, SKCanvas canvas, SKMatrix transform)
    {
        if (element.Children == null || element.Children.Count == 0) return;

        if (string.Equals(element.Display, "none", StringComparison.OrdinalIgnoreCase))
            return;

        bool isVisible = string.IsNullOrEmpty(element.Visibility) ||
                         !string.Equals(element.Visibility, "hidden", StringComparison.OrdinalIgnoreCase) ||
                         !string.Equals(element.Visibility, "collapse", StringComparison.OrdinalIgnoreCase);

        if (!isVisible)
            return;

        int saveCount = canvas.Save();

        SKMatrix anchorMatrix = element.Transform == null || element.Transform.Value.IsIdentity
            ? transform
            : transform.PostConcat(element.Transform.Value);

        if (element.Opacity < 1.0f)
        {
            _sharedPaint.Color = new SKColor(0, 0, 0, (byte)(255 * element.Opacity));
            _sharedPaint.Style = SKPaintStyle.Fill;
            canvas.SaveLayer(_sharedPaint);
        }

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

        if (element.Opacity < 1.0f)
        {
            canvas.Restore();
        }

        canvas.RestoreToCount(saveCount);
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

        int saveCount = canvas.Save();

        SKMatrix markerMatrix = element.Transform == null || element.Transform.Value.IsIdentity
            ? transform
            : transform.PostConcat(element.Transform.Value);

        if (element.Opacity < 1.0f)
        {
            _sharedPaint.Color = new SKColor(0, 0, 0, (byte)(255 * element.Opacity));
            _sharedPaint.Style = SKPaintStyle.Fill;
            canvas.SaveLayer(_sharedPaint);
        }

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

        if (element.Opacity < 1.0f)
        {
            canvas.Restore();
        }

        canvas.RestoreToCount(saveCount);
    }

    public void Visit(SvgMask element, SKCanvas canvas, SKMatrix transform)
    {
        if (element.Children == null || element.Children.Count == 0) return;

        int saveCount = canvas.Save();

        SKMatrix maskMatrix = element.Transform == null || element.Transform.Value.IsIdentity
            ? transform
            : transform.PostConcat(element.Transform.Value);

        canvas.SetMatrix(maskMatrix);

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

        canvas.RestoreToCount(saveCount);
    }

    public void Visit(SvgClipPath element, SKCanvas canvas, SKMatrix transform)
    {
        if (element.Children == null || element.Children.Count == 0) return;

        int saveCount = canvas.Save();

        SKMatrix clipPathMatrix = element.Transform == null || element.Transform.Value.IsIdentity
            ? transform
            : transform.PostConcat(element.Transform.Value);

        canvas.SetMatrix(clipPathMatrix);

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

        canvas.RestoreToCount(saveCount);
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
            path.FillType = effectiveFillStyle.FillRule;
            ConfigurePaint(effectiveFillStyle, true);
            canvas.DrawPath(path, _sharedPaint);
        }
        else
        {
            path.FillType = GetDefaultFillStyle().FillRule;
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
