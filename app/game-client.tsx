"use client";

import { useCallback, useEffect, useMemo, useRef, useState } from "react";

type Floor = 1 | 2;
type Point = { x: number; y: number };
type Actor = Point & { id: string; name: string; room: string; color: string; floor: Floor };
type Clue = Point & { id: string; name: string; detail: string; floor: Floor };
type Dialogue = { speaker: string; text: string; follow?: string };

const MAP_W = 960;
const MAP_H = 600;
const PLAYER_R = 12;

const actors: Actor[] = [
  { id: "amy", name: "Amy", room: "餐厅", x: 525, y: 162, floor: 1, color: "#78947d" },
  { id: "coco", name: "Coco", room: "客厅", x: 300, y: 370, floor: 1, color: "#9c6f78" },
  { id: "dean", name: "Dean", room: "管家室", x: 795, y: 392, floor: 1, color: "#727e91" },
  { id: "ella", name: "Ella", room: "厨房", x: 660, y: 162, floor: 1, color: "#b89564" },
  { id: "ben", name: "Ben", room: "二楼走廊", x: 455, y: 290, floor: 2, color: "#8277a1" },
];

const clues: Clue[] = [
  { id: "milk", name: "牛奶杯", detail: "杯底有少量白色沉淀，需要结合Ella的记忆判断下药时机。", x: 722, y: 175, floor: 1 },
  { id: "clock", name: "客厅座钟", detail: "座钟停顿过一次，Coco记忆中的时间从22:14跳到了22:22。", x: 210, y: 313, floor: 1 },
  { id: "log", name: "跳闸日志", detail: "22:12，Dean离开客厅处理东侧跳闸，Coco的不在场证明出现空白。", x: 840, y: 370, floor: 1 },
  { id: "paper", name: "平整的报纸", detail: "当天报纸平整地落在床边，不可能产生Ben听到的持续揉纸声。", x: 700, y: 145, floor: 2 },
  { id: "cord", name: "窗帘绳", detail: "绳索滑轮有新鲜摩擦痕迹，并残留Coco常用护手霜的气味。", x: 828, y: 114, floor: 2 },
  { id: "lock", name: "自动门锁", detail: "门被带上后会自动锁止，所谓密室不需要凶手从内部反锁。", x: 605, y: 220, floor: 2 },
];

const dialogue: Record<string, Dialogue[]> = {
  amy: [
    { speaker: "Amy", text: "我大概21:50经过主卧，然后就回客厅了。我从没碰过那杯牛奶。", follow: "奇怪：你还没有问她牛奶是否被下药。" },
    { speaker: "Amy", text: "厨房谁都能进。你应该先问负责准备牛奶的Ella。" },
  ],
  coco: [
    { speaker: "Coco", text: "22:00到22:30我一直在客厅和Dean说话，一次都没上楼。", follow: "她把整个不在场证明都压在Dean身上。" },
    { speaker: "Coco", text: "窗帘？我几周没进过Felix的房间了。" },
  ],
  dean: [
    { speaker: "Dean", text: "我和Coco一直在客厅。22:30我照常巡查，房里很安静。" },
    { speaker: "Dean", text: "监控设备很旧。重复画面不一定代表有人篡改。", follow: "他说话时看了一眼洗衣房方向。" },
  ],
  ben: [
    { speaker: "Ben", text: "22:18左右我经过主卧。里面没开灯，但一直有翻报纸的声音。" },
    { speaker: "Ben", text: "我没有敲门。我欠Felix钱……我只是怕见他。", follow: "黑暗中无法读报，声音可能是伪造的。" },
  ],
  ella: [
    { speaker: "Ella", text: "我21:45开始热牛奶，22:00送进主卧。中途去储藏室拿过托盘。" },
    { speaker: "Ella", text: "回来时我看见一个灰绿色袖口从餐厅侧门闪过去。", follow: "Amy今晚穿着灰绿色外套。" },
  ],
};

function dist(a: Point, b: Point) {
  return Math.hypot(a.x - b.x, a.y - b.y);
}

