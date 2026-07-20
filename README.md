# CS247G-P2

Game project repo for CS247G / SymSys 195G.

## Status: tooling is set, concept is not

**What's decided:** Unity as the engine — it can support either an AR
build (AR Foundation/ARKit, `ARTrackedImageManager`-based image tracking,
not a real AprilTag fiducial library — see [`DESIGN.md`](./DESIGN.md) §9)
or a 2D gridworld build from the same project setup. Repo scaffolding
(`.gitignore`, `.gitattributes`/LFS, `LICENSE`) is in place either way.

**What's NOT decided:** the storyline and the game mechanics. Those are
still open. This repo does not represent a team decision on concept —
see below.

## About DESIGN.md

[`DESIGN.md`](./DESIGN.md) is **Daniel's individual initial brainstorming
draft**, written solo before group ideation. It's kept in the repo as a
concrete starting reference and a worked example of the tech constraints
(free Unity, 1 iPad, 4 weeks, no prior AR experience) — not as the team's
chosen storyline or mechanics.

**Before we commit to a direction, everyone writes their own design doc.**
Plan: each team member drafts their own `DESIGN_<name>.md` proposing
storyline elements and/or game mechanics within the same technical
constraints, then we do group ideation and converge on one direction
(likely a synthesis, not a single person's draft as-is) before locking
`DESIGN.md` as the team's actual spec.

## Setup

See [`SETUP.md`](./SETUP.md) for installing Unity Hub, Xcode signing, and
Git LFS before pulling the eventual Unity project into this repo.

## Tutorials & onboarding

See [`TUTORIALS.md`](./TUTORIALS.md) for a learning path — Git/GitHub
first, then Unity fundamentals, then AR- or 2D-specific material
depending on which direction the team lands on.

## License

MIT — see [`LICENSE`](./LICENSE).
