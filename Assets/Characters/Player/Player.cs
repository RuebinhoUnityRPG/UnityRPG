using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Player : MonoBehaviour, IDamagable
{

    [SerializeField] int enemyLayer = 9;
    [SerializeField] float maxHealthPoints = 200f;
    [SerializeField] float damagePerHit = 20f;
    [SerializeField] float minTimeBetweenHits = 0.5f;
    [SerializeField] float maxAttackRange = 2f;

    [SerializeField] Weapon weaponInUse;

    CameraRaycaster cameraRayCaster;

    private float lastHitTime = 0f;

    float currentHealthPoints;

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
        currentHealthPoints = maxHealthPoints;

        PutWeaponInHand();
    }

    private void RegisterForMouseClick()
    {
        cameraRayCaster = FindObjectOfType<CameraRaycaster>();
        cameraRayCaster.notifyMouseClickObservers += OnMouseClick;
    }

    private void PutWeaponInHand()
    {
        var weaponPrefab = weaponInUse.getWeaponPrefab();
        GameObject dominantHand = RequestDominantHand();
        var weapon = Instantiate(weaponPrefab, dominantHand.transform);

        weapon.transform.localPosition = weaponInUse.gripTransform.localPosition;
        weapon.transform.localRotation= weaponInUse.gripTransform.localRotation;
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

    void OnMouseClick(RaycastHit raycastHit, int layerHit)
    {
        if (layerHit == enemyLayer)
        {
            var enemy = raycastHit.collider.gameObject;

            // Check enemy is in range
            if ((enemy.transform.position - transform.position).magnitude > maxAttackRange)
            {
                return;
            }

            var enemyComponent = enemy.GetComponent<Enemy>();
            if (Time.time - lastHitTime > minTimeBetweenHits)
            {
                enemyComponent.TakeDamage(damagePerHit);
                lastHitTime = Time.time;
            }
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealthPoints = Mathf.Clamp(currentHealthPoints - damage, 0f, maxHealthPoints);

        //if (currentHealthPoints <= 0)
        //{
        //    Destroy(gameObject);
        //}
    }
}
