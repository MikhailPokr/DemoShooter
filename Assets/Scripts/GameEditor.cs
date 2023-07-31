using System;
using System.Globalization;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace DemoShooter
{
    [Singleton]
    internal class GameEditor : MonoBehaviour, ISingleton
    {
        public const int DefaultHP = 100;
        public const int DefaultDamage = 3;
        public const float DefaultRange = 1;
        public const float DefaultCooldown = 0.4f;
        public const float DefaultSpeed = 1.5f;
        public const float DefaultRate = 5;


        [SerializeField] private GameObject _editorMenu;
        [SerializeField] private GameObject _cameraTopPoint;
        [SerializeField] private float _minValueX;
        [SerializeField] private float _maxValueX;
        [Space]
        [SerializeField] private Spawner _spawnerPrefab;
        [SerializeField] private GameObject _spawnerButton;
        private bool _enemy = false;
        private int _type = 0;
        private bool _spawnCreate;
        private int _typeIndex;
        [Space]
        [SerializeField] private InputField _fieldHp;
        private int[] _hp = new int[6];
        [SerializeField] private InputField _fieldDamage;
        private int[] _damage = new int[6];
        [SerializeField] private InputField _fieldRange;
        private float[] _range = new float[6];
        [SerializeField] private InputField _fieldCooldown;
        private float[] _cooldown = new float[6];
        [SerializeField] private InputField _fieldSpeed;
        private float[] _speed = new float[6];
        [SerializeField] private InputField _fieldRate;
        private float _rate;
        [Space]
        [SerializeField] private GameObject _arrowGroup;
        [SerializeField] private GameObject _arrowType;
        [SerializeField] private GameObject _arrowMode;
        [Space]
        [SerializeField] private Unit _unitPrefab;
        [SerializeField] private Unit _enemyPrefab;
        private float _arrowX;

        private bool _editMode;
        public bool EditMode => _editMode;

        private void Awake()
        {
            ClickManager.FloorClick += OnFloorClick;

            _fieldHp.onEndEdit.AddListener(OnHpChanged);
            _fieldDamage.onEndEdit.AddListener(OnDamageChanged);
            _fieldRange.onEndEdit.AddListener(OnRangeChanged);
            _fieldCooldown.onEndEdit.AddListener(OnCooldownChanged);
            _fieldSpeed.onEndEdit.AddListener(OnDSpeedChanged);

            _arrowX = _arrowType.transform.localPosition.x;

            DownloadValues();

        }

        private void OnHpChanged(string value) { _hp[_typeIndex] = int.Parse(value); UploadValues(hp: true); }
        private void OnDamageChanged(string value) { _damage[_typeIndex] = int.Parse(value); UploadValues(damage: true); }
        private void OnRangeChanged(string value) { _range[_typeIndex] = float.Parse(value, CultureInfo.InvariantCulture); UploadValues(range: true); }
        private void OnCooldownChanged(string value) { _cooldown[_typeIndex] = float.Parse(value, CultureInfo.InvariantCulture); UploadValues(cooldown: true); }
        private void OnDSpeedChanged(string value) { _speed[_typeIndex] = float.Parse(value, CultureInfo.InvariantCulture); UploadValues(speed: true); }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                _editMode = !_editMode;
                _editorMenu.SetActive(_editMode);

                if (_editMode)
                    Singleton<CameraMovenment>.instance.SetEditMode(_cameraTopPoint, new Vector3(90, -90, 0));
                else
                {
                    Unit unit = Singleton<UnitManager>.instance.CurrentUnit;
                    if (unit != null)
                        Singleton<CameraMovenment>.instance.SetTarget(unit.gameObject);
                    else
                        Singleton<UnitManager>.instance.NextUnit();
                }
            }
        }

        private void DownloadValues()
        {
            _spawnerButton.SetActive(_enemy);

            _hp[_typeIndex] = PlayerPrefs.GetInt("Type" + $"{_typeIndex}" + "HP", DefaultHP);
            _fieldHp.text = _hp[_typeIndex].ToString();

            _damage[_typeIndex] = PlayerPrefs.GetInt("Type" + $"{_typeIndex}" + "Damage", DefaultDamage);
            _fieldDamage.text = _damage[_typeIndex].ToString();

            _range[_typeIndex] = PlayerPrefs.GetFloat("Type" + $"{_typeIndex}" + "Range", DefaultRange);
            _fieldRange.text = _range[_typeIndex].ToString().Replace(",", ".");

            _cooldown[_typeIndex] = PlayerPrefs.GetFloat("Type" + $"{_typeIndex}" + "Cooldown", DefaultCooldown);
            _fieldCooldown.text = _cooldown[_typeIndex].ToString().Replace(",", ".");

            _speed[_typeIndex] = PlayerPrefs.GetFloat("Type" + $"{_typeIndex}" + "Speed", DefaultSpeed);
            _fieldSpeed.text = _speed[_typeIndex].ToString().Replace(",", ".");

            _rate = PlayerPrefs.GetFloat("SpawnRate", DefaultRate);
            _fieldRate.text = _rate.ToString().Replace(",", ".");
        }

        public void UploadValues(bool hp = false, bool damage = false, bool range = false, bool cooldown = false, bool speed = false, bool rate = false)
        {
            if (hp)
                PlayerPrefs.SetInt("Type" + $"{_typeIndex}" + "HP", _hp[_typeIndex]);
            if (damage)
                PlayerPrefs.SetInt("Type" + $"{_typeIndex}" + "Damage", _damage[_typeIndex]);
            if (range)
                PlayerPrefs.SetFloat("Type" + $"{_typeIndex}" + "Range", _range[_typeIndex]);
            if (cooldown)
                PlayerPrefs.SetFloat("Type" + $"{_typeIndex}" + "Cooldown", _cooldown[_typeIndex]);
            if (speed)
                PlayerPrefs.SetFloat("Type" + $"{_typeIndex}" + "Speed", _speed[_typeIndex]);
            if (rate)
                PlayerPrefs.SetFloat("SpawnRate", _rate);
            PlayerPrefs.Save();
        }

        public void SetUnitValues(Unit unit)
        {
            unit.SetValues(_typeIndex > 2, _typeIndex, _hp[_typeIndex], _damage[_typeIndex], _range[_typeIndex], _cooldown[_typeIndex], _speed[_typeIndex]);
        }

        public void SetUnitValues(Unit unit, int index)
        {
            unit.SetValues(index > 2, index, _hp[index], _damage[index], _range[index], _cooldown[index], _speed[index]);
        }

        public void ResetValues()
        {
            PlayerPrefs.DeleteAll();
            DownloadValues();
        }

        public void SetGroup(bool enemy)
        {
            _enemy = enemy;
            _arrowGroup.transform.rotation = Quaternion.Euler(new(0, 0, _enemy ? 180 : 0));
            _typeIndex = _type + (_enemy ? 3 : 0);
            if (!enemy)
                SetMode(false);
            DownloadValues();
        }

        public void SetType(int index)
        {
            _type = index;
            _arrowType.transform.localPosition = new Vector3(_arrowX + (Math.Abs(_arrowX) * index), _arrowType.transform.localPosition.y, _arrowType.transform.localPosition.z);
            _typeIndex = _type + (_enemy ? 3 : 0);
            DownloadValues();
        }

        public void SetMode(bool spawn)
        {
            _spawnCreate = spawn;
            _arrowMode.transform.rotation = Quaternion.Euler(new(0, 0, _spawnCreate ? 180 : 0));
            DownloadValues();
        }

        private void OnFloorClick(Vector3 pos)
        {
            if (!_editMode)
                return;

            if (_spawnCreate)
            {
                pos = new Vector3(pos.x, Singleton<UnitManager>.instance.YPos, pos.z);

                Spawner spawner = Instantiate(_spawnerPrefab, transform);
                spawner.transform.position = pos;
                spawner.SetValues(_typeIndex, _rate);

                return;
            }

            pos = new Vector3(pos.x, Singleton<UnitManager>.instance.YPos, pos.z);

            Unit unit;

            if (!_enemy)
                unit = Instantiate(_unitPrefab, transform);
            else
                unit = Instantiate(_enemyPrefab, transform);

            unit.transform.position = pos;
            SetUnitValues(unit);
        }

        public void MovePoint(float x)
        {
            Vector3 pos = _cameraTopPoint.transform.position;
            if (pos.x + x < _minValueX)
            {
                _cameraTopPoint.transform.position = new Vector3(_minValueX, pos.y, pos.z);
                return;
            }
            if (pos.x + x > _maxValueX)
            {
                _cameraTopPoint.transform.position = new Vector3(_maxValueX, pos.y, pos.z);
                return;
            }
            _cameraTopPoint.transform.position = new Vector3(pos.x + x, pos.y, pos.z);
        }

        public void Exit()
        {
            Application.Quit();
        }
    }
}