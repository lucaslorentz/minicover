dotnet restore

$env:Version="$(Get-Content version)-local-$(Get-Date -UFormat +%Y%m%d%H%M%S)"
dotnet pack -c Release --output "$(Resolve-Path .)/artifacts/local"

dotnet test tests/MiniCover.HitServices.UnitTests/MiniCover.HitServices.UnitTests.csproj
dotnet test tests/MiniCover.UnitTests/MiniCover.UnitTests.csproj
dotnet test tests/MiniCover.ApprovalTests/MiniCover.ApprovalTests.csproj
