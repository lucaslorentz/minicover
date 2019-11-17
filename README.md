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

## Supported .NET core sdks
- 2.1 (Global tool)
- 2.2 (Global tool)
- 3.0 (Global tool or local tool)

## Command line instructions
For global tools execute:
```
minicover --help
```
For local tools execute:
```
dotnet minicover --help
```

## Nuget packages
Nuget packages are available at:
https://www.nuget.org/packages/MiniCover

## Build script example with MiniCover
```shell
dotnet restore
dotnet build

# Instrument assemblies 'tests/**/bin/**.dll' to track hits for source files 'src/**/*.cs' and recognize test methods from 'tests/**/*.cs'
dotnet minicover instrument

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
