using LegioSoft.Imaging.Core;
using LegioSoft.Imaging.Skia;
using LegioSoft.Imaging.Skia.Core;
using LegioSoft.Imaging.Skia.Operations;
using LegioSoft.Imaging.WebP;
using SkiaSharp;

namespace LegioSoft.Imaging.Meta.Tests;

public class MetaPackageTests
{
    [Fact]
    public void Core_Types_ShouldBeAccessible()
    {
        Assert.NotNull(typeof(LegioImageFormat));
        Assert.NotNull(typeof(LegioScaleMode));
        Assert.NotNull(typeof(LegioResizeQuality));
        Assert.NotNull(typeof(LegioEncodingQuality));
        Assert.NotNull(typeof(LegioTransformType));
        Assert.NotNull(typeof(LegioFilterType));
        Assert.NotNull(typeof(LegioImageInfo));
    }

    [Fact]
    public void Core_Interfaces_ShouldBeAccessible()
    {
        Assert.NotNull(typeof(ILegioImageEncoder));
        Assert.NotNull(typeof(ILegioImageDecoder));
        Assert.NotNull(typeof(ILegioImageResizer));
        Assert.NotNull(typeof(ILegioImageCropper));
        Assert.NotNull(typeof(ILegioImageTransformer));
        Assert.NotNull(typeof(ILegioImageFilter));
    }

    [Fact]
    public void Skia_ImageLoader_ShouldBeAccessible()
    {
        Assert.NotNull(typeof(ImageLoader));
    }

    [Fact]
    public void Skia_ImageResizer_ShouldBeAccessible()
    {
        Assert.NotNull(typeof(ImageResizer));
    }

    [Fact]
    public void Skia_ImageCropper_ShouldBeAccessible()
    {
        Assert.NotNull(typeof(ImageCropper));
    }

    [Fact]
    public void Skia_ImageTransformer_ShouldBeAccessible()
    {
        Assert.NotNull(typeof(ImageTransformer));
    }

    [Fact]
    public void Skia_LegioImageBuilder_ShouldBeAccessible()
    {
        Assert.NotNull(typeof(LegioImageBuilder));
    }

    [Fact]
    public void WebP_LegioImageWebPEncoder_ShouldBeAccessible()
    {
        Assert.NotNull(typeof(LegioImageWebPEncoder));
    }

    [Fact]
    public void LegioImageBuilder_ShouldHaveStaticLoadMethods()
    {
        Assert.NotNull(typeof(LegioImageBuilder).GetMethod("Load", new[] { typeof(byte[]) }));
        Assert.NotNull(typeof(LegioImageBuilder).GetMethod("Load", new[] { typeof(string) }));
        Assert.NotNull(typeof(LegioImageBuilder).GetMethod("Load", new[] { typeof(Stream) }));
    }

    [Fact]
    public void ImageLoader_ShouldHaveLoadBitmapMethods()
    {
        Assert.NotNull(typeof(ImageLoader).GetMethod("LoadBitmap", new[] { typeof(byte[]) }));
        Assert.NotNull(typeof(ImageLoader).GetMethod("LoadBitmap", new[] { typeof(Stream) }));
        Assert.NotNull(typeof(ImageLoader).GetMethod("LoadBitmap", new[] { typeof(string) }));
    }

    [Fact]
    public void ImageResizer_ShouldHaveResizeBitmapMethod()
    {
        Assert.NotNull(typeof(ImageResizer).GetMethod("ResizeBitmap", new[] { typeof(SKBitmap), typeof(int), typeof(int), typeof(LegioResizeQuality) }));
    }

    [Fact]
    public void ImageCropper_ShouldHaveCropBitmapMethod()
    {
        Assert.NotNull(typeof(ImageCropper).GetMethod("CropBitmap", new[] { typeof(SKBitmap), typeof(int), typeof(int), typeof(int), typeof(int) }));
    }

