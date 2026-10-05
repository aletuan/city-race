# City Race — Claude Code handoff

Reviewed 6 October 2026 (Asia/Ho_Chi_Minh). This file captures the conversation decisions and repository state for continuation. Read [AGENTS.md](AGENTS.md) as well. Follow the user's latest instructions over this snapshot. This handoff is documentation; no bus implementation or new device test was performed while preparing it.

## Start here

- Project: **City Race**, a minimalist motorbike game set in believable Sài Gòn rush-hour traffic.
- Real checkout: `/Volumes/T7-Workspace/Projects/Codex/city-race` on the Samsung T7.
- Legacy access: `/Users/andy/Workspace/Codex/city-race` reaches the SSD through a symlink. Use the real checkout; do not create a second project.
- Remote: `https://github.com/aletuan/city-race.git`; branch: `main`.
- Gameplay baseline: **`908668f`**, application version **0.0.4**. Working tree was clean and local HEAD matched the cached `origin/main` before this handoff was added. Recheck Git state before editing.
- Active build scene: [Practice.unity](Assets/CityRace/Content/Scenes/Practice.unity). Smoke and Riding are historical scenes, not the current build target.
- **M0 is complete. M1 is in progress.** Potholes exist; the bus is the next feature. Do not mark M1 complete based on tests/builds alone.
- Last device blocker: 0.0.4 installed successfully, but launch was explicitly refused because the iPhone was locked. No later unlock or visual acceptance is recorded. Recheck; do not assume it is still locked today.

Recommended first action: inspect Git state and this file's linked sources, then ask the user to unlock/connect the iPhone if needed for the pending gameplay check. Continue independent bus preparation if the device is unavailable; report manual validation separately.

## Product decisions to preserve

The user wants direct one-finger riding, simple visuals with believable Vietnamese streets/vehicles, and eventual simultaneous races for friends. Start with Sài Gòn, iOS first; support Android and validate Web early through separate builds sharing gameplay code.

The eventual small level has 2–4 players leaving individual homes in one neighbourhood, merging into traffic and racing to work. Minimal character/clothing and familiar motorbike choices come later. Office commutes are first; delivery, evening trips, carrying a partner, rain and punctures are later scenarios. Believable setbacks should have readable causes and responses, not arbitrary punishment.

The user approved this order:

1. A small course with left/right turns, a narrow passage and a temporary destination.
2. Handling adjustments and stuck recovery.
3. Potholes.
4. A bus that signals before pulling toward the kerb.

Items 1–3 are implemented, with manual acceptance still open. This is still an offline greybox, not the full neighbourhood, traffic simulation or multiplayer game. The course's current home/company blocks are placeholders.

## Evidence and remaining acceptance

| Area | Verified evidence | Still open |
| --- | --- | --- |
| M0 | Pinned URP project; signed physical iPhone smoke render; user-supplied HTTPS Chrome screenshot | Web console/version/performance were not verified |
| 0.0.2 | One-finger riding implementation, six PlayMode tests, signed iPhone run and screenshot | Physical handling quality and frame-rate comparison |
| 0.0.3 | Four-corner course, narrow passage, ordered finish, recovery and Restart; eleven tests; iPhone build/install/launch | Device screenshot showed another app, so no visual acceptance for that version |
| 0.0.4 | Two potholes; **16/16 PlayMode tests passed**; Unity export, signed Xcode Debug build, strict signature verification and iPhone installation | Launch blocked by lock screen; native pothole visuals/feel unverified |
| Android / current Web | No current riding/hazard build validation recorded | Modules/device for Android; current Web build and browser testing |
| Performance / playtest | No representative target-device measurements or group playtest | 30/60 fps comparison, allocations/memory, soak, 5–8 participants |

Last test result was re-read during this review: ignored local `Logs/pothole-tests.xml`, result Passed, 16 tests, 0 failures, run on 5 October 2026. This review did **not** rerun tests. Local logs/screenshots are not included in a fresh Git clone.

Recent implementation commits:

- `d25bd14`: one-finger riding and regression tests.
- `50fb5d1`: practice corners, finish and hold recovery.
- `908668f`: visible potholes and temporary speed response.

## Current gameplay and tuning

All values below are prototype settings, not validated final handling.

