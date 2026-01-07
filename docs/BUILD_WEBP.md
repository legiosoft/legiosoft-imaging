# Building WebP Libraries

## Usage

```powershell
powershell -ExecutionPolicy Bypass -File build_webp.ps1
```

The script will prompt for the libwebp version (default: v1.6.0).

## Result

```
src/LegioSoft.Imaging.WebP/runtimes/
├── win-x64/libwebp.dll
├── linux-x64/libwebp.so
└── linux-arm64/libwebp.so
```

## Requirements

- Docker Desktop
- PowerShell
