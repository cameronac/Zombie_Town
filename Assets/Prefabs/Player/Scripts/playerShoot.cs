using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class playerShoot : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] AudioSource audio_source;
    [SerializeField] AudioClip shoot_audio;
    [SerializeField] AudioClip reload_audio;

    [Header("Other")]
    [SerializeField] ParticleSystem particleSystem;
    int magazine = 0;
    int magazine_size = 12;
    int ammo = 24;
    float recoil = 1.5f;

    float distance = 50;
    [SerializeField] int damage = 4;
    [SerializeField] float firerate = 0.1f;
    float reloadTime = 1.5f;

    bool isShooting = false;
    bool isReloading = false;
    Vector3 hit_point = Vector3.zero;

    private void Start()
    {
        UpdateAmmoUI();
    }

    void Update()
    {
        if (Input.GetButtonDown("Shoot"))
        {
            if (!isShooting && magazine > 0 && !isReloading) {
                StartCoroutine(shoot());
            }
        }

        if (Input.GetButtonDown("Reload"))
        {
            if (magazine < magazine_size && ammo > 0)
            {
                StartCoroutine(reload());
            }
        }
    }

    public void AddAmmo(int amount)
    {
        ammo += amount;
        UpdateAmmoUI();
    }

    private void UpdateAmmoUI()
    {
        if (gameManager.instance != null) {
            gameManager.instance.SetAmmo(magazine, ammo);
        }
    }

    //IEnumerators-----------------------
    IEnumerator shoot()
    {
        particleSystem.Play();
        Camera.main.transform.localRotation *= Quaternion.Euler(new Vector3(-recoil, 0, 0));

        audio_source.clip = shoot_audio;
        audio_source.Play();

        magazine -= 1;
        UpdateAmmoUI();

        RaycastHit hit;
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);

        bool isHit = Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, distance);
        
        if (isHit)
        {
            hit_point = hit.point;
            
            IDamage iDamage = hit.collider.GetComponent<IDamage>();

            if (iDamage != null)
            {
                iDamage.TakeDamage(damage);
            }
        }

        isShooting = true;
        yield return new WaitForSeconds(firerate);
        isShooting = false;
    }
   
    IEnumerator reload()
    {
        audio_source.clip = reload_audio;
        audio_source.Play();

        isReloading = true;
        yield return new WaitForSeconds(reloadTime);
        isReloading = false;

        //Reload 
        int needed_ammo = magazine_size - magazine;

        if (ammo > needed_ammo)
        {
            magazine = magazine_size;
            ammo -= needed_ammo;
        } else {
            magazine += ammo;
            ammo = 0;
        }

        UpdateAmmoUI();
    }
    //---------------------------------
}
