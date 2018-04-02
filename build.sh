#!/usr/bin/env bash

set -e

rm -r artifacts || true

dotnet restore

export Version=$(cat version)-local-$(date +%Y%m%d%H%M%S)
dotnet pack -c Release --output $PWD/artifacts/local

dotnet test tests/MiniCover.HitServices.UnitTests/MiniCover.HitServices.UnitTests.csproj
dotnet test tests/MiniCover.UnitTests/MiniCover.UnitTests.csproj

echo "### Running sample build"
cd sample
./build.sh
cd ..
