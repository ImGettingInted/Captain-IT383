using UnityEngine;

public class RocksScript : MonoBehaviour
{
    public float crashDamage = 999f;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip crashSound;

    public void OnTriggerEnter(Collider other)
    {
        PlayerHealth player = other.GetComponent<PlayerHealth>();

        if (player != null)
        {
            Debug.Log("Crashed into rocks. Game Over.");

            if (audioSource != null && crashSound != null)
            {
                audioSource.PlayOneShot(crashSound);
            }

            player.TakeDamage(crashDamage);
        }
    }
}