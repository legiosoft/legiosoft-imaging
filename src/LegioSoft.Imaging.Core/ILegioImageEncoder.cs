using System.IO;

namespace LegioSoft.Imaging.Core;

public interface ILegioImageEncoder
{
    byte[] Encode(byte[] imageData, LegioImageFormat format, LegioEncodingQuality quality = LegioEncodingQuality.High);
    Stream Encode(Stream inputStream, LegioImageFormat format, LegioEncodingQuality quality = LegioEncodingQuality.High, Stream? outputStream = null);
}

public interface ILegioImageDecoder
{
    byte[] Decode(byte[] imageData, out LegioImageFormat format);
    LegioImageInfo GetImageInfo(byte[] imageData);
    LegioImageInfo GetImageInfo(Stream inputStream);
}
