# SkiaSharp Image Processing Example

This example demonstrates how to use **LegioSoft.Imaging.Skia** for comprehensive image processing operations.

## Installation

```bash
dotnet add package LegioSoft.Imaging.Skia
```

Or via NuGet Package Manager:

```
Install-Package LegioSoft.Imaging.Skia
```

## Requirements

- .NET 6.0 or later
- A test image file named `test-image.png` in the examples folder

## Running the Example

```bash
dotnet run
```

## Examples Covered

### 1. Load and Display Info
```csharp
var info = LegioImageBuilder.Load("image.jpg").GetInfo();

Console.WriteLine($"Width: {info.Width}");
Console.WriteLine($"Height: {info.Height}");
Console.WriteLine($"Format: {info.Format}");
Console.WriteLine($"Has Alpha: {info.HasAlpha}");
Console.WriteLine($"Size: {info.ByteSize} bytes");
```

### 2. Resize Operations

#### Exact Dimensions
```csharp
var resized = LegioImageBuilder.Load("image.jpg")
    .Resize(800, 600)
    .Save("resized.jpg", LegioImageFormat.Jpeg, 85);
```

#### Resize to Width (maintains aspect ratio)
```csharp
var resized = LegioImageBuilder.Load("image.jpg")
    .ResizeToWidth(800)
    .Save("resized.jpg", LegioImageFormat.Jpeg, 85);
```

#### Resize to Height (maintains aspect ratio)
```csharp
var resized = LegioImageBuilder.Load("image.jpg")
    .ResizeToHeight(600)
    .Save("resized.jpg", LegioImageFormat.Jpeg, 85);
```

#### Scale by Factor
```csharp
var resized = LegioImageBuilder.Load("image.jpg")
    .Scale(0.5)
    .Save("resized.jpg", LegioImageFormat.Jpeg, 85);
```

### 3. Crop Operations

```csharp
var cropped = LegioImageBuilder.Load("image.jpg")
    .Crop(x: 10, y: 10, width: 200, height: 200)
    .Save("cropped.jpg", LegioImageFormat.Jpeg, 85);
```

### 4. Rotate Operations

```csharp
var rotated = LegioImageBuilder.Load("image.jpg")
    .Rotate(90)
    .Save("rotated.jpg", LegioImageFormat.Jpeg, 85);
```

Valid rotation angles: 90, 180, 270 degrees

### 5. Flip Operations

```csharp
var flipped = LegioImageBuilder.Load("image.jpg")
    .Flip(horizontal: true, vertical: true)
    .Save("flipped.jpg", LegioImageFormat.Jpeg, 85);
```

### 6. Filter Operations

#### Grayscale
```csharp
var grayscale = LegioImageBuilder.Load("image.jpg")
    .Grayscale()
    .Save("grayscale.jpg", LegioImageFormat.Jpeg, 85);
```

#### Sepia
```csharp
var sepia = LegioImageBuilder.Load("image.jpg")
    .Sepia()
    .Save("sepia.jpg", LegioImageFormat.Jpeg, 85);
```

#### Blur
```csharp
var blurred = LegioImageBuilder.Load("image.jpg")
    .Blur(radius: 5)
    .Save("blurred.jpg", LegioImageFormat.Jpeg, 85);
```

Valid blur radius: 1-20

#### Sharpen
```csharp
var sharpened = LegioImageBuilder.Load("image.jpg")
    .Sharpen(amount: 50)
    .Save("sharpened.jpg", LegioImageFormat.Jpeg, 85);
```

Valid sharpen amount: 0-100

### 7. Format Conversion

```csharp
var builder = LegioImageBuilder.Load("image.jpg");

var png = builder.SaveAs(LegioImageFormat.Png);
var jpeg = builder.SaveAs(LegioImageFormat.Jpeg, 85);
var webp = builder.SaveAs(LegioImageFormat.WebP, 85);
var bmp = builder.SaveAs(LegioImageFormat.Bmp);
```

### 8. Quality Control

```csharp
var low = LegioImageBuilder.Load("image.jpg")
    .Quality(50)
    .Save("low-quality.jpg", LegioImageFormat.Jpeg);

var high = LegioImageBuilder.Load("image.jpg")
    .Quality(90)
    .Save("high-quality.jpg", LegioImageFormat.Jpeg);

var maximum = LegioImageBuilder.Load("image.jpg")
    .Quality(100)
    .Save("max-quality.jpg", LegioImageFormat.Jpeg);
```

Quality range: 0-100 for JPEG, WebP

### 9. Chained Operations

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

### 10. Scale Modes

```csharp
var fit = LegioImageBuilder.Load("image.jpg")
    .Resize(800, 600, LegioScaleMode.Fit)
    .Save("fit.jpg", LegioImageFormat.Jpeg, 85);

var fill = LegioImageBuilder.Load("image.jpg")
    .Resize(800, 600, LegioScaleMode.Fill)
    .Save("fill.jpg", LegioImageFormat.Jpeg, 85);

var stretch = LegioImageBuilder.Load("image.jpg")
    .Resize(800, 600, LegioScaleMode.Stretch)
    .Save("stretch.jpg", LegioImageFormat.Jpeg, 85);
```

