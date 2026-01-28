# LegioSoft.Imaging.WebP

WebP codec wrapper using native libwebp v1.6.0. High-performance WebP encoding and decoding with support for multiple color formats. Implements Core service interfaces for standardized image processing.

## Installation

```bash
dotnet add package LegioSoft.Imaging.WebP
```

## Features

- **Core Interface Implementation** - Implements ILegioImageEncoder, ILegioImageDecoder, ILegioImageResizer, ILegioImageCropper, and ILegioImageTransformer from Core package
- **Encoding** - RGBA, RGB, BGRA, BGR formats with lossless/lossy compression
- **Decoding** - Multiple colorspaces (RGBA, RGB, BGRA, BGR, ARGB)
- **Transformation** - Scale, crop, and flip images during encoding/decoding
- **Advanced Options** - Fine-tune compression quality, method, filters, and alpha settings
- **Metadata** - Extract image dimensions, alpha channel, animation, and format type
- **Cross-Platform** - Windows, Linux, macOS native library support

## Core Interfaces

The WebP package implements the following Core interfaces for standardized image processing:

### LegioImageWebPProcessor

Main class implementing all Core interfaces:

```csharp
using LegioSoft.Imaging.Core;
using LegioSoft.Imaging.WebP;

var processor = new LegioImageWebPProcessor();
```

**Implemented Interfaces:**
- `ILegioImageEncoder` - Encode images to WebP format
- `ILegioImageDecoder` - Decode WebP images and get metadata
- `ILegioImageResizer` - Resize WebP images
- `ILegioImageCropper` - Crop WebP images
- `ILegioImageTransformer` - Rotate and flip WebP images

### Encoding (ILegioImageEncoder)

```csharp
var processor = new LegioImageWebPProcessor();

// Encode RGBA data to WebP (byte[] with RGBA pixel data)
byte[] webpData = processor.Encode(
    rgbaData,
    LegioImageFormat.WebP,
    LegioEncodingQuality.High
);

// Encode from stream
using var inputStream = new MemoryStream(rgbaData);
using var outputStream = processor.Encode(
    inputStream,
    LegioImageFormat.WebP,
    LegioEncodingQuality.High
);
```

**Note:** The `imageData` parameter expects raw pixel data (RGBA/RGB bytes), not encoded image file data. Use a decoder (like System.Drawing.Common or SkiaSharp) to convert encoded images to raw pixels first.

### Decoding (ILegioImageDecoder)

```csharp
var processor = new LegioImageWebPProcessor();

// Decode WebP to raw RGBA bytes
byte[] rgbaData = processor.Decode(webpData, out var format);

// Get image metadata
var info = processor.GetImageInfo(webpData);
Console.WriteLine($"Dimensions: {info.Width}x{info.Height}");
Console.WriteLine($"Has Alpha: {info.HasAlpha}");
Console.WriteLine($"Byte Size: {info.ByteSize} bytes");

// Get info from stream
using var stream = new MemoryStream(webpData);
var streamInfo = processor.GetImageInfo(stream);
```

### Resizing (ILegioImageResizer)

```csharp
var processor = new LegioImageWebPProcessor();

// Resize WebP image (native scaling during decode - very fast)
byte[] resized = processor.Resize(
    webpData,
    640,
    480,
    LegioScaleMode.Fit,
    LegioResizeQuality.High
);

// Resize from stream
using var inputStream = new MemoryStream(webpData);
using var outputStream = processor.Resize(
    inputStream,
    640,
    480,
    LegioScaleMode.Fit
);
```

**Scale Modes:**
- `LegioScaleMode.Fit` - Maintain aspect ratio
- `LegioScaleMode.Fill` - Fill dimensions, crop overflow
- `LegioScaleMode.Stretch` - Stretch to exact dimensions

**Resize Quality:**
- `LegioResizeQuality.Low` - Fastest resizing
- `LegioResizeQuality.Medium` - Balanced speed/quality
- `LegioResizeQuality.High` - High quality (default)
- `LegioResizeQuality.Maximum` - Best quality

### Cropping (ILegioImageCropper)

```csharp
var processor = new LegioImageWebPProcessor();

// Crop WebP image (native cropping during decode)
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

### Transformation (ILegioImageTransformer)

```csharp
var processor = new LegioImageWebPProcessor();

// Rotate (90-degree increments)
byte[] rotated = processor.Rotate(webpData, 90);  // 90, 180, 270

// Flip
byte[] flipped = processorFlip(webpData, horizontal: false, vertical: true);

// Transform from stream
using var inputStream = new MemoryStream(webpData);
using var outputStream = processor.Rotate(inputStream, 90);
```

## Static Convenience API (WebPImage)

For quick operations, use the static `WebPImage` class:

### Encoding

```csharp
using LegioSoft.Imaging.WebP;

// Encode RGBA data
byte[] webpData = WebPImage.Encode(rgbaData, width, height, quality: 75);

// Encode with lossless compression
byte[] lossless = WebPImage.EncodeLossless(rgbaData, width, height);

// Encode RGB data (smaller files, no alpha)
byte[] rgbData = ConvertToRGB(rgbaData);
byte[] webpData = WebPImage.EncodeRGB(rgbData, width, height, quality: 80);

