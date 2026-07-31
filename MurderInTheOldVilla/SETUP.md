# Project setup

## Rendering choice

This project uses **High Definition Render Pipeline (HDRP)**. A render pipeline is the
part of Unity that draws materials, lights, shadows, and post-processing.
Assets authored only for another pipeline often appear pink until their
materials are converted.

The
[FPS Horror Game Starter Pack](https://assetstore.unity.com/packages/templates/packs/fps-horror-game-starter-pack-310075)
is HDRP-only. When importing it, leave **Project Settings** unchecked as
instructed by the publisher so the package does not overwrite this project's
input, build, or quality configuration.

## Generate the horror villa

After Unity finishes importing:

1. Download the starter pack from **Package Manager > My Assets**.
2. Import its `Assets/FpsHorrorKit` content but leave `ProjectSettings`
   unchecked.
3. The integrated scene is generated automatically. If needed, select
   **Murder in Old Villa > Build Horror Villa Scene** manually.
4. Confirm the scene was created at
   `Assets/_Project/Scenes/VillaHorrorPrototype.unity`.
5. Open the scene and press Play.
6. Walk with `WASD`, sprint with `Left Shift`, jump with `Space`, look with the
   mouse, and collect evidence with `E`.

Running the generator again safely rebuilds the generated scene and evidence
data.

## Local test build

The generated macOS test build is located at
`Builds/macOS/MurderInTheOldVilla.app`. Builds are intentionally ignored by Git.

Asset Store source files should not be committed to a public repository even
when the package is free. Every collaborator should acquire the package under
their own Unity account. Original scripts, scenes, prefabs, and project settings
remain version-controlled.

Unity 6000.5 deprecates several instance-ID APIs used by the pack's original
Cinemachine/Splines dependencies. Small, license-preserving compatibility copies
are embedded under `Packages/` so the project compiles consistently.

## Git

Git LFS is configured for large textures, audio, models, and video. Unity's
generated `Library`, `Temp`, `Logs`, `Obj`, and `UserSettings` directories are
ignored.

Before committing, check:

```bash
git lfs install
git status
```
