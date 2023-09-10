using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class systemMenu : MonoBehaviour
{
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
}
