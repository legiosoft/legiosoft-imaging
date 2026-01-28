# LegioSoft.Imaging.WebP

Lightweight WebP encoding and decoding using native libwebp 1.6.0.

## Installation

```bash
dotnet add package LegioSoft.Imaging.Core
dotnet add package LegioSoft.Imaging.WebP
```

## API Overview

The WebP package provides two ways to interact:

1. **Core Interface API** (`LegioImageWebPProcessor`) - Implements Core interfaces for standardized operations
2. **Static Convenience API** (`WebPImage`) - Quick operations for common use cases

## Core Interface API

Use this API for framework integration and standardized operations across image format packages.

### LegioImageWebPProcessor

Implements all Core interfaces:
- `ILegioImageEncoder` - Encode to WebP
- `ILegioImageDecoder` - Decode WebP
- `ILegioImageResizer` - Resize WebP
- `ILegioImageCropper` - Crop WebP
- `ILegioImageTransformer` - Transform WebP

### Encode (ILegioImageEncoder)

**Note:** This API expects raw pixel data (RGBA/RGB), not encoded image data. Use a decoder (like System.Drawing.Common or SkiaSharp) to convert encoded images to raw pixels first.

```csharp
using LegioSoft.Imaging.Core;
using LegioSoft.Imaging.WebP;

var processor = new LegioImageWebPProcessor();

// Encode RGBA data to WebP
byte[] rgbaData = new byte[width * height * 4];  // 4 bytes per pixel
byte[] webpData = processor.Encode(
    rgbaData,
    LegioImageFormat.WebP,
    LegioEncodingQuality.High
);

// Encoding quality levels:
// LegioEncodingQuality.Low = 0%
// LegioEncodingQuality.Medium = 50%
// LegioEncodingQuality.High = 75%
// LegioEncodingQuality.Maximum = 100%

// Encode from stream
using var inputStream = new MemoryStream(rgbaData);
using var outputStream = processor.Encode(
    inputStream,
    LegioImageFormat.WebP,
    LegioEncodingQuality.High
);
File.WriteAllBytes("output.webp", ((MemoryStream)outputStream).ToArray());
```

### Decode (ILegioImageDecoder)

```csharp
// Decode WebP to raw RGBA bytes
byte[] webpData = File.ReadAllBytes("input.webp");
byte[] rgbaData = processor.Decode(webpData, out var format);

// Get image metadata
var info = processor.GetImageInfo(webpData);
Console.WriteLine($"Width: {info.Width}");
Console.WriteLine($"Height: {info.Height}");
Console.WriteLine($"Has Alpha: {info.HasAlpha}");
Console.WriteLine($"Format: {info.Format}");  // WebP
Console.WriteLine($"Byte Size: {info.ByteSize}");

// Get info from stream
using var stream = new MemoryStream(webpData);
var streamInfo = processor.GetImageInfo(stream);
```

### Resize (ILegioImageResizer)

WebP uses native scaling during decode for maximum performance.

```csharp
// Resize WebP image
byte[] resized = processor.Resize(
    webpData,
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
// LegioResizeQuality.High - High quality (default)
// LegioResizeQuality.Maximum - Best quality

// Resize from stream
using var inputStream = new MemoryStream(webpData);
using var outputStream = processor.Resize(
    inputStream,
    640,
    480,
    LegioScaleMode.Fit
);
```

### Crop (ILegioImageCropper)

WebP uses native cropping during decode for maximum performance.

```csharp
// Crop WebP image
byte[] cropped = processor.Crop(
    webpData,
    x: 100,
    y: 100,
    width: 400,
    height: 300
);

// Crop from stream
using var inputStream = new MemoryStream(webpData);
using var outputStream = processor.Crop(
    inputStream,
    100,
    100,
    400,
    300
);
```

### Transform (ILegioImageTransformer)

```csharp
// Rotate (90-degree increments: 90, 180, 270)
byte[] rotated = processor.Rotate(webpData, 90);

// Flip (horizontal and/or vertical)
byte[] flipped = processor.Flip(webpData, horizontal: false, vertical: true);

// Note: WebP supports vertical flip natively. Horizontal flip requires re-encoding.

// Transform from stream
using var inputStream = new MemoryStream(webpData);
using var outputStream = processor.Rotate(inputStream, 90);
```

## Static Convenience API

Use this API for quick operations without Core interface overhead.

### Encode

**RGBA to WebP:**
```csharp
byte[] rgbaData = new byte[width * height * 4];  // 4 bytes per pixel
byte[] webpData = WebPImage.Encode(rgbaData, width, height, quality: 85);
File.WriteAllBytes("output.webp", webpData);
```

**RGB to WebP:**
```csharp
byte[] rgbData = new byte[width * height * 3];  // 3 bytes per pixel
byte[] webpData = WebPImage.EncodeRGB(rgbData, width, height, quality: 85);
```

**Lossless:**
```csharp
byte[] webpData = WebPImage.EncodeLossless(rgbaData, width, height);
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
byte[] webpData = WebPImage.EncodeAdvanced(rgbaData, width, height, options);
```

### Decode

**Basic (RGBA):**
```csharp
byte[] webpData = File.ReadAllBytes("input.webp");
byte[] rgbaData = WebPImage.Decode(webpData);
```