function drawRoom(ctx: CanvasRenderingContext2D, x: number, y: number, w: number, h: number, label: string) {
  ctx.fillStyle = "#211d1b";
  ctx.fillRect(x, y, w, h);
  ctx.strokeStyle = "#7c6a55";
  ctx.lineWidth = 5;
  ctx.strokeRect(x, y, w, h);
  ctx.fillStyle = "rgba(226,211,179,.55)";
  ctx.font = "600 14px sans-serif";
  ctx.fillText(label, x + 14, y + 24);
}

function MansionCanvas({
  floor,
  player,
  setPlayer,
  found,
  onInteract,
}: {
  floor: Floor;
  player: Point;
  setPlayer: (p: Point) => void;
  found: string[];
  onInteract: (kind: "actor" | "clue" | "stairs", id: string) => void;
}) {
  const canvasRef = useRef<HTMLCanvasElement>(null);
  const keys = useRef(new Set<string>());
  const playerRef = useRef(player);
  const onInteractRef = useRef(onInteract);
  const nearestRef = useRef<{ kind: "actor" | "clue" | "stairs"; id: string; name: string; p: Point } | null>(null);
  playerRef.current = player;
  onInteractRef.current = onInteract;

  const nearest = useMemo(() => {
    const available = [
      ...actors.filter((a) => a.floor === floor).map((a) => ({ kind: "actor" as const, id: a.id, name: a.name, p: a })),
      ...clues.filter((c) => c.floor === floor && !found.includes(c.id)).map((c) => ({ kind: "clue" as const, id: c.id, name: c.name, p: c })),
      { kind: "stairs" as const, id: "stairs", name: floor === 1 ? "上二楼" : "下一楼", p: { x: 470, y: 505 } },
    ].sort((a, b) => dist(player, a.p) - dist(player, b.p));
    return nearestOrNull(available, player);
  }, [floor, player, found]);
  nearestRef.current = nearest;

  const interact = useCallback(() => {
    if (nearest) onInteract(nearest.kind, nearest.id);
  }, [nearest, onInteract]);

  const handleCanvasClick = useCallback((e: React.PointerEvent<HTMLCanvasElement>) => {
    const rect = e.currentTarget.getBoundingClientRect();
    const p = {
      x: ((e.clientX - rect.left) / rect.width) * MAP_W,
      y: ((e.clientY - rect.top) / rect.height) * MAP_H,
    };
    const targets = [
      ...actors.filter((a) => a.floor === floor).map((a) => ({ kind: "actor" as const, id: a.id, p: a })),
      ...clues.filter((c) => c.floor === floor && !found.includes(c.id)).map((c) => ({ kind: "clue" as const, id: c.id, p: c })),
      { kind: "stairs" as const, id: "stairs", p: { x: 470, y: 485 } },
    ].sort((a, b) => dist(p, a.p) - dist(p, b.p));
    if (targets[0] && dist(p, targets[0].p) <= (targets[0].kind === "stairs" ? 72 : 38)) {
      onInteract(targets[0].kind, targets[0].id);
    }
  }, [floor, found, onInteract]);

  useEffect(() => {
    const down = (e: KeyboardEvent) => {
      if (["ArrowUp", "ArrowDown", "ArrowLeft", "ArrowRight", "w", "a", "s", "d", "W", "A", "S", "D"].includes(e.key)) {
        e.preventDefault();
        keys.current.add(e.key.toLowerCase());
      }
      const isInteractKey = e.code === "KeyE" || e.key.toLowerCase() === "e" || e.code === "Enter" || e.code === "Space";
      const target = nearestRef.current;
      if (isInteractKey && target) {
        e.preventDefault();
        e.stopPropagation();
        onInteractRef.current(target.kind, target.id);
      }
    };
    const up = (e: KeyboardEvent) => keys.current.delete(e.key.toLowerCase());
    document.addEventListener("keydown", down, true);
    document.addEventListener("keyup", up, true);
    return () => {
      document.removeEventListener("keydown", down, true);
      document.removeEventListener("keyup", up, true);
    };
  }, []);

  useEffect(() => {
    let raf = 0;
    let prev = performance.now();
    const tick = (now: number) => {
      const dt = Math.min(32, now - prev) / 16.67;
      prev = now;
      let dx = 0;
      let dy = 0;
      if (keys.current.has("w") || keys.current.has("arrowup")) dy -= 1;
      if (keys.current.has("s") || keys.current.has("arrowdown")) dy += 1;
      if (keys.current.has("a") || keys.current.has("arrowleft")) dx -= 1;
      if (keys.current.has("d") || keys.current.has("arrowright")) dx += 1;
      if (dx || dy) {
        const len = Math.hypot(dx, dy);
        const next = {
          x: Math.max(38, Math.min(MAP_W - 38, playerRef.current.x + (dx / len) * 4.2 * dt)),
          y: Math.max(66, Math.min(MAP_H - 30, playerRef.current.y + (dy / len) * 4.2 * dt)),
        };
        playerRef.current = next;
        setPlayer(next);
      }
      raf = requestAnimationFrame(tick);
    };
    raf = requestAnimationFrame(tick);
    return () => cancelAnimationFrame(raf);
  }, [setPlayer]);

  useEffect(() => {
    const canvas = canvasRef.current;
    if (!canvas) return;
    const ctx = canvas.getContext("2d");
    if (!ctx) return;
    const dpr = window.devicePixelRatio || 1;
    canvas.width = MAP_W * dpr;
    canvas.height = MAP_H * dpr;
    ctx.scale(dpr, dpr);
    ctx.fillStyle = "#141211";
    ctx.fillRect(0, 0, MAP_W, MAP_H);

    if (floor === 1) {
      drawRoom(ctx, 45, 80, 270, 190, "客厅 LIVING ROOM");
      drawRoom(ctx, 335, 80, 220, 190, "餐厅 DINING ROOM");
      drawRoom(ctx, 575, 80, 335, 190, "厨房 KITCHEN");
      drawRoom(ctx, 45, 290, 270, 230, "门厅 FOYER");
      drawRoom(ctx, 335, 290, 220, 230, "主楼梯 GRAND STAIR");
      drawRoom(ctx, 575, 290, 335, 230, "管家室 / 洗衣房");
    } else {
      drawRoom(ctx, 45, 80, 250, 190, "客房 GUEST ROOM");
      drawRoom(ctx, 315, 80, 240, 400, "二楼走廊 UPPER HALL");
      drawRoom(ctx, 575, 80, 335, 260, "主卧 MASTER BEDROOM");
      drawRoom(ctx, 575, 360, 160, 160, "主卫");
      drawRoom(ctx, 750, 360, 160, 160, "杂物间");
    }

    ctx.fillStyle = "rgba(255,255,255,.025)";
    for (let i = 0; i < 40; i++) ctx.fillRect((i * 83) % MAP_W, (i * 47) % MAP_H, 1, 18);

    actors.filter((a) => a.floor === floor).forEach((a) => {
      ctx.beginPath();
      ctx.arc(a.x, a.y, 17, 0, Math.PI * 2);
      ctx.fillStyle = a.color;
      ctx.fill();
      ctx.strokeStyle = "#e6d3af";
      ctx.lineWidth = 2;
      ctx.stroke();
      ctx.fillStyle = "#f2e7d1";
      ctx.font = "600 13px sans-serif";
      ctx.textAlign = "center";
      ctx.fillText(a.name, a.x, a.y - 26);
    });

    clues.filter((c) => c.floor === floor && !found.includes(c.id)).forEach((c) => {
      const pulse = 6 + Math.sin(Date.now() / 300) * 2;
      ctx.beginPath();
      ctx.arc(c.x, c.y, pulse, 0, Math.PI * 2);
      ctx.fillStyle = "#d2a24e";
      ctx.fill();
      ctx.strokeStyle = "rgba(242,204,126,.5)";
      ctx.lineWidth = 5;
      ctx.stroke();
      ctx.fillStyle = "#e0c58f";
      ctx.font = "600 11px sans-serif";
      ctx.textAlign = "left";
      ctx.fillText(`物证 · ${c.name}`, c.x + 13, c.y + 4);
    });

    ctx.fillStyle = "rgba(197,155,82,.18)";
    ctx.fillRect(420, 452, 100, 68);
    ctx.strokeStyle = "#c59b52";
    ctx.lineWidth = 2;
    ctx.strokeRect(420, 452, 100, 68);
    ctx.fillStyle = "#e9dfce";
    ctx.font = "bold 24px sans-serif";
    ctx.textAlign = "center";
    ctx.fillText(floor === 1 ? "⇧" : "⇩", 470, 480);
    ctx.font = "600 12px sans-serif";
    ctx.fillText(floor === 1 ? "前往二楼" : "返回一楼", 470, 505);

    ctx.beginPath();
    ctx.arc(player.x, player.y, PLAYER_R, 0, Math.PI * 2);
    ctx.fillStyle = "#efe2c9";
    ctx.fill();
    ctx.strokeStyle = "#b93632";
    ctx.lineWidth = 4;
    ctx.stroke();

    const vignette = ctx.createRadialGradient(player.x, player.y, 80, player.x, player.y, 390);
    vignette.addColorStop(0, "rgba(0,0,0,0)");
    vignette.addColorStop(1, "rgba(0,0,0,.45)");
    ctx.fillStyle = vignette;
    ctx.fillRect(0, 0, MAP_W, MAP_H);
  }, [floor, player, found]);

  return (
    <div className="map-shell">
      <canvas ref={canvasRef} onPointerDown={handleCanvasClick} className="mansion-map" aria-label={`别墅${floor}楼探索地图；可以直接点击人物、物证和楼梯`} />
      <div className="floor-label">{floor}F</div>
      <button className="floor-switch" onClick={() => onInteract("stairs", "stairs")}>{floor === 1 ? "⇧ 前往二楼" : "⇩ 返回一楼"}</button>
      <div className="map-help">移动到目标附近，或直接点击人物 / 金色物证 / 楼梯</div>
      {nearest && <button className="interaction-prompt" onClick={interact}><kbd>E</kbd> {nearest.kind === "actor" ? "与" : ""}{nearest.name}{nearest.kind === "actor" ? "对话" : nearest.kind === "clue" ? "调查" : ""}</button>}
    </div>
  );
}

