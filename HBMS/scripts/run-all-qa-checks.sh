#!/usr/bin/env bash
# ==============================================================================
# HBMS UNIFIED CROSS-PLATFORM QA & VERIFICATION TEST SUITE (Linux/macOS)
# Kirinyaga University | ECE 2423: Advanced Programming - Group 4
# Case Study: County Referral Hospital Biomedical Equipment Maintenance & SLA
# ==============================================================================
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
HBMS_ROOT="$(cd "${SCRIPT_DIR}/.." && pwd)"
REPO_ROOT="$(cd "${HBMS_ROOT}/.." && pwd)"

echo "================================================================================"
echo "  STARTING FULL HBMS MULTI-TIER QA AND INTEGRITY VERIFICATION SUITE"
echo "  Timestamp: $(date -u +"%Y-%m-%d %H:%M:%SZ")"
echo "================================================================================"

PASSED_COUNT=0
FAILED_COUNT=0

run_step() {
    local step_title="$1"
    local command_str="$2"

    echo ""
    echo ">>> Running QA Step: ${step_title}..."
    if eval "${command_str}"; then
        echo ">>> [PASS] ${step_title}"
        PASSED_COUNT=$((PASSED_COUNT + 1))
    else
        echo ">>> [FAIL] ${step_title}"
        FAILED_COUNT=$((FAILED_COUNT + 1))
    fi
}

# 1. Web Portal Verification
run_step "Web Portal HTML/CSS/JS Lint & Uptime Data Syntax" \
    "bash '${HBMS_ROOT}/scripts/verify-portal.sh'"

# 2. Web Portal Dashboard Unit Tests
run_step "Web Portal Client Dashboard Node.js Unit Tests" \
    "bash '${HBMS_ROOT}/tests/run-portal-tests.sh'"

# 3. Java 17+ Socket Alert Smoke Tests
run_step "Pure Java Socket Alerts Network Smoke Test" \
    "bash '${HBMS_ROOT}/tests/run-java-smoke.sh'"

# 4. Database Schema, Integrity & Analytical Queries
run_step "Database Relational Schema & 6/6 Analytical QA Checks" \
    "python3 '${REPO_ROOT}/database/verify_database.py'"

# 5. VB.NET Desktop Unit Test Suite
run_step "Visual Basic .NET Desktop Automated Unit Tests (PriorityQueue & Domain)" \
    "bash '${HBMS_ROOT}/tests/run-desktop-tests.sh'"

# 6. VB.NET Cross-Platform Simulation
run_step "Visual Basic .NET Cross-Platform CLI End-to-End Simulation" \
    "bash '${HBMS_ROOT}/scripts/run-desktop-simulation.sh'"

echo ""
echo "================================================================================"
echo "  HBMS FULL QA SUITE COMPLETED"
echo "  Summary: ${PASSED_COUNT} Passed | ${FAILED_COUNT} Failed"
echo "================================================================================"

if [ "${FAILED_COUNT}" -eq 0 ]; then
    echo ">>> ALL QA CHECKS PASSED SUCCESSFULLY (6/6 SUITES GREEN)"
    exit 0
else
    echo ">>> WARNING: ONE OR MORE QA SUITES FAILED"
    exit 1
fi
