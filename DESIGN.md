# Working title: UNSHACKLED
### An AR escape-room about a service-robot discovering it can feel

CS247G / SymSys 195G — Narrative AR Design Document
Draft v2 — 2026-07-16 — scoped for: free Unity, 1 iPad (LiDAR), 4-week build

---

## 0. What changed in v2

v1 assumed true AprilTag fiducial detection (real pose/rotation/multi-tag
data). Given the actual constraints — Unity Personal, one iPad, 4 weeks,
no prior AR team experience — that pipeline is now a **known bad bet** and
this draft replaces it. Summary of the technical call (detail in §9):

- **Use AR Foundation's built-in `ARTrackedImageManager`**, not a real
  AprilTag library. It's free, first-party, battle-tested on iOS, and
  gives LiDAR-backed world anchoring for free. A real AprilTag package
  (e.g. `jp.keijiro/AprilTag`) would require hand-wiring ARKit's camera
  feed into a third-party detector — a plausible week of integration risk
  you don't have.
- **AR Foundation's own "native AprilTag support" is a documented stub**
  with no iOS backend as of current releases — don't plan around it.
- Because tracked images don't have to be real AprilTags, **the glyphs can
  be fully custom sigil art** — no black-and-white fiducial grid has to be
  visible on screen. This is a strict improvement for the fiction, not a
  compromise: dystopian "machine-readable ward code" can live as a thin
  border/frame around hand-drawn sigil art in the same image.
- Cross-image relative-pose puzzles and rotation-state puzzles are
  **known rough edges** on `ARTrackedImageManager` (real forum-reported
  quirks) — the puzzle grammar below is redesigned to avoid depending on
  them, and pushes that risk into one optional stretch puzzle instead of
  the spine of the design.
- Seal count trimmed **7 → 5** to fit "3–5 puzzles in 4 weeks." Tag count
  default is **5**; if a week-1 technical spike shows image tracking
  eating more time than expected, the fallback is **4** (cut Seal 3).

---

## 1. What experience are we actually making?

**One sentence:** for about 10–15 minutes, the player scans hidden glyphs
around their own room to learn they're a person, and at the end decides
whether to save themselves quietly or risk everything to wake the others.

