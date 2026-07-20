# CS247G-P2

AR project repo for CS247G / SymSys 195G.

## Status: tooling is set, concept is not

**What's decided:** the technical approach. This repo assumes **Unity +
AR Foundation (ARKit), using `ARTrackedImageManager`-based image tracking**
— not a real AprilTag fiducial library — as the pipeline (see
[`DESIGN.md`](./DESIGN.md) §9 for the reasoning). Repo scaffolding
(`.gitignore`, `.gitattributes`/LFS, `LICENSE`) is set up around that
stack.

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

## Related repo

A separate 2D gridworld game repo will be set up alongside this one
(different tech stack); that project's storyline is also still to be
worked out as a group.

## Team

| Name | Relevant experience |
|---|---|
| Daniel | Unity, Xcode, GitHub, robotics |
| Michelle | GitHub, robotics |
| Xi | Vibe coding |
| Alex | Vibe coding |

Daniel is currently the only team member with Unity/Xcode experience —
worth factoring into how we divide AR-specific build/deploy work vs.
content/design work once mechanics are locked.

## Setup

See [`SETUP.md`](./SETUP.md) for installing Unity Hub, Xcode signing, and
Git LFS before pulling the eventual Unity project into this repo.

## License

MIT — see [`LICENSE`](./LICENSE).
