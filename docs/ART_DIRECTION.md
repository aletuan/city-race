# Art direction

Decided 7 October 2026: **direction B — Sài Gòn at dusk.** Stylised low-poly, warm low sun, purple ambient, lit shopfronts, neon signs and street-lamp pools. Realistic textures (direction C) were rejected for a solo, mobile-first project.

## Principles

- Readability beats mood. Route guides, the finish square, potholes and future hazards must stay brighter and more saturated than the dressing. Hazard materials stay unlit.
- One palette. All dressing uses the materials in `Assets/CityRace/Content/Look/Dusk`; imported free assets (Quaternius/Kenney/KayKit, CC0) must be re-mapped to this palette rather than keeping their own colours.
- Glow is faked. Emissive/unlit HDR colours plus bloom and additive "light pool" quads; no real point lights for lamps, signs or headlights (mobile budget).
- Presentation is separate from simulation. Dressing has no colliders; collision shapes stay simple boxes.
- No brand logos (GDD).

## Current implementation (look pass 1)

`City Race/Look/Apply Dusk Look to Practice` (`python3 tools/unity_project.py look-dusk`) runs `DuskLookBootstrap`. It is rerunnable and only touches presentation:

| Area | Setting |
| --- | --- |
| Sun | Directional, colour (1, .60, .38), intensity 1.6, rotation (30°, −35°), soft shadows, strength .6 |
| Ambient / fog | Trilight sky/equator/ground; linear fog 42–110 m, colour (.47, .33, .45) |
| Camera | Perspective, FOV 34°, follow offset (0, 32, −20), pitch 58°; FXAA; post-processing on. Matches the previous orthographic coverage (size 12) at the bike |
| Post | ACES, exposure +.55, contrast 12, saturation 18, shadows/highlights split tone, bloom (threshold .95, intensity .9), vignette .28 |
| URP shadows | Main light, soft, 2048 map, 45 m, one cascade (mobile and PC assets) |
| Dressing | Ground slab, sidewalks, procedural 2 m shophouses (1–3 floors; kept to one floor south of a road so they never hide it), lit windows, neon signs, street lamps with fake pools, company sign, home window |
| Bike | Headlight, tail light and additive beam; no colliders |

## Open

- Device readability, especially potholes in shadow, and frame rate on iPhone 15 Pro Max are **not yet verified**.
- HUD restyle (UI Toolkit, Be Vietnam Pro, PrimeTween) is the next pass.
- Replace primitive bike/rider and shophouses with Blender-made models in this palette; set up Git LFS first.
