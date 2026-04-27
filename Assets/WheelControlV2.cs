/* using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class WheelControlV2 : XRBaseInteractable
{
    [SerializeField] private Transform wheelTransform;

    public UnityEvent<float> OnWheelRotated;

    private float currentAngle = 0.0f;

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);
        currentAngle = FindWheelAngle();
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);
        currentAngle = FindWheelAngle();
    }

    public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
    {
        base.ProcessInteractable(updatePhase);

        if (updatePhase == XRInteractionUpdateOrder.UpdatePhase.Dynamic)
        {
            if (isSelected)
                RotateWheel();
        }
    }

    private void RotateWheel()
    {
        // Convert that direction to an angle, then rotation
        float totalAngle = FindWheelAngle();

        // Apply difference in angle to wheel
        float angleDifference = currentAngle - totalAngle;
        wheelTransform.Rotate(transform.forward, -angleDifference, Space.Self);
            
        // Store angle for next process
        currentAngle = totalAngle;
        OnWheelRotated?.Invoke(angleDifference);
    }

    private float FindWheelAngle()
    {
        float totalAngle = 0;

        // Combine directions of current interactors
        foreach (IXRSelectInteractor interactor in interactorsSelecting)
        {
            Vector2 direction = FindLocalPoint(interactor.transform.position);
            totalAngle += ConvertToAngle(direction) * FindRotationSensitivity();
        }

        return totalAngle;
    }

    private Vector2 FindLocalPoint(Vector3 position)
    {
        // Convert the hand positions to local, so we can find the angle easier
        return transform.InverseTransformPoint(position).normalized;
    }

    private float ConvertToAngle(Vector2 direction)
    {
        // Use a consistent up direction to find the angle
        return Vector2.SignedAngle(Vector2.up, direction);
    }

    private float FindRotationSensitivity()
    {
        // Use a smaller rotation sensitivity with two hands
        return 1.0f / interactorsSelecting.Count;
    }
}
*/
using System;
using Unity.XR.OpenVR;
using UnityEngine;
using UnityEngine.InputSystem;

public class WheelControlV2 : MonoBehaviour
{
    public InputActionProperty leftTrigger;
    public InputActionProperty rightTrigger;

    public GameObject rightHand;
    private Transform rightHandOriginalParent;
    private bool rightHandOnWheel = false;
    
    public GameObject leftHand;
    private Transform leftHandOriginalParent;
    private bool leftHandOnWheel = false;

    public Transform[] snapPositions;
    public GameObject vehicle;

    void Update()
    {
        bool leftTriggerPressed = leftTrigger.action.ReadValue<float>() > 0.1f;
        bool rightTriggerPressed = rightTrigger.action.ReadValue<float>() > 0.1f;
        if (leftTriggerPressed)
        {
            Debug.Log("leftTriggerPressed");
        }
        if (rightTriggerPressed)
        {
            Debug.Log("rightTriggerPressed");
        }
        ReleaseHandsFromWheel(leftTriggerPressed, rightTriggerPressed);
        ConvertHandRotation();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player Hand"))
        {
            Debug.Log("player hand on wheel");
            if (!rightHandOnWheel && rightTrigger.action.ReadValue<float>() > 0.1f)
                PlaceHandOnWheel(ref rightHand, ref rightHandOriginalParent, ref rightHandOnWheel);
            
            if (!leftHandOnWheel && leftTrigger.action.ReadValue<float>() > 0.1f)
                PlaceHandOnWheel(ref leftHand, ref leftHandOriginalParent, ref leftHandOnWheel);
        }
        else
        {
            Debug.Log("not player hand");
        }
    }
    
    private void ReleaseHandsFromWheel(bool leftPressed, bool rightPressed)
    {
        if (rightHandOnWheel && !rightPressed)
        {
            ResetHand(rightHand, rightHandOriginalParent);
            rightHandOnWheel = false;
        }

        if (leftHandOnWheel && !leftPressed)
        {
            ResetHand(leftHand, leftHandOriginalParent);
            leftHandOnWheel = false;
        }
    }

    private void ResetHand(GameObject hand, Transform parent)
    {
        hand.transform.parent = parent;
        hand.transform.SetPositionAndRotation(parent.position, parent.rotation);
    }

    
    private void ConvertHandRotation()
    {
        if (rightHandOnWheel && !leftHandOnWheel)
        {
            Quaternion newRotation = Quaternion.Euler(0, vehicle.transform.rotation.eulerAngles.y, rightHandOriginalParent.transform.rotation.eulerAngles.z);
            transform.rotation = newRotation;
        }
        else if (!rightHandOnWheel && leftHandOnWheel)
        {
            Quaternion newRotation = Quaternion.Euler(0, vehicle.transform.rotation.eulerAngles.y, leftHandOriginalParent.transform.rotation.eulerAngles.z);
            transform.rotation = newRotation;
        }
        else if (rightHandOnWheel && leftHandOnWheel)
        {
            Quaternion newRotationRight = Quaternion.Euler(0, vehicle.transform.rotation.eulerAngles.y, rightHandOriginalParent.transform.rotation.eulerAngles.z);
            Quaternion newRotationLeft = Quaternion.Euler(0, vehicle.transform.rotation.eulerAngles.y, leftHandOriginalParent.transform.rotation.eulerAngles.z);
            Quaternion finalRotation = Quaternion.Slerp(newRotationLeft, newRotationRight, 1.0f/2.0f);
            transform.rotation = finalRotation;
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