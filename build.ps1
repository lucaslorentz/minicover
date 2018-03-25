dotnet restore

dotnet pack -c Release --output "$(Resolve-Path .)/artifacts" --version-suffix ci-$(Get-Date -UFormat +%Y%m%d%H%M%S)

dotnet test tests/MiniCover.HitServices.UnitTests/MiniCover.HitServices.UnitTests.csproj
dotnet test tests/MiniCover.UnitTests/MiniCover.UnitTests.csproj
dotnet test tests/MiniCover.ApprovalTests/MiniCover.ApprovalTests.csproj
