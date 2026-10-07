using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CityRace.Presentation;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Object = UnityEngine.Object;

namespace CityRace.Editor
{
    // Presentation-only "Sài Gòn dusk" look for the Practice scene (art direction B, docs/ART_DIRECTION.md).
    // Safe to rerun: it rebuilds its own dressing root and lamp children, and only reassigns materials on
    // existing course renderers. Gameplay objects, colliders, checkpoints and tuning are never changed;
    // every primitive it creates has its collider removed.
    public static class DuskLookBootstrap
    {
        private const string Dir = "Assets/CityRace/Content/Look/Dusk";
        private const string DressingName = "Dusk dressing";
        private const string VolumeName = "Dusk volume";
        private const string LampPrefix = "Lamp - ";
        private const float Cell = 2f;

        private static readonly Color FogColour = new Color(.47f, .33f, .45f);

        [MenuItem("City Race/Look/Apply Dusk Look to Practice")]
        public static void Apply()
        {
            if (!Application.isBatchMode && !EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) { return; }
            var scene = EditorSceneManager.OpenScene(PracticeBootstrap.ScenePath);
            Directory.CreateDirectory(Dir);
            AssetDatabase.Refresh();

            var palette = new Palette(RadialGlowTexture());
            var course = GameObject.Find("Practice road")
                ?? throw new InvalidOperationException("Practice road root is missing.");
            var roadCells = RestyleCourse(course, palette, out var reserved);

            var old = GameObject.Find(DressingName);
            if (old != null) { Object.DestroyImmediate(old); }
            var dressing = new GameObject(DressingName).transform;
            BuildNeighbourhood(dressing, roadCells, reserved, palette);
            BuildLandmarks(dressing, palette);
            BuildBikeLamps(palette);

            ConfigureSunAndAmbient();
            ConfigureCamera();
            ConfigureVolume();
            foreach (var asset in new[] { "Assets/Settings/Mobile_RPAsset.asset", "Assets/Settings/PC_RPAsset.asset" })
            { ConfigureShadows(asset); }

            EditorSceneManager.MarkSceneDirty(scene);
            if (!EditorSceneManager.SaveScene(scene)) { throw new InvalidOperationException("Could not save Practice scene."); }
            AssetDatabase.SaveAssets();
            Debug.Log($"Dusk look applied: {dressing.childCount} dressing objects, no colliders added. " +
                      "Check readability and frame rate on device before accepting.");
        }

        // ---------- course ----------

        private static HashSet<Vector2Int> RestyleCourse(GameObject course, Palette p, out HashSet<Vector2Int> reserved)
        {
            var cells = new HashSet<Vector2Int>();
            reserved = new HashSet<Vector2Int>();
            foreach (var renderer in course.GetComponentsInChildren<Renderer>(true))
            {
                switch (renderer.name)
                {
                    case "Road":
                        renderer.sharedMaterial = p.Asphalt;
                        var c = renderer.transform.position;
                        cells.Add(new Vector2Int(Mathf.RoundToInt(c.x / Cell), Mathf.RoundToInt(c.z / Cell)));
                        break;
                    case "Solid kerb": renderer.sharedMaterial = p.Kerb; break;
                    case "Route guide":
                    case "Stop at work": renderer.sharedMaterial = p.Marking; break;
                    case "Home": renderer.sharedMaterial = p.Home; Reserve(renderer.bounds, reserved); break;
                    case "Company gate": renderer.sharedMaterial = p.Company; Reserve(renderer.bounds, reserved); break;
                }
            }
            if (cells.Count == 0) { throw new InvalidOperationException("No road cells found under Practice road."); }
            return cells;
        }

        private static void Reserve(Bounds b, HashSet<Vector2Int> reserved)
        {
            for (var x = Mathf.FloorToInt((b.min.x - .2f) / Cell); x <= Mathf.CeilToInt((b.max.x + .2f) / Cell); x++)
            for (var z = Mathf.FloorToInt((b.min.z - .2f) / Cell); z <= Mathf.CeilToInt((b.max.z + .2f) / Cell); z++)
            {
                var cellMin = new Vector2((x - .5f) * Cell, (z - .5f) * Cell);
                if (cellMin.x < b.max.x && cellMin.x + Cell > b.min.x && cellMin.y < b.max.z && cellMin.y + Cell > b.min.z)
                { reserved.Add(new Vector2Int(x, z)); }
            }
        }

        // ---------- neighbourhood ----------

        private static readonly Vector2Int[] Four = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

