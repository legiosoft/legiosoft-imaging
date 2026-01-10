using System.Runtime.InteropServices;
using LegioSoft.Imaging.WebP.Enums;

namespace LegioSoft.Imaging.WebP.Native;

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
