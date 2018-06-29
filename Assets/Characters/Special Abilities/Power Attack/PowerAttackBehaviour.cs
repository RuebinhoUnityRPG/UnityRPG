using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters
{
    public class PowerAttackBehaviour : AbilityBehaviour
    {
        public override void Use(GameObject target)
        {
            PlayAbilitySound();
            DealDamage(target);
            PlayParticleEffect();
        }

        private void DealDamage(GameObject target)
        {
            print("Power attack used by: " + gameObject.name);
            float damageToDeal = (config as PowerAttackConfig).GetExtraDamage();
            target.GetComponent<HealthSystem>().TakeDamage(damageToDeal);
        }

        private void Start()
        {
            print("Power Attack Behaviour attached to: " + gameObject);
        }
    }
}
