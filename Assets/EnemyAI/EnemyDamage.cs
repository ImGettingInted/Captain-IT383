using UnityEngine;

public class EnemyDamage : MonoBehaviour
{
    public int damage = 10;
    public float hitCooldown = 1f;

    private float nextHitTime = 0f;

    private void OnTriggerEnter(Collider other)
    {
        TryDamage(other);
    }

    private void OnTriggerStay(Collider other)
    {
        TryDamage(other);
    }

    void TryDamage(Collider other)
    {
        if (Time.time < nextHitTime) return;

        PlayerHealth player = other.GetComponentInParent<PlayerHealth>();

        if (player != null)
        {
            player.TakeDamage(damage);
            nextHitTime = Time.time + hitCooldown;

            Debug.Log("Enemy hit player for " + damage);
        }
    }
}