#!/usr/bin/env bash

set -e

rm -r artifacts || true
rm -r coverage || true

dotnet build

echo "# Start Instrument"
./minicover.sh instrument --tests ""
echo "# End Instrument"

dotnet test --no-build

echo "# Start Uninstrument"
./minicover.sh uninstrument
echo "# End Uninstrument"

echo "# Start Report"
./minicover.sh report --threshold 0
echo "# End Report"

if [ -n "${TRAVIS_JOB_ID}" ]; then
	echo "# Start Coveralls Report"
	./minicover.sh coverallsreport \
		\
		--output "coveralls.json" \
		--service-name "travis-ci" \
		--service-job-id "$TRAVIS_JOB_ID"
	echo "# End Coveralls Report"
fi

echo "# Testing sample project"
rm -rd ~/.nuget/packages/minicover/1.0.0 || true
dotnet pack -c Release --output $PWD/sample/nupkgs
cd sample
rm -rf ./coverage
dotnet build
dotnet tool restore -v q
dotnet minicover reset
echo "# Start Instrument"
dotnet minicover instrument
echo "# End Instrument"
dotnet test --no-build
echo "# Start Uninstrument"
dotnet minicover uninstrument
echo "# End Uninstrument"
echo "# Start Report"
dotnet minicover report --threshold 60
echo "# End Report"
echo "# Start HtmlReport"
dotnet minicover htmlreport --threshold 60
echo "# End HtmlReport"
echo "# Start XmlReport"
dotnet minicover xmlreport --threshold 60
echo "# End XmlReport"
echo "# Start OpenCoverReport"
dotnet minicover opencoverreport --threshold 60
echo "# End OpenCoverReport"
echo "# Start CloverReport"
dotnet minicover cloverreport --threshold 60
echo "# End CloverReport"
cd ..
