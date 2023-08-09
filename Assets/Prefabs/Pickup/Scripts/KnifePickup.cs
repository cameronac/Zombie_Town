using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static IPickup;

public class KnifePickup : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out IPickup isPickup))
        {
            isPickup.PickupItem(Items.knife);
            Destroy(gameObject);
        }
    }
}