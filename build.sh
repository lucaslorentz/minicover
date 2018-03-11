#!/usr/bin/env bash

set -e

export "MiniCover=dotnet run -p src/MiniCover/MiniCover.csproj --"

dotnet restore
dotnet build
$MiniCover instrument --sources "test/**/*.cs" --assemblies "test/**/bin/**/*.dll"
$MiniCover reset --solutiondir ./
dotnet test --no-build test/MiniCover.XUnit.Tests/MiniCover.XUnit.Tests.csproj
dotnet test --no-build test/MiniCover.NUnit.Tests/MiniCover.NUnit.Tests.csproj
$MiniCover uninstrument
$MiniCover htmlreport --threshold 90 
$MiniCover xmlreport --threshold 90
$MiniCover report --threshold 90
$MiniCover opencoverreport --threshold 90

if [ "${TRAVIS_PULL_REQUEST}" = "false" ] && [ "${TRAVIS_BRANCH}" = "master" ]; then
	dotnet pack src/MiniCover -c Release --output $PWD/artifacts --version-suffix ci-`date +%Y%m%d%H%M%S`
	dotnet nuget push artifacts/*.nupkg -k $NUGET_KEY -s https://api.nuget.org/v3/index.json
fi
