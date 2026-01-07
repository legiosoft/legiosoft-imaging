using System;
using System.IO;
using LegioSoft.Imaging.Core;

namespace LegioSoft.Imaging.Tests.Common;

public static class TestDataHelper
{
    public static byte[] CreateTestRGBA(int width, int height)
    {
        byte[] data = new byte[width * height * 4];
        
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int index = (y * width + x) * 4;
                
                byte r = (byte)((x * 255) / width);
                byte g = (byte)((y * 255) / height);
                byte b = (byte)(255 - r);
                
                data[index] = r;
                data[index + 1] = g;
                data[index + 2] = b;
                data[index + 3] = 255;
            }
        }
        
        return data;
    }

    public static byte[] CreateTestRGB(int width, int height)
    {
        byte[] data = new byte[width * height * 3];
        
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int index = (y * width + x) * 3;
                
                byte r = (byte)((x * 255) / width);
                byte g = (byte)((y * 255) / height);
                byte b = (byte)(255 - r);
                
                data[index] = r;
                data[index + 1] = g;
                data[index + 2] = b;
            }
        }
        
        return data;
    }

    public static byte[] CreateTestBGRA(int width, int height)
    {
        byte[] data = new byte[width * height * 4];
        
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int index = (y * width + x) * 4;
                
                byte b = (byte)((x * 255) / width);
                byte g = (byte)((y * 255) / height);
                byte r = (byte)(255 - b);
                
                data[index] = b;
                data[index + 1] = g;
                data[index + 2] = r;
                data[index + 3] = 255;
            }
        }
        
        return data;
    }

    public static byte[] CreateTestBGR(int width, int height)
    {
        byte[] data = new byte[width * height * 3];
        
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int index = (y * width + x) * 3;
                
                byte b = (byte)((x * 255) / width);
                byte g = (byte)((y * 255) / height);
                byte r = (byte)(255 - b);
                
                data[index] = b;
                data[index + 1] = g;
                data[index + 2] = r;
            }
        }
        
        return data;
    }

    public static void SaveAsJpeg(string path, byte[] rgbaData, int width, int height)
    {
        try
        {
            using var bitmap = new System.Drawing.Bitmap(width, height);
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int index = (y * width + x) * 4;
                    var color = System.Drawing.Color.FromArgb(
                        rgbaData[index + 3],
                        rgbaData[index],
                        rgbaData[index + 1],
                        rgbaData[index + 2]
                    );
                    bitmap.SetPixel(x, y, color);
                }
            }
            bitmap.Save(path, System.Drawing.Imaging.ImageFormat.Jpeg);
        }
        catch
        {
        }
    }

    public static bool ImagesAreSimilar(byte[] img1, byte[] img2, int maxDiff = 10)
    {
        if (img1 == null || img2 == null)
            return false;
        
        if (img1.Length != img2.Length)
            return false;
        
        int diffCount = 0;
        for (int i = 0; i < img1.Length; i++)
        {
            if (Math.Abs(img1[i] - img2[i]) > maxDiff)
            {
                diffCount++;
                if (diffCount > img1.Length / 100)
                    return false;
            }
        }
        
        return true;
    }

    public static void EnsureDirectoryExists(string path)
    {
        var directory = Path.GetDirectoryName(path);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
    }
}
