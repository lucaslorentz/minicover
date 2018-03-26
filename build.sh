#!/usr/bin/env bash

set -e

export "MiniCover=dotnet run -p src/MiniCover/MiniCover.csproj --"

dotnet restore

dotnet pack -c Release --output $PWD/artifacts --version-suffix ci-`date +%Y%m%d%H%M%S`

dotnet build tests/MiniCover.HitServices.UnitTests/MiniCover.HitServices.UnitTests.csproj
dotnet build tests/MiniCover.UnitTests/MiniCover.UnitTests.csproj

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

if [ "${TRAVIS_PULL_REQUEST}" = "false" ] && [ "${TRAVIS_BRANCH}" = "master" ]; then	
	echo "### Publishing packages"
	dotnet nuget push artifacts/*.nupkg -k $NUGET_KEY -s https://api.nuget.org/v3/index.json
fi
