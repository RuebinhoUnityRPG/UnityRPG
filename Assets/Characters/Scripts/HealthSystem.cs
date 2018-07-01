using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace RPG.Characters
{
    public class HealthSystem : MonoBehaviour
    {
        Character characterMovement;

        [SerializeField] float maxHealthPoints = 200f;
        [SerializeField] Image healthBar;
        float currentHealthPoints;

        AudioSource audioSource;
        //Arrays for sounds
        [SerializeField] AudioClip[] damageSounds;
        [SerializeField] AudioClip[] deathSounds;

        // maybe a parameter for char disappearing
        [SerializeField] float deathVanishSeconds = 3f;

        Animator animator;
        const string DEATH_TRIGGER = "Death";

        // Use this for initialization
        void Start()
        {
            animator = GetComponent<Animator>();
            audioSource = GetComponent<AudioSource>();
            characterMovement = GetComponent<Character>();
            currentHealthPoints = maxHealthPoints;
        }

        // Update is called once per frame
        void Update()
        {
            UpdateHealthBar();
        }

        private void UpdateHealthBar()
        {
            if (healthBar) // enemies may not have health bars
            {
                healthBar.fillAmount = HealthAsPercentage;
            }
        }

        public float HealthAsPercentage
        {
            get
            {
                return currentHealthPoints / maxHealthPoints;
            }
        }

        public void TakeDamage(float damage)
        {

            bool charDies = (currentHealthPoints - damage <= 0); // must ask before reducing health

            currentHealthPoints = Mathf.Clamp(currentHealthPoints - damage, 0f, maxHealthPoints);
            // play sound
            var clip = damageSounds[(int)UnityEngine.Random.Range(0f, damageSounds.Length)];
            audioSource.PlayOneShot(clip);
            //Debug.Log(audioSource.clip);

            if (charDies)
            {
                StartCoroutine(KillCharacter());
            }

        }

        public void Heal(float healPoints)
        {
            currentHealthPoints = Mathf.Clamp(currentHealthPoints + healPoints, 0f, maxHealthPoints);
        }

        IEnumerator KillCharacter()
        {
            StopAllCoroutines();
            characterMovement.Kill();

            //Trigger Death Animation (optional)
            Debug.Log("Death Animation");
            animator.SetTrigger(DEATH_TRIGGER);

            var playerComponent = GetComponent<Player>();
            if (playerComponent && playerComponent.isActiveAndEnabled) //rely on lazy evaluation
            {
                audioSource.clip = deathSounds[(int)UnityEngine.Random.Range(0f, deathSounds.Length)];
                audioSource.Play(); // override any existing sounds

                //Reload the scene after some seconds or player key press input
                yield return new WaitForSecondsRealtime(audioSource.clip.length);
                SceneManager.LoadScene(0);
            } else //assume is enmy for now, reconsider on other NPCs
            {
                DestroyObject(gameObject, deathVanishSeconds);
            }

            //Play Death Sound (optional)
            Debug.Log("Death Sound");
            AudioClip deathSound = deathSounds[(int)UnityEngine.Random.Range(0f, deathSounds.Length)];
            audioSource.clip = deathSound;
            audioSource.Play();
            Debug.Log(audioSource.clip);

            
            
        }
    }
}
