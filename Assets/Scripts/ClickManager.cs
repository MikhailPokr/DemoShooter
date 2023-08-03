using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.PlayerSettings;

namespace DemoShooter
{
    [Singleton]
    internal class ClickManager : MonoBehaviour, ISingleton
    {
        private UnitManager _unitManager;
        private GameEditor _gameEditor;

        public delegate void FloorClickHandler(Vector3 pos, int button);
        public static event FloorClickHandler FloorClick;

        [Space]
        [SerializeField] private Vector2 _area;

        private void Start()
        {
            _unitManager = Singleton<UnitManager>.instance;
            _gameEditor = Singleton<GameEditor>.instance;
        }

        private void Update()
        {
            if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1))
            {
                int button = Input.GetMouseButtonUp(0) ? 0 : 1;

                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                Unit unit = null;

                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    if (Mathf.Abs(hit.point.x) > _area.x / 2 || Mathf.Abs(hit.point.z) > _area.y / 2)
                        return;

                    if (hit.collider != Singleton<UnitManager>.instance.Floor)
                    {
                        unit = hit.collider.gameObject.GetComponentInParent<Unit>();
                    }

                    if (unit != null)
                    {
                        if (_gameEditor.EditMode && button == 1)
                        {
                            unit.KillUnit();
                            return;
                        }
                        if (unit != _unitManager.CurrentUnit && button == 1)
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
                    else if (_gameEditor.EditMode && button == 1)
                    {
                        Spawner spawner = hit.collider.GetComponent<Spawner>();
                        if (spawner != null)
                        {
                            Destroy(spawner.gameObject);
                            return;
                        }
                        Barrier barrier = hit.collider.GetComponent<Barrier>();
                        if (barrier != null)
                        {
                            Destroy(barrier.gameObject);
                            return;
                        }
                    }

                    FloorClick?.Invoke(hit.point, button);
                }
            }
        }
#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(0.1f, 0.1f, 0.1f, 0.4f);
            Gizmos.DrawCube(Vector3.zero, new Vector3(_area.x, 0.15f, _area.y));
        }
#endif
    }
}