#!/usr/bin/env bash
# Root entrypoint for HBMS Unified Cross-Platform QA Suite
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
exec bash "${SCRIPT_DIR}/HBMS/scripts/run-all-qa-checks.sh" "$@"
