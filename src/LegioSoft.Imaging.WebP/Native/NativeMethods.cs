using System.Reflection;
using System.Runtime.InteropServices;
using LegioSoft.Imaging.WebP.Enums;

namespace LegioSoft.Imaging.WebP.Native;

internal static class NativeMethods
{
    #region Constants

    private const string LibraryName = "libwebp";
    private const int WEBP_DECODER_ABI_VERSION = 0x0210;
    private const int WEBP_ENCODER_ABI_VERSION = 0x0210;

    #endregion

    #region Delegate

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int WebPWriterFunction(IntPtr data, UIntPtr data_size, ref WebPPicture picture);

    #endregion

    #region Constructor

    static NativeMethods()
    {
        NativeLibrary.SetDllImportResolver(typeof(NativeMethods).Assembly, DllImportResolver);
    }

    #endregion

    #region DllImport Resolver

    private static IntPtr DllImportResolver(string libraryName, Assembly assembly, DllImportSearchPath? searchPath)
    {
        return libraryName == "libwebp"
            ? NativeLibraryLoader.GetNativeLibrary()
            : IntPtr.Zero;
    }

    #endregion

    #region Simple Decoding API

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int WebPGetInfo(byte[] data, UIntPtr data_size, out int width, out int height);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern VP8StatusCode WebPGetFeatures(byte[] data, UIntPtr data_size, out WebPBitstreamFeatures features);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr WebPDecodeRGBA(byte[] data, UIntPtr data_size, out int width, out int height);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr WebPDecodeARGB(byte[] data, UIntPtr data_size, out int width, out int height);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr WebPDecodeBGRA(byte[] data, UIntPtr data_size, out int width, out int height);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr WebPDecodeRGB(byte[] data, UIntPtr data_size, out int width, out int height);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr WebPDecodeBGR(byte[] data, UIntPtr data_size, out int width, out int height);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr WebPDecodeYUV(byte[] data, UIntPtr data_size, out int width, out int height, out IntPtr u, out IntPtr v, out int stride, out int uv_stride);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr WebPDecodeRGBAInto(byte[] data, UIntPtr data_size, IntPtr output_buffer, UIntPtr output_buffer_size, int output_stride);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr WebPDecodeARGBInto(byte[] data, UIntPtr data_size, IntPtr output_buffer, UIntPtr output_buffer_size, int output_stride);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr WebPDecodeBGRAInto(byte[] data, UIntPtr data_size, IntPtr output_buffer, UIntPtr output_buffer_size, int output_stride);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr WebPDecodeRGBInto(byte[] data, UIntPtr data_size, IntPtr output_buffer, UIntPtr output_buffer_size, int output_stride);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr WebPDecodeBGRInto(byte[] data, UIntPtr data_size, IntPtr output_buffer, UIntPtr output_buffer_size, int output_stride);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    // Returns: luma buffer pointer (Y-plane) on success, IntPtr.Zero on failure
    public static extern IntPtr WebPDecodeYUVInto(
        byte[] data, UIntPtr data_size,
        IntPtr luma, UIntPtr luma_size, int luma_stride,
        IntPtr u, UIntPtr u_size, int u_stride,
        IntPtr v, UIntPtr v_size, int v_stride);

    #endregion

    #region Advanced Decoding API

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int WebPInitDecBufferInternal(ref WebPDecBuffer buffer, int version);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void WebPFreeDecBuffer(ref WebPDecBuffer buffer);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int WebPValidateDecoderConfig(ref WebPDecoderConfig config);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int WebPInitDecoderConfigInternal(ref WebPDecoderConfig config, int version);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern VP8StatusCode WebPDecode(byte[] data, UIntPtr data_size, ref WebPDecoderConfig config);

    #endregion

    #region Init Helper Methods

    public static int WebPConfigInit(ref WebPConfig config)
    {
        return WebPConfigInitInternal(ref config, WebPPreset.DEFAULT, 75.0f, WEBP_ENCODER_ABI_VERSION);
    }

    public static int WebPConfigInit(ref WebPConfig config, WebPPreset preset, float quality)
    {
        return WebPConfigInitInternal(ref config, preset, quality, WEBP_ENCODER_ABI_VERSION);
    }

    public static int WebPPictureInit(ref WebPPicture picture)
    {
        return WebPPictureInitInternal(ref picture, WEBP_ENCODER_ABI_VERSION);
    }

