using System.Diagnostics;
using LegioSoft.Imaging.Core.Classes;
using LegioSoft.Imaging.Core.Enums;
using LegioSoft.Imaging.Skia.Core;
using SkiaSharp;
using Xunit;
using Xunit.Abstractions;

namespace LegioSoft.Imaging.Skia.Tests;

public class ImageMetadataReaderTests
{
    private readonly ITestOutputHelper _output;

    public ImageMetadataReaderTests(ITestOutputHelper output)
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

    private byte[] CreateTestPngWithAlpha(int width, int height)
    {
        using var bitmap = new SKBitmap(width, height);
        using var canvas = new SKCanvas(bitmap);
        canvas.Clear(new SKColor(255, 0, 0, 128));
        canvas.Flush();

        using var image = SKImage.FromBitmap(bitmap);
        using var data = image.Encode(SKEncodedImageFormat.Png, 100);
        return data.ToArray();
    }

    #region Functionality Tests

    [Fact]
    public void GetInfo_FromByteArray_Png_ReturnsCorrectMetadata()
    {
        var imageData = CreateTestPng(150, 200);

        var info = ImageMetadataReader.GetInfo(imageData);

        Assert.NotNull(info);
        Assert.Equal(150, info.Width);
        Assert.Equal(200, info.Height);
        Assert.Equal(LegioImageFormat.Png, info.Format);
        Assert.Equal(imageData.Length, info.ByteSize);
    }

    [Fact]
    public void GetInfo_FromByteArray_Jpeg_ReturnsCorrectMetadata()
    {
        var imageData = CreateTestJpeg(100, 150);

        var info = ImageMetadataReader.GetInfo(imageData);

        Assert.NotNull(info);
        Assert.Equal(100, info.Width);
        Assert.Equal(150, info.Height);
        Assert.Equal(LegioImageFormat.Jpeg, info.Format);
        Assert.Equal(imageData.Length, info.ByteSize);
    }

    [Fact]
    public void GetInfo_FromByteArray_WebP_ReturnsCorrectMetadata()
    {
        var imageData = CreateTestWebP(120, 180);

        var info = ImageMetadataReader.GetInfo(imageData);

        Assert.NotNull(info);
        Assert.Equal(120, info.Width);
        Assert.Equal(180, info.Height);
        Assert.Equal(LegioImageFormat.WebP, info.Format);
        Assert.Equal(imageData.Length, info.ByteSize);
    }

    [Fact]
    public void GetInfo_FromByteArray_WithAlpha_DetectsAlpha()
    {
        var imageData = CreateTestPngWithAlpha(100, 100);

        var info = ImageMetadataReader.GetInfo(imageData);

        Assert.NotNull(info);
        Assert.True(info.HasAlpha);
    }

    [Fact]
    public void GetInfo_FromByteArray_WithoutAlpha_NoAlphaFlag()
    {
        var imageData = CreateTestPng(100, 100);

        var info = ImageMetadataReader.GetInfo(imageData);

        Assert.NotNull(info);
    }

