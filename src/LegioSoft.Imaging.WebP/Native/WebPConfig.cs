using System.Runtime.InteropServices;
using LegioSoft.Imaging.WebP.Enums;

namespace LegioSoft.Imaging.WebP.Native;

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
    public int low_memory;
    public int near_lossless;
    public int exact;
    public int use_delta_palette;
    public int use_low_memory;
    private uint pad1, pad2;
}
