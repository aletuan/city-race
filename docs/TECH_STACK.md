# City Race Technical Stack

Version 0.1 • 3 October 2026

## 1 Decisions and status

**Confirmed direction:** Unity + C#, iOS first, with Android and Web compatibility considered from the start. Share gameplay code and content; create and validate separate platform builds.

**Implementation defaults:** the choices below are the starting architecture, not evidence of a working integration. The repository has no Unity project yet. The exact editor patch, package versions, minimum OS/browser/device versions, hosting budget, and performance measurements remain unset.

| Area | Starting choice | Reason or constraint |
| --- | --- | --- |
| Engine | Unity 6.3 LTS | Stable baseline for this project; pin a tested patch at bootstrap |
| Language | C# supported by the pinned Unity editor | Avoid unsupported language/runtime features and unnecessary external dependencies |
| Rendering | Universal Render Pipeline (URP), simple 3D assets, overhead camera | Shared mobile/web scene; modest lighting and effects |
| Gameplay | GameObjects with small MonoBehaviour adapters and plain C# rules | Appropriate starting complexity for a small race |
| Controls | Unity Input System | Touch plus mouse for browser/desktop testing, using the same command model |
| UI | Unity UI (uGUI) with TextMeshPro | Small menu/HUD scope; font assets must include Vietnamese glyphs |
| Networking candidate | Netcode for GameObjects (NGO) + Unity Transport | Must pass the movement and collision experiment |
| Session services | Unified Multiplayer Services SDK, Sessions and Relay | Private room creation and joining; avoid mixing duplicate session lifecycle implementations |
| Identity | Unity Authentication anonymous sign-in for online rooms | No visible account registration; offline riding does not require services |
| Local preferences | Small versioned local save through a storage adapter | Character, bike, audio, and graphics choices; not authoritative results |
| Testing | Unity Test Framework, EditMode and PlayMode; device/browser checks | Rule tests plus actual integration evidence |
| Source control | GitHub; Git LFS for large binary source assets when added | Commit code, content metadata, settings, and package locks |

