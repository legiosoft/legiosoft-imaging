namespace LegioSoft.Imaging.WebP.Models;

public class WebPEncodeOptions
{
    public WebPEncodeOptions()
    {
        Preset = Enums.WebPPreset.DEFAULT;
        Quality = 75.0f;
        Lossless = false;
        InputFormat = WebPInputFormat.RGBA;
        Method = 4;
        ImageHint = Enums.WebPImageHint.DEFAULT;
        TargetSize = 0;
        TargetPSNR = 0.0f;
        Segments = 4;
        SnsStrength = 50;
        FilterStrength = 60;
        FilterSharpness = 0;
        FilterType = 0;
        Autofilter = true;
        AlphaCompression = 1;
        AlphaFiltering = 1;
        AlphaQuality = 100;
        Pass = 1;
        Preprocessing = 0;
        Partitions = 3;
        PartitionLimit = 0;
        UseSharpYUV = false;
    }

    public Enums.WebPPreset Preset { get; set; }
    public float Quality { get; set; }
    public bool Lossless { get; set; }
    public WebPInputFormat InputFormat { get; set; }
    public int Method { get; set; }
    public Enums.WebPImageHint ImageHint { get; set; }
    public int TargetSize { get; set; }
    public float TargetPSNR { get; set; }
    public int Segments { get; set; }
    public int SnsStrength { get; set; }
    public int FilterStrength { get; set; }
    public int FilterSharpness { get; set; }
    public int FilterType { get; set; }
    public bool Autofilter { get; set; }
    public int AlphaCompression { get; set; }
    public int AlphaFiltering { get; set; }
    public int AlphaQuality { get; set; }
    public int Pass { get; set; }
    public int Preprocessing { get; set; }
    public int Partitions { get; set; }
    public int PartitionLimit { get; set; }
    public bool UseSharpYUV { get; set; }
}

public enum WebPInputFormat
{
    RGBA,
    BGRA,
    RGB,
    BGR
}
