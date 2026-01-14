# LegioSoft.Imaging

Meta package that aggregates all LegioSoft.Imaging libraries. Install this package to get full image processing capabilities.

## Installation

```bash
dotnet add package LegioSoft.Imaging
```

## What's Included

- **LegioSoft.Imaging.Core** (~20KB) - Interfaces, enums, and base types
- **LegioSoft.Imaging.WebP** (~500KB) - WebP encoding/decoding with libwebp 1.6.0
- **LegioSoft.Imaging.Skia** (~565KB) - Full-featured image processing with SkiaSharp 2.88.6

## Supported Frameworks

.NET 6.0, 7.0, 8.0, 9.0, 10.0

## Quick Start

### Image Processing with Skia

```csharp
using LegioSoft.Imaging.Skia;

LegioImageBuilder.Load("photo.jpg")
    .Resize(800, 600)
    .Crop(100, 100, 400, 400)
    .Rotate(90)
    .Grayscale()
    .Save("processed.jpg", 90);
```

### WebP Encode/Decode

```csharp
using LegioSoft.Imaging.WebP;

byte[] rgbaData = new byte[width * height * 4];
byte[] webpData = WebPImage.Encode(rgbaData, width, height, 85);
byte[] decoded = WebPImage.Decode(webpData);
```

### Format Detection

```csharp
using LegioSoft.Imaging.Core;

var imageData = File.ReadAllBytes("image.jpg");
var format = FormatDetector.DetectFormat(imageData);
```

## Skia Operations

```csharp
// Resize
.Resize(800, 600)                              // Exact
.Resize(800, 600, LegioScaleMode.Fit)          // Fit within
.Resize(800, 600, LegioScaleMode.Fill)         // Fill and crop
.Resize(800, 600, LegioScaleMode.Stretch)      // Stretch
.ResizeToWidth(1200)                           // Maintain aspect ratio
.ResizeToHeight(800)                            // Maintain aspect ratio
.Scale(0.5)                                    // Half size

// Transform
.Crop(100, 100, 400, 400)
.Rotate(90)                                    // 0, 90, 180, or 270
.Flip(horizontal: true, vertical: false)

// Filters
.Grayscale()
.Sepia()
.Blur(5)                                      // Radius 1-20
.Sharpen(50)                                  // Amount 0-100

// Color adjustments
.Brightness(30)                                // -255 to 255
.Contrast(20)                                  // -100 to 100
.Invert()

// Save
.Save("output.jpg")
.Save("output.png", LegioImageFormat.Png)
.Save("output.jpg", quality: 90)
byte[] data = SaveAs(LegioImageFormat.Jpeg, 90);
using var stream = SaveAsStream(LegioImageFormat.Png);
```

## WebP Operations

```csharp
// Encode
WebPImage.Encode(rgbaData, width, height, 85)
WebPImage.EncodeRGB(rgbData, width, height, 85)
WebPImage.EncodeLossless(rgbaData, width, height)
WebPImage.EncodeAdvanced(rgbaData, width, height, options)

// Decode
WebPImage.Decode(webpData)
WebPImage.Decode(webpData, WEBP_CSP_MODE.MODE_RGB)
WebPImage.Decode("input.webp")
WebPImage.Decode(stream)

// Transform
WebPImage.Scale(webpData, 400, 300)
WebPImage.Crop(webpData, 100, 100, 400, 300)
WebPImage.Flip(webpData)

// Info
WebPInfo info = WebPImage.GetInfo(webpData)
bool isValid = WebPImage.IsValidWebP(data)
string version = WebPImage.GetVersion() // "1.6.0"
```

## Supported Formats

**Input**: PNG, JPEG, WebP, BMP, GIF

**Output**: PNG, JPEG, WebP, BMP, GIF

## Platforms

- Windows x64
- Linux x64, ARM64
- macOS x64, ARM64

## Individual Packages

If you don't need all packages, install individually:

**WebP Only:**
```bash
dotnet add package LegioSoft.Imaging.Core
dotnet add package LegioSoft.Imaging.WebP
```

**Full Processing:**
```bash
dotnet add package LegioSoft.Imaging.Skia
```

**Core Only:**
```bash
dotnet add package LegioSoft.Imaging.Core
```

## License

Apache License 2.0

## Repository

https://github.com/legiosoft/legiosoft-imaging
