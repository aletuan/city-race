using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace CityRace.Gameplay.Riding
{
    public sealed class DragRideInput : MonoBehaviour
    {
        [SerializeField] private float _radiusFraction = 0.18f;
        [SerializeField] private float _deadZone = 0.12f;
        private Vector2 _origin;
        private bool _tracking;
        private bool _waitForRelease;
        private bool _wasDown;
        private int _touchId;
        private int _width;
        private int _height;
        public Vector2 Command { get; private set; }
        public bool IsTracking => _tracking;
        public Vector2 Origin => _origin;
        public float Radius => Mathf.Min(Screen.safeArea.width, Screen.safeArea.height) * _radiusFraction;

        private void Update()
        {
            var touch = Touchscreen.current;
            var mouse = Mouse.current;
            var down = false;
            var position = Vector2.zero;
            var id = -1;
            if (touch != null)
            {
                var foundTrackedFinger = false;
                foreach (var finger in touch.touches)
                {
                    if (!finger.press.isPressed) { continue; }
                    if (!down || (_tracking && finger.touchId.ReadValue() == _touchId))
                    {
                        position = finger.position.ReadValue();
                        id = finger.touchId.ReadValue();
                    }
                    down = true;
                    if (finger.touchId.ReadValue() == _touchId) { foundTrackedFinger = true; }
                }
                // Releasing the controlling finger must brake, even if another finger stays down.
                if (_tracking && !foundTrackedFinger) { Cancel(); }
            }
            else if (mouse != null)
            {
                down = mouse.leftButton.isPressed;
                position = mouse.position.ReadValue();
            }
            if (_width != Screen.width || _height != Screen.height)
            {
                Cancel();
                _width = Screen.width;
                _height = Screen.height;
            }
            if (!down)
            {
                _tracking = false;
                _waitForRelease = false;
                _wasDown = false;
                Command = Vector2.zero;
                return;
            }
            if (_waitForRelease)
            {
                return;
            }
            if (!_wasDown)
            {
                var safe = Screen.safeArea;
                var overUi = EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(id);
                _tracking = safe.Contains(position) && position.y < safe.yMin + safe.height * 0.65f && !overUi;
                _origin = position;
                _touchId = id;
            }
            _wasDown = true;
            if (!_tracking || id != _touchId)
            {
                Cancel();
                return;
            }
            var drag = Vector2.ClampMagnitude((position - _origin) / Mathf.Max(1f, Radius), 1f);
            var amount = Mathf.InverseLerp(_deadZone, 1f, drag.magnitude);
            Command = drag.sqrMagnitude > 0f ? drag.normalized * amount : Vector2.zero;
        }

        public void Cancel()
        {
            Command = Vector2.zero;
            _tracking = false;
            _waitForRelease = true;
        }

        private void OnApplicationFocus(bool focused)
        {
            if (!focused) { Cancel(); }
        }

        private void OnApplicationPause(bool paused)
        {
            if (paused) { Cancel(); }
        }

        private void OnDisable() { Cancel(); }
    }
}
