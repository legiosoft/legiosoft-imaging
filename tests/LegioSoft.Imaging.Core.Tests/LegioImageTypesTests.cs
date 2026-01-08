using LegioSoft.Imaging.Core;
using Xunit;

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
