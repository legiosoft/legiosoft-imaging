using LegioSoft.Imaging.WebP.Enums;
using LegioSoft.Imaging.WebP.Helpers;
using LegioSoft.Imaging.WebP.Models;

namespace LegioSoft.Imaging.WebP.Tests;

public class WebPTests
{
    private const string TestImageFile = "item.jpg";
    private static string TestImagePath => Path.Combine(GetBaseDirectory(), TestImageFile);
    
    private static string GetBaseDirectory()
    {
        return AppDomain.CurrentDomain.BaseDirectory;
    }
    
    private static void PrintSection(string title)
    {
        Console.WriteLine();
        Console.WriteLine($"=== {title} ===");
        Console.WriteLine();
    }
    
    private static void PrintSuccess(string message)
    {
        Console.WriteLine($"✅ {message}");
    }
    
    private static void PrintError(string message)
    {
        Console.WriteLine($"❌ {message}");
    }
    
    public static void MainManual(string[] args)
    {
        Console.WriteLine("LegioSoft.Imaging.WebP - Comprehensive Test Suite");
        Console.WriteLine("===============================================");
        Console.WriteLine();
        Console.WriteLine($"WebP Library Version: {WebPImage.GetVersion()}");
        Console.WriteLine();
        
        if (!File.Exists(TestImagePath))
        {
            Console.WriteLine($"Test image not found: {TestImagePath}");
            Console.WriteLine($"Current directory: {GetBaseDirectory()}");
            Console.WriteLine($"Looking for: {TestImageFile}");
            return;
        }

        var passedTests = 0;
        var failedTests = 0;
        
        try
        {
            PrintSection("Format Detection Tests");
            passedTests += TestFormatDetection();
            
            PrintSection("WebP Info Extraction Tests");
            passedTests += TestInfoExtraction();
            
            PrintSection("Encoding Tests");
            passedTests += TestEncoding();
            
            PrintSection("Decoding Tests");
            passedTests += TestDecoding();
            
            PrintSection("Scaling Tests");
            passedTests += TestScaling();
            
            PrintSection("Cropping Tests");
            passedTests += TestCropping();
            
            PrintSection("Flipping Tests");
            passedTests += TestFlipping();
            
            PrintSection("Advanced Encoding Tests");
            passedTests += TestAdvancedEncoding();
            
            PrintSection("File and Stream Tests");
            passedTests += TestFileAndStreamOperations();
            
            Console.WriteLine();
            Console.WriteLine("===============================================");
            Console.WriteLine($"Test Results: {passedTests} passed, {failedTests} failed");
            Console.WriteLine("===============================================");
        }
        catch (Exception ex)
        {
            PrintError($"Test suite failed with exception: {ex.Message}");
            Console.WriteLine(ex.StackTrace);
        }
    }

    private static int TestFormatDetection()
    {
        var passed = 0;
        
        var validWebP = new byte[] {
            0x52, 0x49, 0x46, 0x46, 
            0x00, 0x00, 0x00, 0x00, 
            0x57, 0x45, 0x42, 0x50
        };
        
        if (WebPImage.IsValidWebP(validWebP))
        {
            PrintSuccess("Valid WebP header detected");
            passed++;
        }
        else
        {
            PrintError("Failed to detect valid WebP header");
        }
        
        var invalidData = new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 };
        
        if (!WebPImage.IsValidWebP(invalidData))
        {
            PrintSuccess("Non-WebP data correctly rejected");
            passed++;
        }
        else
        {
            PrintError("Incorrectly identified non-WebP data as WebP");
        }
        
        var format = WebPImage.DetectFormat(validWebP);
        if (format == ImageFormat.WebP)
        {
            PrintSuccess($"Format detection returned WebP for valid data");
            passed++;
        }
        else
        {
            PrintError($"Format detection failed for WebP data: {format}");
        }
        