        private static void BuildNeighbourhood(Transform root, HashSet<Vector2Int> road, HashSet<Vector2Int> reserved, Palette p)
        {
            const int margin = 7;
            var min = new Vector2Int(road.Min(c => c.x) - margin, road.Min(c => c.y) - margin);
            var max = new Vector2Int(road.Max(c => c.x) + margin, road.Max(c => c.y) + margin);

            // Chebyshev distance to the nearest road cell (multi-source BFS over 8 neighbours).
            var distance = new Dictionary<Vector2Int, int>();
            var queue = new Queue<Vector2Int>();
            foreach (var c in road) { distance[c] = 0; queue.Enqueue(c); }
            while (queue.Count > 0)
            {
                var c = queue.Dequeue();
                for (var dx = -1; dx <= 1; dx++)
                for (var dz = -1; dz <= 1; dz++)
                {
                    var n = new Vector2Int(c.x + dx, c.y + dz);
                    if (n.x < min.x || n.y < min.y || n.x > max.x || n.y > max.y || distance.ContainsKey(n)) { continue; }
                    distance[n] = distance[c] + 1;
                    queue.Enqueue(n);
                }
            }

            // Ground slab under everything, just below the road surface.
            var size = new Vector3((max.x - min.x + 1) * Cell, .2f, (max.y - min.y + 1) * Cell);
            var centre = new Vector3((min.x + max.x) * .5f * Cell, -.13f, (min.y + max.y) * .5f * Cell);
            Piece("Ground", PrimitiveType.Cube, centre, size, p.Ground, root, castShadows: false);

            var rng = new System.Random(20261007);
            foreach (var cell in distance.Keys.OrderBy(c => c.x).ThenBy(c => c.y))
            {
                var d = distance[cell];
                if (d == 0 || reserved.Contains(cell)) { continue; }
                var at = new Vector3(cell.x * Cell, 0f, cell.y * Cell);
                if (d == 1)
                {
                    Piece("Sidewalk", PrimitiveType.Cube, at + Vector3.up * .05f, new Vector3(Cell, .16f, Cell), p.Sidewalk, root, castShadows: false);
                    var toRoad = Four.FirstOrDefault(dir => road.Contains(cell + dir));
                    if (toRoad != Vector2Int.zero && Mod(cell.x * 7 + cell.y * 13, 4) == 0) { StreetLamp(root, at, toRoad, p); }
                }
                else if (d <= 5)
                {
                    Shophouse(root, cell, at, d, road, distance, rng, p);
                }
            }
        }

        private static void Shophouse(Transform root, Vector2Int cell, Vector3 at, int d, HashSet<Vector2Int> road,
            Dictionary<Vector2Int, int> distance, System.Random rng, Palette p)
        {
            // The camera looks north; tall houses south of a road would hide it, so keep those low.
            var southOfRoad = false;
            for (var k = 1; k <= 4 && !southOfRoad; k++)
            for (var dx = -1; dx <= 1; dx++)
            { if (road.Contains(new Vector2Int(cell.x + dx, cell.y + k))) { southOfRoad = true; } }

            var floors = southOfRoad ? 1 : (d == 2 ? rng.Next(1, 4) : rng.Next(1, 4));
            var height = floors * 3f + .4f;
            var facade = p.Facades[rng.Next(p.Facades.Length)];
            Piece("Shophouse", PrimitiveType.Cube, at + Vector3.up * (height * .5f - .02f), new Vector3(1.88f, height, 1.88f), facade, root);
            Piece("Roof edge", PrimitiveType.Cube, at + Vector3.up * (height + .08f), new Vector3(1.96f, .16f, 1.96f), p.Roof, root);
            if (d != 2) { return; }

            // Front row: windows and signs face the street.
            var face = Four.FirstOrDefault(dir => distance.TryGetValue(cell + dir, out var n) && n < d);
            if (face == Vector2Int.zero) { return; }
            var normal = new Vector3(face.x, 0f, face.y);
            var alongX = face.y != 0;
            Vector3 Panel(float width, float tall) => alongX ? new Vector3(width, tall, .06f) : new Vector3(.06f, tall, width);
            var front = at + normal * .96f;

            for (var f = 0; f < floors; f++)
            {
                if (f == 0)
                {
                    if (rng.NextDouble() < .75) { Piece("Shopfront", PrimitiveType.Cube, front + Vector3.up * 1.05f, Panel(1.4f, 1.9f), p.Window, root, false); }
                    else { Piece("Shutter", PrimitiveType.Cube, front + Vector3.up * 1.05f, Panel(1.5f, 2f), p.Roof, root, false); }
                    continue;
                }
                var lit = rng.NextDouble() < .5;
                Piece(lit ? "Window lit" : "Window dark", PrimitiveType.Cube, front + Vector3.up * (f * 3f + 1.5f), Panel(.9f, 1.1f), lit ? p.Window : p.WindowDark, root, false);
            }
            if (rng.NextDouble() < .45)
            {
                var neon = p.Neons[rng.Next(p.Neons.Length)];
                Piece("Shop sign", PrimitiveType.Cube, front + normal * .08f + Vector3.up * 2.45f, Panel(1.6f, .38f), neon, root, false);
            }
            if (floors >= 2 && rng.NextDouble() < .25)
            {
                var neon = p.Neons[rng.Next(p.Neons.Length)];
                var side = alongX ? Vector3.right : Vector3.forward;
                var blade = alongX ? new Vector3(.08f, 1.4f, .7f) : new Vector3(.7f, 1.4f, .08f);
                Piece("Blade sign", PrimitiveType.Cube, front + normal * .4f + side * .7f + Vector3.up * 4.3f, blade, neon, root, false);
            }
        }

