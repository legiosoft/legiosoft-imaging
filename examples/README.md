# LegioSoft.Imaging Examples

This folder contains console applications demonstrating how to use LegioSoft.Imaging packages.

## Examples

### SkiaSharp Example (`skiaExample/`)

Full-featured image processing with SkiaSharp:
- Resize, crop, rotate, flip
- Filters: grayscale, sepia, blur, sharpen
- Format conversion
- Quality control
- Chained operations

**Run:**
```bash
cd skiaExample
dotnet run
```

**Documentation:** See [skiaExample/README.md](skiaExample/README.md)

### WebP Example (`webPExample/`)

Lightweight WebP encoding and decoding:
- Interface implementation
- Performance comparison
- Package comparison
- Installation guide

**Run:**
```bash
cd webPExample
dotnet run
```

**Documentation:** See [webPExample/README.md](webPExample/README.md)

## Documentation

For detailed usage guides, see the `docs/` folder:

- [SKIA_MANUAL.md](../docs/SKIA_MANUAL.md) - Complete SkiaSharp guide
- [WEBP_MANUAL.md](../docs/WEBP_MANUAL.md) - Complete WebP guide
- [INSTALLATION.md](../docs/INSTALLATION.md) - Installation instructions

## Installation

### Install Lightweight (WebP Only)

```bash
dotnet add package LegioSoft.Imaging.Core
dotnet add package LegioSoft.Imaging.WebP
```

### Install Full Processing (SkiaSharp)

```bash
dotnet add package LegioSoft.Imaging.Core
dotnet add package LegioSoft.Imaging.Skia
```

## Requirements

- .NET 6.0 or later
- Or compatible: .NET Standard 2.1+
