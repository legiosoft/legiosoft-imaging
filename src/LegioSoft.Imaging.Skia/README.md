# LegioSoft.Imaging.Skia

Full-featured image processing library built on SkiaSharp. Provides fluent builder API for easy image manipulation with resize, crop, rotate, flip, and filter operations.

## Installation

```bash
dotnet add package LegioSoft.Imaging.Skia
```

## Features

- **Fluent Builder API** - Chain multiple image operations
- **Resize** - Multiple scale modes (Fit, Fill, Stretch)
- **Crop** - Extract regions from images
- **Transform** - Rotate (90/180/270 degrees) and flip (horizontal/vertical)
- **Filters** - Grayscale, sepia, blur (configurable radius), sharpen
- **Color Adjustments** - Brightness, contrast, invert colors
- **Format Conversion** - PNG, JPEG, WebP
- **Automatic Format Detection** - Detect image format from byte array
- **Quality Control** - 0-100 quality levels for lossy formats

## Quick Start

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

## Core Classes

- `LegioImageBuilder` - Fluent builder API for image operations
- `LegioImageSkiaProcessor` - Implements core interfaces (ILegioImageResizer, ILegioImageCropper, ILegioImageTransformer, ILegioImageFilter)

## Core Module

- `ImageLoader` - Load images from file, byte array, or stream with validation
- `ImageSaver` - Securely save images with validation, quality adjustment, and format checks
- `FormatDetector` - (moved to Core package) Detect image format from byte array

## Operations Module

- `ImageResizer` - Resize operations with quality control
- `ImageCropper` - Crop operations with bounds validation
- `ImageTransformer` - Rotate (90/180/270) and flip (horizontal/vertical) operations
- `ImageFilters` - Apply grayscale, sepia, blur (configurable radius), and sharpen (configurable amount)
- `ImageColorAdjustments` - Apply brightness (-255 to 255), contrast (-100 to 100), and invert colors
- `ImageConverter` - Convert between image formats

## Supported Formats

**Input**: PNG, JPEG, WebP

**Output**:
- PNG (lossless, supports transparency)
- JPEG (lossy, best for photos)
- WebP (modern format, good compression)

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

For detailed API documentation and usage examples, see [docs/SKIA_MANUAL.md](../../docs/SKIA_MANUAL.md).

## Security Features

- **ImageSaver**: Validates bitmap dimensions, quality range (0-100), format support, and encoding success
- **FormatDetector** (Core): Securely identifies formats with signature validation and file structure checks
- **ImageLoader**: Validates input data before loading
- All operations include parameter validation to prevent invalid inputs

## License

Apache License 2.0
