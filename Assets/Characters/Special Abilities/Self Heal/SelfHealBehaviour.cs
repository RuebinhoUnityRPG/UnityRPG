using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters
{
    public class SelfHealBehaviour : AbilityBehaviour
    {
        Player player = null;

        public override void Use(AbilityUseParams abilityUseParams)
        {
            PlayAbilitySound();
            player.HealPlayer((config as SelfHealConfig).GetHealingPointsPerUse());
            PlayParticleEffect();
        }

        private void Start()
        {
            player = GetComponent<Player>();
        }
    }
}
