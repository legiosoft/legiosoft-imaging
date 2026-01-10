using System.Runtime.InteropServices;
using LegioSoft.Imaging.WebP.Enums;
using LegioSoft.Imaging.WebP.Models;

namespace LegioSoft.Imaging.WebP.Encoder;

public class WebPEncoder
{
    private const int WEBP_ENCODER_ABI_VERSION = 0x0210;

    public static byte[] Encode(byte[] rgbaData, int width, int height, float quality = 75.0f)
    {
        return Encode(rgbaData, width, height, quality, false);
    }

    public static byte[] Encode(byte[] rgbaData, int width, int height, float quality, bool lossless)
    {
        if (rgbaData == null || rgbaData.Length == 0)
            throw new ArgumentException("Image data cannot be null or empty", nameof(rgbaData));

        if (width <= 0 || height <= 0)
            throw new ArgumentException("Width and height must be positive", nameof(width));

        if (quality < 0 || quality > 100)
            throw new ArgumentException("Quality must be between 0 and 100", nameof(quality));

        var stride = width * 4;
        var outputPtr = IntPtr.Zero;

        try
        {
            UIntPtr size;
            
            if (lossless)
            {
                size = Native.NativeMethods.WebPEncodeLosslessRGBA(rgbaData, width, height, stride, out outputPtr);
            }
            else
            {
                size = Native.NativeMethods.WebPEncodeRGBA(rgbaData, width, height, stride, quality, out outputPtr);
            }

            if (size == UIntPtr.Zero || outputPtr == IntPtr.Zero)
                throw new InvalidOperationException("Failed to encode image to WebP");

            var result = new byte[(int)size];
            Marshal.Copy(outputPtr, result, 0, (int)size);
            return result;
        }
        finally
        {
            if (outputPtr != IntPtr.Zero)
                Native.NativeMethods.WebPFree(outputPtr);
        }
    }

    public static byte[] EncodeRGB(byte[] rgbData, int width, int height, float quality = 75.0f)
    {
        return EncodeRGB(rgbData, width, height, quality, false);
    }

    public static byte[] EncodeRGB(byte[] rgbData, int width, int height, float quality, bool lossless)
    {
        if (rgbData == null || rgbData.Length == 0)
            throw new ArgumentException("Image data cannot be null or empty", nameof(rgbData));

        if (width <= 0 || height <= 0)
            throw new ArgumentException("Width and height must be positive", nameof(width));

        if (quality < 0 || quality > 100)
            throw new ArgumentException("Quality must be between 0 and 100", nameof(quality));

        var stride = width * 3;
        var outputPtr = IntPtr.Zero;

        try
        {
            UIntPtr size;
            
            if (lossless)
            {
                size = Native.NativeMethods.WebPEncodeLosslessRGB(rgbData, width, height, stride, out outputPtr);
            }
            else
            {
                size = Native.NativeMethods.WebPEncodeRGB(rgbData, width, height, stride, quality, out outputPtr);
            }

            if (size == UIntPtr.Zero || outputPtr == IntPtr.Zero)
                throw new InvalidOperationException("Failed to encode RGB image to WebP");

            var result = new byte[(int)size];
            Marshal.Copy(outputPtr, result, 0, (int)size);
            return result;
        }
        finally
        {
            if (outputPtr != IntPtr.Zero)
                Native.NativeMethods.WebPFree(outputPtr);
        }
    }

    public static byte[] EncodeBGR(byte[] bgrData, int width, int height, float quality = 75.0f)
    {
        return EncodeBGR(bgrData, width, height, quality, false);
    }

    public static byte[] EncodeBGR(byte[] bgrData, int width, int height, float quality, bool lossless)
    {
        if (bgrData == null || bgrData.Length == 0)
            throw new ArgumentException("Image data cannot be null or empty", nameof(bgrData));

        if (width <= 0 || height <= 0)
            throw new ArgumentException("Width and height must be positive", nameof(width));

        if (quality < 0 || quality > 100)
            throw new ArgumentException("Quality must be between 0 and 100", nameof(quality));

        var stride = width * 3;
        var outputPtr = IntPtr.Zero;

        try
        {
            UIntPtr size;
            
            if (lossless)
            {
                size = Native.NativeMethods.WebPEncodeLosslessBGR(bgrData, width, height, stride, out outputPtr);
            }
            else
            {
                size = Native.NativeMethods.WebPEncodeBGR(bgrData, width, height, stride, quality, out outputPtr);
            }

            if (size == UIntPtr.Zero || outputPtr == IntPtr.Zero)
                throw new InvalidOperationException("Failed to encode BGR image to WebP");

            var result = new byte[(int)size];
            Marshal.Copy(outputPtr, result, 0, (int)size);
            return result;
        }
        finally
        {
            if (outputPtr != IntPtr.Zero)
                Native.NativeMethods.WebPFree(outputPtr);
        }
    }

