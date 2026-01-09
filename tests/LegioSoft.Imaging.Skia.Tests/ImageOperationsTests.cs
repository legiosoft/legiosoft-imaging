using LegioSoft.Imaging.Core;
using LegioSoft.Imaging.Skia;
using SkiaSharp;
using Xunit;

namespace LegioSoft.Imaging.Skia.Tests;

public class ImageOperationsTests
{
    private readonly byte[] _testImageData = new byte[]
    {
        0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A, 0x00, 0x00, 0x00, 0x0D, 0x49, 0x48, 0x44, 0x52,
        0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x01, 0x08, 0x02, 0x00, 0x00, 0x00, 0x90, 0x77, 0x53,
        0xDE, 0x00, 0x00, 0x00, 0x0C, 0x49, 0x44, 0x41, 0x54, 0x08, 0xD7, 0x63, 0xF8, 0xCF, 0xC0,
        0x00, 0x00, 0x03, 0x01, 0x01, 0x00, 0x00, 0x00, 0x01, 0x00, 0x01, 0x00, 0x00, 0x00, 0x01,
        0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
        0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
        0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
        0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x49, 0x45, 0x4E, 0x44, 0xAE,
        0x42, 0x60, 0x82
    };

    private byte[] CreateTestPng(int width, int height)
    {
        using var bitmap = new SKBitmap(width, height);
        using var canvas = new SKCanvas(bitmap);
        canvas.Clear(SKColors.Blue);
        canvas.Flush();

        using var image = SKImage.FromBitmap(bitmap);
        using var data = image.Encode(SKEncodedImageFormat.Png, 100);
        return data.ToArray();
    }

    [Fact]
    public void LoadBitmap_ShouldLoadPng()
    {
        var result = ImageOperations.LoadBitmap(_testImageData);
        
        Assert.NotNull(result);
        Assert.Equal(1, result.Width);
        Assert.Equal(1, result.Height);
    }

    [Fact]
    public void LoadBitmap_ShouldThrowOnInvalidData()
    {
        Assert.Throws<InvalidOperationException>(() => ImageOperations.LoadBitmap(new byte[] { 0x00, 0x00 }));
    }

    [Fact]
    public void ResizeBitmap_ShouldResize()
    {
        var imageData = CreateTestPng(100, 100);
        var bitmap = ImageOperations.LoadBitmap(imageData);
        
        var result = ImageOperations.ResizeBitmap(bitmap, 50, 50, LegioResizeQuality.High);
        
        Assert.NotNull(result);
        Assert.Equal(50, result.Width);
        Assert.Equal(50, result.Height);
    }

    [Fact]
    public void ResizeBitmap_ShouldThrowOnInvalidDimensions()
    {
        var imageData = CreateTestPng(100, 100);
        var bitmap = ImageOperations.LoadBitmap(imageData);
        
        Assert.Throws<ArgumentException>(() => ImageOperations.ResizeBitmap(bitmap, 0, 50, LegioResizeQuality.High));
        Assert.Throws<ArgumentException>(() => ImageOperations.ResizeBitmap(bitmap, 50, -1, LegioResizeQuality.High));
    }

    [Theory]
    [InlineData(LegioResizeQuality.Low)]
    [InlineData(LegioResizeQuality.Medium)]
    [InlineData(LegioResizeQuality.High)]
    [InlineData(LegioResizeQuality.Maximum)]
    public void ResizeBitmap_ShouldMapQuality(LegioResizeQuality inputQuality)
    {
        var imageData = CreateTestPng(100, 100);
        var bitmap = ImageOperations.LoadBitmap(imageData);
        
        var result = ImageOperations.ResizeBitmap(bitmap, 50, 50, inputQuality);
        
        Assert.NotNull(result);
    }

    [Fact]
    public void CropBitmap_ShouldCrop()
    {
        var imageData = CreateTestPng(100, 100);
        var bitmap = ImageOperations.LoadBitmap(imageData);
        
        var result = ImageOperations.CropBitmap(bitmap, 10, 10, 50, 50);
        
        Assert.NotNull(result);
        Assert.Equal(50, result.Width);
        Assert.Equal(50, result.Height);
    }

