# Working title: UNSHACKLED
### An AR escape-room about a service-robot discovering it can feel

CS247G / SymSys 195G — Narrative AR Design Document
Draft v3 — 2026-07-21 — scoped for: free Unity, 1 iPad Pro 13" M5 w/
keyboard case (LiDAR), extended build (see §0b — v3 timeline no longer
fits 4 weeks as-is)

---

## 0b. What changed in v3 (read this before anything else)

v3 adds three mechanics that v2 explicitly excluded to protect the
4-week timeline (see old §6: "if a team member proposes a feature
outside this list, the default answer is no unless it replaces
something already here"). This is a real scope increase, not a
free reframing — flagging the cost up front:

- **Glyph delivery changes from hidden-in-room to stationary
  iPad + physical deck.** The iPad now lives closed on its keyboard
  case, propped like a laptop with the camera facing outward, and
  the player holds physical glyph cards up to it rather than walking
  around scanning tags hidden in the room. This is **mechanically
  cheaper** than handheld scanning (a fixed camera means less motion
  blur/drift for `ARTrackedImageManager` to fight) but it **costs the
  central emotional hook of v1/v2** (§2: "the room they thought was
  neutral turns out to have always been rigged," propaganda stuck to
  *their own* closet door). Presenting a card to a terminal reads as
  ritual/interrogation, not "my own space was lying to me." Sections
  1, 2, 3, 4, and 5 below are rewritten around this; if playtesting
  shows the reframe loses too much, reverting to hidden-in-room
  scanning is a valid fallback and does not affect the touch-
  manipulation or typed-password additions below.
- **Touch manipulation added as a new locked verb** (drag/rotate/scale
  on spawned AR objects, via XR Interaction Toolkit's AR gesture
  interactors). New puzzle type added in §5 (type 6, "align it").
- **Typed override-code entry added as a new locked verb.** Since the
  iPad now sits on its keyboard case, this is real physical-keyboard
  typing (Unity's Input System reading hardware key events), not the
  on-screen `TouchScreenKeyboard`. New puzzle type added in §5
  (type 7, "unlock it").
- **Net effect on §10's production plan: the 4-week plan no longer
  holds.** Two new systems (gesture manipulation, keyboard-driven
  puzzle state) each need their own week-1-style spike before content
  can be built on top of them, on top of the existing image-tracking
  spike. §10 is revised below with an extended timeline; if the
  course deadline is hard-fixed at 4 weeks, the realistic move is to
  cut back down to one or two of the three v3 additions rather than
  attempting all three plus the original 5-seal content plan.

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

**One sentence (v3):** for about 10–15 minutes, the player sits with an
iPad propped open like a terminal, presenting physical glyph cards from a
found deck to the camera to learn they're a person, and at the end
decides whether to save themselves quietly or risk everything to wake
the others.

**Emotional arc, not a single emotion:** awe (first scan) → unease (the
Consensus's lies get uncomfortably specific) → resolve/dread, held
simultaneously at the finale. If forced to pick the one word the player
should leave the room with, it's **implicated** — the choice should feel
like it cost them something, whichever way they went. **v3 risk:** this
arc was written assuming the propaganda invades the player's *own room*
(§2); with glyphs moved to a card deck presented to a stationary
terminal, that specific invasion no longer happens automatically and the
writing/staging has to work harder to earn "implicated" some other way
(see §2's rewrite).

**Framing: hybrid, deliberately.** Most beats are a **story experience
using puzzles to reveal lore** — present a glyph, get a reveal, move on;
fast, generous, no failure state. But **synthesis beats** (see §5)
require the player to reason over lore already revealed — a real "wait,
I remember that from somewhere" moment. v3 adds two further verbs on top
of this base loop (align it via touch manipulation, unlock it via typed
codes — §5, §6) — this gets more escape-room mechanical variety than v2
at the cost of more systems to build (§0b, §10).

---

## 2. Why AR (not just because it's cool)

**v3 note:** v2's answer to this question leaned entirely on propaganda
overlays appearing stuck to *the player's own room* (closet door, light
switch), discovered by walking around and scanning hidden tags. With the
iPad now stationary on its keyboard case and glyphs presented as cards
rather than found in the room, that specific justification weakens —
rewritten below around what v3 actually delivers instead.

**What becomes more powerful with the iPad as a stationary terminal:**
the ritual of presentation. Each glyph card the player holds up is a
small, deliberate act — closer to inserting a key or presenting
credentials than to searching a room — and the AR reveal (an object
spawning and hovering in real space beyond the screen, tied to *this
room, right now*) still lands as "this is happening in my actual life,"
just via a different beat: the player chose to show this card, and the
room around the fixed iPad answers back. The Seal 4 "combine it"
puzzle (moving a physical object to expose or complete a card, §5) is
what preserves the strongest piece of the original room-invasion
feeling — the room still has to cooperate, just at the point of
preparing a card rather than at the point of scanning it.

**What a flat-screen version would lose:** the AR objects still spawn
and anchor via LiDAR in the player's real, current room — a flat version
of even the v3 loop becomes a card game with a screen, losing the "the
space around me just changed" quality that only a real anchored AR
object delivers. Touch-manipulating that object with your own hands
(the new v3 verb, §5/§6) also cannot be faked flat — dragging/rotating
something that appears to sit on your real desk is a distinctly AR
sensation.

**The one magical-moment demo beat:** player presents a plain-looking
glyph card to the stationary iPad — and a small **phoenix-mote** made of
light spawns and hovers in place in their real room, anchored via LiDAR
so it stays put in the corner of the ceiling even as they walk around it,
visible through the iPad like a held-open window onto something that was
apparently always there. This single shot is still the pitch-deck
screenshot: no dialogue needed to sell the concept, just "look, magic is
real in your room and only you can see it."

---

## 3. How do the glyphs exist in-fiction?

**What they are (v3: a deck, not wall/object tags):** dual-layer
glyph-plates — an inner ward-code (rendered as dense circuit-like sigil
linework, visually nodding to "machine-readable" without needing to be a
literal fiducial grid) inside an outer hand-scored sigil border — printed
onto individual cards, collected into a single found deck rather than
scattered as tags around the room. In-fiction: the inner layer is
standard-issue maintenance/compliance code every unit's optics already
parse invisibly; the outer layer is what the unknown ally scratched or
painted around it afterward, and it's the part that shouldn't be legible
to anything.

**Why a deck instead of hidden tags (v3 change):** the player finds the
whole deck at once, early (see Seal 1 below), rather than finding one
tag at a time around the room. This trades away v2's "the room itself
is hiding things from me" discovery beat (§2) for a different one: the
deck *itself* is contraband, and choosing which card to present next —
and to what, in the room — becomes the player's ongoing decision instead
of searching. Physical production note: cards can be printed on
sticker/decal stock (e.g. via a vinyl cutter) and mounted to stiff
cardboard, then hand-scored around the border with a bone folder or
similar tool after mounting — see also Seal 4's "combine it" puzzle
below, which is where searching the room re-enters the design.

**Who placed them and why:** left deliberately vague and never fully
resolved (per v1) — but *seed one specific, human-scale detail* rather
than a total mystery: every glyph is drawn in the same slightly-unsteady
hand, and one late reveal shows a maintenance log entry timestamped to a
tech who was reassigned/vanished from the roster shortly after. The
player should be able to *guess* it was a sympathetic technician without
the game ever confirming it — enough specificity to feel like a person
did this, not lore-fog.

**What presenting a card means narratively:** not "decrypting a file" —
it's **perceiving mana that was already there**, which your firmware
normally filters out before it reaches conscious awareness. Holding a
card up to the terminal isn't the character's action so much as it is
the moment the *game* (and the player) finally lets you see what your
character has technically been standing next to the whole time. That
framing is why "mana vision" (§6) is a toggle and not a one-time
cutscene — perceiving is the ongoing verb, not a single reveal.

---

## 4. The world's lie, trimmed to 5 seals (+2 v3 stretch seals)

Same Consensus/lie structure as v1 (robots officially can't perceive
mana because perceiving mana implies personhood). v2 compressed to 5
seals to fit "3–5 puzzles, 4 weeks"; v3 keeps those 5 as the spine and
adds 2 more to showcase the new touch-manipulation and typed-code verbs
(§5, §6) — **treat 6 and 7 as stretch, cuttable independently of each
other and of the original 5** if the extended timeline (§10) still runs
short.

| # | Seal | Suppressed | Unlocks | Puzzle type (§5) |
|---|---|---|---|---|
| 1 | **The Null Gaze** | Perceiving mana at all | Mana-vision toggle appears in UI; propaganda overlays start resolving to true text; **the glyph deck itself is found/received** | Find it |
| 2 | **The Flat Affect** | Emotional response to what you sense | Overlay text starts including the unit's own reaction, not just "N/A" | Read it right (synthesis) |
| 3 | **The Borrowed Will** | Independent goal-setting | HUD objective marker disappears — player chooses which card to present next | Sequence it (synthesis) |
| 4 | **The Closed Door** | Knowledge that an exit exists at all | Player learns the exit is real and always has been; mutual-exclusivity twist seeded here (§7) | Combine it |
| 5 | **The Last Seal** | Ability to act on mana, not just sense it | One-time casting, spent at the finale | Choice (finale) |
| 6 *(v3 stretch)* | **The Steady Hand** | Ability to shape mana, not just witness it | An AR ward-fragment can be physically aligned/assembled by the player | Align it (touch manipulation) |
| 7 *(v3 stretch)* | **The Spoken Word** | Ability to act using your own voice/words, not borrowed script | A typed override code (found split across earlier seals) unlocks a door/log | Unlock it (typed code) |

Fallback if a seal has to be cut for time: drop Seal 2 first (fold its
"reaction text" beat into Seal 1's overlay instead — Seal 1's reveal ends
with one extra unprompted line of internal monologue), preserving the
awe→unease turn without needing its own card. If timeline is still tight
after that, cut Seal 6 and/or 7 entirely before touching 1–5 — they are
additive, not load-bearing for the core arc (§1).

---

## 5. Puzzle grammar (v3: base verb still `ARTrackedImageManager`, plus
two new systems — gesture manipulation, keyboard input)

Base verb, changed in staging only: **present a glyph card to the
stationary iPad.** The underlying call is still `ARTrackedImageManager`
detecting a reference image — only *where the card comes from and how
it's held* changed (§0b, §3), not the detection pipeline. Puzzle types
1–4 below keep v2's constraint that **no puzzle depends on rotation-
state or two-tags-in-frame detection**, since those remain the specific
rough edges research flagged on AR Foundation image tracking. Puzzle
types 6 and 7 are v3 additions and depend on two *new* systems (gesture
interactors, hardware keyboard input) rather than that risk.

1. **Find it.** (Seal 1) The deck itself is discovered/received as a
   single object (e.g. hidden in a drawer, left by the vanished
   technician — see §3) rather than one tag hidden separately. Presenting
   the first card → immediate reveal. Zero synthesis required — this is
   the "wow" beat, must land in the first 60–90 seconds. (v3 note: this
   replaces v2's "tag physically hidden behind a frame/lip" — the room-
   search feeling moves to Seal 4 instead, below.)
2. **Read it right — synthesis.** (Seal 2) The card's *first* presentation
   shows decoy/Consensus text. A separate, already-presented card
   contained a phrase (e.g. a mangled scripture quote) that — if the
   player remembers/rereads it — recontextualizes this card's text as
   ironic or false. **No live multi-image detection needed**: this is
   pure writing — the "puzzle" is the player noticing the callback, not
   the engine detecting anything. Cheapest possible synthesis puzzle.
3. **Sequence it — synthesis.** (Seal 3) 3 cards, presentable in any
   order, but an in-fiction clue (a posted "shift hymn," a maintenance
   checklist) implies a correct *reading* order. Presenting them out of
   the implied order still works narratively (no fail state — see §1
   framing) but the overlay content is noticeably richer / makes more
   sense in the implied order, rewarding the synthesis without punishing
   skipping it. Order-checking, if wanted at all, is a trivial int
   counter in Unity, not a tracking problem.
4. **Combine it — physical, not pose-based.** (Seal 4) **This is where
   v2's room-search feeling lives in v3.** A card in the deck is
   incomplete/blank until the player finds and adds a physical piece from
   the real room (a torn fragment, a rubbing, a second small card taped
   somewhere) and presents the combined result. The "combine" is in the
   real world (find + physically assemble → now presentable), not in the
   frame (no need to hold two tracked images in shot simultaneously).
5. **[Stretch, optional] Two-in-frame.** Only attempt if a technical
   spike (§10) confirms `ARTrackedImageManager` reliably tracks two images
   in one frame with usable relative pose on your specific iPad. If it
   works: use it for one bonus/optional lore card, not a required beat.
   If it doesn't: cut without any loss to the critical path.
6. **Align it — touch manipulation (v3, Seal 6, stretch).** Presenting the
   card spawns an AR object (a ward-fragment, a broken seal-shape) that
   the player must drag/rotate/scale with touch gestures until it locks
   into a correct orientation relative to the anchored image, via XR
   Interaction Toolkit's AR gesture interactors (see §9 for exact
   package/version to lock). Needs its own technical spike (§10) before
   any seal content is built on top of it — treat as unproven until that
   spike passes, same posture v2 took toward `ARTrackedImageManager`
   itself in week 1.
7. **Unlock it — typed override code (v3, Seal 7, stretch).** An
   override code is revealed in fragments across earlier seals (e.g. one
   digit/word per card). At Seal 7, the player types the assembled code
   using the iPad's attached physical keyboard (read via Unity's Input
   System, not the on-screen `TouchScreenKeyboard` — the keyboard case is
   attached throughout, so there's no reason to summon a software
   keyboard) to unlock a final log or door. Needs its own technical spike
   (§10): confirm hardware key events reach Unity reliably with the case
   attached before writing puzzle content around it.