    public static int WebPInitDecoderConfig(ref WebPDecoderConfig config)
    {
        return WebPInitDecoderConfigInternal(ref config, WEBP_DECODER_ABI_VERSION);
    }

    public static int WebPInitDecBuffer(ref WebPDecBuffer buffer)
    {
        return WebPInitDecBufferInternal(ref buffer, WEBP_DECODER_ABI_VERSION);
    }

    #endregion

    #region Incremental Decoding API

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr WebPINewDecoder(IntPtr output_buffer);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr WebPINewRGB(WEBP_CSP_MODE csp, IntPtr output_buffer, UIntPtr output_buffer_size, int output_stride);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr WebPINewYUVA(
        IntPtr luma, UIntPtr luma_size, int luma_stride,
        IntPtr u, UIntPtr u_size, int u_stride,
        IntPtr v, UIntPtr v_size, int v_stride,
        IntPtr a, UIntPtr a_size, int a_stride);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr WebPINewYUV(
        IntPtr luma, UIntPtr luma_size, int luma_stride,
        IntPtr u, UIntPtr u_size, int u_stride,
        IntPtr v, UIntPtr v_size, int v_stride);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void WebPIDelete(IntPtr idec);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern VP8StatusCode WebPIAppend(IntPtr idec, byte[] data, UIntPtr data_size);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern VP8StatusCode WebPIUpdate(IntPtr idec, byte[] data, UIntPtr data_size);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr WebPIDecGetRGB(IntPtr idec, out int last_y, out int width, out int height, out int stride);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr WebPIDecGetYUVA(
        IntPtr idec, out int last_y,
        out IntPtr u, out IntPtr v, out IntPtr a,
        out int width, out int height, out int stride, out int uv_stride, out int a_stride);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr WebPIDecodedArea(IntPtr idec, out int left, out int top, out int width, out int height);

    #endregion

    #region Simple Encoding API

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern UIntPtr WebPEncodeRGB(byte[] rgb, int width, int height, int stride, float quality_factor, out IntPtr output);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern UIntPtr WebPEncodeBGR(byte[] bgr, int width, int height, int stride, float quality_factor, out IntPtr output);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern UIntPtr WebPEncodeRGBA(byte[] rgba, int width, int height, int stride, float quality_factor, out IntPtr output);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern UIntPtr WebPEncodeBGRA(byte[] bgra, int width, int height, int stride, float quality_factor, out IntPtr output);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern UIntPtr WebPEncodeLosslessRGB(byte[] rgb, int width, int height, int stride, out IntPtr output);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern UIntPtr WebPEncodeLosslessBGR(byte[] bgr, int width, int height, int stride, out IntPtr output);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern UIntPtr WebPEncodeLosslessRGBA(byte[] rgba, int width, int height, int stride, out IntPtr output);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern UIntPtr WebPEncodeLosslessBGRA(byte[] bgra, int width, int height, int stride, out IntPtr output);

    #endregion

    #region Advanced Encoding API

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int WebPConfigInitInternal(ref WebPConfig config, WebPPreset preset, float quality, int version);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int WebPConfigLosslessPreset(ref WebPConfig config, int level);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int WebPValidateConfig(ref WebPConfig config);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int WebPPictureInitInternal(ref WebPPicture pic, int version);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int WebPPictureAlloc(ref WebPPicture pic);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void WebPPictureFree(ref WebPPicture pic);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int WebPPictureImportRGB(ref WebPPicture pic, byte[] rgb, int stride);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int WebPPictureImportBGR(ref WebPPicture pic, byte[] bgr, int stride);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int WebPPictureImportRGBA(ref WebPPicture pic, byte[] rgba, int stride);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int WebPPictureImportBGRA(ref WebPPicture pic, byte[] bgra, int stride);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int WebPPictureImportRGBX(ref WebPPicture pic, byte[] rgbx, int stride);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int WebPPictureImportBGRX(ref WebPPicture pic, byte[] bgrx, int stride);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int WebPPictureImportARGB(ref WebPPicture pic, IntPtr argb, int stride);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int WebPPictureImportYUVA(ref WebPPicture pic, ref WebPYUVABuffer yuva);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int WebPPictureCopy(ref WebPPicture src, ref WebPPicture dst);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int WebPPictureCrop(ref WebPPicture pic, int left, int top, int width, int height);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int WebPPictureScale(ref WebPPicture pic, int scaled_width, int scaled_height);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int WebPPictureRescale(ref WebPPicture pic, int scaled_width, int scaled_height);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int WebPPictureView(ref WebPPicture src, int left, int top, int width, int height, ref WebPPicture dst);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int WebPPictureDistortion(ref WebPPicture src, ref WebPPicture ref_pic, int metric_type, ref float result);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int WebPPictureHasTransparency(ref WebPPicture pic);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void WebPCleanupTransparentArea(ref WebPPicture pic);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int WebPEncode(ref WebPConfig config, ref WebPPicture pic);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void WebPMemoryWriterInit(ref WebPMemoryWriter writer);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void WebPMemoryWriterClear(ref WebPMemoryWriter writer);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int WebPMemoryWrite(IntPtr data, UIntPtr data_size, ref WebPPicture picture);

