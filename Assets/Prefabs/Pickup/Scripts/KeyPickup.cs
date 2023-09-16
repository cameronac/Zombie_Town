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
            if(playerState.instance.numCarParts == 5)
            {
                txt = "Seems like we have all the car parts we need. Let's go back to the car to fix it!";
            }
        }

        else if (other.gameObject.TryGetComponent(out IPickup isPickup))
        {
            isPickup.PickupKeyItem(ID);
        }

        gameManager.instance.updateObjective(txt);
        playerState.instance.destroyItems.Add(gameObject.name);
        Destroy(gameObject);
    }
}
