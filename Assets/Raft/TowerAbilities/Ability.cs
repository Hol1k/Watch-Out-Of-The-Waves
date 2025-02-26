using Raft.Scripts;
using UnityEngine;

namespace Raft.TowerAbilities
{
    public abstract class Ability : ScriptableObject
    {
        public abstract void Activate(Tower abilityOwner);
    }
}