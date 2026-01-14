namespace LegioSoft.Imaging.WebP.Helpers;

public static class WebPValidationHelper
{
    public static bool IsWebP(byte[] imageData)
    {
        if (imageData.Length < 12)
        {
            return false;
        }

        return imageData[0] == 0x52 && 
               imageData[1] == 0x49 && 
               imageData[2] == 0x46 && 
               imageData[3] == 0x46 &&
               imageData[8] == 0x57 && 
               imageData[9] == 0x45 && 
               imageData[10] == 0x66 && 
               imageData[11] == 0x50;
    }
}
