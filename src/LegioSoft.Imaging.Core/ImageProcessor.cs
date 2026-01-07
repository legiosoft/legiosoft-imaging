using System;
using System.IO;

namespace LegioSoft.Imaging.Core;

public class ImageProcessor
{
    public ImageInfo GetImageInfo(byte[] imageData)
    {
        if (imageData == null || imageData.Length < 8)
            throw new ArgumentException("Invalid image data", nameof(imageData));

        var format = DetectFormat(imageData);
        
        return new ImageInfo
        {
            Format = format,
            ByteSize = imageData.Length,
            HasAlpha = format == ImageFormat.Png || format == ImageFormat.WebP
        };
    }

    public byte[] ConvertToFormat(byte[] imageData, ImageFormat targetFormat, int quality = 75)
    {
        throw new NotImplementedException("This method should be implemented in Skia package");
    }

    public byte[] Scale(byte[] imageData, int width, int height, ScaleMode mode, ResizeQuality quality = ResizeQuality.High)
    {
        throw new NotImplementedException("This method should be implemented in Skia package");
    }

    public byte[] Crop(byte[] imageData, int x, int y, int width, int height)
    {
        throw new NotImplementedException("This method should be implemented in Skia package");
    }

    public byte[] Rotate(byte[] imageData, int degrees)
    {
        throw new NotImplementedException("This method should be implemented in Skia package");
    }

    public byte[] Flip(byte[] imageData, bool horizontal, bool vertical)
    {
        throw new NotImplementedException("This method should be implemented in Skia package");
    }

    public System.Threading.Tasks.Task<byte[]> ConvertToFormatAsync(byte[] imageData, ImageFormat targetFormat, int quality = 75)
    {
        return System.Threading.Tasks.Task.Run(() => ConvertToFormat(imageData, targetFormat, quality));
    }

    public System.Threading.Tasks.Task<byte[]> ScaleAsync(byte[] imageData, int width, int height, ScaleMode mode)
    {
        return System.Threading.Tasks.Task.Run(() => Scale(imageData, width, height, mode));
    }

    private static ImageFormat DetectFormat(byte[] imageData)
    {
        if (imageData.Length < 12) return ImageFormat.Png;

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
