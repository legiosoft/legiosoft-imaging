using System.Runtime.InteropServices;

namespace LegioSoft.Imaging.WebP.Native;

[StructLayout(LayoutKind.Sequential)]
public struct WebPRGBABuffer
{
    public IntPtr rgba;
    public int stride;
    public UIntPtr size;
}
