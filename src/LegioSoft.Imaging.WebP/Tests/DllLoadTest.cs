using LegioSoft.Imaging.WebP.Helpers;
using LegioSoft.Imaging.WebP.Native;

namespace LegioSoft.Imaging.WebP.Tests;

public class DllLoadTest
{
    public static void Main(string[] args)
    {
        Console.WriteLine("Testing libwebp.dll loading...");
        Console.WriteLine("=================================");
        Console.WriteLine();

        // Test 1: IsWebP helper
        Console.WriteLine("Test 1: WebP Detection");
        try
        {
            var validWebP = new byte[] {
                0x52, 0x49, 0x46, 0x46, // "RIFF"
                0x00, 0x00, 0x00, 0x00, // size (placeholder)
                0x57, 0x45, 0x42, 0x50  // "WEBP"
            };

            var isWebP = WebPValidationHelper.IsWebP(validWebP);
            Console.WriteLine($"✅ IsWebP() works: {isWebP}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ IsWebP() failed: {ex.Message}");
        }

        Console.WriteLine();

        // Test 2: WebPGetInfo (simple P/Invoke test)
        Console.WriteLine("Test 2: WebPGetInfo (P/Invoke)");
        try
        {
            // Valid WebP header (12 bytes minimum)
            var webpHeader = new byte[] {
                0x52, 0x49, 0x46, 0x46, // RIFF
                0x1C, 0x00, 0x00, 0x00, // Chunk size: 28 bytes
                0x57, 0x45, 0x42, 0x50, // WEBP
                0x56, 0x50, 0x38, 0x20, // VP8
                0x0A, 0x00, 0x00, 0x00, // Chunk size: 10 bytes
                0x00, 0x00, 0x00, 0x00  // VP8 data
            };

            var result = NativeMethods.WebPGetInfo(webpHeader, (UIntPtr)webpHeader.Length, out var width, out var height);

            if (result != 0)
            {
                Console.WriteLine($"✅ DLL loaded and WebPGetInfo works");
                Console.WriteLine($"   Image dimensions: {width}x{height}");
            }
            else
            {
                Console.WriteLine("⚠️ DLL loaded but WebPGetInfo returned 0 (invalid WebP data)");
            }
        }
        catch (DllNotFoundException)
        {
            Console.WriteLine("❌ DllNotFoundException: libwebp.dll not found");
            Console.WriteLine("   Make sure libwebp.dll is in runtimes/win-x64/ directory");
        }
        catch (BadImageFormatException)
        {
            Console.WriteLine("❌ BadImageFormatException: Architecture mismatch");
            Console.WriteLine("   Check if you're using 64-bit DLL with 64-bit application");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Unexpected error: {ex.GetType().Name}");
            Console.WriteLine($"   {ex.Message}");
        }

        Console.WriteLine();
        Console.WriteLine("Test complete. Press any key to exit...");
        Console.ReadKey();
    }
}
