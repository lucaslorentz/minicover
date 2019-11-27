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
MiniCover can be installed as a global tool:
```
dotnet tool install --global minicover
```
Or local tool:
```
dotnet tool install minicover
```

## Commands
This is a simplified documentation of MiniCover commands and options.

Use `--help` for more information:
```
minicover --help
```

**When installed as local tool, MiniCover commands must be prefixed with `dotnet`.** Example:
```
dotnet minicover --help
```

### Instrument
```
minicover instrument
```

Use this command to instrument assemblies to record code coverage.

It is based on the following main options:

|option|description|type|default|
|-|-|-|-|
|**--sources**|source files to track coverage|glob|`src/**/*.cs`|
|**--exclude-sources**|exceptions to source option|glob|`**/bin/**/*.cs` and `**/obj/**/*.cs`|
|**--tests**|test files used to recognize test methods|glob|`tests/**/*.cs` and `test/**/*.cs`|
|**--exclude-tests**|exceptions to tests option|glob|`**/bin/**/*.cs` and `**/obj/**/*.cs`|
|**--assemblies**|assemblies considered for instrumentation. Assemblies not related to sources or tests are automatically ignored.|glob|`**/*.dll`|
|**--exclude-assemblies**|Exceptions to assemblies option|glob|`**/obj/**/*.dll`|

Note 1: *[Supported syntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.extensions.filesystemglobbing.matcher?view=dotnet-plat-ext-3.0#remarks) for glob values.*  
Note 2: *You can repeat glob options to represent multiple values. Example: `--sources "a/**/*.cs" --sources "b/**/*.cs"`*

This command also generates a **coverage.json** file with information about the instrumented code.   

### Uninstrument
```
minicover uninstrument
````

Use this command to revert the instrumentation based on **coverage.json** file.

**Make sure you call uninstrument before publishing or packing your application.**

### Reset
```
minicover reset
````

Use this command to reset the recorded coverage so far.

### Report
```
minicover report
````

Use this command to print a coverage report in the console.

The command exits with failure if the coverage doesn't meet a specific threshold (90% by default).

### More commands

- **cloverreport**: Write an Clover-formatted XML report to file
- **coberturareport**: Write a cobertura XML report to file
- **coverallsreport**: Prepare and/or submit coveralls reports
- **htmlreport**: Write html report to folder
- **opencoverreport**: Write an OpenCover-formatted XML report to file
- **xmlreport**: Write an NCover-formatted XML report to file

Use `--help` for more information.

## Build script example
```shell
dotnet restore
dotnet build

# Instrument
minicover instrument

# Reset hits
minicover reset

dotnet test --no-build

# Uninstrument
minicover uninstrument

# Create html reports inside folder coverage-html
minicover htmlreport --threshold 90

# Console report
minicover report --threshold 90
```

## Ignore coverage files

Add the following to your .gitignore file to ignore code coverage results:
```
coverage-html
coverage-hits
coverage.json
```
