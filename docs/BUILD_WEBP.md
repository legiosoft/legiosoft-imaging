# Building libwebp

This document explains how to build Google `libwebp` library for all supported platforms.

## Platform Support

| Platform | Build Method | Status |
|----------|-------------|--------|
| Windows x64 | Visual Studio Build Tools | ✅ Supported (see below) |
| Linux x64 | Docker (glibc) | ✅ Supported (see below) |
| Linux ARM64 | Docker (glibc) | ✅ Supported (see below) |
| macOS x64 (Intel) | CMake from source | ✅ Supported (see below) |
| macOS ARM64 (M1/M2/M3) | CMake from source | ✅ Supported (see below) |

---

## Building on Windows (x64)

### Prerequisites

You need **"Build Tools for Visual Studio 2022"** (or newer) installed.

During installation, ensure you select following workload:
- **Desktop development with C++**

### The "Trusted" Environment

To comply with corporate security requirements, you must **not** use standard PowerShell or CMD.

**IMPORTANT:** Follow these steps exactly:

1. Open Start Menu
2. Search for "x64 Native"
3. Right-click on **"x64 Native Tools Command Prompt for VS 2022"**
4. Select **"Run as administrator"**

This command prompt provides Microsoft C++ compiler (MSVC) with correct x64 environment variables pre-configured.

### Source Code & Version Control

Clone libwebp repository from official Google source:

```bash
git clone https://chromium.googlesource.com/webm/libwebp
cd libwebp
```

### Selecting a Specific Version

To build a specific release version:

1. List all available tags:
   ```bash
   git tag
   ```

2. Checkout desired version (for example, v1.6.0):
   ```bash
   git checkout v1.6.0
   ```

**Warning:** If you switch versions, make sure to delete `build` folder before proceeding to avoid cached configuration issues:
```bash
rmdir /s /q build
```

### Configuration (CMake)

Configure project to build as a shared library (DLL):

```bash
cmake -S . -B build -DBUILD_SHARED_LIBS=ON
```

**Critical:** The `-DBUILD_SHARED_LIBS=ON` flag is essential. Without it, CMake will generate a static library (`.lib`) instead of a dynamic library (`.dll`).

**Note:** You may see "Failed" test messages in the logs regarding NEON, MIPS, and Android architectures. These are **normal** and expected for x64 Windows builds. They should be safely ignored.

### Compilation

Build project in Release mode:

```bash
cmake --build build --config Release
```

This command compiles source code using MSVC and generates optimized release binaries.

### Locating Output

The final build artifacts are located in:

```
build\Release\
```

The main output file is:
- `libwebp.dll` - The dynamic library for runtime linking
- `libwebp.lib` - The import library for linking during development

Additional libraries (such as `libwebpmux.dll`, `libwebpdemux.dll`) may also be present depending on build configuration.

---

## Building on Linux (x64 & ARM64) using Docker

### Prerequisites

- Docker Desktop or Docker Engine installed
- Docker configured for multi-platform builds

### Automated Build Script

The repository includes a PowerShell script to build all Linux platforms:

```powershell
.\build_webp.ps1
```

The script will:
1. Ask for libwebp version (default: v1.6.0)
2. Build Docker images for linux-x64 and linux-arm64
3. Extract compiled libraries
4. Copy them to `src\LegioSoft.Imaging.WebP\runtimes\`

### Manual Docker Build

If you prefer to build manually:

#### Build Linux x64

```bash
docker build --target builder-amd64 -t webp-builder-amd64 -f docker/build-webp/Dockerfile --build-arg WEBP_VERSION=v1.6.0 .
```

#### Build Linux ARM64

```bash
docker build --target builder-arm64 -t webp-builder-arm64 -f docker/build-webp/Dockerfile --build-arg WEBP_VERSION=v1.6.0 .
```

#### Extract Libraries

```bash
# Extract x64
docker create --name temp-amd64 webp-builder-amd64
docker cp temp-amd64:/output/linux-x64 docker/build-webp/output
docker rm temp-amd64

# Extract ARM64
docker create --name temp-arm64 webp-builder-arm64
docker cp temp-arm64:/output/linux-arm64 docker/build-webp/output
docker rm temp-arm64
```

### Docker Build Details

The Dockerfile builds libwebp with the following configuration:

- **Shared libraries only** (`BUILD_SHARED_LIBS=ON`)
- **Release optimization** (`CMAKE_BUILD_TYPE=Release`)
- **Position-independent code** (`CMAKE_POSITION_INDEPENDENT_CODE=ON`)
- **glibc compatibility** (both x64 and ARM64 use Ubuntu 22.04)

This ensures maximum compatibility with Linux distributions including:
- Ubuntu 20.04+
- Debian 11+
- CentOS 7+, RHEL 7+
- Fedora 35+
- Alpine (with gcompat or libc6-compat)

---

## Building on macOS (x64 & ARM64)

### Prerequisites

You need the Apple Command Line Tools and dependencies for image format support.

```bash
# Install Xcode Command Line Tools
xcode-select --install

