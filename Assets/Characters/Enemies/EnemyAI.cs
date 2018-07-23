using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Core;

namespace RPG.Characters
{
    [RequireComponent(typeof(Character))]
    [RequireComponent(typeof(WeaponSystem))]
    public class EnemyAI : MonoBehaviour
    {
        [SerializeField] float chaseRadius = 8f;
        float distanceToPlayer;

        float currentWeaponRange = 4f;

        enum State { attacking, chasing, patrolling, idle};
        State state = State.idle;

        PlayerControl player = null;
        Character character;

        private void Start()
        {
            character = GetComponent<Character>();
            player = FindObjectOfType<PlayerControl>();
            //aiCharacterControl = GetComponent<AICharacterControl>();
        }

        private void Update()
        {
            distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);
            WeaponSystem weaponSystem = GetComponent<WeaponSystem>();
            currentWeaponRange = weaponSystem.GetCurrentWeaponConfig().GetMaxAttackRange();

            if (distanceToPlayer > chaseRadius && state != State.patrolling)
            {
                //stop what we're doing
                // start patrolling
                StopAllCoroutines();
                state = State.patrolling;
            }

            if (distanceToPlayer <= chaseRadius && state != State.chasing)
            {
                //stop what we're doing
                // chase the player
                StopAllCoroutines();
                StartCoroutine(ChasePlayer());

            }

            if (distanceToPlayer <= currentWeaponRange && state != State.attacking)
            {
                //stop what we're doing
                // attack the player
                StopAllCoroutines();
                state = State.attacking;
            }

        }

        IEnumerator ChasePlayer()
        {
            state = State.chasing;
            while (distanceToPlayer >= currentWeaponRange)
            {
                character.SetDestination(player.transform.position);
                yield return new WaitForEndOfFrame();
            }
        }

        private void OnDrawGizmos()
        {
            //Draw chase gizmos
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, chaseRadius);

            //Draw attack gizmos
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, currentWeaponRange);

        }

        public void TakeDamage(float damage)
        {
            //todo remove
        }

    }
}
