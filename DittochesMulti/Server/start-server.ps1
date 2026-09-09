param([string]$ListenAddress = '127.0.0.1', [int]$Port = 7777)
$ErrorActionPreference = 'Stop'
$pythonExe = Join-Path $env:LOCALAPPDATA 'Python\pythoncore-3.14-64\python.exe'
if (Test-Path -LiteralPath $pythonExe) {
    & $pythonExe (Join-Path $PSScriptRoot 'server.py') --host $ListenAddress --port $Port
} else {
    python (Join-Path $PSScriptRoot 'server.py') --host $ListenAddress --port $Port
}
