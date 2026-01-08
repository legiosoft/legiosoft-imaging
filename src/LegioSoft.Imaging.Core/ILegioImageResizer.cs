using System.IO;

namespace LegioSoft.Imaging.Core;

public interface ILegioImageResizer
{
    byte[] Resize(byte[] imageData, int width, int height, LegioScaleMode mode, LegioResizeQuality quality = LegioResizeQuality.High);
    Stream Resize(Stream inputStream, int width, int height, LegioScaleMode mode, LegioResizeQuality quality = LegioResizeQuality.High, Stream? outputStream = null);
}

public interface ILegioImageCropper
{
    byte[] Crop(byte[] imageData, int x, int y, int width, int height);
    Stream Crop(Stream inputStream, int x, int y, int width, int height, Stream? outputStream = null);
}

public interface ILegioImageTransformer
{
    byte[] Rotate(byte[] imageData, int degrees);
    Stream Rotate(Stream inputStream, int degrees, Stream? outputStream = null);
    
    byte[] Flip(byte[] imageData, bool horizontal, bool vertical);
    Stream Flip(Stream inputStream, bool horizontal, bool vertical, Stream? outputStream = null);
}

public interface ILegioImageFilter
{
    byte[] ApplyGrayscale(byte[] imageData);
    Stream ApplyGrayscale(Stream inputStream, Stream? outputStream = null);
    
    byte[] ApplySepia(byte[] imageData);
    Stream ApplySepia(Stream inputStream, Stream? outputStream = null);
    
    byte[] ApplyBlur(byte[] imageData, int radius = 3);
    Stream ApplyBlur(Stream inputStream, int radius = 3, Stream? outputStream = null);
    
    byte[] ApplySharpen(byte[] imageData, int amount = 50);
    Stream ApplySharpen(Stream inputStream, int amount = 50, Stream? outputStream = null);
}
