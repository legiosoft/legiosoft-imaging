using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using LegioSoft.Imaging.WebP.Enums;

namespace LegioSoft.Imaging.WebP;

public static class WebPHelper
{
    public static bool IsWebP(byte[] imageData)
    {
        if (imageData == null || imageData.Length < 12)
            return false;

        return imageData[0] == 0x52 && 
               imageData[1] == 0x49 && 
               imageData[2] == 0x46 && 
               imageData[3] == 0x46 &&
               imageData[8] == 0x57 && 
               imageData[9] == 0x45 && 
               imageData[10] == 0x66 && 
               imageData[11] == 0x50;
    }

    public static ImageFormat DetectImageFormat(byte[] imageData)
    {
        if (imageData == null || imageData.Length < 12)
            return ImageFormat.Unknown;

        if (IsWebP(imageData))
            return ImageFormat.WebP;

        if (imageData[0] == 0xFF && imageData[1] == 0xD8 && imageData[2] == 0xFF)
            return ImageFormat.Jpeg;

        if (imageData[0] == 0x42 && imageData[1] == 0x4D)
            return ImageFormat.Bmp;

        if (imageData[0] == 0x47 && imageData[1] == 0x49 && imageData[2] == 0x46 && imageData[3] == 0x38)
            return ImageFormat.Gif;

        if (imageData[0] == 0x89 && imageData[1] == 0x50 && imageData[2] == 0x4E && imageData[3] == 0x47)
            return ImageFormat.Png;

        return ImageFormat.Unknown;
    }
}

internal static class NativeMethods
{
    private const string DllName = "libwebp.dll";

    #region Simple Decoding API

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int WebPGetInfo(byte[] data, UIntPtr data_size, out int width, out int height);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern VP8StatusCode WebPGetFeatures(byte[] data, UIntPtr data_size, out WebPBitstreamFeatures features);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr WebPDecodeRGBA(byte[] data, UIntPtr data_size, out int width, out int height);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr WebPDecodeARGB(byte[] data, UIntPtr data_size, out int width, out int height);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr WebPDecodeBGRA(byte[] data, UIntPtr data_size, out int width, out int height);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr WebPDecodeRGB(byte[] data, UIntPtr data_size, out int width, out int height);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr WebPDecodeBGR(byte[] data, UIntPtr data_size, out int width, out int height);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr WebPDecodeYUV(byte[] data, UIntPtr data_size, out int width, out int height, out IntPtr u, out IntPtr v, out int stride, out int uv_stride);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr WebPDecodeRGBAInto(byte[] data, UIntPtr data_size, IntPtr output_buffer, UIntPtr output_buffer_size, int output_stride);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr WebPDecodeARGBInto(byte[] data, UIntPtr data_size, IntPtr output_buffer, UIntPtr output_buffer_size, int output_stride);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr WebPDecodeBGRAInto(byte[] data, UIntPtr data_size, IntPtr output_buffer, UIntPtr output_buffer_size, int output_stride);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr WebPDecodeRGBInto(byte[] data, UIntPtr data_size, IntPtr output_buffer, UIntPtr output_buffer_size, int output_stride);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr WebPDecodeBGRInto(byte[] data, UIntPtr data_size, IntPtr output_buffer, UIntPtr output_buffer_size, int output_stride);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr WebPDecodeYUVInto(byte[] data, UIntPtr data_size, IntPtr luma, UIntPtr luma_size, int luma_stride, IntPtr u, UIntPtr u_size, int u_stride, IntPtr v, UIntPtr v_size, int v_stride);

    #endregion

    #region Advanced Decoding API

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int WebPInitDecBufferInternal(ref WebPDecBuffer buffer, int version);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void WebPFreeDecBuffer(ref WebPDecBuffer buffer);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int WebPValidateDecoderConfig(ref WebPDecoderConfig config);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern VP8StatusCode WebPDecode(byte[] data, UIntPtr data_size, ref WebPDecoderConfig config);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int WebPInitDecoderConfigInternal(ref WebPDecoderConfig config, int version);

    #endregion

