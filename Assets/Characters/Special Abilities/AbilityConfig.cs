using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Core;

namespace RPG.Characters
{
    public struct AbilityUseParams
    {
        public IDamagable target;
        public float baseDamage;

        public AbilityUseParams(IDamagable target, float baseDamage)
        {
            this.target = target;
            this.baseDamage = baseDamage;
        }
    }

    public abstract class AbilityConfig : ScriptableObject
    {
        [Header("Special Ability General")]
        [SerializeField] float energyCost = 10f;
        [SerializeField] GameObject particleSystemPrefab = null;
        [SerializeField] AudioClip[] AudioClipsToPlay = null;

        protected AbilityBehaviour behaviour;

        abstract public void AttachComponentTo(GameObject gameObjectToAttachTo);

        public void Use(AbilityUseParams abilityParams)
        {
            behaviour.Use(abilityParams);
        }

        public float GetEnergyCost()
        {
            return energyCost;
        }

        public GameObject GetParticleSystemPrefab()
        {
            return particleSystemPrefab;
        }

        public AudioClip GetRandomAudioClipToPlay()
        {
            return AudioClipsToPlay[Random.Range(0, AudioClipsToPlay.Length)];
        }
    }

}
