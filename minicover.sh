#!/usr/bin/env bash

dotnet run --project src/MiniCover/MiniCover.csproj -f net10.0 -- "$@"