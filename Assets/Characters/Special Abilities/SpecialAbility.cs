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

    public abstract class SpecialAbility : ScriptableObject
    {
        [Header("Special Ability General")]
        [SerializeField] float energyCost = 10f;

        protected ISpecialAbility behaviour;

        abstract public void AttachComponentTo(GameObject gameObjectToAttachTo);

        public void Use(AbilityUseParams abilityParams)
        {
            behaviour.Use(abilityParams);
        }

        public float GetEnergyCost()
        {
            return energyCost;
        }
    }

    public interface ISpecialAbility
    {
        void Use(AbilityUseParams abilityParams);
    }

}
