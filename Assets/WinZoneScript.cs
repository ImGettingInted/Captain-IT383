using UnityEngine;

public class WinZoneScript : MonoBehaviour
{
    public void OnTriggerEnter(Collider other)
    {
        Debug.Log("You Win!");
    }
}
