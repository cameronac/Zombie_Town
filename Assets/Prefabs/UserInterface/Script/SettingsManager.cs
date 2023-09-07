using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    //This script should allow the user to adjust the brightness of the game
    //This script should allow the user to adjust the volume of the game and its objects
    //This script should allow the user to adjust the sensitivity of the mouse
    [SerializeField] private Slider brightnessSlider;
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private Slider mouseSensitivitySlider;
    
    [Range(0,100)][SerializeField] Slider masterVolumeSlider;
    [Range(0,100)][SerializeField] Slider uiVolumeSlider;
    [Range(0,100)][SerializeField] Slider playerVolumeSlider;
    [Range(0,100)][SerializeField] Slider weaponVolumeSlider;
    [Range(0,100)][SerializeField] Slider enemyVolumeSlider;
    [Range(0,100)][SerializeField] Slider musicVolumeSlider;
    [Range(0,100)][SerializeField] Slider carVolumeSlider;
    [Range(0,100)][SerializeField] Slider ambientVolumeSlider;
    

    private Light lightSource;
    private SoundManager soundManager;
    private const float defaultBrightness = 1.0f;
    private const float defaultVolume = 1.0f;

    void Start()
    {
        // Get references to the light source and the SoundManager
        lightSource = GetComponent<Light>();
        soundManager = FindObjectOfType<SoundManager>();

        // Initialize the sliders with their current values
        brightnessSlider.value = PlayerPrefs.GetFloat("Brightness", defaultBrightness);
        mouseSensitivitySlider.value = PlayerPrefs.GetFloat("MouseSensitivity", 2.0f);

        float masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1.0f);
        masterVolumeSlider.value = masterVolume;
        OnMasterVolumeChanged(masterVolume);
    }


    void Update()
    {
        // Apply brightness settings to the light source
        lightSource.intensity = brightnessSlider.value;

        // Apply volume settings to the SoundManager
        soundManager.SetMasterVolume(volumeSlider.value);

        // Apply mouse sensitivity settings
        float newMouseSensitivity = mouseSensitivitySlider.value;
        PlayerPrefs.SetFloat("MouseSensitivity", newMouseSensitivity); // Save mouse sensitivity to PlayerPrefs
        PlayerPrefs.SetFloat("Brightness", brightnessSlider.value);
        PlayerPrefs.SetFloat("Volume", volumeSlider.value);
    }

    void ApplySettings()
    {
        RenderSettings.ambientIntensity = brightnessSlider.value;
        soundManager.SetMasterVolume(volumeSlider.value);


    }

    public void OnMasterVolumeChanged(float value)
    {
        soundManager.SetMasterVolume(value);
        uiVolumeSlider.value = uiVolumeSlider.value * value;
        playerVolumeSlider.value = playerVolumeSlider.value * value;
        enemyVolumeSlider.value = enemyVolumeSlider.value * value;
        musicVolumeSlider.value = musicVolumeSlider.value * value;
        carVolumeSlider.value = carVolumeSlider.value * value;
        ambientVolumeSlider.value = ambientVolumeSlider.value * value;
        PlayerPrefs.SetFloat("MasterVolume", value);
    }

    public void OnUiVolumeChanged(float value)
    {
        //Takes int the users change
        soundManager.SetUiVolume(value);
        //Saves the users change for the next time they play
        PlayerPrefs.SetFloat("UiVolume", value);
    }

    public void OnPlayerVolumeChanged(float value)
    {
        soundManager.SetPlayerVolume(value);
        PlayerPrefs.SetFloat("PlayerVolume", value);
    }

    public void OnWeaponVolumeChanged(float value)
    {
        soundManager.SetWeaponVolume(value);
        PlayerPrefs.SetFloat("WeaponVolume", value);
    }

    public void OnEnemyVolumeChanged(float value)
    {
        soundManager.SetEnemyVolume(value);
        PlayerPrefs.SetFloat("EnemyVolume", value);
    }

    public void OnMusicVolumeChanged(float value)
    {
        soundManager.SetMusicVolume(value);
        PlayerPrefs.SetFloat("MusicVolume", value);
    }

    public void OnCarVolumeChanged(float value)
    {
        soundManager.SetCarVolume(value);
        PlayerPrefs.SetFloat("CarVolume", value);
    }

    public void OnAmbientVolumeChanged(float value)
    {
        soundManager.SetAmbientVolume(value);
        PlayerPrefs.SetFloat("AmbientVolume", value);
    }

    public void OnPlayerDropdownChanged(int index)
    {

    }
}