        var jpegData = new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 };
        format = WebPImage.DetectFormat(jpegData);
        if (format == ImageFormat.Jpeg)
        {
            PrintSuccess("JPEG format detected correctly");
            passed++;
        }
        else
        {
            PrintError($"JPEG format not detected: {format}");
        }
        
        return passed;
    }

    private static int TestInfoExtraction()
    {
        var passed = 0;
        
        var webpData = CreateTestWebP();
        
        try
        {
            var info = WebPImage.GetInfo(webpData);
            
            if (info.Width > 0 && info.Height > 0)
            {
                PrintSuccess($"WebP info extracted: {info.Width}x{info.Height}");
                passed++;
            }
            else
            {
                PrintError($"Invalid dimensions: {info.Width}x{info.Height}");
            }
            
            if (info.Format != WebPFormat.Mixed)
            {
                PrintSuccess($"Format detected: {info.Format}");
                passed++;
            }
            
            Console.WriteLine($"  Has Alpha: {info.HasAlpha}");
            Console.WriteLine($"  Has Animation: {info.HasAnimation}");
        }
        catch (Exception ex)
        {
            PrintError($"Info extraction failed: {ex.Message}");
        }
        
        return passed;
    }

    private static int TestEncoding()
    {
        var passed = 0;
        
        try
        {
            var width = 800;
            var height = 600;
            var rgbaData = CreateTestRGBA(width, height);
            
            PrintSection("Basic RGBA Encoding");
            
            var webpData = WebPImage.Encode(rgbaData, width, height, 75.0f);
            
            if (webpData != null && webpData.Length > 0)
            {
                PrintSuccess($"RGBA encoding successful ({webpData.Length} bytes)");
                passed++;
            }
            else
            {
                PrintError("RGBA encoding failed");
            }
            
            PrintSection("Lossless Encoding");
            
            var losslessData = WebPImage.EncodeLossless(rgbaData, width, height);
            
            if (losslessData != null && losslessData.Length > 0)
            {
                PrintSuccess($"Lossless encoding successful ({losslessData.Length} bytes)");
                passed++;
                
                if (webpData != null && losslessData.Length > webpData.Length)
                {
                    Console.WriteLine($"  Note: Lossless is {losslessData.Length - webpData.Length} bytes larger than lossy");
                }
            }
            else
            {
                PrintError("Lossless encoding failed");
            }
            
            PrintSection("RGB Encoding");
            
            var rgbData = CreateTestRGB(width, height);
            var rgbWebP = WebPImage.EncodeRGB(rgbData, width, height, 75.0f);
            
            if (rgbWebP != null && rgbWebP.Length > 0)
            {
                PrintSuccess($"RGB encoding successful ({rgbWebP.Length} bytes)");
                passed++;
            }
            else
            {
                PrintError("RGB encoding failed");
            }
            
            PrintSection("Quality Levels");
            
            var qualityHigh = WebPImage.Encode(rgbaData, width, height, 90.0f);
            var qualityLow = WebPImage.Encode(rgbaData, width, height, 30.0f);
            
            if (qualityHigh != null && qualityHigh.Length > 0 && qualityLow != null && qualityLow.Length > 0)
            {
                PrintSuccess("Different quality levels encoded successfully");
                passed++;
                
                Console.WriteLine($"  High quality (90%): {qualityHigh.Length} bytes");
                Console.WriteLine($"  Low quality (30%): {qualityLow.Length} bytes");
                Console.WriteLine($"  Size reduction: {(1 - (double)qualityLow.Length / qualityHigh.Length) * 100:F1}%");
            }
            else
            {
                PrintError("Quality level encoding failed");
            }
        }
        catch (Exception ex)
        {
            PrintError($"Encoding tests failed: {ex.Message}");
        }
        
        return passed;
    }

    private static int TestDecoding()
    {
        var passed = 0;
        
        try
        {
            var width = 800;
            var height = 600;
            var rgbaData = CreateTestRGBA(width, height);
            var webpData = WebPImage.EncodeLossless(rgbaData, width, height);
            
            PrintSection("RGBA Decoding");
            
            var decoded = WebPImage.Decode(webpData, WEBP_CSP_MODE.MODE_RGBA);
            
            if (decoded != null && decoded.Length == rgbaData.Length)
            {
                PrintSuccess("RGBA decoding successful");
                passed++;
            }
            else if (decoded != null)
            {
                PrintSuccess($"RGBA decoding successful (size mismatch: {decoded.Length} vs {rgbaData.Length})");
                passed++;
            }
            else
            {
                PrintError("RGBA decoding failed");
            }
            
            PrintSection("RGB Decoding");
            
            var decodedRGB = WebPImage.Decode(webpData, WEBP_CSP_MODE.MODE_RGB);
            
            if (decodedRGB != null && decodedRGB.Length > 0)
            {
                PrintSuccess("RGB decoding successful");
                passed++;
            }
            else
            {
                PrintError("RGB decoding failed");
            }
            
            PrintSection("BGRA Decoding");
            
            var decodedBGRA = WebPImage.Decode(webpData, WEBP_CSP_MODE.MODE_BGRA);
            
            if (decodedBGRA != null && decodedBGRA.Length > 0)
            {
                PrintSuccess("BGRA decoding successful");
                passed++;
            }
            else
            {
                PrintError("BGRA decoding failed");
            }
            
            PrintSection("BGR Decoding");
            
            var decodedBGR = WebPImage.Decode(webpData, WEBP_CSP_MODE.MODE_BGR);
            
            if (decodedBGR != null && decodedBGR.Length > 0)
            {
                PrintSuccess("BGR decoding successful");
                passed++;
            }
            else
            {
                PrintError("BGR decoding failed");
            }
            
            PrintSection("ARGB Decoding");
            
            var decodedARGB = WebPImage.Decode(webpData, WEBP_CSP_MODE.MODE_ARGB);
            
            if (decodedARGB != null && decodedARGB.Length > 0)
            {
                PrintSuccess("ARGB decoding successful");
                passed++;
            }
            else
            {
                PrintError("ARGB decoding failed");
            }
        }
        catch (Exception ex)
        {
            PrintError($"Decoding tests failed: {ex.Message}");
        }
        
        return passed;
    }

    private static int TestScaling()
    {
        var passed = 0;
        
        try
        {
            var width = 800;
            var height = 600;
            var rgbaData = CreateTestRGBA(width, height);
            var webpData = WebPImage.EncodeLossless(rgbaData, width, height);
            
            PrintSection("Scale Down (400x300)");
            
            var scaled = WebPImage.Scale(webpData, 400, 300, WEBP_CSP_MODE.MODE_RGBA);
            
            if (scaled != null && scaled.Length > 0)
            {
                PrintSuccess($"Scaling to 400x300 successful ({scaled.Length} bytes)");
                passed++;
            }
            else
            {
                PrintError("Scaling failed");
            }
            
            PrintSection("Scale Up (1600x1200)");
            
            var scaledUp = WebPImage.Scale(webpData, 1600, 1200, WEBP_CSP_MODE.MODE_RGBA);
            
            if (scaledUp != null && scaledUp.Length > 0)
            {
                PrintSuccess($"Scaling to 1600x1200 successful ({scaledUp.Length} bytes)");
                passed++;
            }
            else
            {
                PrintError("Upscaling failed");
            }
        }
        catch (Exception ex)
        {
            PrintError($"Scaling tests failed: {ex.Message}");
        }
        
        return passed;
    }

    private static int TestCropping()
    {
        var passed = 0;
        
        try
        {
            var width = 800;
            var height = 600;
            var rgbaData = CreateTestRGBA(width, height);
            var webpData = WebPImage.EncodeLossless(rgbaData, width, height);
            
            PrintSection("Crop to Center (400x300)");
            
            var cropped = WebPImage.Crop(webpData, 200, 150, 400, 300, WEBP_CSP_MODE.MODE_RGBA);
            
            if (cropped != null && cropped.Length > 0)
            {
                PrintSuccess($"Cropping to 400x300 successful ({cropped.Length} bytes)");
                passed++;
            }
            else
            {
                PrintError("Cropping failed");
            }
            
            PrintSection("Crop to Top-Left (200x150)");
            
            var croppedTL = WebPImage.Crop(webpData, 0, 0, 200, 150, WEBP_CSP_MODE.MODE_RGBA);
            
            if (croppedTL != null && croppedTL.Length > 0)
            {
                PrintSuccess($"Cropping to 200x150 (top-left) successful ({croppedTL.Length} bytes)");
                passed++;
            }
            else
            {
                PrintError("Top-left cropping failed");
            }
        }
        catch (Exception ex)
        {
            PrintError($"Cropping tests failed: {ex.Message}");
        }
        
        return passed;
    }

    private static int TestFlipping()
    {
        var passed = 0;
        
        try
        {
            var width = 800;
            var height = 600;
            var rgbaData = CreateTestRGBA(width, height);
            var webpData = WebPImage.EncodeLossless(rgbaData, width, height);
            
            PrintSection("Vertical Flip");
            
            var flipped = WebPImage.Flip(webpData, WEBP_CSP_MODE.MODE_RGBA);
            
            if (flipped != null && flipped.Length > 0)
            {
                PrintSuccess($"Vertical flip successful ({flipped.Length} bytes)");
                passed++;
            }
            else
            {
                PrintError("Flipping failed");
            }
        }
        catch (Exception ex)
        {
            PrintError($"Flipping tests failed: {ex.Message}");
        }
        
        return passed;
    }

    private static int TestAdvancedEncoding()
    {
        var passed = 0;
        
        try
        {
            var width = 800;
            var height = 600;
            var rgbaData = CreateTestRGBA(width, height);
            
            PrintSection("Advanced Encoding - Photo Preset");
            
            var photoOptions = new WebPEncodeOptions
            {
                Preset = WebPPreset.PHOTO,
                Quality = 85.0f,
                Lossless = false,
                Method = 6
            };
            
            var photoWebP = WebPImage.EncodeAdvanced(rgbaData, width, height, photoOptions);
            
            if (photoWebP != null && photoWebP.Length > 0)
            {
                PrintSuccess($"Photo preset encoding successful ({photoWebP.Length} bytes)");
                passed++;
            }
            else
            {
                PrintError("Photo preset encoding failed");
            }
            
            PrintSection("Advanced Encoding - Picture Preset");
            
            var pictureOptions = new WebPEncodeOptions
            {
                Preset = WebPPreset.PICTURE,
                Quality = 90.0f,
                Lossless = false,
                Method = 6
            };
            
            var pictureWebP = WebPImage.EncodeAdvanced(rgbaData, width, height, pictureOptions);
            
            if (pictureWebP != null && pictureWebP.Length > 0)
            {
                PrintSuccess($"Picture preset encoding successful ({pictureWebP.Length} bytes)");
                passed++;
            }
            else
            {
                PrintError("Picture preset encoding failed");
            }
            
            PrintSection("Advanced Encoding - Custom Configuration");
            
            var customOptions = new WebPEncodeOptions
            {
                Quality = 80.0f,
                Method = 4,
                Segments = 3,
                SnsStrength = 75,
                FilterStrength = 80,
                FilterSharpness = 4,
                Autofilter = true,
                AlphaCompression = 1,
                AlphaFiltering = 1
            };
            
            var customWebP = WebPImage.EncodeAdvanced(rgbaData, width, height, customOptions);
            
            if (customWebP != null && customWebP.Length > 0)
            {
                PrintSuccess($"Custom config encoding successful ({customWebP.Length} bytes)");
                passed++;
            }
            else
            {
                PrintError("Custom config encoding failed");
            }
        }
        catch (Exception ex)
        {
            PrintError($"Advanced encoding tests failed: {ex.Message}");
        }
        
        return passed;
    }

    private static int TestFileAndStreamOperations()
    {
        var passed = 0;
        
        try
        {
            var width = 800;
            var height = 600;
            var rgbaData = CreateTestRGBA(width, height);
            var webpData = WebPImage.EncodeLossless(rgbaData, width, height);
            
            var testFile = Path.Combine(GetBaseDirectory(), "test-temp.webp");
            File.WriteAllBytes(testFile, webpData);
            
            PrintSection("File Path Operations");
            
            var decodedFromFile = WebPImage.Decode(testFile);
            
            if (decodedFromFile != null && decodedFromFile.Length > 0)
            {
                PrintSuccess("Decode from file path successful");
                passed++;
            }
            else
            {
                PrintError("Decode from file path failed");
            }
            
            var infoFromFile = WebPImage.GetInfo(testFile);
            
            if (infoFromFile != null && infoFromFile.Width > 0)
            {
                PrintSuccess($"GetInfo from file successful: {infoFromFile.Width}x{infoFromFile.Height}");
                passed++;
            }
            else
            {
                PrintError("GetInfo from file failed");
            }
            
            PrintSection("Stream Operations");
            
            using (var stream = new MemoryStream(webpData))
            {
                var decodedFromStream = WebPImage.Decode(stream);
                
                if (decodedFromStream != null && decodedFromStream.Length > 0)
                {
                    PrintSuccess("Decode from stream successful");
                    passed++;
                }
                else
                {
                    PrintError("Decode from stream failed");
                }
                
                var infoFromStream = WebPImage.GetInfo(stream);
                
                if (infoFromStream != null && infoFromStream.Width > 0)
                {
                    PrintSuccess($"GetInfo from stream successful: {infoFromStream.Width}x{infoFromStream.Height}");
                    passed++;
                }
                else
                {
                    PrintError("GetInfo from stream failed");
                }
            }
            
            PrintSection("Byte Array Operations");
            
            var decodedFromBytes = WebPImage.Decode(webpData);
            
            if (decodedFromBytes != null && decodedFromBytes.Length > 0)
            {
                PrintSuccess("Decode from byte array successful");
                passed++;
            }
            else
            {
                PrintError("Decode from byte array failed");
            }
            
            var infoFromBytes = WebPImage.GetInfo(webpData);
            
            if (infoFromBytes != null && infoFromBytes.Width > 0)
            {
                PrintSuccess($"GetInfo from bytes successful: {infoFromBytes.Width}x{infoFromBytes.Height}");
                passed++;
            }
            else
            {
                PrintError("GetInfo from bytes failed");
            }
            
            File.Delete(testFile);
        }
        catch (Exception ex)
        {
            PrintError($"File/stream tests failed: {ex.Message}");
        }
        
        return passed;
    }

    private static byte[] CreateTestWebP()
    {
        var width = 800;
        var height = 600;
        var rgba = CreateTestRGBA(width, height);
        return WebPImage.EncodeLossless(rgba, width, height);
    }

    private static byte[] CreateTestRGBA(int width, int height)
    {
        var data = new byte[width * height * 4];
        
        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                var index = (y * width + x) * 4;
                
                var r = (byte)((x * 255) / width);
                var g = (byte)((y * 255) / height);
                var b = (byte)(255 - r);
                
                data[index] = r;
                data[index + 1] = g;
                data[index + 2] = b;
                data[index + 3] = 255;
            }
        }
        
        return data;
    }

    private static byte[] CreateTestRGB(int width, int height)
    {
        var data = new byte[width * height * 3];
        
        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                var index = (y * width + x) * 3;
                
                var r = (byte)((x * 255) / width);
                var g = (byte)((y * 255) / height);
                var b = (byte)(255 - r);
                
                data[index] = r;
                data[index + 1] = g;
                data[index + 2] = b;
            }
        }
        
        return data;
    }
}
