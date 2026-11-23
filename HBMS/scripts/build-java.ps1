$ErrorActionPreference = 'Stop'
Push-Location (Join-Path $PSScriptRoot '..\java')
try { mvn clean package } finally { Pop-Location }
