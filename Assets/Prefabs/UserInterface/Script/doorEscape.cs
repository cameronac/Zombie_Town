using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class doorEscape : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        
        gameManager.instance.youWin();
    }
}
