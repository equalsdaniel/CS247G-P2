"use client";

import { useEffect, useRef, useState } from "react";
import * as THREE from "three";

type Floor = 1 | 2;
type Lang = "zh" | "en";
type Point = { x: number; y: number };
type Target = Point & { id: string; name: string; floor: Floor; color?: string };

const MAP_W = 960;
const MAP_H = 600;
const SCALE = 55;
const clueNamesEn: Record<string, string> = {
  milk: "Milk Cup", clock: "Living-Room Clock", log: "Breaker Log",
  paper: "Flat Newspaper", cord: "Curtain Cord", lock: "Automatic Lock",
};

const toWorld = (p: Point) => new THREE.Vector3((p.x - MAP_W / 2) / SCALE, 0, (p.y - MAP_H / 2) / SCALE);
const toMap = (v: THREE.Vector3): Point => ({ x: v.x * SCALE + MAP_W / 2, y: v.z * SCALE + MAP_H / 2 });

function box(scene: THREE.Scene, position: [number, number, number], size: [number, number, number], color: number, roughness = .85) {
  const mesh = new THREE.Mesh(new THREE.BoxGeometry(...size), new THREE.MeshStandardMaterial({ color, roughness, metalness: 0, flatShading: true }));
  mesh.position.set(...position);
  mesh.castShadow = true;
  mesh.receiveShadow = true;
  scene.add(mesh);
  return mesh;
}

function labelSprite(text: string, color = "#ead9bc") {
  const canvas = document.createElement("canvas");
  canvas.width = 512; canvas.height = 128;
  const ctx = canvas.getContext("2d")!;
  ctx.fillStyle = "rgba(12,10,10,.78)"; ctx.fillRect(0, 18, 512, 92);
  ctx.strokeStyle = "rgba(197,155,82,.8)"; ctx.strokeRect(2, 20, 508, 88);
  ctx.fillStyle = color; ctx.font = "600 42px Arial"; ctx.textAlign = "center"; ctx.textBaseline = "middle";
  ctx.fillText(text, 256, 65);
  const texture = new THREE.CanvasTexture(canvas);
  const sprite = new THREE.Sprite(new THREE.SpriteMaterial({ map: texture, transparent: true, depthTest: false }));
  sprite.scale.set(2.25, .56, 1);
  return sprite;
}

function addWall(scene: THREE.Scene, x: number, z: number, w: number, d: number, h = 3.1) {
  return box(scene, [x, h / 2, z], [w, h, d], 0x987b63);
}

function checkerTexture() {
  const canvas = document.createElement("canvas"); canvas.width = 16; canvas.height = 16;
  const ctx = canvas.getContext("2d")!; ctx.imageSmoothingEnabled = false;
  ctx.fillStyle = "#8f5e46"; ctx.fillRect(0,0,16,16); ctx.fillStyle = "#a96f4f";
  ctx.fillRect(0,0,8,8); ctx.fillRect(8,8,8,8);
  const texture = new THREE.CanvasTexture(canvas); texture.wrapS = texture.wrapT = THREE.RepeatWrapping;
  texture.repeat.set(13,8); texture.magFilter = texture.minFilter = THREE.NearestFilter; texture.colorSpace = THREE.SRGBColorSpace;
  return texture;
}

const clueWorldPositions: Record<string, [number, number, number]> = {
  milk: [4.65, 1.08, -2.4], clock: [-7.23, .05, -.18], log: [6.72, 1.02, 1.68],
  paper: [5.45, .88, -2.2], cord: [8.02, 1.15, -2.1], lock: [2.22, 1.18, -.35],
};

function clueMaterial(color: number, emissive = 0) {
  return new THREE.MeshStandardMaterial({ color, emissive, emissiveIntensity: emissive ? .35 : 0, roughness: 1, flatShading: true });
}

