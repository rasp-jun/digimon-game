using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

// Online state comes exclusively from the dedicated server; the solo game stays isolated.
public sealed class MultiLauncher : MonoBehaviour
{
    [Serializable] public class UnitDef { public string id, name, sprite, role; public int cost; }
    [Serializable] public class Catalog { public UnitDef[] units; }
    [Serializable] public class Unit { public string id; public int star, slot; }
    [Serializable] public class Player { public string name; public int rating, hp, gold, level, xp; public bool ready; public Unit[] board, bench; public string[] shop; }
    [Serializable] public class Fighter { public int key, side, star; public string id; public float x, y, hp, maxHp; }
    [Serializable] public class Frame { public Fighter[] units; }
    [Serializable] public class Room { public string id, mode, phase, result, message; public int round, side, ratingDelta; public float remaining; public Player[] players; public Frame[] frames; }
    [Serializable] public class State { public string token, name, queue, error; public int rating, waiting; public Room room; }
    [Serializable] public class Command { public string name, key, mode, action, area, targetArea; public int slot, targetSlot; }

    string server = "http://127.0.0.1:7777", nickname = "테이머", token = "", notice = "서버에 접속한 뒤 일반 / 랭크 매칭을 시작하세요.";
    string profile = "", selectedArea = "";
    int selectedSlot = -1;
    bool busy, solo, confirmLeave, connectionError;
    float nextPoll, receivedAt;
    State state;
    Catalog catalog;
    NativeGame soloGame;
    GUIStyle title, text, small, button, box, eyebrow, centered, stat, input;
    readonly Color navy = new Color(.025f,.043f,.075f), surface = new Color(.045f,.075f,.115f), surface2 = new Color(.065f,.105f,.15f);
    readonly Color gold = new Color(.79f,.63f,.30f), paleGold = new Color(.94f,.84f,.57f), cyan = new Color(.22f,.72f,.79f), muted = new Color(.57f,.65f,.72f);
    readonly System.Collections.Generic.Dictionary<string, Texture2D> textures = new System.Collections.Generic.Dictionary<string, Texture2D>();
    readonly System.Collections.Generic.List<Texture2D> uiTextures = new System.Collections.Generic.List<Texture2D>();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Boot()
    {
        if (FindAnyObjectByType<MultiLauncher>() != null) return;
        var root = new GameObject("Dittoches Multi");
        DontDestroyOnLoad(root);
        root.AddComponent<MultiLauncher>();
    }

    void Awake()
    {
        Application.runInBackground = true;
        Application.targetFrameRate = 60;
        profile = Application.isEditor ? "editor" : "player";
        string[] args = Environment.GetCommandLineArgs();
        for (int i = 0; i + 1 < args.Length; i++) if (args[i] == "--profile") profile = args[i + 1];
        server = PlayerPrefs.GetString("multi.server", server);
        nickname = PlayerPrefs.GetString("multi.name." + profile, nickname);
        catalog = JsonUtility.FromJson<Catalog>(Resources.Load<TextAsset>("MultiRoster").text);
    }

    public void ReturnToMulti()
    {
        if (soloGame != null) soloGame.gameObject.SetActive(false);
        solo = false;
    }

    void EnterSolo()
    {
        token = ""; state = null; solo = true;
        if (soloGame == null)
        {
            var root = new GameObject("Dittoches Solo");
            DontDestroyOnLoad(root);
            soloGame = root.AddComponent<NativeGame>();
        }
        soloGame.gameObject.SetActive(true);
    }

    void Update()
    {
        if (!solo && token.Length > 0 && !busy && Time.unscaledTime >= nextPoll)
            StartCoroutine(Request("/state", new Command()));
    }

    string AccountKey()
    {
        // Separate guest identity for each server and local test profile.
        string scope;
        using (var hash = System.Security.Cryptography.SHA256.Create())
            scope = BitConverter.ToString(hash.ComputeHash(Encoding.UTF8.GetBytes(server + "|" + profile))).Replace("-", "");
        string pref = "multi.key." + scope;
        string key = PlayerPrefs.GetString(pref, "");
        if (key.Length != 64)
        {
            byte[] bytes = new byte[32];
            using (var rng = System.Security.Cryptography.RandomNumberGenerator.Create()) rng.GetBytes(bytes);
            key = BitConverter.ToString(bytes).Replace("-", "").ToLowerInvariant();
            PlayerPrefs.SetString(pref, key); PlayerPrefs.Save();
        }
        return key;
    }

