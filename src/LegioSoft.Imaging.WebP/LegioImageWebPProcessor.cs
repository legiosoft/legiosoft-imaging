using LegioSoft.Imaging.Core.Classes;
using LegioSoft.Imaging.Core.Enums;
using LegioSoft.Imaging.Core.Interfaces;
using LegioSoft.Imaging.WebP.Decoder;
using LegioSoft.Imaging.WebP.Encoder;
using LegioSoft.Imaging.WebP.Enums;

namespace LegioSoft.Imaging.WebP;

public class LegioImageWebPProcessor : ILegioImageEncoder, ILegioImageDecoder, ILegioImageResizer, ILegioImageCropper, ILegioImageTransformer
{
    private const int DefaultWidth = 100;
    private const int DefaultHeight = 100;

    public byte[] Encode(byte[] imageData, LegioImageFormat format, LegioEncodingQuality quality = LegioEncodingQuality.High)
    {
        if (imageData == null || imageData.Length == 0)
        {
            throw new ArgumentException("Image data cannot be null or empty", nameof(imageData));
        }

        if (format != LegioImageFormat.WebP)
        {
            throw new NotSupportedException($"WebP encoder only supports WebP format. Requested format: {format}");
        }

        var pixelCount = imageData.Length % 4 == 0 ? imageData.Length / 4 : imageData.Length / 3;
        var size = (int)Math.Sqrt(pixelCount);
        var width = size;
        var height = imageData.Length / (size * (imageData.Length % 4 == 0 ? 4 : 3));

        float qualityValue = quality switch
        {
            LegioEncodingQuality.Low => 0f,
            LegioEncodingQuality.Medium => 50f,
            LegioEncodingQuality.High => 75f,
            LegioEncodingQuality.Maximum => 100f,
            _ => 75f
        };

        if (imageData.Length % 4 == 0)
        {
            return WebPEncoder.Encode(imageData, width, height, qualityValue, false);
        }
        else
        {
            return WebPEncoder.EncodeRGB(imageData, width, height, qualityValue, false);
        }
    }

    public Stream Encode(Stream inputStream, LegioImageFormat format, LegioEncodingQuality quality = LegioEncodingQuality.High, Stream? outputStream = null)
    {
        if (inputStream == null)
        {
            throw new ArgumentNullException(nameof(inputStream));
        }

        if (!inputStream.CanRead)
        {
            throw new ArgumentException("Input stream must be readable", nameof(inputStream));
        }

        var imageData = ReadStream(inputStream);
        var encodedData = Encode(imageData, format, quality);

        var result = outputStream ?? new MemoryStream();
        result.Write(encodedData, 0, encodedData.Length);
        result.Position = 0;
        return result;
    }

    public byte[] Decode(byte[] imageData, out LegioImageFormat format)
    {
        if (imageData == null || imageData.Length == 0)
        {
            throw new ArgumentException("Image data cannot be null or empty", nameof(imageData));
        }

        format = LegioImageFormat.WebP;
        return WebPDecoder.Decode(imageData, out _, out _, WEBP_CSP_MODE.MODE_RGBA);
    }

    public LegioImageInfo GetImageInfo(byte[] imageData)
    {
        if (imageData == null || imageData.Length == 0)
        {
            throw new ArgumentException("Image data cannot be null or empty", nameof(imageData));
        }

        var webPInfo = WebPDecoder.GetInfo(imageData);

        return new LegioImageInfo
        {
            Width = webPInfo.Width,
            Height = webPInfo.Height,
            Format = LegioImageFormat.WebP,
            HasAlpha = webPInfo.HasAlpha,
            ByteSize = imageData.Length
        };
    }

    public LegioImageInfo GetImageInfo(Stream inputStream)
    {
        if (inputStream == null)
        {
            throw new ArgumentNullException(nameof(inputStream));
        }

        if (!inputStream.CanRead)
        {
            throw new ArgumentException("Input stream must be readable", nameof(inputStream));
        }

        var imageData = ReadStream(inputStream);
        return GetImageInfo(imageData);
    }

    public byte[] Resize(byte[] imageData, int width, int height, LegioScaleMode mode, LegioResizeQuality quality = LegioResizeQuality.High)
    {
        if (imageData == null || imageData.Length == 0)
        {
            throw new ArgumentException("Image data cannot be null or empty", nameof(imageData));
        }

        if (width <= 0 || height <= 0)
        {
            throw new ArgumentException("Width and height must be positive", nameof(width));
        }

        return WebPDecoder.DecodeWithScaling(imageData, width, height, WEBP_CSP_MODE.MODE_RGBA);
    }