function makeClueModel(id: string) {
  const group = new THREE.Group();
  const add = (geometry: THREE.BufferGeometry, material: THREE.Material, position: [number,number,number], rotation?: [number,number,number]) => {
    const mesh = new THREE.Mesh(geometry, material); mesh.position.set(...position); if(rotation)mesh.rotation.set(...rotation); mesh.castShadow=true; group.add(mesh); return mesh;
  };
  if (id === "milk") {
    add(new THREE.CylinderGeometry(.28,.3,.055,12),clueMaterial(0xe8d9bb),[0,0,0]);
    add(new THREE.CylinderGeometry(.19,.16,.34,10),clueMaterial(0xf1e5ca),[0,.19,0]);
    add(new THREE.CylinderGeometry(.145,.145,.018,12),clueMaterial(0xe9dfc4,0x8b7148),[0,.365,0]);
    add(new THREE.TorusGeometry(.16,.035,6,10,Math.PI*1.55),clueMaterial(0xf1e5ca),[.18,.22,0],[Math.PI/2,0,Math.PI/2]);
  } else if (id === "clock") {
    add(new THREE.BoxGeometry(.52,1.55,.32),clueMaterial(0x80593b),[0,.78,0]);
    add(new THREE.CylinderGeometry(.22,.22,.045,12),clueMaterial(0xe8d9a9),[0,1.17,-.18],[Math.PI/2,0,0]);
    add(new THREE.BoxGeometry(.025,.16,.025),clueMaterial(0x382b26),[0,1.19,-.22],[0,0,.6]);
    add(new THREE.BoxGeometry(.025,.12,.025),clueMaterial(0x382b26),[.03,1.13,-.22],[0,0,-.7]);
    add(new THREE.CylinderGeometry(.04,.04,.42,8),clueMaterial(0xc59b52),[0,.58,-.2]);
    add(new THREE.SphereGeometry(.09,8,6),clueMaterial(0xc59b52),[0,.34,-.2]);
  } else if (id === "log") {
    add(new THREE.BoxGeometry(.78,.055,.56),clueMaterial(0x604934),[0,0,0]);
    add(new THREE.BoxGeometry(.68,.025,.47),clueMaterial(0xe8dfc8),[0,.04,0]);
    add(new THREE.BoxGeometry(.2,.05,.08),clueMaterial(0x485d63),[0,.075,-.22]);
    for(let i=0;i<4;i++) add(new THREE.BoxGeometry(.5,.012,.018),clueMaterial(0x6d6255),[0,.065,-.11+i*.11]);
  } else if (id === "paper") {
    add(new THREE.BoxGeometry(.78,.025,.56),clueMaterial(0xeee2c4),[0,0,0],[0,.18,0]);
    add(new THREE.BoxGeometry(.7,.018,.5),clueMaterial(0xdccda9),[.06,.025,.03],[0,-.08,0]);
    for(let i=0;i<5;i++) add(new THREE.BoxGeometry(.52,.012,.015),clueMaterial(0x6a6158),[.03,.04,-.17+i*.085],[0,-.08,0]);
  } else if (id === "cord") {
    add(new THREE.TorusGeometry(.3,.035,6,14),clueMaterial(0xc7a06c),[0,.28,0],[0,Math.PI/2,0]);
    add(new THREE.CylinderGeometry(.055,.055,.72,7),clueMaterial(0xc7a06c),[0,-.12,0]);
    add(new THREE.CylinderGeometry(.13,.13,.11,10),clueMaterial(0x76513e),[0,.62,0],[Math.PI/2,0,0]);
  } else if (id === "lock") {
    add(new THREE.BoxGeometry(.42,.62,.15),clueMaterial(0xb8a06d),[0,0,0]);
    add(new THREE.CylinderGeometry(.11,.11,.15,10),clueMaterial(0x6b573b),[0,.08,-.13],[Math.PI/2,0,0]);
    add(new THREE.BoxGeometry(.045,.15,.025),clueMaterial(0x2f2925),[0,-.15,-.09]);
  }
  const marker = add(new THREE.OctahedronGeometry(.065),clueMaterial(0xffd367,0xc87817),[0,id === "clock" ? 1.75 : .72,0]);
  marker.userData.marker = true; marker.userData.baseY = marker.position.y;
  const glow = new THREE.PointLight(0xffc35f,2.8,2.2); glow.position.y=id === "clock" ? 1.1 : .4; group.add(glow);
  return group;
}