    void Connect()
    {
        server = server.Trim().TrimEnd('/'); nickname = nickname.Trim();
        if (!Uri.TryCreate(server, UriKind.Absolute, out Uri uri) || (uri.Scheme != "http" && uri.Scheme != "https") || !string.IsNullOrEmpty(uri.UserInfo))
        { notice = "http://주소:7777 또는 https://주소 형식으로 입력하세요."; return; }
        if (nickname.Length < 1 || nickname.Length > 20) { notice = "닉네임은 1~20자로 입력하세요."; return; }
        PlayerPrefs.SetString("multi.server", server); PlayerPrefs.SetString("multi.name." + profile, nickname); PlayerPrefs.Save();
        StartCoroutine(Request("/login", new Command { name = nickname, key = AccountKey() }));
    }

    IEnumerator Request(string path, Command command)
    {
        busy = true;
        using (var request = new UnityWebRequest(server + path, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(JsonUtility.ToJson(command)));
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            if (token.Length > 0) request.SetRequestHeader("Authorization", "Bearer " + token);
            request.timeout = 5;
            yield return request.SendWebRequest();
            State response = null;
            try { response = JsonUtility.FromJson<State>(request.downloadHandler.text); }
            catch (Exception) { /* Network/proxy responses may not be JSON. */ }
            if (request.result != UnityWebRequest.Result.Success || response == null || !string.IsNullOrEmpty(response.error))
            {
                connectionError = true;
                notice = response != null && !string.IsNullOrEmpty(response.error) ? response.error : "서버 연결 실패 · 주소와 서버 실행 상태를 확인하세요. 자동으로 재시도합니다.";
                if (response != null && response.error != null && response.error.Contains("인증이 만료")) { token = ""; state = null; }
            }
            else
            {
                state = response; token = response.token ?? ""; receivedAt = Time.unscaledTime;
                if (connectionError || path != "/state") notice = path == "/login" ? "서버 접속 완료" : "서버에 연결되었습니다.";
                connectionError = false;
                if (state.room == null || state.room.phase != "prepare") { selectedSlot = -1; selectedArea = ""; }
                if (path == "/leave") confirmLeave = false;
            }
        }
        busy = false; nextPoll = Time.unscaledTime + 1;
    }

    void Send(string path, Command command = null)
    {
        if (!busy) StartCoroutine(Request(path, command ?? new Command()));
    }

    void Styles()
    {
        if (title != null) return;
        title = new GUIStyle(GUI.skin.label) { fontSize = 32, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleLeft }; title.normal.textColor = paleGold;
        text = new GUIStyle(GUI.skin.label) { fontSize = 19, wordWrap = true }; text.normal.textColor = new Color(.88f,.91f,.94f);
        small = new GUIStyle(text) { fontSize = 15 }; small.normal.textColor = muted;
        eyebrow = new GUIStyle(small) { fontSize = 14, fontStyle = FontStyle.Bold }; eyebrow.normal.textColor = gold;
        centered = new GUIStyle(text) { alignment = TextAnchor.MiddleCenter };
        stat = new GUIStyle(text) { fontSize = 18, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter }; stat.normal.textColor = paleGold;
        button = new GUIStyle(GUI.skin.button) { fontSize=17,fontStyle=FontStyle.Bold,wordWrap=true,padding=new RectOffset(14,14,8,8),border=new RectOffset(1,1,1,1) };
        button.normal.background=UiTex(new Color(.075f,.13f,.18f)); button.hover.background=UiTex(new Color(.12f,.22f,.27f)); button.active.background=UiTex(new Color(.20f,.25f,.17f));
        button.focused.background=button.hover.background; button.normal.textColor=new Color(.94f,.91f,.82f); button.hover.textColor=Color.white; button.active.textColor=paleGold; button.focused.textColor=Color.white;
        box = new GUIStyle(GUI.skin.box) { fontSize=18 }; box.normal.background=UiTex(surface);
        input = new GUIStyle(GUI.skin.textField) { fontSize=18,padding=new RectOffset(14,14,8,8),border=new RectOffset(1,1,1,1) };
        input.normal.background=UiTex(new Color(.025f,.05f,.08f)); input.focused.background=UiTex(new Color(.04f,.085f,.115f));
        input.hover.background=input.focused.background; input.normal.textColor=Color.white; input.focused.textColor=Color.white; input.hover.textColor=Color.white;
    }

