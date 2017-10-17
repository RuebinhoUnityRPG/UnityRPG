using System;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;
using UnityEngine.AI;
using RPG.CameraUI; //TODO consider re-wiring

namespace RPG.Characters
{
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(AICharacterControl))]
    [RequireComponent(typeof(ThirdPersonCharacter))]
    public class PlayerMovement : MonoBehaviour
    {

        ThirdPersonCharacter thirdPersonCharacter = null;   // A reference to the ThirdPersonCharacter on the object
        CameraRaycaster cameraRaycaster = null;
        Vector3 clickPoint;
        AICharacterControl aiCharControl = null;
        GameObject walkTarget = null;

        bool isInDirectMode = false;

        private void Start()
        {
            cameraRaycaster = Camera.main.GetComponent<CameraRaycaster>();
            thirdPersonCharacter = GetComponent<ThirdPersonCharacter>();
            aiCharControl = GetComponent<AICharacterControl>();
            walkTarget = new GameObject("Walktarget");

            cameraRaycaster.OnMouseOverPotentiallyWalkable += OnMouseOverPotentiallyWalkable;
            cameraRaycaster.OnMouseOverEnemyHit += OnMouseOverEnemyHit;
        }

        private void OnMouseOverEnemyHit(Enemy enemy)
        {
            if (Input.GetMouseButton(0) || Input.GetMouseButtonDown(1))
            {
                aiCharControl.SetTarget(enemy.transform);
            }
        }

        private void OnMouseOverPotentiallyWalkable(Vector3 destination)
        {
            if (Input.GetMouseButton(0))
            {
                walkTarget.transform.position = destination;
                aiCharControl.SetTarget(walkTarget.transform);
            }
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.G)) //G for gamepad TODO allow remap later or change in menu
            {
                isInDirectMode = !isInDirectMode;
                print("Controlmode Controller: " + isInDirectMode);
            }
        }

        // Fixed update is called in sync with physics
        void FixedUpdate()
        {
            if (isInDirectMode)
            {
                ProcessDirectMovement();
            }
            else
            {
                //ProcessMouseMovement(); // Mouse movement
            }

        }

        void ProcessDirectMovement()
        {
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical"); ;

            // calculate camera relative direction to move:
            Vector3 CameraForward = Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;
            Vector3 movement = v * CameraForward + h * Camera.main.transform.right;

            thirdPersonCharacter.Move(movement, false, false);
        }
    }
}

