# SkiaSharp Image Processing Example

Comprehensive examples for **LegioSoft.Imaging.Skia** image operations.

## Installation

```bash
dotnet add package LegioSoft.Imaging.Skia
```

## Requirements

- .NET 6.0 or later
- Test image: `test-image.png`

## Running

```bash
dotnet run
```

## API Examples

### Load Image

```csharp
var info = LegioImageBuilder.Load("image.jpg").GetInfo();

Console.WriteLine($"Width: {info.Width}");
Console.WriteLine($"Height: {info.Height}");
Console.WriteLine($"Format: {info.Format}");
Console.WriteLine($"Size: {info.ByteSize} bytes");
```

### Resize

```csharp
// Exact dimensions
var resized = LegioImageBuilder.Load("image.jpg")
    .Resize(800, 600)
    .Save("resized.jpg", LegioImageFormat.Jpeg, 85);

// Resize to width (maintains aspect ratio)
var toWidth = LegioImageBuilder.Load("image.jpg")
    .ResizeToWidth(800)
    .Save("resized.jpg", LegioImageFormat.Jpeg, 85);

// Resize to height (maintains aspect ratio)
var toHeight = LegioImageBuilder.Load("image.jpg")
    .ResizeToHeight(600)
    .Save("resized.jpg", LegioImageFormat.Jpeg, 85);

// Scale by factor
var scaled = LegioImageBuilder.Load("image.jpg")
    .Scale(0.5)
    .Save("resized.jpg", LegioImageFormat.Jpeg, 85);
```

### Scale Modes

```csharp
// Fit: Maintain aspect ratio, fit within bounds
var fit = LegioImageBuilder.Load("image.jpg")
    .Resize(800, 600, LegioScaleMode.Fit)
    .Save("fit.jpg", LegioImageFormat.Jpeg, 85);

// Fill: Maintain aspect ratio, fill bounds (may crop)
var fill = LegioImageBuilder.Load("image.jpg")
    .Resize(800, 600, LegioScaleMode.Fill)
    .Save("fill.jpg", LegioImageFormat.Jpeg, 85);

// Stretch: Exact dimensions (may distort)
var stretch = LegioImageBuilder.Load("image.jpg")
    .Resize(800, 600, LegioScaleMode.Stretch)
    .Save("stretch.jpg", LegioImageFormat.Jpeg, 85);
```

### Crop

```csharp
var cropped = LegioImageBuilder.Load("image.jpg")
    .Crop(x: 10, y: 10, width: 200, height: 200)
    .Save("cropped.jpg", LegioImageFormat.Jpeg, 85);
```

### Rotate

```csharp
var rotated = LegioImageBuilder.Load("image.jpg")
    .Rotate(90)
    .Save("rotated.jpg", LegioImageFormat.Jpeg, 85);
```

Valid angles: 0, 90, 180, 270

### Flip

```csharp
// Horizontal
var hFlip = LegioImageBuilder.Load("image.jpg")
    .Flip(horizontal: true, vertical: false)
    .Save("flipped-h.jpg", LegioImageFormat.Jpeg, 85);

// Vertical
var vFlip = LegioImageBuilder.Load("image.jpg")
    .Flip(horizontal: false, vertical: true)
    .Save("flipped-v.jpg", LegioImageFormat.Jpeg, 85);

// Both
var bothFlip = LegioImageBuilder.Load("image.jpg")
    .Flip(horizontal: true, vertical: true)
    .Save("flipped-both.jpg", LegioImageFormat.Jpeg, 85);
```

### Filters

```csharp
// Grayscale
var gray = LegioImageBuilder.Load("image.jpg")
    .Grayscale()
    .Save("grayscale.jpg", LegioImageFormat.Jpeg, 85);

// Sepia
var sepia = LegioImageBuilder.Load("image.jpg")
    .Sepia()
    .Save("sepia.jpg", LegioImageFormat.Jpeg, 85);

// Blur (radius: 1-20)
var blurred = LegioImageBuilder.Load("image.jpg")
    .Blur(5)
    .Save("blurred.jpg", LegioImageFormat.Jpeg, 85);

// Sharpen (amount: 0-100)
var sharpened = LegioImageBuilder.Load("image.jpg")
    .Sharpen(50)
    .Save("sharpened.jpg", LegioImageFormat.Jpeg, 85);
```

### Color Adjustments

```csharp
// Brightness (-255 to 255)
var bright = LegioImageBuilder.Load("image.jpg")
    .Brightness(30)
    .Save("bright.jpg", LegioImageFormat.Jpeg, 85);

// Contrast (-100 to 100)
var contrast = LegioImageBuilder.Load("image.jpg")
    .Contrast(20)
    .Save("contrast.jpg", LegioImageFormat.Jpeg, 85);

// Invert colors
var inverted = LegioImageBuilder.Load("image.jpg")
    .Invert()
    .Save("inverted.jpg", LegioImageFormat.Jpeg, 85);
```

### Quality Control

