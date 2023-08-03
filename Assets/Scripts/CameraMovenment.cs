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

        public void SetSpeed(float speed)
        {
            _movementSpeed = speed;
        }

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


        Vector3 _velocity;
        private void Update()
        {
            if (_target == null)
                return;
            Vector3 targetPosition = _target.transform.position - transform.rotation * Vector3.forward * _forward + Vector3.up * _up;
            if (_movementSpeed == 0)
                _movementSpeed = 0.0001f;
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref _velocity, 1/_movementSpeed);
        }
    }
}