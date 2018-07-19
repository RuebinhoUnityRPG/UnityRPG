using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using RPG.CameraUI; //for mouse events
using RPG.Core;
using UnityEngine.SceneManagement;

//todo extract weapon system
namespace RPG.Characters
{
    public class PlayerControl : MonoBehaviour
    {
        [Range(0f, 100f)] [SerializeField] float critChanceInPerCent = 20f;
        [SerializeField] float critHitMultiplierInPerCent = 125f;
        [SerializeField] ParticleSystem critParticleSystem = null;
        [SerializeField] AudioClip critAudioClip = null;

        WeaponSystem weaponSystem;
        SpecialAbilities abilities;

        Enemy enemy = null;

        CameraRaycaster cameraRayCaster = null;

        AudioSource audioSource;

        Character character;

        private void Start()
        {
            character = GetComponent<Character>();
            weaponSystem = GetComponent<WeaponSystem>();
            abilities = GetComponent<SpecialAbilities>();
            RegisterForMouseEvents();
            audioSource = GetComponent<AudioSource>();
        }

        private void RegisterForMouseEvents()
        {
            cameraRayCaster = FindObjectOfType<CameraRaycaster>();
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

        void OnMouseOverEnemyHit(Enemy enemyToSet)
        {
            this.enemy = enemyToSet;

            if (Input.GetMouseButton(0) && IsTargetInRange(enemyToSet.gameObject))
            {
                weaponSystem.AttackTarget(enemy.gameObject);
            }
            else if (Input.GetMouseButtonDown(1))
            {
                abilities.AttemptSpecialAbility(0);
            }
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
                print("i wanna run!");
                character.SetDestination(destination);
            }
        }
    }
}
