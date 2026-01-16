using LegioSoft.Imaging.Core.Classes;
using LegioSoft.Imaging.Core.Enums;
using LegioSoft.Imaging.Core.Interfaces;
using LegioSoft.Imaging.Skia.Core;
using LegioSoft.Imaging.Skia.Operations;

// ReSharper disable RedundantArgumentDefaultValue

namespace LegioSoft.Imaging.Skia.Processors;

internal class LegioImageSkiaProcessor : ILegioImageResizer, ILegioImageCropper, ILegioImageTransformer,
    ILegioImageFilter
{
    public byte[] Resize(byte[] imageData, int width, int height, LegioScaleMode mode,
        LegioResizeQuality quality = LegioResizeQuality.High)
    {
        return ImageResizer.Resize(imageData, width, height, mode, quality);
    }

    public Stream Resize(Stream inputStream, int width, int height, LegioScaleMode mode,
        LegioResizeQuality quality = LegioResizeQuality.High, Stream? outputStream = null)
    {
        var format = FormatDetector.DetectFormat(inputStream);
        if (inputStream.CanSeek)
            inputStream.Position = 0;

        using var bitmap = ImageLoader.LoadBitmap(inputStream);
        var (targetWidth, targetHeight) =
            ImageResizer.CalculateTargetDimensions(bitmap.Width, bitmap.Height, width, height, mode);
        using var resized = ImageResizer.ResizeBitmap(bitmap, targetWidth, targetHeight, quality);

        if (outputStream != null)
        {
            ImageSaver.SaveBitmap(resized, outputStream, format, 90);
            if (outputStream.CanSeek)
                outputStream.Position = 0;
            return outputStream;
        }

        return new MemoryStream(ImageSaver.SaveBitmap(resized, format, 90));
    }

    public byte[] Crop(byte[] imageData, int x, int y, int width, int height)
    {
        return ImageCropper.Crop(imageData, x, y, width, height);
    }

    public Stream Crop(Stream inputStream, int x, int y, int width, int height, Stream? outputStream = null)
    {
        var format = FormatDetector.DetectFormat(inputStream);
        if (inputStream.CanSeek)
            inputStream.Position = 0;

        using var bitmap = ImageLoader.LoadBitmap(inputStream);
        using var cropped = ImageCropper.CropBitmap(bitmap, x, y, width, height);

        if (outputStream != null)
        {
            ImageSaver.SaveBitmap(cropped, outputStream, format, 90);
            if (outputStream.CanSeek)
                outputStream.Position = 0;
            return outputStream;
        }

        return new MemoryStream(ImageSaver.SaveBitmap(cropped, format, 90));
    }

    public byte[] Rotate(byte[] imageData, int degrees)
    {
        using var bitmap = ImageLoader.LoadBitmap(imageData);
        using var rotated = ImageTransformer.Rotate(bitmap, degrees);
        var format = FormatDetector.DetectFormat(imageData);
        return ImageSaver.SaveBitmap(rotated, format, 90);
    }

    public Stream Rotate(Stream inputStream, int degrees, Stream? outputStream = null)
    {
        var format = FormatDetector.DetectFormat(inputStream);
        if (inputStream.CanSeek)
            inputStream.Position = 0;

        using var bitmap = ImageLoader.LoadBitmap(inputStream);
        using var rotated = ImageTransformer.Rotate(bitmap, degrees);

        if (outputStream != null)
        {
            ImageSaver.SaveBitmap(rotated, outputStream, format, 90);
            if (outputStream.CanSeek)
                outputStream.Position = 0;
            return outputStream;
        }

        return new MemoryStream(ImageSaver.SaveBitmap(rotated, format, 90));
    }

    public byte[] Flip(byte[] imageData, bool horizontal, bool vertical)
    {
        using var bitmap = ImageLoader.LoadBitmap(imageData);
        using var flipped = ImageTransformer.Flip(bitmap, horizontal, vertical);
        var format = FormatDetector.DetectFormat(imageData);
        return ImageSaver.SaveBitmap(flipped, format, 90);
    }

    public Stream Flip(Stream inputStream, bool horizontal, bool vertical, Stream? outputStream = null)
    {
        var format = FormatDetector.DetectFormat(inputStream);
        if (inputStream.CanSeek)
            inputStream.Position = 0;

        using var bitmap = ImageLoader.LoadBitmap(inputStream);
        using var flipped = ImageTransformer.Flip(bitmap, horizontal, vertical);

        if (outputStream != null)
        {
            ImageSaver.SaveBitmap(flipped, outputStream, format, 90);
            if (outputStream.CanSeek)
                outputStream.Position = 0;
            return outputStream;
        }

        return new MemoryStream(ImageSaver.SaveBitmap(flipped, format, 90));
    }

    public byte[] ApplyGrayscale(byte[] imageData)
    {
        using var bitmap = ImageLoader.LoadBitmap(imageData);
        using var gray = ImageFilters.ApplyGrayscale(bitmap);
        var format = FormatDetector.DetectFormat(imageData);
        return ImageSaver.SaveBitmap(gray, format, 90);
    }

    public Stream ApplyGrayscale(Stream inputStream, Stream? outputStream = null)
    {
        var format = FormatDetector.DetectFormat(inputStream);
        if (inputStream.CanSeek)
            inputStream.Position = 0;

        using var bitmap = ImageLoader.LoadBitmap(inputStream);
        using var gray = ImageFilters.ApplyGrayscale(bitmap);

        if (outputStream != null)
        {
            ImageSaver.SaveBitmap(gray, outputStream, format, 90);
            if (outputStream.CanSeek)
                outputStream.Position = 0;
            return outputStream;
        }

        return new MemoryStream(ImageSaver.SaveBitmap(gray, format, 90));
    }

    public byte[] ApplySepia(byte[] imageData)
    {
        using var bitmap = ImageLoader.LoadBitmap(imageData);
        using var sepia = ImageFilters.ApplySepia(bitmap);
        var format = FormatDetector.DetectFormat(imageData);
        return ImageSaver.SaveBitmap(sepia, format, 90);
    }

    public Stream ApplySepia(Stream inputStream, Stream? outputStream = null)
    {
        var format = FormatDetector.DetectFormat(inputStream);
        if (inputStream.CanSeek)
            inputStream.Position = 0;

        using var bitmap = ImageLoader.LoadBitmap(inputStream);
        using var sepia = ImageFilters.ApplySepia(bitmap);

        if (outputStream != null)
        {
            ImageSaver.SaveBitmap(sepia, outputStream, format, 90);
            if (outputStream.CanSeek)
                outputStream.Position = 0;
            return outputStream;
        }

        return new MemoryStream(ImageSaver.SaveBitmap(sepia, format, 90));
    }

    public byte[] ApplyBlur(byte[] imageData, int radius = 3)
    {
        using var bitmap = ImageLoader.LoadBitmap(imageData);
        using var blurred = ImageFilters.ApplyBlur(bitmap, radius);
        var format = FormatDetector.DetectFormat(imageData);
        return ImageSaver.SaveBitmap(blurred, format, 90);
    }

    public Stream ApplyBlur(Stream inputStream, int radius = 3, Stream? outputStream = null)
    {
        var format = FormatDetector.DetectFormat(inputStream);
        if (inputStream.CanSeek)
            inputStream.Position = 0;

        using var bitmap = ImageLoader.LoadBitmap(inputStream);
        using var blurred = ImageFilters.ApplyBlur(bitmap, radius);

        if (outputStream != null)
        {
            ImageSaver.SaveBitmap(blurred, outputStream, format, 90);
            if (outputStream.CanSeek)
                outputStream.Position = 0;
            return outputStream;
        }

        return new MemoryStream(ImageSaver.SaveBitmap(blurred, format, 90));
    }

    public byte[] ApplySharpen(byte[] imageData, int amount = 50)
    {
        using var bitmap = ImageLoader.LoadBitmap(imageData);
        using var sharpened = ImageFilters.ApplySharpen(bitmap, amount);
        var format = FormatDetector.DetectFormat(imageData);
        return ImageSaver.SaveBitmap(sharpened, format, 90);
    }

    public Stream ApplySharpen(Stream inputStream, int amount = 50, Stream? outputStream = null)
    {
        var format = FormatDetector.DetectFormat(inputStream);
        if (inputStream.CanSeek)
            inputStream.Position = 0;

        using var bitmap = ImageLoader.LoadBitmap(inputStream);
        using var sharpened = ImageFilters.ApplySharpen(bitmap, amount);

        if (outputStream != null)
        {
            ImageSaver.SaveBitmap(sharpened, outputStream, format, 90);
            if (outputStream.CanSeek)
                outputStream.Position = 0;
            return outputStream;
        }

        return new MemoryStream(ImageSaver.SaveBitmap(sharpened, format, 90));
    }
}