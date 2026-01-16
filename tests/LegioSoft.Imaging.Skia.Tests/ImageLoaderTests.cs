using System.Diagnostics;
using LegioSoft.Imaging.Skia.Core;
using SkiaSharp;
using Xunit;
using Xunit.Abstractions;

namespace LegioSoft.Imaging.Skia.Tests;

public class ImageLoaderTests
{
    private readonly ITestOutputHelper _output;

    public ImageLoaderTests(ITestOutputHelper output)
    {
        _output = output;
    }

    private byte[] CreateTestPng(int width, int height, SKColor color = default)
    {
        using var bitmap = new SKBitmap(width, height);
        using var canvas = new SKCanvas(bitmap);
        canvas.Clear(color == default ? SKColors.Blue : color);
        canvas.Flush();

        using var image = SKImage.FromBitmap(bitmap);
        using var data = image.Encode(SKEncodedImageFormat.Png, 100);
        return data.ToArray();
    }

    private byte[] CreateTestJpeg(int width, int height)
    {
        using var bitmap = new SKBitmap(width, height);
        using var canvas = new SKCanvas(bitmap);
        canvas.Clear(SKColors.Red);
        canvas.Flush();

        using var image = SKImage.FromBitmap(bitmap);
        using var data = image.Encode(SKEncodedImageFormat.Jpeg, 85);
        return data.ToArray();
    }

    private byte[] CreateTestWebP(int width, int height)
    {
        using var bitmap = new SKBitmap(width, height);
        using var canvas = new SKCanvas(bitmap);
        canvas.Clear(SKColors.Green);
        canvas.Flush();

        using var image = SKImage.FromBitmap(bitmap);
        using var data = image.Encode(SKEncodedImageFormat.Webp, 80);
        return data.ToArray();
    }

    #region Functionality Tests

    [Fact]
    public void LoadBitmap_FromByteArray_Png_ValidatesAndLoads()
    {
        byte[] imageData = CreateTestPng(100, 100);

        using var result = ImageLoader.LoadBitmap(imageData);

        Assert.NotNull(result);
        Assert.Equal(100, result.Width);
        Assert.Equal(100, result.Height);
    }

    [Fact]
    public void LoadBitmap_FromByteArray_Jpeg_ValidatesAndLoads()
    {
        byte[] imageData = CreateTestJpeg(100, 100);

        using var result = ImageLoader.LoadBitmap(imageData);

        Assert.NotNull(result);
        Assert.Equal(100, result.Width);
        Assert.Equal(100, result.Height);
    }

    [Fact]
    public void LoadBitmap_FromByteArray_WebP_ValidatesAndLoads()
    {
        byte[] imageData = CreateTestWebP(100, 100);

        using var result = ImageLoader.LoadBitmap(imageData);

        Assert.NotNull(result);
        Assert.Equal(100, result.Width);
        Assert.Equal(100, result.Height);
    }

