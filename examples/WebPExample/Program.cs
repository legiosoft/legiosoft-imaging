using System.Drawing;
using System.Drawing.Imaging;
using LegioSoft.Imaging.Core;
using LegioSoft.Imaging.WebP;
using LegioSoft.Imaging.WebP.Enums;
using LegioSoft.Imaging.WebP.Models;

namespace WebPExample;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== LegioSoft.Imaging.WebP Examples ===\n");

        var exampleImage = "example.png";

        if (!File.Exists(exampleImage))
        {
            Console.WriteLine($"Error: {exampleImage} not found in output directory.");
            Console.WriteLine("Add test image to test-photos/ folder or check build configuration.");
            return;
        }

        var (imageData, width, height) = LoadPngToRGBA(exampleImage);

        try
        {
            EncodeBasicExample(imageData, width, height);
            EncodeQualityExample(imageData, width, height);
            EncodeLosslessExample(imageData, width, height);
            EncodeRGBExample(imageData, width, height);
            EncodeAdvancedExample(imageData, width, height);
            DecodeExample(imageData, width, height);
            DecodeWithColorSpaceExample(imageData, width, height);
            ScaleExample(imageData, width, height);
            CropExample(imageData, width, height);
            FlipExample(imageData, width, height);
            InfoExample(imageData, width, height);
            LibraryInfoExample();
            ValidationExample();
            ScenariosExample(imageData, width, height);
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Error: {ex.GetType().Name}: {ex.Message}");
            Console.WriteLine($"Stack: {ex.StackTrace}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"Inner: {ex.InnerException.GetType().Name}: {ex.InnerException.Message}");
                Console.WriteLine($"Inner Stack: {ex.InnerException.StackTrace}");
            }
            Console.ResetColor();
        }
    }

    static (byte[] data, int width, int height) LoadPngToRGBA(string filePath)
    {
        Console.WriteLine($"Loading {filePath}...");
        using var bitmap = new Bitmap(filePath);
        int width = bitmap.Width;
        int height = bitmap.Height;

        Console.WriteLine($"  Dimensions: {width}x{height}");

        var pixelData = new byte[width * height * 4];

        var rect = new Rectangle(0, 0, width, height);
        var bitmapData = bitmap.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

        try
        {
            IntPtr ptr = bitmapData.Scan0;
            int bytes = Math.Abs(bitmapData.Stride) * height;
            var rgbValues = new byte[bytes];
            System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int srcIndex = (y * bitmapData.Stride) + (x * 4);
                    int destIndex = (y * width + x) * 4;

                    pixelData[destIndex + 0] = rgbValues[srcIndex + 2];
                    pixelData[destIndex + 1] = rgbValues[srcIndex + 1];
                    pixelData[destIndex + 2] = rgbValues[srcIndex + 0];
                    pixelData[destIndex + 3] = rgbValues[srcIndex + 3];
                }
            }

            Console.WriteLine($"  Decoded to RGBA: {pixelData.Length} bytes\n");
            return (pixelData, width, height);
        }
        finally
        {
            bitmap.UnlockBits(bitmapData);
        }
    }

    static void EncodeBasicExample(byte[] imageData, int width, int height)
    {
        Console.WriteLine("1. Basic Encode");
        Console.WriteLine(new string('-', 50));

        var webP = WebPImage.Encode(imageData, width, height, 75.0f);
        File.WriteAllBytes("output-encoded-basic.webp", webP);

        Console.WriteLine($"  Created: output-encoded-basic.webp ({webP.Length} bytes)");
        Console.WriteLine($"  Source: {imageData.Length} bytes → WebP: {webP.Length} bytes");
        Console.WriteLine($"  Compression ratio: {((1 - (double)webP.Length / imageData.Length) * 100):F1}%");
        Console.WriteLine();
    }

    static void EncodeQualityExample(byte[] imageData, int width, int height)
    {
        Console.WriteLine("2. Encode Quality Levels");
        Console.WriteLine(new string('-', 50));

        var low = WebPImage.Encode(imageData, width, height, 30.0f);
        File.WriteAllBytes("output-quality-low.webp", low);
        Console.WriteLine($"  Low quality (30): {low.Length} bytes");

        var medium = WebPImage.Encode(imageData, width, height, 60.0f);
        File.WriteAllBytes("output-quality-medium.webp", medium);
        Console.WriteLine($"  Medium quality (60): {medium.Length} bytes");

        var high = WebPImage.Encode(imageData, width, height, 85.0f);
        File.WriteAllBytes("output-quality-high.webp", high);
        Console.WriteLine($"  High quality (85): {high.Length} bytes");

        var maximum = WebPImage.Encode(imageData, width, height, 100.0f);
        File.WriteAllBytes("output-quality-maximum.webp", maximum);
        Console.WriteLine($"  Maximum quality (100): {maximum.Length} bytes");
        Console.WriteLine();
    }

    static void EncodeLosslessExample(byte[] imageData, int width, int height)
    {
        Console.WriteLine("3. Lossless Encoding");
        Console.WriteLine(new string('-', 50));

        var lossy = WebPImage.Encode(imageData, width, height, 75.0f);
        File.WriteAllBytes("output-lossy.webp", lossy);
        Console.WriteLine($"  Lossy (quality 75): {lossy.Length} bytes");

        var lossless = WebPImage.EncodeLossless(imageData, width, height);
        File.WriteAllBytes("output-lossless.webp", lossless);
        Console.WriteLine($"  Lossless: {lossless.Length} bytes");

        Console.WriteLine($"  Size difference: {((double)lossless.Length / lossy.Length):F1}x");
        Console.WriteLine();
    }

    static void EncodeRGBExample(byte[] imageData, int width, int height)
    {
        Console.WriteLine("4. RGB Encoding");
        Console.WriteLine(new string('-', 50));

        var rgba = WebPImage.Encode(imageData, width, height, 75.0f);
        File.WriteAllBytes("output-encoded-rgba.webp", rgba);
        Console.WriteLine($"  RGBA encoding: {rgba.Length} bytes");

        var rgbData = ConvertRGBAtoRGB(imageData);
        var rgb = WebPImage.EncodeRGB(rgbData, width, height, 75.0f);
        File.WriteAllBytes("output-encoded-rgb.webp", rgb);
        Console.WriteLine($"  RGB encoding: {rgb.Length} bytes");

        Console.WriteLine($"  RGB saves: {((1 - (double)rgb.Length / rgba.Length) * 100):F1}%");
        Console.WriteLine();
    }

    static byte[] ConvertRGBAtoRGB(byte[] rgbaData)
    {
        int pixelCount = rgbaData.Length / 4;
        var rgbData = new byte[pixelCount * 3];

        for (int i = 0; i < pixelCount; i++)
        {
            int rgbaIndex = i * 4;
            int rgbIndex = i * 3;

            rgbData[rgbIndex + 0] = rgbaData[rgbaIndex + 0];
            rgbData[rgbIndex + 1] = rgbaData[rgbaIndex + 1];
            rgbData[rgbIndex + 2] = rgbaData[rgbaIndex + 2];
        }

        return rgbData;
    }

    static void EncodeAdvancedExample(byte[] imageData, int width, int height)
    {
        Console.WriteLine("5. Advanced Encoding");
        Console.WriteLine(new string('-', 50));

        var options = new WebPEncodeOptions
        {
            Quality = 80.0f,
            Method = 6,
            Lossless = false,
            Pass = 6
        };

        var advanced = WebPImage.EncodeAdvanced(imageData, width, height, options);
        File.WriteAllBytes("output-encoded-advanced.webp", advanced);

        Console.WriteLine($"  Created: output-encoded-advanced.webp ({advanced.Length} bytes)");
        Console.WriteLine($"  Method: {options.Method} (encoding complexity)");
        Console.WriteLine($"  Pass: {options.Pass} (analysis passes)");
        Console.WriteLine();
    }

    static void DecodeExample(byte[] imageData, int width, int height)
    {
        Console.WriteLine("6. Decode");
        Console.WriteLine(new string('-', 50));

        var webP = WebPImage.Encode(imageData, width, height, 75.0f);

        var decoded = WebPImage.Decode(webP);
        File.WriteAllBytes("output-decoded.rgba", decoded);

        Console.WriteLine($"  Created: output-decoded.rgba ({decoded.Length} bytes)");
        Console.WriteLine($"  Original: {webP.Length} bytes → Decoded: {decoded.Length} bytes");
        Console.WriteLine($"  Pixel count: {decoded.Length / 4} (RGBA, 4 bytes per pixel)");
        Console.WriteLine();
    }

    static void DecodeWithColorSpaceExample(byte[] imageData, int width, int height)
    {
        Console.WriteLine("7. Decode with Color Space");
        Console.WriteLine(new string('-', 50));

        var webP = WebPImage.Encode(imageData, width, height, 75.0f);

        var rgba = WebPImage.Decode(webP, WEBP_CSP_MODE.MODE_RGBA);
        File.WriteAllBytes("output-decoded-rgba.rgba", rgba);
        Console.WriteLine($"  RGBA (4 channels): {rgba.Length} bytes");

        var rgb = WebPImage.Decode(webP, WEBP_CSP_MODE.MODE_RGB);
        File.WriteAllBytes("output-decoded-rgb.rgb", rgb);
        Console.WriteLine($"  RGB (3 channels): {rgb.Length} bytes");

        var bgr = WebPImage.Decode(webP, WEBP_CSP_MODE.MODE_BGR);
        File.WriteAllBytes("output-decoded-bgr.bgr", bgr);
        Console.WriteLine($"  BGR (3 channels): {bgr.Length} bytes");

        var argb = WebPImage.Decode(webP, WEBP_CSP_MODE.MODE_ARGB);
        File.WriteAllBytes("output-decoded-argb.argb", argb);
        Console.WriteLine($"  ARGB (4 channels): {argb.Length} bytes");

        Console.WriteLine($"  RGB saves: {((1 - (double)rgb.Length / rgba.Length) * 100):F1}% vs RGBA");
        Console.WriteLine();
    }

    static void ScaleExample(byte[] imageData, int width, int height)
    {
        Console.WriteLine("8. Native Scaling (During Decode)");
        Console.WriteLine(new string('-', 50));

        var webP = WebPImage.Encode(imageData, width, height, 75.0f);

        var fullSize = WebPImage.Decode(webP);
        Console.WriteLine($"  Full size ({width}x{height}): {fullSize.Length} bytes");

        var scaled = WebPImage.Scale(webP, width / 2, height / 2);
        File.WriteAllBytes("output-scaled-400x300.rgba", scaled);
        Console.WriteLine($"  Scaled ({width / 2}x{height / 2}): {scaled.Length} bytes");

        var thumbnail = WebPImage.Scale(webP, width / 4, height / 4);
        File.WriteAllBytes("output-scaled-200x150.rgba", thumbnail);
        Console.WriteLine($"  Thumbnail ({width / 4}x{height / 4}): {thumbnail.Length} bytes");

        Console.WriteLine($"  Thumbnail saves: {((1 - (double)thumbnail.Length / fullSize.Length) * 100):F1}%");
        Console.WriteLine();
    }

    static void CropExample(byte[] imageData, int width, int height)
    {
        Console.WriteLine("9. Native Cropping (During Decode)");
        Console.WriteLine(new string('-', 50));

        var webP = WebPImage.Encode(imageData, width, height, 75.0f);

        var fullSize = WebPImage.Decode(webP);
        Console.WriteLine($"  Full size ({width}x{height}): {fullSize.Length} bytes");

        var cropped = WebPImage.Crop(webP, 100, 100, width / 2, height / 2);
        File.WriteAllBytes("output-cropped.rgba", cropped);
        Console.WriteLine($"  Cropped (100,100, {width / 2}x{height / 2}): {cropped.Length} bytes");

        Console.WriteLine($"  Crop saves: {((1 - (double)cropped.Length / fullSize.Length) * 100):F1}%");
        Console.WriteLine();
    }

    static void FlipExample(byte[] imageData, int width, int height)
    {
        Console.WriteLine("10. Native Flip (During Decode)");
        Console.WriteLine(new string('-', 50));

        var webP = WebPImage.Encode(imageData, width, height, 75.0f);

        var flipped = WebPImage.Flip(webP, WEBP_CSP_MODE.MODE_RGBA);
        File.WriteAllBytes("output-flipped.rgba", flipped);

        Console.WriteLine($"  Created: output-flipped.rgba ({flipped.Length} bytes)");
        Console.WriteLine($"  Flipped vertically (native during decode)");
        Console.WriteLine();
    }

    static void InfoExample(byte[] imageData, int width, int height)
    {
        Console.WriteLine("11. Image Information");
        Console.WriteLine(new string('-', 50));

        var webP = WebPImage.Encode(imageData, width, height, 75.0f);

        var info = WebPImage.GetInfo(webP);
        Console.WriteLine($"  Width: {info.Width}");
        Console.WriteLine($"  Height: {info.Height}");
        Console.WriteLine($"  Has Alpha: {info.HasAlpha}");
        Console.WriteLine($"  Has Animation: {info.HasAnimation}");
        Console.WriteLine($"  Format: {info.Format}");
        Console.WriteLine($"  Total pixels: {info.Width * info.Height:N0}");
        Console.WriteLine();
    }

    static void LibraryInfoExample()
    {
        Console.WriteLine("12. Library Information");
        Console.WriteLine(new string('-', 50));

        var version = WebPImage.GetVersion();
        Console.WriteLine($"  libwebp version: {version}");

        var testWebP = new byte[] { 0x52, 0x49, 0x46, 0x46 };
        var isValid = WebPImage.IsValidWebP(testWebP);
        Console.WriteLine($"  Test WebP validation: {isValid}");

        var validWebP = File.ReadAllBytes("output-encoded-basic.webp");
        var isReallyValid = WebPImage.IsValidWebP(validWebP);
        Console.WriteLine($"  Encoded WebP validation: {isReallyValid}");
        Console.WriteLine();
    }

    static void ValidationExample()
    {
        Console.WriteLine("13. WebP Validation");
        Console.WriteLine(new string('-', 50));

        var invalid = new byte[] { 0x00, 0x01, 0x02, 0x03 };
        Console.WriteLine($"  Invalid data is valid: {WebPImage.IsValidWebP(invalid)}");

        if (File.Exists("output-encoded-basic.webp"))
        {
            var valid = File.ReadAllBytes("output-encoded-basic.webp");
            Console.WriteLine($"  Valid WebP file is valid: {WebPImage.IsValidWebP(valid)}");
        }
        Console.WriteLine();
    }

    static void ScenariosExample(byte[] imageData, int width, int height)
    {
        Console.WriteLine("14. Real-World Scenarios");
        Console.WriteLine(new string('-', 50));

        Console.WriteLine("  Scenario 1: Generate thumbnails from different sources");
        var sizes = new[] { 64, 128, 256, 512 };
        foreach (var size in sizes)
        {
            var webP = WebPImage.Encode(imageData, width, height, 75.0f);
            var thumbnail = WebPImage.Scale(webP, size, size);
            File.WriteAllBytes($"output-thumb-{size}x{size}.rgba", thumbnail);
            Console.WriteLine($"    Generated: {size}x{size} thumbnail ({thumbnail.Length} bytes)");
        }

        Console.WriteLine("  Scenario 2: Multi-quality encoding");
        var webPData = WebPImage.Encode(imageData, width, height, 75.0f);
        var qualities = new[] { 30, 50, 70, 90 };
        foreach (var q in qualities)
        {
            var encoded = WebPImage.Encode(imageData, width, height, (float)q);
            File.WriteAllBytes($"output-quality-{q}.webp", encoded);
            Console.WriteLine($"    Quality {q}: {encoded.Length} bytes");
        }

        Console.WriteLine("  Scenario 3: Color space conversion");
        var decodedRGB = WebPImage.Decode(webPData, WEBP_CSP_MODE.MODE_RGB);
        var decodedBGR = WebPImage.Decode(webPData, WEBP_CSP_MODE.MODE_BGR);
        var decodedRGBA = WebPImage.Decode(webPData, WEBP_CSP_MODE.MODE_RGBA);
        Console.WriteLine($"    RGB: {decodedRGB.Length} bytes");
        Console.WriteLine($"    BGR: {decodedBGR.Length} bytes");
        Console.WriteLine($"    RGBA: {decodedRGBA.Length} bytes");

        Console.WriteLine();
    }
}