    #endregion

    #region Utility Functions

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void WebPFree(IntPtr ptr);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int WebPGetDecoderVersion();

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int WebPGetEncoderVersion();

    #endregion

    #region Safe Wrappers - Decode

    public static T SafeDecodeRGBA<T>(byte[] data, Func<IntPtr, int, int, T> callback)
    {
        if (data == null || data.Length == 0)
            throw new ArgumentException("Data cannot be null or empty", nameof(data));

        UIntPtr dataSize = new UIntPtr((uint)data.Length);
        IntPtr decoded = WebPDecodeRGBA(data, dataSize, out int width, out int height);

        if (decoded == IntPtr.Zero)
            return default!;

        try
        {
            return callback(decoded, width, height);
        }
        finally
        {
            WebPFree(decoded);
        }
    }

    public static T SafeDecodeARGB<T>(byte[] data, Func<IntPtr, int, int, T> callback)
    {
        if (data == null || data.Length == 0)
            throw new ArgumentException("Data cannot be null or empty", nameof(data));

        UIntPtr dataSize = new UIntPtr((uint)data.Length);
        IntPtr decoded = WebPDecodeARGB(data, dataSize, out int width, out int height);

        if (decoded == IntPtr.Zero)
            return default!;

        try
        {
            return callback(decoded, width, height);
        }
        finally
        {
            WebPFree(decoded);
        }
    }

    public static T SafeDecodeBGRA<T>(byte[] data, Func<IntPtr, int, int, T> callback)
    {
        if (data == null || data.Length == 0)
            throw new ArgumentException("Data cannot be null or empty", nameof(data));

        UIntPtr dataSize = new UIntPtr((uint)data.Length);
        IntPtr decoded = WebPDecodeBGRA(data, dataSize, out int width, out int height);

        if (decoded == IntPtr.Zero)
            return default!;

        try
        {
            return callback(decoded, width, height);
        }
        finally
        {
            WebPFree(decoded);
        }
    }

    public static T SafeDecodeRGB<T>(byte[] data, Func<IntPtr, int, int, T> callback)
    {
        if (data == null || data.Length == 0)
            throw new ArgumentException("Data cannot be null or empty", nameof(data));

        UIntPtr dataSize = new UIntPtr((uint)data.Length);
        IntPtr decoded = WebPDecodeRGB(data, dataSize, out int width, out int height);

        if (decoded == IntPtr.Zero)
            return default!;

        try
        {
            return callback(decoded, width, height);
        }
        finally
        {
            WebPFree(decoded);
        }
    }

    public static T SafeDecodeBGR<T>(byte[] data, Func<IntPtr, int, int, T> callback)
    {
        if (data == null || data.Length == 0)
            throw new ArgumentException("Data cannot be null or empty", nameof(data));

        UIntPtr dataSize = new UIntPtr((uint)data.Length);
        IntPtr decoded = WebPDecodeBGR(data, dataSize, out int width, out int height);

        if (decoded == IntPtr.Zero)
            return default!;

        try
        {
            return callback(decoded, width, height);
        }
        finally
        {
            WebPFree(decoded);
        }
    }

    public static T SafeDecodeYUV<T>(byte[] data, Func<IntPtr, IntPtr, IntPtr, int, int, int, int, T> callback)
    {
        if (data == null || data.Length == 0)
            throw new ArgumentException("Data cannot be null or empty", nameof(data));

        UIntPtr dataSize = new UIntPtr((uint)data.Length);
        IntPtr decoded = WebPDecodeYUV(data, dataSize, out int width, out int height, out IntPtr u, out IntPtr v, out int stride, out int uv_stride);

        if (decoded == IntPtr.Zero)
            return default!;

        try
        {
            return callback(decoded, u, v, width, height, stride, uv_stride);
        }
        finally
        {
            WebPFree(decoded);
        }
    }