    Texture2D UiTex(Color color)
    {
        var texture=new Texture2D(1,1,TextureFormat.RGBA32,false) { hideFlags=HideFlags.HideAndDontSave,filterMode=FilterMode.Point };
        texture.SetPixel(0,0,color); texture.Apply(); uiTextures.Add(texture); return texture;
    }

    void OnDestroy()
    {
        foreach(Texture2D texture in uiTextures) if(texture!=null) Destroy(texture);
    }

    bool Btn(Rect rect, string label, bool enabled = true)
    {
        bool previous = GUI.enabled; GUI.enabled = previous && enabled && !busy;
        bool clicked = GUI.Button(rect, label, button); GUI.enabled = previous; return clicked;
    }

    void Panel(Rect rect, Color color)
    {
        Color old = GUI.color; GUI.color = color; GUI.DrawTexture(rect, Texture2D.whiteTexture); GUI.color = old;
    }

    void Border(Rect rect, Color color, float width = 2)
    {
        Panel(new Rect(rect.x,rect.y,rect.width,width),color); Panel(new Rect(rect.x,rect.yMax-width,rect.width,width),color);
        Panel(new Rect(rect.x,rect.y,width,rect.height),color); Panel(new Rect(rect.xMax-width,rect.y,width,rect.height),color);
    }

    void Card(Rect rect, Color fill, Color line)
    {
        Panel(new Rect(rect.x+4,rect.y+7,rect.width,rect.height),new Color(0,0,0,.28f)); Panel(rect,fill); Border(rect,line,1.5f);
    }

    void DrawBackdrop()
    {
        Panel(new Rect(0,0,1600,1000),navy);
        for(int i=0;i<10;i++) Panel(new Rect(0,i*100,1600,100),Color.Lerp(new Color(.025f,.04f,.075f),new Color(.015f,.09f,.11f),i/12f));
        Panel(new Rect(0,0,7,1000),gold); Panel(new Rect(1593,0,7,1000),gold);
        Panel(new Rect(0,76,1600,1),new Color(gold.r,gold.g,gold.b,.45f));
    }

    void Pill(Rect rect, string value, Color color)
    {
        Panel(rect,new Color(color.r,color.g,color.b,.16f)); Border(rect,new Color(color.r,color.g,color.b,.65f),1); GUI.Label(rect,value,stat);
    }

    void OnGUI()
    {
        if (solo) return;
        Styles();
        Matrix4x4 old = GUI.matrix;
        float rawScale=Mathf.Min(Screen.width/1600f,Screen.height/1000f);
        float scale=rawScale>=.85f?Mathf.Round(rawScale*20f)/20f:rawScale;
        float offsetX=Mathf.Floor((Screen.width-1600*scale)/2f),offsetY=Mathf.Floor((Screen.height-1000*scale)/2f);
        GUI.matrix=Matrix4x4.TRS(new Vector3(offsetX,offsetY,0),Quaternion.identity,new Vector3(scale,scale,1));
        Color oldColor=GUI.color; Panel(new Rect(-offsetX/scale,-offsetY/scale,Screen.width/scale,Screen.height/scale),Color.black); GUI.color=oldColor;
        DrawBackdrop();
        if (state != null && state.room != null) DrawMatch(); else DrawLobby();
        Panel(new Rect(0,944,1600,56),new Color(.015f,.028f,.05f,.96f));
        Panel(new Rect(30,965,8,8),busy ? gold : connectionError ? new Color(.9f,.3f,.25f) : cyan);
        GUI.Label(new Rect(50,951,1500,38), busy ? "서버 통신 중  ·  " + notice : notice, small);
        GUI.matrix = old;
    }

