# City Race Engineering Guidelines

Version 0.1 • 3 October 2026 • [Tiếng Việt](ENGINEERING_GUIDELINES.vi.md)

These are project conventions for future implementation, not a claim that the game or its benchmarks already exist. Read [the stack](TECH_STACK.en.md) and [GDD](GDD.en.md) first. Prefer the smallest clear implementation that satisfies the current milestone.

## 1 Apply SOLID to real boundaries

| Principle | City Race convention |
| --- | --- |
| Single responsibility | BikeMotor moves a bike; RaceRules validates finishes; SessionGateway manages connectivity; BikeView renders state. Avoid a GameManager that owns everything. |
| Open/closed | Extend genuinely varying behaviours with small strategies or data. Start simple; do not build a plugin system for one pothole. |
| Liskov substitution | Alternative input/storage/session implementations obey the same contract, including cancellation and failure behaviour. Test observable behaviour, not inheritance structure. |
| Interface segregation | Separate IRaceClock, IRideInput and IPreferencesStore when consumers need those boundaries. Avoid one service interface exposing the entire game. |
| Dependency inversion | Core defines required contracts; infrastructure implements them. Race rules must not locate a global NetworkManager or read UI state. |

Prefer composition over deep inheritance. Use interfaces at external boundaries or actual variation points, not for every class. Inject clocks and randomness where repeatability matters. Keep domain rules free of Unity scene dependencies; Unity-specific movement queries belong in Gameplay adapters. A networking adapter may wrap a vendor singleton internally, but must not spread it through game rules.

## 2 Layout and C# conventions

When code is introduced, use this structure; do not create empty folders just to match it:

- Assets/CityRace/Runtime/Core, Gameplay, Infrastructure, Presentation, Bootstrap.
- Assets/CityRace/Editor for editor-only code.
- Assets/CityRace/Tests/EditMode and PlayMode.
- Assets/CityRace/Content for scenes, prefabs, tuning, art and audio.
- docs/decisions for dated architecture decisions.

Namespace by ownership, for example CityRace.Gameplay.Riding. One main type per file; filename matches the type. Use PascalCase for types, methods, properties and enum members; I-prefix interfaces; camelCase for parameters/locals; _camelCase for private fields. Use four spaces, UTF-8, LF and a final newline. Use braces consistently.

Use private serialized fields for Inspector configuration and read-only properties for consumers. Avoid public mutable fields. Include units in ambiguous values, such as speedMetersPerSecond or stopDurationSeconds. Use stable IDs for network/save references, never display names or Unity instance IDs.

Use the language features supported by the pinned editor; do not assume .NET desktop APIs exist on every build target. Avoid reflection-heavy registration and runtime code generation by default. If stripping or AOT requires preservation metadata, keep it narrow and verify a release build.

Comments explain intent and constraints. Player-facing strings use stable localization keys once UI is added; do not concatenate translated fragments. Include Vietnamese glyphs and test diacritics in TextMeshPro.

## 3 State, time and lifecycle

Model race phases explicitly: Lobby → Countdown → Racing → Results, with clear cancellation/disconnect transitions. Validate commands against phase. Make finish and penalty resolution idempotent so duplicate network events cannot score twice.

Keep authored ScriptableObjects immutable during play. Allocate separate state per race/player. Do not retain player references across scene reloads; reset static state when entering play mode, including when domain reload is disabled.

Read input through Input System, buffer the latest command, and advance movement on a fixed simulation step. Never multiply motion by rendered frame count. Camera and visual smoothing can run at render rate; use LateUpdate where appropriate. There must be one owner of simulation stepping, not both manual and automatic physics advancement.

Use the authoritative race clock for race outcomes. Use presentation clocks for visual effects and UI. Keep fixed timestep and network tick separate; document their relationship. A shared seed does not make Unity physics deterministic across architectures.

Subscribe and unsubscribe symmetrically. Cancel asynchronous operations when their owner is destroyed, the room is left, or the app shuts down. Avoid async void except required event handlers; observe exceptions. Return to the Unity main thread before touching Unity objects. Never block it with Wait, Result or synchronous network I/O.

On focus loss, release held input and apply the defined braking/disconnect policy. Test interruptions, app backgrounding and browser tab suspension. Do not attempt an unlimited simulation catch-up on resume.

## 4 Networking correctness

Only authority changes finish order, traffic signals, penalties and shared obstacles. Validate sender ownership, input bounds, sequence age, message size and command rate. Clients cannot claim a final position or trusted race time.

Separate frequent movement snapshots from infrequent reliable events. Select delivery semantics according to the transport; WSS rides on reliable ordered delivery, so do not promise UDP-like loss behaviour for Web. Never put large payloads or assets in gameplay messages.

Use bounded input/history buffers, explicit protocol/content versions and stable network IDs. Reject incompatible room versions with a clear explanation. Handle duplicate, stale and late events without replaying rewards or penalties.

Interpolate remote visuals. If implementing local prediction, reconcile to authoritative snapshots with bounded correction/history, and separate cosmetic smoothing from collision state. Avoid multiple components independently writing the same transform.

Host advantage and cheating remain limitations of the initial host model. Host disconnect ends the race; no silent promotion to a replacement host. Destroy or disable disconnected-player collision state on authority. Log enough timing information to diagnose divergence without storing credentials or personal identifiers unnecessarily.

## 5 Performance budgets and measurement

These are **provisional engineering targets**, to be revised using measurements on a named lowest-supported device, not marketing promises.

