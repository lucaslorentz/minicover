#!/usr/bin/env bash

set -e

dotnet restore --no-cache
dotnet build

cd tools

# Instrument assemblies inside 'test' folder to detect hits for source files inside 'src' folder
dotnet minicover instrument --workdir ../coverage --parentdir ../ --assemblies **/bin/**/*.dll --sources **/*.cs 

# Reset hits count in case minicover was run for this project
dotnet minicover reset --workdir ../coverage

cd -

for project in test/**/*.csproj; do dotnet test --no-build $project; done

cd tools

# Uninstrument assemblies, it's important if you're going to publish or deploy build outputs
dotnet minicover uninstrument --workdir ../coverage

# This command returns failure if the coverage is lower than the threshold
dotnet minicover report --workdir ../coverage --threshold 80 --verbosity Normal
dotnet minicover report --workdir ../coverage --threshold 80 --verbosity Quiet

# Generate other reports
dotnet minicover htmlreport --workdir ../coverage --threshold 80
dotnet minicover xmlreport --workdir ../coverage --threshold 80
dotnet minicover opencoverreport --workdir ../coverage --threshold 80
dotnet minicover cloverreport --workdir ../coverage --threshold 80

cd ..
