using CityRace.Core;
using UnityEngine;

namespace CityRace.Gameplay.Riding
{
    public sealed class PracticeCourse : MonoBehaviour
    {
        [SerializeField] private BikeMotor _motor;
        [SerializeField] private DragRideInput _input;
        [SerializeField] private Vector3[] _checkpoints;
        private PracticeProgress _progress;
        private float _holdSeconds;
        public PracticeProgress Progress => _progress;
        public float RecoveryFraction => Mathf.Clamp01(_holdSeconds / 1.2f);
        public bool IsFinalApproach => _progress.NextCheckpoint == _checkpoints.Length - 1;
        public Vector3 NextPoint => _checkpoints[_progress.NextCheckpoint];

        public void Configure(BikeMotor motor, DragRideInput input, Vector3[] checkpoints)
        {
            _motor = motor;
            _input = input;
            _checkpoints = checkpoints;
        }

        private void Start() { Restart(); }

        public void Restart()
        {
            _progress = new PracticeProgress(_checkpoints.Length);
            _holdSeconds = 0f;
            _motor.enabled = true;
            PlaceAt(0);
        }

        private void Update()
        {
            if (_progress == null || _progress.Finished) { return; }
            var delta = _motor.transform.position - NextPoint;
            delta.y = 0f;
            // Narrow finish zone requires a deliberate stop at the company gate.
            var radius = _progress.NextCheckpoint == _checkpoints.Length - 1 ? 1.8f : 2.7f;
            _progress.Tick(Time.deltaTime, delta.sqrMagnitude <= radius * radius, _motor.Speed);
            if (_progress.Finished)
            {
                _input.Cancel();
                _motor.enabled = false;
                _holdSeconds = 0f;
                return;
            }
            var holdingStill = _input.IsTracking && _input.Command.sqrMagnitude < 0.0025f && _motor.Speed < 0.3f;
            _holdSeconds = holdingStill ? _holdSeconds + Time.deltaTime : 0f;
            if (_holdSeconds >= 1.2f) { Recover(); }
        }

        public void Recover()
        {
            if (_progress == null || _progress.Finished) { return; }
            PlaceAt(_progress.Recover(FindCheckpointBehindBike()));
            _holdSeconds = 0f;
        }

        private int FindCheckpointBehindBike()
        {
            var bestDistance = float.PositiveInfinity;
            var checkpoint = 0;
            // Project onto the authored route so backtracking cannot turn recovery into a shortcut.
            for (var i = 0; i < _checkpoints.Length - 1; i++)
            {
                var segment = _checkpoints[i + 1] - _checkpoints[i];
                var t = Mathf.Clamp01(Vector3.Dot(_motor.transform.position - _checkpoints[i], segment) / segment.sqrMagnitude);
                var distance = (_motor.transform.position - (_checkpoints[i] + segment * t)).sqrMagnitude;
                if (distance >= bestDistance) { continue; }
                bestDistance = distance;
                checkpoint = t < 0.05f ? Mathf.Max(0, i - 1) : i;
            }
            return checkpoint;
        }

        private void PlaceAt(int index)
        {
            var direction = _checkpoints[Mathf.Min(index + 1, _checkpoints.Length - 1)] - _checkpoints[index];
            direction.y = 0f;
            _motor.ResetPose(_checkpoints[index], Quaternion.LookRotation(direction.normalized));
            _input.Cancel();
        }

        private void OnApplicationFocus(bool focused) { if (!focused) { _holdSeconds = 0f; } }
        private void OnApplicationPause(bool paused) { if (paused) { _holdSeconds = 0f; } }
    }
}
