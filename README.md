# LegioSoft.Imaging

Modular .NET image processing library with unified API. Lightweight WebP support for simple use cases, or full-featured image processing with SkiaSharp for advanced operations.

## Installation

### Meta Package (Recommended)

Install all packages at once:

```bash
dotnet add package LegioSoft.Imaging
```

Or via NuGet Package Manager Console:

```
Install-Package LegioSoft.Imaging
```

### Individual Packages

**Lightweight WebP Only** (~500KB):
```bash
dotnet add package LegioSoft.Imaging.Core
dotnet add package LegioSoft.Imaging.WebP
```

**Full Image Processing** (~5MB):
```bash
dotnet add package LegioSoft.Imaging.Core
dotnet add package LegioSoft.Imaging.Skia
```

**Supported Frameworks**: .NET 6.0, 7.0, 8.0, 9.0, 10.0

## Quick Start

### Basic Image Processing

Resize, crop, rotate, and save in one fluent chain:

```csharp
using LegioSoft.Imaging.Skia;

// Simple resize
LegioImageBuilder.Load("photo.jpg")
    .Resize(800, 600)
    .Save("resized.jpg");

// Multiple operations
var result = LegioImageBuilder.Load("image.png")
    .Resize(1920, 1080, LegioScaleMode.Fit)
    .Crop(100, 100, 400, 400)
    .Rotate(90)
    .Grayscale()
    .Save("processed.jpg", quality: 90);
```

### Format Conversion

Convert between PNG, JPEG, WebP, BMP, and GIF:

```csharp
// PNG to WebP with quality control
var webpData = LegioImageBuilder.Load("image.png")
    .SaveAs(LegioImageFormat.WebP, quality: 85);

// JPEG to PNG (lossless)
LegioImageBuilder.Load("photo.jpg")
    .Save("photo.png");
```

### Image Information

Get image metadata:

```csharp
var info = LegioImageBuilder.Load("image.jpg").GetInfo();

Console.WriteLine($"Dimensions: {info.Width}x{info.Height}");
Console.WriteLine($"Format: {info.Format}");
Console.WriteLine($"Has Alpha: {info.HasAlpha}");
Console.WriteLine($"Size: {info.ByteSize} bytes");
```

## Fluent Builder API

The `LegioImageBuilder` provides a fluent, chainable API for image operations:

### Load

```csharp
// From file path
var builder = LegioImageBuilder.Load("image.jpg");

// From byte array
var imageData = File.ReadAllBytes("image.jpg");
var builder = LegioImageBuilder.Load(imageData);

// From stream
using var stream = File.OpenRead("image.jpg");
var builder = LegioImageBuilder.Load(stream);
```

### Resize

```csharp
// Exact dimensions
.Resize(800, 600)

// Scale mode
.Resize(800, 600, LegioScaleMode.Fit)   // Fit within bounds
.Resize(800, 600, LegioScaleMode.Fill)  // Fill and crop
.Resize(800, 600, LegioScaleMode.Stretch) // Stretch to fit

// Resize to width only (maintains aspect ratio)
.ResizeToWidth(1200)

// Resize to height only (maintains aspect ratio)
.ResizeToHeight(800)

// Scale by factor
.Scale(0.5)  // 50% of original size
.Scale(2.0)  // 200% of original size

// Quality control
.Resize(800, 600, quality: LegioResizeQuality.High)
```

### Crop

```csharp
// Crop from (x, y) with width and height
.Crop(100, 100, 400, 400)
```

### Transform

```csharp
// Rotate (90, 180, or 270 degrees)
.Rotate(90)
.Rotate(180)
.Rotate(270)

// Flip
.Flip(horizontal: true, vertical: false)  // Horizontal flip
.Flip(horizontal: false, vertical: true)  // Vertical flip
.Flip(true, true)                        // Both
```

### Filters

```csharp
// Grayscale
.Grayscale()

// Sepia tone
.Sepia()

// Blur (radius 1-20, default 5)
.Blur()
.Blur(10)

// Sharpen (amount 0-100, default 50)
.Sharpen()
.Sharpen(70)
```

### Color Adjustments

```csharp
// Brightness (-255 to 255)
.Brightness(50)   // Lighten
.Brightness(-50)  // Darken

// Contrast (-100 to 100)
.Contrast(30)
.Contrast(-20)

// Invert colors
.Invert()
```

### Save

```csharp
// Save with detected format
.Save("output.jpg")

// Save with specific format
.Save("output.png", LegioImageFormat.Png)

// Save with quality (0-100, for lossy formats)
.Save("output.jpg", quality: 85)

// Save as byte array
var jpegData = SaveAs(LegioImageFormat.Jpeg, quality: 90);
var webpData = SaveAs(LegioImageFormat.WebP, quality: 80);

// Save as stream
using var stream = SaveAsStream(LegioImageFormat.Png);
```

