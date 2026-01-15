using System.Runtime.InteropServices;
using LegioSoft.Imaging.WebP.Enums;

namespace LegioSoft.Imaging.WebP.Native;

[StructLayout(LayoutKind.Sequential)]
internal unsafe struct WebPPicture
{
    public int use_argb;
    public WebPEncCSP colorspace;
    public int width;
    public int height;
    public IntPtr y;
    public IntPtr u;
    public IntPtr v;
    public int y_stride;
    public int uv_stride;
    public IntPtr a;
    public int a_stride;
    private fixed uint pad1[2];
    public IntPtr argb;
    public int argb_stride;
    private fixed uint pad2[3];
    public IntPtr writer;
    public IntPtr custom_ptr;
    public int extra_info_type;
    public IntPtr extra_info;
    public IntPtr stats;
    public WebPEncodingError error_code;
    public IntPtr progress_hook;
    public IntPtr user_data;
    private fixed uint pad3[3];
    private IntPtr pad4;
    private IntPtr pad5;
    private fixed uint pad6[8];
    private IntPtr memory_;
    private IntPtr memory_argb_;
    private IntPtr pad7;
    private IntPtr pad8;
}
