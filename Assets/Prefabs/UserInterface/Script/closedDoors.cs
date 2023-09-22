using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class closedDoors : MonoBehaviour, IInteract
{
    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.gameObject.GetComponent("playerState") as playerState != null)
    //    {
    //        gameManager.instance.updateObjective("Something must be blocking it from the inside");
    //    }
    //}

    void IInteract.buttonPressed()
    {
        gameManager.instance.updateObjective("Something must be blocking it from the inside");
    }
}