    public static byte[] EncodeBGRA(byte[] bgraData, int width, int height, float quality = 75.0f)
    {
        return EncodeBGRA(bgraData, width, height, quality, false);
    }

    public static byte[] EncodeBGRA(byte[] bgraData, int width, int height, float quality, bool lossless)
    {
        if (bgraData == null || bgraData.Length == 0)
            throw new ArgumentException("Image data cannot be null or empty", nameof(bgraData));

        if (width <= 0 || height <= 0)
            throw new ArgumentException("Width and height must be positive", nameof(width));

        if (quality < 0 || quality > 100)
            throw new ArgumentException("Quality must be between 0 and 100", nameof(quality));

        var stride = width * 4;
        var outputPtr = IntPtr.Zero;

        try
        {
            UIntPtr size;
            
            if (lossless)
            {
                size = Native.NativeMethods.WebPEncodeLosslessBGRA(bgraData, width, height, stride, out outputPtr);
            }
            else
            {
                size = Native.NativeMethods.WebPEncodeBGRA(bgraData, width, height, stride, quality, out outputPtr);
            }

            if (size == UIntPtr.Zero || outputPtr == IntPtr.Zero)
                throw new InvalidOperationException("Failed to encode BGRA image to WebP");

            var result = new byte[(int)size];
            Marshal.Copy(outputPtr, result, 0, (int)size);
            return result;
        }
        finally
        {
            if (outputPtr != IntPtr.Zero)
                Native.NativeMethods.WebPFree(outputPtr);
        }
    }

    public static byte[] EncodeAdvanced(byte[] imageData, int width, int height, WebPEncodeOptions? options = null)
    {
        if (imageData == null || imageData.Length == 0)
            throw new ArgumentException("Image data cannot be null or empty", nameof(imageData));

        if (width <= 0 || height <= 0)
            throw new ArgumentException("Width and height must be positive", nameof(width));

        if (options == null)
            options = new WebPEncodeOptions();

        var config = new Native.WebPConfig();
        
        if (Native.NativeMethods.WebPConfigInitInternal(ref config, options.Preset, options.Quality, WEBP_ENCODER_ABI_VERSION) == 0)
            throw new InvalidOperationException("Failed to initialize WebP encoder config (version mismatch)");

        ApplyEncodeOptions(ref config, options);

        if (Native.NativeMethods.WebPValidateConfig(ref config) == 0)
            throw new InvalidOperationException("Invalid WebP encoder configuration");

        var pic = new Native.WebPPicture();
        
        if (Native.NativeMethods.WebPPictureInitInternal(ref pic, WEBP_ENCODER_ABI_VERSION) == 0)
            throw new InvalidOperationException("Failed to initialize WebP picture (version mismatch)");

        pic.use_argb = 1;
        pic.width = width;
        pic.height = height;

        try
        {
            if (Native.NativeMethods.WebPPictureAlloc(ref pic) == 0)
                throw new InvalidOperationException("Failed to allocate WebP picture");

            var stride = width * 4;
            var importResult = options.InputFormat switch
            {
                WebPInputFormat.RGBA => Native.NativeMethods.WebPPictureImportRGBA(ref pic, imageData, stride),
                WebPInputFormat.BGRA => Native.NativeMethods.WebPPictureImportBGRA(ref pic, imageData, stride),
                WebPInputFormat.RGB => Native.NativeMethods.WebPPictureImportRGB(ref pic, imageData, stride),
                WebPInputFormat.BGR => Native.NativeMethods.WebPPictureImportBGR(ref pic, imageData, stride),
                _ => Native.NativeMethods.WebPPictureImportRGBA(ref pic, imageData, stride)
            };

            if (importResult == 0)
                throw new InvalidOperationException("Failed to import image data into WebP picture");

            var writer = new Native.WebPMemoryWriter();
            Native.NativeMethods.WebPMemoryWriterInit(ref writer);
            
            var writerPtr = Marshal.AllocHGlobal(Marshal.SizeOf<Native.WebPMemoryWriter>());
            Marshal.StructureToPtr(writer, writerPtr, false);
            
            pic.writer = Marshal.GetFunctionPointerForDelegate<Native.NativeMethods.WebPWriterFunction>(Native.NativeMethods.WebPMemoryWrite);
            pic.custom_ptr = writerPtr;

            var encodeResult = Native.NativeMethods.WebPEncode(ref config, ref pic);
            
            if (encodeResult == 0)
                throw new InvalidOperationException($"Failed to encode image to WebP: {pic.error_code}");

            var result = new byte[(int)writer.size];
            Marshal.Copy(writer.mem, result, 0, (int)writer.size);
            Native.NativeMethods.WebPMemoryWriterClear(ref writer);
            
            return result;
        }
        finally
        {
            if (pic.custom_ptr != IntPtr.Zero)
                Marshal.FreeHGlobal(pic.custom_ptr);
            
            Native.NativeMethods.WebPPictureFree(ref pic);
        }
    }

