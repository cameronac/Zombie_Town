using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class carParts : MonoBehaviour, IInteract
{
    [SerializeField] int numRequired;
    [SerializeField] string locked;

    public void pressed()
    {

        if (playerState.instance.numCarParts == numRequired)
            gameManager.instance.youWin();
        else
            gameManager.instance.updateObjective(locked);
    }
        
}
