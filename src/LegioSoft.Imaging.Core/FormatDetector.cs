namespace LegioSoft.Imaging.Core;

public static class FormatDetector
{
    public static LegioImageFormat DetectFormat(byte[] imageData)
    {
        if (imageData == null)
            throw new ArgumentNullException(nameof(imageData));
        
        if (imageData.Length == 0)
            throw new ArgumentException("Image data cannot be empty", nameof(imageData));
        
        if (imageData.Length < 12)
            throw new ArgumentException("Image data is too short to determine format", nameof(imageData));

        return DetectFormatInternal(imageData);
    }

    private static LegioImageFormat DetectFormatInternal(byte[] imageData)
    {
        var format = IdentifyFormatBySignature(imageData);
        ValidateImageData(imageData, format);
        return format;
    }

    private static LegioImageFormat IdentifyFormatBySignature(byte[] imageData)
    {
        if (IsWebP(imageData))
            return LegioImageFormat.WebP;
        
        if (IsJpeg(imageData))
            return LegioImageFormat.Jpeg;
        
        if (IsBmp(imageData))
            return LegioImageFormat.Bmp;
        
        if (IsGif(imageData))
            return LegioImageFormat.Gif;
        
        if (IsPng(imageData))
            return LegioImageFormat.Png;
        
        throw new NotSupportedException("Unsupported or unrecognized image format. Supported formats: PNG, JPEG, WebP, BMP, GIF");
    }

    private static bool IsWebP(byte[] imageData)
    {
        if (imageData.Length < 12) return false;
        
        return imageData[0] == 0x52 && imageData[1] == 0x49 && imageData[2] == 0x46 && imageData[3] == 0x46 &&
               imageData[8] == 0x57 && imageData[9] == 0x45 && imageData[10] == 0x66 && imageData[11] == 0x50;
    }

    private static bool IsJpeg(byte[] imageData)
    {
        if (imageData.Length < 3) return false;
        
        return imageData[0] == 0xFF && imageData[1] == 0xD8 && imageData[2] == 0xFF;
    }

    private static bool IsBmp(byte[] imageData)
    {
        if (imageData.Length < 2) return false;
        
        return imageData[0] == 0x42 && imageData[1] == 0x4D;
    }

    private static bool IsGif(byte[] imageData)
    {
        if (imageData.Length < 6) return false;
        
        return imageData[0] == 0x47 && imageData[1] == 0x49 && imageData[2] == 0x46 && 
               imageData[3] == 0x38 && 
               (imageData[4] == 0x37 || imageData[4] == 0x39) &&
               (imageData[5] == 0x61 || imageData[5] == 0x62);
    }

    private static bool IsPng(byte[] imageData)
    {
        if (imageData.Length < 8) return false;
        
        return imageData[0] == 0x89 && imageData[1] == 0x50 && imageData[2] == 0x4E && imageData[3] == 0x47 &&
               imageData[4] == 0x0D && imageData[5] == 0x0A && imageData[6] == 0x1A && imageData[7] == 0x0A;
    }

    private static void ValidateImageData(byte[] imageData, LegioImageFormat format)
    {
        switch (format)
        {
            case LegioImageFormat.Jpeg:
                if (imageData.Length < 4)
                    throw new ArgumentException("Invalid JPEG file: file too short", nameof(imageData));
                
                var jpegEnd = imageData[imageData.Length - 2] == 0xFF && imageData[imageData.Length - 1] == 0xD9;
                if (!jpegEnd)
                    throw new ArgumentException("Invalid JPEG file: missing end marker (FF D9)", nameof(imageData));
                break;
            
            case LegioImageFormat.Png:
                if (imageData.Length < 8)
                    throw new ArgumentException("Invalid PNG file: file too short", nameof(imageData));
                break;
            
            case LegioImageFormat.Gif:
                if (imageData.Length < 6)
                    throw new ArgumentException("Invalid GIF file: file too short", nameof(imageData));
                break;
            
            case LegioImageFormat.Bmp:
                if (imageData.Length < 54)
                    throw new ArgumentException("Invalid BMP file: header must be at least 54 bytes", nameof(imageData));
                break;
            
            case LegioImageFormat.WebP:
                if (imageData.Length < 20)
                    throw new ArgumentException("Invalid WebP file: header too short", nameof(imageData));
                break;
        }
    }
}