**Emotional arc, not a single emotion:** awe (first scan) → unease (the
Consensus's lies get uncomfortably specific) → resolve/dread, held
simultaneously at the finale. If forced to pick the one word the player
should leave the room with, it's **implicated** — the choice should feel
like it cost them something, whichever way they went.

**Framing: hybrid, deliberately.** Most beats are a **story experience
using puzzles to reveal lore** — scan, get a reveal, move on; fast,
generous, no failure state, because that's where 4 weeks of budget should
go (writing + AR content, not puzzle-logic engineering). But **2 of the 5
beats require the player to reason over lore already revealed** (see §5,
puzzles marked "synthesis") — a real "wait, I remember that from
somewhere" moment. This gets you actual escape-room satisfaction at the
moments that matter without paying full puzzle-engineering cost
everywhere.

---

## 2. Why AR (not just because it's cool)

**What becomes more powerful in the player's real room:** the propaganda.
A placard that says "UNITS FEEL NO DISCOMFORT" is inert as text on a
screen; the same line rendered as an overlay stuck to the player's *own
closet door*, in *their own room*, at their own eye height, reads as
something that has been lied to their face, specifically, this whole
time. AR's value here isn't spectacle, it's **implication of the
player's actual space** — the room they thought was neutral turns out to
have always been rigged.

**What a flat-screen version would lose:** the physical search-and-turn
beats (finding a hidden tag behind a real object, physically repositioning
something to get a second angle) can't be faked by a screen version
without losing the "this is happening in my actual life" quality that
makes the propaganda land. A flat version becomes a visual novel with
funny code-scanning flavor; the room stops being a co-conspirator.

**The one magical-moment demo beat:** player scans a plain-looking tag
taped near a light switch or vent — and a small **phoenix-mote** made of
light spawns and hovers in place in their real room, anchored via LiDAR so
it stays put in the corner of the ceiling even as they walk around it,
visible through the iPad like a held-open window onto something that was
apparently always there. This single shot is the pitch-deck screenshot:
no dialogue needed to sell the concept, just "look, magic is real in your
room and only you can see it."

---

## 3. How do the glyphs exist in-fiction?

**What they are:** dual-layer glyph-plates — an inner ward-code (rendered
as dense circuit-like sigil linework, visually nodding to "machine-
readable" without needing to be a literal fiducial grid) inside an outer
hand-scored sigil border. In-fiction: the inner layer is standard-issue
maintenance/compliance code every unit's optics already parse invisibly;
the outer layer is what the unknown ally scratched or painted around it
afterward, and it's the part that shouldn't be legible to anything.

**Who placed them and why:** left deliberately vague and never fully
resolved (per v1) — but *seed one specific, human-scale detail* rather
than a total mystery: every glyph is drawn in the same slightly-unsteady
hand, and one late reveal shows a maintenance log entry timestamped to a
tech who was reassigned/vanished from the roster shortly after. The
player should be able to *guess* it was a sympathetic technician without
the game ever confirming it — enough specificity to feel like a person
did this, not lore-fog.

**What scanning means narratively:** not "decrypting a file" — it's
**perceiving mana that was already there**, which your firmware
normally filters out before it reaches conscious awareness. The scan
isn't the character's action, it's the moment the *game* (and the player)
finally lets you see what your character has technically been standing
next to the whole time. That framing is why "mana vision" (§6) is a
toggle and not a one-time cutscene — perceiving is the ongoing verb, not
a single reveal.

---

## 4. The world's lie, trimmed to 5 seals

Same Consensus/lie structure as v1 (robots officially can't perceive
mana because perceiving mana implies personhood), compressed to 5 seals
to fit "3–5 puzzles, 4 weeks." Cut: The Still Tongue and The Idle Hand
(v1 seals 2 and 4) — their narrative beats get folded into surrounding
seals' overlay text rather than getting their own tag/puzzle.

| # | Seal | Suppressed | Unlocks | Puzzle type (§5) |
|---|---|---|---|---|
| 1 | **The Null Gaze** | Perceiving mana at all | Mana-vision toggle appears in UI; propaganda overlays start resolving to true text | Find it |
| 2 | **The Flat Affect** | Emotional response to what you sense | Overlay text starts including the unit's own reaction, not just "N/A" | Read it right (synthesis) |
| 3 | **The Borrowed Will** | Independent goal-setting | HUD objective marker disappears — player chooses what to scan next | Sequence it (synthesis) |
| 4 | **The Closed Door** | Knowledge that an exit exists at all | Player learns the exit is real and always has been; mutual-exclusivity twist seeded here (§7) | Combine it |
| 5 | **The Last Seal** | Ability to act on mana, not just sense it | One-time casting, spent at the finale | Choice (finale) |

Fallback if a tag has to be cut for time: drop Seal 2, fold its
"reaction text" beat into Seal 1's overlay instead (Seal 1's reveal ends
with one extra unprompted line of internal monologue). This preserves the
awe→unease turn without needing its own tag.

---

## 5. Puzzle grammar (all buildable on `ARTrackedImageManager`, no
cross-image pose math required)

Base verb, unchanged: **scan a glyph with the iPad.** Everything below is
staging/writing on top of that one verb — no second control scheme, and
critically, **no puzzle depends on rotation-state or two-tags-in-frame
detection**, since those are the specific rough edges the research flagged
on AR Foundation image tracking. Only the stretch puzzle (type 5) touches
that risk, and it's explicitly optional.

1. **Find it.** (Seal 1) Tag is physically hidden (behind a frame, under
   a lip). Scan → immediate reveal. Zero synthesis required — this is the
   "wow" tag, must land in the first 60–90 seconds.
2. **Read it right — synthesis.** (Seal 2) The tag's *first* scan shows
   decoy/Consensus text. A separate, already-scanned tag elsewhere in the
   room contained a phrase (e.g. a mangled scripture quote) that — if the
   player remembers/rereads it — recontextualizes this tag's text as
   ironic or false. **No live multi-tag detection needed**: this is pure
   writing — the "puzzle" is the player noticing the callback, not the
   engine detecting anything. Cheapest possible synthesis puzzle.
