using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static IPickup;

public class AmmoPickup : MonoBehaviour
{
    [SerializeField] bool shotgun;
    void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.TryGetComponent(out IPickup isPickup))
        {   
            if(shotgun)
            {
                isPickup.PickupAmmo(Ammo.shotgun, 4);
                Destroy(gameObject);
            }
            else
            {
                isPickup.PickupAmmo(Ammo.pistol, 12);
                Destroy(gameObject);
            }
        }
    }
}
