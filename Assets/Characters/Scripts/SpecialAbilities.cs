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
            if(currentEnergyPoints < maxEnergyPoints)
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

        public void AttemptSpecialAbility(int abilityIndex)
        {
            var energyCost = abilities[abilityIndex].GetEnergyCost();

            if (energyCost <= currentEnergyPoints) //TODO read from SO
            {
                ConsumeEnergy(energyCost);
                //Use the ability
                print("Using special ability: " + abilityIndex); //todo make work
                //var abilityParams = new AbilityUseParams(enemy, baseDamage);
                //abilities[abilityIndex].Use(abilityParams);
            } else
            {
                //todo play out of energy sound
            }

        }

    }
}
