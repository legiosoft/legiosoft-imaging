using System.Diagnostics;
using System.Text;

namespace LegioSoft.Imaging.Skia.Core;

internal static class ImageSecurity
{
    internal static void ValidateOpenedFile(FileStream fileStream, string originalPath, string[] approvedDirectories, HashSet<string> allowedExtensions)
    {
        string? actualPath = null;

        // Attempt to resolve path via OS Handle (Secure)
        if (OperatingSystem.IsWindows())
        {
            try
            {
                var handle = fileStream.SafeFileHandle.DangerousGetHandle();
                var pathBuilder = new StringBuilder(512);
                var result = Windows.GetFinalPathNameByHandle(
                    handle,
                    pathBuilder,
                    (uint)pathBuilder.Capacity,
                    Windows.FILE_NAME_NORMALIZED);

                if (result != 0)
                {
                    actualPath = pathBuilder.ToString();
                    if (actualPath.StartsWith(@"\\?\"))
                        actualPath = actualPath.Substring(4);
                }
            }
            catch
            {
                // If OS path resolution fails, actualPath remains null.
                // The logic below handles this by failing closed on Windows 
                // (secure) or using a fallback on Linux.
            }
        }

        // Decision Logic: Fail Closed on Windows, Fail Open (Fallback) on Linux
        if (actualPath == null)
        {
            if (OperatingSystem.IsWindows())
            {
                throw new UnauthorizedAccessException(
                    "Security Check Failed: Unable to verify file path via OS handle.");
            }
            else
            {
                actualPath = Path.GetFullPath(originalPath);
            }
        }

        ValidatePathNotContainSymlinks(actualPath);

        var isApproved = approvedDirectories.Any(dir =>
        {
            try
            {
                var fullDir = Path.GetFullPath(dir);
                var searchPath = fullDir + Path.DirectorySeparatorChar;
                var comparison = OperatingSystem.IsWindows() 
                    ? StringComparison.OrdinalIgnoreCase 
                    : StringComparison.Ordinal;

                return actualPath.StartsWith(searchPath, comparison) ||
                       actualPath.Equals(fullDir, comparison);
            }
            catch
            {
                // If path validation fails (e.g., path is malformed), 
                // conservatively treat it as not approved for security.
                return false;
            }
        });

        if (!isApproved)
        {
            throw new UnauthorizedAccessException(
                $"File path '{actualPath}' is outside the approved image directories: " +
                $"{string.Join(", ", approvedDirectories)}.");
        }

        var extension = Path.GetExtension(actualPath);
        if (!allowedExtensions.Contains(extension))
        {
            throw new UnauthorizedAccessException(
                $"File '{actualPath}' has extension '{extension}' which is not allowed.");
        }
    }

    private static void ValidatePathNotContainSymlinks(string fullPath)
    {
        var directory = Path.GetDirectoryName(fullPath);
        if (string.IsNullOrEmpty(directory))
            return;

        var currentDir = directory;
        var visited = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var rootPath = Path.GetPathRoot(fullPath);

        while (!string.IsNullOrEmpty(currentDir) && 
               currentDir != rootPath && 
               !visited.Contains(currentDir))
        {
            visited.Add(currentDir);

            try
            {
                var dirInfo = new DirectoryInfo(currentDir);
                
                if (OperatingSystem.IsWindows())
                {
                    var attributes = dirInfo.Attributes;
                    if ((attributes & FileAttributes.ReparsePoint) != 0)
                    {
                        throw new UnauthorizedAccessException(
                            $"Directory '{currentDir}' is a symbolic link or junction.");
                    }
                }
                else
                {
                    if (dirInfo.LinkTarget != null)
                    {
                        throw new UnauthorizedAccessException(
                            $"Directory '{currentDir}' is a symbolic link.");
                    }
                }
            }
            catch (UnauthorizedAccessException)
            {
                throw;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(
                    $"Warning: Could not check symlink status for directory '{currentDir}': {ex.Message}");
            }

            currentDir = Path.GetDirectoryName(currentDir);
        }
    }

    private static class Windows
    {
        internal const uint FILE_NAME_NORMALIZED = 0x0;

        [System.Runtime.InteropServices.DllImport(
            "kernel32.dll",
            SetLastError = true,
            CharSet = System.Runtime.InteropServices.CharSet.Unicode)]
        internal static extern uint GetFinalPathNameByHandle(
            IntPtr hFile,
            StringBuilder lpszFilePath,
            uint cchFilePath,
            uint dwFlags);
    }
}
