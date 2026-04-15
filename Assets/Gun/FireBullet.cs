using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.InputSystem;

public class FireBullet : MonoBehaviour
{
    public GameObject bullet;
    public Transform spawnPoint;
    public float bulletSpeed = 20f;

    public int maxAmmo = 1;
    private int currentAmmo;

    public float reloadTime = 2f;
    private bool isReloading = false;

    public InputActionProperty reloadAction;

    void Start()
    {
        currentAmmo = maxAmmo;

        XRGrabInteractable grabbable = GetComponent<XRGrabInteractable>();
        grabbable.activated.AddListener(Fire);
    }

    void Update()
    {
        if (reloadAction.action.WasPressedThisFrame() && !isReloading)
        {
            StartCoroutine(Reload());
        }
    }

    public void Fire(ActivateEventArgs args)
    {
        if (isReloading) return;

        if (currentAmmo <= 0)
        {
            Debug.Log("Out of ammo! Press B to reload.");
            return;
        }

        GameObject spawnBullet = Instantiate(bullet);
        spawnBullet.transform.position = spawnPoint.position;

        Rigidbody rb = spawnBullet.GetComponent<Rigidbody>();
        rb.linearVelocity = spawnPoint.forward * bulletSpeed;

        Destroy(spawnBullet, 5f);

        currentAmmo--;
        Debug.Log("Ammo: " + currentAmmo);
    }

    IEnumerator Reload()
    {
        isReloading = true;
        Debug.Log("Reloading...");

        yield return new WaitForSeconds(reloadTime);

        currentAmmo = maxAmmo;
        isReloading = false;

        Debug.Log("Reloaded!");
    }
}
