$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot '..\desktop\HBMS.Desktop\HBMS.Desktop.vbproj'
dotnet restore $project
dotnet build $project -c Release
