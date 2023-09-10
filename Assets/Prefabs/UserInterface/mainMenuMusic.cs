using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mainMenuMusic : MonoBehaviour
{
    [SerializeField] AudioClip music;

    void Start()
    {
        AudioManager.instance.CreateOneDimensionalSound(music, 1, "music");
    }
}
