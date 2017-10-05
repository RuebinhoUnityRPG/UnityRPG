using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, IDamagable
{

    [SerializeField] int enemyLayer = 9;
    [SerializeField] float maxHealthPoints = 200f;
    [SerializeField] float damagePerHit = 20f;
    [SerializeField] float minTimeBetweenHits = 0.5f;
    [SerializeField] float maxAttackRange = 2f;

    [SerializeField] Weapon weaponInUse;
    [SerializeField] GameObject weaponSocket;

    GameObject currentTarget;
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
        var weapon = Instantiate(weaponPrefab, weaponSocket.transform);

        weapon.transform.localPosition = weaponInUse.gripTransform.localPosition;
        weapon.transform.localRotation= weaponInUse.gripTransform.localRotation;
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

            currentTarget = enemy;
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
