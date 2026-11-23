#!/usr/bin/env bash
set -euo pipefail
ROOT="$(cd "$(dirname "$0")/.." && pwd)"
node --test "$ROOT/tests/portal/dashboard.test.js"
node --check "$ROOT/portal/js/dashboard.js"
python3 -m json.tool "$ROOT/portal/data/uptime.json" >/dev/null
echo "Portal checks completed."
