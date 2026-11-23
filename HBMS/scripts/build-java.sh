#!/usr/bin/env sh
set -eu
cd "$(dirname "$0")/../java"
mvn clean package