        private static void StreetLamp(Transform root, Vector3 at, Vector2Int toRoad, Palette p)
        {
            var dir = new Vector3(toRoad.x, 0f, toRoad.y);
            var head = toRoad.x != 0 ? new Vector3(.7f, .1f, .22f) : new Vector3(.22f, .1f, .7f);
            Piece("Lamp post", PrimitiveType.Cube, at + Vector3.up * 1.7f, new Vector3(.1f, 3.4f, .1f), p.Pole, root);
            Piece("Lamp head", PrimitiveType.Cube, at + dir * .35f + Vector3.up * 3.4f, head, p.LampHead, root, false);
            // Fake light pool: additive quad instead of a real point light (mobile budget).
            Piece("Lamp pool", PrimitiveType.Quad, at + dir * 1.4f + Vector3.up * .15f, new Vector3(4.5f, 4.5f, 1f), p.LampPool, root, false,
                Quaternion.Euler(90f, 0f, 0f));
        }

        private static void BuildLandmarks(Transform root, Palette p)
        {
            var company = GameObject.Find("Company gate");
            if (company != null)
            {
                var b = company.GetComponent<Renderer>().bounds;
                Piece("Company sign", PrimitiveType.Cube, new Vector3(b.center.x, b.max.y + .45f, b.center.z), new Vector3(b.size.x * .7f, .5f, .14f), p.Neons[1], root, false);
                Piece("Company glow", PrimitiveType.Quad, new Vector3(b.center.x, .15f, b.min.z - 2f), new Vector3(6f, 4f, 1f), p.LampPool, root, false, Quaternion.Euler(90f, 0f, 0f));
            }
            var home = GameObject.Find("Home");
            if (home != null)
            {
                var b = home.GetComponent<Renderer>().bounds;
                Piece("Home window", PrimitiveType.Cube, new Vector3(b.max.x + .03f, 1.4f, b.center.z), new Vector3(.06f, 1.2f, 1.4f), p.Window, root, false);
                Piece("Home sign", PrimitiveType.Cube, new Vector3(b.max.x + .1f, 2.5f, b.center.z), new Vector3(.08f, .36f, 1.6f), p.Neons[0], root, false);
            }
        }

        // ---------- bike ----------

        private static void BuildBikeLamps(Palette p)
        {
            var bike = GameObject.Find("Player Bike") ?? throw new InvalidOperationException("Player Bike is missing.");
            var visuals = bike.GetComponentInChildren<BikeImpactView>(true);
            var parent = visuals != null ? visuals.transform : bike.transform;
            foreach (var child in parent.Cast<Transform>().Where(t => t.name.StartsWith(LampPrefix)).ToList())
            { Object.DestroyImmediate(child.gameObject); }

            void Lamp(string name, PrimitiveType type, Vector3 local, Vector3 scale, Material material, Quaternion? rotation = null)
            {
                var obj = GameObject.CreatePrimitive(type);
                obj.name = LampPrefix + name;
                // Colliders under the rigidbody would join its compound shape; never keep them.
                Object.DestroyImmediate(obj.GetComponent<Collider>());
                obj.transform.SetPositionAndRotation(bike.transform.TransformPoint(local), bike.transform.rotation * (rotation ?? Quaternion.identity));
                obj.transform.localScale = scale;
                obj.transform.SetParent(parent, true);
                var renderer = obj.GetComponent<MeshRenderer>();
                renderer.sharedMaterial = material;
                renderer.shadowCastingMode = ShadowCastingMode.Off;
                renderer.receiveShadows = false;
            }
            Lamp("headlight", PrimitiveType.Cube, new Vector3(0f, .22f, .71f), new Vector3(.3f, .15f, .05f), p.Headlight);
            Lamp("tail light", PrimitiveType.Cube, new Vector3(0f, .15f, -.71f), new Vector3(.26f, .1f, .05f), p.TailLight);
            Lamp("beam", PrimitiveType.Quad, new Vector3(0f, -.4f, 3.1f), new Vector3(2.4f, 4.6f, 1f), p.Beam, Quaternion.Euler(90f, 0f, 0f));
        }

