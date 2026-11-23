@echo off
setlocal
title HBMS - Full Cross-Platform QA Verification Suite

echo ================================================================================
echo   HBMS - RUNNING FULL SYSTEM QA AND VERIFICATION SUITE
echo ================================================================================
echo.

cd /d "%~dp0"

powershell -NoProfile -ExecutionPolicy Bypass -File "%~dp0run-all-qa-checks.ps1"

echo.
echo ================================================================================
echo   QA Verification Suite Completed. Press any key to exit.
echo ================================================================================
pause >nul