function nearestOrNull<T extends { p: Point }>(items: T[], player: Point): T | null {
  if (!items.length || dist(items[0].p, player) > 150) return null;
  return items[0];
}

function EllaMemory({ onComplete, onClose }: { onComplete: () => void; onClose: () => void }) {
  const [step, setStep] = useState(0);
  const [noticed, setNoticed] = useState(false);
  const steps = [
    ["21:45 · 厨房", "点击炉灶加热牛奶。", "点燃炉灶"],
    ["21:47 · 工作台", "牛奶倒入杯中。托盘却不在原处。", "去储藏室"],
    ["21:48 · 储藏室", "你听见厨房侧门和玻璃轻碰的声音。", "从门缝观察"],
    ["21:49 · 厨房", "一个灰绿色袖口从餐厅侧门消失，杯托上多了一道水环。", "记住细节"],
    ["22:00 · 主卧", "Felix接过牛奶。他还活着，台灯亮着，正在读当天的报纸。", "结束记忆"],
  ];
  const advance = () => {
    if (step === 2) setNoticed(true);
    if (step === steps.length - 1) onComplete();
    else setStep((s) => s + 1);
  };
  return (
    <div className="memory-screen">
      <div className="memory-grain" />
      <button className="close-button" onClick={onClose}>退出记忆</button>
      <div className={`memory-scene memory-step-${step}`}>
        <div className="memory-time">{steps[step][0]}</div>
        <div className="memory-object" aria-hidden="true">
          {step === 0 && "♨"}{step === 1 && "▱"}{step === 2 && "◫"}{step === 3 && "◒"}{step === 4 && "▤"}
        </div>
        <p>{steps[step][1]}</p>
        {noticed && step >= 3 && <div className="memory-clue">记忆线索：灰绿色袖口 · 第二道水环</div>}
        <button className="primary-button" onClick={advance}>{steps[step][2]}</button>
      </div>
    </div>
  );
}