Unity documents 6.3 as an LTS line and supplies the rendering/mobile toolchain. [Unity release guidance](https://unity.com/blog/unity-6-3-lts-is-now-available), [Unity feature sets](https://docs.unity.com/en-us/engine/6000.6/manual/packages-list/feature-sets).

## 2 Platform contract

| Target | Commitment | Initial evidence required |
| --- | --- | --- |
| iOS app | Primary shipping target | Real iPhone build through Xcode; touch, safe area, performance and lifecycle checks |
| Android app | Planned expansion; verify portability early | Physical Android device build, controls and shared-session test |
| Desktop Web | Early prototype and compatibility target; release timing undecided | HTTPS-hosted build, mouse controls, load behaviour and native/web shared session |
| Mobile Web | Early feasibility target, not blanket device support | Safari on iPhone and Chrome on Android; touch, memory, audio unlock and background behaviour |

Unity Web runs in supported browsers using browser graphics and WebAssembly capabilities. Browser support is not a performance guarantee. Choose a concrete test device/browser matrix before claiming support. [Unity browser compatibility](https://docs.unity.com/en-us/engine/6000.3/manual/platform-specific/webgl/intro/browsercompatibility).

Use a macOS/Xcode build path for iOS and the editor-compatible Android toolchain for Android. Pin the actual toolchain in setup documentation when the project exists. Web deployment needs HTTPS, correct WebAssembly and compressed-file headers, and a loading/error screen; build completion alone does not verify hosting.

Keep native-only APIs behind adapters. Browser storage is subject to browser limits and clearing; preferences must recover to defaults. Do not assume arbitrary filesystem access, background execution, managed threads, or dynamic code generation across all targets. [Unity Web limitations](https://docs.unity3d.com/6000.3/Documentation/Manual/webgl-technical-overview.html).

## 3 Runtime boundaries

Use four small dependency boundaries, created only as implementation needs them:

- **Core:** race state, finish validation, penalties, settings schemas, IDs and commands. No UnityEngine, NGO, service SDK or UI dependencies.
- **Gameplay:** input translation, fixed-step movement, traffic and collision adapters; references Core and Unity.
- **Infrastructure:** network, session, storage and platform implementations; references Core contracts and the relevant SDKs.
- **Presentation:** camera, models, animation, audio, HUD and menus; observes state and submits commands.

A Bootstrap composition root wires concrete dependencies. Plain C# services use constructor injection; MonoBehaviour adapters use explicit serialized references or initialization. Assembly definitions enforce boundaries when those modules exist; do not create empty assemblies or an elaborate dependency injection framework in advance.

Typical flow: pointer input → RideCommand → authoritative movement/race processing → state snapshot/events → visuals and HUD. Local visual response and prediction must not grant the client authority over finishes or penalties.

Use ScriptableObjects for authored tuning data, copied into per-race state as needed. Never store mutable live race state in shared asset instances.

## 4 Multiplayer experiment

Start with an iOS or desktop native player as host, plus clients through Relay. The host runs authoritative traffic, race timing, collisions and results. Relay forwards traffic; it does not run the simulation or make a player-hosted match cheat-proof.

Browser clients use secure WebSockets through compatible Unity Transport. Native clients can use a supported Relay transport. Validate the chosen combination with the pinned packages. Browser hosting is not part of the initial acceptance scope and must not be assumed to work through direct inbound sockets. [Unity Relay and NGO integration](https://docs.unity.com/en-us/mps-sdk/tutorials/relay-and-ngo).

Use the Sessions lifecycle as the primary integration path; lower-level Relay examples are reference material, not a second room-management system. Anonymous authentication satisfies “no account required” at the UI level but creates a service identity; a cleared local/browser profile may lose that identity.

Clients send bounded inputs with sequence/tick information. Authority validates ownership, input ranges, rate and race phase. Replicate authoritative state and important discrete events; interpolate remote visuals. Do not send every decorative object's transform each rendered frame.

**Validation gate:** NGO anticipation is not a complete rollback/replay prediction system. Prove responsive steering and acceptable corrections on real devices before expanding multiplayer content. If needed, prototype a bounded movement predictor with authoritative correction; evaluate another networking solution if that becomes disproportionate. Record any replacement as an architecture decision rather than silently introducing a second stack. [NGO anticipation documentation](https://docs.unity3d.com/Packages/com.unity.netcode.gameobjects@2.7/manual/advanced-topics/client-anticipation.html).

Initial lifecycle policy: lock joining after countdown; host loss ends the race with a clear message; a disconnected client is removed from collision participation and marked disconnected. Rejoining an active race, host migration, dedicated servers, public matchmaking, ranked play and anti-cheat guarantees are outside the first slice.

## 5 Movement and content

Start with a constrained arcade bike motor on the road plane, simple collision shapes and authored lane/route graphs. Unity handles collision queries; race rules do not depend on cosmetic lean or wheel animation. Keep rendering smooth independently of the simulation tick.

Do not begin with realistic suspension, WheelCollider tuning, full-city pathfinding, or an entity-component-system rewrite. Add complexity only when a measured gameplay need justifies it. Believable traffic behaviour is a design requirement; physically exact motorcycle simulation is not.

Author simple 3D geometry, shared materials and limited lighting in URP. Camera tilt and orthographic versus perspective projection remain testable choices. Preserve collision readability and recognisable Sài Gòn details as assets replace greyboxes.

## 6 Versioning and project setup

At bootstrap, select a stable 6.3 patch and mutually compatible package versions through Package Manager. Record the editor in ProjectSettings/ProjectVersion.txt and dependencies in Packages/manifest.json and Packages/packages-lock.json. Test upgrades on a branch and include affected platform builds.

Use these package families, resolving exact versions at bootstrap: URP, Input System, uGUI/TextMeshPro as supported by the editor, NGO, Unity Transport, Multiplayer Services, Authentication and Test Framework. Add profiling/network simulation tools only when used. No exact package version is claimed installed by this document.

Project lives at the repository root: Assets/, Packages/, ProjectSettings/ alongside docs/ and AGENTS.md. Use visible metadata and text asset serialization. Configure Unity-specific ignore rules and LFS before adding large binary assets; always keep .meta files with their assets.

## 7 Implementation sequence and exit evidence

1. Bootstrap: pin versions, create the minimal Unity project, document build prerequisites, verify an iPhone build and a hosted Web build.
2. Riding: one bike, a road, pothole and bus; validate touch/mouse controls and collect a performance baseline. Try an Android build during this phase.
3. Multiplayer: two devices, then mixed native/Web clients; test network delay, disconnects and consistent finish results.
4. First slice: private 2–4 player races, four homes, morning commute and the scoped GDD events. Repeat device performance checks with representative traffic.

A milestone is complete only with recorded build/device evidence, not because code compiles in the Editor. No cloud service deployment, CI workflow, or game implementation is performed by this document.

## 8 Unresolved choices

Minimum supported devices/OS versions; exact package pins; traffic density and network tick budgets; hosting costs; final movement prediction strategy; browser-host support if later requested; and whether Web ships as a demo or full game.

Resolve these through focused experiments and dated architecture decisions under docs/decisions/. Consult [engineering conventions](ENGINEERING_GUIDELINES.md) for implementation and validation rules.
