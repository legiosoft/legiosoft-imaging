using LegioSoft.Imaging.Core.Enums;
using LegioSoft.Imaging.Skia.Core;

namespace LegioSoft.Imaging.Skia.Operations;

internal static class ImageConverter
{
    public static byte[] Convert(byte[] imageData, LegioImageFormat targetFormat, int quality = 75)
    {
        var bitmap = ImageLoader.LoadBitmap(imageData);
        return ImageSaver.SaveBitmap(bitmap, targetFormat, quality);
    }
}