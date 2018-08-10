using System;
using System.Collections;
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

        void Start()
        {
            character = GetComponent<Character>();
            PutWeaponInHand(currentWeaponConfig);
            SetAttackAnimation();
        }

        void Update()
        {

            bool targetIsDead;
            bool targetIsOutOfRange;

            if(target == null)
            {
                targetIsDead = false;
                targetIsOutOfRange = false;
            }
            else
            {
                //test if target is dead
                var targetHealth = target.GetComponent<HealthSystem>().HealthAsPercentage;
                targetIsDead = targetHealth <= Mathf.Epsilon;

                //test if target is out of range
                var distanceToTarget = Vector3.Distance(transform.position, target.transform.position);
                targetIsOutOfRange = distanceToTarget > GetCurrentWeaponConfig().GetMaxAttackRange();
            }

            float characterHealth = GetComponent<HealthSystem>().HealthAsPercentage;
            bool characterIsDead = (characterHealth <= Mathf.Epsilon);

            if(characterIsDead || targetIsDead ||targetIsOutOfRange)
            {
                StopAllCoroutines();
            }
        }

        private void SetAttackAnimation()
        {
            if (!character.GetAnimOverrideController())
            {
                Debug.Break();
                Debug.LogAssertion("Please provide " + gameObject + " with an animator override controller.");
            }
            else
            {

                animator = GetComponent<Animator>();
                var animOverrideController = character.GetAnimOverrideController();
                animator.runtimeAnimatorController = animOverrideController;
                animOverrideController[DEFAULT_ATTACK] = currentWeaponConfig.GetWeaponAttackAnimationClip();

            }

        }

        public void AttackTarget(GameObject targetToAttack)
        {
            target = targetToAttack;
            StartCoroutine(AttackTargetRepeatedly());
        }

        IEnumerator AttackTargetRepeatedly()
        {
            //determine if we alive
            bool attackerStillAlive = GetComponent<HealthSystem>().HealthAsPercentage >= Mathf.Epsilon;
            bool targetStillAlive = target.GetComponent<HealthSystem>().HealthAsPercentage >= Mathf.Epsilon;

            while (attackerStillAlive && targetStillAlive)
            {
                float weaponHitPeriod = currentWeaponConfig.GetMinTimeBetweenHits();
                float timeToWaitForAttack = weaponHitPeriod * character.getAnimSpeedMultiplier();

                bool isTimeToHitAgain = Time.time - lastHitTime > timeToWaitForAttack;

                if (isTimeToHitAgain)
                {
                    AttackTargetOnce();
                    print(lastHitTime);
                    lastHitTime = Time.time;
                }

                yield return new WaitForSeconds(timeToWaitForAttack);
            }
        }

        private void AttackTargetOnce()
        {
            transform.LookAt(target.transform);
            animator.SetTrigger(ATTACK_TRIGGER);
            float damageDelay = currentWeaponConfig.GetDamageDelay();
            SetAttackAnimation();
            StartCoroutine(DamageAfterDelay(damageDelay));
        }

        private IEnumerator DamageAfterDelay(float damageDelay)
        {
            print("Attacking Target: " + target);
            yield return new WaitForSecondsRealtime(damageDelay);
            target.GetComponent<HealthSystem>().TakeDamage(CalculateDamage());
            print("damage after delay for " + target);
        }

        //private void AttackTarget()
        //{
        //    if (Time.time - lastHitTime > currentWeaponConfig.GetMinTimeBetweenHits())
        //    {
        //        SetAttackAnimation();
        //        animator.SetTrigger(ATTACK_TRIGGER);
        //        lastHitTime = Time.time;
        //    }
        //}

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
            Assert.IsFalse(numberOfDominantHands <= 0, "No dominant hand found, please add one!" + gameObject);
            Assert.IsFalse(numberOfDominantHands < 1, "Multiple dominantHands found, please remove until one is left!" + gameObject);

            return dominantHands[0].gameObject;
        }

        public void StopAttacking()
        {
            animator.StopPlayback();
            StopAllCoroutines();
        }
    }
}
