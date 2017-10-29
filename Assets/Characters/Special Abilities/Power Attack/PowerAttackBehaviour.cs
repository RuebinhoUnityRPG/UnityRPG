using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters
{
    public class PowerAttackBehaviour : MonoBehaviour, ISpecialAbility
    {
        PowerAttackConfig config;

        public void SetConfig(PowerAttackConfig configToSet)
        {
            this.config = configToSet;
        }

        public void Use(AbilityUseParams abilityUseParams)
        {
            print("Power attack used by: " + gameObject.name);
            float damageToDeal = abilityUseParams.baseDamage + config.GetExtraDamage();
            abilityUseParams.target.TakeDamage(damageToDeal);
        }

        private void Start()
        {
            print("Power Attack Behaviour attached to: " + gameObject);
        }
    }
}
