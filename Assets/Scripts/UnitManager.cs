using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace DemoShooter
{
    [Singleton]
    internal class UnitManager : MonoBehaviour, ISingleton
    {
        [SerializeField] private Collider _floor;
        [SerializeField] private float _yPos;
        public float YPos => _yPos;
        public Collider Floor => _floor;
        [Space] 
        [SerializeField] private float _clickRadius;
        public float ClickRadius => _clickRadius;
        [Space]
        [Header("Meterials")]
        public Material UnitDefault;
        public Material UnitSelected;
        public Material UnitEnemy;
        public Material UnitWiapon1;
        public float ScaleWiapon1; 
        public Material UnitWiapon2;
        public float ScaleWiapon2;
        public Material UnitWiapon3;
        public float ScaleWiapon3;

        private List<Unit> _units;


        private Unit _currentUnit;
        public Unit CurrentUnit => _currentUnit;

        public delegate void SelectedUnitChangedHandler(Unit unit);
        public event SelectedUnitChangedHandler SelectedUnitChanged;


        private void Awake()
        {
            _units = new List<Unit>();

            Unit.UnitCreate += OnUnitCreate;
            Unit.UnitDestroy += OnUnitDestroy;
        }

        public void NextUnit()
        {
            Unit newSelect = null;

            if (_units == null || _units.Count == 0)
                return;
            List<Unit> units = _units.FindAll(x => !x.IsEnemy);
            if (units.Count == 0) 
                return;

            if (_currentUnit != null)
            {
                int inx = units.IndexOf(_currentUnit);
                if (inx < units.Count - 1)
                    newSelect = units[inx + 1];
                else
                    newSelect = units[0];
            }
            else
                newSelect = units[0];

            SetSelected(newSelect);
        }
        public void SetSelected(Unit newSelect)
        {
            _currentUnit = newSelect;
            if (!Singleton<GameEditor>.instance.EditMode)
            {
                Singleton<CameraMovenment>.instance.SetTarget(_currentUnit.gameObject);
            }
            SelectedUnitChanged?.Invoke(_currentUnit);
        }

        public Unit GetClick(Vector3 position)
        {
            float min = -1;
            Unit nearest = null;
            Unit me = null;
            foreach (var unit in _units)
            {
                if (unit.IsEnemy)
                    continue;
                float distance = Vector3.Distance(unit.transform.position, position);
                if (distance > _clickRadius)
                    continue;
                if (distance == 0)
                {
                    me = unit;
                    continue;
                }
                if (min == -1 || distance <= min)
                {
                    min = distance;
                    nearest = unit;
                }
            }
            if (nearest == null)
                return me;
            else 
                return nearest;
        }
        public Unit GetNearestEnemy(Vector3 unitPos, float range, bool imEnemy)
        {
            float min = -1;
            Unit nearest = null;
            foreach (var otherUnit in _units)
            {
                if (imEnemy == otherUnit.IsEnemy)
                    continue;
                float distance = Vector3.Distance(unitPos, otherUnit.transform.position);
                if (distance > range)
                    continue;
                if (min == -1 || distance <= min)
                {
                    min = distance;
                    nearest = otherUnit;
                }
            }
            return nearest;
        }
        public Vector3 GetObstacles(Vector3 pos)
        {
            Vector3 minVector = Vector3.zero;
            foreach (Unit u in _units)
            {
                if (u.gameObject.transform.position == pos)
                    continue;
                if (Vector3.Distance(u.transform.position, pos) <= _clickRadius)
                {
                    Vector3 vector = - pos + u.transform.position;
                    if (minVector == Vector3.zero || vector.magnitude < minVector.magnitude)
                    {
                        minVector = vector;
                    }
                }
            }
            return minVector;
        }


        private void OnUnitCreate(Unit unit)
        {
            _units.Add(unit);
            if (_units.FindAll(x => !x.IsEnemy).Count == 1 && !unit.IsEnemy)
            {
                SetSelected(unit);
            }
        }
        private void OnUnitDestroy(Unit unit)
        {
            if (_currentUnit == unit)
            {
                NextUnit();
            }
            _units.Remove(unit);
        }
    }
}