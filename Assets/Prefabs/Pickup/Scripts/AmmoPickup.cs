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
            }
            else
            {
                isPickup.PickupAmmo(Ammo.pistol, 12);
            }

            playerState.instance.destroyItems.Add(gameObject.name);
            Destroy(gameObject);
        }
    }
}
