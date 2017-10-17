using UnityEngine;
using UnityEngine.UI;
using RPG.CameraUI;

namespace RPG.Characters
{
    public class Energy : MonoBehaviour
    {
        [SerializeField] RawImage energyBarImage;
        [SerializeField] float maxEnergyPoints = 100f;
        [SerializeField] float pointsPerHit = 10f;

        CameraRaycaster cameraRayCaster = null;

        float currentEnergyPoints;

        // Use this for initialization
        void Start()
        {
            currentEnergyPoints = maxEnergyPoints;
            RegisterForMouseClick();
        }

        private void RegisterForMouseClick()
        {
            cameraRayCaster = FindObjectOfType<CameraRaycaster>();
            cameraRayCaster.OnMouseOverEnemyHit += OnMouseOverEnemyHit;
        }

        void OnMouseOverEnemyHit(Enemy enemy)
        {
            if (Input.GetMouseButtonDown(1))
            {
                UpdateEnergyPoints();
                UpdateEnergyBar();
            }
        }

        private void UpdateEnergyPoints()
        {
            float newEnergyPoints = currentEnergyPoints - pointsPerHit;
            currentEnergyPoints = Mathf.Clamp(newEnergyPoints, 0, maxEnergyPoints);
        }

        private void UpdateEnergyBar()
        {
            float xValue = -(EnergyAsPercentage() / 2f) - 0.5f;
            energyBarImage.uvRect = new Rect(xValue, 0f, 0.5f, 1f);
        }

        private float EnergyAsPercentage()
        {
            return currentEnergyPoints / maxEnergyPoints;
        }
    }
}
