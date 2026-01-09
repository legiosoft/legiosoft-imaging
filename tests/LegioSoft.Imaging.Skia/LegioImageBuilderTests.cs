using LegioSoft.Imaging.Core;
using LegioSoft.Imaging.Skia.Core;
using LegioSoft.Imaging.Skia.Operations;
using SkiaSharp;

namespace LegioSoft.Imaging.Skia.Tests;

public class LegioImageBuilderTests
{
    private static readonly string TestAssetsPath = Path.Combine(Directory.GetCurrentDirectory(), "TestAssets");

    private byte[] LoadTestImage(string filename)
    {
        var fullPath = Path.Combine(TestAssetsPath, filename);
        if (!File.Exists(fullPath))
            throw new FileNotFoundException($"Test image not found: {fullPath}");
        
        return File.ReadAllBytes(fullPath);
    }

    [Fact]
    public void Load_FromByteArray_ShouldCreateBuilder()
    {
        var imageData = LoadTestImage("example.png");
        
        var builder = LegioSoft.Imaging.Skia.LegioImageBuilder.Load(imageData);
        
        Assert.NotNull(builder);
    }

    [Fact]
    public void Load_FromNullByteArray_ShouldThrow()
    {
        Assert.Throws<ArgumentNullException>(() => LegioSoft.Imaging.Skia.LegioImageBuilder.Load((byte[])null!));
    }

    [Fact]
    public void Resize_ShouldSetResizeParameters()
    {
        var imageData = LoadTestImage("example.png");
        var builder = LegioSoft.Imaging.Skia.LegioImageBuilder.Load(imageData);
        
        var result = builder.Resize(50, 50);
        
        Assert.NotNull(result);
    }

    [Fact]
    public void Resize_ShouldThrowOnInvalidDimensions()
    {
        var imageData = LoadTestImage("example.png");
        var builder = LegioSoft.Imaging.Skia.LegioImageBuilder.Load(imageData);
        
        Assert.Throws<ArgumentException>(() => builder.Resize(0, 50));
        Assert.Throws<ArgumentException>(() => builder.Resize(50, 0));
    }

    [Fact]
    public void ResizeToWidth_ShouldSetWidth()
    {
        var imageData = LoadTestImage("example.png");
        var builder = LegioSoft.Imaging.Skia.LegioImageBuilder.Load(imageData);
        
        var result = builder.ResizeToWidth(50);
        
        Assert.NotNull(result);
    }

    [Fact]
    public void ResizeToHeight_ShouldSetHeight()
    {
        var imageData = LoadTestImage("example.png");
        var builder = LegioSoft.Imaging.Skia.LegioImageBuilder.Load(imageData);
        
        var result = builder.ResizeToHeight(50);
        
        Assert.NotNull(result);
    }

    [Fact]
    public void Scale_ShouldScaleImage()
    {
        var imageData = LoadTestImage("example.png");
        var builder = LegioSoft.Imaging.Skia.LegioImageBuilder.Load(imageData);
        
        var result = builder.Scale(0.5);
        
        Assert.NotNull(result);
    }

    [Fact]
    public void Scale_ShouldThrowOnInvalidFactor()
    {
        var imageData = LoadTestImage("example.png");
        var builder = LegioSoft.Imaging.Skia.LegioImageBuilder.Load(imageData);
        
        Assert.Throws<ArgumentException>(() => builder.Scale(0));
        Assert.Throws<ArgumentException>(() => builder.Scale(-0.5));
    }

    [Fact]
    public void Crop_ShouldSetCropParameters()
    {
        var imageData = LoadTestImage("example.png");
        var builder = LegioSoft.Imaging.Skia.LegioImageBuilder.Load(imageData);
        
        var result = builder.Crop(10, 10, 50, 50);
        
        Assert.NotNull(result);
    }

    [Fact]
    public void Crop_ShouldThrowOnInvalidParameters()
    {
        var imageData = LoadTestImage("example.png");
        var builder = LegioSoft.Imaging.Skia.LegioImageBuilder.Load(imageData);
        
        Assert.Throws<ArgumentException>(() => builder.Crop(0, 0, 50, 50));
        Assert.Throws<ArgumentException>(() => builder.Crop(10, 10, -50, 50));
    }

