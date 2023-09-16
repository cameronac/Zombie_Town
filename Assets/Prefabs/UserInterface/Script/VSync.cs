using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VSync : MonoBehaviour
{
    public static VSync instance;
    private bool isVSync = true;
    void Start()
    {
        instance = this;
    }

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
