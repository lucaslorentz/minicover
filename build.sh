#!/usr/bin/env bash

set -e

rm -r artifacts || true
rm -r coverage || true

dotnet build

echo "# Start Instrument"
./minicover.sh instrument --assemblies "**/netcoreapp3.0/**/*.dll" --tests ""
echo "# End Instrument"

./minicover.sh reset

dotnet test --no-build

echo "# Start Uninstrument"
./minicover.sh uninstrument
echo "# End Uninstrument"

echo "# Start CoberturaReport"
./minicover.sh coberturareport --threshold 0
echo "# End CoberturaReport"

echo "# Start Report"
./minicover.sh report --threshold 0
echo "# End Report"

if [ -n "${BUILD_BUILDID}" ] && [ -n "${COVERALLS_REPO_TOKEN}" ]; then
	echo "# Start Coveralls Report"
	./minicover.sh coverallsreport \
		--service-name "azure-devops" \
		--repo-token "$COVERALLS_REPO_TOKEN" \
		--commit "$BUILD_SOURCEVERSION" \
		--commit-message "$BUILD_SOURCEVERSIONMESSAGE" \
		--branch "$BUILD_SOURCEBRANCHNAME" \
		--remote "origin" \
		--remote-url "${BUILD_REPOSITORY_URI}"
	echo "# End Coveralls Report"
fi
