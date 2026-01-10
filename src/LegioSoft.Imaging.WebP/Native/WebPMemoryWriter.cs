using System.Runtime.InteropServices;

namespace LegioSoft.Imaging.WebP.Native;

[StructLayout(LayoutKind.Sequential)]
public struct WebPMemoryWriter
{
    public IntPtr mem;
    public UIntPtr size;
    public UIntPtr max_size;
    private uint pad;
}
