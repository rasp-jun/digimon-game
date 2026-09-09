"""
Digimon Auto Battler Prototype
- Python 3.x / tkinter only
- Single-file prototype
- Features:
  * Shop / reroll
  * Buy Digimon
  * Bench and 5-slot board
  * Auto battle
  * DP resource and in-battle evolution
  * Attribute/family synergy
  * Basic Jogress example
  * Round / gold / player HP loop

NOTE:
This is a fan-made gameplay prototype for personal/educational use.
For public or commercial distribution using official Digimon IP, obtain appropriate rights/licenses.
"""

import tkinter as tk
from tkinter import ttk, messagebox
import random
from dataclasses import dataclass, field
from typing import List, Optional, Dict, Tuple

# -----------------------------
# Data
# -----------------------------

@dataclass
class DigimonTemplate:
    name: str
    stage: str
    cost: int
    hp: int
    atk: int
    defense: int
    speed: int
    attribute: str
    family: str
    evolution: Optional[str] = None
    evolution_dp: int = 0
    skill: str = "기본 공격"

@dataclass
class Unit:
    template_name: str
    hp: int
    max_hp: int
    atk: int
    defense: int
    speed: int
    dp: int = 0
    alive: bool = True
    kills: int = 0

    @property
    def template(self):
        return DIGIMON_DB[self.template_name]

    @property
    def name(self):
        return self.template_name

    @property
    def attribute(self):
        return self.template.attribute

    @property
    def family(self):
        return self.template.family

    @property
    def stage(self):
        return self.template.stage

    def reset_for_battle(self):
        t = self.template
        self.max_hp = t.hp
        self.hp = t.hp
        self.atk = t.atk
        self.defense = t.defense
        self.speed = t.speed
        self.dp = 0
        self.alive = True
        self.kills = 0


DIGIMON_DB: Dict[str, DigimonTemplate] = {
    "아구몬": DigimonTemplate("아구몬", "성장기", 2, 120, 25, 10, 12, "백신", "용형", "그레이몬", 35, "베이비 플레임"),
    "그레이몬": DigimonTemplate("그레이몬", "성숙기", 4, 210, 42, 18, 10, "백신", "용형", "메탈그레이몬", 55, "메가 플레임"),
    "메탈그레이몬": DigimonTemplate("메탈그레이몬", "완전체", 6, 330, 62, 28, 8, "백신", "기계형", "워그레이몬", 80, "기가 디스트로이어"),
    "워그레이몬": DigimonTemplate("워그레이몬", "궁극체", 8, 470, 88, 40, 14, "백신", "용형", None, 0, "가이아 포스"),

    "가브몬": DigimonTemplate("가브몬", "성장기", 2, 115, 24, 11, 13, "데이터", "야수형", "가루몬", 35, "쁘띠 파이어"),
    "가루몬": DigimonTemplate("가루몬", "성숙기", 4, 195, 39, 15, 17, "데이터", "야수형", "워가루몬", 55, "폭스 파이어"),
    "워가루몬": DigimonTemplate("워가루몬", "완전체", 6, 300, 65, 22, 20, "데이터", "야수형", "메탈가루몬", 80, "카이저 네일"),
    "메탈가루몬": DigimonTemplate("메탈가루몬", "궁극체", 8, 430, 82, 34, 22, "데이터", "기계형", None, 0, "코큐토스 브레스"),

    "파닥몬": DigimonTemplate("파닥몬", "성장기", 2, 100, 20, 9, 16, "백신", "천사형", "엔젤몬", 35, "에어 샷"),
    "엔젤몬": DigimonTemplate("엔젤몬", "성숙기", 4, 175, 40, 13, 18, "백신", "천사형", "홀리엔젤몬", 55, "헤븐즈 너클"),
    "홀리엔젤몬": DigimonTemplate("홀리엔젤몬", "완전체", 6, 275, 58, 24, 17, "백신", "천사형", None, 0, "헤븐즈 게이트"),

    "텐타몬": DigimonTemplate("텐타몬", "성장기", 2, 105, 21, 12, 14, "백신", "곤충형", "캅테리몬", 35, "쁘띠 썬더"),
    "캅테리몬": DigimonTemplate("캅테리몬", "성숙기", 4, 205, 35, 24, 9, "백신", "곤충형", None, 0, "메가 블래스터"),

    "피요몬": DigimonTemplate("피요몬", "성장기", 2, 105, 22, 8, 18, "백신", "조류형", "버드라몬", 35, "매지컬 파이어"),
    "버드라몬": DigimonTemplate("버드라몬", "성숙기", 4, 180, 38, 12, 21, "백신", "조류형", None, 0, "메테오 윙"),

    "팔몬": DigimonTemplate("팔몬", "성장기", 2, 125, 19, 14, 10, "데이터", "식물형", "니드몬", 35, "포이즌 아이비"),
    "니드몬": DigimonTemplate("니드몬", "성숙기", 4, 225, 34, 23, 8, "데이터", "식물형", None, 0, "니들 스프레이"),
}

