param([String]$version=30) 

dotnet pack .\src\MiniCover /p:Version=$version
dotnet nuget push --source https://pkgs.dev.azure.com/mews/_packaging/mews/nuget/v3/index.json --api-key az src\MiniCover\bin\Release\Mews.MiniCover.$version.nupkg

dotnet pack .\src\MiniCover.Core\ /p:Version=$version
dotnet nuget push --source https://pkgs.dev.azure.com/mews/_packaging/mews/nuget/v3/index.json --api-key az src\MiniCover.Core\bin\Release\Mews.MiniCover.Core.$version.nupkg

dotnet pack .\src\MiniCover.HitServices\ /p:Version=$version
dotnet nuget push --source https://pkgs.dev.azure.com/mews/_packaging/mews/nuget/v3/index.json --api-key az src\MiniCover.HitServices\bin\Release\Mews.MiniCover.HitServices.$version.nupkg

dotnet pack .\src\MiniCover.Reports\ /p:Version=$version
dotnet nuget push --source https://pkgs.dev.azure.com/mews/_packaging/mews/nuget/v3/index.json --api-key az src\MiniCover.Reports\bin\Release\Mews.MiniCover.Reports.$version.nupkg
