using System.Runtime.InteropServices;

namespace LegioSoft.Imaging.WebP.Native;

internal static class WebPMemoryWriterCallbacks
{
    public static int Write(IntPtr data, UIntPtr dataSize, ref WebPPicture picture)
    {
        try
        {
            var writer = Marshal.PtrToStructure<WebPMemoryWriter>(picture.custom_ptr);

            ulong currentSize = writer.size.ToUInt64();
            ulong appendSize = dataSize.ToUInt64();
            ulong requiredSize = checked(currentSize + appendSize);

            if (requiredSize > writer.max_size.ToUInt64())
            {
                ulong currentCapacity = writer.max_size.ToUInt64();
                ulong newSize = Math.Max(
                    currentCapacity == 0 ? 4096UL : currentCapacity * 2,
                    requiredSize
                );

                if (newSize > int.MaxValue)
                    return 0;

                IntPtr newMem = Marshal.AllocHGlobal((int)newSize);
                if (newMem == IntPtr.Zero)
                    return 0;

                if (writer.mem != IntPtr.Zero && currentSize > 0)
                {
                    unsafe
                    {
                        Buffer.MemoryCopy(
                            writer.mem.ToPointer(),
                            newMem.ToPointer(),
                            newSize,
                            currentSize
                        );
                    }
                    Marshal.FreeHGlobal(writer.mem);
                }

                writer.mem = newMem;
                writer.max_size = new UIntPtr(newSize);
                Marshal.StructureToPtr(writer, picture.custom_ptr, false);
            }

            if (data != IntPtr.Zero && appendSize > 0)
            {
                unsafe
                {
                    byte* sourcePtr = (byte*)data.ToPointer();
                    byte* targetPtr = (byte*)writer.mem.ToPointer() + currentSize;
                    ulong copySize = Math.Min(appendSize, (ulong)int.MaxValue);
                    Buffer.MemoryCopy(sourcePtr, targetPtr, appendSize, copySize);
                }
            }

            writer.size = new UIntPtr(requiredSize);
            Marshal.StructureToPtr(writer, picture.custom_ptr, false);

            return 1;
        }
        catch
        {
            return 0;
        }
    }
}
