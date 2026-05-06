using UnityEngine;

public class MoveShip : MonoBehaviour
{
    public Rigidbody shipRb;
    public WheelControlV2 wheelControl;
    public bool moveShip;
    public float moveSpeed;
    public float turnSens;
    private float currentYaw;

    [Header("Sway Settings")]
    public float swayAmount = 2f;   // how much the ship tilts
    public float swaySpeed = 1f;    // how fast it rocks

    void Start()
    {
        currentYaw = shipRb.rotation.eulerAngles.y;
    }
    
    void FixedUpdate()
    {
        if (moveShip)
        {
            float wheelAngle = -transform.localEulerAngles.z;
            float normalizedAngle = Mathf.DeltaAngle(0, wheelAngle);
            
            float clampAngle = Mathf.Clamp(normalizedAngle, -160f, 160f);
            transform.localRotation = Quaternion.Euler(0, 0, -clampAngle);
            currentYaw += clampAngle * turnSens * Time.fixedDeltaTime;

            // 🌊 Simple sway
            float swayX = Mathf.Sin(Time.time * swaySpeed) * swayAmount;
            float swayZ = Mathf.Sin(Time.time * swaySpeed * 1.3f) * swayAmount;

            Quaternion nextRotation = Quaternion.Euler(swayX, currentYaw, swayZ);
            
            Vector3 movement = nextRotation * Vector3.forward * moveSpeed * Time.fixedDeltaTime;

            shipRb.MovePosition(shipRb.position + movement);
            shipRb.MoveRotation(nextRotation);
        }
    }
}