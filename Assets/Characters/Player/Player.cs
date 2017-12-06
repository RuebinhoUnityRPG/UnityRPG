using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using RPG.CameraUI; //TODO consider re-wiring
using RPG.Core;
using RPG.Weapons;
using UnityEngine.SceneManagement;

namespace RPG.Characters
{
    public class Player : MonoBehaviour, IDamagable
    {
        [SerializeField] float maxHealthPoints = 200f;
        [SerializeField] float baseDamage = 20f;

        [SerializeField] Weapon weaponInUse = null;
        [SerializeField] AnimatorOverrideController animOverrideController = null;

        const string DEATH_TRIGGER = "Death";
        const string ATTACK_TRIGGER = "Attack";

        AudioSource audioSource;
        //Arrays for sounds
        [SerializeField] AudioClip[] damageSounds;
        [SerializeField] AudioClip[] deathSounds;

        //Temp for debugging
        [SerializeField] SpecialAbility[] abilities;

        CameraRaycaster cameraRayCaster;
        Animator animator;

        private float lastHitTime = 0f;

        float currentHealthPoints;

        public void TakeDamage(float damage)
        {

            bool playerDies = (currentHealthPoints - damage <= 0);
            if (playerDies)
            {
                ReduceHealth(damage);
                //Kill player
                StartCoroutine(KillPlayer());
            }

            ReduceHealth(damage);
            
        }

        IEnumerator KillPlayer()
        {
            //Trigger Death Animation (optional)
            Debug.Log("Death Animation");
            animator.SetTrigger(DEATH_TRIGGER);
            
            //Play Death Sound (optional)
            Debug.Log("Death Sound");
            AudioClip deathSound = deathSounds[(int)UnityEngine.Random.Range(0f, deathSounds.Length)];
            audioSource.clip = deathSound;
            audioSource.Play();
            Debug.Log(audioSource.clip);

            //Reload the scene after some seconds or player key press input
            yield return new WaitForSecondsRealtime(audioSource.clip.length);
            SceneManager.LoadScene(0);
        }

        private void ReduceHealth(float damage)
        {
            currentHealthPoints = Mathf.Clamp(currentHealthPoints - damage, 0f, maxHealthPoints);
            // play sound
            AudioClip damageSound = damageSounds[(int)UnityEngine.Random.Range(0f, damageSounds.Length)];
            audioSource.clip = damageSound;
            audioSource.Play();
            Debug.Log(audioSource.clip);
        }

        public float HealthAsPercentage
        {
            get
            {
                return currentHealthPoints / maxHealthPoints;
            }
        }

        private void Start()
        {
            RegisterForMouseClick();
            SetCurrentMaxHealth();
            PutWeaponInHand();
            SetupRuntimeAnimator();
            abilities[0].AttachComponentTo(gameObject);
            audioSource = GetComponent<AudioSource>();
        }

        private void SetCurrentMaxHealth()
        {
            currentHealthPoints = maxHealthPoints;
        }

        private void SetupRuntimeAnimator()
        {
            animator = GetComponent<Animator>();
            animator.runtimeAnimatorController = animOverrideController;
            animOverrideController["DEFAULT ATTACK"] = weaponInUse.GetWeaponAttackAnimationClip(); //TODO remove const
        }

        private void RegisterForMouseClick()
        {
            cameraRayCaster = FindObjectOfType<CameraRaycaster>();
            cameraRayCaster.OnMouseOverEnemyHit += OnMouseOverEnemyHit;
        }

        void OnMouseOverEnemyHit(Enemy enemy)
        {
            if (Input.GetMouseButton(0) && IsTargetInRange(enemy.gameObject))
            {
                AttackTarget(enemy);
            }
            else if (Input.GetMouseButtonDown(1))
            {
                AttemptSpecialAbility(0, enemy);
            }
        }

        private void AttemptSpecialAbility(int abilityIndex, Enemy enemy)
        {
            var energyComponent = GetComponent<Energy>();
            var energyCost = abilities[abilityIndex].GetEnergyCost();

            if (energyComponent.IsEnergyAvailable(energyCost)) //TODO read from SO
            {
                energyComponent.ConsumeEnergy(energyCost);
                //Use the ability
                var abilityParams = new AbilityUseParams(enemy, baseDamage);
                abilities[abilityIndex].Use(abilityParams);
            }
        }

        private void PutWeaponInHand()
        {
            var weaponPrefab = weaponInUse.GetWeaponPrefab();
            GameObject dominantHand = RequestDominantHand();
            var weapon = Instantiate(weaponPrefab, dominantHand.transform);

            weapon.transform.localPosition = weaponInUse.gripTransform.localPosition;
            weapon.transform.localRotation = weaponInUse.gripTransform.localRotation;
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

        private void AttackTarget(Enemy enemy)
        {
            if (Time.time - lastHitTime > weaponInUse.GetMinTimeBetweenHits())
            {
                animator.SetTrigger(ATTACK_TRIGGER);
                enemy.TakeDamage(baseDamage);
                lastHitTime = Time.time;
            }
        }

        private bool IsTargetInRange(GameObject target)
        {
            float distanceToTarget = (target.transform.position - transform.position).magnitude;
            return distanceToTarget <= weaponInUse.GetMaxAttackRange();
        }
    }
}