        // ---------- lighting, camera, post ----------

        private static void ConfigureSunAndAmbient()
        {
            var sun = Object.FindObjectsByType<Light>(FindObjectsSortMode.None).FirstOrDefault(l => l.type == LightType.Directional)
                ?? throw new InvalidOperationException("Directional light is missing.");
            sun.color = new Color(1f, .6f, .38f);
            sun.intensity = 1.6f;
            // Low south-east sun: long shadows fall away from the camera, so camera-facing facades stay lit.
            sun.transform.rotation = Quaternion.Euler(30f, -35f, 0f);
            sun.shadows = LightShadows.Soft;
            sun.shadowStrength = .6f;

            RenderSettings.ambientMode = AmbientMode.Trilight;
            RenderSettings.ambientSkyColor = new Color(.66f, .56f, .82f);
            RenderSettings.ambientEquatorColor = new Color(.80f, .56f, .54f);
            RenderSettings.ambientGroundColor = new Color(.16f, .13f, .20f);
            RenderSettings.fog = true;
            RenderSettings.fogMode = FogMode.Linear;
            RenderSettings.fogColor = FogColour;
            RenderSettings.fogStartDistance = 42f;
            RenderSettings.fogEndDistance = 110f;
            RenderSettings.skybox = null;
        }

        private static void ConfigureCamera()
        {
            var camera = Camera.main ?? throw new InvalidOperationException("Main Camera is missing.");
            // Perspective gives buildings depth; offset/FOV keep the previous ground coverage (ortho size 12).
            camera.orthographic = false;
            camera.fieldOfView = 34f;
            camera.nearClipPlane = .5f;
            camera.farClipPlane = 140f;
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = FogColour;
            var data = camera.GetUniversalAdditionalCameraData();
            data.renderPostProcessing = true;
            data.antialiasing = AntialiasingMode.FastApproximateAntialiasing;

            var follow = camera.GetComponent<RidingCamera>();
            if (follow != null)
            {
                var so = new SerializedObject(follow);
                so.FindProperty("_offset").vector3Value = new Vector3(0f, 32f, -20f);
                so.FindProperty("_pitch").floatValue = 58f;
                so.ApplyModifiedPropertiesWithoutUndo();
                camera.transform.position = GameObject.Find("Player Bike").transform.position + new Vector3(0f, 32f, -20f);
                camera.transform.rotation = Quaternion.Euler(58f, 0f, 0f);
            }
        }

        private static void ConfigureVolume()
        {
            var path = $"{Dir}/DuskVolume.asset";
            var profile = AssetDatabase.LoadAssetAtPath<VolumeProfile>(path);
            if (profile == null)
            {
                profile = ScriptableObject.CreateInstance<VolumeProfile>();
                AssetDatabase.CreateAsset(profile, path);
            }
            foreach (var component in profile.components.ToList()) { Object.DestroyImmediate(component, true); }
            profile.components.Clear();

            T Add<T>() where T : VolumeComponent
            {
                var component = profile.Add<T>(false);
                component.name = typeof(T).Name;
                AssetDatabase.AddObjectToAsset(component, profile);
                return component;
            }
            Add<Tonemapping>().mode.Override(TonemappingMode.ACES);
            var grade = Add<ColorAdjustments>();
            grade.postExposure.Override(.55f);
            grade.contrast.Override(12f);
            grade.saturation.Override(18f);
            grade.colorFilter.Override(new Color(1f, .94f, .9f));
            var smh = Add<ShadowsMidtonesHighlights>();
            smh.shadows.Override(new Vector4(.9f, .85f, 1.15f, 0f));
            smh.highlights.Override(new Vector4(1.1f, 1f, .9f, 0f));
            var bloom = Add<Bloom>();
            bloom.threshold.Override(.95f);
            bloom.intensity.Override(.9f);
            bloom.scatter.Override(.65f);
            bloom.tint.Override(new Color(1f, .85f, .75f));
            var vignette = Add<Vignette>();
            vignette.intensity.Override(.28f);
            vignette.smoothness.Override(.45f);
            vignette.color.Override(new Color(.12f, .05f, .12f));
            EditorUtility.SetDirty(profile);

            var volumeObject = GameObject.Find(VolumeName) ?? new GameObject(VolumeName, typeof(Volume));
            var volume = volumeObject.GetComponent<Volume>();
            volume.isGlobal = true;
            volume.priority = 1f;
            volume.sharedProfile = profile;
        }

