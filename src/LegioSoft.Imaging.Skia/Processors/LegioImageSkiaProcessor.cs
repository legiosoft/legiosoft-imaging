using LegioSoft.Imaging.Core;
using LegioSoft.Imaging.Skia.Core;
using LegioSoft.Imaging.Skia.Operations;

namespace LegioSoft.Imaging.Skia.Processors;

public class LegioImageSkiaProcessor : ILegioImageResizer, ILegioImageCropper, ILegioImageTransformer, ILegioImageFilter
{
    public byte[] Resize(byte[] imageData, int width, int height, LegioScaleMode mode, LegioResizeQuality quality = LegioResizeQuality.High)
    {
        return ImageResizer.Resize(imageData, width, height, mode, quality);
    }

    public Stream Resize(Stream inputStream, int width, int height, LegioScaleMode mode, LegioResizeQuality quality = LegioResizeQuality.High, Stream? outputStream = null)
    {
        var imageData = ReadStream(inputStream);
        var result = Resize(imageData, width, height, mode, quality);
        
        if (outputStream != null)
        {
            outputStream.Write(result, 0, result.Length);
            outputStream.Position = 0;
            return outputStream;
        }
        
        return new MemoryStream(result);
    }

    public byte[] Crop(byte[] imageData, int x, int y, int width, int height)
    {
        return ImageCropper.Crop(imageData, x, y, width, height);
    }

    public Stream Crop(Stream inputStream, int x, int y, int width, int height, Stream? outputStream = null)
    {
        var imageData = ReadStream(inputStream);
        var result = Crop(imageData, x, y, width, height);
        
        if (outputStream != null)
        {
            outputStream.Write(result, 0, result.Length);
            outputStream.Position = 0;
            return outputStream;
        }
        
        return new MemoryStream(result);
    }

    public byte[] Rotate(byte[] imageData, int degrees)
    {
        var bitmap = ImageLoader.LoadBitmap(imageData);
        var rotated = ImageTransformer.Rotate(bitmap, degrees);
        var format = FormatDetector.DetectFormat(imageData);
        return ImageSaver.SaveBitmap(rotated, format, 75);
    }

    public Stream Rotate(Stream inputStream, int degrees, Stream? outputStream = null)
    {
        var imageData = ReadStream(inputStream);
        var result = Rotate(imageData, degrees);
        
        if (outputStream != null)
        {
            outputStream.Write(result, 0, result.Length);
            outputStream.Position = 0;
            return outputStream;
        }
        
        return new MemoryStream(result);
    }

    public byte[] Flip(byte[] imageData, bool horizontal, bool vertical)
    {
        var bitmap = ImageLoader.LoadBitmap(imageData);
        var flipped = ImageTransformer.Flip(bitmap, horizontal, vertical);
        var format = FormatDetector.DetectFormat(imageData);
        return ImageSaver.SaveBitmap(flipped, format, 75);
    }

    public Stream Flip(Stream inputStream, bool horizontal, bool vertical, Stream? outputStream = null)
    {
        var imageData = ReadStream(inputStream);
        var result = Flip(imageData, horizontal, vertical);
        
        if (outputStream != null)
        {
            outputStream.Write(result, 0, result.Length);
            outputStream.Position = 0;
            return outputStream;
        }
        
        return new MemoryStream(result);
    }

    public byte[] ApplyGrayscale(byte[] imageData)
    {
        var bitmap = ImageLoader.LoadBitmap(imageData);
        var gray = ImageFilters.ApplyGrayscale(bitmap);
        var format = FormatDetector.DetectFormat(imageData);
        return ImageSaver.SaveBitmap(gray, format, 75);
    }

    public Stream ApplyGrayscale(Stream inputStream, Stream? outputStream = null)
    {
        var imageData = ReadStream(inputStream);
        var result = ApplyGrayscale(imageData);
        
        if (outputStream != null)
        {
            outputStream.Write(result, 0, result.Length);
            outputStream.Position = 0;
            return outputStream;
        }
        
        return new MemoryStream(result);
    }

    public byte[] ApplySepia(byte[] imageData)
    {
        var bitmap = ImageLoader.LoadBitmap(imageData);
        var sepia = ImageFilters.ApplySepia(bitmap);
        var format = FormatDetector.DetectFormat(imageData);
        return ImageSaver.SaveBitmap(sepia, format, 75);
    }

    public Stream ApplySepia(Stream inputStream, Stream? outputStream = null)
    {
        var imageData = ReadStream(inputStream);
        var result = ApplySepia(imageData);
        
        if (outputStream != null)
        {
            outputStream.Write(result, 0, result.Length);
            outputStream.Position = 0;
            return outputStream;
        }
        
        return new MemoryStream(result);
    }

    public byte[] ApplyBlur(byte[] imageData, int radius = 3)
    {
        var bitmap = ImageLoader.LoadBitmap(imageData);
        var blurred = ImageFilters.ApplyBlur(bitmap, radius);
        var format = FormatDetector.DetectFormat(imageData);
        return ImageSaver.SaveBitmap(blurred, format, 75);
    }

    public Stream ApplyBlur(Stream inputStream, int radius = 3, Stream? outputStream = null)
    {
        var imageData = ReadStream(inputStream);
        var result = ApplyBlur(imageData, radius);
        
        if (outputStream != null)
        {
            outputStream.Write(result, 0, result.Length);
            outputStream.Position = 0;
            return outputStream;
        }
        
        return new MemoryStream(result);
    }

    public byte[] ApplySharpen(byte[] imageData, int amount = 50)
    {
        var bitmap = ImageLoader.LoadBitmap(imageData);
        var sharpened = ImageFilters.ApplySharpen(bitmap, amount);
        var format = FormatDetector.DetectFormat(imageData);
        return ImageSaver.SaveBitmap(sharpened, format, 75);
    }

    public Stream ApplySharpen(Stream inputStream, int amount = 50, Stream? outputStream = null)
    {
        var imageData = ReadStream(inputStream);
        var result = ApplySharpen(imageData, amount);
        
        if (outputStream != null)
        {
            outputStream.Write(result, 0, result.Length);
            outputStream.Position = 0;
            return outputStream;
        }
        
        return new MemoryStream(result);
    }

    private static byte[] ReadStream(Stream stream)
    {
        using var ms = new MemoryStream();
        stream.CopyTo(ms);
        return ms.ToArray();
    }
}
