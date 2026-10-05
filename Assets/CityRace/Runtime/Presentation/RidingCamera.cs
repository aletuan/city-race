using UnityEngine;

namespace CityRace.Presentation
{
    public sealed class RidingCamera : MonoBehaviour
    {
        [SerializeField] private Transform _target;
        private Vector3 _offset = new Vector3(0f, 19f, -10f);
        public void Configure(Transform target) { _target = target; }
        private void LateUpdate()
        {
            if (_target == null) { return; }
            var desired = _target.position + _offset;
            transform.position = Vector3.Lerp(transform.position, desired, 1f - Mathf.Exp(-8f * Time.deltaTime));
            transform.rotation = Quaternion.Euler(58f, 0f, 0f);
        }
    }
}
