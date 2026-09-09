/* Fixed-stage roster and original gameplay recipes. */
(function(root){
'use strict';
const data = {
  "units": {
    "koromon": {
      "id": "koromon",
      "name": "코로몬",
      "cost": 1,
      "stage": "유년기",
      "attr": "백신",
      "family": "용형",
      "role": "fighter",
      "hp": 550,
      "atk": 43,
      "speed": 0.8,
      "range": 1,
      "sprite": "Koromon",
      "skill": "bubble"
    },
    "tsunomon": {
      "id": "tsunomon",
      "name": "뿔몬",
      "cost": 1,
      "stage": "유년기",
      "attr": "데이터",
      "family": "야수형",
      "role": "ranger",
      "hp": 430,
      "atk": 43,
      "speed": 0.85,
      "range": 3,
      "sprite": "Tunomon",
      "skill": "bubble"
    },
    "mochimon": {
      "id": "mochimon",
      "name": "모티몬",
      "cost": 1,
      "stage": "유년기",
      "attr": "백신",
      "family": "곤충형",
      "role": "tank",
      "hp": 690,
      "atk": 32,
      "speed": 0.65,
      "range": 1,
      "sprite": "Mochimon",
      "skill": "bubble"
    },
    "tanemon": {
      "id": "tanemon",
      "name": "시드몬",
      "cost": 1,
      "stage": "유년기",
      "attr": "데이터",
      "family": "식물형",
      "role": "support",
      "hp": 460,
      "atk": 29,
      "speed": 0.7,
      "range": 3,
      "sprite": "Tanemon",
      "skill": "heal"
    },
    "pyocomon": {
      "id": "pyocomon",
      "name": "어니몬",
      "cost": 1,
      "stage": "유년기",
      "attr": "백신",
      "family": "조류형",
      "role": "mage",
      "hp": 440,
      "atk": 36,
      "speed": 0.75,
      "range": 3,
      "sprite": "Pyocomon",
      "skill": "bubble"
    },
    "tokomon": {
      "id": "tokomon",
      "name": "토코몬",
      "cost": 1,
      "stage": "유년기",
      "attr": "백신",
      "family": "천사형",
      "role": "support",
      "hp": 490,
      "atk": 28,
      "speed": 0.7,
      "range": 3,
      "sprite": "Tokomon",
      "skill": "ward"
    },
    "agumon": {
      "id": "agumon",
      "name": "아구몬",
      "cost": 2,
      "attr": "백신",
      "family": "용형",
      "role": "fighter",
      "hp": 600,
      "atk": 48,
      "speed": 0.8,
      "range": 1,
      "sprite": "Agumon",
      "skill": "flame",
      "stage": "성장기"
    },
    "gabumon": {
      "id": "gabumon",
      "name": "파피몬",
      "cost": 2,
      "attr": "데이터",
      "family": "야수형",
      "role": "ranger",
      "hp": 460,
      "atk": 49,
      "speed": 0.85,
      "range": 4,
      "sprite": "Gabumon",
      "skill": "pierce",
      "stage": "성장기"
    },
    "tentomon": {
      "id": "tentomon",
      "name": "텐타몬",
      "cost": 2,
      "attr": "백신",
      "family": "곤충형",
      "role": "tank",
      "hp": 760,
      "atk": 36,
      "speed": 0.65,
      "range": 1,
      "sprite": "Tentomon",
      "skill": "shock",
      "stage": "성장기"
    },
    "palmon": {
      "id": "palmon",
      "name": "팔몬",
      "cost": 2,
      "attr": "데이터",
      "family": "식물형",
      "role": "support",
      "hp": 490,
      "atk": 32,
      "speed": 0.7,
      "range": 3,
      "sprite": "Palmon",
      "skill": "heal",
      "stage": "성장기"
    },
    "piyomon": {
      "id": "piyomon",
      "name": "피요몬",
      "cost": 2,
      "attr": "백신",
      "family": "조류형",
      "role": "mage",
      "hp": 530,
      "atk": 43,
      "speed": 0.75,
      "range": 3,
      "sprite": "Piyomon",
      "skill": "flame",
      "stage": "성장기"
    },
    "patamon": {
      "id": "patamon",
      "name": "파닥몬",
      "cost": 2,
      "attr": "백신",
      "family": "천사형",
      "role": "support",
      "hp": 570,
      "atk": 35,
      "speed": 0.7,
      "range": 3,
      "sprite": "Patamon",
      "skill": "ward",
      "stage": "성장기"
    },
    "togemon": {
      "id": "togemon",
      "name": "니드몬",
      "cost": 3,
      "attr": "데이터",
      "family": "식물형",
      "role": "tank",
      "hp": 900,
      "atk": 46,
      "speed": 0.7,
      "range": 1,
      "sprite": "Togemon",
      "skill": "thorns",
      "stage": "성숙기"
    },
    "garurumon": {
      "id": "garurumon",
      "name": "가루몬",
      "cost": 3,
      "attr": "데이터",
      "family": "야수형",
      "role": "fighter",
      "hp": 760,
      "atk": 59,
      "speed": 0.85,
      "range": 1,
      "sprite": "Garurumon",
      "skill": "rush",
      "stage": "성숙기"
    },
    "greymon": {
      "id": "greymon",
      "name": "그레이몬",
      "cost": 3,
      "attr": "백신",
      "family": "용형",
      "role": "tank",
      "hp": 1080,
      "atk": 60,
      "speed": 0.7,
      "range": 1,
      "sprite": "Greymon",
      "skill": "flame",
      "stage": "성숙기"
    },
    "kabuterimon": {
      "id": "kabuterimon",
      "name": "캅테리몬",
      "cost": 3,
      "attr": "바이러스",
      "family": "곤충형",
      "role": "mage",
      "hp": 690,
      "atk": 52,
      "speed": 0.8,
      "range": 3,
      "sprite": "Kabuterimon",
      "skill": "shock",
      "stage": "성숙기"
    },
    "angemon": {
      "id": "angemon",
      "name": "엔젤몬",
      "cost": 3,
      "attr": "백신",
      "family": "천사형",
      "role": "fighter",
      "hp": 890,
      "atk": 70,
      "speed": 0.9,
      "range": 1,
      "sprite": "Angemon",
      "skill": "smite",
      "stage": "성숙기"
    },
    "birdramon": {
      "id": "birdramon",
      "name": "버드라몬",
      "cost": 3,
      "attr": "데이터",
      "family": "조류형",
      "role": "ranger",
      "hp": 670,
      "atk": 68,
      "speed": 0.9,
      "range": 4,
      "sprite": "Birdramon",
      "skill": "flame",
      "stage": "성숙기"
    },
    "metalgreymon": {
      "id": "metalgreymon",
      "name": "메탈그레이몬",
      "cost": 4,
      "attr": "바이러스",
      "family": "용형",
      "role": "ranger",
      "hp": 966,
      "atk": 105,
      "speed": 0.85,
      "range": 4,
      "sprite": "Metal_Greymon",
      "skill": "missile",
      "stage": "완전체"
    },
    "weregarurumon": {
      "id": "weregarurumon",
      "name": "워가루몬",
      "cost": 4,
      "attr": "데이터",
      "family": "야수형",
      "role": "fighter",
      "hp": 1323,
      "atk": 106,
      "speed": 1,
      "range": 1,
      "sprite": "Were_Garurumon",
      "skill": "rush",
      "stage": "완전체"
    },
    "lilimon": {
      "id": "lilimon",
      "name": "릴리몬",
      "cost": 4,
      "attr": "데이터",
      "family": "식물형",
      "role": "support",
      "hp": 920,
      "atk": 62,
      "speed": 0.8,
      "range": 3,
      "sprite": "Lilimon",
      "skill": "heal",
      "stage": "완전체"
    },
    "holyangemon": {
      "id": "holyangemon",
      "name": "홀리엔젤몬",
      "cost": 4,
      "attr": "백신",
      "family": "천사형",
      "role": "mage",
      "hp": 989,
      "atk": 79,
      "speed": 0.8,
      "range": 3,
      "sprite": "Holy_Angemon",
      "skill": "smite",
      "stage": "완전체"
    },
    "atlur": {
      "id": "atlur",
      "name": "아트라캅테리몬",
      "cost": 4,
      "stage": "완전체",
      "attr": "바이러스",
      "family": "곤충형",
      "role": "tank",
      "hp": 1370,
      "atk": 78,
      "speed": 0.8,
      "range": 1,
      "sprite": "Atlur_Kabuterimon",
      "skill": "shock"
    },
    "garudamon": {
      "id": "garudamon",
      "name": "가루다몬",
      "cost": 4,
      "stage": "완전체",
      "attr": "백신",
      "family": "조류형",
      "role": "ranger",
      "hp": 1030,
      "atk": 97,
      "speed": 0.95,
      "range": 4,
      "sprite": "Garudamon",
      "skill": "flame"
    },
    "herakle": {
      "id": "herakle",
      "name": "헤라클레스캅테리몬",
      "cost": 5,
      "attr": "바이러스",
      "family": "곤충형",
      "role": "tank",
      "hp": 1782,
      "atk": 100,
      "speed": 0.8,
      "range": 1,
      "sprite": "Herakle_Kabuterimon",
      "skill": "storm",
      "stage": "궁극체"
    },
    "hououmon": {
      "id": "hououmon",
      "name": "페닉스몬",
      "cost": 5,
      "attr": "백신",
      "family": "조류형",
      "role": "mage",
      "hp": 1185,
      "atk": 107,
      "speed": 0.9,
      "range": 4,
      "sprite": "Hououmon",
      "skill": "nova",
      "stage": "궁극체"
    },
    "wargreymon": {
      "id": "wargreymon",
      "name": "워그레이몬",
      "cost": 5,
      "stage": "궁극체",
      "attr": "백신",
      "family": "용형",
      "role": "fighter",
      "hp": 1550,
      "atk": 128,
      "speed": 1,
      "range": 1,
      "sprite": "War_Greymon",
      "skill": "missile"
    },
    "metalgarurumon": {
      "id": "metalgarurumon",
      "name": "메탈가루몬",
      "cost": 5,
      "stage": "궁극체",
      "attr": "데이터",
      "family": "야수형",
      "role": "ranger",
      "hp": 1240,
      "atk": 120,
      "speed": 1.05,
      "range": 4,
      "sprite": "Metal_Garurumon",
      "skill": "pierce"
    },
    "rosemon": {
      "id": "rosemon",
      "name": "로제몬",
      "cost": 5,
      "stage": "궁극체",
      "attr": "데이터",
      "family": "식물형",
      "role": "support",
      "hp": 1250,
      "atk": 89,
      "speed": 0.9,
      "range": 3,
      "sprite": "Rosemon",
      "skill": "blessing"
    },
    "seraphimon": {
      "id": "seraphimon",
      "name": "세라피몬",
      "cost": 5,
      "stage": "궁극체",
      "attr": "백신",
      "family": "천사형",
      "role": "mage",
      "hp": 1300,
      "atk": 118,
      "speed": 0.9,
      "range": 3,
      "sprite": "Seraphimon",
      "skill": "storm"
    }
  },
  "skills": {
    "flame": {
      "name": "메가 플레임",
      "description": "대상과 인접 적에게 공격력 230%의 마법 피해.",
      "power": 2.3
    },
    "pierce": {
      "name": "블루 블래스터",
      "description": "대상에게 공격력 350% 피해, 방어력 25% 감소 (5초).",
      "power": 3.5
    },
    "shock": {
      "name": "일렉트로 쇼커",
      "description": "가까운 적 3명에게 공격력 200% 피해, 1.2초 기절.",
      "power": 2
    },
    "heal": {
      "name": "플라워 힐",
      "description": "체력 비율이 낮은 아군 2명의 최대 체력 22% 회복.",
      "power": 0
    },
    "ward": {
      "name": "홀리 프로텍션",
      "description": "체력 비율이 낮은 아군 3명에게 최대 체력 22% 보호막 (5초).",
      "power": 0
    },
    "thorns": {
      "name": "니들 스프레이",
      "description": "주변 2칸 적에게 공격력 220% 피해. 자신에게 최대 체력 18% 보호막.",
      "power": 2.2
    },
    "rush": {
      "name": "울프 클로",
      "description": "현재 대상에게 공격력 400% 피해. 가한 피해 35% 회복.",
      "power": 4
    },
    "smite": {
      "name": "헤븐즈 게이트",
      "description": "체력이 가장 낮은 적에게 공격력 450% 마법 피해.",
      "power": 4.5
    },
    "missile": {
      "name": "기가 디스트로이어",
      "description": "체력이 가장 높은 적 주변 2칸에 공격력 260% 마법 피해.",
      "power": 2.6
    },
    "storm": {
      "name": "기가 블래스터",
      "description": "모든 적에게 공격력 210% 마법 피해, 1초 기절.",
      "power": 2.1
    },
    "nova": {
      "name": "스타라이트 익스플로전",
      "description": "모든 적에게 공격력 260% 마법 피해. 아군 전체 최대 체력 10% 회복.",
      "power": 2.6
    },
    "bubble": {
      "name": "거품 공격",
      "description": "대상에게 공격력 250%의 마법 피해.",
      "power": 2.5
    },
    "blessing": {
      "name": "로즈 블레싱",
      "description": "체력 비율이 낮은 아군 3명의 최대 체력 28% 회복, 대상 주변 적에게 공격력 220% 마법 피해.",
      "power": 2.2
    }
  },
  "traits": {
    "백신": {
      "cuts": [
        2,
        4,
        6
      ],
      "text": [
        "방어력 +12",
        "방어력 +25",
        "방어력 +45"
      ]
    },
    "데이터": {
      "cuts": [
        2,
        4,
        6
      ],
      "text": [
        "공격 속도 +12%",
        "공격 속도 +25%",
        "공격 속도 +40%"
      ]
    },
    "바이러스": {
      "cuts": [
        2,
        3
      ],
      "text": [
        "흡혈 15%",
        "흡혈 28%"
      ]
    },
    "용형": {
      "cuts": [
        2,
        3
      ],
      "text": [
        "공격력 +18%",
        "공격력 +32%"
      ]
    },
    "야수형": {
      "cuts": [
        2,
        3
      ],
      "text": [
        "치명타 +20%",
        "치명타 +35%"
      ]
    },
    "곤충형": {
      "cuts": [
        2,
        3
      ],
      "text": [
        "시작 마나 +20",
        "시작 마나 +40"
      ]
    },
    "식물형": {
      "cuts": [
        2,
        3
      ],
      "text": [
        "초당 최대 체력 1.5% 회복",
        "초당 최대 체력 3% 회복"
      ]
    },
    "천사형": {
      "cuts": [
        2,
        3
      ],
      "text": [
        "시작 보호막 160",
        "시작 보호막 300"
      ]
    },
    "조류형": {
      "cuts": [
        2,
        3
      ],
      "text": [
        "스킬 피해 +20%",
        "스킬 피해 +35%"
      ]
    }
  },
  "items": {
    "attackData": {
      "name": "공격 데이터",
      "kind": "component",
      "icon": "✦",
      "description": "공격력 +12%",
      "atk": 0.12
    },
    "chrome": {
      "name": "크롬디지조이드 조각",
      "kind": "component",
      "icon": "⬡",
      "description": "체력 +120, 방어력 +10",
      "hp": 120,
      "armor": 10
    },
    "energy": {
      "name": "순수 에너지",
      "kind": "component",
      "icon": "✧",
      "description": "초당 마나 +2, 스킬 피해 +8%",
      "manaRate": 2,
      "skillPower": 0.08
    },
    "life": {
      "name": "생명 파편",
      "kind": "component",
      "icon": "❋",
      "description": "체력 +150, 초당 최대 체력 0.5% 회복",
      "hp": 150,
      "regen": 0.005
    },
    "courage": {
      "name": "용기의 문장",
      "kind": "complete",
      "icon": "☀",
      "recipe": [
        "attackData",
        "attackData"
      ],
      "description": "공격력 +40%, 치명타 확률 +10%",
      "atk": 0.4,
      "crit": 0.1
    },
    "friendship": {
      "name": "우정의 문장",
      "kind": "complete",
      "icon": "∞",
      "recipe": [
        "attackData",
        "chrome"
      ],
      "description": "공격력 +22%, 공격 속도 +25%, 방어력 +15",
      "atk": 0.22,
      "speed": 0.25,
      "armor": 15
    },
    "love": {
      "name": "사랑의 문장",
      "kind": "complete",
      "icon": "♥",
      "recipe": [
        "attackData",
        "energy"
      ],
      "description": "공격력 +18%, 스킬 피해 +35%",
      "atk": 0.18,
      "skillPower": 0.35
    },
    "sincerity": {
      "name": "성실의 문장",
      "kind": "complete",
      "icon": "✚",
      "recipe": [
        "attackData",
        "life"
      ],
      "description": "공격력 +20%, 흡혈 +25%",
      "atk": 0.2,
      "lifesteal": 0.25
    },
    "knowledge": {
      "name": "지식의 문장",
      "kind": "complete",
      "icon": "◈",
      "recipe": [
        "chrome",
        "chrome"
      ],
      "description": "체력 +250, 방어력 +60",
      "hp": 250,
      "armor": 60
    },
    "hope": {
      "name": "희망의 문장",
      "kind": "complete",
      "icon": "✴",
      "recipe": [
        "chrome",
        "energy"
      ],
      "description": "방어력 +30, 초당 마나 +5",
      "armor": 30,
      "manaRate": 5
    },
    "purity": {
      "name": "순수의 문장",
      "kind": "complete",
      "icon": "❀",
      "recipe": [
        "chrome",
        "life"
      ],
      "description": "체력 +500, 초당 최대 체력 1.5% 회복",
      "hp": 500,
      "regen": 0.015
    },
    "light": {
      "name": "빛의 문장",
      "kind": "complete",
      "icon": "✵",
      "recipe": [
        "energy",
        "energy"
      ],
      "description": "초당 마나 +7, 스킬 피해 +35%",
      "manaRate": 7,
      "skillPower": 0.35
    },
    "miracle": {
      "name": "기적의 캡슐",
      "kind": "complete",
      "icon": "⬢",
      "recipe": [
        "energy",
        "life"
      ],
      "description": "체력 +200, 초당 마나 +4, 스킬 회복·보호막 +45%",
      "hp": 200,
      "manaRate": 4,
      "healPower": 0.45
    },
    "destiny": {
      "name": "운명의 캡슐",
      "kind": "complete",
      "icon": "◆",
      "recipe": [
        "life",
        "life"
      ],
      "description": "체력 +650, 전투 시작 최대 체력 25% 보호막 (전투 동안)",
      "hp": 650,
      "startShield": 0.25
    }
  },
  "augments": {
    "economy": {
      "name": "알뜰한 테이머",
      "icon": "◎",
      "description": "매 라운드 수입 +2G. 즉시 5G.",
      "gold": 2
    },
    "fortress": {
      "name": "수호의 맹세",
      "icon": "⬡",
      "description": "모든 아군 최대 체력 +15%.",
      "hp": 0.15
    },
    "assault": {
      "name": "공격 프로토콜",
      "icon": "✦",
      "description": "모든 아군 공격력 +12%, 공격 속도 +10%.",
      "atk": 0.12
    },
    "scholar": {
      "name": "성장의 기록",
      "icon": "↗",
      "description": "매 라운드 추가 경험치 +2. 즉시 경험치 +4.",
      "xp": 2
    },
    "focus": {
      "name": "집중의 유대",
      "icon": "◇",
      "description": "모든 아군 시작 마나 +15, 초당 마나 +1.",
      "mana": 15
    }
  },
  "roles": {
    "tank": "탱커",
    "fighter": "전사",
    "ranger": "사수",
    "mage": "마법사",
    "support": "지원"
  },
  "poolSizes": {
    "1": 39,
    "2": 26,
    "3": 21,
    "4": 13,
    "5": 10
  },
  "odds": {
    "1": [100, 0, 0, 0, 0],
    "2": [100, 0, 0, 0, 0],
    "3": [
      75,
      25,
      0,
      0,
      0
    ],
    "4": [
      55,
      30,
      15,
      0,
      0
    ],
    "5": [
      40,
      35,
      23,
      2,
      0
    ],
    "6": [
      25,
      40,
      28,
      7,
      0
    ],
    "7": [
      18,
      30,
      35,
      16,
      1
    ],
    "8": [
      15,
      20,
      32,
      28,
      5
    ],
    "9": [
      10,
      15,
      25,
      35,
      15
    ]
  },
  "xp": {
    "1": 2,
    "2": 2,
    "3": 6,
    "4": 10,
    "5": 20,
    "6": 36,
    "7": 60,
    "8": 68
  },
  "colors": [
    "",
    "#99aba8",
    "#65bb83",
    "#68aee7",
    "#b694ef",
    "#e8c577"
  ],
  "stages": {
    "1": "유년기",
    "2": "성장기",
    "3": "성숙기",
    "4": "완전체",
    "5": "궁극체"
  },
  "components": [
    "attackData",
    "chrome",
    "energy",
    "life"
  ],
  "completedItems": [
    "courage",
    "friendship",
    "love",
    "sincerity",
    "knowledge",
    "hope",
    "purity",
    "light",
    "miracle",
    "destiny"
  ]
};
data.creeps={
  kuwagamon:{id:'kuwagamon',name:'쿠가몬',sprite:'Kuwagamon',cost:1,stage:'성숙기',role:'fighter',attr:'바이러스',family:'곤충형',hp:620,atk:52,speed:.72,range:1,skill:'rush'},
  shellmon:{id:'shellmon',name:'쉘몬',sprite:'Shellmon',cost:1,stage:'성숙기',role:'tank',attr:'데이터',family:'야수형',hp:820,atk:42,speed:.58,range:1,skill:'thorns'},
  devimon:{id:'devimon',name:'데블몬',sprite:'Devimon',cost:2,stage:'성숙기',role:'mage',attr:'바이러스',family:'천사형',hp:880,atk:66,speed:.72,range:2,skill:'shock'},
  etemon:{id:'etemon',name:'에테몬',sprite:'Etemon',cost:3,stage:'완전체',role:'fighter',attr:'바이러스',family:'야수형',hp:1250,atk:92,speed:.82,range:1,skill:'rush'},
  vamdemon:{id:'vamdemon',name:'묘티스몬',sprite:'Vamdemon',cost:4,stage:'완전체',role:'mage',attr:'바이러스',family:'천사형',hp:1600,atk:118,speed:.78,range:3,skill:'nova'},
  piemon:{id:'piemon',name:'피에몬',sprite:'Piemon',cost:5,stage:'궁극체',role:'mage',attr:'바이러스',family:'천사형',hp:2200,atk:158,speed:.88,range:3,skill:'missile'},
  apocalymon:{id:'apocalymon',name:'아포카리몬',sprite:'Apocalymon',cost:5,stage:'초궁극체',role:'tank',attr:'바이러스',family:'용형',hp:3200,atk:185,speed:.62,range:2,skill:'storm'}
};
data.legends={
  bitmon:{id:'bitmon',name:'비트몬',sprite:null,glyph:'⌁',color:'#9ee89e',boom:'dataPulse',finisher:'데이터 브레이크'},
  koromon:{id:'koromon',name:'코로몬',sprite:'Koromon',glyph:'●',color:'#f2a27e',boom:'pepperFlame',finisher:'용기의 불꽃'},
  tokomon:{id:'tokomon',name:'토코몬',sprite:'Tokomon',glyph:'◇',color:'#f4e6a0',boom:'holyRay',finisher:'세븐 헤븐즈'},
  pyocomon:{id:'pyocomon',name:'어니몬',sprite:'Pyocomon',glyph:'≋',color:'#ef9fbc',boom:'spiralWind',finisher:'스파이럴 피날레'}
};
data.difficulties={
  easy:{id:'easy',name:'쉬움',enemyScale:.88,aiIncome:-1,description:'AI 전투력 88% · AI 기본 수입 -1G'},
  normal:{id:'normal',name:'보통',enemyScale:1,aiIncome:0,description:'기본 전투력과 경제로 진행'},
  hard:{id:'hard',name:'어려움',enemyScale:1.12,aiIncome:1,description:'AI 전투력 112% · AI 기본 수입 +1G'}
};
Object.defineProperties(data.units,Object.fromEntries(Object.entries(data.creeps).map(([id,value])=>[id,{value,enumerable:false}])));
root.ArenaData=data;if(typeof module!=='undefined')module.exports=data;
})(typeof globalThis!=='undefined'?globalThis:this);
