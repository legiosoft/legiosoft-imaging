using LegioSoft.Imaging.Core.Classes;
using LegioSoft.Imaging.Core.Enums;

// ReSharper disable IdentifierTypo

namespace LegioSoft.Imaging.Core.Tests;

public class LegioImageFormatTests
{
    [Fact]
    public void LegioImageFormat_ShouldHaveAllFormats()
    {
        var formats = Enum.GetValues<LegioImageFormat>();

        Assert.Equal(5, formats.Length);
        Assert.Contains(LegioImageFormat.Png, formats);
        Assert.Contains(LegioImageFormat.Jpeg, formats);
        Assert.Contains(LegioImageFormat.WebP, formats);
        Assert.Contains(LegioImageFormat.Bmp, formats);
        Assert.Contains(LegioImageFormat.Gif, formats);
    }
}

public class LegioScaleModeTests
{
    [Fact]
    public void LegioScaleMode_ShouldHaveAllModes()
    {
        var modes = Enum.GetValues<LegioScaleMode>();

        Assert.Equal(3, modes.Length);
        Assert.Contains(LegioScaleMode.Fit, modes);
        Assert.Contains(LegioScaleMode.Fill, modes);
        Assert.Contains(LegioScaleMode.Stretch, modes);
    }
}

public class LegioResizeQualityTests
{
    [Fact]
    public void LegioResizeQuality_ShouldHaveAllQualities()
    {
        var qualities = Enum.GetValues<LegioResizeQuality>();

        Assert.Equal(4, qualities.Length);
        Assert.Contains(LegioResizeQuality.Low, qualities);
        Assert.Contains(LegioResizeQuality.Medium, qualities);
        Assert.Contains(LegioResizeQuality.High, qualities);
        Assert.Contains(LegioResizeQuality.Maximum, qualities);
    }
}

public class LegioEncodingQualityTests
{
    [Fact]
    public void LegioEncodingQuality_ShouldHaveQualityLevels()
    {
        var qualities = Enum.GetValues<LegioEncodingQuality>();

        Assert.Equal(4, qualities.Length);
        Assert.Contains(LegioEncodingQuality.Low, qualities);
        Assert.Contains(LegioEncodingQuality.Medium, qualities);
        Assert.Contains(LegioEncodingQuality.High, qualities);
        Assert.Contains(LegioEncodingQuality.Maximum, qualities);
    }

    [Fact]
    public void LegioEncodingQuality_Low_ShouldBeZero()
    {
        Assert.Equal(0, (int)LegioEncodingQuality.Low);
    }

    [Fact]
    public void LegioEncodingQuality_Medium_ShouldBeFifty()
    {
        Assert.Equal(50, (int)LegioEncodingQuality.Medium);
    }

    [Fact]
    public void LegioEncodingQuality_High_ShouldBeSeventyFive()
    {
        Assert.Equal(75, (int)LegioEncodingQuality.High);
    }

    [Fact]
    public void LegioEncodingQuality_Maximum_ShouldBeHundred()
    {
        Assert.Equal(100, (int)LegioEncodingQuality.Maximum);
    }
}

public class LegioTransformTypeTests
{
    [Fact]
    public void LegioTransformType_ShouldHaveAllTypes()
    {
        var types = Enum.GetValues<LegioTransformType>();

        Assert.Equal(6, types.Length);
        Assert.Contains(LegioTransformType.None, types);
        Assert.Contains(LegioTransformType.Rotate90, types);
        Assert.Contains(LegioTransformType.Rotate180, types);
        Assert.Contains(LegioTransformType.Rotate270, types);
        Assert.Contains(LegioTransformType.FlipHorizontal, types);
        Assert.Contains(LegioTransformType.FlipVertical, types);
    }
}

public class LegioFilterTypeTests
{
    [Fact]
    public void LegioFilterType_ShouldHaveAllTypes()
    {
        var types = Enum.GetValues<LegioFilterType>();

        Assert.Equal(5, types.Length);
        Assert.Contains(LegioFilterType.None, types);
        Assert.Contains(LegioFilterType.Grayscale, types);
        Assert.Contains(LegioFilterType.Sepia, types);
        Assert.Contains(LegioFilterType.Blur, types);
        Assert.Contains(LegioFilterType.Sharpen, types);
    }
}