    [Fact]
    public void CropBitmap_ShouldThrowOnInvalidParameters()
    {
        var imageData = CreateTestPng(100, 100);
        var bitmap = ImageOperations.LoadBitmap(imageData);
        
        Assert.Throws<ArgumentException>(() => ImageOperations.CropBitmap(bitmap, -10, 10, 50, 50));
        Assert.Throws<ArgumentException>(() => ImageOperations.CropBitmap(bitmap, 10, 10, -50, 50));
        Assert.Throws<ArgumentException>(() => ImageOperations.CropBitmap(bitmap, 90, 90, 50, 50));
    }

    [Fact]
    public void Rotate_ShouldRotate180Degrees()
    {
        var imageData = CreateTestPng(100, 50);
        var bitmap = ImageOperations.LoadBitmap(imageData);
        
        var result = ImageOperations.Rotate(bitmap, 180);
        
        Assert.NotNull(result);
        Assert.Equal(100, result.Width);
        Assert.Equal(50, result.Height);
    }

    [Fact]
    public void Rotate_ShouldRotate270Degrees()
    {
        var imageData = CreateTestPng(100, 50);
        var bitmap = ImageOperations.LoadBitmap(imageData);
        
        var result = ImageOperations.Rotate(bitmap, 270);
        
        Assert.NotNull(result);
        Assert.Equal(50, result.Width);
        Assert.Equal(100, result.Height);
    }

    [Fact]
    public void Rotate_ShouldThrowOnInvalidDegrees()
    {
        var imageData = CreateTestPng(100, 100);
        var bitmap = ImageOperations.LoadBitmap(imageData);
        
        Assert.Throws<ArgumentException>(() => ImageOperations.Rotate(bitmap, 45));
        Assert.Throws<ArgumentException>(() => ImageOperations.Rotate(bitmap, -90));
    }

    [Fact]
    public void Flip_ShouldFlipHorizontal()
    {
        var imageData = CreateTestPng(100, 50);
        var bitmap = ImageOperations.LoadBitmap(imageData);
        
        var result = ImageOperations.Flip(bitmap, horizontal: true, vertical: false);
        
        Assert.NotNull(result);
        Assert.Equal(100, result.Width);
        Assert.Equal(50, result.Height);
    }

    [Fact]
    public void Flip_ShouldFlipVertical()
    {
        var imageData = CreateTestPng(100, 50);
        var bitmap = ImageOperations.LoadBitmap(imageData);
        
        var result = ImageOperations.Flip(bitmap, horizontal: false, vertical: true);
        
        Assert.NotNull(result);
        Assert.Equal(100, result.Width);
        Assert.Equal(50, result.Height);
    }

    [Fact]
    public void Flip_ShouldFlipBoth()
    {
        var imageData = CreateTestPng(100, 50);
        var bitmap = ImageOperations.LoadBitmap(imageData);
        
        var result = ImageOperations.Flip(bitmap, horizontal: true, vertical: true);
        
        Assert.NotNull(result);
        Assert.Equal(100, result.Width);
        Assert.Equal(50, result.Height);
    }

    [Fact]
    public void ApplyGrayscale_ShouldConvertToGrayscale()
    {
        var imageData = CreateTestPng(100, 100);
        var bitmap = ImageOperations.LoadBitmap(imageData);
        
        var result = ImageOperations.ApplyGrayscale(bitmap);
        
        Assert.NotNull(result);
        Assert.Equal(100, result.Width);
        Assert.Equal(100, result.Height);
    }

    [Fact]
    public void ApplySepia_ShouldConvertToSepia()
    {
        var imageData = CreateTestPng(100, 100);
        var bitmap = ImageOperations.LoadBitmap(imageData);
        
        var result = ImageOperations.ApplySepia(bitmap);
        
        Assert.NotNull(result);
        Assert.Equal(100, result.Width);
        Assert.Equal(100, result.Height);
    }

    [Fact]
    public void ApplyBlur_ShouldApplyBlur()
    {
        var imageData = CreateTestPng(100, 100);
        var bitmap = ImageOperations.LoadBitmap(imageData);
        
        var result = ImageOperations.ApplyBlur(bitmap, 5);
        
        Assert.NotNull(result);
        Assert.Equal(100, result.Width);
        Assert.Equal(100, result.Height);
    }

