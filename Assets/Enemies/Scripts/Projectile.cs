using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

    public float damageCaused;
    public float projectileSpeed;

    private Vector3 startPosition;
    private float xPosEnd = 20;
    private float yPosEnd = 20;
    private float zPosEnd = 20;

    private void Start()
    {
        startPosition = gameObject.transform.position;
    }

    private void OnTriggerEnter(Collider collider)
    {
        Component damagableComponent = collider.gameObject.GetComponent(typeof(IDamagable));
        if(damagableComponent)
        {
            (damagableComponent as IDamagable).TakeDamage(damageCaused);
            Destroy(gameObject);
        } else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if ((transform.position.x > (startPosition.x + xPosEnd)) || (transform.position.y > (startPosition.y + yPosEnd)) || (transform.position.y > (startPosition.y + yPosEnd)))
        {
            Destroy(gameObject);
        }
    }
}
