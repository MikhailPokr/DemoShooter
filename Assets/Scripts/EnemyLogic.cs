using Unity.VisualScripting;
using UnityEngine;

namespace DemoShooter
{
    internal class EnemyLogic : MonoBehaviour
    {
        private UnitManager _unitManager;
        private GameEditor _gameEditor;
        private NavMoving _navMoving;
        private Unit _unit;

        private void Start()
        {
            _unitManager = Singleton<UnitManager>.instance;
            _gameEditor = Singleton<GameEditor>.instance;
            _navMoving = GetComponent<NavMoving>();
            _unit = GetComponent<Unit>();
        }
        private void Update()
        {
            if (_gameEditor.EditMode)
                return;
            Unit playerUnit = _unitManager.GetNearestEnemy(transform.position, 200, true);
            if (playerUnit != null)
            {
                if (Vector3.Distance(transform.position, playerUnit.transform.position) < _unit.Wiapon.Range)
                {
                    _navMoving.SetTarget(transform.position);
                }
                else
                {
                    _navMoving.SetTarget(playerUnit.transform.position);
                }
            }
            else
            {
                _navMoving.SetTarget(transform.position);
            }
        }
    }
}