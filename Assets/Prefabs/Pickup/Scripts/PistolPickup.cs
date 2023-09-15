using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PistolPickup : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            IPickup iPickup = other.gameObject.GetComponent<IPickup>();

            if (iPickup != null)
            {
                playerState.instance.destroyItems.Add(gameObject.name);
                iPickup.PickupItem(IPickup.Items.pistol);
                Destroy(gameObject);
            }
        }
    }
}
