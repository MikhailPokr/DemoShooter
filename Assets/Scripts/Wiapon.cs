using Unity.VisualScripting;
using UnityEngine;

namespace DemoShooter
{
    internal class Wiapon : MonoBehaviour
    {
        [SerializeField] private GameObject _bullet;
        [SerializeField] private GameObject _bulletPoint;

        [SerializeField] private float _range;
        public float Range => _range;
        [SerializeField] private int _damage;

        private float _timer = 0;
        [SerializeField] private float _cooldown;

        public void SetValues(float range, int damage, float cooldown, int index)
        {
            Material material;
            float scale = 0;
            if (index == 0 || index == 3)
            {
                material = Singleton<UnitManager>.instance.UnitWiapon1;
                scale = Singleton<UnitManager>.instance.ScaleWiapon1;
            }
            else if (index == 1 || index == 4)
            {
                material = Singleton<UnitManager>.instance.UnitWiapon2;
                scale = Singleton<UnitManager>.instance.ScaleWiapon2;
            }
            else
            {
                material = Singleton<UnitManager>.instance.UnitWiapon3;
                scale = Singleton<UnitManager>.instance.ScaleWiapon3;
            }

            GetComponent<MeshRenderer>().material = material;
            transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, scale);
            _range = range;
            _damage = damage;
            _cooldown = cooldown;
        }

        public void Shoot(Unit host, Unit target)
        {
            _timer += Time.deltaTime;
            if (_timer < _cooldown)
                return;

            _timer = 0;

            Bullet bullet = Instantiate(_bullet).GetComponent<Bullet>();
            bullet.SetValues(host.IsEnemy, _damage);

            bullet.transform.position = _bulletPoint.transform.position;
            bullet.SetTarget(target.gameObject, _range);
        }

    }
}