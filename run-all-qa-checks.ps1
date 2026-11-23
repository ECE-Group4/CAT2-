# Root entrypoint for HBMS Unified Cross-Platform QA Suite (Windows PowerShell)
$ErrorActionPreference = "Stop"
$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
& "$ScriptDir\HBMS\scripts\run-all-qa-checks.ps1" @args
