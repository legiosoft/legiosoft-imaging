# LegioSoft.Imaging.WebP

WebP codec wrapper using native libwebp v1.6.0. High-performance WebP encoding and decoding with support for multiple color formats.

## Installation

```bash
dotnet add package LegioSoft.Imaging.WebP
```

## Features

- **Encoding** - RGBA, RGB, BGRA, BGR formats with lossless/lossy compression
- **Decoding** - Multiple colorspaces (RGBA, RGB, BGRA, BGR, ARGB)
- **Transformation** - Scale, crop, and flip images during encoding/decoding
- **Advanced Options** - Fine-tune compression quality, method, filters, and alpha settings
- **Metadata** - Extract image dimensions, alpha channel, animation, and format type
- **Cross-Platform** - Windows, Linux, macOS native library support

## Core Classes

- `WebPImage` - High-level static API for encoding, decoding, and transformations
- `WebPEncoder` - Low-level encoder with advanced options
- `WebPDecoder` - Low-level decoder with scaling, cropping, and flip support
- `WebPEncodeOptions` - Advanced encoding configuration (quality, method, filters, alpha)
- `WebPInfo` - WebP image metadata (width, height, format, alpha, animation)
- `WebPValidationHelper` - WebP format validation

## Usage

### Basic Encoding

```csharp
using LegioSoft.Imaging.WebP;

// Encode RGBA data to WebP
byte[] rgbaData = File.ReadAllBytes("image.rgba");
byte[] webpData = WebPImage.Encode(rgbaData, width, height, quality: 75);

// Encode with lossless compression
byte[] lossless = WebPImage.EncodeLossless(rgbaData, width, height);

// Encode RGB data
byte[] rgbData = File.ReadAllBytes("image.rgb");
byte[] webpData = WebPImage.EncodeRGB(rgbData, width, height, quality: 80);
```

### Advanced Encoding

```csharp
var options = new WebPEncodeOptions
{
    Quality = 80.0f,
    Lossless = false,
    Method = 6,
    FilterStrength = 60,
    Autofilter = true,
    AlphaQuality = 90
};

byte[] webpData = WebPImage.EncodeAdvanced(rgbaData, width, height, options);
```

### Basic Decoding

```csharp
// Decode WebP to RGBA
byte[] webpData = File.ReadAllBytes("image.webp");
byte[] rgbaData = WebPImage.Decode(webpData);

// Decode with specific colorspace
byte[] bgrData = WebPImage.Decode(webpData, WEBP_CSP_MODE.MODE_BGR);

// Decode from file or stream
byte[] data = WebPImage.Decode("image.webp");
using var stream = File.OpenRead("image.webp");
byte[] data = WebPImage.Decode(stream);
```

### Decoding with Transformations

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

### Encoding with Transformations

```csharp
// Encode with scaling
byte[] scaled = WebPImage.EncodeWithScaling(rgbaData, width, height, targetWidth: 640, targetHeight: 480);

// Encode with cropping
byte[] cropped = WebPImage.EncodeWithCropping(rgbaData, width, height, cropX: 10, cropY: 10, cropWidth: 200, cropHeight: 200);
```

### Validation

```csharp
byte[] imageData = File.ReadAllBytes("file.webp");
bool isValidWebP = WebPImage.IsValidWebP(imageData);
```

### Version Info

```csharp
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

## Platform Support

- .NET 6.0, 7.0, 8.0, 9.0, 10.0
- Windows (x64)
- Linux (x64, ARM64)
- macOS (supported via native library)

## Dependencies

- LegioSoft.Imaging.Core - Core interfaces and types
- libwebp v1.6.0 native library (included)

## License

Apache License 2.0
