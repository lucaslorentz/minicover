#!/usr/bin/env bash

set -e

export "MiniCover=dotnet run -p src/MiniCover/MiniCover.csproj --"

dotnet restore
dotnet build

for project in tests/**/*.csproj; do dotnet test --no-build $project; done

dotnet pack -c Release --output $PWD/artifacts --version-suffix ci-`date +%Y%m%d%H%M%S`

echo "### Running sample build"
cd sample
./build.sh
cd ..

if [ "${TRAVIS_PULL_REQUEST}" = "false" ] && [ "${TRAVIS_BRANCH}" = "master" ]; then	
	echo "### Publishing packages"
	dotnet nuget push artifacts/*.nupkg -k $NUGET_KEY -s https://api.nuget.org/v3/index.json
fi
