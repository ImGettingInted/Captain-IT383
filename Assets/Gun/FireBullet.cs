using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class FireBullet : MonoBehaviour
{
    public GameObject bullet;
    public Transform spawnPoint;
    public float bulletSpeed = 20f;

    public float fireRate = 1f;
    private float nextFireTime = 0f;

    [Header("SFX")]
    public AudioSource audioSource;
    public AudioClip shootSound;

    void Start()
    {
        XRGrabInteractable grabbable = GetComponent<XRGrabInteractable>();
        grabbable.activated.AddListener(Fire);
    }

    public void Fire(ActivateEventArgs args)
    {
        if (Time.time < nextFireTime) return;

        nextFireTime = Time.time + fireRate;

        // 🔊 play sound
        if (audioSource != null && shootSound != null)
        {
            audioSource.PlayOneShot(shootSound);
        }

        GameObject spawnBullet = Instantiate(bullet, spawnPoint.position, spawnPoint.rotation);

        Rigidbody rb = spawnBullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = spawnPoint.forward * bulletSpeed;
        }

        Destroy(spawnBullet, 5f);
    }
}