using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SpatialTracking; // Needed to disable tracking

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

        ReleaseHandsFromWheel(leftTriggerPressed, rightTriggerPressed);
        ConvertHandRotation();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player Hand"))
        {
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
            Vector3 targetPos;

            if (rightHandOnWheel && leftHandOnWheel) {
                targetPos = (rightHand.transform.position + leftHand.transform.position) / 2f;
            } else {
                targetPos = rightHandOnWheel ? rightHand.transform.position : leftHand.transform.position;
            }
            
            Vector3 localPos = transform.InverseTransformPoint(targetPos);
            localPos.z = 0;
            
            float angle = Mathf.Atan2(localPos.y, localPos.x) * Mathf.Rad2Deg;
            
            transform.localRotation = Quaternion.Euler(0, 0, angle - 90f);
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
        
        handOnWheel = true;
    }
}
