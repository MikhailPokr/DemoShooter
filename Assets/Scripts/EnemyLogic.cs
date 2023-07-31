using Unity.VisualScripting;
using UnityEngine;

namespace DemoShooter
{
    internal class EnemyLogic : MonoBehaviour
    {
        private UnitManager _unitManager;
        private GameEditor _gameEditor;
        private MoveToPoint _moveToPoint;
        private Unit _unit;

        private void Start()
        {
            _unitManager = Singleton<UnitManager>.instance;
            _gameEditor = Singleton<GameEditor>.instance;
            _moveToPoint = GetComponent<MoveToPoint>();
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
                    _moveToPoint.SetTarget(transform.position);
                }
                else
                {
                    _moveToPoint.SetTarget(playerUnit.transform.position);
                }
            }
        }
    }
}