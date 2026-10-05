using System;
using System.Collections.Generic;
using CityRace.Gameplay.Riding;
using CityRace.Presentation;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

namespace CityRace.Editor
{
    public static class PracticeBootstrap
    {
        public const string ScenePath = "Assets/CityRace/Content/Scenes/Practice.unity";
        [MenuItem("City Race/Bootstrap/Create Practice Course")]
        public static void CreateScene()
        {
            if (AssetDatabase.LoadAssetAtPath<SceneAsset>(ScenePath) != null)
            { throw new InvalidOperationException("Practice scene already exists; refusing to overwrite it."); }
            if (!Application.isBatchMode && !EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) { return; }
            var scene = EditorSceneManager.OpenScene(RidingBootstrap.ScenePath);
            var asphalt = GameObject.Find("Road").GetComponent<Renderer>().sharedMaterial;
            var kerb = GameObject.Find("Sidewalk").GetComponent<Renderer>().sharedMaterial;
            var yellow = GameObject.Find("Lane marking").GetComponent<Renderer>().sharedMaterial;
            var bike = GameObject.Find("Player Bike");
            foreach (var root in scene.GetRootGameObjects())
            {
                if (root != bike && root.GetComponent<Camera>() == null && root.GetComponent<Light>() == null)
                { UnityEngine.Object.DestroyImmediate(root); }
            }
            var roadRoot = new GameObject("Practice road");
            var cells = new HashSet<Vector2Int>();
            var corners = new[] { new Vector2Int(0, -2), new Vector2Int(0, 12), new Vector2Int(9, 12),
                new Vector2Int(9, 23), new Vector2Int(-2, 23), new Vector2Int(-2, 35) };
            for (var i = 1; i < corners.Length; i++)
            {
                var from = corners[i - 1];
                var to = corners[i];
                var step = new Vector2Int(Math.Sign(to.x - from.x), Math.Sign(to.y - from.y));
                for (var p = from; ; p += step)
                {
                    var narrow = i == 5 && p.y >= 27 && p.y <= 30;
                    for (var x = -1; x <= 1; x++)
                    for (var z = -1; z <= 1; z++)
                    { if (!narrow || x == 0) { cells.Add(p + new Vector2Int(x, z)); } }
                    if (p == to) { break; }
                }
            }
            var directions = new[] { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
            foreach (var cell in cells)
            {
                var center = new Vector3(cell.x * 2, -0.1f, cell.y * 2);
                Box("Road", center, new Vector3(2, .2f, 2), asphalt, roadRoot.transform);
                foreach (var d in directions)
                {
                    if (cells.Contains(cell + d)) { continue; }
                    Box("Solid kerb", center + new Vector3(d.x, .5f, d.y),
                        d.x == 0 ? new Vector3(2.2f, .8f, .2f) : new Vector3(.2f, .8f, 2.2f), kerb, roadRoot.transform);
                }
            }
            var y = bike.transform.position.y;
            var checkpoints = new[] { new Vector3(0,y,0), new Vector3(0,y,23), new Vector3(18,y,24),
                new Vector3(18,y,46), new Vector3(-4,y,46), new Vector3(-4,y,62), new Vector3(-4,y,68) };
            // Small guide dots trace the route and make corners visible before the next gate.
            for (var i = 1; i < checkpoints.Length; i++)
            {
                var a = checkpoints[i - 1]; var b = checkpoints[i];
                var length = Vector3.Distance(a,b);
                for (var d = 2f; d < length; d += 3f)
                {
                    var p = Vector3.Lerp(a,b,d / length); p.y = .025f;
                    var dot = Box("Route guide", p, new Vector3(.16f,.03f,.5f), yellow, roadRoot.transform);
                    dot.transform.rotation = Quaternion.LookRotation(b-a);
                    UnityEngine.Object.DestroyImmediate(dot.GetComponent<Collider>());
                }
            }
            Box("Company gate", new Vector3(-4, 1.5f, 71.5f), new Vector3(5,3,1), kerb, roadRoot.transform);
            var finish = Box("Stop at work", new Vector3(-4,.03f,68), new Vector3(3,.04f,3), yellow, roadRoot.transform);
            UnityEngine.Object.DestroyImmediate(finish.GetComponent<Collider>());
            Box("Home", new Vector3(-5,1.5f,0), new Vector3(3,3,4), kerb, roadRoot.transform);
            var input = bike.GetComponent<DragRideInput>();
            var motor = bike.GetComponent<BikeMotor>();
            // Existing serialized fields do not inherit changed script defaults.
            var serializedMotor = new SerializedObject(motor);
            serializedMotor.FindProperty("_turnDegreesPerSecond").floatValue = 165f;
            serializedMotor.ApplyModifiedPropertiesWithoutUndo();
            var course = bike.AddComponent<PracticeCourse>();
            course.Configure(motor, input, checkpoints);
            Camera.main.orthographicSize = 12f;
            BuildHud(course);
            PlayerSettings.bundleVersion = "0.0.3";
            if (!EditorSceneManager.SaveScene(scene, ScenePath)) { throw new InvalidOperationException("Could not save practice scene."); }
            EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(ScenePath, true) };
            AssetDatabase.SaveAssets();
            Debug.Log("Practice course created with corners, narrow section, ordered finish and recovery.");
        }

