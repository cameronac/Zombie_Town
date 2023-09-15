
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static IPickup;

public class shotgunPickup : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out IPickup isPickup))
        {
            playerState.instance.destroyItems.Add(gameObject.name);
            isPickup.PickupItem(Items.shotgun);
            Destroy(gameObject);
        }
    }
}
