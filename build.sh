#!/usr/bin/env bash

set -e

rm -r artifacts || true
rm -r coverage || true

dotnet build

echo "# Start Instrument"
./minicover.sh instrument --assemblies "**/net10.0/**/*.dll" --tests ""
echo "# End Instrument"

./minicover.sh reset

dotnet test --no-build

echo "# Start Uninstrument"
./minicover.sh uninstrument
echo "# End Uninstrument"

echo "# Start Report"
./minicover.sh report --threshold 34
echo "# End Report"

echo "# Start HtmlReport"
./minicover.sh htmlreport --threshold 34
echo "# End HtmlReport"

echo "# Start CoberturaReport"
./minicover.sh coberturareport
echo "# End CoberturaReport"

if [ -n "${GITHUB_RUN_ID}" ] && [ -n "${COVERALLS_REPO_TOKEN}" ]; then
	last_commit_message=$(git log -1 --pretty=format:"%s")
	last_commit_author_name=$(git log -1 --pretty=format:"%an")
	last_commit_author_email=$(git log -1 --pretty=format:"%ae")

	echo "# Start Coveralls Report"
	./minicover.sh coverallsreport \
		--service-name "github" \
		--repo-token "$COVERALLS_REPO_TOKEN" \
		--service-job-id "$GITHUB_RUN_ID" \
		--commit "$GITHUB_SHA" \
		--commit-message "$last_commit_message" \
		--commit-author-name "$last_commit_author_name" \
		--commit-author-email "$last_commit_author_email" \
		--branch "$GITHUB_REF" \
		--remote "origin" \
		--remote-url "https://github.com/lucaslorentz/minicover" || echo ""
	echo "# End Coveralls Report"
fi
