using LegioSoft.Imaging.Core.Classes;
using LegioSoft.Imaging.Core.Enums;
using LegioSoft.Imaging.WebP.Decoder;
using LegioSoft.Imaging.WebP.Encoder;
using LegioSoft.Imaging.WebP.Enums;

namespace LegioSoft.Imaging.WebP;

public class LegioImageBuilder
{
    private readonly byte[] _webpData;
    private readonly List<Func<byte[], byte[]>> _operations;
    private int _saveQuality = 90;

    private LegioImageBuilder(byte[] webpData)
    {
        _webpData = webpData ?? throw new ArgumentNullException(nameof(webpData));
        _operations = new List<Func<byte[], byte[]>>();
    }

    public static LegioImageBuilder Load(byte[] imageData)
    {
        var format = FormatDetector.DetectFormat(imageData);

        if (format != LegioImageFormat.WebP)
        {
            throw new NotSupportedException($"Only WebP format is supported by LegioImageBuilder. Detected format: {format}");
        }

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
        if (stream == null)
            throw new ArgumentNullException(nameof(stream));

        using var ms = new MemoryStream();
        stream.CopyTo(ms);
        return Load(ms.ToArray());
    }

    public LegioImageBuilder Resize(
        int width,
        int height,
        LegioScaleMode mode = LegioScaleMode.Fit,
        LegioResizeQuality quality = LegioResizeQuality.High)
    {
        if (width <= 0 || height <= 0)
            throw new ArgumentException("Width and height must be positive", nameof(width));

        _operations.Add(data => WebPDecoder.DecodeWithScaling(data, width, height, WEBP_CSP_MODE.MODE_RGBA));
        return this;
    }

    public LegioImageBuilder ResizeToWidth(int width, LegioResizeQuality quality = LegioResizeQuality.High)
    {
        if (width <= 0)
            throw new ArgumentException("Width must be positive", nameof(width));

        var info = WebPDecoder.GetInfo(_webpData);
        var ratio = (double)width / info.Width;
        var newHeight = (int)(info.Height * ratio);
        _operations.Add(data => WebPDecoder.DecodeWithScaling(_webpData, width, newHeight, WEBP_CSP_MODE.MODE_RGBA));
        return this;
    }

    public LegioImageBuilder ResizeToHeight(int height, LegioResizeQuality quality = LegioResizeQuality.High)
    {
        if (height <= 0)
            throw new ArgumentException("Height must be positive", nameof(height));

        var info = WebPDecoder.GetInfo(_webpData);
        var ratio = (double)height / info.Height;
        var newWidth = (int)(info.Width * ratio);
        _operations.Add(data => WebPDecoder.DecodeWithScaling(_webpData, newWidth, height, WEBP_CSP_MODE.MODE_RGBA));
        return this;
    }

    public LegioImageBuilder Scale(double factor, LegioResizeQuality quality = LegioResizeQuality.High)
    {
        if (factor <= 0)
            throw new ArgumentException("Scale factor must be positive", nameof(factor));

        var info = WebPDecoder.GetInfo(_webpData);
        var newWidth = (int)(info.Width * factor);
        var newHeight = (int)(info.Height * factor);
        _operations.Add(data => WebPDecoder.DecodeWithScaling(_webpData, newWidth, newHeight, WEBP_CSP_MODE.MODE_RGBA));
        return this;
    }

    public LegioImageBuilder Crop(int x, int y, int width, int height)
    {
        if (width <= 0 || height <= 0)
            throw new ArgumentException("Width and height must be positive", nameof(width));

        _operations.Add(data => WebPDecoder.DecodeWithCropping(_webpData, x, y, width, height, WEBP_CSP_MODE.MODE_RGBA));
        return this;
    }

    public LegioImageBuilder Rotate(int degrees)
    {
        if (degrees != 0 && degrees != 90 && degrees != 180 && degrees != 270)
            throw new ArgumentException("Rotation must be 0, 90, 180, or 270 degrees", nameof(degrees));

        if (degrees != 0)
        {
            _operations.Add(data =>
            {
                var info = WebPDecoder.GetInfo(_webpData);
                var newWidth = degrees % 180 == 0 ? info.Width : info.Height;
                var newHeight = degrees % 180 == 0 ? info.Height : info.Width;
                return WebPDecoder.DecodeWithScaling(_webpData, newWidth, newHeight, WEBP_CSP_MODE.MODE_RGBA);
            });
        }

        return this;
    }

    public LegioImageBuilder Flip(bool horizontal = true, bool vertical = false)
    {
        _operations.Add(data =>
        {
            if (horizontal)
            {
                throw new NotSupportedException("Horizontal flip is not supported for WebP. Use vertical flip only.");
            }
            return WebPDecoder.DecodeWithFlip(_webpData, WEBP_CSP_MODE.MODE_RGBA);
        });

        return this;
    }

    public LegioImageBuilder Quality(int quality)
    {
        if (quality is < 0 || quality > 100)
            throw new ArgumentException("Quality must be between 0 and 100", nameof(quality));

        _saveQuality = quality;
        return this;
    }

    public byte[] SaveAs(LegioImageFormat format, int? quality = null)
    {
        var finalQuality = quality ?? _saveQuality;
        var data = ApplyOperations();

        if (format != LegioImageFormat.WebP)
        {
            throw new NotSupportedException($"Only WebP format is supported. Requested format: {format}");
        }

        var info = WebPDecoder.GetInfo(data);
        return WebPEncoder.Encode(data, info.Width, info.Height, finalQuality);
    }

    public void Save(string filePath, LegioImageFormat? format = null, int? quality = null)
    {
        var targetFormat = format ?? LegioImageFormat.WebP;
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
        return new LegioImageInfo
        {
            Width = WebPDecoder.GetInfo(_webpData).Width,
            Height = WebPDecoder.GetInfo(_webpData).Height,
            Format = LegioImageFormat.WebP,
            HasAlpha = WebPDecoder.GetInfo(_webpData).HasAlpha,
            ByteSize = _webpData.Length
        };
    }

    private byte[] ApplyOperations()
    {
        byte[] currentData = _webpData;

        foreach (var operation in _operations)
        {
            currentData = operation(currentData);
        }

        return currentData;
    }
}
