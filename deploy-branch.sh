#!/bin/bash

set -e

if [ "${TRAVIS_PULL_REQUEST}" = "false" ] && [ "${TRAVIS_BRANCH}" = "master" ]; then
  echo "Deploying CI..."
  
  export Version=$(cat version)-ci-$(date +%Y%m%d%H%M%S)
  dotnet pack -c Release --output $PWD/artifacts/ci

  #dotnet nuget push artifacts/ci/*.nupkg -k $MYFEED_TOKEN -s https://www.myget.org/F/minicover/api/v3/index.json
  dotnet nuget push artifacts/ci/*.nupkg -k $NUGET_KEY -s https://api.nuget.org/v3/index.json
else
  echo "Skipping CI deploy"
fi
