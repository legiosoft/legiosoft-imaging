using LegioSoft.Imaging.Core.Enums;
using LegioSoft.Imaging.Core.Interfaces;

namespace LegioSoft.Imaging.WebP.Tests;

public class LegioImageWebPProcessorTests
{
    [Fact]
    public void Processor_ShouldImplementILegioImageEncoder()
    {
        var processor = new LegioImageWebPProcessor();
        
        Assert.IsAssignableFrom<ILegioImageEncoder>(processor);
    }

    [Fact]
    public void Processor_ShouldImplementILegioImageDecoder()
    {
        var processor = new LegioImageWebPProcessor();
        
        Assert.IsAssignableFrom<ILegioImageDecoder>(processor);
    }

    [Fact]
    public void Encode_ShouldThrowForUnsupportedTargetFormat()
    {
        var processor = new LegioImageWebPProcessor();
        var testImageData = new byte[] { 0x89, 0x50, 0x4E, 0x47 };
        
        Assert.Throws<NotSupportedException>(() => { _ = processor.Encode(testImageData, LegioImageFormat.Jpeg, LegioEncodingQuality.High); });
        Assert.Throws<NotSupportedException>(() => { _ = processor.Encode(testImageData, LegioImageFormat.Png, LegioEncodingQuality.High); });
    }

    [Fact]
    public void Encode_ShouldThrowForNullOrEmptyImageData()
    {
        var processor = new LegioImageWebPProcessor();
        
        Assert.Throws<ArgumentException>(() => { _ = processor.Encode(null!, LegioImageFormat.WebP, LegioEncodingQuality.High); });
        Assert.Throws<ArgumentException>(() => { _ = processor.Encode(Array.Empty<byte>(), LegioImageFormat.WebP, LegioEncodingQuality.High); });
    }

    [Fact]
    public void Encode_Stream_ShouldThrowForUnreadableStream()
    {
        var processor = new LegioImageWebPProcessor();
        using var stream = new NonReadableMemoryStream();
        
        Assert.Throws<ArgumentException>(() => { _ = processor.Encode(stream, LegioImageFormat.WebP, LegioEncodingQuality.High); });
    }

    [Fact]
    public void Decode_ShouldThrowForNullOrEmptyImageData()
    {
        var processor = new LegioImageWebPProcessor();
        
        Assert.Throws<ArgumentException>(() => { _ = processor.Decode(null!, out _); });
        Assert.Throws<ArgumentException>(() => { _ = processor.Decode(Array.Empty<byte>(), out _); });
    }

    [Fact]
    public void GetImageInfo_ShouldThrowForNullOrEmptyImageData()
    {
        var processor = new LegioImageWebPProcessor();
        
        Assert.Throws<ArgumentException>(() => { _ = processor.GetImageInfo((byte[])null!); });
        Assert.Throws<ArgumentException>(() => { _ = processor.GetImageInfo(Array.Empty<byte>()); });
    }

    [Fact]
    public void GetImageInfo_Stream_ShouldThrowForUnreadableStream()
    {
        var processor = new LegioImageWebPProcessor();
        using var stream = new NonReadableMemoryStream();
        
        Assert.Throws<ArgumentException>(() => { _ = processor.GetImageInfo(stream); });
    }

    private sealed class NonReadableMemoryStream : MemoryStream
    {
        public override bool CanRead => false;
    }
}