SHOP_POOL = [n for n, t in DIGIMON_DB.items() if t.stage == "성장기"]

# -----------------------------
# Helpers
# -----------------------------

def make_unit(name: str) -> Unit:
    t = DIGIMON_DB[name]
    return Unit(name, t.hp, t.hp, t.atk, t.defense, t.speed)

def calc_synergies(units: List[Unit]) -> Tuple[Dict[str, int], Dict[str, int]]:
    attrs, fams = {}, {}
    for u in units:
        if u:
            attrs[u.attribute] = attrs.get(u.attribute, 0) + 1
            fams[u.family] = fams.get(u.family, 0) + 1
    return attrs, fams

def apply_synergies(units: List[Unit]) -> List[str]:
    logs = []
    attrs, fams = calc_synergies(units)

    # Attribute synergy
    for u in units:
        if attrs.get(u.attribute, 0) >= 3:
            u.atk += 8
        if fams.get(u.family, 0) >= 2:
            u.max_hp += 35
            u.hp += 35

    for k, v in attrs.items():
        if v >= 3:
            logs.append(f"[시너지] {k} 3체 이상: 해당 속성 공격력 +8")
    for k, v in fams.items():
        if v >= 2:
            logs.append(f"[시너지] {k} 2체 이상: 해당 계열 최대 HP +35")
    return logs

# -----------------------------
# Game App
# -----------------------------

