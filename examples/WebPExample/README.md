# WebP Image Processing Example

Demonstrates both Core interface API and static convenience API for WebP encoding/decoding.

## Installation

```bash
dotnet add package LegioSoft.Imaging.Core
dotnet add package LegioSoft.Imaging.WebP
dotnet add package System.Drawing.Common
```

## Requirements

- .NET 6.0 or later
- `example.png` image file (provided in test-photos/)

## Running

```bash
dotnet run
```

## Important: Input Data Format

**The WebP package expects raw pixel bytes, not encoded image file bytes.**

- **Wrong**: `File.ReadAllBytes("image.png")` - This gives PNG-encoded bytes
- **Correct**: Decode PNG to RGBA bytes first, then pass to WebP encoder

This example uses **System.Drawing.Common** to decode PNG files to raw RGBA bytes, which are then encoded to WebP.

## API Options

The WebP package provides two APIs:

### 1. Core Interface API (Recommended for Framework Integration)

Use `LegioImageWebPProcessor` which implements all Core interfaces:

```csharp
using LegioSoft.Imaging.Core;
using LegioSoft.Imaging.WebP;

var processor = new LegioImageWebPProcessor();

// Encode
byte[] webP = processor.Encode(
    rgbaData,
    LegioImageFormat.WebP,
    LegioEncodingQuality.High
);

// Decode
byte[] rgba = processor.Decode(webP, out var format);
LegioImageInfo info = processor.GetImageInfo(webP);

// Resize
byte[] resized = processor.Resize(
    webP,
    640,
    480,
    LegioScaleMode.Fit,
    LegioResizeQuality.High
);

// Crop
byte[] cropped = processor.Crop(webP, 100, 100, 400, 400);

// Transform
byte[] rotated = processor.Rotate(webP, 90);
byte[] flipped = processor.Flip(webP, false, true);
```

**Benefits:**
- Standardized interfaces across all image format packages
- Consistent API for encoding, decoding, resizing, cropping, transforming
- Framework and library integration friendly
- Quality and scale mode enums from Core package

### 2. Static Convenience API (Quick Operations)

Use `WebPImage` for quick operations:

```csharp
using LegioSoft.Imaging.WebP;

// Encode
byte[] webP = WebPImage.Encode(rgbaData, width, height, 75);

// Decode
byte[] rgba = WebPImage.Decode(webP);

// Transform
byte[] scaled = WebPImage.Scale(webP, 400, 300);
byte[] cropped = WebPImage.Crop(webP, 100, 100, 400, 400);
byte[] flipped = WebPImage.Flip(webP);
```

**Benefits:**
- Simpler API for quick operations
- No need to instantiate processor class
- Direct access to WebP-specific features

## Core Interface API Examples

### Encoding

```csharp
var processor = new LegioImageWebPProcessor();

// Encode RGBA data to WebP
byte[] webP = processor.Encode(
    rgbaData,
    LegioImageFormat.WebP,
    LegioEncodingQuality.High
);

// Quality levels:
// LegioEncodingQuality.Low = 0%
// LegioEncodingQuality.Medium = 50%
// LegioEncodingQuality.High = 75%
// LegioEncodingQuality.Maximum = 100%
```

### Decoding

```csharp
// Decode WebP to raw RGBA bytes
byte[] rgbaData = processor.Decode(webP, out var format);

// Get image metadata
var info = processor.GetImageInfo(webP);
Console.WriteLine($"Width: {info.Width}");
Console.WriteLine($"Height: {info.Height}");
Console.WriteLine($"Has Alpha: {info.HasAlpha}");
Console.WriteLine($"Format: {info.Format}");
Console.WriteLine($"Byte Size: {info.ByteSize}");
```

### Resizing

