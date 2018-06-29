using UnityEngine;
using UnityEngine.UI;

namespace RPG.Characters
{
    public class SpecialAbilities : MonoBehaviour
    {
        //Temp for debugging
        [SerializeField] AbilityConfig[] abilities;

        [SerializeField] Image energyOrbImage = null;
        [SerializeField] float maxEnergyPoints = 100f;
        [SerializeField] float regenPointsPerSecond = 1f;
        // todo add OutOfEnergySound;
        [SerializeField] AudioClip[] outOfEnergySounds;
        int outOfEnergyCounter = 0;

        float currentEnergyPoints;
        AudioSource audioSource;

        private float EnergyAsPercentage()
        {
            return currentEnergyPoints / maxEnergyPoints;
        }

        // Use this for initialization
        void Start()
        {
            currentEnergyPoints = maxEnergyPoints;

            AttachInitialAbilities();

            UpdateEnergyBar();

            audioSource = GetComponent<AudioSource>();
        }

        private void Update()
        {
            if (currentEnergyPoints < maxEnergyPoints)
            {
                RegenEnergy();
                UpdateEnergyBar();
            }
        }

        private void RegenEnergy()
        {
            var energyToRegen = regenPointsPerSecond * Time.deltaTime;
            currentEnergyPoints = Mathf.Clamp(currentEnergyPoints + energyToRegen, 0, maxEnergyPoints);
        }

        public void ConsumeEnergy(float amount)
        {
            float newEnergyPoints = currentEnergyPoints - amount;
            currentEnergyPoints = Mathf.Clamp(newEnergyPoints, 0, maxEnergyPoints);
            UpdateEnergyBar();
        }

        private void UpdateEnergyBar()
        {
            energyOrbImage.fillAmount = EnergyAsPercentage();
        }

        void AttachInitialAbilities()
        {
            for (int abilityIndex = 0; abilityIndex < abilities.Length; abilityIndex++)
            {
                abilities[abilityIndex].AttachAbilityTo(gameObject);
            }
        }

        public int GetNumberOfAbilities()
        {
            return abilities.Length;
        }

        public void AttemptSpecialAbility(int abilityIndex, GameObject target = null)
        {
            var energyCost = abilities[abilityIndex].GetEnergyCost();

            if (energyCost <= currentEnergyPoints)
            {
                ConsumeEnergy(energyCost);
                outOfEnergyCounter = 0;
                print("ooec: " + outOfEnergyCounter);
                //Use the ability
                print("Using special ability: " + abilityIndex);
                abilities[abilityIndex].Use(target);


            }
            else
            {
                if (outOfEnergyCounter == 0)
                {
                    audioSource.PlayOneShot(outOfEnergySounds[0]);
                    outOfEnergyCounter++;
                }
                else if (outOfEnergyCounter == 1)
                {
                    audioSource.PlayOneShot(outOfEnergySounds[1]);
                    outOfEnergyCounter++;
                }
                else
                {
                    audioSource.PlayOneShot(outOfEnergySounds[2]);
                }
            }

        }

    }
}