    public Stream Resize(Stream inputStream, int width, int height, LegioScaleMode mode, LegioResizeQuality quality = LegioResizeQuality.High, Stream? outputStream = null)
    {
        if (inputStream == null)
        {
            throw new ArgumentNullException(nameof(inputStream));
        }

        if (!inputStream.CanRead)
        {
            throw new ArgumentException("Input stream must be readable", nameof(inputStream));
        }

        var imageData = ReadStream(inputStream);
        var resizedData = Resize(imageData, width, height, mode, quality);

        var result = outputStream ?? new MemoryStream();
        result.Write(resizedData, 0, resizedData.Length);
        result.Position = 0;
        return result;
    }

    public byte[] Crop(byte[] imageData, int x, int y, int width, int height)
    {
        if (imageData == null || imageData.Length == 0)
        {
            throw new ArgumentException("Image data cannot be null or empty", nameof(imageData));
        }

        if (x < 0 || y < 0 || width <= 0 || height <= 0)
        {
            throw new ArgumentException("Crop coordinates and dimensions must be positive", nameof(x));
        }

        return WebPDecoder.DecodeWithCropping(imageData, x, y, width, height, WEBP_CSP_MODE.MODE_RGBA);
    }

    public Stream Crop(Stream inputStream, int x, int y, int width, int height, Stream? outputStream = null)
    {
        if (inputStream == null)
        {
            throw new ArgumentNullException(nameof(inputStream));
        }

        if (!inputStream.CanRead)
        {
            throw new ArgumentException("Input stream must be readable", nameof(inputStream));
        }

        var imageData = ReadStream(inputStream);
        var croppedData = Crop(imageData, x, y, width, height);

        var result = outputStream ?? new MemoryStream();
        result.Write(croppedData, 0, croppedData.Length);
        result.Position = 0;
        return result;
    }

    public byte[] Rotate(byte[] imageData, int degrees)
    {
        if (imageData == null || imageData.Length == 0)
        {
            throw new ArgumentException("Image data cannot be null or empty", nameof(imageData));
        }

        if (degrees % 90 != 0)
        {
            throw new ArgumentException("Rotation must be in 90-degree increments", nameof(degrees));
        }

        var info = WebPDecoder.GetInfo(imageData);
        var rotatedWidth = degrees % 180 == 0 ? info.Width : info.Height;
        var rotatedHeight = degrees % 180 == 0 ? info.Height : info.Width;

        return WebPDecoder.DecodeWithScaling(imageData, rotatedWidth, rotatedHeight, WEBP_CSP_MODE.MODE_RGBA);
    }

    public Stream Rotate(Stream inputStream, int degrees, Stream? outputStream = null)
    {
        if (inputStream == null)
        {
            throw new ArgumentNullException(nameof(inputStream));
        }

        if (!inputStream.CanRead)
        {
            throw new ArgumentException("Input stream must be readable", nameof(inputStream));
        }

        var imageData = ReadStream(inputStream);
        var rotatedData = Rotate(imageData, degrees);

        var result = outputStream ?? new MemoryStream();
        result.Write(rotatedData, 0, rotatedData.Length);
        result.Position = 0;
        return result;
    }

    public byte[] Flip(byte[] imageData, bool horizontal, bool vertical)
    {
        if (imageData == null || imageData.Length == 0)
        {
            throw new ArgumentException("Image data cannot be null or empty", nameof(imageData));
        }

        if (horizontal && !vertical)
        {
            throw new NotSupportedException("Horizontal flip is not supported by WebP decoder. Use vertical flip only.");
        }

        return WebPDecoder.DecodeWithFlip(imageData, WEBP_CSP_MODE.MODE_RGBA);
    }

    public Stream Flip(Stream inputStream, bool horizontal, bool vertical, Stream? outputStream = null)
    {
        if (inputStream == null)
        {
            throw new ArgumentNullException(nameof(inputStream));
        }

        if (!inputStream.CanRead)
        {
            throw new ArgumentException("Input stream must be readable", nameof(inputStream));
        }

        var imageData = ReadStream(inputStream);
        var flippedData = Flip(imageData, horizontal, vertical);

        var result = outputStream ?? new MemoryStream();
        result.Write(flippedData, 0, flippedData.Length);
        result.Position = 0;
        return result;
    }

    private static byte[] ReadStream(Stream stream)
    {
        if (stream == null)
        {
            throw new ArgumentNullException(nameof(stream));
        }

        if (!stream.CanRead)
        {
            throw new ArgumentException("Stream must be readable", nameof(stream));
        }

        long originalPosition = 0;
        bool canSeek = false;

        if (stream.CanSeek)
        {
            try
            {
                originalPosition = stream.Position;
                stream.Seek(0, SeekOrigin.Begin);
                canSeek = true;
            }
            catch (IOException)
            {
                canSeek = false;
            }
        }

        try
        {
            using var memoryStream = new MemoryStream();
            stream.CopyTo(memoryStream);
            return memoryStream.ToArray();
        }
        finally
        {
            if (canSeek && stream.CanSeek)
            {
                try
                {
                    stream.Seek(originalPosition, SeekOrigin.Begin);
                }
                catch (IOException)
                {
                }
            }
        }
    }
}
