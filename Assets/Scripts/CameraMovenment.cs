using Unity.VisualScripting;
using UnityEngine;

namespace DemoShooter
{
    [Singleton]
    internal class CameraMovenment : MonoBehaviour, ISingleton
    {
        private GameObject _target;
        [SerializeField] private Vector3 _rotation;
        [SerializeField] private float _forward;
        [SerializeField] private float _up;
        [SerializeField] private float _movementSpeed = 5f;

        public void SetTarget(GameObject target)
        {
            Camera.main.orthographic = false;
            _target = target;
            transform.rotation = Quaternion.Euler(_rotation);
        }
        public void SetEditMode(GameObject target, Vector3 rotation)
        {
            Camera.main.orthographic = true;
            Camera.main.orthographicSize = 1;

            _target = target;
            transform.rotation = Quaternion.Euler(rotation);
        }

        private void Update()
        {
            if (_target == null)
                return;

            Vector3 targetPosition = _target.transform.position - transform.rotation * Vector3.forward * _forward + Vector3.up * _up;

            transform.position = Vector3.Lerp(transform.position, targetPosition, _movementSpeed * Time.deltaTime);
        }
    }
}