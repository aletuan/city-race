# City Race Backlog

Version 0.1 • 5 October 2026 • [Tiếng Việt](BACKLOG.vi.md) • [Roadmap](ROADMAP.en.md)

P0 blocks the current milestone; P1 follows the foundation. IDs and dependencies are shared across languages. Status reflects observed evidence, not planned success.

| ID | Priority | Task and acceptance | Depends on | Status |
| --- | --- | --- | --- | --- |
| CR-001 | P0 | Inspect editor, Xcode, device, disk and signing; record findings in Setup | None | Done |
| CR-002 | P0 | Install/activate Unity 6000.3.25f1 Apple Silicon plus iOS support; confirm Editor launches | Unity sign-in/license; installation authorised | Todo |
| CR-003 | P0 | Initialise from the installed URP template; commit actual settings, package lock and metadata; compile cleanly | CR-002 | Blocked; bootstrap tooling prepared |
| CR-004 | P0 | Generate camera, road, buildings and bike marker scene; inspect URP rendering | CR-003 | Blocked; scene generator prepared |
| CR-005 | P0 | Export Xcode project, sign with chosen team, install/launch on physical iPhone; observe scene | CR-004, valid Apple signing | Blocked |
| CR-006 | P1 | Install Web support, build and serve over HTTPS; inspect browser scene and loading | CR-004, storage | Todo |
| CR-010 | P0 | Implement normalized pointer input with release/focus-loss braking; mouse and touch behave consistently | M0 | Todo |
| CR-011 | P0 | Implement constrained bike motor with acceleration, turning and braking; compare frame rates | CR-010 | Todo |
| CR-012 | P0 | Add readable pothole/bus/obstacle reactions; no unexplained forced failure | CR-011 | Todo |
| CR-013 | P1 | Tune camera, safe areas and visibility; finger does not obscure critical obstacles | CR-011 | Todo |
| CR-014 | P1 | Install Android modules and run on a real device; record compatibility issues | CR-011, device and storage | Todo |
| CR-015 | P0 | Playtest with 5–8 people using GDD criteria; measure device performance and record decisions | CR-012, CR-013 | Todo |
| CR-020 | P0 | Prove two-player sessions, authority and latency behaviour before four-player content | M1 | Planned milestone |
| CR-021 | P1 | Measure Relay traffic per player-hour, including host contribution; estimate monthly cost | CR-020 | Planned milestone |

## Current blockers

- Storage blocker resolved: internal about 38 GiB free and T7 about 870 GiB free on 5 October. Check temporary installation space as work proceeds.
- No Unity Editor/Hub found in the standard system/user locations. Activation status cannot be verified.
- Xcode 26.6 is available. The previously paired iPhone 15 Pro Max currently reports unavailable; reconnect and recheck OS/Developer Mode before the device run.
- No valid code-signing identity returned by the keychain check. Signing team must be configured.
- Git LFS is absent; install/configure before adding large binary source assets. Current work is text-only.

The full environment and continuation steps are in [Setup](SETUP.en.md). Do not mark CR-003–005 done until the tools have actually run successfully.
