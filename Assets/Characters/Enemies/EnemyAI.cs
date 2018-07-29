using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Core;

namespace RPG.Characters
{
    [RequireComponent(typeof(Character))]
    [RequireComponent(typeof(WeaponSystem))]
    [RequireComponent(typeof(HealthSystem))]
    public class EnemyAI : MonoBehaviour
    {
        [SerializeField] float chaseRadius = 8f;
        [SerializeField] WaypointContainer patrolPathStart;
        [SerializeField] float waypointTolerance = 2.0f;
        [SerializeField] float waypointWaitTime = 0.5f;
        
        float distanceToPlayer;
        int nextWaypointIndex;

        float currentWeaponRange = 4f;

        enum State {attacking, chasing, patrolling, idle};
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
                weaponSystem.StopAttacking();
                state = State.patrolling;
                StartCoroutine(Patrol());
            }

            if (distanceToPlayer <= chaseRadius && state != State.chasing)
            {
                //stop what we're doing
                // chase the player
                StopAllCoroutines();
                weaponSystem.StopAttacking();
                state = State.chasing;
                StartCoroutine(ChasePlayer());

            }

            if (distanceToPlayer <= currentWeaponRange && state != State.attacking)
            {
                //stop what we're doing
                // attack the player
                StopAllCoroutines();
                //state = State.attacking;
                weaponSystem.AttackTarget(player.gameObject);
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
        IEnumerator Patrol()
        {
            state = State.patrolling;
            
            while(patrolPathStart != null)
            {
                //work out where to go next
                //tell char to go there
                //cycle waypoints
                //wait at a waypoint

                Vector3 nextWaypointPosition = patrolPathStart.transform.GetChild(nextWaypointIndex).position;
                character.SetDestination(nextWaypointPosition);
                CycleWaypointsWhenClose(nextWaypointPosition);

                yield return new WaitForSeconds(waypointWaitTime);

            }
        }

        private void CycleWaypointsWhenClose(Vector3 nextWaypointPosition)
        {
            if (Vector3.Distance(transform.position, nextWaypointPosition) <= waypointTolerance)
            {
                nextWaypointIndex = (nextWaypointIndex + 1) % patrolPathStart.transform.childCount;
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

    }
}
