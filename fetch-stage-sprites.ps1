param([string[]]$Names = @(
  'Koromon','Tunomon','Mochimon','Tanemon','Pyocomon','Tokomon','Atlur_Kabuterimon',
  'Kuwagamon','Shellmon','Devimon','Etemon','Vamdemon','Piemon','Apocalymon'
))
$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName System.Drawing
$assetRoot = Join-Path $PSScriptRoot 'assets'
$sources = @{}
foreach ($name in $Names) {
  $file = Join-Path $assetRoot "$name.gif"
  if (Test-Path -LiteralPath $file) { Write-Output "EXISTS $name"; continue }
  $pageName = if ($name -eq 'Atlur_Kabuterimon') { 'Atlur_Kabuterimon_Red' } else { $name }
  $page = Invoke-WebRequest -UseBasicParsing -Uri "https://wikimon.net/$pageName" -TimeoutSec 20
  $links = @([regex]::Matches($page.Content, 'src="(/images/[^\"]+\.gif)"') | ForEach-Object { $_.Groups[1].Value } | Select-Object -Unique)
  $links = $links | Where-Object { $_ -notmatch '/thumb/|cutin|art_mini' } | Sort-Object { if ($_ -match 'vpet_dmc\.gif') {0} elseif ($_ -match 'vpet_dpc\.gif') {1} elseif ($_ -match 'vpet_dm\.gif') {2} else {3} }
  foreach ($link in $links) {
    Invoke-WebRequest -UseBasicParsing -Uri "https://wikimon.net$link" -OutFile $file -TimeoutSec 20
    $img = [System.Drawing.Image]::FromFile($file)
    try { $width=$img.Width; $height=$img.Height } finally { $img.Dispose() }
    if ($width -gt 0 -and $height -gt 0) { $sources[$name] = "https://wikimon.net$link"; Write-Output "OK $name ${width}x${height}"; break }
  }
  if (-not (Test-Path -LiteralPath $file)) { throw "No sprite found: $name" }
}
if ($sources.Count) { $sources | ConvertTo-Json | Set-Content -Encoding UTF8 (Join-Path $assetRoot 'stage-sprite-sources.json') }
