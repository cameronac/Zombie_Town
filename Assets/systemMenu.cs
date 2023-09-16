using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class systemMenu : MonoBehaviour
{
    [SerializeField] public Toggle isFullscreen;
    [SerializeField] public Toggle isVsync;
    public void Start()
    {
        isFullscreen.isOn = Screen.fullScreen;

        if (QualitySettings.vSyncCount == 0)
            isVsync.isOn = false;
        if (QualitySettings.vSyncCount == 1)
            isVsync.isOn = true;
    }
    public void setMusicVolume(float volume)
    {
        AudioManager.music_volume = volume;
    }

    public void setUIVolume(float volume)
    {
        AudioManager.ui_volume = volume;
    }

    public void setSfxVolume(float volume)
    {
        AudioManager.sfx_volume = volume;
    }

    public void setFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    public void setVsync(bool isVsync)
    {
        if(isVsync)
        {
            QualitySettings.vSyncCount = 1;
        }
        else
        {
            QualitySettings.vSyncCount = 0;
        }
    }
}
