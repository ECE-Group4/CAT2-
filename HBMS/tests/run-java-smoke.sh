#!/usr/bin/env bash
set -euo pipefail
ROOT="$(cd "$(dirname "$0")/.." && pwd)"

if command -v javac >/dev/null 2>&1; then
  OUT="$(mktemp -d)"
  trap 'rm -rf "$OUT"' EXIT
  javac -d "$OUT" \
    "$ROOT/java/src/main/java/org/hbms/alerts/AlertMessage.java" \
    "$ROOT/tests/java/AlertMessageSmokeTest.java"
  java -cp "$OUT" org.hbms.alerts.AlertMessageSmokeTest
else
  WORK="$(mktemp -d)"
  trap 'rm -rf "$WORK"' EXIT
  mkdir -p "$WORK/org/hbms/alerts"
  cp "$ROOT/java/src/main/java/org/hbms/alerts/"*.java "$WORK/org/hbms/alerts/"
  cp "$ROOT/tests/java/AlertMessageSmokeTest.java" "$WORK/org/hbms/alerts/"
  (cd "$WORK" && java org/hbms/alerts/AlertMessageSmokeTest.java)
fi
