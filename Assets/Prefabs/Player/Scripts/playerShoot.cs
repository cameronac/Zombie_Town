using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class playerShoot : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] AudioClip pistol_audio;
    [SerializeField] AudioClip shotgun_audio;
    [SerializeField] AudioClip pistol_reload_audio;
    [SerializeField] AudioClip shotgun_reload_audio;
    [SerializeField] AudioClip pistol_dry_fire_audio;
    [SerializeField] AudioClip shotgun_dry_fire_audio;

    [SerializeField] AudioClip knife_hit_audio;
    [SerializeField] AudioClip knife_air_audio;

    [Header("Pistol")]
    [SerializeField] public int magazine_size = 12;
    [SerializeField] public int ammo = 24;
    [SerializeField] float recoil = 1.5f;
    [SerializeField] int pDamage = 4;
    [SerializeField] float pReloadTime = 1.5f;
    [SerializeField] float pFirerate = 0.1f;
    [SerializeField] Animator PistolHold;
    public int magazine = 0;

    [Header("Shotgun")]
    [SerializeField] public int sMagazine_size = 4;
    [SerializeField] public int sAmmo = 16;
    [SerializeField] float sRecoil = 5f;
    [SerializeField] int sDamage = 4;
    [SerializeField] float sReloadTime = 1.5f;
    [SerializeField] float sFirerate = 1.5f;
    [SerializeField] Animator ShotgunHold;
    public int sMagazine = 0;

    [Header("Knife")]
    [SerializeField] float knifeDistance = 1f; 
    [SerializeField] float swingRate = 1.5f;
    [SerializeField] int kDamage = 4;
    [SerializeField] Animator knifeAnim;

    [Header("Other")]
    [SerializeField] cameraControl cCameraControl;
    [SerializeField] new ParticleSystem particleSystem;
    [SerializeField] Light muzzleFlash;
    float distance = 50;
    float sDistance = 30;
    [SerializeField] float healRate = 2f;

    [SerializeField] GameObject sgSpread;
    int numBullets = 5;
    float bulletSpread = 0.1f;
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
                    } else if (magazine <= 0)
                    {
                        AudioManager.instance.CreateSoundAtPosition(pistol_dry_fire_audio, transform.position);
                    }
                    break;
                case playerState.heldItems.shotgun:
                    if (!isShooting && sMagazine > 0 && !isReloading)
                    {
                        StartCoroutine(shotgunShoot());
                    } else
                    {
                        AudioManager.instance.CreateSoundAtPosition(shotgun_dry_fire_audio, transform.position);
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

        if(Input.GetButtonDown("Aim"))
        {
            switch (inst.currItem)
            {
                case playerState.heldItems.pistol:
                    PistolHold.SetBool("ADS", true);
                    break;
                case playerState.heldItems.shotgun:
                    ShotgunHold.SetBool("ADS", true);
                    bulletSpread = 0.05f;
                    break;
            }
        }

        if (Input.GetButtonUp("Aim"))
        {
            switch (inst.currItem)
            {
                case playerState.heldItems.pistol:
                    PistolHold.SetBool("ADS", false);
                    break;
                case playerState.heldItems.shotgun:
                    ShotgunHold.SetBool("ADS", false);
                    bulletSpread = 0.1f;
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
        if (gameManager.instance != null)
        {
            switch (inst.currItem)
            {
                case playerState.heldItems.pistol:
                    gameManager.instance.SetAmmo(magazine, ammo);
                    break;
                case playerState.heldItems.shotgun:
                    gameManager.instance.SetAmmo(sMagazine, sAmmo);
                    break;
                case playerState.heldItems.meds:
                    gameManager.instance.SetAmmo(0, inst.medCount);
                    break;
            }
        }
    }

    public void KnifeAttack()
    {
        Collider[] isHit = Physics.OverlapSphere(inst.KnifeHold.transform.position, knifeDistance);

        foreach (Collider c in isHit)
        {
            IDamage iDamage = c.GetComponent<IDamage>();

            if (iDamage != null && c.tag != "Player")
            {
                iDamage.TakeDamage(kDamage);
            }

            if (c.tag != "Player")
            {
                AudioManager.instance.CreateSoundAtPosition(knife_hit_audio, transform.position);
            }
        }
    }

    //IEnumerators-----------------------
    IEnumerator pistolShoot()
    {
        particleSystem.Play();
        cCameraControl.ApplyRecoil(new Vector3(-recoil, 0, 0));
        AudioManager.instance.CreateSoundAtPosition(pistol_audio, transform.position);

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

            if (hit.collider.tag == "lock")
            {
                BreakableLock dO = hit.collider.GetComponent<BreakableLock>();

                if (dO != null)
                {
                    dO.ShotLock();
                }
            }
        }

        isShooting = true;
        yield return new WaitForSeconds(pFirerate);
        isShooting = false;
    }
    
    IEnumerator shotgunShoot()
    {
        isShooting = true;
        particleSystem.Play();

        cCameraControl.ApplyRecoil(new Vector3(-sRecoil, 0, 0));
        AudioManager.instance.CreateSoundAtPosition(shotgun_audio, transform.position);

        StartCoroutine(eMuzzleFlash());
        sMagazine -= 1;
        UpdateAmmoUI();

        bool isHit;

        for (int i = 0; i < Mathf.Max(1, numBullets); i++)
        {
            Vector3 shootDirection = Camera.main.transform.forward;
            
            shootDirection.x += Random.Range(-bulletSpread, bulletSpread);
            shootDirection.y += Random.Range(-bulletSpread, bulletSpread);
            shootDirection.z += Random.Range(-bulletSpread, bulletSpread);

            isHit = Physics.Raycast(Camera.main.transform.position, shootDirection, out RaycastHit hit, sDistance);

            if (isHit)
            {
                hit_point = hit.point;

                IDamage iDamage = hit.collider.GetComponent<IDamage>();

                if (iDamage != null)
                {
                    iDamage.TakeDamage(sDamage);
                }

                if (hit.collider.tag == "lock")
                {
                    BreakableLock dO = hit.collider.GetComponent<BreakableLock>();

                    if (dO != null)
                    {
                        dO.ShotLock();
                    }
                }
            }
        }

        yield return new WaitForSeconds(sFirerate);
        isShooting = false;
    }

    IEnumerator knifeSwing()
    {
        //do some animation thing
        knifeAnim.SetTrigger("Attacking");

        isShooting = true;
        AudioManager.instance.CreateSoundAtPosition(knife_air_audio, transform.position);

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
        UpdateAmmoUI();
        if (inst.medCount <= 0)
        {
            inst.MedsHold.SetActive(false);
        }
    }

    IEnumerator pistolReload()
    {
        AudioManager.instance.CreateSoundAtPosition(pistol_reload_audio, transform.position);

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

        AudioManager.instance.CreateSoundAtPosition(shotgun_reload_audio, transform.position);

        isReloading = true;
        yield return new WaitForSeconds(sReloadTime);
        isReloading = false;

        //Reload 
        int needed_ammo = sMagazine_size - sMagazine;

        if (sAmmo > needed_ammo)
        {
            sMagazine = sMagazine_size;
            sAmmo -= needed_ammo;
        }
        else
        {
            sMagazine += sAmmo;
            sAmmo = 0;
        }

        UpdateAmmoUI();
    }

    IEnumerator eMuzzleFlash()
    {
        muzzleFlash.enabled = true;
        yield return new WaitForSeconds(0.1f);
        muzzleFlash.enabled = false;
    }
    //---------------------------------
}
