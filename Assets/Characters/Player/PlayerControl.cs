using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using RPG.CameraUI; //for mouse events
using RPG.Core;
using UnityEngine.SceneManagement;

//todo extract weapon system
namespace RPG.Characters
{
    public class PlayerControl : MonoBehaviour
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
        Character character;

        private void Start()
        {
            character = GetComponent<Character>();
            abilities = GetComponent<SpecialAbilities>();
            animator = GetComponent<Animator>();
            RegisterForMouseEvents();
            PutWeaponInHand(currentWeaponConfig); //todo move to WeaponSystem
            SetAttackAnimation(); //todo move to WeaponSystem
            audioSource = GetComponent<AudioSource>();
        }

        private void RegisterForMouseEvents()
        {
            cameraRayCaster = FindObjectOfType<CameraRaycaster>();
            cameraRayCaster.OnMouseOverEnemyHit += OnMouseOverEnemyHit;
            cameraRayCaster.OnMouseOverPotentiallyWalkable += OnMouseOverPotentiallyWalkable;
        }

        private void Update()
        {
            ScanForAbilityKeyDown();
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

        private GameObject RequestDominantHand()
        {
            var dominantHands = GetComponentsInChildren<DominantHand>();
            int numberOfDominantHands = dominantHands.Length;
            // handle 0
            Assert.IsFalse(numberOfDominantHands <= 0, "No dominant hand found, please add one!");
            Assert.IsFalse(numberOfDominantHands < 1, "Multiple dominantHands found, please remove until one is left!");

            return dominantHands[0].gameObject;
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

        //todo move to weaponSystem
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

        bool IsTargetInRange(GameObject target)
        {
            float distanceToTarget = (target.transform.position - transform.position).magnitude;
            return distanceToTarget <= currentWeaponConfig.GetMaxAttackRange();
        }

        //todo moce to weaponSystem
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

        private void OnMouseOverPotentiallyWalkable(Vector3 destination)
        {
            if (Input.GetMouseButton(0))
            {
                character.SetDestination(destination);
            }
        }
    }
}
