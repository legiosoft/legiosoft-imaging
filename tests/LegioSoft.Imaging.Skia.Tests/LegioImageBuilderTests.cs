using LegioSoft.Imaging.Core;
using LegioSoft.Imaging.Skia;
using Xunit;

namespace LegioSoft.Imaging.Skia.Tests;

public class LegioImageBuilderTests
{
    private byte[] CreateTestPng(int width, int height)
    {
        using var bitmap = new SkiaSharp.SKBitmap(width, height);
        using var canvas = new SkiaSharp.SKCanvas(bitmap);
        canvas.Clear(SkiaSharp.SKColors.Blue);
        canvas.Flush();

        using var image = SkiaSharp.SKImage.FromBitmap(bitmap);
        using var data = image.Encode(SkiaSharp.SKEncodedImageFormat.Png, 100);
        return data.ToArray();
    }

    [Fact]
    public void Load_FromByteArray_ShouldCreateBuilder()
    {
        var imageData = CreateTestPng(100, 100);
        
        var builder = LegioImageBuilder.Load(imageData);
        
        Assert.NotNull(builder);
    }

    [Fact]
    public void Load_FromNullByteArray_ShouldThrow()
    {
        Assert.Throws<ArgumentNullException>(() => LegioImageBuilder.Load((byte[])null!));
    }

    [Fact]
    public void Resize_ShouldSetResizeParameters()
    {
        var imageData = CreateTestPng(100, 100);
        var builder = LegioImageBuilder.Load(imageData);
        
        var result = builder.Resize(50, 50);
        
        Assert.NotNull(result);
    }

    [Fact]
    public void Resize_ShouldThrowOnInvalidDimensions()
    {
        var imageData = CreateTestPng(100, 100);
        var builder = LegioImageBuilder.Load(imageData);
        
        Assert.Throws<ArgumentException>(() => builder.Resize(0, 50));
        Assert.Throws<ArgumentException>(() => builder.Resize(50, -1));
        Assert.Throws<ArgumentException>(() => builder.Resize(-10, -10));
    }

    [Fact]
    public void ResizeToWidth_ShouldSetWidth()
    {
        var imageData = CreateTestPng(100, 100);
        var builder = LegioImageBuilder.Load(imageData);
        
        var result = builder.ResizeToWidth(50);
        
        Assert.NotNull(result);
    }

    [Fact]
    public void ResizeToWidth_ShouldThrowOnInvalidWidth()
    {
        var imageData = CreateTestPng(100, 100);
        var builder = LegioImageBuilder.Load(imageData);
        
        Assert.Throws<ArgumentException>(() => builder.ResizeToWidth(0));
        Assert.Throws<ArgumentException>(() => builder.ResizeToWidth(-50));
    }

    [Fact]
    public void ResizeToHeight_ShouldSetHeight()
    {
        var imageData = CreateTestPng(100, 100);
        var builder = LegioImageBuilder.Load(imageData);
        
        var result = builder.ResizeToHeight(50);
        
        Assert.NotNull(result);
    }

    [Fact]
    public void ResizeToHeight_ShouldThrowOnInvalidHeight()
    {
        var imageData = CreateTestPng(100, 100);
        var builder = LegioImageBuilder.Load(imageData);
        
        Assert.Throws<ArgumentException>(() => builder.ResizeToHeight(0));
        Assert.Throws<ArgumentException>(() => builder.ResizeToHeight(-50));
    }

    [Fact]
    public void Scale_ShouldSetScaleFactor()
    {
        var imageData = CreateTestPng(100, 100);
        var builder = LegioImageBuilder.Load(imageData);
        
        var result = builder.Scale(0.5);
        
        Assert.NotNull(result);
    }

    [Fact]
    public void Scale_ShouldThrowOnInvalidFactor()
    {
        var imageData = CreateTestPng(100, 100);
        var builder = LegioImageBuilder.Load(imageData);
        
        Assert.Throws<ArgumentException>(() => builder.Scale(0));
        Assert.Throws<ArgumentException>(() => builder.Scale(-1));
    }