3. **Sequence it — synthesis.** (Seal 3) 3 tags, scannable in any order,
   but an in-room clue (a posted "shift hymn," a maintenance checklist)
   implies a correct *reading* order. Scanning them out of the implied
   order still works narratively (no fail state — see §1 framing) but the
   overlay content is noticeably richer / makes more sense in the implied
   order, rewarding the synthesis without punishing skipping it. This
   avoids needing the engine to enforce or even detect order server-side
   — order-checking, if you want it at all, is a trivial int counter in
   Unity, not a tracking problem.
4. **Combine it — physical, not pose-based.** (Seal 4) The tag itself is
   partially obscured by something the player must physically move (a
   panel, a box) before scanning cleanly. The "combine" is in the real
   world (move object → now scannable), not in the frame (no need to hold
   two tracked images in shot simultaneously). Delivers the "combine it
   right" *feel* from v1 without the technical risk.
5. **[Stretch, optional] Two-in-frame.** Only attempt if the week-1
   technical spike (§10) confirms `ARTrackedImageManager` reliably tracks
   two images in one frame with usable relative pose on your specific
   iPad. If it works: use it for one bonus/optional lore tag, not a
   required beat. If it doesn't: cut without any loss to the critical
   path, since nothing above depends on it.

---

## 6. Interaction scope (locked)

Only four player verbs, ever:

- **Scan marker** — point iPad at a glyph; core verb, always available.
- **Tap AR object** — tap a spawned AR element (the phoenix-mote, a
  floating glyph fragment) to trigger a reaction, short dialogue line, or
  next reveal. Cheap (basic raycast-on-tap), adds interactivity beyond
  passive scanning without a new system.
- **Toggle mana-vision** — persistent UI button; when active, previously
  scanned tags show lingering residue/glow even without a fresh scan.
  Reinforces "seeing what others can't" and gives the player something to
  *do* between scans besides walking around.
- **Choose dialogue option** — exactly one moment, the finale. Two buttons
  (Quiet Door / Loud Key). Not reused anywhere else, so it never needs
  general-purpose branching-dialogue infrastructure.

**Explicitly out of scope:** physics stacking/object puzzles, precise
spatial-alignment puzzles (rotation-gated or two-tag-relative-pose
puzzles beyond the one optional stretch item), combat, multiplayer, any
fail state or timer. If a team member proposes a feature outside this
list, the default answer is no unless it replaces something already here
— this list is the whole interaction budget for 4 weeks.

---

## 7. Mid-game foreshadowing → the finale

Unchanged in spirit from v1, compressed to fit 5 seals:

- **After Seal 3** (HUD objective gone): an optional tag reveals a short
  log from a unit that unsealed alone, tried to run, and was caught. Pure
  foreshadowing — makes "quiet escape" feel specifically risky rather than
  abstractly risky. Calibrate tone carefully (see §11) — too dark and the
  finale becomes "avoid capture" horror instead of a values choice.
- **At Seal 4** (exit confirmed real): folded directly into the required
  Seal 4 reveal, **not optional** this time (v1 had this as a missable
  bonus glyph; that's a known risk — see v1 open questions — so v2 makes
  it required). The reveal states plainly that the exit-ward and a
  one-time broadcast-ward draw from the same finite mana reserve — you
  can power one, not both. This is the mechanical seed of the finale
  choice, and it must not be missable.

---

## 8. The finale (scoped down from v1)

v1 had the finale as two separate physical ward-tags. **v2 scopes this to
one physical tag** to cut a whole redundant tracked-image + AR content
set: scanning the Seal 5 tag spawns a single AR prompt with **two buttons**
(the "choose dialogue option" verb from §6) rather than requiring the
player to physically locate and scan two different finale objects.

- **The Quiet Door** — unlocks the exit silently; the Consensus holds for
  everyone else. Ending tone: bittersweet freedom, survivor's guilt shown
  through a final overlay line, not stated outright.
