using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace DemoShooter
{
    internal class LogicPoint : MonoBehaviour
    {
        private UnitManager _unitManager;

        private float _danger = 0;
        public float Danger => _danger;

        private static List<LogicPoint> _logicPointslist = new();
        private static Dictionary<LogicPoint, Unit> _unitsForPoint = new();


        private void Awake()
        {
            _unitManager = Singleton<UnitManager>.instance;
        }
        private void OnEnable()
        {
            foreach (var point in _logicPointslist) 
            {
                if (Vector3.Distance(point.transform.position, transform.position) <= 0.1f)
                {
                    point.gameObject.SetActive(false);
                    return;
                }
            }
            _logicPointslist.Add(this);
            _unitsForPoint.Add(this, null);
        }

        public bool Check(Unit newUnit) => _unitsForPoint[this] == null || _unitsForPoint[this] == newUnit;
        public static LogicPoint CheckCurrent(Unit unit) => _unitsForPoint.Values.Contains(unit) ? _unitsForPoint.Keys.First(x => _unitsForPoint[x] == unit) : null;
        public static void SetUnit(LogicPoint point, Unit unit)
        {
            _unitsForPoint[point] = unit;
        }

        private void Update()
        {
            List<Unit> units = _unitManager.GetNearestUnits(transform.position);

            _danger = 0;

            foreach (Unit unit in units)
            {
                if (unit.IsEnemy)
                    continue;

                Vector3 directionToUnit = unit.transform.position - transform.position;

                RaycastHit[] hits = Physics.RaycastAll(transform.position, directionToUnit);

                float coef = 1;
                foreach (RaycastHit hit in hits)
                {
                    Barrier barrier = hit.collider.GetComponent<Barrier>();
                    if (barrier != null)
                    {
                        coef *= 1 - barrier.Permeability;
                    }
                }

                _danger += coef;
            }
        }

        private void OnDisable()
        {
            if (_logicPointslist.Contains(this))
                _logicPointslist.Remove(this);
            _unitsForPoint.Remove(this);
        }
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (gameObject.activeSelf)
                Gizmos.color = new Color(0, 1, 0, 0.9f);
            else
                Gizmos.color = new Color(1, 0, 0, 0.9f);
            Gizmos.DrawSphere(transform.position, 0.02f);
        }
#endif
    }
}