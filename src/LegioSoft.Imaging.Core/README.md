# LegioSoft.Imaging.Core

Core interfaces, enums, and base types for LegioSoft.Imaging. Extremely lightweight library containing only shared types.

## Installation

```bash
dotnet add package LegioSoft.Imaging.Core
```

## Features

- **Core Interfaces** - ILegioImageEncoder, ILegioImageDecoder, ILegioImageResizer, ILegioImageCropper, ILegioImageTransformer, ILegioImageFilter
- **Enums** - LegioImageFormat, LegioScaleMode, LegioResizeQuality, LegioEncodingQuality, LegioTransformType, LegioFilterType
- **Base Types** - LegioImageInfo class for image metadata

## Core Types

### Classes

- `FormatDetector` - Securely detect image format from byte arrays with validation
- `LegioImageInfo` - Image metadata (width, height, format, alpha channel, byte size)

### Interfaces

- `ILegioImageEncoder` - Encode images to byte arrays and streams
- `ILegioImageDecoder` - Decode images and get metadata
- `ILegioImageResizer` - Resize images with various modes and quality levels
- `ILegioImageCropper` - Crop images to specified regions
- `ILegioImageTransformer` - Rotate and flip images
- `ILegioImageFilter` - Apply filters like grayscale, sepia, blur, and sharpen

### Enums

- `LegioImageFormat` - PNG, JPEG, WebP
- `LegioScaleMode` - Fit, Fill, Stretch
- `LegioResizeQuality` - Low, Medium, High, Maximum
- `LegioEncodingQuality` - Quality levels for encoding
- `LegioTransformType` - Rotation transformations
- `LegioFilterType` - Filter types

## Usage

Use this package to reference core types without bringing in image processing dependencies:

```csharp
using LegioSoft.Imaging.Core;

// Detect image format from byte array
var imageData = File.ReadAllBytes("image.jpg");
var format = FormatDetector.DetectFormat(imageData);
Console.WriteLine($"Format: {format}"); // Jpeg

// Get image info (requires implementation package)
var info = new LegioImageInfo
{
    Width = 1920,
    Height = 1080,
    Format = LegioImageFormat.Jpeg,
    HasAlpha = false,
    ByteSize = 102400
};

// Use enums for settings
var mode = LegioScaleMode.Fit;
var quality = LegioResizeQuality.High;
```

## Format Detection

`FormatDetector.DetectFormat()` securely identifies image formats:

- Validates input data (null, empty, too short)
- Checks file signatures (magic bytes) for: PNG, JPEG, WebP
- Throws `NotSupportedException` for unknown formats
- Throws `ArgumentException` for corrupted/invalid files

**Supported formats**: PNG, JPEG, WebP

## Platform Support

- .NET 6.0, 7.0, 8.0, 9.0, 10.0
- Windows, Linux, macOS (all platforms)

## Dependencies

- No external dependencies
- Pure .NET library

## License

Apache License 2.0