        private static GameObject Box(string name, Vector3 position, Vector3 scale, Material material, Transform parent)
        {
            var obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            obj.name = name; obj.transform.SetParent(parent);
            obj.transform.position = position; obj.transform.localScale = scale;
            obj.GetComponent<Renderer>().sharedMaterial = material;
            obj.isStatic = true;
            return obj;
        }

        private static void BuildHud(PracticeCourse course)
        {
            var canvas = new GameObject("Practice HUD", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            canvas.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvas.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(390,844); scaler.matchWidthOrHeight = .5f;
            var safe = new GameObject("Safe area", typeof(RectTransform), typeof(RidingHint), typeof(PracticeHud));
            safe.transform.SetParent(canvas.transform,false);
            var status = Label("Route and time", safe.transform, new Vector2(0,1), new Vector2(1,1), new Vector2(8,-80), new Vector2(-8,-32), 19);
            var hint = Label("Controls", safe.transform, Vector2.zero, Vector2.right, new Vector2(8,20), new Vector2(-8,90), 15);
            var button = new GameObject("Restart", typeof(RectTransform), typeof(Image), typeof(Button));
            button.transform.SetParent(safe.transform,false);
            var rect = button.GetComponent<RectTransform>();
            rect.anchorMin = rect.anchorMax = new Vector2(1,1);
            rect.pivot = new Vector2(1,1); rect.anchoredPosition = new Vector2(-12,-88); rect.sizeDelta = new Vector2(96,42);
            button.GetComponent<Image>().color = new Color(.12f,.19f,.23f,.95f);
            Label("Label",button.transform, Vector2.zero, Vector2.one,Vector2.zero,Vector2.zero,16);
            safe.GetComponent<PracticeHud>().Configure(course,status,hint,button.GetComponent<Button>());
            new GameObject("EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));
        }

        private static Text Label(string name, Transform parent, Vector2 min, Vector2 max, Vector2 offsetMin, Vector2 offsetMax, int size)
        {
            var obj = new GameObject(name,typeof(RectTransform),typeof(Text),typeof(Shadow));
            obj.transform.SetParent(parent,false);
            var rect = obj.GetComponent<RectTransform>();
            rect.anchorMin = min; rect.anchorMax = max; rect.offsetMin = offsetMin; rect.offsetMax = offsetMax;
            var text = obj.GetComponent<Text>(); text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = size; text.alignment = TextAnchor.MiddleCenter; text.color = Color.white; text.raycastTarget = false;
            return text;
        }
    }
}
