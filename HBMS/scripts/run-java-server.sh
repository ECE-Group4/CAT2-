#!/usr/bin/env sh
set -eu
ROOT="$(CDPATH= cd -- "$(dirname "$0")/.." && pwd)"
if [ -f "$ROOT/java/target/hbms-alerts-1.0.0.jar" ]; then
  java -jar "$ROOT/java/target/hbms-alerts-1.0.0.jar" --server --host 0.0.0.0 --port 5055 --inbox "$ROOT/integration/inbox/alerts.jsonl"
else
  (cd "$ROOT/java/src/main/java" && java org/hbms/alerts/AlertApplication.java --server --host 0.0.0.0 --port 5055 --inbox "$ROOT/integration/inbox/alerts.jsonl")
fi
