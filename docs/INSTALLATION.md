# Installation

## Packages

### LegioSoft.Imaging.WebP

```bash
dotnet add package LegioSoft.Imaging.Core
dotnet add package LegioSoft.Imaging.WebP
```

WebP encoding/decoding using native libwebp 1.6.0. ~500KB.

### LegioSoft.Imaging.Skia

```bash
dotnet add package LegioSoft.Imaging.Skia
```

Full image processing with SkiaSharp 2.88.6. ~565KB.

### LegioSoft.Imaging.Core

```bash
dotnet add package LegioSoft.Imaging.Core
```

Core interfaces and types. ~20KB.

## Platform Support

| Platform | .NET 6.0+ |
|----------|-----------|
| Windows x64 | ✅ |
| Windows ARM64 | ✅ |
| Linux x64 | ✅ |
| Linux ARM64 | ✅ |
| macOS x64 | ✅ |
| macOS ARM64 | ✅ |

## Choosing a Package

| Need | Package |
|------|---------|
| WebP only | LegioSoft.Imaging.WebP |
| Full processing | LegioSoft.Imaging.Skia |
| Core types | LegioSoft.Imaging.Core |
