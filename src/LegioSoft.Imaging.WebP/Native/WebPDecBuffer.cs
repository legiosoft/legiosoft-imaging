using System.Runtime.InteropServices;
using LegioSoft.Imaging.WebP.Enums;

namespace LegioSoft.Imaging.WebP.Native;

[StructLayout(LayoutKind.Explicit)]
internal struct WebPDecBufferUnion
{
    [FieldOffset(0)]
    public WebPRGBABuffer RGBA;

    [FieldOffset(0)]
    public WebPYUVABuffer YUVA;
}

[StructLayout(LayoutKind.Sequential)]
internal unsafe struct WebPDecBuffer
{
    public WEBP_CSP_MODE colorspace;
    public int width;
    public int height;
    public int is_external_memory;
    public WebPDecBufferUnion u;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
    private uint[] pad;

    public IntPtr private_memory;
}
