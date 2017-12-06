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
            DealDamage(abilityUseParams);
            PlayParticleEffect();
        }

        private void PlayParticleEffect()
        {
            Debug.Log("Power Attack Particle System triggered");
            //Instanciate ParticleSystem prefab attached to the player
            var prefab = Instantiate(config.GetParticleSystemPrefab(), transform.position, Quaternion.identity);
            //TODO decide if pareticle systems attached to player
            //Get the particle system component
            ParticleSystem particleSystemEffect = prefab.GetComponent<ParticleSystem>();
            //Play particle System
            particleSystemEffect.Play();
            //Destroy after duration
            Destroy(prefab, particleSystemEffect.main.duration);
        }

        private void DealDamage(AbilityUseParams abilityUseParams)
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
