using LegioSoft.Imaging.WebP.Enums;

namespace LegioSoft.Imaging.WebP.Models;

public class WebPInfo
{
    public int Width { get; set; }
    public int Height { get; set; }
    public bool HasAlpha { get; set; }
    public bool HasAnimation { get; set; }
    public WebPFormat Format { get; set; }
}

