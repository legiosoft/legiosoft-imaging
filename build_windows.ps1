$ErrorActionPreference = "Stop"

# WebP Library Builder for Windows x64 (64-bit)
# Builds libwebp.dll for Windows x64 using Docker
# Updated to force 64-bit compilation with proper architecture flags

$baseDir = $PSScriptRoot
$outputDir = Join-Path $baseDir "docker\build-webp\output\win-x64"
$dockerfile = Join-Path $baseDir "docker\build-webp\Dockerfile.windows"
$imageName = "webp-builder-win-x64"
$projectRuntimeDir = Join-Path $baseDir "src\LegioSoft.Imaging.WebP\runtimes\win-x64"

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "WebP Builder for Windows x64 (64-bit)" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Ask for version
$defaultVersion = "v1.6.0"
$version = Read-Host "Enter libwebp version to build (default: $defaultVersion)"
if ([string]::IsNullOrWhiteSpace($version)) {
    $version = $defaultVersion
}

Write-Host "Building version: $version" -ForegroundColor Yellow
Write-Host "Architecture: x64 (64-bit)" -ForegroundColor Yellow
Write-Host ""

# Create output directories
New-Item -ItemType Directory -Force -Path $outputDir | Out-Null
New-Item -ItemType Directory -Force -Path $projectRuntimeDir | Out-Null

Write-Host "Building Docker image (64-bit)..." -ForegroundColor White
docker build -t $imageName -f $dockerfile $baseDir

if ($LASTEXITCODE -ne 0) {
    Write-Host "ERROR: Docker image build failed" -ForegroundColor Red
    exit 1
}

Write-Host "Extracting libraries from container..." -ForegroundColor White
$containerName = "$($imageName)-temp"
docker create --name $containerName $imageName
docker cp "$containerName`:/output" (Join-Path $baseDir "docker\build-webp\output")
docker rm $containerName

Write-Host "Copying to project runtimes..." -ForegroundColor White
$dllPath = Join-Path $outputDir "libwebp.dll"
if (Test-Path $dllPath) {
    Copy-Item -Path $dllPath -Destination $projectRuntimeDir -Force
    $fileInfo = Get-Item $dllPath
    
    # Verify it's 64-bit
    $bytes = [System.IO.File]::ReadAllBytes($dllPath)
    $is64Bit = $bytes[23] -eq 0x8b
    
    if ($is64Bit) {
        Write-Host "Copied libwebp.dll ($($fileInfo.Length / 1KB)KB)" -ForegroundColor Green
        Write-Host "Architecture: 64-bit ✓" -ForegroundColor Green
    } else {
        Write-Host "WARNING: libwebp.dll may not be 64-bit (PE header: 0x$($bytes[23]).ToString('X2'))" -ForegroundColor Yellow
    }
    
    Write-Host ""
    
    Write-Host "========================================" -ForegroundColor Cyan
    Write-Host "Build successful!" -ForegroundColor Green
    Write-Host "========================================" -ForegroundColor Cyan
    Write-Host "Output: $projectRuntimeDir\libwebp.dll"
    Write-Host ""
} else {
    Write-Host "ERROR: libwebp.dll not found in $outputDir" -ForegroundColor Red
    exit 1
}
