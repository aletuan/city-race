# Riding prototype — M1, first increment

5 October 2026 • [Tiếng Việt](RIDING_PROTOTYPE.vi.md)

M0 established the build pipeline. This increment begins CR-010/011: one bike on a bounded 120 m practice road, using the same motor for mouse and touch. M1 is not complete.

## Try it

Open `Assets/CityRace/Content/Scenes/Riding.unity` and enter Play Mode, or run the iOS development build (0.0.2). Press in the lower 65% of the safe area, then drag towards the desired direction. Drag farther for more throttle; release to brake. A new press starts at zero throttle. Screen up means world north; the camera follows without rotating with the bike.

The road has solid kerbs and end boundaries. There is no recovery gesture yet: restart the app/scene if wedged against a boundary. Potholes, buses, race timing, destination and multiplayer are later increments.

## Implementation and tuning

- `DragRideInput`: Input System adapter; radius is 18% of the shorter safe-area dimension, dead zone 12%. Cancels on release, focus loss, pause, resize or disable. A second finger cannot inherit throttle; all touches must release after cancellation.
- `BikeMotor`: Rigidbody on the road plane, fixed 50 Hz simulation; maximum 8 m/s, acceleration 5 m/s², braking 16 m/s², maximum steering 135°/s with reduced steering at low speed. No in-place rotation or reverse gear.
- `RidingCamera`: interpolated follow in LateUpdate, fixed 58° overhead angle and orthographic size 10. Camera and handling values remain playtest hypotheses.
- `RidingHint`: safe-area hint in English/Vietnamese according to system language. The temporary hint uses built-in uGUI Text to avoid importing font assets in this increment; replace with keyed TextMeshPro localization and verify Vietnamese glyphs before HUD expansion.
- Runtime assembly depends on Input System and uGUI. No network/service dependencies added. Tests live in a separate test assembly and do not ship in normal builds.
- Build tooling now builds enabled Build Settings scenes; Smoke is retained, Riding is enabled. The generator refuses to overwrite an existing Riding scene.

## Verified and pending

Six Editor PlayMode tests passed on Unity 6000.3.25f1: acceleration/speed cap/braking, bounded steering arc, solid-wall collision, mouse drag/release/cancel/rearm, normalized touch/release, and controlling-finger release with another finger held. Tests use synthetic Input System devices; they are not physical touch evidence. Test-only focus settings are restored after each test. Results: ignored `Logs/riding-tests.xml`.

iOS 0.0.2: Unity export, Xcode 26.6 Debug build, automatic signing, strict code-sign verification, install and launch passed on iPhone 15 Pro Max (iOS 26.3 beta). Xcode device screenshot confirms the road, bike and English hint render; local evidence is `Logs/riding-iphone.png`. This screenshot does not establish handling quality or fps.

Physical touch feel, application background/resume, 30/60 fps comparison, portrait aspect ratios, Vietnamese glyphs and device profiling still need manual validation. Android and Web builds of the riding increment remain unverified. M0 Web evidence applies only to the earlier static scene.

## Run tests

Close the interactive Editor for this project first. Invoke the pinned Unity executable with:

```sh
"/Applications/Unity/Hub/Editor/6000.3.25f1/Unity.app/Contents/MacOS/Unity" -batchmode -projectPath "$PWD" -runTests -testPlatform PlayMode -testResults Logs/riding-tests.xml -logFile Logs/riding-tests.log
python3 tools/unity_project.py export-ios
```

Do not add `-quit` to the test command; the test runner exits when finished. See [Setup](SETUP.en.md) for signing/device prerequisites. Run `python3 tools/unity_project.py riding` only to generate a missing scene, not to update the committed one.

## Next acceptance check

On iPhone: start gently, turn left/right, make a U-turn, release to stop, re-touch without sudden acceleration, add/lift a second finger, then background/resume. Record steering difficulty, braking distance and any stuck states. Tune handling and add a safe recovery gesture before CR-012 pothole/bus work. Continue CR-013 camera/readability, CR-014 Android and CR-015 playtesting afterwards.
