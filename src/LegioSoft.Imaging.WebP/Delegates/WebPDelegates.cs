using System.Runtime.InteropServices;
using LegioSoft.Imaging.WebP.Native;

namespace LegioSoft.Imaging.WebP.Delegates;

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate int WebPWriterFunction(IntPtr data, UIntPtr data_size, ref WebPPicture picture);

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate int WebPProgressHook(int percent, ref WebPPicture picture);
