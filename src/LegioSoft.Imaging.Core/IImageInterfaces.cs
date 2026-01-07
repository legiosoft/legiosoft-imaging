namespace LegioSoft.Imaging.Core;

public interface IImageEncoder
{
    byte[] Encode(System.IO.Stream stream, ImageFormat format, int quality = 75);
}

public interface IImageDecoder
{
    byte[] Decode(byte[] imageData, out ImageFormat format);
    ImageInfo GetImageInfo(byte[] imageData);
}

public interface IImageTransformer
{
    System.IO.Stream Resize(System.IO.Stream inputStream, int width, int height, ScaleMode mode, ResizeQuality quality, System.IO.Stream outputStream, ImageFormat format, int saveQuality = 75);
    System.IO.Stream Crop(System.IO.Stream inputStream, int x, int y, int width, int height, System.IO.Stream outputStream, ImageFormat format, int saveQuality = 75);
    System.IO.Stream Rotate(System.IO.Stream inputStream, int degrees, System.IO.Stream outputStream, ImageFormat format, int saveQuality = 75);
    System.IO.Stream Flip(System.IO.Stream inputStream, bool horizontal, System.IO.Stream outputStream, ImageFormat format, int saveQuality = 75);
    System.IO.Stream ApplyGrayscale(System.IO.Stream inputStream, System.IO.Stream outputStream, ImageFormat format, int saveQuality = 75);
    System.IO.Stream ApplySepia(System.IO.Stream inputStream, System.IO.Stream outputStream, ImageFormat format, int saveQuality = 75);
    System.IO.Stream ApplyBlur(System.IO.Stream inputStream, System.IO.Stream outputStream, ImageFormat format, int saveQuality = 75);
}
