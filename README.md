# MiniCover
Code Coverage Tool for .NET Core

[![Build Status](https://dev.azure.com/lucaslorentzlara/lucaslorentzlara/_apis/build/status/lucaslorentz.minicover?branchName=master)](https://dev.azure.com/lucaslorentzlara/lucaslorentzlara/_build/latest?definitionId=3&branchName=master)
[![Nuget](https://img.shields.io/nuget/v/minicover)](https://www.nuget.org/packages/MiniCover/)
[![Coverage Status](https://coveralls.io/repos/github/lucaslorentz/minicover/badge.svg?branch=master)](https://coveralls.io/github/lucaslorentz/minicover?branch=master)

## Supported .NET Core SDKs
- 2.1 (Global tool)
- 2.2 (Global tool)
- 3.0 (Global tool or local tool)

## Installation
MiniCover can be installed as a global or local .NET Tool.
```
dotnet tool install --global minicover
```
or
```
dotnet tool install minicover
```

## Commands
This is a simplified documentation of MiniCover commands and options.

Use `--help` for more information.

### Instrument
```
dotnet minicover instrument
```

Use this command to instrument assemblies to record code coverage.

It is based on the following main options:
- **sources**: source files to track coverage
- **tests**: test files used to recognize test methods
- **assemblies**: assemblies considered for instrumentation. Assemblies not related to sources or tests are automatically ignored.

You probabbly don't need to set those options because Minicover defaults covers a lot of cases:
- assemblies
  - include: `**/*.dll`
  - exclude: `**/obj/**/*.dll`
- sources
  - include: `src/**/*.cs`
  - exclude: `**/bin/**/*.cs` and `**/obj/**/*.cs`
- tests
  - include: `tests/**/*.cs` and `test/**/*.cs`
  - excludes: `**/bin/**/*.cs` and `**/obj/**/*.cs`

This command also generates a **coverage.json** file with information about the instrumented code.

### Uninstrument
```
dotnet minicover uninstrument
````

Use this command to revert the instrumentation based on **coverage.json** file.

**Make sure you call uninstrument before publishing or packing your application.**

### Reset
```
dotnet minicover reset
````

Use this command to reset the recorded coverage so far.

### Report
```
dotnet minicover report
````

Use this command to print a coverage report in the console.

The command exits with failure if the coverage doesn't meet a specific threshold (90% by default).

### More commands

- **cloverreport**: Write an Clover-formatted XML report to file
- **coverallsreport**: Prepare and/or submit coveralls reports
- **htmlreport**: Write html report to folder
- **opencoverreport**: Write an OpenCover-formatted XML report to file
- **xmlreport**: Write an NCover-formatted XML report to file

Use `--help` for more information.

## Build script example with MiniCover
```shell
dotnet restore
dotnet build

# Instrument
dotnet minicover instrument

# Reset hits
dotnet minicover reset

dotnet test --no-build

# Uninstrument
dotnet minicover uninstrument

# Create html reports inside folder coverage-html
dotnet minicover htmlreport --threshold 90

# Console report
dotnet minicover report --threshold 90
```

## Ignore coverage files

Add the following to your .gitignore file to ignore code coverage results:
```
coverage-html
coverage-hits
coverage.json
```
