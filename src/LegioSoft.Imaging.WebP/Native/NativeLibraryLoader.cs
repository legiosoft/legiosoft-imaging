using System.Reflection;
using System.Runtime.InteropServices;

namespace LegioSoft.Imaging.WebP.Native;

internal static class NativeLibraryLoader
{
    private const string LibraryName = "libwebp";
    
    private static readonly Lazy<IntPtr> _nativeLibrary = new(() =>
    {
        string libraryPath = GetNativeLibraryPath();
        
        if (libraryPath == null)
        {
            throw new PlatformNotSupportedException($"WebP is not supported on the current platform: {RuntimeInformation.OSDescription} ({RuntimeInformation.ProcessArchitecture}) or the native library was not found in the runtimes directory.");
        }

        return NativeLibrary.Load(libraryPath);
    });

    private static string? GetNativeLibraryPath()
    {
        string? assemblyDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        
        if (string.IsNullOrEmpty(assemblyDirectory))
        {
            return null;
        }

        string? runtimeFolder = GetRuntimeFolder();
        
        if (string.IsNullOrEmpty(runtimeFolder))
        {
            return null;
        }

        string? libraryFileName = GetLibraryFileName();
        
        if (string.IsNullOrEmpty(libraryFileName))
        {
            return null;
        }

        string libraryPath = Path.Combine(assemblyDirectory, "runtimes", runtimeFolder, libraryFileName);

        if (File.Exists(libraryPath))
        {
            return libraryPath;
        }

        libraryPath = Path.Combine(assemblyDirectory, libraryFileName);
        
        if (File.Exists(libraryPath))
        {
            return libraryPath;
        }

        return null;
    }

    private static string? GetRuntimeFolder()
    {
        string? os = GetOSPlatform();
        
        if (string.IsNullOrEmpty(os))
        {
            return null;
        }

        string architecture = RuntimeInformation.ProcessArchitecture.ToString().ToLowerInvariant();
        
        return $"{os}-{architecture}";
    }

    private static string? GetOSPlatform()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return "win";
        }
        
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            return "linux";
        }
        
        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            return "osx";
        }

        return null;
    }

    private static string? GetLibraryFileName()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return $"{LibraryName}.dll";
        }
        
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            return $"{LibraryName}.so";
        }
        
        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            return $"{LibraryName}.dylib";
        }

        return null;
    }

    public static IntPtr GetNativeLibrary()
    {
        return _nativeLibrary.Value;
    }

    public static string GetLibraryName()
    {
        return LibraryName;
    }
}
