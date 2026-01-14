$ErrorActionPreference = "Stop"

# WebP Library Builder
# Builds libwebp for multiple platforms using Docker

$baseDir = $PSScriptRoot
$dockerfile = Join-Path $baseDir "docker\build-webp\Dockerfile"
$imageName = "webp-builder-multi"

$targets = @(
    @{Name="linux-x64"; DockerTarget="builder-amd64"; Library="libwebp.so"; RuntimeDir="linux-x64"},
    @{Name="linux-arm64"; DockerTarget="builder-arm64"; Library="libwebp.so"; RuntimeDir="linux-arm64"}
)

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "WebP Multi-Platform Builder" -ForegroundColor Cyan
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
$outputBaseDir = Join-Path $baseDir "docker\build-webp\output"
New-Item -ItemType Directory -Force -Path $outputBaseDir | Out-Null

foreach ($target in $targets) {
    $outputDir = Join-Path $outputBaseDir $target.Name
    $projectRuntimeDir = Join-Path $baseDir "src\LegioSoft.Imaging.WebP\runtimes\$($target.RuntimeDir)"
    New-Item -ItemType Directory -Force -Path $outputDir | Out-Null
    New-Item -ItemType Directory -Force -Path $projectRuntimeDir | Out-Null
}

Write-Host "Building Docker images for all targets..." -ForegroundColor White
Write-Host ""

foreach ($target in $targets) {
    Write-Host "----------------------------------------" -ForegroundColor White
    Write-Host "Building for $($target.Name)..." -ForegroundColor White
    Write-Host "----------------------------------------" -ForegroundColor White
    
    $targetImageName = "$($imageName)-$($target.Name)"
    
    Write-Host "  Building Docker image..." -ForegroundColor Gray
    docker build --target $target.DockerTarget -t $targetImageName -f $dockerfile --build-arg "WEBP_VERSION=$version" $baseDir
    
    if ($LASTEXITCODE -ne 0) {
        Write-Host "  ERROR: Docker image build failed for $($target.Name)" -ForegroundColor Red
        exit 1
    }
    
    Write-Host "  Extracting libraries from container..." -ForegroundColor Gray
    $containerName = "$($targetImageName)-temp"
    docker create --name $containerName $targetImageName
    docker cp "$containerName`:/output/$($target.Name)" $outputBaseDir
    docker rm $containerName
    
    $outputDir = Join-Path $outputBaseDir $target.Name
    $libraryPath = Join-Path $outputDir $target.Library
    
    if (Test-Path $libraryPath) {
        $projectRuntimeDir = Join-Path $baseDir "src\LegioSoft.Imaging.WebP\runtimes\$($target.RuntimeDir)"
        Copy-Item -Path $libraryPath -Destination $projectRuntimeDir -Force
        $fileInfo = Get-Item $libraryPath
        
        $sizeKB = [math]::Round($fileInfo.Length / 1KB, 2)
        Write-Host "  Copied $($target.Library) ($sizeKB KB) to runtimes\$($target.RuntimeDir)\" -ForegroundColor Green
    } else {
        Write-Host "  ERROR: $($target.Library) not found in $outputDir" -ForegroundColor Red
        exit 1
    }
    
    Write-Host ""
}

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Build successful!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Libraries built and copied to:" -ForegroundColor White
foreach ($target in $targets) {
    $runtimeDir = Join-Path $baseDir "src\LegioSoft.Imaging.WebP\runtimes\$($target.RuntimeDir)"
    Write-Host "  $($target.Name): $runtimeDir\$($target.Library)" -ForegroundColor Gray
}
Write-Host ""
