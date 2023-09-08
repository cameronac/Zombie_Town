using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class audioInstance : MonoBehaviour
{
    bool audio_played = false;
    AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (audioSource.isPlaying)
        {
            audio_played = true;
        }

        if (!audioSource.isPlaying && audio_played)
        {
            Destroy(gameObject);
        }
    }
}
