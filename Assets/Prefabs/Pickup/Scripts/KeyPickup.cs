using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PickKeyPickup : MonoBehaviour
{
    [SerializeField] int ID;
    [SerializeField] string txt;
    [SerializeField] bool isCarPart;

    void OnTriggerEnter(Collider other)
    {
        if(isCarPart)
        {
            playerState.instance.numCarParts++;
            gameManager.instance.updateMainObjective(ID);
        }
        else if (other.gameObject.TryGetComponent(out IPickup isPickup))
        {
            isPickup.PickupKeyItem(ID);
        }

        if (playerState.instance.numCarParts < 4) {
            gameManager.instance.updateObjective(txt);
        }

        playerState.instance.destroyItems.Add(gameObject.name);
        Destroy(gameObject);
    }
}
