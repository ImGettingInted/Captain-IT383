using UnityEngine;

public class BulletDamage : MonoBehaviour
{
    public float damage = 20f;

    [Header("Sword Audio (optional)")]
    public bool isSword = false;

    public AudioSource audioSource;
    public AudioClip swordDrawSound;
    public AudioClip swordMissSound;
    public AudioClip swordHitSound;

    private bool hasHit = false;

    void Start()
    {
        // Play draw sound ONLY if this is a sword
        if (isSword && audioSource != null && swordDrawSound != null)
        {
            audioSource.PlayOneShot(swordDrawSound);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        EnemyHealth enemy = other.GetComponentInParent<EnemyHealth>();

        if (enemy != null)
        {
            enemy.TakeDamage(damage);
            Debug.Log("Damage dealt: " + damage);

            hasHit = true;

            if (isSword && audioSource != null && swordHitSound != null)
            {
                audioSource.PlayOneShot(swordHitSound);
            }
        }

        // If it's a sword and it DIDN’T hit an enemy → miss sound
        if (isSword && !hasHit && audioSource != null && swordMissSound != null)
        {
            audioSource.PlayOneShot(swordMissSound);
        }

        Destroy(gameObject);
    }
}