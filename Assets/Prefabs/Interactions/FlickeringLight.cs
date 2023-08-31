using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlickeringLight : MonoBehaviour
{
    Light light;

    //Default Methods----------------------------
    void Start()
    {
        if (TryGetComponent<Light>(out light))
        {
            StartCoroutine(FlickerLight());
        } else
        {
            print("Couldn't Find Light Component in FlickerLight Script!");
        }

        
    }
    //-------------------------------------------


    IEnumerator FlickerLight()
    {
        //Off
        light.enabled = false;
        yield return new WaitForSeconds(Random.Range(0.05f, 0.5f));

        //On
        light.enabled = true;
        yield return new WaitForSeconds(Random.Range(0.05f, 0.5f));

        StartCoroutine(FlickerLight());
    }

}