function makeCharacterModel(id: string, fallbackColor: string) {
  const group = new THREE.Group();
  const mat = (color: number | string) => new THREE.MeshStandardMaterial({ color, roughness: 1, flatShading: true });
  const add = (geometry: THREE.BufferGeometry, color: number | string, position: [number,number,number], rotation?: [number,number,number]) => {
    const mesh=new THREE.Mesh(geometry,mat(color));mesh.position.set(...position);if(rotation)mesh.rotation.set(...rotation);mesh.castShadow=true;group.add(mesh);return mesh;
  };
  const eye = (x:number,y:number,z=-.285,color=0x312a28) => add(new THREE.SphereGeometry(.035,6,4),color,[x,y,z]);

  if(id === "felix") {
    add(new THREE.BoxGeometry(1.05,.34,.62),0x34383f,[0,.2,0]);
    add(new THREE.BoxGeometry(.2,.25,.025),0xe5dac5,[-.28,.24,-.32]);
    add(new THREE.DodecahedronGeometry(.3,0),0xc49a78,[.72,.24,0]);
    add(new THREE.BoxGeometry(.5,.13,.42),0xa9a29a,[.74,.47,.02],[0,0,-.08]);
    eye(.64,.28); eye(.79,.28);
    add(new THREE.BoxGeometry(.2,.04,.04),0x81766d,[.72,.14,-.285]);
    group.userData.labelHeight=1.25;
    return group;
  }

  const designs: Record<string,{body:number|string,skin:number,h:number,w:number}> = {
    amy:{body:0x667b68,skin:0xd2a07f,h:1.08,w:.52}, coco:{body:0xa86f81,skin:0xe0ad8d,h:.98,w:.62},
    dean:{body:0x364b62,skin:0xb98465,h:1.15,w:.68}, ben:{body:0x786aa0,skin:0xd7a280,h:1.16,w:.48},
    ella:{body:0xc29455,skin:0x9f6b4e,h:.9,w:.58},
  };
  const d=designs[id]||{body:fallbackColor,skin:0xc99b7b,h:1,w:.55};
  const bodyY=.42+d.h/2;
  if(id==="ella") add(new THREE.CylinderGeometry(d.w*.42,d.w*.7,d.h,6),d.body,[0,bodyY,0]);
  else add(new THREE.BoxGeometry(d.w,d.h,.44),d.body,[0,bodyY,0]);
  const headY=.42+d.h+.3;
  const head=id==="dean"?add(new THREE.BoxGeometry(.53,.48,.46),d.skin,[0,headY,0]):add(new THREE.DodecahedronGeometry(id==="coco"?.32:.29,0),d.skin,[0,headY,0]);
  head.castShadow=true; eye(-.1,headY+.035); eye(.1,headY+.035);

  if(id==="amy") {
    add(new THREE.BoxGeometry(.58,.2,.45),0x352f30,[0,headY+.22,.03]);
    add(new THREE.BoxGeometry(.12,.48,.4),0x352f30,[-.25,headY-.04,.03]);
    add(new THREE.SphereGeometry(.045,6,4),0xe9d2a1,[.29,headY-.03,-.18]);
    add(new THREE.BoxGeometry(.16,.06,.025),0x8e3f44,[0,headY-.13,-.28]);
  } else if(id==="coco") {
    add(new THREE.BoxGeometry(.68,.2,.4),0x7f493e,[0,headY+.22,.05]);
    add(new THREE.BoxGeometry(.16,.64,.37),0x7f493e,[.27,headY-.13,.08]);
    [-.1,.1].forEach(x=>add(new THREE.TorusGeometry(.085,.015,5,10),0x4d3b35,[x,headY+.035,-.29]));
    add(new THREE.BoxGeometry(.09,.02,.02),0x4d3b35,[0,headY+.035,-.29]);
    add(new THREE.BoxGeometry(.48,.12,.48),0xe2c78f,[0,bodyY+.28,-.05]);
  } else if(id==="dean") {
    add(new THREE.BoxGeometry(.55,.16,.45),0x302a28,[0,headY+.27,.02]);
    add(new THREE.BoxGeometry(.22,.055,.045),0x49352f,[-.11,headY-.1,-.25],[0,0,-.12]);
    add(new THREE.BoxGeometry(.22,.055,.045),0x49352f,[.11,headY-.1,-.25],[0,0,.12]);
    add(new THREE.BoxGeometry(.24,.36,.025),0xe8dfcf,[0,bodyY+.25,-.235]);
    add(new THREE.BoxGeometry(.1,.34,.025),0x7d3040,[0,bodyY+.18,-.25]);
    for(let i=0;i<3;i++)add(new THREE.SphereGeometry(.027,6,4),0xd8b45e,[-.18,bodyY+.32-i*.18,-.24]);
  } else if(id==="ben") {
    add(new THREE.BoxGeometry(.5,.18,.42),0x3b302c,[0,headY+.22,.03]);
    add(new THREE.TorusGeometry(.34,.04,6,12,Math.PI),0x2f3948,[0,headY+.08,0],[0,0,0]);
    add(new THREE.BoxGeometry(.1,.28,.13),0x2f3948,[-.31,headY,0]); add(new THREE.BoxGeometry(.1,.28,.13),0x2f3948,[.31,headY,0]);
    add(new THREE.BoxGeometry(.34,.12,.025),0xc9b5e5,[0,bodyY+.16,-.24]);
  } else if(id==="ella") {
    add(new THREE.SphereGeometry(.2,8,6),0x332b29,[0,headY+.26,.08]);
    add(new THREE.BoxGeometry(.43,.68,.035),0xf0dfbd,[0,bodyY-.02,-.25]);
    add(new THREE.BoxGeometry(.5,.1,.46),0xf0dfbd,[0,bodyY+.42,0]);
    add(new THREE.BoxGeometry(.28,.07,.025),0x874c47,[0,headY-.12,-.28]);
  }
  group.userData.labelHeight=headY+.65;
  return group;
}

