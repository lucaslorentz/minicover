#!/usr/bin/env bash

set -e

dotnet restore

dotnet pack -c Release --output $PWD/artifacts --version-suffix ci-`date +%Y%m%d%H%M%S`

dotnet test tests/MiniCover.HitServices.UnitTests/MiniCover.HitServices.UnitTests.csproj
dotnet test tests/MiniCover.UnitTests/MiniCover.UnitTests.csproj

echo "### Running sample build"
cd sample
./build.sh
cd ..

if [ "${TRAVIS_PULL_REQUEST}" = "false" ] && [ "${TRAVIS_BRANCH}" = "master" ]; then	
	echo "### Publishing packages"
	dotnet nuget push artifacts/*.nupkg -k $NUGET_KEY -s https://api.nuget.org/v3/index.json
fi