    #region Incremental Decoding API

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr WebPINewDecoder(IntPtr output_buffer);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr WebPINewRGB(WEBP_CSP_MODE csp, IntPtr output_buffer, UIntPtr output_buffer_size, int output_stride);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr WebPINewYUVA(IntPtr luma, UIntPtr luma_size, int luma_stride, IntPtr u, UIntPtr u_size, int u_stride, IntPtr v, UIntPtr v_size, int v_stride, IntPtr a, UIntPtr a_size, int a_stride);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr WebPINewYUV(IntPtr luma, UIntPtr luma_size, int luma_stride, IntPtr u, UIntPtr u_size, int u_stride, IntPtr v, UIntPtr v_size, int v_stride);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void WebPIDelete(IntPtr idec);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern VP8StatusCode WebPIAppend(IntPtr idec, byte[] data, UIntPtr data_size);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern VP8StatusCode WebPIUpdate(IntPtr idec, byte[] data, UIntPtr data_size);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr WebPIDecGetRGB(IntPtr idec, out int last_y, out int width, out int height, out int stride);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr WebPIDecGetYUVA(IntPtr idec, out int last_y, out IntPtr u, out IntPtr v, out IntPtr a, out int width, out int height, out int stride, out int uv_stride, out int a_stride);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr WebPIDecodedArea(IntPtr idec, out int left, out int top, out int width, out int height);

    #endregion

    #region Simple Encoding API

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern UIntPtr WebPEncodeRGB(byte[] rgb, int width, int height, int stride, float quality_factor, out IntPtr output);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern UIntPtr WebPEncodeBGR(byte[] bgr, int width, int height, int stride, float quality_factor, out IntPtr output);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern UIntPtr WebPEncodeRGBA(byte[] rgba, int width, int height, int stride, float quality_factor, out IntPtr output);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern UIntPtr WebPEncodeBGRA(byte[] bgra, int width, int height, int stride, float quality_factor, out IntPtr output);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern UIntPtr WebPEncodeLosslessRGB(byte[] rgb, int width, int height, int stride, out IntPtr output);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern UIntPtr WebPEncodeLosslessBGR(byte[] bgr, int width, int height, int stride, out IntPtr output);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern UIntPtr WebPEncodeLosslessRGBA(byte[] rgba, int width, int height, int stride, out IntPtr output);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern UIntPtr WebPEncodeLosslessBGRA(byte[] bgra, int width, int height, int stride, out IntPtr output);

    #endregion

    #region Advanced Encoding API

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int WebPConfigInitInternal(ref WebPConfig config, WebPPreset preset, float quality, int version);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int WebPConfigLosslessPreset(ref WebPConfig config, int level);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int WebPValidateConfig(ref WebPConfig config);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int WebPPictureInitInternal(ref WebPPicture pic, int version);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int WebPPictureAlloc(ref WebPPicture pic);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void WebPPictureFree(ref WebPPicture pic);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int WebPPictureImportRGB(ref WebPPicture pic, byte[] rgb, int stride);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int WebPPictureImportBGR(ref WebPPicture pic, byte[] bgr, int stride);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int WebPPictureImportRGBA(ref WebPPicture pic, byte[] rgba, int stride);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int WebPPictureImportBGRA(ref WebPPicture pic, byte[] bgra, int stride);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int WebPPictureImportRGBX(ref WebPPicture pic, byte[] rgbx, int stride);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int WebPPictureImportBGRX(ref WebPPicture pic, byte[] bgrx, int stride);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int WebPPictureImportARGB(ref WebPPicture pic, byte[] argb, int stride);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int WebPPictureImportARGB(ref WebPPicture pic, UIntPtr argb, int stride);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int WebPPictureImportYUVA(ref WebPPicture pic, ref WebPYUVABuffer yuva);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int WebPPictureCopy(ref WebPPicture src, ref WebPPicture dst);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int WebPPictureCrop(ref WebPPicture pic, int left, int top, int width, int height);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int WebPPictureScale(ref WebPPicture pic, int scaled_width, int scaled_height);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int WebPPictureRescale(ref WebPPicture pic, int scaled_width, int scaled_height);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int WebPPictureView(ref WebPPicture src, int left, int top, int width, int height, ref WebPPicture dst);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int WebPPictureDistortion(ref WebPPicture src, ref WebPPicture ref_pic, int metric_type, ref float result);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int WebPPictureHasTransparency(ref WebPPicture pic);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void WebPCleanupTransparentArea(ref WebPPicture pic);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int WebPEncode(ref WebPConfig config, ref WebPPicture pic);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void WebPMemoryWriterInit(ref WebPMemoryWriter writer);

