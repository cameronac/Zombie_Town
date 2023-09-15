using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;


public class ObjectiveInteract : MonoBehaviour, IInteract
{
    [SerializeField] string newObjective;

    public void buttonPressed()
    {
        gameManager.instance.updateObjective(newObjective);
        Destroy(this);
    }
}
