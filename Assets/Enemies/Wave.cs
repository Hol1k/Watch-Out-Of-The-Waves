using System;
using System.Collections.Generic;
using UnityEngine;

namespace Enemies
{
    [Serializable]
    public class Wave
    {
        [Tooltip("Metres from start")]
        public float position;
        public List<EnemiesCount> enemies = new();
    }
}