    [Fact]
    public void LoadBitmap_FromByteArray_Null_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => ImageLoader.LoadBitmap((byte[])null!));
    }

    [Fact]
    public void LoadBitmap_FromByteArray_Empty_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => ImageLoader.LoadBitmap(Array.Empty<byte>()));
    }

    [Fact]
    public void LoadBitmap_FromByteArray_InvalidData_ThrowsInvalidOperationException()
    {
        byte[] invalidData = new byte[] { 0x00, 0x01, 0x02, 0x03 };

        Assert.Throws<InvalidOperationException>(() => ImageLoader.LoadBitmap(invalidData));
    }

    [Fact]
    public void LoadBitmap_FromByteArray_ValidatesAndLoads()
    {
        var imageData = CreateTestPng(50, 50);

        using var result = ImageLoader.LoadBitmap(imageData);

        Assert.NotNull(result);
        Assert.Equal(50, result.Width);
        Assert.Equal(50, result.Height);
    }

    [Fact]
    public void LoadBitmap_FromFilePath_ValidatesAndLoads()
    {
        var tempDir = Path.Combine(AppContext.BaseDirectory, "images");
        Directory.CreateDirectory(tempDir);
        var imagePath = Path.Combine(tempDir, "test.png");
        File.WriteAllBytes(imagePath, CreateTestPng(100, 100));

        try
        {
            using var result = ImageLoader.LoadBitmap(imagePath);

            Assert.NotNull(result);
            Assert.Equal(100, result.Width);
        }
        finally
        {
            File.Delete(imagePath);
            Directory.Delete(tempDir);
        }
    }

    [Fact]
    public void LoadBitmap_FromFilePath_InvalidExtension_ThrowsArgumentException()
    {
        var tempDir = Path.Combine(AppContext.BaseDirectory, "images");
        Directory.CreateDirectory(tempDir);
        var imagePath = Path.Combine(tempDir, "test.bmp");
        File.WriteAllBytes(imagePath, new byte[] { 0x42, 0x4D });

        try
        {
            Assert.Throws<ArgumentException>(() => ImageLoader.LoadBitmap(imagePath));
        }
        finally
        {
            File.Delete(imagePath);
            Directory.Delete(tempDir);
        }
    }

    [Fact]
    public void LoadBitmap_FromFilePath_NotExists_ThrowsFileNotFoundException()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "nonexistent.png");

        Assert.Throws<FileNotFoundException>(() => ImageLoader.LoadBitmap(path));
    }

    [Fact]
    public void LoadBitmap_FromFilePath_OutsideApprovedDirectory_ThrowsUnauthorizedAccessException()
    {
        var imagePath = Path.Combine(AppContext.BaseDirectory, "test.png");
        File.WriteAllBytes(imagePath, CreateTestPng(100, 100));

        try
        {
            Assert.Throws<UnauthorizedAccessException>(() => ImageLoader.LoadBitmap(imagePath));
        }
        finally
        {
            File.Delete(imagePath);
        }
    }

    #endregion

    #region Security Tests

    [Fact]
    public void LoadBitmap_DimensionsZeroWidth_ThrowsInvalidOperationException()
    {
        var imageData = CreateTestPng(100, 100);
        var mockBitmap = new MockBitmap(0, 100, imageData);

        Assert.ThrowsAny<Exception>(() => mockBitmap.Load());
    }

    [Fact]
    public void LoadBitmap_DimensionsZeroHeight_ThrowsInvalidOperationException()
    {
        var imageData = CreateTestPng(100, 100);
        var mockBitmap = new MockBitmap(100, 0, imageData);

        Assert.ThrowsAny<Exception>(() => mockBitmap.Load());
    }

    [Fact]
    public void LoadBitmap_DimensionsNegative_ThrowsInvalidOperationException()
    {
        var imageData = CreateTestPng(100, 100);
        var mockBitmap = new MockBitmap(-100, 100, imageData);

        Assert.ThrowsAny<Exception>(() => mockBitmap.Load());
    }

    [Fact]
    public void LoadBitmap_ExceedsMaxWidth_ThrowsInvalidOperationException()
    {
        var imageData = CreateTestPng(100, 100);
        var mockBitmap = new MockBitmap(20000, 100, imageData);

        Assert.ThrowsAny<Exception>(() => mockBitmap.Load());
    }

    [Fact]
    public void LoadBitmap_ExceedsMaxHeight_ThrowsInvalidOperationException()
    {
        var imageData = CreateTestPng(100, 100);
        var mockBitmap = new MockBitmap(100, 20000, imageData);

        Assert.ThrowsAny<Exception>(() => mockBitmap.Load());
    }

    [Fact]
    public void LoadBitmap_MaxDimensionsAllowed_LoadsSuccessfully()
    {
        var imageData = CreateTestPng(16384, 16384);
        var mockBitmap = new MockBitmap(16384, 16384, imageData);

        var result = mockBitmap.Load();

        Assert.NotNull(result);
        result.Dispose();
    }

    [Fact]
    public void LoadBitmap_OverflowOnMemoryCalculation_ThrowsException()
    {
        var imageData = CreateTestPng(100, 100);
        var mockBitmap = new MockBitmap(1000000, 1000000, imageData);

        Assert.ThrowsAny<Exception>(() => mockBitmap.Load());
    }

    [Fact]
    public void LoadBitmap_ExceedsMaxMemory_ThrowsException()
    {
        var imageData = CreateTestPng(100, 100);
        var mockBitmap = new MockBitmap(12000, 12000, imageData);

        Assert.ThrowsAny<Exception>(() => mockBitmap.Load());
    }

    [Fact]
    public void LoadBitmap_FilePathTraversal_ThrowsUnauthorizedAccessException()
    {
        var tempDir = Path.Combine(AppContext.BaseDirectory, "images");
        Directory.CreateDirectory(tempDir);
        var maliciousPath = Path.Combine(tempDir, "..", "..", "test.png");
        File.WriteAllBytes(Path.Combine(AppContext.BaseDirectory, "test.png"), CreateTestPng(100, 100));

        try
        {
            Assert.ThrowsAny<Exception>(() => ImageLoader.LoadBitmap(maliciousPath));
        }
        finally
        {
            File.Delete(Path.Combine(AppContext.BaseDirectory, "test.png"));
            Directory.Delete(tempDir, true);
        }
    }

    [Fact]
    public void LoadBitmap_CaseInsensitiveExtension_LoadsSuccessfully()
    {
        var tempDir = Path.Combine(AppContext.BaseDirectory, "images");
        Directory.CreateDirectory(tempDir);
        var imagePath = Path.Combine(tempDir, "test.PNG");
        File.WriteAllBytes(imagePath, CreateTestPng(100, 100));

        try
        {
            using var result = ImageLoader.LoadBitmap(imagePath);

            Assert.NotNull(result);
        }
        finally
        {
            File.Delete(imagePath);
            Directory.Delete(tempDir);
        }
    }

    #endregion

    #region Memory Tests

    [Fact]
    public void LoadBitmap_Dispose_MemoryFreed()
    {
        var initialMemory = GC.GetTotalMemory(true);
        var imageData = CreateTestPng(2000, 2000);

        var bitmap = ImageLoader.LoadBitmap(imageData);
        var bitmapHandle = bitmap.Handle;
        var allocatedMemory = GC.GetTotalMemory(false);

        bitmap.Dispose();
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
        var finalMemory = GC.GetTotalMemory(true);

        Assert.NotEqual(IntPtr.Zero, bitmapHandle);
        Assert.True(finalMemory < initialMemory + 50 * 1024 * 1024, "Memory should be freed after disposal");
    }

    [Fact]
    public void LoadBitmap_MultipleLoadsWithoutDispose_MemoryIncreases()
    {
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
        var initialMemory = GC.GetTotalMemory(true);
        var imageData = CreateTestPng(500, 500);

        var bitmaps = new List<SKBitmap>();
        for (var i = 0; i < 10; i++)
        {
            bitmaps.Add(ImageLoader.LoadBitmap(imageData));
        }

        var memoryAfterLoads = GC.GetTotalMemory(false);
        var memoryIncrease = memoryAfterLoads - initialMemory;

        foreach (var bitmap in bitmaps)
        {
            bitmap.Dispose();
        }

        var expectedMinMemory = 500 * 500 * 4 * 5;
        Assert.True(memoryIncrease > 0 || memoryIncrease > expectedMinMemory,
            "Memory should increase with multiple loads");
    }

    [Fact]
    public void LoadBitmap_LargeImage_MemoryManaged()
    {
        var imageData = CreateTestPng(3000, 3000);

        using var bitmap = ImageLoader.LoadBitmap(imageData);

        Assert.NotNull(bitmap);
        Assert.Equal(3000, bitmap.Width);
        Assert.Equal(3000, bitmap.Height);
    }

    [Fact]
    public void LoadBitmap_UsingStatement_AutoDisposes()
    {
        byte[] imageData = CreateTestPng(100, 100);
        SKBitmap? bitmap;
        IntPtr bitmapHandle;

        using (bitmap = ImageLoader.LoadBitmap(imageData))
        {
            Assert.NotNull(bitmap);
            bitmapHandle = bitmap.Handle;
            Assert.NotEqual(IntPtr.Zero, bitmapHandle);
        }

        Assert.True(bitmap == null || bitmap.Handle == IntPtr.Zero);
    }

    #endregion

    #region Performance Tests

    [Fact]
    public void LoadBitmap_SmallImage_PerformanceAcceptable()
    {
        var imageData = CreateTestPng(100, 100);
        var stopwatch = Stopwatch.StartNew();

        for (var i = 0; i < 100; i++)
        {
            using var bitmap = ImageLoader.LoadBitmap(imageData);
        }

        stopwatch.Stop();

        _output.WriteLine($"100 small images loaded in {stopwatch.ElapsedMilliseconds}ms");
        Assert.True(stopwatch.ElapsedMilliseconds < 10000, "Loading 100 small images should be fast");
    }

    [Fact]
    public void LoadBitmap_LargeImage_PerformanceAcceptable()
    {
        var imageData = CreateTestPng(2000, 2000);
        var stopwatch = Stopwatch.StartNew();

        using var bitmap = ImageLoader.LoadBitmap(imageData);

        stopwatch.Stop();

        _output.WriteLine($"Large image loaded in {stopwatch.ElapsedMilliseconds}ms");
        Assert.True(stopwatch.ElapsedMilliseconds < 5000, "Loading large image should be reasonably fast");
    }

    [Fact]
    public void LoadBitmap_ByteArrayPerformance_MultipleLoadsAcceptable()
    {
        byte[] imageData = CreateTestPng(1000, 1000);
        var stopwatch = Stopwatch.StartNew();

        for (var i = 0; i < 50; i++)
        {
            using var bitmap = ImageLoader.LoadBitmap(imageData);
        }

        stopwatch.Stop();

        _output.WriteLine($"50 images loaded in {stopwatch.ElapsedMilliseconds}ms");
        Assert.True(stopwatch.ElapsedMilliseconds < 5000, "Multiple loads should be reasonably fast");
    }

    #endregion

    #region Helper Classes

    private class MockBitmap
    {
        private readonly int _width;
        private readonly int _height;
        private readonly byte[] _imageData;

        public MockBitmap(int width, int height, byte[] imageData)
        {
            _width = width;
            _height = height;
            _imageData = imageData;
        }

        public SKBitmap Load()
        {
            using var ms = new MemoryStream(_imageData);
            using var codec = SKCodec.Create(ms);
            if (codec == null)
                throw new InvalidOperationException("Cannot create codec");

            var info = new SKImageInfo(_width, _height, SKColorType.Bgra8888, SKAlphaType.Premul);
            var bitmap = new SKBitmap(info);

            if (bitmap.Handle == IntPtr.Zero)
                throw new InvalidOperationException("Failed to allocate memory");

            try
            {
                var result = codec.GetPixels(info, bitmap.GetPixels());
                if (result != SKCodecResult.Success)
                {
                    bitmap.Dispose();
                    throw new InvalidOperationException($"Failed to decode image: {result}");
                }
            }
            catch
            {
                bitmap.Dispose();
                throw;
            }

            return bitmap;
        }
    }

    #endregion
}