---

## 6. Interaction scope (locked, v3: six verbs)

v2 locked four verbs to protect a 4-week timeline. v3 adds two more
(gesture manipulation, typed input) as a deliberate scope increase —
see §0b/§10 for the timeline cost this carries.

- **Present glyph card** — hold a physical card up to the stationary
  iPad's camera; core verb, always available.
- **Tap AR object** — tap a spawned AR element (the phoenix-mote, a
  floating glyph fragment) to trigger a reaction, short dialogue line, or
  next reveal. Cheap (basic raycast-on-tap), adds interactivity beyond
  passive presentation without a new system.
- **Toggle mana-vision** — persistent UI button; when active, previously
  presented cards show lingering residue/glow even without re-presenting.
  Reinforces "seeing what others can't" and gives the player something to
  *do* between presentations.
- **Choose dialogue option** — exactly one moment, the finale. Two buttons
  (Quiet Door / Loud Key). Not reused anywhere else, so it never needs
  general-purpose branching-dialogue infrastructure.
- **Manipulate AR object (v3)** — drag/rotate/scale a spawned AR object
  with touch gestures (XR Interaction Toolkit AR gesture interactors) to
  solve the Seal 6 "align it" puzzle (§5). New system; requires its own
  spike (§10) before content is built on it.
- **Type on physical keyboard (v3)** — with the keyboard case attached,
  type an assembled override code, read via Unity's Input System, to
  solve the Seal 7 "unlock it" puzzle (§5). New system; requires its own
  spike (§10) before content is built on it.

