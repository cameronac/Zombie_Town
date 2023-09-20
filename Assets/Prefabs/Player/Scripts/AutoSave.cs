using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoSave : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            gameManager.instance.saveGame();
        }
        GameObject[] listObjects = GameObject.FindObjectsByType<GameObject>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        foreach(GameObject thing in listObjects)
        {
            if (thing.name == "TZ")
            {
                thing.SetActive(true);
            }
        }
    }
}
