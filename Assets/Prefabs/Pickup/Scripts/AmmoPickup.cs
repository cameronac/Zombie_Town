using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static IPickup;

public class AmmoPickup : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out IPickup isPickup))
        {
            isPickup.PickupAmmo(Ammo.pistol, 20);
            Destroy(gameObject);
        }
    }
}
