# City Race

A multiplayer motorbike racing game set in Sài Gòn rush-hour traffic, with one-finger controls and minimalist, realistic visuals. Built around Unity + C#, with iOS as the primary release target and Android/Web compatibility tested early.

## Documentation

- Game design v0.2: [Tiếng Việt](docs/GDD.vi.md) · [English](docs/GDD.en.md)
- Technical stack v0.1: [Tiếng Việt](docs/TECH_STACK.vi.md) · [English](docs/TECH_STACK.en.md)
- Engineering guidelines v0.1: [Tiếng Việt](docs/ENGINEERING_GUIDELINES.vi.md) · [English](docs/ENGINEERING_GUIDELINES.en.md)
- Roadmap: [Tiếng Việt](docs/ROADMAP.vi.md) · [English](docs/ROADMAP.en.md)
- Backlog: [Tiếng Việt](docs/BACKLOG.vi.md) · [English](docs/BACKLOG.en.md)
- Setup and M0 validation: [Tiếng Việt](docs/SETUP.vi.md) · [English](docs/SETUP.en.md)
- [Agent entry point](AGENTS.md)

Current stage: design and technical planning. This repository contains documentation and unvalidated bootstrap tooling; no generated Unity project, installed Unity packages, game builds, or runtime benchmarks exist yet. M0 establishes the first verified iPhone and Web smoke builds. The technical documents define the implementation baseline and validation gates. Keep both language editions aligned.

## Starting stack

Unity 6.3 LTS, C#, URP, Input System, and uGUI/TextMeshPro. Initial networking candidate: Netcode for GameObjects, Unity Transport, Multiplayer Services Sessions/Relay, and anonymous authentication. Exact versions are pinned when the Unity project is created. Multiplayer responsiveness and native/Web compatibility must be validated before expanding content.

## Repository

[aletuan/city-race](https://github.com/aletuan/city-race)
