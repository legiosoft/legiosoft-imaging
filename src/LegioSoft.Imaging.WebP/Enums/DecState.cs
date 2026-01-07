namespace LegioSoft.Imaging.WebP.Enums;

public enum DecState
{
    OK,
    VP8_HEADER_NOT_FOUND,
    VP8_BITSTREAM_NOT_IN_HEADER,
    VP8_UNSUPPORTED_VERSION,
    VP8_BITSTREAM_ERROR
}