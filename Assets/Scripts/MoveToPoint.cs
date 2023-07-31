using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

namespace DemoShooter
{
    internal class MoveToPoint : MonoBehaviour
    {
        [SerializeField] private float _speed;
        [SerializeField] private bool _notBypass;
        
        private UnitManager _unitManager;
        private GameEditor _gameEditor;

        private Vector3 _startPos;
        private Vector3 _targetPosition;
        private bool _hasTargetObject = false;
        private GameObject _targetObject;
        private NavMeshAgent _navMeshAgent;
        public NavMeshAgent NavMeshAgent => _navMeshAgent;

        private float _range;

        public delegate void RangeOutHandler();
        public event RangeOutHandler RangeOut;

        private bool _stopMoving;
        public bool IsStopMoving => _stopMoving;

        private void Awake()
        {
            _unitManager = Singleton<UnitManager>.instance;
            _gameEditor = Singleton<GameEditor>.instance;
            _navMeshAgent = GetComponent<NavMeshAgent>();
            if (_navMeshAgent != null)
            {
                _navMeshAgent.speed = _speed;
            }
        }

        public void SetTarget(GameObject target, float range = -1)
        {
            _startPos = transform.position;
            _range = range;
            _hasTargetObject = true;
            _targetObject = target;
        }
        public void SetTarget(Vector3 target, float range = -1)
        {
            _startPos = transform.position;
            _range = range;
            _hasTargetObject = false;
            _targetPosition = target;
        }

        public void SetSpeed(float speed)
        {
            _speed = speed;
            _navMeshAgent.speed = speed;
        }

        private void Update()
        {
            if (_gameEditor.EditMode)
            {
                if (_navMeshAgent != null)
                    _navMeshAgent.isStopped = true;
                return;
            }
            else
            {
                if (_navMeshAgent != null)
                    _navMeshAgent.isStopped = false;
            }

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
                RangeOut?.Invoke();
                if (gameObject != null)
                    _startPos = transform.position;
            }

            if (Vector3.Distance(_targetPosition, transform.position) < _speed * 0.02)
            {
                if (_navMeshAgent != null)
                    _navMeshAgent.SetDestination(_targetPosition);
                _stopMoving = true;
                return;
            }

            _stopMoving = false;

            if (_notBypass)
            {
                Vector3 lookDirection = _targetPosition - transform.position;
                lookDirection.y = 0f;
                Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 10 * Time.deltaTime);
                transform.position += (_targetPosition - transform.position).normalized * _speed * Time.deltaTime;
            }
            else
            {
                Vector3 lookDirection = _targetPosition - transform.position;
                lookDirection.y = 0f;
                Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 10 * Time.deltaTime);
                _navMeshAgent.SetDestination(_targetPosition);
            }
        }
    }
}