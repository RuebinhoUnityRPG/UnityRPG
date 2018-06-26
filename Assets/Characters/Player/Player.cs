using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using RPG.CameraUI; //TODO consider re-wiring
using RPG.Core;
using UnityEngine.SceneManagement;

namespace RPG.Characters
{
    public class Player : MonoBehaviour
    {

        [SerializeField] float baseDamage = 20f;
        [Range(0f, 100f)] [SerializeField] float critChanceInPerCent = 20f;
        [SerializeField] float critHitMultiplierInPerCent = 125f;
        [SerializeField] ParticleSystem critParticleSystem = null;
        [SerializeField] AudioClip critAudioClip = null;

        [SerializeField] Weapon currentWeaponConfig = null;
        GameObject weaponObject;
        [SerializeField] AnimatorOverrideController animOverrideController = null;

        SpecialAbilities abilities;

        Animator animator;
        const string ATTACK_TRIGGER = "Attack";
        const string DEFAULT_ATTACK = "DEFAULT ATTACK";

        Enemy enemy = null;

        CameraRaycaster cameraRayCaster = null;

        AudioSource audioSource;

        private float lastHitTime = 0f;

        private void Start()
        {
            abilities = GetComponent<SpecialAbilities>();
            animator = GetComponent<Animator>();
            RegisterForMouseClick();
            PutWeaponInHand(currentWeaponConfig);
            SetAttackAnimation();
            audioSource = GetComponent<AudioSource>();
        }

        private void Update()
        {
            var healthPercentage = GetComponent<HealthSystem>().HealthAsPercentage;
            print(healthPercentage);
            if (healthPercentage > Mathf.Epsilon)
            {
                ScanForAbilityKeyDown();
            }
        }

        private void ScanForAbilityKeyDown()
        {
            for (int keyIndex = 1; keyIndex < abilities.GetNumberOfAbilities(); keyIndex++)
            {
                if (Input.GetKeyDown(keyIndex.ToString()))
                {
                    abilities.AttemptSpecialAbility(keyIndex);
                }
            }
        }

        private void SetAttackAnimation()
        {
            animator = GetComponent<Animator>();
            animator.runtimeAnimatorController = animOverrideController;
            animOverrideController[DEFAULT_ATTACK] = currentWeaponConfig.GetWeaponAttackAnimationClip();
        }

        private void RegisterForMouseClick()
        {
            cameraRayCaster = FindObjectOfType<CameraRaycaster>();
            cameraRayCaster.OnMouseOverEnemyHit += OnMouseOverEnemyHit;
        }

        void OnMouseOverEnemyHit(Enemy enemyToSet)
        {
            this.enemy = enemyToSet;

            if (Input.GetMouseButton(0) && IsTargetInRange(enemyToSet.gameObject))
            {
                AttackTarget();
            }
            else if (Input.GetMouseButtonDown(1))
            {
                abilities.AttemptSpecialAbility(0);
            }
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
            //allow for crit
            bool critChanceSuccessful = UnityEngine.Random.Range(0f, 100f) <= critChanceInPerCent;

            float finalDamage = baseDamage + currentWeaponConfig.GetAdditionalDamage();

            if (critChanceSuccessful)
            {
                finalDamage = finalDamage * critHitMultiplierInPerCent / 100;
                Debug.Log("Crit Damage dealt: " + finalDamage);
                audioSource.clip = critAudioClip;
                audioSource.Play();
                critParticleSystem.Play();
                return finalDamage;
            }
            else
            {
                Debug.Log("Normal Damage dealt: " + finalDamage);
                return finalDamage;
            }

        }

        private bool IsTargetInRange(GameObject target)
        {
            float distanceToTarget = (target.transform.position - transform.position).magnitude;
            return distanceToTarget <= currentWeaponConfig.GetMaxAttackRange();
        }

        public void PutWeaponInHand(Weapon weaponToUse)
        {
            currentWeaponConfig = weaponToUse;
            var weaponPrefab = weaponToUse.GetWeaponPrefab();
            GameObject dominantHand = RequestDominantHand();

            Destroy(weaponObject);

            weaponObject = Instantiate(weaponPrefab, dominantHand.transform);
            weaponObject.transform.localPosition = currentWeaponConfig.gripTransform.localPosition;
            weaponObject.transform.localRotation = currentWeaponConfig.gripTransform.localRotation;
        }
    }
}