    [Fact]
    public void ApplySharpen_ShouldApplySharpen()
    {
        var imageData = CreateTestPng(100, 100);
        var bitmap = ImageOperations.LoadBitmap(imageData);
        
        var result = ImageOperations.ApplySharpen(bitmap, 50);
        
        Assert.NotNull(result);
        Assert.Equal(100, result.Width);
        Assert.Equal(100, result.Height);
    }

    [Fact]
    public void ApplyBrightness_ShouldAdjustBrightness()
    {
        var imageData = CreateTestPng(100, 100);
        var bitmap = ImageOperations.LoadBitmap(imageData);
        
        var result = ImageOperations.ApplyBrightness(bitmap, 30);
        
        Assert.NotNull(result);
        Assert.Equal(100, result.Width);
        Assert.Equal(100, result.Height);
    }

    [Fact]
    public void ApplyContrast_ShouldAdjustContrast()
    {
        var imageData = CreateTestPng(100, 100);
        var bitmap = ImageOperations.LoadBitmap(imageData);
        
        var result = ImageOperations.ApplyContrast(bitmap, 20);
        
        Assert.NotNull(result);
        Assert.Equal(100, result.Width);
        Assert.Equal(100, result.Height);
    }

    [Fact]
    public void ApplyInvert_ShouldInvertColors()
    {
        var imageData = CreateTestPng(100, 100);
        var bitmap = ImageOperations.LoadBitmap(imageData);
        
        var result = ImageOperations.ApplyInvert(bitmap);
        
        Assert.NotNull(result);
        Assert.Equal(100, result.Width);
        Assert.Equal(100, result.Height);
    }

    [Theory]
    [InlineData(LegioImageFormat.Png)]
    [InlineData(LegioImageFormat.Jpeg)]
    [InlineData(LegioImageFormat.WebP)]
    public void SaveBitmap_ShouldConvertFormat(LegioImageFormat inputFormat)
    {
        var imageData = CreateTestPng(100, 100);
        var bitmap = ImageOperations.LoadBitmap(imageData);
        
        var result = ImageOperations.SaveBitmap(bitmap, inputFormat, 75);
        
        Assert.NotNull(result);
        Assert.True(result.Length > 0);
    }

    [Theory]
    [InlineData(LegioImageFormat.Bmp)]
    [InlineData(LegioImageFormat.Gif)]
    public void SaveBitmap_ShouldThrowOnUnsupportedFormat(LegioImageFormat inputFormat)
    {
        var imageData = CreateTestPng(100, 100);
        var bitmap = ImageOperations.LoadBitmap(imageData);
        
        Assert.Throws<NotSupportedException>(() => ImageOperations.SaveBitmap(bitmap, inputFormat, 75));
    }

    [Fact]
    public void DetectFormat_ShouldDetectPng()
    {
        var imageData = CreateTestPng(100, 100);
        
        var format = ImageOperations.DetectFormat(imageData);
        
        Assert.Equal(LegioImageFormat.Png, format);
    }

    [Fact]
    public void Resize_ShouldResizeAndSave()
    {
        var imageData = CreateTestPng(100, 100);
        
        var result = ImageOperations.Resize(imageData, 50, 50, LegioScaleMode.Fit, LegioResizeQuality.High);
        
        Assert.NotNull(result);
        Assert.True(result.Length > 0);
    }

    [Fact]
    public void Crop_ShouldCropAndSave()
    {
        var imageData = CreateTestPng(100, 100);
        
        var result = ImageOperations.Crop(imageData, 10, 10, 50, 50);
        
        Assert.NotNull(result);
        Assert.True(result.Length > 0);
    }

    [Fact]
    public void Convert_ShouldConvertFormat()
    {
        var imageData = CreateTestPng(100, 100);
        
        var result = ImageOperations.Convert(imageData, LegioImageFormat.Jpeg, 85);
        
        Assert.NotNull(result);
        Assert.True(result.Length > 0);
    }
}
