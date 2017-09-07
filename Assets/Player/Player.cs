using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, IDamagable {

    [SerializeField] float maxHealthPoints = 200f;

    float currentHealthPoints = 200f;

    public float healthAsPercentage
    {
        get
        {
            return currentHealthPoints / maxHealthPoints;
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealthPoints = Mathf.Clamp(currentHealthPoints - damage, 0f, maxHealthPoints);

        if(currentHealthPoints <= 0)
        {
            Destroy(gameObject);
        }
    }
}
