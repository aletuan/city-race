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
        [SerializeField] private float _turnDegreesPerSecond = 135f;
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

        private void FixedUpdate()
        {
            var command = _manualInput ? _command : _input != null ? _input.Command : Vector2.zero;
            var speed = Speed;
            var target = command.magnitude * _maxSpeedMetersPerSecond;
            var nextSpeed = Mathf.MoveTowards(speed, target, (target > speed ? _acceleration : _braking) * Time.fixedDeltaTime);
            var rotation = _body.rotation;
            if (command.sqrMagnitude > 0.0001f)
            {
                var desired = Quaternion.LookRotation(new Vector3(command.x, 0f, command.y));
                // Steering needs forward motion; never rotate in place or slide sideways.
                rotation = Quaternion.RotateTowards(rotation, desired,
                    _turnDegreesPerSecond * Mathf.Clamp01(speed / 2f) * Time.fixedDeltaTime);
            }
            _body.MoveRotation(rotation);
            _body.linearVelocity = rotation * Vector3.forward * nextSpeed;
        }

        private void OnDisable()
        {
            if (_body != null) { _body.linearVelocity = Vector3.zero; }
        }
    }
}