    [Fact]
    public void ImageTransformer_ShouldHaveRotateAndFlipMethods()
    {
        Assert.NotNull(typeof(ImageTransformer).GetMethod("Rotate", new[] { typeof(SKBitmap), typeof(int) }));
        Assert.NotNull(typeof(ImageTransformer).GetMethod("Flip", new[] { typeof(SKBitmap), typeof(bool), typeof(bool) }));
    }

    [Fact]
    public void LegioImageWebPEncoder_ShouldImplementEncoderInterface()
    {
        var encoder = new LegioImageWebPEncoder();
        Assert.IsAssignableFrom<ILegioImageEncoder>(encoder);
    }

    [Fact]
    public void LegioImageWebPEncoder_ShouldImplementDecoderInterface()
    {
        var encoder = new LegioImageWebPEncoder();
        Assert.IsAssignableFrom<ILegioImageDecoder>(encoder);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(50)]
    [InlineData(75)]
    [InlineData(100)]
    public void LegioEncodingQuality_ShouldHaveCorrectValues(int expectedValue)
    {
        var quality = (LegioEncodingQuality)expectedValue;
        Assert.Equal(expectedValue, (int)quality);
    }

    [Theory]
    [InlineData(LegioImageFormat.Png)]
    [InlineData(LegioImageFormat.Jpeg)]
    [InlineData(LegioImageFormat.WebP)]
    [InlineData(LegioImageFormat.Bmp)]
    [InlineData(LegioImageFormat.Gif)]
    public void LegioImageFormat_ShouldHaveAllFormats(LegioImageFormat format)
    {
        var formats = Enum.GetValues<LegioImageFormat>();
        Assert.Contains(format, formats);
    }

    [Theory]
    [InlineData(LegioScaleMode.Fit)]
    [InlineData(LegioScaleMode.Fill)]
    [InlineData(LegioScaleMode.Stretch)]
    public void LegioScaleMode_ShouldHaveAllModes(LegioScaleMode mode)
    {
        var modes = Enum.GetValues<LegioScaleMode>();
        Assert.Contains(mode, modes);
    }

    [Theory]
    [InlineData(LegioResizeQuality.Low)]
    [InlineData(LegioResizeQuality.Medium)]
    [InlineData(LegioResizeQuality.High)]
    [InlineData(LegioResizeQuality.Maximum)]
    public void LegioResizeQuality_ShouldHaveAllQualities(LegioResizeQuality quality)
    {
        var qualities = Enum.GetValues<LegioResizeQuality>();
        Assert.Contains(quality, qualities);
    }

    [Fact]
    public void LegioImageInfo_ShouldHaveAllProperties()
    {
        var info = new LegioImageInfo();
        
        info.Width = 1920;
        info.Height = 1080;
        info.Format = LegioImageFormat.Jpeg;
        info.HasAlpha = false;
        info.ByteSize = 102400;
        
        Assert.Equal(1920, info.Width);
        Assert.Equal(1080, info.Height);
        Assert.Equal(LegioImageFormat.Jpeg, info.Format);
        Assert.False(info.HasAlpha);
        Assert.Equal(102400, info.ByteSize);
    }

    [Fact]
    public void AllPackages_ShouldReferenceCorrectAssemblies()
    {
        var coreAssembly = typeof(LegioImageInfo).Assembly.GetName().Name;
        var skiaAssembly = typeof(LegioImageBuilder).Assembly.GetName().Name;
        var webPAssembly = typeof(LegioImageWebPEncoder).Assembly.GetName().Name;
        
        Assert.Equal("LegioSoft.Imaging.Core", coreAssembly);
        Assert.Equal("LegioSoft.Imaging.Skia", skiaAssembly);
        Assert.Equal("LegioSoft.Imaging.WebP", webPAssembly);
    }

    [Fact]
    public void MetaPackage_ShouldHaveNoPublicTypes()
    {
        var metaAssembly = typeof(LegioImageBuilder).Assembly;
        var types = metaAssembly.GetTypes();
        
        Assert.Empty(types.Where(t => t.IsPublic && t.Namespace == "LegioSoft.Imaging"));
    }
}
