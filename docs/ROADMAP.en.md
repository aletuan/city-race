# City Race Roadmap

Version 0.1 • 5 October 2026 • [Tiếng Việt](ROADMAP.vi.md)

Follow [GDD](GDD.en.md), [stack](TECH_STACK.en.md), and [engineering conventions](ENGINEERING_GUIDELINES.en.md). Milestones are evidence gates, not calendar commitments. Work only on the next unproven risk; do not expand content to compensate for poor handling or networking.

| Milestone | Deliverable | Exit evidence | Current status |
| --- | --- | --- | --- |
| M0 Environment and bootstrap | Pinned Unity project, minimal URP scene, repeatable build path | Scene running on a real iPhone; hosted Web smoke test; editor/package versions recorded | In progress; Unity installation and signing blocked |
| M1 Riding prototype | One-finger bike motor, road, pothole, bus, greybox obstacles | New players can start/turn/stop; repeat-play feedback; target-device performance baseline; Android smoke build | Planned |
| M2 Multiplayer prototype | Two homes, two players, shared traffic and destination; then four players | Native/Web session, responsive movement, consistent results, disconnect handling, Relay bytes per player-hour | Planned |
| M3 Complete small level | Four homes, morning commute, scoped events, simple customisation, audio, results and replay | External group completes the whole flow and voluntarily replays; representative-load performance | Planned |
| M4 Production and polish | Refined validated content, fixes, accessibility and optimisation | Device matrix passes; unresolved issues triaged; performance and network budgets met | Planned |
| M5 Release validation | TestFlight, release metadata, support/privacy pages, live-service configuration | Beta feedback resolved; release checklist reviewed; approved release build | Planned |
| M6 Operations | Crash/cost monitoring and player feedback | Changes prioritised from actual usage, stability and costs | Planned |

## Current sequence

1. Finish M0 environment prerequisites and real iPhone smoke run.
2. Verify a Web build and resolve the package lock.
3. Implement only the M1 control/handling tasks in [backlog](BACKLOG.en.md).
4. Run a small playtest; create a dated report when it happens.
5. Start M2 only when riding passes the GDD criteria.

M0's native smoke scene is a build diagnostic, not a riding prototype. Android portability should be checked during M1 rather than left until release.

## Tracking and evidence

Use Backlog as the task tracker until GitHub Issues are deliberately adopted. Statuses: Todo, In progress, Blocked, Done. Done means acceptance evidence exists. Record date, commit, device/OS/browser, editor, build type, observed outcome and limitations. Keep logs and binaries outside Git by default.

Create docs/playtests/YYYY-MM-DD-topic.en.md and .vi.md only after a real session. Include setup, participants without unnecessary personal details, tasks, observations, defects, decisions and next experiment. Separate observations from interpretation. Do not create a report implying a test occurred when it did not.

Technology changes need a short bilingual decision record. Revisit milestone scope when the experiment fails; do not label it complete to meet a date.
