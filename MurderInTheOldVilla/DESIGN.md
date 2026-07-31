# Murder in the Old Villa — Case Design

## Premise

Villa owner Lin is found strangled by a bedroom curtain cord on his birthday
night. The bedroom appears locked from inside, so the death is initially treated
as suicide. The player must reconstruct the events through physical evidence,
testimony, and replayable memory records.

## Cast

| Character | Role |
|---|---|
| Lin | Victim and villa owner |
| Lin-Y | Niece, mastermind, added the sedative |
| Su | Female cousin, direct murderer |
| Wang | Housekeeper, coerced witness |
| Lin-H | Nephew, innocent witness outside the bedroom |
| Mei | Maid, innocent milk deliverer and red herring |

## Core investigation loop

Explore rooms → inspect evidence → interview suspects → replay memories →
compare facts → present a contradiction → unlock a new lead.

## Three deduction breakthroughs

1. **Guilty knowledge:** Lin-Y denies touching the milk before the detective
   ever asks about it.
2. **Lamp contradiction:** Mei sees the lamp on when she delivers the milk,
   while Lin-H later sees it off.
3. **Newspaper paradox:** A newspaper cannot be read in darkness. The sound was
   produced to make the unconscious or dead victim seem alive.

## Evidence

- Curtain Cord — murder weapon bearing Su's fingerprints
- Milk Cup — contains sedative residue
- Flat Newspaper — inconsistent with the reported rustling
- Bedside Lamp — establishes the room's changing state
- Wang's USB — preserves unmodified surveillance footage

## Canonical timeline

The exact minute-by-minute sequence remains provisional until the locked-room
mechanism is finalized. The current playable prototype focuses on exploration
and evidence collection rather than asserting a finished murder timeline.

Important continuity requirement: Su must enter and stage the bedroom before
Lin-H hears the artificial newspaper sound. The previous `22:15` entry time
conflicted with Lin-H hearing the sound at `22:10` and must not be reused
unchanged.

## Prototype scope

The first vertical slice proves:

- first-person movement on macOS;
- object highlighting and interaction;
- collection of the milk cup, newspaper, curtain cord, and USB;
- evidence state stored across scene changes;
- a replaceable graybox villa environment.

Dialogue, memory replay, contradiction presentation, final accusation, and the
locked-room reveal are subsequent milestones.
