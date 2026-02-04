namespace LegioSoft.Imaging.SVG.Parser;

public class SvgParseOptions
{
    public int MaxNestingDepth { get; set; } = 100;
    public int MaxElements { get; set; } = 10000;
}
