using System;
using Unity.XR.OpenVR;
using UnityEngine;
using UnityEngine.InputSystem;

public class WheelController : MonoBehaviour
{
    public bool isBoatActive;
    public GameObject rightHand;
    private Transform rightHandOriginalParent;
    private bool rightHandOnWheel = false;
    [SerializeField]
    private InputActionReference rightTrigger;
    private bool rightTriggerPressed = false;
    
    
    public GameObject leftHand;
    private Transform leftHandOriginalParent;
    private bool leftHandOnWheel = false;
    [SerializeField]
    private InputActionReference leftTrigger;
    private bool leftTriggerPressed = false;
    
    
    public Transform[] snapPositions;

    public Transform forward;
    public GameObject vehicle;
    private Rigidbody vehicleRB;

    [SerializeField]
    float currentWheelRotation;

    private float turnDampening;

    public Transform directionalObject;

    private void OnEnable()
    {
        // Subscribe to events
        rightTrigger.action.performed += OnRightTriggerPressed;
        rightTrigger.action.canceled += OnRightTriggerReleased;

        leftTrigger.action.performed += OnLeftTriggerPressed;
        leftTrigger.action.canceled += OnLeftTriggerReleased;

        // Must enable the actions to receive input
        rightTrigger.action.Enable();
        leftTrigger.action.Enable();
    }

    private void OnDisable()
    {
        // Unsubscribe to prevent memory leaks or errors when object is destroyed
        rightTrigger.action.performed -= OnRightTriggerPressed;
        rightTrigger.action.canceled -= OnRightTriggerReleased;

        leftTrigger.action.performed -= OnLeftTriggerPressed;
        leftTrigger.action.canceled -= OnLeftTriggerReleased;
    }

    // Context contains info about the input, like the value or phase
    private void OnRightTriggerPressed(InputAction.CallbackContext context) => rightTriggerPressed = true;
    private void OnRightTriggerReleased(InputAction.CallbackContext context) => rightTriggerPressed = false;

    private void OnLeftTriggerPressed(InputAction.CallbackContext context) => leftTriggerPressed = true;
    private void OnLeftTriggerReleased(InputAction.CallbackContext context) => leftTriggerPressed = false;
    
    public void Start()
    {
        vehicleRB = vehicle.GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        ReleaseHandsFromWheel();

        ConvertHandRotation();

        TurnVehicle();

        currentWheelRotation = -transform.rotation.eulerAngles.z;
        
        if (isBoatActive)
        {
            Vector3 nextPosition = vehicleRB.position + transform.forward * 1.75f * Time.fixedDeltaTime;
            vehicleRB.MovePosition(nextPosition);
        }
        
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player Hand"))
        {
            
            if (!rightHandOnWheel && rightTriggerPressed)
            {
                PlaceHandOnWheel(ref rightHand, ref rightHandOriginalParent, ref rightHandOnWheel);
            }
            
            if (!leftHandOnWheel && leftTriggerPressed)
            {
                PlaceHandOnWheel(ref leftHand, ref leftHandOriginalParent, ref leftHandOnWheel);
            }
        }
    }

    private void ReleaseHandsFromWheel()
    {
        if (rightHandOnWheel && !rightTriggerPressed)
        {
            rightHand.transform.parent = rightHandOriginalParent;
            rightHand.transform.position = rightHandOriginalParent.position;
            rightHand.transform.rotation = rightHandOriginalParent.rotation;
            rightHandOnWheel = false;
        }

        if (leftHandOnWheel && !leftTriggerPressed)
        {
            leftHand.transform.parent = leftHandOriginalParent;
            leftHand.transform.position = leftHandOriginalParent.position;
            leftHand.transform.rotation = leftHandOriginalParent.rotation;
            leftHandOnWheel = false;
        }

        if (!leftHandOnWheel && !rightHandOnWheel)
        {
            transform.parent = transform.root;
        }
    }

    private void TurnVehicle()
    {
        var turn = -transform.rotation.eulerAngles.z;
        if (turn < -350)
        {
            turn += 360;
        }
        vehicleRB.MoveRotation(Quaternion.RotateTowards(vehicle.transform.rotation, Quaternion.Euler(0, turn, 0), Time.deltaTime * turnDampening));
    }
    private void ConvertHandRotation()
    {
        if (rightHandOnWheel && !leftHandOnWheel)
        {
            Quaternion newRotation = Quaternion.Euler(0, vehicle.transform.rotation.eulerAngles.y, rightHandOriginalParent.transform.rotation.eulerAngles.z);
            directionalObject.rotation = newRotation;
            transform.parent = directionalObject;
        }
        else if (!rightHandOnWheel && leftHandOnWheel)
        {
            Quaternion newRotation = Quaternion.Euler(0, vehicle.transform.rotation.eulerAngles.y, leftHandOriginalParent.transform.rotation.eulerAngles.z);
            directionalObject.rotation = newRotation;
            transform.parent = directionalObject;
        }
        else if (rightHandOnWheel && leftHandOnWheel)
        {
            Quaternion newRotationRight = Quaternion.Euler(0, vehicle.transform.rotation.eulerAngles.y, rightHandOriginalParent.transform.rotation.eulerAngles.z);
            Quaternion newRotationLeft = Quaternion.Euler(0, vehicle.transform.rotation.eulerAngles.y, leftHandOriginalParent.transform.rotation.eulerAngles.z);
            Quaternion finalRotation = Quaternion.Slerp(newRotationLeft, newRotationRight, 1.0f/2.0f);
            directionalObject.rotation = finalRotation;
            transform.parent = directionalObject;
        }
    }
    
    
    private void PlaceHandOnWheel(ref GameObject hand, ref Transform originalParent, ref bool handOnWheel)
    {
        var shortestDistance = Vector3.Distance(snapPositions[0].position, hand.transform.position);
        var bestSnap = snapPositions[0];
        
        foreach (var snapPosition in snapPositions)
        {
            if (snapPosition.childCount == 0)
            {
                var distance = Vector3.Distance(snapPosition.position, hand.transform.position);
                if (distance < shortestDistance)
                {
                    shortestDistance = distance;
                    bestSnap = snapPosition;
                }
            }
        }
        
        originalParent = hand.transform.parent;

        hand.transform.parent = bestSnap.transform;
        hand.transform.position = bestSnap.transform.position;
        
        handOnWheel = true;
    }
}
