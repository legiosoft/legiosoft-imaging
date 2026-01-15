using System.Runtime.InteropServices;

namespace LegioSoft.Imaging.WebP.Native;

[StructLayout(LayoutKind.Sequential)]
internal struct WebPRGBABuffer
{
    public IntPtr rgba;
    public int stride;
    public UIntPtr size;
}
