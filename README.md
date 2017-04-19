# MiniCover
Minimalist Code Coverage Tool for .NET Core

[![Build Status](https://travis-ci.org/lucaslorentz/minicover.svg?branch=master)](https://travis-ci.org/lucaslorentz/minicover)

Heavily based on: https://github.com/gaillard/SharpCover

**THIS PROJECT IS WIP, THERE ARE ONLY PRE-RELEASE NUGET PACKAGES**

## Introduction
Currently no Code Coverage Tool supports .NET Core on Linux.

MiniCover is here to fill that gap. It is focused on simple code base, usage and installation.

## Installation
MiniCover was created to be installed as a .NET Cli Tool. You can do that by adding this to a csproj file:
```
  <ItemGroup>
    <DotNetCliToolReference Include="MiniCover" Version="1.0.0-ci-*" />
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
    <TargetFramework>netcoreapp1.1</TargetFramework>
  </PropertyGroup>
  <ItemGroup>
    <DotNetCliToolReference Include="MiniCover" Version="1.0.0-ci-*" />
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

# Instrument all assemblies inside 'test' folder to detect hits for source files inside 'src' folder
dotnet minicover instrument --workdir ../ --assemblies test/**/*.dll --sources src/**/*.cs 

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
