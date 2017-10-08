using System.Collections;
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

        public GameObject getWeaponPrefab()
        {
            return weaponPrefab;
        }

        public AnimationClip getWeaponAttackAnimationClip()
        {
            RemoveAnimationEvents();
            return attackAnimation;
        }

        // erases all animation events that are in imported asset pack
        private void RemoveAnimationEvents()
        {
            attackAnimation.events = new AnimationEvent[0]; 
        }
    }
}
