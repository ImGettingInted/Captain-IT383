using UnityEngine;
using System.Collections.Generic;

public class Sword : MonoBehaviour
{
    public float damage = 25f;

    // prevents hitting the same enemy multiple times in one swing
    private HashSet<EnemyHealth> hitEnemies = new HashSet<EnemyHealth>();

    private void OnEnable()
    {
        hitEnemies.Clear(); // reset when sword is enabled (or grabbed)
    }

    private void OnTriggerEnter(Collider other)
    {
        EnemyHealth enemy = other.GetComponentInParent<EnemyHealth>();

        if (enemy != null && !hitEnemies.Contains(enemy))
        {
            enemy.TakeDamage(damage);
            hitEnemies.Add(enemy);

            Debug.Log("Sword hit enemy for " + damage);
        }
    }
}
