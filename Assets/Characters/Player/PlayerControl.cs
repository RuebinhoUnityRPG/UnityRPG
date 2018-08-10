using System.Collections;
using UnityEngine;
using RPG.CameraUI; //for mouse events

namespace RPG.Characters
{
    public class PlayerControl : MonoBehaviour
    {
        WeaponSystem weaponSystem;
        SpecialAbilities abilities;

        Character character;

        private void Start()
        {
            character = GetComponent<Character>();
            weaponSystem = GetComponent<WeaponSystem>();
            abilities = GetComponent<SpecialAbilities>();
            RegisterForMouseEvents();
        }

        private void RegisterForMouseEvents()
        {
            var cameraRayCaster = FindObjectOfType<CameraRaycaster>();
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

        void OnMouseOverEnemyHit(EnemyAI enemyToSet)
        {
            if (Input.GetMouseButton(0) && IsTargetInRange(enemyToSet.gameObject))
            {
                weaponSystem.AttackTarget(enemyToSet.gameObject);
            }
            else if(Input.GetMouseButton(0) && !IsTargetInRange(enemyToSet.gameObject))
            {
                //move and attack
                StartCoroutine(MoveAndAttackTarget(enemyToSet));

            }
            else if (Input.GetMouseButtonDown(1) && IsTargetInRange(enemyToSet.gameObject))
            {
                abilities.AttemptSpecialAbility(0, enemyToSet.gameObject);
            }
            else if (Input.GetMouseButtonDown(1) && !IsTargetInRange(enemyToSet.gameObject))
            {
                //move and special ability
                StartCoroutine(MoveAndPowerAttackTarget(enemyToSet));
            }

        }

        IEnumerator MoveIfTargetOutOfRange(GameObject target)
        {
            character.SetDestination(target.transform.position);
            
            while (!IsTargetInRange(target.gameObject))
            {
                yield return new WaitForEndOfFrame();
            }

            yield return new WaitForEndOfFrame();
        }

        IEnumerator MoveAndAttackTarget(EnemyAI enemy)
        {
            yield return StartCoroutine(MoveIfTargetOutOfRange(enemy.gameObject));
            weaponSystem.AttackTarget(enemy.gameObject);
        }

        IEnumerator MoveAndPowerAttackTarget(EnemyAI enemy)
        {
            yield return StartCoroutine(MoveIfTargetOutOfRange(enemy.gameObject));
            abilities.AttemptSpecialAbility(0, enemy.gameObject);
        }

        bool IsTargetInRange(GameObject target)
        {
            float distanceToTarget = (target.transform.position - transform.position).magnitude;
            return distanceToTarget <= weaponSystem.GetCurrentWeaponConfig().GetMaxAttackRange();
        }

        private void OnMouseOverPotentiallyWalkable(Vector3 destination)
        {
            if (Input.GetMouseButton(0))
            {
                weaponSystem.StopAttacking();
                character.SetDestination(destination);
            }
        }
    }
}
