#!/usr/bin/env bash

set -e

export "MiniCover=dotnet run -p src/MiniCover/MiniCover.csproj --"

dotnet restore
dotnet build
$MiniCover instrument --sources "test/**/*.cs" --assemblies "test/**/*.dll"
$MiniCover reset
dotnet test --no-build test/MiniCover.XUnit.Tests/MiniCover.XUnit.Tests.csproj
dotnet test --no-build test/MiniCover.NUnit.Tests/MiniCover.NUnit.Tests.csproj
$MiniCover uninstrument
$MiniCover htmlreport --threshold 90 
$MiniCover report --threshold 90

if [ "${TRAVIS_PULL_REQUEST}" = "false" ] && [ "${TRAVIS_BRANCH}" = "master" ]; then
	dotnet pack src/MiniCover -c Release --output $PWD/artifacts --version-suffix ci-`date +%Y%m%d%H%M%S`
	dotnet nuget push artifacts/* -k $NUGET_KEY -s https://www.nuget.org/api/v2/package
fi