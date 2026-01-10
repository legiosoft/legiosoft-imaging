# Build WebP Native Libraries

Build libwebp native libraries for Windows and Linux.

## Run

```powershell
powershell -ExecutionPolicy Bypass -File build_webp.ps1
```

Enter libwebp version (default: v1.6.0).

## Output

```
src/LegioSoft.Imaging.WebP/runtimes/
├── win-x64/libwebp.dll
├── linux-x64/libwebp.so
└── linux-arm64/libwebp.so
```

## Requirements

- Docker Desktop
- PowerShell
