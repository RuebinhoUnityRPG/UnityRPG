using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Core;

namespace RPG.Characters
{
    public class AOEAttackBehaviour : MonoBehaviour, ISpecialAbility {

        AOEAttackConfig config;

        public void SetConfig(AOEAttackConfig configToSet)
        {
            this.config = configToSet;
        }

        public void Use(AbilityUseParams abilityUseParams)
        {
            print("AOE attack used by: " + gameObject.name);
            //static SphereCast to radius for targets
            RaycastHit[] hits = Physics.SphereCastAll(transform.position, config.GetRadius(), Vector3.up, config.GetRadius());

            //foreach hit, if damagable, deal damage + player base dmg
            foreach (RaycastHit hit in hits)
            {
                var damagable = hit.collider.gameObject.GetComponent<IDamagable>();
                if(damagable != null)
                {
                    float damageToDeal = abilityUseParams.baseDamage + config.GetDamageToEachTarget();
                    damagable.TakeDamage(damageToDeal);
                }
            }
        }

        private void Start()
        {
            print("AOE Attack Behaviour attached to: " + gameObject);
        }
    }
}