| System | Current behaviour |
| --- | --- |
| Input | Start a relative drag in the lower 65% of the safe area. Radius: 18% of the shorter safe-area dimension; dead zone: 12%. Direction sets desired heading; distance sets throttle. |
| Cancellation | Release brakes. Focus loss, pause, resize and disable cancel input. Lifting the controlling finger while another remains does not transfer throttle; release all fingers to rearm. |
| Motor | Rigidbody constrained to the road plane; fixed simulation 50 Hz. Max 8 m/s, acceleration 5 m/s², braking 16 m/s², turn cap 165°/s reduced at low speed. Forward arcs, no reverse button or in-place turning. |
| Corners | Target throttle progressively decreases for heading errors between 20° and 100°, down to 40%. |
| Course | About 107 m of centerline, four corners, a narrow section and ordered checkpoints. Follow yellow guides; stop in the yellow company square below 0.4 m/s for 0.7 s. |
| Recovery | Release, then hold still for 1.2 s while stopped. Return to an earlier passed checkpoint, add 2 s, zero motion and require release before driving again. Route projection limits forward shortcuts during backtracking; start is the lower bound. |
| Replay | Restart button resets the run without reopening the app. Finish disables the motor until Restart. |
| Camera | Fixed north orientation, 58° overhead, orthographic size 12; follows in LateUpdate and snaps after large resets. |
| Potholes | Fixed X/Z positions (0, 8) and (18.7, 34). Dark depression, broken brown rim and amber approach marks; room to pass beside them. |
| Pothole response | At or below 3 m/s: no penalty. Faster entry retains 60% speed and caps acceleration at that speed for 0.45 s. Steering/braking remain available. Cosmetic wobble and 1.2 s message; no extra time penalty. |
| Protection | Pothole immunity 1.2 s; substantial side-wall impact protection 0.6 s; reset protection 0.5 s. Ground contact is not a crash. Reset clears feedback/hit count. |

## Code map

All paths below are relative to this checkout.

| Path | Responsibility |
| --- | --- |
| `Assets/CityRace/Runtime/Core/PracticeProgress.cs` | Plain-C# ordered checkpoints, continuous stop-to-finish, elapsed time and recovery penalty |
| `Assets/CityRace/Runtime/Core/PotholeResponse.cs` | Plain-C# speed threshold and impact constants |
| `Assets/CityRace/Runtime/Gameplay/Riding/DragRideInput.cs` | Input System mouse/touch adapter and cancellation |
| `Assets/CityRace/Runtime/Gameplay/Riding/BikeMotor.cs` | Fixed-step movement, reset, corner easing and temporary impact response |
| `Assets/CityRace/Runtime/Gameplay/Riding/PracticeCourse.cs` | Scene-to-rules adapter, recovery gesture and restart |
| `Assets/CityRace/Runtime/Gameplay/Riding/Pothole.cs` | Trigger entry; forwards contact to the motor |
| `Assets/CityRace/Runtime/Presentation/` | Camera, safe-area layout, practice HUD and cosmetic bike wobble |
| `Assets/CityRace/Tests/PlayMode/` | RidingTests, PracticeTests and PotholeTests; actual course completion and UI pointer replay included |
| `Assets/CityRace/Editor/` | Smoke/riding/practice generators and one-time pothole migration |
| `tools/unity_project.py` | Pinned-editor preflight, scene actions, iOS export and Web build |
| `tools/unity/CityRaceBootstrap.cs` | Source copy of `Assets/CityRace/Editor/CityRaceBootstrap.cs`; keep both aligned when changing it |

Runtime currently uses one `CityRace.Runtime` assembly; Core classes are plain C# but not yet a separate assembly. Do not invent an elaborate architecture for one bus. `BikeMotor.SetCommand` enables a manual override used by tests; `ResetPose` clears it. Do not accidentally leave live input overridden.

## Next work, in order

### A. Close the outstanding device check

- Reconnect/unlock the iPhone; open installed City Race 0.0.4.
- Check road/hazard visibility, turns, narrow passage and stopping at the destination.
- Try avoiding a pothole, crossing slowly and hitting it fast. Confirm the cause is obvious and steering remains available.
- Check recovery, Restart, second-finger release and background/resume.
- Record actual feedback and defects. Fix blocking control/readability issues before expanding traffic.
- Do not label screenshots as proof of fps, responsiveness or a successful group playtest.

### B. Implement the bus part of CR-012

This is the **next proposed implementation plan**, not an already implemented feature or a new approval to publish services.

