#!/usr/bin/env bash

set -e

rm -r artifacts || true

dotnet restore

export Version=$(cat version)-local-$(date +%Y%m%d%H%M%S)
dotnet pack -c Release --output $PWD/artifacts/local

dotnet build tests/MiniCover.HitServices.UnitTests/MiniCover.HitServices.UnitTests.csproj
dotnet build tests/MiniCover.UnitTests/MiniCover.UnitTests.csproj

export "MiniCover=dotnet run -p src/MiniCover/MiniCover.csproj --"
$MiniCover instrument --workdir ./coverage --parentdir ../ --assemblies "tests/**/bin/**/*.dll" --sources "src/**/*.cs"

dotnet test --no-build tests/MiniCover.HitServices.UnitTests/MiniCover.HitServices.UnitTests.csproj
dotnet test --no-build tests/MiniCover.UnitTests/MiniCover.UnitTests.csproj

$MiniCover report --workdir ./coverage --threshold 0

$MiniCover coverallsreport \
	--workdir ./coverage \
	--root-path ../ \
	--output "coveralls.json" \
	--service-name "travis-ci" \
	--service-job-id "$TRAVIS_JOB_ID"

echo "### Running sample build"
cd sample
./build.sh
cd ..
