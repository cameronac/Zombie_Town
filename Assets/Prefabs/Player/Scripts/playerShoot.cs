using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerShoot : MonoBehaviour
{
    float distance = 50;
    int damage;
    float firerate = 1f;
    bool isShooting = false;
    Vector3 hit_point = Vector3.zero;

    void Update()
    {
        if (!isShooting && Input.GetButtonDown("Shoot"))
        {
            StartCoroutine(shoot());
        }
    }

    //Interfaces-----------------------
    IEnumerator shoot()
    {
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
    //---------------------------------
}
