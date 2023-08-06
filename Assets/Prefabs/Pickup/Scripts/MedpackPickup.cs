using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static IPickup;

public class MedpackPickup : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out IPickup isPickup))
        {
            isPickup.PickupFirstAid(FirstAid.first_aid_kit, 1);
            Destroy(gameObject);
        }
    }
}
