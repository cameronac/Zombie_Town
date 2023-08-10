using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorLock : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] GameObject ObjectCheck;
    [SerializeField] Collider erase;

     void OnTriggerEnter(Collider other)
    {
        if(ObjectCheck == null)
        {
            Destroy(erase);
        }
    }

}
