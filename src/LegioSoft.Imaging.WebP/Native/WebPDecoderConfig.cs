using System.Runtime.InteropServices;

namespace LegioSoft.Imaging.WebP.Native;

[StructLayout(LayoutKind.Sequential)]
public struct WebPDecoderConfig
{
    public WebPBitstreamFeatures input;
    public WebPDecBuffer output;
    public WebPDecoderOptions options;
}
