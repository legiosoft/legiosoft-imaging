# LegioSoft.Imaging

Modular .NET image processing library.

## Installation

### All Features

```bash
dotnet add package LegioSoft.Imaging
```

### WebP Only (~500KB)

```bash
dotnet add package LegioSoft.Imaging.Core
dotnet add package LegioSoft.Imaging.WebP
```

### Full Processing (~565KB)

```bash
dotnet add package LegioSoft.Imaging.Skia
```

**Supported Frameworks**: .NET 6.0, 7.0, 8.0, 9.0, 10.0

## Skia Package

### Basic Operations

```csharp
using LegioSoft.Imaging.Skia;

// Load, process, save
LegioImageBuilder.Load("photo.jpg")
    .Resize(800, 600)
    .Save("output.jpg");

// Multiple operations
LegioImageBuilder.Load("image.png")
    .Resize(1920, 1080, LegioScaleMode.Fit)
    .Crop(100, 100, 400, 400)
    .Rotate(90)
    .Grayscale()
    .Save("processed.jpg", 90);
```

### Load

```csharp
// From file, bytes, or stream
LegioImageBuilder.Load("image.jpg")
LegioImageBuilder.Load(File.ReadAllBytes("image.jpg"))
LegioImageBuilder.Load(stream)

// Get metadata
var info = LegioImageBuilder.Load("image.jpg").GetInfo();
// info.Width, info.Height, info.Format, info.HasAlpha, info.ByteSize
```

### Resize

```csharp
.Resize(800, 600)                              // Exact
.Resize(800, 600, LegioScaleMode.Fit)          // Fit within
.Resize(800, 600, LegioScaleMode.Fill)         // Fill and crop
.Resize(800, 600, LegioScaleMode.Stretch)      // Stretch
.ResizeToWidth(1200)                           // Maintain aspect ratio
.ResizeToHeight(800)                            // Maintain aspect ratio
.Scale(0.5)                                    // Half size
```

### Crop, Rotate, Flip

```csharp
.Crop(100, 100, 400, 400)
.Rotate(90)    // 0, 90, 180, or 270
.Flip(horizontal: true, vertical: false)
```

### Filters & Color

```csharp
.Grayscale()
.Sepia()
.Blur(5)          // Radius 1-20
.Sharpen(50)      // Amount 0-100
.Brightness(30)    // -255 to 255
.Contrast(20)      // -100 to 100
.Invert()
```

### Save

```csharp
.Save("output.jpg")                              // Auto format
.Save("output.png", LegioImageFormat.Png)        // Specific format
.Save("output.jpg", quality: 90)                 // With quality

byte[] data = SaveAs(LegioImageFormat.Jpeg, 90); // As bytes
using var stream = SaveAsStream(LegioImageFormat.Png); // As stream
```

### Quality

```csharp
.Quality(85)  // 0-100
```

- JPEG: 70-85 recommended
- WebP: 80-90 recommended
- PNG/BMP/GIF: always lossless

## WebP Package

### Encode

```csharp
using LegioSoft.Imaging.WebP;

// RGBA to WebP
byte[] rgbaData = new byte[width * height * 4];
byte[] webpData = WebPImage.Encode(rgbaData, width, height, 85);
File.WriteAllBytes("output.webp", webpData);

// RGB to WebP
byte[] rgbData = new byte[width * height * 3];
byte[] webpData = WebPImage.EncodeRGB(rgbData, width, height, 85);

// Lossless
byte[] webpData = WebPImage.EncodeLossless(rgbaData, width, height);

// Advanced options
var options = new WebPEncodeOptions
{
    Preset = WebPPreset.PHOTO,
    Quality = 85.0f,
    Method = 6,
    SnsStrength = 75,
    FilterStrength = 80
};
byte[] webpData = WebPImage.EncodeAdvanced(rgbaData, width, height, options);
```

### Decode

```csharp
// Decode to RGBA
byte[] webpData = File.ReadAllBytes("input.webp");
byte[] rgbaData = WebPImage.Decode(webpData);

// With colorspace
byte[] rgbData = WebPImage.Decode(webpData, WEBP_CSP_MODE.MODE_RGB);
byte[] bgraData = WebPImage.Decode(webpData, WEBP_CSP_MODE.MODE_BGRA);

// From file/stream
byte[] data = WebPImage.Decode("input.webp");
byte[] data = WebPImage.Decode(stream);
```

### Transform

```csharp
byte[] scaled = WebPImage.Scale(webpData, 400, 300);
byte[] cropped = WebPImage.Crop(webpData, 100, 100, 400, 300);
byte[] flipped = WebPImage.Flip(webpData);
```

### Info

```csharp
WebPInfo info = WebPImage.GetInfo(webpData);
// info.Width, info.Height, info.HasAlpha, info.HasAnimation

bool isValid = WebPImage.IsValidWebP(data);
string version = WebPImage.GetVersion(); // "1.6.0"
```

## Core Package

```csharp
using LegioSoft.Imaging.Core;

// Detect format
var imageData = File.ReadAllBytes("image.jpg");
var format = FormatDetector.DetectFormat(imageData);
```

## Examples

### Thumbnail

```csharp
LegioImageBuilder.Load("photo.jpg")
    .ResizeToWidth(200)
    .Quality(80)
    .Save("thumb.jpg");
```

### Profile Picture

```csharp
LegioImageBuilder.Load("photo.jpg")
    .Resize(500, 500, LegioScaleMode.Fill)
    .Brightness(10)
    .Contrast(15)
    .Save("profile.jpg", 90);
```

### Web Optimization

```csharp
var data = LegioImageBuilder.Load("large.jpg")
    .ResizeToWidth(1920)
    .SaveAs(LegioImageFormat.WebP, 85);
File.WriteAllBytes("optimized.webp", data);
```

### Batch Process

```csharp
foreach (var file in Directory.GetFiles("input", "*.jpg"))
{
    var name = Path.GetFileNameWithoutExtension(file);
    LegioImageBuilder.Load(file)
        .Resize(1920, 1080, LegioScaleMode.Fit)
        .Quality(85)
        .Save($"output/{name}_resized.jpg");
}
```

## Packages

| Package | Size | Purpose |
|---------|------|---------|
| LegioSoft.Imaging | All-in-one | Core + WebP + Skia |
| LegioSoft.Imaging.Skia | ~565KB | Full image processing |
| LegioSoft.Imaging.WebP | ~500KB | WebP encode/decode |
| LegioSoft.Imaging.Core | ~20KB | Interfaces and types |

## Formats

**Input**: PNG, JPEG, WebP, BMP, GIF

**Output**:
- PNG - Lossless, transparency
- JPEG - Lossy, photos
- WebP - Modern, compressed
- BMP - Uncompressed
- GIF - Limited colors

## Platforms

- Windows x64
- Linux x64, ARM64
- macOS x64, ARM64

## License

Apache License 2.0

## Repository

https://github.com/legiosoft/legiosoft-imaging
