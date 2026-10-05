using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;

namespace CityRace.Editor
{
    // Copied into Assets/CityRace/Editor only after the URP template is created.
    public static class CityRaceBootstrap
    {
        private const string ScenePath = "Assets/CityRace/Content/Scenes/Smoke.unity";
        private const string MaterialDirectory = "Assets/CityRace/Content/Materials";

        [MenuItem("City Race/Bootstrap/Create Smoke Scene")]
        public static void CreateSmokeScene()
        {
            if (AssetDatabase.LoadAssetAtPath<SceneAsset>(ScenePath) != null)
            {
                Debug.Log("Smoke scene already exists; preserving it.");
                return;
            }

            if (GraphicsSettings.defaultRenderPipeline == null)
            {
                throw new InvalidOperationException("Open an initialised URP template before creating the scene.");
            }

            var shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null)
            {
                throw new InvalidOperationException("URP Lit shader is unavailable.");
            }

            // Do not replace an unsaved scene when invoked interactively.
            if (!Application.isBatchMode && !EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                return;
            }

            Directory.CreateDirectory(Path.GetDirectoryName(ScenePath));
            Directory.CreateDirectory(MaterialDirectory);
            AssetDatabase.Refresh();
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            var asphalt = Material("Asphalt", new Color(0.19f, 0.22f, 0.25f), shader);
            var concrete = Material("Concrete", new Color(0.66f, 0.64f, 0.57f), shader);
            var facade = Material("Facade", new Color(0.74f, 0.80f, 0.74f), shader);
            var red = Material("Bike", new Color(0.85f, 0.20f, 0.12f), shader);
            var yellow = Material("Marking", new Color(0.98f, 0.82f, 0.35f), shader);

            Box("Road", new Vector3(0f, -0.1f, 0f), new Vector3(8f, 0.2f, 25f), asphalt);
            for (var side = -1; side <= 1; side += 2)
            {
                Box("Sidewalk", new Vector3(side * 4.7f, 0.05f, 0f), new Vector3(1.4f, 0.3f, 25f), concrete);
                for (var row = 0; row < 5; row++)
                {
                    Box("Building", new Vector3(side * 6.4f, 1.4f, row * 5f - 10f), new Vector3(2f, 2.8f, 3.8f), facade);
                }
            }

            for (var row = -4; row <= 4; row++)
            {
                Box("Lane marking", new Vector3(0f, 0.015f, row * 2.5f), new Vector3(0.1f, 0.02f, 1.1f), yellow);
            }

            Box("Bike marker", new Vector3(-1.5f, 0.45f, -2f), new Vector3(0.65f, 0.7f, 1.4f), red);
            Box("Rider marker", new Vector3(-1.5f, 1f, -2.1f), new Vector3(0.4f, 0.6f, 0.4f), yellow);

            var cameraObject = new GameObject("Main Camera", typeof(Camera), typeof(AudioListener));
            cameraObject.tag = "MainCamera";
            cameraObject.transform.position = new Vector3(0f, 19f, -13f);
            cameraObject.transform.LookAt(new Vector3(0f, 0f, 1f));
            var camera = cameraObject.GetComponent<Camera>();
            camera.orthographic = true;
            camera.orthographicSize = 12f;
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0.64f, 0.76f, 0.82f);
            camera.farClipPlane = 100f;

            var lightObject = new GameObject("Sun", typeof(Light));
            lightObject.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
            var light = lightObject.GetComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 1.2f;
            light.shadows = LightShadows.None;
            RenderSettings.ambientMode = AmbientMode.Flat;
            RenderSettings.ambientLight = new Color(0.6f, 0.6f, 0.6f);

            PlayerSettings.productName = "City Race";
            PlayerSettings.companyName = "City Race";
            PlayerSettings.bundleVersion = "0.0.1";
            PlayerSettings.defaultInterfaceOrientation = UIOrientation.Portrait;
            PlayerSettings.defaultScreenWidth = 390;
            PlayerSettings.defaultScreenHeight = 844;
            PlayerSettings.SetApplicationIdentifier(NamedBuildTarget.iOS, "com.aletuan.cityrace.dev");
            PlayerSettings.SetScriptingBackend(NamedBuildTarget.iOS, ScriptingImplementation.IL2CPP);
            PlayerSettings.iOS.targetDevice = iOSTargetDevice.iPhoneOnly;
            EditorSettings.serializationMode = SerializationMode.ForceText;

            if (!EditorSceneManager.SaveScene(scene, ScenePath))
            {
                throw new IOException("Failed to save the smoke scene.");
            }

            EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(ScenePath, true) };
            AssetDatabase.SaveAssets();
            Debug.Log("City Race smoke scene generated. Build and visually inspect it before claiming success.");
        }

        public static void ExportIos()
        {
            Build(BuildTarget.iOS, "Builds/iOS");
        }

        public static void BuildWeb()
        {
            Build(BuildTarget.WebGL, "Builds/Web");
        }

        private static void Build(BuildTarget target, string output)
        {
            if (EditorUserBuildSettings.activeBuildTarget != target)
            {
                throw new InvalidOperationException("Pass the matching -buildTarget to Unity before executing this method.");
            }

            if (AssetDatabase.LoadAssetAtPath<SceneAsset>(ScenePath) == null)
            {
                throw new InvalidOperationException("Create and inspect the smoke scene before building.");
            }

            var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions
            {
                scenes = new[] { ScenePath },
                locationPathName = output,
                target = target,
                options = BuildOptions.Development
            });

            if (report.summary.result != BuildResult.Succeeded)
            {
                throw new BuildFailedException($"{target} build failed: {report.summary.result}");
            }

            Debug.Log($"{target} export succeeded at {output}; device/browser execution is a separate check.");
        }

        private static Material Material(string name, Color color, Shader shader)
        {
            var path = $"{MaterialDirectory}/{name}.mat";
            var existing = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (existing != null)
            {
                return existing;
            }

            var material = new Material(shader) { name = name };
            material.SetColor("_BaseColor", color);
            AssetDatabase.CreateAsset(material, path);
            return material;
        }

        private static void Box(string name, Vector3 position, Vector3 scale, Material material)
        {
            var item = GameObject.CreatePrimitive(PrimitiveType.Cube);
            item.name = name;
            item.transform.position = position;
            item.transform.localScale = scale;
            item.GetComponent<Renderer>().sharedMaterial = material;
        }
    }
}
