using System;

namespace LegioSoft.Imaging.WebP.Helpers;

public static class WebPHelper
{
    public static bool IsWebP(byte[] imageData)
    {
        if (imageData == null || imageData.Length < 12)
            return false;

        return imageData[0] == 0x52 && 
               imageData[1] == 0x49 && 
               imageData[2] == 0x46 && 
               imageData[3] == 0x46 &&
               imageData[8] == 0x57 && 
               imageData[9] == 0x45 && 
               imageData[10] == 0x66 && 
               imageData[11] == 0x50;
    }

    public static ImageFormat DetectImageFormat(byte[] imageData)
    {
        if (imageData == null || imageData.Length < 12)
            return ImageFormat.Unknown;

        if (IsWebP(imageData))
            return ImageFormat.WebP;

        if (imageData[0] == 0xFF && imageData[1] == 0xD8 && imageData[2] == 0xFF)
            return ImageFormat.Jpeg;

        if (imageData[0] == 0x42 && imageData[1] == 0x4D)
            return ImageFormat.Bmp;

        if (imageData[0] == 0x47 && imageData[1] == 0x49 && imageData[2] == 0x46 && imageData[3] == 0x38)
            return ImageFormat.Gif;

        if (imageData[0] == 0x89 && imageData[1] == 0x50 && imageData[2] == 0x4E && imageData[3] == 0x47)
            return ImageFormat.Png;

        return ImageFormat.Unknown;
    }
}

public enum ImageFormat
{
    Unknown,
    Png,
    Jpeg,
    WebP,
    Bmp,
    Gif
}
