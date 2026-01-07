namespace LegioSoft.Imaging.WebP.Enums;

public enum VP8StatusCode
{
    OK,
    OUT_OF_MEMORY,
    INVALID_PARAM,
    BITSTREAM_ERROR,
    UNSUPPORTED_FEATURE,
    SUSPENDED,
    USER_ABORT,
    NOT_ENOUGH_DATA
}

public enum WEBP_CSP_MODE
{
    MODE_RGB = 0,
    MODE_RGBA = 1,
    MODE_BGR = 2,
    MODE_BGRA = 3,
    MODE_ARGB = 4,
    MODE_YUV = 5,
    MODE_YUVA = 6,
    MODE_LAST = 7
}

public enum WebPEncodingError
{
    OK,
    OUT_OF_MEMORY,
    INVALID_PARAM,
    BITSTREAM_ERROR,
    UNSUPPORTED_FEATURE,
    SUSPENDED,
    USER_ABORT,
    NOT_ENOUGH_DATA
}

public enum WebPImageHint
{
    DEFAULT,
    PICTURE,
    PHOTO,
    GRAPH,
    LAST
}

public enum WebPPreset
{
    DEFAULT,
    PICTURE,
    PHOTO,
    DRAWING,
    ICON,
    TEXT,
    LAST
}