using UnityEngine;

namespace DemoShooter
{
    internal class Bullet : MonoBehaviour
    {
        [SerializeField] private MoveToPoint _moveToPoint;
        public MoveToPoint MoveToPoint => _moveToPoint;

        private bool _enemyUnit;
        private int _damage;
        public void SetValues(bool enemyUnit, int damage)
        {
            _enemyUnit = enemyUnit;
            _damage = damage;

            _moveToPoint.RangeOut += OnRangeOut;
        }

        private void OnRangeOut()
        {
            Destroy(gameObject);
        }

        private void OnTriggerEnter(Collider other)
        {
            Unit unit = other.GetComponentInParent<Unit>();
            if (unit != null)
            {
                unit.ChangeHp(-_damage);
                Destroy(gameObject);
            }
        }
    }
}