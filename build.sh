#!/usr/bin/env bash

set -e

rm -r artifacts || true

dotnet restore

dotnet build tests/MiniCover.HitServices.UnitTests/MiniCover.HitServices.UnitTests.csproj
dotnet build tests/MiniCover.UnitTests/MiniCover.UnitTests.csproj
dotnet build tests/MiniCover.ApprovalTests/MiniCover.ApprovalTests.csproj

export MiniCover="dotnet run -p src/MiniCover/MiniCover.csproj --"
$MiniCover instrument --workdir ./coverage --parentdir ../ --assemblies "tests/**/bin/**/*.dll" --sources "src/**/*.cs"

dotnet test --no-build tests/MiniCover.HitServices.UnitTests/MiniCover.HitServices.UnitTests.csproj
dotnet test --no-build tests/MiniCover.UnitTests/MiniCover.UnitTests.csproj
dotnet test --no-build tests/MiniCover.ApprovalTests/MiniCover.ApprovalTests.csproj

$MiniCover report --workdir ./coverage --threshold 0

if [ -n "${TRAVIS_JOB_ID}" ]; then
	$MiniCover coverallsreport \
		--workdir ./coverage \
		--root-path ../ \
		--output "coveralls.json" \
		--service-name "travis-ci" \
		--service-job-id "$TRAVIS_JOB_ID"
fi
