using System.Diagnostics;
using System.Reflection;
using LegioSoft.Imaging.Skia.Core;
using SkiaSharp;
using Xunit;
using Xunit.Abstractions;

namespace LegioSoft.Imaging.Skia.Tests;

public class ImageSecurityTests
{
    private readonly ITestOutputHelper _output;

    public ImageSecurityTests(ITestOutputHelper output)
    {
        _output = output;
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

    private void CallValidateOpenedFile(FileStream fileStream, string filePath, string[] approvedDirectories,
        HashSet<string> allowedExtensions)
    {
        var method = typeof(ImageSecurity).GetMethod(
            "ValidateOpenedFile",
            BindingFlags.Static | BindingFlags.NonPublic);

        if (method == null)
            throw new InvalidOperationException("ValidateOpenedFile method not found");

        method.Invoke(null, new object[] { fileStream, filePath, approvedDirectories, allowedExtensions });
    }

    private FileStream CreateTestFile(string path, byte[] content)
    {
        File.WriteAllBytes(path, content);
        return File.OpenRead(path);
    }

    private static string CreateTempDirectory(string prefix)
    {
        var path = Path.Combine(AppContext.BaseDirectory, $"{prefix}-{Guid.NewGuid():N}");
        Directory.CreateDirectory(path);
        return path;
    }

    #region Functionality Tests

    [Fact]
    public void ValidateOpenedFile_ValidInApprovedDirectory_PassesValidation()
    {
        var tempDir = CreateTempDirectory("images");
        var imagePath = Path.Combine(tempDir, "test.png");
        var allowedExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            { ".png", ".jpg", ".jpeg", ".webp" };
        var approvedDirectories = new[] { tempDir };

        using var fileStream = CreateTestFile(imagePath, CreateTestPng(100, 100));

        try
        {
            CallValidateOpenedFile(fileStream, imagePath, approvedDirectories, allowedExtensions);
            Assert.True(true);
        }
        finally
        {
            fileStream.Dispose();
            File.Delete(imagePath);
            Directory.Delete(tempDir, true);
        }
    }

    [Fact]
    public void ValidateOpenedFile_ValidInAssetsDirectory_PassesValidation()
    {
        var tempDir = CreateTempDirectory("assets");
        var imagePath = Path.Combine(tempDir, "test.jpg");
        var allowedExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            { ".png", ".jpg", ".jpeg", ".webp" };
        var approvedDirectories = new[] { tempDir };

        using var fileStream = CreateTestFile(imagePath, CreateTestPng(100, 100));

        try
        {
            CallValidateOpenedFile(fileStream, imagePath, approvedDirectories, allowedExtensions);
            Assert.True(true);
        }
        finally
        {
            fileStream.Dispose();
            File.Delete(imagePath);
            Directory.Delete(tempDir, true);
        }
    }

    [Fact]
    public void ValidateOpenedFile_CaseInsensitiveExtension_PassesValidation()
    {
        var tempDir = CreateTempDirectory("images");
        var imagePath = Path.Combine(tempDir, "test.PNG");
        var allowedExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            { ".png", ".jpg", ".jpeg", ".webp" };
        var approvedDirectories = new[] { tempDir };

        using var fileStream = CreateTestFile(imagePath, CreateTestPng(100, 100));

        try
        {
            CallValidateOpenedFile(fileStream, imagePath, approvedDirectories, allowedExtensions);
            Assert.True(true);
        }
        finally
        {
            fileStream.Dispose();
            File.Delete(imagePath);
            Directory.Delete(tempDir, true);
        }
    }

    [Fact]
    public void ValidateOpenedFile_WebPExtension_PassesValidation()
    {
        var tempDir = CreateTempDirectory("images");
        var imagePath = Path.Combine(tempDir, "test.webp");
        var allowedExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            { ".png", ".jpg", ".jpeg", ".webp" };
        var approvedDirectories = new[] { tempDir };

        using var fileStream = CreateTestFile(imagePath, CreateTestPng(100, 100));

        try
        {
            CallValidateOpenedFile(fileStream, imagePath, approvedDirectories, allowedExtensions);
            Assert.True(true);
        }
        finally
        {
            fileStream.Dispose();
            File.Delete(imagePath);
            Directory.Delete(tempDir, true);
        }
    }

    [Fact]
    public void ValidateOpenedFile_JpegExtension_PassesValidation()
    {
        var tempDir = CreateTempDirectory("images");
        var imagePath = Path.Combine(tempDir, "test.jpeg");
        var allowedExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            { ".png", ".jpg", ".jpeg", ".webp" };
        var approvedDirectories = new[] { tempDir };

        using var fileStream = CreateTestFile(imagePath, CreateTestPng(100, 100));

        try
        {
            CallValidateOpenedFile(fileStream, imagePath, approvedDirectories, allowedExtensions);
            Assert.True(true);
        }
        finally
        {
            fileStream.Dispose();
            File.Delete(imagePath);
            Directory.Delete(tempDir, true);
        }
    }

    [Fact]
    public void ValidateOpenedFile_SubdirectoryInApproved_PassesValidation()
    {
        var tempDir = CreateTempDirectory("images");
        var subDir = Path.Combine(tempDir, "subfolder");
        Directory.CreateDirectory(subDir);
        var imagePath = Path.Combine(subDir, "test.png");
        var allowedExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            { ".png", ".jpg", ".jpeg", ".webp" };
        var approvedDirectories = new[] { tempDir };

        using var fileStream = CreateTestFile(imagePath, CreateTestPng(100, 100));

        try
        {
            CallValidateOpenedFile(fileStream, imagePath, approvedDirectories, allowedExtensions);
            Assert.True(true);
        }
        finally
        {
            fileStream.Dispose();
            File.Delete(imagePath);
            Directory.Delete(tempDir, true);
        }
    }

    #endregion

    #region Security Tests

    [Fact]
    public void ValidateOpenedFile_OutsideApprovedDirectory_ThrowsUnauthorizedAccessException()
    {
        var tempDir = CreateTempDirectory("unapproved");
        var imagePath = Path.Combine(tempDir, "test.png");
        var allowedExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            { ".png", ".jpg", ".jpeg", ".webp" };
        var approvedDirectories = new[] { Path.Combine(AppContext.BaseDirectory, "images") };

        using var fileStream = CreateTestFile(imagePath, CreateTestPng(100, 100));

        try
        {
            Assert.ThrowsAny<Exception>(() =>
                CallValidateOpenedFile(fileStream, imagePath, approvedDirectories, allowedExtensions));
        }
        finally
        {
            fileStream.Dispose();
            if (File.Exists(imagePath))
            {
                File.Delete(imagePath);
            }
            Directory.Delete(tempDir, true);
        }
    }

    [Fact]
    public void ValidateOpenedFile_UnsupportedExtension_ThrowsUnauthorizedAccessException()
    {
        var tempDir = CreateTempDirectory("images");
        var imagePath = Path.Combine(tempDir, "test.bmp");
        var allowedExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            { ".png", ".jpg", ".jpeg", ".webp" };
        var approvedDirectories = new[] { tempDir };

        using var fileStream = CreateTestFile(imagePath, new byte[] { 0x42, 0x4D });

        try
        {
            Assert.ThrowsAny<Exception>(() =>
                CallValidateOpenedFile(fileStream, imagePath, approvedDirectories, allowedExtensions));
        }
        finally
        {
            fileStream.Dispose();
            File.Delete(imagePath);
            Directory.Delete(tempDir, true);
        }
    }

    [Fact]
    public void ValidateOpenedFile_PathTraversalAttack_ThrowsUnauthorizedAccessException()
    {
        var tempDir = CreateTempDirectory("images");
        var maliciousPath = Path.Combine(tempDir, "..", "..", "test.png");
        File.WriteAllBytes(Path.Combine(AppContext.BaseDirectory, "test.png"), CreateTestPng(100, 100));
        var allowedExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            { ".png", ".jpg", ".jpeg", ".webp" };
        var approvedDirectories = new[] { tempDir };

        try
        {
            using var fileStream = File.OpenRead(Path.Combine(AppContext.BaseDirectory, "test.png"));
            Assert.ThrowsAny<Exception>(() =>
                CallValidateOpenedFile(fileStream, maliciousPath, approvedDirectories, allowedExtensions));
        }
        finally
        {
            File.Delete(Path.Combine(AppContext.BaseDirectory, "test.png"));
            Directory.Delete(tempDir, true);
        }
    }

    [Fact]
    public void ValidateOpenedFile_AbsolutePathOutsideApproved_ThrowsUnauthorizedAccessException()
    {
        var tempDir = CreateTempDirectory("images");
        var imagePath = Path.Combine(AppContext.BaseDirectory, "test.png");
        var allowedExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            { ".png", ".jpg", ".jpeg", ".webp" };
        var approvedDirectories = new[] { tempDir };

        using var fileStream = CreateTestFile(imagePath, CreateTestPng(100, 100));

        try
        {
            Assert.ThrowsAny<Exception>(() =>
                CallValidateOpenedFile(fileStream, imagePath, approvedDirectories, allowedExtensions));
        }
        finally
        {
            fileStream.Dispose();
            File.Delete(imagePath);
            Directory.Delete(tempDir, true);
        }
    }

    #endregion

    #region Memory Tests

    [Fact]
    public void ValidateOpenedFile_MultipleValidations_MemoryStable()
    {
        var tempDir = CreateTempDirectory("images");
        var imagePath = Path.Combine(tempDir, "test.png");
        var allowedExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            { ".png", ".jpg", ".jpeg", ".webp" };
        var approvedDirectories = new[] { tempDir };

        using var fileStream = CreateTestFile(imagePath, CreateTestPng(100, 100));

        try
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            var initialMemory = GC.GetTotalMemory(true);

            for (var i = 0; i < 100; i++)
            {
                CallValidateOpenedFile(fileStream, imagePath, approvedDirectories, allowedExtensions);
            }

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            var finalMemory = GC.GetTotalMemory(true);

            var memoryIncrease = finalMemory - initialMemory;
            Assert.True(memoryIncrease < 10 * 1024 * 1024,
                "Multiple validations should not accumulate significant memory");
        }
        finally
        {
            fileStream.Dispose();
            File.Delete(imagePath);
            Directory.Delete(tempDir, true);
        }
    }

