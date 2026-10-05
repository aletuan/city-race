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
| Available storage | Internal about 38 GiB; T7 shared APFS free space about 870 GiB; values change over time |
| Selected editor | Unity 6000.3.25f1, recorded in `.unity-version`; not installed |
| Unity Hub | Not found in standard system/user Applications locations |
| Xcode | 26.6, build 17F113 |
| iPhone | Previously paired iPhone 15 Pro Max; currently unavailable in `devicectl`; reconnect before testing |
| Signing | Login Keychain has one Apple Development identity reported expired (CSSMERR_TP_CERT_EXPIRED); zero valid identities |
| Git LFS | Not installed; needed before introducing large binary source assets, not for current text files |
| Project | No generated Assets, Packages or ProjectSettings yet |

Do not commit device identifiers, account details, certificates or provisioning profiles. The user has authorised necessary M0 software installation. Account login, license eligibility and Apple signing still require the user's own credentials/choices; do not purchase services or guess a team.

## Installation and storage

1. Install [Unity Hub](https://docs.unity.com/en-us/hub/install-hub) from Unity. Install the Apple Silicon editor matching `.unity-version`, with iOS Build Support. [Selected release](https://unity.com/releases/editor/whats-new/6000.3.25f1).
2. Sign in and activate an appropriate Unity license; verify the editor opens. Do not silently substitute a different patch.
3. Add Web Build Support for CR-006 using [Hub modules](https://docs.unity.com/en-us/hub/add-modules). Android modules belong to M1.
4. Keep Unity/Xcode installations and global caches internal. Keep the project, project-local Library and Builds on T7. Check both disks during installation; downloads and extraction need temporary space beyond installed size. Do not relocate Docker, simulators, DerivedData or global caches.
5. Keep T7 connected while Unity/IDEs/builds use it. Close them before ejecting. A symlink does not provide an offline copy or automatic backup.

## Bootstrap commands — prepared, not yet Unity-validated

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

Python syntax, CLI help, missing-editor guard, local Markdown links and whitespace are checked during step 1. Unity compilation, template compatibility, generated scene, iOS export, signing, device execution and Web build remain unverified until their respective tasks run. Update this section and the backlog with real evidence, not planned outcomes.

## Signing follow-up — 5 October 2026

The existing City Crew project uses Expo/EAS, has a production submission configuration, and its README records an App Store release. Its build profiles do not override the default credentials source. Remote signing credentials may therefore be managed by EAS; their validity was not inspected. An App Store release does not establish a currently usable local development identity. For City Race, verify the existing Apple Developer team in Xcode and configure automatic development signing for its own bundle ID. Do not revoke or replace City Crew credentials.