        private static void ConfigureShadows(string assetPath)
        {
            var asset = AssetDatabase.LoadAssetAtPath<UniversalRenderPipelineAsset>(assetPath);
            if (asset == null) { return; }
            var so = new SerializedObject(asset);
            void Set(string name, Action<SerializedProperty> apply) { var prop = so.FindProperty(name); if (prop != null) { apply(prop); } }
            Set("m_MainLightShadowsSupported", s => s.boolValue = true);
            Set("m_SoftShadowsSupported", s => s.boolValue = true);
            Set("m_MainLightShadowmapResolution", s => s.intValue = 2048);
            Set("m_ShadowDistance", s => s.floatValue = 45f);
            Set("m_ShadowCascadeCount", s => s.intValue = 1);
            so.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(asset);
        }

        // ---------- materials ----------

        private sealed class Palette
        {
            public readonly Material Asphalt, Kerb, Sidewalk, Ground, Marking, Roof, Pole, Home, Company;
            public readonly Material Window, WindowDark, LampHead, Headlight, TailLight, LampPool, Beam;
            public readonly Material[] Facades, Neons;

            public Palette(Texture2D glow)
            {
                Asphalt = Lit("Asphalt", new Color(.34f, .34f, .40f), .45f);
                Kerb = Lit("Kerb", new Color(.78f, .70f, .64f));
                Sidewalk = Lit("Sidewalk", new Color(.72f, .56f, .48f));
                Ground = Lit("Ground", new Color(.30f, .25f, .30f));
                Roof = Lit("Roof", new Color(.30f, .25f, .30f));
                Pole = Lit("Pole", new Color(.22f, .21f, .25f));
                Marking = Lit("Marking", new Color(1f, .78f, .30f), .2f, new Color(1f, .68f, .22f) * .7f);
                Home = Lit("Home", new Color(.88f, .62f, .50f));
                Company = Lit("Company", new Color(.30f, .44f, .54f), .55f);
                Facades = new[]
                {
                    Lit("Facade peach", new Color(.88f, .62f, .52f)),
                    Lit("Facade mint", new Color(.66f, .78f, .70f)),
                    Lit("Facade mustard", new Color(.92f, .78f, .50f)),
                    Lit("Facade periwinkle", new Color(.60f, .64f, .82f)),
                    Lit("Facade rose", new Color(.86f, .66f, .72f)),
                    Lit("Facade cream", new Color(.90f, .86f, .76f)),
                };
                Window = Glow("Window warm", new Color(2.2f, 1.45f, .65f));
                WindowDark = Lit("Window dark", new Color(.16f, .16f, .24f), .7f);
                LampHead = Glow("Lamp head", new Color(3f, 2.1f, 1.1f));
                Headlight = Glow("Headlight", new Color(3f, 2.8f, 2.2f));
                TailLight = Glow("Tail light", new Color(2.6f, .2f, .15f));
                Neons = new[]
                {
                    Glow("Neon pink", new Color(3f, .45f, 1.6f)),
                    Glow("Neon cyan", new Color(.35f, 2.3f, 3f)),
                    Glow("Neon green", new Color(.5f, 2.8f, .9f)),
                    Glow("Neon red", new Color(3f, .5f, .35f)),
                };
                LampPool = Additive("Lamp pool", new Color(.85f, .58f, .26f), glow);
                Beam = Additive("Headlight beam", new Color(.42f, .40f, .30f), glow);
            }