**Explicitly out of scope:** physics stacking/object puzzles beyond the
one gesture-manipulation seal above, precise multi-object relative-pose
puzzles (rotation-gated or two-tag-relative-pose puzzles beyond the one
optional stretch item), combat, multiplayer, any fail state or timer. If
a team member proposes a feature outside this list of six verbs, the
default answer is no unless it replaces something already here — this
list is the whole interaction budget, and it is now larger than a
4-week build supports as-is (§0b, §10).

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

### 9a. v3 additions: touch manipulation and keyboard input

- **Touch manipulation (Seal 6, "align it"):** Unity's **XR Interaction
  Toolkit** ships AR-specific gesture interactors (tap, drag, pinch,
  twist) that translate touch events into object manipulation — this is
  the first-party path and what to cite/use, rather than hand-rolling
  gesture math. A third-party asset (Lean Touch) is a lighter-weight
  alternative commonly used in tutorials if XR Interaction Toolkit proves
  heavier to wire up than expected; either is viable, but pick one and
  lock it before building Seal 6 content, same as the URP-vs-Built-in
  decision below. Needs a dedicated spike: confirm drag/rotate/scale
  gestures work smoothly on an anchored `ARTrackedImage`-spawned object on
  the actual iPad, since gesture interactors were primarily documented/
  tested against plane-detection placement, not image-anchored objects —
  this combination is the actual open risk, not the gestures themselves.
