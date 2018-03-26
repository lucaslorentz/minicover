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
    <DotNetCliToolReference Include="MiniCover" Version="2.0.0-ci-20180301063918" />
  </ItemGroup>
</Project>
```

Inside the tools folder, execute:
```shell
dotnet restore
dotnet minicover --help
```

After the last command you should see minicover help instructions on your console.

## Build script example with MiniCover
```shell
dotnet restore
dotnet build

cd tools

# Instrument assemblies inside 'test' folder to detect hits for source files inside 'src' folder
dotnet minicover instrument --workdir ../ --assemblies test/**/bin/**/*.dll --sources src/**/*.cs 

# Reset hits count in case minicover was run for this project
dotnet minicover reset

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

## Ignore Coverage Files
Add the following to your .gitignore file to ignore code coverage results:
```
coverage-html
coverage.json
coverage-hits.txt
```
