using LegioSoft.Imaging.WebP.Enums;

namespace LegioSoft.Imaging.WebP.Models;

public class WebPEncodeOptions
{
    public WebPPreset Preset { get; set; } = WebPPreset.DEFAULT;
    public float Quality { get; set; } = 75.0f;
    public bool Lossless { get; set; }
    public WebPInputFormat InputFormat { get; set; } = WebPInputFormat.RGBA;
    public int Method { get; set; } = 4;
    public WebPImageHint ImageHint { get; set; } = WebPImageHint.DEFAULT;
    public int TargetSize { get; set; } = 0;
    public float TargetPSNR { get; set; } = 0.0f;
    public int Segments { get; set; } = 4;
    public int SnsStrength { get; set; } = 50;
    public int FilterStrength { get; set; } = 60;
    public int FilterSharpness { get; set; } = 0;
    public int FilterType { get; set; } = 0;
    public bool Autofilter { get; set; } = true;
    public int AlphaCompression { get; set; } = 1;
    public int AlphaFiltering { get; set; } = 1;
    public int AlphaQuality { get; set; } = 100;
    public int Pass { get; set; } = 1;
    public int Preprocessing { get; set; } = 0;
    public int Partitions { get; set; } = 3;
    public int PartitionLimit { get; set; } = 0;
    public bool UseSharpYUV { get; set; } = false;
}

