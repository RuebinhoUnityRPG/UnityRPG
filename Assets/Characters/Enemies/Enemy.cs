using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Core;

namespace RPG.Characters
{
    public class Enemy : MonoBehaviour
    {


        [SerializeField] float chaseRadius = 8f;

        [SerializeField] float attackDamagePerShot = 9f;
        [SerializeField] float attackRadius = 4f;
        [SerializeField] float firingPeriodInSeconds = 0.5f;
        [SerializeField] float firingPeriodVariation = 0.0f;
        [SerializeField] Vector3 AimOffset = new Vector3(0, 1f, 0);

        [SerializeField] GameObject projectileToUse;
        [SerializeField] GameObject projectileSocket;

        bool isAttacking = false;
        float currentHealthPoints;
        PlayerControl player = null;

        private void Start()
        {
            player = FindObjectOfType<PlayerControl>();
            //aiCharacterControl = GetComponent<AICharacterControl>();
        }

        private void Update()
        {
            float distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);
            if (distanceToPlayer <= attackRadius && !isAttacking)
            {
                isAttacking = true;
                float randomisedDelay = firingPeriodInSeconds; //* UnityEngine.Random.Range(firingPeriodInSeconds - firingPeriodVariation, firingPeriodInSeconds + firingPeriodVariation);
                InvokeRepeating("FireProjectile", 0f, randomisedDelay); //TODO Switch to coroutines            
            }

            if (distanceToPlayer > attackRadius && isAttacking)
            {
                isAttacking = false;
                CancelInvoke();
            }

            if (distanceToPlayer <= chaseRadius)
            {
                //aiCharacterControl.SetTarget(player.transform);
            }
            else
            {
                //aiCharacterControl.SetTarget(transform);
            }
        }

        //TODO separate out Character Fire Logic
        void FireProjectile()
        {
            GameObject newProjectile = Instantiate(projectileToUse, projectileSocket.transform.position, Quaternion.identity);
            Projectile projectileComponent = newProjectile.GetComponent<Projectile>();
            projectileComponent.damageCaused = attackDamagePerShot;
            projectileComponent.SetShooter(gameObject);

            Vector3 unitVectorToPlayer = (player.transform.position + AimOffset - projectileSocket.transform.position).normalized;
            float projectileSpeed = projectileComponent.GetDefaultLaunchSpeed();
            newProjectile.GetComponent<Rigidbody>().velocity = unitVectorToPlayer * projectileSpeed;
        }

        private void OnDrawGizmos()
        {
            //Draw chase gizmos
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, chaseRadius);

            //Draw attack gizmos
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRadius);

        }

        public void TakeDamage(float damage)
        {
            //todo remove
        }

    }
}
