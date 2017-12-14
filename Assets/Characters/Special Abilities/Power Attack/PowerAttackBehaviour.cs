using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters
{
    public class PowerAttackBehaviour : AbilityBehaviour
    {
        public override void Use(AbilityUseParams abilityUseParams)
        {
            DealDamage(abilityUseParams);
            PlayParticleEffect();
        }

        private void DealDamage(AbilityUseParams abilityUseParams)
        {
            print("Power attack used by: " + gameObject.name);
            float damageToDeal = abilityUseParams.baseDamage + (config as PowerAttackConfig).GetExtraDamage();
            abilityUseParams.target.TakeDamage(damageToDeal);
        }

        private void Start()
        {
            print("Power Attack Behaviour attached to: " + gameObject);
        }
    }
}
