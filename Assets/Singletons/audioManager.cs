using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class AudioManager : MonoBehaviour
{
    public static float music_volume = 1.0f;
    public static float ui_volume = 1.0f;
    public static float sfx_volume = 1.0f;

    float previous_music_volume = 1.0f;
    float previous_ui_volume = 1.0f;
    float previous_sfx_volume = 1.0f;
   
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
        for (int i = lAudioSource.Count - 1; i >= 0; i--)
        {
            if (lAudioSource[i] == null)
            {
                lAudioSource.RemoveAt(i);
                volumeMaxAudioSource.RemoveAt(i);
                soundTags.RemoveAt(i);
            } else
            {
                //SFX Volume
                if (sfx_volume != previous_sfx_volume)
                {
                    if (soundTags[i] != "ui" && soundTags[i] != "music") {
                        lAudioSource[i].volume = sfx_volume * volumeMaxAudioSource[i];
                    }
                }

                //Music Volume
                if (music_volume != previous_music_volume)
                {
                    if (soundTags[i] == "music")
                    {
                        lAudioSource[i].volume = music_volume * volumeMaxAudioSource[i];
                    }
                }

                //UI Volume
                if (ui_volume != previous_ui_volume)
                {
                    if (soundTags[i] == "ui")
                    {
                        lAudioSource[i].volume = ui_volume * volumeMaxAudioSource[i];
                    }
                }
            }
        }

        previous_sfx_volume = sfx_volume;
        previous_music_volume = music_volume;
        previous_ui_volume = ui_volume;
    }

    public void CreateSoundAtPosition(AudioClip clip, Vector3 position, float new_volume = 1.0f, string sound_tag = "default")
    {
        if (clip != null) {
            GameObject prefab = Instantiate(audioPrefab, position, Quaternion.identity);
            AudioSource audioSource = prefab.GetComponent<AudioSource>();
            audioSource.spatialBlend = 1f;

            if (sound_tag == "music")
            {
                audioSource.volume = new_volume * music_volume;
            } else if (sound_tag == "ui")
            {
                audioSource.volume = new_volume * ui_volume;
            } else
            {
                audioSource.volume = new_volume * sfx_volume;
            }

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
            audioSource.spatialBlend = 0;

            if (sound_tag == "music")
            {
                audioSource.volume = new_volume * music_volume;
            }
            else if (sound_tag == "ui")
            {
                audioSource.volume = new_volume * ui_volume;
            }
            else
            {
                audioSource.volume = new_volume * sfx_volume;
            }

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
