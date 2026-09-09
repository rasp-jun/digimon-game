const fs=require('node:fs');
let s=fs.readFileSync('game.js','utf8');
function replace(a,b){if(!s.includes(a))throw Error('Missing UI replacement: '+a);s=s.replaceAll(a,b);}
replace("file-island-league-v2","file-island-league-v3");replace('saved?.version===2','saved?.version===3');
s=s.replace(/  function unitCard[\s\S]*?(?=  function board)/,`  function unitCard(u,battle=false){
    const d=D.units[u.id],sel=selected&&me()[selected.area]?.[selected.i]?.uid===u.uid;
    return \`<div class="unit \${sel?'selected':''} \${battle?'battling':''} \${battle&&u.flash>active.visible.combat.time?'hit':''}" style="--rarity:\${D.colors[d.cost]}"><span class="stars">\${'★'.repeat(u.star)}</span>\${sprite(d.sprite)}<span class="unit-name">\${escape(d.name)}</span><span class="unit-items">\${u.items.map(i=>\`<span title="\${D.items[i].name}">\${D.items[i].icon}</span>\`).join('')}</span>\${battle?\`<div class="bars"><div class="bar"><i style="width:\${Math.max(0,u.hp/u.max*100)}%"></i></div><div class="bar mana"><i style="width:\${u.mana}%"></i></div></div>\`:''}</div>\`;
  }
`);
const detailStart=s.indexOf('<div class="evolution-info">');
const detailEnd=s.indexOf('${u?`<div class="detail-items">',detailStart);
if(detailStart<0||detailEnd<0)throw Error('detail stage location');
s=s.slice(0,detailStart)+'<div class="stage-info"><b>${d.cost}코스트 · ${d.stage}</b><br>고정 유닛 · 같은 유닛 3개로 별 승급<br>별이 올라가도 이름과 성장 단계는 유지됩니다.</div>'+s.slice(detailEnd);
replace("${'★'.repeat(star)} · ${D.roles[d.role]}","${'★'.repeat(star)} · ${d.stage} · ${D.roles[d.role]}");
replace('진화의 순간을 지켜보세요. 전투 후 팀을 다시 정비할 수 있습니다.','조합과 장비의 힘을 확인하세요. 전투 후 팀을 다시 정비할 수 있습니다.');
replace('상대를 읽고, 진화의 순간을 준비하세요.','상대를 읽고, 문장과 캡슐로 팀을 완성하세요.');
replace('${i+1}G ${n}%','${i+1}G ${D.stages[i+1]} ${n}%');
replace('${D.roles[d.role]}</span>${sprite(d.sprite)}','${d.stage} · ${D.roles[d.role]}</span>${sprite(d.sprite)}');
replace("${u.evolved?'◇ ':''}",'');
replace('새로운 조합과 진화 경로로 다시 도전해보세요.','새로운 조합과 장비 합성으로 다시 도전해보세요.');
replace('유닛을 선택해 능력과 진화를 확인하세요.','유닛을 선택해 성장 단계와 장비를 확인하세요.');
replace('전투 보급품이 도착했습니다.','재료 보급품이 도착했습니다.');
replace('${pool[id].name}</b><p>${pool[id].description}',"${pool[id].name}${state.reward.type==='item'?' ×2':''}</b><p>${pool[id].description}");
replace("toast('유닛당 장비는 최대 2개입니다.');","toast('장비는 최대 2개입니다. 재료 + 재료는 빈 슬롯 없이도 합성됩니다.');");
const invStart=s.indexOf("    $('#itemCount').textContent=");
const invEnd=s.indexOf("    $('#benchCount')",invStart);
if(invStart<0||invEnd<0)throw Error('inventory location');
s=s.slice(0,invStart)+`    $('#itemCount').textContent=p.inventory.length;
    $('#inventory').innerHTML=p.inventory.map((id,i)=>\`<button class="inventory-item \${D.items[id].kind} \${itemSelected===i?'selected':''}" data-item="\${i}" title="\${D.items[id].name} · \${D.items[id].description}" aria-label="\${D.items[id].name}: \${D.items[id].description}" \${!prep?'disabled':''}><span>\${D.items[id].icon}</span><b>\${D.items[id].name}</b><small>\${D.items[id].kind==='component'?'재료':'완성'}</small></button>\`).join('')||'<span class="empty-note">보관 중인 장비가 없습니다.</span>';
    const chosen=D.items[p.inventory[itemSelected]];
    $('#itemDetail').innerHTML=chosen?\`<b>\${chosen.name}</b><p>\${chosen.description}</p><small>\${chosen.kind==='component'?'아군 클릭 → 장착 / 기존 재료가 있으면 자동 합성':'아군을 클릭해 장착하세요.'}</small>\`:'재료 2개 → 문장 또는 캡슐<br>합성소에서 모든 조합법을 확인하세요.';
    $('#craftBtn').disabled=!prep;
`+s.slice(invEnd);
const inject=`  function renderCraft(){
    const p=me();$('#recipeList').innerHTML=D.completedItems.map(id=>{const d=D.items[id],ready=E.canCraft(p,id)&&state.phase==='prep'&&!state.reward;
      const needed={};d.recipe.forEach(part=>needed[part]=(needed[part]||0)+1);
      return \`<article class="recipe-card \${ready?'ready':''}"><div class="recipe-title"><span>\${d.icon}</span><div><b>\${d.name}</b><small>완성 아이템 · 장착 1칸</small></div></div><p>\${d.description}</p><div class="ingredients">\${Object.entries(needed).map(([part,n])=>{const have=p.inventory.filter(x=>x===part).length;return \`<span class="\${have>=n?'enough':'missing'}">\${D.items[part].icon} \${D.items[part].name} <b>\${have}/\${n}</b></span>\`;}).join('<i>+</i>')}</div><button data-craft="\${id}" \${!ready?'disabled':''}>\${ready?'합성하기 →':'재료 부족'}</button></article>\`;
    }).join('');
  }
`;
s=s.replace('  function showReward(){',inject+'  function showReward(){');
replace("if(b.dataset.reward!==undefined)","if(b.dataset.craft!==undefined){if(E.craft(state,me(),b.dataset.craft)){itemSelected=null;toast(D.items[b.dataset.craft].name+' 합성 완료! 보관함에 추가됐습니다.');render();renderCraft();}}if(b.dataset.reward!==undefined)");
s=s.replace("  $('#rerollBtn').onclick=", "  $('#craftBtn').onclick=()=>{renderCraft();$('#crafting').showModal();};\n  $('#rerollBtn').onclick=");
fs.writeFileSync('game.js',s);
let html=fs.readFileSync('index.html','utf8');
html=html.replaceAll('당신의 팀, 다음 진화.','작은 시작, 궁극의 팀.').replaceAll('상대를 읽고, 진화의 순간을 준비하세요.','상대를 읽고, 문장과 캡슐로 팀을 완성하세요.').replaceAll('능력과 진화 정보','능력과 성장 단계');
html=html.replace('<div id="inventory"></div>', '<div id="inventory"></div><div id="itemDetail" class="item-detail"></div><button id="craftBtn" class="craft-open">장비 합성소 <span>문장 · 캡슐 →</span></button>');
html=html.replace('<div class="panel-note">유닛당 2개 · 준비 중 자유롭게 회수</div>','<div class="panel-note">유닛당 2개 · 재료 추가 장착 시 자동 합성<br>회수는 무료 · 완성품 분해 불가</div>');
html=html.replace(/<article><b>05 · DP와 진화<\/b>[\s\S]*?<\/article>/,'<article><b>05 · 비용별 고정 성장 단계</b><p>1코 유년기 · 2코 성장기 · 3코 성숙기 · 4코 완전체 · 5코 궁극체. 각 단계 6종씩 상점에서 별도로 모집합니다. 전투 중 변신이나 DP는 없습니다. 3개 합성은 같은 모습의 별 승급입니다.</p></article>');
html=html.replace(/<article><b>06 · 조그레스 링크<\/b>[\s\S]*?<\/article>/,'<article><b>06 · 문장과 캡슐 합성</b><p>공격 데이터·크롬디지조이드 조각·순수 에너지·생명 파편, 재료 4종의 모든 2개 조합으로 완성품 10종을 만듭니다. 합성소에서 보관함 재료를 소비하거나 유닛에 두 번째 재료를 장착하세요. 같은 재료 2개도 합성됩니다.</p></article>');
html=html.replace('짝수 라운드에는 장비 1개를 선택합니다.','짝수 라운드에는 재료 한 종류를 선택해 2개 받습니다.');
html=html.replace('하나를 선택하면 리그가 이어집니다.','선택한 보상을 받고 리그를 이어가세요.');
html=html.replace('  <dialog id="reward">','  <dialog id="crafting"><div class="modal-top"><span class="eyebrow">DIGITAL FORGE / 10 RECIPES</span><button data-close="crafting" aria-label="닫기">×</button></div><h2>문장과 캡슐 합성소</h2><p>재료 2개로 팀에 필요한 힘을 만드세요. 아래 보유량은 보관함 기준입니다.<br>장착 중인 재료는 유닛 정보에서 무료 회수할 수 있습니다. 합성은 무료이며 완성품은 분해할 수 없습니다.</p><div id="recipeList" class="recipe-list"></div></dialog>\n  <dialog id="reward">');
fs.writeFileSync('index.html',html);
let css=fs.readFileSync('style.css','utf8');
css=css.replace(/\.unit\.evolved\{[^}]*\}/g,'').replace(/\.unit\.linked\{[^}]*\}/g,'').replace(/\.bar\.dp i\{[^}]*\}/g,'').replace(/\.evo-mark\{[^}]*\}/g,'').replaceAll('.evolution-info','.stage-info');
css+=`\n/* Equipment inventory and recipe workspace */
#inventory{display:grid;grid-template-columns:1fr 1fr;gap:7px;max-height:245px;overflow-y:auto;scrollbar-width:thin}.inventory-item{display:flex;flex-direction:column;align-items:center;gap:5px;padding:9px 4px;border-color:#42584c;background:#1e302c;min-width:0}.inventory-item>span{font-size:23px;color:var(--mint)}.inventory-item b{font-size:9px;word-break:keep-all}.inventory-item small{font-size:8px;color:var(--muted)}.inventory-item.complete{border-color:#8e7e4c;background:#352f2040}.inventory-item.complete>span{color:var(--gold)}.inventory-item.selected{outline:2px solid var(--accent);background:#44553b}.item-detail{margin:0 14px 12px;padding:10px;background:#102027;border-radius:5px;font-size:10px;color:var(--muted);line-height:1.7}.item-detail b{color:var(--accent)}.item-detail p{margin-top:4px}.item-detail small{display:block;font-size:9px;margin-top:5px}.craft-open{display:block;width:calc(100% - 28px);margin:0 14px 14px;border-color:#728752;color:var(--accent);font-size:11px}.craft-open span{display:block;font-size:9px;color:var(--muted);margin-top:4px}#crafting{width:min(920px,95vw)}.recipe-list{display:grid;grid-template-columns:1fr 1fr;gap:12px}.recipe-card{padding:17px;border:1px solid #3b5047;border-radius:8px;background:#15252a}.recipe-card.ready{border-color:#92a865;background:#28372b}.recipe-title{display:flex;align-items:center;gap:13px}.recipe-title>span{font-size:32px;color:var(--gold);width:40px;text-align:center}.recipe-title b{font-size:15px}.recipe-title small{display:block;color:var(--muted);font-size:9px;margin-top:4px}.recipe-card>p{font-size:11px;min-height:42px;margin:12px 0}.ingredients{display:flex;gap:5px;flex-wrap:wrap;align-items:center;min-height:44px;margin-bottom:13px;font-size:9px}.ingredients>span{padding:6px;background:#0b1c21;border-radius:4px}.ingredients i{font-style:normal;color:var(--muted)}.ingredients .enough{color:var(--accent)}.ingredients .missing{color:#e1a49a}.recipe-card>button{width:100%;padding:9px;font-size:11px}.recipe-card.ready>button{background:var(--accent);color:#20321e;font-weight:bold}.odds{flex-wrap:wrap}.stage-info{color:var(--mint)}.stage-info b{color:var(--gold)}
@media(max-width:620px){.recipe-list{grid-template-columns:1fr}.recipe-card>p{min-height:0}.inventory-item b{font-size:8px}.shop-card .role-tag{font-size:7px}.odds{justify-content:flex-start;gap:8px 12px}.left-rail{align-items:start}.left-rail>.panel:first-child{grid-row:auto}.equipment{grid-column:1/-1}#inventory{grid-template-columns:repeat(4,1fr)}}
`;
fs.writeFileSync('style.css',css);
