# LegioSoft.Imaging

Lightweight image processing library for .NET.

## Packages

### LegioSoft.Imaging.Core
Core interfaces and types for image processing.

### LegioSoft.Imaging.Skia
SkiaSharp-based image operations (resize, crop, rotate, filters).

### LegioSoft.Imaging.WebP
Lightweight WebP codec using native libwebp.

### LegioSoft.Imaging
Meta package providing unified API.

## Quick Start

```csharp
using LegioSoft.Imaging.WebP;

// Check WebP format
bool isWebP = WebPHelper.IsWebP(imageData);

// Encode/decode WebP
byte[] webpData = WebPHelper.Encode(rgbaData, width, height, quality: 85);
byte[] decodedData = WebPHelper.Decode(webpData);
```

## Building WebP Libraries

See [docs/BUILD_WEBP.md](docs/BUILD_WEBP.md) for Docker and PowerShell build scripts.

## Status

**Early development** - API may change.

## License

MIT
