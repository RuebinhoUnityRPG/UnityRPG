using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters
{
    public class SelfHealBehaviour : AbilityBehaviour
    {
        Player player = null;
        AudioSource audioSource = null;

        public override void Use(AbilityUseParams abilityUseParams)
        {
            player.HealPlayer((config as SelfHealConfig).GetHealingPointsPerUse());
            print("Self Heal Used by player: " + gameObject);

            audioSource.clip = config.GetAudioClipToPlay();
            audioSource.Play();

            PlayParticleEffect();
        }

        private void Start()
        {
            print("Self Heal Behaviour attached to: " + gameObject);
            player = GetComponent<Player>();
            audioSource = GetComponent<AudioSource>();
        }
    }
}
