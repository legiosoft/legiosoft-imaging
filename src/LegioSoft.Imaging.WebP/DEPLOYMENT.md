# Deploy LegioSoft.Imaging.WebP to NuGet

```bash
cd src/LegioSoft.Imaging.WebP
dotnet pack -c Release
dotnet nuget push bin/Release/LegioSoft.Imaging.WebP.1.0.0-beta1.nupkg --api-key YOUR_API_KEY --source https://api.nuget.org/v3/index.json
```

## Bump Version

Version is defined in `Directory.Build.props` (affects ALL projects):

```bash
# Edit Directory.Build.props, change <Version>, then:
dotnet pack -c Release
```

Or temporary override:
```bash
dotnet pack -c Release /p:Version=1.0.0
```
