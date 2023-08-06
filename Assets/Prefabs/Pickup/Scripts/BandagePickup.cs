using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static IPickup;

public class BandagePickup : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out IPickup isPickup))
        {
            isPickup.PickupFirstAid(FirstAid.bandage, 1);
            Destroy(gameObject);
        }
    }
}