1. Place one recognisable simple bus on a wide stretch, away from the narrow passage and potholes. Preserve a usable passing/waiting option.
2. Use explicit behaviour phases: driving → signalling → gradual pull-in → stopped → departure. Choose and document provisional durations/speeds during implementation; none were agreed in the conversation.
3. Show a visible indicator before lateral movement. No sudden spawning in front of the bike, teleporting or unannounced lane changes.
4. Keep motion and collisions consistent with the fixed simulation step. Avoid crushing the bike against a kerb or producing permanent gridlock. Do not assume a moving kinematic body gives fair collision behaviour without tests.
5. Keep presentation separate from traffic state. Restart must reset bus position, phase, indicators and timers; recovery must not place the bike inside the bus. Define an occupied-recovery-position fallback that never advances progress.
6. Add meaningful tests: signal precedes pull-in, bounded motion, no passing through the bus, escape/waiting behaviour, replay reset and interaction with pothole protection/recovery. Retain the existing regression suite.
7. Build/install on iPhone and inspect actual behaviour. Update docs and backlog with evidence and limitations, then commit/push within the user's existing workflow.

Do not start full-city pathfinding, a traffic framework, multiplayer, police, rain or monetisation as part of this increment.

### C. Finish M1 acceptance before M2

- CR-010/011: implementation exists; physical handling, lifecycle and frame-rate evidence remain open.
- CR-012: potholes implemented; bus, obstacle reactions and physical acceptance remain open.
- CR-013: camera/safe-area scaffolding exists, but readability, finger occlusion and aspect-ratio tuning are not accepted.
- CR-014: Android toolchain/device check and real-device smoke test remain open.
- CR-015: 5–8-person playtest and device performance baseline remain open. Record actual sessions, not hypothetical reports.
- Then M2 / CR-020: two-player authority, mixed native/Web clients, latency and disconnect experiments; expand to four only after evidence.
- CR-021: measure Relay bytes per player-hour, including host traffic. Do not reuse unverified earlier pricing assumptions.

## Toolchain and repeatable commands

Pinned Unity: **6000.3.25f1**, Apple Silicon. Installed support: iOS and Web. Last verified Xcode: **26.6 (17F113)**; physical phone: iPhone 15 Pro Max, iOS 26.3 beta. Recheck device/tool availability today.

Packages verified from the manifest: URP 17.3.0, Input System 1.20.0, uGUI 2.0.0, Test Framework 1.6.0. The template includes Multiplayer Center, but NGO/Transport/Multiplayer Services gameplay integration has **not** been installed or implemented. The future candidate is NGO + Unity Transport + Sessions/Relay with anonymous authentication and private rooms.

Close the interactive Unity Editor for this project before batch runs. Do not force-close unsaved work blindly.

```sh
cd /Volumes/T7-Workspace/Projects/Codex/city-race
git status --short
python3 tools/unity_project.py preflight

"/Applications/Unity/Hub/Editor/6000.3.25f1/Unity.app/Contents/MacOS/Unity" \
  -batchmode -projectPath "$PWD" \
  -runTests -testPlatform PlayMode \
  -testResults Logs/continuation-tests.xml -logFile Logs/continuation-tests.log

python3 tools/unity_project.py export-ios
```

Run commands sequentially and inspect each exit/result. **Do not add `-quit` to the test command**; the test runner exits itself. Builds use enabled Build Settings scenes, currently Practice. Never rerun `init`, `scene`, `riding`, `practice` or `potholes` to resume this checkout: the project/scenes already exist and the migration has already run. These actions deliberately refuse duplicates.

For signing, inspect the existing Xcode account/team and `xcrun devicectl list devices`. Do not guess or commit team/device identifiers. Set the following session variables locally from verified values before using these commands:

```sh
: "${CITYRACE_TEAM_ID:?Set the verified existing Apple team ID locally}"
: "${CITYRACE_DEVICE_ID:?Set the connected devicectl device ID locally}"

xcodebuild -project Builds/iOS/Unity-iPhone.xcodeproj \
  -scheme Unity-iPhone -configuration Debug -destination 'generic/platform=iOS' \
  -derivedDataPath "$HOME/Library/Developer/Xcode/DerivedData/CityRace-M0" \
  -allowProvisioningUpdates -allowProvisioningDeviceRegistration \
  "DEVELOPMENT_TEAM=$CITYRACE_TEAM_ID" CODE_SIGN_STYLE=Automatic build

CITYRACE_APP_PATH="$HOME/Library/Developer/Xcode/DerivedData/CityRace-M0/Build/Products/Debug-iphoneos/CityRace.app"
codesign --verify --deep --strict "$CITYRACE_APP_PATH"
xcrun devicectl device install app --device "$CITYRACE_DEVICE_ID" "$CITYRACE_APP_PATH"
xcrun devicectl device process launch --device "$CITYRACE_DEVICE_ID" com.aletuan.cityrace.dev
```

