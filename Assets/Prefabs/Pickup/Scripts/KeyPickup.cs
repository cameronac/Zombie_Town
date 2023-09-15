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
            if(playerState.instance.numCarParts == 4)
            {
                txt = "Get back to your car and fix it";
            }
        }

        else if (other.gameObject.TryGetComponent(out IPickup isPickup))
        {
            isPickup.PickupKeyItem(ID);
        }

        gameManager.instance.updateObjective(txt);
        Destroy(gameObject);
    }
}
