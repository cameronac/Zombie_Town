using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    float volume = 1.0f;

    [SerializeField] GameObject audioPrefab;
    public static AudioManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void CreateSoundAtPosition(AudioClip clip, Vector3 position, float new_volume = 1.0f)
    {
        GameObject prefab = Instantiate(audioPrefab, position, Quaternion.identity);
        AudioSource audioSource = prefab.GetComponent<AudioSource>();
        audioSource.volume = new_volume;
        audioSource.clip = clip;
        audioSource.Play();
    }

    public void CreateOneDimensionalSound(AudioClip clip, float new_volume = 1.0f)
    {
        GameObject prefab = Instantiate(audioPrefab);
        AudioSource audioSource = prefab.GetComponent<AudioSource>();
        audioSource.volume = new_volume;
        audioSource.clip = clip;
        audioSource.Play();
    }
}
