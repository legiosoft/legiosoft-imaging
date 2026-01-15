using System.Runtime.InteropServices;
using LegioSoft.Imaging.WebP.Enums;
using LegioSoft.Imaging.WebP.Models;
using LegioSoft.Imaging.WebP.Native;

namespace LegioSoft.Imaging.WebP.Decoder;

internal class WebPDecoder
{
    private const int WEBP_DECODER_ABI_VERSION = 0x0210;

    private const int MaxImageWidth = 16384;
    private const int MaxImageHeight = 16384;
    private const long MaxImageMemoryBytes = 512 * 1024 * 1024;

    public static WebPInfo GetInfo(byte[] webpData)
    {
        if (webpData == null || webpData.Length == 0)
        {
            throw new ArgumentException("WebP data cannot be null or empty", nameof(webpData));
        }

        var status = NativeMethods.WebPGetFeatures(webpData, (UIntPtr)webpData.Length, out var features);

        if (status != VP8StatusCode.OK)
        {
            throw new InvalidOperationException($"Failed to get WebP info: {status}");
        }

        var info = new WebPInfo
        {
            Width = features.width,
            Height = features.height,
            HasAlpha = features.has_alpha != 0,
            HasAnimation = features.has_animation != 0,
            Format = features.format switch
            {
                1 => WebPFormat.Lossy,
                2 => WebPFormat.Lossless,
                _ => WebPFormat.Mixed
            }
        };

        ValidateImageDimensions(info);

        return info;
    }

    public static WebPInfo GetInfo(Stream stream)
    {
        if (stream == null)
            throw new ArgumentNullException(nameof(stream));

        var data = ReadStream(stream);
        return GetInfo(data);
    }

    public static WebPInfo GetInfo(string filePath)
    {
        if (string.IsNullOrEmpty(filePath))
        {
            throw new ArgumentException("File path cannot be null or empty", nameof(filePath));
        }

        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException("WebP file not found", filePath);
        }

