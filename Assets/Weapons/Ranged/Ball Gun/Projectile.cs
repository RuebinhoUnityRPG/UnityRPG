using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Core;

namespace RPG.Characters
{
    public class Projectile : MonoBehaviour
    {

        [SerializeField] GameObject shooter;
        [SerializeField] float projectileSpeed;
        public float damageCaused;

        private Vector3 startPosition;
        private float xPosEnd = 20;
        private float yPosEnd = 20;
        private float zPosEnd = 20;

        public void SetShooter(GameObject shooter)
        {
            this.shooter = shooter;
        }

        private void Start()
        {
            startPosition = gameObject.transform.position;
        }

        private void OnCollisionEnter(Collision collision)
        {
            var layerCollidedWith = collision.gameObject.layer;
            if (shooter && layerCollidedWith != shooter.layer)
            {
                //DamageIfDamagable(collision);
            }
        }

        //todo reimplement

        //private void DamageIfDamagable(Collision collision)
        //{
        //    Component damagableComponent = collision.gameObject.GetComponent(typeof(IDamagable));
        //    if (damagableComponent)
        //    {
        //        (damagableComponent as IDamagable).TakeDamage(damageCaused);
        //        Destroy(gameObject);
        //    }
        //    else
        //    {
        //        Destroy(gameObject);
        //    }

        //}

        private void Update()
        {
            if ((transform.position.x > (startPosition.x + xPosEnd)) || (transform.position.y > (startPosition.y + yPosEnd)) || (transform.position.z > (startPosition.z + zPosEnd)))
            {
                Destroy(gameObject);
            }
        }

        public float GetDefaultLaunchSpeed()
        {
            return projectileSpeed;
        }
    }
}
