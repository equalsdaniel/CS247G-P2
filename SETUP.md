# Team setup — UNSHACKLED

Everyone on the team is on a Mac, but we don't yet know who has Apple
Silicon vs. Intel, or which macOS version each person is running. **Step 0
below is mandatory for everyone** — post your results in the team channel
before installing anything, since it changes which Unity Hub build you
grab.

---

## 0. Check your machine first

Run this in Terminal and share the output with the team:

```bash
uname -m        # arm64 = Apple Silicon (M1/M2/M3/M4), x86_64 = Intel
sw_vers         # macOS version
df -h /         # free disk space — see step 1, this is the #1 install blocker
```

Post: chip type, macOS version, and GB free on `/`.

---

## 1. Free up disk space BEFORE installing Unity

**This is the most common reason Unity Hub installs fail or hang** — not
corruption, just running out of room mid-install. Unity Editor + iOS Build
Support is a **15–20+ GB** install. If `df -h /` shows less than ~25 GB
free, clear space first:

- Empty Trash, empty Xcode's old device support / archives
  (`~/Library/Developer/Xcode/DerivedData`, `~/Library/Developer/Xcode/Archives`)
  if you've used Xcode before.
- Check `~/Library/Caches` for large app caches you can safely clear.
- If a previous Unity install attempt failed, remove the partial install
  before retrying: `/Applications/Unity` and `~/Library/Unity`.

If you're not sure what's safe to delete, ask before running anything
destructive — `rm -rf` on the wrong folder doesn't come back.

---

## 2. Install Unity Hub

1. Download Unity Hub: https://unity.com/download (universal binary,
   works on both Apple Silicon and Intel).
2. Sign in / create a free **Unity Personal** account — this project only
   needs the free tier.
3. In Unity Hub → **Installs → Install Editor**, install the version the
   team has agreed on (pin an exact version here once decided — mixed
   Editor versions across teammates causes project-format churn).
4. During install, check the box for **iOS Build Support** — required for
   deploying to the iPad. This is the module most likely to push you over
   the disk-space line in step 1, so double check free space first.

---

## 3. Apple ID for signing (read DESIGN.md §12 first)

- **Xcode is free** from the Mac App Store, no paid enrollment needed.
- The team should agree on **one shared Apple ID** used for all
  "Personal Team" signing in Xcode's Signing & Capabilities tab. If two
  people sign with two different Apple IDs onto the same iPad, Xcode will
  fight itself over provisioning profiles.
- Personal Team limits: 3 devices max, 7-day provisioning profile expiry
  — expect to redeploy from Xcode roughly weekly. Not a blocker with one
  test iPad, just a recurring chore.
- We are **not** using TestFlight ($99/yr Developer Program) — not needed
  for a single test device. See DESIGN.md §12 for the full reasoning.

---

## 4. Clone the repo

```bash
git clone <repo-url>
cd CS247G-P2
```

**Install Git LFS before doing anything else** (textures/audio/models are
tracked via LFS, see `.gitattributes`):

```bash
brew install git-lfs   # if you don't already have it
git lfs install
git lfs pull
```

If you clone without Git LFS installed first, large asset files will show
up as tiny pointer-text files instead of real content — run `git lfs pull`
to fix that after installing.

---

## 5. Open the Unity project

1. Unity Hub → **Add** → select the cloned repo folder.
2. Open with the pinned Editor version from step 2.
3. Once open, confirm **Edit → Project Settings → Editor**:
   - **Asset Serialization → Force Text** (should already be set in the
     committed `ProjectSettings/`, but verify)
   - **Visible Meta Files** should already be on
   - These keep scenes/prefabs diffable — don't change them locally.
4. **AR Foundation / ARKit**: this project uses Unity's built-in
   `ARTrackedImageManager` (AR Foundation + ARKit plugin), **not** a
   third-party AprilTag library — see DESIGN.md §9 for why. Nothing extra
   to install here beyond what's already in the project's package
   manifest; if Package Manager shows AR Foundation / Apple ARKit XR
   Plugin as missing or unresolved, use **Window → Package Manager** and
   let it resolve from the manifest rather than reinstalling manually.

---

## 6. Scene ownership (avoid merge pain)

Per DESIGN.md §12: Force Text avoids opaque binary diffs, but scene/prefab
merge conflicts are still the biggest realistic friction point. **Say out
loud in the team channel when you start editing a scene**, and treat each
scene as owned by one person at a time. Don't rely on Git to merge
simultaneous scene edits.

---

## 7. Building to the iPad

Whoever has the iPad + a Mac in front of them that week can build:
Unity → **Build Settings → iOS → Build** (exports an Xcode project) →
open in Xcode → sign with the shared Apple ID (step 3) → run to device.

---

## Troubleshooting

- **Unity Hub install stalls, fails silently, or the app won't launch
  after install** → almost always disk space. Re-check `df -h /` (want
  25+ GB free) before filing it as a "real" bug. Remove any partial
  `/Applications/Unity` + `~/Library/Unity` and reinstall clean.
- **Xcode nagging about re-signing** → check everyone's using the same
  Apple ID (step 3).
- **Large files showing as tiny text pointers after clone** → Git LFS
  wasn't installed before cloning; run `git lfs pull`.
