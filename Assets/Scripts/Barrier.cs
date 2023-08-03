using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DemoShooter
{
    public class Barrier : MonoBehaviour
    {
        private float _permeability;
        public float Permeability => _permeability / 100f;

        public void SetValues(float permeability)
        {
            _permeability = permeability;
        }

    }
}