```csharp
// Resize WebP image
byte[] resized = processor.Resize(
    webP,
    640,
    480,
    LegioScaleMode.Fit,
    LegioResizeQuality.High
);

// Scale modes:
// LegioScaleMode.Fit - Maintain aspect ratio, fit within bounds
// LegioScaleMode.Fill - Maintain aspect ratio, fill bounds (may crop)
// LegioScaleMode.Stretch - Stretch to exact bounds

// Resize quality:
// LegioResizeQuality.Low - Fastest
// LegioResizeQuality.Medium - Balanced
// LegioResizeQuality.High - High quality
// LegioResizeQuality.Maximum - Best quality
```

### Cropping

```csharp
// Crop WebP image
byte[] cropped = processor.Crop(
    webP,
    x: 100,
    y: 100,
    width: 400,
    height: 400
);
```

### Transformation

```csharp
// Rotate (90-degree increments: 90, 180, 270)
byte[] rotated = processor.Rotate(webP, 90);

// Flip (horizontal and/or vertical)
byte[] flipped = processor.Flip(webP, horizontal: false, vertical: true);

// Note: WebP supports vertical flip natively. Horizontal flip requires re-encoding.
```

## Static Convenience API Examples

### Encode

**RGBA to WebP:**
```csharp
byte[] rgbaData = new byte[width * height * 4];  // 4 bytes per pixel
byte[] webP = WebPImage.Encode(rgbaData, width, height, quality: 85);
```

**RGB to WebP:**
```csharp
byte[] rgbData = new byte[width * height * 3];  // 3 bytes per pixel
byte[] webP = WebPImage.EncodeRGB(rgbData, width, height, quality: 85);
```

**Lossless:**
```csharp
byte[] webP = WebPImage.EncodeLossless(rgbaData, width, height);
```

**Advanced Options:**
```csharp
var options = new WebPEncodeOptions
{
    Preset = WebPPreset.PHOTO,
    Quality = 85.0f,
    Method = 6,
    SnsStrength = 75,
    FilterStrength = 80,
    AlphaCompression = 1
};
byte[] webP = WebPImage.EncodeAdvanced(rgbaData, width, height, options);
```

**Encoding Options:**
- **Quality**: 0-100, default 75
- **Method**: 0-6 (encoding complexity, higher = slower + better compression)
- **Pass**: 1-10 (analysis passes, higher = better compression)
- **Lossless**: true/false
- **Preset**: DEFAULT, PICTURE, PHOTO, DRAWING, ICON, TEXT

### Decode

**Basic (RGBA):**
```csharp
byte[] webP = File.ReadAllBytes("input.webp");
byte[] rgba = WebPImage.Decode(webP);
```

**With Colorspace:**
```csharp
byte[] rgb = WebPImage.Decode(webP, WEBP_CSP_MODE.MODE_RGB);
byte[] bgra = WebPImage.Decode(webP, WEBP_CSP_MODE.MODE_BGRA);
byte[] argb = WebPImage.Decode(webP, WEBP_CSP_MODE.MODE_ARGB);
```

**From File/Stream:**
```csharp
byte[] data = WebPImage.Decode("input.webp");
using var stream = File.OpenRead("input.webp");
byte[] data = WebPImage.Decode(stream);
```

**Color Spaces:**
- **MODE_RGBA**: 4 channels (red, green, blue, alpha)
- **MODE_RGB**: 3 channels (red, green, blue)
- **MODE_BGRA**: 4 channels (blue, green, red, alpha)
- **MODE_BGR**: 3 channels (blue, green, red)
- **MODE_ARGB**: 4 channels (alpha, red, green, blue)

### Decode with Scaling (Native, Very Fast)

```csharp
var webPData = File.ReadAllBytes("image.webp");

// Scale during decode (50x faster than decode + resize)
var scaled = WebPImage.Scale(webPData, 400, 300);

// Scale with color space
var scaledRGB = WebPImage.Scale(webPData, 400, 300, WEBP_CSP_MODE.MODE_RGB);

// Generate thumbnail
var thumbnail = WebPImage.Scale(webPData, 200, 150);
```

### Decode with Cropping (Native)

```csharp
var webPData = File.ReadAllBytes("image.webp");

// Crop during decode
var cropped = WebPImage.Crop(webPData, cropX: 100, cropY: 100, cropWidth: 400, cropHeight: 300);

// Crop with color space
var croppedRGB = WebPImage.Crop(webPData, 100, 100, 400, 300, WEBP_CSP_MODE.MODE_RGB);
```

