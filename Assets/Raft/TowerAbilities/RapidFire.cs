using System.Collections;
using Raft.Scripts;
using UnityEngine;

namespace Raft.TowerAbilities
{
    [CreateAssetMenu(fileName = "Rapid Fire", menuName = "Towers/TowerAbilities/Rapid Fire")]
    public class RapidFire : Ability
    {
        [SerializeField] private float durationBySeconds;
        [SerializeField] private int attackSpeedBonusByPercentage;

        public override void Activate(Tower abilityOwner)
        {
            abilityOwner.StartCoroutine(StartRapidFire(abilityOwner));
        }

        private IEnumerator StartRapidFire(Tower abilityOwner)
        {
            int bonus = (int)(abilityOwner.GetCurrentAttackSpeed(false) * (attackSpeedBonusByPercentage / 100f));

            abilityOwner.attackSpeedBonus += bonus;

            yield return new WaitForSeconds(durationBySeconds);

            abilityOwner.attackSpeedBonus -= bonus;
        }
    }
}