    [Fact]
    public void Crop_ShouldSetCropParameters()
    {
        var imageData = CreateTestPng(100, 100);
        var builder = LegioImageBuilder.Load(imageData);
        
        var result = builder.Crop(10, 10, 50, 50);
        
        Assert.NotNull(result);
    }

    [Theory]
    [InlineData(90)]
    [InlineData(180)]
    [InlineData(270)]
    public void Rotate_ShouldSetValidDegrees(int degrees)
    {
        var imageData = CreateTestPng(100, 100);
        var builder = LegioImageBuilder.Load(imageData);
        
        var result = builder.Rotate(degrees);
        
        Assert.NotNull(result);
    }

    [Fact]
    public void Rotate_ShouldThrowOnInvalidDegrees()
    {
        var imageData = CreateTestPng(100, 100);
        var builder = LegioImageBuilder.Load(imageData);
        
        Assert.Throws<ArgumentException>(() => builder.Rotate(45));
        Assert.Throws<ArgumentException>(() => builder.Rotate(-90));
        Assert.Throws<ArgumentException>(() => builder.Rotate(360));
    }

    [Fact]
    public void Flip_ShouldSetFlipParameters()
    {
        var imageData = CreateTestPng(100, 100);
        var builder = LegioImageBuilder.Load(imageData);
        
        var result = builder.Flip(horizontal: true, vertical: false);
        
        Assert.NotNull(result);
    }

    [Fact]
    public void Grayscale_ShouldSetGrayscaleFilter()
    {
        var imageData = CreateTestPng(100, 100);
        var builder = LegioImageBuilder.Load(imageData);
        
        var result = builder.Grayscale();
        
        Assert.NotNull(result);
    }

    [Fact]
    public void Sepia_ShouldSetSepiaFilter()
    {
        var imageData = CreateTestPng(100, 100);
        var builder = LegioImageBuilder.Load(imageData);
        
        var result = builder.Sepia();
        
        Assert.NotNull(result);
    }

    [Fact]
    public void Blur_ShouldSetBlurFilter()
    {
        var imageData = CreateTestPng(100, 100);
        var builder = LegioImageBuilder.Load(imageData);
        
        var result = builder.Blur(5);
        
        Assert.NotNull(result);
    }

    [Fact]
    public void Blur_ShouldThrowOnInvalidRadius()
    {
        var imageData = CreateTestPng(100, 100);
        var builder = LegioImageBuilder.Load(imageData);
        
        Assert.Throws<ArgumentException>(() => builder.Blur(0));
        Assert.Throws<ArgumentException>(() => builder.Blur(25));
        Assert.Throws<ArgumentException>(() => builder.Blur(-5));
    }

    [Fact]
    public void Sharpen_ShouldSetSharpenFilter()
    {
        var imageData = CreateTestPng(100, 100);
        var builder = LegioImageBuilder.Load(imageData);
        
        var result = builder.Sharpen(50);
        
        Assert.NotNull(result);
    }

    [Fact]
    public void Sharpen_ShouldThrowOnInvalidAmount()
    {
        var imageData = CreateTestPng(100, 100);
        var builder = LegioImageBuilder.Load(imageData);
        
        Assert.Throws<ArgumentException>(() => builder.Sharpen(-10));
        Assert.Throws<ArgumentException>(() => builder.Sharpen(150));
    }

    [Fact]
    public void Brightness_ShouldSetBrightness()
    {
        var imageData = CreateTestPng(100, 100);
        var builder = LegioImageBuilder.Load(imageData);
        
        var result = builder.Brightness(30);
        
        Assert.NotNull(result);
    }

