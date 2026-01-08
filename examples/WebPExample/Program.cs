using LegioSoft.Imaging.Core;
using LegioSoft.Imaging.WebP;
using System;

namespace WebPExample;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== LegioSoft.Imaging.WebP Examples ===\n");

        var exampleImage = "test-image.png";

        if (!File.Exists(exampleImage))
        {
            Console.WriteLine($"Error: {exampleImage} not found. Please add a test image to the examples folder.");
            Console.WriteLine("\nNote: Create a test image by running the Skia example first.");
            return;
        }

        try
        {
            InterfaceImplementationExample(exampleImage);
            FormatSupportExample();
            PerformanceExample(exampleImage);
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Error: {ex.Message}");
            Console.ResetColor();
        }
    }

    static void InterfaceImplementationExample(string imagePath)
    {
        Console.WriteLine("1. Interface Implementation");
        Console.WriteLine(new string('-', 50));

        var encoder = new LegioImageWebPEncoder();
        
        Console.WriteLine("  LegioImageWebPEncoder implements:");
        Console.WriteLine($"    - ILegioImageEncoder: {encoder is ILegioImageEncoder}");
        Console.WriteLine($"    - ILegioImageDecoder: {encoder is ILegioImageDecoder}");
        Console.WriteLine();
    }

    static void FormatSupportExample()
    {
        Console.WriteLine("2. Format Support");
        Console.WriteLine(new string('-', 50));

        Console.WriteLine("  LegioSoft.Imaging.WebP currently implements:");
        Console.WriteLine("    - ILegioImageEncoder interface");
        Console.WriteLine("    - ILegioImageDecoder interface");
        Console.WriteLine("    - Throws NotImplementedException for decode operations");
        Console.WriteLine("    - Throws NotSupportedException for non-WebP encoding");
        Console.WriteLine();
        Console.WriteLine("  Available WebP capabilities:");
        Console.WriteLine("    ✓ WebPDecoder.DecodeWithScaling - Decode WebP with resize during decode");
        Console.WriteLine("    ✓ WebPImage.Scale - Scale WebP images during decode");
        Console.WriteLine("    ✓ Native libwebp scaling (fast and efficient)");
        Console.WriteLine("    ✓ WEBP_CSP_MODE support (RGBA, BGR, etc.)");
        Console.WriteLine();
    }

    static void PerformanceExample(string imagePath)
    {
        Console.WriteLine("3. WebP Package Benefits");
        Console.WriteLine(new string('-', 50));

        Console.WriteLine("  Why use LegioSoft.Imaging.WebP?");
        Console.WriteLine("    ✓ Lightweight (~500KB vs 5MB for Skia)");
        Console.WriteLine("    ✓ Faster for simple WebP operations");
        Console.WriteLine("    ✓ No SkiaSharp dependency");
        Console.WriteLine("    ✓ Cross-platform native libraries");
        Console.WriteLine("    ✓ Lower memory footprint");
        Console.WriteLine("    ✓ Native scaling during decode (no separate resize step)");
        Console.WriteLine();
        Console.WriteLine("  Use cases:");
        Console.WriteLine("    • Web-only applications");
        Console.WriteLine("    • Server-side image conversion (WebP with scaling)");
        Console.WriteLine("    • Mobile apps with limited resources");
        Console.WriteLine("    • CI/CD pipelines");
        Console.WriteLine("    • Thumbnail generation from WebP sources");
        Console.WriteLine();
    }

    static void ComparisonExample()
    {
        Console.WriteLine("4. Package Comparison");
        Console.WriteLine(new string('-', 50));

        Console.WriteLine("  LegioSoft.Imaging.WebP:");
        Console.WriteLine("    Package: ~500KB");
        Console.WriteLine("    Dependencies: LegioSoft.Imaging.Core + libwebp (native)");
        Console.WriteLine("    Features: WebP decode with native scaling");
        Console.WriteLine("    Limitations: Cannot transform non-WebP images");
        Console.WriteLine("    Best for: WebP decoding with scaling");
        Console.WriteLine();
        Console.WriteLine("  LegioSoft.Imaging.Skia:");
        Console.WriteLine("    Package: ~5MB");
        Console.WriteLine("    Dependencies: LegioSoft.Imaging.Core + SkiaSharp");
        Console.WriteLine("    Features: Full image processing (all formats)");
        Console.WriteLine("    Best for: General image processing");
        Console.WriteLine();
    }

    static void UsageExample(string imagePath)
    {
        Console.WriteLine("5. WebP Decode with Scaling");
        Console.WriteLine(new string('-', 50));

        Console.WriteLine("  Decode WebP and scale during decode (native speed):");
        Console.WriteLine();
        Console.WriteLine("  var webpData = File.ReadAllBytes(\"image.webp\");");
        Console.WriteLine("  var scaled = WebPDecoder.DecodeWithScaling(webpData, 800, 600);");
        Console.WriteLine("  File.WriteAllBytes(\"scaled-rgba.data\", scaled);");
        Console.WriteLine();
        Console.WriteLine("  Decode with specific color space:");
        Console.WriteLine("  var bgr = WebPDecoder.DecodeWithScaling(webpData, 800, 600, WEBP_CSP_MODE.MODE_BGR);");
        Console.WriteLine();
        Console.WriteLine("  Scale existing WebP image:");
        Console.WriteLine("  var scaled = WebPImage.Scale(webpData, 800, 600);");
        Console.WriteLine("  File.WriteAllBytes(\"scaled.webp\", scaled);");
        Console.WriteLine();
        Console.WriteLine("  Scale WebP image from file:");
        Console.WriteLine("  var scaled = WebPImage.Scale(\"image.webp\", 800, 600);");
        Console.WriteLine("  File.WriteAllBytes(\"scaled.webp\", scaled);");
        Console.WriteLine();
        Console.WriteLine("  Scale WebP image from stream:");
        Console.WriteLine("  using var stream = File.OpenRead(\"image.webp\");");
        Console.WriteLine("  var scaled = WebPImage.Scale(stream, 800, 600);");
        Console.WriteLine("  File.WriteAllBytes(\"scaled.webp\", scaled);");
        Console.WriteLine();
    }

    static void InstallationExample()
    {
        Console.WriteLine("6. Installation");
        Console.WriteLine(new string('-', 50));

        Console.WriteLine("  Install WebP package:");
        Console.WriteLine("  dotnet add package LegioSoft.Imaging.WebP");
        Console.WriteLine();
        Console.WriteLine("  Or add reference to project file:");
        Console.WriteLine("  <PackageReference Include=\"LegioSoft.Imaging.WebP\" />");
        Console.WriteLine();
        Console.WriteLine("  Don't forget Core package (it's a dependency):");
        Console.WriteLine("  <PackageReference Include=\"LegioSoft.Imaging.Core\" />");
        Console.WriteLine();
    }

    static void ScenariosExample()
    {
        Console.WriteLine("7. Real-World Scenarios");
        Console.WriteLine(new string('-', 50));

        Console.WriteLine("  Scenario 1: Generate thumbnail from WebP");
        Console.WriteLine("  var webpData = File.ReadAllBytes(\"large.webp\");");
        Console.WriteLine("  var thumbnail = WebPDecoder.DecodeWithScaling(webpData, 200, 150);");
        Console.WriteLine("  File.WriteAllBytes(\"thumbnail.rgba\", thumbnail);");
        Console.WriteLine();
        Console.WriteLine("  Scenario 2: Decode WebP to multiple sizes");
        Console.WriteLine("  var webpData = File.ReadAllBytes(\"source.webp\");");
        Console.WriteLine("  var sizes = new[] { 320, 640, 1024 };");
        Console.WriteLine("  foreach (var size in sizes) {");
        Console.WriteLine("    var decoded = WebPDecoder.DecodeWithScaling(webpData, size, (int)(size * 0.75));");
        Console.WriteLine("    File.WriteAllBytes($\"{size}w.rgba\", decoded);");
        Console.WriteLine("  }");
        Console.WriteLine();
        Console.WriteLine("  Scenario 3: WebP batch processing");
        Console.WriteLine("  var webpFiles = Directory.GetFiles(\"input\", \"*.webp\");");
        Console.WriteLine("  foreach (var file in webpFiles) {");
        Console.WriteLine("    var data = File.ReadAllBytes(file);");
        Console.WriteLine("    var scaled = WebPDecoder.DecodeWithScaling(data, 800, 600);");
        Console.WriteLine("    var filename = Path.GetFileNameWithoutExtension(file);");
        Console.WriteLine("    File.WriteAllBytes($\"output/{filename}_scaled.rgba\", scaled);");
        Console.WriteLine("  }");
        Console.WriteLine();
    }
}


        try
        {
            InterfaceImplementationExample(exampleImage);
            FormatSupportExample();
            PerformanceExample(exampleImage);
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Error: {ex.Message}");
            Console.ResetColor();
        }
    }

    static void InterfaceImplementationExample(string imagePath)
    {
        Console.WriteLine("1. Interface Implementation");
        Console.WriteLine(new string('-', 50));

        var encoder = new LegioImageWebPEncoder();
        
        Console.WriteLine("  LegioImageWebPEncoder implements:");
        Console.WriteLine($"    - ILegioImageEncoder: {encoder is ILegioImageEncoder}");
        Console.WriteLine($"    - ILegioImageDecoder: {encoder is ILegioImageDecoder}");
        Console.WriteLine();
    }

    static void FormatSupportExample()
    {
        Console.WriteLine("2. Format Support");
        Console.WriteLine(new string('-', 50));

        Console.WriteLine("  LegioSoft.Imaging.WebP currently implements:");
        Console.WriteLine("    - ILegioImageEncoder interface");
        Console.WriteLine("    - ILegioImageDecoder interface");
        Console.WriteLine("    - Throws NotImplementedException for decode operations");
        Console.WriteLine("    - Throws NotSupportedException for non-WebP encoding");
        Console.WriteLine();
        Console.WriteLine("  Full WebP support will be added in future versions:");
        Console.WriteLine("    - Encoding from RGBA/BGR formats");
        Console.WriteLine("    - Decoding WebP to RGBA");
        Console.WriteLine("    - Image information extraction");
        Console.WriteLine("    - Lossless and lossy encoding");
        Console.WriteLine("    - Native library integration");
        Console.WriteLine();
    }

    static void PerformanceExample(string imagePath)
    {
        Console.WriteLine("3. WebP Package Benefits");
        Console.WriteLine(new string('-', 50));

        Console.WriteLine("  Why use LegioSoft.Imaging.WebP?");
        Console.WriteLine("    ✓ Lightweight (~500KB vs 5MB for Skia)");
        Console.WriteLine("    ✓ Faster for simple WebP operations");
        Console.WriteLine("    ✓ No SkiaSharp dependency");
        Console.WriteLine("    ✓ Cross-platform native libraries");
        Console.WriteLine("    ✓ Lower memory footprint");
        Console.WriteLine();
        Console.WriteLine("  Use cases:");
        Console.WriteLine("    • Web-only applications");
        Console.WriteLine("    • Server-side image conversion");
        Console.WriteLine("    • Mobile apps with limited resources");
        Console.WriteLine("    • CI/CD pipelines");
        Console.WriteLine();
    }

    static void ComparisonExample()
    {
        Console.WriteLine("4. Package Comparison");
        Console.WriteLine(new string('-', 50));

        Console.WriteLine("  LegioSoft.Imaging.WebP:");
        Console.WriteLine("    Package: ~500KB");
        Console.WriteLine("    Dependencies: LegioSoft.Imaging.Core + libwebp (native)");
        Console.WriteLine("    Best for: WebP encoding/decoding only");
        Console.WriteLine();
        Console.WriteLine("  LegioSoft.Imaging.Skia:");
        Console.WriteLine("    Package: ~5MB");
        Console.WriteLine("    Dependencies: LegioSoft.Imaging.Core + SkiaSharp");
        Console.WriteLine("    Best for: Full image processing");
        Console.WriteLine();
    }

    static void UsageExample(string imagePath)
    {
        Console.WriteLine("5. Usage Example");
        Console.WriteLine(new string('-', 50));

        Console.WriteLine("  Current API (NotImplementedException):");
        Console.WriteLine();
        Console.WriteLine("  var encoder = new LegioImageWebPEncoder();");
        Console.WriteLine("  var decoded = encoder.Decode(webpData, out var format);");
        Console.WriteLine("  var info = encoder.GetImageInfo(webpData);");
        Console.WriteLine();
        Console.WriteLine("  Future API (not yet implemented):");
        Console.WriteLine("  var encoder = new LegioImageWebPEncoder();");
        Console.WriteLine("  var webpData = encoder.Encode(rgbaData, LegioImageFormat.WebP, quality: 85);");
        Console.WriteLine("  var webpData = encoder.Encode(rgbaData, width, height, quality: 85);");
        Console.WriteLine("  var webpData = encoder.EncodeLossless(rgbaData, width, height);");
        Console.WriteLine();
    }

    static void InstallationExample()
    {
        Console.WriteLine("6. Installation");
        Console.WriteLine(new string('-', 50));

        Console.WriteLine("  Install WebP package:");
        Console.WriteLine("  dotnet add package LegioSoft.Imaging.WebP");
        Console.WriteLine();
        Console.WriteLine("  Or add reference to project file:");
        Console.WriteLine("  <PackageReference Include=\"LegioSoft.Imaging.WebP\" />");
        Console.WriteLine();
        Console.WriteLine("  Don't forget Core package (it's a dependency):");
        Console.WriteLine("  <PackageReference Include=\"LegioSoft.Imaging.Core\" />");
        Console.WriteLine();
    }
}
