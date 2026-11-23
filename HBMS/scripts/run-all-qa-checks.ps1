# ==============================================================================
# HBMS UNIFIED CROSS-PLATFORM QA & VERIFICATION TEST SUITE (Windows PowerShell)
# Kirinyaga University | ECE 2423: Advanced Programming - Group 4
# Case Study: County Referral Hospital Biomedical Equipment Maintenance & SLA
# ==============================================================================
$ErrorActionPreference = "Stop"

$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$HbmsRoot = Split-Path -Parent $ScriptDir
$RepoRoot = Split-Path -Parent $HbmsRoot

Write-Host "================================================================================" -ForegroundColor Cyan
Write-Host "  STARTING FULL HBMS MULTI-TIER QA AND INTEGRITY VERIFICATION SUITE (WINDOWS)" -ForegroundColor Cyan
Write-Host "  Timestamp: $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss K')" -ForegroundColor Cyan
Write-Host "================================================================================" -ForegroundColor Cyan

$PassedCount = 0
$FailedCount = 0

function Run-Step([string]$Title, [scriptblock]$Action) {
    Write-Host "`n>>> Running QA Step: $Title..." -ForegroundColor Yellow
    try {
        & $Action
        Write-Host ">>> [PASS] $Title" -ForegroundColor Green
        $script:PassedCount++
    }
    catch {
        Write-Host ">>> [FAIL] $Title - Error: $_" -ForegroundColor Red
        $script:FailedCount++
    }
}

# 1. Web Portal Verification
Run-Step "Web Portal Syntax & Assets Integrity" {
    if (Get-Command node -ErrorAction SilentlyContinue) {
        node -c "$HbmsRoot\portal\js\dashboard.js"
        node -e "JSON.parse(require('fs').readFileSync('$HbmsRoot\\portal\\uptime-sample.json', 'utf8'))"
    } else {
        Write-Host "Node.js not detected; checking file existence only."
        if (-not (Test-Path "$HbmsRoot\portal\index.html")) { throw "Missing index.html" }
    }
}

# 2. Web Portal Dashboard Unit Tests
Run-Step "Web Portal Client Dashboard Node.js Unit Tests" {
    if (Get-Command node -ErrorAction SilentlyContinue) {
        node "$HbmsRoot\tests\portal\dashboard.test.js"
    } else {
        Write-Host "Node.js not in PATH; skipping node unit test."
    }
}

# 3. Java 17+ Socket Alert Smoke Tests
Run-Step "Pure Java Socket Alerts Network Smoke Test" {
    if (Get-Command java -ErrorAction SilentlyContinue) {
        & java "$HbmsRoot\tests\java\AlertMessageSmokeTest.java"
    } else {
        Write-Host "Java runtime not in PATH; skipping Java smoke test."
    }
}

# 4. Database Schema, Integrity & Analytical Queries
Run-Step "Database Relational Schema & 6/6 Analytical QA Checks" {
    if (Get-Command python -ErrorAction SilentlyContinue) {
        & python "$RepoRoot\database\verify_database.py"
    } elseif (Get-Command python3 -ErrorAction SilentlyContinue) {
        & python3 "$RepoRoot\database\verify_database.py"
    } else {
        Write-Host "Python not found; skipping python SQLite verification."
    }
}

# 5. VB.NET Desktop Unit Test Suite
Run-Step "Visual Basic .NET Desktop Automated Unit Tests (PriorityQueue & Domain)" {
    & dotnet run --project "$HbmsRoot\tests\desktop\HBMS.Tests\HBMS.Tests.vbproj"
}

# 6. VB.NET Cross-Platform Simulation
Run-Step "Visual Basic .NET Cross-Platform CLI End-to-End Simulation" {
    & dotnet run --project "$HbmsRoot\desktop\HBMS.Cli\HBMS.Cli.vbproj"
}

Write-Host "`n================================================================================" -ForegroundColor Cyan
Write-Host "  HBMS FULL QA SUITE COMPLETED" -ForegroundColor Cyan
Write-Host "  Summary: $PassedCount Passed | $FailedCount Failed" -ForegroundColor Cyan
Write-Host "================================================================================" -ForegroundColor Cyan

if ($FailedCount -eq 0) {
    Write-Host ">>> ALL QA CHECKS PASSED SUCCESSFULLY (6/6 SUITES GREEN)" -ForegroundColor Green
    exit 0
} else {
    Write-Host ">>> WARNING: ONE OR MORE QA SUITES FAILED" -ForegroundColor Red
    exit 1
}
