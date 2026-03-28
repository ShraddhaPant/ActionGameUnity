using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GunShoot : MonoBehaviour
{
    public float range = 100f;
    public Camera fpsCam;

    public GameObject hitEffect;
    public GameObject muzzleFlash;

    private AudioSource audioSource;

    public float fireRate = 0.5f;
    private float nextTimeToFire = 0f;

    public int maxAmmo = 50;
    private int currentAmmo;
    public TextMeshProUGUI ammoText;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        currentAmmo = maxAmmo;
        ammoText.text = "Ammo: " + currentAmmo;
    }

    void Update()
    {
        if (Input.GetButton("Fire1") && Time.time >= nextTimeToFire && currentAmmo > 0)
        {
            nextTimeToFire = Time.time + fireRate;
            Shoot();

            currentAmmo--;
            ammoText.text = "Ammo: " + currentAmmo;
        }
    }

    void Shoot()
    {
        RaycastHit hit;
        StartCoroutine(ShowMuzzleFlash());

        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range))
        {
            Debug.Log("Hit: " + hit.transform.name);

            // 🔥 IMPORTANT FIX (works even if collider is child)
            ZombieHealth target = hit.transform.GetComponentInParent<ZombieHealth>();

            if (target != null)
            {
                Debug.Log("🎯 Hit Zombie!");
                target.TakeDamage(1);
            }
            else
            {
                Debug.Log("Hit object without ZombieHealth");
            }
            GameObject impact = Instantiate(hitEffect, hit.point, Quaternion.LookRotation(hit.normal));
            Destroy(impact, 4f);

            audioSource.Play();
        }
    }

    IEnumerator ShowMuzzleFlash()
    {
        muzzleFlash.SetActive(true);
        yield return new WaitForSeconds(0.05f);
        muzzleFlash.SetActive(false);
    }
}