**Scale Modes:**
- **Fit**: Resizes image to fit within bounds while maintaining aspect ratio
- **Fill**: Resizes image to fill bounds while maintaining aspect ratio (may crop)
- **Stretch**: Resizes image to exact dimensions (may distort)

### 11. Resize Quality

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

**Resize Quality Levels:**
- **Low**: Fast resizing, lower quality
- **Medium**: Balanced performance and quality
- **High**: Best quality for most use cases
- **Maximum**: Highest quality, slower

### 12. Additional Operations

#### Brightness
```csharp
var brighter = LegioImageBuilder.Load("image.jpg")
    .Brightness(30)
    .Save("brighter.jpg", LegioImageFormat.Jpeg, 85);
```

Range: -255 to 255

#### Contrast
```csharp
var contrast = LegioImageBuilder.Load("image.jpg")
    .Contrast(20)
    .Save("contrast.jpg", LegioImageFormat.Jpeg, 85);
```

Range: -100 to 100

#### Invert Colors
```csharp
var inverted = LegioImageBuilder.Load("image.jpg")
    .Invert()
    .Save("inverted.jpg", LegioImageFormat.Jpeg, 85);
```

## Common Patterns

### Thumbnail Generation
```csharp
var thumbnail = LegioImageBuilder.Load("photo.jpg")
    .ResizeToWidth(200)
    .Quality(80)
    .Save("thumbnail.jpg", LegioImageFormat.Jpeg, 80);
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
var inputFiles = Directory.GetFiles("input", "*.jpg");

foreach (var file in inputFiles)
{
    var filename = Path.GetFileNameWithoutExtension(file);
    LegioImageBuilder.Load(file)
        .Resize(1920, 1080, LegioScaleMode.Fit)
        .Save(Path.Combine("output", $"{filename}_resized.jpg"), LegioImageFormat.Jpeg, 85);
}
```

## Error Handling

```csharp
try
{
    var result = LegioImageBuilder.Load("image.jpg")
        .Resize(800, 600)
        .Save("output.jpg", LegioImageFormat.Jpeg, 85);
}
catch (ArgumentException ex)
{
    Console.WriteLine($"Invalid parameters: {ex.Message}");
}
catch (FileNotFoundException ex)
{
    Console.WriteLine($"File not found: {ex.Message}");
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
}
```

## Output Examples

After running the example, you'll find these output files:

- `output-resized-100x100.jpg` - Resized to 100x100
- `output-resized-width50.jpg` - Resized to width 50
- `output-resized-half.jpg` - Resized to 50% of original
- `output-cropped.jpg` - Cropped to 100x100
- `output-rotated-90.jpg` - Rotated 90 degrees
- `output-rotated-180.jpg` - Rotated 180 degrees
- `output-rotated-270.jpg` - Rotated 270 degrees
- `output-flipped-horizontal.jpg` - Flipped horizontally
- `output-flipped-vertical.jpg` - Flipped vertically
- `output-flipped-both.jpg` - Flipped both directions
- `output-grayscale.jpg` - Grayscale version
- `output-sepia.jpg` - Sepia version
- `output-blur.jpg` - Blurred version
- `output-sharpen.jpg` - Sharpened version
- `output-converted-png.png` - PNG format
- `output-converted-jpeg.jpg` - JPEG format
- `output-converted-webp.webp` - WebP format
- `output-converted-bmp.bmp` - BMP format
- `output-quality-low.jpg` - Low quality
- `output-quality-medium.jpg` - Medium quality
- `output-quality-high.jpg` - High quality
- `output-quality-maximum.jpg` - Maximum quality
- `output-chained.jpg` - Multiple operations chained
- `output-scale-fit.jpg` - Fit scale mode
- `output-scale-fill.jpg` - Fill scale mode
- `output-scale-stretch.jpg` - Stretch scale mode

## Best Practices

1. **Use appropriate quality settings:**
   - 70-85 for JPEG (good balance of quality and size)
   - 80-90 for WebP (good balance of quality and size)
   - 100 for PNG (lossless)

2. **Batch operations to avoid loading image multiple times:**
   ```csharp
   var builder = LegioImageBuilder.Load("image.jpg");
   var result1 = builder.Resize(800, 600).Save("1.jpg");
   var result2 = builder.Crop(10, 10, 200, 200).Save("2.jpg");
   ```

3. **Choose the right scale mode:**
   - **Fit**: For thumbnails and responsive images
   - **Fill**: For cover images and banners
   - **Stretch**: Only when exact dimensions are required

4. **Dispose of builders:**
   - The builder is designed for one-time use
   - Load a new builder for each image or chain all operations

## See Also

- [SKIA_MANUAL.md](../../docs/SKIA_MANUAL.md) - Complete SkiaSharp usage guide
- [INSTALLATION.md](../../docs/INSTALLATION.md) - Installation instructions
