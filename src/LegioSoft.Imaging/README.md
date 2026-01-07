# LegioSoft.Imaging

Meta package providing unified API for image processing.

## Installation

```xml
<PackageReference Include="LegioSoft.Imaging" Version="1.0.0-beta1" />
```

Or install individually:
```xml
<PackageReference Include="LegioSoft.Imaging.Skia" Version="1.0.0-beta1" />
<PackageReference Include="LegioSoft.Imaging.WebP" Version="1.0.0-beta1" />
```

## Package Structure

- **LegioSoft.Imaging** - Meta package combining all packages
- **LegioSoft.Imaging.Skia** - SkiaSharp-based image operations
- **LegioSoft.Imaging.WebP** - Native WebP codec (lightweight)

## Usage

```csharp
using LegioSoft.Imaging.WebP;

// Encode WebP
var webpData = WebPHelper.Encode(bitmap, quality: 85);

// Decode WebP
var bitmap = WebPHelper.Decode(webpData);
```

## Status

**Early development** - API may change.
