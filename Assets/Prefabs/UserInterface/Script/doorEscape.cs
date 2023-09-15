using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class doorEscape : MonoBehaviour, IInteract
{
    [SerializeField] int ID;
    [SerializeField] bool locked;

    public void buttonPressed()
    {
        if(locked){
            if(playerState.instance.has_key(ID))
            {
                gameManager.instance.youWin();    
            }
            else
            {
                //do something to tell the player they need a key. not sure how to do that just yet.
            }
        }
        else
            gameManager.instance.youWin();
    }
}
