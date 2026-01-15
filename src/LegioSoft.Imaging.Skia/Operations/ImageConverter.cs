using LegioSoft.Imaging.Core.Enums;

namespace LegioSoft.Imaging.Skia.Operations;

internal static class ImageConverter
{
    public static byte[] Convert(byte[] imageData, LegioImageFormat targetFormat, int quality = 75)
    {
        var bitmap = Core.ImageLoader.LoadBitmap(imageData);
        return Core.ImageSaver.SaveBitmap(bitmap, targetFormat, quality);
    }
}
