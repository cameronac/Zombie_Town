using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEditor.SceneManagement;
using UnityEngine;


public class ObjectiveInteract : MonoBehaviour, IInteract
{
    [SerializeField] string newObjective;

    public void pressed()
    {
        gameManager.instance.updateObjective(newObjective);
        Destroy(this);
    }
}
