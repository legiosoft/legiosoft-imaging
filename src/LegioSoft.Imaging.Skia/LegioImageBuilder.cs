using System;
using System.IO;
using LegioSoft.Imaging.Core;
using SkiaSharp;

namespace LegioSoft.Imaging.Skia;

public class LegioImageBuilder
{
    private byte[] _imageData;
    private LegioImageFormat _format;
    private int? _targetWidth;
    private int? _targetHeight;
    private LegioScaleMode _scaleMode;
    private int? _cropX;
    private int? _cropY;
    private int? _cropWidth;
    private int? _cropHeight;
    private int _rotateDegrees;
    private bool _flipHorizontal;
    private bool _flipVertical;
    private bool _applyGrayscale;
    private bool _applySepia;
    private int _blurRadius;
    private int _sharpenAmount;
    private bool _hasFilter;
    private bool _hasSharpen;
    private int? _brightnessAmount;
    private int? _contrastAmount;
    private bool _invertColors;
    private LegioResizeQuality _resizeQuality;
    private int _saveQuality = 75;

    private LegioImageBuilder(byte[] imageData)
    {
        _imageData = imageData ?? throw new ArgumentNullException(nameof(imageData));
        var detectedFormat = ImageOperations.DetectFormat(imageData);
        _format = detectedFormat;
        _scaleMode = LegioScaleMode.Fit;
        _resizeQuality = LegioResizeQuality.High;
    }

    public static LegioImageBuilder Load(byte[] imageData)
    {
        return new LegioImageBuilder(imageData);
    }

