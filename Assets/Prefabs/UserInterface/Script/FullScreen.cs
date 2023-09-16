using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FullScreen : MonoBehaviour
{
    private static FullScreen instance;
    public bool isFullScreen = true;

    void Start()
    {
        instance = this;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F11))
        {
            ToggleFullscreen();
        }
    }

    public void ToggleFullscreen()
    {
        isFullScreen = !isFullScreen;
        Screen.fullScreen = isFullScreen;
    }

    public static FullScreen Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType<FullScreen>();
            }
            return instance;
        }
    }
}
