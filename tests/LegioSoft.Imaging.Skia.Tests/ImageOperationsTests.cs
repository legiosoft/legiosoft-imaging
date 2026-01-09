using LegioSoft.Imaging.Core;
using LegioSoft.Imaging.Skia.Core;
using LegioSoft.Imaging.Skia.Operations;
using SkiaSharp;
using Xunit;

namespace LegioSoft.Imaging.Skia.Tests;

public class ImageOperationsTests
{
    private static readonly string TestAssetsPath = Path.Combine(AppContext.BaseDirectory, "TestAssets");

    private byte[] LoadTestImage(string filename)
    {
        var fullPath = Path.Combine(TestAssetsPath, filename);
        return File.ReadAllBytes(fullPath);
    }

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
        var imageData = LoadTestImage("example.png");
        var result = Core.ImageLoader.LoadBitmap(imageData);
        
        Assert.NotNull(result);
        Assert.True(result.Width > 0);
        Assert.True(result.Height > 0);
    }

    [Fact]
    public void LoadBitmap_ShouldLoadJpeg()
    {
        var imageData = LoadTestImage("example.jpeg");
        var result = Core.ImageLoader.LoadBitmap(imageData);
        
        Assert.NotNull(result);
        Assert.True(result.Width > 0);
        Assert.True(result.Height > 0);
    }

    [Fact]
    public void LoadBitmap_ShouldLoadBmp()
    {
        var imageData = LoadTestImage("example.bmp");
        var result = Core.ImageLoader.LoadBitmap(imageData);
        
        Assert.NotNull(result);
        Assert.True(result.Width > 0);
        Assert.True(result.Height > 0);
    }

    [Fact]
    public void LoadBitmap_ShouldThrowOnInvalidData()
    {
        Assert.Throws<InvalidOperationException>(() => Core.ImageLoader.LoadBitmap(new byte[] { 0x00, 0x00 }));
    }

    [Fact]
    public void SaveBitmap_ShouldSavePng()
    {
        var bitmap = new SKBitmap(100, 100);
        var result = Core.ImageSaver.SaveBitmap(bitmap, LegioImageFormat.Png, 90);
        
        Assert.NotNull(result);
        Assert.True(result.Length > 0);
    }

    [Fact]
    public void ResizeBitmap_ShouldResize()
    {
        var bitmap = new SKBitmap(200, 100);
        var result = Operations.ImageResizer.ResizeBitmap(bitmap, 150, 75, LegioResizeQuality.High);
        
        Assert.Equal(150, result.Width);
        Assert.Equal(75, result.Height);
    }

    [Fact]
    public void CropBitmap_ShouldCrop()
    {
        var bitmap = new SKBitmap(200, 100);
        var result = Operations.ImageCropper.CropBitmap(bitmap, 25, 25, 50, 50);
        
        Assert.Equal(50, result.Width);
        Assert.Equal(50, result.Height);
    }

    [Fact]
    public void Transform_Rotate_ShouldRotate90()
    {
        var bitmap = new SKBitmap(100, 200);
        var result = Operations.ImageTransformer.Rotate(bitmap, 90);
        
        Assert.Equal(200, result.Width);
        Assert.Equal(100, result.Height);
    }

    [Fact]
    public void Transform_Flip_ShouldFlip()
    {
        var bitmap = new SKBitmap(100, 100);
        var result = Operations.ImageTransformer.Flip(bitmap, true, false);
        
        Assert.Equal(100, result.Width);
        Assert.Equal(100, result.Height);
    }

    [Fact]
    public void Filter_Grayscale_ShouldApply()
    {
        var bitmap = new SKBitmap(100, 100);
        var result = Operations.ImageFilters.ApplyGrayscale(bitmap);
        
        Assert.Equal(100, result.Width);
        Assert.Equal(100, result.Height);
    }

    [Fact]
    public void Filter_Sepia_ShouldApply()
    {
        var bitmap = new SKBitmap(100, 100);
        var result = Operations.ImageFilters.ApplySepia(bitmap);
        
        Assert.Equal(100, result.Width);
        Assert.Equal(100, result.Height);
    }

    [Fact]
    public void Filter_Blur_ShouldApply()
    {
        var bitmap = new SKBitmap(100, 100);
        var result = Operations.ImageFilters.ApplyBlur(bitmap, 5);
        
        Assert.Equal(100, result.Width);
        Assert.Equal(100, result.Height);
    }

    [Fact]
    public void Filter_Sharpen_ShouldApply()
    {
        var bitmap = new SKBitmap(100, 100);
        var result = Operations.ImageFilters.ApplySharpen(bitmap, 50);
        
        Assert.Equal(100, result.Width);
        Assert.Equal(100, result.Height);
    }

    [Fact]
    public void ColorAdjustments_Brightness_ShouldApply()
    {
        var bitmap = new SKBitmap(100, 100);
        var result = Operations.ImageColorAdjustments.ApplyBrightness(bitmap, 30);
        
        Assert.Equal(100, result.Width);
        Assert.Equal(100, result.Height);
    }

    [Fact]
    public void ColorAdjustments_Contrast_ShouldApply()
    {
        var bitmap = new SKBitmap(100, 100);
        var result = Operations.ImageColorAdjustments.ApplyContrast(bitmap, 20);
        
        Assert.Equal(100, result.Width);
        Assert.Equal(100, result.Height);
    }

    [Fact]
    public void ColorAdjustments_Invert_ShouldApply()
    {
        var bitmap = new SKBitmap(100, 100);
        var result = Operations.ImageColorAdjustments.ApplyInvert(bitmap);
        
        Assert.Equal(100, result.Width);
        Assert.Equal(100, result.Height);
    }
}
