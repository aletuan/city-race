using UnityEngine;

namespace CityRace.Presentation
{
    public sealed class RidingCamera : MonoBehaviour
    {
        [SerializeField] private Transform _target;
        [SerializeField] private Vector3 _offset = new Vector3(0f, 19f, -10f);
        [SerializeField] private float _pitch = 58f;
        public void Configure(Transform target) { _target = target; }
        private void LateUpdate()
        {
            if (_target == null) { return; }
            var desired = _target.position + _offset;
            transform.position = (desired - transform.position).sqrMagnitude > 225f
                ? desired : Vector3.Lerp(transform.position, desired, 1f - Mathf.Exp(-8f * Time.deltaTime));
            transform.rotation = Quaternion.Euler(_pitch, 0f, 0f);
        }
    }
}
