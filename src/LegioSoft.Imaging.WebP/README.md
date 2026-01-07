# LegioSoft.Imaging.WebP

Lightweight WebP codec wrapper using native libwebp v1.6.0.

## Features

- Encode/decode WebP images
- Support for multiple color formats (RGBA, RGB, BGRA, BGR)
- Scale and crop during decode
- Lossless and lossy compression
- Cross-platform (Windows, Linux, macOS)

## Quick Start

```csharp
using LegioSoft.Imaging.WebP;

// Encode
byte[] webpData = WebPImage.Encode(rgbaData, width, height, quality: 75);

// Decode
byte[] decoded = WebPImage.Decode(webpData);

// Get info
WebPInfo info = WebPImage.GetInfo(webpData);
Console.WriteLine($"{info.Width}x{info.Height}");
```

## Building Native Libraries

See [docs/BUILD_WEBP.md](../../docs/BUILD_WEBP.md).

## API

- `WebPImage` - High-level API
- `WebPDecoder` - Low-level decoder
- `WebPEncoder` - Low-level encoder
- `WebPEncodeOptions` - Advanced encoding options

## Status

**Early development** - API may change.
