# Riding prototype — M1, practice course (0.0.3)

5 October 2026 • [Tiếng Việt](RIDING_PROTOTYPE.vi.md)

Current scene: `Assets/CityRace/Content/Scenes/Practice.unity`. Smoke and the original straight Riding scene remain available, but Practice is the enabled build scene. Generate only a missing scene with `python3 tools/unity_project.py practice`; existing scenes are never overwritten by the generator.

## Current play loop

Leave the home, follow the yellow route through four left/right corners and a narrow passage, then stop in the yellow square at the company gate. The route is about 107 m along its centerline. Reach checkpoints in order and remain below 0.4 m/s in the finish zone for 0.7 seconds to complete. A route compass and elapsed time appear at the top. Use Restart at any time for a fresh run without reopening the app.

Controls retain relative one-finger dragging. Sharp turns now reduce target throttle progressively (down to 40% at a 100° heading error), and steering caps at 165°/s. Straight-line maximum speed remains 8 m/s; acceleration/braking remain 5/16 m/s², simulation 50 Hz. These are provisional tuning values, not user-validated handling.

To recover: release, then touch and hold still for 1.2 seconds while stopped. A progress message appears. Recovery returns to an earlier passed checkpoint, zeroes velocity and requires releasing before driving again; it adds two seconds. Route projection limits recovery during backtracking so it cannot skip forward along the course (the start is the lower bound). Short/interrupted holds do not trigger it. Finished runs cannot recover; Restart clears the run. The wider camera uses orthographic size 12 and snaps after long resets.

## Boundaries and limitations

`PracticeProgress` owns ordered progress, the stop requirement and recovery penalty in plain C#. `PracticeCourse` adapts Unity positions, input, physics reset and lifecycle. `PracticeHud` observes the course and dispatches Restart. Track geometry is generated once in the Editor, with shared materials and static geometry; no per-frame track generation.

This remains an offline practice course, not a complete race. No potholes, bus traffic, multiplayer or real traffic rules yet. The small prototype HUD continues the documented temporary uGUI/built-in-font approach; TextMeshPro/keyed localization and full Vietnamese glyph QA remain before production HUD work. Automatic corner easing needs feedback from physical playtesting. Device frame rates, thermal performance and Android/Web compatibility of this increment are not yet established.

## Validation for 0.0.3

Eleven Editor PlayMode tests passed on Unity 6000.3.25f1, including synthetic pointer clicks on Restart and an automated drive through the authored course. Use the same pinned Unity test command shown in the historical notes, with `Logs/practice-tests.xml` and `Logs/practice-tests.log`. Coverage includes the actual authored route, stop-to-finish, Restart through pointer events, stationary-hold recovery, no repeated recovery while held, backtracking limits and sharp-turn slowing, plus prior movement/input regression tests.

iOS 0.0.3 export and signed Xcode 26.6 Debug build passed. Strict code-sign verification, installation and process launch on iPhone 15 Pro Max (iOS 26.3 beta) passed; the process was still present after launch. The phone was displaying another app during the screenshot check, so native visual/handling acceptance remains pending user feedback. No unrelated screenshot is stored in project evidence.

---

## Historical 0.0.2 evidence

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
