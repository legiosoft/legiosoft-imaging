# LegioSoft.Imaging.Skia

Full-featured image processing with fluent builder API.

## Installation

```bash
dotnet add package LegioSoft.Imaging.Skia
```

## Quick Start

```csharp
using LegioSoft.Imaging.Skia;

// Load, process, save
LegioImageBuilder.Load("input.jpg")
    .Resize(800, 600)
    .Save("output.jpg");
```

## Load

```csharp
// From file
var builder = LegioImageBuilder.Load("image.jpg");

// From bytes
byte[] data = File.ReadAllBytes("image.jpg");
var builder = LegioImageBuilder.Load(data);

// From stream
using var stream = File.OpenRead("image.jpg");
var builder = LegioImageBuilder.Load(stream);

// Get metadata without full decode
var info = LegioImageBuilder.Load("image.jpg").GetInfo();
// info.Width, info.Height, info.Format, info.HasAlpha, info.ByteSize
```

## Resize

```csharp
// Exact dimensions
.Resize(800, 600)

// Scale mode
.Resize(800, 600, LegioScaleMode.Fit)    // Fit within
.Resize(800, 600, LegioScaleMode.Fill)   // Fill and crop
.Resize(800, 600, LegioScaleMode.Stretch) // Stretch

// Resize to width (maintain aspect ratio)
.ResizeToWidth(1200)

// Resize to height (maintain aspect ratio)
.ResizeToHeight(800)

// Scale by factor
.Scale(0.5)  // Half size
.Scale(2.0)  // Double size

// Resize quality
.Resize(800, 600, quality: LegioResizeQuality.High)
```

**Quality Levels:** Low, Medium, High, Maximum

## Crop

```csharp
.Crop(x: 100, y: 100, width: 400, height: 400)
```

## Transform

```csharp
.Rotate(90)    // 0, 90, 180, or 270
.Flip(horizontal: true, vertical: false)
.Flip(true, true)  // Both
```

## Filters

```csharp
.Grayscale()
.Sepia()
.Blur(5)      // Radius 1-20
.Sharpen(50)  // Amount 0-100
```

## Color Adjustments

```csharp
.Brightness(30)   // -255 to 255
.Contrast(20)     // -100 to 100
.Invert()
```

## Quality

```csharp
.Quality(85)  // 0-100
```

- JPEG: 70-85 recommended
- WebP: 80-90 recommended
- PNG: always lossless (quality ignored)

## Save

```csharp
// Save with detected format
.Save("output.jpg")

// Save with format
.Save("output.png", LegioImageFormat.Png)

// Save with quality
.Save("output.jpg", quality: 90)

// Save as bytes
byte[] data = SaveAs(LegioImageFormat.Jpeg, 90);

// Save as stream
using var stream = SaveAsStream(LegioImageFormat.Png);
```

## Formats

- PNG - Lossless, supports transparency
- JPEG - Lossy, best for photos
- WebP - Modern, good compression

## Examples

**Thumbnail:**
```csharp
LegioImageBuilder.Load("photo.jpg")
    .ResizeToWidth(200)
    .Quality(80)
    .Save("thumb.jpg");
```

**Profile Picture:**
```csharp
LegioImageBuilder.Load("photo.jpg")
    .Resize(500, 500, LegioScaleMode.Fill)
    .Brightness(10)
    .Contrast(15)
    .Save("profile.jpg", 90);
```

**Web Optimization:**
```csharp
var data = LegioImageBuilder.Load("large.jpg")
    .ResizeToWidth(1920)
    .SaveAs(LegioImageFormat.WebP, 85);
File.WriteAllBytes("optimized.webp", data);
```

**Batch Process:**
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

## Performance

Operations execute in order. Intermediate bitmaps auto-disposed.

**Good:**
```csharp
// Loads once, chains operations
LegioImageBuilder.Load("image.jpg")
    .Resize(800, 600)
    .Grayscale()
    .Save("out.jpg");
```

**Bad:**
```csharp
// Loads multiple times
LegioImageBuilder.Load("image.jpg").Resize(800, 600).Save("1.jpg");
LegioImageBuilder.Load("image.jpg").Grayscale().Save("2.jpg");
```

## Platforms

- Windows x64
- Linux x64, ARM64
- macOS x64, ARM64
