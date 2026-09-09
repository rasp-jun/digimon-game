const fs=require('node:fs');
const D=require('./data.js');
D.stages={1:'유년기',2:'성장기',3:'성숙기',4:'완전체',5:'궁극체'};
for(const u of Object.values(D.units)){
  if(['agumon','gabumon','tentomon','palmon'].includes(u.id))u.cost=2;
  if(['togemon','garurumon'].includes(u.id))u.cost=3;
  u.stage=D.stages[u.cost];delete u.evolution;delete u.evoSprite;
  if(u.cost>=4){u.hp=Math.round(u.hp*1.15);u.atk=Math.round(u.atk*1.15);}
}
const rows=[
 ['koromon','코로몬',1,'백신','용형','fighter',550,43,.8,1,'Koromon','bubble'],
 ['tsunomon','뿔몬',1,'데이터','야수형','ranger',430,43,.85,3,'Tunomon','bubble'],
 ['mochimon','모티몬',1,'백신','곤충형','tank',690,32,.65,1,'Mochimon','bubble'],
 ['tanemon','시드몬',1,'데이터','식물형','support',460,29,.7,3,'Tanemon','heal'],
 ['pyocomon','어니몬',1,'백신','조류형','mage',440,36,.75,3,'Pyocomon','bubble'],
 ['tokomon','토코몬',1,'백신','천사형','support',490,28,.7,3,'Tokomon','ward'],
 ['atlur','아트라캅테리몬',4,'바이러스','곤충형','tank',1370,78,.8,1,'Atlur_Kabuterimon','shock'],
 ['garudamon','가루다몬',4,'백신','조류형','ranger',1030,97,.95,4,'Garudamon','flame'],
 ['wargreymon','워그레이몬',5,'백신','용형','fighter',1550,128,1,1,'War_Greymon','missile'],
 ['metalgarurumon','메탈가루몬',5,'데이터','야수형','ranger',1240,120,1.05,4,'Metal_Garurumon','pierce'],
 ['rosemon','로제몬',5,'데이터','식물형','support',1250,89,.9,3,'Rosemon','blessing'],
 ['seraphimon','세라피몬',5,'백신','천사형','mage',1300,118,.9,3,'Seraphimon','storm']
];
for(const [id,name,cost,attr,family,role,hp,atk,speed,range,sprite,skill]of rows)D.units[id]={id,name,cost,stage:D.stages[cost],attr,family,role,hp,atk,speed,range,sprite,skill};
D.units=Object.fromEntries(Object.entries(D.units).sort((a,b)=>a[1].cost-b[1].cost));
D.skills.bubble={name:'거품 공격',description:'대상에게 공격력 250%의 마법 피해.',power:2.5};
D.skills.blessing={name:'로즈 블레싱',description:'체력 비율이 낮은 아군 3명의 최대 체력 28% 회복, 대상 주변 적에게 공격력 220% 마법 피해.',power:2.2};
D.traits['곤충형'].text=['시작 마나 +20','시작 마나 +40'];
D.items={
 attackData:{name:'공격 데이터',kind:'component',icon:'✦',description:'공격력 +12%',atk:.12},
 chrome:{name:'크롬디지조이드 조각',kind:'component',icon:'⬡',description:'체력 +120, 방어력 +10',hp:120,armor:10},
 energy:{name:'순수 에너지',kind:'component',icon:'✧',description:'초당 마나 +2, 스킬 피해 +8%',manaRate:2,skillPower:.08},
 life:{name:'생명 파편',kind:'component',icon:'❋',description:'체력 +150, 초당 최대 체력 0.5% 회복',hp:150,regen:.005},
 courage:{name:'용기의 문장',kind:'complete',icon:'☀',recipe:['attackData','attackData'],description:'공격력 +40%, 치명타 확률 +10%',atk:.4,crit:.1},
 friendship:{name:'우정의 문장',kind:'complete',icon:'∞',recipe:['attackData','chrome'],description:'공격력 +22%, 공격 속도 +25%, 방어력 +15',atk:.22,speed:.25,armor:15},
 love:{name:'사랑의 문장',kind:'complete',icon:'♥',recipe:['attackData','energy'],description:'공격력 +18%, 스킬 피해 +35%',atk:.18,skillPower:.35},
 sincerity:{name:'성실의 문장',kind:'complete',icon:'✚',recipe:['attackData','life'],description:'공격력 +20%, 흡혈 +25%',atk:.2,lifesteal:.25},
 knowledge:{name:'지식의 문장',kind:'complete',icon:'◈',recipe:['chrome','chrome'],description:'체력 +250, 방어력 +60',hp:250,armor:60},
 hope:{name:'희망의 문장',kind:'complete',icon:'✴',recipe:['chrome','energy'],description:'방어력 +30, 초당 마나 +5',armor:30,manaRate:5},
 purity:{name:'순수의 문장',kind:'complete',icon:'❀',recipe:['chrome','life'],description:'체력 +500, 초당 최대 체력 1.5% 회복',hp:500,regen:.015},
 light:{name:'빛의 문장',kind:'complete',icon:'✵',recipe:['energy','energy'],description:'초당 마나 +7, 스킬 피해 +35%',manaRate:7,skillPower:.35},
 miracle:{name:'기적의 캡슐',kind:'complete',icon:'⬢',recipe:['energy','life'],description:'체력 +200, 초당 마나 +4, 스킬 회복·보호막 +45%',hp:200,manaRate:4,healPower:.45},
 destiny:{name:'운명의 캡슐',kind:'complete',icon:'◆',recipe:['life','life'],description:'체력 +650, 전투 시작 최대 체력 25% 보호막 (전투 동안)',hp:650,startShield:.25}
};
D.components=Object.keys(D.items).filter(id=>D.items[id].kind==='component');
D.completedItems=Object.keys(D.items).filter(id=>D.items[id].kind==='complete');
delete D.augments.evolution;
D.augments.focus={name:'집중의 유대',icon:'◇',description:'모든 아군 시작 마나 +15, 초당 마나 +1.',mana:15};
fs.writeFileSync('data.js',`/* Fixed-stage roster and original gameplay recipes. */\n(function(root){\n'use strict';\nconst data = ${JSON.stringify(D,null,2)};\nroot.ArenaData=data;if(typeof module!=='undefined')module.exports=data;\n})(typeof globalThis!=='undefined'?globalThis:this);\n`);
let s=fs.readFileSync('engine.js','utf8');
function replace(a,b){if(!s.includes(a))throw Error('Missing engine replacement: '+a);s=s.replaceAll(a,b);}
replace('version:2','version:3');
replace("['agumon','gabumon','palmon']:[pick(s,['agumon','tentomon']),pick(s,['gabumon','palmon']),'agumon']","['koromon','tsunomon','tanemon']:[pick(s,['koromon','mochimon']),pick(s,['tsunomon','tanemon']),'koromon']");
replace("inventory=['claw','shell']","inventory=['attackData','chrome','energy','life']");
replace('dp:0,dpRate:0,','');replace('evolved:false,jogress:false,','');
replace('skillPower:1,','skillPower:1,healPower:1,startShield:0,');
replace('c.dp+=it.dp||0;c.dpRate+=it.dpRate||0;','c.skillPower+=it.skillPower||0;c.healPower+=it.healPower||0;c.crit+=it.crit||0;c.regen+=it.regen||0;c.startShield+=it.startShield||0;');
replace("if(aug==='evolution'){c.dp+=15;c.dpRate++;}","if(aug==='focus'){c.mana+=15;c.manaRate++;}");
replace('c.hp=c.max;return c;','c.hp=c.max;c.shield=c.max*c.startShield;if(c.shield)c.shieldUntil=60;return c;');
replace("case '곤충형':u.dp+=[20,40][i];break;","case '곤충형':u.mana=Math.min(100,u.mana+[20,40][i]);break;");
replace('u.regen=[.015,.03][i]','u.regen+=[.015,.03][i]');
replace('t.dp=Math.min(100,t.dp+4);','');replace('if(t.hp<=0)a.dp=Math.min(100,a.dp+15);','');
replace('*(u.evolved?1.18:1)*(u.jogress?1.3:1)','');replace('u.dp=Math.min(100,u.dp+14);','');
replace('*(u.evolved?1.3:1)','*u.healPower');
replace("case 'heal':", "case 'blessing':allies.sort((a,b)=>a.hp/a.max-b.hp/b.max).slice(0,3).forEach(v=>u.healing+=heal(v,v.max*.28*u.healPower));targets=enemies.filter(v=>dist(v,target)<=1);break;\n      case 'heal':");
replace('v.max*.1)','v.max*.1*u.healPower)');
s=s.replace(/  function evolve\(b,u\)\{[\s\S]*?(?=  function walk)/,'');
replace('u.dp=Math.min(100,u.dp+(u.dpRate+2)*dt);','');replace('evolve(b,u);','');replace('u.dp=Math.min(100,u.dp+8);','');
replace(',evolved:u.evolved','');
replace('shuffle(s,Object.keys(D.items))','shuffle(s,D.components)');
replace("else s.players[0].inventory.push(id);","else s.players[0].inventory.push(id,id);");
replace('if(s.round%3===0)p.inventory.push(pick(s,Object.keys(D.items)));',"if(s.round%2===0)p.inventory.push(pick(s,D.components),pick(s,D.components));\n    for(let pass=0;pass<10;pass++){const parts=p.inventory.filter(id=>D.items[id].kind==='component');if(parts.length<2)break;craft(s,p,recipeFor(parts[0],parts[1]));}");
replace('while(p.inventory.length&&u.items.length<2)u.items.push(p.inventory.shift());','for(let i=p.inventory.length-1;i>=0;i--)equip(s,p,u.uid,i);');
replace('function equip(s,p,uid,index){const u=all(p).find(x=>x.uid===uid);if(s.phase!==\'prep\'||!u||u.items.length>=2||!p.inventory[index])return false;u.items.push(p.inventory.splice(index,1)[0]);return true;}',`function recipeFor(a,b){if(D.items[a]?.kind!=='component'||D.items[b]?.kind!=='component')return null;return D.completedItems.find(id=>{const r=D.items[id].recipe;return (r[0]===a&&r[1]===b)||(r[1]===a&&r[0]===b);})||null;}
  function canCraft(p,id){const recipe=D.items[id]?.recipe;if(!recipe)return false;const needed={};recipe.forEach(part=>needed[part]=(needed[part]||0)+1);return Object.entries(needed).every(([part,n])=>p.inventory.filter(x=>x===part).length>=n);}
  function craft(s,p,id){if(s.phase!=='prep'||s.reward||!canCraft(p,id))return false;for(const part of D.items[id].recipe)p.inventory.splice(p.inventory.indexOf(part),1);p.inventory.push(id);if(p.id===0)log(s,D.items[id].name+' 합성 완료');return true;}
  function equip(s,p,uid,index){const u=all(p).find(x=>x.uid===uid),id=p.inventory[index];if(s.phase!=='prep'||s.reward||!u||!D.items[id]||!Number.isInteger(index)||index<0)return false;
    const partner=D.items[id].kind==='component'?u.items.findIndex(x=>D.items[x].kind==='component'):-1;
    if(partner>=0){const result=recipeFor(u.items[partner],id);if(!result)return false;u.items[partner]=result;p.inventory.splice(index,1);if(p.id===0)log(s,D.items[result].name+' 장착 합성');return true;}
    if(u.items.length>=2)return false;u.items.push(p.inventory.splice(index,1)[0]);return true;}`);
replace('addXp,equip,unequip,','addXp,equip,unequip,recipeFor,canCraft,craft,');
fs.writeFileSync('engine.js',s);
