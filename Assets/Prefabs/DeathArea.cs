using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathArea : MonoBehaviour
{
    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            gameManager.instance.inDeathArea = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        gameManager.instance.inDeathArea = false;
    }
}
