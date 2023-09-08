using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class carParts : MonoBehaviour, IInteract
{
    [SerializeField] int[] ID;
    [SerializeField] bool locked;

    public void pressed()
    {


        bool win = true;

        for (int i = 0; i < ID.Length; i++)
        {
            if (!playerState.instance.has_key(ID[i]))
            {
                win = false;

            }
            else
            {
                //do something to tell the player they need a key. not sure how to do that just yet.
            }
        }
        if (win)
        {
            gameManager.instance.youWin();
        }
    }
        
}
