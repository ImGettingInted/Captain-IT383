using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SpatialTracking; // Needed to disable tracking

public class WheelControlV2 : MonoBehaviour
{
    public InputActionProperty leftTrigger;
    public InputActionProperty rightTrigger;

    private float wheelStartAngle;
    private float handStartAngle;
    
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
        if (leftTriggerPressed)
        {
            Debug.Log("Left Trigger Pressed");
        }
        bool rightTriggerPressed = rightTrigger.action.ReadValue<float>() > 0.1f;
        if (rightTriggerPressed)
        {
            Debug.Log("Right Trigger Pressed");
        }
        
        ReleaseHandsFromWheel(leftTriggerPressed, rightTriggerPressed);
        ConvertHandRotation();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player Hand"))
        {
            Debug.Log("Player Hand inside wheel");
            if (!rightHandOnWheel && rightTrigger.action.ReadValue<float>() > 0.1f && other.gameObject == rightHand)
                PlaceHandOnWheel(ref rightHand, ref rightHandOriginalParent, ref rightHandOnWheel);
            
            if (!leftHandOnWheel && leftTrigger.action.ReadValue<float>() > 0.1f && other.gameObject == leftHand)
                PlaceHandOnWheel(ref leftHand, ref leftHandOriginalParent, ref leftHandOnWheel);
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
        
        if (hand.TryGetComponent<TrackedPoseDriver>(out var driver)) driver.enabled = true;
        hand.transform.SetPositionAndRotation(parent.position, parent.rotation);
    }

    private void ConvertHandRotation()
    {
        if (rightHandOnWheel || leftHandOnWheel)
        {
            Debug.Log("Hand on Wheel");
            Vector3 targetPos = (rightHandOnWheel && leftHandOnWheel) 
                ? (rightHand.transform.position + leftHand.transform.position) / 2f 
                : (rightHandOnWheel ? rightHand.transform.position : leftHand.transform.position);

            Vector3 localPos = transform.InverseTransformPoint(targetPos);
            float currentHandAngle = Mathf.Atan2(localPos.y, localPos.x) * Mathf.Rad2Deg;

            float angleDelta = currentHandAngle - handStartAngle;
            
            transform.localRotation = Quaternion.Euler(0, 0, wheelStartAngle + angleDelta);
        }
        else 
        {
            transform.localRotation = Quaternion.Euler(0f, 0f, transform.localEulerAngles.z);
        }
    }
    
    private void PlaceHandOnWheel(ref GameObject hand, ref Transform originalParent, ref bool handOnWheel)
    {
        Transform bestSnap = snapPositions[0];
        float shortestDistance = Vector3.Distance(bestSnap.position, hand.transform.position);
        
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
        
        if (hand.TryGetComponent<TrackedPoseDriver>(out var driver)) driver.enabled = false;
        
        hand.transform.parent = bestSnap.transform;
        hand.transform.localPosition = Vector3.zero;
        hand.transform.localRotation = Quaternion.identity;
        
        wheelStartAngle = transform.localEulerAngles.z;
        Vector3 localPos = transform.InverseTransformPoint(hand.transform.position);
        handStartAngle = Mathf.Atan2(localPos.y, localPos.x) * Mathf.Rad2Deg;
        
        handOnWheel = true;
    }
}
