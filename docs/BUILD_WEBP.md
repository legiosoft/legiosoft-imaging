# Building WebP Packages

This guide covers building WebP native libraries using Docker and PowerShell scripts.

## Quick Start

**Recommended Method:** PowerShell script with Docker

```powershell
# Build for current platform
powershell -ExecutionPolicy Bypass -File build-webp-docker.ps1

# Build all platforms (Windows x64, Linux x64, Linux ARM64, macOS x64, macOS ARM64)
powershell -ExecutionPolicy Bypass -File build-webp-docker.ps1 -BuildAll

# Download only (no Docker)
powershell -ExecutionPolicy Bypass -File build-webp-docker.ps1 -DownloadOnly

# Skip ARM64 builds
powershell -ExecutionPolicy Bypass -File build-webp-docker.ps1 -SkipArm64
```

## PowerShell Script Options

| Flag | Description |
|------|-------------|
| `-BuildAll` | Build for all platforms |
| `-SkipArm64` | Skip ARM64 builds (Linux ARM64, macOS ARM64) |
| `-DownloadOnly` | Download pre-built libraries only |
| `-SkipDocker` | Skip Docker, download only |

## Output Structure

After building, libraries are placed in:

```
src/LegioSoft.Imaging.WebP/runtimes/
├── win-x64/libwebp.dll
├── linux-x64/libwebp.so
├── linux-arm64/libwebp.so
├── osx-x64/libwebp.dylib
└── osx-arm64/libwebp.dylib
```

## Prerequisites

- **Docker Desktop** (for building)
- **PowerShell** (Windows) or **Bash** (Linux/macOS)

## Alternative: vcpkg (Windows Only)

```cmd
vcpkg install libwebp:x64-windows
copy C:\vcpkg\installed\x64-windows\bin\libwebp.dll src\LegioSoft.Imaging.WebP\runtimes\win-x64\
```

## Manual Build

For full control, build libwebp from source:

**Windows:**
```cmd
git clone https://chromium.googlesource.com/webm/libwebp
cd libwebp
git checkout v1.6.0
mkdir build && cd build
cmake .. -DBUILD_SHARED_LIBS=ON -G "Visual Studio 17 2022" -A x64
cmake --build . --config Release
```

**Linux/macOS:**
```bash
git clone https://chromium.googlesource.com/webm/libwebp
cd libwebp
git checkout v1.6.0
mkdir build && cd build
cmake .. -DBUILD_SHARED_LIBS=ON
make -j$(nproc)
```

## After Building

Verify libraries load correctly:

```bash
cd src/LegioSoft.Imaging.WebP/Tests
dotnet run
```

Expected output:
```
✅ IsWebP() works: True
✅ DLL loaded and WebPGetInfo works
```

## Common Issues

| Issue | Solution |
|-------|----------|
| Docker not running | Start Docker Desktop |
| "DllNotFoundException" | Check runtimes/ directory structure |
| Windows DLL missing | Use vcpkg or manual build |
| Build too slow | Increase Docker resources to 4GB+ |

## Clean Up

```bash
docker system prune -a
```
