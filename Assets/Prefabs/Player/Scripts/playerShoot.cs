using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerShoot : MonoBehaviour
{
    int magazine = 0;
    int magazine_size = 12;
    int ammo = 24;

    float distance = 50;
    int damage;
    float firerate = 0.1f;
    float reloadTime = 3f;

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

        if (Input.GetButtonDown("Toggle Flashlight"))
        {

        }
    }

    public void AddAmmo(int amount)
    {
        ammo += amount;
        UpdateAmmoUI();
    }

    private void UpdateAmmoUI()
    {
        gameManager.instance.SetAmmo(magazine, ammo);
    }

    //IEnumerators-----------------------
    IEnumerator shoot()
    {
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
        isReloading = true;
        yield return new WaitForSeconds(reloadTime);
        isReloading = false;

        //Reload 
        if (ammo > magazine_size)
        {
            magazine = magazine_size;
            ammo -= magazine_size;
        } else {
            magazine = ammo;
            ammo = 0;
        }

        UpdateAmmoUI();
    }
    //---------------------------------
}
