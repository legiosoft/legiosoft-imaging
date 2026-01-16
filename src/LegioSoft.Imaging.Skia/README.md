# LegioSoft.Imaging.Skia

Full-featured image processing library built on SkiaSharp. Provides fluent builder API for easy image manipulation with resize, crop, rotate, flip, and filter operations.

## Installation

```bash
dotnet add package LegioSoft.Imaging.Skia
```

## Features

- **Fluent Builder API** - Chain multiple image operations
- **Resize** - Multiple scale modes (Fit, Fill, Stretch) and quality levels
- **Crop** - Extract regions from images
- **Transform** - Rotate (90/180/270 degrees) and flip (horizontal/vertical)
- **Filters** - Grayscale, sepia, blur (configurable radius), sharpen (configurable amount)
- **Color Adjustments** - Brightness (-255 to 255), contrast (-100 to 100), invert colors
- **Format Support** - Input: PNG, JPEG, WebP, BMP, GIF | Output: PNG, JPEG, WebP
- **Quality Control** - 0-100 quality levels for lossy formats
- **Memory Efficient** - Automatic disposal of intermediate bitmaps
- **Secure** - Input validation and bounds checking

## Quick Start

```csharp
using LegioSoft.Imaging.Skia;

// Simple resize
LegioImageBuilder.Load("photo.jpg")
    .Resize(800, 600)
    .Save("resized.jpg");

// Multiple operations
LegioImageBuilder.Load("image.png")
    .Resize(1920, 1080, LegioScaleMode.Fit)
    .Crop(100, 100, 400, 400)
    .Rotate(90)
    .Grayscale()
    .Save("processed.jpg", LegioImageFormat.Jpeg, 90);
```

## Load Images

```csharp
// From file
LegioImageBuilder.Load("image.jpg")

// From bytes
byte[] data = File.ReadAllBytes("image.jpg");
LegioImageBuilder.Load(data)

// From stream
using var stream = File.OpenRead("image.jpg");
LegioImageBuilder.Load(stream)

// Get metadata without full decode
var info = LegioImageBuilder.Load("image.jpg").GetInfo();
// info.Width, info.Height, info.Format, info.HasAlpha, info.ByteSize
```

## Resize

```csharp
// Exact dimensions
LegioImageBuilder.Load("image.jpg")
    .Resize(800, 600)
    .Save("output.jpg");

// Scale modes
.Resize(800, 600, LegioScaleMode.Fit)      // Fit within bounds
.Resize(800, 600, LegioScaleMode.Fill)     // Fill bounds (may crop)
.Resize(800, 600, LegioScaleMode.Stretch)  // Stretch to exact size

// Maintain aspect ratio
.ResizeToWidth(1200)  // Scale to 1200px wide
.ResizeToHeight(800)  // Scale to 800px tall
.Scale(0.5)           // Half size

// Resize quality
.Resize(800, 600, quality: LegioResizeQuality.High)
```

**Quality Levels**: Low (fast), Medium (balanced), High (default), Maximum (best quality, slower)

## Crop, Rotate, Flip

```csharp
// Crop region
.Crop(x: 100, y: 100, width: 400, height: 400)

// Rotate (valid angles: 0, 90, 180, 270)
.Rotate(90)
.Rotate(180)
.Rotate(270)

// Flip
.Flip(horizontal: true, vertical: false)   // Horizontal only
.Flip(horizontal: false, vertical: true)   // Vertical only
.Flip(true, true)                           // Both
```

## Filters

```csharp
.Grayscale()
.Sepia()
.Blur(5)      // Radius 1-20, default 5
.Sharpen(50)  // Amount 0-100, default 50
```

## Color Adjustments

```csharp
.Brightness(30)   // Range -255 to 255
.Brightness(-30)  // Darken
.Contrast(20)     // Range -100 to 100
.Contrast(-20)    // Reduce contrast
.Invert()         // Negative effect
```

## Quality Control

```csharp
// Set quality globally
LegioImageBuilder.Load("image.jpg")
    .Quality(90)
    .Save("output.jpg");

// Or override in Save()
.Save("output.jpg", LegioImageFormat.Jpeg, 90)
```

