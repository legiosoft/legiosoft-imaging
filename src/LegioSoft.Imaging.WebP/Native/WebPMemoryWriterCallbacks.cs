using System;
using System.Runtime.InteropServices;

namespace LegioSoft.Imaging.WebP.Native;

internal static class WebPMemoryWriterCallbacks
{
    public static int Write(IntPtr data, UIntPtr dataSize, ref WebPPicture picture)
    {
        var writer = Marshal.PtrToStructure<WebPMemoryWriter>(picture.custom_ptr);
        var requiredSize = (uint)writer.size + (uint)dataSize;

        if (requiredSize > (uint)writer.max_size)
        {
            var newSize = Math.Max(writer.max_size == UIntPtr.Zero ? 4096u : writer.max_size.ToUInt32() * 2, requiredSize);

            var newMem = Marshal.AllocHGlobal((int)newSize);
            if (newMem == IntPtr.Zero)
                return 0;

            if (writer.mem != IntPtr.Zero)
            {
                var oldData = new byte[(ulong)writer.size];
                Marshal.Copy(writer.mem, oldData, 0, (int)writer.size);
                Marshal.Copy(oldData, 0, newMem, (int)writer.size);
                Marshal.FreeHGlobal(writer.mem);
            }

            writer.mem = newMem;
            writer.max_size = new UIntPtr(newSize);

            Marshal.StructureToPtr(writer, picture.custom_ptr, false);
        }

        if (data != IntPtr.Zero && (ulong)dataSize > 0)
        {
            var buffer = new byte[(ulong)dataSize];
            Marshal.Copy(data, buffer, 0, (int)dataSize);
            Marshal.Copy(buffer, 0, writer.mem + (nint)writer.size.ToUInt64(), (int)dataSize);
        }

        writer.size = new UIntPtr(requiredSize);

        Marshal.StructureToPtr(writer, picture.custom_ptr, false);

        return 1;
    }
}