    public static byte[] EncodeWithScaling(byte[] rgbaData, int width, int height, int targetWidth, int targetHeight, float quality = 75.0f)
    {
        if (rgbaData == null || rgbaData.Length == 0)
            throw new ArgumentException("Image data cannot be null or empty", nameof(rgbaData));

        if (width <= 0 || height <= 0 || targetWidth <= 0 || targetHeight <= 0)
            throw new ArgumentException("Width and height must be positive", nameof(width));

        var pic = new Native.WebPPicture();
        
        if (Native.NativeMethods.WebPPictureInitInternal(ref pic, WEBP_ENCODER_ABI_VERSION) == 0)
            throw new InvalidOperationException("Failed to initialize WebP picture (version mismatch)");

        pic.use_argb = 1;
        pic.width = width;
        pic.height = height;

        var config = new Native.WebPConfig();
        
        if (Native.NativeMethods.WebPConfigInitInternal(ref config, WebPPreset.DEFAULT, quality, WEBP_ENCODER_ABI_VERSION) == 0)
            throw new InvalidOperationException("Failed to initialize WebP encoder config (version mismatch)");

        try
        {
            if (Native.NativeMethods.WebPPictureAlloc(ref pic) == 0)
                throw new InvalidOperationException("Failed to allocate WebP picture");

            var stride = width * 4;
            if (Native.NativeMethods.WebPPictureImportRGBA(ref pic, rgbaData, stride) == 0)
                throw new InvalidOperationException("Failed to import image data into WebP picture");

            if (Native.NativeMethods.WebPPictureRescale(ref pic, targetWidth, targetHeight) == 0)
                throw new InvalidOperationException("Failed to scale WebP picture");

            var writer = new Native.WebPMemoryWriter();
            Native.NativeMethods.WebPMemoryWriterInit(ref writer);
            
            var writerPtr = Marshal.AllocHGlobal(Marshal.SizeOf<Native.WebPMemoryWriter>());
            Marshal.StructureToPtr(writer, writerPtr, false);
            
            pic.writer = Marshal.GetFunctionPointerForDelegate<Native.NativeMethods.WebPWriterFunction>(Native.NativeMethods.WebPMemoryWrite);
            pic.custom_ptr = writerPtr;

            var encodeResult = Native.NativeMethods.WebPEncode(ref config, ref pic);
            
            if (encodeResult == 0)
                throw new InvalidOperationException($"Failed to encode image to WebP: {pic.error_code}");

            var result = new byte[(int)writer.size];
            Marshal.Copy(writer.mem, result, 0, (int)writer.size);
            Native.NativeMethods.WebPMemoryWriterClear(ref writer);
            
            return result;
        }
        finally
        {
            if (pic.custom_ptr != IntPtr.Zero)
                Marshal.FreeHGlobal(pic.custom_ptr);
            
            Native.NativeMethods.WebPPictureFree(ref pic);
        }
    }

