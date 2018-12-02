# MiniCover
Minimalist Code Coverage Tool for .NET Core

[![Build Status](https://travis-ci.org/lucaslorentz/minicover.svg?branch=master)](https://travis-ci.org/lucaslorentz/minicover)
[![Build status](https://ci.appveyor.com/api/projects/status/wtoyadiphqee8hy0/branch/master?svg=true)](https://ci.appveyor.com/project/lucaslorentz/minicover/branch/master)
[![Coverage Status](https://coveralls.io/repos/github/lucaslorentz/minicover/badge.svg?branch=master)](https://coveralls.io/github/lucaslorentz/minicover?branch=master)

Heavily based on: https://github.com/gaillard/SharpCover

**THIS PROJECT IS WIP, THERE ARE ONLY PRE-RELEASE NUGET PACKAGES**

## Introduction
Currently no Code Coverage Tool supports .NET Core on Linux.

MiniCover is here to fill that gap. It is focused on simple code base, usage and installation.

## Installation
MiniCover was created to be installed as a .NET Cli Tool. You can do that by adding this to a csproj file, replacing VERSION by the latest version available on [Nuget](https://www.nuget.org/packages/MiniCover/):
```
  <ItemGroup>
    <DotNetCliToolReference Include="MiniCover" Version="2.0.0-ci-VERSION" />
  </ItemGroup>
```

While .NET Core SDK doesn't support CLI Tools at solution level, I recommend the following approach:

Create an empty .NET Core project called **tools** on your solution. If you follow the .NET community convention,
you should have a folder structure similar to:
```
MyRepository
  src
    ...
  test
    ...
  tools
     tools.csproj
  MySolution.sln
```

The **tools.csproj** will be similar to:
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>netcoreapp2.0</TargetFramework>
  </PropertyGroup>
  <ItemGroup>
    <DotNetCliToolReference Include="MiniCover" Version="2.0.0-ci-*" />
  </ItemGroup>
</Project>
```

Inside the tools folder, execute:
```shell
dotnet restore
dotnet minicover --help
```

After the last command you should see minicover help instructions on your console.

## Nuget packages
Continuous integration nuget packages are pushed to:
https://www.myget.org/feed/minicover/package/nuget/MiniCover

Pre-releases and releases nuget packages are pushed to:
https://www.nuget.org/packages/MiniCover

## Build script examples with MiniCover

### Bash

```shell
dotnet restore
dotnet build

cd tools

# Instrument assemblies inside 'test' folder to detect hits for source files inside 'src' folder
dotnet minicover instrument --workdir ../ --assemblies test/**/bin/**/*.dll --sources src/**/*.cs 

# Reset hits count in case minicover was run for this project
dotnet minicover reset --workdir ../

cd ..

for project in test/**/*.csproj; do dotnet test --no-build $project; done

cd tools

# Uninstrument assemblies, it's important if you're going to publish or deploy build outputs
dotnet minicover uninstrument --workdir ../

# Create html reports inside folder coverage-html
dotnet minicover htmlreport --workdir ../ --threshold 90

# Print console report
# This command returns failure if the coverage is lower than the threshold
dotnet minicover report --workdir ../ --threshold 90

cd ..
```

### PowerShell
```powershell
dotnet restore
dotnet build
cd tools
dotnet restore
dotnet minicover instrument --workdir ../ --assemblies test/**/bin/**/*.dll --sources src/**/*.cs
dotnet minicover reset --workdir ../
cd ..\test
gci -Filter *.csproj -Recurse | %{
    dotnet test $_.FullName
}
cd ..\tools
dotnet minicover uninstrument --workdir ../
dotnet minicover htmlreport --workdir ../ --threshold 90
dotnet minicover report --workdir ../ --threshold 90
cd ..
```

## Ignore Coverage Files
Add the following to your .gitignore file to ignore code coverage results:
```
coverage-html
coverage.json
coverage.hits
```
