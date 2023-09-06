using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class SoundObject : MonoBehaviour
{
    public GameObject gameObject;
    public AudioSource audioSource;

    public void SetVolume(float volume)
    {
        audioSource.volume = volume;
    }
}
public class SoundManager : MonoBehaviour
{
    private static SoundManager instance;
    private List<SoundObject> soundObjects = new List<SoundObject>();
    private float masterVolume = 1.0f;

    // Add dictionaries to store sound categories and audio clips
    private Dictionary<string, List<AudioClip>> soundDatabase = new Dictionary<string, List<AudioClip>>();

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);

        // Initialize your sound categories and audio clips
        InitializeSoundDatabase();
    }

    void Update()
    {
        // Handle any global audio management if needed
    }

    public void AdjustAllVolumes()
    {
        foreach (var soundObject in soundObjects)
        {
            soundObject.SetVolume(soundObject.audioSource.volume * masterVolume);
        }
    }

    public float GetMasterVolume()
    {
        return masterVolume;
    }

    public void SetMasterVolume(float volume)
    {
        masterVolume = volume;
        AdjustAllVolumes();
    }

    public void AddSoundObject(GameObject gameObject)
    {
        // Check if the GameObject already has an AudioSource component.
        AudioSource audioSource = gameObject.GetComponent<AudioSource>();

        if (audioSource == null)
        {
            // If not, create a new AudioSource component and attach it to the GameObject.
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Create a new SoundObject instance and add it to the list.
        SoundObject soundObject = new SoundObject
        {
            gameObject = gameObject,
            audioSource = audioSource
        };
        soundObjects.Add(soundObject);
    }

    public void AdjustVolume(GameObject gameObject, float volume)
    {
        SoundObject soundObject = soundObjects.Find(so => so.gameObject == gameObject);
        if (soundObject != null)
        {
            soundObject.SetVolume(volume);
        }
        else
        {
            Debug.LogWarning("SoundObject not found for the specified GameObject.");
        }
    }

    // Method to initialize sound categories and audio clips
    private void InitializeSoundDatabase()
    {
        // Populate soundDatabase with sound categories and audio clips
        soundDatabase["PlayerFootsteps"] = new List<AudioClip> { /* Add player footstep clips here */ };
        soundDatabase["ZombieSounds"] = new List<AudioClip> { /* Add zombie sound clips here */ };
        soundDatabase["CarSounds"] = new List<AudioClip> { /* Add car sound clips here */ };
        // Add more sound categories as needed
    }

    // Method to get audio clips for a specific sound category
    public List<AudioClip> GetSoundCategory(string category)
    {
        if (soundDatabase.ContainsKey(category))
        {
            return soundDatabase[category];
        }
        else
        {
            Debug.LogWarning("Sound category not found in the database: " + category);
            return new List<AudioClip>();
        }
    }

    // Method to play a sound from a specific category
    public void PlaySound(string category)
    {
        List<AudioClip> soundClips = GetSoundCategory(category);
        if (soundClips.Count > 0)
        {
            AudioClip soundClip = soundClips[Random.Range(0, soundClips.Count)];
            // Create a GameObject to play the sound
            GameObject soundObject = new GameObject("SoundObject");
            AudioSource audioSource = soundObject.AddComponent<AudioSource>();
            audioSource.clip = soundClip;
            audioSource.Play();

            // Destroy the sound GameObject when the sound finishes playing
            Destroy(soundObject, audioSource.clip.length);
        }
    }

    public void SetUiVolume(float volume)
    {
        foreach (var soundObject in soundObjects)
        {
            // Check if the GameObject has a "UI" tag or a specific component indicating it's a UI sound.
            if (soundObject.gameObject.CompareTag("UI") || soundObject.gameObject.GetComponent<UISoundComponent>() != null)
            {
                soundObject.SetVolume(volume);
            }
        }
    }

    public void SetMusicVolume(float volume)
    {
        foreach (var soundObject in soundObjects)
        {
            // Check if the GameObject has a "Music" tag or a specific component indicating it's a music sound.
            if (soundObject.gameObject.CompareTag("Music") ||
                soundObject.gameObject.GetComponent<MusicSoundComponent>() != null)
            {
                soundObject.SetVolume(volume);
            }
        }
    }

    public void SetPlayerVolume(float volume)
    {
        foreach (var soundObject in soundObjects)
        {
            // Check if the GameObject has a "Player" tag or a specific component indicating it's a player sound.
            if (soundObject.gameObject.CompareTag("Player") ||
                soundObject.gameObject.GetComponent<PlayerSoundComponent>() != null)
            {
                soundObject.SetVolume(volume);
            }
        }
    }

    public void SetWeaponVolume(float volume)
    {
        foreach (var soundObject in soundObjects)
        {
            // Check if the GameObject has a "Weapon" tag or a specific component indicating it's a weapon sound.
            if (soundObject.gameObject.CompareTag("Weapon") ||
                soundObject.gameObject.GetComponent<WeaponSoundComponent>() != null)
            {
                soundObject.SetVolume(volume);
            }
        }
    }

    public void SetEnemyVolume(float volume)
    {
        foreach (var soundObject in soundObjects)
        {
            // Check if the GameObject has a "Enemy" tag or a specific component indicating it's an enemy sound.
            if (soundObject.gameObject.CompareTag("Enemy") ||
                soundObject.gameObject.GetComponent<EnemySoundComponent>() != null)
            {
                soundObject.SetVolume(volume);
            }
        }
    }

    public void SetAmbientVolume(float volume)
    {
        foreach (var soundObject in soundObjects)
        {
            // Check if the GameObject has an "Ambient" tag or a specific component indicating it's an ambient sound.
            if (soundObject.gameObject.CompareTag("Ambient") ||
                soundObject.gameObject.GetComponent<AmbientSoundComponent>() != null)
            {
                soundObject.SetVolume(volume);
            }
        }
    }

    public void SetCarVolume(float volume)
    {
        foreach (var soundObject in soundObjects)
        {
            // Check if the GameObject has a "Car" tag or a specific component indicating it's a car sound.
            if (soundObject.gameObject.CompareTag("Car") ||
                soundObject.gameObject.GetComponent<CarSoundComponent>() != null)
            {
                soundObject.SetVolume(volume);
            }
        }
    }
}
public class UISoundComponent : MonoBehaviour
{
    // This component is used to identify UI sounds.
    
}
public class MusicSoundComponent : MonoBehaviour
{
    // This component is used to identify music sounds.

}
public class PlayerSoundComponent : MonoBehaviour
{
    // This component is used to identify player sounds.
    [SerializeField] private GameObject player;
    public SoundManager soundManager;
    private AudioSource audioSource;
    public string footstepSoundCategory = "Footsteps 2";
    public string playerHurtSoundCategory = "Player Hurt 2";

    public void PlayFootstepSound()
    {
        soundManager.PlaySound(footstepSoundCategory);
    }

    public void PlayPlayerHurtSound()
    {
        soundManager.PlaySound(playerHurtSoundCategory);
    }
}
public class WeaponSoundComponent : MonoBehaviour
{
    // This component is used to identify weapon sounds.

}
public class EnemySoundComponent : MonoBehaviour
{
    // This component is used to identify enemy sounds.

}
public class AmbientSoundComponent : MonoBehaviour
{
    // This component is used to identify ambient sounds.

}
public class CarSoundComponent : MonoBehaviour
{
    // This component is used to identify car sounds.

}