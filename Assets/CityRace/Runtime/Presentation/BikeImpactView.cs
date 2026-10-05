using CityRace.Gameplay.Riding;
using UnityEngine;

namespace CityRace.Presentation
{
    public sealed class BikeImpactView : MonoBehaviour
    {
        [SerializeField] private BikeMotor _motor;
        private Vector3 _restPosition;
        private Quaternion _restRotation;
        public void Configure(BikeMotor motor) { _motor = motor; }
        private void Awake()
        {
            _restPosition = transform.localPosition;
            _restRotation = transform.localRotation;
        }
        private void LateUpdate()
        {
            var amplitude = _motor.PotholeWobble;
            var wave = Mathf.Sin(Time.time * 45f) * amplitude;
            // Only visual children wobble; collision shape and steering remain under the motor.
            transform.localRotation = _restRotation * Quaternion.Euler(0f, 0f, wave * 7f);
            transform.localPosition = _restPosition + Vector3.up * (Mathf.Abs(wave) * .08f);
        }
        private void OnDisable()
        {
            transform.localPosition = _restPosition;
            transform.localRotation = _restRotation;
        }
    }
}
