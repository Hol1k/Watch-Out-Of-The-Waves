using System.Collections;
using Raft.Scripts;
using UnityEngine;

namespace Raft.TowerAbilities
{
    [CreateAssetMenu(fileName = "High-Power Volley", menuName = "Towers/TowerAbilities/High-Power Volley")]
    public class HighPowerVolley : Ability
    {
        [SerializeField] private float durationBySeconds;
        [SerializeField] private int damageBonusByPercentage;

        public override void Activate(Tower abilityOwner)
        {
            abilityOwner.StartCoroutine(StartHighPowerVolley(abilityOwner));
        }

        private IEnumerator StartHighPowerVolley(Tower abilityOwner)
        {
            int bonus = (int)(abilityOwner.GetCurrentDamage(false) * (damageBonusByPercentage / 100f));

            abilityOwner.damageBonus += bonus;

            yield return new WaitForSeconds(durationBySeconds);

            abilityOwner.damageBonus -= bonus;
        }
    }
}