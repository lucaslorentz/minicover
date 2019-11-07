# MiniCover
Code Coverage Tool for .NET Core

[![Build Status](https://travis-ci.org/lucaslorentz/minicover.svg?branch=master)](https://travis-ci.org/lucaslorentz/minicover)
[![Build status](https://ci.appveyor.com/api/projects/status/wtoyadiphqee8hy0/branch/master?svg=true)](https://ci.appveyor.com/project/lucaslorentz/minicover/branch/master)
[![Coverage Status](https://coveralls.io/repos/github/lucaslorentz/minicover/badge.svg?branch=master)](https://coveralls.io/github/lucaslorentz/minicover?branch=master)

## Installation
MiniCover can be installed as a global or local .NET Cli Tool.
```
dotnet tool install -g minicover
```
or
```
dotnet tool install minicover
```

## Nuget packages
Continuous integration nuget packages are pushed to:
https://www.myget.org/feed/minicover/package/nuget/MiniCover

Pre-releases and releases nuget packages are pushed to:
https://www.nuget.org/packages/MiniCover

## Build script example with MiniCover
```shell
dotnet restore
dotnet build

# Instrument assemblies inside 'test' folder to detect hits for source files inside 'src' folder
dotnet minicover instrument --assemblies test/**/bin/**/*.dll --sources src/**/*.cs 

# Reset hits count in case minicover was run for this project
dotnet minicover reset

dotnet test --no-build

# Uninstrument assemblies, it's important if you're going to publish or deploy build outputs
dotnet minicover uninstrument

# Create html reports inside folder coverage-html
dotnet minicover htmlreport --threshold 90

# Print console report
# This command returns failure if the coverage is lower than the threshold
dotnet minicover report --threshold 90
```

## Ignore Coverage Files
Add the following to your .gitignore file to ignore code coverage results:
```
coverage-html
coverage-hits
coverage.json
```
