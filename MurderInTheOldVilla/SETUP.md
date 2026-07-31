# Project setup

## Rendering choice

This project uses **Universal Render Pipeline (URP)**. A render pipeline is the
part of Unity that draws materials, lights, shadows, and post-processing.
Assets authored only for another pipeline often appear pink until their
materials are converted.

The proposed
[FPS Horror Game Starter Pack](https://assetstore.unity.com/packages/templates/packs/fps-horror-game-starter-pack-310075)
is free, but its Asset Store listing declares HDRP-only compatibility. It is
therefore not imported into this URP baseline.

## Generate the prototype

After Unity finishes importing:

1. The scene is generated automatically on first open. If needed, select
   **Murder in Old Villa > Build Prototype Scene** manually.
2. Confirm the scene was created at
   `Assets/_Project/Scenes/VillaPrototype.unity`.
3. Open the scene and press Play.
4. Walk with `WASD`, look with the mouse, and inspect highlighted objects with
   `E`.

Running the generator again safely rebuilds the generated scene and evidence
data.

## Adding a free environment later

Prefer an environment whose listing explicitly supports **URP** and the Unity
6 generation. Import it into `Assets/AssetStore/<PackageName>`, then:

1. Open `VillaPrototype`.
2. Place the environment model beneath an `Environment` root object.
3. Ensure floors, walls, stairs, and doors have colliders.
4. Keep the `Player`, `GameSystems`, `Evidence`, and UI objects from the
   prototype scene.
5. Replace only the graybox geometry after verifying movement and interactions.

Asset Store source files should not be committed to a public repository even
when the package is free. Every collaborator should acquire the package under
their own Unity account. Original scripts, scenes, prefabs, and project settings
remain version-controlled.

## Git

Git LFS is configured for large textures, audio, models, and video. Unity's
generated `Library`, `Temp`, `Logs`, `Obj`, and `UserSettings` directories are
ignored.

Before committing, check:

```bash
git lfs install
git status
```
