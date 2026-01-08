using System;
using System.IO;
using LegioSoft.Imaging.Core;

namespace LegioSoft.Imaging.Skia;

public class LegioImageSkiaProcessor : ILegioImageResizer, ILegioImageCropper, ILegioImageTransformer, ILegioImageFilter
{
    public byte[] Resize(byte[] imageData, int width, int height, LegioScaleMode mode, LegioResizeQuality quality = LegioResizeQuality.High)
    {
        throw new NotImplementedException("Resize implementation will be added in ImageOperations.cs");
    }

    public Stream Resize(Stream inputStream, int width, int height, LegioScaleMode mode, LegioResizeQuality quality = LegioResizeQuality.High, Stream? outputStream = null)
    {
        throw new NotImplementedException("Resize implementation will be added in ImageOperations.cs");
    }

    public byte[] Crop(byte[] imageData, int x, int y, int width, int height)
    {
        throw new NotImplementedException("Crop implementation will be added in ImageOperations.cs");
    }

    public Stream Crop(Stream inputStream, int x, int y, int width, int height, Stream? outputStream = null)
    {
        throw new NotImplementedException("Crop implementation will be added in ImageOperations.cs");
    }

    public byte[] Rotate(byte[] imageData, int degrees)
    {
        throw new NotImplementedException("Rotate implementation will be added in ImageOperations.cs");
    }

    public Stream Rotate(Stream inputStream, int degrees, Stream? outputStream = null)
    {
        throw new NotImplementedException("Rotate implementation will be added in ImageOperations.cs");
    }

    public byte[] Flip(byte[] imageData, bool horizontal, bool vertical)
    {
        throw new NotImplementedException("Flip implementation will be added in ImageOperations.cs");
    }

    public Stream Flip(Stream inputStream, bool horizontal, bool vertical, Stream? outputStream = null)
    {
        throw new NotImplementedException("Flip implementation will be added in ImageOperations.cs");
    }

    public byte[] ApplyGrayscale(byte[] imageData)
    {
        throw new NotImplementedException("Grayscale implementation will be added in ImageOperations.cs");
    }

    public Stream ApplyGrayscale(Stream inputStream, Stream? outputStream = null)
    {
        throw new NotImplementedException("Grayscale implementation will be added in ImageOperations.cs");
    }

    public byte[] ApplySepia(byte[] imageData)
    {
        throw new NotImplementedException("Sepia implementation will be added in ImageOperations.cs");
    }

    public Stream ApplySepia(Stream inputStream, Stream? outputStream = null)
    {
        throw new NotImplementedException("Sepia implementation will be added in ImageOperations.cs");
    }

    public byte[] ApplyBlur(byte[] imageData, int radius = 3)
    {
        throw new NotImplementedException("Blur implementation will be added in ImageOperations.cs");
    }

    public Stream ApplyBlur(Stream inputStream, int radius = 3, Stream? outputStream = null)
    {
        throw new NotImplementedException("Blur implementation will be added in ImageOperations.cs");
    }

    public byte[] ApplySharpen(byte[] imageData, int amount = 50)
    {
        throw new NotImplementedException("Sharpen implementation will be added in ImageOperations.cs");
    }

    public Stream ApplySharpen(Stream inputStream, int amount = 50, Stream? outputStream = null)
    {
        throw new NotImplementedException("Sharpen implementation will be added in ImageOperations.cs");
    }
}
