using LegioSoft.Imaging.WebP.Enums;
using LegioSoft.Imaging.WebP.Encoder;
using LegioSoft.Imaging.WebP.Models;
using Xunit;

namespace LegioSoft.Imaging.WebP.Tests;

public class WebPEncodingTests : IDisposable
{
    private string TestDataPath;
    private string OutputPath;
    private static readonly bool NativeLibraryAvailable;

    static WebPEncodingTests()
    {
        try
        {
            var version = WebPImage.GetVersion();
            NativeLibraryAvailable = !string.IsNullOrEmpty(version);
        }
        catch (DllNotFoundException)
        {
            NativeLibraryAvailable = false;
        }
        catch (TypeInitializationException)
        {
            NativeLibraryAvailable = false;
        }
    }

    public WebPEncodingTests()
    {
        TestDataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestData");
        OutputPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Output/Encoding");
        
        if (!Directory.Exists(TestDataPath))
        {
            Directory.CreateDirectory(TestDataPath);
        }
        
        if (!Directory.Exists(OutputPath))
        {
            Directory.CreateDirectory(OutputPath);
        }
    }

    private string GetOutputFilePath(string fileName)
    {
        return Path.Combine(OutputPath, fileName);
    }

    private byte[] CreateTestRGBA(int width, int height)
    {
        var data = new byte[width * height * 4];
        
        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                var index = (y * width + x) * 4;
                var r = (byte)((x * 255) / width);
                var g = (byte)((y * 255) / height);
                var b = (byte)(255 - r);
                data[index] = r;
                data[index + 1] = g;
                data[index + 2] = b;
                data[index + 3] = 255;
            }
        }
        
        return data;
    }

    private byte[] CreateTestRGB(int width, int height)
    {
        var data = new byte[width * height * 3];
        
        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                var index = (y * width + x) * 3;
                var r = (byte)((x * 255) / width);
                var g = (byte)((y * 255) / height);
                var b = (byte)(255 - r);
                data[index] = r;
                data[index + 1] = g;
                data[index + 2] = b;
            }
        }
        
        return data;
    }

    private byte[] CreateTestBGRA(int width, int height)
    {
        var data = new byte[width * height * 4];
        
        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                var index = (y * width + x) * 4;
                var b = (byte)((x * 255) / width);
                var g = (byte)((y * 255) / height);
                var r = (byte)(255 - b);
                data[index] = b;
                data[index + 1] = g;
                data[index + 2] = r;
                data[index + 3] = 255;
            }
        }
        
        return data;
    }

    private byte[] CreateTestBGR(int width, int height)
    {
        var data = new byte[width * height * 3];
        
        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                var index = (y * width + x) * 3;
                var b = (byte)((x * 255) / width);
                var g = (byte)((y * 255) / height);
                var r = (byte)(255 - b);
                data[index] = b;
                data[index + 1] = g;
                data[index + 2] = r;
            }
        }
        
        return data;
    }

    private void AssertImageData(byte[] data, string description)
    {
        Assert.NotNull(data);
        Assert.True(data.Length > 0, $"{description}: Image data should not be empty");
    }

    private void AssertFileExists(string path)
    {
        Assert.True(File.Exists(path), $"Expected file does not exist: {path}");
    }

    [Fact]
    [Trait("RequiresNativeLibrary", "true")]
    public void Encode_RGBA_WithQuality_ReturnsWebPData()
    {
        if (!NativeLibraryAvailable)
            return;

        var rgbaData = CreateTestRGBA(800, 600);
        var webpData = WebPImage.Encode(rgbaData, 800, 600, 75.0f);

        AssertImageData(webpData, "RGBA encoding");
        Assert.True(webpData.Length > 0);
    }

    [Fact]
    [Trait("RequiresNativeLibrary", "true")]
    public void Encode_RGBA_DifferentQualityLevels_ProduceDifferentSizes()
    {
        if (!NativeLibraryAvailable)
            return;

        var rgbaData = CreateTestRGBA(800, 600);

        var highQuality = WebPImage.Encode(rgbaData, 800, 600, 90.0f);
        var lowQuality = WebPImage.Encode(rgbaData, 800, 600, 30.0f);

        AssertImageData(highQuality, "High quality encoding");
        AssertImageData(lowQuality, "Low quality encoding");
        Assert.True(highQuality.Length > lowQuality.Length, "High quality should produce larger file");
    }

    [Fact]
    [Trait("RequiresNativeLibrary", "true")]
    public void Encode_Lossless_ReturnsWebPData()
    {
        if (!NativeLibraryAvailable)
            return;

        var rgbaData = CreateTestRGBA(800, 600);
        var webpData = WebPImage.EncodeLossless(rgbaData, 800, 600);

        AssertImageData(webpData, "Lossless encoding");
    }

    [Fact]
    [Trait("RequiresNativeLibrary", "true")]
    public void Encode_LosslessRGB_ReturnsWebPData()
    {
        if (!NativeLibraryAvailable)
            return;

        var rgbData = CreateTestRGB(800, 600);
        var webpData = WebPImage.EncodeLosslessRGB(rgbData, 800, 600);

        AssertImageData(webpData, "Lossless RGB encoding");
    }

    [Fact]
    [Trait("RequiresNativeLibrary", "true")]
    public void Encode_BGRA_WithQuality_ReturnsWebPData()
    {
        if (!NativeLibraryAvailable)
            return;

        var bgraData = CreateTestBGRA(800, 600);
        var webpData = WebPEncoder.EncodeBGRA(bgraData, 800, 600, 75.0f);

        AssertImageData(webpData, "BGRA encoding");
    }

    [Fact]
    [Trait("RequiresNativeLibrary", "true")]
    public void Encode_BGRA_Lossless_ReturnsWebPData()
    {
        if (!NativeLibraryAvailable)
            return;

        var bgraData = CreateTestBGRA(800, 600);
        var webpData = WebPEncoder.EncodeBGRA(bgraData, 800, 600, 75.0f, true);

        AssertImageData(webpData, "BGRA lossless encoding");
    }

    [Fact]
    [Trait("RequiresNativeLibrary", "true")]
    public void Encode_BGR_WithQuality_ReturnsWebPData()
    {
        if (!NativeLibraryAvailable)
            return;

        var bgrData = CreateTestBGR(800, 600);
        var webpData = WebPEncoder.EncodeBGR(bgrData, 800, 600, 75.0f);

        AssertImageData(webpData, "BGR encoding");
    }

    [Fact]
    public void Encode_WithInvalidQuality_ThrowsException()
    {
        var rgbaData = CreateTestRGBA(800, 600);

        Assert.Throws<ArgumentException>(() => WebPImage.Encode(rgbaData, 800, 600, 150.0f));
        Assert.Throws<ArgumentException>(() => WebPImage.Encode(rgbaData, 800, 600, -10.0f));
    }

    [Fact]
    public void Encode_WithInvalidDimensions_ThrowsException()
    {
        var rgbaData = CreateTestRGBA(100, 100);

        Assert.Throws<ArgumentException>(() => WebPImage.Encode(rgbaData, 0, 100, 75.0f));
        Assert.Throws<ArgumentException>(() => WebPImage.Encode(rgbaData, 100, 0, 75.0f));
        Assert.Throws<ArgumentException>(() => WebPImage.Encode(rgbaData, -100, 100, 75.0f));
    }

    [Fact]
    public void Encode_WithNullData_ThrowsException()
    {
        Assert.Throws<ArgumentException>(() => WebPImage.Encode(null!, 800, 600, 75.0f));
    }

    [Fact]
    [Trait("RequiresNativeLibrary", "true")]
    public void EncodeToFile_CreatesValidFile()
    {
        if (!NativeLibraryAvailable)
            return;

        var rgbaData = CreateTestRGBA(800, 600);
        var outputPath = GetOutputFilePath("test-output.webp");

        File.Delete(outputPath);

        try
        {
            using var tempFile = new FileStream(GetOutputFilePath("temp-jpg.bin"), FileMode.Create, FileAccess.Write);
            tempFile.Write(rgbaData, 0, rgbaData.Length);
            tempFile.Close();

            WebPImage.EncodeToFile(GetOutputFilePath("temp-jpg.bin"), outputPath, 75.0f);

            AssertFileExists(outputPath);
            var fileData = File.ReadAllBytes(outputPath);
            Assert.True(fileData.Length > 0);
        }
        finally
        {
            File.Delete(GetOutputFilePath("temp-jpg.bin"));
        }
    }

    [Fact]
    [Trait("RequiresNativeLibrary", "true")]
    public void EncodeToFileRGBA_CreatesValidFile()
    {
        if (!NativeLibraryAvailable)
            return;

        var rgbaData = CreateTestRGBA(800, 600);
        var outputPath = GetOutputFilePath("test-output-rgba.webp");

        WebPImage.EncodeToFileRGBA(rgbaData, 800, 600, outputPath, 75.0f);

        AssertFileExists(outputPath);
        var fileData = File.ReadAllBytes(outputPath);
        Assert.True(fileData.Length > 0);
    }

    [Fact]
    [Trait("RequiresNativeLibrary", "true")]
    public void EncodeAdvanced_WithPhotoP_ReturnsValidData()
    {
        if (!NativeLibraryAvailable)
            return;

        var rgbaData = CreateTestRGBA(800, 600);

        var photoOptions = new WebPEncodeOptions
        {
            Preset = WebPPreset.PHOTO,
            Quality = 85.0f,
            Lossless = false,
            Method = 6
        };

        var photoWebP = WebPImage.EncodeAdvanced(rgbaData, 800, 600, photoOptions);

        AssertImageData(photoWebP, "Advanced encoding - Photo preset");
    }

    [Fact]
    [Trait("RequiresNativeLibrary", "true")]
    public void EncodeAdvanced_WithPicturePreset_ReturnsValidData()
    {
        if (!NativeLibraryAvailable)
            return;

        var rgbaData = CreateTestRGBA(800, 600);

        var pictureOptions = new WebPEncodeOptions
        {
            Preset = WebPPreset.PICTURE,
            Quality = 90.0f,
            Lossless = false,
            Method = 6
        };

        var pictureWebP = WebPImage.EncodeAdvanced(rgbaData, 800, 600, pictureOptions);

        AssertImageData(pictureWebP, "Advanced encoding - Picture preset");
    }

    [Fact]
    [Trait("RequiresNativeLibrary", "true")]
    public void EncodeAdvanced_WithCustomConfiguration_ReturnsValidData()
    {
        if (!NativeLibraryAvailable)
            return;

        var rgbaData = CreateTestRGBA(800, 600);

        var customOptions = new WebPEncodeOptions
        {
            Quality = 80.0f,
            Method = 4,
            Segments = 3,
            SnsStrength = 75,
            FilterStrength = 80,
            FilterSharpness = 4,
            Autofilter = true,
            AlphaCompression = 1,
            AlphaFiltering = 1
        };

        var customWebP = WebPImage.EncodeAdvanced(rgbaData, 800, 600, customOptions);

        AssertImageData(customWebP, "Advanced encoding - Custom config");
    }

    [Fact]
    [Trait("RequiresNativeLibrary", "true")]
    public void EncodeAdvanced_Lossless_ReturnsValidData()
    {
        if (!NativeLibraryAvailable)
            return;

        var rgbaData = CreateTestRGBA(800, 600);

        var options = new WebPEncodeOptions
        {
            Lossless = true,
            Quality = 100.0f,
            Method = 6
        };

        var webpData = WebPImage.EncodeAdvanced(rgbaData, 800, 600, options);

        AssertImageData(webpData, "Advanced encoding - Lossless");
    }

    [Fact]
    [Trait("RequiresNativeLibrary", "true")]
    public void EncodeWithScaling_ScaleDown_ReturnsValidData()
    {
        if (!NativeLibraryAvailable)
            return;

        var rgbaData = CreateTestRGBA(800, 600);

        var webpData = WebPImage.EncodeWithScaling(rgbaData, 800, 600, 400, 300, 75.0f);

        AssertImageData(webpData, "Encoding with scaling down");
    }

    [Fact]
    [Trait("RequiresNativeLibrary", "true")]
    public void EncodeWithScaling_ScaleUp_ReturnsValidData()
    {
        if (!NativeLibraryAvailable)
            return;

        var rgbaData = CreateTestRGBA(400, 300);

        var webpData = WebPImage.EncodeWithScaling(rgbaData, 400, 300, 800, 600, 75.0f);

        AssertImageData(webpData, "Encoding with scaling up");
    }

    [Fact]
    [Trait("RequiresNativeLibrary", "true")]
    public void EncodeWithCropping_CropCenter_ReturnsValidData()
    {
        if (!NativeLibraryAvailable)
            return;

        var rgbaData = CreateTestRGBA(800, 600);

        var webpData = WebPImage.EncodeWithCropping(rgbaData, 800, 600, 200, 150, 400, 300, 75.0f);

        AssertImageData(webpData, "Encoding with cropping");
    }

    [Fact]
    [Trait("RequiresNativeLibrary", "true")]
    public void EncodeWithCropping_CropTopLeft_ReturnsValidData()
    {
        if (!NativeLibraryAvailable)
            return;

        var rgbaData = CreateTestRGBA(800, 600);

        var webpData = WebPImage.EncodeWithCropping(rgbaData, 800, 600, 0, 0, 400, 300, 75.0f);

        AssertImageData(webpData, "Encoding with cropping top-left");
    }

    [Fact]
    [Trait("RequiresNativeLibrary", "true")]
    public void Decode_WebPToRGBA_ReturnsValidData()
    {
        if (!NativeLibraryAvailable)
            return;

        var rgbaData = CreateTestRGBA(800, 600);
        var webpData = WebPImage.EncodeLossless(rgbaData, 800, 600);

        var decoded = WebPImage.Decode(webpData, WEBP_CSP_MODE.MODE_RGBA);

        AssertImageData(decoded, "RGBA decoding");
        Assert.Equal(rgbaData.Length, decoded.Length);
    }

    [Fact]
    [Trait("RequiresNativeLibrary", "true")]
    public void Decode_WebPToRGB_ReturnsValidData()
    {
        if (!NativeLibraryAvailable)
            return;

        var rgbaData = CreateTestRGBA(800, 600);
        var webpData = WebPImage.EncodeLossless(rgbaData, 800, 600);

        var decoded = WebPImage.Decode(webpData, WEBP_CSP_MODE.MODE_RGB);

        AssertImageData(decoded, "RGB decoding");
        Assert.Equal(800 * 600 * 3, decoded.Length);
    }

    [Fact]
    [Trait("RequiresNativeLibrary", "true")]
    public void Decode_FromStream_ReturnsValidData()
    {
        if (!NativeLibraryAvailable)
            return;

        var rgbaData = CreateTestRGBA(800, 600);
        var webpData = WebPImage.EncodeLossless(rgbaData, 800, 600);

        using var stream = new MemoryStream(webpData);
        var decoded = WebPImage.Decode(stream);

        AssertImageData(decoded, "Stream decoding");
    }

    [Fact]
    [Trait("RequiresNativeLibrary", "true")]
    public void Scale_DownToHalf_ReturnsValidData()
    {
        if (!NativeLibraryAvailable)
            return;

        var rgbaData = CreateTestRGBA(800, 600);
        var webpData = WebPImage.EncodeLossless(rgbaData, 800, 600);

        var scaled = WebPImage.Scale(webpData, 400, 300, WEBP_CSP_MODE.MODE_RGBA);

        AssertImageData(scaled, "Scaling down");
        Assert.Equal(400 * 300 * 4, scaled.Length);
    }

    [Fact]
    [Trait("RequiresNativeLibrary", "true")]
    public void Scale_UpToDouble_ReturnsValidData()
    {
        if (!NativeLibraryAvailable)
            return;

        var rgbaData = CreateTestRGBA(400, 300);
        var webpData = WebPImage.EncodeLossless(rgbaData, 400, 300);

        var scaled = WebPImage.Scale(webpData, 800, 600, WEBP_CSP_MODE.MODE_RGBA);

        AssertImageData(scaled, "Scaling up");
        Assert.Equal(800 * 600 * 4, scaled.Length);
    }

    [Fact]
    [Trait("RequiresNativeLibrary", "true")]
    public void Crop_CenterRegion_ReturnsValidData()
    {
        if (!NativeLibraryAvailable)
            return;

        var rgbaData = CreateTestRGBA(800, 600);
        var webpData = WebPImage.EncodeLossless(rgbaData, 800, 600);

        var cropped = WebPImage.Crop(webpData, 200, 150, 400, 300, WEBP_CSP_MODE.MODE_RGBA);

        AssertImageData(cropped, "Cropping");
        Assert.Equal(400 * 300 * 4, cropped.Length);
    }

    [Fact]
    [Trait("RequiresNativeLibrary", "true")]
    public void Crop_TopLeftCorner_ReturnsValidData()
    {
        if (!NativeLibraryAvailable)
            return;

        var rgbaData = CreateTestRGBA(800, 600);
        var webpData = WebPImage.EncodeLossless(rgbaData, 800, 600);

        var cropped = WebPImage.Crop(webpData, 0, 0, 400, 300, WEBP_CSP_MODE.MODE_RGBA);

        AssertImageData(cropped, "Cropping top-left");
        Assert.Equal(400 * 300 * 4, cropped.Length);
    }

    [Fact]
    [Trait("RequiresNativeLibrary", "true")]
    public void Flip_Vertical_ReturnsValidData()
    {
        if (!NativeLibraryAvailable)
            return;

        var rgbaData = CreateTestRGBA(800, 600);
        var webpData = WebPImage.EncodeLossless(rgbaData, 800, 600);

        var flipped = WebPImage.Flip(webpData, WEBP_CSP_MODE.MODE_RGBA);

        AssertImageData(flipped, "Flipping");
        Assert.Equal(rgbaData.Length, flipped.Length);
    }

    [Fact]
    [Trait("RequiresNativeLibrary", "true")]
    public void GetInfo_FromValidWebP_ReturnsCorrectInfo()
    {
        if (!NativeLibraryAvailable)
            return;

        var rgbaData = CreateTestRGBA(800, 600);
        var webpData = WebPImage.EncodeLossless(rgbaData, 800, 600);

        var info = WebPImage.GetInfo(webpData);

        Assert.NotNull(info);
        Assert.Equal(800, info.Width);
        Assert.Equal(600, info.Height);
    }

    [Fact]
    [Trait("RequiresNativeLibrary", "true")]
    public void GetInfo_FromStream_ReturnsCorrectInfo()
    {
        if (!NativeLibraryAvailable)
            return;

        var rgbaData = CreateTestRGBA(800, 600);
        var webpData = WebPImage.EncodeLossless(rgbaData, 800, 600);

        using var stream = new MemoryStream(webpData);
        var info = WebPImage.GetInfo(stream);

        Assert.NotNull(info);
        Assert.Equal(800, info.Width);
        Assert.Equal(600, info.Height);
    }

    [Fact]
    [Trait("RequiresNativeLibrary", "true")]
    public void IsValidWebP_ValidHeader_ReturnsTrue()
    {
        if (!NativeLibraryAvailable)
            return;

        var rgbaData = CreateTestRGBA(800, 600);
        var webpData = WebPImage.EncodeLossless(rgbaData, 800, 600);

        var isValid = WebPImage.IsValidWebP(webpData);

        Assert.True(isValid);
    }

    [Fact]
    public void IsValidWebP_InvalidHeader_ReturnsFalse()
    {
        var invalidData = new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 };

        var isValid = WebPImage.IsValidWebP(invalidData);

        Assert.False(isValid);
    }

    [Fact]
    [Trait("RequiresNativeLibrary", "true")]
    public void GetVersion_ReturnsValidVersion()
    {
        if (!NativeLibraryAvailable)
            return;

        var version = WebPImage.GetVersion();

        Assert.NotNull(version);
        Assert.True(version.Length > 0);
        Assert.Contains(".", version);
    }

    [Fact]
    [Trait("RequiresNativeLibrary", "true")]
    public void DecodeToFile_CreatesValidFile()
    {
        if (!NativeLibraryAvailable)
            return;

        var rgbaData = CreateTestRGBA(800, 600);
        var webpData = WebPImage.EncodeLossless(rgbaData, 800, 600);
        var outputPath = GetOutputFilePath("test-decoded.bin");

        File.Delete(outputPath);

        WebPImage.DecodeToFile(webpData, outputPath, WEBP_CSP_MODE.MODE_RGBA);

        AssertFileExists(outputPath);
        var fileData = File.ReadAllBytes(outputPath);
        Assert.True(fileData.Length > 0);
    }

    public void Dispose()
    {
        if (Directory.Exists(OutputPath))
        {
            try
            {
                foreach (var file in Directory.GetFiles(OutputPath))
                    File.Delete(file);
            }
            catch { }
        }
    }
}