**With Colorspace:**
```csharp
byte[] rgbData = WebPImage.Decode(webpData, WEBP_CSP_MODE.MODE_RGB);
byte[] bgraData = WebPImage.Decode(webpData, WEBP_CSP_MODE.MODE_BGRA);
byte[] argbData = WebPImage.Decode(webpData, WEBP_CSP_MODE.MODE_ARGB);
```

**From File/Stream:**
```csharp
byte[] data = WebPImage.Decode("input.webp");
using var stream = File.OpenRead("input.webp");
byte[] data = WebPImage.Decode(stream);
```

### Transform

**Scale:**
```csharp
byte[] scaled = WebPImage.Scale(webpData, targetWidth: 400, targetHeight: 300);
```

**Crop:**
```csharp
byte[] cropped = WebPImage.Crop(webpData, cropX: 100, cropY: 100, cropWidth: 400, cropHeight: 300);
```

**Flip:**
```csharp
byte[] flipped = WebPImage.Flip(webpData);
```

### Info

```csharp
WebPInfo info = WebPImage.GetInfo(webpData);
// info.Width, info.Height, info.HasAlpha, info.HasAnimation
```

### Validate

```csharp
bool isValid = WebPImage.IsValidWebP(data);
string version = WebPImage.GetVersion();  // "1.6.0"
```

## Advanced Options

| Property | Type | Range | Description |
|----------|------|-------|-------------|
| Quality | float | 0-100 | Compression quality |
| Lossless | bool | - | Lossless mode |
| Method | int | 0-6 | Compression (0=fast, 6=best) |
| SnsStrength | int | 0-100 | Spatial noise shaping |
| FilterStrength | int | 0-100 | Filter strength |
| FilterSharpness | int | 0-7 | Filter sharpness |
| AlphaCompression | int | 0-2 | Alpha compression |
| AlphaQuality | int | 0-100 | Alpha quality |
| Pass | int | 1-10 | Analysis passes |
| Autofilter | bool | - | Auto filter adjustment |
| UseSharpYUV | bool | - | Sharp YUV conversion |

## Presets

- **DEFAULT** - Default compression
- **PICTURE** - Digital pictures, portraits
- **PHOTO** - Outdoor photos, natural lighting
- **ICON** - Small colorful images
- **TEXT** - Text-like images

## Colorspaces

- **MODE_RGBA** - 4 bytes (R, G, B, A)
- **MODE_RGB** - 3 bytes (R, G, B)
- **MODE_BGRA** - 4 bytes (B, G, R, A)
- **MODE_BGR** - 3 bytes (B, G, R)
- **MODE_ARGB** - 4 bytes (A, R, G, B)

## Platforms

- Windows x64
- Linux x64
- Linux ARM64
- macOS x64
- macOS ARM64

## Important Notes

### Input Data Format

The WebP encoder expects **raw pixel bytes**, not encoded image file bytes.

**Wrong:**
```csharp
byte[] data = File.ReadAllBytes("image.png");  // This is PNG-encoded data
byte[] webp = processor.Encode(data, LegioImageFormat.WebP);  // Won't work correctly
```

**Correct:**
```csharp
// Decode PNG to raw RGBA bytes first
byte[] rgbaData = DecodePngToRGBA("image.png", out int width, out int height);
byte[] webp = processor.Encode(rgbaData, LegioImageFormat.WebP);  // Correct
```

### Decoding Other Formats

To use the WebP encoder with other image formats, decode them to raw pixels first:

**Using System.Drawing.Common:**
```csharp
using System.Drawing;
using System.Drawing.Imaging;

byte[] DecodeToRGBA(string filePath, out int width, out int height)
{
    using var bitmap = new Bitmap(filePath);
    width = bitmap.Width;
    height = bitmap.Height;

    var rect = new Rectangle(0, 0, width, height);
    var data = bitmap.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

    byte[] rgbaData = new byte[width * height * 4];
    System.Runtime.InteropServices.Marshal.Copy(data.Scan0, rgbaData, 0, rgbaData.Length);

    bitmap.UnlockBits(data);
    return rgbaData;
}
```

**Using SkiaSharp:**
```csharp
using SkiaSharp;

byte[] DecodeToRGBA(string filePath, out int width, out int height)
{
    using var bitmap = SKBitmap.Decode(filePath);
    width = bitmap.Width;
    height = bitmap.Height;

    byte[] rgbaData = new byte[width * height * 4];
    bitmap.GetPixels().CopyTo(rgbaData);
    return rgbaData;
}
```

## Performance

WebP native operations are extremely fast:

- **Decode + Scale**: ~1ms (native operation)
- **Decode + Resize** (SkiaSharp): ~50ms
- **Compression**: 50-100x smaller than PNG

## Common Patterns

### Thumbnail Generation

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

### Multi-Quality Encoding

```csharp
byte[] rgbaData = DecodeToRGBA("image.png", out int width, out int height);
var processor = new LegioImageWebPProcessor();

var qualities = new[] { LegioEncodingQuality.Low, LegioEncodingQuality.Medium, LegioEncodingQuality.High };
foreach (var quality in qualities)
{
    var webP = processor.Encode(rgbaData, LegioImageFormat.WebP, quality);
    File.WriteAllBytes($"output-{quality}.webp", webP);
}
```

### Batch Processing

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
