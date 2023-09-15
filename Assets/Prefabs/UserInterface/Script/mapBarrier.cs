using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mapBarrier : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.GetComponent("playerState") as playerState != null)
        {
            if(playerState.instance.has_pistol)
            {
                Destroy(gameObject);
            }
            else
            {
                gameManager.instance.updateObjective("pistol required to proceed");
            }
        }
    }
}
