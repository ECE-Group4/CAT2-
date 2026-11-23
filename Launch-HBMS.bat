@echo off
setlocal enabledelayedexpansion
title Hospital Biomedical Maintenance System (HBMS) - Launcher

echo ================================================================================
echo   HOSPITAL BIOMEDICAL MAINTENANCE SYSTEM (HBMS) - WINDOWS LAUNCHER
echo   County Referral Hospital Biomedical Equipment Maintenance & SLA
echo ================================================================================
echo.

cd /d "%~dp0"

:: 1. Check / Create Microsoft Access Database
set "DB_PATH=%~dp0HBMS\database\HBMS.accdb"
if not exist "%DB_PATH%" (
    echo [*] Microsoft Access database not found. Initializing database and synthetic data...
    powershell -NoProfile -ExecutionPolicy Bypass -File "%~dp0HBMS\database\Create-HBMSDatabase.ps1"
    if errorlevel 1 (
        echo [!] Notice: Access ACE COM provider not detected. Setting fallback database environment.
    ) else (
        echo [OK] Access database created successfully at %DB_PATH%
    )
) else (
    echo [OK] Relational database located at %DB_PATH%
)

:: 2. Check / Build Visual Basic .NET Desktop Executable
set "EXE_PATH=%~dp0HBMS\desktop\HBMS.Desktop\bin\Debug\net8.0-windows\HBMS.Desktop.exe"
if not exist "%EXE_PATH%" (
    set "EXE_PATH=%~dp0HBMS\desktop\HBMS.Desktop\bin\Release\net8.0-windows\HBMS.Desktop.exe"
)

if not exist "%EXE_PATH%" (
    echo [*] Building Visual Basic .NET Desktop Application...
    dotnet build "%~dp0HBMS\desktop\HBMS.Desktop\HBMS.Desktop.vbproj" -c Release
    set "EXE_PATH=%~dp0HBMS\desktop\HBMS.Desktop\bin\Release\net8.0-windows\HBMS.Desktop.exe"
)

:: 3. Open Executive Web Portal in Default Browser
echo [*] Launching Executive Management Portal in default web browser...
start "" "%~dp0HBMS\portal\index.html"

:: 4. Launch Desktop Windows Forms Application
echo [*] Launching HBMS Desktop Operations Console...
echo.
echo ================================================================================
echo   DEFAULT INITIAL LOGIN CREDENTIALS:
echo     Username: admin
echo     Password: ChangeMe123!
echo ================================================================================
echo.

if exist "%EXE_PATH%" (
    start "" "%EXE_PATH%"
    echo [OK] Application launched successfully. You may close this window.
) else (
    echo [*] Running application via dotnet run...
    start "" dotnet run --project "%~dp0HBMS\desktop\HBMS.Desktop\HBMS.Desktop.vbproj"
)

timeout /t 5 >nul
exit /b 0