    #endregion

    #region Utility Functions

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void WebPFree(IntPtr ptr);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int WebPGetDecoderVersion();

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int WebPGetEncoderVersion();

    #endregion

    #region Structs

    [StructLayout(LayoutKind.Sequential)]
    public struct WebPBitstreamFeatures
    {
        public int width;
        public int height;
        public int has_alpha;
        public int has_animation;
        public int format;
        private uint pad1, pad2, pad3, pad4, pad5;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct WebPRGBABuffer
    {
        public IntPtr rgba;
        public int stride;
        public UIntPtr size;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct WebPYUVABuffer
    {
        public IntPtr y;
        public IntPtr u;
        public IntPtr v;
        public IntPtr a;
        public int y_stride;
        public int u_stride;
        public int v_stride;
        public int a_stride;
        public UIntPtr y_size;
        public UIntPtr u_size;
        public UIntPtr v_size;
        public UIntPtr a_size;
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct WebPDecBuffer
    {
        [FieldOffset(0)]
        public WEBP_CSP_MODE colorspace;

        [FieldOffset(4)]
        public int width;

        [FieldOffset(8)]
        public int height;

        [FieldOffset(12)]
        public int is_external_memory;

        [FieldOffset(16)]
        public WebPRGBABuffer rgba;

        [FieldOffset(16)]
        public WebPYUVABuffer yuva;

        [FieldOffset(48)]
        private uint pad1, pad2, pad3, pad4;

        [FieldOffset(64)]
        public IntPtr private_memory;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct WebPDecoderOptions
    {
        public int bypass_filtering;
        public int no_fancy_upsampling;
        public int use_cropping;
        public int crop_left;
        public int crop_top;
        public int crop_width;
        public int crop_height;
        public int use_scaling;
        public int scaled_width;
        public int scaled_height;
        public int use_threads;
        public int dithering_strength;
        public int flip;
        public int alpha_dithering_strength;
        private uint pad1, pad2, pad3, pad4, pad5;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct WebPDecoderConfig
    {
        public WebPBitstreamFeatures input;
        public WebPDecBuffer output;
        public WebPDecoderOptions options;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct WebPConfig
    {
        public int lossless;
        public float quality;
        public int method;
        public WebPImageHint image_hint;
        public int target_size;
        public float target_PSNR;
        public int segments;
        public int sns_strength;
        public int filter_strength;
        public int filter_sharpness;
        public int filter_type;
        public int autofilter;
        public int alpha_compression;
        public int alpha_filtering;
        public int alpha_quality;
        public int pass;
        public int show_compressed;
        public int preprocessing;
        public int partitions;
        public int partition_limit;
        public int use_sharp_yuv;
        private uint pad1, pad2, pad3;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct WebPPicture
    {
        public int use_argb;
        public int colorspace;
        public int width;
        public int height;
        public IntPtr y;
        public IntPtr u;
        public IntPtr v;
        public int y_stride;
        public int uv_stride;
        public IntPtr a;
        public int a_stride;
        public IntPtr argb;
        public int argb_stride;
        public IntPtr writer;
        public IntPtr custom_ptr;
        public WebPEncodingError error_code;
        private uint pad1, pad2, pad3, pad4;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct WebPMemoryWriter
    {
        public IntPtr mem;
        public UIntPtr size;
    }

    #endregion
}

public enum ImageFormat
{
    Unknown,
    Png,
    Jpeg,
    WebP,
    Bmp,
    Gif
}