The `CityRace-M0` DerivedData name is historical and still used for subsequent builds. Bundle ID is `com.aletuan.cityrace.dev`; minimum iOS is provisionally 15.0. Previous Apple-team retrieval failed until the user signed out/in to Xcode. Device lock caused DDI/launch failures; do not misdiagnose these as incompatible iOS or revoke certificates. City Crew is a separate released project: leave its credentials untouched.

Web, when needed: `python3 tools/unity_project.py build-web`, then `python3 tools/serve_web.py` serves `Builds/Web` on loopback port 8765. The earlier temporary Cloudflare tunnel/server were stopped; the old URL is not a current preview. The last Web evidence is the static M0 scene, not 0.0.4. An automated browser tool previously encountered an admin-policy verification denial: do not bypass it. Use permitted testing or user-provided evidence and record the limitation.

## Engineering and workspace constraints

- Keep gameplay rules in plain C#, Unity/input/networking behind small boundaries, and visuals separate from collision/simulation. No global all-purpose GameManager.
- Preserve Unity-generated `.meta` files/GUIDs. Use Unity Editor tooling to modify scenes; do not casually hand-edit scene YAML or regenerate the project.
- Pin package/editor versions. No dependency upgrades or large binary assets without a concrete need. Git LFS was absent at the last setup check; verify/configure before large binary source assets.
- Existing HUD temporarily uses uGUI Text with a built-in font and direct English/Vietnamese strings. Keyed localization, TextMeshPro and Vietnamese glyph QA are known unfinished work; do not claim them complete.
- Core tests, synthetic mouse/touch tests and the automated route driver do not replace actual touch playtesting. Profiling targets are provisional: aim for 60 fps native, validate a 30 fps fallback, fixed simulation 50 Hz, and measure GC/memory/soak on real hardware.
- Repo, Unity Library and Builds are on T7; Xcode DerivedData, tools, simulators, Docker data and global caches stay internal. On 6 October the read-only disk check showed about 13 GiB internal and 862 GiB shared T7 free; recheck before large builds/installations.
- Keep T7 connected during development. Do not modify T7-Photos, Photos/iCloud configuration or unrelated Workspace folders. Never delete the retained original internal Photos Library.
- The user prefers Vietnamese conversation, concise progress updates, and autonomous reversible work without repetitive approval questions. Existing workflow includes commit/push to main. Ask for real blockers such as unlocking, credentials or destructive operations; do not treat this handoff as permission to erase storage, install unrelated software, purchase services, publish to the App Store or contact others.
- This handoff does not depend on Codex-specific tools being available in Claude Code. Use available CLI/editor tools; report unavailable UI/device access honestly.

## Source documents and stale sections

Read these for design/conventions, but distinguish history from current state:

1. [GDD](docs/GDD.md).
2. [Technical stack](docs/TECH_STACK.md).
3. [Engineering guidelines](docs/ENGINEERING_GUIDELINES.md).
4. [Current prototype and evidence](docs/RIDING_PROTOTYPE.md).
5. [Backlog](docs/BACKLOG.md), [roadmap](docs/ROADMAP.md), [setup chronology](docs/SETUP.md).

Review findings to reconcile during the next documentation update:

- ROADMAP still labels M1 Planned; implementation is in progress.
- BACKLOG CR-010/011 retains the earlier eleven-test count; the current combined suite has sixteen. CR-013 says Todo despite camera/safe-area scaffolding; its acceptance remains incomplete.
- SETUP contains chronological statements about expired signing, unverified builds and Smoke being active that were superseded by later sections and current code. Do not repeat those as today's status.
- TECH_STACK still contains pre-bootstrap wording such as no Unity project/exact packages unset. Use actual manifest/lock and ProjectSettings for installed versions.
- RIDING_PROTOTYPE contains historical 0.0.2/0.0.3 sections, including “no potholes”; its top 0.0.4 section supersedes them.

Do not silently rewrite old evidence into claims of new tests. Documentation is English only (single source of truth since 6 October 2026); Vietnamese .vi.md editions were removed and must not be recreated.

## Suggested first continuation prompt

> Read CLAUDE.md and AGENTS.md, verify the current checkout, and resume City Race from the 0.0.4 pothole baseline. First close the pending iPhone validation if the device is available, fixing blocking handling/readability defects. Then implement one bus that visibly signals before gradually pulling into a stop, preserving collision fairness, recovery and replay. Keep scope within M1, run relevant tests/builds, record actual evidence in the docs and commit/push. Do not start multiplayer or publish a release yet.
