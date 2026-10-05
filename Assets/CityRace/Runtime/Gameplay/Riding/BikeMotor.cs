using UnityEngine;

namespace CityRace.Gameplay.Riding
{
    [RequireComponent(typeof(Rigidbody))]
    public sealed class BikeMotor : MonoBehaviour
    {
        [SerializeField] private DragRideInput _input;
        [SerializeField] private float _maxSpeedMetersPerSecond = 8f;
        [SerializeField] private float _acceleration = 5f;
        [SerializeField] private float _braking = 16f;
        [SerializeField] private float _turnDegreesPerSecond = 165f;
        private Rigidbody _body;
        private Vector2 _command;
        private bool _manualInput;
        public float Speed => new Vector2(_body.linearVelocity.x, _body.linearVelocity.z).magnitude;

        private void Awake() { _body = GetComponent<Rigidbody>(); }
        public void Configure(DragRideInput input) { _input = input; }

        // Used by simulation tests and future input adapters; no platform-specific movement.
        public void SetCommand(Vector2 command)
        {
            _manualInput = true;
            _command = Vector2.ClampMagnitude(command, 1f);
        }

        public void ResetPose(Vector3 position, Quaternion rotation)
        {
            _command = Vector2.zero;
            _manualInput = false;
            _body.linearVelocity = Vector3.zero;
            _body.angularVelocity = Vector3.zero;
            _body.position = position;
            _body.rotation = rotation;
        }

        private void FixedUpdate()
        {
            var command = _manualInput ? _command : _input != null ? _input.Command : Vector2.zero;
            var speed = Speed;
            var desired = command.sqrMagnitude > 0.0001f
                ? Quaternion.LookRotation(new Vector3(command.x, 0f, command.y)) : _body.rotation;
            var turnAngle = Quaternion.Angle(_body.rotation, desired);
            // Ease the throttle into sharp corners; straight-line top speed is unchanged.
            var cornerFactor = Mathf.Lerp(1f, 0.4f, Mathf.InverseLerp(20f, 100f, turnAngle));
            var target = command.magnitude * _maxSpeedMetersPerSecond * cornerFactor;
            var nextSpeed = Mathf.MoveTowards(speed, target, (target > speed ? _acceleration : _braking) * Time.fixedDeltaTime);
            var rotation = _body.rotation;
            if (command.sqrMagnitude > 0.0001f)
            {
                // Steering needs forward motion; never rotate in place or slide sideways.
                rotation = Quaternion.RotateTowards(rotation, desired,
                    _turnDegreesPerSecond * Mathf.Clamp01(speed / 2f) * Time.fixedDeltaTime);
            }
            _body.MoveRotation(rotation);
            _body.linearVelocity = rotation * Vector3.forward * nextSpeed;
        }

        private void OnDisable()
        {
            if (_body != null)
            {
                _body.linearVelocity = Vector3.zero;
                _body.angularVelocity = Vector3.zero;
            }
        }
    }
}
