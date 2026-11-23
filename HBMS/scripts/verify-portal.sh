#!/usr/bin/env sh
set -eu
ROOT="$(CDPATH= cd -- "$(dirname "$0")/.." && pwd)"
node --check "$ROOT/portal/js/dashboard.js"
python3 -m json.tool "$ROOT/portal/data/uptime.json" >/dev/null
printf 'Portal JavaScript syntax and uptime JSON are valid.\n'
