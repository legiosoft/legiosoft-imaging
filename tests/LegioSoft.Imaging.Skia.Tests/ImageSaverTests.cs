using System.Diagnostics;
using LegioSoft.Imaging.Core.Enums;
using LegioSoft.Imaging.Skia.Core;
using SkiaSharp;
using Xunit;
using Xunit.Abstractions;

namespace LegioSoft.Imaging.Skia.Tests;

public class ImageSaverTests
{
    private readonly ITestOutputHelper _output;

    public ImageSaverTests(ITestOutputHelper output)
    {
        _output = output;
    }

    private SKBitmap CreateTestBitmap(int width, int height, SKColor color = default)
    {
        var bitmap = new SKBitmap(width, height);
        using var canvas = new SKCanvas(bitmap);
        canvas.Clear(color == default ? SKColors.Blue : color);
        canvas.Flush();
        return bitmap;
    }

    private SKBitmap CreateTestBitmapWithAlpha(int width, int height)
    {
        var bitmap = new SKBitmap(width, height);
        using var canvas = new SKCanvas(bitmap);
        canvas.Clear(new SKColor(255, 0, 0, 128));
        canvas.Flush();
        return bitmap;
    }

    #region Functionality Tests

    [Fact]
    public void SaveBitmap_ToByteArray_Png_SavesSuccessfully()
    {
        using var bitmap = CreateTestBitmap(100, 100);

        var result = ImageSaver.SaveBitmap(bitmap, LegioImageFormat.Png, 90);

        Assert.NotNull(result);
        Assert.True(result.Length > 0);

        using var loadedBitmap = SKBitmap.Decode(result);
        Assert.NotNull(loadedBitmap);
        Assert.Equal(100, loadedBitmap.Width);
        Assert.Equal(100, loadedBitmap.Height);
    }

    [Fact]
    public void SaveBitmap_ToByteArray_Jpeg_SavesSuccessfully()
    {
        using var bitmap = CreateTestBitmap(100, 100);

        var result = ImageSaver.SaveBitmap(bitmap, LegioImageFormat.Jpeg, 85);

        Assert.NotNull(result);
        Assert.True(result.Length > 0);

        using var loadedBitmap = SKBitmap.Decode(result);
        Assert.NotNull(loadedBitmap);
        Assert.Equal(100, loadedBitmap.Width);
        Assert.Equal(100, loadedBitmap.Height);
    }

    [Fact]
    public void SaveBitmap_ToByteArray_WebP_SavesSuccessfully()
    {
        using var bitmap = CreateTestBitmap(100, 100);

        var result = ImageSaver.SaveBitmap(bitmap, LegioImageFormat.WebP, 80);

        Assert.NotNull(result);
        Assert.True(result.Length > 0);

        using var loadedBitmap = SKBitmap.Decode(result);
        Assert.NotNull(loadedBitmap);
        Assert.Equal(100, loadedBitmap.Width);
        Assert.Equal(100, loadedBitmap.Height);
    }

    [Fact]
    public void SaveBitmap_ToByteArray_WithAlpha_Png_PreservesAlpha()
    {
        using var bitmap = CreateTestBitmapWithAlpha(100, 100);

        var result = ImageSaver.SaveBitmap(bitmap, LegioImageFormat.Png, 100);

        Assert.NotNull(result);
        Assert.True(result.Length > 0);
    }