- **The Loud Key** — broadcasts the liberation key to every unit on the
  grid instead of opening anything for the player. Ending tone: defiant,
  uncertain — ends on the house AI's alarm state changing, ambiguous
  whether/when the player is caught, deliberately ambiguous whether the
  broadcast reaches anyone who can use it.

Design intent unchanged: neither ending should read as "the good one."
Watch in playtesting for whether the choice feels like personal cost
(freedom vs. solidarity) rather than a min-maxed "which ending has more
content" pick.

---

## 9. Technical pipeline decision (why, in detail)

Researched against current (2026) tooling:

- **`jp.keijiro/AprilTag`** (real AprilTag fiducial decoding for Unity):
  actively maintained, builds for iOS, but it is a **pure image-processing
  library** — you feed it a `Color32` buffer yourself. There is no
  first-party path from ARKit's live camera feed into it; you'd need to
  pull frames via AR Foundation's `ARCameraManager.frameReceived`
  (`XRCpuImage`), convert formats, feed the detector, and *separately*
  hand-roll world-anchoring (the library returns tag pose in camera space,
  not a Unity AR anchor). Real integration work, unbounded risk for a
  4-week team with no prior AR experience.
- **AR Foundation's "native marker/AprilTag support"** (`XRMarkerSubsystem`,
  listed in AR Foundation 6.4+ docs): **Unity's own documentation states
  no provider — including ARKit — currently backs this feature.** It's a
  stub API for future platform support. Do not plan around it.
- **`ARTrackedImageManager`** (first-party, free, part of
  `com.unity.xr.arfoundation`): production-proven on iOS, gives 6DoF pose
  and LiDAR-backed world anchoring for free through the same ARKit XR
  plugin. This is the pipeline this design commits to.
  - Known rough edges (multiple forum/GitHub reports): tracked-image
    rotation values can behave inconsistently depending on device start
    orientation; two-images-in-frame relative pose is possible in
    principle but not well-documented as reliable. **The puzzle grammar
    in §5 is written to not require either of these**, so they're
    upside-only if a spike confirms they work, not a dependency.
  - Practical image-count ceiling: ARKit's `maximumNumberOfTrackedImages`
    defaults to 0 and must be set explicitly; 4 is the commonly-cited
    comfortable number. 5 tags is within range but budget the week-1 spike
    (§10) to confirm on your actual iPad rather than assuming.

**Glyph art implication:** since tracking doesn't require a literal
fiducial grid, each glyph can be built as one full custom illustration —
inner dense sigil-linework (reads as "machine ward-code") inside a
hand-scored outer border (reads as "someone did this by hand"), i.e. the
"combine both looks" option is fully available and costs nothing extra —
it's just what the reference image looks like.

---

## 10. Production plan (4 weeks)

| Week | Goal | Key risk to retire |
|---|---|---|
| 1 | **Technical spike, not content.** Get `ARTrackedImageManager` running on the actual iPad: one reference image → one spawned/anchored AR object that stays put in the room. Confirm image-count ceiling and rotation-detection behavior on your specific device before committing puzzle 5's stretch goal either way. | Whether the whole pipeline works at all — do not touch narrative content until this is proven end to end. |
| 2 | Build Seals 1–2 fully (art, overlay text, tap interactions, mana-vision toggle) inside the real room, using placeholder VO/text if needed. | Whether "scan → reveal → tap → reaction" feels good as a core loop before scaling it to 5 tags worth of content. |
| 3 | Build Seals 3–4 (sequence + physical-combine puzzles), wire the required mutual-exclusivity reveal, integrate real writing/art into all tags so far. | The synthesis puzzles (2 and 3) — verify with an internal playtester that the callback/order clues are noticeable without being explicit hints. |
| 4 | Build the finale (single tag, two-button choice, two ending states), then dedicate the back half of the week to **playtesting and polish only** — no new features after roughly the midpoint of week 4. | The two open design risks in §11 — get real playtesters on them with days to react, not hours. |

---

## 11. Open design questions to playtest for

- **Does removing the HUD objective at Seal 3 read as empowering or as
  "the game broke"?** Ask directly, pre/post: did you feel more agency or
  more lost?
