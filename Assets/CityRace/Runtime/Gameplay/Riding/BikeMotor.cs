using CityRace.Core;
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
        private Collider _collider;
        private Vector2 _command;
        private bool _manualInput;
        private float _potholeSeconds;
        private float _protectionSeconds;
        private float _potholeSpeedLimit;
        private Vector3 _wallNormal;
        private float _wallContactSeconds;
        // Minimum steering authority while pressed against a kerb, so the rider can always turn away.
        private const float BlockedSteerFactor = 0.5f;
        public int PotholeHits { get; private set; }
        public float PotholeFeedbackSeconds { get; private set; }
        public float PotholeWobble => Mathf.Clamp01(_potholeSeconds / PotholeResponse.SlowSeconds);
        public float Speed => new Vector2(_body.linearVelocity.x, _body.linearVelocity.z).magnitude;

        private void Awake() { _body = GetComponent<Rigidbody>(); _collider = GetComponent<Collider>(); }
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
            _potholeSeconds = 0f;
            _protectionSeconds = 0.5f;
            _wallContactSeconds = 0f;
            PotholeFeedbackSeconds = 0f;
            PotholeHits = 0;
            _body.linearVelocity = Vector3.zero;
            _body.angularVelocity = Vector3.zero;
            _body.position = position;
            _body.rotation = rotation;
        }

        public bool TryHitPothole()
        {
            if (!isActiveAndEnabled || !PotholeResponse.ShouldSlow(Speed, _protectionSeconds)) { return false; }
            _body.linearVelocity *= PotholeResponse.RetainedSpeedFraction;
            _potholeSpeedLimit = Speed;
            _potholeSeconds = PotholeResponse.SlowSeconds;
            _protectionSeconds = PotholeResponse.ProtectionSeconds;
            PotholeFeedbackSeconds = 1.2f;
            PotholeHits++;
            return true;
        }

        public bool TouchingWall => _wallContactSeconds > 0f;

        private void OnCollisionStay(Collision collision) { RecordWall(collision); }

        private void RecordWall(Collision collision)
        {
            // Road tiles sit at or below the bike's base; their seams can report sideways normals but are not kerbs.
            if (_collider != null && collision.collider.bounds.max.y <= _collider.bounds.min.y + 0.05f) { return; }
            var sum = Vector3.zero;
            for (var i = 0; i < collision.contactCount; i++)
            {
                var contact = collision.GetContact(i);
                var normal = contact.normal;
                if (Mathf.Abs(normal.y) >= 0.5f) { continue; }
                normal.y = 0f;
                // Orient away from the obstacle regardless of which side reports the contact.
                var away = _body.position - contact.point;
                away.y = 0f;
                if (Vector3.Dot(normal, away) < 0f) { normal = -normal; }
                sum += normal.normalized;
            }
            if (sum.sqrMagnitude < 0.0001f) { return; }
            _wallNormal = sum.normalized;
            _wallContactSeconds = 0.1f;
        }

        private void OnCollisionEnter(Collision collision)
        {
            RecordWall(collision);
            for (var i = 0; i < collision.contactCount; i++)
            {
                var normal = collision.GetContact(i).normal;
                // Road-tile contact is not a crash; only a substantial impact against a side face protects.
                if (Mathf.Abs(normal.y) < 0.5f && Mathf.Abs(Vector3.Dot(collision.relativeVelocity, normal)) > 3f)
                { _protectionSeconds = Mathf.Max(_protectionSeconds, 0.6f); }
            }
        }

        private void FixedUpdate()
        {
            _potholeSeconds = Mathf.Max(0f, _potholeSeconds - Time.fixedDeltaTime);
            _protectionSeconds = Mathf.Max(0f, _protectionSeconds - Time.fixedDeltaTime);
            PotholeFeedbackSeconds = Mathf.Max(0f, PotholeFeedbackSeconds - Time.fixedDeltaTime);
            _wallContactSeconds = Mathf.Max(0f, _wallContactSeconds - Time.fixedDeltaTime);
            var command = _manualInput ? _command : _input != null ? _input.Command : Vector2.zero;
            var speed = Speed;
            var desired = command.sqrMagnitude > 0.0001f
                ? Quaternion.LookRotation(new Vector3(command.x, 0f, command.y)) : _body.rotation;
            var turnAngle = Quaternion.Angle(_body.rotation, desired);
            // Ease the throttle into sharp corners; straight-line top speed is unchanged.
            var cornerFactor = Mathf.Lerp(1f, 0.4f, Mathf.InverseLerp(20f, 100f, turnAngle));
            var target = command.magnitude * _maxSpeedMetersPerSecond * cornerFactor;
            if (_potholeSeconds > 0f) { target = Mathf.Min(target, _potholeSpeedLimit); }
            var nextSpeed = Mathf.MoveTowards(speed, target, (target > speed ? _acceleration : _braking) * Time.fixedDeltaTime);
            var rotation = _body.rotation;
            if (command.sqrMagnitude > 0.0001f)
            {
                // Steering normally needs forward motion. Against a kerb the measured speed collapses,
                // so keep a minimum authority; otherwise the bike stays pinned to the wall indefinitely.
                var steer = Mathf.Clamp01(speed / 2f);
                if (TouchingWall) { steer = Mathf.Max(steer, BlockedSteerFactor); }
                rotation = Quaternion.RotateTowards(rotation, desired, _turnDegreesPerSecond * steer * Time.fixedDeltaTime);
            }
            _body.MoveRotation(rotation);
            var velocity = rotation * Vector3.forward * nextSpeed;
            if (TouchingWall)
            {
                // Scrape along the kerb instead of driving into it; the into-wall component is lost.
                var into = Vector3.Dot(velocity, _wallNormal);
                if (into < 0f) { velocity -= into * _wallNormal; }
            }
            _body.linearVelocity = velocity;
        }

        private void OnDisable()
        {
            _potholeSeconds = 0f;
            PotholeFeedbackSeconds = 0f;
            _wallContactSeconds = 0f;
            if (_body != null)
            {
                _body.linearVelocity = Vector3.zero;
                _body.angularVelocity = Vector3.zero;
            }
        }
    }
}
