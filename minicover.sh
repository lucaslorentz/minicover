#!/usr/bin/env bash

dotnet run -p src/MiniCover/MiniCover.csproj -f netcoreapp3.0 -- "$@"