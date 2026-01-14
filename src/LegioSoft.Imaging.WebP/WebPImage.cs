using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using LegioSoft.Imaging.WebP.Decoder;
using LegioSoft.Imaging.WebP.Encoder;
using LegioSoft.Imaging.WebP.Enums;
using LegioSoft.Imaging.WebP.Models;
using LegioSoft.Imaging.WebP.Helpers;
using LegioSoft.Imaging.WebP.Native;

namespace LegioSoft.Imaging.WebP;

public static class WebPImage
{
    private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".webp"
    };

    private static readonly string[] AllowedDirectories = Array.Empty<string>();
    public static byte[] Encode(byte[] rgbaData, int width, int height, float quality = 75.0f)
    {
        return WebPEncoder.Encode(rgbaData, width, height, quality, false);
    }

    public static byte[] Encode(byte[] rgbaData, int width, int height, bool lossless)
    {
        float quality;
        if (lossless)
        {
                quality = 100.0f;
        }
        else
        {
                quality = 75.0f;
        }
        return WebPEncoder.Encode(rgbaData, width, height, quality, lossless);
    }

    public static byte[] EncodeRGB(byte[] rgbData, int width, int height, float quality = 75.0f)
    {
        return WebPEncoder.EncodeRGB(rgbData, width, height, quality, false);
    }

    public static byte[] EncodeLossless(byte[] rgbaData, int width, int height)
    {
        return WebPEncoder.Encode(rgbaData, width, height, 100.0f, true);
    }

    public static byte[] EncodeLosslessRGB(byte[] rgbData, int width, int height)
    {
        return WebPEncoder.EncodeRGB(rgbData, width, height, 100.0f, true);
    }

    public static byte[] EncodeAdvanced(byte[] imageData, int width, int height, WebPEncodeOptions options)
    {
        return WebPEncoder.EncodeAdvanced(imageData, width, height, options);
    }

    public static void EncodeToFile(string inputPath, string outputPath, float quality = 75.0f)
    {
        var rgbaData = File.ReadAllBytes(inputPath);
        var info = GetImageDimensions(rgbaData);
        var webpData = Encode(rgbaData, info.Width, info.Height, quality);
        File.WriteAllBytes(outputPath, webpData);
    }

    public static void EncodeToFileRGBA(byte[] rgbaData, int width, int height, string outputPath, float quality = 75.0f)
    {
        var webpData = Encode(rgbaData, width, height, quality);
        File.WriteAllBytes(outputPath, webpData);
    }

    public static byte[] Decode(string filePath)
    {
        return Decode(filePath, WEBP_CSP_MODE.MODE_RGBA);
    }

    public static byte[] Decode(string filePath, WEBP_CSP_MODE colorspace)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("File path cannot be null or empty", nameof(filePath));

        ValidateFilePath(filePath);

        if (!File.Exists(filePath))
            throw new FileNotFoundException("WebP file not found", filePath);

        var data = File.ReadAllBytes(filePath);
        return WebPDecoder.Decode(data, out _, out _, colorspace);
    }

    public static byte[] Decode(Stream stream)
    {
        return Decode(stream, WEBP_CSP_MODE.MODE_RGBA);
    }

    public static byte[] Decode(Stream stream, WEBP_CSP_MODE colorspace)
    {
        if (stream == null)
            throw new ArgumentNullException(nameof(stream));

        var data = ReadStream(stream);
        return WebPDecoder.Decode(data, out _, out _, colorspace);
    }

    public static byte[] Decode(byte[] webpData)
    {
        return Decode(webpData, WEBP_CSP_MODE.MODE_RGBA);
    }

    public static byte[] Decode(byte[] webpData, WEBP_CSP_MODE colorspace)
    {
        if (webpData == null || webpData.Length == 0)
            throw new ArgumentException("WebP data cannot be null or empty", nameof(webpData));

        return WebPDecoder.Decode(webpData, out _, out _, colorspace);
    }

    public static void DecodeToFile(string inputPath, string outputPath)
    {
        var decodedData = Decode(inputPath);
        File.WriteAllBytes(outputPath, decodedData);
    }

    public static void DecodeToFile(byte[] webpData, string outputPath, WEBP_CSP_MODE colorspace = WEBP_CSP_MODE.MODE_RGBA)
    {
        var decodedData = Decode(webpData, colorspace);
        File.WriteAllBytes(outputPath, decodedData);
    }

    public static WebPInfo GetInfo(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("File path cannot be null or empty", nameof(filePath));

        ValidateFilePath(filePath);

        if (!File.Exists(filePath))
            throw new FileNotFoundException("WebP file not found", filePath);

        var data = File.ReadAllBytes(filePath);
        return WebPDecoder.GetInfo(data);
    }

    public static WebPInfo GetInfo(Stream stream)
    {
        if (stream == null)
            throw new ArgumentNullException(nameof(stream));

        var data = ReadStream(stream);
        return WebPDecoder.GetInfo(data);
    }

    public static WebPInfo GetInfo(byte[] webpData)
    {
        if (webpData == null || webpData.Length == 0)
            throw new ArgumentException("WebP data cannot be null or empty", nameof(webpData));

        return WebPDecoder.GetInfo(webpData);
    }

    public static byte[] Scale(byte[] webpData, int targetWidth, int targetHeight, WEBP_CSP_MODE colorspace = WEBP_CSP_MODE.MODE_RGBA)
    {
        if (webpData == null || webpData.Length == 0)
            throw new ArgumentException("WebP data cannot be null or empty", nameof(webpData));

        return WebPDecoder.DecodeWithScaling(webpData, targetWidth, targetHeight, colorspace);
    }

    public static byte[] Scale(string filePath, int targetWidth, int targetHeight, WEBP_CSP_MODE colorspace = WEBP_CSP_MODE.MODE_RGBA)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("File path cannot be null or empty", nameof(filePath));

        ValidateFilePath(filePath);

        var data = File.ReadAllBytes(filePath);
        return Scale(data, targetWidth, targetHeight, colorspace);
    }

    public static byte[] Scale(Stream stream, int targetWidth, int targetHeight, WEBP_CSP_MODE colorspace = WEBP_CSP_MODE.MODE_RGBA)
    {
        var data = ReadStream(stream);
        return Scale(data, targetWidth, targetHeight, colorspace);
    }

    public static byte[] Crop(byte[] webpData, int cropX, int cropY, int cropWidth, int cropHeight, WEBP_CSP_MODE colorspace = WEBP_CSP_MODE.MODE_RGBA)
    {
        if (webpData == null || webpData.Length == 0)
            throw new ArgumentException("WebP data cannot be null or empty", nameof(webpData));

        return WebPDecoder.DecodeWithCropping(webpData, cropX, cropY, cropWidth, cropHeight, colorspace);
    }

    public static byte[] Crop(string filePath, int cropX, int cropY, int cropWidth, int cropHeight, WEBP_CSP_MODE colorspace = WEBP_CSP_MODE.MODE_RGBA)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("File path cannot be null or empty", nameof(filePath));

        ValidateFilePath(filePath);

        var data = File.ReadAllBytes(filePath);
        return Crop(data, cropX, cropY, cropWidth, cropHeight, colorspace);
    }

    public static byte[] Crop(Stream stream, int cropX, int cropY, int cropWidth, int cropHeight, WEBP_CSP_MODE colorspace = WEBP_CSP_MODE.MODE_RGBA)
    {
        var data = ReadStream(stream);
        return Crop(data, cropX, cropY, cropWidth, cropHeight, colorspace);
    }

    public static byte[] Flip(byte[] webpData, WEBP_CSP_MODE colorspace = WEBP_CSP_MODE.MODE_RGBA)
    {
        if (webpData == null || webpData.Length == 0)
            throw new ArgumentException("WebP data cannot be null or empty", nameof(webpData));

        return WebPDecoder.DecodeWithFlip(webpData, colorspace);
    }

    public static byte[] Flip(string filePath, WEBP_CSP_MODE colorspace = WEBP_CSP_MODE.MODE_RGBA)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("File path cannot be null or empty", nameof(filePath));

        ValidateFilePath(filePath);

        var data = File.ReadAllBytes(filePath);
        return Flip(data, colorspace);
    }

    public static byte[] Flip(Stream stream, WEBP_CSP_MODE colorspace = WEBP_CSP_MODE.MODE_RGBA)
    {
        var data = ReadStream(stream);
        return Flip(data, colorspace);
    }

    public static byte[] EncodeWithScaling(byte[] rgbaData, int width, int height, int targetWidth, int targetHeight, float quality = 75.0f)
    {
        return WebPEncoder.EncodeWithScaling(rgbaData, width, height, targetWidth, targetHeight, quality);
    }

    public static byte[] EncodeWithCropping(byte[] rgbaData, int width, int height, int cropX, int cropY, int cropWidth, int cropHeight, float quality = 75.0f)
    {
        return WebPEncoder.EncodeWithCropping(rgbaData, width, height, cropX, cropY, cropWidth, cropHeight, quality);
    }

    public static bool IsValidWebP(byte[] data)
    {
        return WebPValidationHelper.IsWebP(data);
    }

    public static string GetVersion()
    {
        var version = NativeMethods.WebPGetDecoderVersion();
        var major = (version >> 16) & 0xFF;
        var minor = (version >> 8) & 0xFF;
        var patch = version & 0xFF;
        return $"{major}.{minor}.{patch}";
    }

    private static byte[] ReadStream(Stream stream)
    {
        if (stream == null)
            throw new ArgumentNullException(nameof(stream));

        if (!stream.CanRead)
            throw new ArgumentException("Stream must be readable", nameof(stream));

        long originalPosition = 0;
        bool canSeek = false;

        if (stream.CanSeek)
        {
            try
            {
                originalPosition = stream.Position;
                stream.Seek(0, SeekOrigin.Begin);
                canSeek = true;
            }
            catch (IOException)
            {
                canSeek = false;
            }
        }

        try
        {
            using var memoryStream = new MemoryStream();
            stream.CopyTo(memoryStream);
            return memoryStream.ToArray();
        }
        finally
        {
            if (canSeek && stream.CanSeek)
            {
                try
                {
                    stream.Seek(originalPosition, SeekOrigin.Begin);
                }
                catch (IOException)
                {
                }
            }
        }
    }

    private static void ValidateFilePath(string filePath)
    {
        var extension = Path.GetExtension(filePath);
        if (!AllowedExtensions.Contains(extension))
            throw new ArgumentException(
                $"File format '{extension}' is not supported. " +
                $"Allowed formats: {string.Join(", ", AllowedExtensions)}",
                nameof(filePath));

        if (AllowedDirectories.Length == 0)
            return;

        string fullPath;
        try
        {
            fullPath = Path.GetFullPath(filePath);
        }
        catch (Exception ex)
        {
            throw new ArgumentException(
                $"Invalid file path: {ex.Message}",
                nameof(filePath), ex);
        }

        bool isAllowed = AllowedDirectories.Any(dir =>
        {
            try
            {
                var fullDir = Path.GetFullPath(dir);
                return fullPath.StartsWith(fullDir + Path.DirectorySeparatorChar,
                    StringComparison.Ordinal);
            }
            catch
            {
                return false;
            }
        });

        if (!isAllowed)
        {
            throw new UnauthorizedAccessException(
                $"File path '{fullPath}' is outside the allowed directories: " +
                $"{string.Join(", ", AllowedDirectories)}. " +
                $"Configure AllowedDirectories for your security policy.");
        }

        try
        {
            var fileInfo = new FileInfo(fullPath);
            if (fileInfo.LinkTarget != null)
            {
                throw new UnauthorizedAccessException(
                    $"Symbolic links are not allowed for security reasons. " +
                    $"File '{fullPath}' is a symbolic link.");
            }
        }
        catch (UnauthorizedAccessException)
        {
            throw;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(
                $"Warning: Could not check symbolic link status: {ex.Message}");
        }
    }

    private static WebPInfo GetImageDimensions(byte[] rgbaData)
    {
        var pixelCount = rgbaData.Length / 4;
        var size = (int)Math.Sqrt(pixelCount);
        return new WebPInfo
        {
            Width = size,
            Height = size,
            HasAlpha = true,
            Format = WebPFormat.Lossless
        };
    }
}
