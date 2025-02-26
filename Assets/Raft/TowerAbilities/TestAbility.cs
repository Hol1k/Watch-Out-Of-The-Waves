using Raft.Scripts;
using UnityEngine;

namespace Raft.TowerAbilities
{
    [CreateAssetMenu(fileName = "TestAbility", menuName = "Towers/TowerAbilities/TestAbility")]
    public class TestAbility : Ability
    {
        public override void Activate(Tower abilityOwner)
        {
            Debug.Log($"Activate TestAbility on Tower: {abilityOwner.buildingType}");
        }
    }
}