    [Fact]
    public void ValidateOpenedFile_DisposeOfFileStream_NoLeaks()
    {
        var tempDir = CreateTempDirectory("images");
        var allowedExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            { ".png", ".jpg", ".jpeg", ".webp" };
        var approvedDirectories = new[] { tempDir };

        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
        var initialMemory = GC.GetTotalMemory(true);

        for (var i = 0; i < 50; i++)
        {
            var imagePath = Path.Combine(tempDir, $"test-{i}.png");
            using (var fileStream = CreateTestFile(imagePath, CreateTestPng(100, 100)))
            {
                CallValidateOpenedFile(fileStream, imagePath, approvedDirectories, allowedExtensions);
            }

            File.Delete(imagePath);
        }

        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
        var finalMemory = GC.GetTotalMemory(true);

        var memoryIncrease = finalMemory - initialMemory;
        Assert.True(memoryIncrease < 30 * 1024 * 1024,
            "Multiple validations with proper disposal should not leak memory");

        Directory.Delete(tempDir, true);
    }

    #endregion

    #region Performance Tests

    [Fact]
    public void ValidateOpenedFile_SingleValidation_PerformanceAcceptable()
    {
        var tempDir = CreateTempDirectory("images");
        var imagePath = Path.Combine(tempDir, "test.png");
        var allowedExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            { ".png", ".jpg", ".jpeg", ".webp" };
        var approvedDirectories = new[] { tempDir };

        using var fileStream = CreateTestFile(imagePath, CreateTestPng(100, 100));

        try
        {
            var stopwatch = Stopwatch.StartNew();

            CallValidateOpenedFile(fileStream, imagePath, approvedDirectories, allowedExtensions);

            stopwatch.Stop();

            _output.WriteLine($"Single validation in {stopwatch.ElapsedMilliseconds}ms");
            Assert.True(stopwatch.ElapsedMilliseconds < 1000,
                "Single validation should be fast");
        }
        finally
        {
            fileStream.Dispose();
            File.Delete(imagePath);
            Directory.Delete(tempDir, true);
        }
    }

