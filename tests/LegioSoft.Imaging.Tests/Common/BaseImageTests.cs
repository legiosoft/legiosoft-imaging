using System;
using System.IO;
using Xunit;
using LegioSoft.Imaging.Core;

namespace LegioSoft.Imaging.Tests.Common;

public abstract class BaseImageTests : IDisposable
{
    protected string TestDataPath { get; }
    protected string OutputPath { get; }

    protected BaseImageTests(string testDataPath = "TestData", string outputPath = "Output")
    {
        TestDataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, testDataPath);
        OutputPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, outputPath);

        if (!Directory.Exists(TestDataPath))
        {
            Directory.CreateDirectory(TestDataPath);
        }

        if (!Directory.Exists(OutputPath))
        {
            Directory.CreateDirectory(OutputPath);
        }
    }

    protected string GetTestFilePath(string fileName)
    {
        var path = Path.Combine(TestDataPath, fileName);
        if (!File.Exists(path))
        {
            throw new FileNotFoundException($"Test file not found: {path}");
        }
        return path;
    }

    protected string GetOutputFilePath(string fileName)
    {
        return Path.Combine(OutputPath, fileName);
    }

    protected byte[] LoadTestFile(string fileName)
    {
        var path = GetTestFilePath(fileName);
        return File.ReadAllBytes(path);
    }

    protected void SaveOutputFile(string fileName, byte[] data)
    {
        var path = GetOutputFilePath(fileName);
        File.WriteAllBytes(path, data);
    }

    protected void AssertFileExists(string path)
    {
        Assert.True(File.Exists(path), $"Expected file does not exist: {path}");
    }

    protected void AssertImageData(byte[] data, string description)
    {
        Assert.NotNull(data);
        Assert.True(data.Length > 0, $"{description}: Image data should not be empty");
    }

    protected void AssertImageInfo(ImageInfo info, int expectedMinWidth = 1, int expectedMinHeight = 1)
    {
        Assert.NotNull(info);
        Assert.True(info.Width >= expectedMinWidth, $"Width should be at least {expectedMinWidth}, got {info.Width}");
        Assert.True(info.Height >= expectedMinHeight, $"Height should be at least {expectedMinHeight}, got {info.Height}");
        Assert.True(info.ByteSize > 0, "Byte size should be positive");
    }

    public virtual void Dispose()
    {
        CleanupOutputFiles();
    }

    protected void CleanupOutputFiles()
    {
        if (Directory.Exists(OutputPath))
        {
            try
            {
                foreach (var file in Directory.GetFiles(OutputPath))
                {
                    File.Delete(file);
                }
            }
            catch
            {
            }
        }
    }
}
