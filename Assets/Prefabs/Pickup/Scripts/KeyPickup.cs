using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickKeyPickup : MonoBehaviour
{
    [SerializeField] int ID;
    [SerializeField] string txt;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out IPickup isPickup))
        {
            isPickup.PickupKeyItem(ID);
            gameManager.instance.updateObjective(txt);
            Destroy(gameObject);
        }
    }

}