**Recommended ranges:**
- JPEG: 70-85
- WebP: 80-90
- PNG: Always 100 (lossless, quality parameter ignored)

## Save Options

```csharp
// Auto-detect format from extension
.Save("output.jpg")

// Specify format
.Save("output.png", LegioImageFormat.Png)

// With quality
.Save("output.jpg", LegioImageFormat.Jpeg, 90)

// As byte array
byte[] data = SaveAs(LegioImageFormat.Jpeg, 90);

// As stream
using var stream = SaveAsStream(LegioImageFormat.Png);
```

## Common Patterns

### Thumbnail Generation

```csharp
LegioImageBuilder.Load("photo.jpg")
    .ResizeToWidth(200)
    .Quality(80)
    .Save("thumbnail.jpg");
```

### Profile Picture

```csharp
LegioImageBuilder.Load("photo.jpg")
    .Resize(500, 500, LegioScaleMode.Fill)
    .Brightness(10)
    .Contrast(15)
    .Save("profile.jpg", LegioImageFormat.Jpeg, 90);
```

### Photo Enhancement

```csharp
LegioImageBuilder.Load("dark-photo.jpg")
    .Brightness(30)
    .Contrast(20)
    .Sharpen(60)
    .Blur(1)
    .Save("enhanced.jpg", LegioImageFormat.Jpeg, 90);
```

### WebP Optimization

```csharp
var data = LegioImageBuilder.Load("large.jpg")
    .ResizeToWidth(1920)
    .SaveAs(LegioImageFormat.WebP, 85);

File.WriteAllBytes("optimized.webp", data);
```

### Batch Processing

```csharp
foreach (var file in Directory.GetFiles("input", "*.jpg"))
{
    var name = Path.GetFileNameWithoutExtension(file);
    LegioImageBuilder.Load(file)
        .Resize(1920, 1080, LegioScaleMode.Fit)
        .Quality(85)
        .Save(Path.Combine("output", $"{name}_resized.jpg"));
}
```

## Best Practices

1. **Chain operations** - Load once, chain all operations, save once
2. **Choose appropriate quality** - Balance file size vs quality
3. **Select right scale mode** - Fit for thumbnails, Fill for covers, Stretch only when needed
4. **Batch efficiently** - Load and process one file at a time in loop

**Good:**
```csharp
LegioImageBuilder.Load("image.jpg")
    .Resize(800, 600)
    .Grayscale()
    .Save("output.jpg");
```

**Avoid:**
```csharp
// Loads image multiple times
LegioImageBuilder.Load("image.jpg").Resize(800, 600).Save("1.jpg");
LegioImageBuilder.Load("image.jpg").Grayscale().Save("2.jpg");
```

## Supported Formats

**Input**: PNG, JPEG, WebP

**Output**: PNG, JPEG, WebP

| Format | Type | Best For |
|--------|------|----------|
| PNG | Lossless | Graphics, screenshots, images with transparency |
| JPEG | Lossy | Photographs, photos |
| WebP | Lossy/Lossless | Modern web images, better compression than JPEG |

## Parameter Ranges

| Parameter | Range | Default |
|-----------|-------|---------|
| Quality | 0-100 | 75 |
| Blur radius | 1-20 | 5 |
| Sharpen amount | 0-100 | 50 |
| Brightness | -255 to 255 | 0 |
| Contrast | -100 to 100 | 0 |
| Rotation | 0, 90, 180, 270 | - |

## Platform Support

- Windows (x64)
- Linux (x64, ARM64)
- macOS (x64, ARM64)

## Dependencies

- .NET 6.0, 7.0, 8.0, 9.0, 10.0
- LegioSoft.Imaging.Core
- SkiaSharp 2.88.6
- Platform-specific SkiaSharp native assets

## Documentation

- [SKIA_MANUAL.md](../../docs/SKIA_MANUAL.md) - Complete usage guide
- [SkiaExample](../../examples/SkiaExample) - Working code examples

## Security Features

- **Image validation** - Checks dimensions (max 16K x 16K) and memory limits (512MB)
- **Format detection** - Secure signature validation (via Core package)
- **Bounds checking** - Validates all operation parameters
- **Input validation** - Validates files, streams, and byte arrays before processing

## License

Apache License 2.0