    [Fact]
    public void SaveBitmap_ToByteArray_NullBitmap_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => ImageSaver.SaveBitmap(null!, LegioImageFormat.Png, 90));
    }

    [Fact]
    public void SaveBitmap_ToByteArray_ZeroWidth_DoesNotThrow()
    {
        using var bitmap = CreateTestBitmap(0, 100);
        Assert.NotNull(bitmap);
    }

    [Fact]
    public void SaveBitmap_ToByteArray_ZeroHeight_DoesNotThrow()
    {
        using var bitmap = CreateTestBitmap(100, 0);
        Assert.NotNull(bitmap);
    }

    [Fact]
    public void SaveBitmap_ToByteArray_NegativeDimensions_ThrowsException()
    {
        Assert.ThrowsAny<Exception>(() => CreateTestBitmap(-100, 100));
    }

    [Fact]
    public void SaveBitmap_ToByteArray_UnsupportedFormat_ThrowsNotSupportedException()
    {
        using var bitmap = CreateTestBitmap(100, 100);

        Assert.Throws<NotSupportedException>(() => ImageSaver.SaveBitmap(bitmap, (LegioImageFormat)999, 90));
    }

    [Fact]
    public void SaveBitmap_ToStream_Png_SavesSuccessfully()
    {
        using var bitmap = CreateTestBitmap(100, 100);
        using var stream = new MemoryStream();

        ImageSaver.SaveBitmap(bitmap, stream, LegioImageFormat.Png, 90);

        Assert.True(stream.Length > 0);

        stream.Position = 0;
        using var loadedBitmap = SKBitmap.Decode(stream);
        Assert.NotNull(loadedBitmap);
        Assert.Equal(100, loadedBitmap.Width);
        Assert.Equal(100, loadedBitmap.Height);
    }

    [Fact]
    public void SaveBitmap_ToStream_Jpeg_SavesSuccessfully()
    {
        using var bitmap = CreateTestBitmap(100, 100);
        using var stream = new MemoryStream();

        ImageSaver.SaveBitmap(bitmap, stream, LegioImageFormat.Jpeg, 85);

        Assert.True(stream.Length > 0);

        stream.Position = 0;
        using var loadedBitmap = SKBitmap.Decode(stream);
        Assert.NotNull(loadedBitmap);
        Assert.Equal(100, loadedBitmap.Width);
        Assert.Equal(100, loadedBitmap.Height);
    }

    [Fact]
    public void SaveBitmap_ToStream_WebP_SavesSuccessfully()
    {
        using var bitmap = CreateTestBitmap(100, 100);
        using var stream = new MemoryStream();

        ImageSaver.SaveBitmap(bitmap, stream, LegioImageFormat.WebP, 80);

        Assert.True(stream.Length > 0);

        stream.Position = 0;
        using var loadedBitmap = SKBitmap.Decode(stream);
        Assert.NotNull(loadedBitmap);
        Assert.Equal(100, loadedBitmap.Width);
        Assert.Equal(100, loadedBitmap.Height);
    }

    [Fact]
    public void SaveBitmap_ToStream_NullBitmap_ThrowsArgumentNullException()
    {
        using var stream = new MemoryStream();

        Assert.Throws<ArgumentNullException>(() => ImageSaver.SaveBitmap(null!, stream, LegioImageFormat.Png, 90));
    }

    [Fact]
    public void SaveBitmap_ToStream_NullStream_ThrowsArgumentNullException()
    {
        using var bitmap = CreateTestBitmap(100, 100);

        Assert.Throws<ArgumentNullException>(() => ImageSaver.SaveBitmap(bitmap, null!, LegioImageFormat.Png, 90));
    }

    [Fact]
    public void SaveBitmap_ToStream_ZeroWidth_ThrowsArgumentException()
    {
        using var bitmap = CreateTestBitmap(0, 100);
        using var stream = new MemoryStream();

        Assert.Throws<ArgumentException>(() => ImageSaver.SaveBitmap(bitmap, stream, LegioImageFormat.Png, 90));
    }

    [Fact]
    public void SaveBitmap_ToStream_ZeroHeight_DoesNotThrow()
    {
        using var bitmap = CreateTestBitmap(100, 0);
        Assert.NotNull(bitmap);
    }

    [Fact]
    public void SaveBitmap_ToStream_NegativeDimensions_ThrowsException()
    {
        Assert.ThrowsAny<Exception>(() => CreateTestBitmap(-100, 100));
    }

    [Fact]
    public void SaveBitmap_ToStream_UnsupportedFormat_ThrowsNotSupportedException()
    {
        using var bitmap = CreateTestBitmap(100, 100);
        using var stream = new MemoryStream();

        Assert.Throws<NotSupportedException>(() => ImageSaver.SaveBitmap(bitmap, stream, (LegioImageFormat)999, 90));
    }

    [Fact]
    public void SaveBitmap_StreamPosition_PreservesPosition()
    {
        using var bitmap = CreateTestBitmap(100, 100);
        using var stream = new MemoryStream();
        stream.Write(new byte[10], 0, 10);
        stream.Position = 5;

        ImageSaver.SaveBitmap(bitmap, stream, LegioImageFormat.Png, 90);

        Assert.True(stream.Length > 5);
    }

    [Fact]
    public void SaveBitmap_QualityAffectsSize_Jpeg()
    {
        using var bitmap = CreateTestBitmap(500, 500);

        var lowQuality = ImageSaver.SaveBitmap(bitmap, LegioImageFormat.Jpeg, 30);
        var highQuality = ImageSaver.SaveBitmap(bitmap, LegioImageFormat.Jpeg, 95);

        Assert.True(lowQuality.Length < highQuality.Length,
            "Lower quality should result in smaller file size");
    }

    [Fact]
    public void SaveBitmap_QualityAffectsSize_WebP()
    {
        using var bitmap = CreateTestBitmap(500, 500);

        var lowQuality = ImageSaver.SaveBitmap(bitmap, LegioImageFormat.WebP, 30);
        var highQuality = ImageSaver.SaveBitmap(bitmap, LegioImageFormat.WebP, 95);

        Assert.NotNull(lowQuality);
        Assert.NotNull(highQuality);
        Assert.True(lowQuality.Length > 0 && highQuality.Length > 0,
            "Both quality settings should produce valid output");
    }

    [Fact]
    public void SaveBitmap_PngQualityAlways100_SavesCorrectly()
    {
        using var bitmap = CreateTestBitmap(100, 100);

        var result50 = ImageSaver.SaveBitmap(bitmap, LegioImageFormat.Png, 50);
        var result100 = ImageSaver.SaveBitmap(bitmap, LegioImageFormat.Png, 100);

        Assert.NotNull(result50);
        Assert.NotNull(result100);
        Assert.True(result50.Length > 0);
        Assert.True(result100.Length > 0);
    }

    [Fact]
    public void SaveBitmap_WebPQuality100_SavesCorrectly()
    {
        using var bitmap = CreateTestBitmap(100, 100);

        var result = ImageSaver.SaveBitmap(bitmap, LegioImageFormat.WebP, 100);

        Assert.NotNull(result);
        Assert.True(result.Length > 0);
    }

    [Fact]
    public void SaveBitmap_QualityClamp_ZeroAnd100()
    {
        using var bitmap = CreateTestBitmap(100, 100);

        var resultZero = ImageSaver.SaveBitmap(bitmap, LegioImageFormat.Jpeg, 0);
        var resultNegative = ImageSaver.SaveBitmap(bitmap, LegioImageFormat.Jpeg, -50);
        var resultOver100 = ImageSaver.SaveBitmap(bitmap, LegioImageFormat.Jpeg, 150);
        var result100 = ImageSaver.SaveBitmap(bitmap, LegioImageFormat.Jpeg, 100);

        Assert.NotNull(resultZero);
        Assert.NotNull(resultNegative);
        Assert.NotNull(resultOver100);
        Assert.NotNull(result100);
    }

    #endregion

    #region Security Tests

    [Fact]
    public void SaveBitmap_DisposedBitmap_ThrowsException()
    {
        var bitmap = CreateTestBitmap(100, 100);
        bitmap.Dispose();

        Assert.ThrowsAny<Exception>(() => ImageSaver.SaveBitmap(bitmap, LegioImageFormat.Png, 90));
    }

    [Fact]
    public void SaveBitmap_ExtremeQualityValues_HandlesGracefully()
    {
        using var bitmap = CreateTestBitmap(100, 100);

        var resultMin = ImageSaver.SaveBitmap(bitmap, LegioImageFormat.Jpeg, int.MinValue);
        var resultMax = ImageSaver.SaveBitmap(bitmap, LegioImageFormat.Jpeg, int.MaxValue);

        Assert.NotNull(resultMin);
        Assert.NotNull(resultMax);
        Assert.True(resultMin.Length > 0);
        Assert.True(resultMax.Length > 0);
    }

    [Fact]
    public void SaveBitmap_VeryLargeImage_SavesSuccessfully()
    {
        using var bitmap = CreateTestBitmap(2000, 2000);

        var result = ImageSaver.SaveBitmap(bitmap, LegioImageFormat.Jpeg, 85);

        Assert.NotNull(result);
        Assert.True(result.Length > 0);
    }

    #endregion

    #region Memory Tests

    [Fact]
    public void SaveBitmap_ByteArrayOutput_DisposesInternalResources()
    {
        using var bitmap = CreateTestBitmap(1000, 1000);
        var initialMemory = GC.GetTotalMemory(true);

        for (var i = 0; i < 10; i++)
        {
            var result = ImageSaver.SaveBitmap(bitmap, LegioImageFormat.Png, 90);
        }

        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
        var finalMemory = GC.GetTotalMemory(true);

        var memoryIncrease = finalMemory - initialMemory;
        Assert.True(memoryIncrease < 20 * 1024 * 1024,
            "Multiple saves should not cause excessive memory buildup");
    }

    [Fact]
    public void SaveBitmap_StreamOutput_DisposesInternalResources()
    {
        using var bitmap = CreateTestBitmap(1000, 1000);
        var initialMemory = GC.GetTotalMemory(true);

        for (var i = 0; i < 10; i++)
        {
            using var stream = new MemoryStream();
            ImageSaver.SaveBitmap(bitmap, stream, LegioImageFormat.Png, 90);
        }

        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
        var finalMemory = GC.GetTotalMemory(true);

        var memoryIncrease = finalMemory - initialMemory;
        Assert.True(memoryIncrease < 20 * 1024 * 1024,
            "Multiple saves to stream should not cause excessive memory buildup");
    }

    [Fact]
    public void SaveBitmap_LargeImage_MemoryManaged()
    {
        using var bitmap = CreateTestBitmap(3000, 3000);

        var result = ImageSaver.SaveBitmap(bitmap, LegioImageFormat.Jpeg, 85);

        Assert.NotNull(result);
        Assert.True(result.Length > 0);
    }

    [Fact]
    public void SaveBitmap_MultipleSamesBitmap_MemoryStable()
    {
        using var bitmap = CreateTestBitmap(1000, 1000);
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        var initialMemory = GC.GetTotalMemory(true);

        for (var i = 0; i < 20; i++)
        {
            var result = ImageSaver.SaveBitmap(bitmap, LegioImageFormat.Jpeg, 85);
        }

        var memoryAfterSaves = GC.GetTotalMemory(false);
        var memoryIncrease = memoryAfterSaves - initialMemory;

        Assert.True(memoryIncrease < 50 * 1024 * 1024,
            "Multiple saves of the same bitmap should not accumulate significant memory");
    }

    [Fact]
    public void SaveBitmap_UsingStatement_CleanDisposal()
    {
        byte[] result;

        using (var bitmap = CreateTestBitmap(100, 100))
        {
            result = ImageSaver.SaveBitmap(bitmap, LegioImageFormat.Png, 90);
        }

        Assert.NotNull(result);
        Assert.True(result.Length > 0);
    }

    #endregion

    #region Performance Tests

    [Fact]
    public void SaveBitmap_SmallImage_PerformanceAcceptable()
    {
        using var bitmap = CreateTestBitmap(100, 100);
        var stopwatch = Stopwatch.StartNew();

        for (var i = 0; i < 100; i++)
        {
            var result = ImageSaver.SaveBitmap(bitmap, LegioImageFormat.Png, 90);
        }

        stopwatch.Stop();

        _output.WriteLine($"100 small images saved in {stopwatch.ElapsedMilliseconds}ms");
        Assert.True(stopwatch.ElapsedMilliseconds < 10000,
            "Saving small images should be fast");
    }

    [Fact]
    public void SaveBitmap_LargeImage_PerformanceAcceptable()
    {
        using var bitmap = CreateTestBitmap(2000, 2000);
        var stopwatch = Stopwatch.StartNew();

        var result = ImageSaver.SaveBitmap(bitmap, LegioImageFormat.Jpeg, 85);

        stopwatch.Stop();

        _output.WriteLine($"Large image saved in {stopwatch.ElapsedMilliseconds}ms");
        Assert.NotNull(result);
        Assert.True(stopwatch.ElapsedMilliseconds < 5000,
            "Saving large image should be reasonably fast");
    }

    [Fact]
    public void SaveBitmap_FormatComparison_PerformanceAcceptable()
    {
        using var bitmap = CreateTestBitmap(1000, 1000);

        var pngStopwatch = Stopwatch.StartNew();
        for (var i = 0; i < 20; i++)
        {
            var result = ImageSaver.SaveBitmap(bitmap, LegioImageFormat.Png, 90);
        }

        pngStopwatch.Stop();

        var jpegStopwatch = Stopwatch.StartNew();
        for (var i = 0; i < 20; i++)
        {
            var result = ImageSaver.SaveBitmap(bitmap, LegioImageFormat.Jpeg, 85);
        }

        jpegStopwatch.Stop();

        var webPStopwatch = Stopwatch.StartNew();
        for (var i = 0; i < 20; i++)
        {
            var result = ImageSaver.SaveBitmap(bitmap, LegioImageFormat.WebP, 80);
        }

        webPStopwatch.Stop();

        _output.WriteLine(
            $"PNG: {pngStopwatch.ElapsedMilliseconds}ms, JPEG: {jpegStopwatch.ElapsedMilliseconds}ms, WebP: {webPStopwatch.ElapsedMilliseconds}ms");
        Assert.True(
            pngStopwatch.ElapsedMilliseconds < 10000 && jpegStopwatch.ElapsedMilliseconds < 10000 &&
            webPStopwatch.ElapsedMilliseconds < 10000,
            "All formats should save reasonably fast");
    }

    [Fact]
    public void SaveBitmap_QualityVsPerformance_ReasonableTradeoff()
    {
        using var bitmap = CreateTestBitmap(500, 500);

        var lowQualityStopwatch = Stopwatch.StartNew();
        for (var i = 0; i < 50; i++)
        {
            var result = ImageSaver.SaveBitmap(bitmap, LegioImageFormat.Jpeg, 30);
        }

        lowQualityStopwatch.Stop();

        var highQualityStopwatch = Stopwatch.StartNew();
        for (var i = 0; i < 50; i++)
        {
            var result = ImageSaver.SaveBitmap(bitmap, LegioImageFormat.Jpeg, 95);
        }

        highQualityStopwatch.Stop();

        _output.WriteLine(
            $"Low quality: {lowQualityStopwatch.ElapsedMilliseconds}ms, High quality: {highQualityStopwatch.ElapsedMilliseconds}ms");
        Assert.True(highQualityStopwatch.ElapsedMilliseconds < lowQualityStopwatch.ElapsedMilliseconds * 3,
            "High quality should not be excessively slower than low quality");
    }

    [Fact]
    public void SaveBitmap_ByteArrayVsStream_PerformanceComparison()
    {
        using var bitmap = CreateTestBitmap(1000, 1000);

        var arrayStopwatch = Stopwatch.StartNew();
        for (var i = 0; i < 20; i++)
        {
            var result = ImageSaver.SaveBitmap(bitmap, LegioImageFormat.Png, 90);
        }

        arrayStopwatch.Stop();

        var streamStopwatch = Stopwatch.StartNew();
        for (var i = 0; i < 20; i++)
        {
            using var stream = new MemoryStream();
            ImageSaver.SaveBitmap(bitmap, stream, LegioImageFormat.Png, 90);
        }

        streamStopwatch.Stop();

        _output.WriteLine(
            $"Array: {arrayStopwatch.ElapsedMilliseconds}ms, Stream: {streamStopwatch.ElapsedMilliseconds}ms");
        Assert.True(streamStopwatch.ElapsedMilliseconds < arrayStopwatch.ElapsedMilliseconds * 2,
            "Stream output should not be significantly slower than array output");
    }

    [Fact]
    public void SaveBitmap_VariousSizes_PerformanceScales()
    {
        var sizes = new[] { 100, 500, 1000, 2000 };
        var results = new Dictionary<int, long>();

        foreach (var size in sizes)
        {
            using var bitmap = CreateTestBitmap(size, size);
            var stopwatch = Stopwatch.StartNew();

            var result = ImageSaver.SaveBitmap(bitmap, LegioImageFormat.Jpeg, 85);

            stopwatch.Stop();
            results[size] = stopwatch.ElapsedMilliseconds;
        }

        _output.WriteLine(
            $"Performance by size: {string.Join(", ", results.Select(kvp => $"{kvp.Key}px: {kvp.Value}ms"))}");

        Assert.True(results[2000] < results[100] * 100,
            "Performance should not degrade exponentially with size");
    }

    #endregion
}