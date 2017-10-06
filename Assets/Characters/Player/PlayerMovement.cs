using System;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(AICharacterControl))]
[RequireComponent(typeof (ThirdPersonCharacter))]
public class PlayerMovement : MonoBehaviour
{

    ThirdPersonCharacter thirdPersonCharacter = null;   // A reference to the ThirdPersonCharacter on the object
    CameraRaycaster cameraRaycaster = null;
    Vector3 clickPoint;
    AICharacterControl aiCharControl = null;
    GameObject walkTarget = null;
        
    bool isInDirectMode = false;

    [SerializeField] const int walkableLayerNumber = 8;
    [SerializeField] const int enemyLayerNumber = 9;

    private void Start()
    {
        cameraRaycaster = Camera.main.GetComponent<CameraRaycaster>();
        thirdPersonCharacter = GetComponent<ThirdPersonCharacter>();
        aiCharControl = GetComponent<AICharacterControl>();
        walkTarget = new GameObject("Walktarget");

        cameraRaycaster.notifyMouseClickObservers += ProcessMouseClick;
    }

    void ProcessMouseClick(RaycastHit raycastHit, int layerHit)
    {
        switch (layerHit)
        {
            case enemyLayerNumber:
                //navigate to enemy
                GameObject enemy = raycastHit.collider.gameObject;
                aiCharControl.SetTarget(enemy.transform);
                break;
            case walkableLayerNumber:
                //navigate to the ground
                walkTarget.transform.position = raycastHit.point;
                aiCharControl.SetTarget(walkTarget.transform);
                break;
            default:
                Debug.Log("Dont know how to handle mouseclick or player movement");
                return;

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
        if(isInDirectMode)
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