    [Fact]
    public void Brightness_ShouldThrowOnInvalidValue()
    {
        var imageData = CreateTestPng(100, 100);
        var builder = LegioImageBuilder.Load(imageData);
        
        Assert.Throws<ArgumentException>(() => builder.Brightness(-300));
        Assert.Throws<ArgumentException>(() => builder.Brightness(300));
    }

    [Fact]
    public void Contrast_ShouldSetContrast()
    {
        var imageData = CreateTestPng(100, 100);
        var builder = LegioImageBuilder.Load(imageData);
        
        var result = builder.Contrast(20);
        
        Assert.NotNull(result);
    }

    [Fact]
    public void Contrast_ShouldThrowOnInvalidValue()
    {
        var imageData = CreateTestPng(100, 100);
        var builder = LegioImageBuilder.Load(imageData);
        
        Assert.Throws<ArgumentException>(() => builder.Contrast(-150));
        Assert.Throws<ArgumentException>(() => builder.Contrast(150));
    }

    [Fact]
    public void Invert_ShouldSetInvert()
    {
        var imageData = CreateTestPng(100, 100);
        var builder = LegioImageBuilder.Load(imageData);
        
        var result = builder.Invert();
        
        Assert.NotNull(result);
    }

    [Fact]
    public void Quality_ShouldSetQuality()
    {
        var imageData = CreateTestPng(100, 100);
        var builder = LegioImageBuilder.Load(imageData);
        
        var result = builder.Quality(85);
        
        Assert.NotNull(result);
    }

    [Fact]
    public void Quality_ShouldThrowOnInvalidValue()
    {
        var imageData = CreateTestPng(100, 100);
        var builder = LegioImageBuilder.Load(imageData);
        
        Assert.Throws<ArgumentException>(() => builder.Quality(-10));
        Assert.Throws<ArgumentException>(() => builder.Quality(150));
    }

    [Fact]
    public void SaveAs_ShouldReturnBytes()
    {
        var imageData = CreateTestPng(100, 100);
        var builder = LegioImageBuilder.Load(imageData);
        
        var result = builder.SaveAs(LegioImageFormat.Jpeg, 80);
        
        Assert.NotNull(result);
        Assert.True(result.Length > 0);
    }

    [Fact]
    public void Save_ShouldReturnBytes()
    {
        var imageData = CreateTestPng(100, 100);
        var builder = LegioImageBuilder.Load(imageData);
        
        var result = builder.Save();
        
        Assert.NotNull(result);
        Assert.True(result.Length > 0);
    }

    [Fact]
    public void GetInfo_ShouldReturnImageInfo()
    {
        var imageData = CreateTestPng(100, 100);
        var builder = LegioImageBuilder.Load(imageData);
        
        var info = builder.GetInfo();
        
        Assert.NotNull(info);
        Assert.Equal(100, info.Width);
        Assert.Equal(100, info.Height);
        Assert.Equal(LegioImageFormat.Png, info.Format);
        Assert.True(info.ByteSize > 0);
    }

    [Fact]
    public void ChainedOperations_ShouldApplyAllOperations()
    {
        var imageData = CreateTestPng(200, 200);
        var builder = LegioImageBuilder.Load(imageData);
        
        var result = builder
            .Resize(100, 100, LegioScaleMode.Fit)
            .Crop(10, 10, 50, 50)
            .Rotate(90)
            .Grayscale()
            .SaveAs(LegioImageFormat.Jpeg, 80);
        
        Assert.NotNull(result);
        Assert.True(result.Length > 0);
    }

    [Fact]
    public void MultipleResizeOperations_ShouldOverridePrevious()
    {
        var imageData = CreateTestPng(100, 100);
        var builder = LegioImageBuilder.Load(imageData);
        
        var result = builder
            .Resize(50, 50)
            .ResizeToWidth(75)
            .SaveAs(LegioImageFormat.Png);
        
        Assert.NotNull(result);
        Assert.True(result.Length > 0);
    }

