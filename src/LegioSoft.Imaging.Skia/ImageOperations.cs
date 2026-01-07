using System;
using System.IO;
using LegioSoft.Imaging.Core;
using SkiaSharp;

namespace LegioSoft.Imaging.Skia;

public class ImageOperations
{
    public static byte[] Resize(byte[] imageData, int width, int height, ScaleMode mode = ScaleMode.Fit, ResizeQuality quality = ResizeQuality.High)
    {
        var bitmap = LoadBitmap(imageData);
        var resized = ResizeBitmap(bitmap, width, height, quality);
        return SaveBitmap(resized, DetectFormat(imageData), 75);
    }

    public static SKBitmap ResizeBitmap(SKBitmap bitmap, int width, int height, ResizeQuality quality = ResizeQuality.High)
    {
        if (width <= 0 || height <= 0)
            throw new ArgumentException("Width and height must be positive", nameof(width));

        var filterQuality = quality switch
        {
            ResizeQuality.Low => SKFilterQuality.Low,
            ResizeQuality.Medium => SKFilterQuality.Medium,
            ResizeQuality.High => SKFilterQuality.High,
            ResizeQuality.Maximum => SKFilterQuality.High,
            _ => SKFilterQuality.High
        };

        var scaledInfo = new SKImageInfo(width, height, bitmap.ColorType, bitmap.AlphaType);
        var scaledBitmap = new SKBitmap(scaledInfo);
        bitmap.ScalePixels(scaledBitmap, filterQuality);

        return scaledBitmap;
    }

    public static byte[] Crop(byte[] imageData, int x, int y, int width, int height)
    {
        var bitmap = LoadBitmap(imageData);
        var cropped = CropBitmap(bitmap, x, y, width, height);
        return SaveBitmap(cropped, DetectFormat(imageData), 75);
    }

    public static SKBitmap CropBitmap(SKBitmap bitmap, int x, int y, int width, int height)
    {
        if (x < 0 || y < 0 || width <= 0 || height <= 0)
            throw new ArgumentException("Invalid crop parameters");

        if (x + width > bitmap.Width || y + height > bitmap.Height)
            throw new ArgumentException("Crop area extends beyond image bounds");

        var croppedBitmap = new SKBitmap(width, height);

        using (var canvas = new SKCanvas(croppedBitmap))
        {
            canvas.DrawBitmap(bitmap, new SKRect(x, y, x + width, y + height), new SKPaint());
        }

        return croppedBitmap;
    }

    public static byte[] Convert(byte[] imageData, ImageFormat targetFormat, int quality = 75)
    {
        var bitmap = LoadBitmap(imageData);
        return SaveBitmap(bitmap, targetFormat, quality);
    }

    public static SKBitmap Rotate(SKBitmap bitmap, int degrees)
    {
        if (degrees != 90 && degrees != 180 && degrees != 270)
            throw new ArgumentException("Rotation must be 90, 180, or 270 degrees", nameof(degrees));

        var radians = degrees * Math.PI / 180;
        var rotatedInfo = new SKImageInfo(
            bitmap.Height, 
            bitmap.Width, 
            bitmap.ColorType, 
            bitmap.AlphaType);

        var rotatedBitmap = new SKBitmap(rotatedInfo);

        using (var canvas = new SKCanvas(rotatedBitmap))
        using (var paint = new SKPaint())
        {
            var matrix = SKMatrix.CreateRotation((float)radians);
            var center = new SKPoint(rotatedBitmap.Width / 2f, rotatedBitmap.Height / 2f);
            canvas.Translate(center.X, center.Y);
            canvas.Concat(ref matrix);
            canvas.DrawBitmap(bitmap, -bitmap.Width / 2f, -bitmap.Height / 2f, paint);
        }

        return rotatedBitmap;
    }

    public static SKBitmap Flip(SKBitmap bitmap, bool horizontal, bool vertical)
    {
        var flippedInfo = bitmap.Info;
        var flippedBitmap = new SKBitmap(flippedInfo);

        using (var canvas = new SKCanvas(flippedBitmap))
        {
            var scaleX = horizontal ? -1f : 1f;
            var scaleY = vertical ? -1f : 1f;
            
            canvas.Scale(scaleX, scaleY);
            canvas.Translate(
                horizontal ? -bitmap.Width : 0, 
                vertical ? -bitmap.Height : 0);
            
            canvas.DrawBitmap(bitmap, 0, 0);
        }

        return flippedBitmap;
    }

