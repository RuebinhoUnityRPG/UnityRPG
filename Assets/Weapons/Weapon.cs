﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Weapons
{
    [CreateAssetMenu(menuName = ("RPG/Weapon"))]
    public class Weapon : ScriptableObject
    {

        public Transform gripTransform;

        [SerializeField] GameObject weaponPrefab;
        [SerializeField] AnimationClip attackAnimation;
        [SerializeField] float minTimeBetweenHits = 0.5f;
        [SerializeField] float maxAttackRange = 2f;

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

        // erases all animation events that are in imported asset pack
        private void RemoveAnimationEvents()
        {
            attackAnimation.events = new AnimationEvent[0]; 
        }
    }
}
