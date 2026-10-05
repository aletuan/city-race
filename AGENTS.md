# City Race agent guide

## Read first

1. [Game design](docs/GDD.en.md) / [Thiết kế game](docs/GDD.vi.md).
2. [Technical stack](docs/TECH_STACK.en.md) / [Tech stack](docs/TECH_STACK.vi.md).
3. [Engineering conventions](docs/ENGINEERING_GUIDELINES.en.md) / [Quy ước kỹ thuật](docs/ENGINEERING_GUIDELINES.vi.md).

User instructions take precedence. The GDD owns gameplay scope; TECH_STACK owns platform and technology decisions; ENGINEERING_GUIDELINES owns implementation rules. If they conflict, resolve against the latest explicit user decision and update both language editions. Ask only when the ambiguity materially blocks the task.

## Project baseline

- Unity + C#, iOS first; Android and Web compatibility tested early. Default engine line: Unity 6.3 LTS, with an exact patch chosen and committed when bootstrapping.
- Start with GameObjects, URP, Input System, and a simple 3D overhead presentation. Camera and handling remain playtest variables.
- Initial multiplayer candidate: Netcode for GameObjects + Unity Transport + Multiplayer Services Sessions/Relay, anonymous authentication, private rooms for 2–4 players. This candidate must pass latency and cross-platform tests before production commitment.
- The repository contains an initialized Unity URP project and an Editor-verified static Smoke scene. The signed iPhone smoke run and user-supplied HTTPS Web render screenshot complete M0; Web console/performance remain unverified. Read [Setup](docs/SETUP.en.md) and [Backlog](docs/BACKLOG.en.md) before starting M1. Never report Unity builds, tests, cloud configuration, or performance results as completed unless actually run and inspected.

M1 riding is now in progress: [prototype controls and validation](docs/RIDING_PROTOTYPE.en.md). Practice (0.0.3) is the enabled scene; Smoke and Riding are retained. Eleven PlayMode tests passed, including course completion, recovery and Restart. The signed 0.0.3 iPhone build/install/launch passed; native visual and handling acceptance is pending. Physical handling, lifecycle and frame-rate acceptance remain open.

## Implementation rules

- Keep race rules in plain C#; isolate Unity, networking, storage, and platform APIs behind small boundaries. Prefer composition, explicit dependencies, and small feature-focused components over global managers or framework scaffolding.
- One authority owns race state, traffic, violations, and results. Separate simulation from visuals. Do not assume Unity physics is deterministic across devices or NGO provides complete prediction/reconciliation.
- Use the same gameplay rules on iOS, Android, and Web. Confine platform conditionals to adapters and build configuration. Web requires a supported transport such as WSS through Relay; raw UDP is not a browser baseline.
- Pin editor/packages; preserve and commit Unity `.meta` files. Do not commit generated Unity caches, credentials, signing material, or local service tokens.
- Profile on real target builds. Performance targets are provisional until measured; document device, build, scenario, and limitations.
- Add meaningful tests for changed rules and integrations. Documentation-only changes need consistency, link, and diff checks, not invented runtime tests.
- Update English and Vietnamese documents together for design or architecture changes. Code identifiers and technical commit messages use English; preserve Vietnamese player-facing text correctly.
- Keep work within the requested scope. A documentation task does not authorize creating paid services, adding gameplay systems, or publishing a game.

## Before delivery

Review the diff and run the checks relevant to the change. Report what changed, what was verified, and what remains untested. Do not invent build commands before the Unity project and build entry points exist. See the engineering guide for the implementation checklist and validation matrix.