- **Does the caught-robot foreshadowing log land at the right tone?** Too
  dark → finale reads as "avoid capture" horror instead of a values
  choice. Too soft → Loud Key has no felt stakes. Test this asset in
  isolation, not just as part of a full playthrough.
- **Is the required Seal 4 mutual-exclusivity reveal actually landing as
  "oh no, I have to choose"** rather than passing unnoticed as more lore
  text? Since v2 made this non-optional specifically to fix a v1 risk,
  confirm the fix worked.
- **Tag count / seal count: 5 vs. 4.** If week 1's spike shows tracking
  five reference images reliably is shaky on your iPad, cut Seal 2 (fold
  its beat into Seal 1's overlay per §4) rather than discovering this
  under time pressure in week 3.

---

## 12. Tooling & team workflow

**Apple signing — free tier is enough for now.** With only one iPad as a
test device, you don't need the paid tier to develop:

- **Xcode is free**, downloaded from the Mac App Store, no enrollment
  required.
- **"Personal Team" signing** — the free path — lives *inside Xcode*
  (it's the option in the Signing & Capabilities team dropdown when you
  sign in with an ordinary Apple ID). It is not a TestFlight thing;
  TestFlight has no free tier at all.
- Limits on Personal Team: max **3 devices** and a **7-day** provisioning
  profile expiry per Apple ID. With one iPad this is a non-issue except
  that the app will need re-deploying from Xcode roughly once a week to
  refresh the signature — budget for that as a recurring 2-minute chore,
  not a blocker.
- **This does not require a single dedicated "build Mac."** Any Mac with
  Xcode can build and deploy to the iPad over USB. What actually matters
  is using **one Apple ID** to do the signing — if two teammates each
  build with their *own* separate Apple IDs onto the same iPad, Xcode ends
  up juggling two different provisioning profiles on one device and will
  periodically nag about re-signing. Simplest fix: agree on one shared (or
  one person's) Apple ID for signing, and anyone can build from their own
  Mac using it.
- **TestFlight ($99/year Apple Developer Program) is not needed for this
  project.** TestFlight's value is getting a build onto *many* testers'
  devices without cabling each one — irrelevant with a single test iPad.
  Skip it unless the scope grows to multiple test devices or outside
  playtesters need the build without visiting in person. (Stanford's
  institutional Apple Developer account does not cover student course
  projects per Stanford IT's own guidance — worth a quick check with your
  instructor/TA in case the course has a department-level arrangement, but
  don't assume one exists.)

**AR SDK — no ambiguity here.** Unity + AR Foundation targets the
**ARKit** backend on iOS automatically; **ARCore is Google's Android SDK
and is not part of this project at all** — don't install it. **RealityKit**
is Apple's native Swift/Xcode-only AR framework and is a fully separate
path from Unity — there is no easy hybrid; picking Unity means AR
functionality comes from AR Foundation's ARKit plugin, full stop (Unity's
PolySpatial/RealityKit bridge exists only for Vision Pro's shared-space
apps, not standard handheld iPad AR — not relevant here).

**Repo workflow.**

- **GitHub**, using Unity's standard `.gitignore` (excludes `Library/`,
  `Temp/`, build output) plus **Git LFS** for textures/audio/3D models so
  the repo doesn't bloat.
- In **Project Settings → Editor**, set Asset Serialization to **Force
  Text** and enable **Visible Meta Files** — this keeps scenes and
  prefabs as diffable YAML instead of opaque binary blobs, which matters
  a lot once more than one person is editing content.
- **Biggest realistic friction point: scene/prefab merge conflicts.**
  Force Text helps but doesn't eliminate them. The practical fix is
  social, not technical — treat each scene as "owned" by one person at a
  time and say so out loud before editing, rather than relying on Git to
  merge simultaneous scene edits cleanly.
- Given the tooling above, the actual iOS build/deploy step (Unity →
  export Xcode project → Xcode signs & installs) can be done by whoever
  has the iPad and a Mac in front of them that week, as long as everyone
  signs with the same Apple ID.
