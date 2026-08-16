using UnityEngine;

namespace Settings
{
    public class CameraManager : MonoBehaviour
    {
        [SerializeField] private Vector3 _offset = new Vector3(0f, 10f, -10f);
        [SerializeField] private float _smoothSpeed = 5f;

        private Transform _target;

        public void SetTarget(Transform target)
        {
            _target = target;

            transform.position = _target.position + _offset;
        }

        private void LateUpdate()
        {
            if (_target == null) return;

            Vector3 desiredPosition = _target.position + _offset;
            transform.position = Vector3.Lerp(transform.position, desiredPosition, _smoothSpeed * Time.deltaTime);
        }
    }
}
