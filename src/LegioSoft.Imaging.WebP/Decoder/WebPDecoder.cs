using LegioSoft.Imaging.WebP.Models;

namespace LegioSoft.Imaging.WebP.Decoder;

public class WebPDecoder
{
    private const int WEBP_DECODER_ABI_VERSION = 0x0210;

    public static WebPInfo GetInfo(byte[] webpData)
    {
        if (webpData == null || webpData.Length == 0)
            throw new ArgumentException("WebP data cannot be null or empty", nameof(webpData));

        var status = Native.NativeMethods.WebPGetFeatures(webpData, (UIntPtr)webpData.Length, out var features);
        
        if (status != Enums.VP8StatusCode.OK)
            throw new InvalidOperationException($"Failed to get WebP info: {status}");

        return new WebPInfo
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
    }

    public static WebPInfo GetInfo(Stream stream)
    {
        if (stream == null)
            throw new ArgumentNullException(nameof(stream));

        byte[] data = ReadStream(stream);
        return GetInfo(data);
    }

    public static WebPInfo GetInfo(string filePath)
    {
        if (string.IsNullOrEmpty(filePath))
            throw new ArgumentException("File path cannot be null or empty", nameof(filePath));

        if (!File.Exists(filePath))
            throw new FileNotFoundException("WebP file not found", filePath);

        byte[] data = File.ReadAllBytes(filePath);
        return GetInfo(data);
    }

    public static byte[] Decode(byte[] webpData, out int width, out int height)
    {
        return Decode(webpData, out width, out height, Enums.WEBP_CSP_MODE.MODE_RGBA);
    }

    public static byte[] Decode(byte[] webpData, out int width, out int height, Enums.WEBP_CSP_MODE colorspace)
    {
        if (webpData == null || webpData.Length == 0)
            throw new ArgumentException("WebP data cannot be null or empty", nameof(webpData));

        IntPtr result = colorspace switch
        {
            Enums.WEBP_CSP_MODE.MODE_RGBA => Native.NativeMethods.WebPDecodeRGBA(webpData, (UIntPtr)webpData.Length, out width, out height),
            Enums.WEBP_CSP_MODE.MODE_ARGB => Native.NativeMethods.WebPDecodeARGB(webpData, (UIntPtr)webpData.Length, out width, out height),
            Enums.WEBP_CSP_MODE.MODE_BGRA => Native.NativeMethods.WebPDecodeBGRA(webpData, (UIntPtr)webpData.Length, out width, out height),
            Enums.WEBP_CSP_MODE.MODE_RGB => Native.NativeMethods.WebPDecodeRGB(webpData, (UIntPtr)webpData.Length, out width, out height),
            Enums.WEBP_CSP_MODE.MODE_BGR => Native.NativeMethods.WebPDecodeBGR(webpData, (UIntPtr)webpData.Length, out width, out height),
            _ => Native.NativeMethods.WebPDecodeRGBA(webpData, (UIntPtr)webpData.Length, out width, out height)
        };

        if (result == IntPtr.Zero)
            throw new InvalidOperationException("Failed to decode WebP data");

        try
        {
            int bytesPerPixel;
            if (colorspace == Enums.WEBP_CSP_MODE.MODE_RGB || colorspace == Enums.WEBP_CSP_MODE.MODE_BGR)
            {
                bytesPerPixel = 3;
            }
            else
            {
                bytesPerPixel = 4;
            }

            var size = width * height * bytesPerPixel;
            var decodedData = new byte[size];
            System.Runtime.InteropServices.Marshal.Copy(result, decodedData, 0, size);
            return decodedData;
        }
        finally
        {
            Native.NativeMethods.WebPFree(result);
        }
    }

    public static byte[] Decode(Stream stream, out int width, out int height)
    {
        return Decode(stream, out width, out height, Enums.WEBP_CSP_MODE.MODE_RGBA);
    }

    public static byte[] Decode(Stream stream, out int width, out int height, Enums.WEBP_CSP_MODE colorspace)
    {
        if (stream == null)
            throw new ArgumentNullException(nameof(stream));

        byte[] data = ReadStream(stream);
        return Decode(data, out width, out height, colorspace);
    }

    public static byte[] Decode(string filePath, out int width, out int height)
    {
        return Decode(filePath, out width, out height, Enums.WEBP_CSP_MODE.MODE_RGBA);
    }

    public static byte[] Decode(string filePath, out int width, out int height, Enums.WEBP_CSP_MODE colorspace)
    {
        if (string.IsNullOrEmpty(filePath))
            throw new ArgumentException("File path cannot be null or empty", nameof(filePath));

        if (!File.Exists(filePath))
            throw new FileNotFoundException("WebP file not found", filePath);

        byte[] data = File.ReadAllBytes(filePath);
        return Decode(data, out width, out height, colorspace);
    }

