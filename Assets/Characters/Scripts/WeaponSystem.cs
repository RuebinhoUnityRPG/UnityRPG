using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace RPG.Characters
{
    public class WeaponSystem : MonoBehaviour
    {
        const string ATTACK_TRIGGER = "Attack";
        const string DEFAULT_ATTACK = "DEFAULT ATTACK";

        [SerializeField] float baseDamage = 20f;
        private float lastHitTime;

        [SerializeField] WeaponConfig currentWeaponConfig = null;
        GameObject weaponObject;

        GameObject target;

        Animator animator;
        Character character;

        // Use this for initialization
        void Start()
        {
            PutWeaponInHand(currentWeaponConfig);
            SetAttackAnimation();
            character = GetComponent<Character>();
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void SetAttackAnimation()
        {
            animator = GetComponent<Animator>();
            var animOverrideController = character.GetAnimOverrideController();
            animator.runtimeAnimatorController = animOverrideController;
            animOverrideController[DEFAULT_ATTACK] = currentWeaponConfig.GetWeaponAttackAnimationClip();
        }

        public void AttackTarget(GameObject targetToAttack)
        {
            target = targetToAttack;
            print("Attacking Target: " + target);
            //todo use a repeat attack co routine
        }

        //todo use co-routines for move and attack
        private void AttackTarget()
        {
            if (Time.time - lastHitTime > currentWeaponConfig.GetMinTimeBetweenHits())
            {
                SetAttackAnimation();
                animator.SetTrigger(ATTACK_TRIGGER);
                lastHitTime = Time.time;
            }
        }

        private float CalculateDamage()
        {
                return baseDamage + currentWeaponConfig.GetAdditionalDamage();
        }

        public WeaponConfig GetCurrentWeaponConfig()
        {
            return currentWeaponConfig;
        }

        public void PutWeaponInHand(WeaponConfig weaponToUse)
        {
            currentWeaponConfig = weaponToUse;
            var weaponPrefab = weaponToUse.GetWeaponPrefab();
            GameObject dominantHand = RequestDominantHand();

            Destroy(weaponObject);

            weaponObject = Instantiate(weaponPrefab, dominantHand.transform);
            weaponObject.transform.localPosition = currentWeaponConfig.gripTransform.localPosition;
            weaponObject.transform.localRotation = currentWeaponConfig.gripTransform.localRotation;
        }

        private GameObject RequestDominantHand()
        {
            var dominantHands = GetComponentsInChildren<DominantHand>();
            int numberOfDominantHands = dominantHands.Length;
            // handle 0
            Assert.IsFalse(numberOfDominantHands <= 0, "No dominant hand found, please add one!");
            Assert.IsFalse(numberOfDominantHands < 1, "Multiple dominantHands found, please remove until one is left!");

            return dominantHands[0].gameObject;
        }
    }
}
