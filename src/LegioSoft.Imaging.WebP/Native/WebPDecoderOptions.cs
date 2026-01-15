using System.Runtime.InteropServices;

namespace LegioSoft.Imaging.WebP.Native;

[StructLayout(LayoutKind.Sequential)]
internal unsafe struct WebPDecoderOptions
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
    private fixed uint pad[5];
}