function buildHouse(scene: THREE.Scene, floor: Floor, lang: Lang) {
  const floorMesh = box(scene, [0, -.08, 0], [17.6, .16, 10.8], 0xffffff);
  floorMesh.material = new THREE.MeshStandardMaterial({ map: checkerTexture(), roughness: 1, flatShading: true });
  const ceiling = box(scene, [0, 3.18, 0], [17.6, .12, 10.8], 0xd4b89b);
  ceiling.material = new THREE.MeshStandardMaterial({ color: 0xd4b89b, emissive: 0x31251e, side: THREE.BackSide, flatShading: true });
  addWall(scene, 0, -5.35, 17.8, .18); addWall(scene, 0, 5.35, 17.8, .18);
  addWall(scene, -8.8, 0, .18, 10.8); addWall(scene, 8.8, 0, .18, 10.8);

  if (floor === 1) {
    addWall(scene, -2.2, -3.3, .16, 4.1); addWall(scene, -2.2, 3.4, .16, 3.8);
    addWall(scene, 3.25, -3.3, .16, 4.1); addWall(scene, 3.25, 3.45, .16, 3.7);
    addWall(scene, -5.55, .55, 6.5, .16); addWall(scene, 5.95, .55, 5.5, .16);
    addWall(scene, -.05, .55, 3.8, .16);

    // Living room: fireplace, sofas, clock, drinks.
    box(scene, [-7.85, 1.05, -2.2], [.65, 2.1, 2.3], 0x6f4b3a);
    box(scene, [-7.45, .55, -2.2], [.38, .85, 1.35], 0x2c2220);
    box(scene, [-5.3, .46, -4.2], [3.1, .8, .75], 0x477a78);
    box(scene, [-3.25, .46, -2.65], [.75, .8, 2.15], 0x477a78);
    box(scene, [-5.2, .28, -2.7], [1.7, .45, 1.05], 0xc1774e);
    box(scene, [-7.25, 1.15, -.15], [.55, 2.25, .42], 0x8d633f);
    const clockFace = new THREE.Mesh(new THREE.CircleGeometry(.2, 24), new THREE.MeshStandardMaterial({ color: 0xc4aa78 }));
    clockFace.position.set(-7.24, 1.65, -.37); clockFace.rotation.x = -Math.PI / 2; scene.add(clockFace);

    // Dining room: long table and six chairs.
    box(scene, [.55, .72, -2.55], [3.7, .18, 1.35], 0xb56d45);
    [[-1,-3.65],[.1,-3.65],[1.2,-3.65],[-1,-1.45],[.1,-1.45],[1.2,-1.45]].forEach(([x,z]) => box(scene, [x,.48,z], [.55,.85,.55], 0x76513e));
    for (let i = -1; i <= 1; i++) {
      const plate = new THREE.Mesh(new THREE.CylinderGeometry(.18,.18,.025,20), new THREE.MeshStandardMaterial({color:0xc9bda5}));
      plate.position.set(i * .9, .83, -2.55); scene.add(plate);
    }

    // Kitchen: worktops, stove, pantry shelves.
    box(scene, [5.9, .55, -4.55], [4.8, 1, .65], 0x6f9382);
    box(scene, [8.05, .55, -2.25], [.65, 1, 3.8], 0x6f9382);
    box(scene, [5.65, .55, -2.4], [2.8, 1, 1.05], 0xd5ad72);
    box(scene, [4.1, 1.05, -4.5], [.8, .1, .55], 0x171719);
    for (let i = 0; i < 4; i++) box(scene, [7.95, .55 + i * .5, -1.05], [.48, .1, 1.1], 0x75614d);

    // Foyer, grand stair and monitor room.
    box(scene, [-5.6, .06, 3.1], [4.6, .05, 3.7], 0x9b4557);
    for (let i = 0; i < 7; i++) box(scene, [-.2, .13 + i * .17, 4.55 - i * .3], [2.5, .22, .38], 0x735b43);
    box(scene, [5.8, .75, 4.45], [3.6, 1.35, .65], 0x3e342e);
    for (let i = 0; i < 3; i++) {
      const screen = box(scene, [4.65 + i * 1.12, 1.18, 4.08], [.9, .62, .08], 0x111a1c);
      (screen.material as THREE.MeshStandardMaterial).emissive.setHex(0x152c30);
    }
    // Dean's inspection table keeps the breaker log in clear view.
    box(scene, [6.72, .5, 1.68], [1.25, .9, .78], 0x7c5b42);
  } else {
    addWall(scene, -2.25, -2.6, .16, 5.5); addWall(scene, 2.15, -2.6, .16, 5.5);
    addWall(scene, 5.45, .2, 6.5, .16); addWall(scene, -5.5, .2, 6.5, .16);
    box(scene, [5.55, .45, -2.35], [3.8, .75, 2.15], 0x668a87); // bed
    box(scene, [5.55, 1.1, -3.25], [3.8, 1.1, .22], 0x76513e);
    box(scene, [8.25, 1.45, -2.1], [.18, 2.9, 3.7], 0x9e5268); // curtains
    box(scene, [3.0, .9, -4.65], [1.3, 1.8, .45], 0x4b3a30); // fireplace
    box(scene, [-6.0, .45, -2.4], [3.5, .75, 2], 0x47403d); // guest bed
    for (let i = 0; i < 7; i++) box(scene, [-.1, .13 + i * .17, 4.55 - i * .3], [2.4, .22, .38], 0x735b43);
  }

  const roomNames = floor === 1
    ? (lang === "zh" ? [["客厅",-5.4,-4.75],["餐厅",.5,-4.75],["厨房",5.8,-4.75],["门厅",-5.5,4.8],["主楼梯",0,4.8],["管家室",5.6,4.8]] : [["LIVING ROOM",-5.4,-4.75],["DINING ROOM",.5,-4.75],["KITCHEN",5.8,-4.75],["FOYER",-5.5,4.8],["GRAND STAIR",0,4.8],["MONITOR ROOM",5.6,4.8]])
    : (lang === "zh" ? [["客房",-5.5,-4.8],["二楼走廊",0,1.3],["主卧",5.5,-4.8]] : [["GUEST ROOM",-5.5,-4.8],["UPPER HALL",0,1.3],["MASTER BEDROOM",5.5,-4.8]]);
  roomNames.forEach(([name,x,z]) => { const s = labelSprite(String(name), "#b7a488"); s.position.set(Number(x), 2.65, Number(z)); s.scale.multiplyScalar(.72); scene.add(s); });
}

