using LegioSoft.Imaging.Core;
using LegioSoft.Imaging.Skia;
using System;
using LegioSoft.Imaging.Core.Enums;

namespace SkiaExample;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== LegioSoft.Imaging.Skia Examples ===\n");

        var exampleImage = "example.png";

        if (!File.Exists(exampleImage))
        {
            Console.WriteLine($"Error: {exampleImage} not found. Please add a test image to the examples folder.");
            return;
        }

        try
        {
            LoadAndDisplayInfo(exampleImage);
            ResizeExample(exampleImage);
            CropExample(exampleImage);
            RotateExample(exampleImage);
            FlipExample(exampleImage);
            FilterExample(exampleImage);
            ColorAdjustmentsExample(exampleImage);
            FormatConversionExample(exampleImage);
            QualityExample(exampleImage);
            ChainOperationsExample(exampleImage);
            SaveExample(exampleImage);
            ScaleModesExample(exampleImage);
            ResizeQualityExample(exampleImage);
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Error: {ex.Message}");
            Console.ResetColor();
        }
    }

    static void LoadAndDisplayInfo(string imagePath)
    {
        Console.WriteLine("1. Load and Display Info");
        Console.WriteLine(new string('-', 50));

        var info = LegioImageBuilder.Load(imagePath).GetInfo();

        Console.WriteLine($"  Width: {info.Width}");
        Console.WriteLine($"  Height: {info.Height}");
        Console.WriteLine($"  Format: {info.Format}");
        Console.WriteLine($"  Has Alpha: {info.HasAlpha}");
        Console.WriteLine($"  Size: {info.ByteSize} bytes");
        Console.WriteLine();
    }

    static void ResizeExample(string imagePath)
    {
        Console.WriteLine("2. Resize Operations");
        Console.WriteLine(new string('-', 50));

        var baseSize = File.ReadAllBytes(imagePath).Length;

        LegioImageBuilder.Load(imagePath)
            .Resize(100, 100, LegioScaleMode.Stretch, LegioResizeQuality.High)
            .Save("output-resized-100x100.jpg", LegioImageFormat.Jpeg, 85);

        Console.WriteLine($"  Created: output-resized-100x100.jpg ({File.ReadAllBytes("output-resized-100x100.jpg").Length} bytes)");

        LegioImageBuilder.Load(imagePath)
            .ResizeToWidth(50)
            .Save("output-resized-width50.jpg", LegioImageFormat.Jpeg, 85);

        Console.WriteLine($"  Created: output-resized-width50.jpg ({File.ReadAllBytes("output-resized-width50.jpg").Length} bytes)");

        LegioImageBuilder.Load(imagePath)
            .Scale(0.5)
            .Save("output-resized-half.jpg", LegioImageFormat.Jpeg, 85);

        Console.WriteLine($"  Created: output-resized-half.jpg ({File.ReadAllBytes("output-resized-half.jpg").Length} bytes)");
        Console.WriteLine();
    }

    static void CropExample(string imagePath)
    {
        Console.WriteLine("3. Crop Operations");
        Console.WriteLine(new string('-', 50));

        LegioImageBuilder.Load(imagePath)
            .Crop(10, 10, 100, 100)
            .Save("output-cropped.jpg", LegioImageFormat.Jpeg, 85);

        Console.WriteLine($"  Created: output-cropped.jpg ({File.ReadAllBytes("output-cropped.jpg").Length} bytes)");
        Console.WriteLine("  Cropped to 100x100 from position (10, 10)");
        Console.WriteLine();
    }

    static void RotateExample(string imagePath)
    {
        Console.WriteLine("4. Rotate Operations");
        Console.WriteLine(new string('-', 50));

        LegioImageBuilder.Load(imagePath)
            .Rotate(90)
            .Save("output-rotated-90.jpg", LegioImageFormat.Jpeg, 85);

        Console.WriteLine($"  Created: output-rotated-90.jpg");

        LegioImageBuilder.Load(imagePath)
            .Rotate(180)
            .Save("output-rotated-180.jpg", LegioImageFormat.Jpeg, 85);

        Console.WriteLine($"  Created: output-rotated-180.jpg");

        LegioImageBuilder.Load(imagePath)
            .Rotate(270)
            .Save("output-rotated-270.jpg", LegioImageFormat.Jpeg, 85);

        Console.WriteLine($"  Created: output-rotated-270.jpg");
        Console.WriteLine();
    }

    static void FlipExample(string imagePath)
    {
        Console.WriteLine("5. Flip Operations");
        Console.WriteLine(new string('-', 50));

        LegioImageBuilder.Load(imagePath)
            .Flip(horizontal: true, vertical: false)
            .Save("output-flipped-horizontal.jpg", LegioImageFormat.Jpeg, 85);

        Console.WriteLine($"  Created: output-flipped-horizontal.jpg");

        LegioImageBuilder.Load(imagePath)
            .Flip(horizontal: false, vertical: true)
            .Save("output-flipped-vertical.jpg", LegioImageFormat.Jpeg, 85);

        Console.WriteLine($"  Created: output-flipped-vertical.jpg");

        LegioImageBuilder.Load(imagePath)
            .Flip(horizontal: true, vertical: true)
            .Save("output-flipped-both.jpg", LegioImageFormat.Jpeg, 85);

        Console.WriteLine($"  Created: output-flipped-both.jpg");
        Console.WriteLine();
    }

    static void FilterExample(string imagePath)
    {
        Console.WriteLine("6. Filter Operations");
        Console.WriteLine(new string('-', 50));

        LegioImageBuilder.Load(imagePath)
            .Grayscale()
            .Save("output-grayscale.jpg", LegioImageFormat.Jpeg, 85);

        Console.WriteLine($"  Created: output-grayscale.jpg");

        LegioImageBuilder.Load(imagePath)
            .Sepia()
            .Save("output-sepia.jpg", LegioImageFormat.Jpeg, 85);

        Console.WriteLine($"  Created: output-sepia.jpg");

        LegioImageBuilder.Load(imagePath)
            .Blur(5)
            .Save("output-blur.jpg", LegioImageFormat.Jpeg, 85);

        Console.WriteLine($"  Created: output-blur.jpg");

        LegioImageBuilder.Load(imagePath)
            .Sharpen(50)
            .Save("output-sharpen.jpg", LegioImageFormat.Jpeg, 85);

        Console.WriteLine($"  Created: output-sharpen.jpg");
        Console.WriteLine();
    }

    static void ColorAdjustmentsExample(string imagePath)
    {
        Console.WriteLine("7. Color Adjustments");
        Console.WriteLine(new string('-', 50));

        LegioImageBuilder.Load(imagePath)
            .Brightness(30)
            .Save("output-brightness.jpg", LegioImageFormat.Jpeg, 85);

        Console.WriteLine($"  Created: output-brightness.jpg (+30 brightness)");

        LegioImageBuilder.Load(imagePath)
            .Brightness(-30)
            .Save("output-darkness.jpg", LegioImageFormat.Jpeg, 85);

        Console.WriteLine($"  Created: output-darkness.jpg (-30 brightness)");

        LegioImageBuilder.Load(imagePath)
            .Contrast(30)
            .Save("output-contrast-high.jpg", LegioImageFormat.Jpeg, 85);

        Console.WriteLine($"  Created: output-contrast-high.jpg (+30 contrast)");

        LegioImageBuilder.Load(imagePath)
            .Contrast(-30)
            .Save("output-contrast-low.jpg", LegioImageFormat.Jpeg, 85);

        Console.WriteLine($"  Created: output-contrast-low.jpg (-30 contrast)");

        LegioImageBuilder.Load(imagePath)
            .Invert()
            .Save("output-inverted.jpg", LegioImageFormat.Jpeg, 85);

        Console.WriteLine($"  Created: output-inverted.jpg");
        Console.WriteLine();
    }

    static void FormatConversionExample(string imagePath)
    {
        Console.WriteLine("8. Format Conversion");
        Console.WriteLine(new string('-', 50));

        var builder = LegioImageBuilder.Load(imagePath);

        var png = builder.SaveAs(LegioImageFormat.Png, 100);

        Console.WriteLine($"  Created: output-converted-png.png ({png.Length} bytes)");

        var jpeg = builder.SaveAs(LegioImageFormat.Jpeg, 85);

        Console.WriteLine($"  Created: output-converted-jpeg.jpg ({jpeg.Length} bytes)");

        var webp = builder.SaveAs(LegioImageFormat.WebP, 85);

        Console.WriteLine($"  Created: output-converted-webp.webp ({webp.Length} bytes)");
        
        Console.WriteLine();
    }

    static void QualityExample(string imagePath)
    {
        Console.WriteLine("9. Quality Control");
        Console.WriteLine(new string('-', 50));

        LegioImageBuilder.Load(imagePath)
            .Resize(200, 200, LegioScaleMode.Stretch, LegioResizeQuality.High)
            .Quality(50)
            .Save("output-quality-low.jpg", LegioImageFormat.Jpeg);

        Console.WriteLine($"  Low quality: {File.ReadAllBytes("output-quality-low.jpg").Length} bytes");

        LegioImageBuilder.Load(imagePath)
            .Resize(200, 200, LegioScaleMode.Stretch, LegioResizeQuality.High)
            .Quality(75)
            .Save("output-quality-medium.jpg", LegioImageFormat.Jpeg);

        Console.WriteLine($"  Medium quality: {File.ReadAllBytes("output-quality-medium.jpg").Length} bytes");

        LegioImageBuilder.Load(imagePath)
            .Resize(200, 200, LegioScaleMode.Stretch, LegioResizeQuality.High)
            .Quality(90)
            .Save("output-quality-high.jpg", LegioImageFormat.Jpeg);

        Console.WriteLine($"  High quality: {File.ReadAllBytes("output-quality-high.jpg").Length} bytes");

        LegioImageBuilder.Load(imagePath)
            .Resize(200, 200, LegioScaleMode.Stretch, LegioResizeQuality.High)
            .Quality(100)
            .Save("output-quality-maximum.jpg", LegioImageFormat.Jpeg);

        Console.WriteLine($"  Maximum quality: {File.ReadAllBytes("output-quality-maximum.jpg").Length} bytes");
        Console.WriteLine();
    }

    static void ChainOperationsExample(string imagePath)
    {
        Console.WriteLine("10. Chained Operations");
        Console.WriteLine(new string('-', 50));

        LegioImageBuilder.Load(imagePath)
            .Resize(300, 300, LegioScaleMode.Fit, LegioResizeQuality.High)
            .Crop(10, 10, 200, 200)
            .Rotate(90)
            .Brightness(20)
            .Contrast(10)
            .Grayscale()
            .Save("output-chained.jpg", LegioImageFormat.Jpeg, 85);

        Console.WriteLine($"  Created: output-chained.jpg ({File.ReadAllBytes("output-chained.jpg").Length} bytes)");
        Console.WriteLine("  Applied: Resize → Crop → Rotate → Brightness → Contrast → Grayscale");
        Console.WriteLine();
    }

    static void SaveExample(string imagePath)
    {
        Console.WriteLine("11. Save Methods");
        Console.WriteLine(new string('-', 50));

        var builder = LegioImageBuilder.Load(imagePath);

        var asBytes = builder.SaveAs(LegioImageFormat.Png);
        Console.WriteLine($"  SaveAs PNG: {asBytes.Length} bytes");

        var asBytesQuality = builder.SaveAs(LegioImageFormat.Jpeg, 90);
        Console.WriteLine($"  SaveAs JPEG with quality: {asBytesQuality.Length} bytes");

        builder.Save("output-save-method.jpg", LegioImageFormat.Jpeg, 85);
        Console.WriteLine($"  Save to file: {File.ReadAllBytes("output-save-method.jpg").Length} bytes");

        using var stream = builder.SaveAsStream(LegioImageFormat.Png);
        Console.WriteLine($"  Save as stream: {stream.Length} bytes");
        Console.WriteLine();
    }

    static void ScaleModesExample(string imagePath)
    {
        Console.WriteLine("12. Scale Modes");
        Console.WriteLine(new string('-', 50));

        LegioImageBuilder.Load(imagePath)
            .Resize(200, 200, LegioScaleMode.Fit, LegioResizeQuality.High)
            .Save("output-scale-fit.jpg", LegioImageFormat.Jpeg, 85);

        Console.WriteLine($"  Fit mode: {File.ReadAllBytes("output-scale-fit.jpg").Length} bytes");

        LegioImageBuilder.Load(imagePath)
            .Resize(200, 200, LegioScaleMode.Fill, LegioResizeQuality.High)
            .Save("output-scale-fill.jpg", LegioImageFormat.Jpeg, 85);

        Console.WriteLine($"  Fill mode: {File.ReadAllBytes("output-scale-fill.jpg").Length} bytes");

        LegioImageBuilder.Load(imagePath)
            .Resize(200, 200, LegioScaleMode.Stretch, LegioResizeQuality.High)
            .Save("output-scale-stretch.jpg", LegioImageFormat.Jpeg, 85);

        Console.WriteLine($"  Stretch mode: {File.ReadAllBytes("output-scale-stretch.jpg").Length} bytes");
        Console.WriteLine();
    }

    static void ResizeQualityExample(string imagePath)
    {
        Console.WriteLine("13. Resize Quality Levels");
        Console.WriteLine(new string('-', 50));

        LegioImageBuilder.Load(imagePath)
            .Resize(400, 300, LegioScaleMode.Stretch, LegioResizeQuality.Low)
            .Save("output-resize-quality-low.jpg", LegioImageFormat.Jpeg, 85);

        Console.WriteLine($"  Low quality resize: {File.ReadAllBytes("output-resize-quality-low.jpg").Length} bytes");

        LegioImageBuilder.Load(imagePath)
            .Resize(400, 300, LegioScaleMode.Stretch, LegioResizeQuality.Medium)
            .Save("output-resize-quality-medium.jpg", LegioImageFormat.Jpeg, 85);

        Console.WriteLine($"  Medium quality resize: {File.ReadAllBytes("output-resize-quality-medium.jpg").Length} bytes");

        LegioImageBuilder.Load(imagePath)
            .Resize(400, 300, LegioScaleMode.Stretch, LegioResizeQuality.High)
            .Save("output-resize-quality-high.jpg", LegioImageFormat.Jpeg, 85);

        Console.WriteLine($"  High quality resize: {File.ReadAllBytes("output-resize-quality-high.jpg").Length} bytes");

        LegioImageBuilder.Load(imagePath)
            .Resize(400, 300, LegioScaleMode.Stretch, LegioResizeQuality.Maximum)
            .Save("output-resize-quality-maximum.jpg", LegioImageFormat.Jpeg, 85);

        Console.WriteLine($"  Maximum quality resize: {File.ReadAllBytes("output-resize-quality-maximum.jpg").Length} bytes");
        Console.WriteLine();
    }
}
