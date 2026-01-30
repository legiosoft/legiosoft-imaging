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
