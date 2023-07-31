using Unity.VisualScripting;
using UnityEngine;

namespace DemoShooter
{
    [Singleton]
    internal class ClickManager : MonoBehaviour, ISingleton
    {
        private UnitManager _unitManager;
        private GameEditor _gameEditor;

        public delegate void FloorClickHandler(Vector3 pos);
        public static event FloorClickHandler FloorClick;

        private void Start()
        {
            _unitManager = Singleton<UnitManager>.instance;
            _gameEditor = Singleton<GameEditor>.instance;
        }

        private void Update()
        {
            if (Input.GetMouseButtonUp(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                Unit unit;

                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    if (hit.collider != Singleton<UnitManager>.instance.Floor)
                    {
                        unit = hit.collider.gameObject.GetComponentInParent<Unit>();
                    }
                    else
                    {
                        unit = _unitManager.GetClick(hit.point);
                    }

                    if (unit != null)
                    {
                        if (_gameEditor.EditMode)
                        {
                            unit.KillUnit();
                            return;
                        }
                        if (unit != _unitManager.CurrentUnit)
                        {
                            if (unit.IsEnemy)
                            {
                                _unitManager.CurrentUnit.SetTarget(unit);
                            }
                            else
                            {
                                _unitManager.SetSelected(unit);
                            }
                            return;
                        }
                    }
                    else if (_gameEditor.EditMode)
                    {
                        Spawner spawner = hit.collider.GetComponent<Spawner>();
                        if (spawner != null)
                        {
                            Destroy(spawner.gameObject);
                            return;
                        }
                        
                    }

                    FloorClick?.Invoke(hit.point);
                    _unitManager.NextUnit();
                }
            }
        }
    }
}