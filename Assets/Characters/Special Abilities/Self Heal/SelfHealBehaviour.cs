using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters
{
    public class SelfHealBehaviour : MonoBehaviour, ISpecialAbility
    {

        SelfHealConfig config = null;
        Player player = null;
        AudioSource audioSource = null;

        public void SetConfig(SelfHealConfig configToSet)
        {
            this.config = configToSet;
        }

        public void Use(AbilityUseParams abilityUseParams)
        {
            player.HealPlayer(config.GetHealingPointsPerUse());
            print("Self Heal Used by player: " + gameObject);

            audioSource.clip = config.GetAudioClipToPlay();
            audioSource.Play();

            PlayParticleEffect();
        }

        private void PlayParticleEffect()
        {
            Debug.Log("Self Heal Particle System triggered");
            //Instanciate ParticleSystem prefab attached to the player
            var prefab = Instantiate(config.GetParticleSystemPrefab(), transform.position, Quaternion.identity);
            //TODO decide if pareticle systems attached to player
            prefab.transform.parent = transform;
            //Get the particle system component
            ParticleSystem particleSystemEffect = prefab.GetComponent<ParticleSystem>();
            //Play particle System
            particleSystemEffect.Play();
            //Destroy after duration
            Destroy(prefab, particleSystemEffect.main.duration);
        }

        private void Start()
        {
            print("Self Heal Behaviour attached to: " + gameObject);
            player = GetComponent<Player>();
            audioSource = GetComponent<AudioSource>();
        }
    }
}
