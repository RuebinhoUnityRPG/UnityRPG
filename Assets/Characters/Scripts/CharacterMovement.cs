using System;
using UnityEngine;
using UnityEngine.AI;
using RPG.CameraUI; //TODO consider re-wiring

namespace RPG.Characters
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class CharacterMovement : MonoBehaviour
    {
        [SerializeField] float stoppingDistance = 1f;
        [SerializeField] float moveSpeedMultiplier = 0.7f;
        [SerializeField] float animationSpeedMultiplier = 0.7f;

        Vector3 clickPoint;
        GameObject walkTarget;
        NavMeshAgent navMeshAgent;

        Animator animator;
        Rigidbody myRigidbody;

        bool isInDirectMode = false;

        [SerializeField] float movingTurnSpeed = 360;
        [SerializeField] float stationaryTurnSpeed = 180;
        [SerializeField] float moveThreshold = 1f;

        float turnAmount;
        float forwardAmount;
        float m_MoveSpeedMultiplier = 1;

        private void Start()
        {
            CameraRaycaster cameraRaycaster = Camera.main.GetComponent<CameraRaycaster>();
            walkTarget = new GameObject("Walktarget");

            navMeshAgent = GetComponent<NavMeshAgent>();
            navMeshAgent.updatePosition = false;
            navMeshAgent.updateRotation = false;
            navMeshAgent.stoppingDistance = stoppingDistance;

            cameraRaycaster.OnMouseOverPotentiallyWalkable += OnMouseOverPotentiallyWalkable;
            cameraRaycaster.OnMouseOverEnemyHit += OnMouseOverEnemyHit;

            animator = GetComponent<Animator>();
            animator.applyRootMotion = true;

            myRigidbody = GetComponent<Rigidbody>();
            myRigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
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
                Move(navMeshAgent.desiredVelocity);
            } else
            {
                Move(Vector3.zero);
            }



            //Commented Code is for direct input

            //if (Input.GetKeyDown(KeyCode.G)) //G for gamepad TODO allow remap later or change in menu
            //{
            //    isInDirectMode = !isInDirectMode;
            //    print("Controlmode Controller: " + isInDirectMode);
            //}
        }

        public void OnAnimatorMove()
        {
            // we implement this function to override the default root motion. 
            // this allows us to modify the positional speed before it's applied. 
            if (Time.deltaTime > 0)
            {
                Vector3 velocity = (animator.deltaPosition * moveSpeedMultiplier) / Time.deltaTime;

                // we preserve the existing y part of the current velocity. 
                velocity.y = myRigidbody.velocity.y;
                myRigidbody.velocity = velocity;
            }

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

        public void Move(Vector3 movement)
        {
            SetForwardAndTurn(movement);
            ApplyExtraTurnRotation();
            // send input and other state parameters to the animator
            UpdateAnimator();
        }

        void SetForwardAndTurn(Vector3 movement)
        {
            // convert the world relative moveInput vector into a local-relative
            // turn amount and forward amount required to head in the desired direction.
            if (movement.magnitude > moveThreshold)
            {
                movement.Normalize();
            }
            var localMovement = transform.InverseTransformDirection(movement);
            turnAmount = Mathf.Atan2(localMovement.x, localMovement.z);
            forwardAmount = localMovement.z;
        }

        void UpdateAnimator()
        {
            // update the animator parameters
            animator.SetFloat("Forward", forwardAmount, 0.1f, Time.deltaTime);
            animator.SetFloat("Turn", turnAmount, 0.1f, Time.deltaTime);
            animator.speed = animationSpeedMultiplier;
        }

        void ApplyExtraTurnRotation()
        {
            // help the character turn faster (this is in addition to root rotation in the animation)
            float turnSpeed = Mathf.Lerp(stationaryTurnSpeed, movingTurnSpeed, forwardAmount);
            transform.Rotate(0, turnAmount * turnSpeed * Time.deltaTime, 0);
        }

        public void Kill()
        {
            // to allow death signaling and stop movement
        }
    }
}

