using System.Runtime.InteropServices;
using LegioSoft.Imaging.WebP.Enums;

namespace LegioSoft.Imaging.WebP.Native;

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