    [Fact]
    public void Rotate_ShouldSetRotation()
    {
        var imageData = LoadTestImage("example.png");
        var builder = LegioSoft.Imaging.Skia.LegioImageBuilder.Load(imageData);
        
        var result = builder.Rotate(90);
        
        Assert.NotNull(result);
    }

    [Fact]
    public void Rotate_ShouldThrowOnInvalidAngle()
    {
        var imageData = LoadTestImage("example.png");
        var builder = LegioSoft.Imaging.Skia.LegioImageBuilder.Load(imageData);
        
        Assert.Throws<ArgumentException>(() => builder.Rotate(45));
    }

    [Fact]
    public void Flip_ShouldSetFlipDirection()
    {
        var imageData = LoadTestImage("example.png");
        var builder = LegioSoft.Imaging.Skia.LegioImageBuilder.Load(imageData);
        
        var result = builder.Flip(horizontal: true, vertical: false);
        
        Assert.NotNull(result);
    }

    [Fact]
    public void Grayscale_ShouldApplyFilter()
    {
        var imageData = LoadTestImage("example.png");
        var builder = LegioSoft.Imaging.Skia.LegioImageBuilder.Load(imageData);
        
        var result = builder.Grayscale();
        
        Assert.NotNull(result);
    }

    [Fact]
    public void Sepia_ShouldApplyFilter()
    {
        var imageData = LoadTestImage("example.png");
        var builder = LegioSoft.Imaging.Skia.LegioImageBuilder.Load(imageData);
        
        var result = builder.Sepia();
        
        Assert.NotNull(result);
    }

    [Fact]
    public void Blur_ShouldApplyFilter()
    {
        var imageData = LoadTestImage("example.png");
        var builder = LegioSoft.Imaging.Skia.LegioImageBuilder.Load(imageData);
        
        var result = builder.Blur(5);
        
        Assert.NotNull(result);
    }

    [Fact]
    public void Blur_ShouldThrowOnInvalidRadius()
    {
        var imageData = LoadTestImage("example.png");
        var builder = LegioSoft.Imaging.Skia.LegioImageBuilder.Load(imageData);
        
        Assert.Throws<ArgumentException>(() => builder.Blur(0));
        Assert.Throws<ArgumentException>(() => builder.Blur(21));
    }

    [Fact]
    public void Sharpen_ShouldApplyFilter()
    {
        var imageData = LoadTestImage("example.png");
        var builder = LegioSoft.Imaging.Skia.LegioImageBuilder.Load(imageData);
        
        var result = builder.sharpen(60);
        
        Assert.NotNull(result);
    }

    [Fact]
    public void Sharpen_ShouldThrowOnInvalidAmount()
    {
        var imageData = LoadTestImage("example.png");
        var builder = LegioSoft.Imaging.Skia.LegioImageBuilder.Load(imageData);
        
        Assert.Throws<ArgumentException>(() => builder.sharpen(-1));
        Assert.Throws<ArgumentException>(() => builder.sharpen(101));
    }

    [Fact]
    public void Brightness_ShouldAdjustBrightness()
    {
        var imageData = LoadTestImage("example.png");
        var builder = LegioSoft.Imaging.Skia.LegioImageBuilder.Load(imageData);
        
        var result = builder.Brightness(30);
        
        Assert.NotNull(result);
    }

    [Fact]
    public void Brightness_ShouldThrowOnInvalidAmount()
    {
        var imageData = LoadTestImage("example.png");
        var builder = LegioSoft.Imaging.Skia.LegioImageBuilder.Load(imageData);
        
        Assert.Throws<ArgumentException>(() => builder.Brightness(-256));
        Assert.Throws<ArgumentException>(() => builder.Brightness(256));
    }

    [Fact]
    public void Contrast_ShouldAdjustContrast()
    {
        var imageData = LoadTestImage("example.png");
        var builder = LegioSoft.Imaging.Skia.LegioImageBuilder.Load(imageData);
        
        var result = builder.Contrast(20);
        
        Assert.NotNull(result);
    }

    [Fact]
    public void Contrast_ShouldThrowOnInvalidAmount()
    {
        var imageData = LoadTestImage("example.png");
        var builder = LegioSoft.Imaging.Skia.LegioImageBuilder.Load(imageData);
        
        Assert.Throws<ArgumentException>(() => builder.Contrast(-101));
        Assert.Throws<ArgumentException>(() => builder.Contrast(101));
    }

