using Unity.VisualScripting;
using UnityEngine;

namespace DemoShooter
{
    internal class Unit : MonoBehaviour
    {
        [SerializeField] private int _hp;
        [Space]
        [SerializeField] private Collider _collider;
        [SerializeField] private MoveToPoint _moveToPoint;
        [SerializeField] private MeshRenderer _meshRenderer;
        [Space]
        [SerializeField] private Wiapon _wiapon;
        public Wiapon Wiapon => _wiapon;

        private Unit _target;

        private GameEditor _gameEditor;
        private UnitManager _unitManager;

        public bool ImSelected => _unitManager.CurrentUnit == this;

        [Space]
        [SerializeField] private bool _imEnemy;
        public bool IsEnemy => _imEnemy;

        public delegate void UnitCreateHendler(Unit unit);
        public static event UnitCreateHendler UnitCreate;
        public delegate void UnitDestroyHendler(Unit unit);
        public static event UnitDestroyHendler UnitDestroy;

        private void Start()
        {
            _unitManager = Singleton<UnitManager>.instance;
            _gameEditor = Singleton<GameEditor>.instance;
            _moveToPoint.SetTarget(transform.position);

            ClickManager.FloorClick += OnFloorClick;
            _unitManager.SelectedUnitChanged += OnSelectedUnitChanged;

            UnitCreate?.Invoke(this);
        }
        private void Update()
        {
            if (_gameEditor.EditMode)
                return;
            if (_moveToPoint.IsStopMoving)
            {
                Unit enemy = _unitManager.GetNearestEnemy(transform.position, _wiapon.Range, _imEnemy);
                if (_target != null && Vector3.Distance(transform.position, _target.transform.position) <= _wiapon.Range)
                    enemy = _target;
                if (enemy != null)
                {
                    Vector3 lookDirection = enemy.transform.position - transform.position;
                    lookDirection.y = 0f;
                    Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
                    transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 10 * Time.deltaTime);

                    _wiapon.Shoot(this, enemy);
                }
            }
        }
        public void SetValues(bool enemy, int index ,int hp, int damage, float range, float cooldown, float speed)
        {
            _hp = hp;
            _wiapon.SetValues(range, damage, cooldown, index);
            _moveToPoint.SetSpeed(speed);
        }

        private void OnSelectedUnitChanged(Unit unit)
        {
            if (_meshRenderer == null)
                return;
            if (unit == this)
            {
                _meshRenderer.material = _unitManager.UnitSelected;
            }
            else
            {
                _meshRenderer.material = IsEnemy ? _unitManager.UnitEnemy : _unitManager.UnitDefault;
            }
        }
        private void OnFloorClick(Vector3 pos)
        {
            if (ImSelected && !_gameEditor.EditMode)
            {
                pos = new Vector3(pos.x, transform.position.y, pos.z);

                _moveToPoint.SetTarget(pos);
            }
        }


        public void SetTarget(Unit unit)
        {
            _target = unit;
        }

        public void ChangeHp(int delta)
        {
            _hp += delta;

            if (_hp <= 0)
                KillUnit();
        }
        public void KillUnit()
        {
            ClickManager.FloorClick -= OnFloorClick;
            _unitManager.SelectedUnitChanged -= OnSelectedUnitChanged;
            UnitDestroy?.Invoke(this);
            Destroy(gameObject);
        }
    }
}

