using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Core;

namespace RPG.Characters
{
    public abstract class AbilityConfig : ScriptableObject
    {
        [Header("Special Ability General")]
        [SerializeField] float energyCost = 10f;
        [SerializeField] GameObject particleSystemPrefab = null;
        [SerializeField] AudioClip[] AudioClipsToPlay = null;

        protected AbilityBehaviour behaviour;

        public abstract AbilityBehaviour GetBehaviourComponent(GameObject objectToAttachTo);

        public void AttachAbilityTo(GameObject gameObjectToAttachTo)
        {
            AbilityBehaviour abilityBehaviourComponent = GetBehaviourComponent(gameObjectToAttachTo);
            abilityBehaviourComponent.SetConfig(this);
            behaviour = abilityBehaviourComponent;
        }

        public void Use(GameObject target)
        {
            behaviour.Use(target);
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
