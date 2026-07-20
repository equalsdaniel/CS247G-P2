# Team tutorials & onboarding

Living doc of what to read/watch before (or while) working in this repo.
Organized so everyone does the Git/GitHub section first, then branches
into Unity material — same Unity project can support both the AR and 2D
gridworld directions, so most of the Unity-fundamentals material below is
useful regardless of which one the team lands on.

Not exhaustive, not written by an expert — this is “here’s roughly what I
remember needing when I first learned this stuff,” organized so you don’t
have to guess the order. Add to it as we find better/newer resources.

---

## 0. Before anything: quick self-check

If you're newer to programming generally, do one gut-check before diving
into Unity: write a tiny script from scratch (no AI assist) that does
something trivial — FizzBuzz, or "move a square around with arrow keys" in
any language. If that's easy, skip straight to Git/GitHub below. If it's
rough, spend a bit of time on C# fundamentals first (link in §2) before
Unity-specific material — Unity's tutorials assume you're comfortable with
variables/functions/loops/conditionals already.

---

## 1. Git & GitHub (everyone, do this first)

Even if you've used Git before, do a quick pass — the specific pain point
for this project is **merge conflicts on Unity scene/prefab files**, which
behave differently from conflicts in normal text/code files.

- **GitHub's own "Hello World" quickstart**: https://docs.github.com/get-started/quickstart/hello-world
  — repos, branches, commits, and the pull-request workflow, hands-on.
- **Git basics**: https://docs.github.com/en/get-started/git-basics
  — the core vocabulary (clone, status, commit, push/pull) if you want the
  concepts spelled out before doing the quickstart above.
- **Resolving merge conflicts**: https://docs.github.com/en/pull-requests/collaborating-with-pull-requests/addressing-merge-conflicts/resolving-a-merge-conflict-on-github
  — do this one even if you know Git already; conflict resolution is the
  actual skill that matters day-to-day on a shared repo.
- **Why Unity scenes conflict badly**: we've set `Force Text` serialization
  (see DESIGN.md §12) so scenes/prefabs are diffable YAML instead of
  opaque binary — but two people editing the *same* scene at the same time
  will still conflict. Practical fix is social: say out loud in the team
  channel when you start editing a scene, treat it as "owned" by one
  person at a time.
- **Git LFS** (large files — textures, audio, models): already configured
  in `.gitattributes`. Just make sure `git lfs install` is run once per
  machine before your first clone/pull — see SETUP.md §4.

---

## 2. C# fundamentals (if your self-check in §0 was rough)

- **Unity Learn's "Junior Programmer" pathway**: https://learn.unity.com/pathway/junior-programmer
  — teaches C# *inside* Unity rather than as abstract programming-101,
  which tends to land better for game dev specifically than a standalone
  C# course.
- Skip this section entirely if §0's self-check felt easy — go straight
  to §3.

---

## 3. Unity Editor basics (everyone new to Unity)

Do this regardless of AR vs. 2D — it's the shared foundation either way.

- **Unity Learn's "Unity Essentials" pathway**: https://learn.unity.com/pathway/unity-essentials
  — Editor interface, GameObjects, Components, Prefabs, Scenes. This
  vocabulary is used constantly in every other tutorial below, so don't
  skip it even if it feels basic.
- **Importing assets / objects into a scene**: Unity Manual —
  https://docs.unity3d.com/Manual/ImportingAssets.html — covers how models,
  textures, and audio actually get pulled into a project and what happens
  to them on import (this matters for us specifically because of Git LFS —
  imported assets are exactly what LFS is tracking).
- **Prefabs workflow**: https://docs.unity3d.com/Manual/Prefabs.html —
  reusable objects; relevant to both AR glyph-plate objects and 2D
  gridworld tile/entity objects.

---

## 4. Collision, triggers, and collision-driven animation (both paths)

This is genuinely shared between AR and 2D — "something happens when two
things touch" is a mechanic in almost every version of this project
(scanning a glyph, an AR object reacting to a tap, a 2D player entering a
tile/zone).

