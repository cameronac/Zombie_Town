using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static float volume = 1.0f;
    float previous_volume = 1.0f;
   
    private List<AudioSource> lAudioSource = new List<AudioSource>();
    private List<float> volumeMaxAudioSource = new List<float>();
    private List<string> soundTags = new List<string>();

    [SerializeField] GameObject audioPrefab;
    public static AudioManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void Update()
    {
        
        for (int i = lAudioSource.Count - 1; i > 0; i--)
        {
            if (lAudioSource[i] == null)
            {
                lAudioSource.RemoveAt(i);
                volumeMaxAudioSource.RemoveAt(i);
                soundTags.RemoveAt(i);
            } else
            {
                if (volume != previous_volume)
                {
                    lAudioSource[i].volume = volume * volumeMaxAudioSource[i];
                }
            }
        }

        previous_volume = volume;
    }

    public void CreateSoundAtPosition(AudioClip clip, Vector3 position, float new_volume = 1.0f, string sound_tag = "default")
    {
        if (clip != null) {
            GameObject prefab = Instantiate(audioPrefab, position, Quaternion.identity);
            AudioSource audioSource = prefab.GetComponent<AudioSource>();
            audioSource.volume = new_volume;
            audioSource.clip = clip;
            audioSource.Play();

            lAudioSource.Add(audioSource);
            volumeMaxAudioSource.Add(new_volume);
            soundTags.Add(sound_tag);
        }
    }

    public void CreateOneDimensionalSound(AudioClip clip, float new_volume = 1.0f, string sound_tag = "default")
    {
        if (clip != null)
        {
            GameObject prefab = Instantiate(audioPrefab);
            AudioSource audioSource = prefab.GetComponent<AudioSource>();
            audioSource.volume = new_volume;
            audioSource.clip = clip;
            audioSource.Play();

            lAudioSource.Add(audioSource);
            volumeMaxAudioSource.Add(new_volume);
            soundTags.Add(sound_tag);
        }
    }

    public void DeleteSoundWithTag(string sound_tag) {
        for (int i = lAudioSource.Count - 1; i > 0; i--)
        {
            if (soundTags[i] == sound_tag)
            {
                if (lAudioSource[i] != null)
                {
                    Destroy(lAudioSource[i].gameObject);
                    lAudioSource.RemoveAt(i);
                    volumeMaxAudioSource.RemoveAt(i);
                    soundTags.RemoveAt(i);
                }
            }
        }
    }
}