function CaseBoard({ found, talked, memoryDone, onClose, onVerdict }: { found: string[]; talked: string[]; memoryDone: boolean; onClose: () => void; onVerdict: (good: boolean) => void }) {
  const [drug, setDrug] = useState("");
  const [killer, setKiller] = useState("");
  const [trick, setTrick] = useState("");
  const enough = found.length >= 5 && talked.length >= 5 && memoryDone;
  return (
    <div className="modal-backdrop">
      <div className="case-board">
        <button className="close-button dark" onClick={onClose}>返回调查</button>
        <p className="eyebrow">CASE 07 · 案件板</p>
        <h2>重建别墅谋杀案</h2>
        <div className="progress-line"><span style={{ width: `${Math.min(100, (found.length / clues.length) * 100)}%` }} /></div>
        <p className="board-status">物证 {found.length}/{clues.length} · 证人 {talked.length}/5 · Ella记忆 {memoryDone ? "完成" : "未完成"}</p>
        <div className="evidence-grid">
          {clues.map((c) => <div className={found.includes(c.id) ? "evidence-card found" : "evidence-card"} key={c.id}><b>{found.includes(c.id) ? c.name : "未发现"}</b><span>{found.includes(c.id) ? c.detail : "继续探索别墅"}</span></div>)}
        </div>
        <div className="verdict-form">
          <label>谁给牛奶下药？<select value={drug} onChange={(e) => setDrug(e.target.value)}><option value="">请选择</option>{["Amy", "Coco", "Dean", "Ben", "Ella"].map(n => <option key={n}>{n}</option>)}</select></label>
          <label>谁实施勒杀？<select value={killer} onChange={(e) => setKiller(e.target.value)}><option value="">请选择</option>{["Amy", "Coco", "Dean", "Ben", "Ella"].map(n => <option key={n}>{n}</option>)}</select></label>
          <label>密室如何形成？<select value={trick} onChange={(e) => setTrick(e.target.value)}><option value="">请选择</option><option value="lock">门关闭后自动锁止</option><option value="secret">凶手从密道离开</option><option value="inside">死者从内部反锁</option></select></label>
          <button disabled={!enough || !drug || !killer || !trick} className="primary-button" onClick={() => onVerdict(drug === "Amy" && killer === "Coco" && trick === "lock")}>{enough ? "提交最终推理" : "需要询问所有人、完成记忆并找到至少5项物证"}</button>
        </div>
      </div>
    </div>
  );
}

