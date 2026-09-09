$ErrorActionPreference = 'Stop'
$assetRoot = Join-Path $PSScriptRoot 'assets'
New-Item -ItemType Directory -Force -Path $assetRoot | Out-Null
Add-Type -AssemblyName System.Drawing
$names = 'Agumon','Greymon','Metal_Greymon','War_Greymon','Gabumon','Garurumon','Were_Garurumon','Metal_Garurumon','Tentomon','Kabuterimon','Atlur_Kabuterimon','Herakle_Kabuterimon','Piyomon','Birdramon','Garudamon','Hououmon','Palmon','Togemon','Lilimon','Rosemon','Patamon','Angemon','Holy_Angemon','Seraphimon','Gomamon','Ikkakumon','Zudomon','Vikemon','Plotmon','Tailmon','Angewomon','Holydramon'
$results = @{}
foreach ($name in $names) {
  try {
    $page = Invoke-WebRequest -Uri "https://wikimon.net/$name" -TimeoutSec 25
    $links = @([regex]::Matches($page.Content, 'src="(/images/[^\"]+\.gif)"') | ForEach-Object { $_.Groups[1].Value } | Select-Object -Unique)
    $links = $links | Sort-Object { if ($_ -match 'dmc\.gif|dpc\.gif') {0} elseif ($_ -match 'vpet_dm\.|vpet_pen\.') {1} elseif ($_ -match 'cutin') {3} else {2} }
    foreach ($link in $links) {
      if ($link -match '/thumb/') { continue }
      $file = Join-Path $assetRoot "$name.gif"
      Invoke-WebRequest -Uri "https://wikimon.net$link" -OutFile $file -TimeoutSec 25
      $img = [System.Drawing.Image]::FromFile($file)
      try { $frames = $img.GetFrameCount([System.Drawing.Imaging.FrameDimension]::Time) } finally {$img.Dispose()}
      if ($frames -gt 1) { $results[$name] = @{src="assets/$name.gif";url="https://wikimon.net$link";frames=$frames}; Write-Output "OK $name frames=$frames"; break }
    }
    if (-not $results.ContainsKey($name)) { Write-Output "MISSING $name" }
  } catch { Write-Output "FAIL $name $($_.Exception.Message)" }
}
$json = $results | ConvertTo-Json -Depth 4
[IO.File]::WriteAllText((Join-Path $PSScriptRoot 'sprites.js'), "const SPRITES = $json;", [Text.UTF8Encoding]::new($false))