    void DrawLobby()
    {
        GUI.Label(new Rect(70, 24, 750, 50), "DITTOCHES", title); GUI.Label(new Rect(257,31,350,36), "MULTI", eyebrow);
        GUI.Label(new Rect(70, 103, 500, 25), "ONLINE CONNECTION", eyebrow);
        GUI.Label(new Rect(70, 132, 1400, 35), "파일 아일랜드에서 펼쳐지는 솔로 리그와 실시간 온라인 대전", text);
        Rect connect = new Rect(70,190,1460,210); Card(connect,surface,new Color(gold.r,gold.g,gold.b,.55f));
        GUI.Label(new Rect(100,210,250,28), "SERVER ADDRESS", eyebrow); GUI.Label(new Rect(790,210,250,28), "TAMER NAME", eyebrow);
        GUI.enabled = !busy && token.Length == 0;
        server = GUI.TextField(new Rect(100,247,655,48),server,200,input); nickname = GUI.TextField(new Rect(790,247,380,48),nickname,20,input);
        GUI.enabled = true;
        if (token.Length == 0)
        { if (Btn(new Rect(1200,247,285,48),"서버 접속")) Connect(); }
        else if (Btn(new Rect(1200,247,285,48),"접속 해제",string.IsNullOrEmpty(state.queue)))
        { token = ""; state = null; notice = "접속을 해제했습니다."; }
        GUI.Label(new Rect(100,325,1070,40),state == null ? "로컬  127.0.0.1:7777   ·   다른 기기는 서버 PC의 IP 주소를 사용하세요" : $"●  ONLINE    {state.name}    ·    {state.rating} RP",small);
        if(state!=null) Pill(new Rect(1265,319,220,40),state.rating+" RP",gold);
        string[] labels = { "솔로 플레이", "일반 대전", "랭크 대전" };
        string[] tags = { "SOLO", "NORMAL", "RANKED" };
        string[] descriptions = { "나만의 속도로 즐기는 리그\n난이도와 전설이를 선택하세요.", "부담 없이 즐기는 온라인 1대1\n승패에 따른 RP 변동이 없습니다.", "실력을 겨루는 온라인 1대1\n승패가 서버 랭크에 반영됩니다." };
        bool queued = state != null && !string.IsNullOrEmpty(state.queue);
        for (int i = 0; i < 3; i++)
        {
            float x=70+i*495; Rect mode=new Rect(x,440,460,400); Color modeLine=i==2?gold:i==1?cyan:new Color(.45f,.58f,.68f);
            Card(mode,surface2,modeLine); Panel(new Rect(x,440,460,7),modeLine);
            GUI.Label(new Rect(x+30,475,400,25),tags[i],eyebrow); GUI.Label(new Rect(x+30,507,400,55),labels[i],title);
            GUI.Label(new Rect(x+30,585,400,85),descriptions[i],text);
            GUI.Label(new Rect(x+30,686,400,30),i==0?"AI 7명  ·  서버 불필요":i==1?"2 PLAYERS  ·  CASUAL":"2 PLAYERS  ·  ELO RATING",small);
            if (Btn(new Rect(x+30,750,400,58),i==0?"솔로 리그 입장":"대전 찾기",!queued&&(i==0||token.Length>0)))
            { if (i == 0) EnterSolo(); else Send("/queue", new Command { mode = i == 1 ? "normal" : "ranked" }); }
        }
        if (queued)
        {
            Card(new Rect(70,862,1460,65),new Color(.05f,.11f,.14f),cyan);
            GUI.Label(new Rect(105,877,1050,35),(state.queue=="ranked"?"RANKED":"NORMAL")+"  ·  상대 테이머를 찾는 중…",text);
            if (Btn(new Rect(1240,871,260,46),"매칭 취소")) Send("/cancel");
        }
    }

    UnitDef Def(string id) { return Array.Find(catalog.units, u => u.id == id); }
    void Portrait(Rect rect, string id)
    {
        UnitDef def = Def(id); if (def == null) return;
        if (!textures.TryGetValue(id, out Texture2D texture)) { texture = Resources.Load<Texture2D>("Sprites/" + def.sprite); textures[id] = texture; }
        if (texture != null) GUI.DrawTexture(rect, texture, ScaleMode.ScaleToFit);
    }

    Unit At(Unit[] units, int slot) { return Array.Find(units, u => u.slot == slot); }
    void ClickSlot(string area, int slot, Unit unit)
    {
        if (selectedSlot >= 0)
        {
            if (selectedArea != area || selectedSlot != slot)
                Send("/action", new Command { action = "move", area = selectedArea, slot = selectedSlot, targetArea = area, targetSlot = slot });
            selectedSlot = -1; selectedArea = "";
        }
        else if (unit != null) { selectedArea = area; selectedSlot = slot; }
    }

