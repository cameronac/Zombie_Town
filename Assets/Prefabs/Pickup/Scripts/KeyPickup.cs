using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickKeyPickup : MonoBehaviour
{
    [SerializeField] int ID;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out IPickup isPickup))
        {
            isPickup.PickupKeyItem(ID);
            gameManager.instance.updateObjective("Escape!");
            Destroy(gameObject);
        }
    }

}
