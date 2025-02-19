using System;
using UnityEngine;

namespace Raft.Scripts
{
    [Serializable]
    public struct BuildingPrefabConfig
    {
        public static BuildingPrefabConfig DefaultConfig = new() {buildingType = 0, prefab = null, health = 0};
        
        public BuildingType buildingType;
        public GameObject prefab;
        public int health;
    }
}