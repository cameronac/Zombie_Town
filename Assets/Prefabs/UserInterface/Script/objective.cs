using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;


public class Objective : MonoBehaviour
{
    [SerializeField] string currentObjective;
    //[SerializeField] string newObjective;

    //private void Start()
    //{
    //    gameManager.instance.updateObjective(currentObjective);
    //    Destroy(gameObject);
    //}

    void OnTriggerEnter(Collider other) 
    {
        if (other.gameObject.tag == "Player")
        {
            gameManager.instance.updateObjective(newObjective);
            Destroy(gameObject);
        }
    }
}
