using UnityEngine;

public class BulletDamage : MonoBehaviour
{
    public float damage = 20f;

    private void OnTriggerEnter(Collider other)
    {
        EnemyHealth enemy = other.GetComponentInParent<EnemyHealth>();

        if (enemy != null)
        {
            enemy.TakeDamage(damage);
            Debug.Log("Damage dealt: " + damage);
        }

        Destroy(gameObject);
    }
}