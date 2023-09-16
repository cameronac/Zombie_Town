using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FullScreen : MonoBehaviour
{
    public Toggle fullScreenToggle;
    public KeyCode fullScreenKey = KeyCode.F11;

    private void Start()
    {
        // Initialize the fullscreen toggle state
        if (fullScreenToggle != null)
        {
            fullScreenToggle.isOn = Screen.fullScreen;
            fullScreenToggle.onValueChanged.AddListener(OnToggleValueChanged);
        }
        else
        {
            Debug.LogWarning("Fullscreen Toggle is not assigned in the Inspector.");
        }
    }

    private void Update()
    {
        // Toggle fullscreen mode when the key is pressed
        if (Input.GetKeyDown(fullScreenKey))
        {
            ToggleFullscreen();
        }
    }

    public void ToggleFullscreen()
    {
        // Toggle fullscreen mode
        Screen.fullScreen = !Screen.fullScreen;

        // Update the toggle UI if available
        if (fullScreenToggle != null)
        {
            fullScreenToggle.isOn = Screen.fullScreen;
        }
    }

    private void OnToggleValueChanged(bool isOn)
    {
        // Toggle fullscreen mode when the toggle UI changes
        ToggleFullscreen();
    }
}