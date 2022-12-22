#!/usr/bin/env bash

dotnet run -p src/MiniCover/MiniCover.csproj -f net7.0 -- "$@"