```csharp
// Set quality for save operations (0-100)
var result = LegioImageBuilder.Load("image.jpg")
    .Quality(90)
    .Save("high-quality.jpg", LegioImageFormat.Jpeg);

// Or override quality in Save() call
var result = LegioImageBuilder.Load("image.jpg")
    .Save("output.jpg", LegioImageFormat.Jpeg, 90);
```

### Format Conversion

```csharp
var builder = LegioImageBuilder.Load("image.jpg");

var png = builder.SaveAs(LegioImageFormat.Png, 100);
var jpeg = builder.SaveAs(LegioImageFormat.Jpeg, 85);
var webp = builder.SaveAs(LegioImageFormat.WebP, 85);
var bmp = builder.SaveAs(LegioImageFormat.Bmp);
```

### Resize Quality

```csharp
var low = LegioImageBuilder.Load("image.jpg")
    .Resize(800, 600, quality: LegioResizeQuality.Low)
    .Save("low-resize.jpg", LegioImageFormat.Jpeg, 85);

var high = LegioImageBuilder.Load("image.jpg")
    .Resize(800, 600, quality: LegioResizeQuality.High)
    .Save("high-resize.jpg", LegioImageFormat.Jpeg, 85);

var maximum = LegioImageBuilder.Load("image.jpg")
    .Resize(800, 600, quality: LegioResizeQuality.Maximum)
    .Save("maximum-resize.jpg", LegioImageFormat.Jpeg, 85);
```

**Quality Levels:**
- **Low**: Fast, lower quality
- **Medium**: Balanced
- **High**: Best quality for most use cases
- **Maximum**: Highest quality, slower

### Chained Operations

```csharp
var processed = LegioImageBuilder.Load("image.jpg")
    .Resize(300, 300, LegioScaleMode.Fit)
    .Crop(10, 10, 200, 200)
    .Rotate(90)
    .Brightness(20)
    .Contrast(10)
    .Grayscale()
    .Save("processed.jpg", LegioImageFormat.Jpeg, 85);
```

### Save Methods

```csharp
var builder = LegioImageBuilder.Load("image.jpg");

// To byte array
var bytes = builder.SaveAs(LegioImageFormat.Png);

// To file
builder.Save("output.jpg", LegioImageFormat.Jpeg, 85);

// To stream
using var stream = builder.SaveAsStream(LegioImageFormat.Png);
```

## Common Patterns

### Thumbnail Generation

```csharp
var thumbnail = LegioImageBuilder.Load("photo.jpg")
    .ResizeToWidth(200)
    .Quality(80)
    .Save("thumbnail.jpg", LegioImageFormat.Jpeg);
```

### Profile Picture

```csharp
var profile = LegioImageBuilder.Load("photo.jpg")
    .Resize(500, 500, LegioScaleMode.Fill)
    .Brightness(10)
    .Contrast(15)
    .Save("profile.jpg", LegioImageFormat.Jpeg, 90);
```

### Photo Enhancement

```csharp
var enhanced = LegioImageBuilder.Load("dark-photo.jpg")
    .Brightness(30)
    .Contrast(20)
    .Sharpen(60)
    .Blur(1)
    .Save("enhanced.jpg", LegioImageFormat.Jpeg, 90);
```

### WebP Optimization

```csharp
var optimized = LegioImageBuilder.Load("large-photo.jpg")
    .ResizeToWidth(1920)
    .SaveAs(LegioImageFormat.WebP, 85);

File.WriteAllBytes("optimized.webp", optimized);
```

### Batch Processing

```csharp
foreach (var file in Directory.GetFiles("input", "*.jpg"))
{
    var filename = Path.GetFileNameWithoutExtension(file);
    LegioImageBuilder.Load(file)
        .Resize(1920, 1080, LegioScaleMode.Fit)
        .Save(Path.Combine("output", $"{filename}_resized.jpg"), LegioImageFormat.Jpeg, 85);
}
```

## Best Practices

1. **Quality settings:**
   - JPEG: 70-85
   - WebP: 80-90
   - PNG/BMP/GIF: 100 (lossless)

2. **Choose scale mode:**
   - **Fit**: Thumbnails, responsive images
   - **Fill**: Cover images, banners
   - **Stretch**: Only when exact dimensions required

3. **Batch operations** to avoid loading image multiple times:
   ```csharp
   var builder = LegioImageBuilder.Load("image.jpg");
   var result1 = builder.Resize(800, 600).Save("1.jpg");
   var result2 = builder.Crop(10, 10, 200, 200).Save("2.jpg");
   ```

## Parameter Ranges

| Parameter | Range | Default |
|-----------|-------|---------|
| Quality | 0-100 | 75 |
| Blur radius | 1-20 | 5 |
| Sharpen amount | 0-100 | 50 |
| Brightness | -255 to 255 | 0 |
| Contrast | -100 to 100 | 0 |
| Rotation | 0, 90, 180, 270 | - |

## See Also

- [SKIA_MANUAL.md](../../docs/SKIA_MANUAL.md) - Complete usage guide
- [INSTALLATION.md](../../docs/INSTALLATION.md) - Installation instructions
