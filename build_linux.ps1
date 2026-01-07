$ErrorActionPreference = "Stop"

# WebP Library Builder for Linux x64
# Builds libwebp.so for Linux x64 using Docker

$baseDir = $PSScriptRoot
$outputDir = Join-Path $baseDir "docker\build-webp\output\linux-x64"
$dockerfile = Join-Path $baseDir "docker\build-webp\Dockerfile"
$imageName = "webp-builder-amd64"
$projectRuntimeDir = Join-Path $baseDir "src\LegioSoft.Imaging.WebP\runtimes\linux-x64"

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "WebP Builder for Linux x64" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Ask for version
$defaultVersion = "v1.6.0"
$version = Read-Host "Enter libwebp version to build (default: $defaultVersion)"
if ([string]::IsNullOrWhiteSpace($version)) {
    $version = $defaultVersion
}

Write-Host "Building version: $version" -ForegroundColor Yellow
Write-Host ""

# Create output directories
New-Item -ItemType Directory -Force -Path $outputDir | Out-Null
New-Item -ItemType Directory -Force -Path $projectRuntimeDir | Out-Null

Write-Host "Building Docker image..." -ForegroundColor White
docker build --target builder-amd64 -t $imageName -f $dockerfile $baseDir

if ($LASTEXITCODE -ne 0) {
    Write-Host "ERROR: Docker image build failed" -ForegroundColor Red
    exit 1
}

Write-Host "Extracting libraries from container..." -ForegroundColor White
$containerName = "$($imageName)-temp"
docker create --name $containerName $imageName
docker cp "$containerName`:/output/linux-x64" (Join-Path $baseDir "docker\build-webp\output")
docker rm $containerName

Write-Host "Copying to project runtimes..." -ForegroundColor White
$soPath = Join-Path $outputDir "libwebp.so"
if (Test-Path $soPath) {
    Copy-Item -Path $soPath -Destination $projectRuntimeDir -Force
    $fileInfo = Get-Item $soPath
    Write-Host "  Copied libwebp.so ($([math]::Round($fileInfo.Length / 1KB, 2))KB)" -ForegroundColor Green
    
    Write-Host ""
    Write-Host "========================================" -ForegroundColor Cyan
    Write-Host "Build successful!" -ForegroundColor Green
    Write-Host "========================================" -ForegroundColor Cyan
    Write-Host "Output: $projectRuntimeDir\libwebp.so"
    Write-Host ""
} else {
    Write-Host "ERROR: libwebp.so not found in $outputDir" -ForegroundColor Red
    exit 1
}