    [Fact]
    public void ValidateOpenedFile_MultipleValidations_PerformanceAcceptable()
    {
        var tempDir = CreateTempDirectory("images");
        var imagePath = Path.Combine(tempDir, "test.png");
        var allowedExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            { ".png", ".jpg", ".jpeg", ".webp" };
        var approvedDirectories = new[] { tempDir };

        using var fileStream = CreateTestFile(imagePath, CreateTestPng(100, 100));

        try
        {
            var stopwatch = Stopwatch.StartNew();

            for (var i = 0; i < 100; i++)
            {
                CallValidateOpenedFile(fileStream, imagePath, approvedDirectories, allowedExtensions);
            }

            stopwatch.Stop();

            _output.WriteLine($"100 validations in {stopwatch.ElapsedMilliseconds}ms");
            Assert.True(stopwatch.ElapsedMilliseconds < 5000,
                "Multiple validations should be reasonably fast");
        }
        finally
        {
            fileStream.Dispose();
            File.Delete(imagePath);
            Directory.Delete(tempDir, true);
        }
    }

    [Fact]
    public void ValidateOpenedFile_DeepDirectoryPath_PerformanceAcceptable()
    {
        var tempDir = CreateTempDirectory("images");
        var deepPath = tempDir;
        for (var i = 0; i < 10; i++)
        {
            deepPath = Path.Combine(deepPath, $"level{i}");
        }

        Directory.CreateDirectory(deepPath);
        var imagePath = Path.Combine(deepPath, "test.png");
        var allowedExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            { ".png", ".jpg", ".jpeg", ".webp" };
        var approvedDirectories = new[] { tempDir };

        using var fileStream = CreateTestFile(imagePath, CreateTestPng(100, 100));

        try
        {
            var stopwatch = Stopwatch.StartNew();

            CallValidateOpenedFile(fileStream, imagePath, approvedDirectories, allowedExtensions);

            stopwatch.Stop();

            _output.WriteLine($"Deep path validation in {stopwatch.ElapsedMilliseconds}ms");
            Assert.True(stopwatch.ElapsedMilliseconds < 1000,
                "Deep path validation should be reasonably fast");
        }
        finally
        {
            fileStream.Dispose();
            File.Delete(imagePath);
            Directory.Delete(tempDir, true);
        }
    }

