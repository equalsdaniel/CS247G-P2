# Development quick start

## First playable

1. Open the project with Unity `6000.5.5f1`.
2. Wait until the status bar finishes importing and the Console has no errors.
3. Confirm the free FPS Horror Game Starter Pack is installed locally under
   `Assets/FpsHorrorKit`.
4. Open `Assets/_Project/Scenes/VillaHorrorPrototype.unity`. If it is missing,
   run **Murder in Old Villa > Build Horror Villa Scene**.
5. Enter Play Mode and test `WASD`, `Left Shift`, `Space`, mouse look, and `E`.

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
5. Replace placeholder evidence meshes with final case-specific props.
