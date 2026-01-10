using System.Runtime.InteropServices;

namespace LegioSoft.Imaging.WebP.Native;

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
