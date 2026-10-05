# City Race Setup

Version 0.1 • 5 October 2026 • [Tiếng Việt](SETUP.vi.md)

## M0: the first technical milestone

M0 means Milestone 0: establish a repeatable path from source to a minimal scene running on real hardware. It is not the playable riding prototype (M1). Follow the [roadmap](ROADMAP.en.md) and [backlog](BACKLOG.en.md).

M0 exits only when the pinned URP project compiles, the smoke scene is visually verified on a physical iPhone, a hosted HTTPS Web build is inspected, and actual editor/package versions and results are recorded. Exporting an Xcode project alone does not prove an iPhone run.

## Local environment snapshot — 5 October 2026

| Item | Observed state |
| --- | --- |
| Mac architecture | Apple Silicon, arm64 |
| Repository | `/Volumes/T7-Workspace/Projects/Codex/city-race` |
| Legacy access path | `~/Workspace/Codex` is a symlink to `/Volumes/T7-Workspace/Projects/Codex` |
| Available storage | Internal about 21 GiB after installation; T7 shared APFS free space about 870 GiB; values change over time |
| Selected editor | Unity 6000.3.25f1, recorded in `.unity-version`; Apple Silicon editor and iOS/Web modules installed |
| Unity Hub | 3.22.2 installed through Homebrew at `/Applications/Unity Hub.app`; signed in, Personal license activation shown on 5 October 2026 |
| Xcode | 26.6, build 17F113 |
| iPhone | Previously paired iPhone 15 Pro Max; currently unavailable in `devicectl`; reconnect before testing |
| Signing | Login Keychain has one Apple Development identity reported expired (CSSMERR_TP_CERT_EXPIRED); zero valid identities |
| Git LFS | Not installed; needed before introducing large binary source assets, not for current text files |
| Project | Assets, Packages and ProjectSettings generated; Smoke verified in Editor Play Mode |

Do not commit device identifiers, account details, certificates or provisioning profiles. The user has authorised necessary M0 software installation. Account login, license eligibility and Apple signing still require the user's own credentials/choices; do not purchase services or guess a team.

## Installation and storage