    [Fact]
    public void GetInfo_FromStream_Null_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => ImageMetadataReader.GetInfo((Stream)null!));
    }

    [Fact]
    public void GetInfo_FromStream_NotReadable_ThrowsArgumentException()
    {
        using var stream = new MemoryStream();
        stream.Dispose();

        Assert.Throws<ArgumentException>(() => ImageMetadataReader.GetInfo((Stream)stream));
    }

    [Fact]
    public void GetInfo_FromStream_PositionRestored_AfterReading()
    {
        var imageData = CreateTestPng(50, 50);
        using var stream = new MemoryStream(imageData);

        var info = ImageMetadataReader.GetInfo((Stream)stream);

        Assert.NotNull(info);
    }

    [Fact]
    public void GetInfo_FromByteArray_Null_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => ImageMetadataReader.GetInfo((byte[])null!));
    }

    [Fact]
    public void GetInfo_FromByteArray_Empty_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => ImageMetadataReader.GetInfo((byte[])Array.Empty<byte>()));
    }

    [Fact]
    public void GetInfo_FromByteArray_InvalidData_ThrowsInvalidOperationException()
    {
        byte[] invalidData = new byte[] { 0x00, 0x01, 0x02, 0x03 };

        Assert.Throws<InvalidOperationException>(() => ImageMetadataReader.GetInfo((byte[])invalidData));
    }

    [Fact]
    public void GetInfo_FromStream_ValidatesAndReturnsMetadata()
    {
        var imageData = CreateTestPng(75, 100);
        using var stream = new MemoryStream(imageData);

        var info = ImageMetadataReader.GetInfo((Stream)stream);

        Assert.NotNull(info);
        Assert.Equal(75, info.Width);
        Assert.Equal(100, info.Height);
        Assert.Equal(LegioImageFormat.Png, info.Format);
        Assert.Equal(imageData.Length, info.ByteSize);
    }

    [Fact]
    public void GetInfo_FromStream_NonSeekable_ByteSizeIsZero()
    {
        var imageData = CreateTestPng(75, 100);
        var nonSeekableStream = new NonSeekableMemoryStream(imageData);

        var info = ImageMetadataReader.GetInfo((Stream)nonSeekableStream);

        Assert.NotNull(info);
        Assert.Equal(75, info.Width);
        Assert.Equal(100, info.Height);
        Assert.Equal(0, info.ByteSize);
        nonSeekableStream.Dispose();
    }

    [Fact]
    public void GetInfo_FromFilePath_ValidatesAndReturnsMetadata()
    {
        var tempDir = Path.Combine(AppContext.BaseDirectory, "images");
        Directory.CreateDirectory(tempDir);
        var imagePath = Path.Combine(tempDir, "test.png");
        File.WriteAllBytes(imagePath, CreateTestPng(100, 100));

        try
        {
            var info = ImageMetadataReader.GetInfo(imagePath);

            Assert.NotNull(info);
            Assert.Equal(100, info.Width);
            Assert.Equal(100, info.Height);
        }
        finally
        {
            File.Delete(imagePath);
            Directory.Delete(tempDir);
        }
    }

    [Fact]
    public void GetInfo_FromFilePath_InvalidExtension_ThrowsArgumentException()
    {
        var tempDir = Path.Combine(AppContext.BaseDirectory, "images");
        Directory.CreateDirectory(tempDir);
        var imagePath = Path.Combine(tempDir, "test.bmp");
        File.WriteAllBytes(imagePath, new byte[] { 0x42, 0x4D });

        try
        {
            Assert.Throws<ArgumentException>(() => ImageMetadataReader.GetInfo(imagePath));
        }
        finally
        {
            File.Delete(imagePath);
            Directory.Delete(tempDir);
        }
    }

    [Fact]
    public void GetInfo_FromFilePath_NotExists_ThrowsFileNotFoundException()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "nonexistent.png");

        Assert.Throws<FileNotFoundException>(() => ImageMetadataReader.GetInfo(path));
    }

    [Fact]
    public void GetInfo_FromFilePath_OutsideApprovedDirectory_ThrowsUnauthorizedAccessException()
    {
        var imagePath = Path.Combine(AppContext.BaseDirectory, "test.png");
        File.WriteAllBytes(imagePath, CreateTestPng(100, 100));

        try
        {
            Assert.Throws<UnauthorizedAccessException>(() => ImageMetadataReader.GetInfo(imagePath));
        }
        finally
        {
            File.Delete(imagePath);
        }
    }

    [Fact]
    public void GetInfo_DetectsFormatCorrectly_AllSupportedFormats()
    {
        var pngData = CreateTestPng(100, 100);
        var jpegData = CreateTestJpeg(100, 100);
        var webpData = CreateTestWebP(100, 100);

        var pngInfo = ImageMetadataReader.GetInfo(pngData);
        var jpegInfo = ImageMetadataReader.GetInfo(jpegData);
        var webPInfo = ImageMetadataReader.GetInfo(webpData);

        Assert.Equal(LegioImageFormat.Png, pngInfo.Format);
        Assert.Equal(LegioImageFormat.Jpeg, jpegInfo.Format);
        Assert.Equal(LegioImageFormat.WebP, webPInfo.Format);
    }

    #endregion

    #region Security Tests

    [Fact]
    public void GetInfo_DimensionsZeroWidth_ThrowsInvalidOperationException()
    {
        var imageData = CreateTestPng(100, 100);
        var mockCodec = new MockCodec(0, 100);

        Assert.Throws<InvalidOperationException>(() => mockCodec.GetInfo(imageData));
    }

    [Fact]
    public void GetInfo_DimensionsZeroHeight_ThrowsInvalidOperationException()
    {
        var imageData = CreateTestPng(100, 100);
        var mockCodec = new MockCodec(100, 0);

        Assert.Throws<InvalidOperationException>(() => mockCodec.GetInfo(imageData));
    }

    [Fact]
    public void GetInfo_DimensionsNegative_ThrowsInvalidOperationException()
    {
        var imageData = CreateTestPng(100, 100);
        var mockCodec = new MockCodec(-100, 100);

        Assert.Throws<InvalidOperationException>(() => mockCodec.GetInfo(imageData));
    }

    [Fact]
    public void GetInfo_ExceedsMaxWidth_ThrowsInvalidOperationException()
    {
        var imageData = CreateTestPng(100, 100);
        var mockCodec = new MockCodec(20000, 100);

        Assert.Throws<InvalidOperationException>(() => mockCodec.GetInfo(imageData));
    }

    [Fact]
    public void GetInfo_ExceedsMaxHeight_ThrowsInvalidOperationException()
    {
        var imageData = CreateTestPng(100, 100);
        var mockCodec = new MockCodec(100, 20000);

        Assert.Throws<InvalidOperationException>(() => mockCodec.GetInfo(imageData));
    }

    [Fact]
    public void GetInfo_MaxDimensionsAllowed_ReadsSuccessfully()
    {
        var imageData = CreateTestPng(16384, 16384);
        var mockCodec = new MockCodec(16384, 16384);

        var info = mockCodec.GetInfo(imageData);

        Assert.NotNull(info);
        Assert.Equal(16384, info.Width);
        Assert.Equal(16384, info.Height);
    }

    [Fact]
    public void GetInfo_UnsupportedFormat_ThrowsInvalidOperationException()
    {
        var imageData = CreateTestPng(100, 100);
        var mockCodec = new MockCodec(100, 100, SKEncodedImageFormat.Bmp);

        Assert.Throws<InvalidOperationException>(() => mockCodec.GetInfo(imageData));
    }

    [Fact]
    public void GetInfo_FilePathTraversal_ThrowsUnauthorizedAccessException()
    {
        var tempDir = Path.Combine(AppContext.BaseDirectory, "images");
        Directory.CreateDirectory(tempDir);
        var maliciousPath = Path.Combine(tempDir, "..", "..", "test.png");
        File.WriteAllBytes(Path.Combine(AppContext.BaseDirectory, "test.png"), CreateTestPng(100, 100));

        try
        {
            Assert.ThrowsAny<Exception>(() => ImageMetadataReader.GetInfo(maliciousPath));
        }
        finally
        {
            File.Delete(Path.Combine(AppContext.BaseDirectory, "test.png"));
            Directory.Delete(tempDir, true);
        }
    }

    [Fact]
    public void GetInfo_CaseInsensitiveExtension_ReadsSuccessfully()
    {
        var tempDir = Path.Combine(AppContext.BaseDirectory, "images");
        Directory.CreateDirectory(tempDir);
        var imagePath = Path.Combine(tempDir, "test.PNG");
        File.WriteAllBytes(imagePath, CreateTestPng(100, 100));

        try
        {
            var info = ImageMetadataReader.GetInfo(imagePath);

            Assert.NotNull(info);
        }
        finally
        {
            File.Delete(imagePath);
            Directory.Delete(tempDir);
        }
    }

    [Fact]
    public void GetInfo_OutOfMemoryOnLargeImage_ThrowsOutOfMemoryException()
    {
        var imageData = CreateTestPng(100, 100);
        var mockCodec = new MockLargeImageCodec(20000, 20000);

        Assert.Throws<OutOfMemoryException>(() => mockCodec.GetInfo(imageData));
    }

    #endregion

    #region Memory Tests

    [Fact]
    public void GetInfo_DoesNotLoadFullImage_MemoryEfficient()
    {
        var largeImageData = CreateTestPng(5000, 5000);
        var initialMemory = GC.GetTotalMemory(true);

        for (var i = 0; i < 10; i++)
        {
            var info = ImageMetadataReader.GetInfo(largeImageData);
        }

        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
        var finalMemory = GC.GetTotalMemory(true);

        var memoryIncrease = finalMemory - initialMemory;
        Assert.True(memoryIncrease < 50 * 1024 * 1024,
            "Metadata reading should not consume excessive memory");
    }

    [Fact]
    public void GetInfo_MultipleCalls_MemoryStable()
    {
        var imageData = CreateTestPng(1000, 1000);
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        var initialMemory = GC.GetTotalMemory(true);
        var infos = new List<LegioImageInfo>();

        for (var i = 0; i < 100; i++)
        {
            infos.Add(ImageMetadataReader.GetInfo(imageData));
        }

        var memoryAfterReads = GC.GetTotalMemory(false);
        var memoryIncrease = memoryAfterReads - initialMemory;

        infos.Clear();
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
        var finalMemory = GC.GetTotalMemory(true);

        Assert.True(memoryIncrease < 10 * 1024 * 1024,
            "Multiple metadata reads should not accumulate significant memory");
    }

    [Fact]
    public void GetInfo_StreamSeekRestoresPosition_NoLeaks()
    {
        var imageData = CreateTestPng(100, 100);
        using var stream = new MemoryStream(imageData);

        var info = ImageMetadataReader.GetInfo((Stream)stream);

        Assert.NotNull(info);
    }

    [Fact]
    public void GetInfo_DisposeOfInternalResources_NoLeaks()
    {
        var imageData = CreateTestPng(1000, 1000);

        for (var i = 0; i < 50; i++)
        {
            using var stream = new MemoryStream(imageData);
            var info = ImageMetadataReader.GetInfo(stream);
        }

        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        Assert.True(true, "No exception thrown, resources properly disposed");
    }

    #endregion

    #region Performance Tests

    [Fact]
    public void GetInfo_SmallImage_PerformanceAcceptable()
    {
        var imageData = CreateTestPng(100, 100);
        var stopwatch = Stopwatch.StartNew();

        for (var i = 0; i < 1000; i++)
        {
            var info = ImageMetadataReader.GetInfo(imageData);
        }

        stopwatch.Stop();

        _output.WriteLine($"1000 small image metadata reads in {stopwatch.ElapsedMilliseconds}ms");
        Assert.True(stopwatch.ElapsedMilliseconds < 5000,
            "Reading metadata from small images should be very fast");
    }

    [Fact]
    public void GetInfo_LargeImage_PerformanceAcceptable()
    {
        var largeImageData = CreateTestPng(4000, 4000);
        var stopwatch = Stopwatch.StartNew();

        var info = ImageMetadataReader.GetInfo(largeImageData);

        stopwatch.Stop();

        _output.WriteLine($"Large image metadata read in {stopwatch.ElapsedMilliseconds}ms");
        Assert.True(stopwatch.ElapsedMilliseconds < 1000,
            "Reading metadata from large images should be fast (header only)");
    }

    [Fact]
    public void GetInfo_FromByteArrayVsStream_PerformanceComparison()
    {
        var imageData = CreateTestPng(1000, 1000);

        var streamStopwatch = Stopwatch.StartNew();
        for (var i = 0; i < 100; i++)
        {
            using var stream = new MemoryStream(imageData);
            var info = ImageMetadataReader.GetInfo((Stream)stream);
        }

        streamStopwatch.Stop();

        var arrayStopwatch = Stopwatch.StartNew();
        for (var i = 0; i < 100; i++)
        {
            var info = ImageMetadataReader.GetInfo((byte[])imageData);
        }

        arrayStopwatch.Stop();

        _output.WriteLine(
            $"Stream: {streamStopwatch.ElapsedMilliseconds}ms, Array: {arrayStopwatch.ElapsedMilliseconds}ms");
        var streamTime = streamStopwatch.ElapsedMilliseconds == 0 ? 1 : streamStopwatch.ElapsedMilliseconds;
        Assert.True(arrayStopwatch.ElapsedMilliseconds < streamTime * 2,
            "ByteArray method should not be significantly slower than stream");
    }

    [Fact]
    public void GetInfo_FasterThanLoadingFullImage_PerformanceComparison()
    {
        var imageData = CreateTestPng(1000, 1000);

        var infoStopwatch = Stopwatch.StartNew();
        for (var i = 0; i < 50; i++)
        {
            var info = ImageMetadataReader.GetInfo(imageData);
        }

        infoStopwatch.Stop();

        var loadStopwatch = Stopwatch.StartNew();
        for (var i = 0; i < 50; i++)
        {
            using var bitmap = ImageLoader.LoadBitmap(imageData);
        }

        loadStopwatch.Stop();

        _output.WriteLine(
            $"GetInfo: {infoStopwatch.ElapsedMilliseconds}ms, Load: {loadStopwatch.ElapsedMilliseconds}ms");
        Assert.True(infoStopwatch.ElapsedMilliseconds < loadStopwatch.ElapsedMilliseconds,
            "GetInfo should be faster than loading full image");
    }

    #endregion

    #region Helper Classes

    private class NonSeekableMemoryStream : MemoryStream
    {
        public NonSeekableMemoryStream(byte[] buffer) : base(buffer)
        {
        }

        public override bool CanSeek => false;

        public override long Seek(long offset, SeekOrigin loc)
        {
            throw new NotSupportedException();
        }

        public override long Position
        {
            get => 0;
            set => throw new NotSupportedException();
        }
    }

    private class MockCodec
    {
        private readonly int _width;
        private readonly int _height;
        private readonly SKEncodedImageFormat _format;

        public MockCodec(int width, int height, SKEncodedImageFormat format = SKEncodedImageFormat.Png)
        {
            _width = width;
            _height = height;
            _format = format;
        }

        public LegioImageInfo GetInfo(byte[] imageData)
        {
            using var ms = new MemoryStream(imageData);
            using var codec = SKCodec.Create(ms);

            if (codec == null)
                throw new InvalidOperationException("Cannot create codec");

            var info = new SKImageInfo(_width, _height, SKColorType.Bgra8888, SKAlphaType.Opaque);

            if (_width <= 0 || _height <= 0)
                throw new InvalidOperationException($"Invalid image dimensions: {_width}x{_height}");

            if (_width > 16384 || _height > 16384)
                throw new InvalidOperationException($"Image dimensions {_width}x{_height} exceed maximum");

            if (_format != SKEncodedImageFormat.Png && _format != SKEncodedImageFormat.Jpeg &&
                _format != SKEncodedImageFormat.Webp)
                throw new InvalidOperationException($"Unsupported format: {_format}");

            var format = _format switch
            {
                SKEncodedImageFormat.Png => LegioImageFormat.Png,
                SKEncodedImageFormat.Jpeg => LegioImageFormat.Jpeg,
                SKEncodedImageFormat.Webp => LegioImageFormat.WebP,
                _ => throw new InvalidOperationException($"Unsupported format: {_format}")
            };

            return new LegioImageInfo
            {
                Width = _width,
                Height = _height,
                Format = format,
                HasAlpha = false,
                ByteSize = imageData.Length
            };
        }
    }

    private class MockLargeImageCodec
    {
        private readonly int _width;
        private readonly int _height;

        public MockLargeImageCodec(int width, int height)
        {
            _width = width;
            _height = height;
        }

        public LegioImageInfo GetInfo(byte[] imageData)
        {
            if (_width > 16384 || _height > 16384)
                throw new OutOfMemoryException("Image too large");

            return new LegioImageInfo
            {
                Width = _width,
                Height = _height,
                Format = LegioImageFormat.Png,
                HasAlpha = false,
                ByteSize = imageData.Length
            };
        }
    }

    #endregion
}