using Raft.Scripts;
using UnityEngine;

namespace Raft.TowerAbilities
{
    public abstract class Ability : ScriptableObject
    {
        public string abilityName;
        [TextArea(3, 10)]
        public string description;
        
        public abstract void Activate(Tower abilityOwner);
    }
}