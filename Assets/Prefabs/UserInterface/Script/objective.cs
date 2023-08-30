using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;


public class Objective : MonoBehaviour
{
    [SerializeField] string newObjective;

    void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag == "Player")
        {
            gameManager.instance.updateObjective(newObjective);
            Destroy(gameObject);
        }
    }
}
