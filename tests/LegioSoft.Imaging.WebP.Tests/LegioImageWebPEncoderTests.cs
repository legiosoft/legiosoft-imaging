using LegioSoft.Imaging.Core.Enums;
using LegioSoft.Imaging.Core.Interfaces;

namespace LegioSoft.Imaging.WebP.Tests;

public class LegioImageWebPEncoderTests
{
    [Fact]
    public void LegioImageWebPEncoder_ShouldImplementILegioImageEncoder()
    {
        var encoder = new LegioImageWebPEncoder();
        
        Assert.IsAssignableFrom<ILegioImageEncoder>(encoder);
    }

    [Fact]
    public void LegioImageWebPEncoder_ShouldImplementILegioImageDecoder()
    {
        var encoder = new LegioImageWebPEncoder();
        
        Assert.IsAssignableFrom<ILegioImageDecoder>(encoder);
    }

    [Fact]
    public void Encode_ShouldThrowOnWebPFormat()
    {
        var encoder = new LegioImageWebPEncoder();
        var testPngData = new byte[] { 0x89, 0x50, 0x4E, 0x47 };
        
        Assert.Throws<NotImplementedException>(() => encoder.Encode(testPngData, LegioImageFormat.WebP, LegioEncodingQuality.High));
    }

    [Fact]
    public void Encode_ShouldThrowOnNonWebPFormat()
    {
        var encoder = new LegioImageWebPEncoder();
        var testPngData = new byte[] { 0x89, 0x50, 0x4E, 0x47 };
        
        Assert.Throws<NotSupportedException>(() => encoder.Encode(testPngData, LegioImageFormat.Jpeg, LegioEncodingQuality.High));
        Assert.Throws<NotSupportedException>(() => encoder.Encode(testPngData, LegioImageFormat.Png, LegioEncodingQuality.High));
    }

    [Fact]
    public void Encode_WithStream_ShouldThrowOnWebPFormat()
    {
        var encoder = new LegioImageWebPEncoder();
        var testPngData = new byte[] { 0x89, 0x50, 0x4E, 0x47 };
        using var stream = new MemoryStream(testPngData);
        
        Assert.Throws<NotImplementedException>(() => encoder.Encode(stream, LegioImageFormat.WebP, LegioEncodingQuality.High));
    }

    [Fact]
    public void Decode_ShouldThrowNotImplemented()
    {
        var encoder = new LegioImageWebPEncoder();
        var webpData = new byte[] { 0x52, 0x49, 0x46, 0x46, 0x10, 0x00, 0x00, 0x00, 0x57, 0x45, 0x42, 0x50 };
        
        Assert.Throws<NotImplementedException>(() => encoder.Decode(webpData, out var format));
    }

    [Fact]
    public void GetImageInfo_ShouldThrowNotImplemented()
    {
        var encoder = new LegioImageWebPEncoder();
        var webpData = new byte[] { 0x52, 0x49, 0x46, 0x46, 0x10, 0x00, 0x00, 0x00, 0x57, 0x45, 0x42, 0x50 };
        
        Assert.Throws<NotImplementedException>(() => encoder.GetImageInfo(webpData));
    }

    [Fact]
    public void GetImageInfo_WithStream_ShouldThrowNotImplemented()
    {
        var encoder = new LegioImageWebPEncoder();
        var webpData = new byte[] { 0x52, 0x49, 0x46, 0x46, 0x10, 0x00, 0x00, 0x00, 0x57, 0x45, 0x42, 0x50 };
        using var stream = new MemoryStream(webpData);
        
        Assert.Throws<NotImplementedException>(() => encoder.GetImageInfo(stream));
    }
}
