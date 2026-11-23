@echo off
setlocal
title HBMS - Simulate Ward Terminal Alert

echo ================================================================================
echo   HBMS - SIMULATE WARD NETWORK ALERT
echo ================================================================================
echo.

cd /d "%~dp0"

echo Generating urgent clinical alert for ICU Ventilator (ASSET-VENT-001)...

powershell -NoProfile -ExecutionPolicy Bypass -Command ^
  "$inbox = Join-Path '%~dp0' 'HBMS\integration\inbox\alerts.jsonl';" ^
  "$dir = Split-Path $inbox;" ^
  "if (-not (Test-Path $dir)) { New-Item -ItemType Directory -Path $dir -Force | Out-Null };" ^
  "$guid = [System.Guid]::NewGuid().ToString();" ^
  "$now = [DateTime]::UtcNow.ToString('o');" ^
  "$msg = @{ messageId = $guid; assetCode = 'ASSET-VENT-001'; wardCode = 'ICU'; severity = 'CRITICAL'; description = 'Ventilator high airway pressure and oxygen blender failure'; timestamp = $now };" ^
  "$json = $msg | ConvertTo-Json -Compress;" ^
  "[System.IO.File]::AppendAllLines($inbox, @($json));" ^
  "Write-Host '[OK] Ward alert appended to inbox: ' $guid -ForegroundColor Green;"

echo.
echo ================================================================================
echo   Next Step in Desktop Application:
echo   1. In the HBMS Desktop Operations Console, click 'Import ward alerts'.
echo   2. Watch the alert automatically convert into a Critical Care breakdown!
echo ================================================================================
echo.
pause