1. Install [Unity Hub](https://docs.unity.com/en-us/hub/install-hub) from Unity. Install the Apple Silicon editor matching `.unity-version`, with iOS Build Support. [Selected release](https://unity.com/releases/editor/whats-new/6000.3.25f1).
2. Sign in and activate an appropriate Unity license; verify the editor opens. Do not silently substitute a different patch.
3. Add Web Build Support for CR-006 using [Hub modules](https://docs.unity.com/en-us/hub/add-modules). Android modules belong to M1.
4. Keep Unity/Xcode installations and global caches internal. Keep the project, project-local Library and Builds on T7. Check both disks during installation; downloads and extraction need temporary space beyond installed size. Do not relocate Docker, simulators, DerivedData or global caches.
5. Keep T7 connected while Unity/IDEs/builds use it. Close them before ejecting. A symlink does not provide an offline copy or automatic backup.

## Bootstrap commands — init and scene verified

Run from the repository root. `preflight` is read-only and returns nonzero if the editor is absent or project-volume free space is below 10 GiB. It is not a full installation-size or signing check.

```sh
python3 tools/unity_project.py preflight
python3 tools/unity_project.py init
python3 tools/unity_project.py scene
python3 tools/unity_project.py export-ios
python3 tools/unity_project.py build-web
```

Run commands one at a time and inspect each result. Close the same project in the Editor before batch operations. The default executable is `/Applications/Unity/Hub/Editor/6000.3.25f1/Unity.app/Contents/MacOS/Unity`; pass `--editor` for an explicitly verified alternative.

`init` requires the official URP blank template. If discovery fails, obtain it through Hub and pass `--template /absolute/path/to/com.unity.template.urp-blank-VERSION.tgz`. Do not fabricate a template or package lock. The script refuses existing project directories and retains `.bootstrap-work/project` on failures for inspection. Do not blindly delete partial work or rerun initialization over it.

Unity generates real settings/packages/metadata in staging, then the script brings them into the repo and copies `tools/unity/CityRaceBootstrap.cs` into `Assets/CityRace/Editor`. After initialization, keep the bootstrap source and its generated editor copy aligned if editing. Inspect compilation, URP materials/camera and `Assets/CityRace/Content/Scenes/Smoke.unity`. The smoke scene contains static road/building/bike markers, not driving logic.

Commit generated source assets with `.meta`, ProjectSettings, manifest and package lock after verification. Logs, Library, Builds and staging remain ignored. Logs are in `Logs/`; iOS output is `Builds/iOS`, Web output `Builds/Web`. Review existing build outputs before rebuilding, since build commands can update them.

## Real-device and browser validation

Open the exported Xcode project, choose the user's signing team and confirm the temporary bundle ID `com.aletuan.cityrace.dev`. Connect/unlock the iPhone, confirm trust and Developer Mode, then build and run. Record device model/OS, editor version, commit, build type, launch/render outcome and any errors; keep credentials private. A successful compile is not visual validation.

For Web, serve the generated build through HTTPS with correct compression headers and inspect loading, rendering and console errors in a named browser/version. Hosting and publishing destinations must be explicit; no paid service is required by this document.

## Current verification

Python syntax, CLI help, missing-editor guard, local Markdown links and whitespace are checked during step 1. Unity initialization, template compatibility, compilation and scene generation now pass. iOS export, signing, device execution and Web build remain unverified. Update this section and the backlog with real evidence, not planned outcomes.

## Signing follow-up — 5 October 2026

The existing City Crew project uses Expo/EAS, has a production submission configuration, and its README records an App Store release. Its build profiles do not override the default credentials source. Remote signing credentials may therefore be managed by EAS; their validity was not inspected. An App Store release does not establish a currently usable local development identity. For City Race, verify the existing Apple Developer team in Xcode and configure automatic development signing for its own bundle ID. Do not revoke or replace City Crew credentials.

## Installation verification — 5 October 2026

Unity Hub 3.22.2 installed via Homebrew. Hub CLI completed installation of 6000.3.25f1 (revision e1dba0a9aba4), ARM64, with `ios` and `webgl`, reporting all tasks successful. The Editor executable returned `6000.3.25f1` with exit code 0 after the first-run terms screen. Hub displays an active Personal license. Preflight now passes. Modules are in the installation root’s `PlaybackEngines/iOSSupport` and `PlaybackEngines/WebGLSupport`, alongside `Unity.app`.

Follow-up inspection found the bundled URP template under a different archive filename; see the bootstrap evidence below. Device/browser builds have not run. Git LFS remains absent; do not add large binary source assets yet.

## Bootstrap evidence — 5 October 2026

CR-003 and CR-004 passed on this Apple Silicon Mac with Unity 6000.3.25f1 (Metal). The bundled archive `com.unity.template.3d-cross-platform-17.0.14.tgz` identifies itself internally as `com.unity.template.urp-blank` version 17.0.14. Tooling now checks package metadata rather than assuming a filename. No template download or manual package-lock construction was needed.

`python3 tools/unity_project.py init` completed with exit code 0. Unity generated settings, metadata and a resolved lock: URP 17.3.0, Input System 1.20.0, uGUI 2.0.0 and Test Framework 1.6.0. The template's other packages are retained; multiplayer gameplay packages have not been added. Initialization migrates template package versions through Unity itself.

`Smoke.unity` contains the road, sidewalks, buildings, markings, static bike/rider, orthographic camera and light. It is the enabled build scene. iOS uses provisional ID `com.aletuan.cityrace.dev`, IL2CPP, portrait and iPhone target. The script source and Assets editor copy match. Unity's first interactive launch upgraded template materials. Play Mode was visually inspected: colored geometry rendered, no missing-shader pink materials. Screenshot: ignored local `Logs/smoke-editor-play.png`. This is an Editor check in Free Aspect, not iPhone portrait validation or a performance benchmark.

Logs contain Unity service connectivity errors and a startup access-token warning; these did not prevent initialization or Play Mode. No C# compilation errors or script exceptions were observed. Do not infer network-service readiness from this test. Next: close the Editor before batch export, configure the existing Apple team, and validate on a connected iPhone.
