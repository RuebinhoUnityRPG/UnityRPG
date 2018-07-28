using System.Collections;
using UnityEngine;

namespace RPG.Characters
{
    public abstract class AbilityBehaviour : MonoBehaviour
    {
        protected AbilityConfig config;

        const string ATTACK_TRIGGER = "Attack";
        const string DEFAULT_ATTACK_STATE = "DEFAULT ATTACK";

        const float PARTICLE_CLEAN_UP_DELAY = 20f;

        public abstract void Use(GameObject target = null);

        public void SetConfig(AbilityConfig configToSet)
        {
            config = configToSet;
        }

        protected void PlayParticleEffect()
        {
            Debug.Log("AOE Attack Particle System triggered");
            //Instanciate ParticleSystem prefab attached to the player
            var particlePrefab = config.GetParticleSystemPrefab();
            var particleObject = Instantiate(particlePrefab, transform.position, particlePrefab.transform.rotation);

            particleObject.transform.parent = transform; // set world space in prefab if required
            //Get the particle system component and play it
            particleObject.GetComponent<ParticleSystem>().Play();
            //Destroy after duration
            StartCoroutine(DestroyParticleWhenFinished(particleObject));
        }

        IEnumerator DestroyParticleWhenFinished(GameObject particlePrefab)
        {
            while (particlePrefab.GetComponent<ParticleSystem>().isPlaying)
            {
                yield return new WaitForSeconds(PARTICLE_CLEAN_UP_DELAY);
            }

            Destroy(particlePrefab);
            yield return new WaitForEndOfFrame();
        }

        protected void PlayAbilitySound()
        {
            print("Ability Sound played");
            var abilitySound = config.GetRandomAudioClipToPlay();
            var audioSource = GetComponent<AudioSource>();
            audioSource.PlayOneShot(abilitySound);
        }

        protected void PlayAbilityAnimationClip()
        {
            print("Ability Animation played");
            var abilityAnimation = config.GetAbilityAnimationClip();
            var animatorOverrideController = GetComponent<Character>().GetAnimOverrideController();
            var animator = GetComponent<Animator>();
            animator.runtimeAnimatorController = animatorOverrideController;

            animatorOverrideController[DEFAULT_ATTACK_STATE] = config.GetAbilityAnimationClip();
            animator.SetTrigger(ATTACK_TRIGGER);
        }
    }
}

