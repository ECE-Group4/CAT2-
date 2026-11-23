#!/usr/bin/env sh
set -eu
ROOT="$(CDPATH= cd -- "$(dirname "$0")/.." && pwd)"
if [ -f "$ROOT/java/target/hbms-alerts-1.0.0.jar" ]; then
  java -jar "$ROOT/java/target/hbms-alerts-1.0.0.jar" --send --host 127.0.0.1 --port 5055 --asset EQ-VENT-001 --ward ICU --severity CRITICAL --description "Synthetic test alert: ventilator not powering on"
else
  (cd "$ROOT/java/src/main/java" && java org/hbms/alerts/AlertApplication.java --send --host 127.0.0.1 --port 5055 --asset EQ-VENT-001 --ward ICU --severity CRITICAL --description "Synthetic test alert: ventilator not powering on")
fi
