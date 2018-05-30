using System;
using UnityEngine;
using UnityEngine.AI;
using RPG.CameraUI; //TODO consider re-wiring

namespace RPG.Characters
{
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(ThirdPersonCharacter))]
    public class CharacterMovement : MonoBehaviour
    {
        [SerializeField] float stoppingDistance = 1f;
        ThirdPersonCharacter character;   // A reference to the ThirdPersonCharacter on the object
        Vector3 clickPoint;
        GameObject walkTarget;
        NavMeshAgent navMeshAgent;

        bool isInDirectMode = false;

        private void Start()
        {
            CameraRaycaster cameraRaycaster = Camera.main.GetComponent<CameraRaycaster>();
            character = GetComponent<ThirdPersonCharacter>();
            walkTarget = new GameObject("Walktarget");

            navMeshAgent = GetComponent<NavMeshAgent>();
            navMeshAgent.updatePosition = false;
            navMeshAgent.updateRotation = false;
            navMeshAgent.stoppingDistance = stoppingDistance;

            cameraRaycaster.OnMouseOverPotentiallyWalkable += OnMouseOverPotentiallyWalkable;
            cameraRaycaster.OnMouseOverEnemyHit += OnMouseOverEnemyHit;
        }

        private void OnMouseOverEnemyHit(Enemy enemy)
        {
            if (Input.GetMouseButton(0) || Input.GetMouseButtonDown(1))
            {
                navMeshAgent.SetDestination(enemy.transform.position);
            }
        }

        private void OnMouseOverPotentiallyWalkable(Vector3 destination)
        {
            if (Input.GetMouseButton(0))
            {
                navMeshAgent.SetDestination(destination);
            }
        }

        void Update()
        {
            if (navMeshAgent.remainingDistance > navMeshAgent.stoppingDistance)
            {
                character.Move(navMeshAgent.desiredVelocity);
            } else
            {
                character.Move(Vector3.zero);
            }



            //Commented Code is for direct input

            //if (Input.GetKeyDown(KeyCode.G)) //G for gamepad TODO allow remap later or change in menu
            //{
            //    isInDirectMode = !isInDirectMode;
            //    print("Controlmode Controller: " + isInDirectMode);
            //}
        }

        // Fixed update is called in sync with physics
        void FixedUpdate()
        {
            if (isInDirectMode)
            {
                //ProcessDirectMovement();
            }
            else
            {
                //ProcessMouseMovement(); // Mouse movement
            }

        }

        //void ProcessDirectMovement()
        //{
        //    float h = Input.GetAxis("Horizontal");
        //    float v = Input.GetAxis("Vertical"); ;

        //    // calculate camera relative direction to move:
        //    Vector3 CameraForward = Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;
        //    Vector3 movement = v * CameraForward + h * Camera.main.transform.right;

        //    character.Move(movement, false, false);
        //}
    }
}

