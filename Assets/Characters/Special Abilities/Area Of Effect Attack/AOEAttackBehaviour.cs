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
            DealRadialDamage(abilityUseParams);
            PlayParticleEffect();
        }

        private void PlayParticleEffect()
        {
            Debug.Log("AOE Attack Particle System triggered");
            //Instanciate ParticleSystem prefab attached to the player
            var particlePrefab = config.GetParticleSystemPrefab();
            var prefab = Instantiate(particlePrefab, transform.position, particlePrefab.transform.rotation);
            //TODO decide if pareticle systems attached to player
            //Get the particle system component
            ParticleSystem particleSystemEffect = prefab.GetComponent<ParticleSystem>();
            //Play particle System
            particleSystemEffect.Play();
            //Destroy after duration
            Destroy(prefab, particleSystemEffect.main.duration);
        }

        private void DealRadialDamage(AbilityUseParams abilityUseParams)
        {
            print("AOE attack used by: " + gameObject.name);
            //static SphereCast to radius for targets
            RaycastHit[] hits = Physics.SphereCastAll(transform.position, config.GetRadius(), Vector3.up, config.GetRadius());

            //foreach hit, if damagable, deal damage + player base dmg
            foreach (RaycastHit hit in hits)
            {
                var damagable = hit.collider.gameObject.GetComponent<IDamagable>();
                bool hitPlayer = hit.collider.gameObject.GetComponent<Player>();
                if (damagable != null && !hitPlayer)
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
