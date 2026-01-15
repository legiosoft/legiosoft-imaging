using System.Runtime.InteropServices;

namespace LegioSoft.Imaging.WebP.Native;

[StructLayout(LayoutKind.Sequential)]
internal struct WebPDecoderConfig
{
    public WebPBitstreamFeatures input;
    public WebPDecBuffer output;
    public WebPDecoderOptions options;
}
