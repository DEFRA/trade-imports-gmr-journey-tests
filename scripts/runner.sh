#!/usr/bin/env bash
set -euo pipefail

root_dir="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
report_dir="$root_dir/test-report"
results_dir="$report_dir/results"
trx_file="$results_dir/test-results.trx"
html_report="$report_dir/index.html"

rm -rf "$report_dir"

dotnet test --logger "trx;LogFileName=$(basename "$trx_file")" --results-directory "$results_dir"

mkdir -p "$report_dir"
dotnet tool run trxlog2html -i "$trx_file" -o "$html_report"

echo "Test report generated at $html_report"

publish_script="$root_dir/scripts/publish-tests.sh"
"$publish_script"