class GameApp(tk.Tk):
    def __init__(self):
        super().__init__()
        self.title("DIGIMON AUTO BATTLER - Prototype")
        self.geometry("1180x760")
        self.minsize(1050, 700)

        self.gold = 12
        self.player_hp = 100
        self.round_no = 1
        self.level = 3
        self.max_board = 5

        self.bench: List[Unit] = []
        self.board: List[Optional[Unit]] = [None] * self.max_board
        self.shop: List[str] = []
        self.selected_bench_index: Optional[int] = None
        self.selected_board_index: Optional[int] = None

        self.build_ui()
        self.roll_shop(free=True)
        self.refresh_all()

    # ---------- UI ----------
    def build_ui(self):
        top = ttk.Frame(self, padding=10)
        top.pack(fill="x")

        self.status_label = ttk.Label(top, text="", font=("맑은 고딕", 12, "bold"))
        self.status_label.pack(side="left")

        ttk.Button(top, text="상점 새로고침 (2G)", command=self.roll_shop).pack(side="right", padx=4)
        ttk.Button(top, text="다음 라운드", command=self.start_battle).pack(side="right", padx=4)
        ttk.Button(top, text="조그레스 시도", command=self.try_jogress).pack(side="right", padx=4)

        body = ttk.Panedwindow(self, orient="horizontal")
        body.pack(fill="both", expand=True, padx=10, pady=(0, 10))

        left = ttk.Frame(body, padding=8)
        center = ttk.Frame(body, padding=8)
        right = ttk.Frame(body, padding=8)
        body.add(left, weight=2)
        body.add(center, weight=3)
        body.add(right, weight=3)

        # Shop
        ttk.Label(left, text="상점", font=("맑은 고딕", 14, "bold")).pack(anchor="w")
        self.shop_frame = ttk.Frame(left)
        self.shop_frame.pack(fill="x", pady=8)

        # Bench
        ttk.Separator(left).pack(fill="x", pady=8)
        ttk.Label(left, text="벤치", font=("맑은 고딕", 14, "bold")).pack(anchor="w")
        self.bench_list = tk.Listbox(left, height=12, font=("맑은 고딕", 10))
        self.bench_list.pack(fill="both", expand=True, pady=6)
        self.bench_list.bind("<<ListboxSelect>>", self.on_bench_select)

        ttk.Button(left, text="선택 유닛 → 필드 배치", command=self.deploy_selected).pack(fill="x", pady=3)
        ttk.Button(left, text="선택 유닛 판매", command=self.sell_selected).pack(fill="x", pady=3)

        # Board
        ttk.Label(center, text="내 필드", font=("맑은 고딕", 14, "bold")).pack(anchor="w")
        self.board_frame = ttk.Frame(center)
        self.board_frame.pack(fill="x", pady=8)

        self.board_buttons = []
        for i in range(self.max_board):
            btn = tk.Button(
                self.board_frame,
                text=f"{i+1}번 슬롯\n[비어있음]",
                width=16,
                height=7,
                command=lambda idx=i: self.select_board(idx)
            )
            btn.grid(row=0, column=i % 3, padx=5, pady=5, sticky="nsew")
            self.board_buttons.append(btn)

        for c in range(3):
            self.board_frame.columnconfigure(c, weight=1)

        ttk.Button(center, text="선택 필드 유닛 → 벤치", command=self.return_to_bench).pack(fill="x", pady=3)

        ttk.Separator(center).pack(fill="x", pady=10)
        ttk.Label(center, text="현재 시너지", font=("맑은 고딕", 12, "bold")).pack(anchor="w")
        self.synergy_label = ttk.Label(center, text="-", justify="left")
        self.synergy_label.pack(anchor="w", pady=6)

        # Log
        ttk.Label(right, text="전투 / 시스템 로그", font=("맑은 고딕", 14, "bold")).pack(anchor="w")
        self.log = tk.Text(right, wrap="word", font=("Consolas", 10))
        self.log.pack(fill="both", expand=True, pady=8)
        self.log.configure(state="disabled")

        ttk.Button(right, text="로그 지우기", command=self.clear_log).pack(fill="x")

    # ---------- Shop ----------
    def roll_shop(self, free=False):
        if not free:
            if self.gold < 2:
                messagebox.showwarning("골드 부족", "상점 새로고침에는 2G가 필요합니다.")
                return
            self.gold -= 2
        self.shop = random.choices(SHOP_POOL, k=5)
        self.refresh_shop()
        self.refresh_status()

    def refresh_shop(self):
        for w in self.shop_frame.winfo_children():
            w.destroy()

        for i, name in enumerate(self.shop):
            t = DIGIMON_DB[name]
            text = f"{name}\n{t.cost}G\n{t.attribute}/{t.family}"
            btn = ttk.Button(self.shop_frame, text=text, command=lambda idx=i: self.buy_from_shop(idx))
            btn.grid(row=i, column=0, sticky="ew", pady=3)
        self.shop_frame.columnconfigure(0, weight=1)

    def buy_from_shop(self, idx):
        if idx >= len(self.shop):
            return
        name = self.shop[idx]
        t = DIGIMON_DB[name]

        if len(self.bench) >= 9:
            messagebox.showwarning("벤치 가득 참", "벤치 최대 9마리입니다.")
            return
        if self.gold < t.cost:
            messagebox.showwarning("골드 부족", f"{name} 구매에는 {t.cost}G가 필요합니다.")
            return

        self.gold -= t.cost
        self.bench.append(make_unit(name))
        self.shop[idx] = random.choice(SHOP_POOL)
        self.add_log(f"[구매] {name}을(를) {t.cost}G에 구매했습니다.")
        self.refresh_all()

    # ---------- Bench / Board ----------
    def on_bench_select(self, _event=None):
        sel = self.bench_list.curselection()
        self.selected_bench_index = sel[0] if sel else None

    def deploy_selected(self):
        if self.selected_bench_index is None:
            return
        empty = next((i for i, u in enumerate(self.board) if u is None), None)
        if empty is None:
            messagebox.showwarning("필드 가득 참", f"필드에는 최대 {self.max_board}마리까지 배치 가능합니다.")
            return

        unit = self.bench.pop(self.selected_bench_index)
        self.board[empty] = unit
        self.selected_bench_index = None
        self.add_log(f"[배치] {unit.name}을(를) 필드 {empty+1}번에 배치했습니다.")
        self.refresh_all()

    def select_board(self, idx):
        self.selected_board_index = idx
        self.refresh_board()

    def return_to_bench(self):
        if self.selected_board_index is None:
            return
        unit = self.board[self.selected_board_index]
        if unit is None:
            return
        if len(self.bench) >= 9:
            messagebox.showwarning("벤치 가득 참", "벤치 공간이 없습니다.")
            return
        self.bench.append(unit)
        self.board[self.selected_board_index] = None
        self.add_log(f"[복귀] {unit.name}을(를) 벤치로 이동했습니다.")
        self.selected_board_index = None
        self.refresh_all()

    def sell_selected(self):
        if self.selected_bench_index is None:
            return
        unit = self.bench.pop(self.selected_bench_index)
        refund = max(1, unit.template.cost // 2)
        self.gold += refund
        self.add_log(f"[판매] {unit.name} 판매: +{refund}G")
        self.selected_bench_index = None
        self.refresh_all()

    # ---------- Jogress ----------
    def try_jogress(self):
        names = [u.name for u in self.board if u]

        # Prototype rule:
        # WarGreymon + MetalGarurumon => Omegamon-like "오메가몬"
        # We define it dynamically to keep the prototype compact.
        if "워그레이몬" in names and "메탈가루몬" in names:
            if "오메가몬" not in DIGIMON_DB:
                DIGIMON_DB["오메가몬"] = DigimonTemplate(
                    "오메가몬", "초궁극체", 10, 760, 128, 58, 20,
                    "백신", "성기사형", None, 0, "그레이 소드 / 가루루 캐논"
                )

            removed = 0
            new_board = []
            for u in self.board:
                if u and u.name in ("워그레이몬", "메탈가루몬") and removed < 2:
                    removed += 1
                    continue
                new_board.append(u)

            while len(new_board) < self.max_board:
                new_board.append(None)

            insert_idx = next((i for i, u in enumerate(new_board) if u is None), 0)
            new_board[insert_idx] = make_unit("오메가몬")
            self.board = new_board[:self.max_board]
            self.add_log("[조그레스] 워그레이몬 + 메탈가루몬 → 오메가몬!")
            self.refresh_all()
        else:
            messagebox.showinfo(
                "조그레스 조건",
                "현재 프로토타입 조그레스 조건:\n워그레이몬 + 메탈가루몬을 동시에 필드에 배치"
            )

    # ---------- Battle ----------
    def start_battle(self):
        player_units = [u for u in self.board if u]
        if not player_units:
            messagebox.showwarning("배치 필요", "전투할 디지몬을 필드에 배치하세요.")
            return

        self.add_log("")
        self.add_log(f"========== ROUND {self.round_no} ==========")

        # Reset and apply synergy
        for u in player_units:
            u.reset_for_battle()
        for line in apply_synergies(player_units):
            self.add_log(line)

        enemy_units = self.generate_enemy_team()
        for u in enemy_units:
            u.reset_for_battle()

        self.add_log("[적 팀] " + ", ".join(u.name for u in enemy_units))

        result, damage = self.simulate_battle(player_units, enemy_units)

        if result == "WIN":
            reward = 5 + min(3, self.round_no // 3)
            self.gold += reward
            self.add_log(f"[승리] +{reward}G")
        else:
            self.player_hp -= damage
            self.add_log(f"[패배] 플레이어 HP -{damage}")

        self.round_no += 1
        self.gold += 3  # base income
        self.add_log("[라운드 수입] +3G")

        if self.player_hp <= 0:
            messagebox.showinfo("게임 오버", f"{self.round_no-1} 라운드까지 생존했습니다.")
            self.reset_game()
        else:
            self.roll_shop(free=True)
            self.refresh_all()

    def generate_enemy_team(self) -> List[Unit]:
        count = min(self.max_board, 2 + self.round_no // 2)
        team = []
        for _ in range(count):
            base = random.choice(SHOP_POOL)
            unit = make_unit(base)

            # Round scaling: chance of starting evolved
            if self.round_no >= 3 and random.random() < min(0.65, self.round_no * 0.07):
                evo = unit.template.evolution
                if evo:
                    unit = make_unit(evo)
                    if self.round_no >= 7 and unit.template.evolution and random.random() < 0.35:
                        unit = make_unit(unit.template.evolution)
            team.append(unit)
        return team

    def simulate_battle(self, allies: List[Unit], enemies: List[Unit]) -> Tuple[str, int]:
        all_units = allies + enemies
        tick = 0

        while any(u.alive for u in allies) and any(u.alive for u in enemies) and tick < 120:
            tick += 1

            attackers = [u for u in all_units if u.alive]
            attackers.sort(key=lambda x: x.speed + random.random() * 5, reverse=True)

            for attacker in attackers:
                if not attacker.alive:
                    continue

                enemy_side = enemies if attacker in allies else allies
                targets = [u for u in enemy_side if u.alive]
                if not targets:
                    break

                # In-battle evolution check
                self.check_evolution(attacker)

                target = random.choice(targets)
                crit = random.random() < 0.10
                base_damage = max(1, attacker.atk - target.defense // 2)
                damage = int(base_damage * (1.5 if crit else 1.0))

                target.hp -= damage
                attacker.dp += 8
                target.dp += 4

                crit_text = " CRIT!" if crit else ""
                self.add_log(
                    f"{attacker.name} → {target.name} : {damage} 피해{crit_text} "
                    f"(HP {max(0,target.hp)}/{target.max_hp})"
                )

                if target.hp <= 0:
                    target.alive = False
                    attacker.kills += 1
                    attacker.dp += 12
                    self.add_log(f"  └ {target.name} 전투불능!")

                # evolve after gaining DP too
                self.check_evolution(attacker)
                if target.alive:
                    self.check_evolution(target)

        alive_allies = [u for u in allies if u.alive]
        alive_enemies = [u for u in enemies if u.alive]

        if alive_allies and not alive_enemies:
            return "WIN", 0

        damage = 5 + len(alive_enemies) * 2
        return "LOSE", damage

    def check_evolution(self, unit: Unit):
        evo_name = unit.template.evolution
        required = unit.template.evolution_dp

        if not evo_name or required <= 0 or unit.dp < required or not unit.alive:
            return

        old_name = unit.name
        old_hp_ratio = max(0.15, unit.hp / max(1, unit.max_hp))
        old_dp = unit.dp

        new_t = DIGIMON_DB[evo_name]
        unit.template_name = evo_name
        unit.max_hp = new_t.hp
        unit.hp = max(1, int(new_t.hp * old_hp_ratio))
        unit.atk = new_t.atk
        unit.defense = new_t.defense
        unit.speed = new_t.speed
        unit.dp = max(0, old_dp - required)

        self.add_log(f"★ 진화! {old_name} → {evo_name} ★")

    # ---------- Refresh ----------
    def refresh_all(self):
        self.refresh_status()
        self.refresh_shop()
        self.refresh_bench()
        self.refresh_board()
        self.refresh_synergy()

    def refresh_status(self):
        self.status_label.config(
            text=f"ROUND {self.round_no}   |   HP {self.player_hp}   |   GOLD {self.gold}G   |   필드 {sum(u is not None for u in self.board)}/{self.max_board}"
        )

    def refresh_bench(self):
        self.bench_list.delete(0, tk.END)
        for u in self.bench:
            t = u.template
            self.bench_list.insert(
                tk.END,
                f"{u.name} [{t.stage}]  HP:{t.hp} ATK:{t.atk} DEF:{t.defense}  {t.attribute}/{t.family}"
            )

    def refresh_board(self):
        for i, btn in enumerate(self.board_buttons):
            u = self.board[i]
            prefix = "▶ " if self.selected_board_index == i else ""
            if u:
                t = u.template
                btn.config(
                    text=f"{prefix}{i+1}번 슬롯\n{u.name}\n[{t.stage}]\nHP {t.hp} / ATK {t.atk}\n{t.attribute} · {t.family}"
                )
            else:
                btn.config(text=f"{prefix}{i+1}번 슬롯\n[비어있음]")

    def refresh_synergy(self):
        units = [u for u in self.board if u]
        if not units:
            self.synergy_label.config(text="-")
            return

        attrs, fams = calc_synergies(units)
        lines = []
        for k, v in attrs.items():
            state = "활성" if v >= 3 else "대기"
            lines.append(f"속성 {k}: {v}체 ({state}, 3체 시 ATK +8)")
        for k, v in fams.items():
            state = "활성" if v >= 2 else "대기"
            lines.append(f"계열 {k}: {v}체 ({state}, 2체 시 HP +35)")
        self.synergy_label.config(text="\n".join(lines))

    def add_log(self, text):
        self.log.configure(state="normal")
        self.log.insert(tk.END, text + "\n")
        self.log.see(tk.END)
        self.log.configure(state="disabled")
        self.update_idletasks()

    def clear_log(self):
        self.log.configure(state="normal")
        self.log.delete("1.0", tk.END)
        self.log.configure(state="disabled")

    def reset_game(self):
        self.gold = 12
        self.player_hp = 100
        self.round_no = 1
        self.bench = []
        self.board = [None] * self.max_board
        self.selected_bench_index = None
        self.selected_board_index = None
        self.roll_shop(free=True)
        self.clear_log()
        self.add_log("새 게임을 시작했습니다.")
        self.refresh_all()


if __name__ == "__main__":
    app = GameApp()
    app.mainloop()