export default function Mansion3D({ floor, lang, player, setPlayer, actors, clues, found, onInteract }: {
  floor: Floor; lang: Lang; player: Point; setPlayer: (p: Point) => void; actors: Target[]; clues: Target[]; found: string[];
  onInteract: (kind: "actor" | "clue" | "stairs", id: string) => void;
}) {
  const mountRef = useRef<HTMLDivElement>(null);
  const stateRef = useRef({ keys: new Set<string>(), yaw: floor === 1 ? Math.PI : 0, target: null as null | { kind: "actor" | "clue" | "stairs"; id: string; label: string } });
  const [prompt, setPrompt] = useState("");
  const [room, setRoom] = useState("");
  const [miniPlayer, setMiniPlayer] = useState(player);
  const [miniYaw, setMiniYaw] = useState(stateRef.current.yaw);

  useEffect(() => {
    const mount = mountRef.current;
    if (!mount) return;
    const scene = new THREE.Scene();
    scene.background = new THREE.Color(0x667677);
    scene.fog = new THREE.FogExp2(0x756b67, .018);
    const camera = new THREE.PerspectiveCamera(67, 1, .05, 70);
    const start = toWorld(player); camera.position.set(start.x, 1.65, start.z);
    const renderer = new THREE.WebGLRenderer({ antialias: false, powerPreference: "high-performance" });
    renderer.setPixelRatio(1); renderer.shadowMap.enabled = true; renderer.shadowMap.type = THREE.BasicShadowMap;
    renderer.toneMapping = THREE.ACESFilmicToneMapping; renderer.toneMappingExposure = 1.62;
    mount.appendChild(renderer.domElement);
    scene.add(new THREE.HemisphereLight(0xffe6bd, 0x4b5062, 3.25));
    scene.add(new THREE.AmbientLight(0xffe8cf, 1.45));
    const warm = new THREE.PointLight(0xffb869, 28, 18); warm.position.set(-4, 2.35, -2.4); warm.castShadow = true; scene.add(warm);
    const cold = new THREE.PointLight(0x9fc7dc, 20, 16); cold.position.set(5.5, 2.3, 2.5); scene.add(cold);
    const hall = new THREE.PointLight(0xffd39b, 18, 13); hall.position.set(0, 2.4, 3.5); scene.add(hall);
    buildHouse(scene, floor, lang);

    const interactive: THREE.Object3D[] = [];
    actors.filter(a => a.floor === floor).forEach(actor => {
      const p = toWorld(actor);
      const group = new THREE.Group(); group.position.set(p.x, 0, p.z); group.userData = { kind: "actor", id: actor.id, label: actor.name };
      const model=makeCharacterModel(actor.id,actor.color||"#777");
      if(actor.id==="felix")group.position.y=.83;
      if(actor.id==="amy")model.position.y=.43;
      group.add(model);
      const label = labelSprite(actor.name); label.position.y = model.userData.labelHeight || 2.28; group.add(label); scene.add(group); interactive.push(group);
    });
    clues.filter(c => c.floor === floor && !found.includes(c.id)).forEach(clue => {
      const group = makeClueModel(clue.id); const position=clueWorldPositions[clue.id] || [toWorld(clue).x,.55,toWorld(clue).z]; group.position.set(...position);
      group.userData = { kind: "clue", id: clue.id, label: lang === "zh" ? clue.name : (clueNamesEn[clue.id] || clue.name) };
      scene.add(group); interactive.push(group);
    });
    const stairs = new THREE.Group(); stairs.position.set(0, .5, 3.75); stairs.userData = { kind: "stairs", id: "stairs", label: floor === 1 ? (lang === "zh" ? "前往二楼" : "Go Upstairs") : (lang === "zh" ? "返回一楼" : "Go Downstairs") };
    const stairMarker = new THREE.Mesh(new THREE.ConeGeometry(.24, .6, 4), new THREE.MeshStandardMaterial({ color: 0xc79c54, emissive: 0x4d3517 })); stairMarker.rotation.z = floor === 1 ? 0 : Math.PI; stairs.add(stairMarker); scene.add(stairs); interactive.push(stairs);

    const resize = () => { const { clientWidth:w, clientHeight:h } = mount; renderer.setSize(Math.max(1,Math.floor(w*.68)),Math.max(1,Math.floor(h*.68)),false); camera.aspect=w/h; camera.updateProjectionMatrix(); };
    resize(); const observer = new ResizeObserver(resize); observer.observe(mount);
    const keyDown = (e: KeyboardEvent) => { stateRef.current.keys.add(e.key.toLowerCase()); if (["e","enter"].includes(e.key.toLowerCase()) && stateRef.current.target) { const t=stateRef.current.target; onInteract(t.kind,t.id); } };
    const keyUp = (e: KeyboardEvent) => stateRef.current.keys.delete(e.key.toLowerCase());
    const mouseMove = (e: MouseEvent) => { if (document.pointerLockElement === renderer.domElement) stateRef.current.yaw -= e.movementX * .0023; };
    const click = () => renderer.domElement.requestPointerLock?.();
    window.addEventListener("keydown",keyDown); window.addEventListener("keyup",keyUp); document.addEventListener("mousemove",mouseMove); renderer.domElement.addEventListener("click",click);
    const raycaster = new THREE.Raycaster(); const center = new THREE.Vector2(0,0); const clock = new THREE.Clock(); let frame=0; let lastPrompt=""; let lastRoom=""; let lastMapUpdate=0;
    const animate = () => {
      frame=requestAnimationFrame(animate); const dt=Math.min(clock.getDelta(),.04); const keys=stateRef.current.keys; const speed=2.65*dt;
      if(keys.has("arrowleft")) stateRef.current.yaw += 1.7*dt; if(keys.has("arrowright")) stateRef.current.yaw -= 1.7*dt;
      const forward=new THREE.Vector3(-Math.sin(stateRef.current.yaw),0,-Math.cos(stateRef.current.yaw)); const right=new THREE.Vector3(forward.z,0,-forward.x);
      const move=new THREE.Vector3(); if(keys.has("w")||keys.has("arrowup"))move.add(forward); if(keys.has("s")||keys.has("arrowdown"))move.sub(forward); if(keys.has("a"))move.sub(right); if(keys.has("d"))move.add(right);
      if(move.lengthSq()){move.normalize().multiplyScalar(speed); const next=camera.position.clone().add(move); next.x=THREE.MathUtils.clamp(next.x,-8.25,8.25); next.z=THREE.MathUtils.clamp(next.z,-4.85,4.85); camera.position.copy(next);}
      camera.rotation.set(0,stateRef.current.yaw,0,"YXZ");
      if(clock.elapsedTime-lastMapUpdate>.08){lastMapUpdate=clock.elapsedTime;const mapped=toMap(camera.position);setMiniPlayer(mapped);setMiniYaw(stateRef.current.yaw);setPlayer(mapped);}
      interactive.forEach(o=>{ if(o.userData.kind==="clue"){const marker=o.children.find(child=>child.userData.marker);if(marker){marker.rotation.y+=dt*2;marker.position.y=marker.userData.baseY+Math.sin(clock.elapsedTime*3)*.05;}} if(o.userData.kind==="stairs") o.position.y=.5+Math.sin(clock.elapsedTime*2)*.08; });
      raycaster.setFromCamera(center,camera); const hits=raycaster.intersectObjects(interactive,true); let chosen:null|THREE.Object3D=null;
      for(const hit of hits){let root:THREE.Object3D|null=hit.object;while(root&& !root.userData.kind)root=root.parent;if(root&&camera.position.distanceTo(root.position)<2.65){chosen=root;break;}}
      if(!chosen){let best=2.0;interactive.forEach(o=>{const d=camera.position.distanceTo(o.position);if(d<best){best=d;chosen=o;}});}
      const target = chosen as THREE.Object3D | null; stateRef.current.target=target?{kind:target.userData.kind,id:target.userData.id,label:target.userData.label}:null;
      const nextPrompt=target?(lang==="zh"?`按 E 互动 · ${target.userData.label}`:`Press E · ${target.userData.label}`):""; if(nextPrompt!==lastPrompt){lastPrompt=nextPrompt;setPrompt(nextPrompt);}
      const x=camera.position.x,z=camera.position.z; const nextRoom=floor===2?(x>2.2?(lang==="zh"?"主卧":"MASTER BEDROOM"):x<-2.2?(lang==="zh"?"客房":"GUEST ROOM"):(lang==="zh"?"二楼走廊":"UPPER HALL")):(z<.55?(x<-2.2?(lang==="zh"?"客厅":"LIVING ROOM"):x>3.25?(lang==="zh"?"厨房":"KITCHEN"):(lang==="zh"?"餐厅":"DINING ROOM")):(x<-2.2?(lang==="zh"?"门厅":"FOYER"):x>3.25?(lang==="zh"?"管家室":"MONITOR ROOM"):(lang==="zh"?"主楼梯":"GRAND STAIR"))); if(nextRoom!==lastRoom){lastRoom=nextRoom;setRoom(nextRoom);}
      renderer.render(scene,camera);
    }; animate();
    return()=>{cancelAnimationFrame(frame);observer.disconnect();window.removeEventListener("keydown",keyDown);window.removeEventListener("keyup",keyUp);document.removeEventListener("mousemove",mouseMove);renderer.domElement.removeEventListener("click",click);if(document.pointerLockElement===renderer.domElement)document.exitPointerLock();renderer.dispose();scene.traverse(o=>{if(o instanceof THREE.Mesh){o.geometry.dispose();const m=o.material as THREE.Material;m.dispose();}});mount.removeChild(renderer.domElement);};
  }, [floor, lang, actors, clues, found, onInteract, setPlayer]);

  return <div className="map-shell three-shell">
    <div className="three-viewport" ref={mountRef} aria-label={lang === "zh" ? `别墅${floor}楼第一人称3D探索场景` : `First-person 3D villa, floor ${floor}`} />
    <div className="three-room">{room}</div><div className="three-floor">{floor}F</div><div className="crosshair" aria-hidden="true">+</div>
    <div className={`mini-map floor-${floor}`} aria-label={lang === "zh" ? `缩略地图，玩家位于${room}` : `Mini map, player in ${room}`}>
      <div className="mini-title"><span>{lang === "zh" ? "别墅平面图" : "VILLA MAP"}</span><b>{floor}F</b></div>
      <div className="mini-plan">
        {floor === 1 ? <>
          <span className="mini-zone living">{lang === "zh" ? "客厅" : "LIVING"}</span><span className="mini-zone dining">{lang === "zh" ? "餐厅" : "DINING"}</span><span className="mini-zone kitchen">{lang === "zh" ? "厨房" : "KITCHEN"}</span>
          <span className="mini-zone foyer">{lang === "zh" ? "门厅" : "FOYER"}</span><span className="mini-zone stair">{lang === "zh" ? "楼梯" : "STAIR"}</span><span className="mini-zone monitor">{lang === "zh" ? "管家室" : "MONITOR"}</span>
        </> : <>
          <span className="mini-zone guest">{lang === "zh" ? "客房" : "GUEST"}</span><span className="mini-zone hall">{lang === "zh" ? "走廊" : "HALL"}</span><span className="mini-zone bedroom">{lang === "zh" ? "主卧" : "MASTER"}</span>
        </>}
        <i className="mini-player" style={{ left: `${Math.max(2,Math.min(98,miniPlayer.x/MAP_W*100))}%`, top: `${Math.max(3,Math.min(97,miniPlayer.y/MAP_H*100))}%`, transform: `translate(-50%,-50%) rotate(${miniYaw}rad)` }} />
      </div>
    </div>
    <div className="map-help">{lang === "zh" ? "点击画面控制视角 · WASD移动 · 鼠标/方向键转向 · E互动" : "Click scene to look · WASD move · Mouse/arrows turn · E interact"}</div>
    {prompt && <button className="interaction-prompt" onClick={()=>{const t=stateRef.current.target;if(t)onInteract(t.kind,t.id);}}><kbd>E</kbd>{prompt.replace(/^按 E 互动 · |^Press E · /,"")}</button>}
    <button className="floor-switch" onClick={()=>onInteract("stairs","stairs")}>{floor===1?(lang==="zh"?"⇧ 前往二楼":"⇧ Go Upstairs"):(lang==="zh"?"⇩ 返回一楼":"⇩ Go Downstairs")}</button>
  </div>;
}
