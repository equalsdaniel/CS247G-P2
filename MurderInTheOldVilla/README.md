# Murder in the Old Villa

English-language first-person detective game developed in Unity.

## Technical baseline

- Unity `6000.5.5f1`
- High Definition Render Pipeline (HDRP) `17.5.0`
- Input System `1.19.0`
- macOS desktop target
- First-person keyboard and mouse controls

HDRP is used for compatibility with the FPS Horror Game Starter Pack and for
the villa's atmospheric lighting.

## Open the project

1. Install Unity `6000.5.5f1` through Unity Hub.
2. In Unity Hub, choose **Add > Add project from disk**.
3. Select this `MurderInTheOldVilla` directory.
4. Allow Unity to resolve packages and import assets.
5. Acquire and download the free FPS Horror Game Starter Pack from the Unity
   Asset Store. Its source is intentionally not redistributed in this repo.
6. Import only its `Assets/FpsHorrorKit` content. Do not import its
   `ProjectSettings`.
7. Unity generates `Assets/_Project/Scenes/VillaHorrorPrototype.unity`.
   If needed, choose **Murder in Old Villa > Build Horror Villa Scene**.
8. Open that scene and press Play.

## Prototype controls

- `WASD`: Move
- `Left Shift`: Sprint
- `Space`: Jump
- Mouse: Look
- `E`: Inspect or interact
- `E`, `Space`, or left click: Advance dialogue
- `Q`: Leave dialogue or cancel inspection
- `Escape`: Release or recapture the mouse

## Character cast (ABCDEF)

- **Amy** — Felix's niece; mastermind who sedated the milk.
- **Ben** — Felix's nephew; innocent witness outside the bedroom.
- **Coco** — Felix's cousin; direct murderer.
- **Dean** — housekeeper; coerced witness who altered his statement.
- **Ella** — maid; innocent milk deliverer and red herring.
- **Felix** — villa owner and murder victim.

The five living characters use looping idle animation, subtle procedural motion,
and turn toward the player during conversations. Felix is deliberately motionless
at the crime scene.

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
    ├── Dialogue/
    └── Player/
```

Original gameplay code belongs under `_Project`. This project's downloaded
starter-pack content lives under `Assets/FpsHorrorKit` and is intentionally
excluded from the public repository.

Character models and animations are from Quaternius' Universal Base Characters
and Universal Animation Library. Both are distributed under CC0; their license
files are included under `Assets/_Project/ThirdParty/Quaternius/Licenses`.

See [SETUP.md](./SETUP.md) for environment-import guidance and
[DESIGN.md](./DESIGN.md) for the current case specification.
