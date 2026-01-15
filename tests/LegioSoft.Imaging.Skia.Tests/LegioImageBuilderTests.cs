using LegioSoft.Imaging.Core.Enums;
using SkiaSharp;
using Xunit;

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
    public void Load_FromByteArray_ShouldCreateBuilder()
    {
        var imageData = LoadTestImage("example.png");
        
        var builder = LegioImageBuilder.Load(imageData);
        
        Assert.NotNull(builder);
    }

    [Fact]
    public void Load_FromNullByteArray_ShouldThrow()
    {
        Assert.Throws<ArgumentNullException>(() => LegioImageBuilder.Load((byte[])null!));
    }

    [Fact]
    public void Resize_ShouldExecuteSuccessfully()
    {
        var imageData = LoadTestImage("example.png");
        var result = LegioImageBuilder.Load(imageData)
            .Resize(50, 50)
            .SaveAs(LegioImageFormat.Png);

        Assert.NotNull(result);
        Assert.True(result.Length > 0);
    }

    [Fact]
    public void Resize_ShouldThrowOnInvalidDimensions()
    {
        var imageData = LoadTestImage("example.png");
        var builder = LegioImageBuilder.Load(imageData);

        Assert.Throws<ArgumentException>(() => builder.Resize(0, 50));
        Assert.Throws<ArgumentException>(() => builder.Resize(50, 0));
        Assert.Throws<ArgumentException>(() => builder.Resize(-50, 50));
    }

    [Fact]
    public void ResizeToWidth_ShouldExecuteSuccessfully()
    {
        var imageData = LoadTestImage("example.png");
        var result = LegioImageBuilder.Load(imageData)
            .ResizeToWidth(50)
            .SaveAs(LegioImageFormat.Png);

        Assert.NotNull(result);
        Assert.True(result.Length > 0);
    }

    [Fact]
    public void ResizeToHeight_ShouldExecuteSuccessfully()
    {
        var imageData = LoadTestImage("example.png");
        var result = LegioImageBuilder.Load(imageData)
            .ResizeToHeight(50)
            .SaveAs(LegioImageFormat.Png);

        Assert.NotNull(result);
        Assert.True(result.Length > 0);
    }

    [Fact]
    public void Scale_ShouldExecuteSuccessfully()
    {
        var imageData = LoadTestImage("example.png");
        var result = LegioImageBuilder.Load(imageData)
            .Scale(0.5)
            .SaveAs(LegioImageFormat.Png);

        Assert.NotNull(result);
        Assert.True(result.Length > 0);
    }

    [Fact]
    public void Scale_ShouldThrowOnInvalidFactor()
    {
        var imageData = LoadTestImage("example.png");
        var builder = LegioImageBuilder.Load(imageData);

        Assert.Throws<ArgumentException>(() => builder.Scale(0));
        Assert.Throws<ArgumentException>(() => builder.Scale(-0.5));
    }

    [Fact]
    public void Crop_ShouldExecuteSuccessfully()
    {
        var imageData = LoadTestImage("example.png");
        var result = LegioImageBuilder.Load(imageData)
            .Crop(10, 10, 50, 50)
            .SaveAs(LegioImageFormat.Png);

        Assert.NotNull(result);
        Assert.True(result.Length > 0);
    }

    [Fact]
    public void Crop_ShouldThrowOnInvalidParameters()
    {
        var imageData = LoadTestImage("example.png");
        var builder = LegioImageBuilder.Load(imageData);

        Assert.Throws<ArgumentException>(() => builder.Crop(10, 10, 0, 50));
        Assert.Throws<ArgumentException>(() => builder.Crop(10, 10, 50, 0));
        Assert.Throws<ArgumentException>(() => builder.Crop(10, 10, -50, 50));
    }

    [Fact]
    public void Rotate_ShouldExecuteSuccessfully()
    {
        var imageData = LoadTestImage("example.png");
        var result = LegioImageBuilder.Load(imageData)
            .Rotate(90)
            .SaveAs(LegioImageFormat.Png);

        Assert.NotNull(result);
        Assert.True(result.Length > 0);
    }

    [Fact]
    public void Rotate_ShouldThrowOnInvalidAngle()
    {
        var imageData = LoadTestImage("example.png");
        var builder = LegioImageBuilder.Load(imageData);

        Assert.Throws<ArgumentException>(() => builder.Rotate(45));
    }

    [Fact]
    public void Flip_ShouldExecuteSuccessfully()
    {
        var imageData = LoadTestImage("example.png");
        var result = LegioImageBuilder.Load(imageData)
            .Flip(horizontal: true, vertical: false)
            .SaveAs(LegioImageFormat.Png);

        Assert.NotNull(result);
        Assert.True(result.Length > 0);
    }

    [Fact]
    public void Grayscale_ShouldExecuteSuccessfully()
    {
        var imageData = LoadTestImage("example.png");
        var result = LegioImageBuilder.Load(imageData)
            .Grayscale()
            .SaveAs(LegioImageFormat.Png);

        Assert.NotNull(result);
        Assert.True(result.Length > 0);
    }

    [Fact]
    public void Sepia_ShouldExecuteSuccessfully()
    {
        var imageData = LoadTestImage("example.png");
        var result = LegioImageBuilder.Load(imageData)
            .Sepia()
            .SaveAs(LegioImageFormat.Png);

        Assert.NotNull(result);
        Assert.True(result.Length > 0);
    }

    [Fact]
    public void Blur_ShouldExecuteSuccessfully()
    {
        var imageData = LoadTestImage("example.png");
        var result = LegioImageBuilder.Load(imageData)
            .Blur(5)
            .SaveAs(LegioImageFormat.Png);

        Assert.NotNull(result);
        Assert.True(result.Length > 0);
    }

    [Fact]
    public void Blur_ShouldThrowOnInvalidRadius()
    {
        var imageData = LoadTestImage("example.png");
        var builder = LegioImageBuilder.Load(imageData);

        Assert.Throws<ArgumentException>(() => builder.Blur(0));
        Assert.Throws<ArgumentException>(() => builder.Blur(21));
    }

    [Fact]
    public void Sharpen_ShouldExecuteSuccessfully()
    {
        var imageData = LoadTestImage("example.png");
        var result = LegioImageBuilder.Load(imageData)
            .Sharpen(60)
            .SaveAs(LegioImageFormat.Png);

        Assert.NotNull(result);
        Assert.True(result.Length > 0);
    }

    [Fact]
    public void Sharpen_ShouldThrowOnInvalidAmount()
    {
        var imageData = LoadTestImage("example.png");
        var builder = LegioImageBuilder.Load(imageData);

        Assert.Throws<ArgumentException>(() => builder.Sharpen(-1));
        Assert.Throws<ArgumentException>(() => builder.Sharpen(101));
    }

    [Fact]
    public void Brightness_ShouldExecuteSuccessfully()
    {
        var imageData = LoadTestImage("example.png");
        var result = LegioImageBuilder.Load(imageData)
            .Brightness(30)
            .SaveAs(LegioImageFormat.Png);

        Assert.NotNull(result);
        Assert.True(result.Length > 0);
    }

    [Fact]
    public void Brightness_ShouldThrowOnInvalidAmount()
    {
        var imageData = LoadTestImage("example.png");
        var builder = LegioImageBuilder.Load(imageData);

        Assert.Throws<ArgumentException>(() => builder.Brightness(-256));
        Assert.Throws<ArgumentException>(() => builder.Brightness(256));
    }

    [Fact]
    public void Contrast_ShouldExecuteSuccessfully()
    {
        var imageData = LoadTestImage("example.png");
        var result = LegioImageBuilder.Load(imageData)
            .Contrast(20)
            .SaveAs(LegioImageFormat.Png);

        Assert.NotNull(result);
        Assert.True(result.Length > 0);
    }

    [Fact]
    public void Contrast_LowValue_ShouldExecuteSuccessfully()
    {
        var imageData = LoadTestImage("example.png");
        var result = LegioImageBuilder.Load(imageData)
            .Contrast(-30)
            .SaveAs(LegioImageFormat.Png);

        Assert.NotNull(result);
        Assert.True(result.Length > 0);
    }

    [Fact]
    public void Contrast_ShouldThrowOnInvalidAmount()
    {
        var imageData = LoadTestImage("example.png");
        var builder = LegioImageBuilder.Load(imageData);

        Assert.Throws<ArgumentException>(() => builder.Contrast(-101));
        Assert.Throws<ArgumentException>(() => builder.Contrast(101));
    }

    [Fact]
    public void Invert_ShouldExecuteSuccessfully()
    {
        var imageData = LoadTestImage("example.png");
        var result = LegioImageBuilder.Load(imageData)
            .Invert()
            .SaveAs(LegioImageFormat.Png);

        Assert.NotNull(result);
        Assert.True(result.Length > 0);
    }

    [Fact]
    public void Quality_ShouldExecuteSuccessfully()
    {
        var imageData = LoadTestImage("example.png");
        var result = LegioImageBuilder.Load(imageData)
            .Quality(85)
            .SaveAs(LegioImageFormat.Png);

        Assert.NotNull(result);
        Assert.True(result.Length > 0);
    }

    [Fact]
    public void Quality_ShouldThrowOnInvalidValue()
    {
        var imageData = LoadTestImage("example.png");
        var builder = LegioImageBuilder.Load(imageData);

        Assert.Throws<ArgumentException>(() => builder.Quality(-1));
        Assert.Throws<ArgumentException>(() => builder.Quality(101));
    }

    [Fact]
    public void Save_ShouldWriteToFile()
    {
        var imageData = LoadTestImage("example.png");
        var outputPath = Path.Combine(Directory.GetCurrentDirectory(), "output.jpg");
        LegioImageBuilder.Load(imageData).Save(outputPath, LegioImageFormat.Jpeg, 90);

        Assert.True(File.Exists(outputPath));
        File.Delete(outputPath);
    }

    [Fact]
    public void Save_ShouldConvertFormat()
    {
        var imageData = LoadTestImage("example.png");
        var jpegData = LegioImageBuilder.Load(imageData).SaveAs(LegioImageFormat.Jpeg, 85);

        Assert.NotNull(jpegData);
        Assert.True(jpegData.Length > 0);
    }

    [Fact]
    public void SaveAsStream_ShouldReturnMemoryStream()
    {
        var imageData = LoadTestImage("example.png");
        using var stream = LegioImageBuilder.Load(imageData).SaveAsStream(LegioImageFormat.Png);

        Assert.NotNull(stream);
        Assert.True(stream.Length > 0);
    }

    [Fact]
    public void Save_WithQuality_ShouldRespectQuality()
    {
        var imageData = LoadTestImage("example.png");
        var lowQuality = LegioImageBuilder.Load(imageData).Quality(50).SaveAs(LegioImageFormat.Jpeg);
        var highQuality = LegioImageBuilder.Load(imageData).Quality(95).SaveAs(LegioImageFormat.Jpeg);

        Assert.NotNull(lowQuality);
        Assert.NotNull(highQuality);
        Assert.True(lowQuality.Length > 0);
        Assert.True(highQuality.Length > 0);
        Assert.True(highQuality.Length > lowQuality.Length);
    }

    [Fact]
    public void SaveAs_ShouldReturnBytes()
    {
        var imageData = LoadTestImage("example.png");
        var result = LegioImageBuilder.Load(imageData).SaveAs(LegioImageFormat.Jpeg, 80);

        Assert.NotNull(result);
        Assert.True(result.Length > 0);
    }

    [Theory]
    [InlineData(LegioImageFormat.Png)]
    [InlineData(LegioImageFormat.Jpeg)]
    [InlineData(LegioImageFormat.WebP)]
    public void SaveAsFormat_ShouldSupportFormats(LegioImageFormat format)
    {
        var imageData = LoadTestImage("example.png");
        var result = LegioImageBuilder.Load(imageData).SaveAs(format, 80);

        Assert.NotNull(result);
        Assert.True(result.Length > 0);
    }
}
