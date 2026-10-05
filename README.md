# City Race

A multiplayer motorbike racing game set in Sài Gòn rush-hour traffic, with one-finger controls and minimalist, realistic visuals. Built around Unity + C#, with iOS as the primary release target and Android/Web compatibility tested early.

## Documentation

Project documentation is English only; it is the single source of truth. Vietnamese is used for player-facing game text, not for repository docs.

- [Game design v0.2](docs/GDD.md)
- [Technical stack v0.1](docs/TECH_STACK.md)
- [Engineering guidelines v0.1](docs/ENGINEERING_GUIDELINES.md)
- [Roadmap](docs/ROADMAP.md)
- [Backlog](docs/BACKLOG.md)
- [Riding prototype and current evidence](docs/RIDING_PROTOTYPE.md)
- [Setup and M0 validation](docs/SETUP.md)
- [Agent entry point](AGENTS.md)

Current stage: M0 smoke milestone complete; M1 riding prototype next. The Unity 6000.3.25f1 URP project and static Smoke scene are initialized. Editor compilation and Play Mode rendering were verified on 5 October 2026. Signed iPhone build/install/launch and portrait rendering are verified; HTTPS Web loading/rendering is confirmed by a user-provided Chrome screenshot. Console checks, mobile Web and runtime benchmarks remain unverified. Follow Setup for commands and current evidence.

## Starting stack

Unity 6.3 LTS, C#, URP, Input System, and uGUI/TextMeshPro. Initial networking candidate: Netcode for GameObjects, Unity Transport, Multiplayer Services Sessions/Relay, and anonymous authentication. Editor and resolved packages are pinned in `.unity-version`, ProjectSettings and Packages. Multiplayer responsiveness and native/Web compatibility must be validated before expanding content.

## Repository

[aletuan/city-race](https://github.com/aletuan/city-race)
