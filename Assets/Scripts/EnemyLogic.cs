using System.Collections.Generic;
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
                    LogicPoint logicPoint = GetBorderPoint(playerUnit.transform.position);
                    if (logicPoint != null)
                    {
                        _navMoving.SetTarget(logicPoint.transform.position);
                    }
                    else
                    {
                        _navMoving.SetTarget(transform.position);
                    }
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

        public LogicPoint GetBorderPoint(Vector3 enemy)
        {
            LogicPoint oldPoint = LogicPoint.CheckCurrent(_unit);

            Collider[] points = Physics.OverlapSphere(enemy, _unit.Wiapon.Range);

            List<LogicPoint> pointsList = new(); 
            foreach (Collider coll in points)
            {
                LogicPoint point = coll.GetComponent<LogicPoint>();
                if (point != null && point.gameObject.activeSelf)
                { 
                    pointsList.Add(point);
                }
            }
            
            float minDanger = -1;
            if (oldPoint != null && pointsList.Contains(oldPoint))
            {
                minDanger = oldPoint.Danger;
            }
            LogicPoint logicPoint = null;

            foreach (LogicPoint point in pointsList)
            {
                if (!point.Check(_unit))
                    continue;
                if (point.Danger < minDanger || minDanger == -1)
                {
                    minDanger = point.Danger;
                    logicPoint = point;
                }
            }
            if (logicPoint != null) 
            {
                LogicPoint.SetUnit(logicPoint, _unit);
            }

            return logicPoint;
        }
    }
}