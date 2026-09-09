'use strict';
const test=require('node:test'),assert=require('node:assert/strict');
const E=require('../engine.js'),D=require('../data.js');
test('six units in every fixed cost/stage, including six real babies',()=>{
 for(let c=1;c<=5;c++){const units=Object.values(D.units).filter(u=>u.cost===c);assert.equal(units.length,6);units.forEach(u=>assert.equal(u.stage,D.stages[c]));}
 assert.equal(D.units.agumon.cost,2);assert.equal(D.units.greymon.cost,3);assert.equal(D.units.metalgreymon.cost,4);assert.equal(D.units.wargreymon.cost,5);
 for(const p of E.newGame(4).players)for(const u of p.board.filter(Boolean))assert.equal(D.units[u.id].cost,1);
});
test('all 10 unordered material pairs have exactly one symmetric recipe',()=>{
 const recipes=new Set();for(let i=0;i<D.components.length;i++)for(let j=i;j<D.components.length;j++){const a=D.components[i],b=D.components[j],id=E.recipeFor(a,b);assert.ok(id);assert.equal(E.recipeFor(b,a),id);recipes.add(id);}
 assert.equal(recipes.size,10);assert.equal(D.completedItems.length,10);assert.equal(E.recipeFor('courage','life'),null);
});
test('each recipe consumes exactly two components, including duplicate ingredients',()=>{
 for(const id of D.completedItems){const s=E.newGame(4),p=s.players[0];p.inventory=['courage',...D.items[id].recipe];assert.equal(E.canCraft(p,id),true);assert.equal(E.craft(s,p,id),true);assert.deepEqual(p.inventory,['courage',id]);assert.equal(E.craft(s,p,id),false);}
 const s=E.newGame(8),p=s.players[0];p.inventory=['attackData'];const before=JSON.stringify(p);assert.equal(E.craft(s,p,'courage'),false);assert.equal(JSON.stringify(p),before);
});
test('crafting and equipment stay open in combat but are blocked while choosing rewards',()=>{
 const s=E.newGame(3),p=s.players[0];p.inventory=['energy','energy'];const u=p.board.find(Boolean);s.phase='battle';assert.equal(E.craft(s,p,'light'),true);assert.equal(E.equip(s,p,u.uid,0),true);assert.deepEqual(u.items,['light']);p.inventory=['life'];s.phase='prep';s.reward={type:'augment',options:['economy']};const before=JSON.stringify(p);assert.equal(E.craft(s,p,'purity'),false);assert.equal(E.equip(s,p,u.uid,0),false);assert.equal(JSON.stringify(p),before);
});
test('second component upgrades the occupied slot even when both slots are full',()=>{
 const s=E.newGame(2),p=s.players[0],u=p.board.find(Boolean);u.items=['courage','chrome'];p.inventory=['life'];assert.equal(E.equip(s,p,u.uid,0),true);assert.deepEqual(u.items,['courage','purity']);assert.equal(p.inventory.length,0);p.inventory=['energy'];assert.equal(E.equip(s,p,u.uid,0),false);assert.deepEqual(u.items,['courage','purity']);assert.deepEqual(p.inventory,['energy']);
});
test('completed items do not merge and can be recovered and sold without loss',()=>{
 const s=E.newGame(2),p=s.players[0],u=p.board[3];p.inventory=['courage','courage'];E.equip(s,p,u.uid,0);E.equip(s,p,u.uid,0);assert.deepEqual(u.items,['courage','courage']);E.unequip(s,p,u.uid,0);E.sell(s,p,'board',3);assert.deepEqual(p.inventory,['courage','courage']);assert.deepEqual(E.audit(s),[]);
});
test('every material and finished item changes its described combat stats',()=>{
 const s=E.newGame(4),p=s.players[0],q=s.players[1],unit=p.board[3];
 const fighter=()=>E.createBattle(p,q,8).units.find(u=>u.uid===unit.uid);
 const base=fighter();for(const [id,it] of Object.entries(D.items)){unit.items=[id];const f=fighter();if(it.hp)assert.equal(f.max,base.max+it.hp);if(it.atk)assert.ok(f.atk>base.atk);if(it.armor)assert.equal(f.armor,base.armor+it.armor);if(it.speed)assert.ok(f.speed>base.speed);if(it.manaRate)assert.equal(f.manaRate,base.manaRate+it.manaRate);if(it.regen)assert.equal(f.regen,base.regen+it.regen);if(it.crit)assert.equal(f.crit,base.crit+it.crit);if(it.skillPower)assert.ok(f.skillPower>base.skillPower);if(it.lifesteal)assert.equal(f.lifesteal,base.lifesteal+it.lifesteal);if(it.healPower)assert.equal(f.healPower,1+it.healPower);if(it.startShield)assert.equal(f.guard,f.max*it.startShield);}
});
test('equipment recipes survive serialization in stage saves',()=>{
 const s=E.newGame(10),p=s.players[0];assert.deepEqual(p.inventory,[]);p.inventory=['energy','energy'];assert.equal(E.craft(s,p,'light'),true);const restored=JSON.parse(JSON.stringify(s));assert.deepEqual(restored.players[0].inventory,['light']);assert.deepEqual(E.audit(restored),[]);assert.equal(restored.version,8);
});
