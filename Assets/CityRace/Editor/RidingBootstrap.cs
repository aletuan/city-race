using System;
using CityRace.Gameplay.Riding;
using CityRace.Presentation;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

namespace CityRace.Editor
{
    public static class RidingBootstrap
    {
        public const string ScenePath = "Assets/CityRace/Content/Scenes/Riding.unity";
        [MenuItem("City Race/Bootstrap/Create Riding Scene")]
        public static void CreateScene()
        {
            if (AssetDatabase.LoadAssetAtPath<SceneAsset>(ScenePath) != null)
            {
                throw new InvalidOperationException("Riding scene exists; edit it rather than overwriting it.");
            }
            if (!Application.isBatchMode && !EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) { return; }
            var scene = EditorSceneManager.OpenScene("Assets/CityRace/Content/Scenes/Smoke.unity");
            var road = GameObject.Find("Road");
            road.transform.position = new Vector3(0f, -0.1f, 45f);
            road.transform.localScale = new Vector3(8f, 0.2f, 120f);
            var asphalt = road.GetComponent<Renderer>().sharedMaterial;
            foreach (var root in scene.GetRootGameObjects())
            {
                if (root.name == "Sidewalk")
                {
                    root.transform.position = new Vector3(root.transform.position.x, 0.05f, 45f);
                    root.transform.localScale = new Vector3(1.4f, 0.3f, 120f);
                }
            }
            var laneMaterial = GameObject.Find("Lane marking").GetComponent<Renderer>().sharedMaterial;
            for (var z = 15; z < 105; z += 5)
            {
                var stripe = GameObject.CreatePrimitive(PrimitiveType.Cube);
                stripe.name = "Lane marking";
                stripe.transform.position = new Vector3(0f, 0.015f, z);
                stripe.transform.localScale = new Vector3(0.1f, 0.02f, 1.1f);
                stripe.GetComponent<Renderer>().sharedMaterial = laneMaterial;
            }
            foreach (var z in new[] { -15f, 105f })
            {
                var wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
                wall.name = "End boundary";
                wall.transform.position = new Vector3(0f, 0.6f, z);
                wall.transform.localScale = new Vector3(8f, 1.2f, 0.5f);
                wall.GetComponent<Renderer>().sharedMaterial = asphalt;
            }
            var bike = GameObject.Find("Bike marker");
            // Give the physics root unit scale so the rider and future visuals inherit clean transforms.
            var rider = GameObject.Find("Rider marker");
            var rootBike = new GameObject("Player Bike");
            rootBike.transform.position = bike.transform.position;
            bike.transform.SetParent(rootBike.transform, true);
            rider.transform.SetParent(rootBike.transform, true);
            UnityEngine.Object.DestroyImmediate(bike.GetComponent<Collider>());
            UnityEngine.Object.DestroyImmediate(rider.GetComponent<Collider>());
            var collider = rootBike.AddComponent<BoxCollider>();
            collider.size = new Vector3(0.65f, 0.7f, 1.4f);
            var body = rootBike.AddComponent<Rigidbody>();
            body.useGravity = false;
            body.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            body.interpolation = RigidbodyInterpolation.Interpolate;
            body.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            var input = rootBike.AddComponent<DragRideInput>();
            rootBike.AddComponent<BikeMotor>().Configure(input);
            var camera = Camera.main;
            camera.orthographicSize = 10f;
            camera.gameObject.AddComponent<RidingCamera>().Configure(rootBike.transform);

            var canvas = new GameObject("Ride instructions", typeof(Canvas), typeof(CanvasScaler));
            canvas.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvas.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(390f, 844f);
            scaler.matchWidthOrHeight = 0.5f;
            var safe = new GameObject("Safe area", typeof(RectTransform), typeof(RidingHint));
            safe.transform.SetParent(canvas.transform, false);
            var label = new GameObject("Drag hint", typeof(RectTransform), typeof(Text));
            label.transform.SetParent(safe.transform, false);
            var text = label.GetComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = 17;
            text.alignment = TextAnchor.MiddleCenter;
            text.color = Color.white;
            text.raycastTarget = false;
            var rect = label.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 0f);
            rect.anchorMax = new Vector2(1f, 0f);
            rect.pivot = new Vector2(0.5f, 0f);
            rect.offsetMin = new Vector2(8f, 20f);
            rect.offsetMax = new Vector2(-8f, 65f);
            safe.GetComponent<RidingHint>().Configure(text);
            Time.fixedDeltaTime = 0.02f;
            PlayerSettings.bundleVersion = "0.0.2";
            if (!EditorSceneManager.SaveScene(scene, ScenePath)) { throw new InvalidOperationException("Could not save Riding scene"); }
            EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(ScenePath, true) };
            AssetDatabase.SaveAssets();
            Debug.Log("Riding scene created; verify steering and braking on device.");
        }
    }
}
