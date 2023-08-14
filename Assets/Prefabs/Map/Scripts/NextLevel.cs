using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NewBehaviourScript : MonoBehaviour
{
    [SerializeField] GameObject nextLevel;
    [SerializeField] string buttonTag = "NextLevel";

    void Start()
    {
        
    }
    
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            nextLevel.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            nextLevel.SetActive(false);
        }
    }

    private void OnButtonDown()
    {
        if (Input.GetButtonDown(buttonTag))
        {
            Debug.Log("Loading scene: " + nextLevel.name);
            SceneManager.LoadScene(nextLevel.name);
        }
    }
}