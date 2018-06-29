using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Core;

namespace RPG.Characters
{
    public class AOEAttackBehaviour : AbilityBehaviour {

        public override void Use(GameObject target)
        {
            print("Ability Sound played");
            PlayAbilitySound();
            DealRadialDamage();
            PlayParticleEffect();
        }

        private void DealRadialDamage()
        {
            print("AOE attack used by: " + gameObject.name);
            //static SphereCast to radius for targets
            RaycastHit[] hits = Physics.SphereCastAll(transform.position, (config as AOEAttackConfig).GetRadius(), Vector3.up, (config as AOEAttackConfig).GetRadius());

            //foreach hit, if damagable, deal damage + player base dmg
            foreach (RaycastHit hit in hits)
            {
                var damagable = hit.collider.gameObject.GetComponent<HealthSystem>();
                bool hitPlayer = hit.collider.gameObject.GetComponent<Player>();
                if (damagable != null && !hitPlayer)
                {
                    float damageToDeal = (config as AOEAttackConfig).GetDamageToEachTarget();
                    damagable.TakeDamage(damageToDeal);
                }
            }
        }
    }
}
