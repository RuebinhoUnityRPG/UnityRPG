using System;
using UnityEngine;
using UnityEngine.AI;
using RPG.CameraUI; //TODO consider re-wiring

namespace RPG.Characters
{
    [SelectionBase]
    public class Character : MonoBehaviour
    {
        #region variables
        [Header("Animator Setup Settings")]
        [SerializeField]
        RuntimeAnimatorController animatorController;
        [SerializeField] AnimatorOverrideController animatorOverrideController;
        [SerializeField] Avatar animatorCharacterAvatar;

        [Header("AudioSource Setup Settings")]
        [SerializeField]
        float audioSourceSpatialBlend = 0f;

        [Header("Capsule Collider Setup Settings")]
        [SerializeField]
        Vector3 colliderCenter = new Vector3(0, 1.03f, 0);
        [SerializeField] float colliderRadius = 0.2f;
        [SerializeField] float colliderHeight = 1.6f;

        [Header("NavMeshAgent Setup Settings")]
        [SerializeField]
        float navMeshAgentSteeringSpeed = 1f;
        [SerializeField] float navMeshAgentStoppingDistance = 1f;

        [Header("Character Movement Settings")]
        [SerializeField]
        float moveSpeedMultiplier = 0.7f;
        [SerializeField] float animationSpeedMultiplier = 0.7f;
        [SerializeField] float movingTurnSpeed = 360f;
        [SerializeField] float stationaryTurnSpeed = 180f;
        [SerializeField] float moveThreshold = 1f;

        GameObject walkTarget;
        NavMeshAgent navMeshAgent;

        Animator animator;
        Rigidbody myRigidbody;

        bool isInDirectMode = false;

        float turnAmount;
        float forwardAmount;
        float m_MoveSpeedMultiplier = 1;

        bool isAlive = true;
        #endregion

        private void Awake()
        {
            AddRequiredComponents();
        }

        private void AddRequiredComponents()
        {
            animator = gameObject.AddComponent<Animator>();
            animator.runtimeAnimatorController = animatorController;
            animator.avatar = animatorCharacterAvatar;

            var capsuleCollider = gameObject.AddComponent<CapsuleCollider>();
            capsuleCollider.center = colliderCenter;
            capsuleCollider.radius = colliderRadius;
            capsuleCollider.height = colliderHeight;

            myRigidbody = gameObject.AddComponent<Rigidbody>();
            myRigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;

            var audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.spatialBlend = audioSourceSpatialBlend;

            navMeshAgent = gameObject.AddComponent<NavMeshAgent>();
            navMeshAgent.speed = navMeshAgentSteeringSpeed;
            navMeshAgent.stoppingDistance = navMeshAgentStoppingDistance;
            navMeshAgent.autoBraking = false;

            navMeshAgent.updatePosition = false;
            navMeshAgent.updateRotation = false;

        }

        private void Start()
        {
            animator.applyRootMotion = true;
        }


        void Update()
        {
            if (navMeshAgent.remainingDistance > navMeshAgent.stoppingDistance && isAlive)
            {
                Move(navMeshAgent.desiredVelocity);
            }
            else
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

        void Move(Vector3 movement)
        {
            SetForwardAndTurn(movement);
            ApplyExtraTurnRotation();
            // send input and other state parameters to the animator
            UpdateAnimator();
        }

        public void SetDestination(Vector3 worldPosition)
        {
            navMeshAgent.destination = worldPosition;
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
            isAlive = false;
        }
    }
}

