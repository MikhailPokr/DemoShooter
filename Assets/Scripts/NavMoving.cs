using Unity.AI.Navigation;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

namespace DemoShooter
{
    internal class NavMoving : MonoBehaviour
    {
        [SerializeField] private float _speed;
        [SerializeField] private bool _notBypass;

        private GameEditor _gameEditor;

        private Vector3 _targetPosition;
        private bool _hasTargetObject = false;
        private GameObject _targetObject;
        private NavMeshAgent _navMeshAgent;
        public NavMeshAgent NavMeshAgent => _navMeshAgent;

        private float _range;

        public bool IsStopMoving => _navMeshAgent.path.corners.Length < 2 || _navMeshAgent.isStopped;

        private void Awake()
        {
            _gameEditor = Singleton<GameEditor>.instance;
            _navMeshAgent = GetComponent<NavMeshAgent>();
            if (_navMeshAgent != null)
            {
                _navMeshAgent.speed = _speed;
            }
        }

        public bool SetTarget(Vector3 target, float range = -1)
        {
            _range = range;
            _hasTargetObject = false;
            _targetPosition = target;
            if (_navMeshAgent != null)
            {
                NavMeshPath path = new NavMeshPath();
                if (!NavMesh.CalculatePath(transform.position, _targetPosition, NavMesh.AllAreas, path))
                {
                    _targetPosition = transform.position;
                    Stop();
                    return false;
                }
            }
            return true;
        }

        public void Stop()
        {
            if (_navMeshAgent != null)
            {
                _navMeshAgent.ResetPath();
            }
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
                Vector3 direction = (_targetPosition - transform.position).normalized;
                _targetPosition = _targetPosition + direction * _range;
            }

            if (Vector3.Distance(transform.position, _targetPosition) <= _navMeshAgent.radius)
            {
                _navMeshAgent.path.ClearCorners();
                _navMeshAgent.isStopped = true;
                return;
            }

            _navMeshAgent.isStopped = false;
            _navMeshAgent.SetDestination(_targetPosition);
        }
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Vector3[] corners = _navMeshAgent.path.corners;
            foreach (Vector3 cor in corners)
            {
                Gizmos.DrawSphere(cor, 0.06f);
            }
        }
#endif
    }
}