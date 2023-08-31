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

    [Header("Pistol")]
    int magazine = 0;
    [SerializeField] int magazine_size = 12;
    [SerializeField] int ammo = 24;
    [SerializeField] float recoil = 1.5f;
    [SerializeField] int pDamage = 4;
    [SerializeField] float pReloadTime = 1.5f;

    [Header("Shotgun")]
    int sMagazine = 0;
    [SerializeField] int sMagazine_size = 12;
    [SerializeField] int sAmmo = 24;
    [SerializeField] float sRecoil = 1.5f;
    [SerializeField] int sDamage = 4;
    [SerializeField] float sReloadTime = 1.5f;

    [Header("Knife")]
    [SerializeField] float knifeDistance = 1f; 
    [SerializeField] float swingRate = 0.5f;
    [SerializeField] int kDamage = 4;

    [Header("Other")]
    [SerializeField] ParticleSystem particleSystem;
    [SerializeField] Light muzzleFlash;
    float distance = 50;
    [SerializeField] float firerate = 0.1f;
    [SerializeField] float healRate = 2f;

    playerState inst;

    bool isShooting = false;
    bool isReloading = false;
    Vector3 hit_point = Vector3.zero;

    private void Start()
    {
        inst = GetComponent<playerState>();
        UpdateAmmoUI();
        muzzleFlash.enabled = false;
    }

    void Update()
    {
        if (Input.GetButtonDown("Shoot"))
        {
            switch(inst.currItem)
            {
                case playerState.heldItems.pistol:
                    if (!isShooting && magazine > 0 && !isReloading)
                    {
                        StartCoroutine(pistolShoot());
                    }
                    break;
                case playerState.heldItems.shotgun:
                    if (!isShooting && sMagazine > 0 && !isReloading)
                    {
                        StartCoroutine(shotgunShoot());
                    }
                    break;
                case playerState.heldItems.knife:
                    if(!isShooting)
                    {
                        StartCoroutine(knifeSwing());
                    }
                    break;
                case playerState.heldItems.meds:
                    if(!isShooting)
                    {
                        StartCoroutine(Heal());
                    }
                    break;
            }
        }

        if (Input.GetButtonDown("Reload"))
        {
            switch (inst.currItem)
            {
                case playerState.heldItems.pistol:
                    if (magazine < magazine_size && ammo > 0)
                    {
                        StartCoroutine(pistolReload());
                    }
                    break;
                case playerState.heldItems.shotgun:
                    if (sMagazine < sMagazine_size && sAmmo > 0)
                    {
                        StartCoroutine(shotgunReload());
                    }
                    break;
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
    IEnumerator pistolShoot()
    {
        particleSystem.Play();
        Camera.main.transform.localRotation *= Quaternion.Euler(new Vector3(-recoil, 0, 0));

        audio_source.clip = shoot_audio;
        audio_source.Play();

        StartCoroutine(eMuzzleFlash());
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
                iDamage.TakeDamage(pDamage);
            }
        }

        isShooting = true;
        yield return new WaitForSeconds(firerate);
        isShooting = false;
    }
    
    IEnumerator shotgunShoot()
    {
        yield return new WaitForSeconds(firerate);
    }

    IEnumerator knifeSwing()
    {
        //do some animation thing
        
        isShooting = true;

        Collider[] isHit = Physics.OverlapSphere(inst.KnifeHold.transform.position, knifeDistance);
        foreach (Collider c in isHit)
        {
            IDamage iDamage = c.GetComponent<IDamage>();

            if (iDamage != null && c.tag != "Player")
            {
                iDamage.TakeDamage(kDamage);
            }
        }

        yield return new WaitForSeconds(swingRate);
        isShooting = false;
        
    }

    IEnumerator Heal()
    {
        inst.medCount--;
        isShooting = true;
        yield return new WaitForSeconds(healRate);
        isShooting = false;
        inst.health = inst.health + 50 >= inst.healthMax ? inst.healthMax : inst.health + 50;
        gameManager.instance.SetHealth(inst.health/100);
    }

    IEnumerator pistolReload()
    {
        audio_source.clip = reload_audio;
        audio_source.Play();

        isReloading = true;
        yield return new WaitForSeconds(pReloadTime);
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
    
    IEnumerator shotgunReload()
    {
        isReloading = true;
        yield return new WaitForSeconds(sReloadTime);
        isReloading = false;
    }

    IEnumerator eMuzzleFlash()
    {
        muzzleFlash.enabled = true;
        yield return new WaitForSeconds(0.1f);
        muzzleFlash.enabled = false;
    }
    //---------------------------------
}
