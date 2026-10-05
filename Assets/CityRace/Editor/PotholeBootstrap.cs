using System;
using CityRace.Gameplay.Riding;
using CityRace.Presentation;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace CityRace.Editor
{
    public static class PotholeBootstrap
    {
        [MenuItem("City Race/Bootstrap/Add Practice Potholes")]
        public static void AddToPractice()
        {
            if (!Application.isBatchMode && !EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) { return; }
            var scene = EditorSceneManager.OpenScene(PracticeBootstrap.ScenePath);
            if (UnityEngine.Object.FindFirstObjectByType<Pothole>() != null)
            { throw new InvalidOperationException("Potholes already exist; refusing duplicate scene migration."); }
            var shader = Shader.Find("Universal Render Pipeline/Unlit");
            if (shader == null) { throw new InvalidOperationException("URP Unlit shader is missing."); }
            var dark = MakeMaterial("PotholeDark", new Color(.07f,.055f,.04f), shader);
            var rim = MakeMaterial("PotholeRim", new Color(.40f,.31f,.22f), shader);
            var warning = MakeMaterial("PotholeWarning", new Color(1f,.68f,.12f), shader);
            AddHole(new Vector3(0,0,8), dark,rim,warning);
            AddHole(new Vector3(18.7f,0,34), dark,rim,warning);
            var motor = UnityEngine.Object.FindFirstObjectByType<BikeMotor>();
            var visuals = new GameObject("Bike visuals");
            visuals.transform.SetParent(motor.transform,false);
            GameObject.Find("Bike marker").transform.SetParent(visuals.transform,true);
            GameObject.Find("Rider marker").transform.SetParent(visuals.transform,true);
            visuals.AddComponent<BikeImpactView>().Configure(motor);
            PlayerSettings.bundleVersion = "0.0.4";
            if (!EditorSceneManager.SaveScene(scene)) { throw new InvalidOperationException("Could not save potholes."); }
            AssetDatabase.SaveAssets();
            Debug.Log("Added two authored potholes; Practice scene GUID and existing route preserved.");
        }

        private static Material MakeMaterial(string name, Color color, Shader shader)
        {
            var path = "Assets/CityRace/Content/Materials/" + name + ".mat";
            var material = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (material != null) { return material; }
            material = new Material(shader) { name = name };
            material.SetColor("_BaseColor",color);
            AssetDatabase.CreateAsset(material,path);
            return material;
        }

        private static void AddHole(Vector3 position, Material dark, Material rim, Material warning)
        {
            var root = new GameObject("Pothole"); root.transform.position = position;
            var trigger = root.AddComponent<BoxCollider>();
            trigger.isTrigger = true; trigger.center = new Vector3(0,.45f,0); trigger.size = new Vector3(1.35f,1f,.9f);
            root.AddComponent<Pothole>();
            Shape(PrimitiveType.Cylinder,"Broken rim",root.transform,new Vector3(0,.025f,0),new Vector3(1.9f,.015f,1.5f),rim);
            Shape(PrimitiveType.Cylinder,"Dark depression",root.transform,new Vector3(0,.055f,0),new Vector3(1.7f,.015f,1.3f),dark);
            for (var i = 0; i < 8; i++)
            {
                var angle = i * Mathf.PI / 4f;
                var stone = Shape(PrimitiveType.Cube,"Broken edge",root.transform,
                    new Vector3(Mathf.Cos(angle)*.88f,.065f,Mathf.Sin(angle)*.68f),new Vector3(.23f,.07f,.14f),rim);
                stone.transform.localRotation = Quaternion.Euler(0,i*47,0);
            }
            // Painted warning strokes are visible before reaching the actual trigger footprint.
            foreach (var x in new[] { -.8f,.8f })
            {
                var mark = Shape(PrimitiveType.Cube,"Approach marking",root.transform,new Vector3(x,.025f,-2.5f),new Vector3(.13f,.03f,.8f),warning);
                mark.transform.localRotation = Quaternion.Euler(0,x>0?-25:25,0);
            }
        }

        private static GameObject Shape(PrimitiveType shape,string name,Transform parent,Vector3 position,Vector3 scale,Material material)
        {
            var obj = GameObject.CreatePrimitive(shape); obj.name = name;
            obj.transform.SetParent(parent,false); obj.transform.localPosition = position; obj.transform.localScale = scale;
            UnityEngine.Object.DestroyImmediate(obj.GetComponent<Collider>());
            obj.GetComponent<Renderer>().sharedMaterial = material;
            obj.isStatic = true;
            return obj;
        }
    }
}
