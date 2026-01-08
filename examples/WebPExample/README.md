# LegioSoft.Imaging.WebP Example

This example demonstrates the **WebP-only package** for lightweight WebP encoding and decoding.

## Installation

```bash
dotnet add package LegioSoft.Imaging.WebP
```

Or via NuGet Package Manager:

```
Install-Package LegioSoft.Imaging.WebP
```

## Why Use LegioSoft.Imaging.WebP?

### Advantages
- **Lightweight**: ~500KB package size vs ~5MB for SkiaSharp version
- **Fast**: Native libwebp implementation for best performance
- **No SkiaSharp dependency**: Reduces bundle size
- **Cross-platform**: Supports Windows, Linux (x64, ARM64)
- **Lower memory usage**: No heavy graphics engine overhead

### Best Use Cases
- **Web-only applications**: Apps that only need WebP support
- **Server-side conversion**: CI/CD pipelines converting images to WebP
- **Mobile apps**: Limited device resources
- **Microservices**: Lightweight image processing services

### When to Use SkiaSharp Instead

Use **LegioSoft.Imaging.Skia** when you need:
- Multiple image formats (PNG, JPEG, BMP, GIF)
- Image transformations (resize, crop, rotate, flip)
- Filters (grayscale, sepia, blur, sharpen)
- Format conversion (JPEG → WebP, PNG → JPEG, etc.)

## Running the Example

```bash
dotnet run
```

## Package Comparison

| Feature | WebP Package | Skia Package |
|---------|---------------|--------------|
| Package Size | ~500KB | ~5MB |
| Dependencies | Core + libwebp | Core + SkiaSharp |
| Format Support | WebP only | PNG, JPEG, WebP, BMP, GIF |
| Transformations | ❌ | ✅ |
| Filters | ❌ | ✅ |
| Resize | ❌ | ✅ |
| Crop | ❌ | ✅ |
| Builder API | ❌ | ✅ |

## Current Implementation

The `LegioImageWebPEncoder` class currently implements:

```csharp
public class LegioImageWebPEncoder : ILegioImageEncoder, ILegioImageDecoder
{
    // Throws NotImplementedException (future feature)
    public byte[] Encode(byte[] imageData, LegioImageFormat format, LegioEncodingQuality quality);
    public Stream Encode(Stream inputStream, LegioImageFormat format, LegioEncodingQuality quality, Stream? outputStream);
    
    // Throws NotImplementedException (future feature)
    public byte[] Decode(byte[] imageData, out LegioImageFormat format);
    
    // Throws NotImplementedException (future feature)
    public LegioImageInfo GetImageInfo(byte[] imageData);
    public LegioImageInfo GetImageInfo(Stream inputStream);
}
```

### Expected Behavior

**Encode:**
```csharp
var encoder = new LegioImageWebPEncoder();

// Throws NotSupportedException (WebP encoding not implemented yet)
var webpData = encoder.Encode(imageData, LegioImageFormat.WebP, LegioEncodingQuality.High);
```

**Decode:**
```csharp
var encoder = new LegioImageWebPEncoder();

// Throws NotImplementedException (decoding not implemented yet)
var decodedData = encoder.Decode(webpData, out var format);
```

**Get Image Info:**
```csharp
var encoder = new LegioImageWebPEncoder();

// Throws NotImplementedException (info extraction not implemented yet)
var info = encoder.GetImageInfo(webpData);
```

## Future Enhancements

The WebP package will support:

1. **Encoding**: Convert RGBA/BGR data to WebP format
   - Lossless encoding
   - Lossy encoding with quality control
   - RGBA and BGR formats
   - Alpha channel support

2. **Decoding**: Convert WebP to RGBA format
   - Lossless and lossy WebP support
   - Extract image dimensions
   - Extract image information (width, height, format)

3. **Advanced Features**:
   - Lossless vs lossy encoding options
   - Quality parameter (0-100)
   - Alpha channel control
   - Native library integration

## Example Scenarios

### Scenario 1: Convert PNG to WebP (Future)
```csharp
// When implemented, you'll be able to:
var encoder = new LegioImageWebPEncoder();
var webpData = encoder.Encode(pngData, LegioImageFormat.WebP, LegioEncodingQuality.High);
```

### Scenario 2: Decode WebP to RGBA (Future)
```csharp
// When implemented, you'll be able to:
var encoder = new LegioImageWebPEncoder();
var rgbaData = encoder.Decode(webpData, out var format);
```

### Scenario 3: WebP Information Extraction (Future)
```csharp
// When implemented, you'll be able to:
var encoder = new LegioImageWebPEncoder();
var info = encoder.GetImageInfo(webpData);
Console.WriteLine($"Width: {info.Width}, Height: {info.Height}");
```

## Migration from SkiaSharp

If you're currently using SkiaSharp but only need WebP:

**Current (SkiaSharp):**
```csharp
using SkiaSharp;
using (var bitmap = SKBitmap.Decode("image.jpg"))
using (var data = bitmap.Encode(SKEncodedImageFormat.Webp, 80))
{
    File.WriteAllBytes("output.webp", data.ToArray());
}
```

**Future (WebP Package):**
```csharp
// Will be (when implemented):
var encoder = new LegioImageWebPEncoder();
var webpData = encoder.Encode(imageData, LegioImageFormat.WebP, LegioEncodingQuality.High);
File.WriteAllBytes("output.webp", webpData);
```

### Benefits of Migration
- **Smaller bundle**: Remove ~4.5MB SkiaSharp dependency
- **Faster startup**: No heavy graphics engine initialization
- **Lower memory**: ~10x less memory usage
- **Simpler deployment**: Fewer native libraries

## Testing the Package

Since encoding/decoding is not implemented yet, you can test the interface compliance:

```csharp
using LegioSoft.Imaging.Core;
using LegioSoft.Imaging.WebP;

var encoder = new LegioImageWebPEncoder();

// Verify interface implementations
Assert.True(encoder is ILegioImageEncoder);
Assert.True(encoder is ILegioImageDecoder);

// Verify throws appropriate exceptions
Assert.Throws<NotSupportedException>(() => 
    encoder.Encode(new byte[0], LegioImageFormat.Jpeg, LegioEncodingQuality.High));

Assert.Throws<NotImplementedException>(() => 
    encoder.Decode(new byte[0], out var format));

Assert.Throws<NotImplementedException>(() => 
    encoder.GetImageInfo(new byte[0]));
```

## Adding WebP to Existing Project

If you already have SkiaSharp and want to add WebP support:

```xml
<ItemGroup>
  <PackageReference Include="LegioSoft.Imaging.Core" />
  <PackageReference Include="LegioSoft.Imaging.WebP" />
  <PackageReference Include="LegioSoft.Imaging.Skia" />
</ItemGroup>
```

Then you can choose the appropriate encoder:

```csharp
// Use WebP encoder for WebP operations
var webPEncoder = new LegioImageWebPEncoder();
var webpData = webPEncoder.Encode(imageData, LegioImageFormat.WebP, quality: 85);

// Use Skia for other operations
var builder = LegioImageBuilder.Load(imageData);
var resized = builder.Resize(800, 600).SaveAs(LegioImageFormat.Jpeg);
```

## See Also

- [WEBP_MANUAL.md](../../docs/WEBP_MANUAL.md) - Complete WebP guide
- [INSTALLATION.md](../../docs/INSTALLATION.md) - Installation instructions
- [SKIA_MANUAL.md](../../docs/SKIA_MANUAL.md) - SkiaSharp guide
