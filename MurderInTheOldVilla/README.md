# Murder in the Old Villa

English-language first-person detective game developed in Unity.

## Technical baseline

- Unity `6000.5.5f1`
- Universal Render Pipeline (URP) `17.5.0`
- Input System `1.19.0`
- macOS desktop target
- First-person keyboard and mouse controls

URP is intentionally retained because the existing project and controller are
already configured for it. The suggested FPS Horror Game Starter Pack is
HDRP-only, so it is not part of this baseline.

## Open the project

1. Install Unity `6000.5.5f1` through Unity Hub.
2. In Unity Hub, choose **Add > Add project from disk**.
3. Select this `MurderInTheOldVilla` directory.
4. Allow Unity to resolve packages and import assets.
5. In Unity, choose **Murder in Old Villa > Build Prototype Scene**.
6. Open `Assets/_Project/Scenes/VillaPrototype.unity` and press Play.

## Prototype controls

- `WASD`: Move
- Mouse: Look
- `E`: Inspect or interact
- `Tab`: Investigation log (when its UI is present)
- `Escape`: Release or recapture the mouse

## Source layout

```text
Assets/_Project/
├── Art/
├── Data/
│   └── Evidence/
├── Prefabs/
├── Scenes/
└── Scripts/
    ├── Editor/
    ├── Interaction/
    ├── Investigation/
    └── Player/
```

Original gameplay code belongs under `_Project`. Imported Asset Store content
belongs under `Assets/AssetStore` or `Assets/ThirdParty` and is intentionally
excluded from the public repository.

See [SETUP.md](./SETUP.md) for environment-import guidance and
[DESIGN.md](./DESIGN.md) for the current case specification.