// Advanced encoding with options
var options = new WebPEncodeOptions
{
    Quality = 80.0f,
    Method = 6,
    FilterStrength = 60,
    Autofilter = true,
    AlphaQuality = 90
};
byte[] advanced = WebPImage.EncodeAdvanced(rgbaData, width, height, options);
```

### Decoding

```csharp
// Decode WebP to RGBA
byte[] rgbaData = WebPImage.Decode(webpData);

// Decode with specific colorspace
byte[] bgrData = WebPImage.Decode(webpData, WEBP_CSP_MODE.MODE_BGR);

// Decode from file or stream
byte[] data = WebPImage.Decode("image.webp");
using var stream = File.OpenRead("image.webp");
byte[] data = WebPImage.Decode(stream);
```

### Transformation

```csharp
// Scale during decode
byte[] scaled = WebPImage.Scale(webpData, 640, 480);

// Crop during decode
byte[] cropped = WebPImage.Crop(webpData, cropX: 10, cropY: 10, cropWidth: 200, cropHeight: 200);

// Flip vertically during decode
byte[] flipped = WebPImage.Flip(webpData);
```

### Image Info

```csharp
WebPInfo info = WebPImage.GetInfo(webpData);
Console.WriteLine($"Dimensions: {info.Width}x{info.Height}");
Console.WriteLine($"Has Alpha: {info.HasAlpha}");
Console.WriteLine($"Animated: {info.HasAnimation}");
Console.WriteLine($"Format: {info.Format}");
```

### Validation

```csharp
byte[] imageData = File.ReadAllBytes("file.webp");
bool isValidWebP = WebPImage.IsValidWebP(imageData);

string version = WebPImage.GetVersion();
Console.WriteLine($"libwebp version: {version}");
```

## Encoding Options

`WebPEncodeOptions` provides advanced control:

- **Preset** - DEFAULT, PICTURE, PHOTO, DRAWING, ICON, TEXT
- **Quality** - 0-100 quality level
- **Lossless** - Enable lossless compression
- **Method** - 0-6 (faster to better compression)
- **ImageHint** - DEFAULT, PICTURE, PHOTO, GRAPH
- **FilterStrength** - 0-100 filter strength
- **FilterSharpness** - 0-7 filter sharpness
- **Autofilter** - Enable automatic filter adjustment
- **AlphaCompression** - 0-1 alpha compression mode
- **AlphaFiltering** - 0-2 alpha filtering
- **AlphaQuality** - 0-100 alpha channel quality
- **Pass** - 1-10 analysis passes
- **SnsStrength** - 0-100 spatial noise shaping
- **Segments** - 1-4 partition count
- **Partitions** - 0-3 partition limit

## Colorspaces

- **MODE_RGBA** - 4 bytes (R, G, B, A)
- **MODE_RGB** - 3 bytes (R, G, B)
- **MODE_BGRA** - 4 bytes (B, G, R, A)
- **MODE_BGR** - 3 bytes (B, G, R)
- **MODE_ARGB** - 4 bytes (A, R, G, B)

## Platform Support

- .NET 6.0, 7.0, 8.0, 9.0, 10.0
- Windows (x64)
- Linux (x64, ARM64)
- macOS (x64, ARM64)

## Dependencies

- LegioSoft.Imaging.Core - Core interfaces and types
- libwebp v1.6.0 native library (included)

### Native Libraries

The package includes pre-compiled libwebp libraries:
- **Windows**: Built with Visual Studio Build Tools (see [BUILD_WEBP.md](../../docs/BUILD_WEBP.md))
- **Linux**: Built with Docker (glibc) for x64 and ARM64 (see [BUILD_WEBP.md](../../docs/BUILD_WEBP.md))
- **macOS**: Built from source using CMake (see [BUILD_WEBP.md](../../docs/BUILD_WEBP.md))

## API Design Philosophy

**Public API:**
- `LegioImageWebPProcessor` - Implements Core interfaces for standardized operations
- `WebPImage` - Static convenience API for quick operations
- `WebPEncodeOptions` - Configuration for advanced encoding
- `WebPInfo` - WebP metadata
- Public enums: `WEBP_CSP_MODE`, `WebPFormat`, `WebPPreset`, `WebPInputFormat`, `WebPImageHint`

**Internal Implementation:**
- `WebPEncoder` - Internal encoding logic
- `WebPDecoder` - Internal decoding logic
- Native classes - P/Invoke wrappers
- Helper classes - Validation utilities

## Usage Patterns

### Using Core Interfaces (Recommended for Framework Integration)

```csharp
using LegioSoft.Imaging.Core;
using LegioSoft.Imaging.WebP;

// Create processor implementing all Core interfaces
var processor = new LegioImageWebPProcessor();

// Decode WebP
var info = processor.GetImageInfo(webpData);
var rgbaData = processor.Decode(webpData, out var format);

// Resize
var resized = processor.Resize(webpData, 800, 600, LegioScaleMode.Fit);

// Crop
var cropped = processor.Crop(webpData, 100, 100, 400, 400);

// Transform
var rotated = processor.Rotate(webpData, 90);
var flipped = processor.Flip(webpData, false, true);
```

### Using Static API (Quick Operations)

```csharp
using LegioSoft.Imaging.WebP;

// Quick encode
byte[] webpData = WebPImage.Encode(rgbaData, width, height, 75);

// Quick decode
byte[] rgbaData = WebPImage.Decode(webpData);

// Quick transform
byte[] scaled = WebPImage.Scale(webpData, 400, 300);
```

## License

Apache License 2.0