## Complete Examples

### WebP Optimization

```csharp
// Convert to WebP for web use
var optimized = LegioImageBuilder.Load("large-photo.jpg")
    .ResizeToWidth(1920)
    .Quality(85)
    .SaveAs(LegioImageFormat.WebP);

File.WriteAllBytes("optimized.webp", optimized);
```

### Thumbnail Generation

```csharp
// Generate thumbnail maintaining aspect ratio
LegioImageBuilder.Load("image.jpg")
    .Resize(300, 300, LegioScaleMode.Fit)
    .Quality(80)
    .Save("thumbnail.jpg");
```

### Image Enhancement

```csharp
// Enhance photo with multiple adjustments
LegioImageBuilder.Load("dark-photo.jpg")
    .Brightness(30)
    .Contrast(20)
    .Sharpen(60)
    .Blur(1)
    .Save("enhanced.jpg", quality: 90);
```

### Batch Processing

```csharp
// Process all images in directory
var inputFiles = Directory.GetFiles("input", "*.jpg");

foreach (var file in inputFiles)
{
    var fileName = Path.GetFileNameWithoutExtension(file);
    var outputPath = Path.Combine("output", $"{fileName}.png");

    LegioImageBuilder.Load(file)
        .Resize(1920, 1080, LegioScaleMode.Fit)
        .Quality(90)
        .Save(outputPath, LegioImageFormat.Png);
}
```

### Profile Picture Processing

```csharp
// Create square profile picture from any image
var processed = LegioImageBuilder.Load("profile-photo.jpg")
    .Resize(500, 500, LegioScaleMode.Fill)  // Square
    .Brightness(10)
    .Contrast(15)
    .SaveAs(LegioImageFormat.Jpeg, quality: 85);
```

### Watermark Effect

```csharp
// Apply subtle blur for watermark effect
LegioImageBuilder.Load("product.jpg")
    .Resize(1200, 800)
    .Blur(2)
    .Grayscale()
    .Save("watermark-preview.jpg");
```

## Package Structure

### LegioSoft.Imaging.Core

Lightweight core package with interfaces, enums, and base types.

- `ILegioImageEncoder` - Encode images
- `ILegioImageDecoder` - Decode and get metadata
- `ILegioImageResizer` - Resize operations
- `ILegioImageCropper` - Crop operations
- `ILegioImageTransformer` - Rotate/flip operations
- `ILegioImageFilter` - Filter operations
- `LegioImageFormat` - PNG, JPEG, WebP, BMP, GIF
- `LegioScaleMode` - Fit, Fill, Stretch
- `LegioResizeQuality` - Low, Medium, High, Maximum
- `LegioImageInfo` - Image metadata

### LegioSoft.Imaging.WebP

Lightweight WebP codec using native libwebp.

- Implements `ILegioImageEncoder` and `ILegioImageDecoder`
- Native libraries for Windows x64, Linux x64, Linux ARM64
- No SkiaSharp dependency
- ~500KB package size

### LegioSoft.Imaging.Skia

Full-featured image processing with SkiaSharp.

- Implements all core interfaces
- Fluent builder API (`LegioImageBuilder`)
- Resize, crop, rotate, flip
- Filters: grayscale, sepia, blur, sharpen
- Color adjustments: brightness, contrast, invert
- Format conversion
- Cross-platform (Windows, macOS, Linux)
- ~5MB package size

### LegioSoft.Imaging

Meta package that includes all components.

- All-in-one installation
- Core + WebP + SkiaSharp
- ~5.5MB package size

## Supported Formats

**Input Formats**: PNG, JPEG, WebP, BMP, GIF

**Output Formats**:
- PNG - Lossless, supports transparency
- JPEG - Lossy, best for photos
- WebP - Modern format, good compression
- BMP - Uncompressed, Windows bitmap
- GIF - Limited color, simple animation support

## Performance Tips

- Use `ResizeToWidth` or `ResizeToHeight` to maintain aspect ratio without extra calculations
- Batch operations in one builder chain to avoid loading image multiple times
- Use appropriate quality settings (70-85 for JPEG, 80-90 for WebP)
- Prefer WebP for web use (better compression than JPEG at similar quality)

## Platform Support

- **Windows**: x64
- **Linux**: x64, ARM64
- **macOS**: x64, ARM64 (via SkiaSharp)

## Dependencies

**Core Package**: No external dependencies

**WebP Package**:
- LegioSoft.Imaging.Core
- Native libwebp libraries (included)

**Skia Package**:
- LegioSoft.Imaging.Core
- SkiaSharp 2.88.6
- Platform-specific SkiaSharp native assets

## License

Apache License 2.0

## Repository

https://github.com/legiosoft/legiosoft-imaging