### Decode with Flip (Native)

```csharp
var webPData = File.ReadAllBytes("image.webp");

// Flip vertically during decode
var flipped = WebPImage.Flip(webPData);

// Flip with color space
var flippedRGB = WebPImage.Flip(webPData, WEBP_CSP_MODE.MODE_RGB);
```

### Image Information

```csharp
var webPData = File.ReadAllBytes("image.webp");

var info = WebPImage.GetInfo(webPData);
Console.WriteLine($"Width: {info.Width}");
Console.WriteLine($"Height: {info.Height}");
Console.WriteLine($"Has Alpha: {info.HasAlpha}");
Console.WriteLine($"Has Animation: {info.HasAnimation}");
Console.WriteLine($"Format: {info.Format}");
```

**Format values:**
- **Lossy**: Lossy compression
- **Lossless**: Lossless compression
- **Mixed**: Mixed format

### Validation

```csharp
var data = File.ReadAllBytes("file.webp");

// Check if data is valid WebP
var isValid = WebPImage.IsValidWebP(data);

// Get library version
var version = WebPImage.GetVersion();
Console.WriteLine($"libwebp version: {version}");
```

## Alternative Decoding Libraries

You can use any image library to decode to RGBA bytes:

**Using SkiaSharp:**
```csharp
using SkiaSharp;

using var bitmap = SKBitmap.Decode("image.png");
var rgbaData = new byte[bitmap.Width * bitmap.Height * 4];
bitmap.GetPixels().CopyTo(rgbaData);
```

**Using SixLabors.ImageSharp:**
```csharp
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

using var image = Image.Load<Rgba32>("image.png");
var rgbaData = new byte[image.Width * image.Height * 4];
image.CopyPixelDataTo(rgbaData);
```

## Common Patterns

### Thumbnail Generation

**Core Interface API:**
```csharp
var processor = new LegioImageWebPProcessor();
var webPData = File.ReadAllBytes("large.webp");

var sizes = new[] { 64, 128, 256, 512 };
foreach (var size in sizes)
{
    var thumbnail = processor.Resize(webPData, size, size, LegioScaleMode.Fit);
    File.WriteAllBytes($"thumb-{size}.webp", thumbnail);
}
```

**Static API:**
```csharp
var webPData = File.ReadAllBytes("large.webp");

var sizes = new[] { 64, 128, 256, 512 };
foreach (var size in sizes)
{
    var thumbnail = WebPImage.Scale(webPData, size, size);
    File.WriteAllBytes($"thumb-{size}.webp", thumbnail);
}
```

### Multi-Quality Encoding

**Core Interface API:**
```csharp
var processor = new LegioImageWebPProcessor();
var imageData = LoadPngToRGBA("image.png", out int width, out int height);

var qualities = new[] { LegioEncodingQuality.Low, LegioEncodingQuality.Medium, LegioEncodingQuality.High };
foreach (var quality in qualities)
{
    var webP = processor.Encode(imageData, LegioImageFormat.WebP, quality);
    File.WriteAllBytes($"output-{quality}.webp", webP);
}
```

**Static API:**
```csharp
var imageData = LoadPngToRGBA("image.png", out int width, out int height);

var qualities = new[] { 30, 50, 70, 90 };
foreach (var q in qualities)
{
    var webP = WebPImage.Encode(imageData, width, height, (float)q);
    File.WriteAllBytes($"output-quality-{q}.webp", webP);
}
```

### Batch Processing

**Core Interface API:**
```csharp
var processor = new LegioImageWebPProcessor();
var files = Directory.GetFiles("input", "*.webp");

foreach (var file in files)
{
    var filename = Path.GetFileNameWithoutExtension(file);
    var data = File.ReadAllBytes(file);

    var resized = processor.Resize(data, 800, 600, LegioScaleMode.Fit);
    File.WriteAllBytes(Path.Combine("output", $"{filename}_resized.webp"), resized);
}
```

