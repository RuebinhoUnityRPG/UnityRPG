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
        [Range (0f, 100f)] [SerializeField] float critChanceInPerCent = 20f;
        [SerializeField] float critHitMultiplierInPerCent = 125f;
        [SerializeField] ParticleSystem critParticleSystem = null;
        [SerializeField] AudioClip critAudioClip = null;

        [SerializeField] Weapon weaponInUse = null;
        [SerializeField] AnimatorOverrideController animOverrideController = null;

        const string DEATH_TRIGGER = "Death";
        const string ATTACK_TRIGGER = "Attack";

        AudioSource audioSource = null;
        //Arrays for sounds
        [SerializeField] AudioClip[] damageSounds;
        [SerializeField] AudioClip[] deathSounds;

        //Temp for debugging
        [SerializeField] AbilityConfig[] abilities;

        Enemy enemy = null;

        CameraRaycaster cameraRayCaster = null;
        Animator animator = null;

        private float lastHitTime = 0f;

        float currentHealthPoints;

        public void TakeDamage(float damage)
        {

            bool playerDies = (currentHealthPoints - damage <= 0); // must ask before reducing health

            currentHealthPoints = Mathf.Clamp(currentHealthPoints - damage, 0f, maxHealthPoints);
            // play sound
            AudioClip damageSound = damageSounds[(int)UnityEngine.Random.Range(0f, damageSounds.Length)];
            audioSource.clip = damageSound;
            audioSource.Play();
            //Debug.Log(audioSource.clip);

            if (playerDies)
            {
                StartCoroutine(KillPlayer());
            }
            
        }

        public void HealPlayer(float healPoints)
        {
            currentHealthPoints = Mathf.Clamp(currentHealthPoints + healPoints, 0f, maxHealthPoints);
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

        public float HealthAsPercentage
        {
            get
            {
                return currentHealthPoints / maxHealthPoints;
            }
        }

        private void Start()
        {
            audioSource = GetComponent<AudioSource>();

            RegisterForMouseClick();
            SetCurrentMaxHealth();
            PutWeaponInHand();
            SetupRuntimeAnimator();
            AttachInitialAbilities();
        }

        private void AttachInitialAbilities()
        {
            for (int abilityIndex = 0; abilityIndex < abilities.Length; abilityIndex++)
            {
                abilities[abilityIndex].AttachComponentTo(gameObject);
            }
        }

        private void Update()
        {
            if (HealthAsPercentage > Mathf.Epsilon)
            {
                ScanForAbilityKeyDown();
            }
        }

        private void ScanForAbilityKeyDown()
        {
            for (int keyIndex = 1; keyIndex < abilities.Length; keyIndex++)
            {
                if (Input.GetKeyDown(keyIndex.ToString()))
                {
                    AttemptSpecialAbility(keyIndex);
                }
            }
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

        void OnMouseOverEnemyHit(Enemy enemyToSet)
        {
            this.enemy = enemyToSet;

            if (Input.GetMouseButton(0) && IsTargetInRange(enemyToSet.gameObject))
            {
                AttackTarget();
            }
            else if (Input.GetMouseButtonDown(1))
            {
                AttemptSpecialAbility(0);
            }
        }

        private void AttemptSpecialAbility(int abilityIndex)
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

        private void AttackTarget()
        {
            if (Time.time - lastHitTime > weaponInUse.GetMinTimeBetweenHits())
            {
                animator.SetTrigger(ATTACK_TRIGGER);
                enemy.TakeDamage(CalculateDamage());
                lastHitTime = Time.time;
            }
        }

        private float CalculateDamage()
        {
            //allow for crit
            bool critChanceSuccessful = UnityEngine.Random.Range(0f, 100f) <= critChanceInPerCent;

            float finalDamage = baseDamage + weaponInUse.GetAdditionalDamage();

            if (critChanceSuccessful)
            {
                finalDamage = finalDamage * critHitMultiplierInPerCent / 100;
                Debug.Log("Crit Damage dealt: " + finalDamage);
                audioSource.clip = critAudioClip;
                audioSource.Play();
                critParticleSystem.Play();
                return finalDamage;
            } else
            {
                Debug.Log("Normal Damage dealt: " + finalDamage);
                return finalDamage;
            }

        }

        private bool IsTargetInRange(GameObject target)
        {
            float distanceToTarget = (target.transform.position - transform.position).magnitude;
            return distanceToTarget <= weaponInUse.GetMaxAttackRange();
        }
    }
}
