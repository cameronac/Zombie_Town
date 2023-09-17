using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class playerShoot : MonoBehaviour
{
    private bool isGunShot = false;

    [Header("Audio")]
    [SerializeField] AudioClip pistol_audio;
    [SerializeField] AudioClip shotgun_audio;
    [SerializeField] AudioClip pistol_reload_audio;
    [SerializeField] AudioClip shotgun_reload_audio;
    [SerializeField] AudioClip pistol_dry_fire_audio;
    [SerializeField] AudioClip shotgun_dry_fire_audio;

    [SerializeField] AudioClip knife_hit_audio;
    [SerializeField] AudioClip knife_air_audio;
    [SerializeField] AudioClip med_audio;

    [Header("Pistol")]
    [SerializeField] public int magazine_size = 12;
    [SerializeField] float recoil = 1.5f;
    [SerializeField] int pDamage = 4;
    [SerializeField] float pReloadTime = 1.5f;
    [SerializeField] float pFirerate = 0.1f;
    [SerializeField] Animator PistolHold;

    [Header("Shotgun")]
    [SerializeField] public int sMagazine_size = 4;
    [SerializeField] float sRecoil = 5f;
    [SerializeField] int sDamage = 4;
    [SerializeField] float sReloadTime = 1.5f;
    [SerializeField] float sFirerate = 1.5f;
    [SerializeField] Animator ShotgunHold;
    [SerializeField] float maxDamageMultiplier = 2.0f;
    [SerializeField] float minDistance = 5.0f;

    [Header("Knife")]
    [SerializeField] float knifeDistance = 1f; 
    [SerializeField] float swingRate = 1.5f;
    [SerializeField] int kDamage = 4;
    [SerializeField] Animator knifeAnim;

    [Header("Meds")]
    [SerializeField] Animator MedsHold;

    [Header("Other")]
    [SerializeField] cameraControl cCameraControl;
    [SerializeField] new ParticleSystem particleSystem;
    [SerializeField] Light muzzleFlash;
    float distance = 50;
    float sDistance = 30;
    [SerializeField] float healRate;

    [SerializeField] GameObject sgSpread;
    int numBullets = 5;
    float bulletSpread = 0.1f;
    playerState inst;

    public bool isShooting = false;
    public bool isReloading = false;
    public bool isAiming = false;
    Vector3 hit_point = Vector3.zero;

    itemSway pistolInst;
    itemSway shotgunInst;
    private void Start()
    {
        pistolInst = PistolHold.GetComponent<itemSway>();
        shotgunInst = ShotgunHold.GetComponent<itemSway>();
        inst = GetComponent<playerState>();
        UpdateAmmoUI();
        muzzleFlash.enabled = false;
    }

    void Update()
    {
        isGunShot = false;

        if (Time.timeScale > 0) {
            if (Input.GetButtonDown("Shoot") && gameManager.instance.activeMenu == null)
            {
                switch(inst.currItem)
                {
                    case playerState.heldItems.pistol:
                        if (!isShooting && inst.pMagazine > 0 && !isReloading)
                        {
                            StartCoroutine(pistolShoot());
                        } else if (inst.pMagazine <= 0)
                        {
                            AudioManager.instance.CreateSoundAtPosition(pistol_dry_fire_audio, transform.position);
                        }
                        break;
                    case playerState.heldItems.shotgun:
                        if (!isShooting && inst.sMagazine > 0 && !isReloading)
                        {
                            StartCoroutine(shotgunShoot());
                        } else if (inst.sMagazine <= 0)
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
        }

        if(Input.GetButtonDown("Aim"))
        {
            switch (inst.currItem)
            {
                case playerState.heldItems.pistol:
                    isAiming = true;
                    PistolHold.SetBool("ADS", true);
                    pistolInst.intesity = 1;
                    break;
                case playerState.heldItems.shotgun:
                    isAiming = true;
                    ShotgunHold.SetBool("ADS", true);
                    bulletSpread = 0.05f;
                    shotgunInst.intesity = 1;
                    break;
            }
        }

        if (Input.GetButtonUp("Aim"))
        {
            switch (inst.currItem)
            {
                case playerState.heldItems.pistol:
                    isAiming = false;
                    PistolHold.SetBool("ADS", false);
                    pistolInst.intesity = 2.5f;
                    break;
                case playerState.heldItems.shotgun:
                    isAiming = false;
                    ShotgunHold.SetBool("ADS", false);
                    bulletSpread = 0.1f;
                    shotgunInst.intesity = 2.5f;
                    break;
            }
        }

        if (Input.GetButtonDown("Reload"))
        {
            switch (inst.currItem)
            {
                case playerState.heldItems.pistol:
                    if (inst.pMagazine < magazine_size && inst.pAmmo > 0)
                    {
                        StartCoroutine(pistolReload());
                    }
                    break;
                case playerState.heldItems.shotgun:
                    if (inst.sMagazine < sMagazine_size && inst.sAmmo > 0)
                    {
                        StartCoroutine(shotgunReload());
                    }
                    break;
            }
        }
    }

    public void AddAmmo(int amount)
    {
        inst.pAmmo += amount;
        UpdateAmmoUI();
    }
    public void AddShotgunAmmo(int amount)
    {
        inst.sAmmo += amount;
        UpdateAmmoUI();
    }

    private void UpdateAmmoUI()
    {
        if (gameManager.instance != null)
        {
            switch (inst.currItem)
            {
                case playerState.heldItems.pistol:
                    gameManager.instance.SetAmmo(inst.pMagazine, inst.pAmmo);
                    break;
                case playerState.heldItems.shotgun:
                    gameManager.instance.SetAmmo(inst.sMagazine, inst.sAmmo);
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

        isGunShot = true;
        StartCoroutine(eMuzzleFlash());
        inst.pMagazine -= 1;
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

        isGunShot = true;
        StartCoroutine(eMuzzleFlash());
        inst.sMagazine -= 1;
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
                    // Calculate damage based on distance
                    float distanceToEnemy = Vector3.Distance(transform.position, hit.collider.transform.position);
                    float damageMultiplier = Mathf.Lerp(1.0f, maxDamageMultiplier, Mathf.Clamp01((distanceToEnemy - minDistance) / (sDistance - minDistance)));

                    // Apply damage to enemyAI
                    iDamage.TakeDamage(sDamage * damageMultiplier);
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
        isShooting = true;
        knifeAnim.SetTrigger("Attacking");

        AudioManager.instance.CreateSoundAtPosition(knife_air_audio, transform.position);

        yield return new WaitForSeconds(swingRate);
        isShooting = false;
    }

    IEnumerator Heal()
    {

        MedsHold.SetBool("Use", true);

        isShooting = true;
        AudioManager.instance.CreateSoundAtPosition(med_audio, transform.position, .25f);
        yield return new WaitForSeconds(healRate);
        isShooting = false;
        MedsHold.SetBool("Use", false);
        inst.medCount--;
        inst.health = inst.health + 50 >= inst.healthMax ? inst.healthMax : inst.health + 50;
        gameManager.instance.SetHealth(inst.health/100);
        UpdateAmmoUI();
        if (inst.medCount <= 0)
        {
            inst.MedsHold.SetActive(false);
            inst.ToggleItem(true);
        }
    }

    IEnumerator pistolReload()
    {
        AudioManager.instance.CreateSoundAtPosition(pistol_reload_audio, transform.position);

        isReloading = true;
        yield return new WaitForSeconds(pReloadTime);
        isReloading = false;

        //Reload 
        int needed_ammo = magazine_size - inst.pMagazine;

        if (inst.pAmmo > needed_ammo)
        {
            inst.pMagazine = magazine_size;
            inst.pAmmo -= needed_ammo;
        } else {
            inst.pMagazine += inst.pAmmo;
            inst.pAmmo = 0;
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
        int needed_ammo = sMagazine_size - inst.sMagazine;

        if (inst.sAmmo > needed_ammo)
        {
            inst.sMagazine = sMagazine_size;
            inst.sAmmo -= needed_ammo;
        }
        else
        {
            inst.sMagazine += inst.sAmmo;
            inst.sAmmo = 0;
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

    //Getter-------------------------------------
    public bool IsGunShot()
    {
        return isGunShot;
    }
    //-------------------------------------------

}
