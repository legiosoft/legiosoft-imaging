using System.Runtime.InteropServices;

namespace LegioSoft.Imaging.WebP.Native;

[StructLayout(LayoutKind.Sequential)]
public unsafe struct WebPMemoryWriter
{
    public IntPtr mem;
    public UIntPtr size;
    public UIntPtr max_size;
    private fixed uint pad[1];
}
