#!/usr/bin/env bash

dotnet run --project src/MiniCover/MiniCover.csproj -f net8.0 -- "$@"