    public static SKBitmap ApplyGrayscale(SKBitmap bitmap)
    {
        var grayscaleInfo = bitmap.Info;
        var grayscaleBitmap = new SKBitmap(grayscaleInfo);
        var grayscalePixels = grayscaleBitmap.GetPixels();

        for (var y = 0; y < bitmap.Height; y++)
        {
            for (var x = 0; x < bitmap.Width; x++)
            {
                var pixel = bitmap.GetPixel(x, y);
                var gray = (byte)(0.299 * pixel.Red + 0.587 * pixel.Green + 0.114 * pixel.Blue);
                grayscaleBitmap.SetPixel(x, y, new SKColor(gray, gray, gray, pixel.Alpha));
            }
        }

        return grayscaleBitmap;
    }

    public static SKBitmap ApplySepia(SKBitmap bitmap)
    {
        var sepiaInfo = bitmap.Info;
        var sepiaBitmap = new SKBitmap(sepiaInfo);

        for (var y = 0; y < bitmap.Height; y++)
        {
            for (var x = 0; x < bitmap.Width; x++)
            {
                var pixel = bitmap.GetPixel(x, y);
                var r = Math.Min(255, (int)(pixel.Red * 0.393 + pixel.Green * 0.769 + pixel.Blue * 0.189));
                var g = Math.Min(255, (int)(pixel.Red * 0.349 + pixel.Green * 0.686 + pixel.Blue * 0.168));
                var b = Math.Min(255, (int)(pixel.Red * 0.272 + pixel.Green * 0.534 + pixel.Blue * 0.131));
                sepiaBitmap.SetPixel(x, y, new SKColor((byte)r, (byte)g, (byte)b, pixel.Alpha));
            }
        }

        return sepiaBitmap;
    }

    public static SKBitmap ApplyBlur(SKBitmap bitmap)
    {
        var blurInfo = bitmap.Info;
        var blurBitmap = new SKBitmap(blurInfo);
        
        using (var paint = new SKPaint())
        using (var filter = SKImageFilter.CreateBlur(5, 5))
        {
            paint.ImageFilter = filter;
            
            using (var canvas = new SKCanvas(blurBitmap))
            {
                canvas.DrawBitmap(bitmap, 0, 0, paint);
            }
        }

        return blurBitmap;
    }

    public static SKBitmap LoadBitmap(byte[] imageData)
    {
        using var ms = new MemoryStream(imageData);
        var codec = SKCodec.Create(ms);
        
        if (codec == null)
            throw new InvalidOperationException("Unable to decode image data. Unsupported format.");

        var bitmap = new SKBitmap(codec.Info);
        codec.GetPixels(bitmap.Info, bitmap.GetPixels());
        codec.Dispose();

        return bitmap;
    }

    public static byte[] SaveBitmap(SKBitmap bitmap, ImageFormat format, int quality = 75)
    {
        using var ms = new MemoryStream();
        var skiaFormat = format switch
        {
            ImageFormat.Jpeg => SKEncodedImageFormat.Jpeg,
            ImageFormat.WebP => SKEncodedImageFormat.Webp,
            ImageFormat.Png => SKEncodedImageFormat.Png,
            ImageFormat.Bmp => SKEncodedImageFormat.Bmp,
            ImageFormat.Gif => SKEncodedImageFormat.Gif,
            _ => SKEncodedImageFormat.Png
        };
        bitmap.Encode(ms, skiaFormat, quality);
        return ms.ToArray();
    }

    public static ImageFormat DetectFormat(byte[] imageData)
    {
        if (imageData.Length < 8) return ImageFormat.Png;

        if (imageData[0] == 0x52 && imageData[1] == 0x49 && imageData[2] == 0x46 && imageData[3] == 0x46 &&
            imageData[8] == 0x57 && imageData[9] == 0x45 && imageData[10] == 0x66 && imageData[11] == 0x50)
            return ImageFormat.WebP;

        if (imageData[0] == 0xFF && imageData[1] == 0xD8 && imageData[2] == 0xFF)
            return ImageFormat.Jpeg;

        if (imageData[0] == 0x42 && imageData[1] == 0x4D)
            return ImageFormat.Bmp;

        if (imageData[0] == 0x47 && imageData[1] == 0x49 && imageData[2] == 0x46 && imageData[3] == 0x38)
            return ImageFormat.Gif;

        if (imageData[0] == 0x89 && imageData[1] == 0x50 && imageData[2] == 0x4E && imageData[3] == 0x47)
            return ImageFormat.Png;

        return ImageFormat.Png;
    }
}
