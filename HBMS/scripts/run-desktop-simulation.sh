#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_ROOT="$(cd "${SCRIPT_DIR}/../.." && pwd)"

DOTNET_BIN="dotnet"
if [ -x "/home/skywalker/.dotnet/dotnet" ]; then
    DOTNET_BIN="/home/skywalker/.dotnet/dotnet"
elif ! command -v dotnet >/dev/null 2>&1; then
    echo "ERROR: dotnet CLI is not installed or not in PATH."
    exit 1
fi

echo "=== Executing HBMS Cross-Platform CLI Simulation ==="
"${DOTNET_BIN}" run --project "${PROJECT_ROOT}/HBMS/desktop/HBMS.Cli/HBMS.Cli.vbproj"
