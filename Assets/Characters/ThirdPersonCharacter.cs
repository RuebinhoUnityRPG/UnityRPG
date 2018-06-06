using UnityEngine;

//TODO change to RPG:Characters namespace
namespace RPG.Characters
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CapsuleCollider))]
    [RequireComponent(typeof(Animator))]
    public class ThirdPersonCharacter : MonoBehaviour
    {
        [SerializeField] float MovingTurnSpeed = 360;
        [SerializeField] float StationaryTurnSpeed = 180;
        [SerializeField] float moveThreshold = 1f; 

        Rigidbody myRigidbody;
        Animator animator;
        float turnAmount;
        float forwardAmount;
        float m_MoveSpeedMultiplier = 1;

        void Start()
        {
            animator = GetComponent<Animator>();
            myRigidbody = GetComponent<Rigidbody>();
            myRigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;

            animator.applyRootMotion = true;
        }


        public void Move(Vector3 movement)
        {
            SetForwardAndTurn(movement);
            ApplyExtraTurnRotation();
            // send input and other state parameters to the animator
            UpdateAnimator();
        }

        private void SetForwardAndTurn(Vector3 movement)
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
        }

        void ApplyExtraTurnRotation()
        {
            // help the character turn faster (this is in addition to root rotation in the animation)
            float turnSpeed = Mathf.Lerp(StationaryTurnSpeed, MovingTurnSpeed, forwardAmount);
            transform.Rotate(0, turnAmount * turnSpeed * Time.deltaTime, 0);
        }
    }
}
