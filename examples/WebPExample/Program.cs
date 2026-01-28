using LegioSoft.Imaging.Core;
using LegioSoft.Imaging.Core.Enums;
using LegioSoft.Imaging.WebP;

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
            Console.WriteLine("Add test image to output directory or check build configuration.");
            return;
        }

        try
        {
            Console.WriteLine("1. Load and Display Info");
            Console.WriteLine(new string('-', 50));
            byte[] imageData;
            int width, height;
            using (var bitmap = new System.Drawing.Bitmap(exampleImage))
            {
                width = bitmap.Width;
                height = bitmap.Height;
                var rect = new System.Drawing.Rectangle(0, 0, width, height);
                var bmpData = bitmap.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                imageData = new byte[width * height * 4];
                System.Runtime.InteropServices.Marshal.Copy(bmpData.Scan0, imageData, 0, imageData.Length);
                bitmap.UnlockBits(bmpData);
            }
            Console.WriteLine($"  Loaded: {exampleImage}");
            Console.WriteLine($"  Dimensions: {width}x{height}");
            Console.WriteLine($"  Pixel data: {imageData.Length} bytes");
            Console.WriteLine();

            Console.WriteLine("2. Encode to WebP");
            Console.WriteLine(new string('-', 50));
            var processor = new LegioImageWebPProcessor();
            var webpData = processor.Encode(imageData, LegioImageFormat.WebP, LegioEncodingQuality.High);
            File.WriteAllBytes("encoded.webp", webpData);
            Console.WriteLine($"  Created: encoded.webp ({webpData.Length} bytes)");
            Console.WriteLine($"  Compression ratio: {((1 - (double)webpData.Length / imageData.Length) * 100):F1}%");
            Console.WriteLine();

            Console.WriteLine("3. Decode WebP");
            Console.WriteLine(new string('-', 50));
            var decoded = processor.Decode(webpData, out var format);
            Console.WriteLine($"  Decoded to RGBA: {decoded.Length} bytes");
            Console.WriteLine($"  Format: {format}");
            Console.WriteLine();

            Console.WriteLine("4. Get Image Info");
            Console.WriteLine(new string('-', 50));
            var info = processor.GetImageInfo(webpData);
            Console.WriteLine($"  Width: {info.Width}");
            Console.WriteLine($"  Height: {info.Height}");
            Console.WriteLine($"  Format: {info.Format}");
            Console.WriteLine($"  Has Alpha: {info.HasAlpha}");
            Console.WriteLine($"  Byte Size: {info.ByteSize} bytes");
            Console.WriteLine();

            Console.WriteLine("5. Resize WebP");
            Console.WriteLine(new string('-', 50));
            var resized = processor.Resize(webpData, width / 2, height / 2, LegioScaleMode.Fit, LegioResizeQuality.High);
            var resizedInfo = processor.GetImageInfo(resized);
            Console.WriteLine($"  Resized to: {resizedInfo.Width}x{resizedInfo.Height}");
            Console.WriteLine($"  Pixel data: {resized.Length} bytes");
            File.WriteAllBytes("resized.rgba", resized);
            Console.WriteLine();

            Console.WriteLine("6. Crop WebP");
            Console.WriteLine(new string('-', 50));
            var cropped = processor.Crop(webpData, 50, 50, width / 2, height / 2);
            Console.WriteLine($"  Cropped: {cropped.Length} bytes");
            File.WriteAllBytes("cropped.rgba", cropped);
            Console.WriteLine();

            Console.WriteLine("7. Rotate WebP");
            Console.WriteLine(new string('-', 50));
            var rotated = processor.Rotate(webpData, 90);
            Console.WriteLine($"  Rotated 90°: {rotated.Length} bytes");
            File.WriteAllBytes("rotated.rgba", rotated);
            Console.WriteLine();

            Console.WriteLine("8. Flip WebP");
            Console.WriteLine(new string('-', 50));
            var flipped = processor.Flip(webpData, false, true);
            Console.WriteLine($"  Flipped vertically: {flipped.Length} bytes");
            File.WriteAllBytes("flipped.rgba", flipped);
            Console.WriteLine();

            Console.WriteLine("9. Save Decoded WebP as PNG");
            Console.WriteLine(new string('-', 50));
            using (var bitmap = new System.Drawing.Bitmap(info.Width, info.Height))
            {
                var rect = new System.Drawing.Rectangle(0, 0, info.Width, info.Height);
                var bmpData = bitmap.LockBits(rect, System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                System.Runtime.InteropServices.Marshal.Copy(decoded, 0, bmpData.Scan0, decoded.Length);
                bitmap.UnlockBits(bmpData);
                bitmap.Save("decoded.png", System.Drawing.Imaging.ImageFormat.Png);
            }
            Console.WriteLine($"  Saved: decoded.png");
            Console.WriteLine();

            Console.WriteLine("10. Resize and Save as PNG");
            Console.WriteLine(new string('-', 50));
            var thumbnail = processor.Resize(webpData, 200, 200, LegioScaleMode.Fit, LegioResizeQuality.High);
            var thumbInfo = processor.GetImageInfo(thumbnail);
            using (var bitmap = new System.Drawing.Bitmap(thumbInfo.Width, thumbInfo.Height))
            {
                var rect = new System.Drawing.Rectangle(0, 0, thumbInfo.Width, thumbInfo.Height);
                var bmpData = bitmap.LockBits(rect, System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                System.Runtime.InteropServices.Marshal.Copy(thumbnail, 0, bmpData.Scan0, thumbnail.Length);
                bitmap.UnlockBits(bmpData);
                bitmap.Save("thumbnail.png", System.Drawing.Imaging.ImageFormat.Png);
            }
            Console.WriteLine($"  Created thumbnail: 200x200");
            Console.WriteLine($"  Saved: thumbnail.png");
            Console.WriteLine();

            Console.WriteLine("11. Multiple Quality Levels");
            Console.WriteLine(new string('-', 50));
            var qualities = new[] { LegioEncodingQuality.Low, LegioEncodingQuality.Medium, LegioEncodingQuality.High, LegioEncodingQuality.Maximum };
            foreach (var quality in qualities)
            {
                var qWebP = processor.Encode(imageData, LegioImageFormat.WebP, quality);
                var fileName = $"quality-{quality}.webp";
                File.WriteAllBytes(fileName, qWebP);
                Console.WriteLine($"  {quality}: {fileName} ({qWebP.Length} bytes)");
            }
            Console.WriteLine();

            Console.WriteLine("=== All Examples Completed Successfully ===");
            Console.WriteLine();
            Console.WriteLine("Generated files:");
            Console.WriteLine("  - encoded.webp (WebP encoded image)");
            Console.WriteLine("  - resized.rgba (Resized pixel data)");
            Console.WriteLine("  - cropped.rgba (Cropped pixel data)");
            Console.WriteLine("  - rotated.rgba (Rotated pixel data)");
            Console.WriteLine("  - flipped.rgba (Flipped pixel data)");
            Console.WriteLine("  - decoded.png (Decoded WebP as PNG)");
            Console.WriteLine("  - thumbnail.png (200x200 thumbnail)");
            Console.WriteLine("  - quality-Low.webp, quality-Medium.webp, etc.");
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Error: {ex.Message}");
            Console.WriteLine($"Stack: {ex.StackTrace}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"Inner: {ex.InnerException.Message}");
            }
            Console.ResetColor();
        }
    }
}
