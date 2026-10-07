using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace CityRace.Editor
{
    // Renders the Practice main camera (with post-processing) to portrait PNGs in Logs/ for art review.
    // Edit-mode only; the camera transform is restored and the scene is not saved.
    public static class LookPreviewCapture
    {
        private static readonly (string name, Vector3 focus)[] Shots =
        {
            ("start", new Vector3(0f, 0f, 4f)),
            ("corners", new Vector3(14f, 0f, 30f)),
            ("finish", new Vector3(-4f, 0f, 60f)),
        };

        [MenuItem("City Race/Look/Capture Practice Preview")]
        public static void Capture()
        {
            EditorSceneManager.OpenScene(PracticeBootstrap.ScenePath);
            var camera = Camera.main;
            var follow = camera.GetComponent<CityRace.Presentation.RidingCamera>();
            var so = follow != null ? new SerializedObject(follow) : null;
            var offset = so != null ? so.FindProperty("_offset").vector3Value : new Vector3(0f, 19f, -10f);
            var pitch = so != null ? so.FindProperty("_pitch").floatValue : 58f;
            var position = camera.transform.position;
            var rotation = camera.transform.rotation;
            Directory.CreateDirectory("Logs");
            var target = new RenderTexture(1290, 2796, 24, RenderTextureFormat.ARGB32) { antiAliasing = 1 };
            camera.allowHDR = true;
            try
            {
                foreach (var (name, focus) in Shots)
                {
                    camera.transform.SetPositionAndRotation(focus + offset, Quaternion.Euler(pitch, 0f, 0f));
                    camera.targetTexture = target;
                    camera.Render();
                    RenderTexture.active = target;
                    var image = new Texture2D(target.width, target.height, TextureFormat.RGB24, false);
                    image.ReadPixels(new Rect(0, 0, target.width, target.height), 0, 0);
                    image.Apply();
                    File.WriteAllBytes($"Logs/look-{name}.png", image.EncodeToPNG());
                    Object.DestroyImmediate(image);
                }
            }
            finally
            {
                camera.targetTexture = null;
                RenderTexture.active = null;
                Object.DestroyImmediate(target);
                camera.transform.SetPositionAndRotation(position, rotation);
            }
            Debug.Log("Look previews written to Logs/look-*.png (editor render, not device evidence).");
        }
    }
}
