using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;


public class Objective : MonoBehaviour
{
    [SerializeField] bool objective = false;
    [SerializeField] string txt;
    [SerializeField] int colide;
    // Start is called before the first frame update
    void Start()
    {
        objective = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player" && objective == false && colide == 0)
        {
            objective = true;
            colide = 1;
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            objective = false;
        }
    }
    public void mission()
    {
        if(!objective)
        {
            gameManager.instance.updateObjective(txt);
        }
    }
    void Update()
    {
        if(Input.GetButtonDown("objective") && colide == 1)
        {
            objective = true;
        }
        else if(Input.GetButtonUp("objective") && colide ==1)
        {
            objective = false;
        }
    }

}
