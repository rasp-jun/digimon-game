using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public sealed class NativeGame : MonoBehaviour
{
    private sealed class UnitDef
    {
        public string id, name, sprite, role; public int cost;
        public UnitDef(string id, string name, int cost, string sprite, string role) { this.id=id; this.name=name; this.cost=cost; this.sprite=sprite; this.role=role; }
    }
    private sealed class Unit { public UnitDef def; public int star=1; public readonly List<int> items=new List<int>(); public Unit(UnitDef def) { this.def=def; } }
    private sealed class UnitRef { public bool boardArea; public int index; public Unit unit; }
    private sealed class UnitMeta { public string attr,family;public float hp,atk,speed;public int range;public UnitMeta(string a,string f,float h,float at,float s,int r){attr=a;family=f;hp=h;atk=at;speed=s;range=r;} }
    private sealed class Fighter
    {
        public Unit unit; public bool enemy, dead; public Vector2 pos; public float hp, maxHp, cooldown, hitFlash, attackFlash, attackScale=1f;
    }
    private sealed class LootOrb { public Vector2 pos; public int rarity, rewardType, amount; }

    private static readonly UnitDef[] Roster = {
        new UnitDef("koromon","코로몬",1,"Koromon","전사"),new UnitDef("tsunomon","뿔몬",1,"Tunomon","사수"),new UnitDef("mochimon","모티몬",1,"Mochimon","탱커"),new UnitDef("tanemon","시드몬",1,"Tanemon","지원"),new UnitDef("pyocomon","어니몬",1,"Pyocomon","마법사"),new UnitDef("tokomon","토코몬",1,"Tokomon","지원"),
        new UnitDef("agumon","아구몬",2,"Agumon","전사"),new UnitDef("gabumon","파피몬",2,"Gabumon","사수"),new UnitDef("tentomon","텐타몬",2,"Tentomon","탱커"),new UnitDef("palmon","팔몬",2,"Palmon","지원"),new UnitDef("piyomon","피요몬",2,"Piyomon","마법사"),new UnitDef("patamon","파닥몬",2,"Patamon","지원"),
        new UnitDef("togemon","니드몬",3,"Togemon","탱커"),new UnitDef("garurumon","가루몬",3,"Garurumon","전사"),new UnitDef("greymon","그레이몬",3,"Greymon","탱커"),new UnitDef("kabuterimon","캅테리몬",3,"Kabuterimon","마법사"),new UnitDef("angemon","엔젤몬",3,"Angemon","전사"),new UnitDef("birdramon","버드라몬",3,"Birdramon","사수"),
        new UnitDef("metalgreymon","메탈그레이몬",4,"Metal_Greymon","사수"),new UnitDef("weregarurumon","워가루몬",4,"Were_Garurumon","전사"),new UnitDef("lilimon","릴리몬",4,"Lilimon","지원"),new UnitDef("holyangemon","홀리엔젤몬",4,"Holy_Angemon","마법사"),new UnitDef("atlur","아트라캅테리몬",4,"Atlur_Kabuterimon","탱커"),new UnitDef("garudamon","가루다몬",4,"Garudamon","사수"),
        new UnitDef("herakle","헤라클레스캅테리몬",5,"Herakle_Kabuterimon","탱커"),new UnitDef("hououmon","페닉스몬",5,"Hououmon","마법사"),new UnitDef("wargreymon","워그레이몬",5,"War_Greymon","전사"),new UnitDef("metalgarurumon","메탈가루몬",5,"Metal_Garurumon","사수"),new UnitDef("rosemon","로제몬",5,"Rosemon","지원"),new UnitDef("seraphimon","세라피몬",5,"Seraphimon","마법사")
    };
    private static readonly string[] Legends={"비트몬","코로몬","토코몬","어니몬"};
    private static readonly string[] LegendSprites={"","Koromon","Tokomon","Pyocomon"};
    private static readonly string[] Difficulties={"쉬움","보통","어려움"};
    private static readonly float[] DifficultyScale={.88f,1f,1.12f};
    private static readonly int[,] ShopOdds={{100,0,0,0,0},{100,0,0,0,0},{75,25,0,0,0},{55,30,15,0,0},{40,35,23,2,0},{25,40,28,7,0},{18,30,35,16,1},{15,20,32,28,5},{10,15,25,35,15}};
    private static readonly string[] ItemNames={"공격 데이터","크롬디지조이드 조각","순수 에너지","생명 파편","용기의 문장","우정의 문장","사랑의 문장","성실의 문장","지식의 문장","희망의 문장","순수의 문장","빛의 문장","기적의 캡슐","운명의 캡슐","자석 제거기"};
    private static readonly string[] ItemIcons={"✦","⬡","✧","❋","☀","∞","♥","✚","◈","✴","❀","✵","⬢","◆","↩"};
    private static readonly string[] ItemDescriptions={"공격력 +12% · 다른 재료와 장착 시 자동 합성","체력 +120 · 다른 재료와 장착 시 자동 합성","공격 속도 +12% · 다른 재료와 장착 시 자동 합성","체력 +150 · 다른 재료와 장착 시 자동 합성","공격력 +40%","공격력 +22% · 공격 속도 +25% · 체력 +80","공격력 +18% · 공격 속도 +18%","공격력 +20% · 체력 +120","체력 +250 · 방어 강화","체력 +180 · 공격 속도 +15%","체력 +500 · 재생 강화","공격 속도 +35% · 스킬 강화","체력 +200 · 공격 속도 +20%","체력 +650 · 시작 보호막","유닛의 모든 장비를 보관함으로 회수하는 1회용 소모품"};
    private static readonly int[,] ItemRecipes={{4,5,6,7},{5,8,9,10},{6,9,11,12},{7,10,12,13}};
    private readonly Unit[] board=new Unit[28], bench=new Unit[9];
    private readonly UnitDef[] shop=new UnitDef[5];
    private readonly Dictionary<string,int> pool=new Dictionary<string,int>();
    private readonly Dictionary<string,Texture2D> textures=new Dictionary<string,Texture2D>();
    private bool lobby=true, battling, win;
    private int difficulty=1, legend, gold, hp=100, level=1, xp, round=1, selectedBench=-1, selectedBoard=-1, selectedItem=-1;
    private float battleProgress;
    private float resultNoticeUntil;
    private string battleText="전투 준비", lastReward="";
    private readonly List<Fighter> fighters=new List<Fighter>();
    private readonly List<LootOrb> lootOrbs=new List<LootOrb>();
    private readonly List<int> inventory=new List<int>();
    private Unit inspectedUnit;
    private Vector2 legendPos=new Vector2(785,690), legendTarget=new Vector2(785,690);
    private bool shopLocked, showCarousel;
    private UnitDef[] carouselUnits=new UnitDef[3]; private int[] carouselItems=new int[3];
    private GUIStyle title, header, label, small, center, button, card, selectedStyle;
    private readonly Color bg=new Color(.035f,.075f,.09f), panel=new Color(.07f,.13f,.15f), accent=new Color(.77f,.9f,.54f);

    // The copied solo game is launched explicitly by MultiLauncher.
    private static void Boot()
    {
        if (FindAnyObjectByType<NativeGame>() != null) return;
        GameObject root=new GameObject("File Island Native Game");
        DontDestroyOnLoad(root); root.AddComponent<NativeGame>();
    }

    private void Awake()
    {
        Application.targetFrameRate=60; Screen.sleepTimeout=SleepTimeout.NeverSleep;
        difficulty=PlayerPrefs.GetInt("multiSoloDifficulty",1); legend=PlayerPrefs.GetInt("multiSoloLegend",0);
        ResetGame();
    }

    private void InitPool() { int[] sizes={0,39,26,21,13,10}; pool.Clear(); foreach(UnitDef d in Roster) pool[d.id]=sizes[d.cost]; pool["koromon"]--; }
    private Texture2D MakeTexture(Color color) { Texture2D t=new Texture2D(1,1); t.SetPixel(0,0,color); t.Apply(); return t; }
    private void Styles()
    {
        if (title!=null) return;
        title=new GUIStyle(GUI.skin.label){fontSize=32,fontStyle=FontStyle.Bold,alignment=TextAnchor.MiddleCenter}; title.normal.textColor=accent;
        header=new GUIStyle(GUI.skin.label){fontSize=19,fontStyle=FontStyle.Bold}; header.normal.textColor=Color.white;
        label=new GUIStyle(GUI.skin.label){fontSize=15}; label.normal.textColor=Color.white;
        small=new GUIStyle(label){fontSize=11}; small.normal.textColor=new Color(.62f,.71f,.69f);
        center=new GUIStyle(label){alignment=TextAnchor.MiddleCenter}; button=new GUIStyle(GUI.skin.button){fontSize=14,fontStyle=FontStyle.Bold}; button.normal.textColor=Color.white;
        card=new GUIStyle(GUI.skin.box); card.normal.background=MakeTexture(new Color(.08f,.16f,.18f)); selectedStyle=new GUIStyle(card); selectedStyle.normal.background=MakeTexture(new Color(.25f,.34f,.20f));
    }
    private Texture2D Tex(string name) { if(string.IsNullOrEmpty(name)) return null; Texture2D t; if(!textures.TryGetValue(name,out t)){t=Resources.Load<Texture2D>("Sprites/"+name); textures[name]=t;} return t; }
    private UnitMeta Meta(string id){switch(id){
        case "koromon":return new UnitMeta("백신","용형",550,43,.8f,1);case "tsunomon":return new UnitMeta("데이터","야수형",430,43,.85f,3);case "mochimon":return new UnitMeta("백신","곤충형",690,32,.65f,1);case "tanemon":return new UnitMeta("데이터","식물형",460,29,.7f,3);case "pyocomon":return new UnitMeta("백신","조류형",440,36,.75f,3);case "tokomon":return new UnitMeta("백신","천사형",490,28,.7f,3);
        case "agumon":return new UnitMeta("백신","용형",600,48,.8f,1);case "gabumon":return new UnitMeta("데이터","야수형",460,49,.85f,4);case "tentomon":return new UnitMeta("백신","곤충형",760,36,.65f,1);case "palmon":return new UnitMeta("데이터","식물형",490,32,.7f,3);case "piyomon":return new UnitMeta("백신","조류형",530,43,.75f,3);case "patamon":return new UnitMeta("백신","천사형",570,35,.7f,3);
        case "togemon":return new UnitMeta("데이터","식물형",900,46,.7f,1);case "garurumon":return new UnitMeta("데이터","야수형",760,59,.85f,1);case "greymon":return new UnitMeta("백신","용형",1080,60,.7f,1);case "kabuterimon":return new UnitMeta("바이러스","곤충형",690,52,.8f,3);case "angemon":return new UnitMeta("백신","천사형",890,70,.9f,1);case "birdramon":return new UnitMeta("데이터","조류형",670,68,.9f,4);
        case "metalgreymon":return new UnitMeta("바이러스","용형",966,105,.85f,4);case "weregarurumon":return new UnitMeta("데이터","야수형",1323,106,1,1);case "lilimon":return new UnitMeta("데이터","식물형",920,62,.8f,3);case "holyangemon":return new UnitMeta("백신","천사형",989,79,.8f,3);case "atlur":return new UnitMeta("바이러스","곤충형",1370,78,.8f,1);case "garudamon":return new UnitMeta("백신","조류형",1030,97,.95f,4);
        case "herakle":return new UnitMeta("바이러스","곤충형",1782,100,.8f,1);case "hououmon":return new UnitMeta("백신","조류형",1185,107,.9f,4);case "wargreymon":return new UnitMeta("백신","용형",1550,128,1,1);case "metalgarurumon":return new UnitMeta("데이터","야수형",1240,120,1.05f,4);case "rosemon":return new UnitMeta("데이터","식물형",1250,89,.9f,3);case "seraphimon":return new UnitMeta("백신","천사형",1300,118,.9f,3);default:return new UnitMeta("바이러스","악당",700,55,.75f,1);}}
    private void OnGUI()
    {
        Styles(); float scale=Mathf.Min(Screen.width/1920f,Screen.height/1080f); Matrix4x4 old=GUI.matrix;
        GUI.matrix=Matrix4x4.TRS(Vector3.zero,Quaternion.identity,new Vector3(scale,scale,1)); DrawRect(new Rect(0,0,1920,1080),bg);
        if(lobby) DrawLobby(); else DrawGame();if(!string.IsNullOrEmpty(GUI.tooltip)){Vector2 p=Event.current.mousePosition;GUI.Box(new Rect(Mathf.Min(p.x+18,1510),Mathf.Min(p.y+18,980),370,68),GUI.tooltip,card);}GUI.matrix=old;
    }
    private void DrawRect(Rect r,Color c){Color old=GUI.color;GUI.color=c;GUI.DrawTexture(r,Texture2D.whiteTexture);GUI.color=old;}
    private bool Btn(Rect r,string text,bool enabled=true){GUI.enabled=enabled;bool hit=GUI.Button(r,text,button);GUI.enabled=true;return hit;}
    private void Portrait(Rect r,string sprite,string glyph=""){Texture2D t=Tex(sprite);if(t)GUI.DrawTexture(r,t,ScaleMode.ScaleToFit,true);else GUI.Label(r,glyph,title);}

    private void DrawLobby()
    {
        if (Btn(new Rect(20,20,260,60),"멀티 모드 선택으로")) { FindAnyObjectByType<MultiLauncher>().ReturnToMulti(); return; }
        DrawRect(new Rect(180,105,1560,870),panel); GUI.Label(new Rect(400,145,1120,55),"FILE ISLAND LEAGUE",title); GUI.Label(new Rect(450,205,1020,35),"UNITY 6.6 NATIVE ANDROID PROTOTYPE",center);
        GUI.Label(new Rect(245,280,390,35),"게임 모드",header); GUI.Box(new Rect(245,325,390,120),GUIContent.none,selectedStyle); GUI.Label(new Rect(275,345,80,80),"1P",title); GUI.Label(new Rect(380,350,220,30),"솔로 플레이",header); GUI.Label(new Rect(380,388,220,25),"플레이어 1명 + AI 7명",small);
        GUI.Label(new Rect(245,490,390,35),"난이도",header);
        for(int i=0;i<3;i++){Rect r=new Rect(245+i*135,535,120,80);GUI.Box(r,GUIContent.none,i==difficulty?selectedStyle:card);if(Btn(new Rect(r.x+5,r.y+5,r.width-10,r.height-10),Difficulties[i]))difficulty=i;}
        GUI.Label(new Rect(245,630,390,50),difficulty==0?"AI 전투력 88% · 처음 플레이 추천":difficulty==1?"표준 전투력 · 기본 난이도":"AI 전투력 112% · 숙련자 추천",small);
        GUI.Label(new Rect(700,280,970,35),"전설이 선택",header);
        for(int i=0;i<4;i++){Rect r=new Rect(700+i*235,325,210,300);GUI.Box(r,GUIContent.none,i==legend?selectedStyle:card);Portrait(new Rect(r.x+35,r.y+25,140,150),LegendSprites[i],"⌁");GUI.Label(new Rect(r.x+10,r.y+190,190,35),Legends[i],center);if(Btn(new Rect(r.x+25,r.y+235,160,45),i==legend?"선택됨":"선택"))legend=i;}
        GUI.Label(new Rect(700,650,970,40),"전설이는 전리품 회수와 승리 연출을 담당하며 능력치에는 영향을 주지 않습니다.",small);
        if(Btn(new Rect(710,760,500,80),Difficulties[difficulty]+" · 솔로 리그 입장")){lobby=false;Save();}
        if(Btn(new Rect(1230,760,300,80),"새 게임 초기화")){ResetGame();lobby=false;}
    }

    private void DrawGame(){DrawTop();DrawLeft();DrawBoard();DrawRight();DrawShop();if(showCarousel)DrawCarousel();}
    private void DrawTop()
    {
        DrawRect(new Rect(0,0,1920,88),new Color(.045f,.09f,.11f)); GUI.Label(new Rect(35,18,370,45),"DIGITAL AUTO ARENA",header);
        Stat(760,"라운드",RoundLabel());Stat(955,"체력",hp.ToString());Stat(1145,"골드",gold+" G");Stat(1335,"레벨 / 경험치",level==9?"LV.9 MAX":$"LV.{level}  {xp}/{NeedXp()}");
        if(Btn(new Rect(1650,20,110,50),"로비",!battling))lobby=true;if(Btn(new Rect(1770,20,115,50),"종료"))Application.Quit();
    }
    private void Stat(float x,string key,string value){GUI.Label(new Rect(x,15,180,25),key,small);GUI.Label(new Rect(x,38,180,38),value,header);}
    private void DrawLeft()
    {
        DrawRect(new Rect(20,108,255,710),panel);GUI.Label(new Rect(40,125,220,30),"활성 시너지",header);
        Trait(175,"전사",board.Count(u=>u!=null&&u.def.role=="전사"));Trait(240,"탱커",board.Count(u=>u!=null&&u.def.role=="탱커"));Trait(305,"사수",board.Count(u=>u!=null&&u.def.role=="사수"));
        GUI.Label(new Rect(40,390,220,30),"전설이",header);GUI.Label(new Rect(40,435,215,36),Legends[legend],center);GUI.Label(new Rect(40,480,215,70),"필드에서 직접 움직이며\n전투와 전리품을 지켜봅니다.",center);
        GUI.Label(new Rect(40,560,220,30),"장비 보관함",header);
        for(int i=0;i<inventory.Count&&i<8;i++){Rect ir=new Rect(40+(i%4)*52,600+(i/4)*50,45,43);int item=inventory[i];GUI.Box(ir,GUIContent.none,i==selectedItem?selectedStyle:card);if(GUI.Button(ir,new GUIContent(ItemIcons[item],ItemNames[item]+"\n"+ItemDescriptions[item]),button)){selectedItem=selectedItem==i?-1:i;selectedBench=-1;selectedBoard=-1;}}
        string itemHelp=selectedItem>=0&&selectedItem<inventory.Count?ItemNames[inventory[selectedItem]]+" · "+ItemDescriptions[inventory[selectedItem]]:"장비 선택 → 유닛 선택 · 재료 2개는 자동 합성";GUI.Label(new Rect(40,699,215,42),itemHelp,small);
        GUI.Label(new Rect(40,746,220,27),"난이도",header);GUI.Label(new Rect(40,775,220,25),Difficulties[difficulty]+$" · AI {DifficultyScale[difficulty]*100:0}%",center);
    }
    private void Trait(float y,string name,int count){GUI.Box(new Rect(38,y,218,52),GUIContent.none,count>=2?selectedStyle:card);GUI.Label(new Rect(50,y+8,110,34),name,label);GUI.Label(new Rect(175,y+8,65,34),count+" / 2",label);}
    private void DrawBoard()
    {
        string boardTitle=battling?battleText:Time.unscaledTime<resultNoticeUntil?battleText:$"{RoundType()} · 배치 {board.Count(u=>u!=null)} / {level} · 유닛 선택 후 이동";GUI.Label(new Rect(300,105,1040,35),boardTitle,header);
        DrawRect(new Rect(300,148,1040,670),new Color(.055f,.16f,.16f));float cw=138,ch=74,gap=7;
        for(int row=0;row<8;row++)for(int col=0;col<7;col++){
            Rect r=new Rect(322+col*(cw+gap),165+row*(ch+gap),cw,ch);DrawRect(r,row<4?new Color(.16f,.11f,.13f):new Color(.08f,.20f,.18f));
            if(!battling&&row>=4){int idx=(row-4)*7+col;Unit u=board[idx];if(idx==selectedBoard)DrawRect(new Rect(r.x+3,r.y+3,r.width-6,r.height-6),new Color(.35f,.52f,.18f));if(GUI.Button(r,GUIContent.none,GUIStyle.none))ClickBoard(idx);if(u!=null)DrawUnit(r,u,false);}
        }
        if(battling)foreach(Fighter f in fighters.Where(x=>!x.dead))DrawFighter(f);
        for(int oi=lootOrbs.Count-1;oi>=0;oi--){LootOrb orb=lootOrbs[oi];Rect or=new Rect(orb.pos.x-24,orb.pos.y-24,48,48);DrawRect(or,orb.rarity==2?new Color(1,.75f,.15f):orb.rarity==1?new Color(.25f,.65f,1):new Color(.65f,1,.65f));GUI.Label(or,"◆",center);if(!battling&&GUI.Button(or,GUIContent.none,GUIStyle.none))CollectOrb(orb);}
        UpdateLegendInput();
        legendPos=Vector2.MoveTowards(legendPos,legendTarget,Time.deltaTime*(battling?260:520));
        Portrait(new Rect(legendPos.x-43,legendPos.y-43,86,86),LegendSprites[legend],"⌁");
        GUI.Label(new Rect(legendPos.x-65,legendPos.y+34,130,20),Legends[legend],center);
        if(battling){DrawRect(new Rect(430,790,780,10),new Color(.1f,.1f,.1f));DrawRect(new Rect(430,790,780*battleProgress,10),accent);}
    }
    private void ClickBoard(int idx)
    {
        if(selectedItem>=0&&board[idx]!=null){Equip(board[idx]);return;}
        if(selectedBench>=0){Unit temp=board[idx];if(temp==null&&board.Count(u=>u!=null)>=level)return;board[idx]=bench[selectedBench];bench[selectedBench]=temp;selectedBench=-1;selectedBoard=-1;Save();return;}
        if(selectedBoard>=0){if(selectedBoard==idx){selectedBoard=-1;return;}Unit temp=board[idx];board[idx]=board[selectedBoard];board[selectedBoard]=temp;selectedBoard=-1;Save();return;}
        if(board[idx]!=null){selectedBoard=idx;inspectedUnit=board[idx];}
    }
    private void UpdateLegendInput()
    {
        if(battling)return;Event e=Event.current;if(e==null||(e.type!=EventType.MouseDown&&e.type!=EventType.MouseDrag))return;
        Vector2 p=e.mousePosition;if(p.x>=310&&p.x<=1330&&p.y>=155&&p.y<=810){legendTarget=new Vector2(Mathf.Clamp(p.x,365,1275),Mathf.Clamp(p.y,205,745));e.Use();}
    }
    private void DrawFighter(Fighter f)
    {
        float x=322+f.pos.x*145,y=165+f.pos.y*81;Rect r=new Rect(x,y,112,70);
        if(f.hitFlash>0)DrawRect(new Rect(x-4,y-4,120,78),new Color(1,.22f,.16f,.75f));
        if(f.attackFlash>0)DrawRect(new Rect(x+35,y+26,95,8),f.enemy?new Color(1,.3f,.2f):accent);
        Portrait(new Rect(x+8,y,62,54),f.unit.def.sprite);GUI.Label(new Rect(x+66,y+4,80,20),f.unit.def.name,small);
        DrawRect(new Rect(x+8,y+57,100,8),new Color(.12f,.08f,.08f));DrawRect(new Rect(x+8,y+57,100*Mathf.Clamp01(f.hp/f.maxHp),8),f.enemy?new Color(.9f,.22f,.18f):new Color(.35f,.9f,.42f));if(!f.enemy&&GUI.Button(r,GUIContent.none,GUIStyle.none)){if(selectedItem>=0)Equip(f.unit);else inspectedUnit=f.unit;}
    }
    private void DrawUnit(Rect r,Unit u,bool enemy){Texture2D t=Tex(u.def.sprite);if(t)GUI.DrawTexture(new Rect(r.x+7,r.y+2,62,55),t,ScaleMode.ScaleToFit,true);GUI.Label(new Rect(r.x+67,r.y+7,r.width-70,22),u.def.name,small);GUI.Label(new Rect(r.x+67,r.y+29,r.width-70,18),new string('★',u.star),small);GUI.Label(new Rect(r.x+67,r.y+49,r.width-70,18),(enemy?"적":u.def.role)+string.Concat(u.items.Select(i=>ItemIcons[i])),small);}
    private void DrawRight()
    {
        DrawRect(new Rect(1360,108,540,710),panel);if(inspectedUnit!=null)DrawUnitDetail();else{GUI.Label(new Rect(1380,125,500,30),"리그 순위",header);string[] names={"나의 테이머","태일","매튜","소라","미나","리키","한솔","나리"};
        for(int i=0;i<8;i++){float y=170+i*55;GUI.Box(new Rect(1380,y,500,46),GUIContent.none,i==0?selectedStyle:card);GUI.Label(new Rect(1395,y+8,35,28),(i+1).ToString(),label);GUI.Label(new Rect(1440,y+8,250,28),names[i],label);GUI.Label(new Rect(1735,y+8,125,28),i==0?hp+" HP":Mathf.Max(0,100-round*i)+" HP",small);}}
        GUI.Label(new Rect(1380,635,500,30),"대기석",header);
        for(int i=0;i<9;i++){Rect r=new Rect(1380+(i%5)*98,675+(i/5)*63,88,55);GUI.Box(r,GUIContent.none,i==selectedBench?selectedStyle:card);if(bench[i]!=null){Portrait(new Rect(r.x+2,r.y+1,45,42),bench[i].def.sprite);GUI.Label(new Rect(r.x+42,r.y+3,43,27),bench[i].def.name,small);GUI.Label(new Rect(r.x+42,r.y+29,43,20),new string('★',bench[i].star),small);}if(GUI.Button(r,GUIContent.none,GUIStyle.none))ClickBench(i);}
        if(Btn(new Rect(1380,780,235,38),"선택 유닛 판매",selectedBench>=0)){Unit u=bench[selectedBench];gold+=u.def.cost*(int)Mathf.Pow(3,u.star-1);pool[u.def.id]+=(int)Mathf.Pow(3,u.star-1);bench[selectedBench]=null;if(inspectedUnit==u)inspectedUnit=null;selectedBench=-1;Save();}
    }
    private void DrawUnitDetail()
    {
        Unit u=inspectedUnit;UnitMeta m=Meta(u.def.id);if(Btn(new Rect(1825,120,55,38),"×")){inspectedUnit=null;return;}Portrait(new Rect(1390,145,150,145),u.def.sprite);GUI.Label(new Rect(1560,150,250,34),u.def.name,header);GUI.Label(new Rect(1560,190,280,26),new string('★',u.star)+$"  ·  {u.def.cost}코스트",label);GUI.Label(new Rect(1560,225,280,25),$"{m.attr} · {m.family} · {u.def.role}",label);
        float mult=Mathf.Pow(1.8f,u.star-1);GUI.Label(new Rect(1390,305,470,28),"유닛 능력치",header);DetailStat(1390,342,"체력",Mathf.RoundToInt(m.hp*mult).ToString());DetailStat(1510,342,"공격력",Mathf.RoundToInt(m.atk*Mathf.Pow(1.5f,u.star-1)).ToString());DetailStat(1630,342,"공속",m.speed.ToString("0.00"));DetailStat(1750,342,"사거리",m.range.ToString());
        GUI.Label(new Rect(1390,405,470,28),"기여 시너지",header);int attrCount=SynergyCount(m.attr),familyCount=SynergyCount(m.family),roleCount=board.Count(x=>x!=null&&x.def.role==u.def.role);SynergyLine(1390,442,m.attr,attrCount,2,"같은 속성 2명부터 속성 효과 활성화");SynergyLine(1390,493,m.family,familyCount,2,"같은 계열 2/4명에서 전투 보너스 강화");SynergyLine(1390,544,u.def.role,roleCount,2,RoleDescription(u.def.role));
        GUI.Label(new Rect(1390,595,120,24),"장착 장비",small);for(int i=0;i<u.items.Count;i++){int item=u.items[i];GUI.Button(new Rect(1510+i*165,588,155,38),new GUIContent(ItemIcons[item]+" "+ItemNames[item],ItemNames[item]+"\n"+ItemDescriptions[item]),button);}
    }
    private void DetailStat(float x,float y,string key,string value){GUI.Box(new Rect(x,y,108,52),GUIContent.none,card);GUI.Label(new Rect(x+5,y+4,98,18),key,small);GUI.Label(new Rect(x+5,y+22,98,25),value,center);}
    private int SynergyCount(string trait){return board.Where(x=>x!=null).GroupBy(x=>x.def.id).Select(g=>Meta(g.First().def.id)).Count(m=>m.attr==trait||m.family==trait);}
    private void SynergyLine(float x,float y,string name,int count,int need,string desc){GUI.Box(new Rect(x,y,470,44),GUIContent.none,count>=need?selectedStyle:card);GUI.Label(new Rect(x+12,y+4,120,20),$"{name}  {count}/{need}",label);GUI.Label(new Rect(x+142,y+5,315,32),desc,small);}
    private string RoleDescription(string role){if(role=="탱커")return "2명: 방어력과 생존력 증가";if(role=="전사")return "2명: 공격력과 흡혈 증가";if(role=="사수")return "2명: 공격 속도 증가";if(role=="마법사")return "2명: 스킬 피해 증가";return "2명: 아군 회복과 보호 효과 증가";}
    private void ClickBench(int idx)
    {
        if(selectedItem>=0&&bench[idx]!=null){Equip(bench[idx]);return;}
        if(battling){selectedBench=bench[idx]!=null?(selectedBench==idx?-1:idx):-1;return;}
        if(selectedBoard>=0){Unit temp=bench[idx];bench[idx]=board[selectedBoard];board[selectedBoard]=temp;selectedBoard=-1;selectedBench=-1;Save();return;}
        if(selectedBench>=0){if(selectedBench==idx){selectedBench=-1;return;}Unit temp=bench[idx];bench[idx]=bench[selectedBench];bench[selectedBench]=temp;selectedBench=-1;Save();return;}
        if(bench[idx]!=null){selectedBench=idx;inspectedUnit=bench[idx];}
    }
    private void Equip(Unit unit)
    {
        if(selectedItem<0||selectedItem>=inventory.Count)return;int item=inventory[selectedItem];
        if(item==14){if(unit.items.Count==0)return;inventory.RemoveAt(selectedItem);inventory.AddRange(unit.items);unit.items.Clear();selectedItem=-1;lastReward="자석 제거기 사용 · 장비를 보관함으로 회수했습니다";Save();return;}
        if(item<=3){int partner=unit.items.FindIndex(x=>x<=3);if(partner>=0){int completed=ItemRecipes[unit.items[partner],item];unit.items[partner]=completed;inventory.RemoveAt(selectedItem);selectedItem=-1;lastReward=ItemNames[completed]+" 자동 합성 완료";Save();return;}}
        if(unit.items.Count>=2)return;unit.items.Add(item);inventory.RemoveAt(selectedItem);selectedItem=-1;Save();
    }
    private void DrawShop()
    {
        DrawRect(new Rect(20,838,1880,222),new Color(.045f,.10f,.12f));GUI.Label(new Rect(40,855,230,30),"디지몬 모집",header);
        GUI.Label(new Rect(270,842,800,22),ShopOddsText(),small);
        if(Btn(new Rect(40,895,145,42),"새로고침 2G",gold>=2)){gold-=2;RollShop();}if(Btn(new Rect(40,943,145,42),"경험치 +4",gold>=4&&level<9)){gold-=4;AddXp(4);Save();}if(Btn(new Rect(40,991,145,42),shopLocked?"잠금 유지 중":"상점 잠금")){shopLocked=!shopLocked;Save();}
        for(int i=0;i<5;i++){Rect r=new Rect(270+i*245,865,225,165);GUI.Box(r,GUIContent.none,card);UnitDef d=shop[i];if(d!=null){Portrait(new Rect(r.x+8,r.y+8,105,100),d.sprite);GUI.Label(new Rect(r.x+112,r.y+12,105,28),d.name,label);GUI.Label(new Rect(r.x+112,r.y+44,105,22),d.role,small);GUI.Label(new Rect(r.x+112,r.y+72,105,25),d.cost+" G",header);GUI.Label(new Rect(r.x+12,r.y+112,95,25),"POOL "+pool[d.id],small);if(Btn(new Rect(r.x+112,r.y+108,100,42),"구매",gold>=d.cost)&&Buy(i))Save();}else GUI.Label(r,"판매 완료",center);}
        string action=RoundType()=="초밥집"?RoundLabel()+" 선택창 열기":RoundLabel()+" 전투 시작";if(Btn(new Rect(1530,875,330,135),battling?"전투 진행 중":action,!battling&&(RoundType()=="초밥집"||board.Any(u=>u!=null)))){if(RoundType()=="초밥집")OpenCarousel();else StartCoroutine(Battle());}
    }
    private string ShopOddsText(){return $"LV.{level} 배치 {board.Count(u=>u!=null)}/{level}  ·  상점 확률  1G {ShopOdds[level-1,0]}%  2G {ShopOdds[level-1,1]}%  3G {ShopOdds[level-1,2]}%  4G {ShopOdds[level-1,3]}%  5G {ShopOdds[level-1,4]}%";}
    private void OpenCarousel(){int cost=Mathf.Clamp(2+(round-4)/7,1,5);UnitDef[] choices=Roster.Where(d=>d.cost==cost&&pool[d.id]>0).OrderBy(_=>UnityEngine.Random.value).ToArray();for(int i=0;i<3;i++){carouselUnits[i]=choices.Length>0?choices[i%choices.Length]:null;carouselItems[i]=UnityEngine.Random.Range(0,4);}showCarousel=true;}
    private void DrawCarousel()
    {
        DrawRect(new Rect(360,220,1200,610),new Color(.025f,.065f,.075f,.98f));GUI.Label(new Rect(520,255,880,55),RoundLabel()+" · 디지타마 광장",title);GUI.Label(new Rect(520,315,880,32),"유닛과 장비 묶음 하나를 선택하세요",center);
        for(int i=0;i<3;i++){Rect r=new Rect(440+i*360,380,320,310);GUI.Box(r,GUIContent.none,card);UnitDef d=carouselUnits[i];if(d!=null)Portrait(new Rect(r.x+75,r.y+20,170,145),d.sprite);GUI.Label(new Rect(r.x+20,r.y+170,280,32),d==null?"골드 보급":d.name,header);GUI.Label(new Rect(r.x+20,r.y+207,280,30),ItemIcons[carouselItems[i]]+" "+ItemNames[carouselItems[i]],center);if(Btn(new Rect(r.x+45,r.y+250,230,45),"선택"))ChooseCarousel(i);}
    }
    private void ChooseCarousel(int index){UnitDef d=carouselUnits[index];if(d!=null){if(!GrantUnit(d))gold+=d.cost;}else gold+=Mathf.Max(2,round/7);inventory.Add(carouselItems[index]);showCarousel=false;round++;if(!shopLocked)RollShop();Save();}

    private IEnumerator Battle()
    {
        battling=true;battleProgress=0;selectedBoard=-1;SetupBattle();battleText=RoundType()=="크립"?"악당 디지몬 토벌 중 · 유닛들이 실제 전투 중":"상대 테이머와 전투 중 · 유닛들이 실제 전투 중";
        float elapsed=0,maxTime=28f;
        while(elapsed<maxTime&&fighters.Any(f=>!f.dead&&!f.enemy)&&fighters.Any(f=>!f.dead&&f.enemy)){
            float dt=Time.deltaTime;elapsed+=dt;battleProgress=Mathf.Clamp01(elapsed/maxTime);UpdateCombat(dt);yield return null;
        }
        win=fighters.Any(f=>!f.dead&&!f.enemy)&&!fighters.Any(f=>!f.dead&&f.enemy);
        if(elapsed>=maxTime)win=fighters.Where(f=>!f.enemy).Sum(f=>Mathf.Max(0,f.hp))>=fighters.Where(f=>f.enemy).Sum(f=>Mathf.Max(0,f.hp));
        battleProgress=1;yield return new WaitForSeconds(.65f);
        int income=5+Mathf.Min(5,gold/10);AddXp(2);
        if(win){gold+=income;if(RoundType()=="크립")CreateLootOrbs();lastReward=RoundType()=="크립"?$"수입 {income}G · 전리품 구슬 {lootOrbs.Count}개 생성":$"수입 {income}G · 승리";}else{int stage=round<=3?1:2+(round-4)/7;int damage=RoundType()=="크립"?stage+1:Mathf.CeilToInt((2+Mathf.Max(0,stage-2)*2+fighters.Count(f=>f.enemy&&!f.dead))*(1+Mathf.Max(0,stage-2)*.1f));hp=Mathf.Max(0,hp-damage);gold+=income;lastReward=$"체력 -{damage} · 수입 {income}G";}
        battleText=(win?"승리 · ":"패배 · ")+lastReward;resultNoticeUntil=Time.unscaledTime+4.5f;round++;if(!shopLocked)RollShop();fighters.Clear();battling=false;Save();
    }
    private void SetupBattle()
    {
        fighters.Clear();
        for(int i=0;i<board.Length;i++)if(board[i]!=null){int col=i%7,row=i/7+4;fighters.Add(CreateFighter(board[i],false,new Vector2(col,row)));}
        int stage=round<=3?1:2+(round-4)/7,step=round<=3?round:1+(round-4)%7;int enemyCount=stage==1?step:Mathf.Min(7,stage+1);UnitDef enemy=EnemyForRound();
        for(int i=0;i<enemyCount;i++){Unit copy=new Unit(enemy);copy.star=Mathf.Clamp(1+round/9,1,3);fighters.Add(CreateFighter(copy,true,new Vector2(i%7,(i/7)+1)));}
    }
    private Fighter CreateFighter(Unit unit,bool enemy,Vector2 pos)
    {
        float scale=1f;if(enemy){int stage=round<=3?1:2+(round-4)/7,step=round<=3?round:1+(round-4)%7;scale=RoundType()=="크립"?(stage==1?.38f+.12f*step:1f+.18f*(stage-2)):DifficultyScale[difficulty]*(1+round*.035f);}float itemHp=unit.items.Sum(ItemHealth);float health=(75+unit.def.cost*32+itemHp)*Mathf.Pow(1.72f,unit.star-1)*scale;
        return new Fighter{unit=unit,enemy=enemy,pos=pos,hp=health,maxHp=health,attackScale=scale,cooldown=UnityEngine.Random.Range(.1f,.55f)};
    }
    private void UpdateCombat(float dt)
    {
        foreach(Fighter f in fighters){f.hitFlash=Mathf.Max(0,f.hitFlash-dt);f.attackFlash=Mathf.Max(0,f.attackFlash-dt);if(f.dead)continue;f.cooldown-=dt;Fighter target=fighters.Where(x=>!x.dead&&x.enemy!=f.enemy).OrderBy(x=>Vector2.Distance(f.pos,x.pos)).FirstOrDefault();if(target==null)continue;
            float distance=Vector2.Distance(f.pos,target.pos);bool ranged=f.unit.def.role=="사수"||f.unit.def.role=="마법사"||f.unit.def.role=="지원";float range=ranged?2.55f:1.05f;
            if(distance>range){float speed=(ranged?.72f:1.05f)*dt;f.pos=Vector2.MoveTowards(f.pos,target.pos,speed);continue;}
            if(f.cooldown>0)continue;float itemAtk=1+f.unit.items.Sum(ItemAttack);float damage=(13+f.unit.def.cost*5)*Mathf.Pow(1.48f,f.unit.star-1)*itemAtk;if(f.enemy)damage*=f.attackScale;target.hp-=damage;target.hitFlash=.18f;f.attackFlash=.16f;float speedBonus=1+f.unit.items.Sum(ItemSpeed);f.cooldown=(ranged?1.05f:.78f)/speedBonus;if(target.hp<=0){target.hp=0;target.dead=true;}
        }
    }
    private float ItemHealth(int i){switch(i){case 1:return 120;case 3:return 150;case 5:return 80;case 7:return 120;case 8:return 250;case 9:return 180;case 10:return 500;case 12:return 200;case 13:return 650;default:return 0;}}
    private float ItemAttack(int i){switch(i){case 0:return .12f;case 4:return .4f;case 5:return .22f;case 6:return .18f;case 7:return .2f;default:return 0;}}
    private float ItemSpeed(int i){switch(i){case 2:return .12f;case 5:return .25f;case 6:return .18f;case 9:return .15f;case 11:return .35f;case 12:return .2f;default:return 0;}}
    private void CreateLootOrbs(){int kills=fighters.Count(f=>f.enemy&&f.dead);for(int i=0;i<kills;i++){float roll=UnityEngine.Random.value;if(roll<.42f)continue;int rarity=roll<.72f?0:roll<.92f?1:2;lootOrbs.Add(new LootOrb{rarity=rarity,rewardType=rarity==1?1:(rarity==2&&UnityEngine.Random.value<.55f?2:0),amount=rarity==2?3+Mathf.Max(1,round/7):1+UnityEngine.Random.Range(0,3),pos=new Vector2(UnityEngine.Random.Range(420,1220),UnityEngine.Random.Range(240,650))});}}
    private void CollectOrb(LootOrb orb){legendTarget=orb.pos;if(orb.rewardType==1){int item=UnityEngine.Random.value<.15f?14:UnityEngine.Random.Range(0,4);inventory.Add(item);lastReward=ItemNames[item]+" 획득";}else if(orb.rewardType==2){UnitDef[] choices=Roster.Where(d=>d.cost==Mathf.Clamp(1+round/7,1,4)&&pool[d.id]>0).ToArray();if(choices.Length>0){UnitDef d=choices[UnityEngine.Random.Range(0,choices.Length)];if(GrantUnit(d))lastReward=d.name+" 획득";else{gold+=d.cost;lastReward=d.cost+"G 전환";}}}else{gold+=orb.amount;lastReward=orb.amount+"G 획득";}lootOrbs.Remove(orb);Save();}
    private bool GrantUnit(UnitDef d){int slot=Array.FindIndex(bench,u=>u==null);if(slot>=0){bench[slot]=new Unit(d);pool[d.id]--;Combine(d.id);return true;}List<UnitRef> pair=FindUnits(d.id,1).Take(2).ToList();if(pair.Count<2)return false;pool[d.id]--;MergePurchasedThird(pair[0],pair[1]);Combine(d.id);return true;}
    private string RoundLabel(){int stage=round<=3?1:2+(round-4)/7,step=round<=3?round:1+(round-4)%7;return stage+"-"+step;}
    private string RoundType(){if(round<=3)return "크립";int step=1+(round-4)%7;return step==4?"초밥집":step==7?"크립":"PvP";}
    private UnitDef EnemyForRound(){string id=round<=1?"Kuwagamon":round==2?"Shellmon":round<8?"Devimon":"Etemon";return new UnitDef(id.ToLower(),id,Mathf.Clamp(1+round/5,1,5),id,"적");}
    private int NeedXp(){int[] need={0,2,2,6,10,20,36,60,68,0};return need[Mathf.Clamp(level,1,9)];}
    private void AddXp(int amount){xp+=amount;while(level<9&&xp>=NeedXp()){xp-=NeedXp();level++;}if(level==9)xp=0;}
    private bool HasThree(string id){return board.Concat(bench).Any(u=>u!=null&&u.def.id==id&&u.star==3);}
    private void RollShop(){foreach(UnitDef old in shop)if(old!=null)pool[old.id]++;for(int i=0;i<5;i++){int roll=UnityEngine.Random.Range(0,100),cost=1;for(int c=0;c<5;c++){roll-=ShopOdds[level-1,c];if(roll<0){cost=c+1;break;}}UnitDef[] eligible=Roster.Where(d=>d.cost==cost&&pool[d.id]>0&&!HasThree(d.id)).ToArray();if(eligible.Length==0){shop[i]=null;continue;}int total=eligible.Sum(d=>pool[d.id]),pick=UnityEngine.Random.Range(0,total);UnitDef chosen=eligible[0];foreach(UnitDef d in eligible){pick-=pool[d.id];if(pick<0){chosen=d;break;}}shop[i]=chosen;pool[chosen.id]--;}}
    private bool Buy(int index)
    {
        UnitDef d=shop[index];if(d==null||gold<d.cost)return false;int slot=Array.FindIndex(bench,u=>u==null);
        if(slot<0){List<UnitRef> pair=FindUnits(d.id,1).Take(2).ToList();if(pair.Count<2)return false;gold-=d.cost;shop[index]=null;MergePurchasedThird(pair[0],pair[1]);Combine(d.id);return true;}
        gold-=d.cost;bench[slot]=new Unit(d);shop[index]=null;Combine(d.id);return true;
    }
    private List<UnitRef> FindUnits(string id,int star)
    {
        List<UnitRef> refs=new List<UnitRef>();for(int i=0;i<board.Length;i++)if(board[i]!=null&&board[i].def.id==id&&board[i].star==star)refs.Add(new UnitRef{boardArea=true,index=i,unit=board[i]});for(int i=0;i<bench.Length;i++)if(bench[i]!=null&&bench[i].def.id==id&&bench[i].star==star)refs.Add(new UnitRef{boardArea=false,index=i,unit=bench[i]});return refs;
    }
    private void ClearRef(UnitRef r){if(r.boardArea)board[r.index]=null;else bench[r.index]=null;}
    private void PutRef(UnitRef r,Unit u){if(r.boardArea)board[r.index]=u;else bench[r.index]=u;}
    private void MergePurchasedThird(UnitRef keep,UnitRef consumed){List<int> items=new List<int>(keep.unit.items);items.AddRange(consumed.unit.items);ClearRef(keep);ClearRef(consumed);keep.unit.star++;keep.unit.items.Clear();keep.unit.items.AddRange(items.Take(2));inventory.AddRange(items.Skip(2));PutRef(keep,keep.unit);}
    private void Combine(string id)
    {
        bool changed=true;while(changed){changed=false;for(int star=1;star<3;star++){List<UnitRef> refs=FindUnits(id,star).Take(3).ToList();if(refs.Count<3)continue;UnitRef keep=refs[0];List<int> items=refs.SelectMany(r=>r.unit.items).ToList();foreach(UnitRef r in refs)ClearRef(r);keep.unit.star++;keep.unit.items.Clear();keep.unit.items.AddRange(items.Take(2));inventory.AddRange(items.Skip(2));PutRef(keep,keep.unit);changed=true;break;}}
        if(HasThree(id))for(int i=0;i<5;i++)if(shop[i]!=null&&shop[i].id==id){pool[id]++;shop[i]=null;}
    }
    private void ResetGame(){Array.Clear(board,0,board.Length);Array.Clear(bench,0,bench.Length);inventory.Clear();lootOrbs.Clear();gold=0;hp=100;level=1;xp=0;round=1;selectedBench=selectedBoard=selectedItem=-1;shopLocked=false;legendPos=legendTarget=new Vector2(785,690);InitPool();board[3]=new Unit(Roster[0]);RollShop();Save();}
    private void Save(){PlayerPrefs.SetInt("multiSoloDifficulty",difficulty);PlayerPrefs.SetInt("multiSoloLegend",legend);PlayerPrefs.SetInt("multiSoloRound",round);PlayerPrefs.SetInt("multiSoloGold",gold);PlayerPrefs.SetInt("multiSoloHp",hp);PlayerPrefs.SetInt("multiSoloLevel",level);PlayerPrefs.SetInt("multiSoloXp",xp);PlayerPrefs.SetInt("multiSoloShopLocked",shopLocked?1:0);PlayerPrefs.Save();}
}