public class LegioImageInfoTests
{
    [Fact]
    public void LegioImageInfo_ShouldHaveDefaultValues()
    {
        var info = new LegioImageInfo();

        Assert.Equal(0, info.Width);
        Assert.Equal(0, info.Height);
        Assert.Equal(0, info.ByteSize);
        Assert.False(info.HasAlpha);
    }

    [Fact]
    public void LegioImageInfo_ShouldAllowSettingProperties()
    {
        var info = new LegioImageInfo
        {
            Width = 1920,
            Height = 1080,
            Format = LegioImageFormat.Jpeg,
            HasAlpha = false,
            ByteSize = 102400
        };

        Assert.Equal(1920, info.Width);
        Assert.Equal(1080, info.Height);
        Assert.Equal(LegioImageFormat.Jpeg, info.Format);
        Assert.False(info.HasAlpha);
        Assert.Equal(102400, info.ByteSize);
    }
}

public class FormatDetectorTests
{
    [Fact]
    public void DetectFormat_WithNullData_ShouldThrowArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => FormatDetector.DetectFormat((byte[])null!));
    }

    [Fact]
    public void DetectFormat_WithEmptyData_ShouldThrowArgumentException()
    {
        Assert.Throws<ArgumentException>(() => FormatDetector.DetectFormat(Array.Empty<byte>()));
    }

    [Fact]
    public void DetectFormat_WithTooShortData_ShouldThrowArgumentException()
    {
        var shortData = new byte[] { 0x89, 0x50 };
        Assert.Throws<ArgumentException>(() => FormatDetector.DetectFormat(shortData));
    }

    [Fact]
    public void DetectFormat_WithPngSignature_ShouldReturnPng()
    {
        var pngData = new byte[]
        {
            0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A,
            0x49, 0x45, 0x4E, 0x44, 0x00, 0x00, 0x00, 0x00, 0x00, 0x49,
            0x45, 0x4E, 0x44, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
        };

        var result = FormatDetector.DetectFormat(pngData);
        Assert.Equal(LegioImageFormat.Png, result);
    }

    [Fact]
    public void DetectFormat_WithJpegSignature_ShouldReturnJpeg()
    {
        var jpegData = new byte[]
        {
            0xFF, 0xD8, 0xFF, 0xE0,
            0xFF, 0xD8, 0xFF, 0xD9
        };

        var result = FormatDetector.DetectFormat(jpegData);
        Assert.Equal(LegioImageFormat.Jpeg, result);
    }

    [Fact]
    public void DetectFormat_WithWebPSignature_ShouldReturnWebP()
    {
        var webpData = new byte[]
        {
            0x52, 0x49, 0x46, 0x46,
            0x00, 0x00, 0x00, 0x00,
            0x57, 0x45, 0x66, 0x50,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
        };

        var result = FormatDetector.DetectFormat(webpData);
        Assert.Equal(LegioImageFormat.WebP, result);
    }

    [Fact]
    public void DetectFormat_WithBmpSignature_ShouldReturnBmp()
    {
        var bmpData = new byte[]
        {
            0x42, 0x4D, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x36,
            0x28, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00
        };

        var result = FormatDetector.DetectFormat(bmpData);
        Assert.Equal(LegioImageFormat.Bmp, result);
    }

    [Fact]
    public void DetectFormat_WithGif87ASignature_ShouldReturnGif()
    {
        var gifData = "GIF87a"u8.ToArray();

        var result = FormatDetector.DetectFormat(gifData);
        Assert.Equal(LegioImageFormat.Gif, result);
    }

    [Fact]
    public void DetectFormat_WithGif89ASignature_ShouldReturnGif()
    {
        var gifData = "GIF89a"u8.ToArray();

        var result = FormatDetector.DetectFormat(gifData);
        Assert.Equal(LegioImageFormat.Gif, result);
    }

    [Fact]
    public void DetectFormat_WithUnknownSignature_ShouldThrowNotSupportedException()
    {
        var unknownData = new byte[]
        {
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
        };

        Assert.Throws<NotSupportedException>(() => FormatDetector.DetectFormat(unknownData));
    }

    [Fact]
    public void DetectFormat_WithInvalidPngNoIEND_ShouldReturnPng()
    {
        var invalidPng = new byte[]
        {
            0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A,
            0x49, 0x45, 0x4E, 0x00, 0x00, 0x00, 0x00
        };

        var result = FormatDetector.DetectFormat(invalidPng);
        Assert.Equal(LegioImageFormat.Png, result);
    }
}