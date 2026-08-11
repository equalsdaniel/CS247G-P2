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
      const body = new THREE.Mesh(new THREE.BoxGeometry(.55, 1.05, .42), new THREE.MeshStandardMaterial({ color: actor.color || "#777", roughness: 1, flatShading: true }));
      body.position.y = .88; body.castShadow = true; group.add(body);
      const head = new THREE.Mesh(new THREE.DodecahedronGeometry(.29, 0), new THREE.MeshStandardMaterial({ color: 0xd5a982, roughness: 1, flatShading: true })); head.position.y = 1.67; head.castShadow = true; group.add(head);
      const hair = new THREE.Mesh(new THREE.BoxGeometry(.48,.16,.42),new THREE.MeshStandardMaterial({color:0x49372f,roughness:1,flatShading:true}));hair.position.set(0,1.88,0);group.add(hair);
      const label = labelSprite(actor.name); label.position.y = 2.28; group.add(label); scene.add(group); interactive.push(group);
    });
    clues.filter(c => c.floor === floor && !found.includes(c.id)).forEach(clue => {
      const p = toWorld(clue); const group = new THREE.Group(); group.position.set(p.x, .55, p.z); group.userData = { kind: "clue", id: clue.id, label: lang === "zh" ? clue.name : (clueNamesEn[clue.id] || clue.name) };
      const orb = new THREE.Mesh(new THREE.OctahedronGeometry(.18), new THREE.MeshStandardMaterial({ color: 0xd7a34c, emissive: 0x6b4614, emissiveIntensity: 1.4 })); group.add(orb);
      const glow = new THREE.PointLight(0xd99f43, 2.2, 2.5); group.add(glow); scene.add(group); interactive.push(group);
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
      interactive.forEach((o,i)=>{ if(o.userData.kind==="clue") o.rotation.y += dt*1.5; if(o.userData.kind==="stairs") o.position.y=.5+Math.sin(clock.elapsedTime*2)*.08; });
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