    public static byte[] DecodeWithScaling(byte[] webpData, int scaledWidth, int scaledHeight, Enums.WEBP_CSP_MODE colorspace = Enums.WEBP_CSP_MODE.MODE_RGBA)
    {
        if (webpData == null || webpData.Length == 0)
            throw new ArgumentException("WebP data cannot be null or empty", nameof(webpData));

        if (scaledWidth <= 0 || scaledHeight <= 0)
            throw new ArgumentException("Scaled dimensions must be positive", nameof(scaledWidth));

        var config = new Native.WebPDecoderConfig();
        
        if (Native.NativeMethods.WebPInitDecoderConfigInternal(ref config, WEBP_DECODER_ABI_VERSION) == 0)
            throw new InvalidOperationException("Failed to initialize WebP decoder config (version mismatch)");

        config.output.colorspace = colorspace;
        config.output.width = scaledWidth;
        config.output.height = scaledHeight;
        config.options.use_scaling = 1;
        config.options.scaled_width = scaledWidth;
        config.options.scaled_height = scaledHeight;

        try
        {
            var status = Native.NativeMethods.WebPDecode(webpData, (UIntPtr)webpData.Length, ref config);
            
            if (status != Enums.VP8StatusCode.OK)
                throw new InvalidOperationException($"Failed to decode WebP with scaling: {status}");

            int bytesPerPixel;
            if (colorspace == Enums.WEBP_CSP_MODE.MODE_RGB || colorspace == Enums.WEBP_CSP_MODE.MODE_BGR)
            {
                bytesPerPixel = 3;
            }
            else
            {
                bytesPerPixel = 4;
            }

            var size = scaledWidth * scaledHeight * bytesPerPixel;
            var decodedData = new byte[size];
            System.Runtime.InteropServices.Marshal.Copy(config.output.rgba.rgba, decodedData, 0, size);
            
            return decodedData;
        }
        finally
        {
            Native.NativeMethods.WebPFreeDecBuffer(ref config.output);
        }
    }

    public static byte[] DecodeWithCropping(byte[] webpData, int cropLeft, int cropTop, int cropWidth, int cropHeight, Enums.WEBP_CSP_MODE colorspace = Enums.WEBP_CSP_MODE.MODE_RGBA)
    {
        if (webpData == null || webpData.Length == 0)
            throw new ArgumentException("WebP data cannot be null or empty", nameof(webpData));

        if (cropWidth <= 0 || cropHeight <= 0 || cropLeft < 0 || cropTop < 0)
            throw new ArgumentException("Invalid crop parameters");

        var config = new Native.WebPDecoderConfig();
        
        if (Native.NativeMethods.WebPInitDecoderConfigInternal(ref config, WEBP_DECODER_ABI_VERSION) == 0)
            throw new InvalidOperationException("Failed to initialize WebP decoder config (version mismatch)");

        config.output.colorspace = colorspace;
        config.options.use_cropping = 1;
        config.options.crop_left = cropLeft;
        config.options.crop_top = cropTop;
        config.options.crop_width = cropWidth;
        config.options.crop_height = cropHeight;

        try
        {
            var status = Native.NativeMethods.WebPDecode(webpData, (UIntPtr)webpData.Length, ref config);
            
            if (status != Enums.VP8StatusCode.OK)
                throw new InvalidOperationException($"Failed to decode WebP with cropping: {status}");

            int bytesPerPixel;
            if (colorspace == Enums.WEBP_CSP_MODE.MODE_RGB || colorspace == Enums.WEBP_CSP_MODE.MODE_BGR)
            {
                bytesPerPixel = 3;
            }
            else
            {
                bytesPerPixel = 4;
            }

            var size = cropWidth * cropHeight * bytesPerPixel;
            var decodedData = new byte[size];
            System.Runtime.InteropServices.Marshal.Copy(config.output.rgba.rgba, decodedData, 0, size);
            
            return decodedData;
        }
        finally
        {
            Native.NativeMethods.WebPFreeDecBuffer(ref config.output);
        }
    }

    public static byte[] DecodeWithFlip(byte[] webpData, Enums.WEBP_CSP_MODE colorspace = Enums.WEBP_CSP_MODE.MODE_RGBA)
    {
        if (webpData == null || webpData.Length == 0)
            throw new ArgumentException("WebP data cannot be null or empty", nameof(webpData));

        var info = GetInfo(webpData);

        var config = new Native.WebPDecoderConfig();
        
        if (Native.NativeMethods.WebPInitDecoderConfigInternal(ref config, WEBP_DECODER_ABI_VERSION) == 0)
            throw new InvalidOperationException("Failed to initialize WebP decoder config (version mismatch)");

        config.output.colorspace = colorspace;
        config.options.flip = 1;

        try
        {
            var status = Native.NativeMethods.WebPDecode(webpData, (UIntPtr)webpData.Length, ref config);
            
            if (status != Enums.VP8StatusCode.OK)
                throw new InvalidOperationException($"Failed to decode WebP with flip: {status}");

            int bytesPerPixel;
            if (colorspace == Enums.WEBP_CSP_MODE.MODE_RGB || colorspace == Enums.WEBP_CSP_MODE.MODE_BGR)
            {
                bytesPerPixel = 3;
            }
            else
            {
                bytesPerPixel = 4;
            }

            var size = info.Width * info.Height * bytesPerPixel;
            var decodedData = new byte[size];
            System.Runtime.InteropServices.Marshal.Copy(config.output.rgba.rgba, decodedData, 0, size);
            
            return decodedData;
        }
        finally
        {
            Native.NativeMethods.WebPFreeDecBuffer(ref config.output);
        }
    }

    private static byte[] ReadStream(Stream stream)
    {
        if (stream.CanSeek)
        {
            stream.Position = 0;
        }

        using (var memoryStream = new MemoryStream())
        {
            stream.CopyTo(memoryStream);
            return memoryStream.ToArray();
        }
    }
}

