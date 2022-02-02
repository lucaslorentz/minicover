#!/usr/bin/env bash

dotnet run -p src/MiniCover/MiniCover.csproj -f net6.0 -- "$@"