    public static LegioImageBuilder Load(string filePath)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"File not found: {filePath}", filePath);
        return Load(File.ReadAllBytes(filePath));
    }

    public static LegioImageBuilder Load(Stream stream)
    {
        if (stream == null) throw new ArgumentNullException(nameof(stream));
        using var ms = new MemoryStream();
        stream.CopyTo(ms);
        return Load(ms.ToArray());
    }

    public LegioImageBuilder Resize(int width, int height, LegioScaleMode mode = LegioScaleMode.Fit, LegioResizeQuality quality = LegioResizeQuality.High)
    {
        if (width <= 0 || height <= 0)
            throw new ArgumentException("Width and height must be positive", nameof(width));
        
        _targetWidth = width;
        _targetHeight = height;
        _scaleMode = mode;
        _resizeQuality = quality;
        return this;
    }

    public LegioImageBuilder ResizeToWidth(int width, LegioResizeQuality quality = LegioResizeQuality.High)
    {
        if (width <= 0)
            throw new ArgumentException("Width must be positive", nameof(width));
        
        var info = GetInfo();
        var ratio = (double)width / info.Width;
        var newHeight = (int)(info.Height * ratio);
        
        return Resize(width, newHeight, LegioScaleMode.Stretch, quality);
    }

    public LegioImageBuilder ResizeToHeight(int height, LegioResizeQuality quality = LegioResizeQuality.High)
    {
        if (height <= 0)
            throw new ArgumentException("Height must be positive", nameof(height));
        
        var info = GetInfo();
        var ratio = (double)height / info.Height;
        var newWidth = (int)(info.Width * ratio);
        
        return Resize(newWidth, height, LegioScaleMode.Stretch, quality);
    }

    public LegioImageBuilder Scale(double factor, LegioResizeQuality quality = LegioResizeQuality.High)
    {
        if (factor <= 0)
            throw new ArgumentException("Scale factor must be positive", nameof(factor));
        
        var info = GetInfo();
        var newWidth = (int)(info.Width * factor);
        var newHeight = (int)(info.Height * factor);
        
        return Resize(newWidth, newHeight, LegioScaleMode.Stretch, quality);
    }

    public LegioImageBuilder Crop(int x, int y, int width, int height)
    {
        if (width <= 0 || height <= 0)
            throw new ArgumentException("Width and height must be positive", nameof(width));
        
        _cropX = x;
        _cropY = y;
        _cropWidth = width;
        _cropHeight = height;
        return this;
    }

    public LegioImageBuilder Rotate(int degrees)
    {
        if (degrees != 0 && degrees != 90 && degrees != 180 && degrees != 270)
            throw new ArgumentException("Rotation must be 0, 90, 180, or 270 degrees", nameof(degrees));
        
        _rotateDegrees = degrees;
        return this;
    }

    public LegioImageBuilder Flip(bool horizontal = true, bool vertical = false)
    {
        _flipHorizontal = horizontal;
        _flipVertical = vertical;
        return this;
    }

    public LegioImageBuilder Grayscale()
    {
        _applyGrayscale = true;
        return this;
    }

    public LegioImageBuilder Sepia()
    {
        _applySepia = true;
        return this;
    }

    public LegioImageBuilder Blur(int radius = 5)
    {
        if (radius <= 0 || radius > 20)
            throw new ArgumentException("Blur radius must be between 1 and 20", nameof(radius));
        
        _blurRadius = radius;
        _hasFilter = true;
        return this;
    }

    public LegioImageBuilder Sharpen(int amount = 50)
    {
        if (amount < 0 || amount > 100)
            throw new ArgumentException("Sharpen amount must be between 0 and 100", nameof(amount));
        
        _sharpenAmount = amount;
        _hasSharpen = true;
        return this;
    }

    public LegioImageBuilder Brightness(int amount)
    {
        if (amount < -255 || amount > 255)
            throw new ArgumentException("Brightness must be between -255 and 255", nameof(amount));
        
        _brightnessAmount = amount;
        return this;
    }

    public LegioImageBuilder Contrast(int amount)
    {
        if (amount < -100 || amount > 100)
            throw new ArgumentException("Contrast must be between -100 and 100", nameof(amount));
        
        _contrastAmount = amount;
        return this;
    }

    public LegioImageBuilder Invert()
    {
        _invertColors = true;
        return this;
    }

    public LegioImageBuilder Quality(int quality)
    {
        if (quality < 0 || quality > 100)
            throw new ArgumentException("Quality must be between 0 and 100", nameof(quality));
        
        _saveQuality = quality;
        return this;
    }

    public byte[] SaveAs(LegioImageFormat format, int? quality = null)
    {
        var finalQuality = quality ?? _saveQuality;
        var result = ApplyOperations();
        return ImageOperations.SaveBitmap(result, format, finalQuality);
    }

    public byte[] Save()
    {
        return SaveAs(_format, _saveQuality);
    }

    public void Save(string filePath, LegioImageFormat? format = null, int? quality = null)
    {
        var targetFormat = format ?? _format;
        var data = SaveAs(targetFormat, quality);
        File.WriteAllBytes(filePath, data);
    }

    public Stream SaveAsStream(LegioImageFormat format, int? quality = null)
    {
        var data = SaveAs(format, quality);
        return new MemoryStream(data);
    }

    public LegioImageInfo GetInfo()
    {
        var bitmap = ImageOperations.LoadBitmap(_imageData);
        return new LegioImageInfo
        {
            Width = bitmap.Width,
            Height = bitmap.Height,
            Format = _format,
            HasAlpha = bitmap.AlphaType != SKAlphaType.Opaque,
            ByteSize = _imageData.Length
        };
    }

    private SKBitmap ApplyOperations()
    {
        var bitmap = ImageOperations.LoadBitmap(_imageData);

        if (_cropX.HasValue)
        {
            var cropX = _cropX!.Value;
            var cropY = _cropY!.Value;
            var cropWidth = _cropWidth!.Value;
            var cropHeight = _cropHeight!.Value;
            bitmap = ImageOperations.CropBitmap(bitmap, cropX, cropY, cropWidth, cropHeight);
        }

        if (_targetWidth.HasValue)
        {
            var targetWidth = _targetWidth!.Value;
            var targetHeight = _targetHeight!.Value;
            bitmap = ImageOperations.ResizeBitmap(bitmap, targetWidth, targetHeight, _resizeQuality);
        }

        if (_rotateDegrees != 0)
        {
            bitmap = ImageOperations.Rotate(bitmap, _rotateDegrees);
        }

        if (_flipHorizontal || _flipVertical)
        {
            bitmap = ImageOperations.Flip(bitmap, _flipHorizontal, _flipVertical);
        }

        if (_invertColors)
        {
            bitmap = ImageOperations.ApplyInvert(bitmap);
        }

        if (_brightnessAmount.HasValue)
        {
            bitmap = ImageOperations.ApplyBrightness(bitmap, _brightnessAmount.Value);
        }

        if (_contrastAmount.HasValue)
        {
            bitmap = ImageOperations.ApplyContrast(bitmap, _contrastAmount.Value);
        }

        if (_applyGrayscale)
        {
            bitmap = ImageOperations.ApplyGrayscale(bitmap);
        }

        if (_applySepia)
        {
            bitmap = ImageOperations.ApplySepia(bitmap);
        }

        if (_hasSharpen)
        {
            bitmap = ImageOperations.ApplySharpen(bitmap, _sharpenAmount);
        }

        if (_hasFilter)
        {
            bitmap = ImageOperations.ApplyBlur(bitmap, _blurRadius);
        }

        return bitmap;
    }
}
