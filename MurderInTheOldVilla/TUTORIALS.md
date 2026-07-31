# Development quick start

## First playable

1. Open the project with Unity `6000.5.5f1`.
2. Wait until the status bar finishes importing and the Console has no errors.
3. Confirm Unity automatically opens
   `Assets/_Project/Scenes/VillaPrototype.unity`. If not, run
   **Murder in Old Villa > Build Prototype Scene**.
5. Enter Play Mode and test `WASD`, mouse look, `E`, and `Escape`.

## Editing rules

- Put original game code and content under `Assets/_Project`.
- Treat scenes as single-owner files while editing to avoid merge conflicts.
- Never commit `Library`, `Temp`, `Logs`, builds, or downloaded Asset Store
  source content.
- Commit every `.meta` file that belongs to a committed asset.

## Next milestones

1. Add a visible evidence journal with collected-object feedback.
2. Add suspect dialogue data and interrogation UI.
3. Add replayable memory records.
4. Add contradiction selection and validation.
5. Replace graybox rooms with a URP-compatible old-villa environment.