    public static byte[] EncodeWithCropping(byte[] rgbaData, int width, int height, int cropX, int cropY, int cropWidth, int cropHeight, float quality = 75.0f)
    {
        if (rgbaData == null || rgbaData.Length == 0)
            throw new ArgumentException("Image data cannot be null or empty", nameof(rgbaData));

        if (cropWidth <= 0 || cropHeight <= 0 || cropX < 0 || cropY < 0)
            throw new ArgumentException("Invalid crop parameters", nameof(cropWidth));

        var pic = new Native.WebPPicture();
        
        if (Native.NativeMethods.WebPPictureInitInternal(ref pic, WEBP_ENCODER_ABI_VERSION) == 0)
            throw new InvalidOperationException("Failed to initialize WebP picture (version mismatch)");

        pic.use_argb = 1;
        pic.width = width;
        pic.height = height;

        var config = new Native.WebPConfig();
        
        if (Native.NativeMethods.WebPConfigInitInternal(ref config, WebPPreset.DEFAULT, quality, WEBP_ENCODER_ABI_VERSION) == 0)
            throw new InvalidOperationException("Failed to initialize WebP encoder config (version mismatch)");

        try
        {
            if (Native.NativeMethods.WebPPictureAlloc(ref pic) == 0)
                throw new InvalidOperationException("Failed to allocate WebP picture");

            var stride = width * 4;
            if (Native.NativeMethods.WebPPictureImportRGBA(ref pic, rgbaData, stride) == 0)
                throw new InvalidOperationException("Failed to import image data into WebP picture");

            if (Native.NativeMethods.WebPPictureCrop(ref pic, cropX, cropY, cropWidth, cropHeight) == 0)
                throw new InvalidOperationException("Failed to crop WebP picture");

            var writer = new Native.WebPMemoryWriter();
            Native.NativeMethods.WebPMemoryWriterInit(ref writer);
            
            var writerPtr = Marshal.AllocHGlobal(Marshal.SizeOf<Native.WebPMemoryWriter>());
            Marshal.StructureToPtr(writer, writerPtr, false);
            
            pic.writer = Marshal.GetFunctionPointerForDelegate<Native.NativeMethods.WebPWriterFunction>(Native.NativeMethods.WebPMemoryWrite);
            pic.custom_ptr = writerPtr;

            var encodeResult = Native.NativeMethods.WebPEncode(ref config, ref pic);
            
            if (encodeResult == 0)
                throw new InvalidOperationException($"Failed to encode image to WebP: {pic.error_code}");

            var result = new byte[(int)writer.size];
            Marshal.Copy(writer.mem, result, 0, (int)writer.size);
            Native.NativeMethods.WebPMemoryWriterClear(ref writer);
            
            return result;
        }
        finally
        {
            if (pic.custom_ptr != IntPtr.Zero)
                Marshal.FreeHGlobal(pic.custom_ptr);
            
            Native.NativeMethods.WebPPictureFree(ref pic);
        }
    }

    private static void ApplyEncodeOptions(ref Native.WebPConfig config, WebPEncodeOptions options)
    {
        if (options.Lossless)
        {
            config.lossless = 1;
        }
        else
        {
            config.lossless = 0;
        }

        config.quality = options.Quality;
        config.method = options.Method;
        config.image_hint = options.ImageHint;
        config.target_size = options.TargetSize;
        config.target_PSNR = options.TargetPSNR;
        config.segments = options.Segments;
        config.sns_strength = options.SnsStrength;
        config.filter_strength = options.FilterStrength;
        config.filter_sharpness = options.FilterSharpness;
        config.filter_type = options.FilterType;

        if (options.Autofilter)
        {
            config.autofilter = 1;
        }
        else
        {
            config.autofilter = 0;
        }

        config.alpha_compression = options.AlphaCompression;
        config.alpha_filtering = options.AlphaFiltering;
        config.alpha_quality = options.AlphaQuality;
        config.pass = options.Pass;
        config.preprocessing = options.Preprocessing;
        config.partitions = options.Partitions;
        config.partition_limit = options.PartitionLimit;

        if (options.UseSharpYUV)
        {
            config.use_sharp_yuv = 1;
        }
        else
        {
            config.use_sharp_yuv = 0;
        }
    }
}
