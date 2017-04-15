#!/bin/sh

set -e

dotnet build ..
dotnet run instrument --workdir .. --sources Test/**/*.cs --assemblies Test/**/*.dll
dotnet test --no-build ../Test/Test.csproj
dotnet run uninstrument --workdir ..
dotnet run htmlreport --workdir .. --threshold 90 
dotnet run report --workdir .. --threshold 90