        var data = File.ReadAllBytes(filePath);
        return GetInfo(data);
    }

    public static byte[] Decode(byte[] webpData, out int width, out int height)
    {
        return Decode(webpData, out width, out height, WEBP_CSP_MODE.MODE_RGBA);
    }

    public static byte[] Decode(byte[] webpData, out int width, out int height, WEBP_CSP_MODE colorspace)
    {
        if (webpData == null || webpData.Length == 0)
            throw new ArgumentException("WebP data cannot be null or empty", nameof(webpData));

        var result = colorspace switch
        {
            WEBP_CSP_MODE.MODE_RGBA => NativeMethods.WebPDecodeRGBA(webpData, (UIntPtr)webpData.Length,
                out width, out height),
            WEBP_CSP_MODE.MODE_ARGB => NativeMethods.WebPDecodeARGB(webpData, (UIntPtr)webpData.Length,
                out width, out height),
            WEBP_CSP_MODE.MODE_BGRA => NativeMethods.WebPDecodeBGRA(webpData, (UIntPtr)webpData.Length,
                out width, out height),
            WEBP_CSP_MODE.MODE_RGB => NativeMethods.WebPDecodeRGB(webpData, (UIntPtr)webpData.Length,
                out width, out height),
            WEBP_CSP_MODE.MODE_BGR => NativeMethods.WebPDecodeBGR(webpData, (UIntPtr)webpData.Length,
                out width, out height),
            _ => NativeMethods.WebPDecodeRGBA(webpData, (UIntPtr)webpData.Length, out width, out height)
        };

        if (result == IntPtr.Zero)
        {
            throw new InvalidOperationException("Failed to decode WebP data");
        }

        try
        {
            int bytesPerPixel;
            if (colorspace is WEBP_CSP_MODE.MODE_RGB or WEBP_CSP_MODE.MODE_BGR)
            {
                bytesPerPixel = 3;
            }
            else
            {
                bytesPerPixel = 4;
            }

            ValidateDecodedDimensions(width, height);

            var size = width * height * bytesPerPixel;
            var decodedData = new byte[size];
            Marshal.Copy(result, decodedData, 0, size);
            return decodedData;
        }
        finally
        {
            NativeMethods.WebPFree(result);
        }
    }

    public static byte[] Decode(Stream stream, out int width, out int height)
    {
        return Decode(stream, out width, out height, WEBP_CSP_MODE.MODE_RGBA);
    }

    public static byte[] Decode(Stream stream, out int width, out int height, WEBP_CSP_MODE colorspace)
    {
        if (stream == null)
        {
            throw new ArgumentNullException(nameof(stream));
        }

        var data = ReadStream(stream);
        return Decode(data, out width, out height, colorspace);
    }

    public static byte[] Decode(string filePath, out int width, out int height)
    {
        return Decode(filePath, out width, out height, WEBP_CSP_MODE.MODE_RGBA);
    }

    public static byte[] Decode(string filePath, out int width, out int height, WEBP_CSP_MODE colorspace)
    {
        if (string.IsNullOrEmpty(filePath))
        {
            throw new ArgumentException("File path cannot be null or empty", nameof(filePath));
        }

        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException("WebP file not found", filePath);
        }

        var data = File.ReadAllBytes(filePath);
        return Decode(data, out width, out height, colorspace);
    }

    public static byte[] DecodeWithScaling(
        byte[] webpData,
        int scaledWidth,
        int scaledHeight,
        WEBP_CSP_MODE colorspace = WEBP_CSP_MODE.MODE_RGBA)
    {
        if (webpData == null || webpData.Length == 0)
        {
            throw new ArgumentException("WebP data cannot be null or empty", nameof(webpData));
        }

        if (scaledWidth <= 0 || scaledHeight <= 0)
        {
            throw new ArgumentException("Scaled dimensions must be positive", nameof(scaledWidth));
        }

        ValidateDecodedDimensions(scaledWidth, scaledHeight);

        var config = new WebPDecoderConfig();

        if (NativeMethods.WebPInitDecoderConfigInternal(ref config, WEBP_DECODER_ABI_VERSION) == 0)
        {
            throw new InvalidOperationException("Failed to initialize WebP decoder config (version mismatch)");
        }

        config.output.colorspace = colorspace;
        config.output.width = scaledWidth;
        config.output.height = scaledHeight;
        config.options.use_scaling = 1;
        config.options.scaled_width = scaledWidth;
        config.options.scaled_height = scaledHeight;

        try
        {
            var status = NativeMethods.WebPDecode(webpData, (UIntPtr)webpData.Length, ref config);

            if (status != VP8StatusCode.OK)
            {
                throw new InvalidOperationException($"Failed to decode WebP with scaling: {status}");
            }

            int bytesPerPixel;
            if (colorspace is WEBP_CSP_MODE.MODE_RGB or WEBP_CSP_MODE.MODE_BGR)
            {
                bytesPerPixel = 3;
            }
            else
            {
                bytesPerPixel = 4;
            }

            var size = scaledWidth * scaledHeight * bytesPerPixel;
            var decodedData = new byte[size];
            Marshal.Copy(config.output.u.RGBA.rgba, decodedData, 0, size);

            return decodedData;
        }
        finally
        {
            NativeMethods.WebPFreeDecBuffer(ref config.output);
        }
    }

    public static byte[] DecodeWithCropping(
        byte[] webpData,
        int cropLeft,
        int cropTop,
        int cropWidth,
        int cropHeight,
        WEBP_CSP_MODE colorspace = WEBP_CSP_MODE.MODE_RGBA)
    {
        if (webpData == null || webpData.Length == 0)
        {
            throw new ArgumentException("WebP data cannot be null or empty", nameof(webpData));
        }

        if (cropWidth <= 0 || cropHeight <= 0 || cropLeft < 0 || cropTop < 0)
        {
            throw new ArgumentException("Invalid crop parameters");
        }

        ValidateDecodedDimensions(cropWidth, cropHeight);

        var config = new WebPDecoderConfig();

        if (NativeMethods.WebPInitDecoderConfigInternal(ref config, WEBP_DECODER_ABI_VERSION) == 0)
        {
            throw new InvalidOperationException("Failed to initialize WebP decoder config (version mismatch)");
        }

        config.output.colorspace = colorspace;
        config.options.use_cropping = 1;
        config.options.crop_left = cropLeft;
        config.options.crop_top = cropTop;
        config.options.crop_width = cropWidth;
        config.options.crop_height = cropHeight;

        try
        {
            var status = NativeMethods.WebPDecode(webpData, (UIntPtr)webpData.Length, ref config);

            if (status != VP8StatusCode.OK)
            {
                throw new InvalidOperationException($"Failed to decode WebP with cropping: {status}");
            }

            int bytesPerPixel;
            if (colorspace is WEBP_CSP_MODE.MODE_RGB or WEBP_CSP_MODE.MODE_BGR)
            {
                bytesPerPixel = 3;
            }
            else
            {
                bytesPerPixel = 4;
            }

            var size = cropWidth * cropHeight * bytesPerPixel;
            var decodedData = new byte[size];
            Marshal.Copy(config.output.u.RGBA.rgba, decodedData, 0, size);

            return decodedData;
        }
        finally
        {
            NativeMethods.WebPFreeDecBuffer(ref config.output);
        }
    }

    public static byte[] DecodeWithFlip(byte[] webpData, WEBP_CSP_MODE colorspace = WEBP_CSP_MODE.MODE_RGBA)
    {
        if (webpData == null || webpData.Length == 0)
        {
            throw new ArgumentException("WebP data cannot be null or empty", nameof(webpData));
        }

        var info = GetInfo(webpData);

        var config = new WebPDecoderConfig();

        if (NativeMethods.WebPInitDecoderConfigInternal(ref config, WEBP_DECODER_ABI_VERSION) == 0)
        {
            throw new InvalidOperationException("Failed to initialize WebP decoder config (version mismatch)");
        }

        config.output.colorspace = colorspace;
        config.options.flip = 1;

        try
        {
            var status = NativeMethods.WebPDecode(webpData, (UIntPtr)webpData.Length, ref config);

            if (status != VP8StatusCode.OK)
            {
                throw new InvalidOperationException($"Failed to decode WebP with flip: {status}");
            }

            int bytesPerPixel;
            if (colorspace is WEBP_CSP_MODE.MODE_RGB or WEBP_CSP_MODE.MODE_BGR)
            {
                bytesPerPixel = 3;
            }
            else
            {
                bytesPerPixel = 4;
            }

            var size = info.Width * info.Height * bytesPerPixel;
            var decodedData = new byte[size];
            Marshal.Copy(config.output.u.RGBA.rgba, decodedData, 0, size);

            return decodedData;
        }
        finally
        {
            NativeMethods.WebPFreeDecBuffer(ref config.output);
        }
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

    private static void ValidateDecodedDimensions(int width, int height)
    {
        if (width <= 0 || height <= 0)
            throw new InvalidOperationException(
                $"Invalid image dimensions: {width}x{height}");

        if (width > MaxImageWidth || height > MaxImageHeight)
            throw new InvalidOperationException(
                $"Image dimensions {width}x{height} exceed maximum " +
                $"allowed size of {MaxImageWidth}x{MaxImageHeight}. " +
                $"This is a security limit to prevent memory exhaustion attacks.");

        long estimatedBytes;
        try
        {
            estimatedBytes = checked((long)width * height * 4);
        }
        catch (OverflowException)
        {
            throw new InvalidOperationException(
                $"Image dimensions {width}x{height} would cause integer overflow");
        }

        if (estimatedBytes > MaxImageMemoryBytes)
            throw new InvalidOperationException(
                $"Image would require approximately {FormatBytes(estimatedBytes)}, " +
                $"which exceeds the maximum allowed memory of {FormatBytes(MaxImageMemoryBytes)}. " +
                $"This is a security limit to prevent denial-of-service attacks.");
    }

    private static void ValidateImageDimensions(WebPInfo info)
    {
        if (info.Width <= 0 || info.Height <= 0)
            throw new InvalidOperationException(
                $"Invalid image dimensions: {info.Width}x{info.Height}");

        if (info.Width > MaxImageWidth || info.Height > MaxImageHeight)
            throw new InvalidOperationException(
                $"Image dimensions {info.Width}x{info.Height} exceed maximum " +
                $"allowed size of {MaxImageWidth}x{MaxImageHeight}. " +
                $"This is a security limit to prevent memory exhaustion attacks.");

        long estimatedBytes;
        try
        {
            estimatedBytes = checked((long)info.Width * info.Height * 4);
        }
        catch (OverflowException)
        {
            throw new InvalidOperationException(
                $"Image dimensions {info.Width}x{info.Height} would cause integer overflow");
        }

        if (estimatedBytes > MaxImageMemoryBytes)
            throw new InvalidOperationException(
                $"Image would require approximately {FormatBytes(estimatedBytes)}, " +
                $"which exceeds the maximum allowed memory of {FormatBytes(MaxImageMemoryBytes)}. " +
                $"This is a security limit to prevent denial-of-service attacks.");
    }

    private static string FormatBytes(long bytes)
    {
        const long KB = 1024;
        const long MB = KB * 1024;
        const long GB = MB * 1024;

        return bytes switch
        {
            >= GB => $"{(double)bytes / GB:F2} GB",
            >= MB => $"{(double)bytes / MB:F2} MB",
            >= KB => $"{(double)bytes / KB:F2} KB",
            _ => $"{bytes} bytes"
        };
    }
}