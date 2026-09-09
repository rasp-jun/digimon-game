[CmdletBinding()]
param(
    [string]$UnityPath
)

$ErrorActionPreference = 'Stop'
$projectPath = $PSScriptRoot

if (-not $UnityPath) {
    $editorRoot = 'C:\Program Files\Unity\Hub\Editor'
    if (Test-Path -LiteralPath $editorRoot) {
        $preferredEditor = Join-Path $editorRoot '6000.6.0f1\Editor\Unity.exe'
        if (Test-Path -LiteralPath $preferredEditor) {
            $UnityPath = $preferredEditor
        } else {
            $UnityPath = Get-ChildItem -LiteralPath $editorRoot -Directory |
                Where-Object { $_.Name -like '6000.6.*' } |
                Sort-Object Name -Descending |
                ForEach-Object { Join-Path $_.FullName 'Editor\Unity.exe' } |
                Where-Object { Test-Path -LiteralPath $_ } |
                Select-Object -First 1
        }
    }
}

if (-not $UnityPath -or -not (Test-Path -LiteralPath $UnityPath)) {
    throw 'Unity 6.6 Editor를 찾지 못했습니다. Unity Hub에서 6000.6과 Android Build Support를 설치하거나 -UnityPath로 Unity.exe 경로를 지정하세요.'
}

$logPath = Join-Path $projectPath 'Builds\Android\unity-native-build.log'
New-Item -ItemType Directory -Path (Split-Path $logPath) -Force | Out-Null
$apkPath = Join-Path $projectPath 'Builds\Android\FileIslandLeague-Native.apk'
$gradleApkPath = Join-Path $projectPath 'Library\Bee\Android\Prj\IL2CPP\Gradle\launcher\build\outputs\apk\release\launcher-release.apk'
Remove-Item -LiteralPath $apkPath -Force -ErrorAction SilentlyContinue
Remove-Item -LiteralPath $gradleApkPath -Force -ErrorAction SilentlyContinue

$unityArgs = @(
    '-quit'
    '-buildTarget', 'Android'
    '-projectPath', $projectPath
    '-executeMethod', 'BuildAndroid.BuildApk'
    '-logFile', $logPath
)

# Unity 6.6의 headless 모듈이 없는 일반 Editor 설치에서도 빌드할 수 있도록
# 창을 숨긴 일반 Editor 프로세스로 실행한다.
$unityProcess = Start-Process -FilePath $UnityPath -ArgumentList $unityArgs -PassThru -WindowStyle Hidden
$unityProcess.WaitForExit()

# 일부 Unity 6.6 설치는 빌드를 마친 뒤 작업자 프로세스를 남기며 최종 APK 복사를
# 건너뛴다. 이때 Gradle이 서명까지 끝낸 산출물을 같은 최종 위치에 보존한다.
if (-not (Test-Path -LiteralPath $apkPath) -and (Test-Path -LiteralPath $gradleApkPath)) {
    Copy-Item -LiteralPath $gradleApkPath -Destination $apkPath -Force
}

if ($unityProcess.ExitCode -ne 0 -and -not (Test-Path -LiteralPath $apkPath)) {
    throw "Unity APK 빌드에 실패했습니다. 로그: $logPath"
}

if (-not (Test-Path -LiteralPath $apkPath)) {
    throw "Unity가 성공 코드를 반환했지만 APK가 없습니다. 로그: $logPath"
}

Write-Host "APK 제작 완료: $apkPath"
