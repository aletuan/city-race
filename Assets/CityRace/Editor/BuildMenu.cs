using UnityEditor;

namespace CityRace.Editor
{
    // Editor menu wrappers for the batch build entry points (same code path as tools/unity_project.py).
    public static class BuildMenu
    {
        [MenuItem("City Race/Build/Export iOS Xcode Project")]
        public static void ExportIos() => CityRaceBootstrap.ExportIos();
    }
}
