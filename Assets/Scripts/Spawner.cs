using Unity.VisualScripting;
using UnityEngine;

namespace DemoShooter
{
    internal class Spawner : MonoBehaviour
    {
        [SerializeField] Unit _enemyPrafab;

        private GameEditor _gameEditor;
        private UnitManager _unitManager;
        private float _rate;
        private int _index = -1;
        
        private float _time = 0;

        public void SetValues(int index, float rate)
        {
            _gameEditor = Singleton<GameEditor>.instance;
            _unitManager = Singleton<UnitManager>.instance;
            _index = index;
            _rate = rate;
        }

        private void Update()
        {
            if (_gameEditor.EditMode)
                return;

            if (_index == -1)
                return;

            if (_unitManager.GetObstacles(transform.position) != Vector3.zero)
                return;

            _time += Time.deltaTime;
            if (_time > _rate)
            {
                _time = 0;
                Unit unit = Instantiate(_enemyPrafab, transform.parent);
                unit.transform.position = transform.position;
                Singleton<GameEditor>.instance.SetUnitValues(unit, _index);
            }
        }
    }
}