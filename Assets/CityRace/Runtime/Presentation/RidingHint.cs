using UnityEngine;
using UnityEngine.UI;

namespace CityRace.Presentation
{
    public sealed class RidingHint : MonoBehaviour
    {
        [SerializeField] private Text _label;
        private Rect _lastSafeArea;
        public void Configure(Text label) { _label = label; }
        private void Start()
        {
            if (_label == null) { return; }
            // Stable key: riding.drag_hint. Only two prototype translations are needed now.
            _label.text = Application.systemLanguage == SystemLanguage.Vietnamese
                ? "Kéo để lái • Nhả tay để phanh" : "Drag to ride • Release to brake";
        }
        private void Update()
        {
            var safe = Screen.safeArea;
            if (safe == _lastSafeArea) { return; }
            _lastSafeArea = safe;
            var rect = (RectTransform)transform;
            rect.anchorMin = new Vector2(safe.xMin / Screen.width, safe.yMin / Screen.height);
            rect.anchorMax = new Vector2(safe.xMax / Screen.width, safe.yMax / Screen.height);
            rect.offsetMin = rect.offsetMax = Vector2.zero;
        }
    }
}
