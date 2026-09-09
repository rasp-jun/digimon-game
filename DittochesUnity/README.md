# File Island League · Unity 6.6 Native Prototype

Unity `6000.6.0f1`에서 실행되는 네이티브 Android 프로토타입입니다. WebView, HTML 실행, 외부 브라우저 미리보기를 사용하지 않습니다. Unity Play 버튼과 Android APK가 동일한 C# 게임 화면을 실행합니다.

## Unity에서 실행

1. Unity Hub에서 `DittochesUnity` 폴더를 엽니다.
2. Unity 상단 Play 버튼을 누릅니다.
3. Game 창 안에서 솔로 로비가 표시됩니다.

로비에서 쉬움·보통·어려움 난이도와 전설이를 선택할 수 있습니다. 입장 후 보드 배치, 상점 구매와 리롤, 경험치 구매, 대기석 판매, 별 합성, 3성 상점 잠금, 간이 자동 전투와 라운드 진행을 확인할 수 있습니다.

## APK 만들기

Unity 메뉴에서 `File Island > Build Native Android APK`를 누르거나 PowerShell에서 다음 명령을 실행합니다.

```powershell
./build-apk.ps1
```

완성 파일은 `Builds/Android/FileIslandLeague-Native.apk`입니다.

- 앱 ID: `com.dittoches.fileisland`
- 최소 Android: API 26(Android 8.0)
- 화면: 가로
- 저장: Unity PlayerPrefs
- Android 진입점: Unity Activity

`Assets/Resources/Sprites`에는 기존 GIF의 첫 프레임을 변환한 Unity용 PNG 이미지가 들어 있습니다. 게임 데이터와 화면은 `Assets/Scripts/NativeGame.cs`에서 실행됩니다.
