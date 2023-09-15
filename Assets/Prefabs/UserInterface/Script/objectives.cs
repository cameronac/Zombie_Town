using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;


public class Objective : MonoBehaviour
{
    [SerializeField] string currentObjective;

    //spawn wall gameObjects around the player or enemy they collided with and when the player obtains a weapon, delete/destroy those gameObjects

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            gameManager.instance.updateObjective(currentObjective);
            Destroy(gameObject);
        }
    }
}
