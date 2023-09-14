using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;


public class Objective : MonoBehaviour
{
    [SerializeField] string currentObjective;

    void OnTriggerEnter(Collider other) 
    {
        if (other.gameObject.tag == "Player")
        {
            //call check completion

            gameManager.instance.updateObjective(currentObjective);
            Destroy(gameObject);
        }
    }

    void CheckCompletion()
    {
        //if objective is completed, call completed objective and current objective
        //otherwise call not completed
    }

    void CompletedObjective()
    {
        //current objective will changed to striked text
    }

    void NotCompleted()
    {
        //current objective will be displayed again as reminder to the player to complete before they can proceed
    }
}
