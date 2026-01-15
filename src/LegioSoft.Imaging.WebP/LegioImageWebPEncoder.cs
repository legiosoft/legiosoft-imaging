using LegioSoft.Imaging.Core.Classes;
using LegioSoft.Imaging.Core.Enums;
using LegioSoft.Imaging.Core.Interfaces;

namespace LegioSoft.Imaging.WebP;

public class LegioImageWebPEncoder : ILegioImageEncoder, ILegioImageDecoder
{
    public byte[] Encode(byte[] imageData, LegioImageFormat format, LegioEncodingQuality quality = LegioEncodingQuality.High)
    {
        if (format != LegioImageFormat.WebP)
        {
            throw new NotSupportedException("WebP encoder only supports WebP format");
        }
        
        throw new NotImplementedException("WebP encoding implementation will be added in WebPDecoder.cs and WebPEncoder.cs");
    }

    public Stream Encode(Stream inputStream, LegioImageFormat format, LegioEncodingQuality quality = LegioEncodingQuality.High, Stream? outputStream = null)
    {
        if (format != LegioImageFormat.WebP)
            throw new NotSupportedException("WebP encoder only supports WebP format");
        
        throw new NotImplementedException("WebP encoding implementation will be added in WebPDecoder.cs and WebPEncoder.cs");
    }

    public byte[] Decode(byte[] imageData, out LegioImageFormat format)
    {
        throw new NotImplementedException("WebP decoding implementation will be added in WebPDecoder.cs and WebPEncoder.cs");
    }

    public LegioImageInfo GetImageInfo(byte[] imageData)
    {
        throw new NotImplementedException("WebP image info extraction will be added in WebPDecoder.cs and WebPEncoder.cs");
    }

    public LegioImageInfo GetImageInfo(Stream inputStream)
    {
        throw new NotImplementedException("WebP image info extraction will be added in WebPDecoder.cs and WebPEncoder.cs");
    }
}