            private static Material Asset(string name, string shaderName)
            {
                var shader = Shader.Find(shaderName) ?? throw new InvalidOperationException($"{shaderName} is unavailable.");
                var path = $"{Dir}/{name}.mat";
                var material = AssetDatabase.LoadAssetAtPath<Material>(path);
                if (material == null)
                {
                    material = new Material(shader) { name = name };
                    AssetDatabase.CreateAsset(material, path);
                }
                else { material.shader = shader; }
                material.enableInstancing = true;
                EditorUtility.SetDirty(material);
                return material;
            }

            private static Material Lit(string name, Color colour, float smoothness = .12f, Color? emission = null)
            {
                var m = Asset(name, "Universal Render Pipeline/Lit");
                m.SetColor("_BaseColor", colour);
                m.SetFloat("_Smoothness", smoothness);
                if (emission.HasValue)
                {
                    m.EnableKeyword("_EMISSION");
                    m.SetColor("_EmissionColor", emission.Value);
                    m.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
                }
                else
                {
                    m.DisableKeyword("_EMISSION");
                    m.SetColor("_EmissionColor", Color.black);
                    m.globalIlluminationFlags = MaterialGlobalIlluminationFlags.EmissiveIsBlack;
                }
                return m;
            }

            // HDR unlit colour above 1 so bloom picks it up; cheap on mobile.
            private static Material Glow(string name, Color hdr)
            {
                var m = Asset(name, "Universal Render Pipeline/Unlit");
                m.SetColor("_BaseColor", hdr);
                return m;
            }

            private static Material Additive(string name, Color colour, Texture texture)
            {
                var m = Asset(name, "Universal Render Pipeline/Unlit");
                m.SetFloat("_Surface", 1f);
                m.SetFloat("_Blend", 2f); // URP: additive
                m.SetFloat("_SrcBlend", (float)BlendMode.One);
                m.SetFloat("_DstBlend", (float)BlendMode.One);
                m.SetFloat("_SrcBlendAlpha", (float)BlendMode.One);
                m.SetFloat("_DstBlendAlpha", (float)BlendMode.One);
                m.SetFloat("_ZWrite", 0f);
                m.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
                m.SetOverrideTag("RenderType", "Transparent");
                m.renderQueue = (int)RenderQueue.Transparent;
                m.SetTexture("_BaseMap", texture);
                m.SetColor("_BaseColor", colour);
                return m;
            }
        }

        private static Texture2D RadialGlowTexture()
        {
            var path = $"{Dir}/RadialGlow.png";
            if (AssetDatabase.LoadAssetAtPath<Texture2D>(path) == null)
            {
                const int size = 64;
                var texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
                for (var y = 0; y < size; y++)
                for (var x = 0; x < size; x++)
                {
                    var r = Vector2.Distance(new Vector2(x + .5f, y + .5f), new Vector2(size * .5f, size * .5f)) / (size * .5f);
                    var v = Mathf.Pow(Mathf.Clamp01(1f - r), 2.2f);
                    texture.SetPixel(x, y, new Color(v, v, v, v));
                }
                File.WriteAllBytes(path, texture.EncodeToPNG());
                Object.DestroyImmediate(texture);
                AssetDatabase.ImportAsset(path);
                var importer = (TextureImporter)AssetImporter.GetAtPath(path);
                importer.wrapMode = TextureWrapMode.Clamp;
                importer.alphaIsTransparency = true;
                importer.SaveAndReimport();
            }
            return AssetDatabase.LoadAssetAtPath<Texture2D>(path);
        }

        // ---------- helpers ----------

        private static int Mod(int a, int m) => ((a % m) + m) % m;

        private static GameObject Piece(string name, PrimitiveType type, Vector3 position, Vector3 scale, Material material,
            Transform parent, bool castShadows = true, Quaternion? rotation = null)
        {
            var obj = GameObject.CreatePrimitive(type);
            obj.name = name;
            Object.DestroyImmediate(obj.GetComponent<Collider>());
            obj.transform.SetParent(parent, false);
            obj.transform.SetPositionAndRotation(position, rotation ?? Quaternion.identity);
            obj.transform.localScale = scale;
            var renderer = obj.GetComponent<MeshRenderer>();
            renderer.sharedMaterial = material;
            renderer.shadowCastingMode = castShadows ? ShadowCastingMode.On : ShadowCastingMode.Off;
            // Lit surfaces (ground, sidewalks, facades) receive shadows; glowing unlit pieces do not.
            renderer.receiveShadows = material.shader.name.EndsWith("/Lit");
            GameObjectUtility.SetStaticEditorFlags(obj, StaticEditorFlags.BatchingStatic);
            return obj;
        }
    }
}