    #endregion

    #region Safe Wrappers - Encode

    private static T ProcessEncodedOutput<T>(IntPtr output, UIntPtr size, Func<IntPtr, UIntPtr, T> callback)
    {
        if (output == IntPtr.Zero || size == UIntPtr.Zero)
            return default!;

        try
        {
            return callback(output, size);
        }
        finally
        {
            WebPFree(output);
        }
    }

    public static T SafeEncodeRGB<T>(byte[] rgb, int width, int height, int stride, float quality_factor, Func<IntPtr, UIntPtr, T> callback)
    {
        if (rgb == null || rgb.Length == 0)
            throw new ArgumentException("RGB data cannot be null or empty", nameof(rgb));

        UIntPtr size = WebPEncodeRGB(rgb, width, height, stride, quality_factor, out IntPtr output);
        return ProcessEncodedOutput(output, size, callback);
    }

    public static T SafeEncodeBGR<T>(byte[] bgr, int width, int height, int stride, float quality_factor, Func<IntPtr, UIntPtr, T> callback)
    {
        if (bgr == null || bgr.Length == 0)
            throw new ArgumentException("BGR data cannot be null or empty", nameof(bgr));

        UIntPtr size = WebPEncodeBGR(bgr, width, height, stride, quality_factor, out IntPtr output);
        return ProcessEncodedOutput(output, size, callback);
    }

    public static T SafeEncodeRGBA<T>(byte[] rgba, int width, int height, int stride, float quality_factor, Func<IntPtr, UIntPtr, T> callback)
    {
        if (rgba == null || rgba.Length == 0)
            throw new ArgumentException("RGBA data cannot be null or empty", nameof(rgba));

        UIntPtr size = WebPEncodeRGBA(rgba, width, height, stride, quality_factor, out IntPtr output);
        return ProcessEncodedOutput(output, size, callback);
    }

    public static T SafeEncodeBGRA<T>(byte[] bgra, int width, int height, int stride, float quality_factor, Func<IntPtr, UIntPtr, T> callback)
    {
        if (bgra == null || bgra.Length == 0)
            throw new ArgumentException("BGRA data cannot be null or empty", nameof(bgra));

        UIntPtr size = WebPEncodeBGRA(bgra, width, height, stride, quality_factor, out IntPtr output);
        return ProcessEncodedOutput(output, size, callback);
    }

    public static T SafeEncodeLosslessRGB<T>(byte[] rgb, int width, int height, int stride, Func<IntPtr, UIntPtr, T> callback)
    {
        if (rgb == null || rgb.Length == 0)
            throw new ArgumentException("RGB data cannot be null or empty", nameof(rgb));

        UIntPtr size = WebPEncodeLosslessRGB(rgb, width, height, stride, out IntPtr output);
        return ProcessEncodedOutput(output, size, callback);
    }

    public static T SafeEncodeLosslessBGR<T>(byte[] bgr, int width, int height, int stride, Func<IntPtr, UIntPtr, T> callback)
    {
        if (bgr == null || bgr.Length == 0)
            throw new ArgumentException("BGR data cannot be null or empty", nameof(bgr));

        UIntPtr size = WebPEncodeLosslessBGR(bgr, width, height, stride, out IntPtr output);
        return ProcessEncodedOutput(output, size, callback);
    }

    public static T SafeEncodeLosslessRGBA<T>(byte[] rgba, int width, int height, int stride, Func<IntPtr, UIntPtr, T> callback)
    {
        if (rgba == null || rgba.Length == 0)
            throw new ArgumentException("RGBA data cannot be null or empty", nameof(rgba));

        UIntPtr size = WebPEncodeLosslessRGBA(rgba, width, height, stride, out IntPtr output);
        return ProcessEncodedOutput(output, size, callback);
    }

    public static T SafeEncodeLosslessBGRA<T>(byte[] bgra, int width, int height, int stride, Func<IntPtr, UIntPtr, T> callback)
    {
        if (bgra == null || bgra.Length == 0)
            throw new ArgumentException("BGRA data cannot be null or empty", nameof(bgra));

        UIntPtr size = WebPEncodeLosslessBGRA(bgra, width, height, stride, out IntPtr output);
        return ProcessEncodedOutput(output, size, callback);
    }

    #endregion
}
