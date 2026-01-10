# LegioSoft.Imaging.WebP

Lightweight WebP encoding and decoding using native libwebp 1.6.0.

## Installation

```bash
dotnet add package LegioSoft.Imaging.Core
dotnet add package LegioSoft.Imaging.WebP
```

## API

### Encode

**RGBA to WebP:**
```csharp
byte[] rgbaData = new byte[width * height * 4];  // 4 bytes per pixel
byte[] webpData = WebPImage.Encode(rgbaData, width, height, quality: 85);
File.WriteAllBytes("output.webp", webpData);
```

**RGB to WebP:**
```csharp
byte[] rgbData = new byte[width * height * 3];  // 3 bytes per pixel
byte[] webpData = WebPImage.EncodeRGB(rgbData, width, height, quality: 85);
```

**Lossless:**
```csharp
byte[] webpData = WebPImage.EncodeLossless(rgbaData, width, height);
```

**Advanced Options:**
```csharp
var options = new WebPEncodeOptions
{
    Preset = WebPPreset.PHOTO,
    Quality = 85.0f,
    Method = 6,
    SnsStrength = 75,
    FilterStrength = 80,
    AlphaCompression = 1
};
byte[] webpData = WebPImage.EncodeAdvanced(rgbaData, width, height, options);
```

### Decode

**Basic (RGBA):**
```csharp
byte[] webpData = File.ReadAllBytes("input.webp");
byte[] rgbaData = WebPImage.Decode(webpData);
```

**With Colorspace:**
```csharp
byte[] rgbData = WebPImage.Decode(webpData, WEBP_CSP_MODE.MODE_RGB);
byte[] bgraData = WebPImage.Decode(webpData, WEBP_CSP_MODE.MODE_BGRA);
byte[] argbData = WebPImage.Decode(webpData, WEBP_CSP_MODE.MODE_ARGB);
```

**From File/Stream:**
```csharp
byte[] data = WebPImage.Decode("input.webp");
using var stream = File.OpenRead("input.webp");
byte[] data = WebPImage.Decode(stream);
```

### Transform

**Scale:**
```csharp
byte[] scaled = WebPImage.Scale(webpData, targetWidth: 400, targetHeight: 300);
```

**Crop:**
```csharp
byte[] cropped = WebPImage.Crop(webpData, cropX: 100, cropY: 100, cropWidth: 400, cropHeight: 300);
```

**Flip:**
```csharp
byte[] flipped = WebPImage.Flip(webpData);
```

### Info

```csharp
WebPInfo info = WebPImage.GetInfo(webpData);
// info.Width, info.Height, info.HasAlpha, info.HasAnimation
```

### Validate

```csharp
bool isValid = WebPImage.IsValidWebP(data);
```

### Version

```csharp
string version = WebPImage.GetVersion();  // "1.6.0"
```

## Advanced Options

| Property | Type | Range | Description |
|----------|------|-------|-------------|
| Quality | float | 0-100 | Compression quality |
| Lossless | bool | - | Lossless mode |
| Method | int | 0-6 | Compression (0=fast, 6=best) |
| SnsStrength | int | 0-100 | Spatial noise shaping |
| FilterStrength | int | 0-100 | Filter strength |
| FilterSharpness | int | 0-7 | Filter sharpness |
| AlphaCompression | int | 0-2 | Alpha compression |
| AlphaQuality | int | 0-100 | Alpha quality |

## Presets

- **DEFAULT** - Default compression
- **PICTURE** - Digital pictures, portraits
- **PHOTO** - Outdoor photos, natural lighting
- **ICON** - Small colorful images
- **TEXT** - Text-like images

## Colorspaces

- **MODE_RGBA** - 4 bytes (R, G, B, A)
- **MODE_RGB** - 3 bytes (R, G, B)
- **MODE_BGRA** - 4 bytes (B, G, R, A)
- **MODE_BGR** - 3 bytes (B, G, R)
- **MODE_ARGB** - 4 bytes (A, R, G, B)

## Platforms

- Windows x64
- Linux x64
- Linux ARM64