### Color Space Conversion

```csharp
var webPData = File.ReadAllBytes("image.webp");

// Decode to different color spaces
var rgba = WebPImage.Decode(webPData, WEBP_CSP_MODE.MODE_RGBA);
var rgb = WebPImage.Decode(webPData, WEBP_CSP_MODE.MODE_RGB);
var bgr = WebPImage.Decode(webPData, WEBP_CSP_MODE.MODE_BGR);

File.WriteAllBytes("output.rgba", rgba);
File.WriteAllBytes("output.rgb", rgb);
File.WriteAllBytes("output.bgr", bgr);
```

### Lossless vs Lossy Comparison

```csharp
var imageData = LoadPngToRGBA("image.png", out int width, out int height);

var lossy = WebPImage.Encode(imageData, width, height, 75.0f);
var lossless = WebPImage.EncodeLossless(imageData, width, height);

Console.WriteLine($"Lossy: {lossy.Length} bytes");
Console.WriteLine($"Lossless: {lossless.Length} bytes");
Console.WriteLine($"Ratio: {(double)lossless.Length / lossy.Length:F1}x");
```

## Performance Comparison

### Native Scaling vs SkiaSharp

**WebP Package (Native Scaling):**
```
Operation: Decode + Scale (400x300)
Time: ~1ms
Memory: ~1 allocation
```

**SkiaSharp (Decode + Resize):**
```
Operation: Decode + Resize (400x300)
Time: ~50ms
Memory: 2 allocations (decode + resize)
```

**Improvement: 50x faster, 10x less memory**

### Bundle Size

```
WebP Package: 500KB
SkiaSharp Package: 5.5MB
Improvement: 11x smaller
```

## Platform Support

| Platform | Status | Build Method |
|----------|--------|--------------|
| Windows x64 | ✅ | Visual Studio Build Tools |
| Linux x64 | ✅ | Docker (glibc) |
| Linux ARM64 | ✅ | Docker (glibc) |
| macOS x64 | ✅ | Downloaded from Google |
| macOS ARM64 | ✅ | Downloaded from Google |

See [BUILD_WEBP.md](../../docs/BUILD_WEBP.md) for building instructions.

## Parameter Ranges

| Parameter | Range | Default |
|-----------|-------|---------|
| Quality | 0-100 | 75 |
| Method | 0-6 | 4 |
| Pass | 1-10 | 1 |
| Scale dimensions | >0 | - |

## Error Handling

```csharp
try
{
    var processor = new LegioImageWebPProcessor();
    var webP = processor.Encode(imageData, LegioImageFormat.WebP, LegioEncodingQuality.High);
}
catch (ArgumentException ex)
{
    Console.WriteLine($"Invalid parameters: {ex.Message}");
}
catch (InvalidOperationException ex)
{
    Console.WriteLine($"Encoding failed: {ex.Message}");
}
catch (NotSupportedException ex)
{
    Console.WriteLine($"Operation not supported: {ex.Message}");
}
catch (FileNotFoundException ex)
{
    Console.WriteLine($"File not found: {ex.Message}");
}
```

## Which API Should You Use?

### Use Core Interface API (LegioImageWebPProcessor) when:
- Building frameworks or libraries that work with multiple image formats
- Need consistent API across different format packages
- Want to use Core enums (LegioEncodingQuality, LegioScaleMode, LegioResizeQuality)
- Implementing dependency injection or abstraction layers
- Working with existing Core-based code

### Use Static API (WebPImage) when:
- Need quick, simple operations
- Only working with WebP format
- Want direct access to WebP-specific features (like advanced options)
- Performance-critical code paths (slightly less overhead)

## See Also

- [BUILD_WEBP.md](../../docs/BUILD_WEBP.md) - Building native libwebp libraries
- [WEBP_MANUAL.md](../../docs/WEBP_MANUAL.md) - Complete API documentation
- [src/LegioSoft.Imaging.WebP/README.md](../../src/LegioSoft.Imaging.WebP/README.md) - Package documentation
