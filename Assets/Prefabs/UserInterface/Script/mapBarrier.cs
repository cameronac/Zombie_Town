using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mapBarrier : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent("playerState") as playerState != null)
        {
            if (playerState.instance.has_pistol || playerState.instance.has_shotgun)
            {
                Destroy(gameObject);
            }
            else
            {
                gameManager.instance.updateObjective("I think we'll need a better weapon before we can go inside the town");
            }
        }
    }
}