using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace DemoShooter
{
    internal class Barrier : MonoBehaviour
    {
        public enum State
        {
            Disabled,
            Showed,
            LongActive,
            ShortActive
        }

        [SerializeField] private float _sizeShort;
        [SerializeField] private float _sizeLong;
        [SerializeField] MeshRenderer _meshRenderer;

        private State _state;

        private float _permeability;
        public float Permeability => _permeability / 100f;
        private LogicPoint[] _points;

        private static List<Barrier> _barrierList = new List<Barrier>();

        

        private void Start()
        {
            _barrierList.Add(this);
            _points = GetComponentsInChildren<LogicPoint>(true);
            UpdateView();
        }

        public static void ShowAll(bool show)
        {
            foreach (Barrier barrier in _barrierList)
            {
                if (show && barrier._state == State.Disabled)
                {
                    barrier._state = State.Showed;
                }
                else if (!show && barrier._state == State.Showed)
                {
                    barrier._state = State.Disabled;
                }
                barrier.UpdateView();
            }
        }

        private void UpdateView()
        {
            switch (_state)
            {
                case State.Disabled:
                    {
                        _meshRenderer.material.color = new Color(0, 0, 0, 0);
                        _permeability = 0;
                        foreach (LogicPoint point in _points)
                            point.gameObject.SetActive(false);
                        break;
                    }
                case State.Showed: 
                    {
                        transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, _sizeLong);
                        _meshRenderer.material.color = Color.yellow;
                        _permeability = 0;
                        foreach (LogicPoint point in _points)
                            point.gameObject.SetActive(false);
                        break;
                    }
                case State.ShortActive:
                    {
                        transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, _sizeShort);
                        _permeability = Singleton<GameEditor>.instance.BarrierPermeability;
                        _meshRenderer.material.color = Color.white;
                        foreach (LogicPoint point in _points)
                            point.gameObject.SetActive(true);
                        break;
                    }
                case State.LongActive:
                    {
                        transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, _sizeLong);
                        _permeability = Singleton<GameEditor>.instance.BarrierPermeability;
                        _meshRenderer.material.color = Color.white;
                        foreach (LogicPoint point in _points)
                            point.gameObject.SetActive(true);
                        break;
                    }
            }
        }

        private void SetPermeability(float permeability)
        {
            _permeability = permeability;
            UpdateView();
        }

        public void Click()
        {
            if (_state == State.Showed || _state == State.LongActive)
            {
                SetPermeability(Singleton<GameEditor>.instance.BarrierPermeability);
            }
            if (_state == State.Showed)
            {
                _state = State.LongActive;
            }
            else if ( _state == State.LongActive) 
            {
                _state = State.ShortActive;
            }
            else if (_state == State.ShortActive)
            {
                _state = State.Showed;
            }
            UpdateView();
        }
    }
}