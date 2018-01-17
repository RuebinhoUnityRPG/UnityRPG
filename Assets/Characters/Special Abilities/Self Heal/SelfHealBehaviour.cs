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

            audioSource.clip = config.GetAudioClipToPlay();
            audioSource.Play();

            PlayParticleEffect();
        }

        private void Start()
        {
            player = GetComponent<Player>();
            audioSource = GetComponent<AudioSource>();
        }
    }
}