- **Typed override code (Seal 7, "unlock it"):** with the keyboard case
  attached, hardware key input should reach Unity through the standard
  Input System without needing `TouchScreenKeyboard` (which is for
  summoning iOS's *on-screen* keyboard — not needed here since a physical
  one is always attached). Needs a dedicated spike: confirm hardware key
  events from the keyboard case are actually received while an AR camera
  session is active and the app has focus — this specific combination
  (AR session + hardware keyboard input) is untested by this team and is
  the real risk, not text input in the abstract.

### 9b. Original v2 pipeline decision (unchanged)

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

**Open technical questions (research before/during week 1 spike):**

- **Render pipeline: URP vs. Built-in.** Not yet decided. URP is Unity's
  standard recommendation for AR Foundation on mobile/iPad performance
  and is the likely default — but this must be confirmed and locked
  *before* anyone starts building shaders or VFX against it, since
  Built-in and URP shaders aren't drop-in compatible. Decide at project
  creation (Unity Hub's New Project template picker), not after.
- **Reference-image-library iteration workflow.** `ARTrackedImageManager`
  requires glyphs to be registered in an `XRReferenceImageLibrary` asset.
  Need to confirm the actual workflow for adding/swapping reference images
  as art gets iterated week-to-week (Seals get real art in weeks 2–3, per
  §10) — specifically whether the library needs a full reimport/rebuild
  per change or supports incremental updates, since this directly affects
  art iteration speed.
- **Phoenix-mote VFX approach (§2's pitch-deck beat).** Needs to be
  achievable with **free/built-in tooling only** (Unity Personal
  constraint, see §12). Default assumption: Unity's built-in Particle
  System plus a URP glow/bloom post-process gets this without a paid
  shader pack — worth ~30 min of research to confirm before committing
  design time to it in week 2.

---

## 10. Production plan (v3: extended — no longer fits 4 weeks, see §0b)

v2 fit 4 weeks by locking 4 verbs and cutting anything with open
technical risk. v3 adds two more systems (gesture manipulation, keyboard
input) that each need their own spike-before-content treatment, same as
`ARTrackedImageManager` got in v2's week 1. Realistic estimate below is
**6 weeks**, not 4 — if the course deadline is a hard 4 weeks, cut Seals
6 and/or 7 (and their verbs) rather than compressing every week's spike
time, since compressing spike time is exactly the mistake v1→v2 was
written to avoid.

| Week | Goal | Key risk to retire |
|---|---|---|
| 1 | **Technical spike, not content — image tracking.** Get `ARTrackedImageManager` running on the actual iPad: one reference card → one spawned/anchored AR object that stays put in the room, presented to a stationary iPad on its keyboard case. Confirm image-count ceiling and rotation-detection behavior on your specific device. | Whether the core pipeline works at all — do not touch narrative content until this is proven end to end. |
| 2 | **Technical spike, not content — v3 additions.** Confirm (a) XR Interaction Toolkit (or Lean Touch) gesture manipulation works on an `ARTrackedImage`-anchored object, not just a plane-placed one, and (b) hardware keyboard input from the case reaches Unity's Input System while an AR session is active. Both are genuinely untested combinations for this team (§9a) — do not commit Seal 6/7 content until both pass. | Whether the two v3 systems work at all, independently confirmed before any content is built on either. |
| 3 | Build Seals 1–2 fully (art, overlay text, tap interactions, mana-vision toggle), using placeholder VO/text if needed. | Whether "present → reveal → tap → reaction" feels good as a core loop before scaling to the full seal count. |
| 4 | Build Seals 3–4 (sequence + physical-combine puzzles), wire the required mutual-exclusivity reveal, integrate real writing/art into all seals so far. | The synthesis puzzles (2 and 3) — verify with an internal playtester that the callback/order clues are noticeable without being explicit hints. |
| 5 | Build Seals 6–7 (align-it gesture puzzle, unlock-it typed-code puzzle) **only if week 2's spikes passed** — otherwise use this week to add polish/depth to Seals 1–5 instead. Build the finale (single card, two-button choice, two ending states). | Whether the v3 stretch seals are worth their build cost vs. just polishing the core 5 — make this call explicitly, don't default into building both. |
| 6 | **Playtesting and polish only** — no new features. | The open design risks in §11 — get real playtesters on them with days to react, not hours. |

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
- **(v3) Does the stationary-iPad-and-card-deck reframe actually lose the
  "my own room is lying to me" feeling** (§2), or does presenting cards
  to a fixed terminal create its own version of implication? Test this
  directly and be willing to revert to hidden-in-room scanning (§0b) if
  playtesters describe the experience as "a card game with a screen"
  rather than "my space just changed."
- **(v3) Does the Seal 6 gesture-alignment puzzle feel like solving
  something, or fiddly/frustrating touch-target hunting** on a small
  screen? AR gesture manipulation on handheld devices is known to be
  fussier than desktop mouse manipulation — test early, not in week 6.
- **(v3) Does typing a physical-keyboard code while mid-AR-session break
  immersion** (looking down at keys instead of at the AR scene), or does
  it read as an intentional "switch modes to focus" beat? This is a real
  open question, not a known-good pattern — test rather than assume.

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
