using System;
using UnityEngine;

namespace Raft.Scripts
{
    [Serializable]
    public struct BuildingPrefabConfig
    {
        public BuildingType buildingType;
        public GameObject prefab;
    }
}