- **Colliders & triggers overview**: https://docs.unity3d.com/Manual/CollidersOverview.html
- **`OnTriggerEnter` / `OnCollisionEnter` scripting basics**: Unity Learn
  has a short module on this within the Junior Programmer pathway (§2) —
  worth doing even if you did the self-check fine, since the API names
  (`OnTriggerEnter2D` vs `OnTriggerEnter`, 2D vs 3D physics) are an easy
  mix-up between the AR (3D) and gridworld (2D) paths.
- **Driving Animator states from code/collisions**: https://docs.unity3d.com/Manual/AnimationOverview.html
  and the Animator Controller manual page — this is the "collision
  triggers an animation" pattern (e.g. tapping the phoenix-mote plays a
  reaction, per DESIGN.md §6's "Tap AR object" verb).

---

## 5. AR-specific (only if the team goes AR)

- **AR Foundation manual — Image tracking**: https://docs.unity3d.com/Packages/com.unity.xr.arfoundation@6.2/manual/features/image-tracking/introduction.html
  — this is our actual pipeline (`ARTrackedImageManager`, see DESIGN.md
  §9). Prefer this over random YouTube tutorials, which are often several
  major AR Foundation versions out of date. **Version note:** this link is
  pinned to AR Foundation 6.2 docs; once we've picked the exact Unity
  Editor version (SETUP.md §2), swap `@6.2` in the URL for whatever AR
  Foundation version Package Manager actually installs for us, since
  Unity's doc URLs are version-namespaced and older/newer pages can differ.
- **LiDAR / plane detection on iPad**: https://docs.unity3d.com/Packages/com.unity.xr.arfoundation@6.2/manual/features/plane-detection.html
  — LiDAR mainly buys us more accurate world-anchoring (the phoenix-mote
  staying put in the room, per DESIGN.md §2) versus non-LiDAR devices;
  worth understanding at a concept level even though we don't have to
  hand-roll plane detection ourselves. Same version-pinning note as above.
- **Unity → Xcode → device build/deploy**: this step is fiddly enough
  (provisioning, signing, the Xcode export step) that it's better as a
  live screen-share walkthrough from Daniel once, rather than solo
  reading — see SETUP.md §3 and §7 for the written reference to follow
  along with afterward.

---

## 6. 2D gridworld-specific (only if the team goes 2D)

- **Tilemap system**: https://docs.unity3d.com/Manual/com.unity.2d.tilemap.html
  — how grid-based levels actually get built and scripted against.
  Search "Unity Tilemap" on Unity Learn for current in-editor tutorials
  covering sprites/2D physics/colliders alongside this — Unity's course
  catalog changes often enough that a single pinned course link here would
  likely go stale faster than the reference manual pages above.
- **Grid-based movement** (tile-snapped or turn-based, depending on what
  we land on): holding off on a specific tutorial link here until the
  actual movement mechanic is chosen — too many genre-specific variants
  to pick one now.

---

## 7. Using AI tools well (everyone)

Using an AI assistant (Claude, Copilot, Cursor, ChatGPT, etc.) to help
write Unity/C# code is genuinely a good way to move faster on a 4-week
timeline, and everyone on this team is encouraged to lean on one. A
couple of small habits make it work even better:

- **Point your AI tool at the real docs, not just its own training memory.**
  Unity's APIs change across versions; pasting a link from §3–§6 above (or
  the Unity Manual page for whatever component you're working with)
  into your prompt gets you an answer scoped to the version we're actually
  using, instead of a plausible-sounding but outdated one.
- **Give it this repo's context.** `DESIGN.md` (game concept/mechanics) and
  `SETUP.md`/this file (tooling constraints) are useful to paste in or
  point an AI agent at before asking it to write something project-specific
  — it'll produce more relevant suggestions than a generic "how do I make
  a Unity trigger" prompt.
- **If something breaks and it's not obvious why, that's a good moment to
  ask the AI to walk you through what it wrote**, or grab a teammate —
  no pressure to have it all memorized, just handy to have a sense of
  where to look when debugging later. 

---

## Open items

- Once the team converges on AR vs. 2D (or confirms it's doing both /
  deciding later), trim this doc so people aren't reading tutorials for
  the path not taken.
- Add better/newer resource links here as we find them — this is a living
  doc, not a fixed syllabus.
