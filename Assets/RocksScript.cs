using UnityEngine;

public class RocksScript : MonoBehaviour
{
    public float crashDamage = 999f;

    public void OnTriggerEnter(Collider other)
    {
        PlayerHealth player = other.GetComponent<PlayerHealth>();

        if (player != null)
        {
            Debug.Log("Crashed into rocks. Game Over.");
            player.TakeDamage(crashDamage);
        }
    }
}