| Area | Initial target or decision rule |
| --- | --- |
| Native rendering | Aim for 60 fps, approximately 16.7 ms per frame; CPU and GPU each need headroom |
| Web/mobile fallback | Validate 30 fps, approximately 33.3 ms, if 60 is not sustainable; do not change race speed or rules |
| Gameplay allocation | Aim for 0 B/frame steady-state allocations in our hot gameplay loops after warm-up; measure SDK/UI allocations separately |
| Simulation | Start at 50 Hz; profile and document changes instead of raising frequency to hide defects |
| Network snapshots | Start experiment at 20 Hz; tune from latency, correction quality and measured bandwidth |
| Soak test | At least 10 minutes and repeated race/replay cycles without steadily growing retained memory |
| Memory and Web download | Record measured peak memory and compressed initial payload on the first asset-bearing build; set numeric caps before asset expansion |
| Representative load | Record active traffic count, player count, effects and quality tier for every comparison |

A passing average fps is insufficient. Record frame-time distribution (including p95 and worst spikes), CPU/GPU bottlenecks, GC allocations, retained memory, bandwidth and visible correction events where applicable. Use Unity Profiler on target development builds to diagnose; confirm player experience on non-development builds. Editor numbers are not device results.

Avoid repeated scene searches, GetComponent calls, LINQ, string formatting, closures and temporary collections in measured hot loops. Cache references and reuse buffers where it helps. These operations are not universally forbidden in setup or tooling.

Pool frequently recycled traffic and effects, with capacity limits and complete reset logic. Profile pool memory; an unbounded pool is a leak. Reset network ownership, subscriptions and physics state when reusing networked objects.

Use simple colliders and a narrow collision-layer matrix. Schedule distant traffic decisions less often without skipping near-player safety checks. Cap active traffic and effects explicitly. Do not introduce jobs/ECS or custom allocators without a measured bottleneck and target-platform validation.

Reuse materials and limit shader variants, transparency, realtime lights and shadows. Profile draw calls and overdraw before pursuing batching changes. Set texture sizes/compression and audio import settings per target. Quality tiers may change rendering, not authoritative obstacles or collisions.

## 6 Cross-platform rules

One gameplay implementation serves all platforms. Confine platform compilation symbols to platform adapters, storage, transport and build configuration.

Translate touch and mouse into the same RideCommand model. Handle safe areas, resolution changes, pointer cancellation and UI-over-input priority. No gameplay depends on screen pixels; normalize drag distance and verify portrait layouts across aspect ratios.

Web must survive audio activation requirements, loss of focus, unavailable storage and failed downloads. Do not assume local saves persist forever. Keep browser C# execution compatible with Unity's documented limitations; do not use background threads as a universal fix. [Unity Web limitations](https://docs.unity3d.com/6000.3/Documentation/Manual/webgl-technical-overview.html).

Treat save data as untrusted: version it, bound values, recover from corrupt data, and migrate or reset safely. Native and browser storage use adapters. Store settings locally; authoritative results come from the current match authority.

Do not embed secrets in any client, including Web builds. Signing material and service credentials belong outside Git. Display recoverable service errors and allow offline riding when session services are unavailable.

## 7 Git, assets and dependencies

Commit .meta files with assets, preserve GUIDs when moving/renaming, and prefer Unity-aware moves. Use Force Text serialization. Never hand-edit scene/prefab YAML casually or regenerate all metadata to solve a merge.

Ignore Library, Temp, Obj, Logs, UserSettings, generated builds and local credentials. Track package manifest/lock, project settings, source assets and required metadata. Use LFS for large binary source assets; configure attributes before adding them. Do not put ordinary text code or .meta files in LFS.

Use English technical commit messages, for example feat(riding): add braking input. Keep changes scoped. Pin dependencies; explain additions and upgrades with purpose, license compatibility and target-platform evidence. Do not automatically upgrade packages during unrelated work.

For meaningful architecture changes, add a short bilingual decision record with context, decision, trade-offs, validation and superseded choice. Keep both language editions semantically aligned; do not translate code identifiers.

## 8 Verification and completion

| Change | Required relevant evidence |
| --- | --- |
| Race rules | EditMode cases for ordering, duplicate finish, unresolved penalty and invalid phase/input |
| Riding or collision | PlayMode coverage plus real-device/manual evidence at relevant frame rates |
| Multiplayer | Two clients and host, consistent results, disconnect/duplicate/late-event checks and mixed native/Web test |
| Rendering/assets/UI | Target screenshots and profiler evidence at representative load; Vietnamese text and safe areas |
| Platform/package/build | Affected platform builds; a successful Editor compile alone is insufficient |
| Documentation | Check local links, paired language content, decision consistency and git diff --check |

For the network experiment, start with a local baseline, then approximately 100 ms and 200 ms simulated RTT with recorded jitter. Test loss for datagram paths and stalls/disconnects on WSS. Label whether simulator values mean one-way delay or RTT. Observe steering response, correction size and finish agreement; do not call the stack validated merely because a room connects.

Test behaviour and boundaries, not private implementation details. Avoid trivial tests that repeat assignments. Add regression coverage for actual defects; do not repeatedly run unrelated suites after relevant checks pass.

Before delivery:

- Confirm requested scope, dependency direction and authority ownership.
- Review changed files, metadata, serialized references and package locks.
- Run relevant tests/builds that are available and inspect results.
- Record device/browser/editor/package versions and limitations for performance claims.
- Update English and Vietnamese docs when architecture or gameplay changes.
- Report unrun checks honestly. Do not fabricate commands, CI results or device access.

Until the Unity project and build scripts exist, only document checks are executable in this repository. Add reproducible setup and build commands when those entry points are implemented.