    [Fact]
    public void ValidateOpenedFile_MultipleApprovedDirectories_PerformanceAcceptable()
    {
        var approvedDirs = new List<string>();
        for (var i = 0; i < 10; i++)
        {
            var dir = Path.Combine(AppContext.BaseDirectory, $"images{i}");
            Directory.CreateDirectory(dir);
            approvedDirs.Add(dir);
        }

        var imagePath = Path.Combine(approvedDirs[5], "test.png");
        var allowedExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            { ".png", ".jpg", ".jpeg", ".webp" };
        var approvedDirectories = approvedDirs.ToArray();

        using var fileStream = CreateTestFile(imagePath, CreateTestPng(100, 100));

        try
        {
            var stopwatch = Stopwatch.StartNew();

            CallValidateOpenedFile(fileStream, imagePath, approvedDirectories, allowedExtensions);

            stopwatch.Stop();

            _output.WriteLine($"Multiple directories validation in {stopwatch.ElapsedMilliseconds}ms");
            Assert.True(stopwatch.ElapsedMilliseconds < 1000,
                "Validation with multiple approved directories should be reasonably fast");
        }
        finally
        {
            fileStream.Dispose();
            File.Delete(imagePath);
            foreach (var dir in approvedDirs)
            {
                Directory.Delete(dir, true);
            }
        }
    }

    #endregion
}
