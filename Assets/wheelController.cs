using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class WheelController: MonoBehaviour
{
    public Rigidbody shipRb;
    public HingeJoint wheelHinge;
    
    public bool moveShip;
    public float moveSpeed;
    public float turnPower;

    private float currentYaw;

    void Start()
    {
        currentYaw = shipRb.rotation.eulerAngles.y;
    }
    
    void FixedUpdate()
    {
        if (moveShip)
        {
            float turnInput = wheelHinge.angle / 180f; 
            currentYaw += turnInput * turnPower * Time.fixedDeltaTime;
        
            Quaternion nextRotation = Quaternion.Euler(0, currentYaw, 0);
            
            Vector3 forwardStep = nextRotation * Vector3.forward * moveSpeed * Time.fixedDeltaTime;
            Vector3 nextPosition = shipRb.position + forwardStep;
            
            shipRb.MovePosition(nextPosition);
            shipRb.MoveRotation(nextRotation);
        }
    }
}