# Install dependencies via Homebrew
brew install cmake automake libtool jpeg libpng libtiff giflib
```

### Source Code & Version Control

Clone libwebp repository from official Google source:

```bash
git clone https://chromium.googlesource.com/webm/libwebp
cd libwebp
```

### Selecting a Specific Version

To build a specific release version:

```bash
# List all available versions
git tag -l

# Checkout desired version (e.g., v1.6.0)
git checkout v1.6.0
```

**Warning:** If you switch versions, delete the build folder before proceeding:
```bash
rm -rf build
```

### Build Methods

#### Method A: Using CMake (Recommended)

This is the modern standard and ensures all paths are mapped correctly.

```bash
# Create a build directory
mkdir build && cd build

# Configure the build
# $(brew --prefix) ensures we find the JPEG/PNG libraries in /usr/local
cmake .. \
  -DCMAKE_PREFIX_PATH=$(brew --prefix) \
  -DWEBP_BUILD_CWEBP=ON \
  -DWEBP_BUILD_DWEBP=ON \
  -DWEBP_BUILD_GIF2WEBP=ON \
  -DWEBP_BUILD_LIBWEBPMUX=ON \
  -DCMAKE_BUILD_TYPE=Release

# Compile
make -j$(sysctl -n hw.ncpu)

# Install
sudo make install
```

#### Method B: Using Autotools

Use this if you prefer the classic Unix ./configure workflow.

```bash
# Generate build scripts
./autogen.sh

# Configure with Homebrew paths
export LDFLAGS="-L$(brew --prefix)/lib"
export CPPFLAGS="-I$(brew --prefix)/include"

./configure --enable-everything

# Build and Install
make -j$(sysctl -n hw.ncpu)
sudo make install
```

### Locating Output

After `sudo make install`, the library is installed to `/usr/local/lib/`:

```bash
# The library
/usr/local/lib/libwebp.dylib

# Copy to project
cp /usr/local/lib/libwebp.dylib src/LegioSoft.Imaging.WebP/runtimes/osx-x64/
```

### Verification

Ensure the library was compiled for the correct architecture:

```bash
# Check the shared library
file /usr/local/lib/libwebp.dylib
```

Expected output:
- **Intel Mac**: `Mach-O 64-bit dynamically linked shared library x86_64`
- **Apple Silicon**: `Mach-O 64-bit dynamically linked shared library arm64`

---

## Copying Libraries to Project

After building, copy the compiled libraries to the appropriate runtimes directories:

```
src/LegioSoft.Imaging.WebP/runtimes/
├── win-x64/
│   └── libwebp.dll
├── linux-x64/
│   ├── libwebp.so
│   ├── libwebp.so.7
│   └── libwebp.so.7.2.0
├── linux-arm64/
│   ├── libwebp.so
│   ├── libwebp.so.7
│   └── libwebp.so.7.2.0
├── osx-x64/
│   └── libwebp.dylib
└── osx-arm64/
    └── libwebp.dylib
```

**Note:** On Linux, you may see multiple `.so` files with version numbers. Only copy the symlinked `libwebp.so` or the full set including versioned files if you need them.

---

## Version Compatibility

Ensure the libwebp version matches the expected ABI version in the C# code:

- **Current version**: v1.6.0
- **Expected ABI version**: 0x0210

This is defined in `WebPDecoder.cs` and `WebPEncoder.cs`:
```csharp
private const int WEBP_DECODER_ABI_VERSION = 0x0210;
private const int WEBP_ENCODER_ABI_VERSION = 0x0210;
```

---

## Troubleshooting

### Windows Build Issues

**Issue**: CMake cannot find compiler
- **Solution**: Use "x64 Native Tools Command Prompt for VS 2022", not regular cmd or PowerShell

**Issue**: Build fails with "Failed" messages for NEON, MIPS
- **Solution**: These are expected on x64 Windows builds. Ignore them.

### Docker Build Issues

**Issue**: Permission denied when extracting files
- **Solution**: Run PowerShell as Administrator

**Issue**: Docker build fails on ARM64 without QEMU
- **Solution**: Install QEMU for cross-platform builds:
  ```bash
  docker run --privileged --rm tonistiigi/binfmt --install all
  ```

**Issue**: Library fails to load on Linux with "cannot open shared object file"
- **Solution**: Ensure the library has execute permissions:
  ```bash
  chmod +x libwebp.so
  ```

### macOS Build Issues

**Issue**: CMake cannot find compiler or SDK
- **Solution**: Ensure Xcode command line tools are installed:
  ```bash
  xcode-select --install
  xcode-select -p
  ```

**Issue**: Homebrew paths not found
- **Solution**: Verify Homebrew is installed and use `$(brew --prefix)`:
  ```bash
  brew --prefix
  ```

**Issue**: Wrong architecture built
- **Solution**: Verify architecture with `file` command:
  ```bash
  file /usr/local/lib/libwebp.dylib
  ```
  Expected output:
  - Intel Mac: `Mach-O 64-bit dynamically linked shared library x86_64`
  - Apple Silicon: `Mach-O 64-bit dynamically linked shared library arm64`