    void DrawSlot(Rect rect, Unit unit, string area, int slot, bool editable)
    {
        bool selected=selectedArea==area&&selectedSlot==slot;
        Panel(rect,selected?new Color(.20f,.25f,.13f):unit==null?new Color(.035f,.075f,.095f,.82f):new Color(.07f,.13f,.17f));
        Border(rect,selected?gold:new Color(.16f,.30f,.34f),selected?2:1);
        if (unit != null)
        {
            Portrait(new Rect(rect.x+4,rect.y+2,50,rect.height-5),unit.id);
            GUI.Label(new Rect(rect.x+55,rect.y+4,rect.width-58,24),Def(unit.id).name,small);
            GUI.Label(new Rect(rect.x+55,rect.y+27,rect.width-58,24),new string('★',unit.star),eyebrow);
        }
        if (editable && !busy && GUI.Button(rect, GUIContent.none, GUIStyle.none)) ClickSlot(area, slot, unit);
    }

    void DrawMatch()
    {
        Room room = state.room; Player me = room.players[room.side], enemy = room.players[1 - room.side];
        if (room.phase == "finished") confirmLeave = false;
        GUI.enabled = !confirmLeave;
        bool fresh = Time.unscaledTime - receivedAt < 6;
        bool editable = room.phase == "prepare" && !me.ready && fresh;
        float remaining = Mathf.Max(0, room.remaining - (Time.unscaledTime - receivedAt));
        GUI.Label(new Rect(30,14,250,24),room.mode=="ranked"?"RANKED MATCH":"NORMAL MATCH",eyebrow);
        GUI.Label(new Rect(30,36,570,42),$"ROUND {room.round}",title);
        Pill(new Rect(610,23,190,43),room.phase=="prepare"?"준비 단계":room.phase=="battle"?"전투 중":"경기 종료",room.phase=="battle"?new Color(.85f,.28f,.22f):cyan);
        Pill(new Rect(815,23,145,43),Mathf.CeilToInt(remaining)+"초",remaining<10?new Color(.9f,.35f,.22f):gold);
        if (Btn(new Rect(1370,20,190,46),room.phase=="finished"?"로비로":"경기 포기"))
        { if (room.phase == "finished") Send("/leave"); else confirmLeave = !confirmLeave; }
        Card(new Rect(25,95,260,165),surface,new Color(gold.r,gold.g,gold.b,.5f));
        GUI.Label(new Rect(45,108,220,25),"MY TACTICIAN",eyebrow); GUI.Label(new Rect(45,136,220,34),me.name,text);
        Pill(new Rect(43,180,102,36),"HP "+me.hp,new Color(.30f,.78f,.52f)); Pill(new Rect(155,180,108,36),me.rating+" RP",gold);
        GUI.Label(new Rect(45,225,220,25),$"{me.gold} G    ·    LV {me.level}    ·    XP {me.xp}",small);
        Card(new Rect(1305,95,270,165),surface,new Color(.65f,.25f,.28f,.7f));
        GUI.Label(new Rect(1325,108,225,25),"OPPONENT",eyebrow); GUI.Label(new Rect(1325,136,225,34),enemy.name,text);
        Pill(new Rect(1323,180,102,36),"HP "+enemy.hp,new Color(.82f,.28f,.25f)); Pill(new Rect(1435,180,118,36),enemy.rating+" RP",gold);
        GUI.Label(new Rect(1325,225,225,25),$"LV {enemy.level}   ·   {(enemy.ready?"준비 완료":"준비 중")}",small);
        GUI.Label(new Rect(310,94,950,30),"상대 진영",eyebrow);
        if (room.phase == "battle") DrawCombat(room, remaining);
        else
        {
            Panel(new Rect(306,130,962,532),new Color(.025f,.11f,.12f,.72f)); Border(new Rect(306,130,962,532),new Color(.16f,.52f,.53f),1);
            for (int row = 0; row < 8; row++) for (int col = 0; col < 7; col++)
            {
                bool own = row >= 4; int slot = own ? (row - 4) * 7 + col : (3 - row) * 7 + (6 - col);
                Unit unit = At(own ? me.board : enemy.board, slot);
                DrawSlot(new Rect(310 + col * 137, 135 + row * 66, 132, 61), unit, own ? "board" : "enemy", slot, own && editable);
            }
            Panel(new Rect(310,397,954,3),gold); GUI.Label(new Rect(1110,402,140,22),"내 진영",eyebrow);
        }
        int boardCount=me.board==null?0:me.board.Length;
        GUI.Label(new Rect(310,674,930,30),$"대기석    ·    전장 {boardCount}/{me.level}    ·    유닛 선택 후 목적지를 선택하세요",eyebrow);
        for (int i = 0; i < 9; i++) DrawSlot(new Rect(310 + i * 106, 710, 101, 70), At(me.bench, i), "bench", i, editable);
        GUI.Label(new Rect(310,790,500,22),"RECRUIT SHOP",eyebrow);
        for (int i = 0; i < me.shop.Length; i++)
        {
            float x = 310 + i * 191; string id = me.shop[i]; UnitDef def = Def(id);
            Card(new Rect(x,814,181,108),surface2,def==null?new Color(.15f,.25f,.3f):gold);
            if (def != null)
            {
                Portrait(new Rect(x + 4, 825, 60, 78), id);
                if (Btn(new Rect(x + 65, 822, 112, 88), def.name + "\n" + def.cost + " G", editable && me.gold >= def.cost))
                    Send("/action", new Command { action = "buy", slot = i });
            }
        }
        GUI.Label(new Rect(30,286,250,24),"ECONOMY",eyebrow);
        if (Btn(new Rect(30,320,245,58),"새로고침     2 G",editable&&me.gold>=2)) Send("/action",new Command{action="reroll"});
        if (Btn(new Rect(30,390,245,58),"경험치 +4     4 G",editable&&me.gold>=4&&me.level<9)) Send("/action",new Command{action="xp"});
        if (Btn(new Rect(30,460,245,58),"선택 유닛 판매",editable&&selectedSlot>=0))
        { Send("/action", new Command { action = "sell", area = selectedArea, slot = selectedSlot }); selectedSlot = -1; selectedArea = ""; }
        if (Btn(new Rect(30,555,245,78),me.ready?"준비 취소":"전투 준비 완료",room.phase=="prepare"&&fresh)) Send("/action",new Command{action="ready"});
        GUI.Label(new Rect(30,652,245,135),"양쪽 모두 준비하면 전투가 시작됩니다.\n제한 시간이 끝나도 자동 시작됩니다.",small);
        Card(new Rect(1305,290,270,180),surface,new Color(.2f,.38f,.43f)); GUI.Label(new Rect(1325,310,225,25),"BATTLE LOG",eyebrow); GUI.Label(new Rect(1325,345,225,105),room.message??"전투 기록이 여기에 표시됩니다.",small);
        if (!fresh) GUI.Label(new Rect(310, 80, 970, 50), "연결 복구 중 · 조작을 잠시 중지합니다.", text);
        if (room.phase == "finished")
        {
            Card(new Rect(510,300,610,270),new Color(.035f,.07f,.12f,.99f),gold);
            GUI.Label(new Rect(560,335,510,60),"경기 결과  ·  "+room.result,title);
            GUI.Label(new Rect(560, 420, 510, 65), room.mode == "ranked" ? $"랭크 변동 {room.ratingDelta:+0;-0;0} RP  /  현재 {state.rating} RP" : "일반 모드 · 랭크 점수 변동 없음", text);
        }
        GUI.enabled = true;
        if (confirmLeave && room.phase != "finished")
        {
            Panel(new Rect(490, 330, 680, 250), new Color(.08f,.1f,.16f));
            GUI.Label(new Rect(530, 360, 600, 80), "경기를 포기하면 패배 처리됩니다.\n랭크 모드에서는 RP에도 반영됩니다.", text);
            if (Btn(new Rect(530, 470, 270, 65), "계속 플레이")) confirmLeave = false;
            if (Btn(new Rect(840, 470, 270, 65), "포기하고 로비로")) Send("/leave");
        }
    }

    void DrawCombat(Room room, float remaining)
    {
        Panel(new Rect(310,135,954,523), new Color(.08f,.15f,.19f));
        if (room.frames == null || room.frames.Length == 0) return;
        float progress = Mathf.Clamp01(1 - remaining / 8f) * (room.frames.Length - 1);
        int frame = Mathf.FloorToInt(progress), next = Mathf.Min(frame + 1, room.frames.Length - 1);
        foreach (Fighter f in room.frames[frame].units)
        {
            if (f.hp <= 0) continue;
            Fighter to = Array.Find(room.frames[next].units, u => u.key == f.key) ?? f;
            float x = Mathf.Lerp(f.x,to.x,progress-frame), y = Mathf.Lerp(f.y,to.y,progress-frame);
            if (room.side == 1) { x = 6-x; y = 7-y; }
            float px = 325+x*133, py = 139+y*64;
            Portrait(new Rect(px,py,75,55),f.id);
            Panel(new Rect(px,py+54,75,5),Color.gray);
            Panel(new Rect(px,py+54,75*f.hp/f.maxHp,5),f.side==room.side ? Color.green : Color.red);
        }
    }
}
