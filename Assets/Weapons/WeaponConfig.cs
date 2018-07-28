using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters
{
    [CreateAssetMenu(menuName = ("RPG/Weapon"))]
    public class WeaponConfig : ScriptableObject
    {

        public Transform gripTransform;

        [SerializeField] GameObject weaponPrefab;
        [SerializeField] AnimationClip attackAnimation;
        [SerializeField] float minTimeBetweenHits = 0.5f;
        [SerializeField] float maxAttackRange = 2f;
        [SerializeField] float additionalDamage = 10f;
        [SerializeField] float damageDelay = 0.5f;

        public GameObject GetWeaponPrefab()
        {
            return weaponPrefab;
        }

        public AnimationClip GetWeaponAttackAnimationClip()
        {
            RemoveAnimationEvents();
            return attackAnimation;
        }

        public float GetMinTimeBetweenHits()
        {
            //TODO consider whether we take animation time into account?
            return minTimeBetweenHits;
        }

        public float GetMaxAttackRange()
        {
            return maxAttackRange;
        }

        public float GetAdditionalDamage()
        {
            return additionalDamage;
        }

        public float GetDamageDelay()
        {
            return damageDelay;
        }

        // erases all animation events that are in imported asset pack
        private void RemoveAnimationEvents()
        {
            attackAnimation.events = new AnimationEvent[0]; 
        }
    }
}
