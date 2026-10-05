using UnityEngine;

namespace CityRace.Gameplay.Riding
{
    [RequireComponent(typeof(BoxCollider))]
    public sealed class Pothole : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            var body = other.attachedRigidbody;
            if (body != null && body.TryGetComponent<BikeMotor>(out var motor)) { motor.TryHitPothole(); }
        }
    }
}
