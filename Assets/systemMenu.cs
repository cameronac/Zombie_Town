using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class systemMenu : MonoBehaviour
{
  public void setVolume(float volume)
    {
        AudioManager.volume = volume;
        
    }
}
