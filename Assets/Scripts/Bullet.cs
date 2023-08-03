using UnityEngine;

namespace DemoShooter
{
    internal class Bullet : MonoBehaviour
    {
        [SerializeField] private float _speed;
        private bool _enemyUnit;
        private int _damage;

        private Vector3 _startPos;
        private Vector3 _targetPosition;
        private bool _hasTargetObject = false;
        private GameObject _targetObject;
        private float _range;

        public void SetTarget(GameObject target, float range = -1)
        {
            _startPos = transform.position;
            _range = range;
            _hasTargetObject = true;
            _targetObject = target;
        }
        
        public void SetValues(bool enemyUnit, int damage, int speed = 2)
        {
            _enemyUnit = enemyUnit;
            _damage = damage;
            _speed = speed;
        }

        private void OnTriggerEnter(Collider other)
        {
            Unit unit = other.GetComponentInParent<Unit>();
            if (unit != null && unit.IsEnemy != _enemyUnit)
            {
                unit.ChangeHp(-_damage);
                Destroy(gameObject);
                return;
            }
        }


        private void Update()
        {
            if (_hasTargetObject && _targetObject != null)
            {
                _targetPosition = _targetObject.transform.position;
            }
            else if (_hasTargetObject && _targetObject == null)
            {
                _hasTargetObject = false;
                Vector3 direction = (_targetPosition - _startPos).normalized;
                _targetPosition = _targetPosition + direction * _range;
            }

            if (_range != -1 && Vector3.Distance(_startPos, transform.position) >= _range)
            {
                Destroy(gameObject);
            }

            Vector3 moveDirection = (_targetPosition - transform.position).normalized;
            float distance = _speed * Time.deltaTime;

            if (Physics.Raycast(transform.position, moveDirection, out RaycastHit hit, distance))
            {
                Destroy(gameObject);
            }
            else
            {
                transform.position += moveDirection * distance;
            }
        }
    }
}