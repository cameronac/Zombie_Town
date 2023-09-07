using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class audioManager : MonoBehaviour
{
    float volume = 1.0f;

    [SerializeField] GameObject audioPrefab;
    public static audioManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    public void CreateSoundAtPosition(AudioClip clip, Vector3 position)
    {
        GameObject prefab = Instantiate(audioPrefab, position, Quaternion.identity);
        AudioSource audioSource = prefab.GetComponent<AudioSource>();
        audioSource.volume = volume;
        audioSource.clip = clip;
    }

    public void CreateOneDimensionalSound(AudioClip clip)
    {
        GameObject prefab = Instantiate(audioPrefab);
        AudioSource audioSource = prefab.GetComponent<AudioSource>();
        audioSource.volume = volume;
        audioSource.clip = clip;
    }
}
