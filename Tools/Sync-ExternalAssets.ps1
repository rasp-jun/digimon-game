param(
    [ValidateSet('Backup','Restore')]
    [string]$Mode = 'Restore',
    [string]$AssetVault = (Join-Path (Split-Path $PSScriptRoot -Parent) 'digimon-game-assets-cloud')
)

$repoRoot = Split-Path $PSScriptRoot -Parent
$projectResources = Join-Path $repoRoot 'DittochesMulti\Assets\Resources'
$vaultResources = Join-Path $AssetVault 'Resources'

if ($Mode -eq 'Backup') {
    New-Item -ItemType Directory -Force -Path $vaultResources | Out-Null
    Get-ChildItem -LiteralPath $projectResources | Copy-Item -Destination $vaultResources -Recurse -Force
    Write-Host "External art backed up to: $vaultResources"
    exit 0
}

if (-not (Test-Path -LiteralPath $vaultResources)) {
    throw "Asset vault not found: $vaultResources"
}

New-Item -ItemType Directory -Force -Path $projectResources | Out-Null
Get-ChildItem -LiteralPath $vaultResources | Copy-Item -Destination $projectResources -Recurse -Force
Write-Host "External art restored to: $projectResources"