    [Fact]
    public void Invert_ShouldInvertColors()
    {
        var imageData = LoadTestImage("example.png");
        var builder = LegioSoft.Imaging.Skia.LegioImageBuilder.Load(imageData);
        
        var result = builder.Invert();
        
        Assert.NotNull(result);
    }

    [Fact]
    public void Quality_ShouldSetQuality()
    {
        var imageData = LoadTestImage("example.png");
        var builder = LegioSoft.Imaging.Skia.LegioImageBuilder.Load(imageData);
        
        var result = builder.Quality(85);
        
        Assert.NotNull(result);
    }

    [Fact]
    public void Quality_ShouldThrowOnInvalidValue()
    {
        var imageData = LoadTestImage("example.png");
        var builder = LegioSoft.Imaging.Skia.LegioImageBuilder.Load(imageData);
        
        Assert.Throws<ArgumentException>(() => builder.Quality(-1));
        Assert.Throws<ArgumentException>(() => builder.Quality(101));
    }

    [Fact]
    public void Save_ShouldWriteToFile()
    {
        var imageData = LoadTestImage("example.png");
        var builder = LegioSoft.Imaging.Skia.LegioImageBuilder.Load(imageData);
        
        var outputPath = Path.Combine(Directory.GetCurrentDirectory(), "output.jpg");
        builder.Save(outputPath, LegioSoft.Imaging.Core.LegioImageFormat.Jpeg, 90);
        
        Assert.True(File.Exists(outputPath));
        File.Delete(outputPath);
    }

    [Fact]
    public void Save_ShouldConvertFormat()
    {
        var imageData = LoadTestImage("example.png");
        var builder = LegioSoft.Imaging.Skia.LegioImageBuilder.Load(imageData);
        var jpegData = builder.SaveAs(LegioSoft.Imaging.Core.LegioImageFormat.Jpeg, 85);
        
        Assert.NotNull(jpegData);
        Assert.True(jpegData.Length > 0);
    }

    [Fact]
    public void SaveAsStream_ShouldReturnMemoryStream()
    {
        var imageData = LoadTestImage("example.png");
        var builder = LegioSoft.Imaging.Skia.LegioImageBuilder.Load(imageData);
        
        using var stream = builder.SaveAsStream(LegioSoft.Imaging.Core.LegioImageFormat.Png);
        
        Assert.NotNull(stream);
        Assert.True(stream.Length > 0);
    }

    [Fact]
    public void Save_WithQuality_ShouldRespectQuality()
    {
        var imageData = LoadTestImage("example.png");
        var builder = LegioSoft.Imaging.Skia.LegioImageBuilder.Load(imageData);
        
        var lowQuality = builder.Quality(50).SaveAs(LegioSoft.Imaging.Core.LegioImageFormat.Jpeg, 85);
        var highQuality = builder.Quality(95).SaveAs(LegioSoft.Imaging.Core.LegioImageFormat.Jpeg, 95);
        
        Assert.True(lowQuality.Length > highQuality.Length);
    }

    [Fact]
    public void SaveAs_ShouldReturnBytes()
    {
        var imageData = LoadTestImage("example.png");
        var builder = LegioSoft.Imaging.Skia.LegioImageBuilder.Load(imageData);
        var result = builder.SaveAs(LegioSoft.Imaging.Core.LegioImageFormat.Jpeg, 80);
        
        Assert.NotNull(result);
        Assert.True(result.Length > 0);
    }

    [Fact]
    public void SaveAsFormat_ShouldSupportFormats(LegioImageFormat format)
    {
        var imageData = LoadTestImage("example.png");
        var builder = LegioSoft.Imaging.Skia.LegioImageBuilder.Load(imageData);
        var result = builder.SaveAs(format, 80);
        
        Assert.NotNull(result);
        Assert.True(result.Length > 0);
    }

    [Fact]
    public void GetInfo_ShouldReturnMetadata()
    {
        var imageData = LoadTestImage("example.png");
        var builder = LegioSoft.Imaging.Skia.LegioImageBuilder.Load(imageData);
        
        var info = builder.GetInfo();
        
        Assert.NotNull(info);
        Assert.Equal(100, info.Width);
        Assert.Equal(100, info.Height);
    }
}