export default function GameClient() {
  const [started, setStarted] = useState(false);
  const [floor, setFloor] = useState<Floor>(1);
  const [player, setPlayerState] = useState<Point>({ x: 170, y: 410 });
  const [found, setFound] = useState<string[]>([]);
  const [talked, setTalked] = useState<string[]>([]);
  const [dialogueOpen, setDialogueOpen] = useState<{ id: string; index: number } | null>(null);
  const [memoryOpen, setMemoryOpen] = useState(false);
  const [memoryDone, setMemoryDone] = useState(false);
  const [boardOpen, setBoardOpen] = useState(false);
  const [result, setResult] = useState<"good" | "bad" | null>(null);
  const [toast, setToast] = useState("");

  const setPlayer = useCallback((p: Point) => setPlayerState(p), []);

  useEffect(() => {
    const raw = localStorage.getItem("case07-save");
    if (!raw) return;
    try {
      const save = JSON.parse(raw);
      setFound(save.found || []);
      setTalked(save.talked || []);
      setMemoryDone(Boolean(save.memoryDone));
    } catch { /* ignore invalid local save */ }
  }, []);

  useEffect(() => {
    localStorage.setItem("case07-save", JSON.stringify({ found, talked, memoryDone }));
  }, [found, talked, memoryDone]);

  useEffect(() => {
    if (!toast) return;
    const t = setTimeout(() => setToast(""), 2600);
    return () => clearTimeout(t);
  }, [toast]);

  const onInteract = useCallback((kind: "actor" | "clue" | "stairs", id: string) => {
    if (kind === "stairs") {
      setFloor((f) => f === 1 ? 2 : 1);
      setPlayerState({ x: 470, y: 475 });
      return;
    }
    if (kind === "actor") {
      setDialogueOpen({ id, index: 0 });
      setTalked((t) => t.includes(id) ? t : [...t, id]);
      return;
    }
    const clue = clues.find((c) => c.id === id);
    if (clue) {
      setFound((f) => f.includes(id) ? f : [...f, id]);
      setToast(`获得物证：${clue.name}`);
    }
  }, []);

  const activeDialogue = dialogueOpen ? dialogue[dialogueOpen.id][dialogueOpen.index] : null;
  const actor = dialogueOpen ? actors.find((a) => a.id === dialogueOpen.id) : null;

  if (!started) {
    return (
      <main className="title-screen">
        <div className="rain" />
        <div className="title-card">
          <p className="eyebrow">MEMORY DETECTIVE FILES · CASE 07</p>
          <h1>别墅谋杀案</h1>
          <p className="title-en">MURDER AT THE OLD VILLA</p>
          <div className="case-summary">暴雨封锁了山路。Felix死在自动上锁的主卧里，五名仍留在别墅中的人各自隐瞒了一段记忆。</div>
          <button className="primary-button large" onClick={() => setStarted(true)}>进入别墅</button>
          <p className="controls">WASD / 方向键移动 · 靠近目标按 E 或 Enter 互动</p>
        </div>
      </main>
    );
  }

  return (
    <main className="game-screen">
      <header className="game-header">
        <div><p className="eyebrow">CASE 07</p><h1>别墅谋杀案</h1></div>
        <div className="header-actions">
          <span>物证 {found.length}/{clues.length}</span>
          <span>证人 {talked.length}/5</span>
          <button onClick={() => setBoardOpen(true)}>打开案件板</button>
        </div>
      </header>
      <MansionCanvas floor={floor} player={player} setPlayer={setPlayer} found={found} onInteract={onInteract} />
      <footer className="game-footer"><span>调查员 Mara</span><span>WASD移动 · E互动 · 楼梯符号切换楼层</span></footer>

      {toast && <div className="toast">{toast}</div>}

      {dialogueOpen && activeDialogue && actor && (
        <div className="dialogue-panel">
          <div className="portrait" style={{ background: actor.color }}>{actor.name.slice(0, 1)}</div>
          <div className="dialogue-copy">
            <p className="speaker">{activeDialogue.speaker} · {actor.room}</p>
            <p>{activeDialogue.text}</p>
            {activeDialogue.follow && <p className="observation">调查记录：{activeDialogue.follow}</p>}
            <div className="dialogue-actions">
              {dialogueOpen.id === "ella" && dialogueOpen.index === dialogue.ella.length - 1 && !memoryDone && <button className="memory-button" onClick={() => { setDialogueOpen(null); setMemoryOpen(true); }}>进入Ella的记忆</button>}
              {dialogueOpen.index < dialogue[dialogueOpen.id].length - 1 ? <button onClick={() => setDialogueOpen({ ...dialogueOpen, index: dialogueOpen.index + 1 })}>继续询问</button> : <button onClick={() => setDialogueOpen(null)}>结束对话</button>}
            </div>
          </div>
        </div>
      )}

      {memoryOpen && <EllaMemory onClose={() => setMemoryOpen(false)} onComplete={() => { setMemoryDone(true); setMemoryOpen(false); setToast("记忆完成：灰绿色袖口与第二道水环"); }} />}
      {boardOpen && <CaseBoard found={found} talked={talked} memoryDone={memoryDone} onClose={() => setBoardOpen(false)} onVerdict={(good) => { setBoardOpen(false); setResult(good ? "good" : "bad"); }} />}

      {result && (
        <div className="ending-screen">
          <p className="eyebrow">FINAL VERDICT</p>
          <h2>{result === "good" ? "完整真相" : "推理仍有矛盾"}</h2>
          <p>{result === "good" ? "Amy利用Ella离开的空隙给牛奶下药。Coco在Felix昏迷后上楼，用窗帘绳将他勒死，再借自动门锁制造密室。黑暗中的报纸声只是伪造死亡时间的表演。" : "现有证据无法支持你的结论。回到别墅，重新比较牛奶、报纸声和自动门锁。"}</p>
          <button className="primary-button" onClick={() => setResult(null)}>{result === "good" ? "返回别墅自由探索" : "继续调查"}</button>
        </div>
      )}
    </main>
  );
}
