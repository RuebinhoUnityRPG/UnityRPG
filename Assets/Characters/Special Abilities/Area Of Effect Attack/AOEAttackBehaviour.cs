using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Core;

namespace RPG.Characters
{
    public class AOEAttackBehaviour : AbilityBehaviour {

        public override void Use(AbilityUseParams abilityUseParams)
        {
            DealRadialDamage(abilityUseParams);
            PlayParticleEffect();
        }

        private void DealRadialDamage(AbilityUseParams abilityUseParams)
        {
            print("AOE attack used by: " + gameObject.name);
            //static SphereCast to radius for targets
            RaycastHit[] hits = Physics.SphereCastAll(transform.position, (config as AOEAttackConfig).GetRadius(), Vector3.up, (config as AOEAttackConfig).GetRadius());

            //foreach hit, if damagable, deal damage + player base dmg
            foreach (RaycastHit hit in hits)
            {
                var damagable = hit.collider.gameObject.GetComponent<IDamagable>();
                bool hitPlayer = hit.collider.gameObject.GetComponent<Player>();
                if (damagable != null && !hitPlayer)
                {
                    float damageToDeal = abilityUseParams.baseDamage + (config as AOEAttackConfig).GetDamageToEachTarget();
                    damagable.TakeDamage(damageToDeal);
                }
            }
        }
    }
}
