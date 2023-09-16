using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VSync : MonoBehaviour
{
    private static VSync instance;
    private bool isVSync = true;
    void Start()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        OnButtonClick();
    }

    public void OnButtonClick()
    {
        isVSync = !isVSync;
        QualitySettings.vSyncCount = isVSync ? 1 : 0;
    }

    public static VSync Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType<VSync>();
            }
            return instance;
        }
    }
}
