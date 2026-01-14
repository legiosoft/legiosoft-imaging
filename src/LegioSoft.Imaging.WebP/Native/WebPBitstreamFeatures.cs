using System.Runtime.InteropServices;

namespace LegioSoft.Imaging.WebP.Native;

[StructLayout(LayoutKind.Sequential)]
public unsafe struct WebPBitstreamFeatures
{
    public int width;
    public int height;
    public int has_alpha;
    public int has_animation;
    public int format;
    private fixed uint pad[5];
}