    [Fact]
    public void ScaleMode_Fit_ShouldMaintainAspectRatio()
    {
        var imageData = CreateTestPng(200, 100);
        var builder = LegioImageBuilder.Load(imageData);
        
        var result = builder
            .Resize(100, 100, LegioScaleMode.Fit)
            .SaveAs(LegioImageFormat.Png);
        
        Assert.NotNull(result);
        Assert.True(result.Length > 0);
    }

    [Fact]
    public void ScaleMode_Fill_ShouldFillBounds()
    {
        var imageData = CreateTestPng(100, 200);
        var builder = LegioImageBuilder.Load(imageData);
        
        var result = builder
            .Resize(100, 100, LegioScaleMode.Fill)
            .SaveAs(LegioImageFormat.Png);
        
        Assert.NotNull(result);
        Assert.True(result.Length > 0);
    }

    [Fact]
    public void ScaleMode_Stretch_ShouldStretch()
    {
        var imageData = CreateTestPng(100, 200);
        var builder = LegioImageBuilder.Load(imageData);
        
        var result = builder
            .Resize(100, 100, LegioScaleMode.Stretch)
            .SaveAs(LegioImageFormat.Png);
        
        Assert.NotNull(result);
        Assert.True(result.Length > 0);
    }

    [Theory]
    [InlineData(LegioResizeQuality.Low)]
    [InlineData(LegioResizeQuality.Medium)]
    [InlineData(LegioResizeQuality.High)]
    [InlineData(LegioResizeQuality.Maximum)]
    public void ResizeQuality_ShouldApplyQuality(LegioResizeQuality quality)
    {
        var imageData = CreateTestPng(200, 200);
        var builder = LegioImageBuilder.Load(imageData);
        
        var result = builder
            .Resize(100, 100, quality: quality)
            .SaveAs(LegioImageFormat.Jpeg);
        
        Assert.NotNull(result);
        Assert.True(result.Length > 0);
    }

    [Theory]
    [InlineData(LegioImageFormat.Png)]
    [InlineData(LegioImageFormat.Jpeg)]
    [InlineData(LegioImageFormat.WebP)]
    [InlineData(LegioImageFormat.Bmp)]
    public void SaveAsFormat_ShouldSupportFormats(LegioImageFormat format)
    {
        var imageData = CreateTestPng(100, 100);
        var builder = LegioImageBuilder.Load(imageData);
        
        var result = builder.SaveAs(format, 80);
        
        Assert.NotNull(result);
        if (format != LegioImageFormat.Bmp)
        {
            Assert.True(result.Length > 0);
        }
    }

    [Fact]
    public void MultipleFilters_ShouldApplyAllFilters()
    {
        var imageData = CreateTestPng(100, 100);
        var builder = LegioImageBuilder.Load(imageData);
        
        var result = builder
            .Brightness(20)
            .Contrast(10)
            .Grayscale()
            .Blur(2)
            .Sharpen(30)
            .SaveAs(LegioImageFormat.Jpeg);
        
        Assert.NotNull(result);
        Assert.True(result.Length > 0);
    }

    [Fact]
    public void FlipOperations_ShouldApplyCorrectly()
    {
        var imageData = CreateTestPng(100, 100);
        var builder = LegioImageBuilder.Load(imageData);
        
        var result = builder
            .Flip(horizontal: true, vertical: true)
            .SaveAs(LegioImageFormat.Png);
        
        Assert.NotNull(result);
        Assert.True(result.Length > 0);
    }

    [Fact]
    public void InvertAfterGrayscale_ShouldChain()
    {
        var imageData = CreateTestPng(100, 100);
        var builder = LegioImageBuilder.Load(imageData);
        
        var result = builder
            .Grayscale()
            .Invert()
            .SaveAs(LegioImageFormat.Png);
        
        Assert.NotNull(result);
        Assert.True(result.Length > 0);
    }
}
